﻿using static AntiDebugLib.Native.Kernel32.Delegates;
using StealthModule;
using StealthModule.MemoryModule;

namespace AntiDebugLib.Native
{
    internal static partial class Kernel32
    {
        private static LocalMemoryModule mappedKernel32;

        #region Properties

        internal static SetHandleInformation SetHandleInformation { get; private set; }

        internal static IsDebuggerPresent IsDebuggerPresent { get; private set; }

        internal static CheckRemoteDebuggerPresent CheckRemoteDebuggerPresent { get; private set; }

        internal static WriteProcessMemory WriteProcessMemory { get; private set; }

        internal static OpenThread OpenThread { get; private set; }

        internal static GetTickCount GetTickCount { get; private set; }

        internal static OutputDebugStringA OutputDebugStringA { get; private set; }

        internal static GetCurrentThread GetCurrentThread { get; private set; }

        internal static GetThreadContext GetThreadContext { get; private set; }

        internal static OpenProcess OpenProcess { get; private set; }

        internal static VirtualProtect VirtualProtect { get; private set; }

        internal static GetProcAddress GetProcAddress { get; private set; }

        #endregion

        internal static void InitNativesUnhooked()
        {
            /*var kernel32Path = ExportResolver.GetModuleFullName("kernel32.dll");
            var kernel32Bytes = File.ReadAllBytes(kernel32Path);
            mappedKernel32 = new MemoryModule(kernel32Bytes);
            var resolver = mappedKernel32.Exports;*/
            var resolver = new ExportResolver("kernel32.dll");
            resolver.CacheAllExports();
            SetHandleInformation = resolver.GetExport<SetHandleInformation>("SetHandleInformation");
            IsDebuggerPresent = resolver.GetExport<IsDebuggerPresent>("IsDebuggerPresent");
            CheckRemoteDebuggerPresent = resolver.GetExport<CheckRemoteDebuggerPresent>("CheckRemoteDebuggerPresent");
            WriteProcessMemory = resolver.GetExport<WriteProcessMemory>("WriteProcessMemory");
            OpenThread = resolver.GetExport<OpenThread>("OpenThread");
            GetTickCount = resolver.GetExport<GetTickCount>("GetTickCount");
            OutputDebugStringA = resolver.GetExport<OutputDebugStringA>("OutputDebugStringA");
            GetCurrentThread = resolver.GetExport<GetCurrentThread>("GetCurrentThread");
            GetThreadContext = resolver.GetExport<GetThreadContext>("GetThreadContext");
            OpenProcess = resolver.GetExport<OpenProcess>("OpenProcess");
            VirtualProtect = resolver.GetExport<VirtualProtect>("VirtualProtect");
            GetProcAddress = resolver.GetExport<GetProcAddress>("GetProcAddress");
        }
    }
}
