namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;

// Create/Update Request DTO for EmpresaMantenimiento


public class EmpresaMantenimientoDto
{
    public string? Nombre { get; set; }
    public string? NombreResponsable { get; set; }
    public string? ApellidoResponsable { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Nit { get; set; }
}

