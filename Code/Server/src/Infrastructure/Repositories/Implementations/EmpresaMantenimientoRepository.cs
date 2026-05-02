using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EmpresaMantenimientoRepository : Repository<EmpresaMantenimientoEntity, EmpresaMantenimientoListDto>
{
    public EmpresaMantenimientoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<EmpresaMantenimientoEntity?> GetByNombre(string nombre)
        => await DbContext.EmpresasMantenimiento.FirstOrDefaultAsync(e => e.Nombre == nombre && !e.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.EmpresasMantenimiento.AnyAsync(e => e.Id == id && !e.EstadoEliminado);

    protected override EmpresaMantenimientoListDto MapToDto(EmpresaMantenimientoEntity entity) => new()
    {
        Id = entity.Id,
        NombreEmpresa = entity.Nombre,
        NombreResponsable = entity.NombreResponsable,
        ApellidoResponsable = entity.ApellidoResponsable,
        Telefono = entity.Telefono,
        Email = null,
        Nit = entity.Nit,
        Direccion = entity.Direccion
    };
}

