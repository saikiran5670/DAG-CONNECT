using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly SessionHelper _sessionHelper;
        protected HeaderObj _userDetails;
        private readonly AccountPrivilegeChecker _privilegeChecker;

        public BaseController(IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(httpContextAccessor.HttpContext.Session);
        }

        public BaseController(IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper, AccountPrivilegeChecker privilegeChecker)
        {
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(httpContextAccessor.HttpContext.Session);
            _privilegeChecker = privilegeChecker;
        }

        protected int GetContextOrgId()
        {
            return _userDetails.ContextOrgId;
        }

        protected int GetUserSelectedOrgId()
        {
            return _userDetails.OrgId;
        }

        protected bool HasAdminPrivilege()
        {
            try
            {
                return _userDetails.RoleLevel == 10 || _userDetails.RoleLevel == 20;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected SessionFeature[] GetUserSubscribeFeatures()
        {
            return _userDetails.UserFeatures;
        }
    }
}
