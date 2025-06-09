public interface IGaveteroRepository
{
    void Crear(CrearGaveteroComando comando);
    void Actualizar(ActualizarGaveteroComando comando);
    void Eliminar(int id);
    List<GaveteroDto> ObtenerTodos();
}
