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
    public UsuarioRepository(ApplicationDbContext dbContext) : base(dbContext) { }

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
    {
        return await DbContext.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Carnet == carnet && !usuario.EstadoEliminado);
    }

    public async Task<Result<object>> Delete(string carnet)
    {
        var entity = await DbContext.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Carnet == carnet && !usuario.EstadoEliminado);

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


