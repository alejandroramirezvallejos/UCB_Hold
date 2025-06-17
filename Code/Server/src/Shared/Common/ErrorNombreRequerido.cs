using Shared.Common;

namespace Shared.Common
{
    public class ErrorNombreRequerido : DomainException
    {
        public ErrorNombreRequerido() : base("El nombre es requerido")
        {
        }
    }
}
