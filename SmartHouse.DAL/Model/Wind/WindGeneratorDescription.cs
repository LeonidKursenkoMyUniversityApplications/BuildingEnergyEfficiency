using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model.Wind
{
    [Serializable]
    public class WindGeneratorDescription
    {
        // Measures in m/s.
        public int Wind { set; get; }
        // Power of wind generator, kWht.
        public double Power { set; get; }
        // Power coefficient.
        public double PowerCoefficient { set; get; }
    }
}
