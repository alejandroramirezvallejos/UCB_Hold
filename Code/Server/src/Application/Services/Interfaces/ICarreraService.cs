public interface ICarreraService
{
    void CrearCarrera(CrearCarreraComando comando);
    List<CarreraDto>? ObtenerTodasCarreras();
    void ActualizarCarrera(ActualizarCarreraComando comando);
    void EliminarCarrera(EliminarCarreraComando comando);
}
