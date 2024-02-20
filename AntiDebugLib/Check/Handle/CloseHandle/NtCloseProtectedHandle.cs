using AntiDebugLib.Utils;
using System;
using System.Runtime.InteropServices;
using static AntiDebugLib.Native.Kernel32;
using static AntiDebugLib.Native.NtDll;
using AntiDebugLib.Native;

namespace AntiDebugLib.Check.Handle.CloseHandle
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L89
    /// </item>
    /// </list>
    /// </summary>
    public class NtCloseProtectedHandle : CheckBase
    {
        public override string Name => "Close Handle: ntdll!NtClose (protected handle)";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        const uint HANDLE_FLAG_PROTECT_FROM_CLOSE = 0x00000002u;

        public override CheckResult CheckActive()
        {
            var random = new Random();
            var hMutex = CreateMutexA(IntPtr.Zero, false, StringUtils.RandomString(random.Next(15, 256), random));
            if (!SetHandleInformation(hMutex, HANDLE_FLAG_PROTECT_FROM_CLOSE, HANDLE_FLAG_PROTECT_FROM_CLOSE))
            {
                Logger.Warning("Failed to protect the handle {handle:X}. SetHandleInformation returned win32 error {errorcode}.", hMutex.ToHex(), Marshal.GetLastWin32Error());
                return Win32Error("SetHandleInformation", new { Handle = hMutex });
            }

            var beingDebugged = false;
            try
            {
                NtClose(hMutex);
            }
            catch
            {
                beingDebugged = true;
            }

            NTSTATUS status;
            // Don't forget to clean up!
            if (!SetHandleInformation(hMutex, HANDLE_FLAG_PROTECT_FROM_CLOSE, 0))
                Logger.Warning("Failed to unprotect the handle {handle:X}. SetHandleInformation returned win32 error {errorcode}.", hMutex.ToHex(), Marshal.GetLastWin32Error());
            else if ((status = NtClose(hMutex)) != 0x0) // STATUS_SUCCESS
                Logger.Warning("Failed to close the handle {handle:X}. NtClose returned NTSTATUS {errorcode}.", hMutex.ToHex(), status);

            return MakeResult(beingDebugged);
        }
    }
}
