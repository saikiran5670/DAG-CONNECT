package net.atos.daf.ct2.utils;

import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.MapperFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import java.text.DateFormat;
import java.text.SimpleDateFormat;

public class JsonMapper {

	  private static Logger log = LogManager.getLogger(JsonMapper.class);

	  public static ObjectMapper configuring() {
	   // DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm a z");
		  DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
		  

	    ObjectMapper objectMapper = new ObjectMapper();
	    objectMapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
	    objectMapper.configure(DeserializationFeature.FAIL_ON_NULL_FOR_PRIMITIVES, false);
	    objectMapper.configure(DeserializationFeature.FAIL_ON_NUMBERS_FOR_ENUMS, false);
	    objectMapper.configure(MapperFeature.ACCEPT_CASE_INSENSITIVE_PROPERTIES, true);

	    objectMapper.setDateFormat(dateFormat);
	   // log.info("JSON-Object Mapping Configuring Done.");

	    return objectMapper;
	  }
	}
