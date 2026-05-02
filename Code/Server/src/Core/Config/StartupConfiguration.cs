using Ardalis.Result.AspNetCore;
using IMT_Reservas.Server.Presentation.Middleware;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace IMT_Reservas.Server.Core.Config;

public static class StartupConfiguration
{
    public static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options => options.AddDefaultResultConvention())
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.PropertyNamingPolicy = null;
                opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => policy
                .WithOrigins("https://localhost:4200", "http://localhost:4200", "https://localhost:7216", "http://localhost:5190")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        builder.Services.AddMongoDb(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var conn = builder.Configuration.GetConnectionString("MongoDb")
                ?? throw new InvalidOperationException("Conexión MongoDB no configurada");
            return new MongoClient(conn);
        });

        builder.Services.AddScoped<IGridFSBucket>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var db = client.GetDatabase("UCB_Hold");
            return new GridFSBucket(db);
        });

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowAll");
        app.UseAppMiddleware();
        app.MapControllers();

        return app;
    }
}
