using NpgsqlTypes;
namespace IMT_Reservas.Server.Core.Entities;

public enum EstadoPrestamo
{
    [PgName("pendiente")]
    Pendiente,
    [PgName("aprobado")]
    Aprobado,
    [PgName("activo")]
    Activo,
    [PgName("rechazado")]
    Rechazado,
    [PgName("finalizado")]
    Finalizado,
    [PgName("cancelado")]
    Cancelado
}

public static class EstadoPrestamoExtensions
{
    public static string ToDbString(this EstadoPrestamo estado) => estado switch
    {
        EstadoPrestamo.Pendiente => "pendiente",
        EstadoPrestamo.Aprobado => "aprobado",
        EstadoPrestamo.Activo => "activo",
        EstadoPrestamo.Rechazado => "rechazado",
        EstadoPrestamo.Finalizado => "finalizado",
        EstadoPrestamo.Cancelado => "cancelado",
        _ => "pendiente"
    };
}
