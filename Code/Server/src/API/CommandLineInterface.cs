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

        //Accesorios
        builder.Services.AddScoped<IObtenerAccesorioConsulta, AccesorioService>();
        builder.Services.AddScoped<ICrearAccesorioComando, AccesorioService>();
        builder.Services.AddScoped<IActualizarAccesorioComando, AccesorioService>();
        builder.Services.AddScoped<IEliminarAccesorioComando, AccesorioService>();

        // Carreras
        builder.Services.AddScoped<IObtenerCarreraConsulta, CarreraService>();
        builder.Services.AddScoped<ICrearCarreraComando, CarreraService>();
        builder.Services.AddScoped<IActualizarCarreraComando, CarreraService>();
        builder.Services.AddScoped<IEliminarCarreraComando, CarreraService>();

        // Categorias
        builder.Services.AddScoped<ICrearCategoriaComando, CategoriaService>();
        builder.Services.AddScoped<IObtenerCategoriaConsulta, CategoriaService>();
        builder.Services.AddScoped<IActualizarCategoriaComando, CategoriaService>();
        builder.Services.AddScoped<IEliminarCategoriaComando, CategoriaService>();

        // Componentes
        builder.Services.AddScoped<IObtenerComponenteConsulta, ComponenteService>();
        builder.Services.AddScoped<ICrearComponenteComando, ComponenteService>();
        builder.Services.AddScoped<IActualizarComponenteComando, ComponenteService>();
        builder.Services.AddScoped<IEliminarComponenteComando, ComponenteService>();

        // Empresas de Mantenimiento
        builder.Services.AddScoped<IObtenerEmpresaMantenimientoConsulta, EmpresaMantenimientoService>();
        builder.Services.AddScoped<ICrearEmpresaMantenimientoComando, EmpresaMantenimientoService>();
        builder.Services.AddScoped<IActualizarEmpresaMantenimientoComando, EmpresaMantenimientoService>();
        builder.Services.AddScoped<IEliminarEmpresaMantenimientoComando, EmpresaMantenimientoService>();

        // Equipos
        builder.Services.AddScoped<IObtenerEquipoConsulta, EquipoService>();
        builder.Services.AddScoped<ICrearEquipoComando, EquipoService>();
        builder.Services.AddScoped<IActualizarEquipoComando, EquipoService>();
        builder.Services.AddScoped<IEliminarEquipoComando, EquipoService>();

        // Gaveteros
        builder.Services.AddScoped<IObtenerGaveteroConsulta, GaveteroService>();
        builder.Services.AddScoped<ICrearGaveteroComando, GaveteroService>();
        builder.Services.AddScoped<IActualizarGaveteroComando, GaveteroService>();
        builder.Services.AddScoped<IEliminarGaveteroComando, GaveteroService>();

        // GrupoEquipo
        builder.Services.AddScoped<IObtenerGrupoEquipoConsulta, GrupoEquipoService>();
        builder.Services.AddScoped<ICrearGrupoEquipoComando, GrupoEquipoService>();
        builder.Services.AddScoped<IActualizarGrupoEquipoComando, GrupoEquipoService>();
        builder.Services.AddScoped<IEliminarGrupoEquipoComando, GrupoEquipoService>();

        // Mantenimientos
        builder.Services.AddScoped<IObtenerMantenimientoConsulta, MantenimientoService>();
        builder.Services.AddScoped<ICrearMantenimientoComando, MantenimientoService>();
        builder.Services.AddScoped<IEliminarMantenimientoComando, MantenimientoService>();

        // Muebles
        builder.Services.AddScoped<IObtenerMuebleConsulta, MuebleService>();
        builder.Services.AddScoped<ICrearMuebleComando, MuebleService>();
        builder.Services.AddScoped<IActualizarMuebleComando, MuebleService>();
        builder.Services.AddScoped<IEliminarMuebleComando, MuebleService>();

        // Prestamos
        builder.Services.AddScoped<ICrearPrestamoComando, PrestamoService>();
        builder.Services.AddScoped<IObtenerPrestamoConsulta, PrestamoService>();
        builder.Services.AddScoped<IEliminarPrestamoComando, PrestamoService>();

        // Usuarios
        builder.Services.AddScoped<ICrearUsuarioComando, UsuarioService>();
        builder.Services.AddScoped<IObtenerUsuarioConsulta, UsuarioService>();
        builder.Services.AddScoped<IActualizarUsuarioComando, UsuarioService>();
        builder.Services.AddScoped<IEliminarUsuarioComando, UsuarioService>();

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
