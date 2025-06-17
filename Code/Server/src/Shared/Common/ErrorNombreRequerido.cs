using Shared.Common;

namespace Shared.Common
{
    public class ErrorNombreRequerido : DomainException
    {
        public ErrorNombreRequerido() : base("El nombre es requerido")
        {
        }

        public ErrorNombreRequerido(string campo) 
            : base($"El {campo} es requerido")
        {
        }
    }
}
