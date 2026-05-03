using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EquipoRepository : Repository<EquipoEntity, EquipoDto>
{
    public EquipoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.Equipos.AnyAsync(e => e.Id == id && !e.EstadoEliminado);

    protected override EquipoDto MapToDto(EquipoEntity entity) => new()
    {
        Id = entity.Id,
        Modelo = null,
        Marca = null,
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
        FechaIngresoEquipo = entity.FechaIngresoEquipo.ToDateTime(TimeOnly.MinValue)
    };
}

