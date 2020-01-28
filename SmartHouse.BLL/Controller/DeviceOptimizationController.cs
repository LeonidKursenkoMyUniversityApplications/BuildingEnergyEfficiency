using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model;

namespace SmartHouse.BLL.Controller
{
    public enum OptimizationType { TwoZone, ThreeZone}
    public class DeviceOptimizationController
    {
        public static void Save(string fileName, List<DeviceOptimization> list)
        {
            BinaryController.WriteDataToBinary(fileName, list);
        }

        public static List<DeviceOptimization> Read(string fileName, List<Device> devices)
        {
            List<DeviceOptimization> list = BinaryController.ReadDataFromBinary<DeviceOptimization>(fileName);

            List<DeviceOptimization> devicesList = new List<DeviceOptimization>();
            foreach (var dev in devices)
            {
                bool isAvailable = list.Exists(x => x.Device.Name.Equals(dev.Name) && x.IsAvailable);
                devicesList.Add(new DeviceOptimization(dev, isAvailable));
            }
            return devicesList;
        }

        public static void Optimized(List<DeviceOptimization> devices, OptimizationType type)
        {
            List<Period> zones = DefineZone(type);
            foreach (var device in devices.Where(x =>x.IsAvailable))
            {
                foreach (var day in device.Device.DayOfWeek)
                {
                    List<Period> newPeriods = new List<Period>();
                    foreach (var period in day.Periods)
                    {
                        OptimizeOnePeriod(period, newPeriods, zones);
                    }
                    day.Periods = newPeriods;
                }
            }
        }

        private static void OptimizeOnePeriod(Period period, List<Period> newPeriods, List<Period> zones)
        {
            if (period.Duration().Hours > 23)
            {
                newPeriods.Add(period);
                return;
            }
            Period zone = zones.First(x =>
                x.Distance(period).Equals(zones.Min(g => g.Distance(period))));
            //zone = zones[0];
            if (period.Duration() < zone.Duration())
                period.MoveTo(zone.Start);
            else
            {
                period.MoveTo(zone.Start.Add(zone.Duration() - period.Duration()));
            }
            int offset = 5;
            while (newPeriods.Exists(x => x.IsIntersect(period)))
            {
                period.MoveTo(period.Start.AddMinutes(offset));
                if (period.Start.Day != period.End.Day)
                {
                    if (offset < 0) break;
                    offset = -5;
                }
            }
            newPeriods.Add(period);
            if (Period.ToTimeSpan(zone.Start) > Period.ToTimeSpan(period.End))
                period.MoveTo(zone.Start);
            //else if (Period.ToTimeSpan(zone.End) < Period.ToTimeSpan(period.Start))
            //    period.MoveTo(zone.Start);
        }

        private static List<Period> DefineZone(OptimizationType type)
        {
            List<Period> zones = null;
            switch (type)
            {
                case OptimizationType.TwoZone:
                    zones = new List<Period>()
                    {
                        Period.SetZone(new TimeSpan(0, 0, 0, 0), new TimeSpan(0, 7, 0, 0)),
                        Period.SetZone(new TimeSpan(0, 23, 0, 0), new TimeSpan(0, 23, 59, 59))
                    };
                    break;
                case OptimizationType.ThreeZone:
                    zones = new List<Period>()
                    {
                        Period.SetZone(new TimeSpan(0, 0, 0, 0), new TimeSpan(0, 7, 0, 0)),
                        Period.SetZone(new TimeSpan(0, 23, 0, 0), new TimeSpan(0, 23, 59, 59))
                    };
                    break;
            }
            return zones;
        }
    }
}
