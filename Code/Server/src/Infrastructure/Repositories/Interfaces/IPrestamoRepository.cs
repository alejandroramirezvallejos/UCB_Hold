public interface IPrestamoRepository
{
    PrestamoDto Crear(CrearPrestamoComando comando);
    PrestamoDto? ObtenerPorId(int id);
    PrestamoDto? Actualizar(ActualizarPrestamoComando comando);
    bool Eliminar(int id);
}