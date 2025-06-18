namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IAccesorioControllerTest
    {
        void GetAccesorios_ConDatos_RetornaOk();
        void GetAccesorios_SinDatos_RetornaOkVacia();
        void GetAccesorios_ServicioError_RetornaBadRequest();
        void CrearAccesorio_Valido_RetornaCreated();
        void CrearAccesorio_Invalido_RetornaBadRequest(CrearAccesorioComando comando, System.Exception excepcionLanzada);
        void CrearAccesorio_RegistroExistente_RetornaConflict();
        void CrearAccesorio_ServicioError_RetornaError500();
        void ActualizarAccesorio_Valido_RetornaOk();
        void ActualizarAccesorio_Invalido_RetornaBadRequest(ActualizarAccesorioComando comando, System.Exception excepcionLanzada);
        void ActualizarAccesorio_NoEncontrado_RetornaNotFound();
        void ActualizarAccesorio_RegistroExistente_RetornaConflict();
        void ActualizarAccesorio_ServicioError_RetornaError500();
        void EliminarAccesorio_Valido_RetornaNoContent();
        void EliminarAccesorio_Invalido_RetornaBadRequest(int idAccesorio, System.Exception excepcionLanzada);
        void EliminarAccesorio_NoEncontrado_RetornaNotFound();
        void EliminarAccesorio_EnUso_RetornaConflict();
        void EliminarAccesorio_ServicioError_RetornaError500();
    }
}

