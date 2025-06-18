namespace IMT_Reservas.Tests.ServiceTests
{
    public interface IEmpresaMantenimientoServiceTest
    {
        void CrearEmpresaMantenimiento_ComandoValido_LlamaRepositorioCrear();
        void CrearEmpresaMantenimiento_NombreVacio_LanzaErrorNombreRequerido();
        void ObtenerTodasEmpresasMantenimiento_CuandoHayDatos_RetornaListaDeDtos();
        void ActualizarEmpresaMantenimiento_ComandoValido_LlamaRepositorioActualizar();
        void EliminarEmpresaMantenimiento_ComandoValido_LlamaRepositorioEliminar();
    }
}

