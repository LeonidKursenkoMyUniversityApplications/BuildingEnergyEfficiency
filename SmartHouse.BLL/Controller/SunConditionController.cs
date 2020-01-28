using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model;

namespace SmartHouse.BLL.Controller
{
    public class SunConditionController
    {
        private string _file;

        public List<Sun> SunConditions { set; get; }
        public List<Sun> SelectedSunConditions { set; get; }
        public List<SunActivity> SunActivities { set; get; }

        public SunConditionController(List<Sun> sunConditions, int year)
        {
            SunConditions = sunConditions;
            CorrectData(year);
        }

        public SunConditionController(string file)
        {
            _file = file;
            SunConditions = BinaryController.ReadDataFromBinary<Sun>(file);
        }

        public SunConditionController(string file, int year)
        {
            _file = file;
            SunConditions = BinaryController.ReadDataFromBinary<Sun>(file);
            CorrectData(year);
        }

        public List<Sun> GetSunConditions(int month, int day)
        {
            // Select all monthes
            if (month == 12) SelectedSunConditions = SunConditions;
            else
            {
                // Select all days
                if (day == -1) SelectedSunConditions = SunConditions
                        .Where(x => x.Date.Month == month + 1).ToList();
                else SelectedSunConditions = SunConditions
                        .Where(d => (d.Date.Month == month + 1) && (d.Date.Day == day + 1))
                        .ToList();
            }
            return SelectedSunConditions;
        }

        public List<Sun> GetSunConditions(DateTime start, DateTime end)
        {
            SelectedSunConditions = SunConditions.Where(x => (x.Date >= start) && (x.Date <= end)).ToList();
            return SelectedSunConditions;
        }

        public void CorrectData(int year)
        {
            var suns = new List<Sun>();
            DateTime date0 = new DateTime(year, 1, 1, 0, 0, 0);
            DateTime dateEnd = date0.AddYears(1);
            for (var date = date0; date < dateEnd; date = date.AddHours(1))
            {
                int index = SunConditions.FindIndex(x => 
                    x.Date.Month == date.Month && 
                    x.Date.Day == date.Day &&
                    x.Date.Hour == date.Hour &&
                    x.Date.Minute == date.Minute);
                if (index == -1)
                {
                    DateTime prev = date.AddHours(-1);
                    int prevIndex = SunConditions.FindIndex(x =>
                        x.Date.Month == prev.Month &&
                        x.Date.Day == prev.Day &&
                        x.Date.Hour == prev.Hour &&
                        x.Date.Minute == prev.Minute);
                    //if(prevIndex != -1)
                        suns.Add(SunConditions[prevIndex]);
                }
                else
                {
                    suns.Add(SunConditions[index]);
                }
                suns[suns.Count - 1].Date = date.AddMilliseconds(0);
            }
            SunConditions = suns;
            SunConditions = SunConditions.Where(x => x.Insolation > 0).ToList();
            //BinaryController.WriteDataToBinary(_file, SunConditions);
        }

        public void IntesityOfSolarInsolation(List<Sun> suns, out List<DateTime> dates, out List<int> insolations)
        {
            dates = suns.Select(s => s.Date).ToList();
            insolations = suns.Select(s => s.Insolation).ToList();
        }

        public void DurationOfSolarActivityModes(List<Sun> sunConditions, out List<int> insolations, out List<double> durations)
        {
            SunActivities = new List<SunActivity>();
            foreach (var sun in sunConditions)
            {
                int i = SunActivities.FindIndex(x => x.Insolation == sun.Insolation);
                if (i != -1)
                {
                    SunActivities[i].Duration += 1;
                }
                else
                {
                    SunActivities.Add(new SunActivity()
                    {
                        Insolation = sun.Insolation,
                        Duration = 1
                    });
                }
            }
            SunActivities.Sort((x, y) => x.Insolation.CompareTo(y.Insolation));
            insolations = SunActivities.Select(x => x.Insolation).ToList();
            durations = SunActivities.Select(x => x.Duration).ToList();
        }
    }
}
