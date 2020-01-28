using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class HeatConsumption
    {
        public int Temperature { set; get; }
        public double Duration { set; get; }
        public double Heat { set; get; }
    }
}
