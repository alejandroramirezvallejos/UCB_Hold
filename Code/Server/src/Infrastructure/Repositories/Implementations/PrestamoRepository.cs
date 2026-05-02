using System.Data;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class PrestamoRepository : Repository<PrestamoListDto>
{
	private readonly MongoDbContexto _mongoDbContext;
	private readonly IGridFSBucket _gridFsBucket;

	public PrestamoRepository(ExecuteQuery executeQuery, MongoDbContexto mongoDbContext, IGridFSBucket gridFsBucket)
		: base(executeQuery)
	{
		_mongoDbContext = mongoDbContext;
		_gridFsBucket = gridFsBucket;
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
		=> "SELECT id_prestamo, carnet, estado_prestamo, fecha_solicitud, id_grupo_equipo, fecha_devolucion_esperada FROM public.prestamos WHERE estado_eliminado = FALSE";

	protected override string SelectById()
		=> "SELECT id_prestamo, carnet, estado_prestamo, fecha_solicitud, id_grupo_equipo, fecha_devolucion_esperada FROM public.prestamos WHERE id_prestamo = @id AND estado_eliminado = FALSE";

	protected override PrestamoListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_prestamo"]),
		CarnetUsuario = row["carnet"].ToString() ?? "",
		EstadoPrestamo = row["estado_prestamo"] == DBNull.Value ? null : row["estado_prestamo"].ToString(),
		FechaSolicitud = row["fecha_solicitud"] == DBNull.Value ? default : Convert.ToDateTime(row["fecha_solicitud"]),
		IdGrupoEquipo = row["id_grupo_equipo"] == DBNull.Value ? null : Convert.ToInt32(row["id_grupo_equipo"]),
		FechaDevolucionEsperada = row["fecha_devolucion_esperada"] == DBNull.Value ? null : Convert.ToDateTime(row["fecha_devolucion_esperada"])
	};
}
