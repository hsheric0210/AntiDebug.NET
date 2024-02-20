using System;
using System.Collections.Generic;

namespace AntiDebugLib
{
    public class CheckResultEventArgs : EventArgs
    {
        public IReadOnlyList<CheckResult> Results { get; }

        public CheckResultEventArgs(IReadOnlyList<CheckResult> results) => Results = results;
    }
}
