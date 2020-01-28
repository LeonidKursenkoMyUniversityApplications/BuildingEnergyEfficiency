using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class WindRose
    {
        public int DirectionAngle { set; get; }
        public string Direction { set; get; }
        public double Frequency { set; get; }
        public double FrequencyPercent { set; get; }
        public List<double> FrequencyPercentCategories { set; get; }

        public WindRose()
        {
            FrequencyPercentCategories = new List<double>();
            for (int i = 0; i < 4; i++)
            {
                FrequencyPercentCategories.Add(0);
            }
        }
    }
}
