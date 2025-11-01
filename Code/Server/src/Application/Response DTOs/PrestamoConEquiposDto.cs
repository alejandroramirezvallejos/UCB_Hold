namespace IMT_Reservas.Server.Application.ResponseDTOs;

public class PrestamoConEquiposDto
{
    public int IdPrestamo { get; set; }
    public List<EquipoAsignadoDto> EquiposAsignados { get; set; } = new();
}
