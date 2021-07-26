using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.ENUM;

namespace net.atos.daf.ct2.reportservice.entity
{
    public class Mapper
    {
        public EnumTranslation MapEnumTranslation(reports.entity.EnumTranslation enumTrans)
        {
            EnumTranslation objenumtrans = new EnumTranslation();
            objenumtrans.Id = enumTrans.Id;
            objenumtrans.Type = string.IsNullOrEmpty(enumTrans.Type) ? string.Empty : enumTrans.Type;
            objenumtrans.Enum = string.IsNullOrEmpty(enumTrans.Enum) ? string.Empty : enumTrans.Enum;
            objenumtrans.ParentEnum = string.IsNullOrEmpty(enumTrans.ParentEnum) ? string.Empty : enumTrans.ParentEnum;
            objenumtrans.Key = string.IsNullOrEmpty(enumTrans.Key) ? string.Empty : enumTrans.Key;
            return objenumtrans;
        }
        internal IEnumerable<UserPreferenceDataColumn> MapUserPreferences(IEnumerable<UserPreferenceReportDataColumn> userPreferences)
        {
            var userPreferenceResult = new List<UserPreferenceDataColumn>();
            foreach (var userpreference in userPreferences)
            {
                userPreferenceResult.Add(new UserPreferenceDataColumn
                {
                    DataAtrributeId = userpreference.DataAtrributeId,
                    Name = userpreference.Name ?? string.Empty,
                    Type = userpreference.Type ?? string.Empty,
                    Key = userpreference.Key ?? string.Empty,
                    State = userpreference.State ?? ((char)ReportPreferenceState.InActive).ToString(),
                    ChartType = userpreference.ChartType ?? string.Empty,
                    ThresholdType = userpreference.ThresholdType ?? string.Empty,
                    ThresholdValue = userpreference.ThresholdValue
                });
            }
            return userPreferenceResult;
        }

