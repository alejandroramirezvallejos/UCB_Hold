using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioRepository, UsuarioDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UsuarioMapper _mapper;
    private readonly IValidator<UsuarioDto> _validator;

    public UsuarioService(UsuarioRepository repository, ApplicationDbContext dbContext, UsuarioMapper mapper, IValidator<UsuarioDto> validator)
        : base(repository) => (_dbContext, _mapper, _validator) = (dbContext, mapper, validator);

    public async Task<Result<UsuarioDto>> Create(UsuarioDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<UsuarioDto>();

        await ResolveCarrera(dto);

        if (await _dbContext.Usuarios.IgnoreQueryFilters().AnyAsync(usuario => usuario.Carnet == dto.Carnet))
            return Result<UsuarioDto>.Error("Carnet ya existe");

        if (await _dbContext.Usuarios.IgnoreQueryFilters().AnyAsync(usuario => usuario.Email == dto.Email))
            return Result<UsuarioDto>.Error("Email ya existe");

        if (!await _dbContext.Carreras.AnyAsync(carrera => carrera.Id == dto.IdCarrera && !carrera.EstadoEliminado))
            return Result<UsuarioDto>.Error("Carrera no existe");

        var entity = _mapper.ToEntity(dto);
        entity.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

        var result = await base.Create(entity);
        
        if (result.IsSuccess && result.Value != null)
            result.Value.CarreraNombre = await GetCarreraNombre(entity.IdCarrera);

        return result;
    }

    public async Task<Result<UsuarioDto>> Update(string carnet, UsuarioDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return validation.ToResult<UsuarioDto>();

        var existing = await _dbContext.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Carnet == carnet && !usuario.EstadoEliminado);

        if (existing == null) 
            return Result<UsuarioDto>.NotFound();

        await ResolveCarrera(dto);
        _mapper.Update(dto, existing);

        if ((dto.IdCarrera ?? 0) > 0) 
            existing.IdCarrera = dto.IdCarrera!.Value;

        if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            existing.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

        await _dbContext.SaveChangesAsync();

        var resultDto = _mapper.ToDto(existing);
        resultDto.CarreraNombre = await GetCarreraNombre(existing.IdCarrera);
       
        return Result<UsuarioDto>.Success(resultDto);
    }

    public async Task<Result<UsuarioDto>> Get(string carnet)
    {
        var usuario = await Repository.GetByCarnet(carnet);
        
        if (usuario == null) 
            return Result<UsuarioDto>.NotFound();

        var dto = _mapper.ToDto(usuario);
        dto.CarreraNombre = await GetCarreraNombre(usuario.IdCarrera);
        
        return Result<UsuarioDto>.Success(dto);
    }

    public async Task<Result<UsuarioDto>> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return Result<UsuarioDto>.Unauthorized("Credenciales requeridas");

        var loginData = await _dbContext.Usuarios
            .AsNoTracking()
            .Where(usuario => usuario.Email == email && !usuario.EstadoEliminado)
            .Select(usuario => new
            {
                Usuario = usuario,
                CarreraNombre = _dbContext.Carreras
                    .Where(carrera => carrera.Id == usuario.IdCarrera)
                    .Select(carrera => carrera.Nombre)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (loginData == null) 
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        var passwordValid = !string.IsNullOrEmpty(loginData.Usuario.Contrasena)
                         && loginData.Usuario.Contrasena.StartsWith("$2")
                         && loginData.Usuario.Contrasena.Length == 60
                         && BCrypt.Net.BCrypt.Verify(password, loginData.Usuario.Contrasena);

        if (!passwordValid) 
            return Result<UsuarioDto>.Unauthorized("Credenciales inválidas");

        var dto = _mapper.ToDto(loginData.Usuario);
        dto.CarreraNombre = loginData.CarreraNombre;
        
        return Result<UsuarioDto>.Success(dto);
    }

    public async Task<Result<object>> Delete(string carnet)
        => await Repository.Delete(carnet);

    private async Task ResolveCarrera(UsuarioDto dto)
    {
        if ((dto.IdCarrera ?? 0) > 0) 
            return;
        
        if (string.IsNullOrWhiteSpace(dto.CarreraNombre)) 
            return;

        var carrera = await _dbContext.Carreras
            .AsNoTracking()
            .FirstOrDefaultAsync(carrera => carrera.Nombre == dto.CarreraNombre && !carrera.EstadoEliminado);

        dto.IdCarrera = carrera?.Id;
    }

    private async Task<string?> GetCarreraNombre(int idCarrera)
        => await _dbContext.Carreras
            .AsNoTracking()
            .Where(carrera => carrera.Id == idCarrera)
            .Select(carrera => carrera.Nombre)
            .FirstOrDefaultAsync();
}
