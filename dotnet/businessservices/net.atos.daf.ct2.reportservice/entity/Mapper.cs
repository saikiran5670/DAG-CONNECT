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
                    State = userpreference.IsExclusive ?? ((char)IsExclusive.Yes).ToString(),
                });
            }
            return userPreferenceResult;
        }

        internal  net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest MapCreateUserPrefences(UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest obj
                   = new net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest();
            obj.AtributesShowNoShow = new List<reports.entity.Atribute>();

            obj.AccountId = objUserPreferenceCreateRequest.AccountId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;

            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new net.atos.daf.ct2.reports.entity.Atribute
                {
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    IsExclusive = objUserPreferenceCreateRequest.AtributesShowNoShow[i].IsExclusive.ToUpper() == ((char)IsExclusive.Yes).ToString() ? Convert.ToChar(IsExclusive.Yes) : Convert.ToChar(IsExclusive.No),
                });
            }
            return obj;
        }

        internal IEnumerable<string> MapVinList(IEnumerable<string> vinList)
        {
            var vinListResult = new List<string>();
            foreach (var vin in vinList)
            {
                vinListResult.Add(vin);
            }
            return vinListResult;
        }
    }
}
