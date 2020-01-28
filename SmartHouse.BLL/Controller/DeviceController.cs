using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model;

namespace SmartHouse.BLL.Controller
{
    public class DeviceController
    {
        private string _fileName;
        private static double eps = 0.00001;
        public List<Device> Devices { set; get; }
        // x => Device, y => DayOfWeek, z => Period.
        public List<List<List<ElectricalLoad>>> ElectricalLoadsForDevices { set; get; }
        public List<List<List<DurationElectricalLoad>>> DurationElectricalLoadsForDevices { set; get; }
        public List<List<double>> ElectricalConsumptions { set; get; }

        public List<RatesConsumption> RatesConsumptions { set; get; }
        public List<ElectricalConsumptionRow> ElectricalConsumptionTable { set; get; }

        public ElectricalPrices ElectricalPrices { set; get; }
        public List<PriceRow> Prices1Phase { set; get; }
        public List<PriceRow> Prices2Phase { set; get; }
        public List<PriceRow> Prices3Phase { set; get; }

        public List<DeviceOptimization> DevicesForOptimization { set; get; }

        public string FileName
        {
            get => _fileName;
        }

        public DeviceController()
        {
            
        }

        public DeviceController(string fileName)
        {
            _fileName = fileName;
            Devices = BinaryController.ReadDataFromBinary<Device>(fileName);
            if(Devices == null || Devices.Count == 0) Devices = new List<Device>();
            ElectricalPrices = new ElectricalPrices();
            CalculateData();
        }

        private void CalculateData()
        {
            GetElectricalLoadsForDevices();
            DurationElectricalLoadsForDevices = GetDurationElectricalLoadsForDevices(ElectricalLoadsForDevices);
            ElectricalConsumptions = GetElectricalConsumptions(DurationElectricalLoadsForDevices);
            SetElectricalConsumptionsTable();
            RatesConsumptions = GetRatesConsumptions(ElectricalConsumptions, DurationElectricalLoadsForDevices);

            CalculatePricesForZones();
        }

        public void Save(string fileName)
        {
            BinaryController.WriteDataToBinary(fileName, Devices);
        }

