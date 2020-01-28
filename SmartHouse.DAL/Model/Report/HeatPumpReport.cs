using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model.Report
{
    public class HeatPumpReport
    {
        public List<string> ImgPaths { set; get; }
        public List<List<string>> HeatPumpCharacteristicTable { set; get; }
        public List<List<string>> HeatPumpCalculationsTable { set; get; }
        public List<List<string>> HeatPumpCostTable { set; get; }

        public double NominalHeatProduction { set; get; }
        public double NominalPower { set; get; }
        public int HeatPumpCount { set; get; }
        public double CirculationPower { set; get; }
        public int CirculationPumpCount { set; get; }
        public double FancoilPower { set; get; }
        public int FancoilCount { set; get; }

        public double TotalHeatLosses { set; get; }
        public double TotalHeatPumpConsumption { set; get; }
        public double TotalHeatSystemConsumption { set; get; }
        public double TotalQuantityHeatPumpProduction { set; get; }
        public double TotalQuantityAdditionalHeatProduction { set; get; }
        public double TotalQuantityHeatSystemProduction { set; get; }
        public double AverageEfficiencyHeatPump { set; get; }
        public double AverageEfficiencyHeatSystem { set; get; }
        public double TotalCost { set; get; }
    }
}
