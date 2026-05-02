using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using IMT_Reservas.Server.Application.Features.Usuario.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Usuario;

public class UsuarioService : Service<UsuarioEntity, UsuarioDetailDto, UsuarioListDto>
{
	private readonly UsuarioRepository _repository;

	public UsuarioService(UsuarioRepository repository, IMapper mapper) : base(repository, mapper)
	{
		_repository = repository;
	}

	protected override Validator<UsuarioEntity> GetValidator() => new UsuarioValidator();

	public Result<UsuarioDetailDto> Create(UsuarioEntity entity) => base.Create(entity);

	public Result<UsuarioDetailDto> Update(UsuarioEntity entity) => base.Update(entity);

	public Result<object> Delete(int id) => base.Delete(id);

	public Result<UsuarioDetailDto> Get(int id) => base.Get(id);

	public Result<List<UsuarioListDto>> GetAllUsuarios(QueryFilter? filter = null) => base.GetAll(filter);

	public Result<UsuarioDetailDto> InitiateSession(string email, string contrasena)
	{
		var result = Repository.GetAll(null);
		if (!result.IsSuccess) return Result<UsuarioDetailDto>.Error("Error al obtener usuarios");

		var usuario = result.Value?.FirstOrDefault(u => u.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true);
		if (usuario == null) return Result<UsuarioDetailDto>.NotFound();

		return Result<UsuarioDetailDto>.Success(AutoMapper.Map<UsuarioDetailDto>(usuario));
	}
}

