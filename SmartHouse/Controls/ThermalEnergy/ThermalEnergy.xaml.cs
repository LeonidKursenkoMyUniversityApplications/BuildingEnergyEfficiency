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
using SmartHouse.Controller;
using SmartHouse.DAL.Model;

namespace SmartHouse.Controls.ThermalEnergy
{
    /// <summary>
    /// Логика взаимодействия для ThermalEnergy.xaml
    /// </summary>
    public partial class ThermalEnergy : UserControl
    {
        private HouseController _houseController;
        private WeatherController _weatherController;
        private House _house;

        private int _selectedFloor = 0;
        private int _selectedRoom = 0;
        private int _selectedWall = 0;

        public ThermalEnergy()
        {
            InitializeComponent();
        }

        public void Init(HouseController houseController, WeatherController weatherController)
        {
            _houseController = houseController;
            _house = _houseController.House;

            _weatherController = weatherController;

            FloorsListBox.SelectionChanged -= FloorsListBox_SelectionChanged;
            FloorsListBox.ItemsSource = _house.Floors.Select(x => x.Name);
            FloorsListBox.SelectedIndex = _selectedFloor;
            FloorsListBox.SelectionChanged += FloorsListBox_SelectionChanged;

            RoomsListBox.ItemsSource = _house.Floors[_selectedFloor].Rooms.Select(x => x.Name);
            RoomsListBox.SelectedIndex = _selectedRoom;

            SetTable();
            HouseDataGrid.ItemsSource = _house.HouseParams;
            HouseTemperatureTextBox.Text = _house.Temperature.ToString();

            InitHel();
            HeatView1.Init(_houseController, _weatherController, HeatType.Common);
            HeatView2.Init(_houseController, _weatherController, HeatType.Individual);
        }

        private void InitHel()
        {
            HydroElectricLoad hel = _house.Hel;
            ShowerUsersTextBox.Text = hel.NumberOfShowerUsers.ToString();
            BathUsersTextBox.Text = hel.NumberOfBathUsers.ToString();
            WaterQuantityForShowerTextBox.Text = hel.WaterQuantityForShower.ToString();
            WaterQuantityForBathTextBox.Text = hel.WaterQuantityForBath.ToString();
            TemperatureForShowerTextBox.Text = hel.TemperatureOfShower.ToString();
            TemperatureForBathTextBox.Text = hel.TemperatureOfBath.ToString();
            TemperatureForInput.Text = hel.TemperatureOfInput.ToString();
            TemperatureForOutput.Text = hel.TemperatureOfOutput.ToString();
            TimeWaterHeatingTextBox.Text = hel.Time.ToString();

            ShowerWaterConsumptionTextBox.Text = hel.ShowerWaterConsumption.ToString();
            BathWaterConsumptionTextBox.Text = hel.BathWaterConsumption.ToString();
            CorrectedShowerWaterConsumptionTextBox.Text = hel.CorrectedShowerWaterConsumption.ToString();
            CorrectedBathWaterConsumptionTextBox.Text = hel.CorrectedBathWaterConsumption.ToString();
            TotalConsumptionTextBox.Text = hel.TotalWaterConsumption.ToString();
            EnergyTextBox.Text = hel.Energy.ToString();
            PowerTextBox.Text = hel.Power.ToString();
        }

