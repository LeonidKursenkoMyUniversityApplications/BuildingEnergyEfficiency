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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using SmartHouse.Controller;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace SmartHouse.Controls.HeatStore
{
    /// <summary>
    /// Логика взаимодействия для HeatStoreView.xaml
    /// </summary>
    public partial class HeatStoreView : UserControl
    {
        private HeatStoreController _heatStoreController;
        private HouseController _houseController;

        private BLL.Model.HeatStore _heatStore;
        private List<TypeOfHeat> _heats;

        private TimeSpan _start = new TimeSpan(0, 7, 0, 0);
        private TimeSpan _end = new TimeSpan(0, 23, 0, 0);
        private double _dayRate = 0.9;
        private double _nightRate = 0.45;
        private double _capacity = 200;
        private double _power =  26;


        public HeatStoreView()
        {
            InitializeComponent();
        }

        public void Init(HeatStoreController heatStoreController, HouseController houseController)
        {
            _houseController = houseController;
            _heatStoreController = heatStoreController;
            _heatStore = _heatStoreController.HeatStore;
            StoreChart.MouseWheel -= StoreChartMouseWheel;
            StoreChart.MouseWheel += StoreChartMouseWheel;

            StartZoneTextBox.Text = _start.ToString(@"hh\:mm");
            EndZoneTextBox.Text = _end.ToString(@"hh\:mm");
            DayRateTextBox.Text = _dayRate.ToString();
            NightRateTextBox.Text = _nightRate.ToString();
            CapacityTextBox.Text = _capacity.ToString();
            PowerTextBox.Text = _power.ToString();
            Calculate();
        }

        private void StoreChartMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            //var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    //yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    //var yMin = yAxis.ScaleView.ViewMinimum;
                    //var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    //var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    //var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    //yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        public void Calculate()
        {
            _heatStore.StartDayZone = _start;
            _heatStore.EndDayZone = _end;
            _heatStore.DayRate = _dayRate;
            _heatStore.NightRate = _nightRate;
            _heatStore.Capacity = _capacity;
            _heatStore.Power = _power;

            _heatStoreController.Calculate(_houseController.Weathers, _houseController.CommonHeatLosses.Characteristic,
                _houseController.CommonHeatLosses.Heats);
            HeatStoreDataGrid.ItemsSource = null;
            HeatStoreDataGrid.ItemsSource = _heatStore.Records;

            TextRange textRange = new TextRange(CommonInfoBox.Document.ContentStart, CommonInfoBox.Document.ContentEnd);
            textRange.Text = "Загальні витрати за визначений період:\n" +
                             $"Спожито електроенергії в нічний час {_heatStore.TotalNightConsumption} кВт·год " +
                             $"на суму {_heatStore.TotalNightCost} грн;\n" +
                             $"Спожито електроенергії в денний час {_heatStore.TotalDayConsumption} кВт·год " +
                             $"на суму {_heatStore.TotalDayCost} грн;\n" +
                             $"Спожито електроенергії за весь період {_heatStore.TotalConsumption} кВт·год " +
                             $"на суму {_heatStore.TotalCost} грн;\n" +
                             $"В найгірший день {_heatStore.WorstDay.Date:dd.MM.yyyy} " +
                             $"в накопичувачі не вистачило {_heatStore.WorstDay.EnergyLack} кВт енергії, " +
                             $"тривалість дефіциту {_heatStore.WorstDay.HourLack} годин.";
            _heats = _heatStore.Heats;
            SetHeatCostChart();
            SetHeatCostTable();
            SetStoreChart();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _start = TimeSpan.Parse(StartZoneTextBox.Text);
                _end = TimeSpan.Parse(EndZoneTextBox.Text);
                _dayRate = Transformer.ToDouble(DayRateTextBox.Text);
                _nightRate = Transformer.ToDouble(NightRateTextBox.Text);
                _capacity = Transformer.ToDouble(CapacityTextBox.Text);
                _power = Transformer.ToDouble(PowerTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
                return;
            }

            
            Calculate();
        }

        private void SetHeatCostTable()
        {
            CostGrid.ItemsSource = _heats;
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
            ChartController.AddSeries(CostChart, series, SeriesChartType.Column, Color.GreenYellow);
            List<string> types = _heats.Select(x => x.Name).ToList();
            List<double> prices = _heats.Select(x => x.TotalPrice).ToList();
            ChartController.Fill(CostChart, series, types, prices);
            ChartController.AxisTitles(CostChart, "Види палива", "Витрати, грн");
            CostChart.Series[series].Label = "#VALY";
        }

        private void SetStoreChart()
        {
            ChartController.Clear(StoreChart);
            string series = "Накопичення тепла";
            ChartController.AddSeries(StoreChart, series, SeriesChartType.Line, Color.Green);
            List<DateTime> dates = _heatStore.Records.Select(x => x.StartDate).ToList();
            List<double> stores = _heatStore.Records.Select(x => x.EndEnergyAmount).ToList();
            ChartController.Fill(StoreChart, series, dates, stores);
            ChartController.AxisTitles(StoreChart, "Дата", "Обсяги енергії у накопичувачі, кВт·год");
            //StoreChart.Series[series].Label = "#VALY";
            StoreChart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
        }

        DataPoint _prevPoint;
        private void GetToolTip(System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e, string label)
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

        private void StoreChart_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            GetToolTip(e, "");
        }

        public void SaveCharts(List<string> paths)
        {
            ChartController.SaveImage(StoreChart, paths[0]);
            ChartController.SaveImage(CostChart, paths[1]);
        }
    }
}
