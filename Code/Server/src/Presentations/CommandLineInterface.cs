using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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
                    EjecutarPrograma(args);
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

            var solutionDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent;
            var testProject = Path.Combine(solutionDir.FullName, "Tests", "IMT_Reservas.Tests.csproj");
            var trxFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".trx");

            if (!File.Exists(testProject))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] No se encontró el proyecto de prueba en: {testProject}");
                Console.ResetColor();
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"test \"{testProject}\" --logger \"trx;LogFileName={trxFile}\" --nologo --verbosity quiet",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (!File.Exists(trxFile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] No se generó el archivo de resultados de las pruebas.");
                Console.ResetColor();
                return;
            }

            var results = new Dictionary<string, bool>();
            var doc = XDocument.Load(trxFile);
            XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

            var testDefinitions = doc.Descendants(ns + "UnitTest")
                .ToDictionary(
                    ut => ut.Attribute("id")?.Value,
                    ut => ut.Descendants(ns + "TestMethod").FirstOrDefault()?.Attribute("className")?.Value
                );

            var unitTestResults = doc.Descendants(ns + "UnitTestResult");

            foreach (var result in unitTestResults)
            {
                var outcome = result.Attribute("outcome")?.Value;
                var testId = result.Attribute("testId")?.Value;

                if (testId != null && testDefinitions.TryGetValue(testId, out var className) && className != null)
                {
                    var shortClassName = className.Split('.').LastOrDefault() ?? className;
                    if (!results.ContainsKey(shortClassName))
                    {
                        results[shortClassName] = true;
                    }
                    if (outcome != "Passed")
                    {
                        results[shortClassName] = false;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Resumen de pruebas ejecutadas");

            foreach (var result in results.OrderBy(r => r.Key))
            {
                Console.Write($"{result.Key,-40} ");
                if (result.Value)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("PASO");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FALLO");
                }
                Console.ResetColor();
            }

            File.Delete(trxFile);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Excepcion durante las pruebas: {ex.Message}");
            Console.ResetColor();
        }
    }
    private static void EjecutarPrograma(string[] args)
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

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMongoDb(builder.Configuration)
            .AddScoped<IComentarioService, ComentarioService>()
            .AddScoped<INotificacionService, NotificacionService>()
            .AddScoped<IComentarioRepository, ComentarioRepository>()
            .AddScoped<INotificacionRepository, NotificacionRepository>();

        builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;//para ayudar a josue
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services.AddScoped<IExecuteQuery, ExecuteQuery>();

        // Registrar servicios
        builder.Services.AddScoped<IAccesorioService, AccesorioService>();
        builder.Services.AddScoped<ICarreraService, CarreraService>();
        builder.Services.AddScoped<ICategoriaService, CategoriaService>();
        builder.Services.AddScoped<IComponenteService, ComponenteService>();
        builder.Services.AddScoped<IEmpresaMantenimientoService, EmpresaMantenimientoService>();
        builder.Services.AddScoped<IEquipoService, EquipoService>();
        builder.Services.AddScoped<IGaveteroService, GaveteroService>();
        builder.Services.AddScoped<IGrupoEquipoService, GrupoEquipoService>();
        builder.Services.AddScoped<IMantenimientoService, MantenimientoService>();
        builder.Services.AddScoped<IMuebleService, MuebleService>();
        builder.Services.AddScoped<IPrestamoService, PrestamoService>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<IComentarioService, ComentarioService>();
        builder.Services.AddScoped<INotificacionService, NotificacionService>();

        // Registrar repositorios
        builder.Services.AddScoped<IAccesorioRepository, AccesorioRepository>();
        builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
        builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        builder.Services.AddScoped<IComponenteRepository, ComponenteRepository>();
        builder.Services.AddScoped<IEmpresaMantenimientoRepository, EmpresaMantenimientoRepository>();
        builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
        builder.Services.AddScoped<IGaveteroRepository, GaveteroRepository>();
        builder.Services.AddScoped<IGrupoEquipoRepository, GrupoEquipoRepository>();
        builder.Services.AddScoped<IMantenimientoRepository, MantenimientoRepository>();
        builder.Services.AddScoped<IMuebleRepository, MuebleRepository>();
        builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
        builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();

        // MongoDB GridFS
        builder.Services.AddSingleton<MongoDB.Driver.IMongoClient>(sp =>
        {
            // Intenta obtener la cadena de conexión de MongoDb, si no existe, usa una por defecto
            var connectionString = builder.Configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27018";
            return new MongoDB.Driver.MongoClient(connectionString);
        });
        builder.Services.AddScoped<MongoDB.Driver.GridFS.IGridFSBucket>(sp =>
        {
            var mongoClient = sp.GetRequiredService<MongoDB.Driver.IMongoClient>();
            var database = mongoClient.GetDatabase("UCB_Hold"); // Nombre correcto de tu base de datos
            return new MongoDB.Driver.GridFS.GridFSBucket(database);
        });

        var app = builder.Build();
              
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