using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Controller
{
    public class ReportWordController : WordController, IReportController
    {
        public void FillWeatherConditions()
        {
            ReplaceTextAll("$currentPeriod$", Report.CurrentPeriod);
            PasteImage(Report.Weather.ImgPaths[0], "weatherTemperatureConditionPicture");
            PasteImage(Report.Weather.ImgPaths[1], "weatherDurationOfTemperaturePicture");
            PasteImage(Report.Weather.ImgPaths[2], "weatherWindsRosePicture");
            PasteImage(Report.Weather.ImgPaths[3], "weatherDurationWindOfActivityPicture");
            PasteImage(Report.Weather.ImgPaths[4], "weatherIntesityOfSolarInsolationPicture");
            PasteImage(Report.Weather.ImgPaths[5], "weatherDurationOfSolarActivityPicture");
        }

        public void FillElectricalLoadSchedule()
        {
            PasteText(Report.ComparePrices1Vs2Phases.ToString("0.##"), "elComparePricePhases1vs2");
            PasteText(Report.ComparePrices1Vs3Phases.ToString("0.##"), "elComparePricePhases1vs3");
            PasteText(Report.Opt2ComparePrices1Vs2Phases.ToString("0.##"), "elOpt2ComparePricePhases1vs2");
            PasteText(Report.Opt3ComparePrices1Vs3Phases.ToString("0.##"), "elOpt3ComparePricePhases1vs3");

            var els = Report.ElectricalLoadSchedule;
            PasteText(els.ElectricalPrices.OneKwhtPriceFor1PhaseLess100.ToString(), "elLoadCost1PhaseLess100");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor1PhaseMore100.ToString(), "elLoadCost1PhaseMore100");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor2Phase.ToString(), "elLoadCost2PhaseDay");
            double result = els.ElectricalPrices.OneKwhtPriceFor2Phase * els.ElectricalPrices.NightFactorFor2Phase;
            PasteText(result.ToString(), "elLoadCost2PhaseNight");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor3Phase.ToString(), "elLoadCost3PhaseBase");
            result = els.ElectricalPrices.OneKwhtPriceFor3Phase * els.ElectricalPrices.MaxLoadFactorFor3Phase;
            PasteText(result.ToString(), "elLoadCost3PhaseMax");
            result = els.ElectricalPrices.OneKwhtPriceFor3Phase * els.ElectricalPrices.NightFactorFor3Phase;
            PasteText(result.ToString(), "elLoadCost3PhaseNight");

            PasteImage(els.ImgPaths[0], "elsMondayPicture");
            PasteImage(els.ImgPaths[1], "elsTuesdayPicture");
            PasteImage(els.ImgPaths[2], "elsWednesdayPicture");
            PasteImage(els.ImgPaths[3], "elsThursdayPicture");
            PasteImage(els.ImgPaths[4], "elsFridayPicture");
            PasteImage(els.ImgPaths[5], "elsSaturdayPicture");
            PasteImage(els.ImgPaths[6], "elsSundayPicture");
            PasteImage(els.ImgPaths[7], "elsAlldayPicture");
            PasteImage(els.ImgPaths[8], "elsConsumptionPicture");

            PasteTable(els.DevicesTable, "elDevicesTable");
            PasteTable(els.DurationElectricalLoadsTables[0], "elDurMondayTable");
            PasteTable(els.DurationElectricalLoadsTables[1], "elDurTuesdayTable");
            PasteTable(els.DurationElectricalLoadsTables[2], "elDurWednesdayTable");
            PasteTable(els.DurationElectricalLoadsTables[3], "elDurThursdayTable");
            PasteTable(els.DurationElectricalLoadsTables[4], "elDurFridayTable");
            PasteTable(els.DurationElectricalLoadsTables[5], "elDurSaturdayTable");
            PasteTable(els.DurationElectricalLoadsTables[6], "elDurSundayTable");
            PasteTable(els.DurationElectricalLoadsTables[7], "elDurAlldayTable");
            PasteTable(els.ElectricalConsumptionsTable, "elConsumptionsTable");
            PasteTable(els.ElectricalRatesTable, "elRatesTable");

            PasteTable(els.ElectricalPrices1PhaseTable, "elPrices1PhaseTable");
            PasteTable(els.ElectricalPrices2PhaseTable, "elPrices2PhaseTable");
            PasteTable(els.ElectricalPrices3PhaseTable, "elPrices3PhaseTable");
        }

        public void FillOpt2ElectricalLoadSchedule()
        {
            var els = Report.Opt2ElectricalLoadSchedule;
            PasteText(els.ElectricalPrices.OneKwhtPriceFor1PhaseLess100.ToString(), "elLoadCost1PhaseLess100Opt2");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor1PhaseMore100.ToString(), "elLoadCost1PhaseMore100Opt2");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor2Phase.ToString(), "elLoadCost2PhaseDayOpt2");
            double result = els.ElectricalPrices.OneKwhtPriceFor2Phase * els.ElectricalPrices.NightFactorFor2Phase;
            PasteText(result.ToString(), "elLoadCost2PhaseNightOpt2");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor3Phase.ToString(), "elLoadCost3PhaseBaseOpt2");
            result = els.ElectricalPrices.OneKwhtPriceFor3Phase * els.ElectricalPrices.MaxLoadFactorFor3Phase;
            PasteText(result.ToString(), "elLoadCost3PhaseMaxOpt2");
            result = els.ElectricalPrices.OneKwhtPriceFor3Phase * els.ElectricalPrices.NightFactorFor3Phase;
            PasteText(result.ToString(), "elLoadCost3PhaseNightOpt2");

            PasteImage(els.ImgPaths[0], "elsOpt2MondayPicture");
            PasteImage(els.ImgPaths[1], "elsOpt2TuesdayPicture");
            PasteImage(els.ImgPaths[2], "elsOpt2WednesdayPicture");
            PasteImage(els.ImgPaths[3], "elsOpt2ThursdayPicture");
            PasteImage(els.ImgPaths[4], "elsOpt2FridayPicture");
            PasteImage(els.ImgPaths[5], "elsOpt2SaturdayPicture");
            PasteImage(els.ImgPaths[6], "elsOpt2SundayPicture");
            PasteImage(els.ImgPaths[7], "elsOpt2AlldayPicture");
            PasteImage(els.ImgPaths[8], "elsOpt2ConsumptionPicture");

            PasteTable(els.DurationElectricalLoadsTables[0], "elOpt2DurMondayTable");
            PasteTable(els.DurationElectricalLoadsTables[1], "elOpt2DurTuesdayTable");
            PasteTable(els.DurationElectricalLoadsTables[2], "elOpt2DurWednesdayTable");
            PasteTable(els.DurationElectricalLoadsTables[3], "elOpt2DurThursdayTable");
            PasteTable(els.DurationElectricalLoadsTables[4], "elOpt2DurFridayTable");
            PasteTable(els.DurationElectricalLoadsTables[5], "elOpt2DurSaturdayTable");
            PasteTable(els.DurationElectricalLoadsTables[6], "elOpt2DurSundayTable");
            PasteTable(els.DurationElectricalLoadsTables[7], "elOpt2DurAlldayTable");
            PasteTable(els.ElectricalConsumptionsTable, "elOpt2ConsumptionsTable");
            PasteTable(els.ElectricalRatesTable, "elOpt2RatesTable");

            PasteTable(els.ElectricalPrices1PhaseTable, "elOpt2Prices1PhaseTable");
            PasteTable(els.ElectricalPrices2PhaseTable, "elOpt2Prices2PhaseTable");
            PasteTable(els.ElectricalPrices3PhaseTable, "elOpt2Prices3PhaseTable");
        }

        public void FillOpt3ElectricalLoadSchedule()
        {
            var els = Report.Opt3ElectricalLoadSchedule;
            PasteText(els.ElectricalPrices.OneKwhtPriceFor1PhaseLess100.ToString(), "elLoadCost1PhaseLess100Opt3");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor1PhaseMore100.ToString(), "elLoadCost1PhaseMore100Opt3");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor2Phase.ToString(), "elLoadCost2PhaseDayOpt3");
            double result = els.ElectricalPrices.OneKwhtPriceFor2Phase * els.ElectricalPrices.NightFactorFor2Phase;
            PasteText(result.ToString(), "elLoadCost2PhaseNightOpt3");
            PasteText(els.ElectricalPrices.OneKwhtPriceFor3Phase.ToString(), "elLoadCost3PhaseBaseOpt3");
            result = els.ElectricalPrices.OneKwhtPriceFor3Phase * els.ElectricalPrices.MaxLoadFactorFor3Phase;
            PasteText(result.ToString(), "elLoadCost3PhaseMaxOpt3");
            result = els.ElectricalPrices.OneKwhtPriceFor3Phase * els.ElectricalPrices.NightFactorFor3Phase;
            PasteText(result.ToString(), "elLoadCost3PhaseNightOpt3");

            PasteImage(els.ImgPaths[0], "elsOpt3MondayPicture");
            PasteImage(els.ImgPaths[1], "elsOpt3TuesdayPicture");
            PasteImage(els.ImgPaths[2], "elsOpt3WednesdayPicture");
            PasteImage(els.ImgPaths[3], "elsOpt3ThursdayPicture");
            PasteImage(els.ImgPaths[4], "elsOpt3FridayPicture");
            PasteImage(els.ImgPaths[5], "elsOpt3SaturdayPicture");
            PasteImage(els.ImgPaths[6], "elsOpt3SundayPicture");
            PasteImage(els.ImgPaths[7], "elsOpt3AlldayPicture");
            PasteImage(els.ImgPaths[8], "elsOpt3ConsumptionPicture");

            PasteTable(els.DurationElectricalLoadsTables[0], "elOpt3DurMondayTable");
            PasteTable(els.DurationElectricalLoadsTables[1], "elOpt3DurTuesdayTable");
            PasteTable(els.DurationElectricalLoadsTables[2], "elOpt3DurWednesdayTable");
            PasteTable(els.DurationElectricalLoadsTables[3], "elOpt3DurThursdayTable");
            PasteTable(els.DurationElectricalLoadsTables[4], "elOpt3DurFridayTable");
            PasteTable(els.DurationElectricalLoadsTables[5], "elOpt3DurSaturdayTable");
            PasteTable(els.DurationElectricalLoadsTables[6], "elOpt3DurSundayTable");
            PasteTable(els.DurationElectricalLoadsTables[7], "elOpt3DurAlldayTable");
            PasteTable(els.ElectricalConsumptionsTable, "elOpt3ConsumptionsTable");
            PasteTable(els.ElectricalRatesTable, "elOpt3RatesTable");

            PasteTable(els.ElectricalPrices1PhaseTable, "elOpt3Prices1PhaseTable");
            PasteTable(els.ElectricalPrices2PhaseTable, "elOpt3Prices2PhaseTable");
            PasteTable(els.ElectricalPrices3PhaseTable, "elOpt3Prices3PhaseTable");
        }

        public void FillHouseHeating()
        {
            PasteText(Report.ThermalEnergy.CommonBuildTemperature.ToString("0.#"), "commonBuildTemperature");
            PasteText(Report.ThermalEnergy.CalcOutsideTemperature.ToString("0.#"), "calcOutsideTemperature");
            PasteText(Report.ThermalEnergy.CalcOutsideTemperature.ToString("0.#"), "calcOutsideTemperature2");

            PasteTable(Report.ThermalEnergy.WaterIncomeParamsTable, "teWaterIncomeParamsTable");
            PasteTable(Report.ThermalEnergy.WaterOutcomeParamsTable, "teWaterOutcomeParamsTable");
            PasteTable(Report.ThermalEnergy.HouseThermalParamsTable, "houseThermalParamsTable");

            // Common build heating scheme.
            PasteTable(Report.ThermalEnergy.CommonScheme.EtalonHeatLossesTable, "commonEtalonHeatLossesTable");
            PasteImage(Report.ThermalEnergy.CommonImgPaths[0], "commonHeatLossesPicture");
            PasteText(Report.ThermalEnergy.CommonScheme.TotalHeatConsumption.ToString("0.##"), "commonTotalHeatEnergy");
            PasteImage(Report.ThermalEnergy.CommonImgPaths[1], "commonEnergyLossesPicture");
            PasteText(Report.ThermalEnergy.CommonScheme.TotalHeatHelConsumption.ToString("0.##"), "commonTotalHeatHelEnergy");
            PasteImage(Report.ThermalEnergy.CommonImgPaths[2], "commonHeatCostPicture");
            PasteTable(Report.ThermalEnergy.CommonScheme.HeatingTypesTable, "commonHeatingTypesTable");
            PasteTable(Report.ThermalEnergy.CommonScheme.HeatingCostsTable, "commonHeatingCostsTable");

            // Individual build heating scheme.
            PasteTable(Report.ThermalEnergy.IndividualTemperatureModesTable, "indTemperatureModesTable");
            PasteTable(Report.ThermalEnergy.IndividualScheme.EtalonHeatLossesTable, "indEtalonHeatLossesTable");
            PasteImage(Report.ThermalEnergy.IndividualImgPaths[0], "indHeatLossesPicture");
            PasteText(Report.ThermalEnergy.IndividualScheme.TotalHeatConsumption.ToString("0.##"), "indTotalHeatEnergy");
            PasteImage(Report.ThermalEnergy.IndividualImgPaths[1], "indEnergyLossesPicture");
            PasteText(Report.ThermalEnergy.IndividualScheme.TotalHeatHelConsumption.ToString("0.##"), "indTotalHeatHelEnergy");
            PasteImage(Report.ThermalEnergy.IndividualImgPaths[2], "indHeatCostPicture");
            PasteTable(Report.ThermalEnergy.IndividualScheme.HeatingTypesTable, "indHeatingTypesTable");
            PasteTable(Report.ThermalEnergy.IndividualScheme.HeatingCostsTable, "indHeatingCostsTable");

            PasteText(Report.ThermalEnergy.CompareIndVsCommon, "compareCommonVsInd");
        }

        public void FillHeatPump()
        {
            PasteText(Report.HeatPump.NominalHeatProduction.ToString("0.##"), "hpNominalHeatProduction");
            PasteText(Report.HeatPump.NominalPower.ToString("0.##"), "hpNominalPower");
            PasteText(Report.HeatPump.HeatPumpCount.ToString(), "hpHeatPumpCount");
            PasteText(Report.HeatPump.CirculationPower.ToString("0.##"), "hpCirculationPumpPower");
            PasteText(Report.HeatPump.CirculationPumpCount.ToString(), "hpCirculationPumpCount");
            PasteText(Report.HeatPump.FancoilPower.ToString("0.##"), "hpFancoilPower");
            PasteText(Report.HeatPump.FancoilCount.ToString(), "hpFancoilCount");

            PasteTable(Report.HeatPump.HeatPumpCharacteristicTable, "heatPumpCharacteristicTable");
            PasteImage(Report.HeatPump.ImgPaths[0], "hpHeatProductionCorrectionPicture");
            PasteImage(Report.HeatPump.ImgPaths[1], "hpHeatPowerCorrectionPicture");
            PasteTable(Report.HeatPump.HeatPumpCalculationsTable, "heatPumpCalculationsTable");
            PasteImage(Report.HeatPump.ImgPaths[2], "hpHeatCostPicture");
            PasteTable(Report.HeatPump.HeatPumpCostTable, "heatPumpCostTable");

            PasteText(Report.HeatPump.TotalHeatLosses.ToString("0.##"), "hpTotalHeatLosses");
            PasteText(Report.HeatPump.TotalHeatPumpConsumption.ToString("0.##"), "hpTotalHeatPumpConsumption");
            PasteText(Report.HeatPump.TotalHeatSystemConsumption.ToString("0.##"), "hpTotalHeatSystemConsumption");
            PasteText(Report.HeatPump.TotalQuantityHeatPumpProduction.ToString("0.##"), "hpTotalQuantityHeatPumpProd");
            PasteText(Report.HeatPump.TotalQuantityAdditionalHeatProduction.ToString("0.##"), "hpTotalQuantityAddHeatProd");
            PasteText(Report.HeatPump.TotalQuantityHeatSystemProduction.ToString("0.##"), "hpTotalQuantityHeatSystemProd");
            PasteText(Report.HeatPump.AverageEfficiencyHeatPump.ToString("0.##"), "hpAverageEfficiencyHeatPump");
            PasteText(Report.HeatPump.AverageEfficiencyHeatSystem.ToString("0.##"), "hpAverageEfficiencyHeatSystem");
            PasteText(Report.HeatPump.TotalCost.ToString("0.##"), "hpTotalCost");
        }

        public void FillHeatStore()
        {
            PasteText(Report.HeatStore.StartDayZone.ToString(@"hh\:mm"), "hsStartTime");
            PasteText(Report.HeatStore.EndDayZone.ToString(@"hh\:mm"), "hsEndTime");
            PasteText(Report.HeatStore.DayRate.ToString("0.##"), "hsDayRate");
            PasteText(Report.HeatStore.NightRate.ToString("0.##"), "hsNightRate");
            PasteText(Report.HeatStore.Capacity.ToString("0.##"), "hsMaxHeatCapacity");
            PasteText(Report.HeatStore.Power.ToString("0.##"), "hsHeatPower");

            PasteImage(Report.HeatStore.ImgPaths[0], "hsModelingPicture");
            PasteImage(Report.HeatStore.ImgPaths[1], "hsHeatingCostPicture");
            PasteTable(Report.HeatStore.CostTable, "hsHeatingCostTable");

            PasteText(Report.HeatStore.TotalDayConsumption.ToString("0.##"), "hsTotalDayConsumption");
            PasteText(Report.HeatStore.TotalDayCost.ToString("0.##"), "hsTotalDayCost");
            PasteText(Report.HeatStore.TotalNightConsumption.ToString("0.##"), "hsTotalNightConsumption");
            PasteText(Report.HeatStore.TotalNightCost.ToString("0.##"), "hsTotalNightCost");
            PasteText(Report.HeatStore.TotalConsumption.ToString("0.##"), "hsTotalConsumption");
            PasteText(Report.HeatStore.TotalCost.ToString("0.##"), "hsTotalCost");
            PasteText(Report.HeatStore.WorstDay.ToString("dd.MM.yyyy"), "hsWorstDay");
            PasteText(Report.HeatStore.EnergyLack.ToString("0.##"), "hsEnergyLack");
            PasteText(Report.HeatStore.HourLack.ToString("0.##"), "hsHourLack");
        }

        public void FillWindEnergy()
        {
            PasteImage(Report.Wind.ImgPaths[0], "windEnergyCharacteristicPicture");
            PasteTable(Report.Wind.WindGeneratorCharacteristicsTable, "windGenCharTable");
            PasteText(Report.Wind.TowerHeight.ToString(), "windTowerHeight");
            PasteImage(Report.Wind.ImgPaths[1], "windEnergyGeneratingPicture");
            PasteTable(Report.Wind.WindGenerationsTable, "windGenTable");
            PasteText(Report.Wind.WindTotalEnergyGen.ToString("0.##"), "windTotalEnergyGen");

            PasteText(Report.Wind.GreenPrice.ToString("0.####"), "greenPrice");
            PasteText(Report.Wind.ReducedPollutionPrice.ToString("0.##"), "reducedPollutionPrice");
            PasteText(Report.Wind.Co2ReducedPollution.ToString("0.##"), "cO2reducedPollution");
            PasteText(Report.Wind.EnergyCost.ToString("0.##"), "energyCost");
            PasteText(Report.Wind.ReducedPollutionCost.ToString("0.##"), "reducedPollutionCost");
        }
    }
}
