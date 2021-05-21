using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using log4net;
using System.Reflection;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class SessionHelper
    {
        private readonly ILog _logger;
        public SessionHelper()
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public HeaderObj GetSessionInfo(ISession session)
        {
            var headerObj = new HeaderObj();
            try
            {
                if (session != null)
                {
                    if (session.Keys.Any(x => x.Equals("accountId")))
                    {
                        headerObj.accountId = Convert.ToInt32(session.GetString("accountId"));
                    }
                    if (session.Keys.Any(x => x.Equals("roleId")))
                    {
                        headerObj.roleId = Convert.ToInt32(session.GetString("roleId"));
                    }
                    if (session.Keys.Any(x => x.Equals("orgId")))
                    {
                        headerObj.orgId = Convert.ToInt32(session.GetString("orgId"));
                    }
                    if (session.Keys.Any(x => x.Equals("contextOrgId")))
                    {
                        headerObj.contextOrgId = Convert.ToInt32(session.GetString("contextOrgId"));
                    }
                }
                return headerObj;
            }
            catch (Exception ex)
            {
                _logger.Error("Error while fetching session information", ex);
                return new HeaderObj();
            }
        }
    }
}
