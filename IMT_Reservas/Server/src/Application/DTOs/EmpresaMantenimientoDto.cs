public class EmpresaMantenimientoDto
{
    public int     Id                  { get; set; }
    public string  NombreEmpresa       { get; set; } = string.Empty;
    public string? Direccion           { get; set; } = null;
    public string? NombreResponsable   { get; set; } = null;
    public string? ApellidoResponsable { get; set; } = null;
    public string? Telefono            { get; set; } = null;
    public string? Nit                 { get; set; } = null;
    public bool    EstaEliminado       { get; set; } = false;
}