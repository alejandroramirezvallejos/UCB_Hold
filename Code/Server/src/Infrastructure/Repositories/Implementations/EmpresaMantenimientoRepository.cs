using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
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
        var dtos = await DbContext.EmpresasMantenimiento
            .AsNoTracking()
            .Select(e => new EmpresaMantenimientoDto
            {
                Id = e.Id,
                NombreResponsable = e.NombreResponsable,
                ApellidoResponsable = e.ApellidoResponsable,
                Telefono = e.Telefono,
                Direccion = e.Direccion
            })
            .ToListAsync();

        return Result<List<EmpresaMantenimientoDto>>.Success(dtos);
    }

    public override async Task<Result<EmpresaMantenimientoDto>> Get(int id)
    {
        var dto = await DbContext.EmpresasMantenimiento
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmpresaMantenimientoDto
            {
                Id = e.Id,
                NombreResponsable = e.NombreResponsable,
                ApellidoResponsable = e.ApellidoResponsable,
                Telefono = e.Telefono,
                Direccion = e.Direccion
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<EmpresaMantenimientoDto>.NotFound()
            : Result<EmpresaMantenimientoDto>.Success(dto);
    }

    public async Task<EmpresaMantenimientoEntity?> GetByNombre(string nombre)
        => await DbContext.EmpresasMantenimiento.AsNoTracking().FirstOrDefaultAsync(e => e.Nombre == nombre && !e.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.EmpresasMantenimiento.AnyAsync(e => e.Id == id && !e.EstadoEliminado);

    protected override EmpresaMantenimientoDto MapToDto(EmpresaMantenimientoEntity entity) => new()
    {
        Id = entity.Id,
        NombreResponsable = entity.NombreResponsable,
        ApellidoResponsable = entity.ApellidoResponsable,
        Telefono = entity.Telefono,
        Direccion = entity.Direccion
    };
}


