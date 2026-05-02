using System.Data;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.MongoDb;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class PrestamoRepository : Repository<PrestamoListDto>
{
	private readonly MongoDbContexto _mongoDbContext;

	public PrestamoRepository(ExecuteQuery executeQuery, MongoDbContexto mongoDbContext)
		: base(executeQuery)
	{
		_mongoDbContext = mongoDbContext;
	}

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.prestamos WHERE id_prestamo = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	protected override string Create()
		=> "INSERT INTO public.prestamos (fecha_prestamo_esperada, fecha_devolucion_esperada, observacion, carnet, estado_eliminado) VALUES (@fechaPrestamoEsperada, @fechaDevolucionEsperada, @observacion, @carnetUsuario, FALSE)";

	protected override string Update()
		=> "UPDATE public.prestamos SET fecha_devolucion_esperada = COALESCE(@fechaDevolucionEsperada, fecha_devolucion_esperada), observacion = COALESCE(@observacion, observacion) WHERE id_prestamo = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.prestamos SET estado_eliminado = TRUE WHERE id_prestamo = @id";

	protected override string SelectAll()
		=> @"SELECT DISTINCT ON (p.id_prestamo) p.id_prestamo, p.carnet, p.estado_prestamo, p.fecha_solicitud,
		        e.id_grupo_equipo, p.fecha_devolucion_esperada,
		        u.nombre as nombre_usuario, u.apellido_paterno as apellido_paterno_usuario, u.telefono as telefono_usuario,
		        ge.nombre as nombre_grupo_equipo, e.codigo_imt, p.fecha_prestamo_esperada, p.fecha_prestamo, p.fecha_devolucion, p.observacion,
		        e.ubicacion as ubicacion_equipo, gv.nombre as nombre_gavetero, m.nombre as nombre_mueble, m.ubicacion as ubicacion_mueble,
		        p.id_contrato, NULL::integer as file_id
		     FROM public.prestamos p
		     LEFT JOIN public.usuarios u ON p.carnet = u.carnet
		     LEFT JOIN public.detalles_prestamos dp ON p.id_prestamo = dp.id_prestamo AND dp.estado_eliminado = FALSE
		     LEFT JOIN public.equipos e ON dp.id_equipo = e.id_equipo AND e.estado_eliminado = FALSE
		     LEFT JOIN public.grupos_equipos ge ON e.id_grupo_equipo = ge.id_grupo_equipo
		     LEFT JOIN public.gaveteros gv ON e.id_gavetero = gv.id_gavetero
		     LEFT JOIN public.muebles m ON gv.id_mueble = m.id_mueble
		     WHERE p.estado_eliminado = FALSE
		     ORDER BY p.id_prestamo";

	protected override string SelectById()
		=> @"SELECT DISTINCT ON (p.id_prestamo) p.id_prestamo, p.carnet, p.estado_prestamo, p.fecha_solicitud,
		        e.id_grupo_equipo, p.fecha_devolucion_esperada,
		        u.nombre as nombre_usuario, u.apellido_paterno as apellido_paterno_usuario, u.telefono as telefono_usuario,
		        ge.nombre as nombre_grupo_equipo, e.codigo_imt, p.fecha_prestamo_esperada, p.fecha_prestamo, p.fecha_devolucion, p.observacion,
		        e.ubicacion as ubicacion_equipo, gv.nombre as nombre_gavetero, m.nombre as nombre_mueble, m.ubicacion as ubicacion_mueble,
		        p.id_contrato, NULL::integer as file_id
		     FROM public.prestamos p
		     LEFT JOIN public.usuarios u ON p.carnet = u.carnet
		     LEFT JOIN public.detalles_prestamos dp ON p.id_prestamo = dp.id_prestamo AND dp.estado_eliminado = FALSE
		     LEFT JOIN public.equipos e ON dp.id_equipo = e.id_equipo AND e.estado_eliminado = FALSE
		     LEFT JOIN public.grupos_equipos ge ON e.id_grupo_equipo = ge.id_grupo_equipo
		     LEFT JOIN public.gaveteros gv ON e.id_gavetero = gv.id_gavetero
		     LEFT JOIN public.muebles m ON gv.id_mueble = m.id_mueble
		     WHERE p.id_prestamo = @id AND p.estado_eliminado = FALSE
		     ORDER BY p.id_prestamo";

	protected override PrestamoListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_prestamo"]),
		CarnetUsuario = row["carnet"] == DBNull.Value ? "" : row["carnet"].ToString() ?? "",
		EstadoPrestamo = row["estado_prestamo"] == DBNull.Value ? null : row["estado_prestamo"].ToString(),
		FechaSolicitud = row["fecha_solicitud"] == DBNull.Value ? default : Convert.ToDateTime(row["fecha_solicitud"]),
		IdGrupoEquipo = row["id_grupo_equipo"] == DBNull.Value ? null : Convert.ToInt32(row["id_grupo_equipo"]),
		FechaDevolucionEsperada = row["fecha_devolucion_esperada"] == DBNull.Value ? null : Convert.ToDateTime(row["fecha_devolucion_esperada"]),
		NombreUsuario = row["nombre_usuario"] == DBNull.Value ? null : row["nombre_usuario"].ToString(),
		ApellidoPaternoUsuario = row["apellido_paterno_usuario"] == DBNull.Value ? null : row["apellido_paterno_usuario"].ToString(),
		TelefonoUsuario = row["telefono_usuario"] == DBNull.Value ? null : row["telefono_usuario"].ToString(),
		NombreGrupoEquipo = row["nombre_grupo_equipo"] == DBNull.Value ? null : row["nombre_grupo_equipo"].ToString(),
		CodigoImtEquipo = row["codigo_imt"] == DBNull.Value ? null : Convert.ToInt32(row["codigo_imt"]),
		FechaPrestamoEsperada = row["fecha_prestamo_esperada"] == DBNull.Value ? null : Convert.ToDateTime(row["fecha_prestamo_esperada"]),
		FechaPrestamo = row["fecha_prestamo"] == DBNull.Value ? null : Convert.ToDateTime(row["fecha_prestamo"]),
		FechaDevolucion = row["fecha_devolucion"] == DBNull.Value ? null : Convert.ToDateTime(row["fecha_devolucion"]),
		Observacion = row["observacion"] == DBNull.Value ? null : row["observacion"].ToString(),
		Ubicacion_Equipo = row["ubicacion_equipo"] == DBNull.Value ? null : row["ubicacion_equipo"].ToString(),
		Nombre_Gavetero = row["nombre_gavetero"] == DBNull.Value ? null : row["nombre_gavetero"].ToString(),
		Nombre_Mueble = row["nombre_mueble"] == DBNull.Value ? null : row["nombre_mueble"].ToString(),
		Ubicacion_Mueble = row["ubicacion_mueble"] == DBNull.Value ? null : row["ubicacion_mueble"].ToString(),
		IdContrato = row["id_contrato"] == DBNull.Value ? null : Convert.ToInt32(row["id_contrato"]),
		FileId = row["file_id"] == DBNull.Value ? null : Convert.ToInt32(row["file_id"])
	};
}
