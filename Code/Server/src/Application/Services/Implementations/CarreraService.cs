using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class CarreraService : BaseServicios,
    ICrearServicio<CrearCarreraComando>,
    IActualizarServicio<ActualizarCarreraComando>,
    IEliminarServicio<EliminarCarreraComando>,
    IObtenerTodosServicio<CarreraDto>
{
    private readonly CarreraRepository _carreraRepository;

    public CarreraService(CarreraRepository carreraRepository)
    {
        _carreraRepository = carreraRepository;
    }

    public virtual void Crear(CrearCarreraComando comando)
    {
        ValidarEntradaCreacion(comando);

        var nombreTrimmed = comando.Nombre!.Trim();

        if (string.IsNullOrWhiteSpace(nombreTrimmed))
            throw new ErrorNombreRequerido();

        // Intentar reactivar si existe una carrera eliminada lógicamente con ese nombre
        if (_carreraRepository.ReactivarEliminadaPorNombre(nombreTrimmed))
            return;

        // Verificar si ya existe una carrera activa con ese nombre
        if (_carreraRepository.ExisteActivaPorNombre(nombreTrimmed))
            throw new ErrorRegistroYaExiste();

        // Insertar nueva carrera (usar record con nombre trimmed)
        var comandoFinal = new CrearCarreraComando(nombreTrimmed);
        _carreraRepository.Crear(comandoFinal);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearCarreraComando
        if (comando is CrearCarreraComando carreraComando)
        {
            if (string.IsNullOrWhiteSpace(carreraComando.Nombre)) throw new ErrorNombreRequerido();
            if (carreraComando.Nombre.Length > 256) throw new ErrorLongitudInvalida("nombre de la carrera", 255);
        }
    }
    public virtual List<CarreraDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _carreraRepository.ObtenerTodos();
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
    public virtual void Actualizar(ActualizarCarreraComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que la carrera exista y esté activa
        if (!_carreraRepository.ExisteActivaPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        var nombreNuevo = comando.Nombre?.Trim();

        if (nombreNuevo != null)
        {
            if (string.IsNullOrWhiteSpace(nombreNuevo))
                throw new ErrorNombreRequerido();

            // Verificar si ya existe otra carrera activa con ese nombre (diferente al ID actual)
            if (_carreraRepository.ExisteActivaPorNombreExcluyendoId(nombreNuevo, comando.Id))
                throw new ErrorRegistroYaExiste();

            // Si existe una carrera eliminada con el mismo nombre, reactivarla y eliminar lógicamente la actual
            if (_carreraRepository.ReactivarEliminadaPorNombre(nombreNuevo))
            {
                _carreraRepository.EliminarLogicamentePorId(comando.Id);
                return;
            }
        }

        // Actualización normal
        var comandoFinal = new ActualizarCarreraComando(comando.Id, nombreNuevo);
        _carreraRepository.Actualizar(comandoFinal);
    }
    
    private void ValidarEntradaActualizacion(ActualizarCarreraComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("carrera");
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre de la carrera", 255);
    }
    public virtual void Eliminar(EliminarCarreraComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que la carrera exista y esté activa
        if (!_carreraRepository.ExisteActivaPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _carreraRepository.Eliminar(comando);
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarCarreraComando
        if (comando is EliminarCarreraComando carreraComando)
        {
            if (carreraComando.Id <= 0) throw new ErrorIdInvalido("carrera");
        }
    }    
    protected override BaseDto MapearFilaADto(DataRow fila)
    {
        return new CarreraDto
        {
            Id = Convert.ToInt32(fila["id_carrera"]),
            Nombre = fila["nombre_carrera"] == DBNull.Value ? null : fila["nombre_carrera"].ToString()
        };
    }
}
