using AntiDebugLib.Native;

namespace AntiDebugLib.Check.Handle.CloseHandle
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/object-handles.html#closehandle
    /// </item>
    /// <item>
    /// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_HandlesValidation.cpp#L26
    /// </item>
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L76
    /// </item>
    /// <item>
    /// MinigamesAntiCheat :: https://github.com/AdvDebug/MinegamesAntiCheat/blob/60bc0894981cb531b8de4a085876e3503e9f79f0/MinegamesAntiCheat/MinegamesAntiCheat/AntiDebugging.cs#L37
    /// </item>
    /// <item>
    /// Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection/blob/585e33cbe57aa97725b3f98658944b01f1844562/src/ObjectHandles/CloseHandleTrick.cs#L11
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/CloseHandle_InvalidHandle.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.B.ii. CloseHandle
    /// </item>
    /// </list>
    /// </summary>
    public class CloseHandleInvalidHandle : CloseHandleInvalidCheckBase
    {
        public override string Name => "Close Handle: kernel32!CloseHandle (invalid handle)";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
        {
            var handle = GetRandomHandle();
            try
            {
                Logger.Debug("Trying to close random handle {handle:X}.", handle);
                Kernel32.CloseHandle(handle);
                return DebuggerNotDetected();
            }
            catch
            {
                return DebuggerDetected(new { Handle = handle });
            }
        }
    }
}
