using AntiDebugLib.Native;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeDefs;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/object-handles.html#ntqueryobject
    /// </item>
    /// <item>
    /// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_HandlesValidation.cpp#L51
    /// </item>
    /// <item>
    /// Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection/blob/585e33cbe57aa97725b3f98658944b01f1844562/src/Hook/IsBadNumberObject.cs#L47
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtQueryObject_AllTypesInformation.cpp
    /// </item>
    /// <item>
    /// https://www.codeproject.com/Articles/30815/An-Anti-Reverse-Engineering-Guide#NtQueryObject
    /// </item>
    /// </list>
    /// </summary>
    public class DebugObjectCount : CheckBase
    {
        public override string Name => "Count of DebugObject (ntdll!NtQueryObject)";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override unsafe CheckResult CheckPassive()
        {
            const uint ObjectAllTypesInformation = 0x3;
            var buffer = IntPtr.Zero;
            var result = DebuggerNotDetected();
            var debugObjectCount = 0;

            try
            {
                var dummyInt = 0u;
                var status = NtQueryObject_ref(IntPtr.Zero, ObjectAllTypesInformation, ref dummyInt, sizeof(ulong), out var allocLength); // it seems just passing (nullptr, 0) to arguments make them return invalid ReturnLength.
                if (status != NTSTATUS.STATUS_INFO_LENGTH_MISMATCH)
                {
                    Logger.Warning("Unable to query the required buffer size. NtQueryObject returned unexpected NTSTATUS {status}.", status);
                    result = NtError("NtQueryObject", status);
                    goto cleanup;
                }

                allocLength += 0x40; // https://www.vbforums.com/showthread.php?859341-How-to-Use-NtQueryObject-function-with-ObjectAllTypesInformation

                Logger.Debug("Allocating buffer of size {size} bytes.", allocLength);
                do
                {
                    buffer = Marshal.AllocHGlobal((IntPtr)allocLength);
                    if (buffer == IntPtr.Zero)
                    {
                        Logger.Warning("Unable to allocate the unmanaged buffer of size {size}.", allocLength);
                        result = Error(new { Function = nameof(Marshal.AllocHGlobal) });
                        goto cleanup;
                    }

                    status = NtQueryObject_IntPtr(IntPtr.Zero, ObjectAllTypesInformation, buffer, allocLength, out var returnLength);

                    if (returnLength > allocLength)
                    {
                        Logger.Debug("It seems the call requires {size} bytes buffer, which is larger than currently allocated {alloc} bytes buffer.", returnLength, allocLength);
                        Marshal.FreeHGlobal(buffer);
                        allocLength = returnLength + 0x40;
                        continue;
                    }

                    Logger.Debug("Return legnth is {size} bytes.", allocLength);
                    if (!NT_SUCCESS(status))
                    {
                        Logger.Warning("Unable to query the all objects. NtQueryObject returned NTSTATUS {status}.", status);
                        result = NtError("NtQueryObject", status);
                        goto cleanup;
                    }
                } while (false);

                var info = Marshal.PtrToStructure<OBJECT_ALL_INFORMATION>(buffer);
                var pinnedArray = GCHandle.Alloc(info.ObjectTypeInformation[0], GCHandleType.Pinned);
                var objInfoLocation = (byte*)pinnedArray.AddrOfPinnedObject();

                for (uint i = 0, j = info.NumberOfObjects; i < j; i++)
                {
                    var typeInfo = *(OBJECT_TYPE_INFORMATION*)objInfoLocation;
                    var typeName = Marshal.PtrToStringUni(typeInfo.TypeName.Buffer, typeInfo.TypeName.Length / 2); // It's a 2-bytes unicode string
                    if (string.Equals(typeName, "DebugObject"))
                    {
                        Logger.Debug("Found DebugObject at {address}. Count of objects: {count}", new IntPtr(objInfoLocation).ToHex(), typeInfo.TotalNumberOfObjects);
                        debugObjectCount += typeInfo.TotalNumberOfObjects;
                    }

                    objInfoLocation = (byte*)typeInfo.TypeName.Buffer;
                    objInfoLocation += typeInfo.TypeName.MaximumLength;

                    var tmp = (ulong)objInfoLocation & (ulong)-sizeof(void*); // align
                    if (tmp != (ulong)objInfoLocation)
                        tmp += (ulong)sizeof(void*);

                    objInfoLocation = (byte*)tmp;
                }

                pinnedArray.Free();
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Unexpected exception caught.");
            }

cleanup:
            if (buffer != IntPtr.Zero)
                Marshal.FreeHGlobal(buffer);

            if (debugObjectCount > 0)
                return DebuggerDetected(new { Count = debugObjectCount });

            return DebuggerNotDetected();
        }
    }
}
