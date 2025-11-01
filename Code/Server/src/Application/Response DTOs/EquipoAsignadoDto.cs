namespace IMT_Reservas.Server.Application.ResponseDTOs;

public class EquipoAsignadoDto
{
    public int IdEquipo { get; set; }
    public string? CodigoImt { get; set; }
    public string? CodigoSerial { get; set; }
    public string? Nombre { get; set; }
    public string? Modelo { get; set; }
    public string? Marca { get; set; }
    public int IdGrupoEquipo { get; set; }
}
