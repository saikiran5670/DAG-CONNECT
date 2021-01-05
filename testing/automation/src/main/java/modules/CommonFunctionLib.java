package modules;

import static executionEngine.DriverScript.TestStep;
import static executionEngine.DriverScript.prop;

import java.io.File;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.List;

import org.apache.commons.io.FileUtils;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.Keys;
import org.openqa.selenium.OutputType;
import org.openqa.selenium.TakesScreenshot;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;
import org.openqa.selenium.ie.InternetExplorerDriver;
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.support.ui.ExpectedCondition;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.Select;
import org.openqa.selenium.support.ui.WebDriverWait;

import com.relevantcodes.extentreports.ExtentReports;
import com.relevantcodes.extentreports.ExtentTest;
import com.relevantcodes.extentreports.LogStatus;

import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

public class CommonFunctionLib 
{
	public static WebDriver driver;
	public static ExtentReports reports;
	public static ExtentTest test;
	public static String URL;

	public CommonFunctionLib() {
		
			URL="DEV";
			System.out.println(URL);
		}
//*********************API Automation************************************************************

	   public static void AddUserGroup_Json() throws Exception {
	    	Log.info("Adding valid single User Group");
			test.log(LogStatus.INFO, "Adding valid single User Group");    	
	    	String url = Constants.UserGroup + Constants.AddUG;
	    	CommonAPI.postRequest_XML_JSON(url);
	    }
	    public static void UpdateUserGroup_Json() throws Exception {
	    	Log.info("Updating valid User Group");
			test.log(LogStatus.INFO, "Updating valid User Group");    	
	    	String url = Constants.UserGroup + Constants.UpdateUG;
	    	CommonAPI.updateRequest(url, "","","","UpdateUserGroup.json");    	
	    }
	    public static void GetUserGroup() throws Exception {
	    	Log.info("Getting valid single User Group");
			test.log(LogStatus.INFO, "Getting valid single User Group");    	
	    	String url = Constants.UserGroup + Constants.GetUG;	    	
	    	CommonAPI.getRequest(url,"organizationId","IsActive","usergroupId");
	    	
	    }
	    
