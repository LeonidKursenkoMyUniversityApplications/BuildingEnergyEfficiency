using System.Collections.Generic;

namespace SmartHouse.BLL.Model
{
    public class ElectricalConsumptionRow
    {
        public string DeviceName { set; get; }
        public List<double> WeekConsumptions { set; get; }
    }
}
