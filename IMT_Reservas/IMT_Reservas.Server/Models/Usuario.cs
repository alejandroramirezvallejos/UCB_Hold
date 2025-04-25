using System.ComponentModel;

public enum TipoUsuario
{
    [Description("Docente")]
    Docente,
    [Description("Administrador")]
    Administrador,
    [Description("Estudiante")]
    Estudiante
}

public class Usuario
{
    private string      _carnet;
    private string      _nombre;
    private string      _apellidoPaterno;
    private string      _apellidoMaterno;
    private TipoUsuario _rol;
    private int         _carreraId;
    private string      _contrasena;
    private string      _email;
    private string      _telefono;
    private string      _nombreReferencia;
    private string      _telefonoReferencia;
    private string      _emailReferencia;
    private bool        _estaEliminado;

    public string Carnet
    {
        get => _carnet;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El carnet del usuario no puede estar vacio",
                          nameof(value));
            _carnet = value.Trim();
        }
    }

    public string Nombre
    {
        get => _nombre;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del usuario no puede estar vacio", 
                          nameof(value));
            _nombre = value.Trim();
        }
    }

    public string ApellidoPaterno
    {
        get => _apellidoPaterno;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El apellido paterno del usuario no puede estar vacio", 
                          nameof(value));
            _apellidoPaterno = value.Trim();
        }
    }

    public string ApellidoMaterno
    {
        get => _apellidoMaterno;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El apellido materno del usuario no puede estar vacio",
                          nameof(value));
            _apellidoMaterno = value.Trim();
        }
    }

    public TipoUsuario Rol
    {
        get => _rol;
        private set
        {
            if (!Enum.IsDefined(typeof(TipoUsuario), value))
                throw new ArgumentException($"El rol de usuario es invalido: '{value}'", 
                          nameof(value));
            _rol = value;
        }
    }

    public int CarreraId
    {
        get => _carreraId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID de carrera debe ser un numero natural: '{value}'",
                          nameof(value));
            _carreraId = value;
        }
    }

    public Carrera Carrera { get; private set; }

    public string Contrasena
    {
        get => _contrasena;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La contraseña del usuario no puede estar vacia", 
                          nameof(value));
            _contrasena = value;
        }
    }

    public string Email
    {
        get => _email;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El email del usuario no puede estar vacio", 
                          nameof(value));
            _email = value.Trim();
        }
    }

    public string Telefono
    {
        get => _telefono;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El telefono del usuario no puede estar vacio", 
                          nameof(value));
            _telefono = value.Trim();
        }
    }

    public string NombreReferencia
    {
        get => _nombreReferencia;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre de referencia no puede estar vacio",
                          nameof(value));
            _nombreReferencia = value.Trim();
        }
    }

    public string TelefonoReferencia
    {
        get => _telefonoReferencia;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El telefono de referencia no puede estar vacio",
                          nameof(value));
            _telefonoReferencia = value.Trim();
        }
    }

    public string EmailReferencia
    {
        get => _emailReferencia;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El email del usuario de referencia no puede estar vacio",
                          nameof(value));
            _emailReferencia = value.Trim();
        }
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Usuario(string carnet, string nombre, string apellidoPaterno, string apellidoMaterno,
                   TipoUsuario rol, int carreraId, string contrasena, string email, string telefono,
                   string nombreReferencia, string telefonoReferencia, string emailReferencia)
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
        EstaEliminado      = false;
    }
}
