using AntiDebugLib.Properties;
using StealthModule;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static AntiDebugLib.Native.NativeDefs;

namespace AntiDebugLib.Native
{
    internal static class AntiDebugLibNative
    {
        private const string EncryptionMagic = /*<dll_crypt_magic>*/"Zt/XfLsNvmFqV%R_k!C*ox~I"/*</dll_crypt_magic>*/; // only use ascii chars

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ulong DMyEntryPoint(uint checkType);

        private static DMyEntryPoint pfnMyEntryPoint;

        internal static ulong PerformNativeCheck(NativeCheckType checkType)
        {
            return 0;
            return pfnMyEntryPoint((uint)checkType);
        }

        // todo: move these two functions to other place
        internal static IntPtr PebAddressCache = IntPtr.Zero;
        internal static IntPtr GetPeb()
        {
            if (PebAddressCache == IntPtr.Zero)
            {
                var processBasicInformation = new PROCESS_BASIC_INFORMATION();
                var status = NtDll.NtQueryInformationProcess_ProcessBasicInfo((IntPtr)(-1), 0x0, ref processBasicInformation, (uint)Marshal.SizeOf(processBasicInformation), out _);
                if (!NT_SUCCESS(status))
                    throw new Exception($"NtQueryInformationProcess returned NTSTATUS '{status}'");

                PebAddressCache = processBasicInformation.PebBaseAddress;
            }

            return PebAddressCache;
        }

        private static LocalMemoryModule nativeModule; // Prevent DLL from get garbage collected

        private static byte[] Decrypt(byte[] encrypted)
        {
            using (var sha = SHA256.Create())
            {
                using (var aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Padding = PaddingMode.ISO10126;

                    aes.Key = sha.ComputeHash(Encoding.UTF8.GetBytes(EncryptionMagic));

                    var ivBuffer = new byte[16];
                    Buffer.BlockCopy(encrypted, 0, ivBuffer, 0, 16);
                    aes.IV = ivBuffer;

                    return aes.CreateDecryptor().TransformFinalBlock(encrypted, 16, encrypted.Length - 16);
                }
            }
        }

        internal static void Init()
        {
            AntiDebug.Logger.Information("Will use {bit}-bit native library.", Environment.Is64BitProcess ? 64 : 32);
            var dll = Decrypt(Environment.Is64BitProcess ? Resources.AntiDebugLibNative_x64 : Resources.AntiDebugLibNative_Win32);
            //nativeModule = new MemoryModule(dll);
            //pfnMyEntryPoint = nativeModule.Exports.GetExport<DMyEntryPoint>(/*<cs_entrypoint>*/"bn7i9LXFVreWz_M3hyqvC2gdU8kJQYO"/*</cs_entrypoint>*/);

            // initialize indirect calls
            Kernel32.InitNatives();
            Kernel32.InitNativesUnhooked();
            NtDll.InitNatives();
            NtDll.InitNativesUnhooked();
            User32.InitNatives();
        }
    }
}
