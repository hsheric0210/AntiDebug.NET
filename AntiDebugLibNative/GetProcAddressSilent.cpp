#include "pch.h"
#include "GetProcAddressSilent.h"

// https://gist.github.com/Wack0/849348f9d4f3a73dac864a556e9372a5
ULONG_PTR GetPEB()
{
#ifdef _M_X64
    return __readgsqword(0x60);
#elif _M_IX86
    return __readfsdword(0x30);
#elif _M_ARM
    return *(ULONG_PTR *)(_MoveFromCoprocessor(15, 0, 13, 0, 2) + 0x30);
#elif _M_ARM64
    return *(ULONG_PTR *)(__getReg(18) + 0x60); // TEB in x18
#elif _M_IA64
    return *(ULONG_PTR *)((size_t)_rdteb() + 0x60); // TEB in r13
#elif _M_ALPHA
    return *(ULONG_PTR *)((size_t)_rdteb() + 0x30); // TEB pointer returned from callpal 0xAB
#elif _M_MIPS
    return *(ULONG_PTR *)((*(size_t *)(0x7ffff030)) + 0x30); // TEB pointer located at 0x7ffff000 (PCR in user-mode) + 0x30
#elif _M_PPC
    // winnt.h of the period uses __builtin_get_gpr13() or __gregister_get(13) depending on _MSC_VER
    return *(ULONG_PTR *)(__gregister_get(13) + 0x30); // TEB in r13
#else
#error "This architecture is currently unsupported"
#endif
}

// Code stolen from https://github.com/hsheric0210/SimpleSyringe/blob/main/SimpleSyringe.Syringe/GetProcAddressSilent.cpp

ULONG_PTR GetModu1eH4ndle(LPCWSTR libraryName)
{
    using namespace std;

    size_t libraryNameLen = wcslen(libraryName);
    wstring libraryNameStr(libraryName);
    transform(libraryNameStr.begin(), libraryNameStr.end(), libraryNameStr.begin(), towupper);

    ULONG_PTR peb = GetPEB();

    auto ldrData = (ULONG_PTR)((_PPEB)peb)->pLdr;
    auto beginEntry = (ULONG_PTR)((PPEB_LDR_DATA)ldrData)->InMemoryOrderModuleList.Flink;
    ULONG_PTR entry = beginEntry;
    while (entry)
    {
        auto dataEntry = (PLDR_DATA_TABLE_ENTRY)entry;
        auto moduleNameBuffer = dataEntry->BaseDllName.pBuffer;
        DWORD moduleNameLength1 = min(dataEntry->BaseDllName.Length, libraryNameLen);
        wstring currentLibraryName(moduleNameBuffer, moduleNameBuffer + moduleNameLength1);
        transform(currentLibraryName.begin(), currentLibraryName.end(), currentLibraryName.begin(), towupper);

        if (libraryNameStr == currentLibraryName) // equalsIgnoreCase
            return (ULONG_PTR)dataEntry->DllBase;

        entry = DEREF(dataEntry);
        if (entry == beginEntry) // Because it's an cyclic doubly linked list
            break;
    }

    return NULL; // Library not found
}

FARPROC GetPr0cAddr3ss(ULONG_PTR dllBase, LPCSTR procName)
{
    ULONG_PTR ntHeader = dllBase + ((PIMAGE_DOS_HEADER)dllBase)->e_lfanew;
    auto exportDirOffset = &(((PIMAGE_NT_HEADERS)ntHeader)->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT]);
    auto exportDir = (PIMAGE_EXPORT_DIRECTORY)(dllBase + exportDirOffset->VirtualAddress);

    ULONG_PTR names = (dllBase + exportDir->AddressOfNames);
    ULONG_PTR nameOrdinals = (dllBase + exportDir->AddressOfNameOrdinals);
    DWORD funcCount = exportDir->NumberOfNames;

    while (funcCount--)
    {
        auto curFuncName = (LPSTR)(dllBase + DEREF_32(names));
        if (!strcmp(procName, curFuncName))
        {
            UINT_PTR address = (dllBase + exportDir->AddressOfFunctions);

            address += (DEREF_16(nameOrdinals) * sizeof(DWORD));
            address = dllBase + DEREF_32(address);

            return (FARPROC)address;
        }

        // get the next exported function name
        names += sizeof(DWORD);

        // get the next exported function name ordinal
        nameOrdinals += sizeof(WORD);
    }

    return nullptr; // Function not found
}