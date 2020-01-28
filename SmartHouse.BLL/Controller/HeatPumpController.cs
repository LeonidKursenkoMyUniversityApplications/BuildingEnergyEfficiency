using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model.HeatPump;
using SmartHouse.DAL.Model.Wind;

namespace SmartHouse.BLL.Controller
{
    public class HeatPumpController
    {
        private string _fileName;

        public HeatPump HeatPump { set; get; }
        public List<TypeOfHeat> Heats { set; get; }
        
        public HeatPumpController(string fileName)
        {
            _fileName = fileName;
            var heatPumps = BinaryController.ReadDataFromBinary<HeatPump>(fileName);
            HeatPump = heatPumps.Count == 0 ? new HeatPump() : heatPumps[0];
        }

        public void Save()
        {
            BinaryController.WriteDataToBinary(_fileName, new List<HeatPump>{ HeatPump });
        }

        public void Calculate(HeatLosses heatLosses)
        {
            HeatPump.HeatPumpCalculations = new List<HeatPumpCalculation>();
            var reports = HeatPump.HeatPumpCalculations;
            for(int i = 0; i < heatLosses.HeatConsumptions.Count; i++)
            {
                if(heatLosses.HeatConsumptions[i].Heat <= 0) continue;
                reports.Add(new HeatPumpCalculation
                {
                    Temperature = heatLosses.HeatConsumptions[i].Temperature,
                    Power = heatLosses.Characteristic[i].Heat,
                    Duration = heatLosses.HeatConsumptions[i].Duration,
                    HeatLoses = heatLosses.HeatConsumptions[i].Heat,
                    HeatProductionCorrection = GetHeatProductionCorrection(heatLosses.HeatConsumptions[i].Temperature),
                    HeatPowerCorrection = GetHeatPowerCorrection(heatLosses.HeatConsumptions[i].Temperature)
                });
            }

            foreach (var report in reports)
            {
                report.HeatProduction = Math.Round(report.HeatProductionCorrection * HeatPump.NominalHeatProduction, 2);
                report.HeatPower = Math.Round(report.HeatPowerCorrection * HeatPump.NominalPower, 2);
                report.HeatPumpCount = GetHeatPumpCount(report.Power, report.HeatProduction);
                report.AdditionalHeatPower = Math.Round(report.Power - report.HeatProduction * report.HeatPumpCount, 2);
                report.AdditionalHeatPower = report.AdditionalHeatPower < 0 ? 0 : report.AdditionalHeatPower;
                report.Load = Math.Round((report.Power - report.AdditionalHeatPower) /
                                         report.HeatProduction / report.HeatPumpCount, 2);
                report.CirculationPumpPower = Math.Round(HeatPump.CirculationPower * HeatPump.CirculationPumpCount +
                    HeatPump.FancoilPower * HeatPump.FancoilCount, 2);
                report.HeatPumpConsumption =
                    Math.Round(report.HeatPower * report.HeatPumpCount * report.Load * report.Duration, 2);
                report.HeatSystemConsumption =
                    Math.Round((report.HeatPower * report.HeatPumpCount * report.Load + report.CirculationPumpPower) * 
                    report.Duration, 2);
                report.QuantityHeatPumpProduction = 
                    Math.Round(report.HeatProduction * report.HeatPumpCount * report.Load * report.Duration, 2);
                report.QuantityHeatSystemProduction = 
                    Math.Round((report.HeatProduction * report.HeatPumpCount * report.Load + report.CirculationPumpPower + 
                    report.AdditionalHeatPower) * report.Duration, 2);
                report.QuantityAdditionalHeatProduction = Math.Round(report.AdditionalHeatPower * report.Duration, 2);
            }

            HeatPump.TotalHeatLosses = reports.Select(x => x.HeatLoses).Sum();
            HeatPump.TotalHeatPumpConsumption = reports.Select(x => x.HeatPumpConsumption).Sum();
            HeatPump.TotalHeatSystemConsumption = reports.Select(x => x.HeatSystemConsumption).Sum();
            HeatPump.TotalQuantityHeatPumpProduction = reports.Select(x => x.QuantityHeatPumpProduction).Sum();
            HeatPump.TotalQuantityAdditionalHeatProduction =
                reports.Select(x => x.QuantityAdditionalHeatProduction).Sum();
            HeatPump.TotalQuantityHeatSystemProduction = reports.Select(x => x.QuantityHeatSystemProduction).Sum();

            HeatPump.AverageEfficiencyHeatPump =
                Math.Round(HeatPump.TotalQuantityHeatPumpProduction / HeatPump.TotalHeatPumpConsumption, 2);
            HeatPump.AverageEfficiencyHeatSystem =
                Math.Round(HeatPump.TotalQuantityHeatSystemProduction / HeatPump.TotalHeatSystemConsumption, 2);
            HeatPump.TotalCost = Math.Round(HeatPump.TotalHeatSystemConsumption * HeatPump.PricePerKwht, 2);
        }

        public double GetHeatProductionCorrection(int temperature)
        {
            var list = HeatPump.HeatPumpDescriptions;
            HeatPumpDescription des = list.FirstOrDefault(x => x.Temperature == temperature);
            if (des != null) return des.HeatProductionCorrection;
            if (temperature < list[0].Temperature)
            {
                return GetLineForecast(list[0].Temperature, list[0].HeatProductionCorrection,
                    list[1].Temperature, list[1].HeatProductionCorrection, temperature);
            }
            int n = list.Count - 1;
            return GetLineForecast(list[n].Temperature, list[n].HeatProductionCorrection,
                list[n - 1].Temperature, list[n - 1].HeatProductionCorrection, temperature);
        }

        public double GetLineForecast(double x1, double y1, double x2, double y2, double x)
        {
            return Math.Round((x - x1) / (x2 - x1) * (y2 - y1) + y1, 4);
        }

        public double GetHeatPowerCorrection(int temperature)
        {
            var list = HeatPump.HeatPumpDescriptions;
            HeatPumpDescription des = list.FirstOrDefault(x => x.Temperature == temperature);
            if (des != null) return des.HeatPowerCorrection;
            if (temperature < list[0].Temperature)
            {
                return GetLineForecast(list[0].Temperature, list[0].HeatPowerCorrection,
                    list[1].Temperature, list[1].HeatPowerCorrection, temperature);
            }
            int n = list.Count - 1;
            return GetLineForecast(list[n].Temperature, list[n].HeatPowerCorrection,
                list[n - 1].Temperature, list[n - 1].HeatPowerCorrection, temperature);
        }

        public int GetHeatPumpCount(double heatLoses, double heatPumpProduction)
        {
            int n = (int) Math.Ceiling(heatLoses / heatPumpProduction);
            return n > HeatPump.HeatPumpCount ? HeatPump.HeatPumpCount : n;
        }

        public void SetHeatsCost(List<TypeOfHeat> heats)
        {
            Heats = heats.ToList();
            Heats.Add(new TypeOfHeat
            {
                Name = "тепловий насос",
                FuelConsumption = HeatPump.TotalQuantityHeatSystemProduction,
                Unit = "кВт∙год",
                TotalPrice = HeatPump.TotalCost
            });
        }
    }
}
