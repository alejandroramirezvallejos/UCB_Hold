//implementar
public interface IMuebleRepository{
    void Crear(CrearMuebleComando mueble);
    void Actualizar(ActualizarMuebleComando mueble);
    void Eliminar(int id);
    List<MuebleDto> ObtenerTodos();
}