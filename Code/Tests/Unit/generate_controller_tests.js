const fs = require('fs');

const controllers = [
    { name: 'Accesorio', dto: 'AccesorioDto' },
    { name: 'Carrera', dto: 'CarreraDto' },
    { name: 'Carrito', dto: 'CarritoDto' },
    { name: 'Categoria', dto: 'CategoriaDto' },
    { name: 'Componente', dto: 'ComponenteDto' },
    { name: 'Contrato', dto: 'ContratoDto' },
    { name: 'EmpresaMantenimiento', dto: 'EmpresaMantenimientoDto' },
    { name: 'Gavetero', dto: 'GaveteroDto' },
    { name: 'Mueble', dto: 'MuebleDto' },
    { name: 'Usuario', dto: 'UsuarioDto' },
    { name: 'Prestamo', dto: 'PrestamoDto' }
];

const template = (name, dto) => `using Ardalis.Result;
using FluentAssertions;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.${name};
using IMT_Reservas.Server.Presentation.Controllers.Implementations;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Runtime.Serialization;

namespace IMT_Reservas.Tests.Unit.Controllers;

[TestFixture]
public class ${name}ControllerTests
{
    private Mock<${name}Service> _serviceMock = null!;
    private ${name}Controller _controller = null!;

    [SetUp]
    public void SetUp()
    {
        // Use FormatterServices to bypass constructor if it's too complex or we don't know the exact arguments
        var mock = new Mock<${name}Service>((object[])null!);
        
        try 
        {
            // Just try with 5 nulls
            _serviceMock = new Mock<${name}Service>(null!, null!, null!, null!, null!);
        }
        catch 
        {
            try { _serviceMock = new Mock<${name}Service>(null!, null!, null!, null!); }
            catch {
                try { _serviceMock = new Mock<${name}Service>(null!, null!, null!); }
                catch {
                    try { _serviceMock = new Mock<${name}Service>(null!, null!); }
                    catch { _serviceMock = new Mock<${name}Service>(null!); }
                }
            }
        }
        _controller = new ${name}Controller(_serviceMock.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOkWithData()
    {
        var data = new List<${dto}> { new ${dto} { Id = 1 } };
        _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(Result<List<${dto}>>.Success(data));

        var result = await _controller.GetAll();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as Response<List<${dto}>>;
        value.Should().NotBeNull();
        value!.Value.Should().HaveCount(1);
    }

    [Test]
    public async Task Get_ExistingId_ReturnsOk()
    {
        var dto = new ${dto} { Id = 1 };
        _serviceMock.Setup(s => s.Get(1)).ReturnsAsync(Result<${dto}>.Success(dto));

        var result = await _controller.Get(1);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as Response<${dto}>;
        value!.Value.Should().Be(dto);
    }

    [Test]
    public async Task Create_ValidDto_ReturnsCreated()
    {
        var dto = new ${dto} { Id = 0 };
        var created = new ${dto} { Id = 1 };
        _serviceMock.Setup(s => s.Create(dto)).ReturnsAsync(Result<${dto}>.Success(created));

        var result = await _controller.Create(dto);

        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var value = createdResult.Value as Response<${dto}>;
        value!.Value.Should().Be(created);
    }

    [Test]
    public async Task Update_ValidDto_ReturnsOk()
    {
        var dto = new ${dto} { Id = 1 };
        _serviceMock.Setup(s => s.Update(1, dto)).ReturnsAsync(Result<${dto}>.Success(dto));

        var result = await _controller.Update(1, dto);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as Response<${dto}>;
        value!.Value.Should().Be(dto);
    }

    [Test]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.Delete(1)).ReturnsAsync(Result<object>.Success(null!));

        var result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
    }
}
`;

for (const c of controllers) {
    fs.writeFileSync(`${c.name}ControllerTests.cs`, template(c.name, c.dto));
}
