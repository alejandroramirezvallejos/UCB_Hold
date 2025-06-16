using System.Data;
using System.Linq;
public class PrestamoService : IPrestamoService
{
    private readonly PrestamoRepository _prestamoRepository;

    public PrestamoService(PrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }    public void CrearPrestamo(CrearPrestamoComando comando)
    {
        try
        {
            if (comando == null)
                throw new ArgumentNullException(nameof(comando), "Los datos del préstamo son requeridos");

            if (comando.GrupoEquipoId == null || comando.GrupoEquipoId.Length == 0)
                throw new ArgumentException("Al menos un grupo de equipo es requerido", nameof(comando.GrupoEquipoId));

            if (comando.GrupoEquipoId.Any(id => id <= 0))
                throw new ArgumentException("Todos los IDs de grupo de equipo deben ser mayores a 0", nameof(comando.GrupoEquipoId));

            if (string.IsNullOrWhiteSpace(comando.CarnetUsuario))
                throw new ArgumentException("El carnet del usuario es requerido", nameof(comando.CarnetUsuario));

            if (comando.FechaDevolucionEsperada <= comando.FechaPrestamoEsperada)
                throw new ArgumentException("La fecha de devolución debe ser posterior a la fecha de préstamo", nameof(comando.FechaDevolucionEsperada));

            if (comando.FechaPrestamoEsperada < DateTime.Now.Date)
                throw new ArgumentException("La fecha de préstamo no puede ser anterior a hoy", nameof(comando.FechaPrestamoEsperada));

            _prestamoRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }
    public List<PrestamoDto>? ObtenerTodosPrestamos()
    {
        try
        {
            DataTable resultado = _prestamoRepository.ObtenerTodos();
            var lista = new List<PrestamoDto>(resultado.Rows.Count);
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
    public void EliminarPrestamo(EliminarPrestamoComando comando)
    {
        try
        {
            _prestamoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
    private static PrestamoDto MapearFilaADto(DataRow fila)
    {
        return new PrestamoDto
        {
            Id = Convert.ToInt32(fila["id_prestamo"]),
            CarnetUsuario = fila["carnet"] == DBNull.Value ? null : fila["carnet"].ToString(),
            NombreUsuario = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
            ApellidoPaternoUsuario = fila["apellido_paterno"] == DBNull.Value ? null : fila["apellido_paterno"].ToString(),
            TelefonoUsuario = fila["telefono"] == DBNull.Value ? null : fila["telefono"].ToString(),
            NombreGrupoEquipo = fila["nombre_grupo_equipo"] == DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            CodigoImt = fila["codigo_imt"] == DBNull.Value ? null : fila["codigo_imt"].ToString(),
            FechaSolicitud = fila["fecha_solicitud"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_solicitud"]),
            FechaPrestamoEsperada = fila["fecha_prestamo_esperada"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_prestamo_esperada"]),
            FechaPrestamo = fila["fecha_prestamo"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_prestamo"]),
            FechaDevolucionEsperada = fila["fecha_devolucion_esperada"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_devolucion_esperada"]),
            FechaDevolucion = fila["fecha_devolucion"] == DBNull.Value ? null : Convert.ToDateTime(fila["fecha_devolucion"]),
            Observacion = fila["observacion"] == DBNull.Value ? null : fila["observacion"].ToString(),
            EstadoPrestamo = fila["estado_prestamo"] == DBNull.Value ? null : fila["estado_prestamo"].ToString(),
        };
    }
}