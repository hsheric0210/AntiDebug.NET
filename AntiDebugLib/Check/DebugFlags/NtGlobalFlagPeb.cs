using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_DebugFlags.cpp#L130
    /// </summary>
    public class NtGlobalFlagPeb : CheckBase
    {
        public override string Name => "_PEB.NtGlobalFlag";

        public override CheckReliability Reliability => CheckReliability.Okay;

        private const uint FLG_HEAP_ENABLE_TAIL_CHECK = 0x10;
        private const uint FLG_HEAP_ENABLE_FREE_CHECK = 0x20;
        private const uint FLG_HEAP_VALIDATE_PARAMETERS = 0x40;

        public override bool CheckActive()
            => (_PEB.ParsePeb().NtGlobalFlag & (FLG_HEAP_ENABLE_TAIL_CHECK | FLG_HEAP_ENABLE_FREE_CHECK | FLG_HEAP_VALIDATE_PARAMETERS)) != 0;
    }
}
