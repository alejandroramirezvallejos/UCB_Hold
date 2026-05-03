namespace IMT_Reservas.Server.Application.Features.Contrato.Dtos;

public class ContratoUploadRequest
{
    public int PrestamoId { get; set; }
    public IFormFile? Archivo { get; set; }
}
