using System.Data;
using IMT_Reservas.Server.Shared.Common;
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
        }        catch (Exception ex)
        {
            // Manejo específico para insertar_gavetero según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Mueble no encontrado
                if (mensaje.Contains("no se encontró el mueble con nombre"))
                {
                    throw new ErrorReferenciaInvalida("mueble");
                }
                
                // Error: Gavetero duplicado
                if (mensaje.Contains("ya existe un gavetero con nombre"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error: Violación de unicidad
                if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad al intentar insertar gavetero"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al insertar gavetero"))
                {
                    throw new Exception($"Error inesperado al insertar gavetero: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al crear gavetero: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al crear gavetero: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }    private void ValidarEntradaCreacion(CrearGaveteroComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (string.IsNullOrWhiteSpace(comando.NombreMueble))
            throw new ErrorNombreMuebleRequerido();

        if (comando.Longitud.HasValue && comando.Longitud <= 0)
            throw new ErrorValorNegativo("longitud");

        if (comando.Profundidad.HasValue && comando.Profundidad <= 0)
            throw new ErrorValorNegativo("profundidad");

        if (comando.Altura.HasValue && comando.Altura <= 0)
            throw new ErrorValorNegativo("altura");
    }public void ActualizarGavetero(ActualizarGaveteroComando comando)
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
        }        catch (Exception ex)
        {
            // Manejo específico para actualizar_gavetero según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Gavetero no encontrado
                if (mensaje.Contains("no se encontró un gavetero activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error: Mueble no encontrado
                if (mensaje.Contains("no se encontró el mueble activo con nombre"))
                {
                    throw new ErrorReferenciaInvalida("mueble");
                }
                
                // Error: Nombre de gavetero duplicado
                if (mensaje.Contains("ya existe otro gavetero activo con el nombre"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error: Violación de unicidad general
                if (errorDb.SqlState == "23505" || mensaje.Contains("violación de unicidad"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error inesperado al actualizar el gavetero"))
                {
                    throw new Exception($"Error inesperado al actualizar gavetero: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al actualizar gavetero: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al actualizar gavetero: {errorRepo.Message}", errorRepo);
            }
            
            throw;
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
        }        catch (Exception ex)
        {
            // Manejo específico para eliminar_gavetero según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Gavetero no encontrado
                if (mensaje.Contains("no se encontró un gavetero activo con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente el gavetero"))
                {
                    throw new Exception($"Error inesperado al eliminar gavetero: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar gavetero: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar gavetero: {errorRepo.Message}", errorRepo);
            }
            
            throw;
        }
    }    private void ValidarEntradaActualizacion(ActualizarGaveteroComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0)
            throw new ErrorIdInvalido();

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (string.IsNullOrWhiteSpace(comando.NombreMueble))
            throw new ErrorNombreMuebleRequerido();

        if (comando.Longitud.HasValue && comando.Longitud <= 0)
            throw new ErrorValorNegativo("longitud");

        if (comando.Profundidad.HasValue && comando.Profundidad <= 0)
            throw new ErrorValorNegativo("profundidad");

        if (comando.Altura.HasValue && comando.Altura <= 0)
            throw new ErrorValorNegativo("altura");
    }

    private void ValidarEntradaEliminacion(EliminarGaveteroComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (comando.Id <= 0)
            throw new ErrorIdInvalido();
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