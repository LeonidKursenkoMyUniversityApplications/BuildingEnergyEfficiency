using System;

namespace SmartHouse.DAL.Model.Report
{
    public class Report
    {
        public string ImgDirectory { set; get; }
        public DateTime StartDateTime { set; get; }
        public DateTime EndDateTime { set; get; }

        public string CurrentPeriod
        {
            get
            {
                return "з " + StartDateTime.ToString("dd.MM.yyyy HH:mm") + " по " + EndDateTime.ToString("dd.MM.yyyy HH:mm");
            }
        }

        public string DateTimeFormat
        {
            get
            {
                if (StartDateTime.Year == EndDateTime.Year)
                {
                    if (StartDateTime.Month == EndDateTime.Month &&
                        StartDateTime.Day == EndDateTime.Day)
                        return "HH:mm";
                }
                return "MM.dd.yyyy HH:mm";
            }
        }

        public WeatherConditionReport Weather { set; get; }
        public ElectricalLoadScheduleReport ElectricalLoadSchedule { set; get; }
        public ElectricalLoadScheduleReport Opt2ElectricalLoadSchedule { set; get; }
        public ElectricalLoadScheduleReport Opt3ElectricalLoadSchedule { set; get; }
        public double ComparePrices1Vs2Phases { set; get; }
        public double ComparePrices1Vs3Phases { set; get; }
        public double Opt2ComparePrices1Vs2Phases { set; get; }
        public double Opt3ComparePrices1Vs3Phases { set; get; }
        public ThermalEnergyReport ThermalEnergy { set; get; }
        public WindReport Wind { set; get; }
        public HeatPumpReport HeatPump { set; get; }
        public HeatStoreReport HeatStore { set; get; }
    }
}
