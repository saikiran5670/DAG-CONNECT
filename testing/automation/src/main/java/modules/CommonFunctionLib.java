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
	public static Vehicles vehi;
	public CommonFunctionLib() {
		
			
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
		//String browser = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
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
		String URL= ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		//System.out.println(URL);
		String baseUrl;
		driver.manage().window().maximize();
		driver.manage().deleteAllCookies();
		switch(URL) {
		case "DEV":
			baseUrl= Constants.UrlDev;
			break;
		case "Test":
			baseUrl= Constants.UrlTest;
			break;
		default:
			baseUrl= Constants.UrlDev;
			break;
		}		
		
		String expectedTitle = Constants.Sign_In;
		String actualTitle = "";
		
		driver.get(baseUrl);
		
		Thread.sleep(5000);
//		if(driver.findElement(By.id("details-button")).isDisplayed()) {
//		Thread.sleep(1000);
//		driver.findElement(By.id("details-button")).click();
//		Thread.sleep(1000);
//		driver.findElement(By.id("proceed-link")).click();
//		waitforPageLoad(driver);
//		}
//		((JavascriptExecutor) driver).executeScript("window.open()");
//		ArrayList<String> tabs = new ArrayList<String>(driver.getWindowHandles());
//		driver.switchTo().window(tabs.get(1));
//		driver.get("https://api.dev2.ct2.atos.net/login");
//		Thread.sleep(1000);
//		driver.findElement(By.id("details-button")).click();
//		Thread.sleep(1000);
//		driver.findElement(By.id("proceed-link")).click();
//		waitforPageLoad(driver);
//		
//		driver.switchTo().window(tabs.get(0));
		Actions actions = new Actions (driver);
	    actions.sendKeys(Keys.PAGE_DOWN).perform();
	    Thread.sleep(1000);
		if (driver.findElement(By.xpath("//span[text()='Accept To Login']")).isDisplayed()) {
			driver.findElement(By.xpath("//span[text()='Accept To Login']")).click();
			waitforPageLoad(driver);	
		}
		
		actualTitle = driver.getTitle();
		if (actualTitle.contentEquals(expectedTitle)) 
		{
			test.log(LogStatus.PASS, "USER LOGIN PAGE LOADED");
			Log.info("USER LOGIN PAGE LODED");
		}
		else 
		{
			test.log(LogStatus.FAIL, "USER LOGIN PAGE NOT LOADED");
			Log.info("USER LOGIN PAGE NOT LOAED");			
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData("USER LOGIN PAGE NOT LOADED", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
		}
		waitforPageLoad(driver);
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
          test.log(LogStatus.INFO, "Successfully clicked on Webelement  :" + object);
		  Log.info("Successfully clicked on Webelement  :" + object);
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
//*********************Enter Text in TextBox***************************************************  
public static void enterBSpace() throws Exception
{
	  try
	  {
		  test.log(LogStatus.INFO, "Enter the text in the field");
		  Log.info("Enter the value into textfield");		  
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		//  String txt = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  
		  WebElement textbox = driver.findElement(getLocator(object));
		  textbox.clear();
		  textbox.sendKeys(Keys.MULTIPLY);
		  textbox.sendKeys(Keys.BACK_SPACE);
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
//*********************Enter Text in TextBox***************************************************  
public static void VerifyCSSValue(String Properties) throws Exception
{
	  try
	  {
		  test.log(LogStatus.INFO, "Verify CSS Value of field");
		  Log.info("Verify CSS Value of field");		  
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String CssValue = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  
		  WebElement element = driver.findElement(getLocator(object));
		  String temp = element.getCssValue(Properties);	  
		  if(CssValue.equalsIgnoreCase(temp)) {
			  test.log(LogStatus.PASS, "Expected Css Value");
			  Log.info("Expected Css Value");		  
			  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.PASS, test.addScreenCapture(screenshotPath));	
		  }else {
			  test.log(LogStatus.FAIL, "Expected Css Value");
			  Log.warn("Expected Css Value");		  
			  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
		  }
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
//*********************Enter Text in TextBox***************************************************  
public static void clearText() throws Exception
{
	  try
	  {
		  test.log(LogStatus.INFO, "Clear the text from input field");
		  Log.info("Clear the text from input field");		  
		  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);		  
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  
		  WebElement textbox = driver.findElement(getLocator(object));
		  textbox.clear();	
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
		//  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  WebElement obj =  driver.findElement(getLocator(object));
		 if  (obj.isEnabled() && sFlag.equalsIgnoreCase("Yes") || obj.isDisplayed() && sFlag.equalsIgnoreCase("Yes"))			  
			  {
			// WebElement ele = wait.until(ExpectedConditions.elementToBeClickable(getLocator(object)));
			 // WebElement ele1 = wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));			  
				  test.log(LogStatus.PASS, object + " Element is enabled");
				  Log.info(object + " Element is enabled");
			  }else if (!obj.isEnabled() && sFlag.equalsIgnoreCase("Yes"))				  
			  {
				  test.log(LogStatus.FAIL, object + " Element is disable, It should be enabled");
				  Log.error(object + " Element is disable, It should be enabled");
				  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		          test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		    
		          ExcelSheet.setCellData(object + " Element is disable, It should be enabled", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			  
			  }else if (obj.isEnabled() && sFlag.equalsIgnoreCase("No"))				  
			  {
				  test.log(LogStatus.FAIL, object + " Element is enabled, It should be disable");
				  Log.error(object + " Element is enabled, It should be disable");
				  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		          test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		    
		          ExcelSheet.setCellData(object + " Element is enabled, It should be disable", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			  
			  }else if (!obj.isEnabled() && sFlag.equalsIgnoreCase("No"))				  
			  {
				  test.log(LogStatus.PASS, object + " Element is disabled");
				  Log.info(object + "Element is disabled");
			  }else
			  {
				  test.log(LogStatus.FAIL, object + " Element is missing");
				  Log.error(object + " Element is missing");
				  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
		          test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		    
		          ExcelSheet.setCellData(object + " Element is missing", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
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
	  }catch (Exception e){
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
	  }catch (Exception e){
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
		  }catch (Exception e){
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
		  }catch (Exception e){
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
		  }catch (Exception e){
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
//String rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]")).getText();
//String[] str;
//if(column.equals("Vehicle Group")||column.equals("Vehicle Group/Vehicle")) {
//	 str = rowvalueF.split("\\r?\\n");
//	 rowvalueF = str[0];
//}
//@@@@@@@@@@
String rowvalueF = null;
rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]")).getText();
if(rowvalueF.startsWith("New")) {
if(driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]/span[3]")).isDisplayed()) {
	 rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]/span[3]")).getText();
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
driver.findElement(getLocator("NEXT_PAGINATION")).click();
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
//*********************Verify Data in Table****************************************************   
public static boolean verifyDataInPopUpTable() throws Exception {
Thread.sleep(3000);
try 
{
Actions actions = new Actions(driver);
actions.sendKeys(Keys.PAGE_UP).perform();
String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
List<WebElement> options = driver.findElements(getLocator("POPUP_COL"));
boolean temp = false;
Thread.sleep(3000);
for (int i = 1; i <= options.size(); i++) 
{
String PartialcolnameF =  getTextFromOR("POPUP_COL_FIRST");
String PartialcolnameS =  getTextFromOR("POPUP_COL_SEC");
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
List<WebElement> options1 = driver.findElements(getLocator("POPUP_ROW"));
Thread.sleep(3000);
for (int j = 1; j <= options1.size(); j++) 
{
String RowPart = getTextFromOR("POPUP_ROW");
String rowvalueF = driver.findElement(By.xpath(RowPart+"[" + j + "]/mat-cell["+i+"]")).getText();
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
temp = true;
return temp;
}
}
}
driver.findElement(getLocator("NEXT_PAGINATION")).click();
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
//*********************Verify Sorting**********************************************************   
public static void verifySorting() throws Exception {
Thread.sleep(5000);
try 
{
ArrayList<String> ObtainedList = new ArrayList<>();
ArrayList<String> SortList = new ArrayList<>();
String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
//String datatype = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
//String AscORDesc = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String Page = driver.findElement(getLocator("PAGINATION")).getText();
String TotalRecord =Page.split(" ")[1];
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
		  Thread.sleep(2000);
		  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
		  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  
		  WebElement textbox = driver.findElement(getLocator(object));
		  textbox.clear();		  
		  textbox.sendKeys(filter_Value);
		  Thread.sleep(1000);
		  test.log(LogStatus.INFO,  "Text entered in Search box");
		  Log.info("Text entered in Search box");				  	  		  
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
		  }catch (Exception e){
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
		  String Pagination = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		  String Row_Value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		  String Page = driver.findElement(getLocator(Pagination)).getText();
		  String TotalRecord =Page.split(" ")[1];
		  String Rowcount =driver.findElement(getLocator(Row_Value)).getText();
		  int Page_No = 1; //Integer.parseInt(TotalRecord);
		  int remainder = 0;
		  if(Integer.parseInt(TotalRecord) > Integer.parseInt(Rowcount))
		  {
		  	Page_No= Integer.parseInt(TotalRecord)/Integer.parseInt(Rowcount);
		  	remainder = Integer.parseInt(TotalRecord)%Integer.parseInt(Rowcount);
		  	if(remainder>0) {
		  		Page_No=Page_No+1;
		  	}
		  }else if(Integer.parseInt(TotalRecord)<= Integer.parseInt(Rowcount)) {
				Page_No =1;
			}
		  if(Page_No>1) {
			  for (int k = 1; k <= Page_No; k++) 
			  {	
			  waitForLoadingImage();
				  if (driver.findElement(getLocator("TABLE")).isDisplayed());
				  {
					  if(Pagination.equalsIgnoreCase("PAGINATION")) {
				  System.out.println(" Next Page button is working");		 
				  driver.findElement(getLocator("NEXT_PAGINATION")).click();
				  test.log(LogStatus.PASS, "Next Page button is working");
			  		Log.info("Next Page button is working");
					  }else {
						  System.out.println(" Next Page button is working");		 
						  driver.findElement(getLocator("POPUP_NEXT_PAGINATION")).click();
						  test.log(LogStatus.PASS, "Next Page button is working");
					  		Log.info("Next Page button is working");
					  }
				  }
			  }
			  if(Pagination.equalsIgnoreCase("PAGINATION")) {
				  System.out.println(" Previous Page button is working");		 
				  driver.findElement(getLocator("PREVIOUS_PAGINATION")).click();
				  test.log(LogStatus.PASS, "Previous Page button is working");
			  		Log.info("Previous Page button is working");
			  }else {
			  System.out.println(" Previous Page button is working");		 
			  driver.findElement(getLocator("POPUP_PREVIOUS_PAGINATION")).click();
			  test.log(LogStatus.PASS, "Previous Page button is working");
		  		Log.info("Previous Page button is working");
			  }
	  	 }else {
		  		test.log(LogStatus.PASS, "Only Single page is available so NEXT and PREVIOUS button is disable");
		  		Log.info("Only Single page is available so NEXT and PREVIOUS button is disable");
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
		String p = String.valueOf(options.size());
		String ANo =  getTextFromOR("GRP_COLUMNHEADER")+ "[" + p + "]";
		String actioncol = driver.findElement(By.xpath(ANo)).getText();
		String ActionCol = actioncol.trim();
		if (ActionCol.equals("Action") || ActionCol.equals("Actions")) 
		{
			ActionColNo= options.size();
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
//	String rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]")).getText();
//	String[] str;
//	if(column.equals("Vehicle Group")||column.equals("Vehicle Group/Vehicle")) {
//		 str = rowvalueF.split("\\r?\\n");
//		 rowvalueF = str[0];
//	}
	
	
	String rowvalueF = null;
	rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]")).getText();
	if(rowvalueF.startsWith("New")) {
	if(driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]/span[3]")).isDisplayed()) {
		 rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]/span[3]")).getText();
	}}
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
	if (column.equals("Name")) {
		String Btn_Delete = getTextFromOR("GRP_DELETE1")+ ActionColNo + getTextFromOR("ROLE_DELETE");
		System.out.println(RowPart + j + Btn_Delete);
		driver.findElement(By.xpath(RowPart + j + Btn_Delete)).click();
		
	}
	else {
	String Btn_Delete = getTextFromOR("GRP_DELETE1")+ ActionColNo + getTextFromOR("GRP_DELETE");
	System.out.println(RowPart + j + Btn_Delete);
	driver.findElement(By.xpath(RowPart + j + Btn_Delete)).click();
	}
	if (driver.findElement(getLocator("GRP_DELETE_DIALOG")).isDisplayed());
	{
		if(Yes_No.equals("Yes")) {
		driver.findElement(getLocator("GRP_DELETE_YES")).click();
		System.out.println("Successfully clicked on Yes button of delete dialog");
		test.log(LogStatus.PASS,  "Successfully clicked on Yes button of delete dialog");
		Log.info("Successfully clicked on Yes button of delete dialog");
		//driver.wait(2000);
		Thread.sleep(2000);
		DriverScript.bResult = true;
		//waitForElementClick("GRP_FILTER_TXT");
		}
		if(Yes_No.equals("No")) {
			driver.findElement(getLocator("GRP_DELETE_NO")).click();
			System.out.println("Successfully clicked on No button of delete dialog");
			test.log(LogStatus.PASS,  "Successfully clicked on No button of delete dialog");
			Log.info("Successfully clicked on No button of delete dialog");
			//driver.wait(2000);
			Thread.sleep(2000);
			//waitForElementClick("GRP_FILTER_TXT");
			DriverScript.bResult = true;	
			}	
		}
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
	return temp;
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
//*********************Click On Check box in TBL************************************************************
public static boolean selectCheckBoxInTbl(String GRPTBL, String COLHEAD, String GRP_ROW, String CELL) throws Exception {
Thread.sleep(3000);
try 
{
Actions actions = new Actions(driver);
actions.sendKeys(Keys.PAGE_UP).perform();
String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
List<WebElement> goptions = driver.findElements(By.xpath(GRPTBL + COLHEAD));
boolean temp = false;
Thread.sleep(3001);
String table = getTextFromOR("GRP_STEP1_TBL");
for (int i = 2; i <= goptions.size(); i++) 
{
String colnameF = driver.findElement(By.xpath(GRPTBL + table + COLHEAD + "["+i+"]" + CELL)).getText();
String colname = colnameF.trim();
if (colname.equals(column.trim())) 
{
System.out.println(column);
System.out.println(colname);
String PAGECOUNT = getTextFromOR("PAGINATION");
String Page = driver.findElement(By.xpath(GRPTBL+ PAGECOUNT)).getText();
String TotalRecord =Page.split(" ")[1];
String ITEMPP = getTextFromOR("GRP_ROW_COUNT_VAL");
String Rowcount =driver.findElement(By.xpath(GRPTBL+ITEMPP)).getText();
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
List<WebElement> options1 = driver.findElements(By.xpath(GRPTBL + table+ GRP_ROW));
Thread.sleep(3000);
for (int j = 1; j < options1.size(); j++) 
{
String rowvalueF = driver.findElement(By.xpath(GRPTBL + table + GRP_ROW + "[" + j + "]/mat-cell["+i+"]")).getText();
String rowvalue = rowvalueF.trim();
	if (rowvalue.equals(value.trim())) 
	{
	System.out.println(value);
	System.out.println(rowvalue);
	System.out.println("Value found in expected column");
	test.log(LogStatus.PASS,  "Value found in expected column");
	Log.info("Value found in expected column");
	String Chekbox = getTextFromOR("GRP_STEP_TBL_CHK");
	driver.findElement(By.xpath(GRPTBL+ table + GRP_ROW + "[" + j + Chekbox)).click();
	Thread.sleep(2001);
	System.out.println("Successfully clicked on check box");
	test.log(LogStatus.PASS,  "Successfully clicked on check box");
	Log.info("Successfully clicked on check box");
	temp = true;
	return temp;
	}
}
}
String NP = getTextFromOR("NEXT_PAGINATION");
driver.findElement(By.xpath(GRPTBL+ NP)).click();
//driver.findElement(getLocator("NEXT_PAGINATION")).click();
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
public static boolean viewRecord(String GRPTBL, String COLHEAD, String GRP_ROW, String CELL) throws Exception {
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
	if (driver.findElement(By.xpath(GRPTBL)).isDisplayed());
	{
	System.out.println(" Next Page button is working");
	Thread.sleep(3000);
	List<WebElement> options1 = driver.findElements(By.xpath(GRP_ROW));
	Thread.sleep(3000);
	for (int j = 1; j <= options1.size(); j++) 
	{
	//String RowPart = getTextFromOR("TABLE_ROW_PART_ONE");
	String rowvalueF = driver.findElement(By.xpath(GRPTBL + GRP_ROW+"[" + j +"]" + CELL+ "[" +i+"]/span[3]")).getText();
	String[] str;
	if(column.equals("Vehicle Group")||column.equals("Vehicle")) {
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
	String Btn_View = getTextFromOR("GRP_EDIT1")+ ActionColNo + getTextFromOR("VIEW");
	System.out.println(GRPTBL + GRP_ROW+"["  + j + Btn_View);
	driver.findElement(By.xpath(GRPTBL + GRP_ROW+"["  + j + Btn_View)).click();
	System.out.println("Successfully clicked on View button.");
	test.log(LogStatus.PASS,  "Successfully clicked on View button.");
	Log.info("Successfully clicked on View button.");
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
//********************* Verify Gobal Icon ****************************************************************
public static boolean VerifyGlobalIcon(String GRPTBL, String COLHEAD, String GRP_ROW, String CELL) throws Exception {
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
	if (driver.findElement(By.xpath(GRPTBL)).isDisplayed());
	{
	System.out.println(" Next Page button is working");
	Thread.sleep(3000);
	List<WebElement> options1 = driver.findElements(By.xpath(GRP_ROW));
	Thread.sleep(3000);
	for (int j = 1; j <= options1.size(); j++) 
	{
	//String RowPart = getTextFromOR("TABLE_ROW_PART_ONE");
	String rowvalueF = driver.findElement(By.xpath(GRPTBL + GRP_ROW+"[" + j +"]" + CELL+ "[" +i+"]/span[3]")).getText();
	String[] str;
	if(column.equals("Vehicle Group")||column.equals("Vehicle")) {
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
	if(driver.findElement(By.xpath(GRPTBL + GRP_ROW+"[" + j +"]" + CELL+ "[" +i+"]/mat-icon[text()=' public ']")).isDisplayed())
	{
	System.out.println("Global Icon present for this record.");
	test.log(LogStatus.PASS,  "Global Icon present for this record.");
	Log.info("Global Icon present for this record.");
	temp = true;
	return temp;
	}
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


//*********************Edit Record From TBL***************************************************************
public static boolean editRecordFrmTbl() throws Exception {
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
Thread.sleep(3001);
for (int j = 1; j <= options1.size(); j++) 
{
String RowPart = getTextFromOR("TABLE_ROW_PART_ONE");
String rowvalueF = null;
rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]")).getText();
if(rowvalueF.startsWith("New")) {
if(driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]/span[3]")).isDisplayed()) {
	 rowvalueF = driver.findElement(By.xpath(RowPart + j + "]/mat-cell["+i+"]/span[3]")).getText();
}}
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

if(column.equals("Name")) {
	String Btn_EditR = getTextFromOR("GRP_EDIT1")+ ActionColNo + getTextFromOR("GRP_EDIT");//"ROLE_EDIT");
	System.out.println(RowPart + j + Btn_EditR);
	driver.findElement(By.xpath(RowPart + j + Btn_EditR)).click();
}else {
	String Btn_Edit = getTextFromOR("GRP_EDIT1")+ ActionColNo + "]"+ getTextFromOR("GRP_EDIT_PEN");
	System.out.println(RowPart + j + Btn_Edit);
	driver.findElement(By.xpath(RowPart + j + Btn_Edit)).click();
}
System.out.println("Successfully clicked on Edit button.");
test.log(LogStatus.PASS,  "Successfully clicked on Edit button.");
Log.info("Successfully clicked on Edit button.");
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
//*********************Click on count present in row of TBL***************************************************************
public static boolean clickOnCount(int colOfCount, String TBL, String Cell) throws Exception {
Thread.sleep(3000);
try 
{
Actions actions = new Actions(driver);
actions.sendKeys(Keys.PAGE_UP).perform();
String column = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
String value = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
String Col = getTextFromOR("GRP_COLUMNHEADER");
List<WebElement> options = driver.findElements(By.xpath(TBL+Col));
boolean temp = false;
Thread.sleep(3000);	
for (int i = 2; i <= options.size(); i++) 
{	
String PartialcolnameF =  getTextFromOR("PART_COL_F_N_FIRST");
String PartialcolnameS =  getTextFromOR("PART_COL_F_N_SEC");
String colnameF = driver.findElement(By.xpath(TBL+ PartialcolnameF + i + PartialcolnameS +Cell)).getText();
String colname = colnameF.trim();
if (colname.equals(column.trim())) 
{
System.out.println(column);
System.out.println(colname);	  
String PAGINATION =  getTextFromOR("PAGINATION");
String Page = driver.findElement(By.xpath(TBL+ PAGINATION)).getText();
String TotalRecord =Page.split(" ")[1];
String ROW =  getTextFromOR("GRP_ROW_COUNT_VAL");
String Rowcount =driver.findElement(By.xpath(TBL+ROW)).getText();
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
Thread.sleep(3001);
String GRow = getTextFromOR("GRP_ROW");
List<WebElement> options1 = driver.findElements(By.xpath(TBL+ GRow));
Thread.sleep(3000);
for (int j = 1; j <= options1.size(); j++) 
{
String RowPart = getTextFromOR("TABLE_ROW_PART_ONE");
String rowvalueF = driver.findElement(By.xpath(TBL+ RowPart + j + "]/mat-cell["+i+"]")).getText();
String rowvalue = rowvalueF.trim();
if (rowvalue.equals(value.trim())) 
{
System.out.println(value);
System.out.println(rowvalue);
System.out.println("Value found in expected column");
test.log(LogStatus.PASS,  "Value found in expected column");
Log.info("Value found in expected column");	
driver.findElement(By.xpath(TBL+RowPart + j + "]/mat-cell["+colOfCount+"]/span")).click();
waitForElementClick("GRP_USER_COUNT");
if(driver.findElement(getLocator("GRP_USER_COUNT")).isDisplayed()) {
	test.log(LogStatus.PASS,  "Count hyperlink is working and dailog box is displayed");
	Log.info("Count hyperlink is working and dailog box is displayed");	
}else {
	test.log(LogStatus.PASS,  "Count hyperlink is not working and dailog box is not displayed");
	Log.info("Count hyperlink is not working and dailog box is not displayed");	
}
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
//*********************Verify Data in Table*****************************************************************   
public static void verifyColInTable(String GRPTBL, String COLHEAD, String ColDiv, String main) throws Exception {
Thread.sleep(3000);
try 
{
	int Cols;
	if(main.equals("Main"))
	{ 
		Cols =1;
	}else{
		Cols=2;
	}
	Actions actions = new Actions(driver);
	actions.sendKeys(Keys.PAGE_UP).perform();
	String column = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);	
	boolean temp = false;
	Thread.sleep(3000);
	List<WebElement> options = driver.findElements(By.xpath(GRPTBL + COLHEAD));	
	Thread.sleep(3000);
	for (int i = Cols; i <= options.size(); i++) 
	{
	String colnameF = driver.findElement(By.xpath(GRPTBL + COLHEAD + "["+i+ColDiv)).getText();
	String colname = colnameF.trim();
	if (colname.equals(column.trim())) 
	{
	System.out.println(column);
	System.out.println(colname);
	test.log(LogStatus.PASS,  "Successfully Verified that column " + column + " is present in table");
	Log.info( "Successfully Verified that column " + column + " is present in table");	
	temp = true;
	DriverScript.bResult = true;
	break;
	}
	}
	if(temp == false ) {
		System.out.println(column);
		test.log(LogStatus.FAIL,  "Column " + column + " is not present in table");
		Log.info( "Column " + column + " is not present in table");
		DriverScript.bResult = false;
	}
 }catch (Exception e){
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
	  }catch (Exception e){
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
	  Thread.sleep(2000);
	  if (driver.findElement(getLocator(object)).isDisplayed())			 
	 {
		  String temp = driver.findElement(getLocator(object)).getText();
		  if (temp.trim().equalsIgnoreCase(txt.trim())||temp.equalsIgnoreCase(""))
		  {
			 test.log(LogStatus.PASS, " field successfully reset");
			 Log.info(txt + " field successfully reset");
			 DriverScript.bResult = true;
	      } else 
          {
	    	 test.log(LogStatus.FAIL, "field not able to reset"); 
	    	 Log.error(txt +  "field not able to reset");		    	 
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
//*********************Verify count in details label****************************************************************************
public static void verifyCountInDetailsLbl(String Label) throws Exception
{
try
{
	String Record_Count = driver.findElement(getLocator(Label)).getText();
	String[] Rec = Record_Count.split("-");
	String Rec_No =Rec[1];
	String tem =Rec_No.replace("(", " ");
	String temp =tem.replace(")", " ");
	 String Page = driver.findElement(getLocator("PAGINATION")).getText();
	  String TotalRecord =Page.split(" ")[1];
	  //String Rowcount =driver.findElement(getLocator("GRP_ROW_COUNT_VAL")).getText();
	  int No = Integer.parseInt(temp.trim());
	  int Page_No =Integer.parseInt(TotalRecord);
	  if(No == Page_No ) {
		  test.log(LogStatus.PASS, "Correct count is displaying in Details label");
			 Log.info(temp.trim() + "Correct count is displaying in Details label");
			 DriverScript.bResult = true;
	  }else{
		  test.log(LogStatus.FAIL, "Incorrect count is displaying in Details label"); 
	    	 Log.error(temp.trim() +  "Incorrect count is displaying in Details label");		    	 
	    	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			  DriverScript.bResult = false;
	  } 
}catch (Exception e){
	  test.log(LogStatus.FAIL, e.getMessage());
	  Log.error("Failed..." + e.getMessage());		  
	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
	  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
	  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
	  DriverScript.bResult = false;
}
}
//*********************Verify Max Input Char****************************************************************************
public static void verifyMaxInputChar(int maxvalue) throws Exception
{
try
{
	test.log(LogStatus.INFO, "Enter the Max input char in the input field");
	  Log.info("Enter the Max input char in the input field");		  
	  String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
	  String txt = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);		  
	  WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
	  wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  
	  WebElement textbox = driver.findElement(getLocator(object));
	  textbox.clear();		  
	  textbox.sendKeys(txt);
	  textbox.sendKeys(Keys.TAB);
	  String Value = textbox.getText();
	  int temp = Value.length();
	  if(temp <= maxvalue) {
		  test.log(LogStatus.PASS, "Input box is accepting maximum " + maxvalue + " char" );
			 Log.info("Input box is accepting maximum " + maxvalue + " char" );
			 DriverScript.bResult = true;
	  }else {
		  test.log(LogStatus.FAIL, "Validation failed for max length of " + maxvalue); 
	    	 Log.error("Validation failed for max length of" + maxvalue);		    	 
	    	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			  DriverScript.bResult = false;
	  }	  
}catch (Exception e){
	  test.log(LogStatus.FAIL, e.getMessage());
	  Log.error("Validation failed..." + e.getMessage());		  
	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
	  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
	  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
	  DriverScript.bResult = false;
}
}
//*********************Verify Max Input Char****************************************************************************
public static void verifyErrorMsgForInputChar(int maxvalue) throws Exception
{
try{
	test.log(LogStatus.INFO, "Enter the Max input char in the input field");
	Log.info("Enter the Max input char in the input field");		  
	String object = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
	String txt = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
//	String Yes_No = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
	WebDriverWait wait = new WebDriverWait(CommonFunctionLib.driver, 30);
	wait.until(ExpectedConditions.visibilityOfElementLocated(getLocator(object)));		  
	WebElement textbox = driver.findElement(getLocator(object));
	textbox.clear();		  
	textbox.sendKeys(txt);
	textbox.sendKeys(Keys.TAB);
	String Value = textbox.getText();
	int temp = Value.length();
	if(temp <= maxvalue) {
		  test.log(LogStatus.PASS, "Input box is accepting maximum char of" + maxvalue);
			 Log.info("Input box is accepting maximum char of" + maxvalue);
			 DriverScript.bResult = true;
	  }else {
		  test.log(LogStatus.FAIL, "Validation failed for max length of" + maxvalue); 
	    	 Log.error("Validation failed for max length of" + maxvalue);		    	 
	    	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			  DriverScript.bResult = false;
	  }	  
}catch (Exception e){
	  test.log(LogStatus.FAIL, e.getMessage());
	  Log.error("Validation failed..." + e.getMessage());		  
	  String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID); 
	  test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));		  
	  ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		  
	  DriverScript.bResult = false;
}
}
}