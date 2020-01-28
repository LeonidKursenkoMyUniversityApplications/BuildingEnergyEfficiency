using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model.HeatPump
{
    [Serializable]
    public class HeatPump
    {
        public List<HeatPumpDescription> HeatPumpDescriptions { set; get; }

        // Measures in kWht.
        public double NominalHeatProduction { set; get; }

        // Measures in kWht.
        public double NominalPower { set; get; }
        public double CirculationPower { set; get; }
        public double FancoilPower { set; get; }

        public int HeatPumpCount { set; get; }
        public int CirculationPumpCount { set; get; }
        public int FancoilCount { set; get; }

        [NonSerialized]
        private List<HeatPumpCalculation> _heatPumpCalculations;
        public List<HeatPumpCalculation> HeatPumpCalculations
        {
            set => _heatPumpCalculations = value;
            get => _heatPumpCalculations;
        }

        public double TotalHeatLosses { set; get; }
        public double TotalHeatPumpConsumption { set; get; }
        public double TotalHeatSystemConsumption { set; get; }
        public double TotalQuantityHeatPumpProduction { set; get; }
        public double TotalQuantityHeatSystemProduction { set; get; }
        public double TotalQuantityAdditionalHeatProduction { set; get; }

        public double AverageEfficiencyHeatSystem { set; get; }
        public double AverageEfficiencyHeatPump { set; get; }

        public double PricePerKwht { set; get; }
        public double TotalCost { set; get; }
    }
}
