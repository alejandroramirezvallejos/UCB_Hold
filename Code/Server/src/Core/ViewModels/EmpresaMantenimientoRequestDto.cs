namespace API.ViewModels;

public class EmpresaMantenimientoRequestDto
{
    public int?     Id                  { get; set; }= null;
    public string? NombreEmpresa        { get; set; } = null;
    public string? NombreResponsable   { get; set; }= null;
    public string? ApellidoResponsable { get; set; }= null;
    public string? Telefono            { get; set; }= null;
    public string? Nit                 { get; set; }= null;
    public string? Direccion           { get; set; }= null;
}