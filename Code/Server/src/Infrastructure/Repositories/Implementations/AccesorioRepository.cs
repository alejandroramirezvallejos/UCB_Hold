using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class AccesorioRepository : Repository<AccesorioEntity, AccesorioDto>
{
    public AccesorioRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<AccesorioDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.Accesorios
            .AsNoTracking()
            .Select(e => new AccesorioDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Tipo = e.Tipo,
                Descripcion = e.Descripcion,
                Precio = e.Precio,
                UrlDataSheet = e.UrlDataSheet,
                IdEquipo = e.IdEquipo,
                CodigoImtEquipoAsociado = null,
                NombreEquipoAsociado = null
            })
            .ToListAsync();

        return Result<List<AccesorioDto>>.Success(dtos);
    }

    public override async Task<Result<AccesorioDto>> Get(int id)
    {
        var dto = await DbContext.Accesorios
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(e => new AccesorioDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Tipo = e.Tipo,
                Descripcion = e.Descripcion,
                Precio = e.Precio,
                UrlDataSheet = e.UrlDataSheet,
                IdEquipo = e.IdEquipo,
                CodigoImtEquipoAsociado = null,
                NombreEquipoAsociado = null
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<AccesorioDto>.NotFound()
            : Result<AccesorioDto>.Success(dto);
    }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Accesorios.AnyAsync(a => a.Id == id && !a.EstadoEliminado);

    public async Task<int?> GetEquipoByCodigoImt(int codigoImt)
        => await DbContext.Equipos
            .Where(e => e.CodigoImt == codigoImt && !e.EstadoEliminado)
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

    protected override AccesorioDto MapToDto(AccesorioEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Modelo = entity.Modelo,
        Tipo = entity.Tipo,
        Descripcion = entity.Descripcion,
        Precio = entity.Precio,
        UrlDataSheet = entity.UrlDataSheet,
        IdEquipo = entity.IdEquipo,
        CodigoImtEquipoAsociado = null,
        NombreEquipoAsociado = null
    };
}

