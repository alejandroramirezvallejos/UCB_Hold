namespace IMT_Reservas.Server.Application.Commands.Equipo;

public class ObtenerDisponibilidadEquiposComando
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int[]? ArrayIds { get; set; } // Array de IDs de grupos equipos
}
