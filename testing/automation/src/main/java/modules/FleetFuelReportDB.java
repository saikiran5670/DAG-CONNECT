package modules;

import static executionEngine.DriverScript.TestStep;

import java.text.DecimalFormat;

import com.relevantcodes.extentreports.LogStatus;

import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

public class FleetFuelReportDB  extends CommonFunctionLib{

private static DecimalFormat df = new DecimalFormat("0.00");
	
	public static void verifyTotalFuelConsumed() throws Exception {		
		try {
			  String Startdatetime = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
			  String User = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			  String Enddatetime =ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
			  String VIN = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
			  String EField = ExcelSheet.getCellData(TestStep, Constants.Col_Parm4, Constants.Sheet_TestSteps);
			  String Col = ExcelSheet.getCellData(TestStep, Constants.Col_Parm5, Constants.Sheet_TestSteps);
			  String ExpField = TripReportDB.verifyTripData(EField, Startdatetime, Enddatetime, User, VIN);
			  			  
			  System.out.println("Verify Total Fuel consumed in summary section");
			  test.log(LogStatus.INFO,  "Verify Total Fuel consumed in summary section");
			  Log.info("Verify Total Fuel consumed in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Fuel_ConsumedValue")).getText();
			  System.out.println(ExpField +" DB Value Of " + Col);
			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
			  Log.info(ExpField +" DB Value Of " + Col);
			  System.out.println(TD +" UI Value Of Total Fuel consumed in summary section");
			  test.log(LogStatus.INFO,  TD +" UI Value Of Total Fuel consumed in summary section");
			  Log.info(TD +" UI Value Of Total Fuel consumed in summary section");
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
	
	
	public static void verifyFuelConsumption() throws Exception {		
		try {
			  String Startdatetime = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
			  String User = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			  String Enddatetime =ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
			  String VIN = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
			  String EField = ExcelSheet.getCellData(TestStep, Constants.Col_Parm4, Constants.Sheet_TestSteps);
			  String Col = ExcelSheet.getCellData(TestStep, Constants.Col_Parm5, Constants.Sheet_TestSteps);
			  String ExpField = TripReportDB.verifyTripData(EField, Startdatetime, Enddatetime, User, VIN);
			  			  
			  System.out.println("Verify Total Fuel Consumption in summary section");
			  test.log(LogStatus.INFO,  "Verify Total Fuel Consumption in summary section");
			  Log.info("Verify Total Fuel Consumption in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Fuel_ConsumptionValue")).getText();
			  System.out.println(ExpField +" DB Value Of " + Col);
			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
			  Log.info(ExpField +" DB Value Of " + Col);
			  System.out.println(TD +" UI Value Of Total Fuel Consumption in summary section");
			  test.log(LogStatus.INFO,  TD +" UI Value Of Total Fuel Consumption in summary section");
			  Log.info(TD +" UI Value Of Total Fuel Consumption in summary section");
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
	public static void verifyIdleDurationINFF() throws Exception {		
		try {
			  String Startdatetime = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
			  String User = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			  String Enddatetime =ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
			  String VIN = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
			  String EField = ExcelSheet.getCellData(TestStep, Constants.Col_Parm4, Constants.Sheet_TestSteps);
			  String Col = ExcelSheet.getCellData(TestStep, Constants.Col_Parm5, Constants.Sheet_TestSteps);
			  String ExpField = TripReportDB.verifyTripData(EField, Startdatetime, Enddatetime, User, VIN);
//			  System.out.println(ExpField +" DB Value Of " + Col);
//			  test.log(LogStatus.INFO,  ExpField +" DB Value Of " + Col);
//			  Log.info(ExpField +" DB Value Of " + Col);	
//			  CommonFunctionLib.verifyDBDataInTable(ExpField, Col);
			  			  
			  System.out.println("Verify Idle Duration in summary section");
			  test.log(LogStatus.INFO,  "Verify Idle Duration in summary section");
			  Log.info("Verify Idle Duration in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Fuel_IdealDurationValue")).getText();
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
	public static void verifyTotalDistanceFF() throws Exception {		
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
			  String table = "Fleet_Fuel_Report_Tbl";
			  CommonFunctionLib.verifyDBDataInTable(ExpField, Col, table);
			  			  
			  System.out.println("Verify Total Distance in summary section");
			  test.log(LogStatus.INFO,  "Verify Total Distance in summary section");
			  Log.info("Verify Total Distance in summary section");	
			  
			  String TD = driver.findElement(getLocator("Fleet_Fuel_DistanceValue")).getText();
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
		public static void verifyTotalTripFF() throws Exception {		
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
				  String table = "Fleet_Fuel_Report_Tbl";;
				  CommonFunctionLib.verifyDBDataInTable(ExpField, Col, table);
				  			  
				  System.out.println("Verify Number of Trips in summary section");
				  test.log(LogStatus.INFO,  "Verify Number of Trips in summary section");
				  Log.info("Verify Number of Trips in summary section");	
				  
				  String TD = driver.findElement(getLocator("Fleet_Fuel_NoOfTripsValue")).getText();
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

	
}
