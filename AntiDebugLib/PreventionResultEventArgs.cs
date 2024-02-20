using System;
using System.Collections.Generic;

namespace AntiDebugLib
{
    public class PreventionResultEventArgs : EventArgs
    {
        public IReadOnlyList<PreventionResult> Results { get; }

        public PreventionResultEventArgs(IReadOnlyList<PreventionResult> results) => Results = results;
    }
}
