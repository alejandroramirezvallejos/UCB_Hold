//implementar
public interface ICarreraRepository
{
    CarreraDto Crear(CrearCarreraComando comando);
    CarreraDto? ObtenerPorId(int id);
    CarreraDto? Actualizar(ActualizarCarreraComando comando);
    bool Eliminar(int id);
    List<CarreraDto> ObtenerTodas();
}