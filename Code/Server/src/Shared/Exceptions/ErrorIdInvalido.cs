namespace IMT_Reservas.Server.Shared.Common
{
    public class ErrorIdInvalido : Exception
    {
        public ErrorIdInvalido(string entidad) : base($"El ID {entidad} es requerido y debe ser mayor a 0")
        {
        }
    }
}
