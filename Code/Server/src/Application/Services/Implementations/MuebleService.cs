using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class MuebleService : ServiciosAbstraccion, IMuebleService
{
    private readonly IMuebleRepository _muebleRepository;    public MuebleService(IMuebleRepository muebleRepository)
    {
        _muebleRepository = muebleRepository;
    }
    public virtual void CrearMueble(CrearMuebleComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _muebleRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
            throw;
        }
    }
    public override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando);
        if (comando is CrearMuebleComando cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.Nombre)) throw new ErrorNombreRequerido();
            if (cmd.Costo.HasValue && cmd.Costo < 0) throw new ErrorValorNegativo("costo");
            if (cmd.Longitud.HasValue && cmd.Longitud <= 0) throw new ErrorValorNegativo("longitud");
            if (cmd.Profundidad.HasValue && cmd.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
            if (cmd.Altura.HasValue && cmd.Altura <= 0) throw new ErrorValorNegativo("altura");
        }
    }
    public override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        base.InterpretarErrorCreacion(comando, ex);
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un mueble con el mismo nombre")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar mueble")) throw new Exception($"Error inesperado al insertar mueble: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear mueble: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear mueble: {errorRepo.Message}", errorRepo);
    }
    public virtual void ActualizarMueble(ActualizarMuebleComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _muebleRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorValorNegativo) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no se encontró un mueble activo con id")) throw new ErrorRegistroNoEncontrado();
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe un mueble con el nombre")) throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error inesperado al actualizar mueble")) throw new Exception($"Error inesperado al actualizar mueble: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al actualizar mueble: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar mueble: {errorRepo.Message}", errorRepo);
            throw;
        }
    }    private void ValidarEntradaActualizacion(ActualizarMuebleComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("mueble");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre mueble", 255);
        if (comando.Costo.HasValue && comando.Costo < 0) throw new ErrorValorNegativo("costo");
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }
    public virtual void EliminarMueble(EliminarMuebleComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _muebleRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
            throw;
        }
    }
    public override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando);
        if (comando is EliminarMuebleComando cmd)
        {
            if (cmd.Id <= 0) throw new ErrorIdInvalido("mueble");
        }
    }
    public override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        base.InterpretarErrorEliminacion(comando, ex);
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un mueble activo con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el mueble")) throw new Exception($"Error inesperado al eliminar mueble: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar mueble: {errorDb.Message}", errorDb);
        }        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar mueble: {errorRepo.Message}", errorRepo);
    }
    public virtual List<MuebleDto>? ObtenerTodosMuebles()
    {
        try
        {
            DataTable resultado = _muebleRepository.ObtenerTodos();
            var lista = new List<MuebleDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as MuebleDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    public override BaseDto MapearFilaADto(DataRow fila)
    {
        return new MuebleDto
        {
            Id = Convert.ToInt32(fila["id_mueble"]),
            Nombre = fila["nombre_mueble"] == DBNull.Value ? null : fila["nombre_mueble"].ToString(),
            NumeroGaveteros = fila["numero_gaveteros_mueble"] == DBNull.Value ? null : Convert.ToInt32(fila["numero_gaveteros_mueble"]),
            Ubicacion = fila["ubicacion_mueble"] == DBNull.Value ? null : fila["ubicacion_mueble"].ToString(),
            Tipo = fila["tipo_mueble"] == DBNull.Value ? null : fila["tipo_mueble"].ToString(),
            Costo = fila["costo_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_mueble"]),
            Longitud = fila["longitud_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["longitud_mueble"]),
            Profundidad = fila["profundidad_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["profundidad_mueble"]),
            Altura = fila["altura_mueble"] == DBNull.Value ? null : Convert.ToDouble(fila["altura_mueble"])
        };
    }
}