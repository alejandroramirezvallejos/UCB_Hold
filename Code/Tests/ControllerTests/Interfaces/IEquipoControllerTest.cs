namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IEquipoControllerTest
    {
        void GetEquipos_ConDatos_RetornaOk();
        void GetEquipos_SinDatos_RetornaOkVacia();
        void GetEquipos_ServicioError_RetornaBadRequest();
        void CrearEquipo_Valido_RetornaCreated();
        void CrearEquipo_Invalido_RetornaBadRequest(CrearEquipoComando comando, System.Exception excepcionLanzada);
        void CrearEquipo_RegistroExistente_RetornaConflict();
        void CrearEquipo_ServicioError_RetornaError500();
        void ActualizarEquipo_Valido_RetornaOk();
        void ActualizarEquipo_Invalido_RetornaBadRequest(ActualizarEquipoComando comando, System.Exception excepcionLanzada);
        void ActualizarEquipo_NoEncontrado_RetornaNotFound();
        void ActualizarEquipo_ServicioError_RetornaError500();
        void EliminarEquipo_Valido_RetornaNoContent();
        void EliminarEquipo_Invalido_RetornaBadRequest(int idEquipo, System.Exception excepcionLanzada);
        void EliminarEquipo_NoEncontrado_RetornaNotFound();
        void EliminarEquipo_EnUso_RetornaConflict();
        void EliminarEquipo_ServicioError_RetornaError500();
    }
}

