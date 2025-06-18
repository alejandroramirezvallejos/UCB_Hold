namespace IMT_Reservas.Tests.RepositoryTests
{
    public interface IGrupoEquipoRepositoryTest
    {
        void Crear_LlamaExecuteSpNR_ConParametrosCorrectos();
        void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable();
        void ObtenerPorId_LlamaEjecutarFuncion_YRetornaDataTable();
        void ObtenerPorNombreYCategoria_LlamaEjecutarFuncion_YRetornaDataTable();
        void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos();
        void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos();
        void Repositorio_CuandoHayExcepcion_LanzaExcepcion();
    }
}

