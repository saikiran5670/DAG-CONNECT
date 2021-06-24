namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class Mapper
    {
        internal reportservice.UserPreferenceCreateRequest MapCreateUserPrefences(UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            reportservice.UserPreferenceCreateRequest obj
                   = new reportservice.UserPreferenceCreateRequest();

            obj.OrganizationId = objUserPreferenceCreateRequest.OrganizationId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            obj.AccountId = objUserPreferenceCreateRequest.AccountId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            //obj.Type = objUserPreferenceCreateRequest.Type.ToString();
            //obj.CharType = objUserPreferenceCreateRequest.ChartType.ToString();
            obj.CreatedAt = objUserPreferenceCreateRequest.CreatedAt;
            obj.ModifiedAt = objUserPreferenceCreateRequest.ModifiedAt;

            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new reportservice.Atribute()
                {
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    State = objUserPreferenceCreateRequest.AtributesShowNoShow[i].State.ToString(),
                    Type = objUserPreferenceCreateRequest.AtributesShowNoShow[i].Type,
                    CharType = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ChartType,
                    ThresholdType = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ThresholdType,
                    ThresholdValue = objUserPreferenceCreateRequest.AtributesShowNoShow[i].ThresholdValue
                });
            }
            return obj;
        }

        internal reportservice.CreateEcoScoreProfileRequest MapCreateEcoScoreProfile(EcoScoreProfileCreateRequest request)
        {
            var grpcRequest = new reportservice.CreateEcoScoreProfileRequest
            {
                Name = request.Name,
                Description = request.Description,
                IsDAFStandard = request.IsDAFStandard
            };

            foreach (var kpi in request.ProfileKPIs)
            {
                grpcRequest.ProfileKPIs.Add(new reportservice.CreateEcoScoreProfileKPI()
                {
                    KPIId = kpi.KPIId,
                    LimitValue = kpi.LimitValue,
                    TargetValue = kpi.TargetValue,
                    LowerValue = kpi.LowerValue,
                    UpperValue = kpi.UpperValue
                });
            }
            return grpcRequest;
        }
        internal reportservice.UpdateEcoScoreProfileRequest MapUpdateEcoScoreProfile(EcoScoreProfileUpdateRequest request)
        {
            var grpcRequest = new reportservice.UpdateEcoScoreProfileRequest();

            grpcRequest.ProfileId = request.ProfileId;
            grpcRequest.Name = request.Name;
            grpcRequest.Description = request.Description;

            foreach (var kpi in request.ProfileKPIs)
            {
                grpcRequest.ProfileKPIs.Add(new reportservice.CreateEcoScoreProfileKPI()
                {
                    KPIId = kpi.KPIId,
                    LimitValue = kpi.LimitValue,
                    TargetValue = kpi.TargetValue,
                    LowerValue = kpi.LowerValue,
                    UpperValue = kpi.UpperValue
                });
            }
            return grpcRequest;
        }

        internal reportservice.GetEcoScoreReportByAllDriversRequest MapEcoScoreReportByAllDriver(EcoScoreReportByAllDriversRequest request)
        {
            var grpcRequest = new reportservice.GetEcoScoreReportByAllDriversRequest
            {
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                MinTripDistance = request.MinTripDistance,
                MinDriverTotalDistance = request.MinDriverTotalDistance,
                TargetProfileId = request.TargetProfileId,
                ReportId = request.ReportId
            };
            grpcRequest.VINs.AddRange(request.VINs);
            return grpcRequest;
        }

        /// <summary>
        /// Initially created for Eco Score report. Later can be generalized.
        /// </summary>
        /// <param name="objUserPreferenceCreateRequest"></param>
        /// <param name="accountId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        internal reportservice.ReportUserPreferenceCreateRequest MapCreateReportUserPreferences(ReportUserPreferenceCreateRequest objUserPreferenceCreateRequest,
                                                                                                int accountId, int orgId)
        {
            reportservice.ReportUserPreferenceCreateRequest objRequest = new reportservice.ReportUserPreferenceCreateRequest();

            objRequest.ReportId = objUserPreferenceCreateRequest.ReportId;
            objRequest.AccountId = accountId;
            objRequest.OrganizationId = orgId;

            for (int i = 0; i < objUserPreferenceCreateRequest.Attributes.Count; i++)
            {
                objRequest.Attributes.Add(new reportservice.UserPreferenceAttribute()
                {
                    DataAttributeId = objUserPreferenceCreateRequest.Attributes[i].DataAttributeId,
                    State = objUserPreferenceCreateRequest.Attributes[i].State.ToString(),
                    Type = objUserPreferenceCreateRequest.Attributes[i].Type,
                    ChartType = objUserPreferenceCreateRequest.Attributes[i].ChartType,
                    ThresholdType = objUserPreferenceCreateRequest.Attributes[i].ThresholdType,
                    ThresholdValue = objUserPreferenceCreateRequest.Attributes[i].ThresholdValue
                });
            }
            return objRequest;
        }
    }
}
