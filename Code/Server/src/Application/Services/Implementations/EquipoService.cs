using System.Data;
using Ardalis.Result;

public class EquipoService : BaseServicios, IEquipoService
{
    private readonly IEquipoRepository _equipoRepository;
    private readonly IGrupoEquipoRepository _grupoEquipoRepository;

    public EquipoService(IEquipoRepository equipoRepository, IGrupoEquipoRepository grupoEquipoRepository)
    {
        _equipoRepository = equipoRepository;
        _grupoEquipoRepository = grupoEquipoRepository;
    }

    public Result<EquipoDto> Crear(CrearEquipoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<EquipoDto>.Invalid(validResult.ValidationErrors.ToArray());

        var idGrupoEquipo = _equipoRepository.ObtenerGrupoEquipoIdPorNombreModeloMarca(
            comando.NombreGrupoEquipo!, comando.Modelo!, comando.Marca!);
        if (idGrupoEquipo == null)
            return Result<EquipoDto>.NotFound("El grupo de equipo no fue encontrado");

        int? idGavetero = null;
        if (!string.IsNullOrWhiteSpace(comando.NombreGavetero))
        {
            idGavetero = _equipoRepository.ObtenerGaveteroIdPorNombre(comando.NombreGavetero);
            if (idGavetero == null)
                return Result<EquipoDto>.NotFound("El gavetero no fue encontrado");
        }

        var idCategoria = _equipoRepository.ObtenerCategoriaIdPorGrupoEquipoId(idGrupoEquipo.Value);
        if (idCategoria == null)
            return Result<EquipoDto>.NotFound("La categoría no fue encontrada");

        var codigoImt = _equipoRepository.GenerarCodigoImt(idCategoria.Value);

        _equipoRepository.Crear(idGrupoEquipo.Value, codigoImt, idGavetero, comando);
        _grupoEquipoRepository.ActualizarCantidad(idGrupoEquipo.Value, 1);
        _grupoEquipoRepository.ActualizarCostoPromedio(idGrupoEquipo.Value);

        return Result<EquipoDto>.Success(null);
    }

    public Result<List<EquipoDto>> ObtenerTodos()
    {
        var repoResult = _equipoRepository.ObtenerTodos();
        if (!repoResult.IsSuccess)
            return Result<List<EquipoDto>>.Error("Error al obtener los equipos");

        var resultado = repoResult.Value;
        var lista = new List<EquipoDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
        {
            var dto = MapearFilaADto(fila) as EquipoDto;
            if (dto != null) lista.Add(dto);
        }
        return lista.Count == 0
            ? Result<List<EquipoDto>>.NotFound("No se encontraron equipos")
            : Result<List<EquipoDto>>.Success(lista);
    }

    public Result<EquipoDto> Actualizar(ActualizarEquipoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<EquipoDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_equipoRepository.ExisteActivoPorId(comando.Id))
            return Result<EquipoDto>.NotFound("El equipo no fue encontrado");

        var grupoActualId = _equipoRepository.ObtenerGrupoEquipoIdPorEquipoId(comando.Id);

        int? nuevoIdGrupoEquipo = null;
        int? nuevoIdGavetero = null;

        if (!string.IsNullOrWhiteSpace(comando.NombreGavetero))
        {
            nuevoIdGavetero = _equipoRepository.ObtenerGaveteroIdPorNombre(comando.NombreGavetero);
            if (nuevoIdGavetero == null)
                return Result<EquipoDto>.NotFound("El gavetero no fue encontrado");
        }

        if (!string.IsNullOrWhiteSpace(comando.EstadoEquipo))
        {
            var isValidState = comando.EstadoEquipo switch
            {
                "operativo" or "inoperativo" or "parcialmente_operativo" => true,
                _ => false
            };
            if (!isValidState)
                return Result<EquipoDto>.Invalid(new ValidationError("EstadoEquipo", "Estado de equipo inválido"));
        }

        _equipoRepository.Actualizar(nuevoIdGrupoEquipo, nuevoIdGavetero, comando);

        if (grupoActualId.HasValue)
            _grupoEquipoRepository.ActualizarCostoPromedio(grupoActualId.Value);

        if (nuevoIdGrupoEquipo.HasValue && grupoActualId.HasValue && nuevoIdGrupoEquipo.Value != grupoActualId.Value)
        {
            _grupoEquipoRepository.ActualizarCantidad(grupoActualId.Value, -1);
            _grupoEquipoRepository.ActualizarCantidad(nuevoIdGrupoEquipo.Value, 1);
            _grupoEquipoRepository.ActualizarCostoPromedio(nuevoIdGrupoEquipo.Value);
        }

