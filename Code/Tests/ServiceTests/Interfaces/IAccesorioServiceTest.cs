namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IAccesorioServiceTest
    {
        void CrearAccesorio_ComandoValido_LlamaRepositorioCrear();
        void CrearAccesorio_NombreVacio_LanzaErrorNombreRequerido();
        void CrearAccesorio_NombreExcedeLimite_LanzaErrorLongitudInvalida();
        void CrearAccesorio_CodigoImtInvalido_LanzaErrorIdInvalido();
        void CrearAccesorio_PrecioNegativo_LanzaErrorValorNegativo();
        void ObtenerTodosAccesorios_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarAccesorio_ComandoValido_LlamaRepositorioActualizar();
        void ActualizarAccesorio_IdInvalido_LanzaErrorIdInvalido();
        void EliminarAccesorio_ComandoValido_LlamaRepositorioEliminar();
        void EliminarAccesorio_IdInvalido_LanzaErrorIdInvalido();
    }
}

