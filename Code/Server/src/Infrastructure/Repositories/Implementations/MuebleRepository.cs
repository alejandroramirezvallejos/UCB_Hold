using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MuebleRepository : Repository<MuebleListDto>
{
	public MuebleRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

	public async Task<bool> ExisteActivoPorId(int id)
	{
		const string sql = "SELECT EXISTS(SELECT 1 FROM public.muebles WHERE id_mueble = @id AND estado_eliminado = FALSE)";
		var parameters = new Dictionary<string, object?> { ["id"] = id };
		var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
		return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
	}

	public void ActualizarNumeroGaveteros(int idMueble, int incremento)
	{
		const string sql = "UPDATE public.muebles SET numero_gaveteros = GREATEST(0, COALESCE(numero_gaveteros, 0) + @incremento) WHERE id_mueble = @id";
		var parameters = new Dictionary<string, object?> { ["id"] = idMueble, ["incremento"] = incremento };
		ExecuteQuery.EjecutarSpNR(sql, parameters);
	}

	protected override string Create()
		=> "INSERT INTO public.muebles (nombre, tipo, costo, ubicacion, longitud, profundidad, altura, estado_eliminado) VALUES (@nombre, @tipo, @costo, @ubicacion, @longitud, @profundidad, @altura, FALSE)";

	protected override string Update()
		=> "UPDATE public.muebles SET nombre = COALESCE(@nombre, nombre), tipo = COALESCE(@tipo, tipo), costo = COALESCE(@costo, costo), ubicacion = COALESCE(@ubicacion, ubicacion), longitud = COALESCE(@longitud, longitud), profundidad = COALESCE(@profundidad, profundidad), altura = COALESCE(@altura, altura) WHERE id_mueble = @id AND estado_eliminado = FALSE";

	protected override string Delete()
		=> "UPDATE public.muebles SET estado_eliminado = TRUE WHERE id_mueble = @id";

	protected override string SelectAll()
		=> "SELECT id_mueble, nombre, ubicacion FROM public.muebles WHERE estado_eliminado = FALSE";

	protected override string SelectById()
		=> "SELECT id_mueble, nombre, ubicacion FROM public.muebles WHERE id_mueble = @id AND estado_eliminado = FALSE";

	protected override MuebleListDto MapRowToDto(DataRow row) => new()
	{
		Id = Convert.ToInt32(row["id_mueble"]),
		Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
		Ubicacion = row["ubicacion"] == DBNull.Value ? null : row["ubicacion"].ToString()
	};
}

