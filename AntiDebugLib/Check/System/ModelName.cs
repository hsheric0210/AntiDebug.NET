using System;
using System.Management;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L76
    /// </summary>
    internal class ModelName : CheckBase
    {
        public override string Name => "Computor Model Name";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckPassive()
        {
            using (var ObjectSearcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            using (var ObjectItems = ObjectSearcher.Get())
            {
                foreach (var Item in ObjectItems)
                {
                    var ManufacturerString = Item["Manufacturer"].ToString();
                    var ModelName = Item["Model"].ToString();
                    if (string.Equals(Item["Manufacturer"].ToString(), "Microsoft Corporation", StringComparison.OrdinalIgnoreCase)
                        && ModelName.IndexOf("Virtual", StringComparison.OrdinalIgnoreCase) >= 0
                        || ManufacturerString.IndexOf("vmware", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
