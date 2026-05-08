using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EquipoRepository : Repository<EquipoEntity, EquipoDto>
{
    public EquipoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    private static string MapEstado(EstadoEquipo estado) => estado switch
    {
        EstadoEquipo.ParcialmenteOperativo => "parcialmente_operativo",
        EstadoEquipo.Inoperativo => "inoperativo",
        _ => "operativo"
    };

    public override async Task<Result<List<EquipoDto>>> GetAll(QueryFilter? filter = null)
    {
        var gruposEquipos = await DbContext.GruposEquipos.AsNoTracking().ToListAsync();
        var gaveteros = await DbContext.Gaveteros.AsNoTracking().ToListAsync();

        var dtos = await DbContext.Equipos
            .AsNoTracking()
            .Where(e => !e.EstadoEliminado)
            .ToListAsync();

        var result = dtos.Select(e => new EquipoDto
        {
            Id = e.Id,
            CodigoImt = e.CodigoImt,
            CodigoUcb = e.CodigoUcb,
            NumeroSerial = e.NumeroSerial,
            EstadoEquipo = MapEstado(e.EstadoEquipo),
            Ubicacion = e.Ubicacion,
            CostoReferencia = e.CostoReferencia,
            Descripcion = e.Descripcion,
            TiempoMaximoPrestamo = e.TiempoMaximoPrestamo,
            Procedencia = e.Procedencia,
            NombreGrupoEquipo = gruposEquipos.FirstOrDefault(g => g.Id == e.IdGrupoEquipo)?.Nombre,
            IdGrupoEquipo = e.IdGrupoEquipo,
            NombreGavetero = gaveteros.FirstOrDefault(g => g.Id == e.IdGavetero)?.Nombre,
            IdGavetero = e.IdGavetero,
            FechaIngresoEquipo = new DateTime(e.FechaIngresoEquipo.Year, e.FechaIngresoEquipo.Month, e.FechaIngresoEquipo.Day)
        }).ToList();

        return Result<List<EquipoDto>>.Success(result);
    }

    public override async Task<Result<EquipoDto>> Get(int id)
    {
        var gruposEquipos = await DbContext.GruposEquipos.AsNoTracking().ToListAsync();
        var gaveteros = await DbContext.Gaveteros.AsNoTracking().ToListAsync();

        var entity = await DbContext.Equipos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.EstadoEliminado);

        if (entity == null)
            return Result<EquipoDto>.NotFound();

        var dto = new EquipoDto
        {
            Id = entity.Id,
            CodigoImt = entity.CodigoImt,
            CodigoUcb = entity.CodigoUcb,
            NumeroSerial = entity.NumeroSerial,
            EstadoEquipo = MapEstado(entity.EstadoEquipo),
            Ubicacion = entity.Ubicacion,
            CostoReferencia = entity.CostoReferencia,
            Descripcion = entity.Descripcion,
            TiempoMaximoPrestamo = entity.TiempoMaximoPrestamo,
            Procedencia = entity.Procedencia,
            NombreGrupoEquipo = gruposEquipos.FirstOrDefault(g => g.Id == entity.IdGrupoEquipo)?.Nombre,
            IdGrupoEquipo = entity.IdGrupoEquipo,
            NombreGavetero = gaveteros.FirstOrDefault(g => g.Id == entity.IdGavetero)?.Nombre,
            IdGavetero = entity.IdGavetero,
            FechaIngresoEquipo = new DateTime(entity.FechaIngresoEquipo.Year, entity.FechaIngresoEquipo.Month, entity.FechaIngresoEquipo.Day)
        };

        return Result<EquipoDto>.Success(dto);
    }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Equipos.AnyAsync(e => e.Id == id && !e.EstadoEliminado);

    protected override EquipoDto MapToDto(EquipoEntity entity) => new()
    {
        Id = entity.Id,
        CodigoImt = entity.CodigoImt,
        CodigoUcb = entity.CodigoUcb,
        NumeroSerial = entity.NumeroSerial,
        EstadoEquipo = MapEstado(entity.EstadoEquipo),
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

