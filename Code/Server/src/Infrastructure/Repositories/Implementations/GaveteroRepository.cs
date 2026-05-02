using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GaveteroRepository : Repository<GaveteroListDto>
{
	public GaveteroRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE id_gavetero = @id AND estado_eliminado = FALSE)";
		var parametros = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parametros);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	public async Task<bool> ExisteActivoPorNombre(string nombre)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE nombre = @nombre AND estado_eliminado = FALSE)";
		var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parametros);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	public async Task<bool> ExisteActivoPorNombreExcluyendoId(string nombre, int idExcluir)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE nombre = @nombre AND estado_eliminado = FALSE AND id_gavetero <> @idExcluir)";
		var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["idExcluir"] = idExcluir };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parametros);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	public int? ObtenerMuebleIdPorNombre(string nombreMueble)
	{
		const string sql = "SELECT id_mueble FROM public.muebles WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
		var parametros = new Dictionary<string, object?> { ["nombre"] = nombreMueble };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parametros);
		return dt?.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
	}

	public int? ObtenerMuebleIdPorGaveteroId(int gaveteroId)
	{
		const string sql = "SELECT id_mueble FROM public.gaveteros WHERE id_gavetero = @id AND estado_eliminado = FALSE";
		var parametros = new Dictionary<string, object?> { ["id"] = gaveteroId };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parametros);
		return dt?.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
	}

	protected override string Create()
		=> "INSERT INTO public.gaveteros (nombre, tipo, id_mueble, longitud, profundidad, altura, estado_eliminado) VALUES (@nombre, @tipo, @id_mueble, @longitud, @profundidad, @altura, FALSE)";

	protected override string Update()
		=> "UPDATE public.gaveteros SET nombre = COALESCE(@nombre, nombre), tipo = COALESCE(@tipo, tipo), id_mueble = COALESCE(@id_mueble, id_mueble), longitud = COALESCE(@longitud, longitud), profundidad = COALESCE(@profundidad, profundidad), altura = COALESCE(@altura, altura) WHERE id_gavetero = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.gaveteros SET estado_eliminado = TRUE WHERE id_gavetero = @id";

	protected override string SelectAll()
		=> "SELECT id_gavetero, nombre, tipo, id_mueble, longitud, profundidad, altura FROM public.gaveteros WHERE estado_eliminado = FALSE";

	protected override string SelectById()
		=> "SELECT id_gavetero, nombre, tipo, id_mueble, longitud, profundidad, altura FROM public.gaveteros WHERE id_gavetero = @id AND estado_eliminado = FALSE";

	protected override GaveteroListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_gavetero"]),
		Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
		IdMueble = Convert.ToInt32(row["id_mueble"])
	};
}

