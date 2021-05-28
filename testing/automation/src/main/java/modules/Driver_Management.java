/**
 * 
 */
package modules;

import static executionEngine.DriverScript.TestStep;

import com.relevantcodes.extentreports.LogStatus;

import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

/**
 * @author A718166
 *
 */
public class Driver_Management extends CommonFunctionLib{
	public static void verifyDriverColumnName() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]";
				//div/div";
			CommonFunctionLib.verifyColInTable(GRPTBL, COLHEAD, ColDiv,"Main");
			;
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
	public static void verifyConsentIconInTbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");
			String TBLROW = getTextFromOR("GRP_ROW"); 
			String ColDiv = "]";
				//div/div";
			CommonFunctionLib.checkIconInTbl(GRPTBL, COLHEAD,TBLROW, ColDiv);
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
	public static void viewDriver() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/mat-cell";
			CommonFunctionLib.viewRecord(GRPTBL, COLHEAD, GRP_ROW, CELL);
			
				
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
