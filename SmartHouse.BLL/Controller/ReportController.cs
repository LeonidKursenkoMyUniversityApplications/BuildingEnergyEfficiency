using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SmartHouse.BLL.Model;
using SmartHouse.DAL.Model;
using SmartHouse.DAL.Model.HeatPump;
using SmartHouse.DAL.Model.Report;
using SmartHouse.DAL.Model.Wind;
using HeatPumpReport = SmartHouse.DAL.Model.Report.HeatPumpReport;

namespace SmartHouse.BLL.Controller
{
    public class ReportController
    {
        public WeatherController WeatherController { set; get; }
        public SunConditionController SunConditionController { set; get; }
        public DeviceController DeviceController { set; get; }
        public DeviceController DeviceOpt2Controller { set; get; }
        public DeviceController DeviceOpt3Controller { set; get; }
        public HouseController HouseController { set; get; }
        public WindEnergyController WindEnergyController { set; get; }
        public HeatPumpController HeatPumpController { set; get; }
        public HeatStoreController HeatStoreController { set; get; }

        public Report Report { set; get; }

        public void CopyData()
        {
            Report = new Report
            {
                StartDateTime = WeatherController.StartSelectedDateTime,
                EndDateTime = WeatherController.EndSelectedDateTime,
                Weather = new WeatherConditionReport(),
                ElectricalLoadSchedule = new ElectricalLoadScheduleReport
                {
                    DevicesTable = DevicesToTable(DeviceController.Devices),
                    DurationElectricalLoadsTables = DurationElectricalLoadsToTable(DeviceController.DurationElectricalLoadsForDevices),
                    ElectricalConsumptionsTable = ElectricalConsumptionsToTable(DeviceController.ElectricalConsumptionTable),
                    ElectricalRatesTable = ElectricalRatesToTable(DeviceController.RatesConsumptions),
                    ElectricalPrices = DeviceController.ElectricalPrices.Copy(),
                    ElectricalPrices1PhaseTable = PricesForPhasesToTable(DeviceController.Prices1Phase),
                    ElectricalPrices2PhaseTable = PricesForPhasesToTable(DeviceController.Prices2Phase),
                    ElectricalPrices3PhaseTable = PricesForPhasesToTable(DeviceController.Prices3Phase)
                },
                Opt2ElectricalLoadSchedule = new ElectricalLoadScheduleReport
                {
                    DevicesTable = DevicesToTable(DeviceOpt2Controller.Devices),
                    DurationElectricalLoadsTables = DurationElectricalLoadsToTable(DeviceOpt2Controller.DurationElectricalLoadsForDevices),
                    ElectricalConsumptionsTable = ElectricalConsumptionsToTable(DeviceOpt2Controller.ElectricalConsumptionTable),
                    ElectricalRatesTable = ElectricalRatesToTable(DeviceOpt2Controller.RatesConsumptions),
                    ElectricalPrices = DeviceOpt2Controller.ElectricalPrices.Copy(),
                    ElectricalPrices1PhaseTable = PricesForPhasesToTable(DeviceOpt2Controller.Prices1Phase),
                    ElectricalPrices2PhaseTable = PricesForPhasesToTable(DeviceOpt2Controller.Prices2Phase),
                    ElectricalPrices3PhaseTable = PricesForPhasesToTable(DeviceOpt2Controller.Prices3Phase)
                },
                Opt3ElectricalLoadSchedule = new ElectricalLoadScheduleReport
                {
                    DevicesTable = DevicesToTable(DeviceOpt3Controller.Devices),
                    DurationElectricalLoadsTables = DurationElectricalLoadsToTable(DeviceOpt3Controller.DurationElectricalLoadsForDevices),
                    ElectricalConsumptionsTable = ElectricalConsumptionsToTable(DeviceOpt3Controller.ElectricalConsumptionTable),
                    ElectricalRatesTable = ElectricalRatesToTable(DeviceOpt3Controller.RatesConsumptions),
                    ElectricalPrices = DeviceOpt3Controller.ElectricalPrices.Copy(),
                    ElectricalPrices1PhaseTable = PricesForPhasesToTable(DeviceOpt3Controller.Prices1Phase),
                    ElectricalPrices2PhaseTable = PricesForPhasesToTable(DeviceOpt3Controller.Prices2Phase),
                    ElectricalPrices3PhaseTable = PricesForPhasesToTable(DeviceOpt3Controller.Prices3Phase)
                },
                ComparePrices1Vs2Phases = CompareValues(DeviceController.ElectricalPrices.TotalCostFor1Phase,
                    DeviceController.ElectricalPrices.TotalCostFor2Phase),
                ComparePrices1Vs3Phases = CompareValues(DeviceController.ElectricalPrices.TotalCostFor1Phase,
                    DeviceController.ElectricalPrices.TotalCostFor3Phase),
                Opt2ComparePrices1Vs2Phases = CompareValues(DeviceOpt2Controller.ElectricalPrices.TotalCostFor1Phase,
                    DeviceOpt2Controller.ElectricalPrices.TotalCostFor2Phase),
                Opt3ComparePrices1Vs3Phases = CompareValues(DeviceOpt3Controller.ElectricalPrices.TotalCostFor1Phase,
                    DeviceOpt3Controller.ElectricalPrices.TotalCostFor3Phase),
                ThermalEnergy = new ThermalEnergyReport
                {
                    WaterIncomeParamsTable = WaterIncomeParamsToTable(HouseController.House.Hel),
                    WaterOutcomeParamsTable = WaterOutcomeParamsToTable(HouseController.House.Hel),
                    HouseThermalParamsTable = HeatParamsToTable(HouseController.House.HouseParams),
                    CommonBuildTemperature = HouseController.House.Temperature,
                    CalcOutsideTemperature = HouseController.CalculatedTemperature,
                    CommonScheme = new ThermalEnergySchemeReport
                    {
                        EtalonHeatLossesTable = HeatSchemeToTable(HouseController.CommonHeatLosses.GetDataTable),
                        TotalHeatConsumption = HouseController.CommonHeatLosses.TotalHeatConsumption,
                        TotalHeatHelConsumption = HouseController.CommonHeatLosses.TotalHeatHelConsumption,
                        HeatingTypesTable = HeatingTypesToTable(HouseController.CommonHeatLosses.Heats),
                        HeatingCostsTable = HeatingCostsToTable(HouseController.CommonHeatLosses.Heats)
                    },
                    IndividualTemperatureModesTable = IndividualTemperatureModesToTable(HouseController.House),
                    IndividualScheme = new ThermalEnergySchemeReport
                    {
                        EtalonHeatLossesTable = HeatSchemeToTable(HouseController.IndividualHeatLosses.GetDataTable),
                        TotalHeatConsumption = HouseController.IndividualHeatLosses.TotalHeatConsumption,
                        TotalHeatHelConsumption = HouseController.IndividualHeatLosses.TotalHeatHelConsumption,
                        HeatingTypesTable = HeatingTypesToTable(HouseController.IndividualHeatLosses.Heats),
                        HeatingCostsTable = HeatingCostsToTable(HouseController.IndividualHeatLosses.Heats)
                    },
                    CompareIndVsCommon = ConclusionIndVsCommon(HouseController.IndividualHeatLosses.TotalHeatHelConsumption,
                        HouseController.CommonHeatLosses.TotalHeatHelConsumption)
                },
                Wind = new WindReport
                {
                    WindGeneratorCharacteristicsTable = WindGenerationDescriptionToTable(WindEnergyController.WindGenDes),
                    WindGenerationsTable = WindGenerationToTable(WindEnergyController.WindGenerations),
                    TowerHeight = WindEnergyController.Height,
                    WindTotalEnergyGen = WindEnergyController.TotalEnergy / 1000,
                    GreenPrice = WindEnergyController.GreenPrice,
                    ReducedPollutionPrice = WindEnergyController.ReducedPollutionPrice,
                    Co2ReducedPollution = WindEnergyController.Co2ReducedPollution,
                    EnergyCost = WindEnergyController.EnergyCost,
                    ReducedPollutionCost = WindEnergyController.ReducedPollutionCost
                },
                HeatPump = new HeatPumpReport
                {
                    HeatPumpCharacteristicTable = HeatPumpCharacteristicToTable(HeatPumpController.HeatPump.HeatPumpDescriptions),
                    HeatPumpCalculationsTable = HeatPumpCalculationsToTable(HeatPumpController.HeatPump.HeatPumpCalculations),
                    HeatPumpCostTable = HeatingCostsToTable(HeatPumpController.Heats),
                    NominalHeatProduction = HeatPumpController.HeatPump.NominalHeatProduction,
                    NominalPower = HeatPumpController.HeatPump.NominalPower,
                    HeatPumpCount = HeatPumpController.HeatPump.HeatPumpCount,
                    CirculationPower = HeatPumpController.HeatPump.CirculationPower,
                    CirculationPumpCount = HeatPumpController.HeatPump.CirculationPumpCount,
                    FancoilPower = HeatPumpController.HeatPump.FancoilPower,
                    FancoilCount = HeatPumpController.HeatPump.FancoilCount,
                    TotalHeatLosses = HeatPumpController.HeatPump.TotalHeatLosses,
                    TotalHeatPumpConsumption = HeatPumpController.HeatPump.TotalHeatPumpConsumption,
                    TotalHeatSystemConsumption = HeatPumpController.HeatPump.TotalHeatSystemConsumption,
                    TotalQuantityHeatPumpProduction = HeatPumpController.HeatPump.TotalQuantityHeatPumpProduction,
                    TotalQuantityAdditionalHeatProduction = HeatPumpController.HeatPump.TotalQuantityAdditionalHeatProduction,
                    TotalQuantityHeatSystemProduction = HeatPumpController.HeatPump.TotalQuantityHeatSystemProduction,
                    AverageEfficiencyHeatPump = HeatPumpController.HeatPump.AverageEfficiencyHeatPump,
                    AverageEfficiencyHeatSystem = HeatPumpController.HeatPump.AverageEfficiencyHeatSystem,
                    TotalCost = HeatPumpController.HeatPump.TotalCost
                },
                HeatStore = new HeatStoreReport
                {
                    CostTable = HeatingCostsToTable(HeatStoreController.HeatStore.Heats),
                    StartDayZone = HeatStoreController.HeatStore.StartDayZone,
                    EndDayZone = HeatStoreController.HeatStore.EndDayZone,
                    DayRate = HeatStoreController.HeatStore.DayRate,
                    NightRate = HeatStoreController.HeatStore.NightRate,
                    Capacity = HeatStoreController.HeatStore.Capacity,
                    Power = HeatStoreController.HeatStore.Power,
                    TotalDayConsumption = HeatStoreController.HeatStore.TotalDayConsumption,
                    TotalNightConsumption = HeatStoreController.HeatStore.TotalNightConsumption,
                    TotalDayCost = HeatStoreController.HeatStore.TotalDayCost,
                    TotalNightCost = HeatStoreController.HeatStore.TotalNightCost,
                    TotalConsumption = HeatStoreController.HeatStore.TotalConsumption,
                    TotalCost = HeatStoreController.HeatStore.TotalCost,
                    WorstDay = HeatStoreController.HeatStore.WorstDay.Date,
                    EnergyLack = HeatStoreController.HeatStore.WorstDay.EnergyLack,
                    HourLack = HeatStoreController.HeatStore.WorstDay.HourLack
                }
            };
        }

