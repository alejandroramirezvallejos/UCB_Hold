using Ardalis.Result;

public interface IGrupoEquipoService
{
    Result<GrupoEquipoDto> Crear(CrearGrupoEquipoComando comando);
    Result<List<GrupoEquipoDto>> ObtenerTodos();
    Result<GrupoEquipoDto> Actualizar(ActualizarGrupoEquipoComando comando);
    Result<GrupoEquipoDto> Eliminar(EliminarGrupoEquipoComando comando);
    GrupoEquipoDto? ObtenerGrupoEquipoPorId(ObtenerGrupoEquipoPorIdConsulta consulta);
    List<GrupoEquipoDto>? ObtenerGrupoEquipoPorNombreYCategoria(ObtenerGrupoEquipoPorNombreYCategoriaConsulta consulta);
}
