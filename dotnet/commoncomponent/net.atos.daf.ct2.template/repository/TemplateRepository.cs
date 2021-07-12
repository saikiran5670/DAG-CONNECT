//TO DO: ROmve this, after testing

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Dapper;
//using net.atos.daf.ct2.data;
//using net.atos.daf.ct2.template.entity;
//using net.atos.daf.ct2.template.ENUM;

//namespace net.atos.daf.ct2.template.repository
//{
//    public class TemplateRepository : ITemplateRepository
//    {

//        private readonly IDataAccess _dataAccess;

//        public TemplateRepository(IDataAccess dataAccess)
//        {
//            _dataAccess = dataAccess;
//        }

//        public async Task<ReportTemplate> GetMultiLingualTempalte(ReportType eventType, ReportContentType contentType, string languageCode)
//        {
//            try
//            {
//                var parameter = new DynamicParameters();
//                parameter.Add("@contentType", (char)contentType);
//                parameter.Add("@eventName", eventType.ToString());

//                string emailTemplateQuery =
//                    @"select id as TemplateId, description as Description, type as ContentType, event_name as ReportType
//                    from master.emailtemplate
//                    where type=@contentType and event_name=@eventName";

//                var template = await _dataAccess.QueryFirstAsync<ReportTemplate>(emailTemplateQuery, parameter);

//                parameter = new DynamicParameters();
//                parameter.Add("@languageCode", languageCode);
//                parameter.Add("@templateId", template.TemplateId);

//                string emailTemplateLabelQuery;
//                if (languageCode.Equals("EN-GB"))
//                {
//                    emailTemplateLabelQuery =
//                        @"select etl.key as LabelKey, tl.value as TranslatedValue 
//                        from master.emailtemplatelabels etl
//                        INNER JOIN translation.translation tl ON etl.key=tl.name and tl.code=@languageCode
//                        WHERE etl.email_template_id=@templateId";
//                }
//                else
//                {
//                    emailTemplateLabelQuery =
//                        @"select etl.key as LabelKey, 
//	                    coalesce(tl1.value, tl2.value) as TranslatedValue 
//	                    from master.emailtemplatelabels etl
//	                    left JOIN translation.translation tl1  ON etl.key=tl1.name and tl1.code=@languageCode
//	                    left JOIN translation.translation tl2  ON etl.key=tl2.name and tl2.code='EN-GB'
//	                    WHERE etl.email_template_id=@templateId";
//                }
//                var labels = await _dataAccess.QueryAsync<ReportTemplateTranslationLabel>(emailTemplateLabelQuery, parameter);
//                template.TemplateLabels = labels;
//                return template;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//    }
//}
