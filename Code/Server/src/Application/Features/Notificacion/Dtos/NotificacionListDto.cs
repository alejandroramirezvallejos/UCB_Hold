namespace IMT_Reservas.Server.Application.Features.Notificacion.Dtos;

public class NotificacionListDto
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public string? Titulo { get; set; }
    public bool EsLeido { get; set; }
    public DateTime FechaCreacion { get; set; }
}
