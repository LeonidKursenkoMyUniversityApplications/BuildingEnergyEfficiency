using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using SmartHouse.BLL.Controller;
using SmartHouse.Controller;
using Binding = System.Windows.Data.Binding;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace SmartHouse.Controls.WindEnergy
{
    /// <summary>
    /// Логика взаимодействия для WindEnergyView.xaml
    /// </summary>
    public partial class WindEnergyView : UserControl
    {
        private WeatherController _weatherController;
        private WindEnergyController _weController;
        private double _height = 12;

        public WindEnergyView()
        {
            InitializeComponent();
        }

        public void Init(WindEnergyController windEnergyController, WeatherController weatherController)
        {
            _weController = windEnergyController;
            _weatherController = weatherController;
            PrepareStartEndDateTimePickers();
            TowerTextBox.Text = _height.ToString();

            _weController.GreenPrice = 0.108;
            GreenPriceTextBox.Text = _weController.GreenPrice.ToString();
            _weController.ReducedPollutionPrice = 10;
            ReducedPollutionPriceTextBox.Text = _weController.ReducedPollutionPrice.ToString();
            SetWinGenDesTable();
            SetWindGenDesChart();
            ChooseWeatherPeriod();
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

        private void SaveWindGenButton_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Бажаєте зберегти зміни?", "Збереження", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.No) return;
            try
            {
                _height = Transformer.ToDouble(TowerTextBox.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
                return;
            }

            _weController.Save();
            Calculate();
        }

        public void SetWinGenDesTable()
        {
            WindGenDesGrid.ItemsSource = _weController.WindGenDes;
            WindGenDesGrid.Columns.Clear();
            DataGridTextColumn column;
            column = new DataGridTextColumn()
            {
                Header = "Швидкість вітру, м/с",
                Binding = new Binding("Wind")
            };
            WindGenDesGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Потужність, кВт",
                Binding = new Binding("Power")
            };
            WindGenDesGrid.Columns.Add(column);
        }

        public void SetWindGenDesChart()
        {
            ChartController.Clear(WindGenChart);
            string series = "Power";
            ChartController.AddSeries(WindGenChart, series, SeriesChartType.Line, Color.CornflowerBlue);
            List<int> winds = _weController.WindGenDes.Select(x => x.Wind).ToList();
            List<double> powers = _weController.WindGenDes.Select(x => x.Power).ToList();
            ChartController.Fill(WindGenChart, series, winds, powers);
            ChartController.AxisTitles(WindGenChart, "Швидкість вітру, м/с", "Потужність, кВт");
            WindGenChart.Series[0].MarkerStyle = MarkerStyle.Star4;
            WindGenChart.Series[0].MarkerSize = 14;
            WindGenChart.ChartAreas[0].AxisX.Minimum = 0;
        }

        DataPoint _prevPoint;
        private void WindGenChart_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            GetToolTip(e, "кВтˑгод");
        }

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

        private void ConfigDateTimePicker(DateTimePicker picker)
        {
            picker.MinDate = _weatherController.StartDateTime;
            picker.MaxDate = _weatherController.EndDateTime;
        }
        
        private void ChooseWeatherPeriod()
        {
            _weController.Weathers = _weatherController.Weathers.ToList();
            _weController.Weathers = _weController
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

        private void Calculate()
        {
            _weController.Height = _height;
            _weController.HeightMeteorogical = 10;
            _weController.GetWindGenerations();
            SetCharacteristicTable();
            TotalEnergyLabel.Content = $"Всього вироблено енергії:{Math.Round(_weController.TotalEnergy / 1000, 2)}  МВт∙год";
            SetWindChart();
            _weController.GetEconomic();
            Co2TextBox.Text = _weController.Co2ReducedPollution.ToString();
            EnergyCostTextBox.Text = _weController.EnergyCost.ToString();
            ReducedPollutionCostTextBox.Text = _weController.ReducedPollutionCost.ToString();
        }

        private void SetCharacteristicTable()
        {
            WindGrid.ItemsSource = _weController.WindGenerations;
            WindGrid.Columns.Clear();
            DataGridTextColumn column;
            column = new DataGridTextColumn()
            {
                Header = "Швидкість вітру, м/с",
                Binding = new Binding("Wind")
            };
            WindGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Сумарна тривалість, год",
                Binding = new Binding("Duration")
            };
            WindGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Потужність ВЕУ, кВт",
                Binding = new Binding("Power")
            };
            WindGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Енергія вироблена ВЕУ, кВт∙год",
                Binding = new Binding("Energy")
            };
            WindGrid.Columns.Add(column);
        }

        private void SetWindChart()
        {
            ChartController.Clear(WindChart);
            string series = "Energy";
            ChartController.AddSeries(WindChart, series, SeriesChartType.Line, Color.CornflowerBlue);
            List<int> winds = _weController.WindGenerations.Select(x => x.Wind).ToList();
            List<double> powers = _weController.WindGenerations.Select(x => x.Energy).ToList();
            ChartController.Fill(WindChart, series, winds, powers);
            ChartController.AxisTitles(WindChart, "Швидкість вітру, м/с", "Енергія вироблена ВЕУ, кВт∙год");
            WindChart.Series[0].MarkerStyle = MarkerStyle.Star4;
            WindChart.Series[0].MarkerSize = 14;
            WindChart.ChartAreas[0].AxisX.Minimum = 0;
        }

        private void WindChart_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            GetToolTip(e, "кВтˑгод");
        }

        private void CalculateCostButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _weController.GreenPrice = Transformer.ToDouble(GreenPriceTextBox.Text);
                _weController.ReducedPollutionPrice = Transformer.ToDouble(ReducedPollutionPriceTextBox.Text);
            }
            catch
            {
                return;
            }
            Calculate();
        }

        public void SaveCharts(List<string> paths)
        {
            ChartController.SaveImage(WindGenChart, paths[0]);
            ChartController.SaveImage(WindChart, paths[1]);
        }
    }
}
