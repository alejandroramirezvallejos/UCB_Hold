namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IComponenteControllerTest
    {
        void GetComponentes_ConDatos_RetornaOk();
        void GetComponentes_SinDatos_RetornaOkVacia();
        void GetComponentes_ServicioError_RetornaBadRequest();
        void CrearComponente_Valido_RetornaCreated();
        void CrearComponente_Invalido_RetornaBadRequest(CrearComponenteComando comando, System.Exception excepcionLanzada);
        void CrearComponente_RegistroExistente_RetornaConflict();
        void CrearComponente_ServicioError_RetornaError500();
        void ActualizarComponente_Valido_RetornaOk();
        void ActualizarComponente_Invalido_RetornaBadRequest(ActualizarComponenteComando comando, System.Exception excepcionLanzada);
        void ActualizarComponente_NoEncontrado_RetornaNotFound();
        void ActualizarComponente_RegistroExistente_RetornaConflict();
        void ActualizarComponente_ServicioError_RetornaError500();
        void EliminarComponente_Valido_RetornaNoContent();
        void EliminarComponente_Invalido_RetornaBadRequest(int idComponente, System.Exception excepcionLanzada);
        void EliminarComponente_NoEncontrado_RetornaNotFound();
        void EliminarComponente_EnUso_RetornaConflict();
        void EliminarComponente_ServicioError_RetornaError500();
    }
}

