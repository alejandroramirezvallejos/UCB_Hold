using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using System.Data;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EmpresaMantenimientoRepository : Repository<EmpresaMantenimientoListDto>
{
    public EmpresaMantenimientoRepository(ExecuteQuery executeQuery) : base(executeQuery) { }

    public bool ExisteActivoPorId(int id)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM public.empresas_mantenimiento WHERE id_empresa_mantenimiento = @id AND estado_eliminado = FALSE)";
        var parameters = new Dictionary<string, object?> { ["id"] = id };
        var dt = ExecuteQuery.EjecutarFuncion(sql, parameters);
        return dt?.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    protected override string CreateStatement()
        => "INSERT INTO public.empresas_mantenimiento (nombre, contacto, email, telefono, estado_eliminado) VALUES (@nombre, @contacto, @email, @telefono, FALSE)";

    protected override string UpdateStatement()
        => "UPDATE public.empresas_mantenimiento SET nombre = COALESCE(@nombre, nombre), contacto = COALESCE(@contacto, contacto), email = COALESCE(@email, email), telefono = COALESCE(@telefono, telefono) WHERE id_empresa_mantenimiento = @id AND estado_eliminado = FALSE";

    protected override string DeleteStatement()
        => "UPDATE public.empresas_mantenimiento SET estado_eliminado = TRUE WHERE id_empresa_mantenimiento = @id";

    protected override string SelectAll()
        => "SELECT id_empresa_mantenimiento, nombre, contacto, email, telefono, apellido_responsable, nit, direccion FROM public.empresas_mantenimiento WHERE estado_eliminado = FALSE";

    protected override string SelectById()
        => "SELECT id_empresa_mantenimiento, nombre, contacto, email, telefono, apellido_responsable, nit, direccion FROM public.empresas_mantenimiento WHERE id_empresa_mantenimiento = @id AND estado_eliminado = FALSE";

    protected override EmpresaMantenimientoListDto MapRowToDto(DataRow row) => new()
    {
        Id = Convert.ToInt32(row["id_empresa_mantenimiento"]),
        NombreEmpresa = row["nombre"] == DBNull.Value ? null : row["nombre"].ToString(),
        NombreResponsable = row["contacto"] == DBNull.Value ? null : row["contacto"].ToString(),
        ApellidoResponsable = row["apellido_responsable"] == DBNull.Value ? null : row["apellido_responsable"].ToString(),
        Telefono = row["telefono"] == DBNull.Value ? null : row["telefono"].ToString(),
        Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
        Nit = row["nit"] == DBNull.Value ? null : row["nit"].ToString(),
        Direccion = row["direccion"] == DBNull.Value ? null : row["direccion"].ToString()
    };
}

