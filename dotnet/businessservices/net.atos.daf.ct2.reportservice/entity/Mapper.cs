﻿using Google.Protobuf.Collections;
using net.atos.daf.ct2.reports.ENUM;
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
                    State = userpreference.State ?? ((char)ReportPreferenceState.InActive).ToString(),
                });
            }
            return userPreferenceResult;
        }

        internal net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest MapCreateUserPrefences(UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest obj
                   = new net.atos.daf.ct2.reports.entity.UserPreferenceCreateRequest();
            obj.AtributesShowNoShow = new List<reports.entity.Atribute>();

            obj.OrganizationId = objUserPreferenceCreateRequest.OrganizationId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            obj.AccountId = objUserPreferenceCreateRequest.AccountId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            obj.Type = Convert.ToChar(objUserPreferenceCreateRequest.Type);
            obj.ChartType = Convert.ToChar(objUserPreferenceCreateRequest.CharType);
            obj.CreatedAt = objUserPreferenceCreateRequest.CreatedAt;
            obj.ModifiedAt = objUserPreferenceCreateRequest.ModifiedAt;

            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new net.atos.daf.ct2.reports.entity.Atribute
                {
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    State = objUserPreferenceCreateRequest.AtributesShowNoShow[i].State == ((char)ReportPreferenceState.Active).ToString() ? Convert.ToChar(ReportPreferenceState.Active) : Convert.ToChar(ReportPreferenceState.InActive),
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
