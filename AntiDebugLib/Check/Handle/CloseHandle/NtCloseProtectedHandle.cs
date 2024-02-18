﻿using AntiDebugLib.Utils;
using System;
using System.Runtime.InteropServices;
using static AntiDebugLib.Native.Kernel32;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.Exploits
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

        public override bool CheckActive()
        {
            var random = new Random();
            var hMutex = CreateMutexA(IntPtr.Zero, false, StringUtils.RandomString(random.Next(15, 256), random));
            if (!SetHandleInformation(hMutex, HANDLE_FLAG_PROTECT_FROM_CLOSE, HANDLE_FLAG_PROTECT_FROM_CLOSE))
            {
                Logger.Warning("Failed to call kernel32!SetHandleInformation. Win32 error {errorcode}.", Marshal.GetLastWin32Error());
                return false;
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

            // Don't forget to clean up!
            if (!SetHandleInformation(hMutex, HANDLE_FLAG_PROTECT_FROM_CLOSE, 0) || !NtClose(hMutex))
                Logger.Warning("Failed to unprotect and close the handle. Win32 error {errorcode}.", Marshal.GetLastWin32Error());

            return beingDebugged;
        }
    }
}