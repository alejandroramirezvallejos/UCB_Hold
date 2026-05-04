namespace IMT_Reservas.Server.Application.Features.Prestamo.Dtos;

public class ContratoPrestamoDto
{
    public int PrestamoId { get; set; }
    public IFormFile Contrato { get; set; } = null!;
}
