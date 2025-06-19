using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Server.Application.Interfaces
{
    public interface IAccesorioService
    {
        List<AccesorioDto> ObtenerTodosAccesorios();
        void CrearAccesorio(CrearAccesorioComando comando);
        void ActualizarAccesorio(ActualizarAccesorioComando comando);
        void EliminarAccesorio(EliminarAccesorioComando comando);
    }
}

