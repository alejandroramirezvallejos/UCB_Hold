using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class CarreraService : ServiciosAbstraccion, ICarreraService
{
    private readonly ICarreraRepository _carreraRepository;
    public CarreraService(ICarreraRepository carreraRepository) => _carreraRepository = carreraRepository;
    public virtual void CrearCarrera(CrearCarreraComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _carreraRepository.Crear(comando);
        }        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
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
            if (mensaje.Contains("el nombre de la carrera no puede estar vacío")) throw new ErrorNombreRequerido();
            if (errorDb.SqlState == "23505" || mensaje.Contains("ya existe una carrera con el nombre")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("error al insertar carrera")) throw new Exception($"Error inesperado al insertar carrera: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear carrera: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear carrera: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en creación");
    }
    
    public override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearCarreraComando
        if (comando is CrearCarreraComando carreraComando)
        {
            if (string.IsNullOrWhiteSpace(carreraComando.Nombre)) throw new ErrorNombreRequerido();
            if (carreraComando.Nombre.Length > 256) throw new ErrorLongitudInvalida("nombre de la carrera", 255);
        }
    }
    public virtual List<CarreraDto>? ObtenerTodasCarreras()
    {
        try
        {
            DataTable resultado = _carreraRepository.ObtenerTodas();
            var lista = new List<CarreraDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var baseDto = MapearFilaADto(fila);
                if (baseDto is CarreraDto carrera)
                    lista.Add(carrera);
            }
            return lista;
        }
        catch { throw; }
    }
    public virtual void ActualizarCarrera(ActualizarCarreraComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _carreraRepository.Actualizar(comando);
        }        catch (ErrorIdInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorActualizacion(comando, ex);
        }
    }
    
    private void InterpretarErrorActualizacion<T>(T comando, Exception ex)
    {
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró una carrera activa con el id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("el nuevo nombre de la carrera no puede estar vacío")) throw new ErrorNombreRequerido();
            if (mensaje.Contains("el id de la carrera debe ser un número positivo")) throw new ErrorIdInvalido("carrera");
            if (mensaje.Contains("ya existe otra carrera con el nombre")) throw new ErrorRegistroYaExiste();
            throw new Exception($"Error inesperado de base de datos al actualizar carrera: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar carrera: {errorRepo.Message}", errorRepo);
        throw ex;
    }
    private void ValidarEntradaActualizacion(ActualizarCarreraComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("carrera");
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre de la carrera", 255);
    }
    public virtual void EliminarCarrera(EliminarCarreraComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _carreraRepository.Eliminar(comando.Id);        
            }
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
            if (mensaje.Contains("no se encontró una carrera activa con id")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente la carrera")) throw new Exception($"Error inesperado al eliminar carrera: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar carrera: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar carrera: {errorRepo.Message}", errorRepo);
        throw ex ?? new Exception("Error desconocido en eliminación");
    }
    
    public override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarCarreraComando
        if (comando is EliminarCarreraComando carreraComando)
        {
            if (carreraComando.Id <= 0) throw new ErrorIdInvalido("carrera");
        }
    }    
    public override BaseDto MapearFilaADto(DataRow fila)
    {
        return new CarreraDto
        {
            Id = Convert.ToInt32(fila["id_carrera"]),
            Nombre = fila["nombre_carrera"] == DBNull.Value ? null : fila["nombre_carrera"].ToString()
        };
    }
}
