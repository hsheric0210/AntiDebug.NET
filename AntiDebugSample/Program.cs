using AntiDebugLib;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;

namespace AntiDebugSample
{
    internal class Program
    {
        private static object consoleLock = new object();

        static void Main(string[] args)
        {
            const string logTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] <{Module:lj}> {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: logTemplate, theme: AnsiConsoleTheme.Code)
            .WriteTo.File("antidebug.log", restrictedToMinimumLevel: LogEventLevel.Verbose, outputTemplate: logTemplate, encoding: System.Text.Encoding.UTF8)
            .CreateLogger();

            Log.Information("Arguments provided: {args}", string.Join(" ", args));

            AntiDebug.Logger = new SerilogDelegate(Log.Logger);
            AntiDebug.Initialize();
            AntiDebug.CheckFinished += AntiDebug_CheckFinished;
            AntiDebug.PreventionFinished += AntiDebug_PreventionFinished;

            AntiDebug.BeginChecks();

            while (true)
            {
                var line = Console.ReadLine();
                if (line == "gc")
                {
                    Log.Information("Performing GC.");
                    GC.Collect();
                }

                Console.WriteLine("Command input: " + line);
            }
        }

        private static void AntiDebug_CheckFinished(object sender, CheckResultEventArgs e)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("* Anti-Debug checks are performed.");
                Console.ForegroundColor = ConsoleColor.Gray;

                var paddingLength = e.Results.Select(r => r.CheckName.Length).Max() + 8;

                foreach (var result in e.Results)
                {
                    if (result.Reliability == CheckReliability.Perfect)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else if (result.Reliability == CheckReliability.Great)
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else if (result.Reliability == CheckReliability.Okay)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(result.CheckName);
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.Write(new string(' ', paddingLength - result.CheckName.Length));

                    if (result.Type == CheckResultType.DebuggerDetected)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Fail");
                    }
                    else if (result.Type == CheckResultType.Error)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("Error");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Pass");
                    }

                    if (result.AdditionalInfo != null)
                    {
                        Console.Write("    ");
                        Console.Write(result.AdditionalInfo.ToString());
                    }

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        private static void AntiDebug_PreventionFinished(object sender, PreventionResultEventArgs e)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("* Debugging prevention methods are applied.");
                Console.ForegroundColor = ConsoleColor.Gray;

                var paddingLength = e.Results.Select(r => r.PreventionName.Length).Max() + 8;

                foreach (var result in e.Results)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(result.PreventionName);

                    Console.Write(new string(' ', paddingLength - result.PreventionName.Length));

                    if (result.Type == PreventionResultType.Incompatible)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Incompatible");
                    }
                    else if (result.Type == PreventionResultType.Error)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("Error");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Applied");
                    }

                    if (result.AdditionalInfo != null)
                    {
                        Console.Write("    ");
                        Console.Write(result.AdditionalInfo.ToString());
                    }

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }
    }
}
