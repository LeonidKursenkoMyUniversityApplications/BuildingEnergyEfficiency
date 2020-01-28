using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Model.Report
{
    public class WindReport
    {
        public List<string> ImgPaths { set; get; }
        public List<List<string>> WindGeneratorCharacteristicsTable { set; get; }
        public List<List<string>> WindGenerationsTable { set; get; }
        public double TowerHeight { set; get; }
        public double WindTotalEnergyGen { set; get; }

        public double GreenPrice { set; get; }
        public double ReducedPollutionPrice { set; get; }
        public double Co2ReducedPollution { set; get; }
        public double EnergyCost { set; get; }
        public double ReducedPollutionCost { set; get; }
    }
}
