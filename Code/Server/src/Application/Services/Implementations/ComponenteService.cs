using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class ComponenteService : BaseServicios, IComponenteService
{
    private readonly IComponenteRepository _componenteRepository;
    public ComponenteService(IComponenteRepository componenteRepository) => _componenteRepository = componenteRepository;
    public virtual void CrearComponente(CrearComponenteComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _componenteRepository.Crear(comando);
        }        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorModeloRequerido) { throw; }
        catch (ErrorCodigoImtRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
        }
    }
    
    protected override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró el equipo con código imt")) throw new ErrorCodigoImtNoEncontrado();
            if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un componente con esos datos")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar componente")) throw new Exception($"Error inesperado al insertar componente: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear componente: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear componente: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en creación");
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearComponenteComando
        if (comando is CrearComponenteComando componenteComando)
        {
            if (string.IsNullOrWhiteSpace(componenteComando.Nombre)) throw new ErrorNombreRequerido();
            if (componenteComando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
            if(string.IsNullOrWhiteSpace(componenteComando.Modelo)) throw new ErrorModeloRequerido();
            if (componenteComando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
            if (componenteComando.CodigoIMT <= 0) throw new ErrorCodigoImtRequerido();
            if (componenteComando.PrecioReferencia.HasValue && componenteComando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
        }
    }    public virtual List<ComponenteDto>? ObtenerTodosComponentes()
    {
        try
        {
            DataTable resultado = _componenteRepository.ObtenerTodos();
            var lista = new List<ComponenteDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as ComponenteDto;
                if (dto != null) lista.Add(dto);
            }
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
        catch (ErrorValorNegativo) { throw; }        catch (Exception ex)
        {
            InterpretarErrorActualizacion(comando, ex);
        }
    }
    public void InterpretarErrorActualizacion(ActualizarComponenteComando comando, Exception ex)
    {
       if (ex is ErrorDataBase errorDb)
       {
           var mensaje = errorDb.Message?.ToLower() ?? "";
           if (mensaje.Contains("no se encontró un componente activo con id")) throw new ErrorRegistroNoEncontrado();
           if (mensaje.Contains("no se encontró un equipo activo con código imt")) throw new ErrorCodigoImtNoEncontrado();
           if (errorDb.SqlState == "23505" || mensaje.Contains("error de violación de unicidad")) throw new ErrorRegistroYaExiste();
           if (mensaje.Contains("error inesperado al actualizar el componente")) throw new Exception($"Error inesperado al actualizar componente: {errorDb.Message}", errorDb);
           throw new Exception($"Error inesperado de base de datos al actualizar componente: {errorDb.Message}", errorDb);
       }
       if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar componente: {errorRepo.Message}", errorRepo);
       throw ex ?? new Exception("Error desconocido en actualización");
    }

    private void ValidarEntradaActualizacion(ActualizarComponenteComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("componente");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre", 255);
        if (!string.IsNullOrWhiteSpace(comando.Modelo) && comando.Modelo.Length > 255) throw new ErrorLongitudInvalida("modelo", 255);
        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0) throw new ErrorValorNegativo("precio de referencia");
    }

    public virtual void EliminarComponente(EliminarComponenteComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _componenteRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
        }
    }    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarComponenteComando
        if (comando is EliminarComponenteComando componenteComando)
        {
            if (componenteComando.Id <= 0) throw new ErrorIdInvalido("componente");
        }
    }
    
    protected override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un componente activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el componente")) throw new Exception($"Error inesperado al eliminar componente: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar componente: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar componente: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en eliminación");
    }
    protected override BaseDto MapearFilaADto(DataRow fila) => new ComponenteDto
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