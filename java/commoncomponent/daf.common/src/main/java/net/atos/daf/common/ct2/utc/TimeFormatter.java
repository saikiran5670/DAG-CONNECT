package net.atos.daf.common.ct2.utc;

import java.io.Serializable;
import java.text.ParseException;
import java.time.Instant;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Date;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.util.DAFConstants;

public class TimeFormatter implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static DateTimeFormatter isoInstantFormatter = DateTimeFormatter.ISO_INSTANT;
	
	/** The log object. */
    private static Logger logger = LoggerFactory.getLogger(TimeFormatter.class);
    
    /** singleton. */
    private static TimeFormatter singleton = null;

    private static ZoneId zoneId = null;

    /**
    * Singleton pattern.
    */
    private TimeFormatter() {
    }

    /**
    * Get the TimeFormatter instance.
    * @return TimeFormatter The singleton instance.
    * 
    */
    public static synchronized TimeFormatter getInstance() {
          if (null == singleton) {
               singleton = new TimeFormatter();
               zoneId = ZoneId.of(DAFConstants.ZONE_UTC);
          }
          return singleton;
    }
	
	/**
	 * This method returns the current UTC time
	 * return long
	 */
	public long getCurrentUTCTime() {
		return ZonedDateTime.now(zoneId).toInstant().toEpochMilli();
	}
	
	/**
	 * This method returns the current UTC time
	 * return long
	 */
	public long getCurrentUTCTimeInSec() {
		return ZonedDateTime.now(zoneId).toInstant().getEpochSecond();
	}
			
	/**
	 * This method subtract timeInSeconds from utcTime provided
	 * return long
	 */
	public long subMilliSecFromUTCTime(long utcTime, int timeInMilliSeconds) {
		return Instant.ofEpochMilli(utcTime).minusMillis(timeInMilliSeconds).toEpochMilli();
	}
	
	/**
	 * This method subtract timeInMilliSeconds from utcTime provided
	 * return long
	 */
//	public long subMilliSecFromUTCTime(long startTime, long endTime) {
//		return Instant.ofEpochMilli(endTime).minusMillis(startTime).toEpochMilli();
//	}
	
	/**
	 * This method subtract timeInMilliSeconds from utcTime provided
	 * return long
	 */
	public long subMilliSecFromUTCTime(long startTime, long timeInMilliSeconds) {
		return Instant.ofEpochMilli(startTime).minusMillis(timeInMilliSeconds).toEpochMilli();
	}
	
	/**
	 * This method subtract past UTC time from current UTC time provided
	 * return long
	 */
	public long subPastUtcTmFrmCurrentUtcTm(long startTime, long endTime) {
		return Instant.ofEpochMilli(endTime).minusMillis(startTime).toEpochMilli();
	}
	
	//TODO - Just to cross check , remove this
	public long subMilliSecFromUTCTimeTest(long startTime, long timeInMilliSeconds) {
		return new Date(startTime - timeInMilliSeconds).getTime();
	}
	
	/**
	 * This method subtract timeInSeconds from utcTime provided
	 * return long
	 */
	public long subSecondsFromUTCTime(long startTime, long timeInSeconds) {
		return Instant.ofEpochMilli(startTime).minusSeconds(timeInSeconds).toEpochMilli();
	}
	

	/**
	 * This method adds timeInMilliSeconds to utcTime provided
	 * return long
	 */
	public long addMilliSecToUTCTime(long utcTime, int timeInMilliSeconds) {
		return Instant.ofEpochMilli(utcTime).plusMillis(timeInMilliSeconds).toEpochMilli();
	}
	
	/**
	 * input long data type
	 * This method adds timeInMilliSeconds to utcTime provided
	 * return long
	 */
	public long addMilliSecToUTCTime(long utcTime, long timeInMilliSeconds) {
		return Instant.ofEpochMilli(utcTime).plusMillis(timeInMilliSeconds).toEpochMilli();
	}
	
	/**
	 * input long data type
	 * This method adds timeInSeconds to utcTime provided
	 * return long
	 */
	public long addSecondsToUTCTime(long utcTime, long timeInSeconds) {
		return Instant.ofEpochMilli(utcTime).plusSeconds(timeInSeconds).toEpochMilli();
	}
	
	//TODO Check the functionality
	/*public long getTimeDiffInMilli(long currentUtcTime, long pastUtcTime) {
		return Duration.between(Instant.ofEpochMilli(pastUtcTime), Instant.ofEpochMilli(currentUtcTime)).toMillis();
	}*/
	
	//TODO need to evaluate usecase of this scenario
	public long getUTCEpochMilli(LocalDateTime localdateTime) {
		return localdateTime.atZone(zoneId).toInstant().toEpochMilli();
	}
	
	public String getUTCStringFromEpochMilli(Long utcInMilliSec) {
		return Instant.ofEpochMilli(utcInMilliSec).toString();
	}
	
	/**
	 * input date string data type and date string format
	 * This method converts date string of provided format to epochMilli
	 * return long
	 */
	public long convertUTCToEpochMilli(String dateStr, String timeFormat) throws ParseException {
		DateTimeFormatter formatter = DateTimeFormatter.ofPattern(timeFormat);
		return ZonedDateTime.parse(dateStr, formatter.withZone(zoneId)).toInstant().toEpochMilli();
	}
	
	
	/**
	 * input start and end date string data type and date string format
	 * This method is used to calculate difference of time between given date string formats 
	 * return long
	 */
	public long getTimeDiffInMilli(String startTime, String endTime, String timeFormat) {
		DateTimeFormatter formatter = DateTimeFormatter.ofPattern(timeFormat);
		
		long endTm = ZonedDateTime.parse(endTime, formatter.withZone(zoneId)).toInstant().toEpochMilli();
		long startTm = ZonedDateTime.parse(startTime, formatter.withZone(zoneId)).toInstant().toEpochMilli();
		
		return Instant.ofEpochMilli(endTm).minusMillis(startTm).toEpochMilli();
	}
	
	public Long getUTCTime(String utcTime){	
		return ZonedDateTime.parse(utcTime, isoInstantFormatter.withZone(zoneId)).toInstant().toEpochMilli();
	}
	
	
	public Long getNewDate(String utcTime){	
		return ZonedDateTime.parse(utcTime, isoInstantFormatter.withZone(zoneId)).toInstant().toEpochMilli();
	}
}
