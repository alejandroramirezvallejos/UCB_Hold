using System.Data;
public class EquipoService : IEquipoService
{
    private readonly EquipoRepository _equipoRepository;

    public EquipoService(EquipoRepository equipoRepository)
    {
        _equipoRepository = equipoRepository;
    }
    public void CrearEquipo(CrearEquipoComando comando)
    {
        try
        {
            _equipoRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public void ActualizarEquipo(ActualizarEquipoComando comando)
    {
        try
        {
            _equipoRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarEquipo(EliminarEquipoComando comando)
    {
        try
        {
            _equipoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    public List<EquipoDto>? ObtenerTodosEquipos()
    {
        try
        {
            DataTable resultado = _equipoRepository.ObtenerTodos();
            var lista = new List<EquipoDto>(resultado.Rows.Count);
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
    private static EquipoDto MapearFilaADto(DataRow fila)
    {
        return new EquipoDto
        {
            Id = Convert.ToInt32(fila["id_equipo"]),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            CodigoImt = fila["codigo_imt_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["codigo_imt_equipo"]),
            CodigoUcb = fila["codigo_ucb_equipo"] == DBNull.Value ? null : fila["codigo_ucb_equipo"].ToString(),
            Descripcion = fila["descripcion_equipo"] == DBNull.Value ? null : fila["descripcion_equipo"].ToString(),
            NumeroSerial = fila["numero_serial_equipo"] == DBNull.Value ? null : fila["numero_serial_equipo"].ToString(),
            Ubicacion = fila["ubicacion_equipo"] == DBNull.Value ? null : fila["ubicacion_equipo"].ToString(),
            Procedencia = fila["procedencia_equipo"] == DBNull.Value ? null : fila["procedencia_equipo"].ToString(),
            TiempoMaximoPrestamo = fila["tiempo_max_prestamo_equipo"] == DBNull.Value ? null : Convert.ToInt32(fila["tiempo_max_prestamo_equipo"]),
            NombreGavetero = fila["nombre_gavetero_equipo"] == DBNull.Value ? null : fila["nombre_gavetero_equipo"].ToString(),
            EstadoEquipo = fila["estado_equipo_equipo"] == DBNull.Value ? null : fila["estado_equipo_equipo"].ToString(),
            CostoReferencia = fila["costo_referencia_equipo"] == DBNull.Value ? null : Convert.ToDouble(fila["costo_referencia_equipo"]),
        };
    }
}