namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IMuebleServiceTest
    {
        void CrearMueble_ComandoValido_LlamaRepositorioCrear();
        void CrearMueble_NombreVacio_LanzaErrorNombreRequerido();
        void CrearMueble_CostoNegativo_LanzaErrorValorNegativo();
        void ObtenerTodosMuebles_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarMueble_ComandoValido_LlamaRepositorioActualizar();
        void EliminarMueble_ComandoValido_LlamaRepositorioEliminar();
    }
}

