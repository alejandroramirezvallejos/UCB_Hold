namespace IMT_Reservas.Server.Shared.Common
{
    public class ErrorIdInvalido : Exception
    {
        public ErrorIdInvalido(string message = "El ID es requerido y debe ser mayor a 0") : base(message)
        {
        }
    }
}
