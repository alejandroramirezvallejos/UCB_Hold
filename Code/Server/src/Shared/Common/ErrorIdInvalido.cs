using Shared.Common;

namespace Shared.Common
{
    public class ErrorIdInvalido : DomainException
    {
        public ErrorIdInvalido() 
            : base("El ID debe ser mayor a 0")
        {
        }
        
        public ErrorIdInvalido(string entidad) 
            : base($"El ID de {entidad} debe ser mayor a 0")
        {
        }
    }
}
