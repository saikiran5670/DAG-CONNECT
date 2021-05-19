using Google.Protobuf.Collections;
using net.atos.daf.ct2.alert.ENUM;
using net.atos.daf.ct2.reports.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    IsExclusive = userpreference.IsExclusive ?? ((char)IsExclusive.Yes).ToString(),
                });
            }
            return userPreferenceResult;
        }

        internal async Task<net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest> MapCreateUserPrefences(UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest obj
                   = new net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest();
            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new net.atos.daf.ct2.reports.entity.Atribute
                {
                    AccountId = objUserPreferenceCreateRequest.AccountId,
                    ReportId = objUserPreferenceCreateRequest.ReportId,
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    IsExclusive = objUserPreferenceCreateRequest.AtributesShowNoShow[i].IsExclusive.ToUpper() == IsExclusive.Yes.ToString() ? Convert.ToChar(IsExclusive.Yes) : Convert.ToChar(IsExclusive.No),
                }); ; 
            }
            return  obj;
        }
    }
}
