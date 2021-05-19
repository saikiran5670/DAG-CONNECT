using Google.Protobuf.Collections;
using net.atos.daf.ct2.reports.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.reportservice.entity
{
    public class Mapper
    {
        internal RepeatedField<UserPreferenceDataColumn> GetUserPrefences(IEnumerable<UserPrefernceReportDataColumn> userPrefernces)
        {
            var userPreferenceResult = new RepeatedField<UserPreferenceDataColumn>();
            foreach (var userpreference in userPrefernces)
            {
                userPreferenceResult.Add(new UserPreferenceDataColumn
                {
                    DataAtrributeId = userpreference.DataAtrributeId,
                    Name = userpreference.Name,
                    Description = userpreference.Description,
                    Type = userpreference.Type,
                    Key = userpreference.Key,
                    IsExclusive = userpreference.IsExclusive,
                });
            }
            return userPreferenceResult;
        }
    }
}
