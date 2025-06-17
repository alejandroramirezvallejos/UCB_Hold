using Shared.Common;

namespace Shared.Common
{
    public class ErrorRegistroNoEncontrado : DomainException
    {
        public ErrorRegistroNoEncontrado(string entidad) 
            : base($"No se encontró el {entidad} especificado")
        {
        }
        
        public ErrorRegistroNoEncontrado(string entidad, string identificador) 
            : base($"No se encontró {entidad} con identificador '{identificador}'")
        {
        }
    }
}
