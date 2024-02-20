using AntiDebugLib.Native;
using System.Runtime.InteropServices;

namespace AntiDebugLib
{
    public abstract class PreventionBase : ModuleBase
    {
        /// <summary>
        /// Take action to prevent from being debugged.
        /// Only single execution of this function on the startup of the application is enough.
        /// </summary>
        /// <returns><c>true</c> if the action is successfully applied, <c>false</c> otherwise.</returns>
        public virtual PreventionResult PreventPassive() => new PreventionResult(Name, PreventionResultType.NotImplemented, null);

        /// <summary>
        /// Take action to prevent from being debugged.
        /// This function should be executed every seconds to have such effects.
        /// </summary>
        /// <returns><c>true</c> if the action is successfully applied, <c>false</c> otherwise.</returns>
        public virtual PreventionResult PreventActive() => new PreventionResult(Name, PreventionResultType.NotImplemented, null);

        protected PreventionResult MakeResult(bool detected, object info = null) => detected ? Applied(info) : Failed(info);
        protected PreventionResult Failed(object info = null) => new PreventionResult(Name, PreventionResultType.Failed, info);
        protected PreventionResult Applied(object info = null) => new PreventionResult(Name, PreventionResultType.Applied, info);

        protected PreventionResult Error(object info = null) => new PreventionResult(Name, PreventionResultType.Error, info);
        protected PreventionResult NtError(string function, NTSTATUS ntStatus, object additionalInfo = null) => new PreventionResult(Name, PreventionResultType.Error, new NtErrorInfo(function, ntStatus, additionalInfo));
        protected PreventionResult Win32Error(string function, object additionalInfo = null) => new PreventionResult(Name, PreventionResultType.Error, new Win32ErrorInfo(function, Marshal.GetLastWin32Error(), additionalInfo));

        protected PreventionResult Incompatible(object info = null) => new PreventionResult(Name, PreventionResultType.Incompatible, info);
    }
}
