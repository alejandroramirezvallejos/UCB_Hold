using Shared.Common;

namespace Shared.Common
{
    public class ErrorModeloRequerido : DomainException
    {
        public ErrorModeloRequerido() : base("El modelo es requerido")
        {
        }
    }
}
