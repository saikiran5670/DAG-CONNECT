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
                    if (session.Keys.Any(x => x.Equals(SessionConstants.AccountKey)))
                    {
                        headerObj.accountId = session.GetInt32(SessionConstants.AccountKey).Value;
                    }
                    if (session.Keys.Any(x => x.Equals(SessionConstants.RoleKey)))
                    {
                        headerObj.roleId = session.GetInt32(SessionConstants.RoleKey).Value;
                    }
                    if (session.Keys.Any(x => x.Equals(SessionConstants.OrgKey)))
                    {
                        headerObj.orgId = session.GetInt32(SessionConstants.OrgKey).Value;
                    }
                    if (session.Keys.Any(x => x.Equals(SessionConstants.ContextOrgKey)))
                    {
                        headerObj.contextOrgId = session.GetInt32(SessionConstants.ContextOrgKey).Value;
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
