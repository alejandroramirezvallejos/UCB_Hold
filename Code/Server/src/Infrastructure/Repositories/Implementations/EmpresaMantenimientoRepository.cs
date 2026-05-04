using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EmpresaMantenimientoRepository : Repository<EmpresaMantenimientoEntity, EmpresaMantenimientoDto>
{
    public EmpresaMantenimientoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<EmpresaMantenimientoDto>>> GetAll(QueryFilter? filter = null)
    {
        var entities = await DbContext.EmpresasMantenimiento
            .AsNoTracking()
            .ToListAsync();
        
        return Result<List<EmpresaMantenimientoDto>>.Success(entities.Select(MapToDto).ToList());
    }

    public override async Task<Result<EmpresaMantenimientoDto>> Get(int id)
    {
        var entity = await DbContext.EmpresasMantenimiento
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return entity == null
            ? Result<EmpresaMantenimientoDto>.NotFound()
            : Result<EmpresaMantenimientoDto>.Success(MapToDto(entity));
    }

    public async Task<EmpresaMantenimientoEntity?> GetByNombre(string nombre)
        => await DbContext.EmpresasMantenimiento.FirstOrDefaultAsync(e => e.Nombre == nombre && !e.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.EmpresasMantenimiento.AnyAsync(e => e.Id == id && !e.EstadoEliminado);

    protected override EmpresaMantenimientoDto MapToDto(EmpresaMantenimientoEntity entity) => new()
    {
        Id = entity.Id,
        NombreResponsable = entity.NombreResponsable,
        ApellidoResponsable = entity.ApellidoResponsable,
        Telefono = entity.Telefono,
        Nit = entity.Nit,
        Direccion = entity.Direccion
    };
}


