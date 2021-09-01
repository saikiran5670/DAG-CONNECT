package modules;
import static executionEngine.DriverScript.TestStep;

import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.json.simple.JSONArray;

import com.relevantcodes.extentreports.LogStatus;

import executionEngine.DriverScript;
import objectProperties.Constants;
import utiliities.ExcelSheet;
import utiliities.Log;
public class TripReportDB extends CommonFunctionLib{
	private static DecimalFormat df = new DecimalFormat("0.00");
	
	public static void verifyDataCalculation() throws Exception {		
		try {
			String Startdatetime ="2021.08.23 13:02:02"; 
			  String User = "'ulka.pate@atos.net'";
			  String Enddatetime ="2021.08.23 13:10:18";
			  String Vehicle = "XLRAE75PC0E348696";
			  String EField = "TotalTrip";
			String ExpField = verifyTripData(EField, Startdatetime, Enddatetime, User, EField);
			
		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}
	
	//*********************Verifying distance, driving time, idle duration, average speed and Fuel Consumed***********************************************
	
	public static String verifyTripData(String ExpenctedField, String Sdate, String Edate, String Usr, String VIN) throws Exception {		
		try {
			// total distance Verification 08/23/2021 13:02:02  08/23/2021 13:10:18

			  String Startdatetime =Sdate; 
			  String User = Usr;
			  String Enddatetime =Edate;
			  String Vehicle = VIN;
			  String Vin = "vin = '"+ Vehicle +"' and";
			  String ReturnField = ExpenctedField;
			  long TripStartTime =CommonFunctionLib.getMilisecond(Startdatetime, User);
			  System.out.println("Trip Start Time "+TripStartTime);			   
			  long TripEndTime =CommonFunctionLib.getMilisecond(Enddatetime, User);
			  System.out.println("Trip End Time "+ TripEndTime);
			 
			  
			// Number of Vehicles in given time range select count(*) from ( select distinct deptno, job from em
			  String Visibility = "SELECT vin FROM master.vehicle where organization_id='10'";
			  List<String> Vhvisibility = CommonFunctionLib.connectToM(Visibility);
			  String jsonStr = JSONArray.toJSONString(Vhvisibility);
		      System.out.println(jsonStr);
		      String str = jsonStr.replace("[", "");
		      String temp1 = str.replace("]", "");
		      String temp = temp1.replace("\"", "'");
			  System.out.println(temp);
			  String NoVeh="SELECT count(distinct vin) FROM tripdetail.trip_statistics where "+ Vin +"  start_time_stamp >= '" + Long.toString(TripStartTime) +"' and end_time_stamp <= '"+ Long.toString(TripEndTime)+ "' and vin in ("+ temp +")";
			  System.out.println(NoVeh);
			  String NoVehicles = CommonFunctionLib.connectToDatamart(NoVeh);
			  System.out.println("Number of vehicles are " + NoVehicles);
			 			  			  
			//Total Trip count
			 String Tripcount="SELECT count(trip_id) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(TripStartTime) +" and end_time_stamp <= "+ Long.toString(TripEndTime); 
			 String TCount = CommonFunctionLib.connectToDatamart(Tripcount);
			 System.out.println("Ttotal Trips" + TCount);
			  
			 //getting all trip id's from given time range.
			  String TripId ="SELECT trip_id FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(TripStartTime) +" and end_time_stamp <= " + Long.toString(TripEndTime); 
			  ArrayList TTrip = CommonFunctionLib.connectToDM(TripId); 			  
			  double TotalDistance = 0,TotalFuel = 0,TotalIdleDuration =0; 
			  System.out.println("Total Trip id's " + TTrip.size()); 
			  
			  //Verifying distance, driving time, idle duration, average speed and Fuel Consumed
			  for(int i = 0; i< TTrip.size(); i++){
			  System.out.println(i + ". Trip id " + TTrip.get(i)); 
			  //String TripDistance ="SELECT veh_message_distance FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String TripDistance ="SELECT etl_gps_distance FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String dist = CommonFunctionLib.connectToDatamart(TripDistance);
			 System.out.println(" Distance for Trip id "+ TTrip.get(i)+ " is "+ dist + " In meter"); 
			 double SingelTD = Double.parseDouble(dist)/1000; 
			 String SingleTripDistance = df.format(SingelTD); 
			 System.out.println(SingleTripDistance);			 	
			 TotalDistance =TotalDistance + Double.parseDouble(SingleTripDistance);
			
			 // Average Speed Calculated here ---------------------- 
			 String DTime ="SELECT etl_gps_driving_time FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String DriveTime = CommonFunctionLib.connectToDatamart(DTime);			 
			 System.out.println(" Drive time for Trip id "+ TTrip.get(i)+ " is "+DriveTime); 
			String DriveT = ConvertSecondsInHHMM(Integer.parseInt(DriveTime));
			System.out.println(" Drive time in HH:MM format for Trip id "+ TTrip.get(i)+ " is "+DriveT);
			 String IdTime ="SELECT idle_duration FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String IdealTime = CommonFunctionLib.connectToDatamart(IdTime);
			 System.out.println(" Ideal time for Trip id "+ TTrip.get(i)+ " is "+ IdealTime);
			 String IdealT = ConvertSecondsInHHMM(Integer.parseInt(IdealTime));
			 System.out.println(" Drive time in HH:MM format for Trip id "+ TTrip.get(i)+ " is "+IdealT);
				 
			 TotalIdleDuration =TotalIdleDuration + Double.parseDouble(IdealTime);
			 
			 double ASpeed = Double.parseDouble(dist)/(Double.parseDouble(DriveTime)+Double.parseDouble(IdealTime)); 
			 double AVGSpeed = (ASpeed*3600)/1000; 
			 String TripASpeed = df.format(AVGSpeed);// Average speed in Hrs with default round up
			 System.out.println(" Average speed for Trip id "+ TTrip.get(i)+ " is "+TripASpeed);
			  
			  // Fuel Consumed calculate here ----------------------- 
			 String FConsumed="SELECT etl_gps_fuel_consumed FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String PTFConsume = CommonFunctionLib.connectToDatamart(FConsumed); 
			 double SingelTF = Double.parseDouble(PTFConsume)/1000; 
			 String Fuel = df.format(SingelTF);//Fuel consumed in litter
			 System.out.println(" Fuel for Trip id"+ TTrip.get(i)+ "is "+Fuel); 
			 TotalFuel =TotalFuel + Double.parseDouble(Fuel);
			
			 } 
			  // calculated total values for given time tange -----------------------
			  String Returnf=null;
			  switch(ReturnField)
			  {
			  case "TotalTrip":				  
				  System.out.println("Total Trips are "+ TCount);
				  Returnf =TCount;
			  case "TotalDistance":	
				  System.out.println("Total Distance of this vehicle is "+ df.format(TotalDistance)+ " in Kilometer");	
				  Returnf = df.format(TotalDistance);
			  case "TotalFuel":
				  System.out.println("Total Fuel consumed by this vehicle is "+ df.format(TotalFuel) + " in litter");
				  Returnf =df.format(TotalFuel);
			  case "TotalIdleDuration":
				  String TIdleDuration = ConvertSecondsInHHMM((int) TotalIdleDuration);
				  System.out.print("Total idle_duration of this Vehicle is "+ TIdleDuration);
				  Returnf =TIdleDuration;
			  case "FuelConsumption":
				  System.out.println("Total Fuel Consumption by this Vehicle is "+(TotalFuel/TotalDistance)*100);
				  Returnf =df.format((TotalFuel/TotalDistance)*100);
			  case "NoVehicles":				  
				  System.out.println("Number of vehicles are " + NoVehicles);
				  Returnf =NoVehicles;
			  }
			  return Returnf;
		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			return null;
			}	
}

	//*********************Verify Single Trip distance**************************************************************
	
	public static void verifyTripDistance() throws Exception {		
		try {
	long St = getTripStartTime("2021.08.23 13:02:02");
	long Et = getTripEndTime("2021.08.23 13:10:18");
	String Vin = "'XLRAE75PC0E348696'";
	String TripIdInput ="SELECT trip_id FROM tripdetail.trip_statistics where vin = "+ Vin +" and start_time_stamp >= "
			  + Long.toString(St) +" and end_time_stamp <= " + Long.toString(Et);
	String TripId = CommonFunctionLib.connectToDatamart(TripIdInput);
	
	//Distance (km)
	String TripDistance ="SELECT etl_gps_distance FROM tripdetail.trip_statistics where vin = 'XLRAE75PC0E348696' and start_time_stamp >= "+ Long.toString(St) +" and end_time_stamp <= "+ Long.toString(Et);
	String dist = CommonFunctionLib.connectToDatamart(TripDistance);
	double SingelTD = Double.parseDouble(dist)/1000;
	System.out.println(df.format(SingelTD));
	String t =df.format(SingelTD);
	test.log(LogStatus.INFO,  t + " Database value of Distance (km)" );
	Log.info(t + " Database value of Distance (km)");	
	 CommonFunctionLib.verifyDBDataInTable(df.format(SingelTD),"Distance (km)");
	 
	 //Driving Time (hh:mm)
	 String DTime ="SELECT etl_gps_driving_time FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'"; 
	 String DriveTime = CommonFunctionLib.connectToDatamart(DTime);			 
	 System.out.println(" Drive time for this Trip id "+ TripId + " is "+DriveTime); 
	String DriveT = ConvertSecondsInHHMM(Integer.parseInt(DriveTime));
	System.out.println(DriveT +" Database value of Drive time  (hh:mm)");
	test.log(LogStatus.INFO,  DriveT +" Database value of Drive time  (hh:mm)");
	Log.info(DriveT +" Database value of Drive time  (hh:mm)");
	 CommonFunctionLib.verifyDBDataInTable(DriveT,"Driving Time (hh:mm)");
	
	 //Idle Duration (hh:mm)
	 String IdTime ="SELECT idle_duration FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'"; 
	 String IdealTime = CommonFunctionLib.connectToDatamart(IdTime);
	 System.out.println(" Ideal time for Trip id "+ TripId+ " is "+ IdealTime);
	 String IdealT = ConvertSecondsInHHMM(Integer.parseInt(IdealTime));
	 System.out.println(IdealT + " Database value of Ideal time  (hh:mm)");
	 test.log(LogStatus.INFO,  IdealT + " Database value of Ideal time  (hh:mm)");
	 Log.info(IdealT + " Database value of Ideal time (hh:mm)");
	 CommonFunctionLib.verifyDBDataInTable(IdealT,"Idle Duration (hh:mm)");
	  
	 //Average Speed (km/h)
	 double ASpeed = Double.parseDouble(dist)/(Double.parseDouble(DriveTime)+Double.parseDouble(IdealTime)); 
	 double AVGSpeed = (ASpeed*3600)/1000; 
	 String TripASpeed = df.format(AVGSpeed);// Average speed in Hrs with default round up
	 System.out.println(TripASpeed + " Database value of Average Speed (km/h)");
	 test.log(LogStatus.INFO,  TripASpeed + " Database value of Average Speed (km/h)");
	 Log.info(TripASpeed + " Database value of Average Speed (km/h)");
	 CommonFunctionLib.verifyDBDataInTable(TripASpeed,"Average Speed (km/h)");
	
	 //Fuel Consumed (ltr)
	 String FConsumed="SELECT etl_gps_fuel_consumed FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String PTFConsume = CommonFunctionLib.connectToDatamart(FConsumed); 
	 double SingelTF = Double.parseDouble(PTFConsume)/1000; 
	 String Fuel = df.format(SingelTF);//Fuel consumed in litter
	 System.out.println(Fuel + " Database value of Fuel Consumed (ltr)"); 
	 test.log(LogStatus.INFO, Fuel + " Database value of Fuel Consumed (ltr)");
	 Log.info(Fuel + " Database value of Fuel Consumed (ltr)");
	 CommonFunctionLib.verifyDBDataInTable(Fuel,"Fuel Consumed (ltr)");

		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}
	
		//*********************get trip End Time in miliseconds **************************************************************
	
	public static long getTripEndTime(String Enddate) throws Exception {
		try {//String Rptdate ="2021.08.17 09:21:26";08/23/2021 11:08:05
			String Enddatetime =Enddate; //"2021.08.23 11:08:05";
			String User ="'ulka.pate@atos.net'";
			long TripEndTime =CommonFunctionLib.getMilisecond(Enddatetime, User);
			System.out.println("Trip End Time "+ TripEndTime);
			test.log(LogStatus.INFO, "Trip End Time "+ TripEndTime);
			Log.info("Trip End Time "+ TripEndTime);
			return TripEndTime;
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;
				return 0;
				}	
	}
	//********************* get trip Start Time in miliseconds **************************************************************
	
	public static long getTripStartTime(String Startdate) throws Exception {
		try {//String Rptdate ="2021.08.17 09:21:26"; 	08/23/2021 11:04:42
			String Startdatetime =Startdate;//"2021.08.23 11:04:42";
			String User ="'ulka.pate@atos.net'";
			long TripStartTime =CommonFunctionLib.getMilisecond(Startdatetime, User);
			System.out.println("Trip Start Time "+TripStartTime);
			test.log(LogStatus.INFO, "Trip Start Time "+ TripStartTime);
			Log.info("Trip Start Time "+ TripStartTime);
			return TripStartTime;
			}catch (Exception e) {
				test.log(LogStatus.FAIL, e.getMessage());
				Log.error("Data is not present in table..." + e.getMessage());
				String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
				test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
				ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
				DriverScript.bResult = false;	
				return 0;
				}	
	}
	
	
}
