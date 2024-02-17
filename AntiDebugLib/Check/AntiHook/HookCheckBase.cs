using System;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.AntiDebugLibNative;

namespace AntiDebugLib.Check.AntiHook
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/main/AntiCrack-DotNet/HooksDetection.cs
    /// </summary>
    internal abstract class HookCheckBase : CheckBase
    {
        protected abstract string DllName { get; }

        protected abstract string[] ProcNames { get; }

        protected abstract byte[] BadOpCodes { get; }

        public override CheckReliability Reliability => CheckReliability.Okay;

        public override bool CheckPassive()
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
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Information(ex, "Failed to check procedure hooking for dll: {dllName}", DllName);
            }

            return false;
        }
    }
}
