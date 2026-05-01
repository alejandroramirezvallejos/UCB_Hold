using System.Data;
using Ardalis.Result;

public class CarreraService : Service<CarreraDto>, ICrud<CarreraDto, CrearCarreraComando, ActualizarCarreraComando, EliminarCarreraComando>
{
    private readonly ICarreraRepository _carreraRepository;

    public CarreraService(ICarreraRepository carreraRepository) => _carreraRepository = carreraRepository;

    public Result<CarreraDto?> Crear(CrearCarreraComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<CarreraDto?>.Invalid(validResult.ValidationErrors.ToArray());

        var nombreTrimmed = comando.Nombre!.Trim();

        if (string.IsNullOrWhiteSpace(nombreTrimmed))
            return Result<CarreraDto?>.Invalid(new ValidationError("Nombre", "El nombre es requerido"));

        if (_carreraRepository.ReactivarEliminadaPorNombre(nombreTrimmed))
            return Result<CarreraDto?>.Success(null);

        if (_carreraRepository.ExisteActivaPorNombre(nombreTrimmed))
            return Result<CarreraDto?>.Conflict("Ya existe una carrera activa con este nombre");

        var comandoFinal = new CrearCarreraComando(nombreTrimmed);
        return _carreraRepository.Crear(comandoFinal);
    }

    public Result<CarreraDto?> Actualizar(ActualizarCarreraComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<CarreraDto?>.Invalid(validResult.ValidationErrors.ToArray());

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
        return _carreraRepository.Actualizar(comandoFinal);
    }

    public Result<CarreraDto?> Eliminar(EliminarCarreraComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<CarreraDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_carreraRepository.ExisteActivaPorId(comando.Id))
            return Result<CarreraDto?>.NotFound("La carrera no fue encontrada");

        return _carreraRepository.Eliminar(comando);
    }

    protected override Result<DataTable> ObtenerDataTable()
    {
        var result = _carreraRepository.ObtenerTodos();
        if (!result.IsSuccess)
            return Result<DataTable>.Error("Error al obtener las carreras");
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

    protected override Dto MapearFilaADto(DataRow fila) => new CarreraDto
    {
        Id = Convert.ToInt32(fila["id_carrera"]),
        Nombre = fila["nombre_carrera"] == DBNull.Value ? null : fila["nombre_carrera"].ToString()
    };
}
