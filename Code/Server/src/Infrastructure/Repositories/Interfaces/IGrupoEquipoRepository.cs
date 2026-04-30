using System.Data;
using Ardalis.Result;

public interface IGrupoEquipoRepository
{
    Result<GrupoEquipoDto> Crear(CrearGrupoEquipoComando comando);
    Result<GrupoEquipoDto> Crear(int idCategoria, CrearGrupoEquipoComando comando);
    Result<GrupoEquipoDto> Actualizar(ActualizarGrupoEquipoComando comando);
    Result<GrupoEquipoDto> Actualizar(int? idCategoria, ActualizarGrupoEquipoComando comando);
    Result<GrupoEquipoDto> Eliminar(EliminarGrupoEquipoComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    bool ExisteDuplicadoPorNombreModeloMarca(string nombre, string modelo, string marca);
    bool ExisteDuplicadoPorNombreModeloMarcaExcluyendoId(string nombre, string modelo, string marca, int idExcluir);
    int? ObtenerCategoriaIdPorNombre(string nombreCategoria);
    void ActualizarCantidad(int idGrupoEquipo, int incremento);
    void ActualizarCostoPromedio(int idGrupoEquipo);
    DataTable? ObtenerPorId(int id);
    DataTable ObtenerPorNombreYCategoria(string? nombre, string? categoria);
}
