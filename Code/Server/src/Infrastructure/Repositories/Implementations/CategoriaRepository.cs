using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CategoriaRepository : Repository<CategoriaListDto>
{
    public CategoriaRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM public.categorias WHERE id_categoria = @id AND estado_eliminado = FALSE)";
        var parameters = new Dictionary<string, object?> { ["id"] = id };
        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    protected override string CreateStatement()
        => "INSERT INTO public.categorias (nombre, estado_eliminado) VALUES (@nombre, FALSE)";

    protected override string UpdateStatement()
        => "UPDATE public.categorias SET nombre = COALESCE(@nombre, nombre) WHERE id_categoria = @id AND estado_eliminado = FALSE";

    protected override string DeleteStatement()
        => "UPDATE public.categorias SET estado_eliminado = TRUE WHERE id_categoria = @id";

    protected override string SelectAll()
        => "SELECT id_categoria, nombre FROM public.categorias WHERE estado_eliminado = FALSE ORDER BY nombre ASC";

    protected override string SelectById()
        => "SELECT id_categoria, nombre FROM public.categorias WHERE id_categoria = @id AND estado_eliminado = FALSE";

    protected override CategoriaListDto MapRowToDto(DataRow row) => new()
    {
        Id = Convert.ToInt32(row["id_categoria"]),
        Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString()
    };
}

