﻿using System;
using System.Runtime.InteropServices;
using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/debug-flags.html#kuser_shared_data
    /// </item>
    /// <item>
    /// https://www.geoffchappell.com/studies/windows/km/ntoskrnl/inc/api/ntexapi_x/kuser_shared_data/index.htm
    /// </item>
    /// </list>
    /// </summary>
    public class KdDebuggerAttached : CheckBase
    {
        public override string Name => "KUSER_SHARED_DATA.KdDebuggerAttached";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive() => (Marshal.ReadInt16(new IntPtr(0x7FFE02D4)) & 0b11) != 0;
    }
}