using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model;
using SmartHouse.Model;

namespace SmartHouse.BLL.Controller
{
    public class WeatherController
    {
        private List<string> windDirections = new List<string>()
        {
            "Північний", "Пн-Сх", "Східний", "Пд-Сх", "Південий", "Пд-З", "Західний", "Пн-З", "Змінний","Штиль"
        };

        private List<string> _files;
        private List<List<Weather>> _weathersListDb;
        public List<Weather> Weathers { set; get; }
        public List<Weather> SelectedWeathers { set; get; }
        public int CorrrectsCounter { set; get; }
        public List<DurationTemperature> DurationTemperatures { set; get; }
        public List<WindActivity> WindActivities { set; get; }
        public List<WindRose> WindRoses { set; get; }
        public List<string> CategoriesInfo { set; get; }

        // Gets selected period.
        public DateTime StartDateTime
        {
            get { return Weathers.Min(x => x.Date); }
        }

        public DateTime EndDateTime
        {
            get { return Weathers.Max(x => x.Date); }
        }

        public DateTime StartSelectedDateTime
        {
            get { return SelectedWeathers.Min(x => x.Date); }
        }

        public DateTime EndSelectedDateTime
        {
            get { return SelectedWeathers.Max(x => x.Date); }
        }

        public int Year
        {
            get { return Weathers.Min(x => x.Date.Year); }
        }

        public WeatherController(List<string> files)
        {
            _files = files;
            _weathersListDb = BinaryController.ReadAllWeatherFromBinary(files);
            CorrectWeather();
            Weathers = _weathersListDb.SelectMany(x => x).ToList();
        }

        public List<Weather> GetWeathers(int month, int day)
        {
            // Select all monthes
            if (month == 12) SelectedWeathers = Weathers.ToList();
            else // Select all days
            {
                if (day == -1) SelectedWeathers = Weathers.Where(d => d.Date.Month == month + 1).ToList();
                else
                    SelectedWeathers = Weathers.Where(d => (d.Date.Month == month + 1) && (d.Date.Day == day + 1)).ToList();
            }
            return SelectedWeathers;
        }

        public List<Weather> GetWeathers(DateTime start, DateTime end)
        {
            SelectedWeathers = Weathers.Where(x => (x.Date >= start) && (x.Date <= end)).ToList();
            return SelectedWeathers;
        }

        public void CorrectWeather()
        {
            CorrrectsCounter = 0;
            for (int month = 0; month < _weathersListDb.Count; month++)
            {
                DateTime date = _weathersListDb[month][0].Date;
                for (int t = 0; t < _weathersListDb[month].Count - 1; t++)
                {
                    CorrectWindDirection(month, t);
                    CorrectWindSpeed(month, t);
                    date = date.AddMinutes(30);
                    while (_weathersListDb[month][t + 1].Date.Equals(date) != true)
                    {
                        Weather weather = new Weather()
                        {
                            Date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0),
                            Temperature =
                                (_weathersListDb[month][t].Temperature + _weathersListDb[month][t + 1].Temperature) / 2,
                            WindDirection = _weathersListDb[month][t].WindDirection,
                            WindPower = (_weathersListDb[month][t].WindPower + _weathersListDb[month][t + 1].WindPower) / 2
                        };
                        _weathersListDb[month].Insert(t + 1, weather);
                        t++;
                        CorrrectsCounter++;
                        if (_weathersListDb[month][t + 1].Date.Equals(date)) break;
                        date = date.AddMinutes(30);
                    }
                }
            }
            //Weathers = _weathersListDb.SelectMany(x => x).ToList();
            //BinaryController.WriteAllWeatherToBinary(_files, _weathersListDb);
        }

        private void CorrectWindDirection(int month, int t)
        {
            Weather weather = _weathersListDb[month][t];
            if (weather.WindPower == 0) weather.WindDirection = "Штиль";
            if (weather.WindDirection == "Северный") weather.WindDirection = "Північний";
            if (weather.WindDirection == "Южный") weather.WindDirection = "Південий";
            if (weather.WindDirection == "Западный") weather.WindDirection = "Західний";
            if (weather.WindDirection == "Восточный") weather.WindDirection = "Східний";
            if (weather.WindDirection == "С-З") weather.WindDirection = "Пн-З";
            if (weather.WindDirection == "С-В") weather.WindDirection = "Пн-Сх";
            if (weather.WindDirection == "Ю-З") weather.WindDirection = "Пд-З";
            if (weather.WindDirection == "Ю-В") weather.WindDirection = "Пд-Сх";
            if (weather.WindDirection == "Переменный") weather.WindDirection = "Змінний";
        }

        private void CorrectWindSpeed(int month, int t)
        {
            Weather weather = _weathersListDb[month][t];
            if (weather.WindPower < 0) weather.WindPower = Math.Abs(weather.WindPower);
        }
        
        public void TemperatureCondition(List<Weather> weathers, out List<DateTime> dates, out List<int> temperatures)
        {
            dates = weathers.Select(w => w.Date).ToList();
            temperatures = weathers.Select(w => w.Temperature).ToList();
        }

