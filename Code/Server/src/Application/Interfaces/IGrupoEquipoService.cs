public interface IGrupoEquipoService
{
    void CrearGrupoEquipo(CrearGrupoEquipoComando comando);
    GrupoEquipoDto? ObtenerGrupoEquipoPorId(ObtenerGrupoEquipoPorIdConsulta consulta);
    List<GrupoEquipoDto>? ObtenerTodosGruposEquipos();
    List<GrupoEquipoDto>? ObtenerGrupoEquipoPorNombreYCategoria(ObtenerGrupoEquipoPorNombreYCategoriaConsulta consulta);
    void ActualizarGrupoEquipo(ActualizarGrupoEquipoComando comando);
    void EliminarGrupoEquipo(EliminarGrupoEquipoComando comando);
}
