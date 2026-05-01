using System.Data;
using Ardalis.Result;

public class GaveteroService : Service<GaveteroDto>, ICrud<GaveteroDto, CrearGaveteroComando, ActualizarGaveteroComando, EliminarGaveteroComando>
{
    private readonly IGaveteroRepository _gaveteroRepository;
    private readonly IMuebleRepository _muebleRepository;

    public GaveteroService(IGaveteroRepository gaveteroRepository, IMuebleRepository muebleRepository)
    {
        _gaveteroRepository = gaveteroRepository;
        _muebleRepository = muebleRepository;
    }

    public Result<GaveteroDto?> Crear(CrearGaveteroComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<GaveteroDto?>.Invalid(validResult.ValidationErrors.ToArray());

        var idMueble = _gaveteroRepository.ObtenerMuebleIdPorNombre(comando.NombreMueble!);
        if (idMueble == null)
            return Result<GaveteroDto?>.NotFound("El mueble no fue encontrado");

        if (_gaveteroRepository.ExisteActivoPorNombre(comando.Nombre!))
            return Result<GaveteroDto?>.Conflict("Ya existe un gavetero activo con este nombre");

        _gaveteroRepository.Crear(idMueble.Value, comando);
        _muebleRepository.ActualizarNumeroGaveteros(idMueble.Value, 1);

        return Result<GaveteroDto?>.Success(null);
    }

    public Result<GaveteroDto?> Actualizar(ActualizarGaveteroComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<GaveteroDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_gaveteroRepository.ExisteActivoPorId(comando.Id))
            return Result<GaveteroDto?>.NotFound("El gavetero no fue encontrado");

        if (!string.IsNullOrWhiteSpace(comando.Nombre))
            if (_gaveteroRepository.ExisteActivoPorNombreExcluyendoId(comando.Nombre, comando.Id))
                return Result<GaveteroDto?>.Conflict("Ya existe otro gavetero activo con ese nombre");

        int? nuevoIdMueble = null;
        int? viejoIdMueble = null;

        if (!string.IsNullOrWhiteSpace(comando.NombreMueble))
        {
            nuevoIdMueble = _gaveteroRepository.ObtenerMuebleIdPorNombre(comando.NombreMueble);
            if (nuevoIdMueble == null)
                return Result<GaveteroDto?>.NotFound("El mueble no fue encontrado");

            viejoIdMueble = _gaveteroRepository.ObtenerMuebleIdPorGaveteroId(comando.Id);
        }

        _gaveteroRepository.Actualizar(nuevoIdMueble, comando);

        if (nuevoIdMueble.HasValue && viejoIdMueble.HasValue && nuevoIdMueble.Value != viejoIdMueble.Value)
        {
            _muebleRepository.ActualizarNumeroGaveteros(viejoIdMueble.Value, -1);
            _muebleRepository.ActualizarNumeroGaveteros(nuevoIdMueble.Value, 1);
        }

        return Result<GaveteroDto?>.Success(null);
    }

    public Result<GaveteroDto?> Eliminar(EliminarGaveteroComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<GaveteroDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_gaveteroRepository.ExisteActivoPorId(comando.Id))
            return Result<GaveteroDto?>.NotFound("El gavetero no fue encontrado");

        var idMueble = _gaveteroRepository.ObtenerMuebleIdPorGaveteroId(comando.Id);

        _gaveteroRepository.Eliminar(comando);

        if (idMueble.HasValue)
            _muebleRepository.ActualizarNumeroGaveteros(idMueble.Value, -1);

        return Result<GaveteroDto?>.Success(null);
    }

    protected override Result<DataTable> ObtenerDataTable()
    {
        var result = _gaveteroRepository.ObtenerTodos();
        if (!result.IsSuccess)
            return Result<DataTable>.Error("Error al obtener los gaveteros");
        return result;
    }

    private Result<CrearGaveteroComando> ValidarEntrada(CrearGaveteroComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.NombreMueble))
            errors.Add(new("NombreMueble", "El nombre del mueble es requerido"));

        if (comando?.Longitud.HasValue == true && comando.Longitud <= 0)
            errors.Add(new("Longitud", "La longitud debe ser mayor a 0"));

        if (comando?.Profundidad.HasValue == true && comando.Profundidad <= 0)
            errors.Add(new("Profundidad", "La profundidad debe ser mayor a 0"));

        if (comando?.Altura.HasValue == true && comando.Altura <= 0)
            errors.Add(new("Altura", "La altura debe ser mayor a 0"));

        return errors.Any()
            ? Result<CrearGaveteroComando>.Invalid(errors.ToArray())
            : Result<CrearGaveteroComando>.Success(comando!);
    }

    private Result<ActualizarGaveteroComando> ValidarEntrada(ActualizarGaveteroComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.NombreMueble) && comando.NombreMueble.Length > 255)
            errors.Add(new("NombreMueble", "El nombre del mueble no puede tener más de 255 caracteres"));

        if (comando?.Longitud.HasValue == true && comando.Longitud <= 0)
            errors.Add(new("Longitud", "La longitud debe ser mayor a 0"));

        if (comando?.Profundidad.HasValue == true && comando.Profundidad <= 0)
            errors.Add(new("Profundidad", "La profundidad debe ser mayor a 0"));

        if (comando?.Altura.HasValue == true && comando.Altura <= 0)
            errors.Add(new("Altura", "La altura debe ser mayor a 0"));

        return errors.Any()
            ? Result<ActualizarGaveteroComando>.Invalid(errors.ToArray())
            : Result<ActualizarGaveteroComando>.Success(comando!);
    }

    private Result<EliminarGaveteroComando> ValidarEntrada(EliminarGaveteroComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarGaveteroComando>.Invalid(errors.ToArray())
            : Result<EliminarGaveteroComando>.Success(comando!);
    }

    protected override Dto MapearFilaADto(DataRow fila) => new GaveteroDto
    {
        Id = Convert.ToInt32(fila["id_gavetero"]),
        Nombre = fila["nombre_gavetero"] == DBNull.Value ? null : Convert.ToString(fila["nombre_gavetero"]),
        Tipo = fila["tipo_gavetero"] == DBNull.Value ? null : Convert.ToString(fila["tipo_gavetero"]),
        NombreMueble = fila["nombre_mueble"] == DBNull.Value ? null : Convert.ToString(fila["nombre_mueble"]),
        Longitud = fila["longitud_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud_gavetero"]),
        Profundidad = fila["profundidad_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad_gavetero"]),
        Altura = fila["altura_gavetero"] == DBNull.Value ? null : Convert.ToDouble(fila["altura_gavetero"])
    };
}
