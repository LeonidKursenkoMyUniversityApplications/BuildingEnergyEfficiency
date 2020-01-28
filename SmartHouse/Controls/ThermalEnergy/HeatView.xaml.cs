using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using SmartHouse.Controller;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;
using ToolTipEventArgs = System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace SmartHouse.Controls.ThermalEnergy
{
    /// <summary>
    /// Логика взаимодействия для HeatView.xaml
    /// </summary>
    public partial class HeatView : UserControl
    {
        private HouseController _houseController;
        private WeatherController _weatherController;
        private HeatLosses _heatLosses;
        private HeatType _heatType;

        public HeatView()
        {
            InitializeComponent();
        }

        public void Init(HouseController houseController, WeatherController weatherController, HeatType type)
        {
            _weatherController = weatherController;
            PrepareStartEndDateTimePickers();
            _houseController = houseController;
            _houseController.CalculatedTemperature = -22;
            CalculatedTemperatureTextBox.Text = _houseController.CalculatedTemperature.ToString();

            _heatType = type;
            
            ChooseWeatherPeriod();
            HeatLossesDataGrid.ItemsSource = _heatLosses.GetDataTable.DefaultView;
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

        private void ConfigDateTimePicker(DateTimePicker picker)
        {
            picker.MinDate = _weatherController.StartDateTime;
            picker.MaxDate = _weatherController.EndDateTime;
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
            _houseController.Weathers = _weatherController.Weathers.ToList();
            _houseController.Weathers = _houseController
                .GetWeathers(DateTimePickerStart.Value, DateTimePickerEnd.Value);
            Calculate();
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

        DataPoint _prevPoint;
        private void CharacteristicHeatLossesChart_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            GetToolTip(e, "кВтˑгод");
        }

        private void GetToolTip(ToolTipEventArgs e, string label)
        {
            if (_prevPoint != null)
            {
                _prevPoint.IsValueShownAsLabel = false;
                _prevPoint.Label = "";
            }

            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                var prop = e.HitTestResult.Object as DataPoint;
                if (prop != null)
                {
                    prop.IsValueShownAsLabel = true;
                    prop.Label = $"{prop.YValues[0]}{label}";
                }
                _prevPoint = prop;
            }
        }

        private void CalculatedTemperatureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _houseController.CalculatedTemperature = Transformer.ToDouble(CalculatedTemperatureTextBox.Text);
            }
            catch
            {
                MessageBox.Show("Розрахункова температура приймає недопустиме значення", "Помилка");
                return;
            }
            Calculate();
            HeatLossesDataGrid.ItemsSource = _heatLosses.GetDataTable.DefaultView;
        }

        private void Calculate()
        {
            _houseController.Calculate();
            if (_heatType == HeatType.Common) _heatLosses = _houseController.CommonHeatLosses;
            else _heatLosses = _houseController.IndividualHeatLosses;
            CharacteristicHeatConsumption();
            HeatConsumption();
            CalculateCost();
        }

        private void CharacteristicHeatConsumption()
        {
            ChartController.Clear(CharacteristicChart);
            string temperatureSeries = "T, °C";
            ChartController.AddSeries(CharacteristicChart, temperatureSeries, SeriesChartType.Line, Color.CornflowerBlue);
            List<int> temperatures = _heatLosses.Characteristic.Select(x => x.Temperature).ToList();
            List<double> heats = _heatLosses.Characteristic.Select(x => x.Heat).ToList();
            ChartController.Fill(CharacteristicChart, temperatureSeries, temperatures, heats);
            ChartController.AxisTitles(CharacteristicChart, "Температура, °C", "Тепловтрати, кВтˑгод");
            CharacteristicChart.Series[0].MarkerStyle = MarkerStyle.Star4;
            CharacteristicChart.Series[0].MarkerSize = 14;

            SetCharacteristicTable();
        }

        private void SetCharacteristicTable()
        {
            CharacteristicGrid.ItemsSource = _heatLosses.Characteristic;
            CharacteristicGrid.Columns.Clear();
            DataGridTextColumn column;
            column = new DataGridTextColumn()
            {
                Header = "Температура, °C",
                Binding = new System.Windows.Data.Binding("Temperature")
            };
            CharacteristicGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Тепловтрати, кВтˑгод",
                Binding = new System.Windows.Data.Binding("Heat")
            };
            CharacteristicGrid.Columns.Add(column);
        }

        private void HeatChart_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            GetToolTip(e, "кВт");
        }

        private void HeatConsumption()
        {
            ChartController.Clear(HeatChart);
            string temperatureSeries = "T, °C";
            ChartController.AddSeries(HeatChart, temperatureSeries, SeriesChartType.Line, Color.CornflowerBlue);
            List<int> temperatures = _heatLosses.HeatConsumptions.Select(x => x.Temperature).ToList();
            List<double> heats = _heatLosses.HeatConsumptions.Select(x => x.Heat).ToList();
            ChartController.Fill(HeatChart, temperatureSeries, temperatures, heats);
            ChartController.AxisTitles(HeatChart, "Температура, °C", "Тепловтрати, кВт");
            HeatChart.Series[0].MarkerStyle = MarkerStyle.Star4;
            HeatChart.Series[0].MarkerSize = 14;

            SetHeatConsumptionTable();
            TotalConsumptionLabel.Content = $"Загальні витрати за період {_heatLosses.TotalHeatConsumption} кВт";
            TotalHeatHelConsumptionLabel.Content = $"Витрати енергії на опалення та ГВП {_heatLosses.TotalHeatHelConsumption} кВт";
        }

        private void SetHeatConsumptionTable()
        {
            HeatGrid.ItemsSource = _heatLosses.HeatConsumptions;
            HeatGrid.Columns.Clear();
            DataGridTextColumn column;
            column = new DataGridTextColumn()
            {
                Header = "Температура, °C",
                Binding = new System.Windows.Data.Binding("Temperature")
            };
            HeatGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Тепловтрати, кВт",
                Binding = new System.Windows.Data.Binding("Heat")
            };
            HeatGrid.Columns.Add(column);
        }

        private void SetFuelParametresTable()
        {
            TypeHeatGrid.ItemsSource = _heatLosses.Heats;
            TypeHeatGrid.Columns.Clear();
            DataGridTextColumn column;
            column = new DataGridTextColumn()
            {
                Header = "Вид палива",
                Binding = new System.Windows.Data.Binding("Name"),
                IsReadOnly = true
            };
            TypeHeatGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Кількість палива на 1 кВтˑгод енергії",
                Binding = new System.Windows.Data.Binding("FuelPerKWht")
            };
            TypeHeatGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Одиниця виміру",
                Binding = new System.Windows.Data.Binding("Unit"),
                IsReadOnly = true
            };
            TypeHeatGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Вартість палива, грн",
                Binding = new System.Windows.Data.Binding("CostPerFuelUnit")
            };
            TypeHeatGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "ККД котла",
                Binding = new System.Windows.Data.Binding("Efficience")
            };
            TypeHeatGrid.Columns.Add(column);
        }

        private void SetHeatCostTable()
        {
            CostGrid.ItemsSource = _heatLosses.Heats;
            CostGrid.Columns.Clear();
            DataGridTextColumn column;
            column = new DataGridTextColumn()
            {
                Header = "Вид палива",
                Binding = new System.Windows.Data.Binding("Name"),
                IsReadOnly = true
            };
            CostGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Витрати палива",
                Binding = new System.Windows.Data.Binding("FuelConsumption"),
                IsReadOnly = true
            };
            CostGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Одиниця виміру",
                Binding = new System.Windows.Data.Binding("Unit"),
                IsReadOnly = true
            };
            CostGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Витрати, грн",
                Binding = new System.Windows.Data.Binding("TotalPrice"),
                IsReadOnly = true
            };
            CostGrid.Columns.Add(column);
        }

        private void SetHeatCostChart()
        {
            ChartController.Clear(CostChart);
            string series = "Види палива";
            ChartController.AddSeries(CostChart, series, SeriesChartType.Column, Color.DarkOrange);
            List<string> types = _heatLosses.Heats.Select(x => x.Name).ToList();
            List<double> prices = _heatLosses.Heats.Select(x => x.TotalPrice).ToList();
            ChartController.Fill(CostChart, series, types, prices);
            ChartController.AxisTitles(CostChart, "Види палива", "Витрати, грн");
            CostChart.Series[series].Label = "#VALY";
        }

        private void CalculateCost()
        {
            _houseController.CalculatePrice(_heatLosses);
            SetFuelParametresTable();
            SetHeatCostTable();
            SetHeatCostChart();
        }

        private void CalculatedCostButton_Click(object sender, RoutedEventArgs e)
        {
            CalculateCost();
        }

        public void SaveCharts(List<string> paths)
        {
            ChartController.SaveImage(CharacteristicChart, paths[0]);
            ChartController.SaveImage(HeatChart, paths[1]);
            ChartController.SaveImage(CostChart, paths[2]);
        }
    }
}
