namespace IMT_Reservas.Server.Core.Config;

public static class CliPresentation
{
    public static void MostrarBanner()
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
    }

    public static void MostrarAyuda()
    {
        Console.WriteLine(@"
Comandos disponibles:
  program --run      Ejecutar servidor web
  tests --run        Ejecutar suite de pruebas
  help               Mostrar esta ayuda
  exit               Salir
        ");
    }

    public static void Error(string msg) => ImprimeColor(msg, ConsoleColor.Red);
    public static void Warning(string msg) => ImprimeColor(msg, ConsoleColor.Yellow);
    public static void Info(string msg) => ImprimeColor(msg, ConsoleColor.DarkBlue);

    private static void ImprimeColor(string msg, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ResetColor();
    }
}
