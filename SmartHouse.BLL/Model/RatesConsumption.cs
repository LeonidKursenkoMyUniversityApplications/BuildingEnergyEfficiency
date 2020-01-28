using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class RatesConsumption
    {
        // Day of week
        public string Name { set; get; }
        // Wспож - обсяги споживання 
        public double Consumption { set; get; }
        // пікове (Pпік.) 
        public double PowerMax { set; get; }
        // середнє (Pсер.) навантаження 
        public double PowerAverage { set; get; }
        // тривалість використання максимального навантаження (Тmax)
        public double DurationOfMaxPower { set; get; }
        // ступінь нерівномірності ГЕН
        public double DegreeOfUnevenness { set; get; }
        // коефіцієнт використання встановленої потужності (kвик.). 
        public double RateOfUsePower { set; get; }
    }
}
