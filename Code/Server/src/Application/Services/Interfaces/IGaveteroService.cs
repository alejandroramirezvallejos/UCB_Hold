public interface IGaveteroService
{
    void CrearGavetero(CrearGaveteroComando comando);
    void ActualizarGavetero(ActualizarGaveteroComando comando);
    void EliminarGavetero(EliminarGaveteroComando comando);
    List<GaveteroDto>? ObtenerTodosGaveteros();
}
