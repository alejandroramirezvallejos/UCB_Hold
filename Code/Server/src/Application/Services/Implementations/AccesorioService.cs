using System.Data;
using Ardalis.Result;

public class AccesorioService : Service<AccesorioDto>, ICrud<AccesorioDto, CrearAccesorioComando, ActualizarAccesorioComando, EliminarAccesorioComando>
{
    private readonly IAccesorioRepository _accesorioRepository;

    public AccesorioService(IAccesorioRepository accesorioRepository) => _accesorioRepository = accesorioRepository;

    public Result<AccesorioDto?> Crear(CrearAccesorioComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<AccesorioDto?>.Invalid(validResult.ValidationErrors.ToArray());

        var idEquipo = _accesorioRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT!.Value);
        if (idEquipo == null)
            return Result<AccesorioDto?>.NotFound("El código IMT no fue encontrado");

        return _accesorioRepository.Crear(idEquipo.Value, comando);
    }

    public Result<AccesorioDto?> Actualizar(ActualizarAccesorioComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<AccesorioDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_accesorioRepository.ExisteActivoPorId(comando.Id))
            return Result<AccesorioDto?>.NotFound("No se encontró el registro especificado");

        int? idEquipo = null;
        if (comando.CodigoIMT > 0)
        {
            idEquipo = _accesorioRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT!.Value);
            if (idEquipo == null)
                return Result<AccesorioDto?>.NotFound("El código IMT no fue encontrado");
        }

        return _accesorioRepository.Actualizar(idEquipo, comando);
    }

    public Result<AccesorioDto?> Eliminar(EliminarAccesorioComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<AccesorioDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_accesorioRepository.ExisteActivoPorId(comando.Id))
            return Result<AccesorioDto?>.NotFound("No se encontró el registro especificado");

        return _accesorioRepository.Eliminar(comando);
    }

    protected override Result<DataTable> ObtenerDataTable()
    {
        var result = _accesorioRepository.ObtenerTodos();
        if (!result.IsSuccess)
            return Result<DataTable>.Error("Error al obtener los accesorios");
        return result;
    }

    private Result<CrearAccesorioComando> ValidarEntrada(CrearAccesorioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (comando?.Nombre?.Length > 256)
            errors.Add(new("Nombre", "El nombre no puede tener más de 256 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.Modelo))
            errors.Add(new("Modelo", "El modelo es requerido"));

        if (comando?.CodigoIMT <= 0)
            errors.Add(new("CodigoIMT", "El código IMT es inválido"));

        if (comando?.Precio.HasValue == true && comando.Precio.Value <= 0)
            errors.Add(new("Precio", "El precio no puede ser negativo"));

        return errors.Any()
            ? Result<CrearAccesorioComando>.Invalid(errors.ToArray())
            : Result<CrearAccesorioComando>.Success(comando!);
    }

    private Result<ActualizarAccesorioComando> ValidarEntrada(ActualizarAccesorioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        if (comando?.CodigoIMT < 0)
            errors.Add(new("CodigoIMT", "El código IMT es inválido"));

        if (comando?.Precio.HasValue == true && comando.Precio.Value < 0)
            errors.Add(new("Precio", "El precio no puede ser negativo"));

        return errors.Any()
            ? Result<ActualizarAccesorioComando>.Invalid(errors.ToArray())
            : Result<ActualizarAccesorioComando>.Success(comando!);
    }

    private Result<EliminarAccesorioComando> ValidarEntrada(EliminarAccesorioComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarAccesorioComando>.Invalid(errors.ToArray())
            : Result<EliminarAccesorioComando>.Success(comando!);
    }

    protected override Dto MapearFilaADto(DataRow fila) => new AccesorioDto
    {
        Id = Convert.ToInt32(fila["id_accesorio"]),
        Nombre = fila["nombre_accesorio"] == DBNull.Value ? null : fila["nombre_accesorio"].ToString(),
        Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
        Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
        Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
        CodigoImtEquipoAsociado = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
        Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
        UrlDataSheet = fila["url_data_sheet_accesorio"] == DBNull.Value ? null : fila["url_data_sheet_accesorio"].ToString(),
    };
}
