namespace IMT_Reservas.Tests.ControllerTests
{
    public interface IUsuarioControllerTest
    {
        void GetUsuarios_ConDatos_RetornaOk();
        void CrearUsuario_Valido_RetornaOk();
        void ActualizarUsuario_Valido_RetornaOk();
        void EliminarUsuario_Valido_RetornaOk();
        void IniciarSesion_Valido_RetornaOkConUsuario();
        void IniciarSesion_Invalido_RetornaOkNulo();
    }
}

