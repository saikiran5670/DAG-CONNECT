package modules;

import static executionEngine.DriverScript.TestStep;
import java.util.List;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import com.relevantcodes.extentreports.LogStatus;
import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

public class User_Group extends CommonFunctionLib {	
	public static void verifyUserColumnName() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]/div";
			CommonFunctionLib.verifyColInTable(GRPTBL, COLHEAD, ColDiv, "Sub");
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
	public static void verifyUserGrpColumnName() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]";
			CommonFunctionLib.verifyColInTable(GRPTBL, COLHEAD, ColDiv,"Main");
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
	
	public static void verifySearchTxtValue() throws Exception {
		try {
			CommonFunctionLib.VerifyCSSValue("placeholder");
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
	public static void selectUser() throws Exception {
		try {
			//String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/div";
			CommonFunctionLib.selectCheckBoxInTbl("", COLHEAD, GRP_ROW, CELL,"");
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	
	
	public static void verifyUserGroupCount() throws Exception {
		try {		
			CommonFunctionLib.verifyCountInDetailsLbl("GRP_DETAILS_LB");				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void verify60CharMaxLength() throws Exception {
		try {
			CommonFunctionLib.verifyMaxInputChar(60);				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void verify120CharMaxLength() throws Exception {
		try {
			CommonFunctionLib.verifyMaxInputChar(120);				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void verifyUserCount() throws Exception {
		try {
			int ColNo = 0; 
			String column = "Accounts";
			List<WebElement> options = driver.findElements(getLocator("GRP_COLUMNHEADER"));
			Thread.sleep(3000);	
			for (int i = 1; i <= options.size(); i++) 
			{
			String PartialcolnameF =  getTextFromOR("PART_COL_F_N_FIRST");
			String PartialcolnameS =  getTextFromOR("PART_COL_F_N_SEC");
			String colnameF = driver.findElement(By.xpath(PartialcolnameF + i + PartialcolnameS)).getText();
			String colname = colnameF.trim();
			if (colname.equals(column.trim())) 
			{
			System.out.println(column);
			System.out.println(colname);
			ColNo= i;
			}
			}
			CommonFunctionLib.clickOnCount(ColNo,"","", "Main");				
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