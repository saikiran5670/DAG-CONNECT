using System;
using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.portalservice.Entity.Common;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class SessionHelper
    {
        private readonly ILog _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _httpContextAccessor = httpContextAccessor;
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
                    if (session.Keys.Any(x => x.Equals(SessionConstants.ContextOrgKey)))
                    {
                        headerObj.ContextOrgId = session.GetInt32(SessionConstants.ContextOrgKey).Value;
                    }
                    //if (session.Keys.Any(x => x.Equals(SessionConstants.FeaturesKey)))
                    //{
                    //    headerObj.UserFeatures = session.GetObject<string[]>(SessionConstants.FeaturesKey);
                    //}

                    _logger.Info(headerObj.ToString() + $"\nSession Id - { session.Id }");
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
