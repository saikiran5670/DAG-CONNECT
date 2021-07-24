package net.atos.daf.ct2.util;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;

public class Utils implements Serializable {

    private static final Logger logger = LoggerFactory.getLogger(Utils.class);
    private static ObjectMapper mapper;
    private static final long serialVersionUID = 1L;
    static {
        mapper = new ObjectMapper();
    }

    public static String writeValueAsString(Object obj){
        try {
            return mapper.writeValueAsString(obj);
        } catch (JsonProcessingException e) {
            return "";
        }
    }

    public static Object readValueAsObject(String json, Class clazz) throws Exception {
        try {
            return mapper.readValue(json,clazz);
        } catch (JsonProcessingException e) {
            return clazz.getDeclaredConstructor().newInstance();
        }
    }

    public static Object readValueAsObject(String json, TypeReference clazz) throws Exception {
        try {
            return mapper.readValue(json,clazz);
        } catch (JsonProcessingException e) {
            return null;
        }
    }
}
