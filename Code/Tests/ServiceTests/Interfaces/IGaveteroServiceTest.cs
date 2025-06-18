namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IGaveteroServiceTest
    {
        void CrearGavetero_ComandoValido_LlamaRepositorioCrear();
        void CrearGavetero_NombreVacio_LanzaErrorNombreRequerido();
        void CrearGavetero_LongitudNegativa_LanzaErrorValorNegativo();
        void ObtenerTodosGaveteros_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarGavetero_ComandoValido_LlamaRepositorioActualizar();
        void EliminarGavetero_ComandoValido_LlamaRepositorioEliminar();
    }
}

