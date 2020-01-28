using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class DeviceDayOfWeek
    {
        public string Name { set; get; }
        public List<Period> Periods { set; get; }

        public DeviceDayOfWeek(string name, List<Period> periods)
        {
            Name = name;
            Periods = periods;
        }
    }
}
