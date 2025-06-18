namespace IMT_Reservas.Tests.ServiceTests
{
    public interface ICategoriaServiceTest
    {
        void CrearCategoria_ComandoValido_LlamaRepositorioCrear();
        void CrearCategoria_NombreVacio_LanzaErrorNombreRequerido();
        void CrearCategoria_NombreExcedeLimite_LanzaErrorLongitudInvalida();
        void ObtenerTodasCategorias_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarCarrera_ComandoValido_LlamaRepositorioActualizar();
        void ActualizarCategoria_IdInvalido_LanzaErrorIdInvalido();
        void ActualizarCategoria_NombreVacio_LanzaErrorNombreRequerido();
        void ActualizarCategoria_NombreExcedeLimite_LanzaErrorLongitudInvalida();
        void EliminarCategoria_ComandoValido_LlamaRepositorioEliminar();
        void EliminarCategoria_IdInvalido_LanzaErrorIdInvalido();
    }
}

