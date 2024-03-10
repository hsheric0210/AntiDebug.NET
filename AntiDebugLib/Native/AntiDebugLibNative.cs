using AntiDebugLib.Properties;
using StealthModule;
using System;
using System.Runtime.InteropServices;
using static AntiDebugLib.Native.NativeDefs;

namespace AntiDebugLib.Native
{
    internal static class AntiDebugLibNative
    {
        private const string EncryptionMagic = /*<dll_crypt_magic>*/"/sbRgX4CpG1S[9j(3."/*</dll_crypt_magic>*/; // only use ascii chars

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ulong DMyEntryPoint(uint checkType);

        private static DMyEntryPoint pfnMyEntryPoint;

        internal static ulong PerformNativeCheck(NativeCheckType checkType) => pfnMyEntryPoint((uint)checkType);

        // todo: move these two functions to other place
        internal static IntPtr PebAddressCache = IntPtr.Zero;
        internal static IntPtr GetPeb()
        {
            if (PebAddressCache == IntPtr.Zero)
            {
                var processBasicInformation = new PROCESS_BASIC_INFORMATION();
                var status = NtDll.NtQueryInformationProcess_ProcessBasicInfo((IntPtr)(-1), 0x0, ref processBasicInformation, (uint)Marshal.SizeOf(processBasicInformation), out _);
                if (!NT_SUCCESS(status))
                    throw new Exception($"NtQueryInformationProcess returned NTSTATUS {status}");

                PebAddressCache = processBasicInformation.PebBaseAddress;
            }

            return PebAddressCache;
        }

        private static MemoryModule nativeModule; // Prevent DLL from get garbage collected

        private static byte[] Decrypt(byte[] encrypted)
        {
            var decrypted = new byte[encrypted.Length];
            for (int i = 0, j = 0; i < encrypted.Length; i++)
            {
                decrypted[i] = (byte)(encrypted[i] ^ EncryptionMagic[j++]);
                if (j >= EncryptionMagic.Length)
                    j = 0;
            }
            return decrypted;
        }

        internal static void Init()
        {
            AntiDebug.Logger.Information("Will use {bit}-bit native library.", Environment.Is64BitProcess ? 64 : 32);
            var dll = Decrypt(Environment.Is64BitProcess ? Resources.AntiDebugLibNative_x64 : Resources.AntiDebugLibNative_Win32);
            nativeModule = new MemoryModule(dll);
            pfnMyEntryPoint = nativeModule.Exports.GetExport<DMyEntryPoint>(/*<cs_entrypoint>*/"hNktTZ8dCX7zhp9GRjIc0Ev6fQm25n_w"/*</cs_entrypoint>*/);

            // initialize indirect calls
            Kernel32.InitNatives();
            Kernel32.InitNativesUnhooked();
            NtDll.InitNatives();
            NtDll.InitNativesUnhooked();
            User32.InitNatives();
        }
    }
}
