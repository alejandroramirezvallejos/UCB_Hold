namespace IMT_Reservas.Server.Core.Entities;

public static class EstadoPrestamoParse
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

    public static bool TryParse(string? value, out EstadoPrestamo estado)
    {
        estado = EstadoPrestamo.Pendiente;
    
        if (string.IsNullOrWhiteSpace(value))
            return false;

        switch (value.Trim().ToLowerInvariant())
        {
            case "pendiente":
                estado = EstadoPrestamo.Pendiente;
                return true;
            case "aprobado":
                estado = EstadoPrestamo.Aprobado;
                return true;
            case "activo":
                estado = EstadoPrestamo.Activo;
                return true;
            case "rechazado":
                estado = EstadoPrestamo.Rechazado;
                return true;
            case "finalizado":
                estado = EstadoPrestamo.Finalizado;
                return true;
            case "cancelado":
                estado = EstadoPrestamo.Cancelado;
                return true;
            default:
                return false;
        }
    }

    public static EstadoPrestamo ParseOrDefault(string? value, EstadoPrestamo fallback)
        => TryParse(value, out var estado) ? estado : fallback;
}
