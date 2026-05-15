using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EmpresaMantenimientoRepository : Repository<EmpresaMantenimientoEntity, EmpresaMantenimientoDto>
{
    private readonly EmpresaMantenimientoMapper _mapper;

    public EmpresaMantenimientoRepository(ApplicationDbContext dbContext, EmpresaMantenimientoMapper mapper)
        : base(dbContext) => _mapper = mapper;

    public override async Task<Result<List<EmpresaMantenimientoDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.EmpresasMantenimiento
            .AsNoTracking()
            .Select(e => new EmpresaMantenimientoDto
            {
                Id = e.Id,
                NombreEmpresa = e.Nombre,
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
                NombreEmpresa = e.Nombre,
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
    
    protected override EmpresaMantenimientoDto MapToDto(EmpresaMantenimientoEntity entity) => _mapper.ToDto(entity);
}


