using System.Data;
using IMT_Reservas.Server.Shared.Common;
public class MuebleService : BaseServicios,
    ICrearServicio<CrearMuebleComando>,
    IActualizarServicio<ActualizarMuebleComando>,
    IEliminarServicio<EliminarMuebleComando>,
    IObtenerTodosServicio<MuebleDto>
{
    private readonly MuebleRepository _muebleRepository;

    public MuebleService(MuebleRepository muebleRepository)
    {
        _muebleRepository = muebleRepository;
    }

    public virtual void Crear(CrearMuebleComando comando)
    {
        ValidarEntradaCreacion(comando);
        _muebleRepository.Crear(comando);
    }
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando);
        if (comando is CrearMuebleComando cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.Nombre)) throw new ErrorNombreRequerido();
            if (cmd.Costo.HasValue && cmd.Costo < 0) throw new ErrorValorNegativo("costo");
            if (cmd.Longitud.HasValue && cmd.Longitud <= 0) throw new ErrorValorNegativo("longitud");
            if (cmd.Profundidad.HasValue && cmd.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
            if (cmd.Altura.HasValue && cmd.Altura <= 0) throw new ErrorValorNegativo("altura");
        }
    }
    public virtual void Actualizar(ActualizarMuebleComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que el mueble exista y esté activo
        if (!_muebleRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _muebleRepository.Actualizar(comando);
    }
    private void ValidarEntradaActualizacion(ActualizarMuebleComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
        if (comando.Id <= 0) throw new ErrorIdInvalido("mueble");
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre mueble", 255);
        if (comando.Costo.HasValue && comando.Costo < 0) throw new ErrorValorNegativo("costo");
        if (comando.Longitud.HasValue && comando.Longitud <= 0) throw new ErrorValorNegativo("longitud");
        if (comando.Profundidad.HasValue && comando.Profundidad <= 0) throw new ErrorValorNegativo("profundidad");
        if (comando.Altura.HasValue && comando.Altura <= 0) throw new ErrorValorNegativo("altura");
    }
    public virtual void Eliminar(EliminarMuebleComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el mueble exista y esté activo
        if (!_muebleRepository.ExisteActivoPorId(comando.Id))
            throw new ErrorRegistroNoEncontrado();

        _muebleRepository.Eliminar(comando);
    }
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando);
        if (comando is EliminarMuebleComando cmd)
        {
            if (cmd.Id <= 0) throw new ErrorIdInvalido("mueble");
        }
    }
    public virtual List<MuebleDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _muebleRepository.ObtenerTodos();
            var lista = new List<MuebleDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as MuebleDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    protected override BaseDto MapearFilaADto(DataRow fila)
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
    }
}