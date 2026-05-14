using IMT_Reservas.Server.Core.Entities;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public static class EstadoPrestamoState
{
    private static readonly Dictionary<EstadoPrestamo, EstadoPrestamo[]> _transitions = new()
    {
        [EstadoPrestamo.Pendiente]  = [EstadoPrestamo.Aprobado, EstadoPrestamo.Rechazado, EstadoPrestamo.Cancelado],
        [EstadoPrestamo.Aprobado]   = [EstadoPrestamo.Activo, EstadoPrestamo.Cancelado],
        [EstadoPrestamo.Activo]     = [EstadoPrestamo.Finalizado, EstadoPrestamo.Cancelado],
        [EstadoPrestamo.Rechazado]  = [],
        [EstadoPrestamo.Finalizado] = [],
        [EstadoPrestamo.Cancelado]  = []
    };

    public static bool CanTransition(EstadoPrestamo current, EstadoPrestamo next)
        => _transitions.TryGetValue(current, out var validNext) && validNext.Contains(next);

    public static EstadoPrestamo? Parse(string? estado) => estado?.ToLowerInvariant() switch
    {
        "pendiente"  => EstadoPrestamo.Pendiente,
        "aprobado"   => EstadoPrestamo.Aprobado,
        "activo"     => EstadoPrestamo.Activo,
        "rechazado"  => EstadoPrestamo.Rechazado,
        "finalizado" => EstadoPrestamo.Finalizado,
        "cancelado"  => EstadoPrestamo.Cancelado,
        _ => null
    };

    public static string ToText(EstadoPrestamo estado) => estado.ToString().ToLowerInvariant();
}
