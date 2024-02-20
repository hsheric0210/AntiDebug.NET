using AntiDebugLib.Native;

namespace AntiDebugLib
{
    public enum CheckReliability
    {
        None = 0,
        Perfect = 1,
        Great = 2,
        Okay = 3,
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
        public string Function { get; }
        public NTSTATUS NtStatus { get; }
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
        public string Function { get; }
        public int Win32Error { get; }
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
        public string CheckName { get; }
        public CheckReliability Reliability { get; }
        public CheckResultType Type { get; }
        public object AdditionalInfo { get; }

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
