package modules;

import static executionEngine.DriverScript.TestStep;

import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.List;
import org.json.simple.JSONArray;
import org.openqa.selenium.WebElement;
import com.relevantcodes.extentreports.LogStatus;
import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

public class FleetUtilizationDB extends CommonFunctionLib{
	private static DecimalFormat df = new DecimalFormat("0.00");
	
	public static void verifyTotalDistance() throws Exception {		
		try {
			  String Startdatetime = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
			  String User = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			  String Enddatetime =ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
			  String VIN = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
			  String EField = ExcelSheet.getCellData(TestStep, Constants.Col_Parm4, Constants.Sheet_TestSteps);
			  String Col = ExcelSheet.getCellData(TestStep, Constants.Col_Parm5, Constants.Sheet_TestSteps);
			  String ExpField = TripReportDB.verifyTripData(EField, Startdatetime, Enddatetime, User, VIN);
			  System.out.println(ExpField +" DB Value Of " + Col);
			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
			  Log.info(ExpField +" DB Value Of " + Col);	
			  CommonFunctionLib.verifyDBDataInTable(ExpField, Col);
			
		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}
	
}
