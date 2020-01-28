using System;
using System.Collections.Generic;
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
using SmartHouse.DAL.Model;

namespace SmartHouse.Controls
{
    /// <summary>
    /// Логика взаимодействия для RatesConsumeptionView.xaml
    /// </summary>
    public partial class RatesConsumeptionView : UserControl
    {
        private DeviceController _deviceController;
        private List<RatesConsumption> _ratesConsumptions;

        public RatesConsumeptionView()
        {
            InitializeComponent();
        }

        public void Init(DeviceController deviceController)
        {
            _deviceController = deviceController;
            _ratesConsumptions = _deviceController.RatesConsumptions;
            SetTable();
        }

        public void SetTable()
        {
            DataGrid.ItemsSource = _ratesConsumptions;
            DataGrid.Columns.Clear();

            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Показник",
                Binding = new Binding("Name")
            };
            DataGrid.Columns.Add(column);

            for (int i = 0; i < Constants.RatesOfConsumption.Count; i++)
            {
                column = new DataGridTextColumn()
                {
                    Header = Constants.RatesOfConsumption[i],
                    Binding = new Binding($"{Constants.RatesOfConsumptionProps[i]}")
                };
                column.Binding.StringFormat = "0.####";
                DataGrid.Columns.Add(column);
            }
            
        }
    }
}
