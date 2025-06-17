using Microsoft.AspNetCore.Mvc;
using Shared.Common;

public static class CommandLineInterface
{
    private static IHost? _webHost = null;

    public static void Iniciar(string[] args)
    {
        MostrarBanner();
        if (args.Length > 0)
        {
            ProcesarComando(args);
        }
        else
        {
            ModoInteractivo();
        }
    }
    private static void ModoInteractivo()
    {
        while (true)
        {
            Console.Write("ucb> ");
            var entrada = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entrada))
                continue;

            var argumentos = entrada.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var comando = argumentos[0].ToLower();
            if (comando == "exit")
                break;

            ProcesarComando(argumentos);
            Console.WriteLine();
        }
    }
    private static void ProcesarComando(string[] args)
    {
        var comando = args[0].ToLower();
        switch (comando)
        {
            case "tests":
                if (args.Length > 1 && args[1] == "--run")
                    EjecutarPruebas();
                else
                    MostrarAyuda();
                break;

            case "program":
                if (args.Length > 1 && args[1] == "--run")
                    EjecutarPrograma();
                else
                    MostrarAyuda();
                break;

            case "help":
                if (args.Length > 1 && args[1] == "--quino")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARNING] Deberias revisar la documentacion");
                    Console.ResetColor();
                }
                else
                    MostrarAyuda();
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Comando invalido");
                Console.ResetColor();
                break;
        }
    }

    private static void EjecutarPruebas()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Ejecutando pruebas...\n");
            Console.ResetColor();

            //TODO: Colocar las pruebas

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            //Console.WriteLine($"Pruebas completadas: {total} ejecutadas");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Excepcion durante las pruebas: {ex.Message}");
            Console.ResetColor();
        }
    }
    private static void EjecutarPrograma()
    {
        if (_webHost != null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARNING] El servidor ya está en ejecución");
            Console.ResetColor();
            return;
        }
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Iniciando programa...\n");
        Console.ResetColor();

        var builder = WebApplication.CreateBuilder();        
        builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services.AddScoped<ExecuteQuery>();

        // Registrar servicios
        builder.Services.AddScoped<AccesorioService>();
        builder.Services.AddScoped<CarreraService>();
        builder.Services.AddScoped<CategoriaService>();
        builder.Services.AddScoped<ComponenteService>();
        builder.Services.AddScoped<EmpresaMantenimientoService>();
        builder.Services.AddScoped<EquipoService>();
        builder.Services.AddScoped<GaveteroService>();
        builder.Services.AddScoped<GrupoEquipoService>();
        builder.Services.AddScoped<MantenimientoService>();
        builder.Services.AddScoped<MuebleService>();
        builder.Services.AddScoped<PrestamoService>();
        builder.Services.AddScoped<UsuarioService>();

        // Registrar repositorios
        builder.Services.AddScoped<AccesorioRepository>();
        builder.Services.AddScoped<CarreraRepository>();
        builder.Services.AddScoped<CategoriaRepository>();
        builder.Services.AddScoped<ComponenteRepository>();
        builder.Services.AddScoped<EmpresaMantenimientoRepository>();
        builder.Services.AddScoped<EquipoRepository>();
        builder.Services.AddScoped<GaveteroRepository>();
        builder.Services.AddScoped<GrupoEquipoRepository>();
        builder.Services.AddScoped<MantenimientoRepository>();
        builder.Services.AddScoped<MuebleRepository>();
        builder.Services.AddScoped<PrestamoRepository>();
        builder.Services.AddScoped<UsuarioRepository>();        var app = builder.Build();
        
        // Middleware de manejo de errores (debe ser el primero)
        app.UseMiddleware<ErrorHandlingMiddleware>();
        
        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();
        app.MapFallbackToFile("/index.html");

        Task.Run(() => app.Run());

        _webHost = app;
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("Servidor iniciado");
        Console.ResetColor();
    }

    private static void MostrarAyuda()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Comandos disponibles:");
        Console.WriteLine("  tests   --run  -> Ejecutar las pruebas");
        Console.WriteLine("  program --run  -> Iniciar el programa");
        Console.WriteLine("  exit           -> Salir del modo interactivo");
        Console.ResetColor();
    }

    private static void MostrarBanner()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(@"
        ██╗   ██╗ ██████╗██████╗      ██████╗██╗     ██╗
        ██║   ██║██╔════╝██╔══██╗    ██╔════╝██║     ██║
        ██║   ██║██║     ██████╔╝    ██║     ██║     ██║
        ██║   ██║██║     ██╔══██╗    ██║     ██║     ██║
        ╚██████╔╝╚██████╗██████╔╝    ╚██████╗███████╗██║
         ╚═════╝  ╚═════╝╚═════╝      ╚═════╝╚══════╝╚═╝

               UCB Command Line Interface 3.15
        ");
        Console.ResetColor();
        Console.WriteLine();
    }
}
