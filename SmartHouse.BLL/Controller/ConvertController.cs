using SmartHouse.DAL.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.Controller;
using SmartHouse.DAL.Model;

namespace SmartHouse.BLL.Controller
{
    public class ConvertController
    {
        public void ConvertToBinary(string file)
        {
            for (int i = 1; i <= 12; i++)
            {
                GetWeatherExcel(file + i);
                //BinaryController.ReadDataFromBinary(file + i);
            }
        }

        public void GetWeatherExcel(string file)
        {
            ExcelController excelController = new ExcelController();
            List<Weather> weathers = excelController.GetWeathers(file + ".xlsx");
            BinaryController.WriteDataToBinary(file, weathers);
        }

        public List<Sun> GetSunConditionsFromExcel(string file)
        {
            ExcelController excelController = new ExcelController();
            List<Sun> sunConditions = excelController.GetSunConditions(file + ".xlsx");
            //BinaryController.WriteDataToBinary(file, sunConditions);
            return sunConditions;
        }

        public static List<List<ElectricalLoad>> ToElectricalLoad(Device device)
        {
            List<List<ElectricalLoad>> weekLoads = new List<List<ElectricalLoad>>();
            List<ElectricalLoad> dayLoads;
            foreach (var dayOfWeek in device.DayOfWeek)
            {
                dayLoads = new List<ElectricalLoad>();
                foreach (var period in dayOfWeek.Periods)
                {
                    ElectricalLoad load = new ElectricalLoad(device.Power, period.Start, period.End);
                    dayLoads.Add(load);
                }
                weekLoads.Add(dayLoads);
            }
            return weekLoads;
        }

        public static List<List<List<ElectricalLoad>>> ToElectricalLoadList(List<Device> devices)
        {
            List<List<List<ElectricalLoad>>> electricalLoadList = new List<List<List<ElectricalLoad>>>();
            foreach (var device in devices)
            {
                electricalLoadList.Add(ToElectricalLoad(device));
            }
            return electricalLoadList;
        }

        
    }
}
