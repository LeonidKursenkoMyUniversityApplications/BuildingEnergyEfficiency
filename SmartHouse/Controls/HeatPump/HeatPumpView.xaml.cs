using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using SmartHouse.Controller;
using SmartHouse.DAL.Model.HeatPump;
using Color = System.Drawing.Color;

namespace SmartHouse.Controls.HeatPomp
{
    /// <summary>
    /// Логика взаимодействия для HeatPumpView.xaml
    /// </summary>
    public partial class HeatPumpView : UserControl
    {
        private HouseController _houseController;
        private HeatPumpController _heatPumpController;
        private DeviceController _deviceController;
        private HeatPump _heatPump;
        private List<TypeOfHeat> _heats;

        public List<HeatPumpDescription> HeatPumpDescriptions { set; get; }

        public HeatPumpView()
        {
            InitializeComponent();
        }

        public void Init(HeatPumpController heatPumpController, HouseController houseController, DeviceController deviceController)
        {
            _deviceController = deviceController;
            _houseController = houseController;
            _heatPumpController = heatPumpController;
            _heatPump = _heatPumpController.HeatPump;
            HeatPumpDesGrid.ItemsSource = _heatPump.HeatPumpDescriptions;
            CoefficientComboBox.SelectedIndex = 0;
            NominalHeatProductionTextBox.Text = _heatPump.NominalHeatProduction.ToString();
            NominalPowerTextBox.Text = _heatPump.NominalPower.ToString();
            HeatPumpCountTextBox.Text = _heatPump.HeatPumpCount.ToString();
            CirculationPumpCountTextBox.Text = _heatPump.CirculationPumpCount.ToString();
            CirculationPumpPowerTextBox.Text = _heatPump.CirculationPower.ToString();
            FancoilCountTextBox.Text = _heatPump.FancoilCount.ToString();
            FancoilPowerTextBox.Text = _heatPump.FancoilPower.ToString();
            _heatPump.PricePerKwht = _deviceController.ElectricalPrices.OneKwhtPriceFor1PhaseMore100;
            Calculate();
        }

        private void Calculate()
        {
            _heats = _houseController.CommonHeatLosses.Heats.ToList();
            _heatPumpController.Calculate(_houseController.CommonHeatLosses);
            HeatDataGrid.ItemsSource = null;
            HeatDataGrid.ItemsSource = _heatPump.HeatPumpCalculations;
            TextRange textRange = new TextRange(CommonInfoBox.Document.ContentStart, CommonInfoBox.Document.ContentEnd);
            textRange.Text = "Загальні витрати за визначений період:\n" +
                             $"Qтеп.= {_heatPump.TotalHeatLosses} кВтˑгод\n " +
                             $"Wспож.тн.= {_heatPump.TotalHeatPumpConsumption} кВт∙год\n" +
                             $"Wспож.сис.= {_heatPump.TotalHeatSystemConsumption} кВт∙год\n" +
                             $"Qтн.= {_heatPump.TotalQuantityHeatPumpProduction} кВт∙год\n" +
                             $"Qдогр.= {_heatPump.TotalQuantityAdditionalHeatProduction} кВт∙год\n" +
                             $"Qсис.= {_heatPump.TotalQuantityHeatSystemProduction} кВт∙год\n" + 
                             $"COPтн.= {_heatPump.AverageEfficiencyHeatPump}\n" +
                             $"COPсист.= {_heatPump.AverageEfficiencyHeatSystem}\n" +
                             $"Витрати= {_heatPump.TotalCost} грн.";
            _heatPumpController.SetHeatsCost(_heats);
            _heats = _heatPumpController.Heats;
            SetHeatCostTable();
            SetHeatCostChart();
        }

        public void SetHeatProductionCorrectionChart()
        {
            SetHeatProductionCorrectionChart(HeatPumpChart);
        }

