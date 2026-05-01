using Ardalis.Result;

public interface ICrud<TDto, TCrear, TActualizar, TEliminar> where TDto : Dto
{
    Result<TDto?> Crear(TCrear comando);
    Result<TDto?> Actualizar(TActualizar comando);
    Result<TDto?> Eliminar(TEliminar comando);
}
