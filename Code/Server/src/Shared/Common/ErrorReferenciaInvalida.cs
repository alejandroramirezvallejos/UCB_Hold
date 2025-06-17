using Shared.Common;

namespace Shared.Common
{
    public class ErrorReferenciaInvalida : DomainException
    {
        public ErrorReferenciaInvalida(string entidad) 
            : base($"La referencia a {entidad} no es válida")
        {
        }
        
        public ErrorReferenciaInvalida(string entidad, string valor) 
            : base($"No existe {entidad} con el valor '{valor}'")
        {
        }
    }
}
