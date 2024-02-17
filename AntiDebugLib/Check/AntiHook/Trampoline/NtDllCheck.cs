namespace AntiDebugLib.Check.AntiHook
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/main/AntiCrack-DotNet/HooksDetection.cs
    /// </summary>
    internal class NtDllCheck : TrampolineCheckBase
    {
        public override string Name => "Hooking: ntdll";

        protected override string DllName => "ntdll.dll";

        protected override string[] ProcNames => new string[]
        {
            "NtQueryInformationProcess",
            "NtSetInformationThread",
            "NtClose",
            "NtGetContextThread",
            "NtQuerySystemInformation"
        };

        protected override byte[] BadOpCodes => new byte[] { 255, 0x90, 0xE9 };
    }
}
