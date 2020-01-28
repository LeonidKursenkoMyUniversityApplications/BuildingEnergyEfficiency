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

namespace SmartHouse.Controls.GraphEnergyConsumption
{
    /// <summary>
    /// Логика взаимодействия для OptGelForZonesView.xaml
    /// </summary>
    public partial class OptGelForZonesView : UserControl
    {
        private DeviceController _deviceController;

        public OptGelForZonesView()
        {
            InitializeComponent();
        }

        public void Init(DeviceController deviceController)
        {
            _deviceController = deviceController;
            DeviceTableChart.Init(_deviceController);
            RatesConsumeptionView.Init(_deviceController);
            PriceConsumptionView.Init(_deviceController);
        }

        public void SaveCharts(List<string> paths)
        {
            DeviceTableChart.SaveCharts(paths);
        }
    }
}
