namespace AntiDebugLib.Check.Memory.AntiStepOver
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L224
    /// </item>
    /// </list>
    /// </summary>
    public class AntiStepWPM : NativeCheckBase
    {
        public override string Name => "Anti Step-over - Memory overwrite with WriteProcessMemory";

        public override CheckReliability Reliability => CheckReliability.Great;

        public override CheckResult CheckActive()
            => CallNativeCheck(NativeCheckType.Memory_AntiStepOver_WriteProcessMemory);
    }
}
