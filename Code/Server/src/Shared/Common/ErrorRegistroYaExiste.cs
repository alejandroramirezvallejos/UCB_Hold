using Shared.Common;

namespace Shared.Common
{
    public class ErrorRegistroYaExiste : DomainException
    {
        public ErrorRegistroYaExiste() 
            : base($"Ya existe un registro con estos datos")
        {
        }
    }
}
