using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.rfmsdataservice.CustomAttributes
{
    public class AuthorizeHandler :
          AuthorizationHandler<AuthorizeRequirement>
    {
        private readonly IAccountManager _accountManager;
        private readonly ILog _logger;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizeHandler(IAccountManager accountManager, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            this._accountManager = accountManager;
            _cache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, AuthorizeRequirement requirement)
        {
            string emailAddress = string.Empty;
            var emailClaim = context.User.Claims.Where(x => x.Type.Equals("email") || x.Type.Equals(ClaimTypes.Email)).FirstOrDefault();

            if (emailClaim != null && !string.IsNullOrEmpty(emailClaim.Value))
            {
                emailAddress = emailClaim.Value;
                _logger.Info($"[rFMSDataService] Email claim received: {emailAddress}");
            }
            else
            {
                _logger.Info($"[rFMSDataService] Email claim FAILED: {emailAddress}");
                context.Fail();
                return;
            }

            try
            {
                var isExists = await _accountManager.CheckForFeatureAccessByEmailId(emailAddress, requirement.FeatureName);
                _logger.Info($"[rFMSDataService] Is user authorized: {isExists}");
                if (isExists)
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    httpContext.Items["AuthorizedFeature"] = requirement.FeatureName;
                    httpContext.Items["AuthorizedPaths"] = GetAuthorizedPaths(emailAddress, requirement.FeatureName);
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.Info($"[rFMSDataService] Email claim FAILED 1");
                    context.Fail();
                }
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("[rFMSDataService] Error occurred while authorizing the request", ex);
                context.Fail();
                return;
            }
        }

        private List<string> GetAuthorizedPaths(string email, string authorizedFeature)
        {
            List<string> authorizedPaths = new List<string>();
            List<string> lstFeatures = new List<string> { AccessPolicies.RFMS_VEHICLE_DATA_ACCESS_POLICY,
                                                          AccessPolicies.RFMS_VEHICLE_POSITION_ACCESS_POLICY,
                                                          AccessPolicies.RFMS_VEHICLE_STATUS_ACCESS_POLICY };
            foreach (var feature in lstFeatures)
            {
                if (feature != authorizedFeature)
                {
                    switch (feature)
                    {
                        case AccessPolicies.RFMS_VEHICLE_DATA_ACCESS_POLICY:
                            Task<bool> hasVehicleApiAccess = Task.Run<bool>(async () => await _accountManager.CheckForFeatureAccessByEmailId(email, AccessPolicies.RFMS_VEHICLE_DATA_ACCESS_POLICY));
                            if (hasVehicleApiAccess.Result)
                                authorizedPaths.Add("/vehicles");
                            break;
                        case AccessPolicies.RFMS_VEHICLE_POSITION_ACCESS_POLICY:
                            Task<bool> hasVehiclePositionApiAccess = Task.Run<bool>(async () => await _accountManager.CheckForFeatureAccessByEmailId(email, AccessPolicies.RFMS_VEHICLE_POSITION_ACCESS_POLICY));
                            if (hasVehiclePositionApiAccess.Result)
                                authorizedPaths.Add("/vehiclepositions");
                            break;
                        case AccessPolicies.RFMS_VEHICLE_STATUS_ACCESS_POLICY:
                            Task<bool> hasVehicleStatusApiAccess = Task.Run<bool>(async () => await _accountManager.CheckForFeatureAccessByEmailId(email, AccessPolicies.RFMS_VEHICLE_STATUS_ACCESS_POLICY));
                            if (hasVehicleStatusApiAccess.Result)
                                authorizedPaths.Add("/vehiclestatuses");
                            break;
                    }
                }
            }
            return authorizedPaths;
        }

    }
}
