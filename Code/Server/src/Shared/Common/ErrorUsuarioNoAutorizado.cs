namespace IMT_Reservas.Server.Shared.Common;

public class ErrorUsuarioNoAutorizado : Exception
{
    public ErrorUsuarioNoAutorizado() : base("El usuario no está autorizado para realizar esta acción")
    {
    }

    public ErrorUsuarioNoAutorizado(string mensaje) : base(mensaje)
    {
    }
}
