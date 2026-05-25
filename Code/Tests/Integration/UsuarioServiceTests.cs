using BCryptLib = BCrypt.Net.BCrypt;
using FluentAssertions;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Tests.Integration;

[TestFixture]
internal class UsuarioServiceTests : ServiceTest<UsuarioService>
{
    protected override UsuarioService CreateService(ApplicationDbContext db)
    {
        var mapper    = new UsuarioMapper();
        var repo      = new UsuarioRepository(db, mapper);
        var validator = new UsuarioValidator(db);
        
        return new UsuarioService(repo, mapper, validator);
    }

    [Test]
    public async Task Create_ValidData_ReturnsSuccessAndHashesPassword()
    {
        var dto = BuildValidUsuario("U001", "u001@ucb.edu.bo");

        var result = await Sut.Create(dto);

        result.IsSuccess.Should().BeTrue();
        var stored = Db.Usuarios.Single(u => u.Carnet == "U001");
        BCryptLib.Verify("Test@1234", stored.Contrasena).Should().BeTrue();
    }

    [Test]
    public async Task Create_EmptyPassword_ReturnsErrorBeforeValidation()
    {
        var dto = BuildValidUsuario("U001", "u001@ucb.edu.bo", contrasena: "");

        var result = await Sut.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Contraseña requerida");
    }

    [Test]
    public async Task Create_DuplicateCarnet_ReturnsError()
    {
        await Sut.Create(BuildValidUsuario("U001", "u001@ucb.edu.bo"));

        var result = await Sut.Create(BuildValidUsuario("U001", "otro@ucb.edu.bo"));

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Carnet ya existe");
    }

    [Test]
    public async Task Create_DuplicateEmail_ReturnsError()
    {
        await Sut.Create(BuildValidUsuario("U001", "shared@ucb.edu.bo"));

        var result = await Sut.Create(BuildValidUsuario("U002", "shared@ucb.edu.bo"));

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Email ya existe");
    }

    [Test]
    public async Task Create_DuplicateTelefono_ReturnsError()
    {
        await Sut.Create(BuildValidUsuario("U001", "u001@ucb.edu.bo", telefono: "77777777"));

        var result = await Sut.Create(BuildValidUsuario("U002", "u002@ucb.edu.bo", telefono: "77777777"));

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Teléfono ya registrado");
    }

    [Test]
    public async Task Create_PasswordWithoutUppercase_ReturnsValidationError()
    {
        var dto = BuildValidUsuario("U001", "u001@ucb.edu.bo", contrasena: "nouppercas@1");

        var result = await Sut.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Contraseña debe tener al menos una mayúscula");
    }

    [Test]
    public async Task Create_PasswordWithoutSpecialChar_ReturnsValidationError()
    {
        var dto = BuildValidUsuario("U001", "u001@ucb.edu.bo", contrasena: "NoSpecial123");

        var result = await Sut.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Contraseña debe tener al menos un carácter especial");
    }

    [Test]
    public async Task Create_PasswordWithoutNumber_ReturnsValidationError()
    {
        var dto = BuildValidUsuario("U001", "u001@ucb.edu.bo", contrasena: "NoNumber@!");

        var result = await Sut.Create(dto);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Contraseña debe tener al menos un número");
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsSuccess()
    {
        await Sut.Create(BuildValidUsuario("U001", "u001@ucb.edu.bo"));

        var result = await Sut.Login("u001@ucb.edu.bo", "Test@1234");

        result.IsSuccess.Should().BeTrue();
        result.Value.Carnet.Should().Be("U001");
    }

    [Test]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        await Sut.Create(BuildValidUsuario("U001", "u001@ucb.edu.bo"));

        var result = await Sut.Login("u001@ucb.edu.bo", "WrongPass@1");

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(Ardalis.Result.ResultStatus.Unauthorized);
    }

    [Test]
    public async Task Login_NonExistentEmail_ReturnsUnauthorized()
    {
        var result = await Sut.Login("nobody@ucb.edu.bo", "Test@1234");

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(Ardalis.Result.ResultStatus.Unauthorized);
    }

    [Test]
    public async Task Login_EmptyCredentials_ReturnsUnauthorized()
    {
        var result = await Sut.Login("", "");

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(Ardalis.Result.ResultStatus.Unauthorized);
    }

    [Test]
    public async Task Update_SameTelefonoSameUser_Succeeds()
    {
        await Sut.Create(BuildValidUsuario("U001", "u001@ucb.edu.bo", telefono: "77777777"));

        var result = await Sut.Update("U001", BuildValidUsuario("U001", "u001@ucb.edu.bo", telefono: "77777777", contrasena: null));

        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Update_DuplicateTelefonoOtherUser_ReturnsError()
    {
        await Sut.Create(BuildValidUsuario("U001", "u001@ucb.edu.bo", telefono: "77777777"));
        await Sut.Create(BuildValidUsuario("U002", "u002@ucb.edu.bo", telefono: "88888888"));

        var result = await Sut.Update("U002", BuildValidUsuario("U002", "u002@ucb.edu.bo", telefono: "77777777", contrasena: null));

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Teléfono ya registrado");
    }

    [Test]
    public async Task Delete_ExistingUser_SoftDeletes()
    {
        await Sut.Create(BuildValidUsuario("U001", "u001@ucb.edu.bo"));

        var result = await Sut.Delete("U001");

        result.IsSuccess.Should().BeTrue();
        Db.Usuarios.IgnoreQueryFilters().Single(u => u.Carnet == "U001").EstadoEliminado.Should().BeTrue();
    }

    private static UsuarioDto BuildValidUsuario(
        string  carnet,
        string  email,
        string? telefono  = null,
        string? contrasena = "Test@1234") => new()
    {
        Carnet          = carnet,
        Nombre          = "Test",
        ApellidoPaterno = "Usuario",
        Email           = email,
        Telefono        = telefono,
        Contrasena      = contrasena
    };
}
