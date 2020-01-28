using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Windows.Forms.DataVisualization.Charting;
using SmartHouse.Controller;
using SmartHouse.DAL.Controller;
using SmartHouse.DAL.Model;
using SmartHouse.BLL.Controller;
using SmartHouse.BLL.Model;
using SmartHouse.Controls;
using SmartHouse.Controls.MeteorogicalAnalysis;
using Binding = System.Windows.Data.Binding;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;

namespace SmartHouse
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _sunFile;
        private List<string> _weatherFiles;
        //private readonly string _fileWeather = @"киев+\2012-";
        //private readonly string _fileSun = @"киев+\soldata";
        private readonly string _fileSource = @"data\source_path";
        private readonly string _fileDevice = @"data\devices";
        private readonly string _fileHouse = @"data\house";
        private readonly string _fileWindEnergy = @"data\wind_generator";
        private readonly string _fileHeatPump = @"data\heat_pump";
        private readonly string _helpPath = @"data\Довідка.pdf";
        #region Set data about paths for report.
        private readonly string _imagesDirectory = System.IO.Path.GetFullPath(@"saves\img");
        private readonly List<string> _weatherChartPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\temperature_conditions.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\duration_of_temperature.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\winds_rose.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\duration_of_wind_activity.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\intensity_of_solar_insolation.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\duration_of_solar_activity.bmp")
        };

        private readonly List<string> _elLoadChartPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\ELS_monday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_tuesday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_wednesday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_thursday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_friday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_saturday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_sunday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_all.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELS_consume.bmp")
        };

        private readonly List<string> _elLoadOpt2ChartPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_monday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_tuesday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_wednesday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_thursday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_friday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_saturday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_sunday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_all.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt2_consume.bmp")
        };

        private readonly List<string> _elLoadOpt3ChartPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_monday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_tuesday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_wednesday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_thursday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_friday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_saturday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_sunday.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_all.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\ELSOpt3_consume.bmp")
        };

        private readonly List<string> _commonHeatingPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\common_heat_losses.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\common_energy_losses.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\common_heat_cost.bmp")
        };

        private readonly List<string> _individualHeatingPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\individual_heat_losses.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\individual_energy_losses.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\individual_heat_cost.bmp")
        };

        private readonly List<string> _windEnergyPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\wind_energy_characteristic.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\wind_energy_generating.bmp")
        };

        private readonly List<string> _heatPumpPaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\hp_heat_production_correction.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\hp_heat_power_correction.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\hp_heat_cost.bmp")
        };

        private readonly List<string> _heatStorePaths = new List<string>()
        {
            System.IO.Path.GetFullPath(@"saves\img\hs_heat_store.bmp"),
            System.IO.Path.GetFullPath(@"saves\img\hs_heat_cost.bmp")
        };
        #endregion
        private ConvertController _convertController = new ConvertController();

        private WeatherController _weatherController;
        private SunConditionController _sunConditionController;

        private DeviceController _deviceController;

        private HouseController _houseController;
        private WindEnergyController _windEnergyController;
        private HeatPumpController _heatPumpController;
        private HeatStoreController _heatStoreController;

        public MainWindow()
        {
            //StartActions();
            InitializeComponent();
            
            //MessageBox.Show(_weatherController.CorrrectsCounter.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartActions();
        }

        private void StartActions()
        {
            MainTabControl.IsEnabled = false;
            var sources = BinaryController.ReadDataFromBinary<Pathes>(_fileSource);
            if (sources.Count >= 1)
            {
                Pathes pathes = sources[0];
                _weatherFiles = pathes.WeatherFiles;
                _sunFile = pathes.SunFile;
            }
            if (_weatherFiles == null)
            {
                ChooseData();
            }
            else
            {
                InitData();
            }
            MainTabControl.IsEnabled = true;
        }

        private void InitData()
        {
            // Task 1
            _weatherController = new WeatherController(_weatherFiles);
            _sunConditionController = new SunConditionController(System.IO.Path.GetFullPath(_sunFile));
            MeteorogicalAnalysisView.Init(_weatherController, _sunConditionController);
            Calculate();
            MeteorogicalAnalysisView.TimePeriodChanged += Calculate;
            
        }

        public void Calculate()
        {
            // Task 2
            _deviceController = new DeviceController(_fileDevice);
            ElectricalLoadScheduleView.Init(_deviceController);

            // Task3
            _houseController = new HouseController(_fileHouse);
            ThermalEnergy.Init(_houseController, _weatherController);

            // Task4
            _windEnergyController = new WindEnergyController(_fileWindEnergy);
            WindEnergyView.Init(_windEnergyController, _weatherController);

            // Task5
            _heatPumpController = new HeatPumpController(_fileHeatPump);
            HeatPumpView.Init(_heatPumpController, _houseController, _deviceController);

            // Task 6
            _heatStoreController = new HeatStoreController();
            HeatStoreView.Init(_heatStoreController, _houseController);
        }
        
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Збереження";
            saveFileDialog.FileName = "Звіт"; 
            saveFileDialog.DefaultExt = ".docx"; 
            saveFileDialog.Filter = "Word files (.doc)|*.docx";
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory + @"\saves";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReportController reportController = new ReportController
                {
                    WeatherController = _weatherController,
                    SunConditionController = _sunConditionController,
                    DeviceController = _deviceController,
                    DeviceOpt2Controller = ElectricalLoadScheduleView.OptimizationView.Device2ZoneOptController,
                    DeviceOpt3Controller = ElectricalLoadScheduleView.OptimizationView.Device3ZoneOptController,
                    HouseController = _houseController,
                    WindEnergyController = _windEnergyController,
                    HeatPumpController = _heatPumpController,
                    HeatStoreController = _heatStoreController
                };
                reportController.CopyData();
                reportController.Report.ImgDirectory = _imagesDirectory;
                MeteorogicalAnalysisView.SaveCharts(_weatherChartPaths);
                reportController.Report.Weather.ImgPaths = _weatherChartPaths;

                ElectricalLoadScheduleView.SaveCharts(_elLoadChartPaths, _elLoadOpt2ChartPaths, _elLoadOpt3ChartPaths);
                reportController.Report.ElectricalLoadSchedule.ImgPaths = _elLoadChartPaths;
                reportController.Report.Opt2ElectricalLoadSchedule.ImgPaths = _elLoadOpt2ChartPaths;
                reportController.Report.Opt3ElectricalLoadSchedule.ImgPaths = _elLoadOpt3ChartPaths;

                reportController.Report.ThermalEnergy.CommonImgPaths = _commonHeatingPaths;
                reportController.Report.ThermalEnergy.IndividualImgPaths = _individualHeatingPaths;
                ThermalEnergy.SaveCharts(_commonHeatingPaths, _individualHeatingPaths);

                reportController.Report.Wind.ImgPaths = _windEnergyPaths;
                WindEnergyView.SaveCharts(_windEnergyPaths);

                reportController.Report.HeatPump.ImgPaths = _heatPumpPaths;
                HeatPumpView.SaveCharts(_heatPumpPaths);

                reportController.Report.HeatStore.ImgPaths = _heatStorePaths;
                HeatStoreView.SaveCharts(_heatStorePaths);

                ReportWordController reportWordController = new ReportWordController
                {
                    Report = reportController.Report
                };

                string path = System.IO.Path.GetFullPath(saveFileDialog.FileName);
                ProgressBarWindow progressBarWindow = new ProgressBarWindow(reportWordController, path);
                progressBarWindow.Show();
                
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutBox1 aboutBox1 = new AboutBox1();
            aboutBox1.Show();
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(_helpPath);
        }

        private void ChooseDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChooseData();
        }

        private void ChooseData()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Виберіть каталог метеорологічних даних регіону";
            folderDialog.SelectedPath = @"киев+\"; 
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderDialog.SelectedPath;
                List<string> files = Directory.GetFiles(path, "*.xlsx").ToList();
                for (int i = 0; i < files.Count; i++)
                {
                    files[i] = files[i].Replace(Directory.GetCurrentDirectory(), ".").Replace(".xlsx", "");
                }
                _sunFile = files.Last(x => x.Contains("soldata"));
                files.Remove(_sunFile);
                files.Sort();
                _weatherFiles = files;
                ConvertProgressBarWindow progressBarWindow = new ConvertProgressBarWindow(_weatherFiles, _sunFile);
                progressBarWindow.Show();
                Pathes pathes = new Pathes
                {
                    SunFile = _sunFile,
                    WeatherFiles = _weatherFiles
                };
                BinaryController.WriteDataToBinary(_fileSource, new List<Pathes> {pathes});
                MeteorogicalAnalysisView.TimePeriodChanged -= Calculate;
                //InitData();
                // Task 1
                _weatherController = new WeatherController(_weatherFiles);
                //_weatherController.CorrectWeather();
                _sunConditionController = new SunConditionController(System.IO.Path.GetFullPath(_sunFile));
                //_sunConditionController.CorrectData(_weatherController.Year);
                MeteorogicalAnalysisView.Init(_weatherController, _sunConditionController);
                Calculate();
                MeteorogicalAnalysisView.TimePeriodChanged += Calculate;
            }
            else
            {
                Close();
            }
        }
    }
}
