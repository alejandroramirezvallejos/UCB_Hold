using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GrupoEquipoRepository : Repository<GrupoEquipoListDto>
{
    public GrupoEquipoRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM public.grupos_equipos WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE)";
        var parameters = new Dictionary<string, object?> { ["id"] = id };
        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    protected override string CreateStatement()
        => "INSERT INTO public.grupos_equipos (id_categoria, nombre, modelo, marca, cantidad, estado_eliminado) VALUES (@idCategoria, @nombre, @modelo, @marca, @cantidad, FALSE)";

    protected override string UpdateStatement()
        => "UPDATE public.grupos_equipos SET id_categoria = COALESCE(@idCategoria, id_categoria), nombre = COALESCE(@nombre, nombre), modelo = COALESCE(@modelo, modelo), marca = COALESCE(@marca, marca), cantidad = COALESCE(@cantidad, cantidad) WHERE id_grupo_equipo = @id AND estado_eliminado = FALSE";

    protected override string DeleteStatement()
        => "UPDATE public.grupos_equipos SET estado_eliminado = TRUE WHERE id_grupo_equipo = @id";

    protected override string SelectAll()
        => @"SELECT ge.id_grupo_equipo, ge.id_categoria, ge.nombre, ge.modelo, ge.marca, ge.cantidad,
		        ge.descripcion, ge.url_data_sheet, ge.link, c.nombre as nombre_categoria,
		        COALESCE(AVG(e.costo_referencia), 0) as costo_promedio
		     FROM public.grupos_equipos ge
		     LEFT JOIN public.categorias c ON ge.id_categoria = c.id_categoria
		     LEFT JOIN public.equipos e ON ge.id_grupo_equipo = e.id_grupo_equipo AND e.estado_eliminado = FALSE
		     WHERE ge.estado_eliminado = FALSE
		     GROUP BY ge.id_grupo_equipo, ge.id_categoria, ge.nombre, ge.modelo, ge.marca, ge.cantidad,
		              ge.descripcion, ge.url_data_sheet, ge.link, c.nombre";

    protected override string SelectById()
        => @"SELECT ge.id_grupo_equipo, ge.id_categoria, ge.nombre, ge.modelo, ge.marca, ge.cantidad,
		        ge.descripcion, ge.url_data_sheet, ge.link, c.nombre as nombre_categoria,
		        COALESCE(AVG(e.costo_referencia), 0) as costo_promedio
		     FROM public.grupos_equipos ge
		     LEFT JOIN public.categorias c ON ge.id_categoria = c.id_categoria
		     LEFT JOIN public.equipos e ON ge.id_grupo_equipo = e.id_grupo_equipo AND e.estado_eliminado = FALSE
		     WHERE ge.id_grupo_equipo = @id AND ge.estado_eliminado = FALSE
		     GROUP BY ge.id_grupo_equipo, ge.id_categoria, ge.nombre, ge.modelo, ge.marca, ge.cantidad,
		              ge.descripcion, ge.url_data_sheet, ge.link, c.nombre";

    protected override GrupoEquipoListDto MapRowToDto(DataRow row) => new()
    {
        id = Convert.ToInt32(row["id_grupo_equipo"]),
        IdCategoria = row["id_categoria"] == DBNull.Value ? null : Convert.ToInt32(row["id_categoria"]),
        nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
        modelo = row["modelo"] == DBNull.Value ? null : row["modelo"].ToString(),
        marca = row["marca"] == DBNull.Value ? null : row["marca"].ToString(),
        Cantidad = row["cantidad"] == DBNull.Value ? null : Convert.ToInt32(row["cantidad"]),
        descripcion = row["descripcion"] == DBNull.Value ? null : row["descripcion"].ToString(),
        url_data_sheet = row["url_data_sheet"] == DBNull.Value ? null : row["url_data_sheet"].ToString(),
        link = row["link"] == DBNull.Value ? null : row["link"].ToString(),
        nombreCategoria = row["nombre_categoria"] == DBNull.Value ? null : row["nombre_categoria"].ToString(),
        CostoPromedio = row["costo_promedio"] == DBNull.Value ? null : Convert.ToDecimal(row["costo_promedio"])
    };
}
