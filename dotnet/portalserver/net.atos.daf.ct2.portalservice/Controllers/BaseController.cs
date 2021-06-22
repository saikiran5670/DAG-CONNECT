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

        protected async Task<bool> HasAdminPrivilege()
        {
            try
            {
                int level = await _privilegeChecker.GetLevelByRoleId(_userDetails.OrgId, _userDetails.RoleId);
                return level == 10 || level == 20;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
