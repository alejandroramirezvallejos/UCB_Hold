namespace IMT_Reservas.Server.Core.Config;

public static class CliInteractive
{
    public static void ModoInteractivo()
    {
        while (true)
        {
            Console.Write("ucb> ");
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.Trim().ToLower() == "exit")
                break;

            var args = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            CliCommands.ProcesarComando(args);
            Console.WriteLine();
        }
    }
}
