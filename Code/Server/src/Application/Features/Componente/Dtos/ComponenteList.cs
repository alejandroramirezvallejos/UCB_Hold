namespace IMT_Reservas.Server.Application.Features.Componente.Dtos;

public class ComponenteList
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Precio { get; set; }
    public int? IdEquipo { get; set; }
}
