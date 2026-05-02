using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class UsuarioRepository : Repository<UsuarioListDto>
{
    public UsuarioRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM public.usuarios WHERE carnet = @id AND estado_eliminado = FALSE)";
        var parameters = new Dictionary<string, object?> { ["id"] = id };
        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerCarreraIdPorNombre(string nombreCarrera)
    {
        const string sql = "SELECT id_carrera FROM public.carreras WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parameters = new Dictionary<string, object?> { ["nombre"] = nombreCarrera };
        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
    }

    protected override string CreateStatement()
        => "INSERT INTO public.usuarios (carnet, nombre, apellido_paterno, apellido_materno, rol, email, contrasena, id_carrera, telefono, telefono_referencia, nombre_referencia, email_referencia, estado_eliminado) VALUES (@carnet, @nombre, @apellido_paterno, @apellido_materno, @rol, @email, @contrasena, @id_carrera, @telefono, @telefono_referencia, @nombre_referencia, @email_referencia, FALSE)";

    protected override string UpdateStatement()
        => "UPDATE public.usuarios SET nombre = COALESCE(@nombre, nombre), apellido_paterno = COALESCE(@apellido_paterno, apellido_paterno), apellido_materno = COALESCE(@apellido_materno, apellido_materno), rol = COALESCE(@rol, rol), email = COALESCE(@email, email), contrasena = COALESCE(@contrasena, contrasena), id_carrera = COALESCE(@id_carrera, id_carrera), telefono = COALESCE(@telefono, telefono), telefono_referencia = COALESCE(@telefono_referencia, telefono_referencia), nombre_referencia = COALESCE(@nombre_referencia, nombre_referencia), email_referencia = COALESCE(@email_referencia, email_referencia) WHERE carnet = @id AND estado_eliminado = FALSE";

    protected override string DeleteStatement()
        => "UPDATE public.usuarios SET estado_eliminado = TRUE WHERE carnet = @id";

    protected override string SelectAll()
        => "SELECT carnet, nombre, apellido_paterno, apellido_materno, rol, email, id_carrera, telefono, telefono_referencia, nombre_referencia, email_referencia FROM public.usuarios WHERE estado_eliminado = FALSE";

    protected override string SelectById()
        => "SELECT carnet, nombre, apellido_paterno, apellido_materno, rol, email, id_carrera, telefono, telefono_referencia, nombre_referencia, email_referencia FROM public.usuarios WHERE carnet = @id AND estado_eliminado = FALSE";

    protected override UsuarioListDto MapRowToDto(DataRow row) => new()
    {
        Id = 0,
        Carnet = row["carnet"].ToString() ?? "",
        Nombre = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
        ApellidoPaterno = row["apellido_paterno"] == DBNull.Value ? null : row["apellido_paterno"].ToString(),
        ApellidoMaterno = row["apellido_materno"] == DBNull.Value ? null : row["apellido_materno"].ToString(),
        Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
        IdCarrera = row["id_carrera"] == DBNull.Value ? null : Convert.ToInt32(row["id_carrera"]),
        Rol = row["rol"] == DBNull.Value ? null : row["rol"].ToString(),
        Telefono = row["telefono"] == DBNull.Value ? null : row["telefono"].ToString(),
        TelefonoReferencia = row["telefono_referencia"] == DBNull.Value ? null : row["telefono_referencia"].ToString(),
        NombreReferencia = row["nombre_referencia"] == DBNull.Value ? null : row["nombre_referencia"].ToString(),
        EmailReferencia = row["email_referencia"] == DBNull.Value ? null : row["email_referencia"].ToString()
    };
}

