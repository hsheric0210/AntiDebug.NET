/*
 * DLLFromMemory.Net
 *
 * Load a native DLL from memory without the need to allow unsafe code
 *
 * Copyright (C) 2018 - 2019 by Bernhard Schelling
 *
 * Based on Memory Module.net 0.2
 * Copyright (C) 2012 - 2018 by Andreas Kanzler (andi_kanzler(at)gmx.de)
 * https://github.com/Scavanger/MemoryModule.net
 *
 * Based on Memory DLL loading code Version 0.0.4
 * Copyright (C) 2004 - 2015 by Joachim Bauch (mail(at)joachim-bauch.de)
 * https://github.com/fancycode/MemoryModule
 *
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 2.0 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is MemoryModule.c
 *
 * The Initial Developer of the Original Code is Joachim Bauch.
 *
 * Portions created by Joachim Bauch are Copyright (C) 2004 - 2015
 * Joachim Bauch. All Rights Reserved.
 *
 * Portions created by Andreas Kanzler are Copyright (C) 2012 - 2018
 * Andreas Kanzler. All Rights Reserved.
 *
 * Portions created by Bernhard Schelling are Copyright (C) 2018 - 2019
 * Bernhard Schelling. All Rights Reserved.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public partial class DLLFromMemory
{
    class NativeCalls
    {
        internal const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;
        internal const uint IMAGE_NT_SIGNATURE = 0x00004550;
        internal const uint IMAGE_FILE_MACHINE_I386 = 0x014c;
        internal const uint IMAGE_FILE_MACHINE_AMD64 = 0x8664;
        internal const uint PAGE_NOCACHE = 0x200;
        internal const uint IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040;
        internal const uint IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080;
        internal const uint IMAGE_SCN_MEM_DISCARDABLE = 0x02000000;
        internal const uint IMAGE_SCN_MEM_NOT_CACHED = 0x04000000;
        internal const uint IMAGE_FILE_DLL = 0x2000;

        internal delegate IntPtr DVirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        internal delegate IntPtr DLoadLibrary(IntPtr lpFileName);
        internal delegate bool DVirtualFree(IntPtr lpAddress, IntPtr dwSize, AllocationType dwFreeType);
        internal delegate bool DVirtualProtect(IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        internal delegate bool DFreeLibrary(IntPtr hModule);
        internal delegate void DGetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);
        internal delegate IntPtr DGetProcAddress(IntPtr hModule, IntPtr procName);

        static bool nativeInitialized;
        internal static DLoadLibrary LoadLibrary;
        internal static DFreeLibrary FreeLibrary;
        internal static DVirtualAlloc VirtualAlloc;
        internal static DVirtualFree VirtualFree;
        internal static DVirtualProtect VirtualProtect;
        internal static DGetNativeSystemInfo GetNativeSystemInfo;
        internal static DGetProcAddress GetProcAddress;

        // Equivalent to the IMAGE_FIRST_SECTION macro
        internal static IntPtr IMAGE_FIRST_SECTION(IntPtr pNTHeader, ushort ntheader_FileHeader_SizeOfOptionalHeader) => PtrAdd(pNTHeader, Of.IMAGE_NT_HEADERS_OptionalHeader + ntheader_FileHeader_SizeOfOptionalHeader);

        // Equivalent to the IMAGE_FIRST_SECTION macro
        internal static int IMAGE_FIRST_SECTION(int lfanew, ushort ntheader_FileHeader_SizeOfOptionalHeader) => lfanew + Of.IMAGE_NT_HEADERS_OptionalHeader + ntheader_FileHeader_SizeOfOptionalHeader;

        // Equivalent to the IMAGE_ORDINAL32/64 macros
        internal static IntPtr IMAGE_ORDINAL(IntPtr ordinal) => (IntPtr)(int)(unchecked((ulong)ordinal.ToInt64()) & 0xffff);

        // Equivalent to the IMAGE_SNAP_BY_ORDINAL32/64 macro
        internal static bool IMAGE_SNAP_BY_ORDINAL(IntPtr ordinal) => IntPtr.Size == 8 ? (ordinal.ToInt64() < 0) : (ordinal.ToInt32() < 0);

        internal static void InitNatives()
        {
            if (nativeInitialized)
                return;

            var kernel32 = DInvoke.GetModuleHandle("kernel32.dll");
            var exports = new string[] {
                 "LoadLibraryA",
                 "FreeLibrary",
                 "VirtualAlloc",
                 "VirtualFree",
                 "VirtualProtect",
                 "GetNativeSystemInfo",
                 "GetProcAddress",
            };

            var addresses = DInvoke.GetProcAddressBatch(kernel32, exports, true);
            LoadLibrary = Marshal.GetDelegateForFunctionPointer<DLoadLibrary>(addresses[0]);
            FreeLibrary = Marshal.GetDelegateForFunctionPointer<DFreeLibrary>(addresses[1]);
            VirtualAlloc = Marshal.GetDelegateForFunctionPointer<DVirtualAlloc>(addresses[2]);
            VirtualFree = Marshal.GetDelegateForFunctionPointer<DVirtualFree>(addresses[3]);
            VirtualProtect = Marshal.GetDelegateForFunctionPointer<DVirtualProtect>(addresses[4]);
            GetNativeSystemInfo = Marshal.GetDelegateForFunctionPointer<DGetNativeSystemInfo>(addresses[5]);
            GetProcAddress = Marshal.GetDelegateForFunctionPointer<DGetProcAddress>(addresses[6]);
            nativeInitialized = true;
        }
    }
}