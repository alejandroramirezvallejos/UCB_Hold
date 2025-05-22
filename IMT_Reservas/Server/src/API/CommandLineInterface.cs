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
        builder.Services.AddScoped<IExecuteQuery, ExecuteQuery>();

        // Registrar los repositorios
        builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
        builder.Services.AddScoped<IGrupoEquipoRepository, GrupoEquipoRepository>();
        builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();

        // Equipos
        builder.Services.AddScoped<IObtenerEquipoConsulta, EquipoService>();
        builder.Services.AddScoped<ICrearEquipoComando, EquipoService>();
        builder.Services.AddScoped<IActualizarEquipoComando, EquipoService>();
        builder.Services.AddScoped<IEliminarEquipoComando, EquipoService>();

        // GrupoEquipo
        builder.Services.AddScoped<IObtenerGrupoEquipoConsulta, GrupoEquipoService>();
        builder.Services.AddScoped<IObtenerGruposEquiposConsulta, GrupoEquipoService>();
        builder.Services.AddScoped<ICrearGrupoEquipoComando, GrupoEquipoService>();
        builder.Services.AddScoped<IActualizarGrupoEquipoComando, GrupoEquipoService>();
        builder.Services.AddScoped<IEliminarGrupoEquipoComando, GrupoEquipoService>();

        // Categorias
        builder.Services.AddScoped<ICrearCategoriaComando, CategoriaService>();
        builder.Services.AddScoped<IObtenerCategoriaConsulta, CategoriaService>();
        builder.Services.AddScoped<IObtenerCategoriasConsulta, CategoriaService>();
        builder.Services.AddScoped<IActualizarCategoriaComando, CategoriaService>();
        builder.Services.AddScoped<IEliminarCategoriaComando, CategoriaService>();

        // Usuarios
        //builder.Services.AddScoped<ICrearUsuarioComando, UsuarioService>();
        //builder.Services.AddScoped<IObtenerUsuarioConsulta, UsuarioService>();
        //builder.Services.AddScoped<IActualizarUsuarioComando, UsuarioService>();
        //builder.Services.AddScoped<IEliminarUsuarioComando, UsuarioService>();

        // Prestamos
        builder.Services.AddScoped<ICrearPrestamoComando, PrestamoService>();
        builder.Services.AddScoped<IObtenerPrestamoConsulta, PrestamoService>();
        builder.Services.AddScoped<IActualizarPrestamoComando, PrestamoService>();
        builder.Services.AddScoped<IEliminarPrestamoComando, PrestamoService>();


        builder.Services.AddControllers();
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

        var app = builder.Build();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
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
