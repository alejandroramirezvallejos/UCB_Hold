namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IUsuarioServiceTest
    {
        void CrearUsuario_ComandoValido_LlamaRepositorioCrear();
        void CrearUsuario_CarnetVacio_LanzaErrorNombreRequerido();
        void CrearUsuario_EmailInvalido_LanzaErrorNombreRequerido();
        void ObtenerTodosUsuarios_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarUsuario_ComandoValido_LlamaRepositorioActualizar();
        void EliminarUsuario_ComandoValido_LlamaRepositorioEliminar();
        void IniciarSesionUsuario_CredencialesValidas_RetornaUsuarioDto();
        void IniciarSesionUsuario_CredencialesInvalidas_RetornaNull();
    }
}

