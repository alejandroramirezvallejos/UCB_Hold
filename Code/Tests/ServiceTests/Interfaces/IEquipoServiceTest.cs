namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IEquipoServiceTest
    {
        void CrearEquipo_ComandoValido_LlamaRepositorioCrear();
        void CrearEquipo_NombreGrupoVacio_LanzaErrorNombreRequerido();
        void ObtenerTodosEquipos_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarEquipo_ComandoValido_LlamaRepositorioActualizar();
        void EliminarEquipo_ComandoValido_LlamaRepositorioEliminar();
    }
}

