using Shared.Common;

namespace Shared.Common
{
    public class ErrorCodigoImtRequerido : DomainException
    {
        public ErrorCodigoImtRequerido() :
        base("El código IMT es requerido y no puede ser menor a cero")
        {
        }
    }
}
