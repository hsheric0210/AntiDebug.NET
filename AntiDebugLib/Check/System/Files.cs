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
    internal class Files : CheckBase
    {
        public override string Name => "Drivers";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private readonly string[] driverNames = new string[]
        {
            "netkvm.sys",       // NetKVM

            "balloon.sys",      // virtio-win
            "vioinput.sys",     // virtio-win
            "viofs.sys",        // virtio-win
            "vioser.sys",       // virtio-win

            "vboxmouse.sys",            // VirtualBox
            "vboxguest.sys",            // VirtualBox
            "vboxsf.sys",               // VirtualBox
            "vboxvideo.sys",            // VirtualBox
            "vboxdisp.dll",             // VirtualBox
            "vboxhook.dll",             // VirtualBox
            "vboxmrxnp.dll",            // VirtualBox
            "vboxogl.dll",              // VirtualBox
            "vboxoglarrayspu.dll",      // VirtualBox
            "vboxoglcrutil.dll",        // VirtualBox
            "vboxoglerrorspu.dll",      // VirtualBox
            "vboxoglfeedbackspu.dll",   // VirtualBox
            "vboxoglpackspu.dll",       // VirtualBox
            "vboxoglpassthroughspu.dll",// VirtualBox
            "vboxservice.dll",          // VirtualBox
            "vboxtray.exe",             // VirtualBox
            "VBoxControl.exe",          // VirtualBox

            "vmnet.sys",        // vmware
            "vmmouse.sys",      // vmware
            "vmusb.sys",        // vmware
            "vm3dmp.sys",       // vmware
            "vmci.sys",         // vmware
            "vmhgfs.sys",       // vmware
            "vmmemctl.sys",     // vmware
            "vmx86.sys",        // vmware
            "vmrawdsk.sys",     // vmware
            "vmusbmouse.sys",   // vmware
            "vmkdb.sys",        // vmware
            "vmnetuserif.sys",  // vmware
            "vmnetadapter.sys", // vmware
        };

        public override CheckResult CheckPassive()
        {
            foreach (var name in driverNames)
            {
                var path = Path.Combine(Environment.SystemDirectory, name);
                if (File.Exists(path))
                {
                    Logger.Information("Bad module file {name} found on system32.", name);
                    return DebuggerDetected(new { Path = path });
                }

                path = Path.Combine(Environment.SystemDirectory, "drivers", name);
                if (File.Exists(path))
                {
                    Logger.Information("Bad module file {name} found on drivers directory.", name);
                    return DebuggerDetected(new { Path = path });
                }
            }

            return DebuggerNotDetected();
        }
    }
}
