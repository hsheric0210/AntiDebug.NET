using static AntiDebugLib.Native.NativeDefs;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_DebugFlags.cpp#L130
    /// </item>
    /// <item>
    /// aegis :: https://github.com/rafael-santiago/aegis/blob/e648ef933db02dce89a73ed7fceb03cfb0dcb59b/src/native/windows/aegis_native.c#L119
    /// </item>
    /// <item>
    /// Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection/blob/585e33cbe57aa97725b3f98658944b01f1844562/src/Flags/Manual/NtGlobalFlag.cs#L15
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtGlobalFlag.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 1. NtGlobalFlag
    /// </item>
    /// </list>
    /// </summary>
    public class NtGlobalFlagPeb : CheckBase
    {
        public override string Name => "PEB NtGlobalFlag";

        public override CheckReliability Reliability => CheckReliability.Okay;

        private const uint FLG_HEAP_ENABLE_TAIL_CHECK = 0x10;
        private const uint FLG_HEAP_ENABLE_FREE_CHECK = 0x20;
        private const uint FLG_HEAP_VALIDATE_PARAMETERS = 0x40;

        public override bool CheckActive()
        {
            var ntGlobalFlag = _PEB.ParsePeb().NtGlobalFlag;
            Logger.Debug("NtGlobalFlag is {value:X}.", ntGlobalFlag);
            return (ntGlobalFlag & (FLG_HEAP_ENABLE_TAIL_CHECK | FLG_HEAP_ENABLE_FREE_CHECK | FLG_HEAP_VALIDATE_PARAMETERS)) != 0;
        }
    }
}
