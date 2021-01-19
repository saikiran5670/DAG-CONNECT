package net.atos.daf.common.ct2.utc;

import java.io.Serializable;
import java.text.ParseException;
import java.time.Instant;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;

public class TimeFormatter implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	
	/**
	 * This method returns the current UTC time
	 * return long
	 */
	public static long getCurrentUTCTime() {
		return ZonedDateTime.now(ZoneId.of("UTC")).toInstant().toEpochMilli();
	}
	
	/*public static long subSecondsFromUTCTime(long utcTime, int timeInSeconds) {
		return Instant.ofEpochMilli(utcTime).minusSeconds(timeInSeconds).toEpochMilli();
	}*/
	
	/**
	 * This method subtract timeInSeconds from utcTime provided
	 * return long
	 */
	public static long subMilliSecFromUTCTime(long utcTime, int timeInSeconds) {
		return Instant.ofEpochMilli(utcTime).minusMillis(timeInSeconds).toEpochMilli();
	}
	
	/*public static long subMilliSecFromUTCTime(long utcTime, long timeInSeconds) {
		return Instant.ofEpochMilli(utcTime).minusMillis(timeInSeconds).toEpochMilli();
	}*/
	
	/**
	 * This method subtract timeInMilliSeconds from utcTime provided
	 * return long
	 */
	public static long subMilliSecFromUTCTime(long startTime, long endTime) {
		return Instant.ofEpochMilli(endTime).minusMillis(startTime).toEpochMilli();
	}
	
	/*public static long addSecondsToUTCTime(long utcTime, int timeInSeconds) {
		return Instant.ofEpochMilli(utcTime).plusSeconds(timeInSeconds).toEpochMilli();
	}*/
	
	public static long addMilliSecToUTCTime(long utcTime, int timeInSeconds) {
		return Instant.ofEpochMilli(utcTime).plusMillis(timeInSeconds).toEpochMilli();
	}
	
	public static long addMilliSecToUTCTime(long utcTime, long timeInSeconds) {
		return Instant.ofEpochMilli(utcTime).plusMillis(timeInSeconds).toEpochMilli();
	}
	
	//TODO Check the functionality
	/*public static long getTimeDiffInMilli(long currentUtcTime, long pastUtcTime) {
		return Duration.between(Instant.ofEpochMilli(pastUtcTime), Instant.ofEpochMilli(currentUtcTime)).toMillis();
	}*/
	
	public static long getUTCEpochMilli(LocalDateTime localdateTime) {
		return localdateTime.atZone(ZoneId.of("UTC")).toInstant().toEpochMilli();
	}
	
	public static String getUTCStringFromEpochMilli(Long utcInMilliSec) {
		return Instant.ofEpochMilli(utcInMilliSec).toString();
	}
	
	public static Long convertUTCToEpochMilli(String dateStr, String timeFormat) throws ParseException {
		DateTimeFormatter formatter = DateTimeFormatter.ofPattern(timeFormat);
		return ZonedDateTime.parse(dateStr, formatter.withZone(ZoneId.of("UTC"))).toInstant().toEpochMilli();
	}
	
	
	public static long getTimeDiffInMilli(String startTime, String endTime, String timeFormat) {
		DateTimeFormatter formatter = DateTimeFormatter.ofPattern(timeFormat);
		
		long endTm = ZonedDateTime.parse(endTime, formatter.withZone(ZoneId.of("UTC"))).toInstant().toEpochMilli();
		long startTm = ZonedDateTime.parse(startTime, formatter.withZone(ZoneId.of("UTC"))).toInstant().toEpochMilli();
		
		return Instant.ofEpochMilli(endTm).minusMillis(startTm).toEpochMilli();
	}
}
