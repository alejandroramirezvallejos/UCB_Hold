using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class AccesorioRepository : Repository<AccesorioEntity, AccesorioDto>
{
    public AccesorioRepository(ApplicationDbContext dbContext, AccesorioMapper mapper)
        : base(dbContext, mapper) { }

    public override async Task<Result<AccesorioDto>> Create(AccesorioEntity entity)
    {
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync();

        return await Get(entity.Id);
    }

    public override async Task<Result<AccesorioDto>> Update(AccesorioEntity entity)
    {
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();

        return await Get(entity.Id);
    }

    public override async Task<Result<List<AccesorioDto>>> GetAll()
    {
        var dtos = await DbContext
            .Accesorios.AsNoTracking()
            .Join(
                DbContext.Equipos,
                accesorio => accesorio.IdEquipo,
                equipo => equipo.Id,
                (accesorio, equipo) =>
                    new AccesorioDto
                    {
                        Id = accesorio.Id,
                        Nombre = accesorio.Nombre,
                        Modelo = accesorio.Modelo,
                        Tipo = accesorio.Tipo,
                        Descripcion = accesorio.Descripcion,
                        Precio = accesorio.Precio,
                        UrlDataSheet = accesorio.UrlDataSheet,
                        IdEquipo = accesorio.IdEquipo,
                        CodigoImtEquipoAsociado = equipo.CodigoImt.ToString(),
                        NombreEquipoAsociado = equipo.Descripcion,
                    }
            )
            .ToListAsync();

        return Result<List<AccesorioDto>>.Success(dtos);
    }

    public override async Task<Result<AccesorioDto>> Get(int id)
    {
        var dto = await DbContext
            .Accesorios.AsNoTracking()
            .Where(accesorio => accesorio.Id == id)
            .Join(
                DbContext.Equipos,
                accesorio => accesorio.IdEquipo,
                equipo => equipo.Id,
                (accesorio, equipo) =>
                    new AccesorioDto
                    {
                        Id = accesorio.Id,
                        Nombre = accesorio.Nombre,
                        Modelo = accesorio.Modelo,
                        Tipo = accesorio.Tipo,
                        Descripcion = accesorio.Descripcion,
                        Precio = accesorio.Precio,
                        UrlDataSheet = accesorio.UrlDataSheet,
                        IdEquipo = accesorio.IdEquipo,
                        CodigoImtEquipoAsociado = equipo.CodigoImt.ToString(),
                        NombreEquipoAsociado = equipo.Descripcion,
                    }
            )
            .FirstOrDefaultAsync();

        return dto == null ? Result<AccesorioDto>.NotFound() : Result<AccesorioDto>.Success(dto);
    }

    public async Task<int?> GetEquipoByCodigoImt(int codigoImt) =>
        await DbContext
            .Equipos.AsNoTracking()
            .Where(equipo => equipo.CodigoImt == codigoImt && !equipo.EstadoEliminado)
            .Select(equipo => equipo.Id)
            .FirstOrDefaultAsync();
}
