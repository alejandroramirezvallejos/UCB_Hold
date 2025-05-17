public class ReservasError: Exception
{
    public ReservasError(string mensaje)
    {
        throw new Exception(mensaje);
    } 
}