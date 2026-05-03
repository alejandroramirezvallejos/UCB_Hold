namespace IMT_Reservas.Server.Application.Features.Carrito.Dtos;

public class CarritoDto
{
    public int? Id { get; set; }
    public string? NombreGrupoEquipo { get; set; }
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public int? Cantidad { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFinal { get; set; }
}
