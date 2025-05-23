public interface IEquipoRepository
{
    EquipoDto Crear(CrearEquipoComando comando);
    EquipoDto? Actualizar(ActualizarEquipoComando comando);
    bool Eliminar(int id);
    EquipoDto? ObtenerPorId(int id);
}