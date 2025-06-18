namespace IMT_Reservas.Tests.ControllerTests
{
    public interface ICarreraControllerTest
    {
        void GetCarreras_ConDatos_RetornaOk();
        void GetCarreras_SinDatos_RetornaOkVacia();
        void GetCarreras_ServicioError_RetornaBadRequest();
        void CrearCarrera_Valido_RetornaCreated();
        void CrearCarrera_Invalido_RetornaBadRequest(CrearCarreraComando comando, System.Exception excepcionLanzada);
        void CrearCarrera_NombreExistente_RetornaConflict();
        void CrearCarrera_ServicioError_RetornaError500();
        void ActualizarCarrera_Valido_RetornaOk();
        void ActualizarCarrera_Invalido_RetornaBadRequest(ActualizarCarreraComando comando, System.Exception excepcionLanzada);
        void ActualizarCarrera_NoEncontrada_RetornaNotFound();
        void ActualizarCarrera_NombreExistente_RetornaConflict();
        void ActualizarCarrera_ServicioError_RetornaError500();
        void EliminarCarrera_Valido_RetornaNoContent();
        void EliminarCarrera_Invalido_RetornaBadRequest(int idCarrera, System.Exception excepcionLanzada);
        void EliminarCarrera_NoEncontrada_RetornaNotFound();
        void EliminarCarrera_EnUso_RetornaConflict();
        void EliminarCarrera_ServicioError_RetornaError500();
    }
}

