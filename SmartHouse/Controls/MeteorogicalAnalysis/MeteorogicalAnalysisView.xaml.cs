using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using SmartHouse.Controller;
using SmartHouse.DAL.Model;
using Binding = System.Windows.Data.Binding;
using UserControl = System.Windows.Controls.UserControl;
using Color = System.Drawing.Color;

namespace SmartHouse.Controls.MeteorogicalAnalysis
{
    /// <summary>
    /// Логика взаимодействия для MeteorogicalAnalysisView.xaml
    /// </summary>
    public partial class MeteorogicalAnalysisView : UserControl
    {
        private readonly List<string> _monthes = Constants.Monthes;

        private int _selectedDay = 0;
        private int _selectedMonth = 0;
        private int _selectedAnalyzeType = 0;

        private WeatherController _weatherController;
        private List<Weather> _weathers;
        private List<Sun> _sunConditions;
        private SunConditionController _sunConditionController;

        public delegate void TimePeriodHandler();

        public event TimePeriodHandler TimePeriodChanged;

        public MeteorogicalAnalysisView()
        {
            InitializeComponent();
            //InitInterface();
            WeatherChart.ChartAreas.Add(new ChartArea("Default"));
            WeatherChart.Legends.Add(new Legend("Legend1"));
            WeatherChart.MouseWheel += ChartController.Сhart_MouseWheel;
            
        }
        
        public void Init(WeatherController weatherController, SunConditionController sunConditionController)
        {
            _weatherController = weatherController;
            _sunConditionController = sunConditionController;
            UpdateWeather();
            InitInterface();
        }

        private void InitInterface()
        {
            MonthListBox.ItemsSource = _monthes;
            MonthListBox.SelectionChanged -= MonthListBox_SelectionChanged;
            MonthListBox.SelectedIndex = _selectedMonth;
            MonthListBox.SelectionChanged += MonthListBox_SelectionChanged;

            DayListBoxPrepare();

            //WeatherChart.ChartAreas.Add(new ChartArea("Default"));
            //WeatherChart.Legends.Add(new Legend("Legend1"));

            AnalyzeTypeListBox.SelectedIndex = _selectedAnalyzeType;
            PrepareStartEndDateTimePickers();

            //WeatherChart.MouseWheel += ChartController.Сhart_MouseWheel;

        }
        
        private void UpdateWeather()
        {
            _weathers = _weatherController.GetWeathers(_selectedMonth, _selectedDay);
            _sunConditions = _sunConditionController.GetSunConditions(_selectedMonth, _selectedDay);
            WeatherGrid.ItemsSource = _weathers;
        }

        private void UpdateWeather(DateTime start, DateTime end)
        {
            _weathers = _weatherController.GetWeathers(start, end);
            _sunConditions = _sunConditionController.GetSunConditions(start, end);
            WeatherGrid.ItemsSource = _weathers;
        }

        private void DayListBoxPrepare()
        {
            List<string> days = new List<string>();
            if (_selectedMonth != 12)
            {
                int daysInMonth = DateTime.DaysInMonth(_weathers[0].Date.Year, _selectedMonth + 1);
                for (int i = 1; i <= daysInMonth; i++)
                    days.Add(i.ToString());
            }
            days.Add("всі");
            DayListBox.ItemsSource = days;
            DayListBox.SelectionChanged -= DayListBox_SelectionChanged;
            DayListBox.SelectedIndex = _selectedDay;
            DayListBox.SelectionChanged += DayListBox_SelectionChanged;
        }

        private void PrepareStartEndDateTimePickers()
        {
            DateTimePickerStart.ValueChanged -= DateTimePickerStart_ValueChanged;
            DateTimePickerEnd.ValueChanged -= DateTimePickerEnd_ValueChanged;

            DateTimePickerStart.Value = _weatherController.StartSelectedDateTime;
            DateTimePickerEnd.Value = _weatherController.EndSelectedDateTime;

            ConfigDateTimePicker(DateTimePickerStart);
            DateTimePickerStart.ValueChanged += DateTimePickerStart_ValueChanged;

            ConfigDateTimePicker(DateTimePickerEnd);
            DateTimePickerEnd.ValueChanged += DateTimePickerEnd_ValueChanged;
        }

        #region Analyze type
        private void AnalyzeTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AnalyzeTypeChoice();
        }

