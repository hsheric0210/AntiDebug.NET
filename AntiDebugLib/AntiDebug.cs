using AntiDebugLib.Check;
using AntiDebugLib.Check.AntiHook;
using AntiDebugLib.Check.DebugFlags;
using AntiDebugLib.Check.Exploits;
using AntiDebugLib.Check.Timing;
using AntiDebugLib.Native;
using AntiDebugLib.Prevention;
using AntiDebugLib.Prevention.Exploits;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AntiDebugLib
{
    public class AntiDebug
    {
        public static event EventHandler<DebuggerDetectedEventArgs> DebuggerDetected;

        private static IReadOnlyList<CheckBase> checks;
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
                // checks
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

                // preventions
                new MalformedOutputDebugString(),
                new SandboxieCrasher(),
                new HideThreads(),
                new OverwriteDbgBreakPoint(),
                new OverwriteDbgUiConnectToDbg(),
                new OverwriteDbgUiRemoteBreakin(),
            };
        }

        public static void BeginChecks(int checkPeriod = 3000)
        {
            // run passive checks
            foreach (var check in checks)
            {
                if (check.CheckPassive())
                    DebuggerDetected?.Invoke(null, new DebuggerDetectedEventArgs(check.Name, check.Reliability));

                check.PreventPassive();
            }

            if (activeThread != null)
                return;

            activeThread = new Thread(new ParameterizedThreadStart(ActiveCheckProc));
            activeThreadCancel = new CancellationTokenSource();
            var param = new ActiveCheckProcParameter { activeChecks = checks, cancelToken = activeThreadCancel.Token, checkPeriod = checkPeriod };
            activeThread.Start(param);
        }

        struct ActiveCheckProcParameter
        {
            public IReadOnlyList<CheckBase> activeChecks;
            public CancellationToken cancelToken;
            public int checkPeriod;
        }

        static void ActiveCheckProc(object oparam)
        {
            var param = (ActiveCheckProcParameter)oparam;
            while (!param.cancelToken.IsCancellationRequested)
            {
                foreach (var check in param.activeChecks)
                {
                    if (check.CheckActive())
                        DebuggerDetected?.Invoke(null, new DebuggerDetectedEventArgs(check.Name, check.Reliability));

                    check.PreventActive();
                }

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

        public bool IsDebuggerPresent()
        {
            foreach (var check in checks)
            {
                if (check.CheckPassive() || check.CheckActive())
                    return true;
            }

            return false;
        }
    }
}
