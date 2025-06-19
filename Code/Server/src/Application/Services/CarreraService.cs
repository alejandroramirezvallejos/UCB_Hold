using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class CarreraService : ICarreraService
{
    private readonly ICarreraRepository _carreraRepository;
    public CarreraService(ICarreraRepository carreraRepository)
    {
        _carreraRepository = carreraRepository;
    }    
    public virtual void CrearCarrera(CrearCarreraComando comando)
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
        }        catch (Exception ex)
        {
            // Manejo específico para insertar_carrera según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Nombre vacío (aunque ya se valida en la entrada)
                if (mensaje.Contains("el nombre de la carrera no puede estar vacío"))
                {
                    throw new ErrorNombreRequerido();
                }
                
                // Error: Nombre duplicado
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe una carrera con el nombre"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al insertar carrera"))
                {
                    throw new Exception($"Error inesperado al insertar carrera: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al crear carrera: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al crear carrera: {errorRepo.Message}", errorRepo);
            }
            
            throw;
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
    public virtual List<CarreraDto>? ObtenerTodasCarreras()
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
    }    public virtual void ActualizarCarrera(ActualizarCarreraComando comando)
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
        }        catch (Exception ex)
        {
            // Manejo específico para actualizar_carrera según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Carrera no encontrada
                if (mensaje.Contains("no se encontró una carrera activa con el id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error: Nombre vacío (aunque ya se valida en la entrada)
                if (mensaje.Contains("el nuevo nombre de la carrera no puede estar vacío"))
                {
                    throw new ErrorNombreRequerido();
                }
                
                // Error: ID inválido (aunque ya se valida en la entrada)
                if (mensaje.Contains("el id de la carrera debe ser un número positivo"))
                {
                    throw new ErrorIdInvalido();
                }
                
                // Error: Nombre duplicado
                if (mensaje.Contains("ya existe otra carrera con el nombre"))
                {
                    throw new ErrorRegistroYaExiste();
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al actualizar carrera: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al actualizar carrera: {errorRepo.Message}", errorRepo);
            }
            
            throw;
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

        if (comando.Nombre.Length > 255)
            throw new ErrorLongitudInvalida("nombre de la carrera", 255);
    }    public virtual void EliminarCarrera(EliminarCarreraComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _carreraRepository.Eliminar(comando.Id);
        }
        catch (ErrorIdInvalido)
        {
            throw;
        }        catch (Exception ex)
        {
            // Manejo específico para eliminar_carrera según el procedimiento almacenado
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                
                // Error: Carrera no encontrada
                if (mensaje.Contains("no se encontró una carrera activa con id"))
                {
                    throw new ErrorRegistroNoEncontrado();
                }
                
                // Error genérico del procedimiento
                if (mensaje.Contains("error al eliminar lógicamente la carrera"))
                {
                    throw new Exception($"Error inesperado al eliminar carrera: {errorDb.Message}", errorDb);
                }
                
                // Otros errores de base de datos
                throw new Exception($"Error inesperado de base de datos al eliminar carrera: {errorDb.Message}", errorDb);
            }
            
            if (ex is ErrorRepository errorRepo)
            {
                throw new Exception($"Error del repositorio al eliminar carrera: {errorRepo.Message}", errorRepo);
            }
            
            throw;
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