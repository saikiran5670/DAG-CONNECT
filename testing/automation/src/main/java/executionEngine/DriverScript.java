package executionEngine;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.lang.reflect.Method;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Properties;

import com.relevantcodes.extentreports.ExtentReports;

import modules.CommonFunctionLib;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

public class DriverScript 
{
	public static Properties prop;
	
	public static int TestStep;
	public static int TestLastStep;
	
	public static String PageObject;
	public static String TestCaseID;
	public static String RunMode;
	public static String ActionKeyword;
	
	public static CommonFunctionLib commfunction;
	public static Method method[];
		
	public static boolean bResult;
	
    public DriverScript() 
     {
		commfunction = new CommonFunctionLib();
		method = commfunction.getClass().getMethods();
     }
		
	public static void main (String[] args)throws Exception 
    {
		try 
        {
		  String localDir = System.getProperty("user.dir");
		  System.out.println(localDir + Constants.OR_Path);
		  File file;		
		  file = new File(localDir + Constants.OR_Path);	
		  FileInputStream fileInput = null;	  
	      fileInput = new FileInputStream(file);
	      prop = new Properties(System.getProperties());			
	      prop.load(fileInput);
	      
	      String path =  localDir + Constants.Path_TestData;
		  System.out.println(path);
		  ExcelSheet.setExccelFile(path);	
		  DriverScript startEngine = new DriverScript();
		  startEngine.executeTestCase();
        }     
	  catch (IOException e1) 
        {
	      e1.printStackTrace();
	    }		
    }
	
     public void executeTestCase() throws Exception 
      {
    	 try
    	 {
    		 int TotalTestCases = ExcelSheet.getRowCount(Constants.Sheet_TestCases);
    		 //Append test result with date format & screenshot to avoid duplicate or override 
    		 String dateName = new SimpleDateFormat("ddMMMyyyyHHmmss").format(new Date());    
    		 CommonFunctionLib.reports = new ExtentReports(System.getProperty("user.dir") + "/HtmlReport/Result_"+ dateName +".html", true);	
    		 //This loop will execute number times = number of test cases
    		 for (int iTestcase = 1; iTestcase <= TotalTestCases-1; iTestcase++)
             {
	            bResult = true;
	            TestCaseID = ExcelSheet.getCellData(iTestcase, Constants.Col_TestCaseID, Constants.Sheet_TestCases);
	            RunMode = ExcelSheet.getCellData(iTestcase, Constants.Col_RunMode, Constants.Sheet_TestCases);
	 
	           if (RunMode.equals("Yes"))
	             {
		             TestStep = ExcelSheet.getRowContains(TestCaseID, Constants.Col_TestCaseID, Constants.Sheet_TestSteps);
		             TestLastStep = ExcelSheet.getStepCount(Constants.Sheet_TestSteps, TestCaseID, TestStep);
		 
		             Log.startTestCase(TestCaseID);
	    
		            bResult = true; 
	                for (; TestStep < TestLastStep; TestStep++)
	                  {
		                 ActionKeyword = ExcelSheet.getCellData(TestStep, Constants.Col_ActionKeyword, Constants.Sheet_TestSteps);
		                 PageObject = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		                 execute_Action();
		 
		               if (!bResult)
		                {
			              ExcelSheet.setCellData(Constants.KEYWORD_FAIL, iTestcase, Constants.Col_Result, Constants.Sheet_TestCases);
			              Log.endTestCase(TestCaseID);
			              break;
		                }
		              }
	 
	               if(bResult)
	                {
		              ExcelSheet.setCellData(Constants.KEYWORD_PASS, iTestcase, Constants.Col_Result, Constants.Sheet_TestCases);
		              Log.endTestCase(TestCaseID);
		            }
				
	            }
             }
 
        CommonFunctionLib.reports.flush();
    	 } 
    	    
   	  catch (IOException e1) 
           {
   	      e1.printStackTrace();
   	      }
     }

   private static void execute_Action() throws Exception
    {
	
	  for (int i=0; i<method.length-1; i++)
	    {
		  if (method[i].getName().equals(ActionKeyword))
		   {
			 method[i].invoke(commfunction);
			
			  if(bResult)
			   {
				 ExcelSheet.setCellData(Constants.KEYWORD_PASS, TestStep, Constants.Col_TestStepResult, Constants.Sheet_TestSteps);
				 ExcelSheet.setCellData(Constants.KEYWORD_OUTPUT, TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				 break;
			   }else
			   {
				 ExcelSheet.setCellData(Constants.KEYWORD_FAIL, TestStep, Constants.Col_TestStepResult, Constants.Sheet_TestSteps);
				 CommonFunctionLib.closeBrowser();
				break;
			   }
			
		   }
	    }
     } 
 
}
