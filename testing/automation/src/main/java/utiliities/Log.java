package utiliities;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import modules.CommonFunctionLib;

public class Log 
{
	
	private static final Logger Log = LogManager.getLogger(Log.class.getName());
    
	
	public static void startTestCase (String TestCaseName) 
	{
		
		Log.info("*************************************************************************************");
		Log.info("***************************" + TestCaseName + "**************************************");
		Log.info("*************************************************************************************");
		
		CommonFunctionLib.test = CommonFunctionLib.reports.startTest(TestCaseName);
	}
	public static void startTestCases(String testName, String description) {
		Log.info("*************************************************************************************");
		Log.info("***************************" + testName + "**************************************");
		Log.info("*************************************************************************************");
		
		CommonFunctionLib.test = CommonFunctionLib.reports.startTest(testName,description);
	}
	
    public static void endTestCase (String TestCaseName) 
    {
		
		Log.info("*************************************************************************************");
		Log.info("***************************" + "E---N---D" + "***************************************");
		Log.info("*************************************************************************************");
		
		 CommonFunctionLib.reports.endTest(CommonFunctionLib.test);
	}
  
    
	
    public static void info(String message) 
    {
    	Log.info(message);
    }
    
    public static void warn(String message)
    {
    	Log.warn(message);
    }
    public static void error(String message)
    {
    	Log.error(message);
    }
    public static void fatal(String message)
    {
    	Log.fatal(message);
    }
    public static void debug(String message)
    {
    	Log.debug(message);
    }
}
