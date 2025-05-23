public interface IEmpresaMantenimiento
{
    int     Id                  { get; }
    string  NombreEmpresa       { get; }
    string? Direccion           { get; }
    string? NombreResponsable   { get; }
    string? ApellidoResponsable { get; }
    string? Telefono            { get; }
    string? Nit                 { get; }
}
 