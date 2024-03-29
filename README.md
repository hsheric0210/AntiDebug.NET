# AntiDebug.NET :: Anti-debugging made easy

<p align="center" style="text-align:center">
    <img alt="AntiDebug.NET logo" src="icon.png">
</p>

<div align="center" style="text-align:center">
    <img alt="GitHub License" src="https://img.shields.io/github/license/hsheric0210/AntiDebug.NET">
    <img alt="GitHub Issues or Pull Requests" src="https://img.shields.io/github/issues/hsheric0210/AntiDebug.NET">
    <p style="font-size:large">Protect your .NET program from being debugged!</p>
</div>

---

Various anti-debugging and anti-vm, anti-sandbox techniques are all supported!

To prevent detection and ease obfuscation, all sensitive function calls are handled indirectly and dynamically.
Instead of using direct P/Invoke, they're all retrieved dynamically via hand-made GetProcAddress alternative. (Manually scan IAT to find function, to bypass GetProcAddress hooking)

Complicated anti-debug features are implemented using a native DLL. It is loaded 'in-memory' (without leaving file on disk) when it's executed and freed on exit.

## Disclaimer

Use at your own risk! Your program may not work after applying AntiDebug.NET.

As it uses various _Anti-Anti-Anti-Debug_ techniques, your Anti-virus softwares may complain about your program as if it is infected.

### This project _Manual Maps_ DLL: `kernel32.dll` and `ntdll.dll`

Popular Anti-Anti-Debug solutions such as [ScyllaHide](https://github.com/x64dbg/ScyllaHide) just hooks all debugging-related functions and manipulates its parameters or return value to bypass Anti-Debug solutions.

There is a lot of methods to counter this. Basically, this problem is very similar to 'AV/EDR bypassing.'
[There is a great documentation about this.](https://www.advania.co.uk/insights/blog/a-practical-guide-to-bypassing-userland-api-hooking/)

AntiDebug.NET do manual map the whole `kernel32.dll` and `ntdll.dll` modules using [StealthModule.NET](https://github.com/hsheric0210/StealthModule.NET) to the process memory and then call its exports.

**Since this solution is also (in-)famous for AV/EDR bypassing method, your Anti-virus software may start screaming on this. It may terminate, quarantine or delete your program right after.**

As there is no malicious intention of this manual-mapping behavior, you can ignore the warning or notification from your anti-virus.

If you really doubt about it, feel free to look around this repository, and look if there is any _malicious_ code.

## Binary files are not available for download or share

This project will _NOT_ be published anywhere such as NuGet, GitHub Release, etc. in a binary form as **it could trigger web antiviruses and safe search**.

You should download the project and then manually compile it. Then copy the 'AntiDebugLib.dll' to your project and then add reference to it.

**Don't forget to add the project folder (or at least the dll output folder) to the exclusion list of antivirus!**

## Usage

```csharp
using AntiDebugLib;
```

There are two types of checks. Passive checks are executed once at the begin. Active checks are executed for each 3 seconds. (This period can be changed)

### Standard usage (detect debuggers)

Initialize the AntiDebug modules, register the event handler, and then begin the job.

`AntiDebug.Initialize()` will create and initialize check and prevention instances.

`AntiDebug.BeginChecks()` will perform all passive checks, then start a thread to perform active checks periodically. You can specify the optional `int` parameter to set active check execution period in milliseconds.

```csharp
AntiDebug.Initialize();
AntiDebug.DebuggerDetected += AntiDebug_DebuggerDetected;
AntiDebug.BeginChecks();
```

The handler method

```csharp
private static void AntiDebug_DebuggerDetected(object sender, DebuggerDetectedEventArgs e)
{
    Console.WriteLine("A potential debugging behavior is detected! (Check name: " + e.Result.CheckName + ", Check reliability: " + e.Result.Reliability + ")");
}
```

### Monitoring usage (print all check and prevention results)

The code is too long to note here. See `Program.cs` in AntiDebugSample project for the exact implementation.

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
* [ ] Unhook on start to prevent IAT overwrite hooking.

### ShowStopper (= Checkpoint Research Anti-Debug Tricks)

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

* [ ] WriteWatch
* [ ] WUDF_IsDebuggerPresent
* [x] SetHandleInformation_API
* [x] SeDebugPrivilege
* [ ] ProcessJob
* [x] ProcessHeap_ForceFlags
* [x] ProcessHeap_Flags
* [x] PageExceptionBreakpointCheck
* [ ] NtSystemDebugControl
* [x] NtSetInformationThread_ThreadHideFromDebugger
* [x] NtQueryObject_ObjectTypeInformation
* [ ] NtQueryObject_AllTypesInformation
* [x] NtGlobalFlag

</details>
