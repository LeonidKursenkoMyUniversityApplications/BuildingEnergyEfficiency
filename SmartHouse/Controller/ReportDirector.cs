using SmartHouse.DAL.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.Controller
{
    public class ReportDirector
    {
        public void Build(IReportController reportController, string fileName, BackgroundWorker worker)
        {
            reportController.InitDocument();
            worker?.ReportProgress(2);
            reportController.FillWeatherConditions();
            worker?.ReportProgress(10);
            reportController.FillElectricalLoadSchedule();
            worker?.ReportProgress(20);
            reportController.FillOpt2ElectricalLoadSchedule();
            worker?.ReportProgress(25);
            reportController.FillOpt3ElectricalLoadSchedule();
            worker?.ReportProgress(30);
            reportController.FillHouseHeating();
            worker?.ReportProgress(50);
            reportController.FillHeatPump();
            worker?.ReportProgress(65);
            reportController.FillWindEnergy();
            worker?.ReportProgress(85);
            reportController.FillHeatStore();
            worker?.ReportProgress(96);
            reportController.SaveDocument(fileName);
            worker?.ReportProgress(99);
            reportController.DeleteTempFiles();
            worker?.ReportProgress(100);
        }
    }
}
