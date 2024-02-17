using System.ServiceProcess;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L113
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiVM/KVM.cpp
    /// </item>
    /// </list>
    /// </summary>
    internal class Services : CheckBase
    {
        public override string Name => "Services";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private readonly string[] serviceNames = new string[]
        {
            "vmbus",
            "VMBusHID",
            "hyperkbd",
            "hvservice",
            "HvHost",
            "HyperVideo",
            "vmgid",
            "vmicguestinterface",
            "vmicheartbeat",
            "vmickvpexchange",
            "vmicrdv",
            "vmicshutdown",
            "vmictimesync",
            "vmicvmsession",
            "vmicvss",
            "vioscsi",
            "viostor",
            "VirtIO-FS Service",
            "VirtioSerial",
            "Balloon",
            "BalloonService",
            "netkvm",
            "VBoxWddm",
            "VBoxSF",               //VirtualBox Shared Folders
            "VBoxMouse",            //VirtualBox Guest Mouse
            "VBoxGuest",            //VirtualBox Guest Driver
            "vmci",                 //VMWare VMCI Bus Driver
            "vmhgfs",               //VMWare Host Guest Control Redirector
            "vmmouse",
            "vmmemctl",             //VMWare Guest Memory Controller Driver
            "vmusb",
            "vmusbmouse",
            "vmx_svga",
            "vmxnet",
            "vmx86"
        };

        public override bool CheckPassive()
        {
            foreach (var service in ServiceController.GetServices())
            {
                if (service.Status == ServiceControllerStatus.Stopped)
                    continue;

                foreach (var name in serviceNames)
                {
                    if (service.ServiceName.Contains(name))
                    {
                        Logger.Information("Bad service {name} is running.", name);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
