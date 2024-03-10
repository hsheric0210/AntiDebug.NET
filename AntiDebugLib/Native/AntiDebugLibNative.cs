using AntiDebugLib.Properties;
using StealthModule;
using System;
using System.Runtime.InteropServices;

namespace AntiDebugLib.Native
{
    internal static class AntiDebugLibNative
    {
        private const string EncryptionMagic = /*<dll_crypt_magic>*/"AntiDebug.NET"/*</dll_crypt_magic>*/; // only use ascii chars

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate ulong DMyEntryPoint(uint checkType);

        private static DMyEntryPoint pfnMyEntryPoint;

        internal static ulong PerformNativeCheck(NativeCheckType checkType) => pfnMyEntryPoint((uint)checkType);

        internal static IntPtr GetPeb()
        {
            return IntPtr.Zero; // TODo
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
            pfnMyEntryPoint = nativeModule.Exports.GetExport<DMyEntryPoint>(/*<cs_entrypoint>*/"AD43568293496"/*</cs_entrypoint>*/);

            // initialize indirect calls
            Kernel32.InitNatives();
            Kernel32.InitNativesUnhooked();
            NtDll.InitNatives();
            NtDll.InitNativesUnhooked();
            User32.InitNatives();
        }
    }
}
