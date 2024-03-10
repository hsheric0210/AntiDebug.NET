using AntiDebugLib.Native;

namespace AntiDebugLib
{
    public enum CheckReliability
    {
        None = 0,

        /// <summary>
        /// When the check with this reliability level is triggered,
        /// it DOES mean that an active debugger is present or attached to this process.
        /// </summary>
        Perfect = 1,

        /// <summary>
        /// When the check with this reliability level is triggered,
        /// it can mean a debugger is present with a high probability.
        /// </summary>
        Great = 2,

        /// <summary>
        /// When the check with this reliability level is triggered,
        /// it may mean a debugger is/was present.
        /// </summary>
        Okay = 3,

        /// <summary>
        /// When the check with this reliability level is triggered,
        /// it might mean a debugger is/was present, but it is not sure.
        /// </summary>
        Bad = 4,
    }

    public enum CheckResultType
    {
        DebuggerNotDetected = 0,
        DebuggerDetected = 1,
        Error = 2,
        NotImplemented = 3,
    }

    public readonly struct NtErrorInfo
    {
        /// <summary>
        /// The function name that failed with a NTSTATUS error.
        /// </summary>
        public string Function { get; }

        /// <summary>
        /// The NTSTATUS code.
        /// </summary>
        public NTSTATUS NtStatus { get; }

        /// <summary>
        /// Additional information about the function call.
        /// </summary>
        public object AdditionalInfo { get; }

        public NtErrorInfo(string function, NTSTATUS ntStatus, object additionalInfo)
        {
            Function = function;
            NtStatus = ntStatus;
            AdditionalInfo = additionalInfo;
        }

        public override string ToString() => $"Function {Function} returned NTSTATUS {NtStatus}" + (AdditionalInfo == null ? "" : " additional info: " + AdditionalInfo.ToString());
    }

    public readonly struct Win32ErrorInfo
    {
        /// <summary>
        /// The function name that failed with a Win32 error.
        /// </summary>
        public string Function { get; }

        /// <summary>
        /// The Win32 error code
        /// </summary>
        public int Win32Error { get; }

        /// <summary>
        /// Additional information about the function call.
        /// </summary>
        public object AdditionalInfo { get; }

        public Win32ErrorInfo(string function, int win32Error, object additionalInfo)
        {
            Function = function;
            Win32Error = win32Error;
            AdditionalInfo = additionalInfo;
        }

        public override string ToString() => $"Function {Function} returned Win32 error {Win32Error}" + (AdditionalInfo == null ? "" : " additional info: " + AdditionalInfo.ToString());
    }

    public readonly struct CheckResult
    {
        /// <summary>
        /// The check name.
        /// </summary>
        public string CheckName { get; }

        /// <summary>
        /// Reliability of this check.
        /// </summary>
        public CheckReliability Reliability { get; }

        /// <summary>
        /// Check result type.
        /// </summary>
        public CheckResultType Type { get; }

        /// <summary>
        /// Additional check result information.
        /// </summary>
        public object AdditionalInfo { get; }

        /// <summary>
        /// Does this check is 'triggered?'
        /// </summary>
        public bool IsDetected => Type == CheckResultType.DebuggerDetected;

        public CheckResult(string checkName, CheckReliability reliability, CheckResultType debuggerDetected, object additionalInfo)
        {
            CheckName = checkName;
            Reliability = reliability;
            Type = debuggerDetected;
            AdditionalInfo = additionalInfo;
        }
    }
}
