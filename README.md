# AntiDebug.NET :: Anti-debugging made easy

<svg xmlns="http://www.w3.org/2000/svg" transform="rotate(-14)" width="200" height="200" style="display:block;margin:auto" viewBox="0 0 640 512"><!--!Font Awesome Free 6.5.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2024 Fonticons, Inc.--><path fill="#b42424" d="M38.8 5.1C28.4-3.1 13.3-1.2 5.1 9.2S-1.2 34.7 9.2 42.9l592 464c10.4 8.2 25.5 6.3 33.7-4.1s6.3-25.5-4.1-33.7L477.4 348.9c1.7-9.4 2.6-19 2.6-28.9h64c17.7 0 32-14.3 32-32s-14.3-32-32-32H479.7c-1.1-14.1-5-27.5-11.1-39.5c.7-.6 1.4-1.2 2.1-1.9l64-64c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0l-64 64c-.7 .7-1.3 1.4-1.9 2.1C409.2 164.1 393.1 160 376 160H264c-8.3 0-16.3 1-24 2.8L38.8 5.1zm392 430.3L336 360.7V479.2c36.6-3.6 69.7-19.6 94.8-43.8zM166.7 227.3c-3.4 9-5.6 18.7-6.4 28.7H96c-17.7 0-32 14.3-32 32s14.3 32 32 32h64c0 24.6 5.5 47.8 15.4 68.6c-2.2 1.3-4.2 2.9-6 4.8l-64 64c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l63.1-63.1c24.5 21.8 55.8 36.2 90.3 39.6V335.5L166.7 227.3zM320 0c-53 0-96 43-96 96v3.6c0 15.7 12.7 28.4 28.4 28.4H387.6c15.7 0 28.4-12.7 28.4-28.4V96c0-53-43-96-96-96z"/></svg>

<div style="text-align:center">
<img alt="GitHub License" src="https://img.shields.io/github/license/hsheric0210/AntiDebug.NET">
<img alt="GitHub Issues or Pull Requests" src="https://img.shields.io/github/issues/hsheric0210/AntiDebug.NET">
<p style="font-size:large">Protect your .NET program from being debugged!</p>
</div>

---

Various anti-debugging and anti-vm, anti-sandbox techniques are all supported!

To prevent detection and ease obfuscation, all sensitive function calls are handled indirectly and dynamically.
Instead of using direct P/Invoke, they're all retrieved dynamically via hand-made GetProcAddress alternative. (Manually scan IAT to find function, to bypass GetProcAddress hooking)

TODO: Unhook on start to prevent IAT overwrite hooking.

Complicated anti-debug features are implemented using a native DLL. It is loaded 'in-memory' (without leaving file on disk) when it's executed and freed on exit.

## Binary files are not available for download or share

This project will *NOT* be published anywhere such as NuGet, GitHub Release, etc. in a binary form as **it could trigger web antiviruses and safe search**.

You should download the project and then manually compile it. Then copy the 'AntiDebugLib.dll' to your project and then add reference to it.

**Don't forget to add the project folder (or at least the dll output folder) to the exclusion list of antivirus!**

## Changing magic values

For those who worried about getting caught by native export name strings: Use `RenameNativeExports.ps1`; it will help you to rename native dll export names.

You can also edit the native dll XOR encryption key with this tool.

Usage:

1. Open the powershell, set cwd to this project solution folder. (use `pushd` command to set cwd to the folder where `ChangeMagics.ps1` file is located)
2. Enter: `.\ChangeMagics.ps1`
3. Enter the function names you want.
4. Don't forget to re-build the solution!

## Related articles and repositories

<details>
<summary>Click to expand</summary>

### Related Articles

* The "Ultimate" Anti-Debugging Reference :: http://pferrie.epizy.com/papers/antidebug.pdf
* Check Point Research Anti-Debug Tricks :: https://anti-debug.checkpoint.com/

#### Anti-unpacker tricks by Peter Ferrie

* Paper :: https://pferrie.tripod.com/papers/unpackers.pdf

* Part 1 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200812.pdf
* Part 2 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200901.pdf
* Part 3 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200902.pdf
* Part 4 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200903.pdf
* Part 5 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200904.pdf
* Part 6 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200905.pdf
* Part 7 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200906.pdf
* Part 8 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200812.pdf