        public void Update()
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                for (int j = 0; j < Devices[i].DayOfWeek.Count; j++)
                {
                    for (int k = 0; k < Devices[i].DayOfWeek[j].Periods.Count; k++)
                    {
                        var d = Devices[i].DayOfWeek[j].Periods[k].Start;
                        d = new DateTime(d.Year, d.Month, j + 1, d.Hour, d.Minute, d.Second);
                        Devices[i].DayOfWeek[j].Periods[k].Start = d;

                        d = Devices[i].DayOfWeek[j].Periods[k].End;
                        d = new DateTime(d.Year, d.Month, j + 1, d.Hour, d.Minute, d.Second);
                        Devices[i].DayOfWeek[j].Periods[k].End = d;
                    }
                     
                }
            }
            CalculateData();
        }
        
        public void GetElectricalLoadsForDevices()
        {
            ElectricalLoadsForDevices = ConvertController.ToElectricalLoadList(Devices);
            AddTotalSumLoad();
            AddWeeksSumLoad();
            //CorrectTotalSumLoad();
        }

        // Тижневе навантаження для кожного пристрою.
        private void AddWeeksSumLoad()
        {
            for (int i = 0; i < ElectricalLoadsForDevices.Count; i++)
            {
                ElectricalLoadsForDevices[i].Add(ElectricalLoadsForDevices[i].SelectMany(x => x).ToList());
            }
        }

        // Сумарне навантаження всіх пристрої за кожний день та за весь тиждень.
        private void AddTotalSumLoad()
        {
            //List<ElectricalLoad> dayTotalLoads;
            List<List<ElectricalLoad>> weekTotalLoads = new List<List<ElectricalLoad>>();
            for (int i = 0; i < ElectricalLoadsForDevices[0].Count; i++)
            {
                weekTotalLoads.Add(AddTotalSumLoad(i));
            }
            ElectricalLoadsForDevices.Add(weekTotalLoads);
        }

        // Сумарне навантаження всіх пристрої за день.
        private List<ElectricalLoad> AddTotalSumLoad(int day)
        {
            // x => Device, y => Period
            List<List<ElectricalLoad>> electricalLoads = new List<List<ElectricalLoad>>();
            foreach (var device in ElectricalLoadsForDevices)
            {
                electricalLoads.Add(device[day]);
            }
            List<ElectricalLoad> eLoads = electricalLoads.SelectMany(x => x).ToList();
            List<DateTime> timeIntervals = eLoads.Select(x => x.Start).ToList();
            timeIntervals.AddRange(eLoads.Select(x => x.End).ToList());
            timeIntervals = timeIntervals.Distinct().ToList();
            timeIntervals.Sort((a, b) => a.CompareTo(b));

            List<ElectricalLoad> totalLoads = new List<ElectricalLoad>();
            double totalPower = 0;
            for (int i = 0; i < timeIntervals.Count - 1; i++)
            {
                List<ElectricalLoad> foundedLoads = 
                    eLoads.Where(x => (x.Start >= timeIntervals[i] && x.Start < timeIntervals[i + 1]) ||
                                      (x.Start < timeIntervals[i] && x.End >= timeIntervals[i + 1])).ToList();
                totalPower = foundedLoads.Sum(x => x.Power);
                ElectricalLoad totalLoad = new ElectricalLoad(totalPower, timeIntervals[i], timeIntervals[i + 1]);
                totalLoads.Add(totalLoad);
            }
            for (int i = 0; i < totalLoads.Count - 1; i++)
            {
                if (Math.Abs(totalLoads[i].Power - totalLoads[i + 1].Power) < eps)
                {
                    totalLoads[i].End = totalLoads[i + 1].End;
                    totalLoads.RemoveAt(i + 1);
                }
            }
            return totalLoads;
        }

        private void CorrectTotalSumLoad()
        {
            var loads = ElectricalLoadsForDevices[ElectricalLoadsForDevices.Count - 1];
            var totalLoads = loads[loads.Count - 1];
            for (int i = 0; i < totalLoads.Count - 1; i++)
            {
                if (Math.Abs(totalLoads[i].Power - totalLoads[i + 1].Power) < eps)
                {
                    totalLoads[i].End = totalLoads[i + 1].End;
                    totalLoads.RemoveAt(i + 1);
                }
            }
        }

        private List<List<List<DurationElectricalLoad>>> GetDurationElectricalLoadsForDevices(
            List<List<List<ElectricalLoad>>> deviceElLoads)
        {
            List<List<List<DurationElectricalLoad>>> durationElLoads = new List<List<List<DurationElectricalLoad>>>();
            foreach (var weekElLoads in deviceElLoads)
            {
                List<List<DurationElectricalLoad>> weekDurations = new List<List<DurationElectricalLoad>>();
                foreach (var dayElLoads in weekElLoads)
                {
                    List<DurationElectricalLoad> dayDurations = new List<DurationElectricalLoad>();
                    List<double> loads = dayElLoads.Select(x => x.Power).Distinct().ToList();
                    loads.Sort();
                    foreach (var load  in loads)
                    {
                        List<ElectricalLoad> currentLoads = dayElLoads.FindAll(
                            x => Math.Abs(x.Power - load) < eps).ToList();
                        TimeSpan duration = new TimeSpan(0, 0, 0, 0);
                        foreach (var currentLoad in currentLoads)
                        {
                            var t = currentLoad.End - currentLoad.Start;
                            duration += new TimeSpan(t.Days, t.Hours, t.Minutes, t.Seconds);
                        }
                        dayDurations.Add(new DurationElectricalLoad
                        {
                            Power = load,
                            Duration =  duration.TotalHours
                        });
                    }
                    weekDurations.Add(dayDurations);
                }
                durationElLoads.Add(weekDurations);
            }
            return durationElLoads;
        }

        private List<List<double>> GetElectricalConsumptions(List<List<List<DurationElectricalLoad>>> deviceDurElLoads)
        {
            List<List<double>> elConsumptions = new List<List<double>>();
            foreach (var weekDurations in deviceDurElLoads)
            {
                List<double> weekConsumptions = new List<double>();
                foreach (var dayDurations in weekDurations)
                {
                    weekConsumptions.Add(dayDurations.Sum(x => x.Power * x.Duration));
                }
                elConsumptions.Add(weekConsumptions);
            }
            var totalSum = elConsumptions.Last().ToList();
            totalSum.Remove(totalSum.Last());
            double total = totalSum.Sum();
            elConsumptions[elConsumptions.Count - 1][elConsumptions[elConsumptions.Count - 1].Count - 1] = total;

            return elConsumptions;
        }

        public void SetElectricalConsumptionsTable()
        {
            var devices = Devices.Select(x => x.Name).ToList();
            devices.Add("Всі");
            List<ElectricalConsumptionRow> rows = new List<ElectricalConsumptionRow>();
            for (int i = 0; i < devices.Count; i++)
            {
                rows.Add(new ElectricalConsumptionRow
                {
                    DeviceName = devices[i],
                    WeekConsumptions = ElectricalConsumptions[i].ToList()
                });
            }
            ElectricalConsumptionTable = rows;
        }

        private List<RatesConsumption> GetRatesConsumptions(List<List<double>> electricalConsumptions,
            List<List<List<DurationElectricalLoad>>> deviceDurElLoads)
        {
            List<RatesConsumption> ratesConsumptions = new List<RatesConsumption>();
            var consumes = electricalConsumptions[electricalConsumptions.Count - 1];
            List<string> dayOfWeek = Constants.DayOfWeek.ToList();
            dayOfWeek.Add("Тиждень");
            List<List<DurationElectricalLoad>> durationLoads = deviceDurElLoads[deviceDurElLoads.Count - 1].ToList();

            for (int i = 0; i < consumes.Count; i++)
            {
                double powerMax = durationLoads[i].Select(x => x.Power).Max();
                double powerAverage = durationLoads[i].Sum(x => x.Power * x.Duration) /
                                     durationLoads[i].Sum(x => x.Duration);
                ratesConsumptions.Add(new RatesConsumption
                {
                    Name = dayOfWeek[i],
                    Consumption = consumes[i],
                    PowerMax = powerMax,
                    PowerAverage = powerAverage,
                    DurationOfMaxPower = durationLoads[i].Select(x => x.Duration).Max(),
                    DegreeOfUnevenness = powerAverage / powerMax,
                    RateOfUsePower = powerAverage / durationLoads[i].Sum(x => x.Power)
                });
            }
            var temp = ratesConsumptions.ToList();
            var weekConsum = temp[temp.Count - 1]; 
            temp.RemoveAt(temp.Count - 1);
            weekConsum.DurationOfMaxPower = temp.First(x => x.PowerMax >= weekConsum.PowerMax)
                .DurationOfMaxPower;
            return ratesConsumptions;
        }

        private ElectricalPrices GetElectricalPrices(List<ElectricalLoad> electricalLoads, 
            List<DurationElectricalLoad> durationElectricalLoads, ElectricalPrices ePrices)
        {
            GetElectricalPricesFor1Phase(durationElectricalLoads, ePrices);
            Set1PhaseTable();
            GetElectricalPricesFor2Phase(electricalLoads, durationElectricalLoads, ePrices);
            Set2PhaseTable();
            GetElectricalPricesFor3Phase(electricalLoads, durationElectricalLoads, ePrices);
            Set3PhaseTable();
            return ePrices;
        }

        private void GetElectricalPricesFor1Phase(List<DurationElectricalLoad> durElLoads, ElectricalPrices ePrices)
        {
            double consumption = ElectricalConsumptions.Last().Last();//durElLoads.Sum(x => x.Power * x.Duration);
            if (consumption <= 100)
            {
                ePrices.CostFor1PhaseLess100 = Math.Round(ePrices.OneKwhtPriceFor1PhaseLess100 * consumption, 2);
                ePrices.CostFor1PhaseMore100 = 0;
                ePrices.ConsumptionFor1PhaseLess100 = consumption;
                ePrices.ConsumptionFor1PhaseMore100 = 0;
            }
            else
            {
                ePrices.CostFor1PhaseLess100 = Math.Round(ePrices.OneKwhtPriceFor1PhaseLess100 * 100, 2);
                ePrices.CostFor1PhaseMore100 = Math.Round(ePrices.OneKwhtPriceFor1PhaseMore100 * (consumption - 100), 2);
                ePrices.ConsumptionFor1PhaseLess100 = 100;
                ePrices.ConsumptionFor1PhaseMore100 = consumption - 100;
            }
            ePrices.TotalCostFor1Phase = ePrices.CostFor1PhaseLess100 + ePrices.CostFor1PhaseMore100;
            ePrices.TotalConsumptionFor1Phase =
                ePrices.ConsumptionFor1PhaseLess100 + ePrices.ConsumptionFor1PhaseMore100;
        }

        public void CalculatePricesForZones()
        {
            var elLoadsForAll = ElectricalLoadsForDevices[ElectricalLoadsForDevices.Count - 1];
            var elLoadsForWeek = elLoadsForAll[elLoadsForAll.Count - 1];

            var elDurForAll = DurationElectricalLoadsForDevices[DurationElectricalLoadsForDevices.Count - 1];
            var elDurForWeek = elDurForAll[elDurForAll.Count - 1];

            ElectricalPrices = GetElectricalPrices(elLoadsForWeek, elDurForWeek, ElectricalPrices);
        }

        private void GetElectricalPricesFor2Phase(List<ElectricalLoad> elLoads,
            List<DurationElectricalLoad> durElLoads, ElectricalPrices ePrices)
        {
            double dayConsumption = GetConsumptionForPeriod(7, 23, elLoads);
            double nightConsumtion = ElectricalConsumptions.Last().Last() - dayConsumption;
            ePrices.DayCost2Phase = Math.Round(dayConsumption * ePrices.DayFactorFor2Phase * 
                ePrices.OneKwhtPriceFor2Phase, 2);

            ePrices.NightCost2Phase = Math.Round(nightConsumtion * ePrices.NightFactorFor2Phase *
                ePrices.OneKwhtPriceFor2Phase, 2);
            
            ePrices.TotalCostFor2Phase = ePrices.DayCost2Phase + ePrices.NightCost2Phase;

            ePrices.NightConsumptionFor2Phase = nightConsumtion;
            ePrices.DayConsumptionFor2Phase = dayConsumption;
            ePrices.TotalConsumptionFor2Phase = nightConsumtion + dayConsumption;
        }

        private double GetConsumptionForPeriod(int startHour, int endHour, List<ElectricalLoad> elLoads)
        {
            List<double> consumptions = new List<double>();
            for (int i = 0; i < elLoads.Count; i++)
            {
                var startDate = elLoads[i].Start;
                var endDate = elLoads[i].End;
                if (startDate.Hour >= endHour || endDate.Hour <= startHour) continue;
                TimeSpan startTime = new TimeSpan(startDate.Day, startHour, 0, 0);
                TimeSpan endTime = new TimeSpan(endDate.Day, endHour, 0, 0);
                if (startDate.Hour >= startHour && startDate.Hour < endHour)
                {
                    startTime = new TimeSpan(startDate.Day, startDate.Hour, startDate.Minute, startDate.Second);
                }
                if (endDate.Hour >= startHour && endDate.Hour < endHour)
                {
                    endTime = new TimeSpan(endDate.Day, endDate.Hour, endDate.Minute, endDate.Second);
                }
                double duration = (endTime - startTime).TotalHours;
                consumptions.Add(duration * elLoads[i].Power);
            }

            return consumptions.Sum();
        }

        private void GetElectricalPricesFor3Phase(List<ElectricalLoad> elLoads,
            List<DurationElectricalLoad> durElLoads, ElectricalPrices ePrices)
        {
            double maxConsumption = GetConsumptionForPeriod(8, 11, elLoads);
            maxConsumption += GetConsumptionForPeriod(20, 22, elLoads);

            //double halfConsumption = GetConsumptionForPeriod(7, 8, elLoads);
            //halfConsumption += GetConsumptionForPeriod(11, 20, elLoads);
            //halfConsumption += GetConsumptionForPeriod(22, 23, elLoads);

            //double nightConsumption = durElLoads.Sum(x => x.Power * x.Duration) - maxConsumption - halfConsumption;
            double nightConsumption = ElectricalConsumptions.Last().Last() - GetConsumptionForPeriod(7, 23, elLoads);
            double halfConsumption = ElectricalConsumptions.Last().Last() - nightConsumption - maxConsumption;

            ePrices.NightCostFor3Phase = Math.Round(nightConsumption * ePrices.NightFactorFor3Phase *
                ePrices.OneKwhtPriceFor3Phase, 2);
            ePrices.MaxLoadCostFor3Phase = Math.Round(maxConsumption * ePrices.MaxLoadFactorFor3Phase *
                ePrices.OneKwhtPriceFor3Phase, 2);
            ePrices.HalfMaxLoadCostFor3Phase = Math.Round(halfConsumption * ePrices.HalfMaxLoadFactorFor3Phase *
                ePrices.OneKwhtPriceFor3Phase, 2);
            
            ePrices.TotalCostFor3Phase = ePrices.NightCostFor3Phase +
                                          ePrices.MaxLoadCostFor3Phase + ePrices.HalfMaxLoadCostFor3Phase;

            ePrices.NightConsumptionFor3Phase = nightConsumption;
            ePrices.MaxLoadConsumptionFor3Phase = maxConsumption;
            ePrices.HalfMaxLoadConsumptionFor3Phase = halfConsumption;
            ePrices.TotalConsumptionFor3Phase = nightConsumption + maxConsumption + halfConsumption;
        }

        private void Set1PhaseTable()
        {
            var prices = ElectricalPrices;
            List<PriceRow> rows = new List<PriceRow>();
            rows.Add(new PriceRow
            {
                Name = "За обсяг, спожитий до 100 кВт∙год електроенергії",
                Price = prices.CostFor1PhaseLess100,
                Consumption = prices.ConsumptionFor1PhaseLess100
            });

            rows.Add(new PriceRow
            {
                Name = "За обсяг, спожитий понад 100 кВт∙год електроенергії",
                Price = prices.CostFor1PhaseMore100,
                Consumption = prices.ConsumptionFor1PhaseMore100
            });

            rows.Add(new PriceRow
            {
                Name = "Загальна сума, грн",
                Price = prices.TotalCostFor1Phase,
                Consumption = prices.TotalConsumptionFor1Phase
            });

            Prices1Phase = rows;
        }

        private void Set2PhaseTable()
        {
            var prices = ElectricalPrices;
            List<PriceRow> rows = new List<PriceRow>();
            rows.Add(new PriceRow
            {
                Name = "За ніч",
                Price = prices.NightCost2Phase,
                Consumption = prices.NightConsumptionFor2Phase
            });

            rows.Add(new PriceRow
            {
                Name = "За день",
                Price = prices.DayCost2Phase,
                Consumption = prices.DayConsumptionFor2Phase
            });

            rows.Add(new PriceRow
            {
                Name = "Загальна сума",
                Price = prices.TotalCostFor2Phase,
                Consumption = prices.TotalConsumptionFor2Phase
            });
            Prices2Phase = rows;
        }

        private void Set3PhaseTable()
        {
            var prices = ElectricalPrices;
            List<PriceRow> rows = new List<PriceRow>();
            rows.Add(new PriceRow
            {
                Name = "За ніч",
                Price = prices.NightCostFor3Phase,
                Consumption = prices.NightConsumptionFor3Phase
            });

            rows.Add(new PriceRow
            {
                Name = "За піковий період",
                Price = prices.MaxLoadCostFor3Phase,
                Consumption = prices.MaxLoadConsumptionFor3Phase
            });

            rows.Add(new PriceRow
            {
                Name = "За напівпіковий період",
                Price = prices.HalfMaxLoadCostFor3Phase,
                Consumption = prices.HalfMaxLoadConsumptionFor3Phase
            });

            rows.Add(new PriceRow
            {
                Name = "Загальна сума",
                Price = prices.TotalCostFor3Phase,
                Consumption = prices.TotalConsumptionFor3Phase
            });
            Prices3Phase = rows;
        }

        public List<DeviceOptimization> GetDeviceOptimizations()
        {
            DevicesForOptimization = Devices.Select(x => new DeviceOptimization(x, false)).ToList();
            return DevicesForOptimization;
        }

        public DeviceController Copy()
        {
            return new DeviceController(_fileName);
        }

        public void StartOptimization(OptimizationType type)
        {
            DeviceOptimizationController.Optimized(DevicesForOptimization, type);
            Devices = DevicesForOptimization.Select(x => x.Device).ToList();
            CalculateData();
        }
    }
}
