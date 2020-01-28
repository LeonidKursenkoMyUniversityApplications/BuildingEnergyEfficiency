using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using Binding = System.Windows.Data.Binding;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace SmartHouse.Controls.GraphEnergyConsumption
{
    /// <summary>
    /// Логика взаимодействия для OptimizationGraphEnergyLoadView.xaml
    /// </summary>
    public partial class OptimizationGraphEnergyLoadView : UserControl
    {
        private string _deviceFile = @"temp/deviceOptimization";
        private DeviceController _deviceController;
        public DeviceController Device2ZoneOptController { set; get; }
        public DeviceController Device3ZoneOptController { set; get; }
        public List<DeviceOptimization> DeviceOptimizations { set; get; }

        public OptimizationGraphEnergyLoadView()
        {
            InitializeComponent();
        }

        public void Init(DeviceController deviceController)
        {
            _deviceController = deviceController;
            //DeviceOptimizations = _deviceController.GetDeviceOptimizations();
            DeviceOptimizations =
                DeviceOptimizationController.Read(_deviceFile, _deviceController.Devices);
            _deviceController.DevicesForOptimization = DeviceOptimizations;
            DevicesListBox.ItemsSource = null;
            DevicesListBox.ItemsSource = DeviceOptimizations;
            //DevicesListBox.UpdateLayout();

            Device2ZoneOptController = _deviceController.Copy();
            For2ZonesView.Init(Device2ZoneOptController);

            Device3ZoneOptController = _deviceController.Copy();
            For3ZonesView.Init(Device3ZoneOptController);

            Optimization();
            OptTabControl.SelectedIndex = 1;
        }

        
        private void OptimizationButton_Click(object sender, RoutedEventArgs e)
        {
            Optimization();
            OptTabControl.SelectedIndex = 1;
        }

        private void Optimization()
        {
            DeviceOptimizationController.Save(_deviceFile, DeviceOptimizations);
            Device2ZoneOptController.DevicesForOptimization = DeviceOptimizations;
            Device2ZoneOptController.StartOptimization(OptimizationType.TwoZone);
            For2ZonesView.Init(Device2ZoneOptController);

            Device3ZoneOptController.DevicesForOptimization = DeviceOptimizations;
            Device3ZoneOptController.StartOptimization(OptimizationType.ThreeZone);
            For3ZonesView.Init(Device3ZoneOptController);
        }

        public void SaveCharts(List<string> opt2Paths, List<string> opt3Paths)
        {
            For2ZonesView.SaveCharts(opt2Paths);
            For3ZonesView.SaveCharts(opt3Paths);
        }
    }
}
