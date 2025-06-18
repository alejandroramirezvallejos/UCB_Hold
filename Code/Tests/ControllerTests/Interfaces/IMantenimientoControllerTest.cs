namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IMantenimientoControllerTest
    {
        void GetMantenimientos_ConDatos_RetornaOk();
        void GetMantenimientos_SinDatos_RetornaOkVacia();
        void GetMantenimientos_ServicioError_RetornaBadRequest();
        void CrearMantenimiento_Valido_RetornaCreated();
        void CrearMantenimiento_Invalido_RetornaBadRequest(CrearMantenimientoComando comando, System.Exception excepcionLanzada);
        void CrearMantenimiento_ServicioError_RetornaError500();
        void EliminarMantenimiento_Valido_RetornaNoContent();
        void EliminarMantenimiento_Invalido_RetornaBadRequest(int idMantenimiento, System.Exception excepcionLanzada);
        void EliminarMantenimiento_NoEncontrado_RetornaNotFound();
        void EliminarMantenimiento_EnUso_RetornaConflict();
        void EliminarMantenimiento_ServicioError_RetornaError500();
    }
}

