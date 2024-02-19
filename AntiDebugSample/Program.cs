using AntiDebugLib;
using MyStealer;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;

namespace AntiDebugSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Console(theme: AnsiConsoleTheme.Code).CreateLogger();

            Log.Information("Arguments provided: {args}", string.Join(" ", args));

            AntiDebug.Logger = new SerilogDelegate(Log.Logger);
            AntiDebug.Initialize();
            AntiDebug.DebuggerDetected += AntiDebug_DebuggerDetected;

            AntiDebug.BeginChecks();

            while (true)
            {
                var line = Console.ReadLine();
                Console.WriteLine("User entered: " + line);
            }
        }

        private static void AntiDebug_DebuggerDetected(object sender, DebuggerDetectedEventArgs e)
        {
            Log.Warning("AntiDebug.NET detected (possible) debugger! Detection module: {name}, Reliability: {reliability}", e.CheckName, e.Reliability);
        }
    }
}
