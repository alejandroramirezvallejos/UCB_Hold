public class EmpresaMantenimiento
{
    public int    Id                  { get; private set; }
    public string NombreEmpresa       { get; private set; }
    public string Direccion           { get; private set; }
    public string NombreResponsable   { get; private set; }
    public string ApellidoResponsable { get; private set; }
    public string Telefono            { get; private set; }
    public string Nit                 { get; private set; }
    public bool   EstaEliminado       { get; private set; }

    public EmpresaMantenimiento(string nombreEmpresa, string direccion, string nombreResponsable, 
                                string apellidoResponsable, string telefono, string nit)
    {
        NombreEmpresa       = nombreEmpresa;
        Direccion           = direccion;
        NombreResponsable   = nombreResponsable;
        ApellidoResponsable = apellidoResponsable;
        Telefono            = telefono;
        Nit                 = nit;
        EstaEliminado       = false;
    }
}
