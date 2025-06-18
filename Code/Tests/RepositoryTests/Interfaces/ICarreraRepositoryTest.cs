namespace IMT_Reservas.Tests.RepositoryTests
{
    public interface ICarreraRepositoryTest
    {
        void Crear_LlamaExecuteSpNR_ConParametrosCorrectos();
        void ObtenerTodas_LlamaEjecutarFuncion_YRetornaDataTable();
        void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos();
        void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos();
        void Repositorio_CuandoHayExcepcion_LanzaExcepcion();
    }
}

