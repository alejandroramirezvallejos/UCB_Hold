using System.Data;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class AccesorioRepository : Repository<AccesorioListDto>
{
	public AccesorioRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.accesorios WHERE id_accesorio = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	public int? ObtenerEquipoIdPorCodigoImt(int codigoImt)
	{
		const string sql = "SELECT id_equipo FROM public.equipos WHERE codigo_imt = @codigoImt AND estado_eliminado = FALSE LIMIT 1";
		var parameters = new Dictionary<string, object?> { ["codigoImt"] = codigoImt };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
	}

	protected override string Create()
		=> "INSERT INTO public.accesorios (id_equipo, nombre, modelo, tipo, precio, estado_eliminado) VALUES (@idEquipo, @nombre, @modelo, @tipo, @precio, FALSE)";

	protected override string Update()
		=> "UPDATE public.accesorios SET id_equipo = COALESCE(@idEquipo, id_equipo), nombre = COALESCE(@nombre, nombre), modelo = COALESCE(@modelo, modelo), tipo = COALESCE(@tipo, tipo), precio = COALESCE(@precio, precio) WHERE id_accesorio = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.accesorios SET estado_eliminado = TRUE WHERE id_accesorio = @id";

	protected override string SelectAll()
		=> "SELECT id_accesorio, nombre, modelo, tipo, precio FROM public.accesorios WHERE estado_eliminado = FALSE";

	protected override string SelectById()
		=> "SELECT id_accesorio, nombre, modelo, tipo, precio FROM public.accesorios WHERE id_accesorio = @id AND estado_eliminado = FALSE";

	protected override AccesorioListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_accesorio"]),
		Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
		Modelo = row["modelo"] == DBNull.Value ? null : row["modelo"].ToString(),
		Tipo = row["tipo"] == DBNull.Value ? null : row["tipo"].ToString(),
		Precio = row["precio"] == DBNull.Value ? null : Convert.ToDecimal(row["precio"])
	};
}
