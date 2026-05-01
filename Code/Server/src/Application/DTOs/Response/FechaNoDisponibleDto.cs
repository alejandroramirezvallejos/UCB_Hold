namespace IMT_Reservas.Server.Application.DTOs.Response;

public class FechaNoDisponibleDto
{
    public int IdGrupoEquipo { get; set; }
    public DateTime FechaNoDisponible { get; set; }
    public int CantidadDisponible { get; set; }
}
