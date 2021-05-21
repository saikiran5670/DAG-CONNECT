using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class Mapper
    {
        internal reportservice.UserPreferenceCreateRequest MapCreateUserPrefences(net.atos.daf.ct2.portalservice.Entity.Report.UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            reportservice.UserPreferenceCreateRequest obj
                   = new reportservice.UserPreferenceCreateRequest();

            obj.AccountId = objUserPreferenceCreateRequest.AccountId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;

            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new reportservice.Atribute()
                {
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    IsExclusive = objUserPreferenceCreateRequest.AtributesShowNoShow[i].IsExclusive.ToString()
                });
            }
            return obj;
        }
    }
}