        private List<List<string>> DevicesToTable(List<Device> devices)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>{"Споживач", "Потужність, кВт"});
            foreach (var device in devices)
            {
                table.Add(new List<string>{device.Name, device.Power.ToString()});
            }
            return table;
        }

        private List<List<List<string>>> DurationElectricalLoadsToTable(List<List<List<DurationElectricalLoad>>> electricalLoadsList)
        {
            List<List<List<string>>> tables = new List<List<List<string>>>();
            foreach (var electricalLoads  in electricalLoadsList.Last())
            {
                List<List<string>> table = new List<List<string>>();
                table.Add(new List<string> { "Потужність, кВт", "Тривалість, год" });
                foreach (var load in electricalLoads)
                {
                    table.Add(new List<string> { load.Power.ToString(), load.Duration.ToString("0.####") });
                }
                tables.Add(table);
            }
            return tables;
        }

        private List<List<string>> ElectricalConsumptionsToTable(List<ElectricalConsumptionRow> elConsumptions)
        {
            List<List<string>> table = new List<List<string>>();
            var headers = Constants.DayOfWeek.ToList();
            headers.Add("Всі");
            headers.Insert(0, "Споживання, кВт·год");
            table.Add(headers);
            foreach (var elConsumption in elConsumptions)
            {
                List<string> row = elConsumption.WeekConsumptions.Select(x => x.ToString("0.####")).ToList();
                row.Insert(0, elConsumption.DeviceName);
                table.Add(row);
            }
            return table;
        }

        private List<List<string>> ElectricalRatesToTable(List<RatesConsumption> ratesConsumptions)
        {
            List<List<string>> table = new List<List<string>>();
            var headers = Constants.RatesOfConsumption.ToList();
            headers.Insert(0, "Показник");
            table.Add(headers);
            foreach (var ratesConsumption in ratesConsumptions)
            {
                List<string> row = new List<string>
                {
                    ratesConsumption.Name,
                    ratesConsumption.Consumption.ToString("0.####"),
                    ratesConsumption.PowerMax.ToString("0.####"),
                    ratesConsumption.PowerAverage.ToString("0.####"),
                    ratesConsumption.DurationOfMaxPower.ToString("0.####"),
                    ratesConsumption.DegreeOfUnevenness.ToString("0.####"),
                    ratesConsumption.RateOfUsePower.ToString("0.####")
                };
                table.Add(row);
            }
            return table;
        }

        private List<List<string>> PricesForPhasesToTable(List<PriceRow> prices)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string> { "Назва", "Споживання, кВт·год", "Витрати, грн"});
            foreach (var price in prices)
            {
                table.Add(new List<string> { price.Name, price.Consumption.ToString("0.###"), price.Price.ToString("0.##")});
            }
            return table;
        }

        private double CompareValues(double p1, double p2)
        {
            return (p1 - p2) / p1 * 100;
        }

        private List<List<string>> WaterIncomeParamsToTable(HydroElectricLoad hel)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string> { "Параметр", "Значення" });
            table.Add(new List<string> { "Кількість користувачів душу", hel.NumberOfShowerUsers.ToString()});
            table.Add(new List<string> { "Кількість користувачів ванни", hel.NumberOfBathUsers.ToString() });
            table.Add(new List<string> { "Добова норма споживання води для душу, л", hel.WaterQuantityForShower.ToString()});
            table.Add(new List<string> { "Добова норма споживання води для ванни, л", hel.WaterQuantityForBath.ToString()});
            table.Add(new List<string> { "Температура води для душу, °С", hel.TemperatureOfShower.ToString()});
            table.Add(new List<string> { "Температура води для ванни, °С", hel.TemperatureOfBath.ToString()});
            table.Add(new List<string> { "Температура вхідної води, °С", hel.TemperatureOfInput.ToString()});
            table.Add(new List<string> { "Температура води на виході з бака, °С", hel.TemperatureOfOutput.ToString()});
            table.Add(new List<string> { "Час для нагрівання бака, год", hel.Time.ToString()});
            return table;
        }

        private List<List<string>> WaterOutcomeParamsToTable(HydroElectricLoad hel)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string> { "Параметр", "Значення" });
            table.Add(new List<string> { "Обсяги споживання води на прийоми душу, л/добу", hel.ShowerWaterConsumption.ToString() });
            table.Add(new List<string> { "Обсяги споживання води на прийоми ванни, л/добу", hel.BathWaterConsumption.ToString() });
            table.Add(new List<string> { "Відкореговані обсяги споживання води на прийоми душу, л/добу", hel.CorrectedShowerWaterConsumption.ToString() });
            table.Add(new List<string> { "Відкореговані обсяги споживання води на прийоми ванни, л/добу", hel.CorrectedBathWaterConsumption.ToString() });
            table.Add(new List<string> { "Загальні обсяги споживання води,  м³/добу", hel.TotalWaterConsumption.ToString() });
            table.Add(new List<string> { "Енергія необхідна для нагріву води,  кВт·год", hel.Energy.ToString() });
            table.Add(new List<string> { "Необхідна теплова потужність нагрівача, кВт", hel.Power.ToString() });
            return table;
        }

        private List<List<string>> HeatParamsToTable(List<HouseThermalParam> hps)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string> { "Параметр", "Термічний опір, (м²·К)/Вт", "Коефіцієнт теплопередачі, Вт/(м²·К)"});
            foreach (var hp in hps)
            {
                table.Add(new List<string> { hp.Name, hp.ThermalResist.ToString(), hp.ThermalTransfer.ToString() });
            }
            return table;
        }

        private List<List<string>> HeatSchemeToTable(DataTable dt)
        {
            List<List<string>> table = new List<List<string>>();
            List<string> headers = new List<string>();
            foreach (DataColumn column in dt.Columns)
            {
                headers.Add(column.ColumnName);
            }
            table.Add(headers);
            foreach (DataRow row in dt.Rows)
            {
                table.Add(new List<string> {row[0].ToString(), row[1].ToString()});
            }
            return table;
        }

        private List<List<string>> HeatingTypesToTable(List<TypeOfHeat> list)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>
            {
                "Вид палива", "Кількість палива на 1 кВтˑгод енергії", "Одиниця виміру",
                "Вартість палива, грн", "ККД котла"
            });
            foreach (var li in list)
            {
                table.Add(new List<string>
                {
                    li.Name, li.FuelPerKWht.ToString("0.##"), li.Unit, li.CostPerFuelUnit.ToString("0.##"),
                    li.Efficience.ToString("0.##")
                });
            }
            return table;
        }

        private List<List<string>> HeatingCostsToTable(List<TypeOfHeat> list)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>
            {
                "Вид палива", "Витрати палива", "Одиниця виміру", "Витрати, грн"
            });
            foreach (var li in list)
            {
                table.Add(new List<string>
                {
                    li.Name, li.FuelConsumption.ToString("0.##"), li.Unit, li.TotalPrice.ToString("0.##")
                });
            }
            return table;
        }

        private List<List<string>> IndividualTemperatureModesToTable(House house)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>
            {
                "Поверх", "Кімната", "Температура, °C"
            });
            foreach (var floor in house.Floors)
            {
                foreach (var room in floor.Rooms)
                {
                    table.Add(new List<string>
                    {
                        floor.Name, room.Name, room.Temperature.ToString("0.#")
                    });
                }
            }
            return table;
        }

        private string ConclusionIndVsCommon(double individual, double common)
        {
            double result = CompareValues(common, individual);
            return result >= 0 ? $"до зменшення витрат енергії на {result:0.##}%" : 
                $" до збільшення витрат енергії на {Math.Abs(result):0.##}%";
        }

        private List<List<string>> WindGenerationDescriptionToTable(List<WindGeneratorDescription> list)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>
            {
                "Швидкість вітру, м/с", "Потужність, кВт"
            });
            foreach (var li in list)
            {
                table.Add(new List<string>
                {
                    li.Wind.ToString(), li.Power.ToString("0.##")
                });
            }
            return table;
        }

        private List<List<string>> WindGenerationToTable(List<WindGeneration> list)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>
            {
                "Швидкість вітру, м/с", "Сумарна тривалість, год", "Потужність ВЕУ, кВт", "Енергія вироблена ВЕУ, кВт∙год"
            });
            foreach (var li in list)
            {
                table.Add(new List<string>
                {
                    li.Wind.ToString(), li.Duration.ToString("0.##"), li.Power.ToString("0.##"), li.Energy.ToString("0.##")
                });
            }
            return table;
        }

        private List<List<string>> HeatPumpCharacteristicToTable(List<HeatPumpDescription> list)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>
            {
                "Температура, °C", "Корекція теплопродуктивності", "Корекция споживаної потужності у режимі нагріву"
            });
            foreach (var li in list)
            {
                table.Add(new List<string>
                {
                    li.Temperature.ToString(), li.HeatProductionCorrection.ToString(), li.HeatPowerCorrection.ToString()
                });
            }
            return table;
        }

        private List<List<string>> HeatPumpCalculationsToTable(List<HeatPumpCalculation> list)
        {
            List<List<string>> table = new List<List<string>>();
            table.Add(new List<string>
            {
                "T, °C", "Pтеп, кВт", "t, год", "Qтеп, кВтˑгод", "Ктеп", "Qроб.тн, кВт", "Кес", "Рспож.тн, кВт",
                "Nблоків, шт", "Рдод.нагр, кВт", "Кзавантаж", "Рцирк.нас, кВт", "Wспож.тн, кВт∙год", "Wспож.сис, кВт∙год",
                "Qтн, кВт∙год", "Qдогр, кВт∙год", "Qсис, кВт∙год"
            });
            foreach (var li in list)
            {
                table.Add(new List<string>
                {
                    li.Temperature.ToString(),
                    li.Power.ToString("0.##"),
                    li.Duration.ToString("0.##"),
                    li.HeatLoses.ToString("0.##"),
                    li.HeatProductionCorrection.ToString("0.##"),
                    li.HeatProduction.ToString("0.##"),
                    li.HeatPowerCorrection.ToString("0.##"),
                    li.HeatPower.ToString("0.##"),
                    li.HeatPumpCount.ToString(),
                    li.AdditionalHeatPower.ToString("0.##"),
                    li.Load.ToString("0.##"),
                    li.CirculationPumpPower.ToString("0.##"),
                    li.HeatPumpConsumption.ToString("0.##"),
                    li.HeatSystemConsumption.ToString("0.##"),
                    li.QuantityHeatPumpProduction.ToString("0.##"),
                    li.QuantityAdditionalHeatProduction.ToString("0.##"),
                    li.QuantityHeatSystemProduction.ToString("0.##")
                });
            }
            return table;
        }
    }
}
