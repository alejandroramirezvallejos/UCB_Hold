using Shared.Common;

namespace Shared.Common
{
    public class ErrorRegistroYaExiste : DomainException
    {
        public ErrorRegistroYaExiste(string entidad) 
            : base($"Ya existe un {entidad} con estos datos")
        {
        }
        
        public ErrorRegistroYaExiste(string entidad, string campo, string valor) 
            : base($"Ya existe un {entidad} con {campo} '{valor}'")
        {
        }
    }
}