	    public static void GetUserGroupDetails() throws Exception {
	    	Log.info("Getting valid single User Group details");
			test.log(LogStatus.INFO, "Getting valid single User Group details");    	
	    	String url = Constants.UserGroup + Constants.GetUGD;
	    	CommonAPI.getRequest(url,"OrganizationId","UserGroupId","usergroupId");    	
	    }
	    public static void DeleteUserGroup() throws Exception {
	    	Log.info("Deleting valid single User Group");
			test.log(LogStatus.INFO, "Deleting valid single User Group");    	
	    	String url = Constants.UserGroup + Constants.DELUG;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.deleteRequest(url,"usergroupId","organizationId","usergroupId");
	    		break;
	    	case "QA":
				CommonAPI.deleteRequest(url,"usergroupId","organizationId","usergroupId");
	    		break;
			default:	
				CommonAPI.deleteRequest(url,"usergroupId","organizationId","usergroupId");
	    	}
	    	
	    }
	    public static void GetUser() throws Exception {
	    	Log.info("Getting User details");
			test.log(LogStatus.INFO, "Getting User details");    	
	    	String url = Constants.UserManagement + Constants.GetU;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.getRequest(url,"userId","","userId");
	    		break;
	    	case "QA":
				CommonAPI.getRequest(url,"userId","","userId");
	    		break;
			default:	
				CommonAPI.getRequest(url,"userId","","userId");
	    	}
	    	
	    }
	    public static void DeleteUser() throws Exception {
	    	Log.info("Deleting User");
			test.log(LogStatus.INFO, "Deleting User");    	
	    	String url = Constants.UserManagement + Constants.DELU;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.deleteRequest(url,"usergroupId","","usergroupId");
	    		break;
	    	case "QA":
				CommonAPI.deleteRequest(url,"usergroupId","","usergroupId");
	    		break;
			default:	
				CommonAPI.deleteRequest(url,"usergroupId","","usergroupId");
	    	}
	    	
	    }
	    public static void UpdateUser_Json() throws Exception {
	    	Log.info("Updating User details");
			test.log(LogStatus.INFO, "Updating User details");    	
	    	String url = Constants.UserManagement + Constants.UpdateU;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.updateRequest(url,"FirstName","LastName","UserId","UpdateUser.json");
	    		break;
	    	case "QA":
				CommonAPI.updateRequest(url,"FirstName","LastName","UserId","UpdateUser.json");
	    		break;
			default:	
				CommonAPI.updateRequest(url,"FirstName","LastName","UserId","UpdateUser.json");
	    	}
	    	
	    }
	    public static void AddUser_Json() throws Exception {
	    	Log.info("Adding valid single User details");
			test.log(LogStatus.INFO, "Adding valid single User details");    	
	    	String url = Constants.UserManagement + Constants.AddU;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.postRequest_XML_JSON(url);
	    		break;
	    	case "QA":
				CommonAPI.postRequest_XML_JSON(url);
	    		break;
			default:	
				CommonAPI.postRequest_XML_JSON(url);
	    	}
	    	
	    }
	    
	    public static void AddVehicle() throws Exception {
	    	Log.info("Adding valid single Vehicle details");
			test.log(LogStatus.INFO, "Adding valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.AddV;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.postRequest_XML_JSON(url);
	    		break;
	    	case "QA":
				CommonAPI.postRequest_XML_JSON(url);
	    		break;
			default:	
				CommonAPI.postRequest_XML_JSON(url);
	    	}
	    	
	    }
	    public static void AddVehicleGroup() throws Exception {
	    	Log.info("Adding valid single Vehicle Group details");
			test.log(LogStatus.INFO, "Adding valid single Vehicle Group details");    	
	    	String url = Constants.VehicleManagement + Constants.AddVG;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.postRequest_XML_JSON(url);
	    		break;
	    	case "QA":
				CommonAPI.postRequest_XML_JSON(url);
	    		break;
			default:	
				CommonAPI.postRequest_XML_JSON(url);
	    	}
	    	
	    }
	    
	    public static void UpdateVehicle() throws Exception {
	    	Log.info("Updating valid single Vehicle details");
			test.log(LogStatus.INFO, "Updating valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.UpdateV;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.updateRequest(url,"","","","UpdateVehicle.json");
	    		break;
	    	case "QA":
				CommonAPI.updateRequest(url,"","","","UpdateVehicle.json");
	    		break;
			default:	
				CommonAPI.updateRequest(url,"","","","UpdateVehicle.json");
	    	}
	    	
	    }
	    public static void DeleteVehicle() throws Exception {
	    	Log.info("Deleteing valid single Vehicle details");
			test.log(LogStatus.INFO, "Deleting valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.DELV;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.deleteRequest(url,"vehicleID","userId","vehicleID");
	    		break;
	    	case "QA":
				CommonAPI.deleteRequest(url,"vehicleID","userId","vehicleID");
	    		break;
			default:	
				CommonAPI.deleteRequest(url,"vehicleID","userId","vehicleID");
	    	}
	    	
	    }
	    public static void GetVehicle() throws Exception {
	    	Log.info("Getting valid single Vehicle details");
			test.log(LogStatus.INFO, "Getting valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.GetV;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.getRequest(url,"vehicleID","","vehicleID");
	    		break;
	    	case "QA":
				CommonAPI.getRequest(url,"vehicleID","","vehicleID");
	    		break;
			default:	
				CommonAPI.getRequest(url,"vehicleID","","vehicleID");
	    	}
	    	
	    }
	    public static void UpdateVehicleGroup() throws Exception {
	    	Log.info("Updating valid single vehicle group details");
			test.log(LogStatus.INFO, "Updating valid single vehicle group details");    	
	    	String url = Constants.VehicleManagement + Constants.UpdateV;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.updateRequest(url,"","","","UpdateVehicleGroup.json");
	    		break;
	    	case "QA":
				CommonAPI.updateRequest(url,"","","","UpdateVehicleGroup.json");
	    		break;
			default:	
				CommonAPI.updateRequest(url,"","","","UpdateVehicleGroup.json");
	    	}
	    	
	    }
	    
	    public static void DeleteVehicleGroup() throws Exception {
	    	Log.info("Deleting valid single vehicle group details");
			test.log(LogStatus.INFO, "Deleting valid single vehicle group details");    	
	    	String url = Constants.VehicleManagement + Constants.DELVG;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.deleteRequest(url,"vehicleGroupID","userId","vehicleGroupID");
	    		break;
	    	case "QA":
				CommonAPI.deleteRequest(url,"vehicleGroupID","userId","vehicleGroupID");
	    		break;
			default:	
				CommonAPI.deleteRequest(url,"vehicleGroupID","userId","vehicleGroupID");
	    	}
	    	
	    }
	    public static void GetVehicleGroup() throws Exception {
	    	Log.info("Getting valid single vehicle group details");
			test.log(LogStatus.INFO, "Getting valid single vehicle group details");    	
	    	String url = Constants.VehicleManagement + Constants.GetVG;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.getRequest(url,"vehicleGroupID","","vehicleGroupID");
	    		break;
	    	case "QA":
				CommonAPI.getRequest(url,"vehicleGroupID","","vehicleGroupID");
	    		break;
			default:	
				CommonAPI.getRequest(url,"vehicleGroupID","","vehicleGroupID");
	    	}
	    	
	    }
	    
	    public static void AddRole() throws Exception {
	    	Log.info("Adding valid Role details");
			test.log(LogStatus.INFO, "Adding valid Role details");    	
	    	String url = Constants.RoleManagement + Constants.AddR;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.postRequest_XML_JSON(url);
	    		break;
	    	case "QA":
				CommonAPI.postRequest_XML_JSON(url);
	    		break;
			default:	
				CommonAPI.postRequest_XML_JSON(url);
	    	}
	    	
	    }
	    
	    public static void UpdateRole() throws Exception {
	    	Log.info("Updating valid Role details");
			test.log(LogStatus.INFO, "Updating valid Role details");    	
	    	String url = Constants.RoleManagement + Constants.UpdateR;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.updateRequest(url,"","","","UpdateRole.json");
	    		break;
	    	case "QA":
				CommonAPI.updateRequest(url,"","","","UpdateRole.json");
	    		break;
			default:	
				CommonAPI.updateRequest(url,"","","","UpdateRole.json");
	    	}
	    	
	    }
	    public static void DeleteRole() throws Exception {
	    	Log.info("Deleting Role");
			test.log(LogStatus.INFO, "Deleting Role");    	
	    	String url = Constants.RoleManagement + Constants.DELR;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.deleteRequest(url,"roleId","userId","roleId");
	    		break;
	    	case "QA":
				CommonAPI.deleteRequest(url,"roleId","userId","roleId");
	    		break;
			default:	
				CommonAPI.deleteRequest(url,"roleId","userId","roleId");
	    	}
	    	
	    }
	    public static void GetRole() throws Exception {
	    	Log.info("Getting Role by roleId");
			test.log(LogStatus.INFO, "Getting Role by roleId");    	
	    	String url = Constants.RoleManagement + Constants.GetR;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.getRequest(url,"roleId","","roleId");
	    		break;
	    	case "QA":
				CommonAPI.getRequest(url,"roleId","","roleId");
	    		break;
			default:	
				CommonAPI.getRequest(url,"roleId","","roleId");
	    	}
	    	
	    }
	    
	    public static void CheckRoleNameExist() throws Exception {
	    	Log.info("Getting Role by Name");
			test.log(LogStatus.INFO, "Getting Role by Name");    	
	    	String url = Constants.RoleManagement + Constants.GetR;
	    	switch(URL){
	    	case "DEV":
	    		CommonAPI.getRequest(url,"roleName","","roleName");
	    		break;
	    	case "QA":
				CommonAPI.getRequest(url,"roleName","","roleName");
	    		break;
			default:	
				CommonAPI.getRequest(url,"roleName","","roleName");
	    	}
	    	
	    }
	
//*********************UI Automation************************************************************
	
