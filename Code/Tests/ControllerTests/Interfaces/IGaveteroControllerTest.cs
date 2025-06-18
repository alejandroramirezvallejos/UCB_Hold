namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IGaveteroControllerTest
    {
        void GetGaveteros_ConDatos_RetornaOk();
        void GetGaveteros_SinDatos_RetornaOkVacia();
        void GetGaveteros_ServicioError_RetornaBadRequest();
        void CrearGavetero_Valido_RetornaCreated();
        void CrearGavetero_Invalido_RetornaBadRequest(CrearGaveteroComando comando, System.Exception excepcionLanzada);
        void CrearGavetero_RegistroExistente_RetornaConflict();
        void CrearGavetero_ServicioError_RetornaError500();
        void ActualizarGavetero_Valido_RetornaOk();
        void ActualizarGavetero_Invalido_RetornaBadRequest(ActualizarGaveteroComando comando, System.Exception excepcionLanzada);
        void ActualizarGavetero_NoEncontrado_RetornaNotFound();
        void ActualizarGavetero_ServicioError_RetornaError500();
        void EliminarGavetero_Valido_RetornaNoContent();
        void EliminarGavetero_Invalido_RetornaBadRequest(int idGavetero, System.Exception excepcionLanzada);
        void EliminarGavetero_NoEncontrado_RetornaNotFound();
        void EliminarGavetero_EnUso_RetornaConflict();
        void EliminarGavetero_ServicioError_RetornaError500();
    }
}

