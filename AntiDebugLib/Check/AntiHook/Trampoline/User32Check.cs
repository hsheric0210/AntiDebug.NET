namespace AntiDebugLib.Check.AntiHook
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/main/AntiCrack-DotNet/HooksDetection.cs
    /// </summary>
    internal class User32Check : TrampolineCheckBase
    {
        public override string Name => "Hooking: user32";

        protected override string DllName => "user32.dll";

        protected override string[] ProcNames => new string[]
        {
            "FindWindowW",
            "FindWindowA",
            "FindWindowExW",
            "FindWindowExA",
            "GetForegroundWindow",
            "GetWindowTextLengthA",
            "GetWindowTextA",
            "BlockInput"
        };

        protected override byte[] BadOpCodes => new byte[] { 0x90, 0xE9 };
    }
}
