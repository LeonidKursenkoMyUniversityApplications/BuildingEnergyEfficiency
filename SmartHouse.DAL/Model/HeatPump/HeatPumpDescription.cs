using System;

namespace SmartHouse.DAL.Model.HeatPump
{
    [Serializable]
    public class HeatPumpDescription
    {
        public int Temperature { set; get; }
        public double HeatProductionCorrection { set; get; }
        public double HeatPowerCorrection { set; get; }
    }
}
