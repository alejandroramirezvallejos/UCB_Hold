public class EmpresaMantenimiento : IEmpresaMantenimiento, IEliminacionLogica
{
    private int     _id;
    private string  _nombreEmpresa       = string.Empty;
    private string? _direccion           = null;
    private string? _nombreResponsable   = null;
    private string? _apellidoResponsable = null;
    private string? _telefono            = null;
    private string? _nit                 = null;
    private bool    _estaEliminado       = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID de la empresa");
    }

    public string NombreEmpresa
    {
        get => _nombreEmpresa;
        private set => _nombreEmpresa = Verificar.SiEsVacio(value, "El nombre de la empresa");
    }

    public string? Direccion
    {
        get => _direccion;
        private set => _direccion = value is not null
                       ? Verificar.SiEsVacio(value, "La direccion de la empresa")
                       : null;
    }

    public string? NombreResponsable
    {
        get => _nombreResponsable;
        private set => _nombreResponsable = value is not null
                       ? Verificar.SiEsVacio(value, "El nombre del responsable")
                       : null;
    }

    public string? ApellidoResponsable
    {
        get => _apellidoResponsable;
        private set => _apellidoResponsable = value is not null
                       ? Verificar.SiEsVacio(value, "El apellido del responsable")
                       : null;
    }

    public string? Telefono
    {
        get => _telefono;
        private set => _telefono = value is not null
                      ? Verificar.SiEsVacio(value, "El telefono del responsable")
                      : null;
    }

    public string? Nit
    {
        get => _nit;
        private set => _nit = value is not null
                       ? Verificar.SiEsVacio(value, "El NIT de la empresa")
                       : null;
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public EmpresaMantenimiento(string nombreEmpresa, string? direccion, string? nombreResponsable,
                                string? apellidoResponsable, string? telefono, string? nit)
    {
        NombreEmpresa       = nombreEmpresa;
        Direccion           = direccion;
        NombreResponsable   = nombreResponsable;
        ApellidoResponsable = apellidoResponsable;
        Telefono            = telefono;
        Nit                 = nit;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
