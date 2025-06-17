using Shared.Common;

namespace Shared.Common
{
    public class ErrorRegistroNoEncontrado : DomainException
    {
        public ErrorRegistroNoEncontrado() 
            : base($"No se encontró el registro especificado")
        {
        }
    }
}
