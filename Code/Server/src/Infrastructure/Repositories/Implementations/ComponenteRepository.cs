using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ComponenteRepository : Repository<ComponenteListDto>
{
    public ComponenteRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM public.componentes WHERE id_componente = @id AND estado_eliminado = FALSE)";
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

    protected override string CreateStatement()
        => "INSERT INTO public.componentes (nombre, modelo, tipo, id_equipo, descripcion, precio_referencia, url_data_sheet, estado_eliminado) VALUES (@nombre, @modelo, @tipo, @id_equipo, @descripcion, @precio_referencia, @url_data_sheet, FALSE)";

    protected override string UpdateStatement()
        => "UPDATE public.componentes SET nombre = COALESCE(@nombre, nombre), modelo = COALESCE(@modelo, modelo), tipo = COALESCE(@tipo, tipo), id_equipo = COALESCE(@id_equipo, id_equipo), descripcion = COALESCE(@descripcion, descripcion), precio_referencia = COALESCE(@precio_referencia, precio_referencia), url_data_sheet = COALESCE(@url_data_sheet, url_data_sheet) WHERE id_componente = @id AND estado_eliminado = FALSE";

    protected override string DeleteStatement()
        => "UPDATE public.componentes SET estado_eliminado = TRUE WHERE id_componente = @id";

    protected override string SelectAll()
        => "SELECT id_componente, nombre, modelo, tipo, descripcion, precio_referencia, id_equipo, url_data_sheet FROM public.componentes WHERE estado_eliminado = FALSE";

    protected override string SelectById()
        => "SELECT id_componente, nombre, modelo, tipo, descripcion, precio_referencia, id_equipo, url_data_sheet FROM public.componentes WHERE id_componente = @id AND estado_eliminado = FALSE";

    protected override ComponenteListDto MapRowToDto(DataRow row) => new()
    {
        Id = Convert.ToInt32(row["id_componente"]),
        Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
        Modelo = row["modelo"] == DBNull.Value ? null : row["modelo"].ToString(),
        Descripcion = row["descripcion"] == DBNull.Value ? null : row["descripcion"].ToString(),
        Precio = row["precio_referencia"] == DBNull.Value ? 0 : Convert.ToDecimal(row["precio_referencia"]),
        IdEquipo = Convert.ToInt32(row["id_equipo"])
    };
}

