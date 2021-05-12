package modules;

import static executionEngine.DriverScript.TestStep;

import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.util.ArrayList;
import java.util.Base64;
import java.util.Map;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

import com.github.wnameless.json.flattener.JsonFlattener;
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
	    	CommonAPI.deleteRequest(url,"usergroupId","organizationId","usergroupId");
	    	}
	    public static void GetUser() throws Exception {
	    	Log.info("Getting User details");
			test.log(LogStatus.INFO, "Getting User details");    	
	    	String url = Constants.UserManagement + Constants.GetU;
	    	CommonAPI.getRequest(url,"userId","","userId");
	    	}
	    public static void DeleteUser() throws Exception {
	    	Log.info("Deleting User");
			test.log(LogStatus.INFO, "Deleting User");    	
	    	String url = Constants.UserManagement + Constants.DELU;
	    	CommonAPI.deleteRequest(url,"usergroupId","","usergroupId");
	    	}
	    public static void UpdateUser_Json() throws Exception {
	    	Log.info("Updating User details");
			test.log(LogStatus.INFO, "Updating User details");    	
	    	String url = Constants.UserManagement + Constants.UpdateU;
	    	CommonAPI.updateRequest(url,"FirstName","LastName","UserId","UpdateUser.json");
	    	}
	    public static void AddUser_Json() throws Exception {
	    	Log.info("Adding valid single User details");
			test.log(LogStatus.INFO, "Adding valid single User details");    	
	    	String url = Constants.UserManagement + Constants.AddU;
	    	CommonAPI.postRequest_XML_JSON(url);
	    	}	    
	    public static void AddVehicle() throws Exception {
	    	Log.info("Adding valid single Vehicle details");
			test.log(LogStatus.INFO, "Adding valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.AddV;
	    	CommonAPI.postRequest_XML_JSON(url);
	    	}
	    public static void AddVehicleGroup() throws Exception {
	    	Log.info("Adding valid single Vehicle Group details");
			test.log(LogStatus.INFO, "Adding valid single Vehicle Group details");    	
	    	String url = Constants.VehicleManagement + Constants.AddVG;
	    	CommonAPI.postRequest_XML_JSON(url);
	    	}	    
	    public static void UpdateVehicle() throws Exception {
	    	Log.info("Updating valid single Vehicle details");
			test.log(LogStatus.INFO, "Updating valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.UpdateV;
	    	CommonAPI.updateRequest(url,"","","","UpdateVehicle.json");
	    	}
	    public static void DeleteVehicle() throws Exception {
	    	Log.info("Deleteing valid single Vehicle details");
			test.log(LogStatus.INFO, "Deleting valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.DELV;
	    	CommonAPI.deleteRequest(url,"vehicleID","userId","vehicleID");
	    	}
	    public static void GetVehicle() throws Exception {
	    	Log.info("Getting valid single Vehicle details");
			test.log(LogStatus.INFO, "Getting valid single Vehicle details");    	
	    	String url = Constants.VehicleManagement + Constants.GetV;
	    	CommonAPI.getRequest(url,"vehicleID","","vehicleID");
	    	}
	    public static void UpdateVehicleGroup() throws Exception {
	    	Log.info("Updating valid single vehicle group details");
			test.log(LogStatus.INFO, "Updating valid single vehicle group details");    	
	    	String url = Constants.VehicleManagement + Constants.UpdateV;
	    	CommonAPI.updateRequest(url,"","","","UpdateVehicleGroup.json");
	    	}	    
	    public static void DeleteVehicleGroup() throws Exception {
	    	Log.info("Deleting valid single vehicle group details");
			test.log(LogStatus.INFO, "Deleting valid single vehicle group details");    	
	    	String url = Constants.VehicleManagement + Constants.DELVG;
	    	CommonAPI.deleteRequest(url,"vehicleGroupID","userId","vehicleGroupID");
	    	}
	    public static void GetVehicleGroup() throws Exception {
	    	Log.info("Getting valid single vehicle group details");
			test.log(LogStatus.INFO, "Getting valid single vehicle group details");    	
	    	String url = Constants.VehicleManagement + Constants.GetVG;
	    	CommonAPI.getRequest(url,"vehicleGroupID","","vehicleGroupID");
	    	}	    
	    public static void AddRole() throws Exception {
	    	Log.info("Adding valid Role details");
			test.log(LogStatus.INFO, "Adding valid Role details");    	
	    	String url = Constants.RoleManagement + Constants.AddR;
	    	CommonAPI.postRequest_XML_JSON(url);
	    	}	    
	    public static void UpdateRole() throws Exception {
	    	Log.info("Updating valid Role details");
			test.log(LogStatus.INFO, "Updating valid Role details");    	
	    	String url = Constants.RoleManagement + Constants.UpdateR;
	    	CommonAPI.updateRequest(url,"","","","UpdateRole.json");
	    	}
	    public static void DeleteRole() throws Exception {
	    	Log.info("Deleting Role");
			test.log(LogStatus.INFO, "Deleting Role");    	
	    	String url = Constants.RoleManagement + Constants.DELR;
	    	CommonAPI.deleteRequest(url,"roleId","userId","roleId");
	    	}
	    public static void GetRole() throws Exception {
	    	Log.info("Getting Role by roleId");
			test.log(LogStatus.INFO, "Getting Role by roleId");    	
	    	String url = Constants.RoleManagement + Constants.GetR;
	    	CommonAPI.getRequest(url,"roleId","","roleId");
	    	}	    
	    public static void CheckRoleNameExist() throws Exception {
	    	Log.info("Getting Role by Name");
			test.log(LogStatus.INFO, "Getting Role by Name");    	
	    	String url = Constants.RoleManagement + Constants.GetR;
	    	CommonAPI.getRequest(url,"roleName","","roleName");
	    	}
	    
//*********************External API********************************************** 	    
	    public static void Customer_Data() throws Exception {
	    	Log.info("Updating Customer data");
			test.log(LogStatus.INFO, "Updating Customer data");    	
	    	String url = Constants.APITestURL;
	    	String module = Constants.customer_data;
	    	String request = Constants.update;
	    	CommonAPI.postRequest_withAuth(url,module,request,"ulka.pate@atos.net", "Ulka@1234567");
	    	}	
	    public static void KeyHandover() throws Exception {
	    	Log.info("Updating Customer data");
			test.log(LogStatus.INFO, "Updating Customer data");    	
	    	String url = Constants.APITestURL;
	    	String module = Constants.customer_data;
	    	String request = Constants.keyhandover;
	    	CommonAPI.postRequest_withAuth(url,module,request,"ulka.pate@atos.net", "Ulka@1234567");
	    	}
	    public static void Vehicle_Data() throws Exception {
	    	Log.info("Updating Vehicle data");
			test.log(LogStatus.INFO, "Updating Vehicle data");    	
	    	String url = Constants.APITestURL;
	    	String module = Constants.Vehicle_data;
	    	String request = Constants.update;
	    	CommonAPI.postRequest_withAuth(url,module,request,"ulka.pate@atos.net", "Ulka@1234567");
	    	}	
	    public static void Vehicle_DataWithInvalidUser() throws Exception {
	    	Log.info("Updating vehicle data with invalid user");
			test.log(LogStatus.INFO, "Updating vehicle data with invalid user");    	
	    	String url = Constants.APITestURL;
	    	String module = Constants.Vehicle_data;
	    	String request = Constants.update;
	    	CommonAPI.postRequest_withAuth(url,module,request,"ulka.patel@atos.net", "Ulka@123451");
	    	}	
	    public static void Subscription() throws Exception {
	    	Log.info("Updating Subscription of package");
			test.log(LogStatus.INFO, "Updating Subscription of package");    	
	    	String url = Constants.APITestURL;
	    	String module = Constants.Subscription;
	    	String request = Constants.update;
	    	CommonAPI.postRequest_withAuth(url,module,request,"ulka.pate@atos.net", "Ulka@1234567");
	    	}	
	   // Subscription_withAuth
	    
	    public static void Subscription_withAuth() throws Exception {
	    	Log.info("Updating Subscription of package");
			test.log(LogStatus.INFO, "Updating Subscription of package");    	
	    	String url = Constants.APITestURL;
	    	String module = Constants.Subscription;
	    	String request = Constants.update;
	    	CommonAPI.Subscriptions_withAuth(url,module,request,"ulka.pate@atos.net", "Ulka@1234567");
	    	}	
	    
	    
	    
//***************************Common Functions************************************************************
	    public static String encode(String str1, String str2) {
            return new String(Base64.getEncoder().encode((str1 + ":" + str2).getBytes()));
        	}
	    public static String auth(String url, String UserName, String Pass) {//, String param1, String param2) {
	   	   	try {
	    		String token=null;
	    		int statusCode;	   
	    		System.out.println(url);
	    		RestAssured.baseURI =url + "auth";
	    		RestAssured.useRelaxedHTTPSValidation();	    		
	            RequestSpecification httpRequest = RestAssured.given();//.auth().basic("kingdomadmin@ct.net", "zaq1ZAQ!");
	            String authBasic = encode(UserName, Pass);
	    		httpRequest.header("Authorization", String.format("Basic %s", authBasic));
	            Response response = httpRequest.post();//request(Method.POST, "");
	            statusCode = response.getStatusCode();
	            System.out.println(statusCode);
	            if(statusCode==200) {
	            JsonPath jsonPathEvaluator = response.jsonPath();
	            token = jsonPathEvaluator.get("access_token");//("accessToken");//
	            DriverScript.bResult=true;
	            test.log(LogStatus.INFO, "Token Genrated succesfully and status code is " + statusCode);
	            Log.info("Token Genrated succesfully and status code is " + statusCode);
	            }
	    		return token;	    		
	    		}catch (Exception e){
	    			e.printStackTrace();
	    			Log.info("Unable to update Record : " +e.getMessage() );
	    		    test.log(LogStatus.FAIL, "Unable to update Record : " +e.getMessage() );	
	    		    DriverScript.bResult=false;
	    		    return null;
	    		}				
	        }
	    public static void postRequest_withAuth(String url, String module, String request,String UserName, String Pass) {//, String param1, String param2) {
	   	   	try {
	   	   	System.out.println(url);
	    		test.log(LogStatus.PASS, "Verifying " +request+ " of "+ module);
 	            Log.info("Verifying " +request+ " of "+ module);
	    		String bearerToken = auth(url, UserName, Pass);
				 System.out.println(bearerToken);
	    		String validTC = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
	    		String fileName = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
	    		String localDir = System.getProperty("user.dir");
	    		String path =localDir + Constants.Json_Path;
	    		int statusCode;		
	    		File addData = new File(path + fileName);
	    		System.out.println(path + fileName);
	    		String URL = url + module + request;
	    		RestAssured.baseURI = URL;
	            RequestSpecification httpRequest = RestAssured.given();
	            if(fileName.endsWith(".json")){	        
	            httpRequest.header("Content-Type", "application/json");
	            }
	            if(fileName.endsWith(".xml")){
	            httpRequest.header("Content-Type", "application/xml");	
	            }
	            httpRequest.header("Authorization", "Bearer "+ bearerToken);
	            httpRequest.body(addData);
	            
	            Response response = httpRequest.request(Method.POST);
	            statusCode = response.getStatusCode();
	            System.out.println(statusCode);
	            String temp = response.asString();
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
//	    				JsonPath jsonPathEvaluator = response.jsonPath();
//	    				// Get specific element from JSON document 
//	    				String p1 = jsonPathEvaluator.get(param1);
//	    				System.out.println(p1);		
//	    				String p2 = jsonPathEvaluator.get(param2);
//	    				System.out.println(p1);		
//	    				RequestSpecification httpRequest1 = RestAssured.given();
//	    			    httpRequest1.request(Method.DELETE,p1);
//	    			    response = httpRequest.queryParam(param1, p1).queryParam(p2, param2).request(Method.PUT, "");
//	    				
	    			   //S DriverScript.bResult=true;	
	    		}else if(validTC.equalsIgnoreCase("No")){
	    			switch(statusCode) {
	    			case 400: //Bad Request
	    				System.out.println("status code- " + statusCode + " Response is- " + temp);
	    				test.log(LogStatus.PASS, "status code- " + statusCode + " Response is- " + temp);
	    				Log.info("status code- " + statusCode + " Response is- " + temp);
	    	        	test.log(LogStatus.PASS, "Getting above response and status code for invalid Test case is " + statusCode);
	    	            Log.info("Getting above response and status code for invalid Test case is " + statusCode);	        
	    		        DriverScript.bResult = true;
	    				ExcelSheet.setCellData("Getting " + temp + " response & status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	    				break;
	    			case 401://Unauthorized
	    		        test.log(LogStatus.PASS, "You are not authorized to perform this operation. " + statusCode);
	    	            Log.info("You are not authorized to perform this operation. " + statusCode);
	    	            DriverScript.bResult = true;
	    	            ExcelSheet.setCellData("You are not authorized to perform this operation and status code is: 401", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	    	            break; 
	    			case 404: //The request could not be completed due to a conflict with the current state of the target resource
	    				test.log(LogStatus.PASS, "Record not found. " + statusCode);
	    	            Log.info("Record not found. " + statusCode);	        
	    		        DriverScript.bResult = true;
	    				ExcelSheet.setCellData("Record not found and status code is: 404", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	    				break;
	    			case 409: //The request could not be completed due to a conflict with the current state of the target resource
	    				test.log(LogStatus.PASS, "The Record is already whitelisted." + statusCode);
	    	            Log.info("The Record is already whitelisted." + statusCode);	        
	    		        DriverScript.bResult = true;
	    				ExcelSheet.setCellData("The Record is already whitelisted and status code is: 409", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
	    				break;
//	    			case 403: //Forbidden
//	    				test.log(LogStatus.PASS, "Request has beed forbidden." + statusCode);
//	    	            Log.info("Request has beed forbidden." + statusCode);	        
//	    		        DriverScript.bResult = false;
//	    				ExcelSheet.setCellData("Request has beed forbidden and status code is: 403", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
//	    				break;
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
	    @SuppressWarnings("unchecked")
	    //This function is only for unsubscribe method 
		public static void Subscriptions_withAuth(String url, String module, String request,String UserName, String Pass) {//, String param1, String param2) {
	   	   	try {
	   	   	System.out.println(url);
	    		test.log(LogStatus.PASS, "Verifying " +request+ " of "+ module);
 	            Log.info("Verifying " +request+ " of "+ module);
	    		String bearerToken = auth(url, UserName, Pass);
				 System.out.println(bearerToken);
	    		String validTC = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
	    		String fileName = ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
	    		String Un_Sub_fileName = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
	    		String localDir = System.getProperty("user.dir");
	    		String path =localDir + Constants.Json_Path;
	    		int statusCode;		
	    		File addData = new File(path + fileName);
	    		System.out.println(path + fileName);
	    		String URL = url + module + request;
	    		RestAssured.baseURI = URL;
	            RequestSpecification httpRequest = RestAssured.given();
	            if(fileName.endsWith(".json")){	        
	            httpRequest.header("Content-Type", "application/json");
	            }
	            if(fileName.endsWith(".xml")){
	            httpRequest.header("Content-Type", "application/xml");	
	            }
	            httpRequest.header("Authorization", "Bearer "+ bearerToken);
	            httpRequest.body(addData);
	            Response response = httpRequest.request(Method.POST);
	            statusCode = response.getStatusCode();
	            System.out.println(statusCode);
	            String temp = response.asString();
	            JsonPath jsonPathEvaluator = response.jsonPath();
				// Get specific element from JSON document 
				String p1 = jsonPathEvaluator.get("orderId");
				System.out.println(p1);		
				
				JSONParser parsera = new JSONParser();
				Object objOrg = parsera.parse(new FileReader(path+ fileName));
		        JSONObject jsonObjectOrg = (JSONObject)objOrg;//"organizationID"
		        Map<String, Object> flattenedJsonMap =JsonFlattener.flattenAsMap(jsonObjectOrg.toString());
		        Map<String, Object> items =flattenedJsonMap;            		
		    	String OrgID = items.getOrDefault("SubscribeEvent.organizationID", "No vaLue").toString();
		        System.out.println(items.getOrDefault("SubscribeEvent.organizationID", "No vaLue"));
		      				
			        JSONParser parser = new JSONParser();
					Object obj = parser.parse(new FileReader(path+ Un_Sub_fileName));
			        JSONObject jsonObject = (JSONObject)obj;
			    	System.out.println(jsonObject.toString());
			    	JSONObject updated_jsonObject =replaceINTkeyInJSONObject(jsonObject,"orderID",Integer.parseInt(p1));
			    	System.out.println(updated_jsonObject.toString());
			    	JSONObject updated_jsonObject1 =replacekeyInJSONObject(updated_jsonObject,"organizationID", OrgID);
			    	System.out.println(updated_jsonObject1.toString());
			         
			        //Write JSON file
			        try (FileWriter file = new FileWriter(path +Un_Sub_fileName)) {
			            //We can write any JSONArray or JSONObject instance to the file
			            file.write(updated_jsonObject1.toJSONString()); 
			            file.flush();
			        }
	            
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
//	    				JsonPath jsonPathEvaluator = response.jsonPath();
//	    				// Get specific element from JSON document 
//	    				String p1 = jsonPathEvaluator.get(param1);
//	    				System.out.println(p1);		
//	    				String p2 = jsonPathEvaluator.get(param2);
//	    				System.out.println(p1);		
//	    				RequestSpecification httpRequest1 = RestAssured.given();
//	    			    httpRequest1.request(Method.DELETE,p1);
//	    			    response = httpRequest.queryParam(param1, p1).queryParam(p2, param2).request(Method.PUT, "");
//	    				
	    			   //S DriverScript.bResult=true;	
	    		}else if(validTC.equalsIgnoreCase("No")){
	    			switch(statusCode) {
	    			case 400: //Bad Request
	    				System.out.println("status code- " + statusCode + " Response is- " + temp);
	    				test.log(LogStatus.PASS, "status code- " + statusCode + " Response is- " + temp);
	    				Log.info("status code- " + statusCode + " Response is- " + temp);
	    	        	test.log(LogStatus.PASS, "Getting above response and status code for invalid Test case is " + statusCode);
	    	            Log.info("Getting above response and status code for invalid Test case is " + statusCode);	        
	    		        DriverScript.bResult = true;
	    				ExcelSheet.setCellData("Getting " + temp + " response & status code is: 400", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
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
//	    			case 403: //Forbidden
//	    				test.log(LogStatus.PASS, "Request has beed forbidden." + statusCode);
//	    	            Log.info("Request has beed forbidden." + statusCode);	        
//	    		        DriverScript.bResult = false;
//	    				ExcelSheet.setCellData("Request has beed forbidden and status code is: 403", TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
//	    				break;
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
	    
	    private static JSONObject replacekeyInJSONObject(JSONObject jsonObject, String jsonKey, String jsonValue) {

	        for (Object key : jsonObject.keySet()) {
	            if (key.equals(jsonKey) && ((jsonObject.get(key) instanceof String)||(jsonObject.get(key) instanceof Number)||jsonObject.get(key) ==null)) {
	                jsonObject.put(key, jsonValue);
	                return jsonObject;
	            } else if (jsonObject.get(key) instanceof JSONObject) {
	                JSONObject modifiedJsonobject = (JSONObject) jsonObject.get(key);
	                if (modifiedJsonobject != null) {
	                    replacekeyInJSONObject(modifiedJsonobject, jsonKey, jsonValue);
	                }
	            }

	        }
	        return jsonObject;
	    }
	    private static JSONObject replaceINTkeyInJSONObject(JSONObject jsonObject, String jsonKey, Number jsonValue) {

	        for (Object key : jsonObject.keySet()) {
	            if (key.equals(jsonKey) && ((jsonObject.get(key) instanceof String)||(jsonObject.get(key) instanceof Number)||jsonObject.get(key) ==null)) {
	                jsonObject.put(key, jsonValue);
	                return jsonObject;
	            } else if (jsonObject.get(key) instanceof JSONObject) {
	                JSONObject modifiedJsonobject = (JSONObject) jsonObject.get(key);
	                if (modifiedJsonobject != null) {
	                	replaceINTkeyInJSONObject(modifiedJsonobject, jsonKey, jsonValue);
	                }
	            }

	        }
	        return jsonObject;
	    }

public static void postRequest_XML_JSON(String url) {
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
//		String Param = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps);
//		String validTC = ExcelSheet.getCellData(TestStep, Constants.Col_Param1, Constants.Sheet_TestSteps);
//		Log.info("Update request initiated for this record: " + Param);
//		test.log(LogStatus.INFO, "Update request initiated for this record: " + Param);			 
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
		}catch (Exception e) {
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
