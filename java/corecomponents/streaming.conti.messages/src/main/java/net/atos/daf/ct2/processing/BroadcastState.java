package net.atos.daf.ct2.processing;

import lombok.NoArgsConstructor;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.ct2.pojo.Message;
import org.apache.flink.api.common.state.MapStateDescriptor;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.common.typeinfo.TypeInformation;

@NoArgsConstructor
public class BroadcastState<UK, UV> {

    public MapStateDescriptor<Message<UK>, KafkaRecord<UV>> stateInitialization(String broadcastName) {

        return new MapStateDescriptor<Message<UK>, KafkaRecord<UV>>(
                broadcastName,
                TypeInformation.of(
                        new TypeHint<Message<UK>>() {

                            @Override
                            public TypeInformation<Message<UK>> getTypeInfo() {
                                return super.getTypeInfo();
                            }
                        }),
                TypeInformation.of(
                        new TypeHint<KafkaRecord<UV>>() {

                            @Override
                            public TypeInformation<KafkaRecord<UV>> getTypeInfo() {
                                return super.getTypeInfo();
                            }
                        }));


    }
}
