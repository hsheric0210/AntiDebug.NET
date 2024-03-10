namespace AntiDebugLib.Check.Assembler
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L224
    /// </item>
    /// </list>
    /// </summary>
    public class PopfAndTrap : NativeCheckBase
    {
        public override string Name => "POPF and Trap";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
            => CallNativeCheck(NativeCheckType.Assembler_PopfAndTrap);
    }
}
