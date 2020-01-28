using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.BLL.Model
{
    public class WindGeneration
    {
        public int Wind { set; get; }
        public double Duration { set; get; }
        public double Power { set; get; }
        public double Energy { set; get; }
    }
}
