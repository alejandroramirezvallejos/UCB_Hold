using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class GaveteroService : ServiciosAbstraccion, IGaveteroService
{
    private readonly IGaveteroRepository _gaveteroRepository;
    public GaveteroService(IGaveteroRepository gaveteroRepository)
    {
        _gaveteroRepository = gaveteroRepository;
    }
    public virtual void CrearGavetero(CrearGaveteroComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _gaveteroRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
        }
    }    
    public override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearGaveteroComando
        if (comando is CrearGaveteroComando gaveteroComando)
        {
            if (string.IsNullOrWhiteSpace(gaveteroComando.Nombre)) throw new ErrorNombreRequerido();
            if (string.IsNullOrWhiteSpace(gaveteroComando.NombreMueble)) throw new ErrorNombreMuebleRequerido();
            if (gaveteroComando.Longitud.HasValue && gaveteroComando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
            if (gaveteroComando.Profundidad.HasValue && gaveteroComando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
            if (gaveteroComando.Altura.HasValue && gaveteroComando.Altura <= 0) throw new ErrorValorNegativo("altura");
        }
    }
    
    public override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró el mueble con nombre")) throw new ErrorMuebleNoEncontrado();
            if (mensaje.Contains("ya existe un gavetero con nombre")) throw new ErrorRegistroYaExiste();
            if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad al intentar insertar gavetero")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar gavetero")) throw new Exception($"Error inesperado al insertar gavetero: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear gavetero: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear gavetero: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en creación");
    }
    public virtual void ActualizarGavetero(ActualizarGaveteroComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _gaveteroRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }        catch (Exception ex)
        {
            InterpretarErrorActualizacion(comando, ex);
        }
    }
    
    private void ValidarEntradaActualizacion(ActualizarGaveteroComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("gavetero");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre gavetero", 255);
        if (!string.IsNullOrWhiteSpace(comando.NombreMueble) && comando.NombreMueble.Length > 255) throw new ErrorLongitudInvalida("nombre mueble", 255);
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }

    public virtual void EliminarGavetero(EliminarGaveteroComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _gaveteroRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
        }
    }    
    public override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarGaveteroComando
        if (comando is EliminarGaveteroComando gaveteroComando)
        {
            if (gaveteroComando.Id <= 0) throw new ErrorIdInvalido("gavetero");
        }
    }
    
    public override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un gavetero activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el gavetero")) throw new Exception($"Error inesperado al eliminar gavetero: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar gavetero: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar gavetero: {errorRepo.Message}", errorRepo);        throw ex ?? new Exception("Error desconocido en eliminación");
    }
    
    private void InterpretarErrorActualizacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un gavetero activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("no se encontró el mueble activo con nombre")) throw new ErrorMuebleNoEncontrado();
            if (mensaje.Contains("ya existe otro gavetero activo con el nombre")) throw new ErrorRegistroYaExiste();
            if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error inesperado al actualizar el gavetero")) throw new Exception($"Error inesperado al actualizar gavetero: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al actualizar gavetero: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar gavetero: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en actualización");
    }public virtual List<GaveteroDto>? ObtenerTodosGaveteros()
    {
        try
        {
            DataTable resultado = _gaveteroRepository.ObtenerTodos();
            var lista = new List<GaveteroDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as GaveteroDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }    
    public override BaseDto MapearFilaADto(DataRow fila)
    {
        return new GaveteroDto
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
}