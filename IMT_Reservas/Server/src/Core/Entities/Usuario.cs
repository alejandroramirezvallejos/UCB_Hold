public class Usuario : IUsuario, IEliminacionLogica
{
    private string  _carnet             = string.Empty;
    private string  _nombre             = string.Empty;
    private string  _apellidoPaterno    = string.Empty;
    private string  _apellidoMaterno    = string.Empty;
    private string  _rol                = string.Empty;
    private int     _carreraId;
    private string  _contrasena         = string.Empty;
    private string  _email              = string.Empty;
    private string  _telefono           = string.Empty;
    private string? _nombreReferencia   = null;
    private string? _telefonoReferencia = null;
    private string? _emailReferencia    = null;
    private bool    _estaEliminado      = false;
    public string Carnet
    {
        get => _carnet;
        private set => _carnet = Verificar.SiEsVacio(value, "El carnet del usuario");
    }

    public string Nombre
    {
        get => _nombre;
        private set => _nombre = Verificar.SiEsVacio(value, "El nombre del usuario");
    }

    public string ApellidoPaterno
    {
        get => _apellidoPaterno;
        private set => _apellidoPaterno = Verificar.SiEsVacio(value, "El apellido paterno del usuario");
    }

    public string ApellidoMaterno
    {
        get => _apellidoMaterno;
        private set => _apellidoMaterno = Verificar.SiEsVacio(value, "El apellido materno del usuario");
    }

    public string Rol
    {
        get => _rol;
        private set
        {
            Enum enumVal = Verificar.SiEstaEnEnum<TipoDeUsuario>(value, "El rol del usuario");
            _rol = enumVal.ToString();
        }
    }

    public int CarreraId
    {
        get => _carreraId;
        private set => _carreraId = Verificar.SiEsNatural(value, "El ID de la carrera del usuario");
    }

    public string Contrasena
    {
        get => _contrasena;
        private set => _contrasena = Verificar.SiEsVacio(value, "La contraseña del usuario");
    }

    public string Email
    {
        get => _email;
        private set => _email = Verificar.SiEsVacio(value, "El email del usuario");
    }

    public string Telefono
    {
        get => _telefono;
        private set => _telefono = Verificar.SiEsVacio(value, "El telefono del usuario");
    }

    public string? NombreReferencia
    {
        get => _nombreReferencia;
        private set => _nombreReferencia = value is not null
                       ? Verificar.SiEsVacio(value, "El nombre de referencia")
                       : null;
    }

    public string? TelefonoReferencia
    {
        get => _telefonoReferencia;
        private set => _telefonoReferencia = value is not null
                       ? Verificar.SiEsVacio(value, "El telefono de referencia")
                       : null;
    }

    public string? EmailReferencia
    {
        get => _emailReferencia;
        private set => _emailReferencia = value is not null
                       ? Verificar.SiEsVacio(value, "El email de referencia")
                       : null;
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Usuario(string carnet,string nombre,string apellidoPaterno, string apellidoMaterno, 
        string rol, int carreraId, string contrasena, string email, string telefono,
        string? nombreReferencia, string? telefonoReferencia, string? emailReferencia)
    {
        Carnet             = carnet;
        Nombre             = nombre;
        ApellidoPaterno    = apellidoPaterno;
        ApellidoMaterno    = apellidoMaterno;
        Rol                = rol;
        CarreraId          = carreraId;
        Contrasena         = contrasena;
        Email              = email;
        Telefono           = telefono;
        NombreReferencia   = nombreReferencia;
        TelefonoReferencia = telefonoReferencia;
        EmailReferencia    = emailReferencia;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
