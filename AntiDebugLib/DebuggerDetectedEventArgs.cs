using System;

namespace AntiDebugLib
{
    public class DebuggerDetectedEventArgs : EventArgs
    {
        /// <summary>
        /// The triggered check result.
        /// </summary>
        public CheckResult Result { get; }

        public DebuggerDetectedEventArgs(CheckResult result) => Result = result;
    }
}
