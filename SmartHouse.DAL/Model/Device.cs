using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Device
    {
        public string Name { set; get; }
        public double Power { set; get; }
        public List<DeviceDayOfWeek> DayOfWeek { set; get; }

        public Device(string name, double power, List<DeviceDayOfWeek> dayOfWeek)
        {
            Name = name;
            Power = power;
            DayOfWeek = dayOfWeek;
        }

        public Device(string name, double power)
        {
            Name = name;
            Power = power;
            DayOfWeek = new List<DeviceDayOfWeek>();
            
            for (int i = 0; i < 7; i++)
            {
                DayOfWeek.Add(new DeviceDayOfWeek(Constants.DayOfWeek[i], new List<Period>()));
            }
        }

        public Device() : this("", 0)
        {
            
        }
    }
}
