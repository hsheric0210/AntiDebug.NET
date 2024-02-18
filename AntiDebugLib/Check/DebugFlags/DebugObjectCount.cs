using System;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeStructs;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.Exploits
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

        public override bool CheckPassive()
        {
            const uint ObjectAllTypesInformation = 0x3;
            var buffer = IntPtr.Zero;
            var beingDebugged = false;

            try
            {
                var status = NtQueryObject(IntPtr.Zero, ObjectAllTypesInformation, IntPtr.Zero, 0, out var size);
                if (status != STATUS_INFO_LENGTH_MISMATCH)
                {
                    Logger.Error("NtQueryObject(get_length) failed: NTSTATUS {status:X}", status);
                    goto cleanup;
                }

                buffer = Marshal.AllocHGlobal((IntPtr)size);
                if (buffer == IntPtr.Zero)
                {
                    Logger.Error("Unmanaged memory allocation failed.");
                    goto cleanup;
                }

                status = NtQueryObject(IntPtr.Zero, ObjectAllTypesInformation, buffer, size, out var size2);
                if (status != 0)
                {
                    Logger.Error("NtQueryObject(get_data) failed: NTSTATUS {status:X}", status);
                    goto cleanup;
                }

                var info = Marshal.PtrToStructure<OBJECT_ALL_INFORMATION>(buffer);
                for (uint i = 0, j = info.NumberOfObjects; i < j; i++)
                {
                    var tinfo = info.ObjectTypeInformation[i];
                    var tname = Marshal.PtrToStringUni(tinfo.TypeName.Buffer, tinfo.TypeName.Length);
                    if (string.Equals(tname, "DebugObject") && tinfo.TotalNumberOfObjects > 0)
                    {
                        beingDebugged = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occurred.");
            }

cleanup:
            if (buffer != IntPtr.Zero)
                Marshal.FreeHGlobal(buffer);
            return beingDebugged;
        }
    }
}
