namespace IMT_Reservas.Server.Presentation.Middleware;

public static class GlobalExceptionMiddlewareExtensions
{
    public static void UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
