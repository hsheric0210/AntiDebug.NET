﻿/*
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
using System.Runtime.InteropServices;

public partial class DLLFromMemory
{
    class NativeCalls
    {
        public const ushort IMAGE_DOS_SIGNATURE = 0x5A4D;
        public const uint IMAGE_NT_SIGNATURE = 0x00004550;
        public const uint IMAGE_FILE_MACHINE_I386 = 0x014c;
        public const uint IMAGE_FILE_MACHINE_AMD64 = 0x8664;
        public const uint PAGE_NOCACHE = 0x200;
        public const uint IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040;
        public const uint IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080;
        public const uint IMAGE_SCN_MEM_DISCARDABLE = 0x02000000;
        public const uint IMAGE_SCN_MEM_NOT_CACHED = 0x04000000;
        public const uint IMAGE_FILE_DLL = 0x2000;

        public delegate IntPtr DVirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        public delegate IntPtr DLoadLibrary(IntPtr lpFileName);
        public delegate bool DVirtualFree(IntPtr lpAddress, IntPtr dwSize, AllocationType dwFreeType);
        public delegate bool DVirtualProtect(IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        public delegate bool DFreeLibrary(IntPtr hModule);
        public delegate void DGetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

        static bool nativeInitialized;
        public static DLoadLibrary LoadLibrary;
        public static DFreeLibrary FreeLibrary;
        public static DVirtualAlloc VirtualAlloc;
        public static DVirtualFree VirtualFree;
        public static DVirtualProtect VirtualProtect;
        public static DGetNativeSystemInfo GetNativeSystemInfo;

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        internal static extern IntPtr MemSet(IntPtr dest, int c, UIntPtr count);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, IntPtr procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        // Equivalent to the IMAGE_FIRST_SECTION macro
        public static IntPtr IMAGE_FIRST_SECTION(IntPtr pNTHeader, ushort ntheader_FileHeader_SizeOfOptionalHeader) => PtrAdd(pNTHeader, Of.IMAGE_NT_HEADERS_OptionalHeader + ntheader_FileHeader_SizeOfOptionalHeader);

        // Equivalent to the IMAGE_FIRST_SECTION macro
        public static int IMAGE_FIRST_SECTION(int lfanew, ushort ntheader_FileHeader_SizeOfOptionalHeader) => lfanew + Of.IMAGE_NT_HEADERS_OptionalHeader + ntheader_FileHeader_SizeOfOptionalHeader;

        // Equivalent to the IMAGE_ORDINAL32/64 macros
        public static IntPtr IMAGE_ORDINAL(IntPtr ordinal) => (IntPtr)(int)(unchecked((ulong)ordinal.ToInt64()) & 0xffff);

        // Equivalent to the IMAGE_SNAP_BY_ORDINAL32/64 macro
        public static bool IMAGE_SNAP_BY_ORDINAL(IntPtr ordinal) => IntPtr.Size == 8 ? (ordinal.ToInt64() < 0) : (ordinal.ToInt32() < 0);

        internal static void InitNatives()
        {
            if (nativeInitialized)
                return;

            var kernel32 = GetModuleHandle("kernel32.dll");
            var llaAddr = GetProcAddress(kernel32, "LoadLibraryA");
            Console.WriteLine("LoadLibraryA available on " + llaAddr.ToInt64().ToString("X16"));
            LoadLibrary = Marshal.GetDelegateForFunctionPointer<DLoadLibrary>(llaAddr);
            FreeLibrary = Marshal.GetDelegateForFunctionPointer<DFreeLibrary>(GetProcAddress(kernel32, "FreeLibrary"));
            VirtualAlloc = Marshal.GetDelegateForFunctionPointer<DVirtualAlloc>(GetProcAddress(kernel32, "VirtualAlloc"));
            VirtualFree = Marshal.GetDelegateForFunctionPointer<DVirtualFree>(GetProcAddress(kernel32, "VirtualFree"));
            VirtualProtect = Marshal.GetDelegateForFunctionPointer<DVirtualProtect>(GetProcAddress(kernel32, "VirtualProtect"));
            GetNativeSystemInfo = Marshal.GetDelegateForFunctionPointer<DGetNativeSystemInfo>(GetProcAddress(kernel32, "GetNativeSystemInfo"));
            nativeInitialized = true;
        }
    }
}