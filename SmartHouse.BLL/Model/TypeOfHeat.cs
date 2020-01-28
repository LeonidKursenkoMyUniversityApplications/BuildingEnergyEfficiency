using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class TypeOfHeat
    {
        public string Name { set; get; }
        // Кількість палива на 1 кВтˑгод енергії.
        public double FuelPerKWht { set; get; }
        // Unit of measure.
        public string Unit { set; get; }
        // Ціна одиниці палива
        public double CostPerFuelUnit { set; get; }
        // ККД
        public double Efficience { set; get; }
        // Final price.
        public double TotalPrice { set; get; }
        // Fuel consumption.
        public double FuelConsumption { set; get; }
    }
}
