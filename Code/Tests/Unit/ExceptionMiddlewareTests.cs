using System.Net;
using System.Text.Json;
using FluentAssertions;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Presentation.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace IMT_Reservas.Tests.Unit;

[TestFixture]
public class ExceptionMiddlewareTests
{
    private Mock<ILogger<ExceptionMiddleware>> _loggerMock = null!;
    private DefaultHttpContext _httpContext = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    private async Task<Response<object>?> GetResponse()
    {
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(_httpContext.Response.Body);
        var text = await reader.ReadToEndAsync();
        return JsonSerializer.Deserialize<Response<object>>(text, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }

    [Test]
    public async Task Invoke_NoException_DoesNotModifyResponse()
    {
        // Arrange
        var middleware = new ExceptionMiddleware(innerHttpContext => Task.CompletedTask, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Test]
    public async Task Invoke_KeyNotFoundException_Returns404()
    {
        // Arrange
        var middleware = new ExceptionMiddleware(innerHttpContext => throw new KeyNotFoundException("Item not found"), _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(404);
        var response = await GetResponse();
        response.Should().NotBeNull();
        response!.Status.Should().Be(404);
        response.Errors.Should().Contain("Item not found");
    }

    [Test]
    public async Task Invoke_DbUpdateException_Returns409()
    {
        // Arrange
        var middleware = new ExceptionMiddleware(innerHttpContext => throw new DbUpdateException("DB Error"), _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(409);
        var response = await GetResponse();
        response.Should().NotBeNull();
        response!.Status.Should().Be(409);
        response.Errors.Should().Contain("Conflicto al guardar: registro duplicado o restricción violada");
    }

    [Test]
    public async Task Invoke_InvalidOperationException_Returns409()
    {
        // Arrange
        var middleware = new ExceptionMiddleware(innerHttpContext => throw new InvalidOperationException("Invalid Op"), _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(409);
        var response = await GetResponse();
        response.Should().NotBeNull();
        response!.Status.Should().Be(409);
        response.Errors.Should().Contain("Invalid Op");
    }

    [Test]
    public async Task Invoke_ArgumentException_Returns400()
    {
        // Arrange
        var middleware = new ExceptionMiddleware(innerHttpContext => throw new ArgumentException("Bad Arg"), _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(400);
        var response = await GetResponse();
        response.Should().NotBeNull();
        response!.Status.Should().Be(400);
        response.Errors.Should().Contain("Bad Arg");
    }

    [Test]
    public async Task Invoke_GenericException_Returns500()
    {
        // Arrange
        var middleware = new ExceptionMiddleware(innerHttpContext => throw new Exception("Unexpected error"), _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(500);
        var response = await GetResponse();
        response.Should().NotBeNull();
        response!.Status.Should().Be(500);
        response.Errors.Should().Contain("Error interno del servidor. Por favor intenta de nuevo más tarde.");
    }
}
