using System;
using System.Diagnostics;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L182
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiAnalysis/process.cpp
    /// </item>
    /// </list>
    /// </summary>
    internal class Processes : CheckBase
    {
        public override string Name => "Processes";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private readonly string[] processNames = new string[]
        {
            "VBoxService",                  // VirtualBox
            "VGAuthService",                // vmware
            "vmtoolsd",                     // vmware
            "vmwaretray",                   // vmware
            "vmwareuser",                   // vmware
            "vmacthlp",                     // vmware
            "vmusrvc",                      //
            "qemu-ga",                      // QEmu
            "vdagent",                      // SPICE guest tools
            "vdservice",                    // SPICE guest tools
            "ollydbg",                      // OllyDebug debugger
            "ProcessHacker",                // Process Hacker
            "tcpview",                      // Part of Sysinternals Suite
            "autoruns",                     // Part of Sysinternals Suite
            "autorunsc",                    // Part of Sysinternals Suite
            "filemon",                      // Part of Sysinternals Suite
            "procmon",                      // Part of Sysinternals Suite
            "regmon",                       // Part of Sysinternals Suite
            "procexp",                      // Part of Sysinternals Suite
            "idaq",                         // IDA Pro Interactive Disassembler
            "idaq64",                       // IDA Pro Interactive Disassembler
            "ImmunityDebugger",             // ImmunityDebugger
            "Wireshark",                    // Wireshark packet sniffer
            "dumpcap",                      // Network traffic dump tool
            "HookExplorer",                 // Find various types of runtime hooks
            "ImportREC",                    // Import Reconstructor
            "PETools",                      // PE Tool
            "LordPE",                       // LordPE
            "SysInspector",                 // ESET SysInspector
            "proc_analyzer",                // Part of SysAnalyzer iDefense
            "sysAnalyzer",                  // Part of SysAnalyzer iDefense
            "sniff_hit",                    // Part of SysAnalyzer iDefense
            "windbg",                       // Microsoft WinDbg
            "joeboxcontrol",                // Part of Joe Sandbox
            "joeboxserver",                 // Part of Joe Sandbox
            "joeboxserver",                 // Part of Joe Sandbox
            "ResourceHacker",               // Resource Hacker
            "x32dbg",                       // x32dbg
            "x64dbg",                       // x64dbg
            "Fiddler",                      // Fiddler
            "httpdebugger",                 // Http Debugger
            "cheatengine-i386",             // Cheat Engine
            "cheatengine-x86_64",           // Cheat Engine
            "cheatengine-x86_64-SSE4-AVX2", // Cheat Engine
            "frida-helper-32",              // Frida
            "frida-helper-64",              // Frida
            "prl_cc",                       // Parallels
            "prl_tools",                    // Parallels
            "vmsrvc",                       // VirtualPC
            "vmusrvc",                      // VirtualPC
            "xenservice",                   // Citrix Xen
        };

        public override CheckResult CheckPassive()
        {
            foreach (var process in Process.GetProcesses())
            {
                foreach (var name in processNames)
                {
                    if (string.Equals(process.ProcessName, name, StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.Information("Bad process {name} is running.", name);
                        return DebuggerDetected(new { Name = name });
                    }
                }
            }

            return DebuggerNotDetected();
        }
    }
}
