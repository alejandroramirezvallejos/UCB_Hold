namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class ContratoPrestamoDto
{
    public int PrestamoId { get; set; }
    public IFormFile Contrato { get; set; } = null!;
}
