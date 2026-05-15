using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using DetalleMantenimientoEntity = IMT_Reservas.Server.Core.Entities.DetalleMantenimiento;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService : Service<MantenimientoEntity, MantenimientoRepository, MantenimientoDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MantenimientoMapper _mapper;

    public MantenimientoService(MantenimientoRepository repository, ApplicationDbContext dbContext, MantenimientoMapper mapper, IValidator<MantenimientoDto> validator)
        : base(repository, validator) { _dbContext = dbContext; _mapper = mapper; }

    protected override MantenimientoEntity MapToEntity(MantenimientoDto dto) => _mapper.ToEntity(dto);

    public async Task<Result<MantenimientoDto>> Create(MantenimientoDto dto)
    {
        await ResolveEmpresa(dto);

        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<MantenimientoDto>();

        var entity = MapToEntity(dto);
        var result = await base.Create(entity);

        if (!result.IsSuccess) 
            return result;

        await AssignDetalles(entity.Id, dto);
        return result;
    }

    public async Task<Result<MantenimientoDto>> Update(int id, MantenimientoDto dto)
    {
        await ResolveEmpresa(dto);

        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<MantenimientoDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;
        
        return await base.Update(entity);
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var detalles = await _dbContext.DetallesMantenimientos
            .Where(detalle => detalle.IdMantenimiento == id && !detalle.EstadoEliminado)
            .ToListAsync();

        foreach (var detalle in detalles) 
            detalle.EstadoEliminado = true;

        if (detalles.Count > 0) 
            await _dbContext.SaveChangesAsync();

        return await base.Delete(id);
    }

    private async Task ResolveEmpresa(MantenimientoDto dto)
    {
        if ((dto.IdEmpresa ?? 0) > 0) 
            return;
        
        if (string.IsNullOrWhiteSpace(dto.NombreEmpresaMantenimiento)) 
            return;

        var empresa = await _dbContext.EmpresasMantenimiento
            .AsNoTracking()
            .FirstOrDefaultAsync(empresa => empresa.Nombre == dto.NombreEmpresaMantenimiento && !empresa.EstadoEliminado);

        dto.IdEmpresa = empresa?.Id;
    }

    private async Task AssignDetalles(int mantenimientoId, MantenimientoDto dto)
    {
        if (dto.CodigoIMT == null || dto.CodigoIMT.Length == 0) 
            return;

        for (var i = 0; i < dto.CodigoIMT.Length; i++)
        {
            var equipo = await _dbContext.Equipos
                .FirstOrDefaultAsync(equipo => equipo.CodigoImt == dto.CodigoIMT[i] && !equipo.EstadoEliminado);

            if (equipo == null) 
                continue;

            _dbContext.DetallesMantenimientos.Add(new DetalleMantenimientoEntity
            {
                IdMantenimiento = mantenimientoId,
                IdEquipo = equipo.Id,
                TipoMantenimiento = dto.TiposMantenimiento?.ElementAtOrDefault(i),
                Descripcion = dto.DescripcionesEquipo?.ElementAtOrDefault(i),
                EstadoEliminado = false
            });
        }

        await _dbContext.SaveChangesAsync();
    }
}
