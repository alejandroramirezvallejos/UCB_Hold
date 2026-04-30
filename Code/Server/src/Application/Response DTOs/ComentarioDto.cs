public class ComentarioDto : Dto
{
    public new string Id { get; set; } = null!;
    public string? CarnetUsuario { get; set; }
    public string? NombreUsuario { get; set; }
    public string? ApellidoPaternoUsuario { get; set; }
    public int IdGrupoEquipo { get; set; }
    public string? Contenido { get; set; }
    public int Likes { get; set; }
    public DateTime? FechaCreacion { get; set; }
}