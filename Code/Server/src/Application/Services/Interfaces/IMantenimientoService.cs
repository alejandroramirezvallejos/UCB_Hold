public interface IMantenimientoService
{
    void CrearMantenimiento(CrearMantenimientoComando comando);
    void EliminarMantenimiento(EliminarMantenimientoComando comando);
    List<MantenimientoDto>? ObtenerTodosMantenimientos();
}
