using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class AccesorioRepository : Repository<AccesorioEntity, AccesorioList>
{
    public AccesorioRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.Accesorios.AnyAsync(a => a.Id == id && !a.EstadoEliminado);

    public async Task<int?> GetEquipoByCodigoImt(int codigoImt)
        => await DbContext.Equipos
            .Where(e => e.CodigoImt == codigoImt && !e.EstadoEliminado)
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

    protected override AccesorioList MapToDto(AccesorioEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Modelo = entity.Modelo,
        Tipo = entity.Tipo,
        Descripcion = entity.Descripcion,
        CodigoImt = null,
        Precio = (decimal?)entity.Precio,
        UrlDataSheet = entity.UrlDataSheet,
        NombreEquipoAsociado = null
    };
}
