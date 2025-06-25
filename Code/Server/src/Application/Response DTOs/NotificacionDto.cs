public class NotificacionDto : BaseDto
{
    public new string Id { get; set; } // PARA EL MONGO QUE USE STRING ID ENVES DE INT
    public string CarnetUsuario { get; set; }
    public string Titulo { get; set; }
    public string Contenido { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public bool Leido { get; set; }
}