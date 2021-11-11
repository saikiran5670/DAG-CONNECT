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
			  String table = "";
			  CommonFunctionLib.verifyDBDataInTable(ExpField, Col, table);
			  			  
			  System.out.println("Verify Total Distance in summary section");
			  test.log(LogStatus.INFO,  "Verify Total Distance in summary section");
			  Log.info("Verify Total Distance in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Utilisation_Summary_TotalDistanceValue")).getText();
			  System.out.println(ExpField +" DB Value Of " + Col);
			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
			  Log.info(ExpField +" DB Value Of " + Col);
			  System.out.println(TD +" UI Value Of Total Distance in summary section");
			  test.log(LogStatus.INFO,  TD +" UI Value Of Total Distance in summary section");
			  Log.info(TD +" UI Value Of Total Distance in summary section");
			  String[] TotalD =TD.split(" ");
			  if(TotalD[0].equalsIgnoreCase(ExpField)) {
				System.out.println(TotalD[0] +" Value Matched");
				test.log(LogStatus.PASS,  TotalD[0] +" Value Matched");
				Log.info(TotalD[0] +" Value Matched");	
			   }else {
				System.out.println(TotalD[0] +" Value Not Matched");
				test.log(LogStatus.PASS,  TotalD[0] +" Value Not Matched");
				Log.info(TotalD[0] +" Value Not Matched");
			   }
			
		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}
//----------------
	public static void verifyTotalTrip() throws Exception {		
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
			  String table = "";
			  CommonFunctionLib.verifyDBDataInTable(ExpField, Col, table);
			  			  
			  System.out.println("Verify Number of Trips in summary section");
			  test.log(LogStatus.INFO,  "Verify Number of Trips in summary section");
			  Log.info("Verify Number of Trips in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Utilisation_Summary_NumberOfTripsValue")).getText();
			  System.out.println(ExpField +" DB Value Of " + Col);
			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
			  Log.info(ExpField +" DB Value Of " + Col);
			  System.out.println(TD +" UI Value for Number of Trips in summary section");
			  test.log(LogStatus.INFO,  TD +" UI Value for Number of Trips in summary section");
			  Log.info(TD +" UI Value for Number of Trips in summary section");
			  if(TD.equalsIgnoreCase(ExpField)) {
				System.out.println(TD +" Value Matched");
				test.log(LogStatus.PASS,  TD +" Value Matched");
				Log.info(TD +" Value Matched");	
			   }else {
				System.out.println(TD +" Value Not Matched");
				test.log(LogStatus.PASS,  TD +" Value Not Matched");
				Log.info(TD +" Value Not Matched");
			   }
			
		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}

//-------------------------------
	public static void verifyNoOfVehicle() throws Exception {		
		try {
			  String Startdatetime = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
			  String User = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			  String Enddatetime =ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
			  String VIN = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
			  String EField = ExcelSheet.getCellData(TestStep, Constants.Col_Parm4, Constants.Sheet_TestSteps);
			  //String Col = ExcelSheet.getCellData(TestStep, Constants.Col_Parm5, Constants.Sheet_TestSteps);
			  String ExpField = TripReportDB.verifyTripData(EField, Startdatetime, Enddatetime, User, VIN);
//			  System.out.println(ExpField +" DB Value Of " + Col);
//			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
//			  Log.info(ExpField +" DB Value Of " + Col);	
//			 // CommonFunctionLib.verifyDBDataInTable(ExpField, Col);
//			  			  
			  System.out.println("Verify Number of Vehicles in summary section");
			  test.log(LogStatus.INFO,  "Verify Number of Vehicles in summary section");
			  Log.info("Verify Number of Vehicles in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Utilisation_Summary_NumberOfVehiclesValue")).getText();
			  System.out.println(ExpField +" DB Value for Number of Vehicles");
			  test.log(LogStatus.INFO,  ExpField +" DB Value for Number of Vehicles");
			  Log.info(ExpField +" DB Value for Number of Vehicles");
			  System.out.println(TD +" UI Value for Number of Vehicles in summary section");
			  test.log(LogStatus.INFO,  TD +" UI Value for Number of Vehicles in summary section");
			  Log.info(TD +" UI Value for Number of Vehicles in summary section");
			  if(TD.equalsIgnoreCase(ExpField)) {
				System.out.println(TD +" Value Matched");
				test.log(LogStatus.PASS,  TD +" Value Matched");
				Log.info(TD +" Value Matched");	
			   }else {
				System.out.println(TD +" Value Not Matched");
				test.log(LogStatus.PASS,  TD +" Value Not Matched");
				Log.info(TD +" Value Not Matched");
			   }
			
		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}
//----------------------------
	
	public static void verifyAvgDistancePerDay() throws Exception {		
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
			  String table = "";
			  CommonFunctionLib.verifyDBDataInTable(ExpField, Col, table);
			  			  
			  System.out.println("Verify Average Distance per day in summary section");
			  test.log(LogStatus.INFO,  "Verify Average Distance per day in summary section");
			  Log.info("Verify Average Distance per day in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Utilisation_Summary_AverageDistancePerDayValue")).getText();
			  System.out.println(ExpField +" DB Value Of " + Col);
			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
			  Log.info(ExpField +" DB Value Of " + Col);
			  System.out.println(TD +" UI Value Of Average Distance per day in summary section");
			  test.log(LogStatus.INFO,  TD +" UI Value Of Average Distance per day in summary section");
			  Log.info(TD +" UI Value Of Average Distance per day in summary section");
			  String[] TotalD =TD.split(" ");
			  if(TotalD[0].equalsIgnoreCase(ExpField)) {
				System.out.println(TotalD[0] +" Value Matched");
				test.log(LogStatus.PASS,  TotalD[0] +" Value Matched");
				Log.info(TotalD[0] +" Value Matched");	
			   }else {
				System.out.println(TotalD[0] +" Value Not Matched");
				test.log(LogStatus.PASS,  TotalD[0] +" Value Not Matched");
				Log.info(TotalD[0] +" Value Not Matched");
			   }
			
		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}
	
//----------------------------	
	public static void verifyIdleDuration() throws Exception {		
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
			  String table ="";
			  CommonFunctionLib.verifyDBDataInTable(ExpField, Col, table);
			  			  
			  System.out.println("Verify Idle Duration in summary section");
			  test.log(LogStatus.INFO,  "Verify Idle Duration in summary section");
			  Log.info("Verify Idle Duration in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Utilisation_Summary_IdleDurationValue")).getText();
			  System.out.println(ExpField +" DB Value Of " + Col);
			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
			  Log.info(ExpField +" DB Value Of " + Col);
			  System.out.println(TD +" UI Value Of Idle Duration in summary section");
			  test.log(LogStatus.INFO,  TD +" UI Value Of Idle Duration in summary section");
			  Log.info(TD +" UI Value Of Idle Duration in summary section");
			  String[] TotalD =TD.split(" ");
			  if(TotalD[0].equalsIgnoreCase(ExpField)) {
				System.out.println(TotalD[0] +" Value Matched");
				test.log(LogStatus.PASS,  TotalD[0] +" Value Matched");
				Log.info(TotalD[0] +" Value Matched");	
			   }else {
				System.out.println(TotalD[0] +" Value Not Matched");
				test.log(LogStatus.PASS,  TotalD[0] +" Value Not Matched");
				Log.info(TotalD[0] +" Value Not Matched");
			   }
			
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