        public void SetHeatProductionCorrectionChart(Chart chart)
        {
            ChartController.Clear(chart);
            string series = "Coefficient";
            ChartController.AddSeries(chart, series, SeriesChartType.Line, Color.CornflowerBlue);
            List<int> temperatures = _heatPump.HeatPumpDescriptions.Select(x => x.Temperature).ToList();
            List<double> heatProds = _heatPump.HeatPumpDescriptions.Select(x => x.HeatProductionCorrection).ToList();
            ChartController.Fill(chart, series, temperatures, heatProds);
            ChartController.AxisTitles(chart, "Температура, °C", "Коефіцієнт корекції");
            chart.Series[0].MarkerStyle = MarkerStyle.Star4;
            chart.Series[0].MarkerSize = 14;
        }

        public void SetHeatPowerCorrectionChart()
        {
            SetHeatPowerCorrectionChart(HeatPumpChart);
        }

        public void SetHeatPowerCorrectionChart(Chart chart)
        {
            ChartController.Clear(chart);
            string series = "Coefficient";
            ChartController.AddSeries(chart, series, SeriesChartType.Line, Color.CornflowerBlue);
            List<int> temperatures = _heatPump.HeatPumpDescriptions.Select(x => x.Temperature).ToList();
            List<double> heatPowers = _heatPump.HeatPumpDescriptions.Select(x => x.HeatPowerCorrection).ToList();
            ChartController.Fill(chart, series, temperatures, heatPowers);
            ChartController.AxisTitles(chart, "Температура, °C", "Коефіцієнт корекції");
            chart.Series[0].MarkerStyle = MarkerStyle.Star4;
            chart.Series[0].MarkerSize = 14;
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

        private void SaveHeatPumpButton_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Бажаєте зберегти зміни?", "Збереження", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.No) return;
            double nominalHeatProduction;
            double nominalPower;
            int heatPumpCount, circulationPumpCount, fancoilCount;
            double circulationPumpPower, fancoilPower;
            try
            {
                nominalHeatProduction = Transformer.ToDouble(NominalHeatProductionTextBox.Text);
                nominalPower = Transformer.ToDouble(NominalPowerTextBox.Text);
                heatPumpCount = Transformer.ToInt(HeatPumpCountTextBox.Text);
                circulationPumpCount = Transformer.ToInt(CirculationPumpCountTextBox.Text);
                fancoilCount = Transformer.ToInt(FancoilCountTextBox.Text);
                circulationPumpPower = Transformer.ToDouble(CirculationPumpPowerTextBox.Text);
                fancoilPower = Transformer.ToDouble(FancoilPowerTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
                return;
            }
            _heatPump.NominalHeatProduction = nominalHeatProduction;
            _heatPump.NominalPower = nominalPower;
            _heatPump.HeatPumpCount = heatPumpCount;
            _heatPump.CirculationPumpCount = circulationPumpCount;
            _heatPump.FancoilCount = fancoilCount;
            _heatPump.CirculationPower = circulationPumpPower;
            _heatPump.FancoilPower = fancoilPower;
            _heatPumpController.Save();
            Calculate();
        }

        private void CoefficientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CoefficientComboBox.SelectedIndex)
            {
                case 0: SetHeatProductionCorrectionChart();
                    break;
                case 1: SetHeatPowerCorrectionChart();
                    break;
            }
        }

        private void HeatPumpChart_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            GetToolTip(e, "");
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
            ChartController.AddSeries(CostChart, series, SeriesChartType.Column, Color.DarkOrange);
            List<string> types = _heats.Select(x => x.Name).ToList();
            List<double> prices = _heats.Select(x => x.TotalPrice).ToList();
            ChartController.Fill(CostChart, series, types, prices);
            ChartController.AxisTitles(CostChart, "Види палива", "Витрати, грн");
            CostChart.Series[series].Label = "#VALY";
        }

        public void SaveCharts(List<string> paths)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add(new ChartArea("Default"));
            chart.Legends.Add(new Legend("Legend1"));
            chart.Legends[0].Enabled = false;
            SetHeatProductionCorrectionChart(chart);
            ChartController.SaveImage(chart, paths[0]);
            SetHeatPowerCorrectionChart(chart);
            ChartController.SaveImage(chart, paths[1]);
            ChartController.SaveImage(CostChart, paths[2]);
        }
        
    }
}
