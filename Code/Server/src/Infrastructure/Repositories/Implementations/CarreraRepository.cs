using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarreraRepository : Repository<CarreraEntity, CarreraListDto>
{
    public CarreraRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<CarreraEntity?> GetByNombre(string nombre)
        => await DbContext.Carreras.FirstOrDefaultAsync(c => c.Nombre == nombre && !c.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Carreras.AnyAsync(c => c.Id == id && !c.EstadoEliminado);

    protected override CarreraListDto MapToDto(CarreraEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre
    };
}

