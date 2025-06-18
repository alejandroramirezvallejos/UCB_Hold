namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IMantenimientoServiceTest
    {
        void CrearMantenimiento_ComandoValido_LlamaRepositorioCrear();
        void CrearMantenimiento_FechaFinAnteriorAInicio_LanzaErrorFechaInvalida();
        void ObtenerTodosMantenimientos_CuandoHayDatos_RetornaListaDeDtos();
        void EliminarMantenimiento_ComandoValido_LlamaRepositorioEliminar();
    }
}

