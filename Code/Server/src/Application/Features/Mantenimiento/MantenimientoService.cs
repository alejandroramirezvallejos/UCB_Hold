using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using DetalleMantenimientoEntity = IMT_Reservas.Server.Core.Entities.DetalleMantenimiento;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService : Service<MantenimientoEntity, MantenimientoRepository, MantenimientoDto>
{
    private readonly ApplicationDbContext _dbContext;

    public MantenimientoService(MantenimientoRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _dbContext = dbContext;
    }

    public override async Task<Result<MantenimientoDto>> Create(MantenimientoEntity entity)
    {
        if (entity.Costo <= 0)
            return Result<MantenimientoDto>.Error("Costo debe ser mayor a 0");

        if (entity.FechaMantenimiento >= entity.FechaFinalMantenimiento)
            return Result<MantenimientoDto>.Error("Fecha mantenimiento menor a final");

        var empresaExists = await _dbContext.EmpresasMantenimiento
            .AnyAsync(e => e.Id == entity.IdEmpresa && !e.EstadoEliminado);

        if (!empresaExists)
            return Result<MantenimientoDto>.Error("Empresa mantenimiento no existe");

        return await base.Create(entity);
    }

    public async Task<Result<MantenimientoDto>> CreateWithDetalles(MantenimientoDto dto)
    {
        if (dto.Costo <= 0)
            return Result<MantenimientoDto>.Error("Costo debe ser mayor a 0");

        if (dto.FechaMantenimiento >= dto.FechaFinalMantenimiento)
            return Result<MantenimientoDto>.Error("Fecha mantenimiento menor a final");

        var idEmpresa = dto.IdEmpresa;
        
        if ((!idEmpresa.HasValue || idEmpresa == 0) && !string.IsNullOrWhiteSpace(dto.NombreEmpresaMantenimiento))
        {
            var empresa = await _dbContext.EmpresasMantenimiento
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Nombre == dto.NombreEmpresaMantenimiento && !e.EstadoEliminado);
            idEmpresa = empresa?.Id;
        }

        if (!idEmpresa.HasValue || idEmpresa == 0)
            return Result<MantenimientoDto>.Error("Empresa mantenimiento no existe");

        var entity = new MantenimientoEntity
        {
            IdEmpresa = idEmpresa.Value,
            FechaMantenimiento = dto.FechaMantenimiento ?? DateTime.UtcNow,
            FechaFinalMantenimiento = dto.FechaFinalMantenimiento ?? DateTime.UtcNow,
            Costo = dto.Costo,
            Descripcion = dto.Descripcion,
            EstadoEliminado = false
        };

        var result = await base.Create(entity);
        
        if (!result.IsSuccess) 
            return result;

        if (dto.CodigoIMT?.Length > 0)
        {
            for (var i = 0; i < dto.CodigoIMT.Length; i++)
            {
                var equipo = await _dbContext.Equipos
                    .FirstOrDefaultAsync(e => e.CodigoImt == dto.CodigoIMT[i] && !e.EstadoEliminado);

                if (equipo == null) continue;

                _dbContext.DetallesMantenimientos.Add(new DetalleMantenimientoEntity
                {
                    IdMantenimiento = result.Value!.Id!.Value,
                    IdEquipo = equipo.Id,
                    TipoMantenimiento = dto.TiposMantenimiento?.ElementAtOrDefault(i),
                    Descripcion = dto.DescripcionesEquipo?.ElementAtOrDefault(i),
                    EstadoEliminado = false
                });
            }
            await _dbContext.SaveChangesAsync();
        }

        return result;
    }
}
