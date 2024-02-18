using System;
using System.IO;

using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.Exploits
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/object-handles.html#loadlibrary
    /// </item>
    /// <item>
    /// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_HandlesValidation.cpp#L42
    /// </item>
    /// </list>
    /// </summary>
    public class LoadThenOpen : CheckBase
    {
        public override string Name => "LoadLibrary then try to open";

        private static readonly string[] randomBinary = new string[] {
            Path.Combine(Environment.SystemDirectory, "calc.exe"),
            Path.Combine(Environment.SystemDirectory, "notepad.exe"),
            Path.Combine(Environment.SystemDirectory, "control.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "word.exe")
        };

        public override CheckReliability Reliability => CheckReliability.Bad;

        public override bool CheckPassive()
        {
            var path = randomBinary[new Random().Next(randomBinary.Length)];
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
