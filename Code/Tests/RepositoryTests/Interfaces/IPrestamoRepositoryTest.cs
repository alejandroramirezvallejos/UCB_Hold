namespace IMT_Reservas.Tests.RepositoryTests
{
    public interface IPrestamoRepositoryTest
    {
        void Crear_LlamaExecuteSpNR_ConParametrosCorrectos();
        void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable();
        void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos();
        void Repositorio_CuandoHayExcepcion_LanzaExcepcion();
    }
}

