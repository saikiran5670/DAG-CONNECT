using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly SessionHelper _sessionHelper;
        protected HeaderObj _userDetails;

        public BaseController(IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(_httpContextAccessor.HttpContext.Session);
        }

        protected int AssignOrgContextByAccountId(int requestedAccountId)
        {
            //Check if org context to be applied
            if (requestedAccountId == _userDetails.AccountId)
                return _userDetails.OrgId;
            return _userDetails.ContextOrgId;
        }

        protected int AssignOrgContextByRoleId(int requestedRoleId)
        {
            //Check if org context to be applied
            if (requestedRoleId == _userDetails.RoleId)
                return _userDetails.OrgId;
            return _userDetails.ContextOrgId;
        }

        protected int GetContextOrgId()
        {
            return _userDetails.ContextOrgId;
        }

        protected int GetUserSelectedOrgId()
        {
            return _userDetails.OrgId;
        }
    }
}
