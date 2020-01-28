using System;

namespace SmartHouse.BLL.Model
{
    [Serializable]
    public class HouseThermalParam
    {
        public string Name { set; get; }
        // R
        public double ThermalResist { set; get; }
        // k
        public double ThermalTransfer { set; get; }

        public HouseThermalParam(string name, double thermalResist, double thermalTransfer)
        {
            Name = name;
            ThermalResist = thermalResist;
            ThermalTransfer = thermalTransfer;
        }
    }
}
