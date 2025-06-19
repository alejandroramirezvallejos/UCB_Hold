public class NotificacionDto
{
    public string    Id            { get; set; }
    public string    CarnetUsuario { get; set; } 
    public string    Titulo        { get; set; } = null!;
    public string    Contenido     { get; set; } = null!;
    public DateTime? FechaEnvio    { get; set; } = null;
}