using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Weather
    {
        public DateTime Date { set; get; }
        public int Temperature { set; get; }
        public string WindDirection { set; get; }
        public int WindPower { set; get; }

        public Weather(DateTime date, int temperature, string windDirection, int windPower)
        {
            Date = date;
            Temperature = temperature;
            WindDirection = windDirection;
            WindPower = windPower;
        }

        public Weather() : this(new DateTime(), 0, "", 0)
        {
        }
    }
}