//*********************OPEN BROWSER************************************************************	
public static void openBrowser() throws Exception 
	{
	  try 
		{
		//Write log in ExtentReports(HTML) 
		test.log(LogStatus.INFO, "Browser Launching");
		//Write log in TestLog
		Log.info("Browser Launching");		
		String browser= "Chrome";		
		switch (browser)
		{		
			case "Chrome": 
				if(Constants.Chrome_Driver_Path_Server.isEmpty()) {
				System.setProperty("webdriver.chrome.driver",System.getProperty("user.dir") + "/src/main/resources/browsers/chromedriver.exe");
				  ChromeOptions chromeOptions= new ChromeOptions();
				  //chromeOptions.addArguments("--window-size=1920,1080");
					//chromeOptions.addArguments("--headless");
					//chromeOptions.addArguments("--disable-dev-shm-usage");
					//chromeOptions.addArguments("--no-sandbox");
					//chromeOptions.addArguments("--verbose");
					//chromeOptions.addArguments("--whitelisted-ips='192-168-18-27'");

				  chromeOptions.addArguments("--disable-web-security");
				  chromeOptions.addArguments("--allow-running-insecure-content");
				  driver = new ChromeDriver(chromeOptions); 
				
					//driver = new ChromeDriver();
				break;
				}else {
					System.setProperty("webdriver.chrome.driver",Constants.Chrome_Driver_Path_Server);
					    ChromeOptions chromeOptions= new ChromeOptions();
						chromeOptions.addArguments("--headless");
						chromeOptions.addArguments("--disable-dev-shm-usage");
						chromeOptions.addArguments("--no-sandbox");
						chromeOptions.addArguments("--verbose");
						chromeOptions.addArguments("--whitelisted-ips='192-168-18-27'");
						driver = new ChromeDriver(chromeOptions);
					break;
				}
			case "IE":
				System.setProperty("webdriver.ie.driver",System.getProperty("user.dir") +"/src/main/resources/IEDriverServer.exe");
				 driver = new InternetExplorerDriver(); 
				break;
			default :
				if(Constants.Chrome_Driver_Path_Server.isEmpty()) {
					System.setProperty("webdriver.chrome.driver",System.getProperty("user.dir") + "/src/main/resources/browsers/chromedriver.exe");
					 driver = new ChromeDriver(); 
					break;
					}else {
						System.setProperty("webdriver.chrome.driver",Constants.Chrome_Driver_Path_Server);
						ChromeOptions chromeOptions= new ChromeOptions();
						chromeOptions.addArguments("--headless");
						chromeOptions.addArguments("--disable-dev-shm-usage");
						chromeOptions.addArguments("--no-sandbox");
						chromeOptions.addArguments("--verbose");
						chromeOptions.addArguments("--whitelisted-ips='192-168-18-27'");
						//chromeOptions.setBinary(Constants.Chrome_Driver_Path_Server_exe);
						
						//ChromeDriver driver = new ChromeDriver(chromeOptions);
						 driver = new ChromeDriver(chromeOptions); 
						break;
					}			
	    }		
		test.log(LogStatus.INFO, "Browser Launched Sucessfully");
		Log.info("Browser Launched Sucessfully");
		}
		catch (Exception e)
		  {
			  test.log(LogStatus.FAIL, e.getMessage());
			  Log.error("Browser not Launched..." + e.getMessage());			  
			  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));			  
			  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);				  
			  DriverScript.bResult = false;
		  }
   } 
//*********************NAVIGATE URL************************************************************
public static void navigateUrl() throws Exception 
   {
		try
		{
		//String ENVURL = System.getenv("ENV_URL_DM");
		//String URL=ENVURL;
		String URL="DEV";
		System.out.println(URL);
		String baseUrl;
		driver.manage().window().maximize();
		driver.manage().deleteAllCookies();
		switch(URL) {
		case "DEV":
			baseUrl= Constants.UrlDev;
			break;
		case "QA":
			baseUrl= Constants.UrlQA;
			break;
		default:
			baseUrl= Constants.UrlDev;
			break;
		}		
		
		String expectedTitle = Constants.Sign_In;
		String actualTitle = "";
		
		driver.get(baseUrl);
		
		Thread.sleep(5000);
		
//		actualTitle = driver.getTitle();
//		if (actualTitle.contentEquals(expectedTitle)) 
//		{
//			test.log(LogStatus.PASS, "USER LOGIN PAGE LOADED");
//			Log.info("USER LOGIN PAGE LODED");
//		}
//		else 
//		{
//			test.log(LogStatus.FAIL, "USER LOGIN PAGE NOT LOADED");
//			Log.info("USER LOGIN PAGE NOT LOAED");			
//			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
//			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
//			ExcelSheet.setCellData("USER LOGIN PAGE NOT LOADED", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
//			DriverScript.bResult = false;
//		}
//		waitforPageLoad(driver);
		
   }
	catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("CONNECTED TRUCK LOGIN PAGE NOT LOADED..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);			  
		  DriverScript.bResult = false;
	  }   
	}
//*********************GET LOCATOR - X PATH****************************************************
public static By getLocator(String element)     {
    	 By by = null;    	 
    	 String locator = prop.getProperty(element);
    	 String locatorType = locator.split("~")[0];
    	 String locatorValue = locator.split("~")[1];    	 
    	 switch (locatorType) 
    	 {
    	 case "XPATH":
    	   by = By.xpath(locatorValue);
    	   break;
    	 case "ID":
      	   by = By.id(locatorValue);
      	   break;
    	 case "NAME":
      	   by = By.name(locatorValue);
      	   break; 
    	 case "CLASSNAME":
      	   by = By.className(locatorValue);
      	   break;  
    	 case "TAGNAME":
      	   by = By.tagName(locatorValue);
      	   break; 
    	 case "LINKTEXT":
      	   by = By.linkText(locatorValue);
      	   break;
    	 case "PARTIALLINKTEXT":
      	   by = By.partialLinkText(locatorValue);
      	   break;
    	 case "CSSSELECTOR":
      	   by = By.cssSelector(locatorValue);
      	   break;      	   
         default :
        		test.log(LogStatus.INFO, "Locator Type Not Available");
    			Log.info("Locator Type Not Available");
          break;
    	 }     	 
    	 return by;
     }
//*********************GET TEXT FROM OBJECT REPOSITORY*****************************************
public static String getTextFromOR(String element){    	    	 
    	 String locator = prop.getProperty(element);
    	// String locatorType = locator.split("~")[0];
    	 String locatorValue = locator.split("~")[1];
    	 return locatorValue;
     }
//*********************WAIT FOR PAGE LOAD******************************************************
public static void waitforPageLoad(WebDriver driver) throws InterruptedException 
     {    	 
    	 ExpectedCondition<Boolean> pgeLoadCondition = new  ExpectedCondition<Boolean>() 
    	 {;    	
    	 public Boolean apply(WebDriver driver) 
    	 {
    		 return((JavascriptExecutor)driver).executeScript("return document.readyState").equals("complete");
    	 }    	 
    	 };    	 
    	 WebDriverWait wait = new WebDriverWait(driver, 60);
    	 wait.until(pgeLoadCondition);
    	 Thread.sleep(2000);
     }
//*********************WAIT FOR ELEMENT ******************************************************* 
public static void waitForElements() throws Exception
   {
     try 
     {
    	Thread.sleep(5000);
   	    String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
   	    waitForElementClick(object);
     }
     catch (Exception e)
     {
   	  test.log(LogStatus.FAIL, e.getMessage());
   	  Log.error("Not able to click on Webelement..." + e.getMessage());	  
   	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
   	  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));	  
   	  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
   	  DriverScript.bResult = false;
     }
   } 
