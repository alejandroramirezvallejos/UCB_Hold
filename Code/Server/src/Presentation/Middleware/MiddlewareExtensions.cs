using Microsoft.AspNetCore.Builder;
using IMT_Reservas.Server.Presentations.Middleware;

namespace IMT_Reservas.Server.Presentation.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app)
    {
        app.UseGlobalExceptionMiddleware();
        return app;
    }

    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}
