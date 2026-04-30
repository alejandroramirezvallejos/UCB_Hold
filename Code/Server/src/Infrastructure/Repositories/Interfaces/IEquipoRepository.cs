using System.Data;
using Ardalis.Result;

public interface IEquipoRepository
{
    Result<EquipoDto> Crear(CrearEquipoComando comando);
    Result<EquipoDto> Crear(int idGrupoEquipo, int codigoImt, int? idGavetero, CrearEquipoComando comando);
    Result<EquipoDto> Actualizar(ActualizarEquipoComando comando);
    Result<EquipoDto> Actualizar(int? idGrupoEquipo, int? idGavetero, ActualizarEquipoComando comando);
    Result<EquipoDto> Eliminar(EliminarEquipoComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    int? ObtenerGrupoEquipoIdPorNombreModeloMarca(string nombre, string modelo, string marca);
    int? ObtenerGaveteroIdPorNombre(string nombreGavetero);
    int GenerarCodigoImt(int idCategoria);
    int? ObtenerCategoriaIdPorGrupoEquipoId(int idGrupoEquipo);
    int? ObtenerGrupoEquipoIdPorEquipoId(int idEquipo);
    int? ObtenerEquipoIdPorCodigoImt(int codigoImt);
}
