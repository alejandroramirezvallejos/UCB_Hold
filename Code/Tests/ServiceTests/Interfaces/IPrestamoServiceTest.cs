namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IPrestamoServiceTest
    {
        void CrearPrestamo_ComandoValido_LlamaRepositorioCrear();
        void CrearPrestamo_CarnetUsuarioVacio_LanzaErrorNombreRequerido();
        void CrearPrestamo_GrupoEquipoIdNulo_LanzaErrorIdInvalido();
        void CrearPrestamo_GrupoEquipoIdVacio_LanzaErrorIdInvalido();
        void CrearPrestamo_FechaPrestamoPasada_LanzaArgumentException();
        void CrearPrestamo_FechaDevolucionAnteriorAPrestamo_LanzaArgumentException();
        void ObtenerTodosPrestamos_CuandoHayDatos_RetornaListaDeDtos();
        void EliminarPrestamo_ComandoValido_LlamaRepositorioEliminar();
        void EliminarPrestamo_IdInvalido_LanzaErrorIdInvalido();
    }
}

