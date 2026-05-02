using System.Diagnostics;
using System.Xml.Linq;

namespace IMT_Reservas.Server.Core.Config;

public static class CliCommands
{
    private static WebApplication? _app;

    public static void ProcesarComando(string[] args)
    {
        var cmd = args[0].ToLower();
        switch (cmd)
        {
            case "program" when args.Length > 1 && args[1] == "--run":
                EjecutarPrograma();
                break;
            case "tests" when args.Length > 1 && args[1] == "--run":
                EjecutarTests();
                break;
            case "help":
                CliPresentation.MostrarAyuda();
                break;
            default:
                CliPresentation.Error("[ERROR] Comando inválido");
                break;
        }
    }

    private static void EjecutarPrograma()
    {
        if (_app != null)
        {
            CliPresentation.Warning("[WARNING] Servidor ya activo");
            return;
        }
        CliPresentation.Info("Iniciando servidor...\n");
        try
        {
            _app = StartupConfiguration.Build(Array.Empty<string>());
            _app.Run();
        }
        catch (Exception ex)
        {
            CliPresentation.Error($"[ERROR] {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static void EjecutarTests()
    {
        CliPresentation.Info("Ejecutando tests...\n");
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? ".";
            var testProj = Path.Combine(baseDir, "..", "..", "..", "..", "Tests.csproj");
            var trxDir = Path.Combine(baseDir, "test-results");
            Directory.CreateDirectory(trxDir);
            var trxFile = Path.Combine(trxDir, $"results-{DateTime.Now:yyyyMMdd-HHmmss}.trx");

            var proc = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"test \"{testProj}\" --logger \"trx;LogFileName={trxFile}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

            proc?.WaitForExit();

            if (!File.Exists(trxFile))
            {
                CliPresentation.Warning("No test results found");
                return;
            }

            var doc = XDocument.Load(trxFile);
            var results = new Dictionary<string, bool>();
            var ns = XNamespace.Get("http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

            var testDefs = doc.Descendants(ns + "UnitTest")
                .ToDictionary(
                    ut => ut.Attribute("id")?.Value ?? "",
                    ut => ut.Descendants(ns + "TestMethod").FirstOrDefault()?.Attribute("className")?.Value ?? ""
                );

            var unitTests = doc.Descendants(ns + "UnitTestResult");
            foreach (var test in unitTests)
            {
                var outcome = test.Attribute("outcome")?.Value;
                var testId = test.Attribute("testId")?.Value;
                if (testId != null && testDefs.TryGetValue(testId, out var className) && className != null)
                {
                    var name = className.Split('.').LastOrDefault() ?? className;
                    if (!results.ContainsKey(name))
                        results[name] = true;
                    if (outcome != "Passed")
                        results[name] = false;
                }
            }

            Console.WriteLine("\nResumen:");
            foreach (var (name, passed) in results.OrderBy(r => r.Key))
            {
                Console.Write($"{name,-40} ");
                if (passed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ PASS");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("✗ FAIL");
                }
                Console.ResetColor();
            }

            File.Delete(trxFile);
        }
        catch (Exception ex)
        {
            CliPresentation.Error($"[ERROR] {ex.Message}");
        }
    }
}
