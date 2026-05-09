using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Core.Abstraction;
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
        var entities = await DbContext.Equipos
            .AsNoTracking()
            .Include(e => e.GrupoEquipo)
            .Include(e => e.Gavetero)
            .ToListAsync();

        var dtos = entities.Select(ToEquipoDto).ToList();
       
        return Result<List<EquipoDto>>.Success(dtos);
    }

    public override async Task<Result<EquipoDto>> Get(int id)
    {
        var entity = await DbContext.Equipos
            .AsNoTracking()
            .Include(e => e.GrupoEquipo)
            .Include(e => e.Gavetero)
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity == null
            ? Result<EquipoDto>.NotFound()
            : Result<EquipoDto>.Success(ToEquipoDto(entity));
    }

    private static EquipoDto ToEquipoDto(EquipoEntity e) => new()
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
        NombreGrupoEquipo = e.GrupoEquipo?.Nombre,
        IdGrupoEquipo = e.IdGrupoEquipo,
        NombreGavetero = e.Gavetero?.Nombre,
        IdGavetero = e.IdGavetero,
        FechaIngresoEquipo = new DateTime(e.FechaIngresoEquipo.Year, e.FechaIngresoEquipo.Month, e.FechaIngresoEquipo.Day)
    };

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.Equipos
            .FirstOrDefaultAsync(e => e.Id == id && !e.EstadoEliminado);

        if (entity == null)
            return Result<object>.NotFound();

        entity.EstadoEliminado = true; 
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }

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

