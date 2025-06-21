using Microsoft.AspNetCore.Http;

public record AceptarPrestamoComando
{
    public int PrestamoId { get; set; }
    public IFormFile Contrato { get; set; }
}
