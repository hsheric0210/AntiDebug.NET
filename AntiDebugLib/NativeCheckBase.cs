using AntiDebugLib.Native;

namespace AntiDebugLib
{
    public abstract class NativeCheckBase : CheckBase
    {
        protected CheckResult CallNativeCheck(NativeCheckType checkType)
        {
            var result = AntiDebugLibNative.PerformNativeCheck(checkType);
            if (unchecked((long)result) == -1)
                return new CheckResult(Name, Reliability, CheckResultType.NotImplemented, null);

            return MakeResult((result & 0x1) == 1);
        }
    }
}
