namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IGrupoEquipoServiceTest
    {
        void CrearGrupoEquipo_ComandoValido_LlamaRepositorioCrear();
        void CrearGrupoEquipo_NombreVacio_LanzaErrorNombreRequerido();
        void ObtenerTodosGruposEquipos_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarGrupoEquipo_ComandoValido_LlamaRepositorioActualizar();
        void EliminarGrupoEquipo_ComandoValido_LlamaRepositorioEliminar();
    }
}

