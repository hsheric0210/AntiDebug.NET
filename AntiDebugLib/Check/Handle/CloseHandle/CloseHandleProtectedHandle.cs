using AntiDebugLib.Native;
using AntiDebugLib.Utils;
using System;
using System.Runtime.InteropServices;

namespace AntiDebugLib.Check.Handle.CloseHandle
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L89
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/SetHandleInformation_API.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class CloseHandleProtectedHandle : CheckBase
    {
        public override string Name => "Close Handle: kernel32!CloseHandle (protected handle)";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        const uint HANDLE_FLAG_PROTECT_FROM_CLOSE = 0x00000002u;

        public override CheckResult CheckActive()
        {
            var random = new Random();
            var hMutex = Kernel32.CreateMutexA(IntPtr.Zero, false, StringUtils.RandomString(random.Next(15, 256), random));
            if (!Kernel32.SetHandleInformation(hMutex, HANDLE_FLAG_PROTECT_FROM_CLOSE, HANDLE_FLAG_PROTECT_FROM_CLOSE))
            {
                Logger.Warning("Failed to protect the handle {handle:X}. SetHandleInformation returned win32 error {errorcode}.", hMutex.ToHex(), Marshal.GetLastWin32Error());
                return Win32Error("SetHandleInformation", new { Handle = hMutex });
            }

            var beingDebugged = false;
            try
            {
                Kernel32.CloseHandle(hMutex);
            }
            catch
            {
                beingDebugged = true;
            }

            // Don't forget to clean up!
            if (!Kernel32.SetHandleInformation(hMutex, HANDLE_FLAG_PROTECT_FROM_CLOSE, 0) || !Kernel32.CloseHandle(hMutex))
                Logger.Warning("Failed to unprotect and close the handle {handle:X}. SetHandleInformation or CloseHandle returned win32 error {errorcode}.", hMutex.ToHex(), Marshal.GetLastWin32Error());

            return MakeResult(beingDebugged);
        }
    }
}
