using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class UsuarioRepository : Repository<UsuarioEntity, UsuarioListDto>
{
    public UsuarioRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<UsuarioEntity?> GetByCarnet(string carnet)
    {
        return await DbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Carnet == carnet && !u.EstadoEliminado);
    }

    public async Task<UsuarioEntity?> GetByEmail(string email)
    {
        return await DbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email && !u.EstadoEliminado);
    }

    public async Task<bool> ExistsActive(string carnet)
    {
        return await DbContext.Usuarios
            .AnyAsync(u => u.Carnet == carnet && !u.EstadoEliminado);
    }

    public async Task<Result<object>> Delete(string carnet)
    {
        try
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
        catch (DbUpdateException ex)
        {
            return Result<object>.Error($"Database error: {ex.InnerException?.Message}");
        }
    }

    protected override UsuarioListDto MapToDto(UsuarioEntity entity) => new()
    {
        Id = 0,
        Carnet = entity.Carnet,
        Nombre = entity.Nombre,
        ApellidoPaterno = entity.ApellidoPaterno,
        ApellidoMaterno = entity.ApellidoMaterno,
        Email = entity.Email,
        IdCarrera = entity.IdCarrera,
        Rol = entity.Rol,
        Telefono = entity.Telefono,
        TelefonoReferencia = entity.TelefonoReferencia,
        NombreReferencia = entity.NombreReferencia,
        EmailReferencia = entity.EmailReferencia
    };
}