        return Result<EquipoDto>.Success(null);
    }

    public Result<EquipoDto> Eliminar(EliminarEquipoComando comando)
    {
        var validResult = ValidarEntrada(comando);
        if (!validResult.IsSuccess) return Result<EquipoDto>.Invalid(validResult.ValidationErrors.ToArray());

        if (!_equipoRepository.ExisteActivoPorId(comando.Id))
            return Result<EquipoDto>.NotFound("El equipo no fue encontrado");

        var grupoActualId = _equipoRepository.ObtenerGrupoEquipoIdPorEquipoId(comando.Id);

        _equipoRepository.Eliminar(comando);

        if (grupoActualId.HasValue)
        {
            _grupoEquipoRepository.ActualizarCantidad(grupoActualId.Value, -1);
            _grupoEquipoRepository.ActualizarCostoPromedio(grupoActualId.Value);
        }

        return Result<EquipoDto>.Success(null);
    }

    private Result<CrearEquipoComando> ValidarEntrada(CrearEquipoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.NombreGrupoEquipo))
            errors.Add(new("NombreGrupoEquipo", "El nombre del grupo de equipo es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Modelo))
            errors.Add(new("Modelo", "El modelo es requerido"));

        if (string.IsNullOrWhiteSpace(comando?.Marca))
            errors.Add(new("Marca", "La marca es requerida"));

        if (comando?.CostoReferencia.HasValue == true && comando.CostoReferencia < 0)
            errors.Add(new("CostoReferencia", "El costo de referencia no puede ser negativo"));

        if (comando?.TiempoMaximoPrestamo.HasValue == true && comando.TiempoMaximoPrestamo <= 0)
            errors.Add(new("TiempoMaximoPrestamo", "El tiempo máximo de préstamo debe ser mayor a 0"));

        return errors.Any()
            ? Result<CrearEquipoComando>.Invalid(errors.ToArray())
            : Result<CrearEquipoComando>.Success(comando!);
    }

    private Result<ActualizarEquipoComando> ValidarEntrada(ActualizarEquipoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        if (comando?.CostoReferencia.HasValue == true && comando.CostoReferencia < 0)
            errors.Add(new("CostoReferencia", "El costo de referencia no puede ser negativo"));

        if (comando?.TiempoMaximoPrestamo.HasValue == true && comando.TiempoMaximoPrestamo <= 0)
            errors.Add(new("TiempoMaximoPrestamo", "El tiempo máximo de préstamo debe ser mayor a 0"));

        return errors.Any()
            ? Result<ActualizarEquipoComando>.Invalid(errors.ToArray())
            : Result<ActualizarEquipoComando>.Success(comando!);
    }

    private Result<EliminarEquipoComando> ValidarEntrada(EliminarEquipoComando comando)
    {
        var errors = new List<ValidationError>();

        if (comando == null)
            errors.Add(new("comando", "El comando es requerido"));

        if (comando?.Id <= 0)
            errors.Add(new("Id", "El ID debe ser mayor a 0"));

        return errors.Any()
            ? Result<EliminarEquipoComando>.Invalid(errors.ToArray())
            : Result<EliminarEquipoComando>.Success(comando!);
    }

    protected override BaseDto MapearFilaADto(DataRow fila) => new EquipoDto
    {
        Id = Convert.ToInt32(fila["id_equipo"]),
        NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
        Modelo = fila["modelo_equipo"] == DBNull.Value ? null : fila["modelo_equipo"].ToString(),
        Marca = fila["marca_equipo"] == DBNull.Value ? null : fila["marca_equipo"].ToString(),
        CodigoImt = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
        CodigoUcb = fila["codigo_ucb_equipo"] == DBNull.Value ? null : fila["codigo_ucb_equipo"].ToString(),
        Descripcion = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString(),
        NumeroSerial = fila["numero_serial_equipo"] == DBNull.Value ? null : fila["numero_serial_equipo"].ToString(),
        Ubicacion = fila["ubicacion_equipo"] == DBNull.Value ? null : fila["ubicacion_equipo"].ToString(),
        Procedencia = fila["procedencia_equipo"] == DBNull.Value ? null : fila["procedencia_equipo"].ToString(),
        TiempoMaximoPrestamo = fila["tiempo_max_prestamo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["tiempo_max_prestamo_equipo"]),
        NombreGavetero = fila["nombre_gavetero_equipo"] == DBNull.Value ? null : fila["nombre_gavetero_equipo"].ToString(),
        EstadoEquipo = fila["estado_equipo_equipo"] == DBNull.Value ? null : fila["estado_equipo_equipo"].ToString(),
        CostoReferencia = fila["costo_referencia_equipo"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_referencia_equipo"]),
    };
}
