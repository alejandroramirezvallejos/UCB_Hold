using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DataBaseExecuteQuery>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// 1. Add CORS service
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyOrigin", // Or any name you like
        policy =>
        {
            policy.WithOrigins("https://localhost:61829") // Replace with your Angular app's origin
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //NOTE: Verificacion de la conexion con la DB
    var configuration = app.Configuration;
    var db = new DataBaseExecuteQuery(configuration);
    bool exito = await db.ProbarConexion();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 2. Use the CORS middleware
app.UseCors("AllowMyOrigin"); // Make sure this is before app.UseRouting()

app.UseRouting();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();