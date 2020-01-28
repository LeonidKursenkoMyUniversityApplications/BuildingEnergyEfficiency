using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class HeatStore
    {
        public TimeSpan StartDayZone { set; get; }
        public TimeSpan EndDayZone { set; get; }
        public double DayRate { set; get; }
        public double NightRate { set; get; }
        public double Capacity { set; get; }
        public double Power { set; get; }
        public List<HeatStoreData> Records { set; get; }
        
        public double TotalNightConsumption { set; get; }
        public double TotalDayConsumption { set; get; }
        public double TotalConsumption { set; get; }

        public double TotalNightCost { set; get; }
        public double TotalDayCost { set; get; }
        public double TotalCost { set; get; }

        public double TotalHourLack { set; get; }

        public class Day
        {
            public DateTime Date { set; get; }
            public double HourLack { set; get; }
            public double EnergyLack { set; get; }
        }

        public Day WorstDay { set; get; }
        public List<TypeOfHeat> Heats { set; get; }
    }
}
