using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Core.Abstraction;
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
        var accesorios = await DbContext.Accesorios.AsNoTracking().ToListAsync();
        var equipos = await DbContext.Equipos.AsNoTracking().ToListAsync();

        var dtos = accesorios.Select(e => new AccesorioDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Modelo = e.Modelo,
            Tipo = e.Tipo,
            Descripcion = e.Descripcion,
            Precio = e.Precio,
            UrlDataSheet = e.UrlDataSheet,
            IdEquipo = e.IdEquipo,
            CodigoImtEquipoAsociado = equipos.FirstOrDefault(eq => eq.Id == e.IdEquipo)?.CodigoImt.ToString(),
            NombreEquipoAsociado = equipos.FirstOrDefault(eq => eq.Id == e.IdEquipo)?.Descripcion
        }).ToList();

        return Result<List<AccesorioDto>>.Success(dtos);
    }

    public override async Task<Result<AccesorioDto>> Get(int id)
    {
        var accesorio = await DbContext.Accesorios.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        
        if (accesorio == null)
            return Result<AccesorioDto>.NotFound();

        var equipo = await DbContext.Equipos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == accesorio.IdEquipo);

        return Result<AccesorioDto>.Success(new AccesorioDto
        {
            Id = accesorio.Id,
            Nombre = accesorio.Nombre,
            Modelo = accesorio.Modelo,
            Tipo = accesorio.Tipo,
            Descripcion = accesorio.Descripcion,
            Precio = accesorio.Precio,
            UrlDataSheet = accesorio.UrlDataSheet,
            IdEquipo = accesorio.IdEquipo,
            CodigoImtEquipoAsociado = equipo?.CodigoImt.ToString(),
            NombreEquipoAsociado = equipo?.Descripcion
        });
    }
    
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

