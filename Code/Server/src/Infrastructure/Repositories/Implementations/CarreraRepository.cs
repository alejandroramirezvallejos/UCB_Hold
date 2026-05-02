using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarreraRepository : Repository<CarreraListDto>
{
	public CarreraRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.carreras WHERE id_carrera = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	public async Task<bool> ExisteActivaPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.carreras WHERE id_carrera = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	protected override string Create()
		=> "INSERT INTO public.carreras (nombre, estado_eliminado) VALUES (@nombre, FALSE)";

	protected override string Update()
		=> "UPDATE public.carreras SET nombre = COALESCE(@nombre, nombre) WHERE id_carrera = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.carreras SET estado_eliminado = TRUE WHERE id_carrera = @id";

	protected override string SelectAll()
		=> "SELECT id_carrera, nombre FROM public.carreras WHERE estado_eliminado = FALSE ORDER BY nombre ASC";

	protected override string SelectById()
		=> "SELECT id_carrera, nombre FROM public.carreras WHERE id_carrera = @id AND estado_eliminado = FALSE";

	protected override CarreraListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_carrera"]),
		Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString()
	};
}

