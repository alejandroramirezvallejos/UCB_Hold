using System.Data;
using Ardalis.Result;

public class MantenimientoService : BaseServicios, IMantenimientoService
{
    private readonly IMantenimientoRepository _mantenimientoRepository;

    public MantenimientoService(IMantenimientoRepository mantenimientoRepository)
    {
        _mantenimientoRepository = mantenimientoRepository;
    }

    public virtual Result<MantenimientoDto> Crear(CrearMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<MantenimientoDto>.Invalid(validResult.ValidationErrors.ToArray());

        var idEmpresa = _mantenimientoRepository.ObtenerEmpresaIdPorNombre(comando.NombreEmpresaMantenimiento!);
        if (idEmpresa == null)
            return Result<MantenimientoDto>.NotFound("La empresa de mantenimiento no fue encontrada");

        var equipoIds = new int[comando.CodigoIMT!.Length];
        for (int i = 0; i < comando.CodigoIMT.Length; i++)
        {
            var idEquipo = _mantenimientoRepository.ObtenerEquipoIdPorCodigoImt(comando.CodigoIMT[i]);
            if (idEquipo == null)
                return Result<MantenimientoDto>.NotFound("El código IMT no fue encontrado");
            equipoIds[i] = idEquipo.Value;
        }

        var idMantenimiento = _mantenimientoRepository.CrearMantenimiento(idEmpresa.Value, comando);

        for (int i = 0; i < equipoIds.Length; i++)
        {
            var tipo = comando.TipoMantenimiento != null && i < comando.TipoMantenimiento.Length
                ? comando.TipoMantenimiento[i] : null;
            var desc = comando.DescripcionEquipo != null && i < comando.DescripcionEquipo.Length
                ? comando.DescripcionEquipo[i] : null;
            _mantenimientoRepository.CrearDetalleMantenimiento(idMantenimiento, equipoIds[i], tipo, desc);
        }

        return Result<MantenimientoDto>.Success(null);
    }

    public virtual Result<List<MantenimientoDto>> ObtenerTodos()
    {
        var repoResult = _mantenimientoRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<MantenimientoDto>>.Error("Error al obtener los mantenimientos");

        var resultado = repoResult.Value;
        var lista = new List<MantenimientoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var dto = MapearFilaADto(fila) as MantenimientoDto;
            if (dto != null) lista.Add(dto);
        }
        return lista.Count == 0
            ? Result<List<MantenimientoDto>>.NotFound("No se encontraron mantenimientos")
            : Result<List<MantenimientoDto>>.Success(lista);
    }

    public virtual Result<MantenimientoDto> Eliminar(EliminarMantenimientoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<MantenimientoDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_mantenimientoRepository.ExisteActivoPorId(comando.Id))
            return Result<MantenimientoDto>.NotFound("El mantenimiento no fue encontrado");

        var result = _mantenimientoRepository.Eliminar(comando);
        return result;
    }

    private Result<CrearMantenimientoComando> ValidarEntrada(CrearMantenimientoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.FechaMantenimiento == null)
            errors.Add(new("FechaMantenimiento", "La fecha de mantenimiento es requerida"));

        if (comando?.FechaFinalDeMantenimiento == null)
            errors.Add(new("FechaFinalDeMantenimiento", "La fecha final de mantenimiento es requerida"));

        if (comando?.FechaFinalDeMantenimiento < comando?.FechaMantenimiento)
            errors.Add(new("FechaFinalDeMantenimiento", "La fecha final no puede ser anterior a la fecha de inicio"));

        if (string.IsNullOrWhiteSpace(comando?.NombreEmpresaMantenimiento))
            errors.Add(new("NombreEmpresaMantenimiento", "El nombre de la empresa es requerido"));

        if (comando?.CodigoIMT == null || comando.CodigoIMT.Length == 0)
            errors.Add(new("CodigoIMT", "Los códigos IMT son requeridos"));

        if (comando?.TipoMantenimiento == null || comando.TipoMantenimiento.Length == 0)
            errors.Add(new("TipoMantenimiento", "Los tipos de mantenimiento son requeridos"));

        if (comando?.CodigoIMT?.Length != comando?.TipoMantenimiento?.Length)
            errors.Add(new("CodigoIMT", "Los códigos IMT y tipos de mantenimiento deben tener la misma cantidad"));

        if (comando?.CodigoIMT?.Any(codigo => codigo <= 0) == true)
            errors.Add(new("CodigoIMT", "Los códigos IMT deben ser válidos"));

        if (comando?.Costo.HasValue == true && comando.Costo.Value < 0)
            errors.Add(new("Costo", "El costo no puede ser negativo"));

        return errors.Any()
            ? Result<CrearMantenimientoComando>.Invalid(errors.ToArray())
            : Result<CrearMantenimientoComando>.Success(comando!);
    }

    private Result<EliminarMantenimientoComando> ValidarEntrada(EliminarMantenimientoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarMantenimientoComando>.Invalid(errors.ToArray())
            : Result<EliminarMantenimientoComando>.Success(comando!);
    }

    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new MantenimientoDto
        {
            Id = Convert.ToInt32(fila["id_mantenimiento"]),
            FechaMantenimiento = fila["fecha_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_mantenimiento"])),
            FechaFinalDeMantenimiento = fila["fecha_final_mantenimiento"] == DBNull.Value ? null : DateOnly.FromDateTime(Convert.ToDateTime(fila["fecha_final_mantenimiento"])),
            NombreEmpresaMantenimiento = fila["nombre_empresa_mantenimiento"] == DBNull.Value ? null : fila["nombre_empresa_mantenimiento"].ToString(),
            Costo = fila["costo_mantenimiento"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mantenimiento"]),
            Descripcion = fila["descripcion_mantenimiento"] == DBNull.Value ? null : fila["descripcion_mantenimiento"].ToString(),
            CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            TipoMantenimiento = fila["tipo_detalle_mantenimiento"] == DBNull.Value ? null : fila["tipo_detalle_mantenimiento"].ToString(),
            DescripcionEquipo = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString()
        };
    }
}
