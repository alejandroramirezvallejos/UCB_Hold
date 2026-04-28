using System.Data;
using IMT_Reservas.Server.Shared.Common;

public class GaveteroService : BaseServicios,
    ICrearServicio<CrearGaveteroComando>,
    IActualizarServicio<ActualizarGaveteroComando>,
    IEliminarServicio<EliminarGaveteroComando>,
    IObtenerTodosServicio<GaveteroDto>
{
    private readonly GaveteroRepository _gaveteroRepository;
    private readonly MuebleRepository _muebleRepository;

    public GaveteroService(GaveteroRepository gaveteroRepository, MuebleRepository muebleRepository)
    {
        _gaveteroRepository = gaveteroRepository;
        _muebleRepository = muebleRepository;
    }

    public virtual void Crear(CrearGaveteroComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Resolver FK: nombre del mueble → id_mueble
        var idMueble = _gaveteroRepository.ObtenerMuebleIdPorNombre(comando.NombreMueble!);
        if (idMueble == null)
            throw new ErrorMuebleNoEncontrado();

        // Verificar si ya existe un gavetero activo con ese nombre
        if (_gaveteroRepository.ExisteActivoPorNombre(comando.Nombre!))
            throw new ErrorRegistroYaExiste();

        // Insertar gavetero
        _gaveteroRepository.Crear(idMueble.Value, comando);

        // Trigger logic: incrementar numero_gaveteros del mueble
        _muebleRepository.ActualizarNumeroGaveteros(idMueble.Value, 1);
    }
    
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando); // Validación base (null check)
        
        // Validaciones específicas para CrearGaveteroComando
        if (comando is CrearGaveteroComando gaveteroComando)
        {
            if (string.IsNullOrWhiteSpace(gaveteroComando.Nombre)) throw new ErrorNombreRequerido();
            if (string.IsNullOrWhiteSpace(gaveteroComando.NombreMueble)) throw new ErrorNombreMuebleRequerido();
            if (gaveteroComando.Longitud.HasValue && gaveteroComando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
            if (gaveteroComando.Profundidad.HasValue && gaveteroComando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
            if (gaveteroComando.Altura.HasValue && gaveteroComando.Altura <= 0) throw new ErrorValorNegativo("altura");
        }
    }

    public virtual void Actualizar(ActualizarGaveteroComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que el gavetero exista y esté activo
        if (!_gaveteroRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        // Verificar duplicados de nombre
        if (!string.IsNullOrWhiteSpace(comando.Nombre))
        {
            if (_gaveteroRepository.ExisteActivoPorNombreExcluyendoId(comando.Nombre, comando.Id))
                throw new ErrorRegistroYaExiste();
        }

        int? nuevoIdMueble = null;
        int? viejoIdMueble = null;

        // Resolver FK del mueble si se está cambiando
        if (!string.IsNullOrWhiteSpace(comando.NombreMueble))
        {
            nuevoIdMueble = _gaveteroRepository.ObtenerMuebleIdPorNombre(comando.NombreMueble);
            if (nuevoIdMueble == null)
                throw new ErrorMuebleNoEncontrado();

            // Obtener el mueble actual del gavetero para trigger logic
            viejoIdMueble = _gaveteroRepository.ObtenerMuebleIdPorGaveteroId(comando.Id);
        }

        _gaveteroRepository.Actualizar(nuevoIdMueble, comando);

        // Trigger logic: si cambió de mueble, ajustar conteos
        if (nuevoIdMueble.HasValue && viejoIdMueble.HasValue && nuevoIdMueble.Value != viejoIdMueble.Value)
        {
            _muebleRepository.ActualizarNumeroGaveteros(viejoIdMueble.Value, -1);
            _muebleRepository.ActualizarNumeroGaveteros(nuevoIdMueble.Value, 1);
        }
    }
    
    private void ValidarEntradaActualizacion(ActualizarGaveteroComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("gavetero");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre gavetero", 255);
        if (!string.IsNullOrWhiteSpace(comando.NombreMueble) && comando.NombreMueble.Length > 255) throw new ErrorLongitudInvalida("nombre mueble", 255);
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }

    public virtual void Eliminar(EliminarGaveteroComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el gavetero exista y esté activo
        if (!_gaveteroRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        // Obtener el mueble actual para trigger logic
        var idMueble = _gaveteroRepository.ObtenerMuebleIdPorGaveteroId(comando.Id);

        _gaveteroRepository.Eliminar(comando);

        // Trigger logic: decrementar numero_gaveteros del mueble
        if (idMueble.HasValue)
            _muebleRepository.ActualizarNumeroGaveteros(idMueble.Value, -1);
    }
    
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando); // Validación base (null check)
        
        // Validaciones específicas para EliminarGaveteroComando
        if (comando is EliminarGaveteroComando gaveteroComando)
        {
            if (gaveteroComando.Id <= 0) throw new ErrorIdInvalido("gavetero");
        }
    }

    public virtual List<GaveteroDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _gaveteroRepository.ObtenerTodos();
            var lista = new List<GaveteroDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as GaveteroDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    
    protected override BaseDto MapearFilaADto(DataRow fila)
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