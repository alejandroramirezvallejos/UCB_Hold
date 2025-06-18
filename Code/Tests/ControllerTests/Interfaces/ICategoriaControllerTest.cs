namespace IMT_Reservas.Tests.ControllerTests
{
    public interface ICategoriaControllerTest
    {
        void GetCategorias_ConDatos_RetornaOk();
        void GetCategorias_SinDatos_RetornaOkVacia();
        void GetCategorias_ServicioError_RetornaBadRequest();
        void CrearCategoria_Valida_RetornaCreated();
        void CrearCategoria_Invalida_RetornaBadRequest(CrearCategoriaComando comando, System.Exception excepcionLanzada);
        void CrearCategoria_RegistroExistente_RetornaConflict();
        void CrearCategoria_ServicioError_RetornaError500();
        void ActualizarCategoria_Valida_RetornaOk();
        void ActualizarCategoria_Invalida_RetornaBadRequest(ActualizarCategoriaComando comando, System.Exception excepcionLanzada);
        void ActualizarCategoria_NoEncontrada_RetornaNotFound();
        void ActualizarCategoria_RegistroExistente_RetornaConflict();
        void ActualizarCategoria_ServicioError_RetornaError500();
        void EliminarCategoria_Valida_RetornaNoContent();
        void EliminarCategoria_Invalida_RetornaBadRequest(int idCategoria, System.Exception excepcionLanzada);
        void EliminarCategoria_NoEncontrada_RetornaNotFound();
        void EliminarCategoria_EnUso_RetornaConflict();
        void EliminarCategoria_ServicioError_RetornaError500();
    }
}

