using System.Data;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public PrestamoRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearPrestamoComando comando)//TODO: Array
    {
        const string sql = @"
            CALL public.insertar_prestamo(
	        @GrupoEquipoId,
	        @fechaPrestamoEsperada,
	        @fechaDevolucionEsperada,
	        @observacion,
	        @carnetUsuario,
	        @contrato)";

        var parametros = new Dictionary<string, object?>
        {
            ["GrupoEquipoId"]          = comando.GrupoEquipoId ?? (object)DBNull.Value,
            ["fechaPrestamoEsperada"]   = comando.FechaPrestamoEsperada,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada ,
            ["observacion"]             = comando.Observacion ?? (object)DBNull.Value,
            ["carnetUsuario"]           = comando.CarnetUsuario ?? (object)DBNull.Value,
            ["contrato"]                = comando.Contrato ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el préstamo", ex);
        }
    }
    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_prestamo(
	    @id
        )";
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
            {
                ["id"] = id
            });
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el préstamo", ex);
        }
    }

    public List<PrestamoDto> ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_prestamos()
        ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            var lista = new List<PrestamoDto>(dt.Rows.Count);
            foreach (DataRow fila in dt.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los préstamos", ex);
        }
    }
    private static PrestamoDto MapearFilaADto(DataRow fila)
    {
        return new PrestamoDto
        {
            CarnetUsuario           = fila["carnet"]==DBNull.Value ? null : fila["carnet"].ToString(),
            NombreUsuario           = fila["nombre"]==DBNull.Value ? null : fila["nombre"].ToString(),
            ApellidoPaternoUsuario  = fila["apellido_paterno"]==DBNull.Value ? null : fila["apellido_paterno"].ToString(),
            TelefonoUsuario         = fila["telefono"]==DBNull.Value ? null : fila["telefono"].ToString(),
            NombreGrupoEquipo       = fila["nombre_grupo_equipo"]==DBNull.Value ? null : fila["nombre_grupo_equipo"].ToString(),
            CodigoImt               = fila["codigo_imt"]==DBNull.Value ? null : fila["codigo_imt"].ToString(),
            FechaSolicitud          = fila["fecha_solicitud"]==DBNull.Value ? null : Convert.ToDateTime(fila["fecha_solicitud"]),
            FechaPrestamoEsperada   = fila["fecha_prestamo_esperada"]==DBNull.Value ? null : Convert.ToDateTime(fila["fecha_prestamo_esperada"]),
            FechaPrestamo           = fila["fecha_prestamo"]==DBNull.Value ? null : Convert.ToDateTime(fila["fecha_prestamo"]),
            FechaDevolucionEsperada = fila["fecha_devolucion_esperada"]==DBNull.Value ? null : Convert.ToDateTime(fila["fecha_devolucion_esperada"]),
            FechaDevolucion         = fila["fecha_devolucion"]==DBNull.Value ? null : Convert.ToDateTime(fila["fecha_devolucion"]),
            Observacion             = fila["observacion"]==DBNull.Value ? null : fila["observacion"].ToString(),
            EstadoPrestamo          = fila["estado_prestamo"]==DBNull.Value ? null : fila["estado_prestamo"].ToString(),
        };
    }
}