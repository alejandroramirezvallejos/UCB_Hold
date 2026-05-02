namespace IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;

public class GrupoEquipoDetailDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int IdCategoria { get; set; }
    public bool EstadoEliminado { get; set; }
}
