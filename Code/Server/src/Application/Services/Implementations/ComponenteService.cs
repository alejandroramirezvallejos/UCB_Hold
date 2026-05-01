using System.Data;
using Ardalis.Result;

public class ComponenteService : Service<ComponenteDto>, ICrud<ComponenteDto, CrearComponenteComando, ActualizarComponenteComando, EliminarComponenteComando>
{
    private readonly IComponenteRepository _componenteRepository;

    public ComponenteService(IComponenteRepository componenteRepository) => _componenteRepository = componenteRepository;

    public Result<ComponenteDto?> Crear(CrearComponenteComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<ComponenteDto?>.Invalid(validResult.ValidationErrors.ToArray());

        var idEquipo = _componenteRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT!.Value);
        if (idEquipo == null)
            return Result<ComponenteDto?>.NotFound("El código IMT no fue encontrado");

        return _componenteRepository.Crear(idEquipo.Value, comando);
    }

    public Result<ComponenteDto?> Actualizar(ActualizarComponenteComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<ComponenteDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_componenteRepository.ExisteActivoPorId(comando.Id))
            return Result<ComponenteDto?>.NotFound("El componente no fue encontrado");

        int? idEquipo = null;
        if (comando.CodigoIMT.HasValue && comando.CodigoIMT.Value > 0)
        {
            idEquipo = _componenteRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT.Value);
            if (idEquipo == null)
                return Result<ComponenteDto?>.NotFound("El código IMT no fue encontrado");
        }

        return _componenteRepository.Actualizar(idEquipo, comando);
    }

    public Result<ComponenteDto?> Eliminar(EliminarComponenteComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess)
            return Result<ComponenteDto?>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_componenteRepository.ExisteActivoPorId(comando.Id))
            return Result<ComponenteDto?>.NotFound("El componente no fue encontrado");

        return _componenteRepository.Eliminar(comando);
    }

    protected override Result<DataTable> ObtenerDataTable()
    {
        var result = _componenteRepository.ObtenerTodos();
        if (!result.IsSuccess)
            return Result<DataTable>.Error("Error al obtener los componentes");
        return result;
    }

    private Result<CrearComponenteComando> ValidarEntrada(CrearComponenteComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Nombre))
            errors.Add(new("Nombre", "El nombre es requerido"));

        if (comando?.Nombre?.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        if (string.IsNullOrWhiteSpace(comando?.Modelo))
            errors.Add(new("Modelo", "El modelo es requerido"));

        if (comando?.Modelo?.Length > 255)
            errors.Add(new("Modelo", "El modelo no puede tener más de 255 caracteres"));

        if (comando?.CodigoIMT <= 0)
            errors.Add(new("CodigoIMT", "El código IMT es inválido"));

        if (comando?.PrecioReferencia.HasValue == true && comando.PrecioReferencia.Value < 0)
            errors.Add(new("PrecioReferencia", "El precio de referencia no puede ser negativo"));

        return errors.Any()
            ? Result<CrearComponenteComando>.Invalid(errors.ToArray())
            : Result<CrearComponenteComando>.Success(comando!);
    }

    private Result<ActualizarComponenteComando> ValidarEntrada(ActualizarComponenteComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (!string.IsNullOrWhiteSpace(comando?.Nombre) && comando.Nombre.Length > 255)
            errors.Add(new("Nombre", "El nombre no puede tener más de 255 caracteres"));

        if (!string.IsNullOrWhiteSpace(comando?.Modelo) && comando.Modelo.Length > 255)
            errors.Add(new("Modelo", "El modelo no puede tener más de 255 caracteres"));

        if (comando?.PrecioReferencia.HasValue == true && comando.PrecioReferencia.Value < 0)
            errors.Add(new("PrecioReferencia", "El precio de referencia no puede ser negativo"));

        return errors.Any()
            ? Result<ActualizarComponenteComando>.Invalid(errors.ToArray())
            : Result<ActualizarComponenteComando>.Success(comando!);
    }

    private Result<EliminarComponenteComando> ValidarEntrada(EliminarComponenteComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarComponenteComando>.Invalid(errors.ToArray())
            : Result<EliminarComponenteComando>.Success(comando!);
    }

    protected override Dto MapearFilaADto(DataRow fila) => new ComponenteDto
    {
        Id = Convert.ToInt32(fila["id_componente"]),
        Nombre = fila["nombre_componente"] == DBNull.Value ? null : fila["nombre_componente"].ToString(),
        Modelo = fila["modelo_componente"] == DBNull.Value ? null : fila["modelo_componente"].ToString(),
        Tipo = fila["tipo_componente"] == DBNull.Value ? null : fila["tipo_componente"].ToString(),
        Descripcion = fila["descripcion_componente"] == DBNull.Value ? null : fila["descripcion_componente"].ToString(),
        PrecioReferencia = fila["precio_referencia_componente"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_referencia_componente"]),
        NombreEquipo = fila["nombre_equipo"] == DBNull.Value ? null : fila["nombre_equipo"].ToString(),
        CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
        UrlDataSheet = fila["url_data_sheet_equipo"] == DBNull.Value ? null : fila["url_data_sheet_equipo"].ToString(),
    };
}
