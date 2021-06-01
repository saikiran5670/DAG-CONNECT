using System;
using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.AspNetCore.Http;

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
                        headerObj.AccountId = session.GetInt32(SessionConstants.AccountKey).Value;
                    }
                    if (session.Keys.Any(x => x.Equals(SessionConstants.RoleKey)))
                    {
                        headerObj.RoleId = session.GetInt32(SessionConstants.RoleKey).Value;
                    }
                    if (session.Keys.Any(x => x.Equals(SessionConstants.OrgKey)))
                    {
                        headerObj.OrgId = session.GetInt32(SessionConstants.OrgKey).Value;
                    }
                    //if (session.Keys.Any(x => x.Equals(SessionConstants.ContextOrgKey)))
                    //{
                    //    headerObj.contextOrgId = session.GetInt32(SessionConstants.ContextOrgKey).Value;
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while fetching session information", ex);
            }
            return headerObj;
        }
    }
}
