namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public class Mapper
    {
        internal reportservice.UserPreferenceCreateRequest MapCreateUserPrefences(net.atos.daf.ct2.portalservice.Entity.Report.UserPreferenceCreateRequest objUserPreferenceCreateRequest)
        {
            reportservice.UserPreferenceCreateRequest obj
                   = new reportservice.UserPreferenceCreateRequest();

            obj.OrganizationId = objUserPreferenceCreateRequest.OrganizationId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            obj.AccountId = objUserPreferenceCreateRequest.AccountId;
            obj.ReportId = objUserPreferenceCreateRequest.ReportId;
            obj.Type = objUserPreferenceCreateRequest.Type.ToString();
            obj.CharType = objUserPreferenceCreateRequest.ChartType.ToString();
            obj.CreatedAt = objUserPreferenceCreateRequest.CreatedAt;
            obj.ModifiedAt = objUserPreferenceCreateRequest.ModifiedAt;

            for (int i = 0; i < objUserPreferenceCreateRequest.AtributesShowNoShow.Count; i++)
            {
                obj.AtributesShowNoShow.Add(new reportservice.Atribute()
                {
                    DataAttributeId = objUserPreferenceCreateRequest.AtributesShowNoShow[i].DataAttributeId,
                    State = objUserPreferenceCreateRequest.AtributesShowNoShow[i].State.ToString()
                });
            }
            return obj;
        }
    }
}
