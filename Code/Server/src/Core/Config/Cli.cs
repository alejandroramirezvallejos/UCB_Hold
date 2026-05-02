using IMT_Reservas.Server.Core.Config;

public static class Cli
{
    public static void Iniciar(string[] args)
    {
        CliPresentation.MostrarBanner();

        if (args.Length > 0)
            CliCommands.ProcesarComando(args);
        else
            CliInteractive.ModoInteractivo();
    }
}
