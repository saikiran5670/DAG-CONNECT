package modules;

import static executionEngine.DriverScript.TestStep;
import com.relevantcodes.extentreports.LogStatus;
import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

public class User extends CommonFunctionLib{
	public static void verify30CharMaxLength() throws Exception {
		try {
			CommonFunctionLib.verifyMaxInputChar(30);				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void verify20CharMaxLength() throws Exception {
		try {
			CommonFunctionLib.verifyMaxInputChar(20);				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void selectRoleInUser() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP2_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/div/div";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL);
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void selectGroupInUser() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP3_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/div";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL);
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void selectRoleInUserPOPUP() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_POPUP_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL);
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void verifyUserPopUpColumnName() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_POPUP_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]";
			CommonFunctionLib.verifyColInTable(GRPTBL, COLHEAD, ColDiv, "Main");
			DriverScript.bResult = true;
				
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
