using System.Data;
using Ardalis.Result;

public class CarreraService : Service
{
    private readonly ICarreraRepository _carreraRepository;

    public CarreraService(ICarreraRepository carreraRepository)
    {
        _carreraRepository = carreraRepository;
    }

    public virtual Result<CarreraDto?> Crear(CrearCarreraComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<CarreraDto?>.Invalid(validResult.ValidationErrors.ToArray());

        var nombreTrimmed = comando.Nombre!.Trim();

        if (string.IsNullOrWhiteSpace(nombreTrimmed))
            return Result<CarreraDto?>.Invalid(new ValidationError("Nombre", "El nombre es requerido"));

        if (_carreraRepository.ReactivarEliminadaPorNombre(nombreTrimmed))
            return Result<CarreraDto?>.Success(null);

        if (_carreraRepository.ExisteActivaPorNombre(nombreTrimmed))
            return Result<CarreraDto?>.Conflict("Ya existe una carrera activa con este nombre");

        var comandoFinal = new CrearCarreraComando(nombreTrimmed);
        var result = _carreraRepository.Crear(comandoFinal);
        return result;
    }

    public virtual Result<List<CarreraDto?>> ObtenerTodos()
    {
        var repoResult = _carreraRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<CarreraDto?>>.Error("Error al obtener las carreras");

        var resultado = repoResult.Value;
        var lista = new List<CarreraDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var baseDto = MapearFilaADto(fila);
            if (baseDto is CarreraDto carrera)
                lista.Add(carrera);
        }
        return lista.Count == 0
            ? Result<List<CarreraDto?>>.NotFound("No se encontraron carreras")
            : Result<List<CarreraDto?>>.Success(lista);
    }

    public virtual Result<CarreraDto?> Actualizar(ActualizarCarreraComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<CarreraDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_carreraRepository.ExisteActivaPorId(comando.Id))
            return Result<CarreraDto?>.NotFound("La carrera no fue encontrada");

        var nombreNuevo = comando.Nombre?.Trim();

        if (nombreNuevo != null)
        {
            if (string.IsNullOrWhiteSpace(nombreNuevo))
                return Result<CarreraDto?>.Invalid(new ValidationError("Nombre", "El nombre es requerido"));

            if (_carreraRepository.ExisteActivaPorNombreExcluyendoId(nombreNuevo, comando.Id))
                return Result<CarreraDto?>.Conflict("Ya existe otra carrera activa con ese nombre");

            if (_carreraRepository.ReactivarEliminadaPorNombre(nombreNuevo))
            {
                _carreraRepository.EliminarLogicamentePorId(comando.Id);
                return Result<CarreraDto?>.Success(null);
            }
        }

        var comandoFinal = new ActualizarCarreraComando(comando.Id, nombreNuevo);
        var result = _carreraRepository.Actualizar(comandoFinal);
        return result;
    }

    public virtual Result<CarreraDto?> Eliminar(EliminarCarreraComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<CarreraDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_carreraRepository.ExisteActivaPorId(comando.Id))
            return Result<CarreraDto?>.NotFound("La carrera no fue encontrada");

        var result = _carreraRepository.Eliminar(comando);
        return result;
    }

    private Result<CrearCarreraComando> ValidarEntrada(CrearCarreraComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (comando?.Nombre?.Length > 256)
            errors.Add(new("Nombre", "El nombre no puede tener más de 256 caracteres"));

        return errors.Any()
            ? Result<CrearCarreraComando>.Invalid(errors.ToArray())
            : Result<CrearCarreraComando>.Success(comando!);
    }

    private Result<ActualizarCarreraComando> ValidarEntrada(ActualizarCarreraComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        return errors.Any()
            ? Result<ActualizarCarreraComando>.Invalid(errors.ToArray())
            : Result<ActualizarCarreraComando>.Success(comando!);
    }

    private Result<EliminarCarreraComando> ValidarEntrada(EliminarCarreraComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarCarreraComando>.Invalid(errors.ToArray())
            : Result<EliminarCarreraComando>.Success(comando!);
    }

    protected override Dto MapearFilaADto(DataRow fila)
    {
        return new CarreraDto
        {
            Id = Convert.ToInt32(fila["id_carrera"]),
            Nombre = fila["nombre_carrera"] == DBNull.Value ? null : fila["nombre_carrera"].ToString()
        };
    }
}
