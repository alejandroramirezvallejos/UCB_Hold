public class ComentarioDto
{
    public string Id { get; set; }
    public string CarnetUsuario { get; set; }
    public string NombreUsuario { get; set; }
    public string ApellidoPaternoUsuario { get; set; }
    public int IdGrupoEquipo { get; set; }
    public string Contenido { get; set; }
    public int Likes { get; set; }
    public DateTime? FechaCreacion { get; set; }
}