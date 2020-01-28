using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartHouse.Controller;
using SmartHouse.DAL.Controller;

namespace SmartHouse.Controls
{
    /// <summary>
    /// Логика взаимодействия для ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        private IReportController _reportController;
        private string _fileName;
        public ProgressBarWindow(IReportController reportController, string fileName)
        {
            InitializeComponent();
            _reportController = reportController;
            _fileName = fileName;
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ReportDirector reportDirector = new ReportDirector();
            reportDirector.Build(_reportController, _fileName, worker);
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StatusProgressBar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Відбулася помилка");
            }
            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }
    }
}
