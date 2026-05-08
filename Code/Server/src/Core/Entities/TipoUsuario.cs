using NpgsqlTypes;
namespace IMT_Reservas.Server.Core.Entities;

public enum TipoUsuario
{
    [PgName("docente")]
    Docente,
    [PgName("administrador")]
    Administrador,
    [PgName("estudiante")]
    Estudiante
}
