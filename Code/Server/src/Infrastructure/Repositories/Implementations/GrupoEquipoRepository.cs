using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GrupoEquipoRepository : Repository<GrupoEquipoListDto>
{
	public GrupoEquipoRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.grupos_equipos WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	protected override string Create()
		=> "INSERT INTO public.grupos_equipos (id_categoria, nombre, modelo, marca, cantidad, estado_eliminado) VALUES (@idCategoria, @nombre, @modelo, @marca, @cantidad, FALSE)";

	protected override string Update()
		=> "UPDATE public.grupos_equipos SET id_categoria = COALESCE(@idCategoria, id_categoria), nombre = COALESCE(@nombre, nombre), modelo = COALESCE(@modelo, modelo), marca = COALESCE(@marca, marca), cantidad = COALESCE(@cantidad, cantidad) WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.grupos_equipos SET estado_eliminado = TRUE WHERE id_grupo_equipo = @id";

	protected override string SelectAll()
		=> "SELECT id_grupo_equipo, id_categoria, nombre, modelo, marca, cantidad FROM public.grupos_equipos WHERE estado_eliminado = FALSE";

	protected override string SelectById()
		=> "SELECT id_grupo_equipo, id_categoria, nombre, modelo, marca, cantidad FROM public.grupos_equipos WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE";

	protected override GrupoEquipoListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_grupo_equipo"]),
		IdCategoria = Convert.ToInt32(row["id_categoria"]),
		Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
		Modelo = row["modelo"] == DBNull.Value ? null : row["modelo"].ToString(),
		Marca = row["marca"] == DBNull.Value ? null : row["marca"].ToString(),
		Cantidad = row["cantidad"] == DBNull.Value ? null : Convert.ToInt32(row["cantidad"])
	};
}
