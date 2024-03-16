namespace AntiDebugLib.Check.Memory
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research::https://anti-debug.checkpoint.com/techniques/process-memory.html#ntqueryvirtualmemory
    /// </item>
    /// </list>
    /// </summary>
    public class WorkingSetShare : NativeCheckBase
    {
        public override string Name => "Memory Working Set Share Flags";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
            => CallNativeCheck(NativeCheckType.Memory_WorkingSetShare);
    }
}
