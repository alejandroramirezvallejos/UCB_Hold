using FluentAssertions;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Tests.Unit;

[TestFixture]
internal class DbContextMappingTests
{
    private static ApplicationDbContext BuildContext()
        => new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Test]
    public void Contrato_DoesNotMap_EstadoEliminado()
    {
        using var db = BuildContext();

        var prop = db.Model.FindEntityType(typeof(Contrato))!.FindProperty("EstadoEliminado");

        prop.Should().BeNull("contratos no tiene una columna estado_eliminado y contrato tiene hard-delete");
    }

    [Test]
    public void Usuario_DoesNotMap_Id()
    {
        using var db = BuildContext();

        var prop = db.Model.FindEntityType(typeof(Usuario))!.FindProperty("Id");

        prop.Should().BeNull("usuarios está identificado por el carnet, no por el id");
    }
}