//*********************IMAGE LOAD**************************************************************  
public static void waitForLoadingImage() throws Exception
     
     {
   	  try
   	  {
   	  //String LoadingImg = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
   	  for (int i =0; i<= 20; i++)
   	  {
   		  if (driver.findElements(getLocator("LOADING_IMG")).size() > 0 )
   		  {
   			  System.out.println("Please wait Application in-pr0gress.....");
   			  Thread.sleep(5000);
   		  }
   		  else
   		  {
   			  break;
   		  }
   	  }
   	  }
   	  catch (Exception e)
   	  {
   		  test.log(LogStatus.FAIL, e.getMessage());
   		  Log.error("Image not loaded..." + e.getMessage());		  
   		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
   		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
   		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);			  
   		  DriverScript.bResult = false;
   	  }
     }
//*********************CLOSE BROWSER***********************************************************
public static void closeBrowser() 
   {    	
    	driver.close();
    	driver.quit();
   }
//*********************GET SCREENSHOTS*********************************************************
public static String getScreenshot (WebDriver driver, String screenshotName) throws Exception 
  {
	  String dateName = new SimpleDateFormat ("ddMMMyyyyHHmmss").format(new Date());	  
	  TakesScreenshot ts = (TakesScreenshot) driver;	  
	  File source = ts.getScreenshotAs(OutputType.FILE);	  
	  String destination = System.getProperty("user.dir") + "/TestScreenshots/" + screenshotName + dateName + ".png";	  
	  File finalDestinaion = new File(destination);	  
	  FileUtils.copyFile(source, finalDestinaion);	  
	  return destination;
  }
//*********************WAIT FOR ELEMENT CLICK**************************************************
public static void waitForElementClick (String object) throws Exception
{
  try 
  {
	  test.log(LogStatus.INFO, "Clicking on Webelement" + object);
	  Log.info("Clicking on Webelement  :" + object);	  
	  WebDriverWait wait = new WebDriverWait(driver, 120);
	  wait.until(ExpectedConditions.elementToBeClickable(getLocator(object)));	 	  
  }
  catch (Exception e)
  {
	  test.log(LogStatus.FAIL, e.getMessage());
	  Log.error("Not able to click on Webelement..." + e.getMessage());	  
	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
	  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));	  
	  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);	  
	  DriverScript.bResult = false;
  }
}
//*********************Drop Down***************************************************************
public static void selectValueFromDropDown() throws Exception
{
	try
	  {
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  Select DDVALUE = new Select(driver.findElement(getLocator(object)));
		  DDVALUE.selectByVisibleText(value);		  
	  }catch (Exception e)
		  {
			  test.log(LogStatus.FAIL, e.getMessage());
			  Log.error("Not able to select value from drop down..." + e.getMessage());			  
			  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));			  
			  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
			  DriverScript.bResult = false;
		  }
}
//*********************Click*******************************************************************
public static void click() throws Exception
  {
	  try
	  {
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);		  
		  test.log(LogStatus.INFO, "Clicking on Webelement  :" + object);
		  Log.info("Clicking on Webelement  :" + object);		  
		  WebDriverWait wait = new WebDriverWait(driver, 60);
		  wait.until(ExpectedConditions.elementToBeClickable(getLocator(object)));		  
          driver.findElement(getLocator(object)).click();
          waitforPageLoad(driver);
          waitForLoadingImage();
          Thread.sleep(1000);
	  }
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Not able to click on Webelement..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
	  }
  }
//*********************Java script for Click***************************************************
public static void jsClick() throws Exception
  {
	  try
	  {
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 		  
		  test.log(LogStatus.INFO, "Clicking on Webelement using JS  :" + object);
		  Log.info("Clicking on Webelement using JS" + object);		  
		  WebElement element = driver.findElement(getLocator(object));
		  JavascriptExecutor js = (JavascriptExecutor) driver;
		  js.executeScript("arguments[0].click()",element);
		  waitforPageLoad(driver);
	  }
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Not able to click on Webelement..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
	  }
  }
//*********************Mouse Over Click********************************************************
public static void mouseOverClick() throws Exception
  {
	  try
	  {
		  Thread.sleep(3000);
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);		  
		  test.log(LogStatus.INFO, "Clicking on Webelement using mouseover  :" + object);
		  Log.info("Clicking on Webelement using mouseover" + object);		  
		  WebElement Button = driver.findElement(getLocator(object));
		  Actions action = new Actions (driver);
		  action.moveToElement(Button).perform();
		  Thread.sleep(6000);
		  action.click(Button).perform();
		  waitforPageLoad(driver);
	  }
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Not able to click on Webelement..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
	  }
  }  
//*********************Enter Text in TextBox***************************************************  
public static void enterText() throws Exception
  {
	  try
	  {
		  test.log(LogStatus.INFO, "Enter the text in the field");
		  Log.info("Enter the value into textfield");		  
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String txt = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  
		  WebElement textbox = driver.findElement(getLocator(object));
		  textbox.clear();		  
		  textbox.sendKeys(txt);
	  }
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Not able to Enter Text..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
	  }
  }
//*********************VERIFY OBJECT ENABLED***************************************************  
public static void isObjectEnabled() throws Exception
  {
	  String object = null;
	  try
	  {
		  object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String sFlag = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  WebElement ele = wait.until(ExpectedConditions.elementToBeClickable(getLocator(object)));
		  WebElement ele1 = wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		
		  if  (ele.isEnabled() && sFlag.equalsIgnoreCase("Yes") || ele1.isDisplayed() && sFlag.equalsIgnoreCase("Yes"))			  
			  {
				  test.log(LogStatus.PASS, object + "Element is enabled");
				  Log.info(object + "Element is enabled");
			  }else if (ele.isEnabled() == false && sFlag.equalsIgnoreCase("No"))				  
			  {
				  test.log(LogStatus.FAIL, object + "Element is disabled");
				  Log.info(object + "Element is disabled");
				  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		          test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		    
		          ExcelSheet.setCellData(object + "Element is missing", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			  }else
			  {
				  test.log(LogStatus.FAIL, object + "Element is missing");
				  Log.error(object + "Element is missing");
				  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		          test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		    
		          ExcelSheet.setCellData(object + "Element is missing", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			  }		       
		          		  
		}
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error(object + "Element is Disabled..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
	  }
  }  
//*********************VERIFY OBJECT DISPLAYED*************************************************  
public static void isObjectDisplayed() throws Exception
  {
	  String object = null;
	  try
	  {
		  object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String sFlag = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);		 	
		  WebElement ele = null, ele1 = null;		  
		  if  (sFlag.equalsIgnoreCase("Yes"))			  
			  {
				ele = wait.until(ExpectedConditions.elementToBeClickable(getLocator(object)));
				ele1 = wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));
			  }			  
		  if (!(driver.getPageSource().contains(object)) && sFlag.equalsIgnoreCase("No"))				  
			  {
				  test.log(LogStatus.PASS, object + "Element is not displayed");
				  Log.info(object + "Element is not displayed");
			  }else 
	             if (ele.isDisplayed() && sFlag.equalsIgnoreCase("Yes") || ele1.isDisplayed() && sFlag.equalsIgnoreCase("Yes"))	  
			  {
				  test.log(LogStatus.PASS, object + "Element is available");
				  Log.info(object + "Element is available");
			  } else
			  {
				  test.log(LogStatus.FAIL, object + "Element is missing");
				  Log.error(object + "Element is missing");				  
				  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));				  
				  ExcelSheet.setCellData(object + "Element is missing", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			  }
		}
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error(object + "Element is not displayed..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
	  }
  }  
