using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MantenimientoRepository : Repository<MantenimientoListDto>
{
	public MantenimientoRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.mantenimientos WHERE id_mantenimiento = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	protected override string Create()
		=> "INSERT INTO public.mantenimientos (fecha_mantenimiento, fecha_final_mantenimiento, id_empresa, costo, descripcion, estado_eliminado) VALUES (@fechaMantenimiento, @fechaFinalMantenimiento, @idEmpresa, @costo, @descripcion, FALSE)";

	protected override string Update()
		=> "UPDATE public.mantenimientos SET fecha_mantenimiento = COALESCE(@fechaMantenimiento, fecha_mantenimiento), fecha_final_mantenimiento = COALESCE(@fechaFinalMantenimiento, fecha_final_mantenimiento), id_empresa = COALESCE(@idEmpresa, id_empresa), costo = COALESCE(@costo, costo), descripcion = COALESCE(@descripcion, descripcion) WHERE id_mantenimiento = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.mantenimientos SET estado_eliminado = TRUE WHERE id_mantenimiento = @id";

	protected override string SelectAll()
		=> "SELECT id_mantenimiento, fecha_mantenimiento, fecha_final_mantenimiento, id_empresa, costo, descripcion FROM public.mantenimientos WHERE estado_eliminado = FALSE";

	protected override string SelectById()
		=> "SELECT id_mantenimiento, fecha_mantenimiento, fecha_final_mantenimiento, id_empresa, costo, descripcion FROM public.mantenimientos WHERE id_mantenimiento = @id AND estado_eliminado = FALSE";

	protected override MantenimientoListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_mantenimiento"]),
		FechaMantenimiento = row["fecha_mantenimiento"] == DBNull.Value ? default : Convert.ToDateTime(row["fecha_mantenimiento"]),
		FechaFinalMantenimiento = row["fecha_final_mantenimiento"] == DBNull.Value ? default : Convert.ToDateTime(row["fecha_final_mantenimiento"]),
		IdEmpresa = row["id_empresa"] == DBNull.Value ? null : Convert.ToInt32(row["id_empresa"]),
		Costo = row["costo"] == DBNull.Value ? null : Convert.ToDecimal(row["costo"]),
		Descripcion = row["descripcion"] == DBNull.Value ? null : row["descripcion"].ToString()
	};
}
