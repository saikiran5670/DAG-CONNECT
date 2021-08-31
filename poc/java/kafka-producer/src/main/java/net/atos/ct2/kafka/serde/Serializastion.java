package net.atos.ct2.kafka.serde;

import net.atos.daf.ct2.serde.KafkaMessageSerializeSchema;
import org.apache.commons.io.Charsets;
import org.apache.flink.api.common.serialization.SerializationSchema;
import org.apache.kafka.common.serialization.Serializer;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.util.Map;

public class Serializastion<T>  implements Serializer<T> {

    @Override
    public byte[] serialize(String s, T t) {
        byte[] bytes = null;
        ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        if (t.getClass() == String.class) {
            return ((String)t).getBytes(Charsets.UTF_8);
        } else {
            try {
                ObjectOutputStream objectOutputStream = new ObjectOutputStream(byteArrayOutputStream);
                objectOutputStream.writeObject(t);
                objectOutputStream.flush();
                bytes = byteArrayOutputStream.toByteArray();
                byteArrayOutputStream.close();
            } catch (IOException var5) {
                System.err.println("Unable to Read Object \n"+var5);
            }

            return bytes;
        }
    }

    @Override
    public void configure(Map<String, ?> configs, boolean isKey) {
        Serializer.super.configure(configs, isKey);
    }

    @Override
    public void close() {
        Serializer.super.close();
    }
}
