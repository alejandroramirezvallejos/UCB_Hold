using System.Data;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

public class AccesorioService : ServiciosAbstraccion, IAccesorioService
{
    private readonly IAccesorioRepository _accesorioRepository;
    public AccesorioService(IAccesorioRepository accesorioRepository) => _accesorioRepository = accesorioRepository;
    public virtual void CrearAccesorio(CrearAccesorioComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _accesorioRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorModeloRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (ErrorCodigoImtInvalido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
        }
    }
    
    public override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró el equipo con código imt"))
                throw new ErrorCodigoImtNoEncontrado();
            if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un accesorio con esos datos"))
                throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar accesorio"))
                throw new Exception($"Error al crear accesorio: {errorDb.Message}", errorDb);
            throw new Exception($"Error de base de datos al crear accesorio: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo)
            throw new Exception($"Error del repositorio al crear accesorio: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en creación");
    }
    public override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearAccesorioComando
        if (comando is CrearAccesorioComando accesorioComando)
        {
            if (string.IsNullOrWhiteSpace(accesorioComando.Nombre)) throw new ErrorNombreRequerido();
            if (string.IsNullOrWhiteSpace(accesorioComando.Modelo)) throw new ErrorModeloRequerido();
            if (accesorioComando.Nombre.Length > 256) throw new ErrorLongitudInvalida("nombre del accesorio", 255);
            if (accesorioComando.CodigoIMT <= 0) throw new ErrorCodigoImtInvalido();
            if (accesorioComando.Precio.HasValue && accesorioComando.Precio.Value <= 0) throw new ErrorValorNegativo("precio");
        }
    }
    public virtual List<AccesorioDto>? ObtenerTodosAccesorios()
    {
        try
        {
            DataTable dt = _accesorioRepository.ObtenerTodos();
            var lista = new List<AccesorioDto>(dt.Rows.Count);            
            foreach (DataRow row in dt.Rows)
            {
                var baseDto = MapearFilaADto(row);
                if (baseDto is AccesorioDto accesorio)
                    lista.Add(accesorio);
            }
            return lista;
        }
        catch { throw; }
    }
    public virtual void ActualizarAccesorio(ActualizarAccesorioComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _accesorioRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorModeloRequerido) { throw; }
        catch (ErrorCodigoImtInvalido) { throw; }
        catch (ErrorCodigoImtNoEncontrado) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró un accesorio activo con id"))
                    throw new ErrorRegistroNoEncontrado();
                if (mensaje.Contains("no se encontró un equipo activo con código imt"))
                    throw new ErrorCodigoImtNoEncontrado();
                if (errorDb.SqlState == "23505" || mensaje.Contains("error de violación de unicidad"))
                    throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error inesperado al actualizar el accesorio"))
                    throw new Exception($"Error inesperado al actualizar el accesorio: {errorDb.Message}", errorDb);
                throw new Exception($"Error de base de datos al actualizar accesorio: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo)
                throw new Exception($"Error del repositorio al actualizar accesorio: {errorRepo.Message}", errorRepo);
            throw;
        }
    }
    private void ValidarEntradaActualizacion(ActualizarAccesorioComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("accesorio");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre del accesorio", 255);
        if (comando.CodigoIMT <= 0) throw new ErrorCodigoImtInvalido();
        if (comando.Precio.HasValue && comando.Precio.Value < 0) throw new ErrorValorNegativo("precio");
    }
    public virtual void EliminarAccesorio(EliminarAccesorioComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _accesorioRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw;        }
        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
        }
    }
    
    public override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un accesorio activo con id"))
                throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el accesorio"))
                throw new Exception($"Error al eliminar accesorio: {errorDb.Message}", errorDb);
            throw new Exception($"Error de base de datos al eliminar accesorio: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo)
            throw new Exception($"Error del repositorio al eliminar accesorio: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en eliminación");
    }
    public override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarAccesorioComando
        if (comando is EliminarAccesorioComando accesorioComando)
        {
            if (accesorioComando.Id <= 0) throw new ErrorIdInvalido("accesorio");
        }
    }    
    public override BaseDto MapearFilaADto(DataRow fila)
    {
        return new AccesorioDto
        {
            Id = Convert.ToInt32(fila["id_accesorio"]),
            Nombre = fila["nombre_accesorio"] == DBNull.Value ? null : fila["nombre_accesorio"].ToString(),
            Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
            Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
            Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
            NombreEquipoAsociado = fila["nombre_equipo_asociado"] == DBNull.Value ? null : fila["nombre_equipo_asociado"].ToString(),
            CodigoImtEquipoAsociado = fila["codigo_imt_equipo_asociado"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo_asociado"]),
            Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
            UrlDataSheet = fila["url_data_sheet_accesorio"] == DBNull.Value ? null : fila["url_data_sheet_accesorio"].ToString(),
        };
    }
}