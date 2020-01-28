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
using SmartHouse.DAL.Model;
using SmartHouse.Controller;
using Color = System.Drawing.Color;

namespace SmartHouse.Controls.ElectricalLoadSchedule
{
    /// <summary>
    /// Логика взаимодействия для ElectricalLoadScheduleView.xaml
    /// </summary>
    public partial class ElectricalLoadScheduleView : UserControl
    {
        private DeviceController _deviceController;
        private int _selectedDevice = 0;
        private int _selectedDeviceDayOfWeek = 0;

        private List<Device> _tempDevices;
        private List<DeviceDayOfWeek> _tempDeviceDayOfWeeks;
        private List<Period> _tempDevicePeriods;
        public ElectricalLoadScheduleView()
        {
            InitializeComponent();
            DevicePeriodsChart.MouseWheel += ChartController.Сhart_MouseWheel;
        }

        public void Init(DeviceController deviceController)
        {
            _deviceController = deviceController;
            _tempDevices = _deviceController.Devices.ToList();
            DeviceDataGrid.ItemsSource = _tempDevices;

            DeviceTypeListBox.ItemsSource = _tempDevices.Select(x => x.Name).ToList();
            DeviceTypeListBox.SelectionChanged -= DeviceTypeListBox_SelectionChanged;
            DeviceTypeListBox.SelectedIndex = _selectedDevice;
            DeviceTypeListBox.SelectionChanged += DeviceTypeListBox_SelectionChanged;

            DeviceDayOfWeekListBox.ItemsSource = Constants.DayOfWeek;
            DeviceDayOfWeekListBox.SelectedIndex = _selectedDeviceDayOfWeek;

            RealGraphEnergyLoadView.Init(_deviceController);
            OptimizationView.Init(_deviceController);
        }
        
        private void AddDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            _tempDevices.Add(new Device($"Device{DeviceDataGrid.Items.Count}", 0));
            DeviceDataGrid.ItemsSource = null;
            DeviceDataGrid.ItemsSource = _tempDevices;
        }

        private void SaveDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Бажаєте зберегти зміни?", "Збереження", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _deviceController.Devices = _tempDevices.ToList();
                _deviceController.Save(_deviceController.FileName);
            }
        }

        private void DeleteDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = DeviceDataGrid.SelectedIndex;
                if (index == -1) return;
                MessageBoxResult result = MessageBox.Show($"Бажаєте видалити {_tempDevices[index].Name}?", "Видалення", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _tempDevices.RemoveAt(index);
                    DeviceDataGrid.ItemsSource = null;
                    DeviceDataGrid.ItemsSource = _tempDevices;
                }

            }
            catch
            {
                // ignored
            }
        }

        private void DeviceDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DeviceDataGrid.SelectedIndex == -1) DeleteDeviceButton.IsEnabled = false;
            else DeleteDeviceButton.IsEnabled = true;
        }

        public void SaveCharts(List<string> paths, List<string> opt2Paths, List<string> opt3Paths)
        {
            RealGraphEnergyLoadView.SaveCharts(paths);
            OptimizationView.SaveCharts(opt2Paths, opt3Paths);
        }
        

        #region Tab2

        private void DeviceTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectDevice();
        }

        private void SelectDevice()
        {
            _selectedDevice = DeviceTypeListBox.SelectedIndex;
            _selectedDeviceDayOfWeek = DeviceDayOfWeekListBox.SelectedIndex;
            string search = DeviceTypeListBox.SelectedItem.ToString();
            _tempDeviceDayOfWeeks = _deviceController.Devices.First(x => x.Name == search).DayOfWeek.ToList();
            _tempDevicePeriods = _tempDeviceDayOfWeeks[_selectedDeviceDayOfWeek].Periods.ToList();
            DevicePeriodsDataGrid.ItemsSource = _tempDevicePeriods;

            ChartController.Clear(DevicePeriodsChart);
            string seriesName = _tempDevices[_selectedDevice].Name;
            ChartController.AddSeries(DevicePeriodsChart, seriesName,
                SeriesChartType.Area, Color.DarkOrchid);
            ChartController.AxisTitles(DevicePeriodsChart, "Час", "Потужність, кВт");
            ChartController.Fill(DevicePeriodsChart, seriesName, _tempDevicePeriods, _tempDevices[_selectedDevice].Power);
        }

        private void DeviceDayOfWeekListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectDevice();
        }

        private void DeleteDevicePeriodButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = DevicePeriodsDataGrid.SelectedIndex;
                if (index == -1) return;
                MessageBoxResult result = MessageBox.Show(
                    $"Бажаєте видалити період {_tempDevicePeriods[index].Start}-{_tempDevicePeriods[index].End}?",
                    "Видалення", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _tempDevicePeriods.RemoveAt(index);
                    DevicePeriodsDataGrid.ItemsSource = null;
                    DevicePeriodsDataGrid.ItemsSource = _tempDevicePeriods;
                }

            }
            catch
            {
                // ignored
            }
        }

        private void AddDevicePeriodButton_Click(object sender, RoutedEventArgs e)
        {
            int day = _selectedDeviceDayOfWeek + 1;
            _tempDevicePeriods.Add(new Period(new DateTime(2018, 10, day, 0, 0, 0),
                new DateTime(2018, 10, day, 0, 0, 0)));
            DevicePeriodsDataGrid.ItemsSource = null;
            DevicePeriodsDataGrid.ItemsSource = _tempDevicePeriods;
        }

        private void SaveDevicePeriodButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Бажаєте зберегти зміни?", "Збереження", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                _tempDeviceDayOfWeeks[_selectedDeviceDayOfWeek].Periods = _tempDevicePeriods;
                _tempDevices[_selectedDevice].DayOfWeek = _tempDeviceDayOfWeeks;
                _deviceController.Devices = _tempDevices.ToList();
                _deviceController.Update();
                _deviceController.Save(_deviceController.FileName);
            }
        }

        private void DevicePeriodsDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DeleteDevicePeriodButton.IsEnabled = DevicePeriodsDataGrid.SelectedIndex != -1;
        }
        #endregion
    }
}
