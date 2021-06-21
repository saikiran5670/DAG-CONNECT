using System;
using System.Collections.Generic;
using System.Linq;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reports.ENUM;

namespace net.atos.daf.ct2.reportservice.entity
{
    public class Mapper
    {
        internal IEnumerable<UserPreferenceDataColumn> MapUserPrefences(IEnumerable<UserPrefernceReportDataColumn> userPrefernces)
        {
            var userPreferenceResult = new List<UserPreferenceDataColumn>();
            foreach (var userpreference in userPrefernces)
            {
                userPreferenceResult.Add(new UserPreferenceDataColumn
                {
                    DataAtrributeId = userpreference.DataAtrributeId,
                    Name = userpreference.Name,
                    Description = userpreference.Description ?? string.Empty,
                    Type = userpreference.Type,
                    Key = userpreference.Key,
                    State = userpreference.State ?? ((char)ReportPreferenceState.InActive).ToString(),
                });
            }
            return userPreferenceResult;
        }

        internal net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest MapCreateUserPrefences(UserPreferenceCreateRequest objUserPreferenceCreateRequest)
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
                    ChartType = objUserPreferenceCreateRequest.AtributesShowNoShow[i].CharType.ToCharArray().FirstOrDefault(),
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
    }
}
