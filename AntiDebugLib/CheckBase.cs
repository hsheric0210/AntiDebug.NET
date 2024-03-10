using AntiDebugLib.Native;
using System.Runtime.InteropServices;

namespace AntiDebugLib
{
    public abstract class CheckBase : ModuleBase
    {
        /// <summary>
        /// The reliability of this anti-debugging  check
        /// </summary>
        public abstract CheckReliability Reliability { get; }

        /// <summary>
        /// Check whether a debugger or similar behavior is present.
        /// Only single execution of this function on the startup of the application is enough.
        /// </summary>
        /// <returns><c>true</c> if debugging action is present, <c>false</c> otherwise.</returns>
        public virtual CheckResult CheckPassive() => new CheckResult(Name, Reliability, CheckResultType.NotImplemented, null);

        /// <summary>
        /// Check whether a debugger or similar behavior is present.
        /// This function should be executed every seconds to have such effects.
        /// </summary>
        /// <returns><c>true</c> if debugging action is present, <c>false</c> otherwise.</returns>
        public virtual CheckResult CheckActive() => new CheckResult(Name, Reliability, CheckResultType.NotImplemented, null);

        protected CheckResult MakeResult(bool detected, object info = null) => detected ? DebuggerDetected(info) : DebuggerNotDetected(info);

        protected CheckResult DebuggerNotDetected(object info = null) => new CheckResult(Name, Reliability, CheckResultType.DebuggerNotDetected, info);
        protected CheckResult DebuggerDetected(object info = null) => new CheckResult(Name, Reliability, CheckResultType.DebuggerDetected, info);

        protected CheckResult Error(object info = null) => new CheckResult(Name, Reliability, CheckResultType.Error, info);
        protected CheckResult NtError(string function, NTSTATUS ntStatus, object additionalInfo = null) => new CheckResult(Name, Reliability, CheckResultType.Error, new NtErrorInfo(function, ntStatus, additionalInfo));
        protected CheckResult Win32Error(string function, object additionalInfo = null) => new CheckResult(Name, Reliability, CheckResultType.Error, new Win32ErrorInfo(function, Marshal.GetLastWin32Error(), additionalInfo));
    }
}
