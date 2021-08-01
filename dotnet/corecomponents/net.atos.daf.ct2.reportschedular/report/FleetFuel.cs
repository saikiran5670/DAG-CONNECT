using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.email.Enum;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.map.entity;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reportscheduler.entity;
using net.atos.daf.ct2.reportscheduler.helper;
using net.atos.daf.ct2.reportscheduler.report;
using net.atos.daf.ct2.reportscheduler.repository;
using net.atos.daf.ct2.template;
using net.atos.daf.ct2.unitconversion;
using net.atos.daf.ct2.unitconversion.ENUM;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.visibility;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.account.report
{
    public class FleetFuel : IReport
    {
        private readonly IReportSchedulerRepository _reportSchedularRepository;
        private readonly IVisibilityManager _visibilityManager;
        private readonly ITemplateManager _templateManager;
        private readonly IUnitConversionManager _unitConversionManager;
        private readonly IUnitManager _unitManager;
        private readonly EmailEventType _evenType;
        private readonly EmailContentType _contentType;
        private readonly IMapManager _mapManager;
        private readonly reportscheduler.helper.MapHelper _mapHelper;

        public string VIN { get; private set; }
        public string TimeZoneName { get; private set; }
        public string DateFormatName { get; private set; }
        public string VehicleName { get; private set; }
        public string RegistrationNo { get; private set; }
        public long FromDate { get; private set; }
        public long ToDate { get; private set; }
        public IEnumerable<VehicleList> VehicleLists { get; private set; }
        public string VehicleGroups { get; private set; }
        public string VehicleNames { get; private set; }
        public IEnumerable<string> VINs { get; private set; }
        public string TimeFormatName { get; private set; }
        public UnitToConvert UnitToConvert { get; private set; }
        public bool IsAllParameterSet { get; private set; } = false;
        public ReportCreationScheduler ReportSchedulerData { get; private set; }
        public IReportManager ReportManager { get; }
        public string DateTimeFormat { get; private set; }
        internal List<FleetFuelPdfDetails> FleetFuelPdfDetails { get; private set; }
        public double TotalIdleDuration { get; private set; }
        public double TotalFuelConsumption { get; private set; }
        public double TotalCO2Emission { get; private set; }
        public List<FleetFuelPdfTripDetails> FleetFuelTripPdfDetails { get; private set; }
        public int TotalNumberOfTrips { get; private set; }
        public double TotalDistance { get; private set; }
        public double TotalFuelConsumed { get; private set; }

        public FleetFuel(IReportManager reportManager,
                          IReportSchedulerRepository reportSchedularRepository,
                          IVisibilityManager visibilityManager, ITemplateManager templateManager,
                          IUnitConversionManager unitConversionManager, IUnitManager unitManager,
                          EmailEventType evenType, EmailContentType contentType, IMapManager mapManager)
        {
            ReportManager = reportManager;
            _reportSchedularRepository = reportSchedularRepository;
            _visibilityManager = visibilityManager;
            _templateManager = templateManager;
            _unitConversionManager = unitConversionManager;
            _unitManager = unitManager;
            _evenType = evenType;
            _contentType = contentType;
            _mapManager = mapManager;
            _mapHelper = new reportscheduler.helper.MapHelper(_mapManager);
        }

        public void SetParameters(ReportCreationScheduler reportSchedulerData, IEnumerable<VehicleList> vehicleLists)
        {
            FromDate = reportSchedulerData.StartDate;
            ToDate = reportSchedulerData.EndDate;
            VehicleLists = vehicleLists;
            VINs = vehicleLists.Select(s => s.VIN).Distinct();
            ReportSchedulerData = reportSchedulerData;
            TimeZoneName = reportSchedulerData.TimeZoneId > 0 ? TimeZoneSingleton.GetInstance(_reportSchedularRepository).GetTimeZoneName(reportSchedulerData.TimeZoneId) : TimeConstants.UTC;
            DateFormatName = reportSchedulerData.DateFormatId > 0 ? DateFormatSingleton.GetInstance(_reportSchedularRepository).GetDateFormatName(reportSchedulerData.DateFormatId) : FormatConstants.DATE_FORMAT;
            TimeFormatName = reportSchedulerData.TimeFormatId > 0 ? TimeFormatSingleton.GetInstance(_reportSchedularRepository).GetTimeFormatName(reportSchedulerData.TimeFormatId) : FormatConstants.TIME_FORMAT_24;
            UnitToConvert = reportSchedulerData.UnitId > 0 ? UnitNameSingleton.GetInstance(_reportSchedularRepository).GetUnitName(reportSchedulerData.UnitId) : UnitToConvert.Metric;
            DateTimeFormat = $"{DateFormatName} {TimeFormatName}";
            IsAllParameterSet = true;
        }

        public Task<string> GenerateSummary()
        {
            var html = string.Empty;
            return Task.FromResult<string>(html.ToString());
        }

        private async Task<string> GetRankingTable()
        {
            var rankingPdfDetails = new List<FleetFuelRankingPdf>();
            int count = 1;
            foreach (var item in FleetFuelPdfDetails.OrderByDescending(o => o.FuelConsumption))
            {
                rankingPdfDetails.Add(new FleetFuelRankingPdf()
                {
                    Ranking = count,
                    VehicleName = item.VehicleName,
                    VIN = item.VIN,
                    VehicleRegistrationNo = item.VehicleRegistrationNo,
                    Consumption = item.FuelConsumption
                });
                count++;
            }
            var html = ReportHelper
                        .ToDataTableAndGenerateHTML<FleetFuelRankingPdf>
                            (rankingPdfDetails);
            return await Task.FromResult<string>(html);
        }

        public async Task<string> GenerateTable()
        {
            var result = await ReportManager.GetFleetFuelDetailsByVehicle(new FleetFuelFilter { StartDateTime = FromDate, EndDateTime = ToDate, VINs = VINs.ToList() });
            var fleetFuelPdfDetails = new List<FleetFuelPdfDetails>();
            TotalIdleDuration = result.Sum(s => s.IdleDuration);
            foreach (var item in result)
            {
                fleetFuelPdfDetails.Add(
                    new FleetFuelPdfDetails
                    {
                        VehicleName = item.VehicleName,
                        VIN = item.VIN,
                        VehicleRegistrationNo = item.VehicleRegistrationNo,
                        Distance = await _unitConversionManager.GetDistance(item.Distance, DistanceUnit.Meter, UnitToConvert),
                        AverageDistancePerDay = await _unitConversionManager.GetDistance(item.AverageDistancePerDay, DistanceUnit.Meter, UnitToConvert),
                        AverageSpeed = await _unitConversionManager.GetSpeed(item.AverageSpeed, SpeedUnit.MeterPerMilliSec, UnitToConvert),
                        MaxSpeed = await _unitConversionManager.GetSpeed(item.MaxSpeed, SpeedUnit.KmPerHour, UnitToConvert),
                        NumberOfTrips = item.NumberOfTrips,
                        AverageGrossWeightComb = await _unitConversionManager.GetWeight(item.AverageGrossWeightComb, WeightUnit.KiloGram, UnitToConvert),
                        FuelConsumed = await _unitConversionManager.GetVolume(item.FuelConsumed, VolumeUnit.MilliLiter, UnitToConvert),
                        FuelConsumption = await _unitConversionManager.GetVolumePer100Km(item.Distance, item.FuelConsumed, VolumeUnit.MilliLiter, UnitToConvert),
                        CO2Emission = await _unitConversionManager.GetWeight(item.CO2Emission, WeightUnit.Tons, UnitToConvert),
                        IdleDuration = item.IdleDuration,
                        PTODuration = item.PTODuration,
                        HarshBrakeDuration = item.HarshBrakeDuration,
                        HeavyThrottleDuration = item.HeavyThrottleDuration,
                        CruiseControlDistance30_50 = item.CruiseControlDistance30_50,
                        CruiseControlDistance50_75 = item.CruiseControlDistance50_75,
                        CruiseControlDistance75 = item.CruiseControlDistance50_75,
                        AverageTrafficClassification = item.AverageTrafficClassification,
                        CCFuelConsumption = await _unitConversionManager.GetVolumePer100Km(item.CCFuelDistance, item.CCFuelConsumed, VolumeUnit.MilliLiter, UnitToConvert),
                        FuelconsumptionCCnonactive = await _unitConversionManager.GetVolumePer100Km(item.CCFuelDistanceNotActive, item.CCFuelConsumedNotActive, VolumeUnit.MilliLiter, UnitToConvert),
                        IdlingConsumption = item.IdlingConsumption < 0.002 ? IdlingConsumptionConstants.VERY_GOOD : item.IdlingConsumption > 0.020 ? IdlingConsumptionConstants.MODERATE : IdlingConsumptionConstants.GOOD,
                        DPAScore = item.DPAScore < 0.3 ? DPAScoreConstants.LIGHT : item.DPAScore > 1.3 ? DPAScoreConstants.MEDIUM : DPAScoreConstants.HIGH
                    });
            }
            TotalNumberOfTrips = fleetFuelPdfDetails.Sum(s => s.NumberOfTrips);
            TotalDistance = fleetFuelPdfDetails.Sum(s => s.Distance);
            TotalFuelConsumed = fleetFuelPdfDetails.Sum(s => s.FuelConsumed);
            TotalFuelConsumption = fleetFuelPdfDetails.Sum(s => s.FuelConsumption);
            TotalCO2Emission = fleetFuelPdfDetails.Sum(s => s.CO2Emission);
            FleetFuelPdfDetails = fleetFuelPdfDetails;
            var html = ReportHelper
                        .ToDataTableAndGenerateHTML<FleetFuelPdfDetails>
                            (fleetFuelPdfDetails);
            return await Task.FromResult<string>(html);
        }

        public async Task<string> GenerateTableForSingle()
        {
            var result = await ReportManager.GetFleetFuelTripDetailsByVehicle(new FleetFuelFilter { StartDateTime = FromDate, EndDateTime = ToDate, VINs = VINs.ToList() }, false);
            var fleetFuelPdfTripDetails = new List<FleetFuelPdfTripDetails>();
            TotalIdleDuration = result.Sum(s => s.IdleDuration);
            foreach (var item in result)
            {
                fleetFuelPdfTripDetails.Add(
                    new FleetFuelPdfTripDetails
                    {
                        VehicleName = item.VehicleName,
                        VIN = item.VIN,
                        VehicleRegistrationNo = item.VehicleRegistrationNo,
                        Distance = await _unitConversionManager.GetDistance(item.Distance, DistanceUnit.Meter, UnitToConvert),
                        AverageSpeed = await _unitConversionManager.GetSpeed(item.AverageSpeed, SpeedUnit.MeterPerMilliSec, UnitToConvert),
                        MaxSpeed = await _unitConversionManager.GetSpeed(item.MaxSpeed, SpeedUnit.KmPerHour, UnitToConvert),
                        GrossWeightComb = await _unitConversionManager.GetWeight(item.AverageGrossWeightComb, WeightUnit.KiloGram, UnitToConvert),
                        FuelConsumed = await _unitConversionManager.GetVolume(item.FuelConsumed, VolumeUnit.MilliLiter, UnitToConvert),
                        FuelConsumption = await _unitConversionManager.GetVolumePer100Km(item.Distance, item.FuelConsumed, VolumeUnit.MilliLiter, UnitToConvert),
                        CO2Emission = await _unitConversionManager.GetWeight(item.CO2Emission, WeightUnit.Tons, UnitToConvert),
                        IdleDuration = item.IdleDuration,
                        PTODuration = item.PTODuration,
                        HarshBrakeDuration = item.HarshBrakeDuration,
                        HeavyThrottleDuration = item.HeavyThrottleDuration,
                        CruiseControlDistance30_50 = item.CruiseControlDistance30_50,
                        CruiseControlDistance50_75 = item.CruiseControlDistance50_75,
                        CruiseControlDistance75 = item.CruiseControlDistance50_75,
                        AverageTrafficClassification = item.AverageTrafficClassification,
                        CCFuelConsumption = await _unitConversionManager.GetVolumePer100Km(item.CCFuelDistance, item.CCFuelConsumed, VolumeUnit.MilliLiter, UnitToConvert),
                        FuelconsumptionCCnonactive = await _unitConversionManager.GetVolumePer100Km(item.CCFuelDistanceNotActive, item.CCFuelConsumedNotActive, VolumeUnit.MilliLiter, UnitToConvert),
                        IdlingConsumption = item.IdlingConsumption < 0.002 ? IdlingConsumptionConstants.VERY_GOOD : item.IdlingConsumption > 0.020 ? IdlingConsumptionConstants.MODERATE : IdlingConsumptionConstants.GOOD,
                        DPAScore = item.DPAScore < 0.3 ? DPAScoreConstants.LIGHT : item.DPAScore > 1.3 ? DPAScoreConstants.MEDIUM : DPAScoreConstants.HIGH,
                        StartPosition = string.IsNullOrEmpty(item.StartPosition) ? await _mapHelper.GetAddress(item.Startpositionlattitude, item.Startpositionlongitude) : item.StartPosition,
                        EndPosition = string.IsNullOrEmpty(item.EndPosition) ? await _mapHelper.GetAddress(item.Endpositionlattitude, item.Endpositionlongitude) : item.EndPosition,
                        StartDate = TimeZoneHelper.GetDateTimeFromUTC(item.StartDate, TimeZoneName, DateTimeFormat),
                        EndDate = TimeZoneHelper.GetDateTimeFromUTC(item.EndDate, TimeZoneName, DateTimeFormat)
                    });

            }

            TotalDistance = fleetFuelPdfTripDetails.Sum(s => s.Distance);
            TotalFuelConsumed = fleetFuelPdfTripDetails.Sum(s => s.FuelConsumed);
            TotalFuelConsumption = fleetFuelPdfTripDetails.Sum(s => s.FuelConsumption);
            TotalCO2Emission = fleetFuelPdfTripDetails.Sum(s => s.CO2Emission);
            FleetFuelTripPdfDetails = fleetFuelPdfTripDetails;
            var html = ReportHelper
                        .ToDataTableAndGenerateHTML<FleetFuelPdfTripDetails>
                            (fleetFuelPdfTripDetails);
            return await Task.FromResult<string>(html);
        }

        public async Task<string> GenerateTemplate(byte[] logoBytes)
        {
            if (!IsAllParameterSet) throw new Exception(TripReportConstants.ALL_PARAM_MSG);
            var fromDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(FromDate, TimeConstants.UTC, $"{DateFormatName} {TimeFormatName}"));
            var toDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(ToDate, TimeConstants.UTC, $"{DateFormatName} {TimeFormatName}"));

            StringBuilder html = new StringBuilder();
            //ReportTemplateSingleto.
            //                        GetInstance()
            //                        .GetReportTemplate(_templateManager, ReportSchedulerData.ReportId, _evenType,
            //                                        _contentType, ReportSchedulerData.Code)
            var timeSpanUnit = await _unitManager.GetTimeSpanUnit(UnitToConvert);
            var distanceUnit = await _unitManager.GetDistanceUnit(UnitToConvert);
            var volumeUnit = await _unitManager.GetVolumeUnit(UnitToConvert);
            var volumnPerDistancedUnit = await _unitManager.GetVolumePerDistanceUnit(UnitToConvert);
            var volumePer100KmUnit = await _unitManager.GetVolumePer100KmUnit(UnitToConvert);
            var weightUnit = await _unitManager.GetWeightUnit(UnitToConvert);
            var speedUnit = await _unitManager.GetSpeedUnit(UnitToConvert);

            if (VehicleLists.Count() == 1)
            {
                var vehicle = VehicleLists.FirstOrDefault();
                html.AppendFormat(ReportTemplateContants.REPORT_TEMPLATE_FLEET_FUEL_SINGLE
                                  , logoBytes != null ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(logoBytes))
                                                    : ImageSingleton.GetInstance().GetDefaultLogo()
                                  , await GenerateTableForSingle()
                                  , fromDate.ToString(DateTimeFormat)
                                  , vehicle.VehicleGroupName ?? "All"
                                  , toDate.ToString(DateTimeFormat)
                                  , vehicle.VehicleName
                                  , TotalNumberOfTrips
                                  , Math.Round(TotalDistance, 2)
                                  , distanceUnit
                                  , Math.Round(TotalFuelConsumed, 2)
                                  , volumeUnit
                                  , await _unitConversionManager.GetTimeSpan(TotalIdleDuration, TimeUnit.Seconds, UnitToConvert)
                                  , timeSpanUnit
                                  , Math.Round(TotalFuelConsumption, 2)
                                  , volumePer100KmUnit
                                  , Math.Round(TotalCO2Emission, 2)
                                  , weightUnit
                                  , vehicle.VehicleName
                                  , vehicle.VIN
                                  , vehicle.RegistrationNo
                                  , distanceUnit
                                  , volumeUnit
                                  , volumePer100KmUnit
                                  , weightUnit
                                  , volumePer100KmUnit
                                  , volumePer100KmUnit
                                  , weightUnit
                                  , speedUnit
                                  , speedUnit
                                  , ImageSingleton.GetInstance().GetLogo()
                    );
            }
            else
            {
                html.AppendFormat(ReportTemplateContants.REPORT_TEMPLATE_FLEET_FUEL
                                  , logoBytes != null ? string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(logoBytes))
                                                    : ImageSingleton.GetInstance().GetDefaultLogo()
                                  , await GenerateTable()
                                  , fromDate.ToString(DateTimeFormat)
                                  , VehicleLists.Any(s => !string.IsNullOrEmpty(s.VehicleGroupName)) ? string.Join(',', VehicleLists.Select(s => s.VehicleGroupName).Distinct().ToArray()) : "All"
                                  , toDate.ToString(DateTimeFormat)
                                  , string.Join(',', VehicleLists.Select(s => s.VehicleName).Distinct().ToArray())
                                  , TotalNumberOfTrips
                                  , Math.Round(TotalDistance, 2)
                                  , distanceUnit
                                  , Math.Round(TotalFuelConsumed, 2)
                                  , volumeUnit
                                  , await _unitConversionManager.GetTimeSpan(TotalIdleDuration, TimeUnit.Seconds, UnitToConvert)
                                  , timeSpanUnit
                                  , Math.Round(TotalFuelConsumption, 2)
                                  , volumePer100KmUnit
                                  , Math.Round(TotalCO2Emission, 2)
                                  , weightUnit
                                  , distanceUnit
                                  , distanceUnit
                                  , speedUnit
                                  , speedUnit
                                  , weightUnit
                                  , volumeUnit
                                  , volumePer100KmUnit
                                  , weightUnit
                                  , volumePer100KmUnit
                                  , volumePer100KmUnit
                                  , volumePer100KmUnit
                                  , await GetRankingTable()
                                  , ImageSingleton.GetInstance().GetLogo()
                    );
            }
            return html.ToString();
        }

        private async Task<string> GetAddress(double lat, double lng)
        {
            if (lat != 0 && lng != 0)
            {
                try
                {
                    var lookupAddress = await _mapManager.GetMapAddress(new LookupAddress() { Latitude = lat, Longitude = lng });
                    return lookupAddress != null ? (lookupAddress.Address ?? string.Empty) : string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
