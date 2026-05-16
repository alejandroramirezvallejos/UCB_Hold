using IMT_Reservas.Server.Core.Entities;
namespace IMT_Reservas.Server.Application.Features.Prestamo.State;

public static class PrestamoState
{
    private static readonly Dictionary<EstadoPrestamo, EstadoPrestamoBase> _states = new()
    {
        [EstadoPrestamo.Pendiente]  = new EstadoPendiente(),
        [EstadoPrestamo.Aprobado]   = new EstadoAprobado(),
        [EstadoPrestamo.Activo]     = new EstadoActivo(),
        [EstadoPrestamo.Rechazado]  = new EstadoRechazado(),
        [EstadoPrestamo.Finalizado] = new EstadoFinalizado(),
        [EstadoPrestamo.Cancelado]  = new EstadoCancelado()
    };

    private static readonly Dictionary<string, EstadoPrestamo> _parseMap =
        _states.ToDictionary(kv => kv.Value.Nombre, kv => kv.Key);

    public static EstadoPrestamoBase GetState(EstadoPrestamo estado) => _states[estado];

    public static bool CanTransition(EstadoPrestamo current, EstadoPrestamo next)
        => _states.TryGetValue(current, out var state) && state.CanTransitionTo(next);

    public static EstadoPrestamo? Parse(string? estado)
        => estado != null && _parseMap.TryGetValue(estado.ToLowerInvariant(), out var e) ? e : null;

    public static string ToText(EstadoPrestamo estado) => _states[estado].Nombre;
}