        internal net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest MapCreateUserPreferences(UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest obj
                   = new net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest
                   {
                       AtributesShowNoShow = new List<reports.entity.Atribute>(),

                       OrganizationId = objUserPreferenceCreateRequest.OrganizationId,
                       ReportId = objUserPreferenceCreateRequest.ReportId,
                       AccountId = objUserPreferenceCreateRequest.AccountId
                   };
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            obj.CreatedAt = objUserPreferenceCreateRequest.CreatedAt;
            obj.ModifiedAt = objUserPreferenceCreateRequest.ModifiedAt;
            //obj.ChartType = Convert.ToChar(objUserPreferenceCreateRequest.CharType);
            //obj.ThresholdType = objUserPreferenceCreateRequest.ThresholdType;
            //obj.ThresholdValue = objUserPreferenceCreateRequest.ThresholdValue;

            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new net.atos.daf.ct2.reports.entity.Atribute
                {
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    State = objUserPreferenceCreateRequest.AtributesShowNoShow[i].State == ((char)ReportPreferenceState.Active).ToString() ? Convert.ToChar(ReportPreferenceState.Active) : Convert.ToChar(ReportPreferenceState.InActive),
                    Type = objUserPreferenceCreateRequest.AtributesShowNoShow[i].Type.ToCharArray().FirstOrDefault(),
                    ChartType = objUserPreferenceCreateRequest.AtributesShowNoShow[i].CharType == "" ? new char() : (char)objUserPreferenceCreateRequest.AtributesShowNoShow[i].CharType[0],
                    ThresholdType = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ThresholdType,
                    ThresholdValue = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ThresholdValue,
                });
            }
            return obj;
        }

        internal IEnumerable<string> MapVinList(IEnumerable<string> vinList)
        {
            var vinListResult = new List<string>();
            foreach (string vin in vinList)
            {
                vinListResult.Add(vin);
            }
            return vinListResult;
        }

        internal reports.entity.ReportUserPreferenceCreateRequest MapCreateReportUserPreferences(ReportUserPreferenceCreateRequest request)
        {
            reports.entity.ReportUserPreferenceCreateRequest objRequest
                   = new reports.entity.ReportUserPreferenceCreateRequest
                   {
                       Attributes = new List<reports.entity.UserPreferenceAttribute>(),
                       OrganizationId = request.OrganizationId,
                       ReportId = request.ReportId,
                       AccountId = request.AccountId
                   };

            foreach (var attribute in request.Attributes)
            {
                objRequest.Attributes.Add(new reports.entity.UserPreferenceAttribute
                {
                    DataAttributeId = attribute.DataAttributeId,
                    State = (ReportUserPreferenceState)(char)attribute.State,
                    Type = (ReportPreferenceType)(char)attribute.Type,
                    ChartType = attribute.ChartType > 0 ? (ReportPreferenceChartType)(char)attribute.ChartType : new ReportPreferenceChartType?(),
                    ThresholdType = attribute.ThresholdType > 0 ? (ReportPreferenceThresholdType?)(char)attribute.ThresholdType : new ReportPreferenceThresholdType?(),
                    ThresholdValue = attribute.ThresholdValue,
                });
            }
            return objRequest;
        }

        internal GetReportUserPreferenceResponse MapReportUserPreferences(IEnumerable<reports.entity.ReportUserPreference> userPreferences)
        {
            var root = userPreferences.Where(up => up.Name.IndexOf('.') == -1).First();

            var preferences = FillRecursive(userPreferences, new int[] { root.DataAttributeId }).FirstOrDefault();

            return new GetReportUserPreferenceResponse
            {
                TargetProfileId = root.TargetProfileId ?? 0,
                UserPreference = preferences,
                Code = Responsecode.Success
            };
        }

        private static List<ReportUserPreference> FillRecursive(IEnumerable<reports.entity.ReportUserPreference> flatObjects, int[] parentIds)
        {
            List<ReportUserPreference> recursiveObjects = new List<ReportUserPreference>();
            if (parentIds != null)
            {
                foreach (var item in flatObjects.Where(x => parentIds.Contains(x.DataAttributeId)))
                {
                    if (item.ReportAttributeType == ReportAttributeType.Simple ||
                        item.ReportAttributeType == ReportAttributeType.Complex)
                    {
                        var preference = new ReportUserPreference
                        {
                            DataAttributeId = item.DataAttributeId,
                            Name = item.Name ?? string.Empty,
                            Key = item.Key ?? string.Empty,
                            State = item.State ?? ((char)ReportPreferenceState.InActive).ToString(),
                            ChartType = item.ChartType ?? string.Empty,
                            ThresholdType = item.ThresholdType ?? string.Empty,
                            ThresholdValue = item.ThresholdValue
                        };
                        preference.SubReportUserPreferences.AddRange(FillRecursive(flatObjects, item.SubDataAttributes));
                        recursiveObjects.Add(preference);
                    }
                }
            }
            return recursiveObjects;
        }

        /// <summary>
        /// Mapper to covert GRPC request object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal EcoScoreReportByAllDriversRequest MapEcoScoreReportByAllDriversRequest(GetEcoScoreReportByAllDriversRequest request)
        {
            var objRequest = new EcoScoreReportByAllDriversRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                VINs = request.VINs.ToList<string>(),
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                OrgId = request.OrgId,
                AccountId = request.AccountId,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId
            };
            return objRequest;
        }

        /// <summary>
        /// Mapper to covert object to  GRPC response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal IEnumerable<EcoScoreReportDriversRanking> MapEcoScoreReportByAllDriversResponse(List<EcoScoreReportByAllDrivers> response)
        {
            var lstDriverRanking = new List<EcoScoreReportDriversRanking>();
            foreach (var item in response)
            {
                var ranking = new EcoScoreReportDriversRanking
                {
                    Ranking = item.Ranking,
                    DriverName = item.DriverName ?? string.Empty,
                    DriverId = item.DriverId ?? string.Empty,
                    EcoScoreRanking = item.EcoScoreRanking,
                    EcoScoreRankingColor = item.EcoScoreRankingColor ?? string.Empty
                };
                lstDriverRanking.Add(ranking);
            }
            return lstDriverRanking;
        }

        /// <summary>
        /// Mapper to covert GRPC request object
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal EcoScoreReportCompareDriversRequest MapEcoScoreReportCompareDriversRequest(GetEcoScoreReportCompareDriversRequest request)
        {
            var objRequest = new EcoScoreReportCompareDriversRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                VINs = request.VINs.ToList<string>(),
                DriverIds = request.DriverIds.ToList<string>(),
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId,
                OrgId = request.OrgId,
                AccountId = request.AccountId
            };
            return objRequest;
        }

        internal IEnumerable<EcoScoreReportDrivers> MapEcoScoreReportDrivers(IEnumerable<reports.entity.EcoScoreReportCompareDrivers> result)
        {
            var lstDrivers = new List<EcoScoreReportDrivers>();
            foreach (var item in result)
            {
                var obj = new EcoScoreReportDrivers
                {
                    DriverName = item.DriverName,
                    DriverId = item.DriverId
                };
                lstDrivers.Add(obj);
            }
            return lstDrivers;
        }

        internal EcoScoreReportCompareDrivers MapEcoScoreReportCompareDriversResponse(IEnumerable<reports.entity.EcoScoreReportCompareDrivers> compareResult, IEnumerable<reports.entity.EcoScoreCompareReportAtttributes> reportAttributes)
        {
            var root = reportAttributes.Where(up => up.Name.IndexOf('.') == -1).First();

            return FillRecursiveEcoScoreCompareReport(reportAttributes, new int[] { root.DataAttributeId }, compareResult).FirstOrDefault();
        }

        private static List<EcoScoreReportCompareDrivers> FillRecursiveEcoScoreCompareReport(IEnumerable<reports.entity.EcoScoreCompareReportAtttributes> flatObjects, int[] parentIds, IEnumerable<reports.entity.EcoScoreReportCompareDrivers> compareResult)
        {
            List<EcoScoreReportCompareDrivers> recursiveObjects = new List<EcoScoreReportCompareDrivers>();
            try
            {
                if (parentIds != null)
                {
                    foreach (var item in flatObjects.Where(x => parentIds.Contains(x.DataAttributeId)))
                    {
                        var preference = new EcoScoreReportCompareDrivers
                        {
                            DataAttributeId = item.DataAttributeId,
                            Name = item.Name ?? string.Empty,
                            Key = item.Key ?? string.Empty,
                            LimitType = item.LimitType ?? string.Empty,
                            LimitValue = item.TargetValue,
                            TargetValue = item.TargetValue,
                            RangeValueType = item.RangeValueType ?? string.Empty
                        };
                        if (!string.IsNullOrEmpty(item.DBColumnName))
                            preference.Score.AddRange(GetEcoScoreCompareReportAttributeValues(item.DBColumnName, compareResult));
                        preference.SubCompareDrivers.AddRange(FillRecursiveEcoScoreCompareReport(flatObjects, item.SubDataAttributes, compareResult));

                        recursiveObjects.Add(preference);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Error occurred while parsing the EcoScore compare drivers - FillRecursiveEcoScoreCompareReport().");
            }
            return recursiveObjects;
        }

        private static List<EcoScoreReportAttribute> GetEcoScoreCompareReportAttributeValues(string attributeName, IEnumerable<reports.entity.EcoScoreReportCompareDrivers> compareResult)
        {
            var lstAttributes = new List<EcoScoreReportAttribute>();
            try
            {
                EcoScoreReportAttribute obj;
                foreach (var item in compareResult)
                {
                    obj = new EcoScoreReportAttribute();
                    obj.DriverId = item.DriverId;
                    obj.Value = String.Format("{0:0.0}", Convert.ToDecimal(item.GetType().GetProperties().Where(y => y.Name.Equals(attributeName)).Select(x => x.GetValue(item)).FirstOrDefault()));
                    lstAttributes.Add(obj);
                }
            }
            catch (Exception)
            {
                throw new Exception("Error occurred while parsing the EcoScore compare drivers - GetEcoScoreCompareReportAttributeValues().");
            }
            return lstAttributes;
        }

        private static RankingColor? GetEcoScoreAttributeColor(string limitType, double limitValue, double targetValue, double score)
        {
            if (Convert.ToChar(limitType) == ((char)LimitType.Min))
            {
                if (score <= limitValue)
                    return RankingColor.Red;
                else if (score >= targetValue)
                    return RankingColor.Green;
                else
                    return RankingColor.Amber;
            }
            else if (Convert.ToChar(limitType) == ((char)LimitType.Max))
            {
                if (score <= targetValue)
                    return RankingColor.Red;
                else if (score >= limitValue)
                    return RankingColor.Green;
                else
                    return RankingColor.Amber;
            }
            return null;
        }
        public FleetOverviewDetails ToFleetOverviewDetailsResponse(net.atos.daf.ct2.reports.entity.FleetOverviewDetails fleetOverviewEntity)
        {
            FleetOverviewDetails response = new FleetOverviewDetails
            {
                Id = fleetOverviewEntity?.Id ?? 0,
                TripId = fleetOverviewEntity?.TripId ?? string.Empty,
                Vin = fleetOverviewEntity?.Vin ?? string.Empty,
                StartTimeStamp = fleetOverviewEntity?.StartTimeStamp ?? 0,
                EndTimeStamp = fleetOverviewEntity?.EndTimeStamp ?? 0,
                Driver1Id = fleetOverviewEntity?.Driver1Id ?? string.Empty,
                TripDistance = fleetOverviewEntity?.TripDistance ?? 0,
                DrivingTime = fleetOverviewEntity?.DrivingTime ?? 0,
                FuelConsumption = fleetOverviewEntity?.FuelConsumption ?? 0,
                VehicleDrivingStatusType = fleetOverviewEntity?.VehicleDrivingStatusType,
                OdometerVal = fleetOverviewEntity?.OdometerVal ?? 0,
                DistanceUntilNextService = fleetOverviewEntity?.DistanceUntilNextService ?? 0,
                LatestReceivedPositionLattitude = fleetOverviewEntity?.LatestReceivedPositionLattitude ?? 0,
                LatestReceivedPositionLongitude = fleetOverviewEntity?.LatestReceivedPositionLongitude ?? 0,
                LatestReceivedPositionHeading = fleetOverviewEntity?.LatestReceivedPositionHeading ?? 0,
                StartPositionLattitude = fleetOverviewEntity?.StartPositionLattitude ?? 0,
                StartPositionLongitude = fleetOverviewEntity?.StartPositionLongitude ?? 0,
                StartPositionHeading = fleetOverviewEntity?.StartPositionHeading ?? 0,
                LatestProcessedMessageTimeStamp = fleetOverviewEntity?.LatestProcessedMessageTimeStamp ?? 0,
                VehicleHealthStatusType = fleetOverviewEntity?.VehicleHealthStatusType ?? string.Empty,
                LatestWarningClass = fleetOverviewEntity?.LatestWarningClass ?? 0,
                LatestWarningNumber = fleetOverviewEntity?.LatestWarningNumber ?? 0,
                LatestWarningType = fleetOverviewEntity?.LatestWarningType ?? string.Empty,
                LatestWarningTimestamp = fleetOverviewEntity?.LatestWarningTimestamp ?? 0,
                LatestWarningPositionLatitude = fleetOverviewEntity?.LatestWarningPositionLatitude ?? 0,
                LatestWarningPositionLongitude = fleetOverviewEntity?.LatestWarningPositionLongitude ?? 0,
                Vid = fleetOverviewEntity?.Vid ?? string.Empty,
                RegistrationNo = fleetOverviewEntity?.RegistrationNo ?? string.Empty,
                DriverName = fleetOverviewEntity?.DriverName ?? string.Empty,
                LatestGeolocationAddressId = fleetOverviewEntity?.LatestGeolocationAddressId ?? 0,
                LatestGeolocationAddress = fleetOverviewEntity?.LatestGeolocationAddress ?? string.Empty,
                StartGeolocationAddressId = fleetOverviewEntity?.StartGeolocationAddressId ?? 0,
                StartGeolocationAddress = fleetOverviewEntity?.StartGeolocationAddress ?? string.Empty,
                LatestWarningGeolocationAddressId = fleetOverviewEntity?.LatestWarningGeolocationAddressId ?? 0,
                LatestWarningGeolocationAddress = fleetOverviewEntity?.LatestWarningGeolocationAddress ?? string.Empty,
                LatestWarningName = fleetOverviewEntity?.LatestWarningName ?? string.Empty,
                VehicleName = fleetOverviewEntity?.VehicleName ?? string.Empty
            };
            if (fleetOverviewEntity.LiveFleetPositions != null && fleetOverviewEntity.LiveFleetPositions.Count > 0)
            {
                foreach (var item in fleetOverviewEntity.LiveFleetPositions)
                {
                    response.LiveFleetPosition.Add(ToLiveFleetPositionResponse(item));
                }
            }

            if (fleetOverviewEntity.FleetOverviewAlert != null && fleetOverviewEntity.FleetOverviewAlert.Count > 0)
            {
                foreach (var item in fleetOverviewEntity.FleetOverviewAlert)
                {
                    response.FleetOverviewAlert.Add(ToLiveFleetAlertResponse(item));
                }
            }
            return response;
        }
        public LiveFleetPosition ToLiveFleetPositionResponse(net.atos.daf.ct2.reports.entity.LiveFleetPosition fleetOverviewEntity)
        {
            LiveFleetPosition liveFleetPosition = new LiveFleetPosition
            {
                Id = fleetOverviewEntity?.Id ?? 0,
                //TripId = fleetOverviewEntity.TripId,
                GpsAltitude = fleetOverviewEntity?.GpsAltitude ?? 0,
                GpsHeading = fleetOverviewEntity?.GpsHeading ?? 0,
                GpsLatitude = fleetOverviewEntity?.GpsLatitude ?? 0,
                GpsLongitude = fleetOverviewEntity?.GpsLongitude ?? 0,
                Co2Emission = fleetOverviewEntity?.Co2emission ?? 0,
                Fuelconsumtion = fleetOverviewEntity?.Fuelconsumtion ?? 0,
            };
            return liveFleetPosition;
        }
        public FleetOverviewAlert ToLiveFleetAlertResponse(net.atos.daf.ct2.reports.entity.FleetOverviewAlert fleetOverviewAlertEntity)
        {
            FleetOverviewAlert fleetOverviewAlert = new FleetOverviewAlert
            {
                Id = fleetOverviewAlertEntity.Id,
                Name = fleetOverviewAlertEntity.AlertName,
                Type = fleetOverviewAlertEntity.AlertType,
                Time = fleetOverviewAlertEntity.AlertTime,
                Level = fleetOverviewAlertEntity.AlertLevel,
                CategoryType = fleetOverviewAlertEntity.CategoryType,
                Latitude = fleetOverviewAlertEntity.AlertLatitude,
                Longitude = fleetOverviewAlertEntity.AlertLongitude,
                GeolocationAddressId = fleetOverviewAlertEntity.AlertGeolocationAddressId,
                GeolocationAddress = fleetOverviewAlertEntity.AlertGeolocationAddress,
                AlertId = fleetOverviewAlertEntity.AlertId
            };
            return fleetOverviewAlert;
        }

        public FuelBenchmarkDetails MapFuelBenchmarktoModel(net.atos.daf.ct2.reports.entity.FuelBenchmarkDetails request)
        {
            FuelBenchmarkDetails fuelbenchmark = new FuelBenchmarkDetails();
            fuelbenchmark.NumberOfActiveVehicles = request.NumberOfActiveVehicles;
            fuelbenchmark.NumberOfTotalVehicles = request.NumberOfTotalVehicles;
            fuelbenchmark.TotalMileage = request.TotalMileage;
            fuelbenchmark.TotalFuelConsumed = request.TotalFuelConsumed;
            fuelbenchmark.AverageFuelConsumption = request.AverageFuelConsumption;
            foreach (var item in request.Ranking)
            {
                Ranking objRanking = new Ranking();
                objRanking.VIN = item.VIN;
                objRanking.FuelConsumption = item.FuelConsumption;
                objRanking.VehicleName = item.VehicleName;
                fuelbenchmark.Ranking.Add(objRanking);
            }
            return fuelbenchmark;
        }

        internal EcoScoreReportSingleDriverRequest MapEcoScoreReportSingleDriverRequest(GetEcoScoreReportSingleDriverRequest request)
        {
            var objRequest = new EcoScoreReportSingleDriverRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                VINs = request.VINs.ToList<string>(),
                DriverId = request.DriverId,
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId,
                OrgId = request.OrgId,
                AccountId = request.AccountId,
                UoM = request.UoM
            };
            return objRequest;
        }

        internal EcoScoreReportSingleDriverOverallPerformance MapEcoScoreReportSingleDriverOverallPerformance(IEnumerable<reports.entity.EcoScoreReportSingleDriver> result, IEnumerable<reports.entity.EcoScoreCompareReportAtttributes> reportAttributes)
        {
            var objOverall = new EcoScoreReportSingleDriverOverallPerformance();
            var dataMartOverall = result.Where(x => x.HeaderType == "Overall_Driver").FirstOrDefault();
            if (dataMartOverall != null)
            {
                objOverall.EcoScore = MapEcoScoreReportOverallPerformanceKPI(OverallPerformance.EcoScore.ToString(), dataMartOverall, reportAttributes);
                objOverall.FuelConsumption = MapEcoScoreReportOverallPerformanceKPI(OverallPerformance.FuelConsumption.ToString(), dataMartOverall, reportAttributes);
                objOverall.AnticipationScore = MapEcoScoreReportOverallPerformanceKPI(OverallPerformance.AnticipationScore.ToString(), dataMartOverall, reportAttributes);
                objOverall.BrakingScore = MapEcoScoreReportOverallPerformanceKPI(OverallPerformance.BrakingScore.ToString(), dataMartOverall, reportAttributes);
            }
            return objOverall;
        }

        private static EcoScoreReportOverallPerformanceKPI MapEcoScoreReportOverallPerformanceKPI(string kpiName, reports.entity.EcoScoreReportSingleDriver dataMartOverall, IEnumerable<reports.entity.EcoScoreCompareReportAtttributes> reportAttributes)
        {
            var objKPI = new EcoScoreReportOverallPerformanceKPI();
            var dataAttribute = new EcoScoreCompareReportAtttributes();
            if (dataMartOverall != null)
            {
                dataAttribute = reportAttributes.Where(x => x.DBColumnName == kpiName).FirstOrDefault();
                objKPI.DataAttributeId = dataAttribute.DataAttributeId;
                objKPI.LimitType = dataAttribute.LimitType;
                objKPI.LimitValue = dataAttribute.LimitValue;
                objKPI.TargetValue = dataAttribute.TargetValue;
                if (kpiName == OverallPerformance.EcoScore.ToString())
                    objKPI.Score = String.Format("{0:0}", Convert.ToDecimal(dataMartOverall.GetType().GetProperties().Where(y => y.Name.Equals(kpiName)).Select(x => x.GetValue(dataMartOverall)).FirstOrDefault()));
                else
                    objKPI.Score = String.Format("{0:0.0}", Convert.ToDecimal(dataMartOverall.GetType().GetProperties().Where(y => y.Name.Equals(kpiName)).Select(x => x.GetValue(dataMartOverall)).FirstOrDefault()));
            }
            return objKPI;
        }

        internal IEnumerable<EcoScoreReportSingleDriverHeader> MapEcoScoreReportSingleDriverHeader(IEnumerable<reports.entity.EcoScoreReportSingleDriver> result)
        {
            var lstDriver = new List<EcoScoreReportSingleDriverHeader>();
            foreach (var item in result)
            {
                var obj = new EcoScoreReportSingleDriverHeader
                {
                    HeaderType = item.HeaderType,
                    VIN = item.VIN ?? string.Empty,
                    VehicleName = item.VehicleName ?? string.Empty,
                    RegistrationNo = item.RegistrationNo ?? string.Empty
                };
                lstDriver.Add(obj);
            }
            return lstDriver;
        }

        internal EcoScoreReportSingleDriver MapEcoScoreReportSingleDriverResponse(IEnumerable<reports.entity.EcoScoreReportSingleDriver> result, IEnumerable<reports.entity.EcoScoreCompareReportAtttributes> reportAttributes)
        {
            var root = reportAttributes.Where(up => up.Name.IndexOf('.') == -1).First();

            return FillRecursiveEcoScoreSingleDriverReport(reportAttributes, new int[] { root.DataAttributeId }, result).FirstOrDefault();
        }

        private static List<EcoScoreReportSingleDriver> FillRecursiveEcoScoreSingleDriverReport(IEnumerable<reports.entity.EcoScoreCompareReportAtttributes> flatObjects, int[] parentIds, IEnumerable<reports.entity.EcoScoreReportSingleDriver> result)
        {
            List<EcoScoreReportSingleDriver> recursiveObjects = new List<EcoScoreReportSingleDriver>();
            try
            {
                if (parentIds != null)
                {
                    foreach (var item in flatObjects.Where(x => parentIds.Contains(x.DataAttributeId)))
                    {
                        var preference = new EcoScoreReportSingleDriver
                        {
                            DataAttributeId = item.DataAttributeId,
                            Name = item.Name ?? string.Empty,
                            Key = item.Key ?? string.Empty,
                            LimitType = item.LimitType ?? string.Empty,
                            LimitValue = item.TargetValue,
                            TargetValue = item.TargetValue,
                            RangeValueType = item.RangeValueType ?? string.Empty
                        };
                        if (!string.IsNullOrEmpty(item.DBColumnName))
                            preference.Score.AddRange(GetEcoScoreSingleDriverReportAttributeValues(item.DBColumnName, result));
                        preference.SubSingleDriver.AddRange(FillRecursiveEcoScoreSingleDriverReport(flatObjects, item.SubDataAttributes, result));

                        recursiveObjects.Add(preference);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Error occurred while parsing the EcoScore single driver - FillRecursiveEcoScoreSingleDriverReport().");
            }
            return recursiveObjects;
        }

        private static List<EcoScoreReportSingleDriverAttribute> GetEcoScoreSingleDriverReportAttributeValues(string attributeName, IEnumerable<reports.entity.EcoScoreReportSingleDriver> result)
        {
            var lstAttributes = new List<EcoScoreReportSingleDriverAttribute>();
            try
            {
                EcoScoreReportSingleDriverAttribute obj;
                foreach (var item in result)
                {
                    obj = new EcoScoreReportSingleDriverAttribute();
                    obj.HeaderType = item.HeaderType;
                    obj.VIN = item.VIN ?? string.Empty;
                    obj.Value = String.Format("{0:0.0}", Convert.ToDecimal(item.GetType().GetProperties().Where(y => y.Name.Equals(attributeName)).Select(x => x.GetValue(item)).FirstOrDefault()));
                    lstAttributes.Add(obj);
                }
            }
            catch (Exception)
            {
                throw new Exception("Error occurred while parsing the EcoScore single driver - GetEcoScoreSingleDriverReportAttributeValues().");
            }
            return lstAttributes;
        }

        internal EcoScoreSingleDriverBarPieChart MapEcoScoreAverageGrossWeightChartResponse(List<reports.entity.EcoScoreSingleDriverBarPieChart> result)
        {
            var avgGrossWeight = new EcoScoreSingleDriverBarPieChart();
            string[] labels = new string[6] { "0-10 t", "10-20 t", "20-30 t", "30-40 t", "40-50 t", ">50 t" };
            avgGrossWeight.XAxisLabel.AddRange(labels);

            List<string> vehicleName = result.Select(x => x.VehicleName).Distinct().ToList();
            if (vehicleName.Count > 0)
            {
                var lstChartData = new List<EcoScoreSingleDriverChartDataSet>();
                foreach (var item in vehicleName)
                {
                    var obj = new EcoScoreSingleDriverChartDataSet();
                    obj.Label = item;
                    foreach (var lbl in labels)
                    {
                        var avg = result.Where(x => x.VehicleName == item && x.X_Axis == lbl).FirstOrDefault();
                        if (avg != null)
                            obj.Data.Add(avg.Y_Axis);
                        else
                            obj.Data.Add(new double());
                    }
                    lstChartData.Add(obj);
                }
                avgGrossWeight.ChartDataSet.AddRange(lstChartData);
            }
            return avgGrossWeight;
        }

        internal EcoScoreSingleDriverBarPieChart MapEcoScoreAverageDrivingSpeedChartResponse(List<reports.entity.EcoScoreSingleDriverBarPieChart> result, string unit)
        {
            var avgDrivingSpeed = new EcoScoreSingleDriverBarPieChart();
            string[] labels;
            if (unit == UoM.Imperial.ToString())
                labels = new string[5] { "0-15 mph", "15-30 mph", "30-45 mph", "45-50 mph", ">50 mph" };
            else
                labels = new string[5] { "0-30 kmph", "30-50 kmph", "50-75 kmph", "75-85 kmph", ">85 kmph" };
            avgDrivingSpeed.XAxisLabel.AddRange(labels);

            List<string> vehicleName = result.Select(x => x.VehicleName).Distinct().ToList();
            if (vehicleName.Count > 0)
            {
                var lstChartData = new List<EcoScoreSingleDriverChartDataSet>();
                foreach (var item in vehicleName)
                {
                    var obj = new EcoScoreSingleDriverChartDataSet();
                    obj.Label = item;
                    foreach (var lbl in labels)
                    {
                        var avg = result.Where(x => x.VehicleName == item && x.X_Axis == lbl).FirstOrDefault();
                        if (avg != null)
                            obj.Data.Add(avg.Y_Axis);
                        else
                            obj.Data.Add(new double());
                    }
                    lstChartData.Add(obj);
                }
                avgDrivingSpeed.ChartDataSet.AddRange(lstChartData);
            }
            return avgDrivingSpeed;
        }

        internal List<EcoScoreTrendlines> MapEcoScoreReportTrendlines(IEnumerable<reports.entity.EcoScoreReportSingleDriver> result, IEnumerable<reports.entity.EcoScoreCompareReportAtttributes> reportAttributes)
        {
            var trendlines = new List<EcoScoreTrendlines>();
            var vins = result.Select(x => x.VIN).Distinct();
            foreach (var item in vins)
            {
                var objTrendline = new EcoScoreTrendlines();
                objTrendline.VIN = item;

                var lstKPIbyDays = result.Where(x => x.VIN == item).ToList();
                if (lstKPIbyDays != null)
                {
                    objTrendline.VehicleName = lstKPIbyDays.FirstOrDefault().VehicleName;

                    #region Get Report Attribute Details for each KPI and get daywise value

                    //EcoScore Company
                    var attribute = reportAttributes.Where(x => x.DBColumnName == "EcoScore").FirstOrDefault();
                    var objKPIInfo = new EcoScoreTrendlinesKPIInfo();
                    objKPIInfo.EcoScoreCompany.Name = attribute.Name;
                    objKPIInfo.EcoScoreCompany.Key = attribute.Key;

                    var lstDaywiseValue = new List<EcoScoreTrendlinesKPIScore>();
                    var lstKPI = lstKPIbyDays.Where(x => x.HeaderType == "Overall_Company").ToList();
                    foreach (var kpi in lstKPI)
                    {
                        var data = new EcoScoreTrendlinesKPIScore();
                        data.Day = kpi.Day;
                        data.Value = String.Format("{0:0.0}", kpi.EcoScore);
                        lstDaywiseValue.Add(data);
                    }
                    objKPIInfo.EcoScoreCompany.Data.AddRange(lstDaywiseValue.ToArray());

                    //EcoScore 
                    attribute = reportAttributes.Where(x => x.DBColumnName == "EcoScore").FirstOrDefault();
                    objKPIInfo = new EcoScoreTrendlinesKPIInfo();
                    objKPIInfo.EcoScore.Name = attribute.Name;
                    objKPIInfo.EcoScore.Key = attribute.Key;

                    lstDaywiseValue = new List<EcoScoreTrendlinesKPIScore>();
                    lstKPI = lstKPIbyDays.Where(x => x.HeaderType == "Overall_Driver").ToList();
                    foreach (var kpi in lstKPI)
                    {
                        var data = new EcoScoreTrendlinesKPIScore();
                        data.Day = kpi.Day;
                        data.Value = String.Format("{0:0.0}", kpi.EcoScore);
                        lstDaywiseValue.Add(data);
                    }
                    objKPIInfo.EcoScore.Data.AddRange(lstDaywiseValue.ToArray());

                    //Fuel Consumption
                    attribute = reportAttributes.Where(x => x.DBColumnName == "FuelConsumption").FirstOrDefault();
                    objKPIInfo = new EcoScoreTrendlinesKPIInfo();
                    objKPIInfo.FuelConsumption.Name = attribute.Name;
                    objKPIInfo.FuelConsumption.Key = attribute.Key;

                    lstDaywiseValue = new List<EcoScoreTrendlinesKPIScore>();
                    foreach (var kpi in lstKPIbyDays)
                    {
                        var data = new EcoScoreTrendlinesKPIScore();
                        data.Day = kpi.VIN;
                        data.Value = String.Format("{0:0.0}", kpi.FuelConsumption);
                        lstDaywiseValue.Add(data);
                    }
                    objKPIInfo.FuelConsumption.Data.AddRange(lstDaywiseValue.ToArray());
                    #endregion

                    objTrendline.KPIInfo = objKPIInfo;
                }
                trendlines.Add(objTrendline);
            }
            return trendlines;
        }
    }

}