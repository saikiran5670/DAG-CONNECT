package net.atos.daf.common.ct2.utc;

import static org.junit.Assert.assertTrue;

import org.junit.Test;

import junit.framework.Assert;

public class TimeFormaterTest {

	@Test
	public void testGetCurrentUtcTime() throws Exception {
		assertTrue("Current UTC time greater than previous time :: ",
				TimeFormatter.getCurrentUTCTime() > 1611055667622L);
	}

	@Test
	public void testSubMilliSecFromUTCTime() throws Exception {
		Assert.assertEquals(1611064206919L, TimeFormatter.subMilliSecFromUTCTime(1611064208919L, 2000));
	}

	@Test
	public void testSubMSFromUTCTime() throws Exception {
		Assert.assertEquals(1611064206919L, TimeFormatter.subMilliSecFromUTCTime(1611064208919L, 2000L));
	}

	@Test
	public void testSubPastUtcTmFrmCurrentUtcTm() throws Exception {
		Assert.assertEquals(2000L, TimeFormatter.subPastUtcTmFrmCurrentUtcTm(1611064206919L, 1611064208919L));
	}

	@Test
	public void testSubSecondsFromUTCTime() throws Exception {
		Assert.assertEquals(1611064206919L, TimeFormatter.subSecondsFromUTCTime(1611064208919L, 2L));
	}

	@Test
	public void testAddMilliSecToUTCTime() throws Exception {
		Assert.assertEquals(1611150608919L, TimeFormatter.addMilliSecToUTCTime(1611064208919L, 86400000L));
	}

	@Test
	public void addMSToUTCTime() throws Exception {
		Assert.assertEquals(1611064209919L, TimeFormatter.addMilliSecToUTCTime(1611064208919L, 1000L));
	}

	@Test
	public void testAddSecondsToUTCTime() throws Exception {
		Assert.assertEquals(1611064210919L, TimeFormatter.addSecondsToUTCTime(1611064208919L, 2L));
	}

	@Test
	public void testConvertUTCToEpochMilli() throws Exception {
		Assert.assertEquals(1611064208919L,
				TimeFormatter.convertUTCToEpochMilli("2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
	}

	@Test
	public void testGetTimeDiffInMilli() throws Exception {
		Assert.assertEquals(2000L, TimeFormatter.getTimeDiffInMilli("2021-01-19T13:50:06.919Z",
				"2021-01-19T13:50:08.919Z", "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"));
	}

}
