using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Pathes
    {
        public List<string> WeatherFiles { set; get; }
        public string SunFile { set; get; }
    }
}
