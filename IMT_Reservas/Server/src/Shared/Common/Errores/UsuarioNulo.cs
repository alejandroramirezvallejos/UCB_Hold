public class UsuarioNuloException: ReservasError
{
    public UsuarioNuloException(string mensaje):base(mensaje)
    {
        throw new Exception(mensaje);
    }
}