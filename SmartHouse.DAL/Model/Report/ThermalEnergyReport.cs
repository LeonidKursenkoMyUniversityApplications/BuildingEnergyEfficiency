using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;

namespace SmartHouse.DAL.Model.Report
{
    public class ThermalEnergyReport
    {
        public List<List<string>> WaterIncomeParamsTable { set; get; }
        public List<List<string>> WaterOutcomeParamsTable { set; get; }
        public List<List<string>> HouseThermalParamsTable { set; get; }
        public List<string> CommonImgPaths { set; get; }
        public List<string> IndividualImgPaths { set; get; }
        public double CommonBuildTemperature { set; get; }
        public double CalcOutsideTemperature { set; get; }
        public ThermalEnergySchemeReport CommonScheme{ set; get;}
        public List<List<string>> IndividualTemperatureModesTable { set; get; }
        public ThermalEnergySchemeReport IndividualScheme { set; get; }
        public string CompareIndVsCommon { set; get; }
    }
}
