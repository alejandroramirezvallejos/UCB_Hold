using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EquipoRepository : Repository<EquipoEntity, EquipoDto>
{
    public EquipoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<EquipoDto>>> GetAll(QueryFilter? filter = null)
    {
        var entities = await DbContext.Equipos.AsNoTracking().ToListAsync();

        return Result<List<EquipoDto>>.Success(entities.Select(MapToDto).ToList());
    }

    public override async Task<Result<EquipoDto>> Get(int id)
    {
        var entity = await DbContext.Equipos
            .FirstOrDefaultAsync(e => e.Id == id && !e.EstadoEliminado);

        return entity == null
            ? Result<EquipoDto>.NotFound()
            : Result<EquipoDto>.Success(MapToDto(entity));
    }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Equipos.AnyAsync(e => e.Id == id && !e.EstadoEliminado);

    protected override EquipoDto MapToDto(EquipoEntity entity) => new()
    {
        Id = entity.Id,
        CodigoImt = entity.CodigoImt,
        CodigoUcb = entity.CodigoUcb,
        NumeroSerial = entity.NumeroSerial,
        EstadoEquipo = entity.EstadoEquipo,
        Ubicacion = entity.Ubicacion,
        CostoReferencia = entity.CostoReferencia,
        Descripcion = entity.Descripcion,
        TiempoMaximoPrestamo = entity.TiempoMaximoPrestamo,
        Procedencia = entity.Procedencia,
        IdGrupoEquipo = entity.IdGrupoEquipo,
        IdGavetero = entity.IdGavetero,
        FechaIngresoEquipo = new DateTime(entity.FechaIngresoEquipo.Year, entity.FechaIngresoEquipo.Month, entity.FechaIngresoEquipo.Day)
    };
}

