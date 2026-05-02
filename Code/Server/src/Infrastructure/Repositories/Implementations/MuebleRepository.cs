using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MuebleRepository : Repository<MuebleListDto>
{
    public MuebleRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

    public bool ExisteActivoPorId(int id)
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

    protected override string CreateStatement()
        => "INSERT INTO public.muebles (nombre, tipo, costo, ubicacion, longitud, profundidad, altura, estado_eliminado) VALUES (@nombre, @tipo, @costo, @ubicacion, @longitud, @profundidad, @altura, FALSE)";

    protected override string UpdateStatement()
        => "UPDATE public.muebles SET nombre = COALESCE(@nombre, nombre), tipo = COALESCE(@tipo, tipo), costo = COALESCE(@costo, costo), ubicacion = COALESCE(@ubicacion, ubicacion), longitud = COALESCE(@longitud, longitud), profundidad = COALESCE(@profundidad, profundidad), altura = COALESCE(@altura, altura) WHERE id_mueble = @id AND estado_eliminado = FALSE";

    protected override string DeleteStatement()
        => "UPDATE public.muebles SET estado_eliminado = TRUE WHERE id_mueble = @id";

    protected override string SelectAll()
        => "SELECT id_mueble, nombre, ubicacion, numero_gaveteros, tipo, costo, longitud, profundidad, altura FROM public.muebles WHERE estado_eliminado = FALSE";

    protected override string SelectById()
        => "SELECT id_mueble, nombre, ubicacion, numero_gaveteros, tipo, costo, longitud, profundidad, altura FROM public.muebles WHERE id_mueble = @id AND estado_eliminado = FALSE";

    protected override MuebleListDto MapRowToDto(DataRow row) => new()
    {
        Id = Convert.ToInt32(row["id_mueble"]),
        Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
        Ubicacion = row["ubicacion"] == DBNull.Value ? null : row["ubicacion"].ToString(),
        NumeroGaveteros = row["numero_gaveteros"] == DBNull.Value ? null : Convert.ToInt32(row["numero_gaveteros"]),
        Tipo = row["tipo"] == DBNull.Value ? null : row["tipo"].ToString(),
        Costo = row["costo"] == DBNull.Value ? null : Convert.ToDecimal(row["costo"]),
        Longitud = row["longitud"] == DBNull.Value ? null : Convert.ToDecimal(row["longitud"]),
        Profundidad = row["profundidad"] == DBNull.Value ? null : Convert.ToDecimal(row["profundidad"]),
        Altura = row["altura"] == DBNull.Value ? null : Convert.ToDecimal(row["altura"])
    };
}

