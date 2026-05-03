namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;

public class EmpresaMantenimientoDetail
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? NombreResponsable { get; set; }
    public string? ApellidoResponsable { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public bool EstadoEliminado { get; set; }
}
