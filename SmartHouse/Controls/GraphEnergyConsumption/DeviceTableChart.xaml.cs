using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using SmartHouse.Controller;
using SmartHouse.DAL.Model;
using Color = System.Drawing.Color;

namespace SmartHouse.Controls.GraphEnergyConsumption
{
    /// <summary>
    /// Логика взаимодействия для DeviceTableChart.xaml
    /// </summary>

    public partial class DeviceTableChart : UserControl
    {
        private DeviceController _deviceController;
        private List<string> _typeOfDeviceGraph;

        private int _selectedGraphType = 0;
        private int _selectedDevice = 0;
        private int _selectedDayOfWeek = 0;

        private List<Device> _devices;
        private List<ElectricalLoad> _dayElectricalLoads;
        private List<List<ElectricalLoad>> _weekElectricalLoads;
        private List<List<List<ElectricalLoad>>> _electricalLoadsForDevices;

        private List<DurationElectricalLoad> _dayDurationElectricalLoads;
        private List<List<DurationElectricalLoad>> _weekDurationElectricalLoads;
        private List<List<List<DurationElectricalLoad>>> _durationElectricalLoadForDevices;

        private List<List<double>> _electricalConsumptions;

        public DeviceTableChart()
        {
            InitializeComponent();
            Chart.MouseWheel += ChartController.Сhart_MouseWheel;
        }

        public void Init(DeviceController deviceController)
        {
            _deviceController = deviceController;
            _electricalLoadsForDevices = _deviceController.ElectricalLoadsForDevices;
            _durationElectricalLoadForDevices = _deviceController.DurationElectricalLoadsForDevices;

            _typeOfDeviceGraph = new List<string>() { "Навантаження", "Тривалості навантаження", "Споживання" };
            DeviceGraphTypeListBox.ItemsSource = _typeOfDeviceGraph;
            DeviceGraphTypeListBox.SelectionChanged -= DeviceGraphTypeListBox_SelectionChanged;
            DeviceGraphTypeListBox.SelectedIndex = _selectedGraphType;
            DeviceGraphTypeListBox.SelectionChanged += DeviceGraphTypeListBox_SelectionChanged;

            _devices = _deviceController.Devices;
            var devices = _devices.Select(x => x.Name).ToList();
            devices.Add("Всі");
            DeviceTypeListBox.ItemsSource = devices;
            DeviceTypeListBox.SelectionChanged -= DeviceTypeListBox_SelectionChanged;
            DeviceTypeListBox.SelectedIndex = _selectedDevice;
            DeviceTypeListBox.SelectionChanged += DeviceTypeListBox_SelectionChanged;

            var dayOfWeek = Constants.DayOfWeek.ToList();
            dayOfWeek.Add("Всі");
            DeviceDayOfWeekListBox.ItemsSource = dayOfWeek;
            //DeviceDayOfWeekListBox.SelectionChanged -= DeviceDayOfWeekListBox_SelectionChanged;
            DeviceDayOfWeekListBox.SelectedIndex = _selectedDayOfWeek;
            //DeviceDayOfWeekListBox.SelectionChanged += DeviceDayOfWeekListBox_SelectionChanged;
        }
        
        private void DeviceGraphTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Select();
        }