//*********************Read Text***************************************************************  
public static String readText() throws Exception
  {
	  try
	  { 
		  String Read_Txt = null;
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);		  
		  test.log(LogStatus.INFO, "Verify text on");
		  Log.info("Verify text on  :" + object );		  
		  if (driver.findElement(getLocator(object)).isDisplayed())
		  {
			  Read_Txt = driver.findElement(getLocator(object)).getText();
			  Log.info(Read_Txt + " Text stored" );
		  }
		  Thread.sleep(5000);
		  return Read_Txt;
	  }
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Not able to read Text..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
		  return null;
	  }
	
  }
//*********************Verify Text*************************************************************  
public static void verifyText() throws Exception
    {
	  try
	  {
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String txt = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  test.log(LogStatus.INFO, "Verify Text On Page");
		  Log.info("Verify Text On Page  :" + object);		  
		  if (driver.findElement(getLocator(object)).isDisplayed())			 
		 {
			  String temp = driver.findElement(getLocator(object)).getText().trim();
			  if (temp.equalsIgnoreCase(txt.trim()))
			  {
				 test.log(LogStatus.PASS, "Text Found");
				 Log.info(txt + "Text Found");
				 DriverScript.bResult = true;
		      } else 
	          {
		    	 test.log(LogStatus.FAIL, "Text Not Found"); 
		    	 Log.error(txt +  "Text not found");		    	 
		    	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				  DriverScript.bResult = false;
		      }	    	  
	      }		  
	  }
	  catch (Exception e)
	  {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Text Not found..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
	 }
    }
//*********************Scroll By Position******************************************************   
public static void scrollByPosition() throws Exception
  {
	  try {
		    Actions actions = new Actions (driver);
		    actions.sendKeys(Keys.PAGE_UP).perform();
		    actions.sendKeys(Keys.PAGE_DOWN).perform();
		    Thread.sleep(1000);
		  }
	  catch (Exception e)
	      {
		     test.log(LogStatus.FAIL, e.getMessage());
		     Log.error("Not able to scroll by position..." + e.getMessage());		     
		     String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		     test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		     
		     ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		     
		     DriverScript.bResult = false;
	      }
  }
//*********************Scroll By Visible Element***********************************************   
public static void scrollByVisibleElement() throws Exception
  {
	 String Tablepath;
	  try {		  
		    Tablepath = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		    WebElement Element = driver.findElement(getLocator(Tablepath));		  
		    JavascriptExecutor js = (JavascriptExecutor) driver;
		    js.executeScript("arguments[0].scrollIntoView(true);" ,Element);		  
		    test.log(LogStatus.PASS, "Element is visible after scrolling page");
		    Log.info("Element is visible after scrolling page");		  
		  }
	  catch (Exception e)
	      {
		     test.log(LogStatus.FAIL, e.getMessage());
		     Log.error("Not able to scroll page for element to see..." + e.getMessage());		     
		     String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		     test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		    
		     ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		     
		     DriverScript.bResult = false;
	      }
  }  
//*********************Page Refresh************************************************************   
public static void pageRefresh() throws Exception
  {	  
	  try {		  
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  test.log(LogStatus.INFO,  "Refresh the Page");
		  Log.info("Refresh the Page");		  
		  driver.navigate().refresh();		  		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  	  		  
		  }
	  catch (Exception e)
	      {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Page Not refreshed..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
		  
		  }
  }
//*********************Verify Data in Table****************************************************   
public static boolean verifyDataInTable() throws Exception {
Thread.sleep(3000);
try 
{
Actions actions = new Actions(driver);
actions.sendKeys(Keys.PAGE_UP).perform();
String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
List<WebElement> options = driver.findElements(getLocator("GRP_COLUMNHEADER"));
boolean temp = false;
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
String Page = driver.findElement(getLocator("PAGINATION")).getText();
String TotalRecord =Page.split(" ")[4];
String Rowcount =driver.findElement(getLocator("GRP_ROW_COUNT_VAL")).getText();
int Page_No =Integer.parseInt(TotalRecord);
int remainder = 0;
if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
{
	Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
	remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
	if(remainder>0) {
		Page_No=Page_No+1;
	}
}
//int Page_No = Page.size()-2;
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
driver.findElement(getLocator("NEXT_PAGINATION")).click();
}
return temp;
}
}
} catch (Exception e) 
{
test.log(LogStatus.FAIL, e.getMessage());
Log.error("Data is not present in table..." + e.getMessage());
String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
DriverScript.bResult = false;
return false;
}
return false;
}
//*********************Verify Sorting**********************************************************   
public static void verifySorting() throws Exception {

Thread.sleep(5000);
try 
{
ArrayList<String> ObtainedList = new ArrayList<>();
ArrayList<String> SortList = new ArrayList<>();

String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String datatype = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String AscORDesc = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);

String Page = driver.findElement(getLocator("PAGINATION")).getText();
String TotalRecord =Page.split(" ")[4];
String Rowcount =driver.findElement(getLocator("GRP_ROW_COUNT_VAL")).getText();
int Page_No =Integer.parseInt(TotalRecord);
int remainder = 0;
if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
{
	Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
	remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
	if(remainder>0) {
		Page_No=Page_No+1;
	}
}

String RowNumber = getTextFromOR("TABLE_ROW_PART_ONE");
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

//int Page_No = Page.size()-2;
Thread.sleep(3000);
for (int k = 1; k <= Page_No; k++) 
{	
	

if (driver.findElement(getLocator("TABLE")).isDisplayed());
{
System.out.println("Page No :" + k + "   Page button is working");

List<WebElement> options1 = driver.findElements(getLocator("GRP_ROW"));

Thread.sleep(3000);
for (int j = 1; j < options1.size(); j++) 
{
String rowvalueF = driver.findElement(By.xpath(RowNumber + j + "]/mat-cell["+i+"]")).getText();	
String rowvalue = rowvalueF.trim();
ObtainedList.add(rowvalue);
}
}
driver.findElement(getLocator("NEXT_PAGINATION")).click();
waitForLoadingImage();
}
break;
}
}
driver.navigate().refresh();
waitforPageLoad(driver);
waitForLoadingImage();
Thread.sleep(5000);
String SORTLIST1 =  getTextFromOR("SORT_LIST_PART1");
String SORTLIST2 =  getTextFromOR("SORT_LIST_PART2");

