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
public class Landmark extends CommonFunctionLib  {
	
	public static void verifyDataInLM_GeoTable() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GEOFENCE_SEC"); 
			verifyDataInLMTable(GRPTBL);
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
	public static void verifyDataInLM_PoiTable() throws Exception {
		try {
			String GRPTBL = getTextFromOR("LM_POI_TBL"); 
			verifyDataInLMTable(GRPTBL);
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
	public static void selectGeoFrmTbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GEOFENCE_SEC"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/div";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL,"");
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void selectPOIFrmTbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("LM_POI_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER"); 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL,"");
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void selectPOIFrmGeo() throws Exception {
		try {
			String GRPTBL = "";// getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER"); 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL,"");
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void VerifyPoiDel() throws Exception {
		try {
			String GRPTBL = getTextFromOR("LM_POI_TBL"); 
			verifyDeletedRecord(GRPTBL);
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
	
	public static void VerifyGeoDel() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GEOFENCE_SEC"); 
			verifyDeletedRecord(GRPTBL);
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
	public static void VerifyColInLM_Table() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]/div";
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
	public static void VerifyColInLM_Popup_Tbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("LM_POPUP_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]/div";
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
	public static void VerifyColInLMCatPopup_Tbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("LM_POPUP_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]/div";
			CommonFunctionLib.verifyColInTable(GRPTBL, COLHEAD, ColDiv, "");
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
	public static void VerifyColInLM_Tbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("GRP_STEP1_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String ColDiv = "]/div";
			CommonFunctionLib.verifyColInTable(GRPTBL, COLHEAD, ColDiv, "LM");
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
	public static void verifyPOICount() throws Exception {
		try {
			int ColNo = 0; 
			String column = "POI ";
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
	public static void verifyGeoCount() throws Exception {
		try {
			int ColNo = 0; 
			String column = "Geofence ";
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
	
	public static void verifyCatPOICount() throws Exception {
		try {
			int ColNo = 0; 
			String column = "Number of POI ";
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
	public static void verifyCatGeoCount() throws Exception {
		try {
			int ColNo = 0; 
			String column = "Number of Geofence ";
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
	public static void selectGeoFrmGroupTbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("LM_GRP_GEO_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/div";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL,"");
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void selectCategoryFrmTbl() throws Exception {
		try {
			String GRPTBL = "";//getTextFromOR(""); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/div";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL,"LM");
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void selectPoiFrmGroupTbl() throws Exception {
		try {
			String GRPTBL = getTextFromOR("LM_GRP_POI_TBL"); 
			String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");; 
			String GRP_ROW = getTextFromOR("GRP_ROW");
			String CELL = "/div";
			CommonFunctionLib.selectCheckBoxInTbl(GRPTBL, COLHEAD, GRP_ROW, CELL,"");
			
				
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;				
				}	
	}
	public static void viewManageGrp() throws Exception {
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
	public static void verifyDeletedRecord(String tbl) throws Exception
	{	  
		  try {		  
			String wantToDelete = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			
			boolean isDel = verifyDataInLMTable(tbl);
			if(wantToDelete.equalsIgnoreCase("Yes")) {
			if(isDel == false) {
				System.out.println("Record deleted successfully.");
				test.log(LogStatus.PASS,  "Record deleted successfully.");
				Log.info("Record deleted successfully.");
			}else {
				System.out.println("Not able to delete record.");
				test.log(LogStatus.FAIL,  "Not able to delete record.");
				Log.info("Not able to delete record.");
			}
			}else if(wantToDelete.equalsIgnoreCase("No")) {
				if(isDel == true) {
					System.out.println("Delete request successfully discarded.");
					test.log(LogStatus.PASS,  "Delete request successfully discarded.");
					Log.info("Delete request successfully discarded.");
				}else {
					System.out.println("Not able to discarded delete request.");
					test.log(LogStatus.FAIL,  "Not able to discarded delete request.");
					Log.info("Not able to discarded delete request.");
				}
			}		  
		  	}catch (Exception e){
			  test.log(LogStatus.FAIL, e.getMessage());
			  Log.error("Page Not refreshed..." + e.getMessage());		  
			  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
			  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
			  DriverScript.bResult = false;		  
			  }
	}
	public static boolean verifyDataInLMTable(String tbl) throws Exception {
		Thread.sleep(3000);
		try 
		{
		Actions actions = new Actions(driver);
		actions.sendKeys(Keys.PAGE_UP).perform();
		String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		String colF =  getTextFromOR("GRP_COLUMNHEADER");
		List<WebElement> options = driver.findElements(By.xpath(tbl + colF));
		boolean temp = false;
		Thread.sleep(3000);
		for (int i = 1; i <= options.size(); i++) 
		{
		String PartialcolnameF =  getTextFromOR("PART_COL_F_N_FIRST");
		String PartialcolnameS =  getTextFromOR("PART_COL_F_N_SEC");
		String colnameF = driver.findElement(By.xpath(tbl + PartialcolnameF + i + PartialcolnameS)).getText();
		String colname = colnameF.trim();
		if (colname.equals(column.trim())) 
		{
		System.out.println(column);
		System.out.println(colname);	  	
		String LMtemp = getTextFromOR("PAGINATION");
		String Page = driver.findElement(By.xpath(tbl + LMtemp)).getText();
		String TotalRecord =Page.split(" ")[1];
		String LMRow = getTextFromOR("GRP_ROW_COUNT_VAL");
		String Rowcount =driver.findElement(By.xpath(tbl + LMRow)).getText();
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
		String RowPG = getTextFromOR("GRP_ROW");
		List<WebElement> options1 = driver.findElements(By.xpath(tbl + RowPG));
		Thread.sleep(3000);
		for (int j = 1; j <= options1.size(); j++) 
		{
		String RowPart = getTextFromOR("TABLE_ROW_PART_ONE");
		//String rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]")).getText();
		//String[] str;
		//if(column.equals("Vehicle Group")||column.equals("Vehicle Group/Vehicle")) {
//			 str = rowvalueF.split("\\r?\\n");
//			 rowvalueF = str[0];
		//}
		//@@@@@@@@@@
		String rowvalueF = null;
		rowvalueF = driver.findElement(By.xpath(tbl+ RowPart + j + "]/mat-cell["+i+"]")).getText();
		if(rowvalueF.startsWith("New")) {
		if(driver.findElement(By.xpath(tbl + RowPart + j + "]/mat-cell["+i+"]/span[3]")).isDisplayed()) {
			 rowvalueF = driver.findElement(By.xpath(tbl+RowPart + j + "]/mat-cell["+i+"]/span[3]")).getText();
			 System.out.println("New red tag is displayed");
			 test.log(LogStatus.PASS,  "New red tag is displayed");
			 Log.info("New red tag is displayed");	
		}
		}
		String[] str;
		if(column.equals("Vehicle Group")||column.equals("Vehicle Group/Vehicle")) {
			 str = rowvalueF.split("\\r?\\n");
			 rowvalueF = str[0];
		}
		//@@@@@@@@@@
		String rowvalue = rowvalueF.trim();
		if (rowvalue.equals(value.trim())) 
		{
		System.out.println(value);
		System.out.println(rowvalue);
		System.out.println("Value found in expected column");
		test.log(LogStatus.PASS,  "Value found in expected column");
		Log.info("Value found in expected column");	
		temp = true;
		return temp;
		}
		}
		}
		String nextP = getTextFromOR("NEXT_PAGINATION");
		driver.findElement(By.xpath(tbl + nextP)).click();
		
		}
		}
		}
		return temp;
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

}
