using System;

namespace AntiDebugLib.Check.Memory
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/LowFragmentationHeap.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class LowFragmentationHeap : CheckBase
    {
        public override string Name => "HEAP->LowFragmentationHeap check";

        public override CheckReliability Reliability => CheckReliability.Okay;

        public override CheckResult CheckActive()
        {
            throw new NotImplementedException();
        }
    }
}
