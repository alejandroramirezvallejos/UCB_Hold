using Shared.Common;

namespace Shared.Common
{
    public class ErrorFechaInvalida : DomainException
    {
        public ErrorFechaInvalida() 
            : base($"La fecha es inválida")
        {
        }
    }
}