driver.findElement(By.xpath(SORTLIST1+ column +SORTLIST2)).click();
waitForLoadingImage();

//List<WebElement> options1 = driver.findElements(getLocator("GRP_COLUMNHEADER"));
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

Thread.sleep(3000);
for (int k = 1; k <= Page_No; k++) 
{	

if (driver.findElement(getLocator("TABLE")).isDisplayed());
{
System.out.println("Page No :" + k + "   Page button is working");

List<WebElement> options1 = driver.findElements(getLocator("GRP_ROW"));

Thread.sleep(3000);
for (int j = 1; j < options1.size(); j++) 
{
String rowvalueF = driver.findElement(By.xpath(RowNumber + j + "]/mat-cell["+i+"]")).getText();	
String rowvalue = rowvalueF.trim();
SortList.add(rowvalue);
}
}
driver.findElement(getLocator("NEXT_PAGINATION")).click();
waitForLoadingImage();
}
break;
}
}

ArrayList<String> sortedList = new ArrayList<>();

for (String s : ObtainedList) 
{
sortedList.add(s);
}

//Collections.sort(sortedList, String.CASE_INSENSITIVE_ORDER);
//Collections.sort(sortedList, Collections.reverseOrder());

Collections.sort(sortedList);

if (sortedList.equals(SortList)) 
{
System.out.println("List is sorted");
test.log(LogStatus.PASS, "Column is sorted");
Log.error("Column is sorted");
} else 
{
System.out.println("Column is not sorted");
test.log(LogStatus.FAIL, "Column is not sorted");
Log.error("Column is not sorted");
String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
}
} catch (Exception e) 
{
test.log(LogStatus.FAIL, e.getMessage());
Log.error("Sorted data is incorrect..." + e.getMessage());

String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));

ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);

