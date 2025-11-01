namespace IMT_Reservas.Server.Application.ResponseDTOs;

public class DisponibilidadEquipoDto
{
    public DateTime Fecha { get; set; }
    public int IdGrupoEquipo { get; set; }
    public long CantidadDisponible { get; set; }
}
