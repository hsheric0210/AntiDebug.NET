using AntiDebugLib.Check.System;
using AntiDebugLib.Check.Hooking;
using AntiDebugLib.Check.Hooking.Trampoline;
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
using AntiDebugLib.Check.Memory;
using AntiDebugLib.Check.Assembler;
using AntiDebugLib.Check.ExceptionHandler;

namespace AntiDebugLib
{
    public partial class AntiDebug
    {
        public static event EventHandler<DebuggerDetectedEventArgs> DebuggerDetected;
        public static event EventHandler<CheckResultEventArgs> CheckFinished;
        public static event EventHandler<PreventionResultEventArgs> PreventionFinished;

        private static IReadOnlyList<CheckBase> checks;
        private static IReadOnlyList<CheckBase> timingChecks;
        private static IReadOnlyList<PreventionBase> preventions;

        private static Thread[] threads;

        private static CancellationTokenSource threadCancel;

        /// <summary>
        /// The logging interface for the global AntiDebug.NET instance.
        /// </summary>
        /// <remarks>
        /// Note: Logger must be set before the <c>Initialize()</c> function call.
        /// </remarks>
        public static ILogger Logger { get; set; } = new DummyLogger();

        /// <summary>
        /// Initialize the AntiDebug.NET instance.
        /// You MUST call this method before calling <c>BeginChecks</c> or <c>IsDebuggerPresent</c> or you will get errors.
        /// </summary>
        /// <remarks>
        /// Preparation of the native methods and instantization of checks and preventions are done here.
        /// </remarks>
        public static void Initialize()
        {
            AntiDebugLibNative.Init(); // initialize (indirect) native calls and native checks

            checks = new List<CheckBase>()
            {
                //new NtSetInformationThreadHook(),
                //new QueryDebugObjectCountHook(),

                new NtDllCheck(),
                new Kernel32Check(),
                new KernelBaseCheck(),
                new User32Check(),
                new Win32UCheck(),

                //new NtSetInformationThreadHook(),
                //new QueryDebugObjectCountHook(),

                // todo: module bounds checks


                new Int3(),
                new Int3Long(),
                new Int2D(),
                new IceBP(),
                new StackSegmentRegister(),
                new InstructionCounting(),
                new PopfAndTrap(),
                new InstructionPrefixes(),
                new DebugRegisterModification(),

                new SEH(),
                //new UnhandledExceptionFilter(), // this will crash the program as the top level exception filter is overwritten by .NET process.
                new RaiseException(),
                new VEH(),

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
                new WudfIsDebuggerPresent(),
                //new WudfIsDebuggerPresent(),

                //new LowFragmentationHeap(),
                //new PageExecptionBreakpoint(),
                //new PageGuardBreakpoint(),
                //new WorkingSetShare(),
                //new WriteWatch(),

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
                //new ProcessJob(),
            };

            timingChecks = new List<CheckBase>()
            {
                new GetTickCountVariance(),
                new SleepDurationDecreased(),

                new RdtscDiffLocky(),
                new RdtscDiffVmExit(),

                //new NtYieldExecution(),
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

        /// <summary>
        /// Begin the anti-debug checks.
        /// Once called, it will first run all 'passive' preventions and checks.
        /// Then it will start threads for active checks. If the threads are already created, no new threads will be created.
        /// </summary>
        /// <remarks>
        /// You can receive the detection event when the (potential) debugging activity is detected, by <c>DebuggerDetected</c> event.
        /// </remarks>
        /// <param name="activeCheckPeriodMillis"></param>
        /// <param name="timingCheckPeriodMillis"></param>
        /// <param name="activePreventionPeriodMillis"></param>
        [HandleProcessCorruptedStateExceptions]
        public static void BeginChecks(
            int activeCheckPeriodMillis = 3000,
            int timingCheckPeriodMillis = 5000,
            int activePreventionPeriodMillis = 1000)
        {
            // run passive preventions
            var preventResults = new List<PreventionResult>();
            foreach (var prevention in preventions)
            {
                try
                {
                    var result = prevention.PreventPassive();
                    if (result.Type != PreventionResultType.NotImplemented)
                        preventResults.Add(result);
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
                    if (result.Type != CheckResultType.NotImplemented)
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

            if (threads != null)
                return;

            threadCancel = new CancellationTokenSource();

            threads = new Thread[3];
            threads[0] = StartThread(new ParameterizedThreadStart(ActiveCheckProc), new ActiveCheckThreadParameter { availableChecks = checks, cancelToken = threadCancel.Token, executionPeriod = activeCheckPeriodMillis });
            threads[1] = StartThread(new ParameterizedThreadStart(ActiveCheckProc), new ActiveCheckThreadParameter { availableChecks = timingChecks, cancelToken = threadCancel.Token, executionPeriod = timingCheckPeriodMillis });
            threads[2] = StartThread(new ParameterizedThreadStart(ActivePreventionProc), new ActivePreventionThreadParameter { availablePreventions = preventions, cancelToken = threadCancel.Token, executionPeriod = activePreventionPeriodMillis });
        }

        private static Thread StartThread(ParameterizedThreadStart proc, object param)
        {
            var thread = new Thread(proc);
            thread.Start(param);
            return thread;
        }

        /// <summary>
        /// Stops all anti-debug checks. All running anti-debug threads will be cancelled as soon as possible.
        /// </summary>
        /// <param name="waitUntilThreadsExit">Wait until all anti-debug threads do exit.</param>
        public static void EndChecks(bool waitUntilThreadsExit = false)
        {
            threadCancel.Cancel();

            if (waitUntilThreadsExit)
            {
                foreach (var thread in threads)
                    thread.Join();
            }

            threads = null;

            threadCancel.Dispose();
            threadCancel = null;
        }

        /// <summary>
        /// Apply all debugger prevention measures on-demand.
        /// Both 'passive' and 'active' checks are included.
        /// </summary>
        public void ApplyPreventions()
        {
            foreach (var prevention in preventions)
            {
                prevention.PreventPassive();
                prevention.PreventActive();
            }
        }

        /// <summary>
        /// Perform the anti-debug checks on-demand.
        /// Both 'passive' and 'active' checks are included.
        /// </summary>
        /// <returns><c>true</c> if (potential) debugging activity is detected, <c>false</c> otherwise.</returns>
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
