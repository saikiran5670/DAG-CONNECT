using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc;

namespace net.atos.daf.ct2.packageservice.Common
{
    public class PackageCdcHelper
    {
        private readonly IPackageAlertCdcManager _packageMgmAlertCdcManager;

        public PackageCdcHelper(IPackageAlertCdcManager packageMgmAlertCdcManager)
        {
            _packageMgmAlertCdcManager = packageMgmAlertCdcManager;
        }
        public async Task TriggerPackageCdc(int packageid, string packageState)
        {
            _ = await Task.Run(() => _packageMgmAlertCdcManager.GetVehiclesAndAlertFromPackageConfiguration(packageid, packageState));
        }
    }
}
