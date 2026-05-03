namespace IMT_Reservas.Server.Application.Features.Categoria.Dtos;


public class CategoriaListDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
}




public class CategoriaDetailDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public bool EstadoEliminado { get; set; }
}

