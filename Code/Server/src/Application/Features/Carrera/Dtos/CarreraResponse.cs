namespace IMT_Reservas.Server.Application.Features.Carrera.Dtos;

public class CarreraListDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
}

public class CarreraDetailDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public bool EstadoEliminado { get; set; }
}

