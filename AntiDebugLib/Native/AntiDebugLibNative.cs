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
        internal delegate ulong DMyEntryPoint();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DMyGetPeb();

        private static DMyEntryPoint pfnMyEntryPoint;
        private static DMyGetPeb pfnMyGetPeb;

        internal static ulong DoNativeChecks() => pfnMyEntryPoint();

        internal static IntPtr GetPeb() => pfnMyGetPeb();

        private static MemoryModule nativeModule; // Prevent DLL from get garbage collected

        private static string DecorateFunctionName(string name, int paramSize)
        {
            if (Environment.Is64BitProcess)
                return name;

            return '_' + name + '@' + paramSize; // Example: _MyFunctionName@0
        }

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
            pfnMyEntryPoint = nativeModule.Exports.GetExport<DMyEntryPoint>(DecorateFunctionName(/*<cs_entrypoint>*/"AD43568293496"/*</cs_entrypoint>*/, 0));
            pfnMyGetPeb = nativeModule.Exports.GetExport<DMyGetPeb>(DecorateFunctionName(/*<cs_getpeb>*/"AD4567348905025"/*</cs_getpeb>*/, 0));

            // initialize indirect calls
            Kernel32.InitNatives();
            Kernel32.InitNativesUnhooked();
            NtDll.InitNatives();
            NtDll.InitNativesUnhooked();
            User32.InitNatives();
        }
    }
}
