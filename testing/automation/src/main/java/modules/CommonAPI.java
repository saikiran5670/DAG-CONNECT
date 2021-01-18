package modules;

import static executionEngine.DriverScript.TestStep;

import java.io.File;
import java.util.ArrayList;
import org.json.simple.JSONObject;
import com.relevantcodes.extentreports.LogStatus;

import executionEngine.DriverScript;
import io.restassured.RestAssured;
import io.restassured.http.Method;
import io.restassured.path.json.JsonPath;
import io.restassured.response.Response;
import io.restassured.specification.RequestSpecification;
import junit.framework.Assert;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;

public class CommonAPI extends CommonFunctionLib {
	
public static void postRequest_XML_JSON(String url) {//, String param1, String param2) {
	 
	try {
		String validTC = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
		String fileName = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		String localDir = System.getProperty("user.dir");
		String path =localDir + Constants.Json_Path;
		int statusCode;		
		File addData = new File(path + fileName);					
		RestAssured.baseURI = url;
        RequestSpecification httpRequest = RestAssured.given();
        if(fileName.endsWith(".json")){	        
        httpRequest.header("Content-Type", "application/json");
        }
        if(fileName.endsWith(".xml")){
        httpRequest.header("Content-Type", "application/xml");	
        }
        httpRequest.body(addData);
        Response response = httpRequest.request(Method.POST, "");
        statusCode = response.getStatusCode();
        System.out.println(statusCode);
		if(validTC.equalsIgnoreCase("Yes")){
			switch(statusCode) {
			case 200: //OK
		        test.log(LogStatus.PASS, "Request succesfully proceed and status code is " + statusCode);
	            Log.info("Request succesfully proceed and status code is " + statusCode);
	            DriverScript.bResult = true;
	            ExcelSheet.setCellData("Request succesfully proceed and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	            Assert.assertEquals("Correct status code returned", statusCode /*actual value*/, 200 /*expected value*/);
	            break; 
	        case 201: //Created
		        test.log(LogStatus.PASS, "Record succesfully created and status code is " + statusCode);
	            Log.info("Record succesfully created and status code is " + statusCode);
	            DriverScript.bResult = true;
	            ExcelSheet.setCellData("Record succesfully created and status code is: 201", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	            break;
	        case 202: //It indicates that the request has been accepted for processing
		        test.log(LogStatus.PASS, "Request has been accepted for processing and status code is " + statusCode);
	            Log.info("Request has been accepted for processing and status code is " + statusCode);
	            DriverScript.bResult = true;
	            ExcelSheet.setCellData("Request has been accepted for processing and status code is: 202", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	            break;
	        default:   
		    	test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
		        Log.info("Not getting expected response." + statusCode);
		        DriverScript.bResult=false;	
		        break;
			}
//				JsonPath jsonPathEvaluator = response.jsonPath();
//				// Get specific element from JSON document 
//				String p1 = jsonPathEvaluator.get(param1);
//				System.out.println(p1);		
//				String p2 = jsonPathEvaluator.get(param2);
//				System.out.println(p1);		
//				RequestSpecification httpRequest1 = RestAssured.given();
//			    httpRequest1.request(Method.DELETE,p1);
//			    response = httpRequest.queryParam(param1, p1).queryParam(p2, param2).request(Method.PUT, "");
//				
			    DriverScript.bResult=true;	
		}else if(validTC.equalsIgnoreCase("No")){
			switch(statusCode) {
			case 400: //Bad Request
	        	test.log(LogStatus.PASS, "Getting invalid response and status code is " + statusCode);
	            Log.info("Getting invalid response and status code is " + statusCode);	        
		        DriverScript.bResult = true;
				ExcelSheet.setCellData("Getting invalid response status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				break;
			case 401://Unauthorized
		        test.log(LogStatus.PASS, "You are not authorized to perform this operation. " + statusCode);
	            Log.info("You are not authorized to perform this operation. " + statusCode);
	            DriverScript.bResult = true;
	            ExcelSheet.setCellData("You are not authorized to perform this operation and status code is: 401", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	            break; 
			case 409: //The request could not be completed due to a conflict with the current state of the target resource
				test.log(LogStatus.PASS, "The Record is already whitelisted." + statusCode);
	            Log.info("The Record is already whitelisted." + statusCode);	        
		        DriverScript.bResult = false;
				ExcelSheet.setCellData("The Record is already whitelisted and status code is: 409", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				break;
			 default:   
		    	test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
		        Log.info("Not getting expected response." + statusCode);
		        DriverScript.bResult=false;	
		        break;
			}
		}else {
			test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
            Log.info("Not getting expected response." + statusCode);	
		}
		} catch (Exception e) {
			e.printStackTrace();
			Log.info("Unable to update Record : " +e.getMessage() );
		    test.log(LogStatus.FAIL, "Unable to update Record : " +e.getMessage() );	
		    DriverScript.bResult=false;
		}	
    }
 
public static void getRequest(String url, String Param1, String Param2, String Exp_id) {
	 System.out.println(url);
	 String validTC;
	try {
		String Param1_Val = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		String Param2_val = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
		validTC = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		if(Param1.equals(" ") && Param2.equals(" ")) {
			Log.info("Trying to get details of all records ");
			test.log(LogStatus.INFO, "Trying to get details of all records ");
		}else {
			Log.info("Trying to get details by : " +Param1 +" and by "+ Param2);
			test.log(LogStatus.INFO, "Trying to get details by : " +Param1 +" and by "+ Param2);	
		}			
		if (Param1_Val.isEmpty() || Param1_Val== null) {
			Param1 ="";
		}
		if (Param2_val.isEmpty() || Param2_val== null) {
			Param2 ="";	
		}
		 	RestAssured.baseURI = url;
		    RequestSpecification httpRequest = RestAssured.given();
		    Response  response = httpRequest.queryParam(Param1, Param1_Val).queryParam(Param2, Param2_val).request(Method.GET, "");
//		    Response response=null;
//	       // Response response = httpRequest.request(Method.GET, Param);
//		    if(Param1_Val.isEmpty()||!Param2_val.isEmpty()) {
//		    	 response = httpRequest.queryParam(Param2, Param2_val).request(Method.GET, "");
//		    }
//		    if(!Param1_Val.isEmpty()||Param2_val.isEmpty()) {
//		    	 response = httpRequest.queryParam(Param1, Param1_Val).request(Method.GET, "");
//		    }
//		     if(!Param2_val.isEmpty()||!Param1_Val.isEmpty()) {
//	         response = httpRequest.queryParam(Param1, Param1_Val).queryParam(Param2, Param2_val).request(Method.GET, "");
//			}		    
//		    if(Param1_Val.isEmpty()||Param2_val.isEmpty()) {
//		    	response = httpRequest.request(Method.GET, "");
//		    }
	       // validateResponse(response);
	        int statusCode = response.getStatusCode();
	        System.out.println(statusCode);
	        JsonPath existingData = response.jsonPath();
	        ArrayList<String> id = existingData.get(Exp_id);
	        if(validTC.equalsIgnoreCase("Yes")){				 
			switch(statusCode) {					
				case 200: //OK
					if(Param1.equals(" ") && Param2.equals(" ")) {  
			        test.log(LogStatus.PASS, "Sucessfully received details of all record and status code is:" + statusCode);
		            Log.info("Sucessfully received details of all record and status code is: " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("Sucessfully received details of all record and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            Assert.assertEquals("Correct status code returned", statusCode /*actual value*/, 200 /*expected value*/);
		            }else{
			        	Log.info("Sucessfully received details of record : " +Param1 + "record is: " + id);
				        test.log(LogStatus.PASS, "Sucessfully received details of record : " +Param1 + "record is: " + id);	
				        DriverScript.bResult=true;	
					}
		            break;
		       case 202: //It indicates that the request has been accepted for processing
		    	   if(Param1.equals(" ") && Param2.equals(" ")) {
			        test.log(LogStatus.PASS, "Request has been processing for all record and status code is " + statusCode);
		            Log.info("Request has been processing for all record and status code is " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("Request has been processing for all record and status code is: 202", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            break;
		    	   }else{
			        	Log.info("Sucessfully received details of record : " +Param1 + "record is: " + id);
				        test.log(LogStatus.PASS, "Sucessfully received details of record : " +Param1 + "record is: " + id);	
				        DriverScript.bResult=true;	
		    	   }
		       default:   
		    	   test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
		            Log.info("Not getting expected response." + statusCode);
		            DriverScript.bResult=false;	
		            break;
				}
			}else if(validTC.equalsIgnoreCase("No")){
				switch(statusCode) {
				case 400: //Bad Request
		        	test.log(LogStatus.PASS, "Getting invalid response and status code is " + statusCode);
		            Log.info("Getting invalid response and status code is " + statusCode);	        
			        DriverScript.bResult = true;
					ExcelSheet.setCellData("Getting invalid response status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
					 break;
				case 401://Unauthorized
			        test.log(LogStatus.PASS, "You are not authorized to perform this operation. " + statusCode);
		            Log.info("You are not authorized to perform this operation. " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("You are not authorized to perform this operation and status code is: 401", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		            	
		            break; 
				case 409: //The request could not be completed due to a conflict with the current state of the target resource
					test.log(LogStatus.PASS, "The Record is already whitelisted." + statusCode);
		            Log.info("The Record is already whitelisted." + statusCode);	        
			         ExcelSheet.setCellData("The Record is already whitelisted and status code is: 409", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
					 DriverScript.bResult=true;	
					break;
				case 204: //No Content						  
			        test.log(LogStatus.PASS, "This record is not present and status code is:" + statusCode);
		            Log.info("his record is not present and status code is: " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("his record is not present and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            Assert.assertEquals("Correct status code returned", statusCode /*actual value*/, 200 /*expected value*/);
		            break;
		       case 404: //Not Found
			        test.log(LogStatus.PASS, "This record is not found and status code is " + statusCode);
		            Log.info("This record is not found and status code is " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("Request has been processing for all record and status code is: 202", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            break;		
				default:
					test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
		            Log.info("Not getting expected response." + statusCode);
		            DriverScript.bResult=false;	
		            break;
				}
			}			
	} catch (Exception e) {
		e.printStackTrace();
		Log.info("Unable to received details of Vehicle : " +e.getMessage() );
       test.log(LogStatus.FAIL, "Unable received details of Vehicle : " +e.getMessage() );	
       DriverScript.bResult=false;		
	}
	    }


public static void deleteRequest(String url, String Param1, String Param2, String Exp_id) {
	 String validTC;
	try {
		String Param1_Val = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
		String Param2_Val = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
		validTC = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
		Log.info("Deleting particular record : " + Param1_Val);
		test.log(LogStatus.INFO, "Deleting particular record : " + Param1_Val);	
					 
		 RestAssured.baseURI = url;
		    RequestSpecification httpRequest = RestAssured.given();
	        //Response response = httpRequest.request(Method.DELETE, Param);
		    Response response;
	        if(url.contains("DeleteVehicle")||url.contains("DeleteRole")) {
	        response = httpRequest.queryParam(Param1, Param1_Val).queryParam(Param2, Param2_Val).request(Method.PUT, "");
	        }else {
	        response = httpRequest.queryParam(Param1, Param1_Val).queryParam(Param2, Param2_Val).request(Method.POST, "");
	        }// validateResponse(response);
	        int statusCode = response.getStatusCode();
	        System.out.println(statusCode);
	        if(validTC.equalsIgnoreCase("Yes")){				 
				switch(statusCode) {
					case 200: //OK
				        test.log(LogStatus.PASS, "Succesfully record deleted and status code is " + statusCode);
			            Log.info("Succesfully record deleted and  and status code is " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("Succesfully record deleted and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			            Assert.assertEquals("Correct status code returned", statusCode /*actual value*/, 200 /*expected value*/);
			            break; 
					case 204: //No Content						  
				        test.log(LogStatus.PASS, "Succesfully record deleted and status code is:" + statusCode);
			            Log.info("Succesfully record deleted and status code is: " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("Succesfully record deleted and status code is: 204", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			            Assert.assertEquals("Correct status code returned", statusCode /*actual value*/, 200 /*expected value*/);
			            break;
			       case 404: //Not Found
				        test.log(LogStatus.PASS, "Succesfully record deleted and status code is " + statusCode);
			            Log.info("Succesfully record deleted and status code is " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("Succesfully record deleted and status code is: 404", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			            break;			    	  
			       default:   
			    	   test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
			            Log.info("Not getting expected response." + statusCode);
			            DriverScript.bResult=false;	
			            break;
					}
				}else if(validTC.equalsIgnoreCase("No")){
					switch(statusCode) {
					case 400: //Bad Request
			        	test.log(LogStatus.PASS, "Getting invalid response and status code is " + statusCode);
			            Log.info("Getting invalid response and status code is " + statusCode);	        
				        DriverScript.bResult = true;
						ExcelSheet.setCellData("Getting invalid response status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
						 break;
					case 401://Unauthorized
				        test.log(LogStatus.PASS, "You are not authorized to perform this operation. " + statusCode);
			            Log.info("You are not authorized to perform this operation. " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("You are not authorized to perform this operation and status code is: 401", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);		            	
			            break; 
					case 409: //The request could not be completed due to a conflict with the current state of the target resource
						test.log(LogStatus.PASS, "The Record is already whitelisted." + statusCode);
			            Log.info("The Record is already whitelisted." + statusCode);	        
				         ExcelSheet.setCellData("The Record is already whitelisted and status code is: 409", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
						 DriverScript.bResult=true;	
						break;
					default:
						test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
			            Log.info("Not getting expected response." + statusCode);
			            DriverScript.bResult=false;	
			            break;
					}
				}
		       	   
	} catch (Exception e) {
		e.printStackTrace();
		Log.info("Unable to delete details of Vehicle : " +e.getMessage() );
       test.log(LogStatus.FAIL, "Unable to delete details of Vehicle : " +e.getMessage() );	
       DriverScript.bResult=false;
		
	}
	    }


public static void updateRequest(String url, String Param1, String Param2, String UserID, String fileName) {
	try {
		 Log.info("Update request initiated ");
			test.log(LogStatus.INFO, "Update request initiated ");				
			
			String validTC = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
			//String USerID = ExcelSheet.getCellData(TestStep, Constants.Col_Param1, Constants.Sheet_TestSteps); 
			String Param1_Val = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			String Param2_Val = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
			//File addData = null;			
			
			String localDir = System.getProperty("user.dir");
			String path =localDir + Constants.Json_Path;
			int statusCode;		
			File addData = new File(path + fileName);					
			RestAssured.baseURI = url;
	        RequestSpecification httpRequest = RestAssured.given();
	        if(fileName.endsWith(".json")){	        
	        httpRequest.header("Content-Type", "application/json");
	        }
	        if(fileName.endsWith(".xml")){
	        httpRequest.header("Content-Type", "application/xml");	
	        }
	        httpRequest.body(addData);
	        Response response;
	        if(url.contains("UpdateVehicle")||url.contains("UpdateRole")) {
	        response = httpRequest.queryParam(Param1, Param1_Val).queryParam(Param2, Param2_Val).request(Method.PUT, "");
	        }else {
	        response = httpRequest.queryParam(Param1, Param1_Val).queryParam(Param2, Param2_Val).request(Method.POST, "");
	        }
	       // Response response = httpRequest.request(Method.POST, "");
	        statusCode = response.getStatusCode();
	       // System.out.println(statusCode);
			
			
//			
//		String Param = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
//		String validTC = ExcelSheet.getCellData(TestStep, Constants.Col_Param1, Constants.Sheet_TestSteps);
//		Log.info("Update request initiated for this record: " + Param);
//		test.log(LogStatus.INFO, "Update request initiated for this record: " + Param);				
//			 
//		 	RestAssured.baseURI = url;		 
//	        RequestSpecification httpRequest = RestAssured.given();
//	        JSONObject VHupdateData = updateData.get(0);
//	        Log.info("count is "+updateData.size());
//			httpRequest.header("Content-Type", "application/json");
//	        httpRequest.body(VHupdateData.toJSONString());
//	        System.out.println(VHupdateData.toJSONString());
//	        Response response = httpRequest.request(Method.PUT, "");
	       // int statusCode = response.getStatusCode();
	        System.out.println("Status code is "+statusCode);
	        Log.info("Status code is "+statusCode);	       
	        if(validTC.equalsIgnoreCase("Yes")){
				switch(statusCode) {
				case 200: //OK
			        test.log(LogStatus.PASS, "Record has been Updated and status code is " + statusCode);
		            Log.info("Record has been Updated and status code is " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("Record has been Updated and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            Assert.assertEquals("Correct status code returned", statusCode /*actual value*/, 200 /*expected value*/);
		            break; 
		        case 201: //Created
			        test.log(LogStatus.PASS, "Record has been Updated and status code is " + statusCode);
		            Log.info("Record has been Updated and status code is " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("Record succesfully created and status code is: 201", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            break;
		        case 202: //It indicates that the request has been accepted for processing
			        test.log(LogStatus.PASS, "Record has been Updated and status code is " + statusCode);
		            Log.info("Record has been Updated and status code is " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("Request has been accepted for processing and status code is: 202", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            break;
		        default:
					test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
		            Log.info("Not getting expected response." + statusCode);
		            DriverScript.bResult=false;	
		            break;
				}
			}else if(validTC.equalsIgnoreCase("No")){
				switch(statusCode) {
				case 400: //Bad Request
		        	test.log(LogStatus.PASS, "Getting invalid response and status code is " + statusCode);
		            Log.info("Getting invalid response and status code is " + statusCode);	        
			        DriverScript.bResult = true;
					ExcelSheet.setCellData("Getting invalid response status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
					break;
				case 401://Unauthorized
			        test.log(LogStatus.PASS, "You are not authorized to perform this operation. " + statusCode);
		            Log.info("You are not authorized to perform this operation. " + statusCode);
		            DriverScript.bResult = true;
		            ExcelSheet.setCellData("You are not authorized to perform this operation and status code is: 401", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
		            break; 
				case 409: //The request could not be completed due to a conflict with the current state of the target resource
					test.log(LogStatus.PASS, "The Record is already whitelisted." + statusCode);
		            Log.info("The Record is already whitelisted." + statusCode);	        
			        DriverScript.bResult = true;
					ExcelSheet.setCellData("The Record is already whitelisted and status code is: 409", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
					break;
				default:
					test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
		            Log.info("Not getting expected response." + statusCode);
		            DriverScript.bResult=false;	
		            break;
				}
			}			
			
	} catch (Exception e) {
		e.printStackTrace();
		Log.info("Unable to received details of Vehicle : " +e.getMessage() );
       test.log(LogStatus.FAIL, "Unable to received details of Vehicle : " +e.getMessage() );	
       DriverScript.bResult=false;
		
	}
	    }


static void validateResponse(Response response ) throws Exception {
	 int statusCode = response.getStatusCode();
      switch(statusCode) {
       case 200: //OK
	        test.log(LogStatus.PASS, "Request succesfully proceed and status code is " + statusCode);
           Log.info("Request succesfully proceed and status code is " + statusCode);
           DriverScript.bResult = true;
           ExcelSheet.setCellData("Request succesfully proceed and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
           break; 
       case 201: //Created
	        test.log(LogStatus.PASS, "Request succesfully proceed and status code is " + statusCode);
           Log.info("Request succesfully proceed and status code is " + statusCode);
           DriverScript.bResult = true;
           ExcelSheet.setCellData("Request succesfully proceed and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
           break;
       case 202: //It indicates that the request has been accepted for processing
	        test.log(LogStatus.PASS, "Request succesfully updated and status code is " + statusCode);
           Log.info("Request succesfully updated and status code is " + statusCode);
           DriverScript.bResult = true;
           ExcelSheet.setCellData("Request succesfully proceed and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
           break;
      case 204: //No Content
	        test.log(LogStatus.PASS, "Deleted succesfully and status code is " + statusCode);
           Log.info("Deleted succesfully and status code is " + statusCode);
           DriverScript.bResult = true;
           ExcelSheet.setCellData("Request succesfully proceed and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
           break; 
      case 400: //Bad Request
       	test.log(LogStatus.PASS, "Getting invalid response and status code is " + statusCode);
           Log.info("Getting invalid response and status code is " + statusCode);	        
	        DriverScript.bResult = true;
			ExcelSheet.setCellData("Getting invalid response status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			break;
      case 401://Unauthorized
	        test.log(LogStatus.PASS, "You are not authorized to perform this operation. " + statusCode);
           Log.info("You are not authorized to perform this operation. " + statusCode);
           DriverScript.bResult = true;
           ExcelSheet.setCellData("You are not authorized to perform this operation and status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
           break; 
      case 404://Not Found
   	   test.log(LogStatus.FAIL, "Vehicle or file not found " + statusCode);
           Log.info("Vehicle or file not found " + statusCode);	        
	        DriverScript.bResult = false;
			ExcelSheet.setCellData("Vehicle or file not found ", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			break;
      case 409: //The request could not be completed due to a conflict with the current state of the target resource
   	   test.log(LogStatus.FAIL, "The vehicle already whitelisted." + statusCode);
           Log.info("The vehicle already whitelisted." + statusCode);	        
	        DriverScript.bResult = false;
			ExcelSheet.setCellData("The vehicle already whitelisted.", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			break;
      case 500: //Internal Server Error
   	   test.log(LogStatus.FAIL, "Getting Internal Server Error " + statusCode);
           Log.info("Getting Internal Server Error " + statusCode);	        
	        DriverScript.bResult = false;
			ExcelSheet.setCellData("Internal Server Error", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			break;
    
      default:
       	test.log(LogStatus.FAIL, "Expected response not getting " + statusCode);
           Log.info("Expected response not getting" + statusCode);	        
	        DriverScript.bResult = false;
			ExcelSheet.setCellData("Expected response not getting", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
       	break;
       }
      
	}
//Post Request for API using excel. 
public static void postRequest(JSONObject updateData, String url) {
	 try {
		 String validTC = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject,Constants.Sheet_TestSteps);
			Log.info("Post request initiated for data set: " +updateData.toString());
			test.log(LogStatus.INFO, "Post request initiated for data set: " + updateData.toString());				
				
			 RestAssured.baseURI = url;
			 
		        RequestSpecification httpRequest = RestAssured.given();
//		        JSONArray addData = new JSONArray();
//		        JSONObject addData1 = new JSONObject();
//		        System.out.println("count is "+updateData.size());
//				for(int i=0; i<updateData.size();i++ ) {
//		        addData.add(updateData.get(i));
//		        System.out.println(updateData.get(i).toJSONString());
//				}	        
		        httpRequest.header("Content-Type", "application/json");	        
		        httpRequest.body(updateData.toJSONString());
		        System.out.println(updateData.toJSONString());
		        Response response = httpRequest.request(Method.POST, "");
		        DriverScript.bResult=true;
		        int statusCode = response.getStatusCode();
		        System.out.println(statusCode);
				if(validTC.equalsIgnoreCase("Yes")){
					switch(statusCode) {
					case 200: //OK
				        test.log(LogStatus.PASS, "Request succesfully proceed and status code is " + statusCode);
			            Log.info("Request succesfully proceed and status code is " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("Request succesfully proceed and status code is: 200", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			            Assert.assertEquals("Correct status code returned", statusCode /*actual value*/, 200 /*expected value*/);
			            break; 
			        case 201: //Created
				        test.log(LogStatus.PASS, "Record succesfully created and status code is " + statusCode);
			            Log.info("Record succesfully created and status code is " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("Record succesfully created and status code is: 201", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			            break;
			        case 202: //It indicates that the request has been accepted for processing
				        test.log(LogStatus.PASS, "Request has been accepted for processing and status code is " + statusCode);
			            Log.info("Request has been accepted for processing and status code is " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("Request has been accepted for processing and status code is: 202", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			            break;
			        default:   
				    	   test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
				            Log.info("Not getting expected response." + statusCode);
				            DriverScript.bResult=false;	
				            break;
					}		       
//					JsonPath jsonPathEvaluator = response.jsonPath();
//					ArrayList<String> reading = jsonPathEvaluator.get("usergroupId");
//					System.out.println(reading.get(0));
//					for(int i=0; i<reading.size();i++) {
//					RequestSpecification httpRequest1 = RestAssured.given();
//			        Response response1 = httpRequest1.request(Method.DELETE, reading.get(i));				
//					}
				}else if(validTC.equalsIgnoreCase("No")){
					switch(statusCode) {
					case 400: //Bad Request
			        	test.log(LogStatus.PASS, "Getting invalid response and status code is " + statusCode);
			            Log.info("Getting invalid response and status code is " + statusCode);	        
				        DriverScript.bResult = true;
						ExcelSheet.setCellData("Getting invalid response status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
						break;
					case 401://Unauthorized
				        test.log(LogStatus.PASS, "You are not authorized to perform this operation. " + statusCode);
			            Log.info("You are not authorized to perform this operation. " + statusCode);
			            DriverScript.bResult = true;
			            ExcelSheet.setCellData("You are not authorized to perform this operation and status code is: 401", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			            break; 
					case 409: //The request could not be completed due to a conflict with the current state of the target resource
						test.log(LogStatus.PASS, "The Record is already whitelisted." + statusCode);
			            Log.info("The Record is already whitelisted." + statusCode);	        
				        DriverScript.bResult = true;
						ExcelSheet.setCellData("The Record is already whitelisted and status code is: 409", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
						break;
					 default:   
				    	   test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
				            Log.info("Not getting expected response." + statusCode);
				            DriverScript.bResult=false;	
				            break;
					}
				}else {
					test.log(LogStatus.FAIL, "Not getting expected response." + statusCode);
		            Log.info("Not getting expected response." + statusCode);	
				}
	 } catch (Exception e) {
		 e.printStackTrace();
		 Log.info("Unable to update Record : " +e.getMessage() );
		 test.log(LogStatus.FAIL, "Unable to update Record : " +e.getMessage() );	
		 DriverScript.bResult=false;
	 	}
 }

}
