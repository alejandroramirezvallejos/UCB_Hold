using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class EquipoService : BaseServicios, IEquipoService
{
    private readonly IEquipoRepository _equipoRepository;
    public EquipoService(IEquipoRepository equipoRepository) => _equipoRepository = equipoRepository;    public void CrearEquipo(CrearEquipoComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _equipoRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (ErrorIdInvalido) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
        }
    }    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearEquipoComando
        if (comando is CrearEquipoComando equipoComando)
        {
            if (string.IsNullOrWhiteSpace(equipoComando.NombreGrupoEquipo)) throw new ErrorNombreRequerido();
            if (string.IsNullOrWhiteSpace(equipoComando.Modelo)) throw new ErrorModeloRequerido();
            if (string.IsNullOrWhiteSpace(equipoComando.Marca)) throw new ErrorMarcaRequerida();
            if (equipoComando.CostoReferencia.HasValue && equipoComando.CostoReferencia < 0) throw new ErrorValorNegativo("costo de referencia");
            if (equipoComando.TiempoMaximoPrestamo.HasValue && equipoComando.TiempoMaximoPrestamo <= 0) throw new ErrorValorNegativo("Tiempo máximo de préstamo");
        }
    }
    
    protected override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró el grupo de equipos con nombre")) throw new ErrorGrupoEquipoNoEncontrado();
            if (mensaje.Contains("no se encontro el gavetero con nombre")) throw new ErrorGaveteroNoEncontrado();
            if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un equipo con ese código ucb o número serial")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar equipo")) throw new Exception($"Error inesperado al insertar equipo: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear equipo: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear equipo: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en creación");
    }
    public void ActualizarEquipo(ActualizarEquipoComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _equipoRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró un equipo activo con id")) throw new ErrorRegistroNoEncontrado();
                if (mensaje.Contains("no se encontró el grupo de equipos con nombre")) throw new ErrorGrupoEquipoNoEncontrado();
                if (mensaje.Contains("no se encontró el gavetero con nombre")) throw new ErrorGaveteroNoEncontrado();
                if (mensaje.Contains("valor inválido para estado_equipo")) throw new ArgumentException("Estado de equipo inválido. Debe ser 'operativo', 'inoperativo', o 'parcialmente_operativo'.");
                if (errorDb.SqlState == "23505") throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error inesperado al actualizar el equipo")) throw new Exception($"Error inesperado al actualizar equipo: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al actualizar equipo: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar equipo: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    private void ValidarEntradaActualizacion(ActualizarEquipoComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("equipo");
        if (comando.CostoReferencia.HasValue && comando.CostoReferencia < 0) throw new ErrorValorNegativo("costo de referencia");
        if (comando.TiempoMaximoPrestamo.HasValue && comando.TiempoMaximoPrestamo <= 0) throw new ErrorValorNegativo("Tiempo máximo de préstamo");
    }
    public void EliminarEquipo(EliminarEquipoComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _equipoRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
        }
    }    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); 
        
        if (comando is EliminarEquipoComando equipoComando)
        {
            if (equipoComando.Id <= 0) throw new ErrorIdInvalido("equipo");
        }
    }
    
    protected override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un equipo activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el equipo")) throw new Exception($"Error inesperado al eliminar equipo: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar equipo: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar equipo: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en eliminación");
    }    
    public List<EquipoDto>? ObtenerTodosEquipos()
    {
        try
        {
            DataTable resultado = _equipoRepository.ObtenerTodos();
            var lista = new List<EquipoDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as EquipoDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
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