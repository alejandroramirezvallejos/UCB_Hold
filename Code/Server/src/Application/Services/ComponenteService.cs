using System.Data;
using Shared.Common;
public class ComponenteService : IComponenteService
{
    private readonly ComponenteRepository _componenteRepository;
    public ComponenteService(ComponenteRepository componenteRepository)
    {
        _componenteRepository = componenteRepository;
    }    public void CrearComponente(CrearComponenteComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _componenteRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorModeloRequerido)
        {
            throw;
        }
        catch (ErrorCodigoImtRequerido)
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
                ["modelo"] = comando.Modelo,
                ["codigoImt"] = comando.CodigoIMT
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "componente", parametros);
        }
    }

    private void ValidarEntradaCreacion(CrearComponenteComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 255)
            throw new ErrorLongitudInvalida("nombre", 255);
        if(string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorModeloRequerido();
        if (comando.Modelo.Length > 255)
            throw new ErrorLongitudInvalida("modelo", 255);

        if (comando.CodigoIMT <= 0)
            throw new ErrorCodigoImtRequerido();

        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0)
            throw new ErrorValorNegativo("precio de referencia");
    }
    public List<ComponenteDto>? ObtenerTodosComponentes()
    {
        try
        {
            DataTable resultado = _componenteRepository.ObtenerTodos();
            var lista = new List<ComponenteDto>(resultado.Rows.Count);
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
    public void ActualizarComponente(ActualizarComponenteComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _componenteRepository.Actualizar(comando);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }catch (ErrorModeloRequerido)
        {
            throw;
        }
        catch (ErrorCodigoImtRequerido)
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
                ["modelo"] = comando.Modelo
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "componente", parametros);
        }
    }

    public void EliminarComponente(EliminarComponenteComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _componenteRepository.Eliminar(comando.Id);
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
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "componente", parametros);
        }
    }    private void ValidarEntradaActualizacion(ActualizarComponenteComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Nombre.Length > 100)
            throw new ErrorLongitudInvalida("nombre", 100);
        if (string.IsNullOrWhiteSpace(comando.Modelo))
            throw new ErrorModeloRequerido();
        if (comando.Modelo.Length > 50)
            throw new ErrorLongitudInvalida("modelo", 50);

        if (comando.CodigoIMT <= 0)
            throw new ErrorCodigoImtRequerido();

        if (comando.PrecioReferencia.HasValue && comando.PrecioReferencia.Value < 0)
            throw new ErrorValorNegativo("precio de referencia");
    }    private void ValidarEntradaEliminacion(EliminarComponenteComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }
    private static ComponenteDto MapearFilaADto(DataRow fila)
    {
        return new ComponenteDto
        {
            Id = Convert.ToInt32(fila["id_componente"]),
            Nombre = fila["nombre_componente"] == DBNull.Value ? null : fila["nombre_componente"].ToString(),
            Modelo = fila["modelo_componente"] == DBNull.Value ? null : fila["modelo_componente"].ToString(),
            Tipo = fila["tipo_componente"] == DBNull.Value ? null : fila["tipo_componente"].ToString(),
            Descripcion = fila["descripcion_componente"] == DBNull.Value ? null : fila["descripcion_componente"].ToString(),
            PrecioReferencia = fila["precio_referencia_componente"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_referencia_componente"]),
            NombreEquipo = fila["nombre_equipo"] == DBNull.Value ? null : fila["nombre_equipo"].ToString(),
            CodigoImtEquipo = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"])
        };
    }
}