        public void DurationOfTemperatureModes(List<Weather> weathers, out List<int> temperatures, out List<double> temperatureDurations)
        {
            temperatures = new List<int>();
            temperatureDurations = new List<double>();
            DurationTemperatures = new List<DurationTemperature>();
            foreach (var weather in weathers)
            {
                int i = temperatures.IndexOf(weather.Temperature);
                if (i != -1)
                {
                    temperatureDurations[i] += 0.5;
                }
                else
                {
                    temperatures.Add(weather.Temperature);
                    temperatureDurations.Add(0.5);
                }
            }

            for (int i = 0; i < temperatures.Count; i++)
            {
                DurationTemperatures.Add(new DurationTemperature()
                {
                    Duration = temperatureDurations[i],
                    Temperature = temperatures[i]
                });
            }
            DurationTemperatures.Sort((x,y)=> x.Temperature.CompareTo(y.Temperature));
        }

        public void WindRose(List<Weather> weathers, out List<int> directions, out List<List<double>> frequencyPercents)
        {
            WindRoses = new List<WindRose>();
            int angle = 0;
            foreach (var wind in windDirections)
            {
                WindRoses.Add(new WindRose()
                {
                    Direction = wind,
                    DirectionAngle = angle
                });
                angle += 45;
            }
            foreach (var weather in weathers)
            {
                int i = WindRoses.FindIndex(x => x.Direction == weather.WindDirection);
                if (i != -1) WindRoses[i].Frequency += 0.5;
            }
            double sum = WindRoses.Sum(x => x.Frequency);
            int maxSpeed = weathers.Max(x => x.WindPower);
            GetCategoriesInfo(maxSpeed);
            for (int i = 0; i < WindRoses.Count; i++)
            {
                WindRoses[i].FrequencyPercent = Math.Round(WindRoses[i].Frequency / sum * 100, 2);
                foreach (var weather in weathers)
                {
                    if (weather.WindDirection == WindRoses[i].Direction)
                    {
                        double speed = (double) weather.WindPower / maxSpeed;
                        if (speed > 0 && speed <= 0.25) WindRoses[i].FrequencyPercentCategories[0] += 0.5;
                        if (speed > 0.25 && speed <= 0.5) WindRoses[i].FrequencyPercentCategories[1] += 0.5;
                        if (speed > 0.5 && speed <= 0.75) WindRoses[i].FrequencyPercentCategories[2] += 0.5;
                        if (speed > 0.75 && speed <= 1) WindRoses[i].FrequencyPercentCategories[3] += 0.5;
                    }
                }
                for (int j = 0; j < WindRoses[i].FrequencyPercentCategories.Count; j++)
                {
                    WindRoses[i].FrequencyPercentCategories[j] = Math.Round(WindRoses[i].FrequencyPercentCategories[j] / sum * 100, 2);
                }
            }
            directions = WindRoses.Where(x =>
                    x.Direction != windDirections[windDirections.Count - 1] &&
                    x.Direction != windDirections[windDirections.Count - 2])
                .Select(x => x.DirectionAngle).ToList();
            directions.Add(directions[0]);
            frequencyPercents = WindRoses.Where(x =>
                    x.Direction != windDirections[windDirections.Count - 1] &&
                    x.Direction != windDirections[windDirections.Count - 2])
                .Select(x => x.FrequencyPercentCategories).ToList();
            frequencyPercents.Add(frequencyPercents[0]);
            List<List<double>> tempList = new List<List<double>>();
            for (int i = 0; i < frequencyPercents[0].Count; i++)
            {
                tempList.Add(new List<double>());
                for (int j = 0; j < frequencyPercents.Count; j++)
                {
                    tempList[i].Add(frequencyPercents[j][i]);
                }
            }
            frequencyPercents = tempList;
        }

        private void GetCategoriesInfo(int maxSpeed)
        {
            CategoriesInfo = new List<string>();
            for (double i = 0; i < 1; i+= 0.25)
            {
                CategoriesInfo.Add($"швидкість вітру {i*maxSpeed}-{(i+0.25)*maxSpeed} м/с");
            }
        }

        public void DurationOfWindActivityModes(List<Weather> weathers, out List<int> speeds, out List<double> speedDurations)
        {
            WindActivities = new List<WindActivity>();
            foreach (var weather in weathers)
            {
                int i = WindActivities.FindIndex(x => x.Speed == weather.WindPower);
                if (i != -1)
                {
                    WindActivities[i].Duration += 0.5;
                }
                else
                {
                    WindActivities.Add(new WindActivity
                    {
                        Speed = weather.WindPower,
                        Duration = 0.5
                    });
                }
            }
            WindActivities.Sort((x, y)=> x.Speed.CompareTo(y.Speed));
            speeds = WindActivities.Select(x => x.Speed).ToList();
            speedDurations = WindActivities.Select(x => x.Duration).ToList();
        }
    }
}