DriverScript.bResult = false;
}
}	
//*********************verify Filter************************************************************   
public static void verifyFilter() throws Exception
{	  
	  try {		  
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String filter_Value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		  Thread.sleep(5000);
		  driver.findElement(getLocator(object)).sendKeys(filter_Value);
		  driver.findElement(getLocator(object)).sendKeys(Keys.TAB);
		  Thread.sleep(5000);
		  //s
		  test.log(LogStatus.INFO,  "Refresh the Page");
		  Log.info("Refresh the Page");		  
		 // driver.navigate().refresh();		  		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  	  		  
		  }
	  catch (Exception e)
	      {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Page Not refreshed..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
		  
		  }
}
//*********************verify Item Per page************************************************************   
public static void verifyItemPerPage() throws Exception
{	  
	  try {		  
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String Value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		  Thread.sleep(5000);		 
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 60);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		
		  driver.findElement(getLocator(object)).click();
		  String temp = getTextFromOR("ITEM_PP_VALUE") + Value +"\"]";		  
		  driver.findElement(By.xpath(temp)).click();;
		  Thread.sleep(5000);
		  test.log(LogStatus.INFO,  "Item Per Page is " + Value);
		  Log.info("Item Per Page is " + Value);	  		  
		 // WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));
		  List<WebElement> options = driver.findElements(getLocator("GRP_ROW"));
		  int Num =options.size()-1;
		  if(Num == Integer.parseInt(Value)) {
			  System.out.println("Expected Item per page displayed");
			  test.log(LogStatus.PASS,  "Expected Item per page displayed");
			  Log.info("Expected Item per page displayed");	

		  }
		  
		  }
	  catch (Exception e)
	      {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Page Not refreshed..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
		  
		  }
}
//*********************verify Item Per page************************************************************   
public static void verifyPagination() throws Exception
{	  
	  try {		  
		  String Page = driver.findElement(getLocator("PAGINATION")).getText();
		  String TotalRecord =Page.split(" ")[4];
		  String Rowcount =driver.findElement(getLocator("GRP_ROW_COUNT_VAL")).getText();
		  int Page_No =Integer.parseInt(TotalRecord);
		  int remainder = 0;
		  if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
		  {
		  	Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
		  	remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
		  	if(remainder>0) {
		  		Page_No=Page_No+1;
		  	}
		  }
		  for (int k = 1; k <= Page_No; k++) 
		  {	
		  waitForLoadingImage();
		  if (driver.findElement(getLocator("TABLE")).isDisplayed());
		  {
		  System.out.println(" Next Page button is working");		 
		  driver.findElement(getLocator("NEXT_PAGINATION")).click();
		  }
		  }
	  	}
	  	catch (Exception e)
	      {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Page Not refreshed..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
		  
		  }
}
//*********************Delete Record From TBL************************************************************
public static boolean deleteRecordFrmTbl() throws Exception {
Thread.sleep(3000);
try 
{
	Actions actions = new Actions(driver);
	actions.sendKeys(Keys.PAGE_UP).perform();
	String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
	String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
	String Yes_No = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
	List<WebElement> options = driver.findElements(getLocator("GRP_COLUMNHEADER"));
	boolean temp = false;
	int ActionColNo = 0;
	Thread.sleep(3000);
	//for (int a = 1; a <= options.size(); a++) 
	//{
//		String PartialcolnameF =  getTextFromOR("PART_COL_F_N_FIRST");
//		String PartialcolnameS =  getTextFromOR("PART_COL_F_N_SEC");

		String p = String.valueOf(options.size());
		String ANo =  getTextFromOR("GRP_COLUMNHEADER")+ "[" + p + "]";
		String actioncol = driver.findElement(By.xpath(ANo)).getText();
		String ActionCol = actioncol.trim();
		if (ActionCol.equals("Action") || ActionCol.equals("Actions")) 
		{
			ActionColNo= options.size();
		}
	//}
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
	String TotalRecord =Page.split(" ")[4];
	String Rowcount =driver.findElement(getLocator("GRP_ROW_COUNT_VAL")).getText();
	int Page_No =Integer.parseInt(TotalRecord);
	int remainder = 0;
	if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
	{
		Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
		remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
		if(remainder>0) {
			Page_No=Page_No+1;
		}
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
	String rowvalue = rowvalueF.trim();
	if (rowvalue.equals(value.trim())) 
	{
	System.out.println(value);
	System.out.println(rowvalue);
	System.out.println("Value found in expected column");
	test.log(LogStatus.PASS,  "Value found in expected column");
	Log.info("Value found in expected column");	
	String Btn_Delete = getTextFromOR("GRP_DELETE1")+ ActionColNo + getTextFromOR("GRP_DELETE");
	System.out.println(RowPart + j + Btn_Delete);
	driver.findElement(By.xpath(RowPart + j + Btn_Delete)).click();

	//if (driver.findElement(getLocator("GRP_EDIT_DIALOG")).isDisplayed());
	//{
//		if(Yes_No.equals("Yes")) {
//		driver.findElement(getLocator("GRP_EDIT_UPDATE")).click();
//		System.out.println("Successfully clicked on Update btn of edit dialog");
//		test.log(LogStatus.PASS,  "Successfully clicked on Update btn of edit dialog");
//		Log.info("Successfully clicked on Update btn of edit dialog");
//		}
//		if(Yes_No.equals("No")) {
//			driver.findElement(getLocator("GRP_EDIT_CANCEL")).click();
//			System.out.println("Successfully clicked on Cancel btn of edit dialog");
//			test.log(LogStatus.PASS,  "Successfully clicked on Cancel btn of edit dialog");
//			Log.info("Successfully clicked on Cancel btn of edit dialog");	
//			}
	//
//		}
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
} catch (Exception e) 
{
test.log(LogStatus.FAIL, e.getMessage());
Log.error("Data is not present in table..." + e.getMessage());
String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
DriverScript.bResult = false;
return false;
}
return false;
}
//*********************verify Item Per page************************************************************   
public static void verifyDeletedRecord() throws Exception
{	  
	  try {		  
		String wantToDelete = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
		boolean isDel = verifyDataInTable();
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
		}else {
			if(isDel == true) {
				System.out.println("Not able to delete record.");
				test.log(LogStatus.PASS,  "Not able to delete record.");
				Log.info("Not able to delete record.");
			}else {
				System.out.println("Record deleted successfully.");
				test.log(LogStatus.FAIL,  "Record deleted successfully.");
				Log.info("Record deleted successfully.");
			}
		}
		  
	  	}catch (Exception e)
	      {
		  test.log(LogStatus.FAIL, e.getMessage());
		  Log.error("Page Not refreshed..." + e.getMessage());		  
		  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
		  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
		  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
		  DriverScript.bResult = false;
		  
		  }
}
//*********************Click On Check box in TBL************************************************************
public static boolean selectCheckBoxInTbl() throws Exception {
Thread.sleep(3000);
try 
{
Actions actions = new Actions(driver);
actions.sendKeys(Keys.PAGE_UP).perform();
String column = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
String table = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
//String Yes_No = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
String GRPTBL = getTextFromOR(table);
//if(column.trim().equalsIgnoreCase("User Role")) {
//	GRPTBL = getTextFromOR("GRP_STEP1_TBL");
//}
//if(column.trim().equalsIgnoreCase("Vehicle Group/Vehicle")) {
//	GRPTBL = getTextFromOR("GRP_STEP2_TBL");
//}
String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");

List<WebElement> options = driver.findElements(By.xpath(GRPTBL + COLHEAD));
boolean temp = false;
Thread.sleep(3000);
for (int i = 2; i <= options.size(); i++) 
{

String colnameF = driver.findElement(By.xpath(GRPTBL + COLHEAD + "["+i+"]/div")).getText();
String colname = colnameF.trim();
if (colname.equals(column.trim())) 
{
System.out.println(column);
System.out.println(colname);
String PAGECOUNT = getTextFromOR("PAGINATION");
String Page = driver.findElement(By.xpath(GRPTBL + PAGECOUNT)).getText();
String TotalRecord =Page.split(" ")[4];
String ITEMPP = getTextFromOR("GRP_ROW_COUNT_VAL");
String Rowcount =driver.findElement(By.xpath(GRPTBL + ITEMPP)).getText();
int Page_No =Integer.parseInt(TotalRecord);
int remainder = 0;
if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
{
	Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
	remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
	if(remainder>0) {
		Page_No=Page_No+1;
	}
}
Thread.sleep(3000);
for (int k = 1; k <= Page_No; k++) 
{	

waitForLoadingImage();
if (driver.findElement(getLocator("TABLE")).isDisplayed());
{
System.out.println(" Next Page button is working");
Thread.sleep(3000);
String GRP_ROW = getTextFromOR("GRP_ROW");
List<WebElement> options1 = driver.findElements(By.xpath(GRPTBL + GRP_ROW));
Thread.sleep(3000);
for (int j = 1; j < options1.size(); j++) 
{
//String RowPart = getTextFromOR("TABLE_ROW_PART_ONE");
String rowvalueF = driver.findElement(By.xpath(GRPTBL + GRP_ROW + "[" + j + "]/mat-cell["+i+"]")).getText();
String rowvalue = rowvalueF.trim();
if (rowvalue.equals(value.trim())) 
{
System.out.println(value);
System.out.println(rowvalue);
System.out.println("Value found in expected column");
test.log(LogStatus.PASS,  "Value found in expected column");
Log.info("Value found in expected column");	

	String Chekbox = getTextFromOR("GRP_STEP_TBL_CHK");
	driver.findElement(By.xpath(GRPTBL + GRP_ROW + "[" + j + Chekbox)).click();
	Thread.sleep(2000);
	System.out.println("Successfully clicked on check box");
	test.log(LogStatus.PASS,  "Successfully clicked on check box");
	Log.info("Successfully clicked on check box");
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
} catch (Exception e) 
{
test.log(LogStatus.FAIL, e.getMessage());
Log.error("Data is not present in table..." + e.getMessage());
String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
DriverScript.bResult = false;
return false;
}
return false;
}
//*********************Click On Check box in TBL************************************************************
public static boolean selectVehicleForNewVHGroup() throws Exception {
try 
{
	Thread.sleep(3000);
	Vehicle_Mana.selectVehicleForNewVHGroup();
} catch (Exception e) 
{
test.log(LogStatus.FAIL, e.getMessage());
Log.error("Data is not present in table..." + e.getMessage());
String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
DriverScript.bResult = false;
return false;
}
return false;
}
//*********************Delete Record From TBL***************************************************************
public static boolean editRecordFrmTbl() throws Exception {
Thread.sleep(3000);
try 
{
Actions actions = new Actions(driver);
actions.sendKeys(Keys.PAGE_UP).perform();
String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
String Yes_No = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
List<WebElement> options = driver.findElements(getLocator("GRP_COLUMNHEADER"));
boolean temp = false;
int ActionColNo = 0;
Thread.sleep(3000);
//for (int a = 1; a <= options.size(); a++) 
//{
//	String PartialcolnameF =  getTextFromOR("PART_COL_F_N_FIRST");
//	String PartialcolnameS =  getTextFromOR("PART_COL_F_N_SEC");

	String p = String.valueOf(options.size());
	String ANo =  getTextFromOR("GRP_COLUMNHEADER")+ "[" + p + "]";
	System.out.println(ANo);
	String actioncol = driver.findElement(By.xpath(ANo)).getText();
	String ActionCol = actioncol.trim();
	if (ActionCol.equals("Action")|| ActionCol.equals("Actions")) 
	{
		ActionColNo= options.size();
	}
//}
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
String TotalRecord =Page.split(" ")[4];
String Rowcount =driver.findElement(getLocator("GRP_ROW_COUNT_VAL")).getText();
int Page_No =Integer.parseInt(TotalRecord);
int remainder = 0;
if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
{
	Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
	remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
	if(remainder>0) {
		Page_No=Page_No+1;
	}
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
String rowvalue = rowvalueF.trim();
if (rowvalue.equals(value.trim())) 
{
System.out.println(value);
System.out.println(rowvalue);
System.out.println("Value found in expected column");
test.log(LogStatus.PASS,  "Value found in expected column");
Log.info("Value found in expected column");	
String Btn_Edit = getTextFromOR("GRP_EDIT1")+ ActionColNo + getTextFromOR("GRP_EDIT");
System.out.println(RowPart + j + Btn_Edit);
driver.findElement(By.xpath(RowPart + j + Btn_Edit)).click();

//if (driver.findElement(getLocator("GRP_EDIT_DIALOG")).isDisplayed());
//{
//	if(Yes_No.equals("Yes")) {
//	driver.findElement(getLocator("GRP_EDIT_UPDATE")).click();
//	System.out.println("Successfully clicked on Update btn of edit dialog");
//	test.log(LogStatus.PASS,  "Successfully clicked on Update btn of edit dialog");
//	Log.info("Successfully clicked on Update btn of edit dialog");
//	}
//	if(Yes_No.equals("No")) {
//		driver.findElement(getLocator("GRP_EDIT_CANCEL")).click();
//		System.out.println("Successfully clicked on Cancel btn of edit dialog");
//		test.log(LogStatus.PASS,  "Successfully clicked on Cancel btn of edit dialog");
//		Log.info("Successfully clicked on Cancel btn of edit dialog");	
//		}
//
//	}
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
} catch (Exception e) 
{
test.log(LogStatus.FAIL, e.getMessage());
Log.error("Data is not present in table..." + e.getMessage());
String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
DriverScript.bResult = false;
return false;
}
return false;
}
//*********************Verify Data in Table*****************************************************************   
public static void verifyColInTable() throws Exception {
Thread.sleep(3000);
try 
{
	Actions actions = new Actions(driver);
	actions.sendKeys(Keys.PAGE_UP).perform();
	String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);	
	boolean temp = false;
	Thread.sleep(3000);
	String GRPTBL = null;
	if(column.trim().equalsIgnoreCase("User Role")) {
		GRPTBL = getTextFromOR("GRP_STEP1_TBL");
	}
	if(column.trim().equalsIgnoreCase("Vehicle Group/Vehicle")) {
		GRPTBL = getTextFromOR("GRP_STEP2_TBL");
	}	
	String COLHEAD = getTextFromOR("GRP_COLUMNHEADER");

	List<WebElement> options = driver.findElements(By.xpath(GRPTBL + COLHEAD));	
	Thread.sleep(3000);
	for (int i = 2; i <= options.size(); i++) 
	{

	String colnameF = driver.findElement(By.xpath(GRPTBL + COLHEAD + "["+i+"]/div")).getText();
	String colname = colnameF.trim();
	if (colname.equals(column.trim())) 
	{
	System.out.println(column);
	System.out.println(colname);
	test.log(LogStatus.PASS,  "Successfully Verified that column" + column + "is present in table");
	Log.info( "Successfully Verified that column" + column + "is present in table");	
	temp = true;
	break;
	}
	}
	if(temp == false ) {
		System.out.println(column);
		test.log(LogStatus.PASS,  "Successfully Verified that column" + column + "is not present in table");
		Log.info( "Successfully Verified that column" + column + "is not present in table");
	}
 } catch (Exception e) 
	{
	test.log(LogStatus.FAIL, e.getMessage());
	Log.error("Data is not present in table..." + e.getMessage());
	String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
	test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
	ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	DriverScript.bResult = false;
	
	}
 }
//*********************Drop Down****************************************************************************
public static void selectValueFromList() throws Exception
{
	try
	  {
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  driver.findElement(getLocator(object)).click();
				
		  List<WebElement> options = driver.findElements(getLocator("DDOPTION"));	
			Thread.sleep(3000);
			for (WebElement ele: options) 
			{

			String Option = ele.getText();
			String colname = Option.trim();
			if (colname.equals(value.trim())) 
			{
			ele.click();
			System.out.println(colname);
			test.log(LogStatus.PASS,  "Successfully selected option " + value + " from drop down " + object);
			Log.info( "Successfully selected option " + value + " from drop down " + object);	
			Thread.sleep(1000);
			break;
			}
			}
		  
	  }catch (Exception e)
		  {
			  test.log(LogStatus.FAIL, e.getMessage());
			  Log.error("Not able to select value from drop down..." + e.getMessage());			  
			  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));			  
			  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
			  DriverScript.bResult = false;
		  }
}
//*********************Verify Reset Field****************************************************************************
public static void verifyResetEle() throws Exception
{
  try
  {
	  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
	  String txt = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
	  test.log(LogStatus.INFO, "Verify Text On Page");
	  Log.info("Verify Text On Page  :" + object);		  
	  if (driver.findElement(getLocator(object)).isDisplayed())			 
	 {
		  String temp = driver.findElement(getLocator(object)).getText();
		  if (temp.trim().equalsIgnoreCase(txt.trim())||temp.equalsIgnoreCase(""))
		  {
			 test.log(LogStatus.PASS, "Text Found");
			 Log.info(txt + "Text Found");
			 DriverScript.bResult = true;
	      } else 
          {
	    	 test.log(LogStatus.FAIL, "Text Not Found"); 
	    	 Log.error(txt +  "Text not found");		    	 
	    	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			  DriverScript.bResult = false;
	      }	    	  
      }		  
  }
  catch (Exception e)
  {
	  test.log(LogStatus.FAIL, e.getMessage());
	  Log.error("Text Not found..." + e.getMessage());		  
	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
	  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
	  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
	  DriverScript.bResult = false;
 }
}
}