using System;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.AntiDebugLibNative;

namespace AntiDebugLib.Check.AntiHook
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/main/AntiCrack-DotNet/HooksDetection.cs
    /// </item>
    /// </list>
    /// </summary>
    internal abstract class TrampolineCheckBase : CheckBase
    {
        protected abstract string DllName { get; }

        protected abstract string[] ProcNames { get; }

        protected abstract byte[] BadOpCodes { get; }

        public override CheckReliability Reliability => CheckReliability.Okay;

        public override CheckResult CheckPassive()
        {
            try
            {
                var handle = MyGetModuleHandle(DllName);
                foreach (var proc in ProcNames)
                {
                    var procAddr = MyGetProcAddress(handle, proc);
                    var ops = new byte[1];
                    Marshal.Copy(procAddr, ops, 0, 1);

                    foreach (var badOps in BadOpCodes)
                    {
                        if (ops[0] == badOps)
                        {
                            var opcode = "0x" + badOps.ToString("X2");
                            Logger.Debug("Found bad opcode {op} from function {name}.", opcode, proc);
                            return DebuggerDetected(new { Function = proc, OpCode = opcode });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Failed to check procedure hooking for dll: {dllName}", DllName);
            }

            return DebuggerNotDetected();
        }
    }
}
