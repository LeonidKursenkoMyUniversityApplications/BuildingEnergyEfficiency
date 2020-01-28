namespace SmartHouse.DAL.Model
{
    public class ElectricalPrices
    {
        public double OneKwhtPriceFor1PhaseLess100 { set; get; }
        public double OneKwhtPriceFor1PhaseMore100 { set; get; }

        public double CostFor1PhaseLess100 { set; get; }
        public double CostFor1PhaseMore100 { set; get; }
        public double TotalCostFor1Phase { set; get; }

        public double ConsumptionFor1PhaseLess100 { set; get; }
        public double ConsumptionFor1PhaseMore100 { set; get; }
        public double TotalConsumptionFor1Phase { set; get; }


        public double OneKwhtPriceFor2Phase { set; get; }
        public double NightFactorFor2Phase { set; get; }
        public double DayFactorFor2Phase { set; get; }

        public double NightCost2Phase { set; get; }
        public double DayCost2Phase { set; get; }
        public double TotalCostFor2Phase { set; get; }

        public double NightConsumptionFor2Phase { set; get; }
        public double DayConsumptionFor2Phase { set; get; }
        public double TotalConsumptionFor2Phase { set; get; }


        public double OneKwhtPriceFor3Phase { set; get; }
        public double NightFactorFor3Phase { set; get; }
        public double MaxLoadFactorFor3Phase { set; get; }
        public double HalfMaxLoadFactorFor3Phase { set; get; }

        public double NightCostFor3Phase { set; get; }
        public double MaxLoadCostFor3Phase { set; get; }
        public double HalfMaxLoadCostFor3Phase { set; get; }
        public double TotalCostFor3Phase { set; get; }

        public double NightConsumptionFor3Phase { set; get; }
        public double MaxLoadConsumptionFor3Phase { set; get; }
        public double HalfMaxLoadConsumptionFor3Phase { set; get; }
        public double TotalConsumptionFor3Phase { set; get; }
        
        public ElectricalPrices Copy()
        {
            return new ElectricalPrices
            {
                OneKwhtPriceFor1PhaseLess100 = OneKwhtPriceFor1PhaseLess100,
                OneKwhtPriceFor1PhaseMore100 = OneKwhtPriceFor1PhaseMore100,
                CostFor1PhaseLess100 = CostFor1PhaseLess100,
                CostFor1PhaseMore100 = CostFor1PhaseMore100,
                TotalCostFor1Phase = TotalCostFor1Phase,
                ConsumptionFor1PhaseLess100 = ConsumptionFor1PhaseLess100,
                ConsumptionFor1PhaseMore100 = ConsumptionFor1PhaseMore100,
                TotalConsumptionFor1Phase = TotalConsumptionFor1Phase,
                OneKwhtPriceFor2Phase = OneKwhtPriceFor2Phase,
                NightFactorFor2Phase = NightFactorFor2Phase,
                DayFactorFor2Phase = DayFactorFor2Phase,
                NightCost2Phase = NightCost2Phase,
                DayCost2Phase = DayCost2Phase,
                TotalCostFor2Phase = TotalCostFor2Phase,
                NightConsumptionFor2Phase = NightConsumptionFor2Phase,
                DayConsumptionFor2Phase = DayConsumptionFor2Phase,
                TotalConsumptionFor2Phase = TotalConsumptionFor2Phase,
                OneKwhtPriceFor3Phase = OneKwhtPriceFor3Phase,
                NightFactorFor3Phase = NightFactorFor3Phase,
                MaxLoadFactorFor3Phase = MaxLoadFactorFor3Phase,
                HalfMaxLoadFactorFor3Phase = HalfMaxLoadFactorFor3Phase,
                NightCostFor3Phase = NightCostFor3Phase,
                MaxLoadCostFor3Phase = MaxLoadCostFor3Phase,
                HalfMaxLoadCostFor3Phase = HalfMaxLoadCostFor3Phase,
                TotalCostFor3Phase = TotalCostFor3Phase,
                NightConsumptionFor3Phase = NightConsumptionFor3Phase,
                MaxLoadConsumptionFor3Phase = MaxLoadConsumptionFor3Phase,
                HalfMaxLoadConsumptionFor3Phase = HalfMaxLoadConsumptionFor3Phase,
                TotalConsumptionFor3Phase = TotalConsumptionFor3Phase
            };
        }
    }
}
