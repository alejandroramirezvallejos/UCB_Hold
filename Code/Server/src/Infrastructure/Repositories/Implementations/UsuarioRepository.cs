using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class UsuarioRepository : Repository<UsuarioEntity, UsuarioDto>
{
    public UsuarioRepository(ApplicationDbContext dbContext, UsuarioMapper mapper)
        : base(dbContext, mapper) { }

    public override async Task<Result<List<UsuarioDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.Usuarios
            .AsNoTracking()
            .Join(DbContext.Carreras, usuario => usuario.IdCarrera, carrera => carrera.Id, (usuario, carrera) => new UsuarioDto
            {
                Carnet = usuario.Carnet,
                Nombre = usuario.Nombre,
                ApellidoPaterno = usuario.ApellidoPaterno,
                ApellidoMaterno = usuario.ApellidoMaterno,
                Rol = usuario.Rol.ToString().ToLowerInvariant(),
                Email = usuario.Email,
                CarreraNombre = carrera.Nombre,
                IdCarrera = usuario.IdCarrera,
                Telefono = usuario.Telefono,
                TelefonoReferencia = usuario.TelefonoReferencia,
                NombreReferencia = usuario.NombreReferencia,
                EmailReferencia = usuario.EmailReferencia,
                ImagenFrenteCarnet = usuario.ImagenFrenteCarnet,
                ImagenAtrasCarnet = usuario.ImagenAtrasCarnet
            })
            .ToListAsync();

        return Result<List<UsuarioDto>>.Success(dtos);
    }

    public async Task<UsuarioEntity?> GetByCarnet(string carnet)
        => await DbContext.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Carnet == carnet && !u.EstadoEliminado);

    public async Task<UsuarioEntity?> GetTrackedByCarnet(string carnet)
        => await DbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Carnet == carnet && !u.EstadoEliminado);

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

    public async Task<bool> ExistsByCarnet(string carnet)
        => await DbContext.Usuarios.IgnoreQueryFilters()
            .AnyAsync(u => u.Carnet == carnet);

    public async Task<bool> ExistsByEmail(string email)
        => await DbContext.Usuarios.IgnoreQueryFilters()
            .AnyAsync(u => u.Email == email);

    public async Task<int?> FindCarreraIdByNombre(string nombre)
        => await DbContext.Carreras.AsNoTracking()
            .Where(c => c.Nombre == nombre && !c.EstadoEliminado)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();

    public async Task<string?> GetCarreraNombre(int idCarrera)
        => await DbContext.Carreras.AsNoTracking()
            .Where(c => c.Id == idCarrera)
            .Select(c => c.Nombre)
            .FirstOrDefaultAsync();

    public async Task<(UsuarioEntity? Usuario, string? CarreraNombre)> GetByEmailWithCarrera(string email)
    {
        var result = await DbContext.Usuarios.AsNoTracking()
            .Where(u => u.Email == email && !u.EstadoEliminado)
            .Select(u => new
            {
                Usuario = u,
                CarreraNombre = DbContext.Carreras
                    .Where(c => c.Id == u.IdCarrera)
                    .Select(c => c.Nombre)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        return result == null ? (null, null) : (result.Usuario, result.CarreraNombre);
    }

    public async Task UpdateEntity(UsuarioEntity entity)
    {
        DbContext.Usuarios.Update(entity);
        await DbContext.SaveChangesAsync();
    }
}
