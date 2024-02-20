using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L110
    /// </item>
    /// <item>
    /// aegis :: https://github.com/rafael-santiago/aegis/blob/e648ef933db02dce89a73ed7fceb03cfb0dcb59b/src/native/windows/aegis_native.c#L96
    /// </item>
    /// <item>
    /// Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection/blob/585e33cbe57aa97725b3f98658944b01f1844562/src/Flags/IsDebuggerPresentFlag.cs#L10
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/IsDebuggerPresent.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.D.vii. IsDebuggerPresent
    /// </item>
    /// </list>
    /// </summary>
    public class IsDebuggerPresent : CheckBase
    {
        public override string Name => "kernel32!IsDebuggerPresent()";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive() => MakeResult(IsDebuggerPresent());
    }
}
