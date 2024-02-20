using System;

namespace AntiDebugLib
{
    public class DebuggerDetectedEventArgs : EventArgs
    {
        public CheckResult Result { get; }

        public DebuggerDetectedEventArgs(CheckResult result) => Result = result;
    }
}