        private void FloorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FloorsListBox.SelectedIndex != -1)
                _selectedFloor = FloorsListBox.SelectedIndex;
            RoomsListBox.SelectionChanged -= RoomsListBox_SelectionChanged;
            RoomsListBox.ItemsSource = _house.Floors[_selectedFloor].Rooms.Select(x => x.Name);
            _selectedRoom = 0;
            RoomsListBox.SelectionChanged += RoomsListBox_SelectionChanged;
            RoomsListBox.SelectedIndex = _selectedRoom;
            //Select();
        }

        private void RoomsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomsListBox.SelectedIndex != -1)
                _selectedRoom = RoomsListBox.SelectedIndex;
            Select();
        }

        private void Select()
        {
            Room room = _house.Floors[_selectedFloor].Rooms[_selectedRoom];
            RoomTemperatureTextBox.Text = room.Temperature.ToString();
            RoomAreaTextBox.Text = room.Area.ToString();
            WindowsAreaTextBox.Text = room.WindowsArea.ToString();
            TotalRoomAreaTextBox.Text = room.TotalArea.ToString();
            DataGrid.ItemsSource = null;
            SetTable();
        }

        private void SetTable()
        {
            List<Wall> walls = _house.Floors[_selectedFloor].Rooms[_selectedRoom].Walls;
            DataGrid.ItemsSource = walls;
            DataGrid.Columns.Clear();
            DataGridTextColumn column = new DataGridTextColumn
            {
                Header = "Положення стіни",
                Binding = new Binding("GetWallType"),
                IsReadOnly = true
            };
            DataGrid.Columns.Add(column);

            column = new DataGridTextColumn
            {
                Header = "Площа стіни, м²",
                Binding = new Binding("Area")
            };
            DataGrid.Columns.Add(column);
        }

        private void SaveRoomButton_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Бажаєте зберегти зміни?", "Збереження", MessageBoxButton.YesNo);
            if(res == MessageBoxResult.No) return;
            Room room = _house.Floors[_selectedFloor].Rooms[_selectedRoom];
            double area, windowsArea, roomTemp, houseTemp;
            try
            {
                area = Transformer.ToDouble(RoomAreaTextBox.Text);
                windowsArea = Transformer.ToDouble(WindowsAreaTextBox.Text);
                roomTemp = Transformer.ToDouble(RoomTemperatureTextBox.Text);
                houseTemp = Transformer.ToDouble(HouseTemperatureTextBox.Text);
            }
            catch
            {
                return;
            }
            _house.Temperature = houseTemp;
            room.Temperature = roomTemp;
            room.Area = area;
            room.WindowsArea = windowsArea;
            TotalRoomAreaTextBox.Text = room.TotalArea.ToString();
            _houseController.Save();
        }

        private void HelButton_Click(object sender, RoutedEventArgs e)
        {
            HydroElectricLoad hel = _house.Hel;
            try
            {
                hel.NumberOfShowerUsers = Transformer.ToInt(ShowerUsersTextBox.Text);
                hel.NumberOfBathUsers = Transformer.ToInt(BathUsersTextBox.Text);
                hel.WaterQuantityForShower = Transformer.ToDouble(WaterQuantityForShowerTextBox.Text);
                hel.WaterQuantityForBath = Transformer.ToDouble(WaterQuantityForBathTextBox.Text);
                hel.TemperatureOfShower = Transformer.ToDouble(TemperatureForShowerTextBox.Text);
                hel.TemperatureOfBath = Transformer.ToDouble(TemperatureForBathTextBox.Text);
                hel.TemperatureOfInput = Transformer.ToDouble(TemperatureForInput.Text);
                hel.TemperatureOfOutput = Transformer.ToDouble(TemperatureForOutput.Text);
                hel.Time = Transformer.ToDouble(TimeWaterHeatingTextBox.Text);
            }
            catch
            {
                return;
            }

            ShowerWaterConsumptionTextBox.Text = hel.ShowerWaterConsumption.ToString();
            BathWaterConsumptionTextBox.Text = hel.BathWaterConsumption.ToString();
            CorrectedShowerWaterConsumptionTextBox.Text = hel.CorrectedShowerWaterConsumption.ToString();
            CorrectedBathWaterConsumptionTextBox.Text = hel.CorrectedBathWaterConsumption.ToString();
            TotalConsumptionTextBox.Text = hel.TotalWaterConsumption.ToString();
            EnergyTextBox.Text = hel.Energy.ToString();
            PowerTextBox.Text = hel.Power.ToString();
            _houseController.Save();
        }

        public void SaveCharts(List<string> commonImgPaths, List<string> individualImgPaths)
        {
            HeatView1.SaveCharts(commonImgPaths);
            HeatView2.SaveCharts(individualImgPaths);
        }
    }
}
