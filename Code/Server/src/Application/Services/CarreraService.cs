using System.Data;
using Shared.Common;

public class CarreraService : ICarreraService
{
    private readonly CarreraRepository _carreraRepository;
    public CarreraService(CarreraRepository carreraRepository)
    {
        _carreraRepository = carreraRepository;
    }    
    public void CrearCarrera(CrearCarreraComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _carreraRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["nombre"] = comando.Nombre
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "carrera", parametros);
        }
    }    private void ValidarEntradaCreacion(CrearCarreraComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Nombre.Length > 256)
            throw new ErrorLongitudInvalida("nombre de la carrera", 256);
    }
    public List<CarreraDto>? ObtenerTodasCarreras()
    {
        try
        {
            DataTable resultado = _carreraRepository.ObtenerTodas();
            var lista = new List<CarreraDto>(resultado.Rows.Count);
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
    }    public void ActualizarCarrera(ActualizarCarreraComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _carreraRepository.Actualizar(comando);
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
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["id"] = comando.Id,
                ["nombre"] = comando.Nombre
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "carrera", parametros);
        }
    }

    private void ValidarEntradaActualizacion(ActualizarCarreraComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Nombre.Length > 256)
            throw new ErrorLongitudInvalida("nombre de la carrera", 256);
    }    public void EliminarCarrera(EliminarCarreraComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _carreraRepository.Eliminar(comando.Id);
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
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "carrera", parametros);
        }
    }

    private void ValidarEntradaEliminacion(EliminarCarreraComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
    }
    private CarreraDto MapearFilaADto(DataRow fila)
    {
        return new CarreraDto
        {
            Id = Convert.ToInt32(fila["id_carrera"]),
            Nombre = fila["nombre_carrera"] == DBNull.Value ? null : fila["nombre_carrera"].ToString()
        };
    }
}