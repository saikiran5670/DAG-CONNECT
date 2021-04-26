
using System.Threading.Tasks;

namespace net.atos.daf.ct2.tcucore
{
    public interface ITcuProvisioningDataPost
    {
        Task PostTcuProvisioningMesssageToDAF(TCUDataSend tcuDataSend);
    }
}
