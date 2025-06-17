using Shared.Common;

namespace Shared.Common
{
    public class ErrorEmailInvalido : DomainException
    {
        public ErrorEmailInvalido() 
            : base($"El formato del email no es válido")
        {
        }
    }
}
