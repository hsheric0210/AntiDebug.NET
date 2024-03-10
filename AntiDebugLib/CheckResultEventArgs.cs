using System;
using System.Collections.Generic;

namespace AntiDebugLib
{
    public class CheckResultEventArgs : EventArgs
    {
        /// <summary>
        /// The array of check results.
        /// Contains all checks, not only triggered ones.
        /// </summary>
        public IReadOnlyList<CheckResult> Results { get; }

        public CheckResultEventArgs(IReadOnlyList<CheckResult> results) => Results = results;
    }
}
