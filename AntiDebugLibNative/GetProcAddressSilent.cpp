#include "pch.h"
#include "GetProcAddressSilent.h"

ULONG_PTR GetPEB()
{
#ifdef _WIN64
    return __readgsqword(0x60);
#else
    return __readfsdword(0x30);
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