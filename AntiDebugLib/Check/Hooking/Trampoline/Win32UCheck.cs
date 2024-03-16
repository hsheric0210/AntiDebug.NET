namespace AntiDebugLib.Check.Hooking.Trampoline
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/main/AntiCrack-DotNet/HooksDetection.cs
    /// </summary>
    internal class Win32UCheck : TrampolineCheckBase
    {
        public override string Name => "Hooking: win32u";

        protected override string DllName => "win32u.dll";

        protected override string[] ProcNames => new string[]
        {
            "NtUserBlockInput",
            "NtUserFindWindowEx",
            "NtUserQueryWindow",
            "NtUserGetForegroundWindow"
        };

        protected override byte[] BadOpCodes => new byte[] { 255, 0x90, 0xE9 };
    }
}
