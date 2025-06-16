public interface IEquipoService
{
    void CrearEquipo(CrearEquipoComando comando);
    void ActualizarEquipo(ActualizarEquipoComando comando);
    void EliminarEquipo(EliminarEquipoComando comando);
    List<EquipoDto>? ObtenerTodosEquipos();
}
