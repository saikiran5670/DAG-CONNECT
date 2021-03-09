package net.atos.daf.common.ct2.utc;

import static org.junit.Assert.assertTrue;

import org.junit.Test;

import junit.framework.Assert;

public class TimeFormaterTest {

	@Test
	public void testGetCurrentUtcTime() throws Exception {
	//	System.out.println("TimeFormatter.getInstance().getCurrentUTCTime() :: "+TimeFormatter.getInstance().getCurrentUTCTime());
		assertTrue("Current UTC time greater than previous time :: ",
				TimeFormatter.getInstance().getCurrentUTCTime() > 1611055667622L);
	}

	@Test
	public void testSubMilliSecFromUTCTime() throws Exception {
		Assert.assertEquals(1611064206919L, TimeFormatter.getInstance().subMilliSecFromUTCTime(1611064208919L, 2000));
	}

	@Test
	public void testSubMSFromUTCTime() throws Exception {
		Assert.assertEquals(1611064206919L, TimeFormatter.getInstance().subMilliSecFromUTCTime(1611064208919L, 2000L));
	}

	@Test
	public void testSubPastUtcTmFrmCurrentUtcTm() throws Exception {
		Assert.assertEquals(2000L, TimeFormatter.getInstance().subPastUtcTmFrmCurrentUtcTm(1611064206919L, 1611064208919L));
	}

	@Test
	public void testSubSecondsFromUTCTime() throws Exception {
		Assert.assertEquals(1611064206919L, TimeFormatter.getInstance().subSecondsFromUTCTime(1611064208919L, 2L));
	}

	@Test
	public void testAddMilliSecToUTCTime() throws Exception {
		Assert.assertEquals(1611150608919L, TimeFormatter.getInstance().addMilliSecToUTCTime(1611064208919L, 86400000L));
	}

	@Test
	public void addMSToUTCTime() throws Exception {
		Assert.assertEquals(1611064209919L, TimeFormatter.getInstance().addMilliSecToUTCTime(1611064208919L, 1000L));
	}

	@Test
	public void testAddSecondsToUTCTime() throws Exception {
		Assert.assertEquals(1611064210919L, TimeFormatter.getInstance().addSecondsToUTCTime(1611064208919L, 2L));
	}

	@Test
	public void testConvertUTCToEpochMilli() throws Exception {
		//System.out.println(TimeFormatter.getInstance().convertUTCToEpochMilli("2020-11-02 16:55:35.0", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
		Assert.assertEquals(1611064208919L,
				TimeFormatter.getInstance().convertUTCToEpochMilli("2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
	}
	

	@Test
	public void testGetTimeDiffInMilli() throws Exception {
		/*System.out.println(TimeFormatter.getInstance().getTimeDiffInMilli("2021-01-19T13:50:06.919Z",
				"2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
		
		System.out.println("converted 2 secs to hours :: "+TimeUnit.HOURS.convert(TimeFormatter.getInstance().getTimeDiffInMilli("2021-01-19T13:50:06.919Z",
				"2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"),TimeUnit.MILLISECONDS));
		
		System.out.println("converted 2 secs to hours :: "+TimeUnit.MILLISECONDS.toHours(TimeFormatter.getInstance().getTimeDiffInMilli("2021-01-19T13:50:06.919Z",
				"2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")));
		Long aa= TimeFormatter.getInstance().getTimeDiffInMilli("2021-01-19T13:50:06.919Z",
				"2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
		double bb = aa.doubleValue();
		System.out.println("hr is :: "+bb/3600000);*/
		
		Assert.assertEquals(2000L, TimeFormatter.getInstance().getTimeDiffInMilli("2021-01-19T13:50:06.919Z",
				"2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
	}
	
	@Test
	public void testGetUTCTime() throws Exception {
		System.out.println("UTCTime :::" + TimeFormatter.getInstance().getUTCTime("2021-01-19T13:50:06.919Z"));
		//System.out.println("UTCTime2 :::" + TimeFormatter.getInstance().getUTCTime("2020-01-01T01:23:34"));
		//System.out.println("UTCTime3 :::" + TimeFormatter.getInstance().getUTCTime("Mon Nov 02 16:55:37 UTC 2020"));
		System.out.println(TimeFormatter.getInstance().subPastUtcTmFrmCurrentUtcTm(1611064206919L, 1611064208919L));
		
		//TODO - Just to cross check , remove this
		//public static long subMilliSecFromUTCTimeTest(long startTime, long timeInMilliSeconds) 
		
	}
	
	
	
	
	
}
