using System.Data;
using Shared.Common;
public class MuebleService : IMuebleService
{
    private readonly MuebleRepository _muebleRepository;

    public MuebleService(MuebleRepository muebleRepository)
    {
        _muebleRepository = muebleRepository;
    }
    public void CrearMueble(CrearMuebleComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _muebleRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["nombre"] = comando.Nombre,
                ["tipo"] = comando.Tipo,
                ["ubicacion"] = comando.Ubicacion
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "mueble", parametros);
        }
    }
    public void ActualizarMueble(ActualizarMuebleComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _muebleRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorValorNegativo)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id,
                ["nombre"] = comando.Nombre,
                ["tipo"] = comando.Tipo,
                ["ubicacion"] = comando.Ubicacion
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "mueble", parametros);
        }
    }
    public void EliminarMueble(EliminarMuebleComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _muebleRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "mueble", parametros);
        }
    }
    public List<MuebleDto>? ObtenerTodosMuebles()
    {
        try
        {
            DataTable resultado = _muebleRepository.ObtenerTodos();
            var lista = new List<MuebleDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch
        {
            throw;
        }
    }
    private MuebleDto MapearFilaADto(DataRow fila)
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
    }    private void ValidarEntradaCreacion(CrearMuebleComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Costo.HasValue && comando.Costo < 0)
            throw new ErrorValorNegativo("costo");

        if (comando.Longitud.HasValue && comando.Longitud <= 0)
            throw new ErrorValorNegativo("longitud");

        if (comando.Profundidad.HasValue && comando.Profundidad <= 0)
            throw new ErrorValorNegativo("profundidad");

        if (comando.Altura.HasValue && comando.Altura <= 0)
            throw new ErrorValorNegativo("altura");
    }

    private void ValidarEntradaActualizacion(ActualizarMuebleComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Costo.HasValue && comando.Costo < 0)
            throw new ErrorValorNegativo("costo");

        if (comando.Longitud.HasValue && comando.Longitud <= 0)
            throw new ErrorValorNegativo("longitud");

        if (comando.Profundidad.HasValue && comando.Profundidad <= 0)
            throw new ErrorValorNegativo("profundidad");

        if (comando.Altura.HasValue && comando.Altura <= 0)
            throw new ErrorValorNegativo("altura");
    }

    private void ValidarEntradaEliminacion(EliminarMuebleComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }
}