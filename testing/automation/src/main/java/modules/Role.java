/**
 * 
 */
package modules;

import static executionEngine.DriverScript.TestStep;

import java.util.List;

import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.interactions.Actions;

import com.relevantcodes.extentreports.LogStatus;

import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

/**
 * @author A718166
 *
 */
public class Role extends CommonFunctionLib  {
	public static void viewRole() throws Exception {
		try {
			String GRPTBL = getTextFromOR("ROLL_TBL"); 
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
	}//mat-row[@role="row"][2]/mat-cell[1]/mat-icon
	
	public static void VerifyGlobalRole() throws Exception {
		try {
			String GRPTBL = getTextFromOR("ROLL_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/mat-cell";
			CommonFunctionLib.VerifyGlobalIcon(GRPTBL, COLHEAD, GRP_ROW, CELL);
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static boolean copyRole() throws Exception {
		Thread.sleep(3000);
		try 
		{
		Actions actions = new Actions(driver);
		actions.sendKeys(Keys.PAGE_UP).perform();
		String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		//String Yes_No = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
		List<WebElement> options = driver.findElements(getLocator("GRP_COLUMNHEADER"));
		boolean temp = false;
		int ActionColNo = 0;
		Thread.sleep(3000);
		for (int a = 1; a <= options.size(); a++) {
			String ANo =  getTextFromOR("GRP_COLUMNHEADER")+ "[" + a + "]";
			String ActionCol = driver.findElement(By.xpath(ANo)).getText();
			String ActionColN = ActionCol.trim();
			if (ActionColN.equals("Action")|| ActionColN.equals("Actions"))  
			{
				ActionColNo = a;
				break;
			}	
		}
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
		String Page = driver.findElement(getLocator("PAGINATION")).getText();
		String TotalRecord =Page.split(" ")[1];
		String Rowcount =driver.findElement(getLocator("GRP_ROW_COUNT_VAL")).getText();
		int Page_No =1; //Integer.parseInt(TotalRecord);
		int remainder = 0;
		if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
		{
			Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
			remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
			if(remainder>0) {
				Page_No=Page_No+1;
			}
		}else if(Integer.parseInt(TotalRecord)< Integer.parseInt(Rowcount)) {
			Page_No =1;
		}
		Thread.sleep(3000);
		for (int k = 1; k <= Page_No; k++) 
		{
		waitForLoadingImage();
		if (driver.findElement(getLocator("TABLE")).isDisplayed());
		{
		System.out.println(" Next Page button is working");
		Thread.sleep(3000);
		List<WebElement> options1 = driver.findElements(getLocator("GRP_ROW"));
		Thread.sleep(3000);
		for (int j = 1; j <= options1.size(); j++) 
		{
		String RowPart = getTextFromOR("TABLE_ROW_PART_ONE");
		String rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]")).getText();
		String[] str;
		if(column.equals("Vehicle Group")||column.equals("Vehicle Group/Vehicle")) {
			 str = rowvalueF.split("\\r?\\n");
			 rowvalueF = str[0];
		}
		String rowvalue = rowvalueF.trim();
		if (rowvalue.equals(value.trim())) 
		{
		System.out.println(value);
		System.out.println(rowvalue);
		System.out.println("Value found in expected column");
		test.log(LogStatus.PASS,  "Value found in expected column");
		Log.info("Value found in expected column");	
		String Btn_Copy = getTextFromOR("GRP_EDIT1")+ ActionColNo + getTextFromOR("COPY");
		System.out.println(RowPart + j + Btn_Copy);
		driver.findElement(By.xpath(RowPart + j + Btn_Copy)).click();
		System.out.println("Successfully clicked on Copy button.");
		test.log(LogStatus.PASS,  "Successfully clicked on Copy button.");
		Log.info("Successfully clicked on Copy button.");
		temp = true;
		return temp;
		}
		}
		}
		driver.findElement(getLocator("NEXT_PAGINATION")).click();
		}
		return temp;
		}
		}
		return false;
		}catch (Exception e){
		test.log(LogStatus.FAIL, e.getMessage());
		Log.error("Data is not present in table..." + e.getMessage());
		String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
		ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		DriverScript.bResult = false;
		return false;
		}
		}
	public static void verifyRoleColumnName() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
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
