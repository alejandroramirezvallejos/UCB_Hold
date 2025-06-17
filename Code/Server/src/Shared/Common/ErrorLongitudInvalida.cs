using Shared.Common;

namespace Shared.Common
{
    public class ErrorLongitudInvalida : DomainException
    {
        public ErrorLongitudInvalida(string campo, int longitudMaxima) 
            : base($"El {campo} no puede exceder {longitudMaxima} caracteres")
        {
        }
        
        public ErrorLongitudInvalida(string campo, int longitudMinima, int longitudMaxima) 
            : base($"El {campo} debe tener entre {longitudMinima} y {longitudMaxima} caracteres")
        {
        }
    }
}
