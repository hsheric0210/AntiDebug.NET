using System;
using System.IO;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.Exploits
{
    /// <summary>
    /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_HandlesValidation.cpp#L4
    /// https://ezbeat.tistory.com/219
    /// </summary>
    public class LoadThenOpen : CheckBase
    {
        public override string Name => "LoadLibrary then try to open";

        public override CheckReliability Reliability => CheckReliability.Great;

        public override bool CheckPassive()
        {
            var path = Path.Combine(Environment.SystemDirectory, "calc.exe"); // Try to load calculator XD
            var lib = IntPtr.Zero;
            try
            {
                lib = LoadLibrary(path);

                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Dispose();

                return false;
            }
            catch (Exception ex)
            {
                Logger.Information(ex, "CreateFile() failed for {path}. (possible being debugged)", path);
                return true; // CreateFile will return INVALID_IntPtr_VALUE
            }
            finally
            {
                if (lib != IntPtr.Zero)
                    FreeLibrary(lib);
            }
        }
    }
}