### Related Repositories

* al-khaser :: https://github.com/LordNoteworthy/al-khaser
* ShowStopper :: https://github.com/CheckPointSW/showstopper
* HackOvert/AntiDBG :: https://github.com/HackOvert/AntiDBG
* ThomasThelen/Anti-Debugging :: https://github.com/ThomasThelen/Anti-Debugging
* revsic/AntiDebugging :: https://github.com/revsic/AntiDebugging
* Aegis :: https://github.com/rafael-santiago/aegis
* Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection
* Blackhat 2012 Presentation Samples :: https://github.com/rrbranco/blackhat2012

* MinigamesAntiCheat :: https://github.com/AdvDebug/MinegamesAntiCheat

* Ahora57/RaceCondition :: https://github.com/Ahora57/RaceCondition
* Ahora57/Unabomber :: https://github.com/Ahora57/Unabomber

* AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet
* NetShield Protector :: https://github.com/AdvDebug/NetShield_Protector
* MindLated :: https://github.com/Sato-Isolated/MindLated

* ScyllaHide :: https://github.com/x64dbg/ScyllaHide
* HyperHide :: https://github.com/Air14/HyperHide
* Unprotect :: https://github.com/fr0gger/unprotect

</details>

## TODO list

<details>
<summary>Click to expand</summary>

* [x] Implement features in C# if possible. Only use native part when the feature is not able to or too hard to be implemented in C#.
* [x] Reduce the count of exports in native dll as much as possible. (To ease renaming exports)
    * Use 64-bit bitflag to transfer check configuration and results.
    * Use dynamic loader to load the dll in-memory on C# part.

### Checkpoint Research Anti-Debugging Techniques

* [x] /debugflags/CheckRemoteDebuggerPresent
* [x] /debugflags/RtlQueryProcessHeapInformation
* [x] /debugflags/RtlQueryProcessDebugInformation
* [x] /debugflags/BeingDebugged (PEB)
* [x] /debugflags/NtGlobalFlag (PEB)
* [x] /debugflags/HeapFlags (PEB)
* [ ] /debugflags/HeapProtection (0xABABABAB or 0xFEEEFEEE)

* [ ] /directdbginteraction/AntiDebug_BlockInput
* [x] /directdbginteraction/AntiDebug_NtSetInformationThread
* [ ] /directdbginteraction/AntiDebug_SuspendThread

* [x] /handlesvalidation/OpenProcess
* [x] /handlesvalidation/CreateFile
* [x] /handlesvalidation/LoadLibrary
* [x] /handlesvalidation/NtQueryObject

* [x] /memorychecks/AntiDebug_MemoryBreakpoints
* [x] /memorychecks/AntiDebug_HardwareBreakpoints
* [x] /memorychecks/AntiDebug_Toolhelp32ReadProcessMemory (_returnaddress)
* [x] /memorychecks/AntiDebug_FunctionPatch

* [ ] /misc/AntiDebug_FindWindow
* [ ] /misc/AntiDebug_ParentProcessCheck_NtQueryInformationProcess
* [ ] /misc/AntiDebug_DbgPrint
* [ ] /misc/AntiDebug_DbgSetDebugFilterState

* [ ] /timing/AntiDebug_GetLocalTime
* [ ] /timing/AntiDebug_GetSystemTime
* [ ] /timing/AntiDebug_QueryPerformanceCounter
* [ ] /timing/AntiDebug_timeGetTime

### al-khaser

* [ ] /al_khaser/WriteWatch
* [ ] /al_khaser/WUDF_IsDebuggerPresent
* [x] /al_khaser/SetHandleInformation_API
* [x] /al_khaser/SeDebugPrivilege
* [ ] /al_khaser/ProcessJob
* [x] /al_khaser/ProcessHeap_ForceFlags
* [x] /al_khaser/ProcessHeap_Flags
* [x] /al_khaser/PageExceptionBreakpointCheck
* [ ] /al_khaser/NtSystemDebugControl
* [x] /al_khaser/NtSetInformationThread_ThreadHideFromDebugger
* [x] /al_khaser/NtQueryObject_ObjectTypeInformation
* [ ] /al_khaser/NtQueryObject_AllTypesInformation
* [x] /al_khaser/NtGlobalFlag

</details>
