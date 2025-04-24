public enum TipoUsuario { Docente, Administrador, Estudiante } 

public class Usuario //TODO: Convertir a clase abstracta
{
    public string      Carnet             { get; private set; }
    public string      Nombre             { get; private set; }
    public string      ApellidoPaterno    { get; private set; }
    public string      ApellidoMaterno    { get; private set; }
    public TipoUsuario Rol                { get; private set; }
    public int         CarreraId          { get; private set; }
    public Carrera     Carrera            { get; private set; }
    public string      Contrasena         { get; private set; }
    public string      Email              { get; private set; }
    public string      Telefono           { get; private set; }
    public string      NombreReferencia   { get; private set; }
    public string      TelefonoReferencia { get; private set; }
    public string      EmailReferencia    { get; private set; }
    public bool        EstaEliminado      { get; private set; }

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
