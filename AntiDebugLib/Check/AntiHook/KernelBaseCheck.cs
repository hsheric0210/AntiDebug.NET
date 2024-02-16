namespace AntiDebugLib.Check.AntiHook
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/main/AntiCrack-DotNet/HooksDetection.cs
    /// </summary>
    internal class KernelBaseCheck : HookCheckBase
    {
        public override string Name => "Hooking: KernelBase";

        protected override string DllName => "KernelBase.dll";

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

        protected override byte[] BadOpCodes => new byte[] { 255, 0x90, 0xE9 };
    }
}
