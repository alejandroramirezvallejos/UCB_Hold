using System.Data;
using Shared.Common;

public class AccesorioService : IAccesorioService
{
    private readonly AccesorioRepository _accesorioRepository;
    public AccesorioService(AccesorioRepository accesorioRepository)
    {
        _accesorioRepository = accesorioRepository;
    }    
    public void CrearAccesorio(CrearAccesorioComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _accesorioRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw; 
        }
        catch (ErrorLongitudInvalida)
        {
            throw; // Re-lanzar excepciones de validación
        }
        catch (ErrorIdInvalido)
        {
            throw; // Re-lanzar excepciones de validación
        }
        catch (ErrorValorNegativo)
        {
            throw; // Re-lanzar excepciones de validación
        }
        catch (Exception ex)
        {
            // 3. Interpretar errores de PostgreSQL
            var parametros = new Dictionary<string, object?>
            {
                ["nombre"] = comando.Nombre,
                ["codigoImt"] = comando.CodigoIMT,
                ["precio"] = comando.Precio
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "accesorio", parametros);
        }
    }

    private void ValidarEntradaCreacion(CrearAccesorioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido("nombre del accesorio");

        if (comando.Nombre.Length > 100)
            throw new ErrorLongitudInvalida("nombre del accesorio", 100);

        if (comando.CodigoIMT <= 0)
            throw new ErrorIdInvalido("código IMT");

        if (comando.Precio.HasValue && comando.Precio.Value < 0)
            throw new ErrorValorNegativo("precio", comando.Precio.Value);
    }
    public List<AccesorioDto>? ObtenerTodosAccesorios()
    {
        try
        {
            DataTable dt = _accesorioRepository.ObtenerTodos();
            var lista = new List<AccesorioDto>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
                lista.Add(MapearFilaADto(row));
            return lista;
        }
        catch
        {
            throw;
        }
    }    public void ActualizarAccesorio(ActualizarAccesorioComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _accesorioRepository.Actualizar(comando);
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
                ["codigoImt"] = comando.CodigoIMT
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "accesorio", parametros);
        }
    }    public void EliminarAccesorio(EliminarAccesorioComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _accesorioRepository.Eliminar(comando.Id);
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
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "accesorio", parametros);
        }
    }
    private static AccesorioDto MapearFilaADto(DataRow fila)
    {
        return new AccesorioDto
        {
            Id = Convert.ToInt32(fila["id_accesorio"]),
            Nombre = fila["nombre_accesorio"]==DBNull.Value? null : fila["nombre_accesorio"].ToString(),
            Modelo = fila["modelo_accesorio"] == DBNull.Value ? null : fila["modelo_accesorio"].ToString(),
            Tipo = fila["tipo_accesorio"] == DBNull.Value ? null : fila["tipo_accesorio"].ToString(),
            Precio = fila["precio_accesorio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio_accesorio"]),
            NombreEquipoAsociado = fila["nombre_equipo_asociado"] == DBNull.Value ? null : fila["nombre_equipo_asociado"].ToString(),
            CodigoImtEquipoAsociado = fila["codigo_imt_equipo_asociado"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo_asociado"]),
            Descripcion = fila["descripcion_accesorio"] == DBNull.Value ? null : fila["descripcion_accesorio"].ToString(),
        };
    }

    private void ValidarEntradaActualizacion(ActualizarAccesorioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID del accesorio");

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido("nombre del accesorio");

        if (comando.Nombre.Length > 100)
            throw new ErrorLongitudInvalida("nombre del accesorio", 100);

        if (comando.CodigoIMT <= 0)
            throw new ErrorIdInvalido("código IMT");

        if (comando.Precio.HasValue && comando.Precio.Value < 0)
            throw new ErrorValorNegativo("precio", comando.Precio.Value);
    }

    private void ValidarEntradaEliminacion(EliminarAccesorioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido("ID del accesorio");
    }
}