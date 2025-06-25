public class ComentarioDto : BaseDto
{
    public new string Id { get; set; } // PARA EL MONGO QUE USA string en el id enves de int
    public string CarnetUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string ApellidoPaternoUsuario { get; set; }
    public int IdGrupoEquipo { get; set; }
    public string Contenido { get; set; }
    public int Likes { get; set; }
    public DateTime? FechaCreacion { get; set; }
}