using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model.Report
{
    public class ThermalEnergySchemeReport
    {
        public List<List<string>> EtalonHeatLossesTable { set; get; }
        public double TotalHeatConsumption { set; get; }
        public double TotalHeatHelConsumption { set; get; }
        public List<List<string>> HeatingTypesTable { set; get; }
        public List<List<string>> HeatingCostsTable { set; get; }
    }
}
