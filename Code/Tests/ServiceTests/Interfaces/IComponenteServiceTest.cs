namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IComponenteServiceTest
    {
        void CrearComponente_ComandoValido_LlamaRepositorioCrear();
        void CrearComponente_NombreVacio_LanzaErrorNombreRequerido();
        void CrearComponente_NombreExcedeLimite_LanzaErrorLongitudInvalida();
        void CrearComponente_ModeloExcedeLimite_LanzaErrorLongitudInvalida();
        void CrearComponente_CodigoImtInvalido_LanzaErrorIdInvalido();
        void CrearComponente_PrecioNegativo_LanzaErrorValorNegativo();
        void ObtenerTodosComponentes_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarComponente_ComandoValido_LlamaRepositorioActualizar();
        void ActualizarComponente_IdInvalido_LanzaErrorIdInvalido();
        void EliminarComponente_ComandoValido_LlamaRepositorioEliminar();
        void EliminarComponente_IdInvalido_LanzaErrorIdInvalido();
    }
}

