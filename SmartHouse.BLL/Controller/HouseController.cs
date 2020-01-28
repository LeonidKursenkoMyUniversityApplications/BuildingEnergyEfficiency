using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model;

namespace SmartHouse.BLL.Controller
{
    public enum HeatType { Common, Individual}
    public class HouseController
    {
        private string _fileName;
        public House House { set; get; }

        // Розрахункова температура.
        public double CalculatedTemperature { set; get; }
        public HeatLosses CommonHeatLosses { set; get; }
        public HeatLosses IndividualHeatLosses { set; get; }

        public List<Weather> Weathers { set; get; }
        
        public HouseController(string fileName)
        {
            _fileName = fileName;

            var houses = BinaryController.ReadDataFromBinary<House>(fileName);
            House = houses.Count == 0 ? new House() : houses[0];
            CommonHeatLosses = new HeatLosses();
            IndividualHeatLosses = new HeatLosses();
        }

        public void Save()
        {
            BinaryController.WriteDataToBinary(_fileName, new List<House>{House});
        }

        public void Calculate()
        {
            GetCommonHeatLosses();
            GetCommonCharacteristicHeatLosses();
            GetHeat(CommonHeatLosses);
            CalculatePrice(CommonHeatLosses);

            GetIndividualHeatLosses();
            GetIndividualCharacteristicHeatLosses();
            GetHeat(IndividualHeatLosses);
            CalculatePrice(IndividualHeatLosses);
        }

        private void GetCommonHeatLosses()
        {
            double k = House.HouseParams.First(x => x.Name.Equals(Constants.Ground)).ThermalTransfer;
            double area = House.GroundArea;
            double tInside = House.Temperature;
            double tOutSide = CalculatedTemperature;
            CommonHeatLosses.Ground = HeatLosses(k, area, tInside, tOutSide);

            k = House.HouseParams.First(x => x.Name.Equals(Constants.Roof)).ThermalTransfer;
            area = House.RoofArea;
            CommonHeatLosses.Roof = HeatLosses(k, area, tInside, tOutSide);

            k = House.HouseParams.First(x => x.Name.Equals(Constants.OutsideWalls)).ThermalTransfer;
            area = House.OutsideWallsArea;
            CommonHeatLosses.Walls = HeatLosses(k, area, tInside, tOutSide);

            k = House.HouseParams.First(x => x.Name.Equals(Constants.Window)).ThermalTransfer;
            area = House.WindowsArea;
            CommonHeatLosses.Windows = HeatLosses(k, area, tInside, tOutSide);

            CommonHeatLosses.Total = CommonHeatLosses.Ground + CommonHeatLosses.Roof +
                                     CommonHeatLosses.Walls + CommonHeatLosses.Windows;
        }

        private void GetIndividualHeatLosses()
        {
            double k = House.HouseParams.First(x => x.Name.Equals(Constants.Ground)).ThermalTransfer;
            double tOutSide = CalculatedTemperature;
            double area;
            double tInside;
            IndividualHeatLosses.Ground = 0;
            foreach (var room in House.Floors[0].Rooms)
            {
                area = room.Area;
                tInside = room.Temperature;
                IndividualHeatLosses.Ground += HeatLosses(k, area, tInside, tOutSide);
            }

            double k0 = k;
            k = House.HouseParams.First(x => x.Name.Equals(Constants.Roof)).ThermalTransfer;
            area = House.RoofArea;
            tInside = House.Temperature;
            IndividualHeatLosses.Roof = Math.Round(IndividualHeatLosses.Ground / k0 * k, 2);

            k = House.HouseParams.First(x => x.Name.Equals(Constants.OutsideWalls)).ThermalTransfer;
            IndividualHeatLosses.Walls = 0;
            foreach (var floor in House.Floors)
            {
                foreach (var room in floor.Rooms)
                {
                    area = room.Walls.Where(x => x.NeirRoom == null).Sum(x => x.Area);
                    tInside = room.Temperature;
                    IndividualHeatLosses.Walls += HeatLosses(k, area, tInside, tOutSide);
                }
            }

            k = House.HouseParams.First(x => x.Name.Equals(Constants.Window)).ThermalTransfer;
            IndividualHeatLosses.Windows = 0;
            foreach (var floor in House.Floors)
            {
                foreach (var room in floor.Rooms)
                {
                    area = room.WindowsArea;
                    tInside = room.Temperature;
                    IndividualHeatLosses.Windows += HeatLosses(k, area, tInside, tOutSide);
                }
            }

            IndividualHeatLosses.Total = IndividualHeatLosses.Ground + IndividualHeatLosses.Roof +
                                         IndividualHeatLosses.Walls + IndividualHeatLosses.Windows;
        }

        private double HeatLosses(double thermalTransfer, double area, double tInside, double tOutSide)
        {
            return Math.Round(thermalTransfer * area * (tInside - tOutSide) * 0.001, 2);
        }

        public List<Weather> GetWeathers(DateTime start, DateTime end)
        {
            return Weathers.Where(x => (x.Date >= start) && (x.Date <= end)).ToList();
        }

