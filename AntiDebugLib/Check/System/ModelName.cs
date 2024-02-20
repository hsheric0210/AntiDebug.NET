using System;
using System.Management;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiVirtualization.cs#L76
    /// </item>
    /// </list>
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
                    var manufacturer = Item["Manufacturer"].ToString();
                    var model = Item["Model"].ToString();
                    if (string.Equals(manufacturer, "Microsoft Corporation", StringComparison.OrdinalIgnoreCase) && model.IndexOf("Virtual", StringComparison.OrdinalIgnoreCase) >= 0 || manufacturer.IndexOf("vmware", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Logger.Information("Suspicious computor manufacturer {manufacturer} and model name {name}.", manufacturer, model);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
