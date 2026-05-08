using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Usuario;
using Microsoft.EntityFrameworkCore;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class UsuarioRepository : Repository<UsuarioEntity, UsuarioDto>
{
    public UsuarioRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<UsuarioDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.Usuarios
            .AsNoTracking()
            .Join(DbContext.Carreras, u => u.IdCarrera, c => c.Id, (u, c) => new UsuarioDto
            {
                Carnet = u.Carnet,
                Nombre = u.Nombre,
                ApellidoPaterno = u.ApellidoPaterno,
                ApellidoMaterno = u.ApellidoMaterno,
                Rol = u.Rol.ToString().ToLowerInvariant(),
                Email = u.Email,
                CarreraNombre = c.Nombre,
                IdCarrera = u.IdCarrera,
                Telefono = u.Telefono,
                TelefonoReferencia = u.TelefonoReferencia,
                NombreReferencia = u.NombreReferencia,
                EmailReferencia = u.EmailReferencia,
                ImagenFrenteCarnet = u.ImagenFrenteCarnet,
                ImagenAtrasCarnet = u.ImagenAtrasCarnet
            })
            .ToListAsync();

        return Result<List<UsuarioDto>>.Success(dtos);
    }

    public async Task<UsuarioEntity?> GetByCarnet(string carnet)
    {
        return await DbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Carnet == carnet && !u.EstadoEliminado);
    }

    public async Task<UsuarioEntity?> GetByEmail(string email)
    {
        return await DbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && !u.EstadoEliminado);
    }

    public async Task<bool> ExistsActive(string carnet)
    {
        return await DbContext.Usuarios
            .AnyAsync(u => u.Carnet == carnet && !u.EstadoEliminado);
    }

    public async Task<Result<object>> Delete(string carnet)
    {
        var entity = await DbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Carnet == carnet && !u.EstadoEliminado);

        if (entity == null)
            return Result<object>.NotFound();

        entity.EstadoEliminado = true;
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }

    protected override UsuarioDto MapToDto(UsuarioEntity entity) => new()
    {
        Carnet = entity.Carnet,
        Nombre = entity.Nombre,
        ApellidoPaterno = entity.ApellidoPaterno,
        ApellidoMaterno = entity.ApellidoMaterno,
        Rol = entity.Rol.ToString().ToLowerInvariant(),
        Email = entity.Email,
        CarreraNombre = null,
        IdCarrera = entity.IdCarrera,
        Telefono = entity.Telefono,
        TelefonoReferencia = entity.TelefonoReferencia,
        NombreReferencia = entity.NombreReferencia,
        EmailReferencia = entity.EmailReferencia,
        ImagenFrenteCarnet = entity.ImagenFrenteCarnet,
        ImagenAtrasCarnet = entity.ImagenAtrasCarnet
    };
}


