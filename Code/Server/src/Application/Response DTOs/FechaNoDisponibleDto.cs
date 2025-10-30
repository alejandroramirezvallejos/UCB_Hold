namespace IMT_Reservas.Server.Application.ResponseDTOs;

public class FechaNoDisponibleDto
{
    public int IdGrupoEquipo { get; set; }
    public DateTime FechaNoDisponible { get; set; }
    public int CantidadDisponible { get; set; }
}
