using System;
using System.IO;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L96
    /// </item>
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L142
    /// </item>
    /// </list>
    /// </summary>
    internal class Drivers : CheckBase
    {
        public override string Name => "Drivers";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private readonly string[] driverNames = new string[]
        {
            "netkvm.sys", // NetKVM

            "balloon.sys", // virtio-win
            "vioinput.sys", //virtio-win
            "viofs.sys", // virtio-win
            "vioser.sys", // virtio-win

            "vboxmouse.sys", // VirtualBox
            "vboxguest.sys", // VirtualBox
            "vboxsf.sys", // VirtualBox
            "vboxvideo.sys", // VirtualBox
            "vboxogl.dll", // VirtualBox

            "vmmouse.sys", // vmware
        };

        public override bool CheckPassive()
        {
            foreach (var name in driverNames)
            {
                if (File.Exists(Path.Combine(Environment.SystemDirectory, name)) || File.Exists(Path.Combine(Environment.SystemDirectory, "drivers", name)))
                {
                    Logger.Information("Bad driver {name} found.", name);
                    return true;
                }
            }

            return false;
        }
    }
}
