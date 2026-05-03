namespace IMT_Reservas.Server.Application.Features.Carrito.Dtos;

public class DisponibilidadEquipoDto
{
    public int IdGrupoEquipo { get; set; }
    public DateTime Fecha { get; set; }
    public int CantidadDisponible { get; set; }
}

public class FechaNoDisponibleDto
{
    public int IdGrupoEquipo { get; set; }
    public DateTime FechaNoDisponible { get; set; }
    public int CantidadDisponible { get; set; }
}
