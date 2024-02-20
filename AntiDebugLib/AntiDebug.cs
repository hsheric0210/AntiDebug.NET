using AntiDebugLib.Check;
using AntiDebugLib.Check.AntiHook;
using AntiDebugLib.Check.DebugFlags;
using AntiDebugLib.Check.Handle;
using AntiDebugLib.Check.Handle.CloseHandle;
using AntiDebugLib.Check.Timing;
using AntiDebugLib.Native;
using AntiDebugLib.Prevention;
using AntiDebugLib.Prevention.Exploits;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace AntiDebugLib
{
    public class AntiDebug
    {
        public static event EventHandler<DebuggerDetectedEventArgs> DebuggerDetected;
        public static event EventHandler<CheckResultEventArgs> CheckFinished;
        public static event EventHandler<PreventionResultEventArgs> PreventionFinished;

        private static IReadOnlyList<CheckBase> checks;
        private static IReadOnlyList<PreventionBase> preventions;
        private static Thread activeThread;
        private static CancellationTokenSource activeThreadCancel;

        /// <summary>
        /// Note: Logger must be set before the <c>Initialize()</c> function call.
        /// </summary>
        public static ILogger Logger { get; set; } = new DummyLogger();

        public static void Initialize()
        {
            AntiDebugLibNative.Init(); // initialize (indirect) native calls and native checks

            checks = new List<CheckBase>()
            {
                new NtDllCheck(),
                new Kernel32Check(),
                new KernelBaseCheck(),
                new User32Check(),
                new Win32UCheck(),

                new BeingDebuggedPeb(),
                new CheckRemoteDebuggerPresent(),
                new DebuggerBreak(),
                new DebugObjectCount(),
                new HardwareRegisterBreakpoints(),
                new HeapFlagsPeb(),
                new HeapFlagsRtlQueryProcessDebugInformation(),
                new HeapFlagsRtlQueryProcessHeapInformation(),
                new IsDebuggerAttached(),
                new IsDebuggerPresent(),
                new KdDebuggerAttached(),
                new NtGlobalFlagPeb(),
                new OutputDebugString(),
                new ProcessDebugFlags(),
                new ProcessDebugObject(),
                new ProcessDebugPort(),

                new CloseHandleInvalidHandle(),
                new CloseHandleProtectedHandle(),
                new NtCloseInvalidHandle(),
                new NtCloseProtectedHandle(),
                new LoadThenOpen(),
                new ModuleFileOpen(),
                new OpenCsrss(),

                new DriverIntegrity(),
                new Files(),
                new KernelDebugger(),
                new LoadedModules(),
                new ModelName(),
                new Processes(),
                new Services(),
                new WmiPortConnectors(),

                new GetTickCountVariance(),
                new SleepDurationDecreased(),
            };

            preventions = new List<PreventionBase>()
            {
                new MalformedOutputDebugString(),
                new SandboxieCrasher(),
                new HideThreads(),
                new OverwriteDbgBreakPoint(),
                new OverwriteDbgUiConnectToDbg(),
                new OverwriteDbgUiRemoteBreakin(),
            };
        }

        [HandleProcessCorruptedStateExceptions]
        public static void BeginChecks(int activeCheckPeriodMillis = 3000)
        {
            // run passive preventions
            var preventResults = new List<PreventionResult>();
            foreach (var prevention in preventions)
            {
                try
                {
                    preventResults.Add(prevention.PreventPassive());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error running the passive prevention {name}.", prevention.Name);
                }
            }

            PreventionFinished?.Invoke(null, new PreventionResultEventArgs(preventResults));

            // run passive checks
            var checkResults = new List<CheckResult>();
            foreach (var check in checks)
            {
                try
                {
                    var result = check.CheckPassive();
                    checkResults.Add(result);
                    if (result.Type == CheckResultType.DebuggerDetected)
                        DebuggerDetected?.Invoke(null, new DebuggerDetectedEventArgs(result));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error running the passive check {name}.", check.Name);
                }
            }

            CheckFinished?.Invoke(null, new CheckResultEventArgs(checkResults));

            if (activeThread != null)
                return;

            activeThread = new Thread(new ParameterizedThreadStart(ActiveCheckProc));
            activeThreadCancel = new CancellationTokenSource();
            var param = new ActiveCheckProcParameter { activeChecks = checks, activePreventions = preventions, cancelToken = activeThreadCancel.Token, checkPeriod = activeCheckPeriodMillis };
            activeThread.Start(param);
        }

        struct ActiveCheckProcParameter
        {
            public IReadOnlyList<CheckBase> activeChecks;
            public IReadOnlyList<PreventionBase> activePreventions;
            public CancellationToken cancelToken;
            public int checkPeriod;
        }

        [HandleProcessCorruptedStateExceptions]
        static void ActiveCheckProc(object oparam)
        {
            var param = (ActiveCheckProcParameter)oparam;
            while (!param.cancelToken.IsCancellationRequested)
            {
                // run active preventions
                var preventResults = new List<PreventionResult>();
                foreach (var prevention in param.activePreventions)
                {
                    try
                    {
                        preventResults.Add(prevention.PreventActive());
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error running the active prevention {name}.", prevention.Name);
                    }
                }

                PreventionFinished?.Invoke(null, new PreventionResultEventArgs(preventResults));

                // run passive checks
                var checkResults = new List<CheckResult>();
                foreach (var check in checks)
                {
                    try
                    {
                        var result = check.CheckActive();
                        checkResults.Add(result);
                        if (result.Type == CheckResultType.DebuggerDetected)
                            DebuggerDetected?.Invoke(null, new DebuggerDetectedEventArgs(result));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error running the active check {name}.", check.Name);
                    }
                }

                CheckFinished?.Invoke(null, new CheckResultEventArgs(checkResults));
                Thread.Sleep(param.checkPeriod);
            }
        }

        public static void EndChecks()
        {
            activeThreadCancel.Cancel();

            activeThread = null;

            activeThreadCancel.Dispose();
            activeThreadCancel = null;
        }

        public void ApplyPreventions()
        {
            foreach (var prevention in preventions)
            {
                prevention.PreventPassive();
                prevention.PreventActive();
            }
        }

        public bool IsDebuggerPresent()
        {
            foreach (var check in checks)
            {
                if (check.CheckPassive().Type == CheckResultType.DebuggerDetected
                    || check.CheckActive().Type == CheckResultType.DebuggerDetected)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
