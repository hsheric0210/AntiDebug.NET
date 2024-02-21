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
using System.Runtime.InteropServices;

public partial class DLLFromMemory : IDisposable
{

    public bool Disposed { get; private set; }
    public bool IsDll { get; private set; }

    IntPtr pCode = IntPtr.Zero;
    IntPtr pNTHeaders = IntPtr.Zero;
    IntPtr[] ImportModules;
    bool _initialized = false;
    DllEntryDelegate _dllEntry = null;
    ExeEntryDelegate _exeEntry = null;
    bool _isRelocated = false;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate bool DllEntryDelegate(IntPtr hinstDLL, DllReason fdwReason, IntPtr lpReserved);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate int ExeEntryDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    delegate void ImageTlsDelegate(IntPtr dllHandle, DllReason reason, IntPtr reserved);

    /// <summary>
    /// Loads a unmanged (native) DLL in the memory.
    /// </summary>
    /// <param name="data">Dll as a byte array</param>
    public DLLFromMemory(byte[] data)
    {
        NativeCalls.InitNatives();
        Disposed = false;
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        MemoryLoadLibrary(data);
    }

    ~DLLFromMemory()
    {
        Dispose();
    }

    /// <summary>
    /// Returns a delegate for a function inside the DLL.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
    /// <param name="funcName">The name of the function to be searched.</param>
    /// <returns>A delegate instance of type TDelegate</returns>
    public TDelegate GetDelegateFromFuncName<TDelegate>(string funcName) where TDelegate : class
    {
        if (!typeof(Delegate).IsAssignableFrom(typeof(TDelegate)))
            throw new ArgumentException(typeof(TDelegate).Name + " is not a delegate");
        if (!(Marshal.GetDelegateForFunctionPointer(GetPtrFromFuncName(funcName), typeof(TDelegate)) is TDelegate res))
            throw new DllException("Unable to get managed delegate");
        return res;
    }

    /// <summary>
    /// Returns a delegate for a function inside the DLL.
    /// </summary>
    /// <param name="funcName">The Name of the function to be searched.</param>
    /// <param name="delegateType">The type of the delegate to be returned.</param>
    /// <returns>A delegate instance that can be cast to the appropriate delegate type.</returns>
    public Delegate GetDelegateFromFuncName(string funcName, Type delegateType)
    {
        if (delegateType == null)
            throw new ArgumentNullException(nameof(delegateType));
        if (!typeof(Delegate).IsAssignableFrom(delegateType))
            throw new ArgumentException(delegateType.Name + " is not a delegate");
        var res = Marshal.GetDelegateForFunctionPointer(GetPtrFromFuncName(funcName), delegateType);
        if (res == null)
            throw new DllException("Unable to get managed delegate");
        return res;
    }

