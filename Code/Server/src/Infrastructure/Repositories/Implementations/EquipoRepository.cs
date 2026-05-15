using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EquipoRepository : Repository<EquipoEntity, EquipoDto>
{
    private readonly EquipoMapper _mapper;

    public EquipoRepository(ApplicationDbContext dbContext, EquipoMapper mapper)
        : base(dbContext) { _mapper = mapper; }

    protected override EquipoDto MapToDto(EquipoEntity entity) => _mapper.ToDto(entity);

    private static string EstadoEquipoToText(EstadoEquipo estado) => estado switch
    {
        EstadoEquipo.ParcialmenteOperativo => "parcialmente_operativo",
        EstadoEquipo.Inoperativo => "inoperativo",
        _ => "operativo"
    };

    public override async Task<Result<List<EquipoDto>>> GetAll(QueryFilter? filter = null)
    {
        var rows = await DbContext.Equipos
            .AsNoTracking()
            .Select(equipo => new
            {
                equipo.Id,
                equipo.CodigoImt,
                equipo.CodigoUcb,
                equipo.NumeroSerial,
                equipo.EstadoEquipo,
                equipo.Ubicacion,
                equipo.CostoReferencia,
                equipo.Descripcion,
                equipo.TiempoMaximoPrestamo,
                equipo.Procedencia,
                equipo.IdGrupoEquipo,
                NombreGrupoEquipo = equipo.GrupoEquipo != null ? equipo.GrupoEquipo.Nombre : null,
                equipo.IdGavetero,
                NombreGavetero = equipo.Gavetero != null ? equipo.Gavetero.Nombre : null,
                equipo.FechaIngresoEquipo
            })
            .ToListAsync();

        var dtos = rows.Select(equipo => new EquipoDto
        {
            Id = equipo.Id,
            CodigoImt = equipo.CodigoImt,
            CodigoUcb = equipo.CodigoUcb,
            NumeroSerial = equipo.NumeroSerial,
            EstadoEquipo = EstadoEquipoToText(equipo.EstadoEquipo),
            Ubicacion = equipo.Ubicacion,
            CostoReferencia = equipo.CostoReferencia,
            Descripcion = equipo.Descripcion,
            TiempoMaximoPrestamo = equipo.TiempoMaximoPrestamo,
            Procedencia = equipo.Procedencia,
            IdGrupoEquipo = equipo.IdGrupoEquipo,
            NombreGrupoEquipo = equipo.NombreGrupoEquipo,
            IdGavetero = equipo.IdGavetero,
            NombreGavetero = equipo.NombreGavetero,
            FechaIngresoEquipo = new DateTime(equipo.FechaIngresoEquipo.Year, equipo.FechaIngresoEquipo.Month, equipo.FechaIngresoEquipo.Day)
        }).ToList();

        return Result<List<EquipoDto>>.Success(dtos);
    }

    public override async Task<Result<EquipoDto>> Get(int id)
    {
        var row = await DbContext.Equipos
            .AsNoTracking()
            .Where(equipo => equipo.Id == id)
            .Select(equipo => new
            {
                equipo.Id,
                equipo.CodigoImt,
                equipo.CodigoUcb,
                equipo.NumeroSerial,
                equipo.EstadoEquipo,
                equipo.Ubicacion,
                equipo.CostoReferencia,
                equipo.Descripcion,
                equipo.TiempoMaximoPrestamo,
                equipo.Procedencia,
                equipo.IdGrupoEquipo,
                NombreGrupoEquipo = equipo.GrupoEquipo != null ? equipo.GrupoEquipo.Nombre : null,
                equipo.IdGavetero,
                NombreGavetero = equipo.Gavetero != null ? equipo.Gavetero.Nombre : null,
                equipo.FechaIngresoEquipo
            })
            .FirstOrDefaultAsync();

        if (row == null) 
            return Result<EquipoDto>.NotFound();

        return Result<EquipoDto>.Success(new EquipoDto
        {
            Id = row.Id,
            CodigoImt = row.CodigoImt,
            CodigoUcb = row.CodigoUcb,
            NumeroSerial = row.NumeroSerial,
            EstadoEquipo = EstadoEquipoToText(row.EstadoEquipo),
            Ubicacion = row.Ubicacion,
            CostoReferencia = row.CostoReferencia,
            Descripcion = row.Descripcion,
            TiempoMaximoPrestamo = row.TiempoMaximoPrestamo,
            Procedencia = row.Procedencia,
            IdGrupoEquipo = row.IdGrupoEquipo,
            NombreGrupoEquipo = row.NombreGrupoEquipo,
            IdGavetero = row.IdGavetero,
            NombreGavetero = row.NombreGavetero,
            FechaIngresoEquipo = new DateTime(row.FechaIngresoEquipo.Year, row.FechaIngresoEquipo.Month, row.FechaIngresoEquipo.Day)
        });
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.Equipos
            .FirstOrDefaultAsync(equipo => equipo.Id == id && !equipo.EstadoEliminado);

        if (entity == null) 
            return Result<object>.NotFound();

        entity.EstadoEliminado = true;
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }

}
