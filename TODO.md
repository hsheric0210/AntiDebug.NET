# TODO List

* 최대한 많은 부분은 C# 단에서 구현하기.
* 진짜로 C/C++의 기능을 사용하지 않으면 구현할 수 없는 부분만 `.Native` 단으로 빼내기.
* `.Native` 모듈의 DLLExport를 최대한 줄이기. (디텍을 피하기 위해 함수 이름을 사용자가 자유롭게 바꿀 수 있도록 돕기 위함)
  * 하나의 함수 당 64개의 검사를 수행하고, 성공한 검사와 실패한 검사를 uint64 형에 비트플래그 형태로 반환하면 될 것 같음.
  * .NET 쪽에서 불러올 때도 DLL 다이나믹 로딩 형태로, GetProcAddress로 불러와 쓰도록 하기.

---

c# antidbg impl

/debugflags/CheckRemoteDebuggerPresent
/debugflags/RtlQueryProcessHeapInformation
/debugflags/RtlQueryProcessDebugInformation
/debugflags/BeingDebugged (PEB)
/debugflags/NtGlobalFlag (PEB)
/debugflags/HeapFlags (PEB)
/debugflags/HeapProtection (0xABABABAB or 0xFEEEFEEE)

/directdbginteraction/AntiDebug_BlockInput
/directdbginteraction/AntiDebug_NtSetInformationThread
/directdbginteraction/AntiDebug_SuspendThread

/handlesvalidation/OpenProcess
/handlesvalidation/CreateFile
/handlesvalidation/LoadLibrary
/handlesvalidation/NtQueryObject

https://github.com/CheckPointSW/showstopper/blob/master/src/not_suspicious/Technique_MemoryChecks.cpp
/memorychecks/AntiDebug_MemoryBreakpoints
/memorychecks/AntiDebug_HardwareBreakpoints
/memorychecks/AntiDebug_Toolhelp32ReadProcessMemory (_returnaddress)
/memorychecks/AntiDebug_FunctionPatch

https://github.com/CheckPointSW/showstopper/blob/master/src/not_suspicious/Technique_Misc.cpp
/misc/AntiDebug_FindWindow
/misc/AntiDebug_ParentProcessCheck_NtQueryInformationProcess
/misc/AntiDebug_DbgPrint
/misc/AntiDebug_DbgSetDebugFilterState

https://github.com/CheckPointSW/showstopper/blob/master/src/not_suspicious/Technique_Timing.h
/timing/AntiDebug_GetLocalTime
/timing/AntiDebug_GetSystemTime
/timing/AntiDebug_QueryPerformanceCounter
/timing/AntiDebug_timeGetTime

https://github.com/LordNoteworthy/al-khaser/tree/master/al-khaser/AntiDebug
/antidbg/WriteWatch
/antidbg/WUDF_IsDebuggerPresent
/antidbg/SetHandleInformation_API
/antidbg/SeDebugPrivilege
/antidbg/ProcessJob
/antidbg/ProcessHeap_ForceFlags
/antidbg/ProcessHeap_Flags
/antidbg/PageExceptionBreakpointCheck
/antidbg/NtSystemDebugControl
/antidbg/NtSetInformationThread_ThreadHideFromDebugger
/antidbg/NtQueryObject_ObjectTypeInformation
/antidbg/NtQueryObject_AllTypesInformation
/antidbg/NtGlobalFlag
