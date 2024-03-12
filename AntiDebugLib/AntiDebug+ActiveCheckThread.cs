using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace AntiDebugLib
{
    public partial class AntiDebug
    {
        private struct ActiveCheckThreadParameter
        {
            public IReadOnlyList<CheckBase> availableChecks;
            public CancellationToken cancelToken;
            public int executionPeriod;
        }

        [HandleProcessCorruptedStateExceptions]
        private static void ActiveCheckProc(object oparam)
        {
            var param = (ActiveCheckThreadParameter)oparam;
            while (!param.cancelToken.IsCancellationRequested)
            {
                var checkResults = new List<CheckResult>();
                foreach (var check in checks)
                {
                    try
                    {
                        var result = check.CheckActive();
                        if (result.Type != CheckResultType.NotImplemented)
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
                Thread.Sleep(param.executionPeriod);
            }
        }

    }
}
