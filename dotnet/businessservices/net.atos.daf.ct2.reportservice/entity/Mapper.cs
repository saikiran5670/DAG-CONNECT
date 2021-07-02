using System;
using System.Collections.Generic;
using System.Linq;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.ENUM;

namespace net.atos.daf.ct2.reportservice.entity
{
    public class Mapper
    {
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
                       AccountId = request.AccountId,
                       ContextOrgId = request.ContextOrgId
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

        internal ReportUserPreference MapReportUserPreferences(IEnumerable<reports.entity.ReportUserPreference> userPreferences)
        {
            var root = userPreferences.Where(up => up.Name.IndexOf('.') == -1).First();

            return FillRecursive(userPreferences, new int[] { root.DataAttributeId }).FirstOrDefault();
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
            if (parentIds != null)
            {
                foreach (var item in flatObjects.Where(x => parentIds.Contains(x.DataAttributeId)))
                {
                    var preference = new EcoScoreReportCompareDrivers
                    {
                        DataAttributeId = item.DataAttributeId,
                        Name = item.Name ?? item.Name,
                        Key = item.Key ?? item.Key,
                        Target = item.TargetValue
                    };

                    /// Score needs to be updated by getting the result from GetEcoScoreCompareReportAttributeValues()
                    preference.Score.AddRange(new List<EcoScoreReportAttribute>());
                    preference.SubCompareDrivers.AddRange(FillRecursiveEcoScoreCompareReport(flatObjects, item.SubDataAttributes, compareResult));

                    recursiveObjects.Add(preference);
                }
            }
            return recursiveObjects;
        }

        private static List<EcoScoreReportAttribute> GetEcoScoreCompareReportAttributeValues(string attributeName, IEnumerable<reports.entity.EcoScoreReportCompareDrivers> compareResult)
        {
            var lstAttributes = new List<EcoScoreReportAttribute>();
            var obj = new EcoScoreReportAttribute();
            var lst = new List<reports.entity.EcoScoreReportCompareDrivers>();
            switch ((ReportAttribute)Enum.Parse(typeof(ReportAttribute), attributeName))
            {
                case ReportAttribute.EcoScore:
                    foreach (var item in compareResult.Select(x => new { x.DriverId, x.EcoScore }).OrderBy(x => x.DriverId).ToList())
                    {
                        obj.DriverId = item.DriverId;
                        obj.Value = item.EcoScore;
                        obj.Color = string.Empty;
                        lstAttributes.Add(obj);
                    }
                    break;

                case ReportAttribute.FuelConsumption:
                    foreach (var item in compareResult.Select(x => new { x.DriverId, x.FuelConsumption }).OrderBy(x => x.DriverId).ToList())
                    {
                        obj.DriverId = item.DriverId;
                        obj.Value = item.FuelConsumption;
                        obj.Color = string.Empty;
                        lstAttributes.Add(obj);
                    }
                    break;

                default:
                    break;
            }
            return lstAttributes;
        }

    }
}