        private void AnalyzeTypeChoice()
        {
            int selected = AnalyzeTypeListBox.SelectedIndex;
            switch (selected)
            {
                case 0:
                    TemperatureCondition();
                    break;
                case 1:
                    DurationOfTemperatureModes();
                    break;
                case 2:
                    WindsRose();
                    break;
                case 3:
                    DurationOfWindActivityModes();
                    break;
                case 4:
                    IntesityOfSolarInsolation();
                    break;
                case 5:
                    DurationOfSolarActivityModes();
                    break;
            }
        }

        private void TemperatureCondition()
        {
            TemperatureConditionChart(WeatherChart);
            SetWeatherTable();
        }

        private void TemperatureConditionChart(Chart chart)
        {
            ChartController.Clear(chart);
            string temperatureSeries = "T, °C";
            ChartController.AddSeries(chart, temperatureSeries, SeriesChartType.Area, Color.CornflowerBlue);
            _weatherController.TemperatureCondition(_weathers, out var dates, out var temperatures);
            ChartController.Fill(chart, temperatureSeries, dates, temperatures);
            ChartController.AxisTitles(chart, "дата", "T, °C");
            SmartLabelStyle smartLabel = new SmartLabelStyle
            {
                AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Partial,
                IsMarkerOverlappingAllowed = true
            };
            chart.Series[0].SmartLabelStyle = smartLabel;
        }
        
