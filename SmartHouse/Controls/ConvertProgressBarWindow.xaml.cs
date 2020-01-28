using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using SmartHouse.BLL.Controller;
using SmartHouse.DAL.Controller;

namespace SmartHouse.Controls
{
    /// <summary>
    /// Логика взаимодействия для ConvertProgressBarWindow.xaml
    /// </summary>
    public partial class ConvertProgressBarWindow : Window
    {
        private ConvertController _convertController;
        public WeatherController WeatherController;
        public SunConditionController SunConditionController;
        private List<string> _weatherFiles;
        private string _sunFile;
        public ConvertProgressBarWindow(List<string> weatherFiles, string sunFile)
        {
            InitializeComponent();
            _weatherFiles = weatherFiles;
            _sunFile = sunFile;
            _convertController = new ConvertController();
        }

        public void Work()
        {
            StatusProgressBar.Value = 1;
            foreach (var weatherFile in _weatherFiles)
            {
                _convertController.GetWeatherExcel(weatherFile);
                StatusProgressBar.Value += 100 / 13;
            }
            WeatherController = new WeatherController(_weatherFiles);
            var sunConditions = _convertController.GetSunConditionsFromExcel(_sunFile);
            SunConditionController = new SunConditionController(sunConditions, WeatherController.Year);
            BinaryController.WriteDataToBinary(_sunFile, SunConditionController.SunConditions);
            StatusProgressBar.Value = 100;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Work();
        }
    }
}
