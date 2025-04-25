public class EmpresaMantenimiento
{
    private int    _id;
    private string _nombreEmpresa;
    private string _direccion;
    private string _nombreResponsable;
    private string _apellidoResponsable;
    private string _telefono;
    private string _nit;
    private bool   _estaEliminado;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID de la empresa debe ser un numero natural: '{value}'",
                          nameof(value));
            _id = value;
        }
    }

    public string NombreEmpresa
    {
        get => _nombreEmpresa;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre de la empresa no puede estar vacio",
                          nameof(value));
            _nombreEmpresa = value.Trim();
        }
    }

    public string Direccion
    {
        get => _direccion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La direccion de la empresa no puede estar vacia",
                          nameof(value));
            _direccion = value.Trim();
        }
    }

    public string NombreResponsable
    {
        get => _nombreResponsable;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del responsable no puede estar vacio",
                          nameof(value));
            _nombreResponsable = value.Trim();
        }
    }

    public string ApellidoResponsable
    {
        get => _apellidoResponsable;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El apellido del responsable no puede estar vacio",
                          nameof(value));
            _apellidoResponsable = value.Trim();
        }
    }

    public string Telefono
    {
        get => _telefono;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El telefono del responsable no puede estar vacio",
                          nameof(value));

            _telefono = value.Trim();
        }
    }

    public string Nit
    {
        get => _nit;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El NIT del responsable no puede estar vacio",
                          nameof(value));

            _nit = value.Trim();
        }
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public EmpresaMantenimiento(int id, string nombreEmpresa, string direccion, string nombreResponsable,
                                string apellidoResponsable, string telefono, string nit)
    {
        Id                  = id;
        NombreEmpresa       = nombreEmpresa;
        Direccion           = direccion;
        NombreResponsable   = nombreResponsable;
        ApellidoResponsable = apellidoResponsable;
        Telefono            = telefono;
        Nit                 = nit;
        EstaEliminado       = false;
    }

    public void Eliminar() => EstaEliminado = true;
}
