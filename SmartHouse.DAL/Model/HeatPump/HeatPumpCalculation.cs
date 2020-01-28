using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model.HeatPump
{
    public class HeatPumpCalculation
    {
        public int Temperature { set; get; }
        public double Power { set; get; }
        public double Duration { set; get; }
        public double HeatLoses { set; get; }
        public double HeatProductionCorrection { set; get; }
        public double HeatProduction { set; get; }
        public double HeatPowerCorrection { set; get; }
        public double HeatPower { set; get; }
        public int HeatPumpCount { set; get; }
        public double AdditionalHeatPower { set; get; }
        public double Load { set; get; }
        public double CirculationPumpPower { set; get; }
        public double HeatPumpConsumption { set; get; }
        public double HeatSystemConsumption { set; get; }
        public double QuantityHeatPumpProduction { set; get; }
        public double QuantityHeatSystemProduction { set; get; }
        public double QuantityAdditionalHeatProduction { set; get; }
    }
}
