using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Model;
using System.Windows.Forms;

namespace SmartHouse.BLL.Controller
{
    public class HeatStoreController
    {
        public HeatStore HeatStore { set; get; }

        public HeatStoreController()
        {
            HeatStore = new HeatStore();
        }

        public void Calculate(List<Weather> weathers, List<CharacteristicHeatLosses> losses, List<TypeOfHeat> heats)
        {
            HeatStore.Records = new List<HeatStoreData>();
            HeatStore.TotalDayConsumption = 0;
            HeatStore.TotalNightConsumption = 0;

            List<HeatStore.Day> days = new List<HeatStore.Day>();
            HeatStore.Day day = new HeatStore.Day
            {
                Date = weathers[0].Date
            };
            
            foreach (var weather in weathers)
            {
                double power = losses.First(x => x.Temperature == weather.Temperature).Heat;
                power = power < 0 ? 0 : power;
                double heatLosses = power * 0.5;
                double startEnergy = GetStartEnergy();
                double availablePower = GetAvailablePower(weather.Date);
                double realPower = GetRealPower(availablePower, startEnergy, heatLosses);
                double endEnergy = GetEndEnergy(availablePower, startEnergy, heatLosses);
                double dayRateEnergy = GetDayRateEnergy(realPower, weather.Date);
                double nightRateEnergy = GetNightRateEnergy(realPower, weather.Date);
                availablePower = CorrectAvailablePower(realPower);

                HeatStore.TotalDayConsumption += dayRateEnergy;
                HeatStore.TotalNightConsumption += nightRateEnergy;

                HeatStore.Records.Add(new HeatStoreData
                {
                    StartDate = weather.Date,
                    EndDate = weather.Date.AddMinutes(30),
                    Temperature = weather.Temperature,
                    HeatLossesPower = power,
                    HeatLosses = heatLosses,
                    AvailablePower = availablePower,
                    RealPower = realPower,
                    StartEnergyAmount = startEnergy,
                    EndEnergyAmount = endEnergy,
                    DayRateEnergy = dayRateEnergy,
                    NightRateEnergy = nightRateEnergy
                });

                if (weather.Date.Year == day.Date.Year &&
                    weather.Date.Month == day.Date.Month &&
                    weather.Date.Day == day.Date.Day)
                {
                    day.EnergyLack += dayRateEnergy;
                    day.HourLack += dayRateEnergy > 0.0001 ? 0.5 : 0;
                }
                else
                {
                    days.Add(day);
                    day = new HeatStore.Day
                    {
                        Date = weather.Date
                    };
                }
            }
            days.Add(day);

            HeatStore.TotalDayConsumption = Math.Round(HeatStore.TotalDayConsumption);
            HeatStore.TotalNightConsumption = Math.Round(HeatStore.TotalNightConsumption);
            HeatStore.TotalConsumption = HeatStore.TotalNightConsumption + HeatStore.TotalDayConsumption;
            HeatStore.TotalDayCost = Math.Round(HeatStore.TotalDayConsumption * HeatStore.DayRate, 2);
            HeatStore.TotalNightCost = Math.Round(HeatStore.TotalNightConsumption * HeatStore.NightRate, 2);
            HeatStore.TotalCost = HeatStore.TotalDayCost + HeatStore.TotalNightCost;
            HeatStore.WorstDay = days.MaxBy(x => x.HourLack).First();

            HeatStore.Heats = heats.ToList();
            HeatStore.Heats.Add(new TypeOfHeat
            {
                Name = "теплонакопичувач",
                Unit = "кВт·год",
                FuelConsumption = HeatStore.TotalConsumption,
                TotalPrice = HeatStore.TotalCost
            });
            var f = HeatStore.Heats.FirstOrDefault(x => x.Name.Equals("електрична енергія"));
            if (f != null)
            {
                f.FuelConsumption = HeatStore.TotalConsumption;
                f.TotalPrice = Math.Round(HeatStore.TotalConsumption * HeatStore.DayRate, 2);
            }

        }
        
        public double GetStartEnergy()
        {
            return HeatStore.Records.Count > 0 ? HeatStore.Records[HeatStore.Records.Count - 1].EndEnergyAmount : 0;
        }
        
        public double GetAvailablePower(DateTime date)
        {
            return IsNight(date) ? HeatStore.Power : 0;
        }

        public bool IsNight(DateTime date)
        {
            TimeSpan time = new TimeSpan(0, date.Hour, date.Minute, 0);
            return time < HeatStore.StartDayZone || time >= HeatStore.EndDayZone;
        }

        public double GetRealPower(double power, double startEnergy, double heatLosses)
        {
            double consumeption = 0.5 * power;
            double endEnergy = startEnergy + consumeption - heatLosses;
            if (HeatStore.Capacity < endEnergy)
            {
                consumeption = HeatStore.Capacity - startEnergy + heatLosses;
            }
            if (endEnergy < 0)
            {
                consumeption = heatLosses - startEnergy;
                consumeption = consumeption < 0 ? 0 : consumeption;
            }
            return Math.Round(consumeption, 2);
        }

        public double GetEndEnergy(double power, double startEnergy, double heatLosses)
        {
            double consumeption = 0.5 * power;
            double endEnergy = startEnergy + consumeption - heatLosses;
            endEnergy = endEnergy < 0 ? 0 : endEnergy;
            endEnergy = HeatStore.Capacity < endEnergy ? HeatStore.Capacity : endEnergy;
            return Math.Round(endEnergy, 2);
        }

        public double GetNightRateEnergy(double consumeption, DateTime date)
        {
            return IsNight(date) ? consumeption : 0;
        }

        public double GetDayRateEnergy(double consumeption, DateTime date)
        {
            return !IsNight(date) ? consumeption : 0;
        }

        public double CorrectAvailablePower(double consumption)
        {
            return consumption > 0 ? HeatStore.Power : 0;
        }

    }
}
