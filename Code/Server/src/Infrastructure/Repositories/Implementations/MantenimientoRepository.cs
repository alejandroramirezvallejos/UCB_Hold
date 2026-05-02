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
		=> @"SELECT DISTINCT ON (m.id_mantenimiento) m.id_mantenimiento, m.fecha_mantenimiento, m.fecha_final_mantenimiento, m.id_empresa, m.costo, m.descripcion,
		        em.nombre as nombre_empresa_mantenimiento, dm.tipo_mantenimiento,
		        ge.nombre as nombre_grupo_equipo, e.codigo_imt, e.descripcion as descripcion_equipo
		     FROM public.mantenimientos m
		     LEFT JOIN public.empresas_mantenimiento em ON m.id_empresa = em.id_empresa_mantenimiento
		     LEFT JOIN public.detalles_mantenimientos dm ON m.id_mantenimiento = dm.id_mantenimiento AND dm.estado_eliminado = FALSE
		     LEFT JOIN public.equipos e ON dm.id_equipo = e.id_equipo AND e.estado_eliminado = FALSE
		     LEFT JOIN public.grupos_equipos ge ON e.id_grupo_equipo = ge.id_grupo_equipo
		     WHERE m.estado_eliminado = FALSE
		     ORDER BY m.id_mantenimiento";

	protected override string SelectById()
		=> @"SELECT DISTINCT ON (m.id_mantenimiento) m.id_mantenimiento, m.fecha_mantenimiento, m.fecha_final_mantenimiento, m.id_empresa, m.costo, m.descripcion,
		        em.nombre as nombre_empresa_mantenimiento, dm.tipo_mantenimiento,
		        ge.nombre as nombre_grupo_equipo, e.codigo_imt, e.descripcion as descripcion_equipo
		     FROM public.mantenimientos m
		     LEFT JOIN public.empresas_mantenimiento em ON m.id_empresa = em.id_empresa_mantenimiento
		     LEFT JOIN public.detalles_mantenimientos dm ON m.id_mantenimiento = dm.id_mantenimiento AND dm.estado_eliminado = FALSE
		     LEFT JOIN public.equipos e ON dm.id_equipo = e.id_equipo AND e.estado_eliminado = FALSE
		     LEFT JOIN public.grupos_equipos ge ON e.id_grupo_equipo = ge.id_grupo_equipo
		     WHERE m.id_mantenimiento = @id AND m.estado_eliminado = FALSE
		     ORDER BY m.id_mantenimiento";

	protected override MantenimientoListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_mantenimiento"]),
		FechaMantenimiento = row["fecha_mantenimiento"] == DBNull.Value ? default : Convert.ToDateTime(row["fecha_mantenimiento"]),
		FechaFinalDeMantenimiento = row["fecha_final_mantenimiento"] == DBNull.Value ? default : Convert.ToDateTime(row["fecha_final_mantenimiento"]),
		IdEmpresa = row["id_empresa"] == DBNull.Value ? null : Convert.ToInt32(row["id_empresa"]),
		Costo = row["costo"] == DBNull.Value ? null : Convert.ToDecimal(row["costo"]),
		Descripcion = row["descripcion"] == DBNull.Value ? null : row["descripcion"].ToString(),
		NombreEmpresaMantenimiento = row["nombre_empresa_mantenimiento"] == DBNull.Value ? null : row["nombre_empresa_mantenimiento"].ToString(),
		TipoMantenimiento = row["tipo_mantenimiento"] == DBNull.Value ? null : row["tipo_mantenimiento"].ToString(),
		NombreGrupoEquipo = row["nombre_grupo_equipo"] == DBNull.Value ? null : row["nombre_grupo_equipo"].ToString(),
		CodigoImtEquipo = row["codigo_imt"] == DBNull.Value ? null : Convert.ToInt32(row["codigo_imt"]),
		DescripcionEquipo = row["descripcion_equipo"] == DBNull.Value ? null : row["descripcion_equipo"].ToString()
	};
}
