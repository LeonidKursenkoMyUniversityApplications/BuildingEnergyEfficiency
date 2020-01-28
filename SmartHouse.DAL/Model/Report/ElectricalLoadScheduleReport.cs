using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;

namespace SmartHouse.DAL.Model.Report
{
    public class ElectricalLoadScheduleReport
    {
        public List<string> ImgPaths { set; get; }
        //public List<string> ImgOpt2Paths { set; get; }
        //public List<string> ImgOpt3Paths { set; get; }

        public List<List<string>> DevicesTable { set; get; }
        public List<List<List<string>>> DurationElectricalLoadsTables { set; get; }
        public List<List<string>> ElectricalConsumptionsTable { set; get; }
        public List<List<string>> ElectricalRatesTable { set; get; }
        public ElectricalPrices  ElectricalPrices { set; get; }

        public List<List<string>> ElectricalPrices1PhaseTable { set; get; }
        public List<List<string>> ElectricalPrices2PhaseTable { set; get; }
        public List<List<string>> ElectricalPrices3PhaseTable { set; get; }
    }
}
