using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using SmartHouse.Model;

namespace SmartHouse.Controls
{
    /// <summary>
    /// Логика взаимодействия для PriceConsumeptionView.xaml
    /// </summary>
    public partial class PriceConsumeptionView : UserControl
    {
        private double _priceFor1PhaseLess100 = 0.9;
        private double _priceFor1PhaseMore100 = 1.68;

        private double _priceFor2Phase = 0.9;

        private double _priceFor3Phase = 0.9;

        private DeviceController _deviceController;

        public PriceConsumeptionView()
        {
            InitializeComponent();
        }

        public void Init(DeviceController deviceController)
        {
            _deviceController = deviceController;
            
            CalculateFor1Phase();
            CalculateFor2Phase();
            CalculateFor3Phase();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox1PhaseLess100.Text = _priceFor1PhaseLess100.ToString();
            TextBox1PhaseMore100.Text = _priceFor1PhaseMore100.ToString();

            TextBox2PhasePrice.Text = _priceFor2Phase.ToString();

            TextBox3PhasePrice.Text = _priceFor3Phase.ToString();
        }

        private void Button1Phase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _priceFor1PhaseLess100 = Double.Parse(TextBox1PhaseLess100.Text);
                _priceFor1PhaseMore100 = Double.Parse(TextBox1PhaseMore100.Text);
            }
            catch
            {
                MessageBox.Show("Недопустимий формат вхідний даних");
            }
            CalculateFor1Phase();
            CalculateFor2Phase();
        }

        private void CalculateFor1Phase()
        {
            _deviceController.ElectricalPrices.OneKwhtPriceFor1PhaseLess100 = _priceFor1PhaseLess100;
            _deviceController.ElectricalPrices.OneKwhtPriceFor1PhaseMore100 = _priceFor1PhaseMore100;
            _deviceController.CalculatePricesForZones();

            Set1PhaseTable();
        }

        private void Set1PhaseTable()
        {
            FillTable(_deviceController.Prices1Phase, DataGrid1Phase);
        }

        private void FillTable(List<PriceRow> rows, DataGrid dataGrid)
        {
            dataGrid.Columns.Clear();
            dataGrid.ItemsSource = rows;
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Назва",
                Binding = new Binding("Name")
            };
            dataGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Споживання, кВт·год",
                Binding = new Binding("Consumption")
            };
            column.Binding.StringFormat = "0.###";
            dataGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Витрати, грн",
                Binding = new Binding("Price")
            };
            column.Binding.StringFormat = "0.##";
            dataGrid.Columns.Add(column);
        }

        private void Button2Phase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _priceFor2Phase = Double.Parse(TextBox2PhasePrice.Text);
            }
            catch
            {
                MessageBox.Show("Недопустимий формат вхідний даних");
            }
            CalculateFor2Phase();
        }

        private void CalculateFor2Phase()
        {
            _deviceController.ElectricalPrices.OneKwhtPriceFor2Phase = _priceFor2Phase;
            _deviceController.ElectricalPrices.DayFactorFor2Phase = 1;
            _deviceController.ElectricalPrices.NightFactorFor2Phase = 0.5;
            _deviceController.CalculatePricesForZones();

            Set2PhaseTable();
        }

        private void Set2PhaseTable()
        {
            FillTable(_deviceController.Prices2Phase, DataGrid2Phase);
        }

        private void Button3Phase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _priceFor3Phase = Double.Parse(TextBox3PhasePrice.Text);
            }
            catch
            {
                MessageBox.Show("Недопустимий формат вхідний даних");
            }
            CalculateFor3Phase();
        }

        private void CalculateFor3Phase()
        {
            _deviceController.ElectricalPrices.OneKwhtPriceFor3Phase = _priceFor3Phase;
            _deviceController.ElectricalPrices.MaxLoadFactorFor3Phase = 1.5;
            _deviceController.ElectricalPrices.HalfMaxLoadFactorFor3Phase = 1;
            _deviceController.ElectricalPrices.NightFactorFor3Phase = 0.4;
            _deviceController.CalculatePricesForZones();

            Set3PhaseTable();
        }

        private void Set3PhaseTable()
        {
            FillTable(_deviceController.Prices3Phase, DataGrid3Phase);
        }
    }
}
