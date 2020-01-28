using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model;
using SmartHouse.DAL.Model.Wind;

namespace SmartHouse.BLL.Controller
{
    public class WindEnergyController
    {
        private string _fileName;

        public List<Weather> Weathers { set; get; }
        public List<WindGeneratorDescription> WindGenDes { set; get; }
        public List<WindGeneration> WindGenerations { set; get; }

        // Вироблена електроенергія.
        public double TotalEnergy { set; get; }
        // Зелений тариф.
        public double GreenPrice { set; get; }
        // Ціна продажу одиниць скорочення викидів (ОСВ) за тону.
        public double ReducedPollutionPrice { set; get; }
        // Оцінка обсягів скорочень викидів парникових газів у тонах СО2 еквіваленту.
        public double Co2ReducedPollution { set; get; }
        // Дохід від продажу електроенергії.
        public double EnergyCost { set; get; }
        // Дохід від продажу одиниць скорочення викидів (ОСВ).
        public double ReducedPollutionCost { set; get; }

        // Height of wind generator tower, m.
        public double Height { set; get; }
        // Height of meteorogical measures, m.
        public double HeightMeteorogical { set; get; }

        public WindEnergyController(string fileName)
        {
            _fileName = fileName;
            var windGenDes = BinaryController.ReadDataFromBinary<WindGeneratorDescription>(fileName);
            WindGenDes = windGenDes.Count == 0 ? new List<WindGeneratorDescription>() : windGenDes;
        }

        public void Save()
        {
            BinaryController.WriteDataToBinary(_fileName, WindGenDes);
        }

        public List<Weather> GetWeathers(DateTime start, DateTime end)
        {
            return Weathers.Where(x => (x.Date >= start) && (x.Date <= end)).ToList();
        }

        public void GetWindGenerations()
        {
            WindGenerations = new List<WindGeneration>();
            foreach (var description in WindGenDes)
            {
                WindGenerations.Add(new WindGeneration
                {
                    Wind = description.Wind,
                    Power = description.Power
                });
            }
            foreach (var weather in Weathers)
            {
                double wind = Math.Round(weather.WindPower * Math.Pow(Height / HeightMeteorogical, 0.14), 0);
                int i = WindGenerations.FindIndex(x => x.Wind == wind);
                if (i != -1)
                {
                    WindGenerations[i].Duration += 0.5;
                }
            }
            foreach (var windGeneration in WindGenerations)
            {
                windGeneration.Energy = Math.Round(windGeneration.Duration * windGeneration.Power, 2);
            }

            TotalEnergy = WindGenerations.Sum(x => x.Energy);
        }

        public void GetEconomic()
        {
            Co2ReducedPollution = Math.Round(1.06 * TotalEnergy / 1000, 2);
            EnergyCost = Math.Round(GreenPrice * TotalEnergy, 2);
            ReducedPollutionCost = Math.Round(ReducedPollutionPrice * Co2ReducedPollution, 2);
        }
    }
}
