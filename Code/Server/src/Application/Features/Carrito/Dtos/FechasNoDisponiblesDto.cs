namespace IMT_Reservas.Server.Application.Features.Carrito.Dtos;

public class FechasNoDisponiblesDto
{
    public int IdGrupoEquipo { get; set; }
    public DateTime FechaNoDisponible { get; set; }
    public int CantidadDisponible { get; set; }
}
