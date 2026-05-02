namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;

public class EmpresaMantenimientoDetailDto
{
	public int Id { get; set; }
	public string? Nombre { get; set; }
	public string? Contacto { get; set; }
	public string? Email { get; set; }
	public string? Telefono { get; set; }
	public bool EstadoEliminado { get; set; }
}
