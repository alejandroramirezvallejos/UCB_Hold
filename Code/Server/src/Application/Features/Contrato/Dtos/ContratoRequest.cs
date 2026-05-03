namespace IMT_Reservas.Server.Application.Features.Contrato.Dtos;

public class ContratoRequest
{
    public int PrestamoId { get; set; }
    public IFormFile? Archivo { get; set; }
}
