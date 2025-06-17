using Shared.Common;

namespace Shared.Common
{
    public class ErrorComponenteYaExiste : DomainException
    {
        public ErrorComponenteYaExiste(string nombre) 
            : base($"Ya existe un componente con el nombre '{nombre}'")
        {
        }
        
        public ErrorComponenteYaExiste(int codigoIMT) 
            : base($"Ya existe un componente con el código IMT '{codigoIMT}'")
        {
        }
    }
}
