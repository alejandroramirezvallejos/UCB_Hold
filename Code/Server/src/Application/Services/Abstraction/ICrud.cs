using Ardalis.Result;
using IMT_Reservas.Server.Application.DTOs.Response;

namespace IMT_Reservas.Server.Application.Services.ClasesBase;

public interface ICrud<TDto, TCrear, TActualizar, TEliminar> where TDto : Dto
{
    Result<TDto?> Crear(TCrear comando);
    Result<TDto?> Actualizar(TActualizar comando);
    Result<TDto?> Eliminar(TEliminar comando);
}
