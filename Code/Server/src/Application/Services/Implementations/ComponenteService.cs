using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class ComponenteService : IComponenteService
{
    private readonly IComponenteRepository _componenteRepository;
    public ComponenteService(IComponenteRepository componenteRepository) => _componenteRepository = componenteRepository;
    public virtual void CrearComponente(CrearComponenteComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _componenteRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorModeloRequerido) { throw; }
        catch (ErrorCodigoImtRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró el equipo con código imt")) throw new ErrorReferenciaInvalida("equipo");
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un componente con esos datos")) throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error al insertar componente")) throw new Exception($"Error inesperado al insertar componente: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al crear componente: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear componente: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    private void ValidarEntradaCreacion(CrearComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if(string.IsNullOrWhiteSpace(comando.Modelo)) throw new ErrorModeloRequerido();
        if (comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
        if (comando.CodigoIMT <= 0) throw new ErrorCodigoImtRequerido();
        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
    }
    public virtual List<ComponenteDto>? ObtenerTodosComponentes()
    {
        try
        {
            DataTable resultado = _componenteRepository.ObtenerTodos();
            var lista = new List<ComponenteDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
                lista.Add(MapearFilaADto(fila));
            return lista;
        }
        catch { throw; }
    }
    public virtual void ActualizarComponente(ActualizarComponenteComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _componenteRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (ErrorModeloRequerido) { throw; }
        catch (ErrorCodigoImtRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró un componente activo con id")) throw new ErrorRegistroNoEncontrado();
                if (mensaje.Contains("no se encontró un equipo activo con código imt")) throw new ErrorReferenciaInvalida("equipo");
                if (errorDb.SqlState == "23505" || mensaje.Contains("error de violación de unicidad")) throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error inesperado al actualizar el componente")) throw new Exception($"Error inesperado al actualizar componente: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al actualizar componente: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar componente: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    public virtual void EliminarComponente(EliminarComponenteComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _componenteRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró un componente activo con id")) throw new ErrorRegistroNoEncontrado();
                if (mensaje.Contains("error al eliminar lógicamente el componente")) throw new Exception($"Error inesperado al eliminar componente: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al eliminar componente: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar componente: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    private void ValidarEntradaActualizacion(ActualizarComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido();
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if (!string.IsNullOrWhiteSpace(comando.Modelo) && comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
    }
    private void ValidarEntradaEliminacion(EliminarComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido();
    }
    private static ComponenteDto MapearFilaADto(DataRow fila) => new ComponenteDto
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