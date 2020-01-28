using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model.Report
{
    public class HeatStoreReport
    {
        public List<string> ImgPaths { set; get; }
        public List<List<string>> CostTable { set; get; }
        public TimeSpan StartDayZone { set; get; }
        public TimeSpan EndDayZone { set; get; }
        public double DayRate { set; get; }
        public double NightRate { set; get; }
        public double Capacity { set; get; }
        public double Power { set; get; }

        public double TotalDayConsumption { set; get; }
        public double TotalNightConsumption { set; get; }
        public double TotalNightCost { set; get; }
        public double TotalDayCost { set; get; }
        public double TotalConsumption { set; get; }
        public double TotalCost { set; get; }
        public DateTime WorstDay { set; get; }
        public double EnergyLack { set; get; }
        public double HourLack { set; get; }

    }
}
