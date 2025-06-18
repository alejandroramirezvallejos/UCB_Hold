namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IMuebleControllerTest
    {
        void GetMuebles_ConDatos_RetornaOk();
        void GetMuebles_SinDatos_RetornaOkVacia();
        void GetMuebles_ServicioError_RetornaBadRequest();
        void CrearMueble_Valido_RetornaCreated();
        void CrearMueble_Invalido_RetornaBadRequest(CrearMuebleComando comando, System.Exception excepcionLanzada);
        void CrearMueble_RegistroExistente_RetornaConflict();
        void CrearMueble_ServicioError_RetornaError500();
        void ActualizarMueble_Valido_RetornaOk();
        void ActualizarMueble_Invalido_RetornaBadRequest(ActualizarMuebleComando comando, System.Exception excepcionLanzada);
        void ActualizarMueble_NoEncontrado_RetornaNotFound();
        void ActualizarMueble_ServicioError_RetornaError500();
        void EliminarMueble_Valido_RetornaNoContent();
        void EliminarMueble_Invalido_RetornaBadRequest(int idMueble, System.Exception excepcionLanzada);
        void EliminarMueble_NoEncontrado_RetornaNotFound();
        void EliminarMueble_EnUso_RetornaConflict();
        void EliminarMueble_ServicioError_RetornaError500();
    }
}

