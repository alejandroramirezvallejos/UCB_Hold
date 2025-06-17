using Shared.Common;

namespace Shared.Common
{
    public class ErrorValorNegativo : DomainException
    {
        public ErrorValorNegativo(string campo) 
            : base($"El {campo} no puede ser negativo")
        {
        }
        
        public ErrorValorNegativo(string campo, double valor) 
            : base($"El {campo} '{valor}' no puede ser negativo")
        {
        }
    }
}
