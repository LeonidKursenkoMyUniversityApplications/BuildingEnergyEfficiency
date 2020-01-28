using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model
{
    [Serializable]
    public class Sun
    {
        public DateTime Date { set; get; }
        public int Insolation { set; get; }

        public Sun(DateTime date, int insolation)
        {
            Date = date;
            Insolation = insolation;
        }

        public Sun() : this(new DateTime(), 0)
        {
            
        }
    }
}
