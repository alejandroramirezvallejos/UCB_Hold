using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo;
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
        var dtos = await DbContext.Equipos
            .AsNoTracking()
            .Select(e => new EquipoDto
            {
                Id = e.Id,
                CodigoImt = e.CodigoImt,
                CodigoUcb = e.CodigoUcb,
                NumeroSerial = e.NumeroSerial,
                EstadoEquipo = e.EstadoEquipo,
                Ubicacion = e.Ubicacion,
                CostoReferencia = e.CostoReferencia,
                Descripcion = e.Descripcion,
                TiempoMaximoPrestamo = e.TiempoMaximoPrestamo,
                Procedencia = e.Procedencia,
                IdGrupoEquipo = e.IdGrupoEquipo,
                IdGavetero = e.IdGavetero,
                FechaIngresoEquipo = new DateTime(e.FechaIngresoEquipo.Year, e.FechaIngresoEquipo.Month, e.FechaIngresoEquipo.Day)
            })
            .ToListAsync();

        return Result<List<EquipoDto>>.Success(dtos);
    }

    public override async Task<Result<EquipoDto>> Get(int id)
    {
        var dto = await DbContext.Equipos
            .AsNoTracking()
            .Where(e => e.Id == id && !e.EstadoEliminado)
            .Select(e => new EquipoDto
            {
                Id = e.Id,
                CodigoImt = e.CodigoImt,
                CodigoUcb = e.CodigoUcb,
                NumeroSerial = e.NumeroSerial,
                EstadoEquipo = e.EstadoEquipo,
                Ubicacion = e.Ubicacion,
                CostoReferencia = e.CostoReferencia,
                Descripcion = e.Descripcion,
                TiempoMaximoPrestamo = e.TiempoMaximoPrestamo,
                Procedencia = e.Procedencia,
                IdGrupoEquipo = e.IdGrupoEquipo,
                IdGavetero = e.IdGavetero,
                FechaIngresoEquipo = new DateTime(e.FechaIngresoEquipo.Year, e.FechaIngresoEquipo.Month, e.FechaIngresoEquipo.Day)
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<EquipoDto>.NotFound()
            : Result<EquipoDto>.Success(dto);
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

