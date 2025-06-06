//implementar
public interface IMuebleRepository{
    MuebleDto? ObtenerPorId(Guid id);
    MuebleDto Crear(MuebleDto mueble);
    MuebleDto? Actualizar(MuebleDto mueble);
    bool Eliminar(Guid id);
    List<MuebleDto> ObtenerTodos();
}