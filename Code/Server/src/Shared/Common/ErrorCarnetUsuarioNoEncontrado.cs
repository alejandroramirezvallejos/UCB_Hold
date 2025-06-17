using Shared.Common;

namespace Shared.Common
{
    public class ErrorCarnetUsuarioNoEncontrado : DomainException
    {
        public ErrorCarnetUsuarioNoEncontrado(string carnet) 
            : base($"No se encontró un usuario con el carnet '{carnet}'")
        {
        }
    }
}