    IntPtr GetPtrFromFuncName(string funcName)
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(DLLFromMemory));
        if (string.IsNullOrEmpty(funcName))
            throw new ArgumentException(nameof(funcName));
        if (!IsDll)
            throw new InvalidOperationException("Loaded Module is not a DLL");
        if (!_initialized)
            throw new InvalidOperationException("Dll is not initialized");

        var pDirectory = PtrAdd(pNTHeaders, Of.IMAGE_NT_HEADERS_OptionalHeader + (Is64BitProcess ? Of64.IMAGE_OPTIONAL_HEADER_ExportTable : Of32.IMAGE_OPTIONAL_HEADER_ExportTable));
        var Directory = PtrRead<IMAGE_DATA_DIRECTORY>(pDirectory);
        if (Directory.Size == 0)
            throw new DllException("Dll has no export table");

        var pExports = PtrAdd(pCode, Directory.VirtualAddress);
        var Exports = PtrRead<IMAGE_EXPORT_DIRECTORY>(pExports);
        if (Exports.NumberOfFunctions == 0 || Exports.NumberOfNames == 0)
            throw new DllException("Dll exports no functions");

        var pNameRef = PtrAdd(pCode, Exports.AddressOfNames);
        var pOrdinal = PtrAdd(pCode, Exports.AddressOfNameOrdinals);
        for (var i = 0; i < Exports.NumberOfNames; i++, pNameRef = PtrAdd(pNameRef, sizeof(uint)), pOrdinal = PtrAdd(pOrdinal, sizeof(ushort)))
        {
            var NameRef = PtrRead<uint>(pNameRef);
            var Ordinal = PtrRead<ushort>(pOrdinal);
            var curFuncName = Marshal.PtrToStringAnsi(PtrAdd(pCode, NameRef));
            if (curFuncName == funcName)
            {
                if (Ordinal > Exports.NumberOfFunctions)
                    throw new DllException("Invalid function ordinal");
                var pAddressOfFunction = PtrAdd(pCode, Exports.AddressOfFunctions + (uint)(Ordinal * 4));
                return PtrAdd(pCode, PtrRead<uint>(pAddressOfFunction));
            }
        }

        throw new DllException("Dll exports no function named " + funcName);
    }

    /// <summary>
    /// Call entry point of executable.
    /// </summary>
    /// <returns>Exitcode of executable</returns>
    public int MemoryCallEntryPoint()
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(DLLFromMemory));
        if (IsDll || _exeEntry == null || !_isRelocated)
            throw new DllException("Unable to call entry point. Is loaded module a dll?");
        return _exeEntry();
    }

    void MemoryLoadLibrary(byte[] data)
    {
        if (data.Length < Marshal.SizeOf(typeof(IMAGE_DOS_HEADER)))
            throw new DllException("Not a valid executable file");
        var DosHeader = BytesReadStructAt<IMAGE_DOS_HEADER>(data, 0);
        if (DosHeader.e_magic != NativeCalls.IMAGE_DOS_SIGNATURE)
            throw new BadImageFormatException("Not a valid executable file");

        if (data.Length < DosHeader.e_lfanew + Marshal.SizeOf(typeof(IMAGE_NT_HEADERS)))
            throw new DllException("Not a valid executable file");
        var OrgNTHeaders = BytesReadStructAt<IMAGE_NT_HEADERS>(data, DosHeader.e_lfanew);

        if (OrgNTHeaders.Signature != NativeCalls.IMAGE_NT_SIGNATURE)
            throw new BadImageFormatException("Not a valid PE file");
        if (OrgNTHeaders.FileHeader.Machine != GetMachineType())
            throw new BadImageFormatException("Machine type doesn't fit (i386 vs. AMD64)");
        if ((OrgNTHeaders.OptionalHeader.SectionAlignment & 1) > 0)
            throw new BadImageFormatException("Wrong section alignment"); //Only support multiple of 2
        if (OrgNTHeaders.OptionalHeader.AddressOfEntryPoint == 0)
            throw new DllException("Module has no entry point");

        NativeCalls.GetNativeSystemInfo(out var systemInfo);
        uint lastSectionEnd = 0;
        var ofSection = NativeCalls.IMAGE_FIRST_SECTION(DosHeader.e_lfanew, OrgNTHeaders.FileHeader.SizeOfOptionalHeader);
        for (var i = 0; i != OrgNTHeaders.FileHeader.NumberOfSections; i++, ofSection += Sz.IMAGE_SECTION_HEADER)
        {
            var Section = BytesReadStructAt<IMAGE_SECTION_HEADER>(data, ofSection);
            var endOfSection = Section.VirtualAddress + (Section.SizeOfRawData > 0 ? Section.SizeOfRawData : OrgNTHeaders.OptionalHeader.SectionAlignment);
            if (endOfSection > lastSectionEnd)
                lastSectionEnd = endOfSection;
        }

        var alignedImageSize = AlignValueUp(OrgNTHeaders.OptionalHeader.SizeOfImage, systemInfo.dwPageSize);
        var alignedLastSection = AlignValueUp(lastSectionEnd, systemInfo.dwPageSize);
        if (alignedImageSize != alignedLastSection)
            throw new BadImageFormatException("Wrong section alignment");

        IntPtr oldHeader_OptionalHeader_ImageBase;
        if (Is64BitProcess)
            oldHeader_OptionalHeader_ImageBase = (IntPtr)unchecked((long)OrgNTHeaders.OptionalHeader.ImageBaseLong);
        else
            oldHeader_OptionalHeader_ImageBase = (IntPtr)unchecked((int)(OrgNTHeaders.OptionalHeader.ImageBaseLong >> 32));

        // reserve memory for image of library
        pCode = NativeCalls.VirtualAlloc(oldHeader_OptionalHeader_ImageBase, (UIntPtr)OrgNTHeaders.OptionalHeader.SizeOfImage, AllocationType.RESERVE | AllocationType.COMMIT, MemoryProtection.READWRITE);
        //pCode = IntPtr.Zero; //test relocation with this

        // try to allocate memory at arbitrary position
        if (pCode == IntPtr.Zero)
            pCode = NativeCalls.VirtualAlloc(IntPtr.Zero, (UIntPtr)OrgNTHeaders.OptionalHeader.SizeOfImage, AllocationType.RESERVE | AllocationType.COMMIT, MemoryProtection.READWRITE);

        if (pCode == IntPtr.Zero)
            throw new DllException("Out of Memory");

        if (Is64BitProcess && PtrSpanBoundary(pCode, alignedImageSize, 32))
        {
            // Memory block may not span 4 GB (32 bit) boundaries.
            var BlockedMemory = new System.Collections.Generic.List<IntPtr>();
            while (PtrSpanBoundary(pCode, alignedImageSize, 32))
            {
                BlockedMemory.Add(pCode);
                pCode = NativeCalls.VirtualAlloc(IntPtr.Zero, (UIntPtr)alignedImageSize, AllocationType.RESERVE | AllocationType.COMMIT, MemoryProtection.READWRITE);
                if (pCode == IntPtr.Zero)
                    break;
            }
            foreach (var ptr in BlockedMemory)
                NativeCalls.VirtualFree(ptr, IntPtr.Zero, AllocationType.RELEASE);
            if (pCode == IntPtr.Zero)
                throw new DllException("Out of Memory");
        }

        // commit memory for headers
        var headers = NativeCalls.VirtualAlloc(pCode, (UIntPtr)OrgNTHeaders.OptionalHeader.SizeOfHeaders, AllocationType.COMMIT, MemoryProtection.READWRITE);
        if (headers == IntPtr.Zero)
            throw new DllException("Out of Memory");

        // copy PE header to code
        Marshal.Copy(data, 0, headers, (int)OrgNTHeaders.OptionalHeader.SizeOfHeaders);
        pNTHeaders = PtrAdd(headers, DosHeader.e_lfanew);

        var locationDelta = PtrSub(pCode, oldHeader_OptionalHeader_ImageBase);
        if (locationDelta != IntPtr.Zero)
        {
            // update relocated position
            Marshal.OffsetOf(typeof(IMAGE_NT_HEADERS), "OptionalHeader");
            Marshal.OffsetOf(typeof(IMAGE_OPTIONAL_HEADER), "ImageBaseLong");
            var pImageBase = PtrAdd(pNTHeaders, Of.IMAGE_NT_HEADERS_OptionalHeader + (Is64BitProcess ? Of64.IMAGE_OPTIONAL_HEADER_ImageBase : Of32.IMAGE_OPTIONAL_HEADER_ImageBase));
            PtrWrite(pImageBase, pCode);
        }

        // copy sections from DLL file block to new memory location
        CopySections(ref OrgNTHeaders, pCode, pNTHeaders, data);

        // adjust base address of imported data
        _isRelocated = locationDelta != IntPtr.Zero ? PerformBaseRelocation(ref OrgNTHeaders, pCode, locationDelta) : true;

        // load required dlls and adjust function table of imports
        ImportModules = BuildImportTable(ref OrgNTHeaders, pCode);

        // mark memory pages depending on section headers and release
        // sections that are marked as "discardable"
        FinalizeSections(ref OrgNTHeaders, pCode, pNTHeaders, systemInfo.dwPageSize);

        // TLS callbacks are executed BEFORE the main loading
        ExecuteTLS(ref OrgNTHeaders, pCode, pNTHeaders);

        // get entry point of loaded library
        IsDll = (OrgNTHeaders.FileHeader.Characteristics & NativeCalls.IMAGE_FILE_DLL) != 0;
        if (OrgNTHeaders.OptionalHeader.AddressOfEntryPoint != 0)
        {
            if (IsDll)
            {
                // notify library about attaching to process
                var dllEntryPtr = PtrAdd(pCode, OrgNTHeaders.OptionalHeader.AddressOfEntryPoint);
                _dllEntry = (DllEntryDelegate)Marshal.GetDelegateForFunctionPointer(dllEntryPtr, typeof(DllEntryDelegate));

                _initialized = _dllEntry != null && _dllEntry(pCode, DllReason.DLL_PROCESS_ATTACH, IntPtr.Zero);
                if (!_initialized)
                    throw new DllException("Can't attach DLL to process");
            }
            else
            {
                var exeEntryPtr = PtrAdd(pCode, OrgNTHeaders.OptionalHeader.AddressOfEntryPoint);
                _exeEntry = (ExeEntryDelegate)Marshal.GetDelegateForFunctionPointer(exeEntryPtr, typeof(ExeEntryDelegate));
            }
        }
    }

    static void CopySections(ref IMAGE_NT_HEADERS OrgNTHeaders, IntPtr pCode, IntPtr pNTHeaders, byte[] data)
    {
        var pSection = NativeCalls.IMAGE_FIRST_SECTION(pNTHeaders, OrgNTHeaders.FileHeader.SizeOfOptionalHeader);
        for (var i = 0; i < OrgNTHeaders.FileHeader.NumberOfSections; i++, pSection = PtrAdd(pSection, Sz.IMAGE_SECTION_HEADER))
        {
            var Section = PtrRead<IMAGE_SECTION_HEADER>(pSection);
            if (Section.SizeOfRawData == 0)
            {
                // section doesn't contain data in the dll itself, but may define uninitialized data
                var size = OrgNTHeaders.OptionalHeader.SectionAlignment;
                if (size > 0)
                {
                    var dest = NativeCalls.VirtualAlloc(PtrAdd(pCode, Section.VirtualAddress), (UIntPtr)size, AllocationType.COMMIT, MemoryProtection.READWRITE);
                    if (dest == IntPtr.Zero)
                        throw new DllException("Unable to allocate memory");

                    // Always use position from file to support alignments smaller than page size (allocation above will align to page size).
                    dest = PtrAdd(pCode, Section.VirtualAddress);

                    // NOTE: On 64bit systems we truncate to 32bit here but expand again later when "PhysicalAddress" is used.
                    PtrWrite(PtrAdd(pSection, Of.IMAGE_SECTION_HEADER_PhysicalAddress), unchecked((uint)(ulong)(long)dest));

                    NativeCalls.MemSet(dest, 0, (UIntPtr)size);
                }

                // section is empty
                continue;
            }
            else
            {
                // commit memory block and copy data from dll
                var dest = NativeCalls.VirtualAlloc(PtrAdd(pCode, Section.VirtualAddress), (UIntPtr)Section.SizeOfRawData, AllocationType.COMMIT, MemoryProtection.READWRITE);
                if (dest == IntPtr.Zero)
                    throw new DllException("Out of memory");

                // Always use position from file to support alignments smaller than page size (allocation above will align to page size).
                dest = PtrAdd(pCode, Section.VirtualAddress);
                Marshal.Copy(data, checked((int)Section.PointerToRawData), dest, checked((int)Section.SizeOfRawData));

                // NOTE: On 64bit systems we truncate to 32bit here but expand again later when "PhysicalAddress" is used.
                PtrWrite(PtrAdd(pSection, Of.IMAGE_SECTION_HEADER_PhysicalAddress), unchecked((uint)(ulong)(long)dest));
            }
        }
    }

    static bool PerformBaseRelocation(ref IMAGE_NT_HEADERS OrgNTHeaders, IntPtr pCode, IntPtr delta)
    {
        if (OrgNTHeaders.OptionalHeader.BaseRelocationTable.Size == 0)
            return delta == IntPtr.Zero;

        for (var pRelocation = PtrAdd(pCode, OrgNTHeaders.OptionalHeader.BaseRelocationTable.VirtualAddress); ;)
        {
            var Relocation = PtrRead<IMAGE_BASE_RELOCATION>(pRelocation);
            if (Relocation.VirtualAdress == 0)
                break;

            var pDest = PtrAdd(pCode, Relocation.VirtualAdress);
            var pRelInfo = PtrAdd(pRelocation, Sz.IMAGE_BASE_RELOCATION);
            var RelCount = (Relocation.SizeOfBlock - Sz.IMAGE_BASE_RELOCATION) / 2;
            for (uint i = 0; i != RelCount; i++, pRelInfo = PtrAdd(pRelInfo, sizeof(ushort)))
            {
                var relInfo = (ushort)Marshal.PtrToStructure(pRelInfo, typeof(ushort));
                var type = (BasedRelocationType)(relInfo >> 12); // the upper 4 bits define the type of relocation
                var offset = relInfo & 0xfff; // the lower 12 bits define the offset
                var pPatchAddr = PtrAdd(pDest, offset);

                switch (type)
                {
                    case BasedRelocationType.IMAGE_REL_BASED_ABSOLUTE:
                        // skip relocation
                        break;
                    case BasedRelocationType.IMAGE_REL_BASED_HIGHLOW:
                        // change complete 32 bit address
                        var patchAddrHL = (int)Marshal.PtrToStructure(pPatchAddr, typeof(int));
                        patchAddrHL += (int)delta;
                        Marshal.StructureToPtr(patchAddrHL, pPatchAddr, false);
                        break;
                    case BasedRelocationType.IMAGE_REL_BASED_DIR64:
                        var patchAddr64 = (long)Marshal.PtrToStructure(pPatchAddr, typeof(long));
                        patchAddr64 += (long)delta;
                        Marshal.StructureToPtr(patchAddr64, pPatchAddr, false);
                        break;
                }
            }

            // advance to next relocation block
            pRelocation = PtrAdd(pRelocation, Relocation.SizeOfBlock);
        }
        return true;
    }

    static IntPtr[] BuildImportTable(ref IMAGE_NT_HEADERS OrgNTHeaders, IntPtr pCode)
    {
        var ImportModules = new System.Collections.Generic.List<IntPtr>();
        var NumEntries = OrgNTHeaders.OptionalHeader.ImportTable.Size / Sz.IMAGE_IMPORT_DESCRIPTOR;
        var pImportDesc = PtrAdd(pCode, OrgNTHeaders.OptionalHeader.ImportTable.VirtualAddress);
        for (uint i = 0; i != NumEntries; i++, pImportDesc = PtrAdd(pImportDesc, Sz.IMAGE_IMPORT_DESCRIPTOR))
        {
            var ImportDesc = PtrRead<IMAGE_IMPORT_DESCRIPTOR>(pImportDesc);
            if (ImportDesc.Name == 0)
                break;

            var handle = NativeCalls.LoadLibrary(PtrAdd(pCode, ImportDesc.Name));
            if (PtrIsInvalidHandle(handle))
            {
                foreach (var m in ImportModules)
                    NativeCalls.FreeLibrary(m);
                ImportModules.Clear();
                throw new DllException("Can't load libary " + Marshal.PtrToStringAnsi(PtrAdd(pCode, ImportDesc.Name)));
            }
            ImportModules.Add(handle);

            IntPtr pThunkRef, pFuncRef;
            if (ImportDesc.OriginalFirstThunk > 0)
            {
                pThunkRef = PtrAdd(pCode, ImportDesc.OriginalFirstThunk);
                pFuncRef = PtrAdd(pCode, ImportDesc.FirstThunk);
            }
            else
            {
                // no hint table
                pThunkRef = PtrAdd(pCode, ImportDesc.FirstThunk);
                pFuncRef = PtrAdd(pCode, ImportDesc.FirstThunk);
            }
            for (var SzRef = IntPtr.Size; ; pThunkRef = PtrAdd(pThunkRef, SzRef), pFuncRef = PtrAdd(pFuncRef, SzRef))
            {
                IntPtr ReadThunkRef = PtrRead<IntPtr>(pThunkRef), WriteFuncRef;
                if (ReadThunkRef == IntPtr.Zero)
                    break;
                if (NativeCalls.IMAGE_SNAP_BY_ORDINAL(ReadThunkRef))
                {
                    WriteFuncRef = NativeCalls.GetProcAddress(handle, NativeCalls.IMAGE_ORDINAL(ReadThunkRef));
                }
                else
                {
                    WriteFuncRef = NativeCalls.GetProcAddress(handle, PtrAdd(PtrAdd(pCode, ReadThunkRef), Of.IMAGE_IMPORT_BY_NAME_Name));
                }
                if (WriteFuncRef == IntPtr.Zero)
                    throw new DllException("Can't get adress for imported function");
                PtrWrite(pFuncRef, WriteFuncRef);
            }
        }
        return ImportModules.Count > 0 ? ImportModules.ToArray() : null;
    }

    static void FinalizeSections(ref IMAGE_NT_HEADERS OrgNTHeaders, IntPtr pCode, IntPtr pNTHeaders, uint PageSize)
    {
        var imageOffset = Is64BitProcess ? (UIntPtr)(unchecked((ulong)pCode.ToInt64()) & 0xffffffff00000000) : UIntPtr.Zero;
        var pSection = NativeCalls.IMAGE_FIRST_SECTION(pNTHeaders, OrgNTHeaders.FileHeader.SizeOfOptionalHeader);
        var Section = PtrRead<IMAGE_SECTION_HEADER>(pSection);
        var sectionData = new SectionFinalizeData();
        sectionData.Address = PtrBitOr(PtrAdd((IntPtr)0, Section.PhysicalAddress), imageOffset);
        sectionData.AlignedAddress = PtrAlignDown(sectionData.Address, (UIntPtr)PageSize);
        sectionData.Size = GetRealSectionSize(ref Section, ref OrgNTHeaders);
        sectionData.Characteristics = Section.Characteristics;
        sectionData.Last = false;
        pSection = PtrAdd(pSection, Sz.IMAGE_SECTION_HEADER);

        // loop through all sections and change access flags
        for (var i = 1; i < OrgNTHeaders.FileHeader.NumberOfSections; i++, pSection = PtrAdd(pSection, Sz.IMAGE_SECTION_HEADER))
        {
            Section = PtrRead<IMAGE_SECTION_HEADER>(pSection);
            var sectionAddress = PtrBitOr(PtrAdd((IntPtr)0, Section.PhysicalAddress), imageOffset);
            var alignedAddress = PtrAlignDown(sectionAddress, (UIntPtr)PageSize);
            var sectionSize = GetRealSectionSize(ref Section, ref OrgNTHeaders);

            // Combine access flags of all sections that share a page
            // TODO(fancycode): We currently share flags of a trailing large section with the page of a first small section. This should be optimized.
            var a = PtrAdd(sectionData.Address, sectionData.Size);
            ulong b = unchecked((ulong)a.ToInt64()), c = unchecked((ulong)alignedAddress);

            if (sectionData.AlignedAddress == alignedAddress || unchecked((ulong)PtrAdd(sectionData.Address, sectionData.Size).ToInt64()) > unchecked((ulong)alignedAddress))
            {
                // Section shares page with previous
                if ((Section.Characteristics & NativeCalls.IMAGE_SCN_MEM_DISCARDABLE) == 0 || (sectionData.Characteristics & NativeCalls.IMAGE_SCN_MEM_DISCARDABLE) == 0)
                {
                    sectionData.Characteristics = (sectionData.Characteristics | Section.Characteristics) & ~NativeCalls.IMAGE_SCN_MEM_DISCARDABLE;
                }
                else
                {
                    sectionData.Characteristics |= Section.Characteristics;
                }
                sectionData.Size = PtrSub(PtrAdd(sectionAddress, sectionSize), sectionData.Address);
                continue;
            }

            FinalizeSection(sectionData, PageSize, OrgNTHeaders.OptionalHeader.SectionAlignment);

            sectionData.Address = sectionAddress;
            sectionData.AlignedAddress = alignedAddress;
            sectionData.Size = sectionSize;
            sectionData.Characteristics = Section.Characteristics;
        }
        sectionData.Last = true;
        FinalizeSection(sectionData, PageSize, OrgNTHeaders.OptionalHeader.SectionAlignment);
    }

    static void FinalizeSection(SectionFinalizeData SectionData, uint PageSize, uint SectionAlignment)
    {
        if (SectionData.Size == IntPtr.Zero)
            return;

        if ((SectionData.Characteristics & NativeCalls.IMAGE_SCN_MEM_DISCARDABLE) > 0)
        {
            // section is not needed any more and can safely be freed
            if (SectionData.Address == SectionData.AlignedAddress &&
                (SectionData.Last ||
                    SectionAlignment == PageSize ||
                    unchecked((ulong)SectionData.Size.ToInt64()) % PageSize == 0)
                )
            {
                // Only allowed to decommit whole pages
                NativeCalls.VirtualFree(SectionData.Address, SectionData.Size, AllocationType.DECOMMIT);
            }
            return;
        }

        // determine protection flags based on characteristics
        var readable = (SectionData.Characteristics & (uint)ImageSectionFlags.IMAGE_SCN_MEM_READ) != 0 ? 1 : 0;
        var writeable = (SectionData.Characteristics & (uint)ImageSectionFlags.IMAGE_SCN_MEM_WRITE) != 0 ? 1 : 0;
        var executable = (SectionData.Characteristics & (uint)ImageSectionFlags.IMAGE_SCN_MEM_EXECUTE) != 0 ? 1 : 0;
        var protect = (uint)ProtectionFlags[executable, readable, writeable];
        if ((SectionData.Characteristics & NativeCalls.IMAGE_SCN_MEM_NOT_CACHED) > 0)
            protect |= NativeCalls.PAGE_NOCACHE;

        // change memory access flags
        if (!NativeCalls.VirtualProtect(SectionData.Address, SectionData.Size, protect, out var oldProtect))
            throw new DllException("Error protecting memory page");
    }

    static void ExecuteTLS(ref IMAGE_NT_HEADERS OrgNTHeaders, IntPtr pCode, IntPtr pNTHeaders)
    {
        if (OrgNTHeaders.OptionalHeader.TLSTable.VirtualAddress == 0)
            return;
        var tlsDir = PtrRead<IMAGE_TLS_DIRECTORY>(PtrAdd(pCode, OrgNTHeaders.OptionalHeader.TLSTable.VirtualAddress));
        var pCallBack = tlsDir.AddressOfCallBacks;
        if (pCallBack != IntPtr.Zero)
        {
            for (IntPtr Callback; (Callback = PtrRead<IntPtr>(pCallBack)) != IntPtr.Zero; pCallBack = PtrAdd(pCallBack, IntPtr.Size))
            {
                var tls = (ImageTlsDelegate)Marshal.GetDelegateForFunctionPointer(Callback, typeof(ImageTlsDelegate));
                tls(pCode, DllReason.DLL_PROCESS_ATTACH, IntPtr.Zero);
            }
        }
    }

    /// <summary>
    /// Check if the process runs in 64bit mode or in 32bit mode
    /// </summary>
    /// <returns>True if process is 64bit, false if it is 32bit</returns>
    public static bool Is64BitProcess => IntPtr.Size == 8;

    static uint GetMachineType() => IntPtr.Size == 8 ? NativeCalls.IMAGE_FILE_MACHINE_AMD64 : NativeCalls.IMAGE_FILE_MACHINE_I386;

    static uint AlignValueUp(uint value, uint alignment) => (value + alignment - 1) & ~(alignment - 1);

    static IntPtr GetRealSectionSize(ref IMAGE_SECTION_HEADER Section, ref IMAGE_NT_HEADERS NTHeaders)
    {
        var size = Section.SizeOfRawData;
        if (size == 0)
        {
            if ((Section.Characteristics & NativeCalls.IMAGE_SCN_CNT_INITIALIZED_DATA) > 0)
            {
                size = NTHeaders.OptionalHeader.SizeOfInitializedData;
            }
            else if ((Section.Characteristics & NativeCalls.IMAGE_SCN_CNT_UNINITIALIZED_DATA) > 0)
            {
                size = NTHeaders.OptionalHeader.SizeOfUninitializedData;
            }
        }
        return IntPtr.Size == 8 ? (IntPtr)unchecked((long)size) : (IntPtr)unchecked((int)size);
    }

    public void Close() => ((IDisposable)this).Dispose();

    void IDisposable.Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (_initialized)
        {
            if (_dllEntry != null)
                _dllEntry.Invoke(pCode, DllReason.DLL_PROCESS_DETACH, IntPtr.Zero);
            _initialized = false;
        }

        if (ImportModules != null)
        {
            foreach (var m in ImportModules)
                if (!PtrIsInvalidHandle(m))
                    NativeCalls.FreeLibrary(m);
            ImportModules = null;
        }

        if (pCode != IntPtr.Zero)
        {
            NativeCalls.VirtualFree(pCode, IntPtr.Zero, AllocationType.RELEASE);
            pCode = IntPtr.Zero;
            pNTHeaders = IntPtr.Zero;
        }

        Disposed = true;
    }

    // Protection flags for memory pages (Executable, Readable, Writeable)
    static readonly PageProtection[,,] ProtectionFlags = new PageProtection[2, 2, 2]
    {
        {
            // not executable
            { PageProtection.NOACCESS, PageProtection.WRITECOPY },
            { PageProtection.READONLY, PageProtection.READWRITE }
        },
        {
            // executable
            { PageProtection.EXECUTE, PageProtection.EXECUTE_WRITECOPY },
            { PageProtection.EXECUTE_READ, PageProtection.EXECUTE_READWRITE }
        }
    };

    class Of
    {
        internal const int IMAGE_NT_HEADERS_OptionalHeader = 24;
        internal const int IMAGE_SECTION_HEADER_PhysicalAddress = 8;
        internal const int IMAGE_IMPORT_BY_NAME_Name = 2;
    }

    class Of32
    {
        internal const int IMAGE_OPTIONAL_HEADER_ImageBase = 28;
        internal const int IMAGE_OPTIONAL_HEADER_ExportTable = 96;
    }

    class Of64
    {
        internal const int IMAGE_OPTIONAL_HEADER_ImageBase = 24;
        internal const int IMAGE_OPTIONAL_HEADER_ExportTable = 112;
    }

    class Sz
    {
        internal const int IMAGE_SECTION_HEADER = 40;
        internal const int IMAGE_BASE_RELOCATION = 8;
        internal const int IMAGE_IMPORT_DESCRIPTOR = 20;
    }

    static T BytesReadStructAt<T>(byte[] buf, int offset)
    {
        var size = Marshal.SizeOf(typeof(T));
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buf, offset, ptr, size);
        var res = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return res;
    }
}