using System.Data;
using Ardalis.Result;
using IMT_Reservas.Server.Application.DTOs.Response;

namespace IMT_Reservas.Server.Application.Services.ClasesBase;

public abstract class Service<TDto> where TDto : Dto
{
    public virtual Result<List<TDto?>> ObtenerTodos()
    {
        var resultado = ObtenerDataTable();
        if (!resultado.IsSuccess)
            return Result<List<TDto?>>.Error(resultado.Errors.FirstOrDefault() ?? "Error al obtener los datos");

        var lista = resultado.Value.Rows.Cast<DataRow>()
            .Select(MapearFilaADto)
            .OfType<TDto>()
            .ToList<TDto?>();

        return Result<List<TDto?>>.Success(lista);
    }

    protected virtual Result<DataTable> ObtenerDataTable()
    {
        throw new NotImplementedException("ObtenerDataTable must be implemented by derived classes");
    }

    protected abstract Dto MapearFilaADto(DataRow fila);
}
