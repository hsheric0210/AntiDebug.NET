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
    public partial class AntiDebug
    {
        private struct ActivePreventionThreadParameter
        {
            public IReadOnlyList<PreventionBase> availablePreventions;
            public CancellationToken cancelToken;
            public int executionPeriod;
        }

        [HandleProcessCorruptedStateExceptions]
        private static void ActivePreventionProc(object oparam)
        {
            var param = (ActivePreventionThreadParameter)oparam;
            while (!param.cancelToken.IsCancellationRequested)
            {
                var preventResults = new List<PreventionResult>();
                foreach (var prevention in param.availablePreventions)
                {
                    try
                    {
                        var result = prevention.PreventActive();
                        if (result.Type != PreventionResultType.NotImplemented)
                            preventResults.Add(result);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Error running the active prevention {name}.", prevention.Name);
                    }
                }

                PreventionFinished?.Invoke(null, new PreventionResultEventArgs(preventResults));
                Thread.Sleep(param.executionPeriod);
            }
        }
    }
}