        private void SetWeatherTable()
        {
            WeatherGrid.ItemsSource = _weathers;
            WeatherGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Дата",
                Binding = new Binding("Date")
            };
            column.Binding.StringFormat = "dd.MM.yyyy";
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Час",
                Binding = new Binding("Date")
            };
            column.Binding.StringFormat = "HH:mm";
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Температура, °C",
                Binding = new Binding("Temperature")
            };
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Напрям вітру",
                Binding = new Binding("WindDirection")
            };
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Швидкість вітру, м/с",
                Binding = new Binding("WindPower")
            };
            WeatherGrid.Columns.Add(column);
        }

        private void DurationOfTemperatureModes()
        {
            DurationOfTemperatureModesChart(WeatherChart);
            SetDurationTemperatureTable();
        }

        private void DurationOfTemperatureModesChart(Chart chart)
        {
            ChartController.Clear(chart);
            string temperatureSeries = "t, год";
            ChartController.AddSeries(chart, temperatureSeries, SeriesChartType.Column, Color.Orange);
            _weatherController.DurationOfTemperatureModes(_weathers, out var temperatures, out var temperatureDurations);
            ChartController.Fill(chart, temperatureSeries, temperatures, temperatureDurations);
            ChartController.AxisTitles(chart, "T, °C", "t, год");
        }

        private void SetDurationTemperatureTable()
        {
            WeatherGrid.ItemsSource = _weatherController.DurationTemperatures;
            WeatherGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Температура, °C",
                Binding = new Binding("Temperature")
            };
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Тривалість, год",
                Binding = new Binding("Duration")
            };
            WeatherGrid.Columns.Add(column);
        }

        private void WindsRose()
        {
            WindsRoseChart(WeatherChart);
            SetWindRoseTable();
        }

        private void WindsRoseChart(Chart chart)
        {
            ChartController.Clear(chart);
            _weatherController.WindRose(_weathers, out var directions, out var frequencyPercents);
            List<string> speedSeries = _weatherController.CategoriesInfo;
            List<Color> colors = new List<Color> { Color.Red, Color.DarkOrange, Color.CornflowerBlue, Color.GreenYellow };
            for (int i = 0; i < speedSeries.Count; i++)
            {
                ChartController.AddSeries(chart, speedSeries[i], SeriesChartType.Polar, colors[i]);
                ChartController.Fill(chart, speedSeries[i], directions, frequencyPercents[i]);
                chart.Series[speedSeries[i]].BorderWidth = 5;
            }
            List<WindRose> windRoses = _weatherController.WindRoses;
            for (int i = 0; i < windRoses.Count - 2; i++)
            {
                chart.Series[speedSeries[0]].Points[i].AxisLabel = windRoses[i].Direction;
            }
            chart.ChartAreas[0].AxisX.Interval = 45;
            //ChartController.AxisTitles(chart, "швидкість, м/с", "t, год");
        }

        private void SetWindRoseTable()
        {
            WeatherGrid.ItemsSource = _weatherController.WindRoses;
            WeatherGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Напрям вітру",
                Binding = new Binding("Direction")
            };
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Частота появи, год",
                Binding = new Binding("Frequency")
            };
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Частота появи, %",
                Binding = new Binding("FrequencyPercent")
            };
            WeatherGrid.Columns.Add(column);
        }

        private void DurationOfWindActivityModes()
        {
            DurationOfWindActivityModesChart(WeatherChart);
            SetDurationOfWindActivityTable();
        }

        private void DurationOfWindActivityModesChart(Chart chart)
        {
            ChartController.Clear(chart);
            string speedSeries = "t, год";
            ChartController.AddSeries(chart, speedSeries, SeriesChartType.Column, Color.GreenYellow);
            _weatherController.DurationOfWindActivityModes(_weathers, out var speeds, out var speedDurations);
            ChartController.Fill(chart, speedSeries, speeds, speedDurations);
            ChartController.AxisTitles(chart, "швидкість, м/с", "t, год");
        }

        private void SetDurationOfWindActivityTable()
        {
            WeatherGrid.ItemsSource = _weatherController.WindActivities;
            WeatherGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Швидкість вітру, м/с",
                Binding = new Binding("Speed")
            };
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Тривалість, год",
                Binding = new Binding("Duration")
            };
            WeatherGrid.Columns.Add(column);
        }

        private void IntesityOfSolarInsolation()
        {
            IntesityOfSolarInsolationChart(WeatherChart);
            SetInsolationTable();
        }

        private void IntesityOfSolarInsolationChart(Chart chart)
        {
            ChartController.Clear(chart);
            string seriesName = "Вт/м²";
            ChartController.AddSeries(chart, seriesName, SeriesChartType.Column, Color.Orange);
            _sunConditionController.IntesityOfSolarInsolation(_sunConditions, out var dates, out var insolations);
            ChartController.Fill(chart, seriesName, dates, insolations);
            ChartController.AxisTitles(chart, "Дата", "Вт/м²");
        }

        private void SetInsolationTable()
        {
            WeatherGrid.ItemsSource = _sunConditionController.SelectedSunConditions;
            WeatherGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Дата",
                Binding = new Binding("Date")
            };
            column.Binding.StringFormat = "dd.MM.yyyy";
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Час",
                Binding = new Binding("Date")
            };
            column.Binding.StringFormat = "HH:mm";
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Інтенсивність сонячної інсоляції, Вт/м²",
                Binding = new Binding("Insolation")
            };
            WeatherGrid.Columns.Add(column);
        }

        private void DurationOfSolarActivityModes()
        {
            DurationOfSolarActivityModesChart(WeatherChart);
            SetDurationOfSunActivityTable();
        }

        private void DurationOfSolarActivityModesChart(Chart chart)
        {
            ChartController.Clear(chart);
            string seriesName = "Тривалість режимів сонячної активності";
            ChartController.AddSeries(chart, seriesName, SeriesChartType.Column, Color.DarkSeaGreen);
            _sunConditionController.DurationOfSolarActivityModes(_sunConditions, out var insolations, out var durations);
            ChartController.Fill(chart, seriesName, insolations, durations);
            ChartController.AxisTitles(chart, "Вт/м²", "t, год");
            chart.ChartAreas[0].AxisY.Interval = 1;
        }

        private void SetDurationOfSunActivityTable()
        {
            WeatherGrid.ItemsSource = _sunConditionController.SunActivities;
            WeatherGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Сонячна активність, Вт/м²",
                Binding = new Binding("Insolation")
            };
            WeatherGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Тривалість, год",
                Binding = new Binding("Duration")
            };
            WeatherGrid.Columns.Add(column);
        }

        public void SaveCharts(List<string> paths)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add(new ChartArea("Default"));
            chart.Legends.Add(new Legend("Legend1"));
            TemperatureConditionChart(chart);
            ChartController.SaveImage(chart, paths[0]);

            DurationOfTemperatureModesChart(chart);
            ChartController.SaveImage(chart, paths[1]);

            WindsRoseChart(chart);
            ChartController.SaveImage(chart, paths[2]);

            DurationOfWindActivityModesChart(chart);
            ChartController.SaveImage(chart, paths[3]);

            IntesityOfSolarInsolationChart(chart);
            ChartController.SaveImage(chart, paths[4]);

            DurationOfSolarActivityModesChart(chart);
            ChartController.SaveImage(chart, paths[5]);
        }

        
        #endregion

        #region Month choice
        private void MonthListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMonth = MonthListBox.SelectedIndex;
            DayListBoxPrepare();
            DayListBox.SelectedIndex = 0;
            SelectWeather();
        }
        

        private void DayListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDay = DayListBox.SelectedIndex;
            SelectWeather();
        }

        private void SelectWeather()
        {
            if (_selectedMonth == 12)
            {
                DayListBox.IsEnabled = false;
                _selectedDay = -1;
            }
            else
            {
                DayListBox.IsEnabled = true;
                int daysInMonth = DateTime.DaysInMonth(_weathers[0].Date.Year, _selectedMonth + 1);
                if (_selectedDay == daysInMonth) _selectedDay = -1;
            }

            UpdateWeather();
            AnalyzeTypeChoice();

            PrepareStartEndDateTimePickers();
            //Calculate();
            TimePeriodChanged?.Invoke();
        }

        DataPoint _prevPoint = null;
        private void WeatherChart_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            if (_prevPoint != null)
            {
                _prevPoint.MarkerStyle = MarkerStyle.None;
                _prevPoint.IsValueShownAsLabel = false;
                _prevPoint.Label = "";
            }
            //MessageBox.Show(e.HitTestResult.ChartElementType.ToString());
            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.DataPoint:
                    var dataPoint = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
                    //e.Text = string.Format("X:\t{0}\nY:\t{1}", dataPoint.XValue, dataPoint.YValues[0]);
                    var prop = e.HitTestResult.Object as DataPoint;
                    if (prop != null)
                    {
                        prop.IsValueShownAsLabel = true;
                        prop.MarkerStyle = MarkerStyle.Star4;
                        DateTime date;
                        switch (AnalyzeTypeListBox.SelectedIndex)
                        {
                            case 0:
                                date = DateTime.FromOADate(prop.XValue);
                                prop.Label = $"{date}\n{prop.YValues[0]}°C";
                                break;
                            case 1:
                                prop.Label = $"{prop.XValue}°C\n{prop.YValues[0]} год";
                                break;
                            case 2:
                                prop.Label = $"{prop.YValues[0]}%";
                                break;
                            case 3:
                                prop.Label = $"{prop.XValue} м/с\n{prop.YValues[0]} год";
                                break;
                            case 4:
                                date = DateTime.FromOADate(prop.XValue);
                                prop.Label = $"{date.ToString()}\n{prop.YValues[0]} Вт/м^2";
                                break;
                            case 5:
                                prop.Label = $"{prop.XValue} Вт/м^2\n{prop.YValues[0]} год";
                                break;
                        }
                    }
                    _prevPoint = prop;
                    break;
            }
        }

        private void DateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            if (DateTimePickerStart.Value > DateTimePickerEnd.Value)
            {
                DateTimePickerEnd.ValueChanged -= DateTimePickerEnd_ValueChanged;
                DateTimePickerEnd.Value = DateTimePickerStart.Value;
                DateTimePickerEnd.ValueChanged += DateTimePickerEnd_ValueChanged;
            }
            ChooseWeatherPeriod();
        }

        private void ChooseWeatherPeriod()
        {
            //if (DateTimePickerStart.Value > DateTimePickerEnd.Value) return;
            UpdateWeather(DateTimePickerStart.Value, DateTimePickerEnd.Value);
            AnalyzeTypeChoice();
            //Calculate();
            TimePeriodChanged?.Invoke();
        }

        private void DateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            if (DateTimePickerStart.Value > DateTimePickerEnd.Value)
            {
                DateTimePickerStart.ValueChanged -= DateTimePickerStart_ValueChanged;
                DateTimePickerStart.Value = DateTimePickerEnd.Value;
                DateTimePickerStart.ValueChanged += DateTimePickerStart_ValueChanged;
            }
            ChooseWeatherPeriod();
        }

        private void ConfigDateTimePicker(DateTimePicker picker)
        {
            picker.MinDate = _weatherController.StartDateTime;
            picker.MaxDate = _weatherController.EndDateTime;
        }
        #endregion

    }
}
