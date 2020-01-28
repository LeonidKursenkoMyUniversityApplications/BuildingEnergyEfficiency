using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Controller
{
    public interface IReportController : IDocumentController
    {
        void FillWeatherConditions();
        void FillElectricalLoadSchedule();
        void FillOpt2ElectricalLoadSchedule();
        void FillOpt3ElectricalLoadSchedule();
        void FillHouseHeating();
        void FillHeatPump();
        void FillHeatStore();
        void FillWindEnergy();
    }
}
