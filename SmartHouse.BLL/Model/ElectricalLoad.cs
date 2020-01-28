using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class ElectricalLoad
    {
        public double Power { set; get; }
        public DateTime Start { set; get; }
        public DateTime End { set; get; }

        public ElectricalLoad(double power, DateTime start, DateTime end)
        {
            Power = power;
            Start = start;
            End = end;
        }

        public ElectricalLoad() : this(0, new DateTime(), new DateTime())
        {

        }
    }
}
