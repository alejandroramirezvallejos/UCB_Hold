using System.Data;
using Shared.Common;
public class GaveteroService : IGaveteroService
{
    private readonly GaveteroRepository _gaveteroRepository;

    public GaveteroService(GaveteroRepository gaveteroRepository)
    {
        _gaveteroRepository = gaveteroRepository;
    }    public void CrearGavetero(CrearGaveteroComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _gaveteroRepository.Crear(comando);
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
                ["nombreMueble"] = comando.NombreMueble,
                ["tipo"] = comando.Tipo
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "gavetero", parametros);
        }
    }

    private void ValidarEntradaCreacion(CrearGaveteroComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido("nombre del gavetero");

        if (string.IsNullOrWhiteSpace(comando.NombreMueble))
            throw new ErrorNombreRequerido("nombre del mueble");

        if (comando.Longitud.HasValue && comando.Longitud <= 0)
            throw new ErrorValorNegativo("longitud", comando.Longitud.Value);

        if (comando.Profundidad.HasValue && comando.Profundidad <= 0)
            throw new ErrorValorNegativo("profundidad", comando.Profundidad.Value);

        if (comando.Altura.HasValue && comando.Altura <= 0)
            throw new ErrorValorNegativo("altura", comando.Altura.Value);
    }    public void ActualizarGavetero(ActualizarGaveteroComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _gaveteroRepository.Actualizar(comando);
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
                ["nombreMueble"] = comando.NombreMueble
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "gavetero", parametros);
        }
    }

    public void EliminarGavetero(EliminarGaveteroComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _gaveteroRepository.Eliminar(comando.Id);
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
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "gavetero", parametros);
        }
    }

    private void ValidarEntradaActualizacion(ActualizarGaveteroComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID del gavetero");

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido("nombre del gavetero");

        if (string.IsNullOrWhiteSpace(comando.NombreMueble))
            throw new ErrorNombreRequerido("nombre del mueble");

        if (comando.Longitud.HasValue && comando.Longitud <= 0)
            throw new ErrorValorNegativo("longitud", comando.Longitud.Value);

        if (comando.Profundidad.HasValue && comando.Profundidad <= 0)
            throw new ErrorValorNegativo("profundidad", comando.Profundidad.Value);

        if (comando.Altura.HasValue && comando.Altura <= 0)
            throw new ErrorValorNegativo("altura", comando.Altura.Value);
    }

    private void ValidarEntradaEliminacion(EliminarGaveteroComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID del gavetero");
    }
    public List<GaveteroDto>? ObtenerTodosGaveteros()
    {
        try
        {
            DataTable resultado = _gaveteroRepository.ObtenerTodos();
            var lista = new List<GaveteroDto>(resultado.Rows.Count);
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
    private GaveteroDto MapearFilaADto(DataRow fila)
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