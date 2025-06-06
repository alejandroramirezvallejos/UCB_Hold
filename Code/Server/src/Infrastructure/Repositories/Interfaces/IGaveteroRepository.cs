//implementar
public interface IGaveteroRepository
{
    GaveteroDto Crear(CrearGaveteroComando comando);
    GaveteroDto? ObtenerPorId(int id);
    GaveteroDto? Actualizar(ActualizarGaveteroComando comando);
    bool Eliminar(int id);
    List<GaveteroDto> ObtenerTodos();
}
