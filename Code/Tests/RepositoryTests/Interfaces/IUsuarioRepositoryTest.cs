namespace IMT_Reservas.Tests.RepositoryTests
{
    public interface IUsuarioRepositoryTest
    {
        void Crear_LlamaExecuteSpNR_ConParametrosCorrectos();
        void ObtenerTodos_LlamaEjecutarFuncion_YRetornaDataTable();
        void ObtenerPorEmailYContrasena_LlamaEjecutarFuncion_YRetornaDataTable();
        void ObtenerPorEmailYContrasena_CuandoNoHayFilas_RetornaNull();
        void Actualizar_LlamaExecuteSpNR_ConParametrosCorrectos();
        void Eliminar_LlamaExecuteSpNR_ConParametrosCorrectos();
        void Repositorio_CuandoHayExcepcion_LanzaExcepcion();
    }
}

