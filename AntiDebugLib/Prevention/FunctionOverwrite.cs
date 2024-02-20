using AntiDebugLib.Native;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static AntiDebugLib.Native.Kernel32;
using static AntiDebugLib.Native.NativeDefs;

namespace AntiDebugLib.Prevention
{
    public abstract class FunctionOverwrite : PreventionBase
    {
        protected PreventionResult OverwriteFunction(IntPtr proc, byte[] instr)
        {
            var length = (uint)instr.Length;
            if (!VirtualProtect(proc, new IntPtr(length), MemoryProtection.EXECUTE_READWRITE, out var oldProtect))
            {
                Logger.Warning("Failed to make address {address} RWX. VirtualProtect returned Win32 error {error}.", proc.ToHex(), Marshal.GetLastWin32Error());
                return Win32Error("VirtualProtect");
            }

            if (!WriteProcessMemory(Process.GetCurrentProcess().SafeHandle, proc, instr, length, 0))
            {
                Logger.Warning("Failed to overwrite address {address} RWX. WriteProcessMemory returned Win32 error {error}.", proc.ToHex(), Marshal.GetLastWin32Error());
                return Win32Error("WriteProcessMemory");
            }

            if (!VirtualProtect(proc, new IntPtr(length), oldProtect, out var oldProtect2))
                Logger.Warning("Failed to make address {address} back to {protect}. VirtualProtect returned Win32 error {error}.", proc.ToHex(), oldProtect, Marshal.GetLastWin32Error());

            return Applied();
        }
    }
}
