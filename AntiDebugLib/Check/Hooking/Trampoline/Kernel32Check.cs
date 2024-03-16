namespace AntiDebugLib.Check.Hooking.Trampoline
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/main/AntiCrack-DotNet/HooksDetection.cs
    /// </summary>
    internal class Kernel32Check : TrampolineCheckBase
    {
        public override string Name => "Hooking: kernel32";

        protected override string DllName => "kernel32.dll";

        protected override string[] ProcNames => new string[]
        {
            "IsDebuggerPresent",
            "CheckRemoteDebuggerPresent",
            "GetThreadContext",
            "CloseHandle",
            "OutputDebugStringA",
            "OutputDebugStringW",
            "GetTickCount",
            "SetHandleInformation"
        };

        protected override byte[] BadOpCodes => new byte[] { 0x90, 0xE9 };
    }
}
