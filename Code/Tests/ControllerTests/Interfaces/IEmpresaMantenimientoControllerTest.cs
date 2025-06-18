namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IEmpresaMantenimientoControllerTest
    {
        void GetEmpresas_ConDatos_RetornaOk();
        void GetEmpresas_SinDatos_RetornaOkVacia();
        void GetEmpresas_ServicioError_RetornaBadRequest();
        void CrearEmpresa_Valida_RetornaCreated();
        void CrearEmpresa_Invalida_RetornaBadRequest(CrearEmpresaMantenimientoComando comando, System.Exception excepcionLanzada);
        void CrearEmpresa_RegistroExistente_RetornaConflict();
        void CrearEmpresa_ServicioError_RetornaError500();
        void ActualizarEmpresa_Valida_RetornaOk();
        void ActualizarEmpresa_Invalida_RetornaBadRequest(ActualizarEmpresaMantenimientoComando comando, System.Exception excepcionLanzada);
        void ActualizarEmpresa_NoEncontrada_RetornaNotFound();
        void ActualizarEmpresa_RegistroExistente_RetornaConflict();
        void ActualizarEmpresa_ServicioError_RetornaError500();
        void EliminarEmpresa_Valida_RetornaNoContent();
        void EliminarEmpresa_Invalida_RetornaBadRequest(int idEmpresa, System.Exception excepcionLanzada);
        void EliminarEmpresa_NoEncontrada_RetornaNotFound();
        void EliminarEmpresa_EnUso_RetornaConflict();
        void EliminarEmpresa_ServicioError_RetornaError500();
    }
}

