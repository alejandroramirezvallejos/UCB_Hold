namespace IMT_Reservas.Tests.ServiceTests
{
    public interface ICarreraServiceTest
    {
        void CrearCarrera_ComandoValido_LlamaRepositorioCrear();
        void CrearCarrera_NombreVacio_LanzaErrorNombreRequerido();
        void CrearCarrera_NombreExcedeLimite_LanzaErrorLongitudInvalida();
        void ObtenerTodasCarreras_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarCarrera_ComandoValido_LlamaRepositorioActualizar();
        void ActualizarCarrera_IdInvalido_LanzaErrorIdInvalido();
        void ActualizarCarrera_NombreVacio_LanzaErrorNombreRequerido();
        void ActualizarCarrera_NombreExcedeLimite_LanzaErrorLongitudInvalida();
        void EliminarCarrera_ComandoValido_LlamaRepositorioEliminar();
        void EliminarCarrera_IdInvalido_LanzaErrorIdInvalido();
    }
}