        private void DeviceTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Select();
        }

        private void DeviceDayOfWeekListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Select();
        }

        private void Select()
        {
            _selectedGraphType = DeviceGraphTypeListBox.SelectedIndex;
            _selectedDevice = DeviceTypeListBox.SelectedIndex;
            _selectedDayOfWeek = DeviceDayOfWeekListBox.SelectedIndex;

            switch (_selectedGraphType)
            {
                case 0: ViewElectricalLoads();
                    break;
                case 1: ViewDurationsOfElectricalLoad();
                    break;
                case 2: ViewElectricalConsumption();
                    break;
            }

            
        }

        private void ViewElectricalLoads()
        {
            _weekElectricalLoads = _electricalLoadsForDevices[_selectedDevice];
            _dayElectricalLoads = _weekElectricalLoads[_selectedDayOfWeek];
            SetLoadTable();
            ElectricalLoadChart(Chart, _dayElectricalLoads, _selectedDayOfWeek);
        }

        private void ElectricalLoadChart(Chart chart, List<ElectricalLoad> dayElectricalLoads, int selectedDayOfWeek)
        {
            ChartController.Clear(chart);
            string seriesName = DeviceTypeListBox.SelectedItem.ToString();
            ChartController.AddSeries(chart, seriesName,
                SeriesChartType.Area, Color.DarkOrchid);
            ChartController.AxisTitles(chart, "Час", "Потужність, кВт");
            ChartController.Fill(chart, seriesName, dayElectricalLoads);
            string stringFormat = selectedDayOfWeek == 7 ? "dddd HH:mm:ss" : "HH:mm:ss";
            chart.ChartAreas[0].AxisX.LabelStyle.Format = stringFormat;
        }

        private void SetLoadTable()
        {
            DataGrid.ItemsSource = _dayElectricalLoads;
            DataGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Від",
                Binding = new Binding("Start")
            };
            string stringFormat = _selectedDayOfWeek == 7 ? "dddd HH:mm:ss" : "HH:mm:ss";
            column.Binding.StringFormat = stringFormat;
            DataGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "До",
                Binding = new Binding("End")
            };
            column.Binding.StringFormat = stringFormat;
            DataGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Потужність, кВт",
                Binding = new Binding("Power")
            };
            DataGrid.Columns.Add(column);
        }

        private void ViewDurationsOfElectricalLoad()
        {
            _weekDurationElectricalLoads = _durationElectricalLoadForDevices[_selectedDevice];
            _dayDurationElectricalLoads = _weekDurationElectricalLoads[_selectedDayOfWeek];
            SetDurationLoadTable();
            DurationOfElectricalLoadChart(Chart, _dayDurationElectricalLoads);
        }

        private void DurationOfElectricalLoadChart(Chart chart, List<DurationElectricalLoad> dayDurationElectricalLoads)
        {
            ChartController.Clear(chart);
            string seriesName = DeviceTypeListBox.SelectedItem.ToString();
            ChartController.AddSeries(chart, seriesName,
                SeriesChartType.Column, Color.DarkOrchid);
            ChartController.AxisTitles(chart, "Навантаження, кВт", "Тривалість, год");
            var durations = dayDurationElectricalLoads.Select(x => x.Duration).ToList();
            var loads = dayDurationElectricalLoads.Select(x => x.Power).ToList();
            ChartController.Fill(chart, seriesName, loads, durations);
        }

        private void SetDurationLoadTable()
        {
            DataGrid.ItemsSource = _dayDurationElectricalLoads;
            DataGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Потужність, кВт",
                Binding = new Binding("Power")
            };
            DataGrid.Columns.Add(column);

            column = new DataGridTextColumn()
            {
                Header = "Тривалість, год",
                Binding = new Binding("Duration")
            };
            column.Binding.StringFormat = "0.####";
            DataGrid.Columns.Add(column);
        }

        private void ViewElectricalConsumption()
        {
            _electricalConsumptions = _deviceController.ElectricalConsumptions;
            SetElectricalConsumptionsTable();
            List<double> consumptions = new List<double>();
            List<string> days = new List<string>();
            // If user select device.
            if (_selectedDevice != DeviceTypeListBox.Items.Count - 1)
            {
                // If user select day of week.
                if (_selectedDayOfWeek != DeviceDayOfWeekListBox.Items.Count - 1)
                {
                    consumptions.Add(_electricalConsumptions[_selectedDevice][_selectedDayOfWeek]);
                    days.Add(Constants.DayOfWeek[_selectedDayOfWeek]);
                }
                // If user select week.
                else
                {
                    consumptions = _electricalConsumptions[_selectedDevice].ToList();
                    consumptions.RemoveAt(consumptions.Count - 1);
                    days = Constants.DayOfWeek.ToList();
                }
            }
            // If user select all devices
            else
            {
                // If user select day of week.
                if (_selectedDayOfWeek != DeviceDayOfWeekListBox.Items.Count - 1)
                {
                    consumptions.Add(_electricalConsumptions[_selectedDevice][_selectedDayOfWeek]);
                    days.Add(Constants.DayOfWeek[_selectedDayOfWeek]);
                }
                // If user select week.
                else
                {
                    consumptions = _electricalConsumptions[_selectedDevice].ToList();
                    consumptions.RemoveAt(consumptions.Count - 1);
                    days = Constants.DayOfWeek.ToList();
                }
            }

            ElectricalConsumptionChart(Chart, consumptions, days);
        }

        private void ElectricalConsumptionChart(Chart chart, List<double> consumptions, List<string> days)
        {
            ChartController.Clear(chart);
            string seriesName = DeviceTypeListBox.SelectedItem.ToString();
            ChartController.AddSeries(chart, seriesName,
                SeriesChartType.Column, Color.DarkOrchid);
            ChartController.AxisTitles(chart, "Дні", "Споживання, кВт·год");
            ChartController.Fill(chart, seriesName, days, consumptions);
        }

        private void SetElectricalConsumptionsTable()
        {
            List<ElectricalConsumptionRow> rows = _deviceController.ElectricalConsumptionTable;
            var dayOfWeek = Constants.DayOfWeek.ToList();
            dayOfWeek.Add("Всі");

            DataGrid.Columns.Clear();
            DataGrid.ItemsSource = rows;
            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Споживання, кВт·год",
                Binding = new Binding("DeviceName")
            };
            DataGrid.Columns.Add(column);
            
            for (int i = 0; i < dayOfWeek.Count; i++)
            {
                column = new DataGridTextColumn()
                {
                    Header = dayOfWeek[i],
                    Binding = new Binding($"WeekConsumptions[{i}]")
                };
                column.Binding.StringFormat = "0.####";
                DataGrid.Columns.Add(column);
            }
            
        }

        public void SaveCharts(List<string> paths)
        {
            Chart chart = new Chart();
            chart.ChartAreas.Add(new ChartArea("Default"));
            chart.Legends.Add(new Legend("Legend1"));
            chart.Legends[0].Enabled = false;

            var weekElectricalLoads = _electricalLoadsForDevices[_electricalLoadsForDevices.Count - 1];
            for (int selectedDayOfWeek = 0; selectedDayOfWeek <= 7; selectedDayOfWeek++)
            {
                var dayElectricalLoads = weekElectricalLoads[selectedDayOfWeek];
                ElectricalLoadChart(chart, dayElectricalLoads, selectedDayOfWeek);
                ChartController.SaveImage(chart, paths[selectedDayOfWeek]);
            }

            var electricalConsumptions = _deviceController.ElectricalConsumptions;
            List<double> consumptions = electricalConsumptions[electricalConsumptions.Count - 1].ToList();
            consumptions.RemoveAt(consumptions.Count - 1);
            List<string> days = Constants.DayOfWeek.ToList();
            ElectricalConsumptionChart(chart, consumptions, days);
            ChartController.SaveImage(chart, paths[8]);
        }
    }
}
