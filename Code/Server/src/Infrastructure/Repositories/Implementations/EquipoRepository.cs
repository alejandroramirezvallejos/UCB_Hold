using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EquipoRepository : Repository<EquipoListDto>
{
	public EquipoRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.equipos WHERE id_equipo = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	protected override string Create()
		=> "INSERT INTO public.equipos (id_grupo_equipo, codigo_imt, descripcion, numero_serial, ubicacion, costo_referencia, tiempo_max_prestamo, procedencia, id_gavetero, estado_eliminado, codigo_ucb) VALUES (@idGrupoEquipo, @codigoImt, @descripcion, @numeroSerial, @ubicacion, @costoReferencia, @tiempoMaxPrestamo, @procedencia, @idGavetero, FALSE, @codigoUcb)";

	protected override string Update()
		=> "UPDATE public.equipos SET id_grupo_equipo = COALESCE(@idGrupoEquipo, id_grupo_equipo), descripcion = COALESCE(@descripcion, descripcion), numero_serial = COALESCE(@numeroSerial, numero_serial), ubicacion = COALESCE(@ubicacion, ubicacion), costo_referencia = COALESCE(@costoReferencia, costo_referencia), tiempo_max_prestamo = COALESCE(@tiempoMaxPrestamo, tiempo_max_prestamo), procedencia = COALESCE(@procedencia, procedencia), id_gavetero = COALESCE(@idGavetero, id_gavetero), estado_equipo = COALESCE(@estadoEquipo::estado_equipo, estado_equipo), codigo_ucb = COALESCE(@codigoUcb, codigo_ucb) WHERE id_equipo = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.equipos SET estado_eliminado = TRUE WHERE id_equipo = @id";

	protected override string SelectAll()
		=> "SELECT id_equipo, id_grupo_equipo, codigo_imt, descripcion, numero_serial, ubicacion, costo_referencia, tiempo_max_prestamo, procedencia, id_gavetero, estado_equipo, codigo_ucb FROM public.equipos WHERE estado_eliminado = FALSE";

	protected override string SelectById()
		=> "SELECT id_equipo, id_grupo_equipo, codigo_imt, descripcion, numero_serial, ubicacion, costo_referencia, tiempo_max_prestamo, procedencia, id_gavetero, estado_equipo, codigo_ucb FROM public.equipos WHERE id_equipo = @id AND estado_eliminado = FALSE";

	protected override EquipoListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_equipo"]),
		IdGrupoEquipo = Convert.ToInt32(row["id_grupo_equipo"]),
		CodigoImt = row["codigo_imt"] == DBNull.Value ? null : Convert.ToInt32(row["codigo_imt"]),
		Descripcion = row["descripcion"] == DBNull.Value ? null : row["descripcion"].ToString(),
		NumeroSerial = row["numero_serial"] == DBNull.Value ? null : row["numero_serial"].ToString(),
		Ubicacion = row["ubicacion"] == DBNull.Value ? null : row["ubicacion"].ToString(),
		CostoReferencia = row["costo_referencia"] == DBNull.Value ? null : Convert.ToDecimal(row["costo_referencia"]),
		TiempoMaxPrestamo = row["tiempo_max_prestamo"] == DBNull.Value ? null : Convert.ToInt32(row["tiempo_max_prestamo"]),
		Procedencia = row["procedencia"] == DBNull.Value ? null : row["procedencia"].ToString(),
		IdGavetero = row["id_gavetero"] == DBNull.Value ? null : Convert.ToInt32(row["id_gavetero"]),
		EstadoEquipo = row["estado_equipo"] == DBNull.Value ? null : row["estado_equipo"].ToString(),
		CodigoUcb = row["codigo_ucb"] == DBNull.Value ? null : row["codigo_ucb"].ToString()
	};
}
