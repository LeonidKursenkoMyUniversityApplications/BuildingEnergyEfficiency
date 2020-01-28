using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class HydroElectricLoad
    {
        public int NumberOfShowerUsers { set; get; }
        public int NumberOfBathUsers { set; get; }

        // Measures in litres per day for one user.
        public double WaterQuantityForShower { set; get; }
        public double WaterQuantityForBath { set; get; }

        // Measures in C.
        public double TemperatureOfShower { set; get; }
        public double TemperatureOfBath { set; get; }

        public double TemperatureOfInput { set; get; }
        public double TemperatureOfOutput { set; get; }

        // Time that needs for water heating, hours.
        public double Time { set; get; }

        public double ShowerWaterConsumption => NumberOfShowerUsers * WaterQuantityForShower;
        public double BathWaterConsumption => NumberOfBathUsers * WaterQuantityForBath;

        // Корегування витрати гарячої води для визначеної температури на виході з бака ГВП.
        // Measures in litres per day.
        public double CorrectedShowerWaterConsumption =>
            Math.Round(ShowerWaterConsumption * (TemperatureOfShower - TemperatureOfInput) /
            (TemperatureOfOutput - TemperatureOfInput), 2);
        
        public double CorrectedBathWaterConsumption =>
            Math.Round(BathWaterConsumption * (TemperatureOfBath - TemperatureOfInput) /
                       (TemperatureOfOutput - TemperatureOfInput), 2);

        // Measures in m^3 per day.
        public double TotalWaterConsumption => Math.Round((CorrectedShowerWaterConsumption + 
            CorrectedBathWaterConsumption) / 998.23, 2);

        // Measures in kWht*hour.
        public double Energy => Math.Round(1.163 * TotalWaterConsumption * (TemperatureOfOutput -
                                                                            TemperatureOfInput), 2);

        // Measures in kWht.
        public double Power => Math.Round(Energy / Time, 2);
    }
}
