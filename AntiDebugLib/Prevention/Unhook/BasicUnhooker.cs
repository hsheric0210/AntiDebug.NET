using AntiDebugLib.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Prevention.Unhook
{
    /// <summary>
    /// Basic unhooker.
    /// Removes IAT hooks by overwriting the Import Access Table with my own exe's or dll's one.
    /// Removes EAT hooks by overwriting the Export Address Table with the dll's original one.
    /// Removes inline hooks(trampolines) by overwriting '.text' section of dll with the dll's original one.
    /// I don't know much about pointer hooking, but I think it is likely to be removed by just overwriting '.text' section too.
    /// But instead of using Win32 functions such as CreateFileMapping, MapViewOfFile,
    /// manually finds the '.text' section and overwrites byte-by-byte to prevent detection.
    /// </summary>
    internal abstract class BasicUnhooker : CheckBase
    {
        /// <summary>
        /// The PE file name.
        /// For example: kernel32.dll
        /// </summary>
        public abstract string PeName { get; }

        /// <summary>
        /// The original path of PE file.
        /// For example: C:\Windows\System32\kernel32.dll
        /// </summary>
        public abstract string OriginalFilePath { get; }

        protected BasicUnhooker()
        {
        }

        private bool ParsePE(IntPtr moduleBase, out _IMAGE_DOS_HEADER dosHeader, out _IMAGE_NT_HEADERS ntHeader)
        {
            dosHeader = Marshal.PtrToStructure<_IMAGE_DOS_HEADER>(moduleBase);
            ntHeader = default;

            if (dosHeader.e_magic != IMAGE_DOS_SIGNATURE)
            {
                Logger.Error("Not a valid PE file. (Invalid DOS header)");
                return false;
            }

            var ntHeaderAddress = moduleBase.Add(dosHeader.e_lfanew);
            ntHeader = Marshal.PtrToStructure<_IMAGE_NT_HEADERS>(ntHeaderAddress);
            if (ntHeader.Signature != IMAGE_NT_SIGNATURE)
            {
                Logger.Error("Not a valid PE file. (Invalid NT signature)");
                return false;
            }

            return true;
        }

        private IntPtr GetSectionStartAddress(IntPtr moduleBase, _IMAGE_DOS_HEADER dosHeader, _IMAGE_NT_HEADERS ntHeader)
        {
            var ntHeaderAddress = moduleBase.Add(dosHeader.e_lfanew);
            var optionalHeaderStartAddress = ntHeaderAddress + 4 + Marshal.SizeOf<_IMAGE_FILE_HEADER>();
            var optHeaderSize = ntHeader.FileHeader.SizeOfOptionalHeader;
            return optionalHeaderStartAddress + optHeaderSize;
        }

        private _IMAGE_SECTION_HEADER[] GetIAT(IntPtr moduleBase, _IMAGE_DOS_HEADER dosHeader, _IMAGE_NT_HEADERS ntHeader)
        {
            var address = GetSectionStartAddress(moduleBase, dosHeader, ntHeader);
            var count = ntHeader.OptionalHeader.NumberOfSections;

            var headers = new _IMAGE_SECTION_HEADER[count];
            for (var i = 0; i < count; i++)
            {
                headers[i] = Marshal.PtrToStructure<_IMAGE_SECTION_HEADER>(address);
                address += Marshal.SizeOf<_IMAGE_SECTION_HEADER>();
            }

            return headers;
        }

        private _IMAGE_SECTION_HEADER[] GetSectionHeaders(IntPtr moduleBase, _IMAGE_DOS_HEADER dosHeader, _IMAGE_NT_HEADERS ntHeader)
        {
            var address = GetSectionStartAddress(moduleBase, dosHeader, ntHeader);
            var count = ntHeader.FileHeader.NumberOfSections;

            var headers = new _IMAGE_SECTION_HEADER[count];
            for (var i = 0; i < count; i++)
            {
                headers[i] = Marshal.PtrToStructure<_IMAGE_SECTION_HEADER>(address);
                address += Marshal.SizeOf<_IMAGE_SECTION_HEADER>();
            }

            return headers;
        }

        private bool Unhook(IntPtr myBase, IntPtr originalBase)
        {
            var sectionCount = ntHeader.FileHeader.NumberOfSections;
            var sections = new PeSection[sectionCount];
            for (var i = 0; i < sectionCount; i++)
            {
                var sectionHeader = Marshal.PtrToStructure<_IMAGE_SECTION_HEADER>(sectionHeaderStartAddress);
                var sectionDataAddress = originalBase.Add(sectionHeader.PointerToRawData); // https://777bareman777.github.io/2019/09/20/UnderstandPE6/
                var sectionData = new byte[sectionHeader.SizeOfRawData];

                Marshal.Copy(sectionDataAddress, sectionData, 0, sectionData.Length);
                sections[i] = new PeSection
                {
                    Header = sectionHeader,
                    Data = sectionData
                };

                sectionHeaderStartAddress += Marshal.SizeOf<_IMAGE_SECTION_HEADER>();
            }
        }

        public override bool PreventPassive()
        {
            if (!File.Exists(OriginalFilePath))
            {
                Logger.Error("The dll {dll} doesn't exists on {path}.", PeName, OriginalFilePath);
                return false;
            }

            var moduleHandle = GetModuleHandleA(PeName);

            var data = File.ReadAllBytes(OriginalFilePath);
            var peBase = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, peBase, data.Length);

            var sections = Unhook(moduleHandle, peBase);

        }

        private struct PeSection
        {
            public _IMAGE_SECTION_HEADER Header;
            public byte[] Data;
        }

        private struct IatTable
        {

        }

        private struct EatTable
        {

        }
    }
}
