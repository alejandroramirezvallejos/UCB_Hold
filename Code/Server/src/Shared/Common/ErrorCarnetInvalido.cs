using Shared.Common;

namespace Shared.Common
{
    public class ErrorCarnetInvalido : DomainException
    {
        public ErrorCarnetInvalido() 
            : base($"El carnet no es válido")
        {
        }
    }
}
