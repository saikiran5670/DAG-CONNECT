using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface IVehicleCdcManager
    {
        Task VehicleCdcProducer(List<int> vehicleCdcList, entity.KafkaConfiguration kafkaConfiguration);

        //Task VehicleCdcConsumer(KafkaEntity kafkaEntity);
    }
}