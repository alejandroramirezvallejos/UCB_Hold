using FluentAssertions;
using IMT_Reservas.Server.Application.Features.Notificacion;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Tests.Helpers;

namespace IMT_Reservas.Tests.Integration;

[TestFixture]
internal class NotificacionServiceTests : ServiceTest<NotificacionService>
{
    protected override NotificacionService CreateService(ApplicationDbContext db) =>
        new(new NotificacionRepository(db));

    [Test]
    public async Task Create_GuardaYListaPorCarnet()
    {
        await Sut.Create(
            "U001",
            TipoNotificacion.PrestamoAprobado,
            "Préstamo aprobado",
            "contenido"
        );

        var result = await Sut.GetByCarnet("U001");

        result.IsSuccess.Should().BeTrue();
        result
            .Value.Should()
            .ContainSingle(n => n.Titulo == "Préstamo aprobado" && n.Leido == false);
    }

    [Test]
    public async Task GetByCarnet_SoloDelUsuario()
    {
        await Sut.Create("U001", TipoNotificacion.PrestamoAprobado, "Para U001");
        await Sut.Create("U002", TipoNotificacion.PrestamoRechazado, "Para U002");

        var result = await Sut.GetByCarnet("U001");

        result.Value.Should().OnlyContain(n => n.CarnetUsuario == "U001");
    }

    [Test]
    public async Task CreateForAdmins_NotificaSoloAdministradores()
    {
        Db.Usuarios.Add(BuildUsuario("A001", TipoUsuario.Administrador));
        Db.Usuarios.Add(BuildUsuario("E001", TipoUsuario.Estudiante));
        await Db.SaveChangesAsync();

        await Sut.CreateForAdmins(
            TipoNotificacion.AdminNuevoPrestamo,
            "Nueva reserva",
            "detalle"
        );

        (await Sut.GetByCarnet("A001")).Value.Should().HaveCount(1);
        (await Sut.GetByCarnet("E001")).Value.Should().BeEmpty();
    }

    private static Usuario BuildUsuario(string carnet, TipoUsuario rol) =>
        new()
        {
            Carnet = carnet,
            Nombre = "Test",
            ApellidoPaterno = "User",
            ApellidoMaterno = "User",
            Email = carnet + "@ucb.edu.bo",
            Contrasena = "x",
            Telefono = carnet,
            Rol = rol,
            IdCarrera = 1,
        };
}
