using static AntiDebugLib.Native.NativeStructs;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/debug-flags.html#manual-checks-peb-beingdebugged-flag
    /// </item>
    /// <item>
    /// ShowStopper :: 
    /// </item>
    /// <item>
    /// aegis :: https://github.com/rafael-santiago/aegis/blob/e648ef933db02dce89a73ed7fceb03cfb0dcb59b/src/native/windows/aegis_native.c#L110
    /// </item>
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L105
    /// </item>
    /// <item>
    /// Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection/blob/585e33cbe57aa97725b3f98658944b01f1844562/src/Flags/Manual/PEBBeingDebugged.cs
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/BeingDebugged.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class BeingDebuggedPeb : CheckBase
    {
        public override string Name => "PEB->BeingDebugged";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
            => _PEB.ParsePeb().BeingDebugged != 0;
    }
}
