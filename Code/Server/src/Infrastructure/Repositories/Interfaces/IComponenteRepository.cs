using System.Data;
using Ardalis.Result;

public interface IComponenteRepository
{
    Result<ComponenteDto> Crear(CrearComponenteComando comando);
    Result<ComponenteDto> Crear(int idEquipo, CrearComponenteComando comando);
    Result<ComponenteDto> Actualizar(ActualizarComponenteComando comando);
    Result<ComponenteDto> Actualizar(int? idEquipo, ActualizarComponenteComando comando);
    Result<ComponenteDto> Eliminar(EliminarComponenteComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    int? ObtenerEquipoIdPorCodigoImt(int codigoImt);
}