        public double GetHeatConsumption(double temperature, double heatCalc, double tCalc, double heatIn, double tIn)
        {
            return (heatIn - heatCalc) / (tIn - tCalc) * (temperature - tCalc) + heatCalc;
        }

        public double GetCommonHeatConsumption(double temperature)
        {
            return GetHeatConsumption(temperature, CommonHeatLosses.Total, CalculatedTemperature, 0, House.Temperature);
        }

        public double GetIndividualHeatConsumption(double temperature)
        {
            double maxT = House.Floors.Select(f => f.Rooms).SelectMany(x => x).ToList().Max(x => x.Temperature);
            return GetHeatConsumption(temperature, IndividualHeatLosses.Total, 
                CalculatedTemperature, 0, maxT);
        }

        public void GetCommonCharacteristicHeatLosses()
        {
            CommonHeatLosses.Characteristic = new List<CharacteristicHeatLosses>();
            List<int> temperatures = Weathers.Select(x => x.Temperature).Distinct().ToList();
            temperatures.Sort();
            foreach (var temperature in temperatures)
            {
                CommonHeatLosses.Characteristic.Add(new CharacteristicHeatLosses()
                {
                    Temperature = temperature,
                    Heat = Math.Round(GetCommonHeatConsumption(temperature), 2)
                });
            }
        }

        public void GetIndividualCharacteristicHeatLosses()
        {
            IndividualHeatLosses.Characteristic = new List<CharacteristicHeatLosses>();
            List<int> temperatures = Weathers.Select(x => x.Temperature).Distinct().ToList();
            temperatures.Sort();
            foreach (var temperature in temperatures)
            {
                IndividualHeatLosses.Characteristic.Add(new CharacteristicHeatLosses()
                {
                    Temperature = temperature,
                    Heat = Math.Round(GetIndividualHeatConsumption(temperature), 2)
                });
            }
        }

        public void GetHeat(HeatLosses heatLosses)
        {
            heatLosses.HeatConsumptions = new List<HeatConsumption>();
            var chs = heatLosses.Characteristic;
            foreach (var ch in chs)
            {
                double duration = Weathers.Count(x => x.Temperature == ch.Temperature) * 0.5;
                heatLosses.HeatConsumptions.Add(new HeatConsumption()
                {
                    Temperature = ch.Temperature,
                    Duration = duration,
                    Heat = Math.Round(ch.Heat * duration, 2)
                });
            }
            heatLosses.TotalHeatConsumption = heatLosses.HeatConsumptions.Select(x => x.Heat)
                .Where(x => x > 0).Sum();
            heatLosses.TotalHeatHelConsumption = Math.Round(heatLosses.TotalHeatConsumption +
                                                 House.Hel.Energy / 48 * Weathers.Count, 2);
        }

        public void SetHeatTypes(HeatLosses heatLosses)
        {
            if(heatLosses.Heats != null) return;
            heatLosses.Heats = new List<TypeOfHeat>
            {
                new TypeOfHeat
                {
                    Name = "централізоване мережа",
                    CostPerFuelUnit = 1.11,
                    FuelPerKWht = 1,
                    Efficience = 1,
                    Unit = "кВт"
                },
                new TypeOfHeat
                {
                    Name = "природний газ",
                    CostPerFuelUnit = 6.95,
                    FuelPerKWht = 0.1075,
                    Efficience = 0.93,
                    Unit = "м³"
                },
                new TypeOfHeat
                {
                    Name = "вугілля кам'яне",
                    CostPerFuelUnit = 7,
                    FuelPerKWht = 0.1792,
                    Efficience = 0.86,
                    Unit = "кг"
                },
                new TypeOfHeat
                {
                    Name = "паливні брикети",
                    CostPerFuelUnit = 2.8,
                    FuelPerKWht = 0.1953,
                    Efficience = 0.92,
                    Unit = "кг"
                },
                new TypeOfHeat
                {
                    Name = "дрова дубові",
                    CostPerFuelUnit = 1.2,
                    FuelPerKWht = 0.287,
                    Efficience = 0.83,
                    Unit = "кг"
                },
                new TypeOfHeat
                {
                    Name = "електрична енергія",
                    CostPerFuelUnit = 1.68,
                    FuelPerKWht = 1.01,
                    Efficience = 0.98,
                    Unit = "кВтˑгод"
                }
            };
        }

        public void GetHeatPrices(HeatLosses heatLosses)
        {
            var heats = heatLosses.Heats;
            foreach (var heat in heats)
            {
                heat.FuelConsumption = Math.Round(heatLosses.TotalHeatHelConsumption * heat.FuelPerKWht /
                                       heat.Efficience, 2);
                heat.TotalPrice = Math.Round(heat.FuelConsumption * heat.CostPerFuelUnit, 2);
            }
        }

        public void CalculatePrice(HeatLosses heatLosses)
        {
            SetHeatTypes(heatLosses);
            GetHeatPrices(heatLosses);
        }
        
    }
}
