using Shared.Common;

namespace Shared.Common
{
    public class ErrorRegistroEnUso : DomainException
    {
        public ErrorRegistroEnUso(string entidad) 
            : base($"No se puede eliminar el {entidad} porque está siendo utilizado")
        {
        }
        
        public ErrorRegistroEnUso(string entidad, string referencia) 
            : base($"No se puede eliminar el {entidad} porque tiene {referencia} asociados")
        {
        }
    }
}
