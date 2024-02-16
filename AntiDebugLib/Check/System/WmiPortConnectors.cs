using System.Management;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L202
    /// </summary>
    internal class WmiPortConnectors : CheckBase
    {
        public override string Name => "WMI Win32_PortConnector";

        public override CheckReliability Reliability => CheckReliability.Great;

        public override bool CheckPassive()
            => new ManagementObjectSearcher("SELECT * FROM Win32_PortConnector").Get().Count == 0;
    }
}
