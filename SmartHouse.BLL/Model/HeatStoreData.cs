using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class HeatStoreData
    {
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public int Temperature { set; get; }
        public double HeatLossesPower { set; get; }
        public double HeatLosses { set; get; }
        public double StartEnergyAmount { set; get; }
        public double EndEnergyAmount { set; get; }
        public double AvailablePower { set; get; }
        public double RealPower { set; get; }
        public double NightRateEnergy { set; get; }
        public double DayRateEnergy { set; get; }

    }
}
