package net.atos.daf.common.ct2.utc;

import java.util.Arrays;

import org.junit.Test;

import junit.framework.Assert;

public class TimeFormaterTest {
	
	TimeFormatter obj = new TimeFormatter();
	
	@Test
	public void testCurrentUtilTime() throws Exception {
		
		System.out.println("hello");
		System.out.println(obj.getCurrentUTCTime());
		System.out.println(obj.getUTCStringFromEpochMilli(1610345862505L));
		//System.out.println(" ts :: "+ obj.convertDateStrToTimestamp("2020-08-06T16:42:19.000Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
		
		//Testing of Trip fields
		System.out.println("start time :: "+obj.getUTCStringFromEpochMilli(1596775285000L));
		System.out.println("end time :: "+obj.getUTCStringFromEpochMilli(  1596775380000L));
		System.out.println(" tim ediff  time :: "+obj.subMilliSecFromUTCTime(1596775285000L,1596775380000L ));
		System.out.println(" Adding time :: "+obj.addMilliSecToUTCTime(1596775285000L,95000L ));
		
		Long addMilli = obj.addMilliSecToUTCTime(1610345862505L, 60000);
		System.out.println(addMilli);
		System.out.println(obj.getUTCStringFromEpochMilli(addMilli));
		
		Long subMilli = obj.subMilliSecFromUTCTime(1610345862505L, 60000L);
		System.out.println(subMilli);
		System.out.println(obj.getUTCStringFromEpochMilli(subMilli));
		
		
		
		System.out.println("fue :: "+(38421 * (8.5 /10000000)));
		
		
		//Assert.assertEquals(ZonedDateTime.now().toInstant().toEpochMilli(), obj.getCurrentUTCTime());
		Assert.assertEquals(Long.valueOf(1596732139000L), obj.convertUTCToEpochMilli("2020-08-06T16:42:19.000Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
	//long aa =obj.testutc();
	//	System.out.println(aa);
	}
	
	
	/*public static long testutc()
	{
		DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
		final String dateTime= "2020-08-06T16:42:19.000Z";
		
		System.out.println("time =" + ZonedDateTime.parse(dateTime, formatter.withZone(ZoneId.of("UTC"))).toInstant().toEpochMilli());
		return ZonedDateTime.parse(dateTime, formatter.withZone(ZoneId.of("UTC"))).toInstant().toEpochMilli();
		
	}*/
}
