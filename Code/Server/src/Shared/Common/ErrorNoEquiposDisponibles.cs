using Shared.Common;

namespace Shared.Common
{
    public class ErrorNoEquiposDisponibles : DomainException
    {
        public ErrorNoEquiposDisponibles() 
            : base("No hay equipos disponibles para préstamo en las fechas solicitadas")
        {
        }
        
        public ErrorNoEquiposDisponibles(int idGrupo) 
            : base($"No se encontró equipo disponible para el grupo ID '{idGrupo}' en las fechas solicitadas")
        {
        }
        
        public ErrorNoEquiposDisponibles(int idGrupo, DateTime fechaInicio, DateTime fechaFin) 
            : base($"No hay equipos disponibles del grupo '{idGrupo}' para el período {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}")
        {
        }
    }
}
