package net.atos.daf.ct2.processing;

import lombok.NoArgsConstructor;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;

@NoArgsConstructor
public class BroadcastState<U> {

  public MapStateDescriptor<Message<U>, KafkaRecord<U>> stateInitialization(String broadcastName) {

    return new MapStateDescriptor<Message<U>, KafkaRecord<U>>(
        broadcastName,
        TypeInformation.of(
            new TypeHint<Message<U>>() {

              @Override
              public TypeInformation<Message<U>> getTypeInfo() {
                return super.getTypeInfo();
              }
            }),
        TypeInformation.of(
            new TypeHint<KafkaRecord<U>>() {

              @Override
              public TypeInformation<KafkaRecord<U>> getTypeInfo() {
                return super.getTypeInfo();
              }
            }));
  }
}
