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
			  String Vehicle = " XLR0998HGFFT74611";
			  String EField = "TotalTrip";
			String ExpField = verifyTripData(EField, Startdatetime, Enddatetime, User, Vehicle);
			
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

			  //calculate number of days
			  String NOD="SELECT count(distinct CAST(to_timestamp(end_time_stamp/1000)as DATE)) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= '" + Long.toString(TripStartTime) +"' and end_time_stamp <= '"+ Long.toString(TripEndTime)+ "' and vin in ("+ temp +")";
			  System.out.println(NOD);
			  String NoOfDays = CommonFunctionLib.connectToDatamart(NOD);
			  System.out.println("Number of days are " + NoOfDays);	
			  
			//Total Trip count
			 String Tripcount="SELECT count(trip_id) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(TripStartTime) +" and end_time_stamp <= "+ Long.toString(TripEndTime); 
			 String TCount = CommonFunctionLib.connectToDatamart(Tripcount);
			 System.out.println("Ttotal Trips" + TCount);
			  
			 //getting all trip id's from given time range.
			  String TripId ="SELECT trip_id FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(TripStartTime) +" and end_time_stamp <= " + Long.toString(TripEndTime); 
			  ArrayList TTrip = CommonFunctionLib.connectToDM(TripId); 			  
			  double TotalDistance = 0,TotalFuel = 0,TotalIdleDuration =0, TotalDriveTime=0; 
			  System.out.println("Total Trip id's " + TTrip.size()); 
			  
			  //get max speed fro all trips			  
			  String Mspeed="SELECT MAX(max_speed) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
					  + Long.toString(TripStartTime) +" and end_time_stamp <= "+ Long.toString(TripEndTime); 
			  String MaxSpeed1 = CommonFunctionLib.connectToDatamart(Mspeed);
			  double MaxSpeed =(Double.parseDouble(MaxSpeed1)*3600)/1000;
			  System.out.println("Max speed is " + MaxSpeed);	 
			  
			  //Average Gross Weight Comb 
			  String AGWC="SELECT SUM(average_gross_weight_comb) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= '" + Long.toString(TripStartTime) +"' and end_time_stamp <= '"+ Long.toString(TripEndTime)+ "' and vin in ("+ temp +")";
			  System.out.println(AGWC);
			  String TAGWC = CommonFunctionLib.connectToDatamart(AGWC);
			  System.out.println("Number of days are " + TAGWC);
			  double AvgGrossWgt = Double.parseDouble(TAGWC)/1000; 
				 String AvgGrossWeight = df.format(AvgGrossWgt); 
				 System.out.println(AvgGrossWeight); 
			  
				  //CO2 Emission CO2Emission, AvgGrossWeight
				  String Co2e="SELECT SUM(co2_emission) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= '" + Long.toString(TripStartTime) +"' and end_time_stamp <= '"+ Long.toString(TripEndTime)+ "' and vin in ("+ temp +")";
				  System.out.println(Co2e);
				  String Co2eme = CommonFunctionLib.connectToDatamart(Co2e);
				  System.out.println("Number of days are " + Co2eme);
				  double TCo2eme = Double.parseDouble(Co2eme)/1000; 
					 String CO2Emission = df.format(TCo2eme); 
					 System.out.println(CO2Emission); 
			  
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
			TotalDriveTime =TotalDriveTime + Double.parseDouble(DriveTime);
			
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
				  System.out.println("Number of Trips are "+ TCount);
				  Returnf =TCount;
				  break;
			  case "TotalDistance":	
				  System.out.println("Total Distance of this vehicle is "+ df.format(TotalDistance)+ " in Kilometer");	
				  Returnf = df.format(TotalDistance);
				  break;
			  case "TotalFuel":
				  System.out.println("Total Fuel consumed by this vehicle is "+ df.format(TotalFuel) + " in litter");
				  Returnf =df.format(TotalFuel);
				  break;
			  case "TotalIdleDuration":
				  String TIdleDuration = ConvertSecondsInHHMM((int) TotalIdleDuration);
				  System.out.print("Total idle_duration of this Vehicle is "+ TIdleDuration);
				  Returnf =TIdleDuration;
				  break;
			  case "FuelConsumption":
				  System.out.println("Total Fuel Consumption by this Vehicle is "+(TotalFuel/TotalDistance)*100);
				  Returnf =df.format((TotalFuel/TotalDistance)*100);
				  break;
			  case "NoVehicles":				  
				  System.out.println("Number of vehicles are " + NoVehicles);
				  Returnf =NoVehicles;
				  break;
			  case "AverageDistancePerDay":				  
				  System.out.println("Average distance per day is" + df.format(TotalDistance/Double.parseDouble(NoOfDays)));
				  Returnf =NoVehicles;
				  break;
			  case "TripTime":
				  double TTTime =  TotalDriveTime + TotalIdleDuration;
				  String TotalTripTime = ConvertSecondsInHHMM((int) TTTime);	 
				  System.out.println("Total Trip time of this Vehicle is "+ TotalTripTime);
				  Returnf =TotalTripTime;
				  break;
			  case "MaxSpeed":	  
				  System.out.println("Max speed is " + df.format(MaxSpeed));
				  Returnf = String.valueOf(MaxSpeed);
				  break;
			  case "Co2Emission":	  
				  System.out.println("CO2 Emission is " +CO2Emission);
				  Returnf =CO2Emission;
				  break;
			  case "AverageGrossWeightcombo":
				  System.out.println("Average Gross Weight combo is " + AvgGrossWeight);
				  Returnf =AvgGrossWeight;
				  break;
					 
			  }
			 System.out.println(Returnf);
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
//**************************************** Imprerial ***************************************************************
	public static String verifyImperialTripData(String ExpenctedField, String Sdate, String Edate, String Usr, String VIN) throws Exception {		
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

			  //calculate number of days
			  String NOD="SELECT count(distinct CAST(to_timestamp(end_time_stamp/1000)as DATE)) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= '" + Long.toString(TripStartTime) +"' and end_time_stamp <= '"+ Long.toString(TripEndTime)+ "' and vin in ("+ temp +")";
			  System.out.println(NOD);
			  String NoOfDays = CommonFunctionLib.connectToDatamart(NOD);
			  System.out.println("Number of days are " + NoOfDays);	
			  
			//Total Trip count
			 String Tripcount="SELECT count(trip_id) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(TripStartTime) +" and end_time_stamp <= "+ Long.toString(TripEndTime); 
			 String TCount = CommonFunctionLib.connectToDatamart(Tripcount);
			 System.out.println("Ttotal Trips" + TCount);
			  
			 //getting all trip id's from given time range.
			  String TripId ="SELECT trip_id FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(TripStartTime) +" and end_time_stamp <= " + Long.toString(TripEndTime); 
			  ArrayList TTrip = CommonFunctionLib.connectToDM(TripId); 			  
			  double TotalDistance = 0,TotalFuel = 0,TotalIdleDuration =0, TotalDriveTime=0; 
			  System.out.println("Total Trip id's " + TTrip.size()); 
			  
			  //get max speed fro all trips			  
			  String Mspeed="SELECT MAX(max_speed) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
					  + Long.toString(TripStartTime) +" and end_time_stamp <= "+ Long.toString(TripEndTime); 
			  String MaxSpeed1 = CommonFunctionLib.connectToDatamart(Mspeed);
			  double MaxSpeed =(Double.parseDouble(MaxSpeed1)*3600)/1609;
			  System.out.println("Max speed is " + MaxSpeed);	 
			  
			  //Average Gross Weight Comb 
			  String AGWC="SELECT SUM(average_gross_weight_comb) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= '" + Long.toString(TripStartTime) +"' and end_time_stamp <= '"+ Long.toString(TripEndTime)+ "' and vin in ("+ temp +")";
			  System.out.println(AGWC);
			  String TAGWC = CommonFunctionLib.connectToDatamart(AGWC);
			  System.out.println("Number of days are " + TAGWC);
			  double AvgGrossWgt = Double.parseDouble(TAGWC)/1000; 
				 String AvgGrossWeight = df.format(AvgGrossWgt); 
				 System.out.println(AvgGrossWeight); 
			  
				  //CO2 Emission CO2Emission, AvgGrossWeight
				  String Co2e="SELECT SUM(co2_emission) FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= '" + Long.toString(TripStartTime) +"' and end_time_stamp <= '"+ Long.toString(TripEndTime)+ "' and vin in ("+ temp +")";
				  System.out.println(Co2e);
				  String Co2eme = CommonFunctionLib.connectToDatamart(Co2e);
				  System.out.println("Number of days are " + Co2eme);
				  double TCo2eme = Double.parseDouble(Co2eme)/1000; 
					 String CO2Emission = df.format(TCo2eme); 
					 System.out.println(CO2Emission); 
			  
			  //Verifying distance, driving time, idle duration, average speed and Fuel Consumed
			  for(int i = 0; i< TTrip.size(); i++){
			  System.out.println(i + ". Trip id " + TTrip.get(i)); 
			  //String TripDistance ="SELECT veh_message_distance FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String TripDistance ="SELECT etl_gps_distance FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String dist = CommonFunctionLib.connectToDatamart(TripDistance);
			 System.out.println(" Distance for Trip id "+ TTrip.get(i)+ " is "+ dist + " In meter"); 
			 double SingelTD = Double.parseDouble(dist)/1609; 
			 String SingleTripDistance = df.format(SingelTD); 
			 System.out.println(SingleTripDistance);			 	
			 TotalDistance =TotalDistance + Double.parseDouble(SingleTripDistance);
			
			 // Average Speed Calculated here ---------------------- 
			 String DTime ="SELECT etl_gps_driving_time FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String DriveTime = CommonFunctionLib.connectToDatamart(DTime);			 
			 System.out.println(" Drive time for Trip id "+ TTrip.get(i)+ " is "+DriveTime); 
			String DriveT = ConvertSecondsInHHMM(Integer.parseInt(DriveTime));
			System.out.println(" Drive time in HH:MM format for Trip id "+ TTrip.get(i)+ " is "+DriveT);
			TotalDriveTime =TotalDriveTime + Double.parseDouble(DriveTime);
			
			 String IdTime ="SELECT idle_duration FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String IdealTime = CommonFunctionLib.connectToDatamart(IdTime);
			 System.out.println(" Ideal time for Trip id "+ TTrip.get(i)+ " is "+ IdealTime);
			 String IdealT = ConvertSecondsInHHMM(Integer.parseInt(IdealTime));
			 System.out.println(" Drive time in HH:MM format for Trip id "+ TTrip.get(i)+ " is "+IdealT);
				 
			 TotalIdleDuration =TotalIdleDuration + Double.parseDouble(IdealTime);
			 
			 double ASpeed = Double.parseDouble(dist)/(Double.parseDouble(DriveTime)+Double.parseDouble(IdealTime)); 
			 double AVGSpeed = (ASpeed*3600)/1609; 
			 String TripASpeed = df.format(AVGSpeed);// Average speed in Hrs with default round up
			 System.out.println(" Average speed for Trip id "+ TTrip.get(i)+ " is "+TripASpeed);
			  
			  // Fuel Consumed calculate here ----------------------- 
			 String FConsumed="SELECT etl_gps_fuel_consumed FROM tripdetail.trip_statistics where trip_id=" +"'"+TTrip.get(i)+"'"; 
			 String PTFConsume = CommonFunctionLib.connectToDatamart(FConsumed); 
			 double SingelTF = Double.parseDouble(PTFConsume)/3785; 
			 String Fuel = df.format(SingelTF);//Fuel consumed in litter
			 System.out.println(" Fuel for Trip id"+ TTrip.get(i)+ "is "+Fuel); 
			 TotalFuel =TotalFuel + Double.parseDouble(Fuel);
			
			 } 
			  // calculated total values for given time tange -----------------------
			  String Returnf=null;
			  switch(ReturnField)
			  {
			  case "TotalTrip":				  
				  System.out.println("Number of Trips are "+ TCount);
				  Returnf =TCount;
				  break;
			  case "TotalDistance":	
				  System.out.println("Total Distance of this vehicle is "+ df.format(TotalDistance)+ " in Kilometer");	
				  Returnf = df.format(TotalDistance);
				  break;
			  case "TotalFuel":
				  System.out.println("Total Fuel consumed by this vehicle is "+ df.format(TotalFuel) + " in litter");
				  Returnf =df.format(TotalFuel);
				  break;
			  case "TotalIdleDuration":
				  String TIdleDuration = ConvertSecondsInHHMM((int) TotalIdleDuration);
				  System.out.print("Total idle_duration of this Vehicle is "+ TIdleDuration);
				  Returnf =TIdleDuration;
				  break;
			  case "FuelConsumption":
				  System.out.println("Total Fuel Consumption by this Vehicle is "+(TotalFuel/TotalDistance)*100);
				  Returnf =df.format((TotalFuel/TotalDistance)*100);
				  break;
			  case "NoVehicles":				  
				  System.out.println("Number of vehicles are " + NoVehicles);
				  Returnf =NoVehicles;
				  break;
			  case "AverageDistancePerDay":				  
				  System.out.println("Average distance per day is" + df.format(TotalDistance/Double.parseDouble(NoOfDays)));
				  Returnf =NoVehicles;
				  break;
			  case "TripTime":
				  double TTTime =  TotalDriveTime + TotalIdleDuration;
				  String TotalTripTime = ConvertSecondsInHHMM((int) TTTime);	 
				  System.out.println("Total Trip time of this Vehicle is "+ TotalTripTime);
				  Returnf =TotalTripTime;
				  break;
			  case "MaxSpeed":	  
				  System.out.println("Max speed is " + df.format(MaxSpeed));
				  Returnf = String.valueOf(MaxSpeed);
				  break;
			  case "Co2Emission":	  
				  System.out.println("CO2 Emission is " +CO2Emission);
				  Returnf =CO2Emission;
				  break;
			  case "AverageGrossWeightcombo":
				  System.out.println("Average Gross Weight combo is " + AvgGrossWeight);
				  Returnf =AvgGrossWeight;
				  break;
					 
			  }
			 System.out.println(Returnf);
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
		try {//08/30/2021 13:58:01
			
			 String Startdatetime = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
			  String User = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			  String Enddatetime =ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
			  String VIN = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
//	long St = getTripStartTime("2021.08.30 13:58:01");
//	long Et = getTripEndTime("2021.08.30 14:02:51");
			  long St = getTripStartTime(Startdatetime, User);
			  long Et = getTripEndTime(Enddatetime,User);
	String Vin = "vin = '"+ VIN +"' and";//"'XLRAE75PC0E348696'";
	String TripIdInput ="SELECT trip_id FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(St) +" and end_time_stamp <= " + Long.toString(Et);
	String TripId = CommonFunctionLib.connectToDatamart(TripIdInput);
	
	//Distance (km)
	String TripDistance ="SELECT etl_gps_distance FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "+ Long.toString(St) +" and end_time_stamp <= "+ Long.toString(Et);
	String dist = CommonFunctionLib.connectToDatamart(TripDistance);
	double SingelTD = Double.parseDouble(dist)/1000;
	System.out.println(df.format(SingelTD));
	String t =df.format(SingelTD);
	test.log(LogStatus.INFO,  t + " Database value of Distance (km)" );
	Log.info(t + " Database value of Distance (km)");	
	String table = "";
	 CommonFunctionLib.verifyDBDataInTable(df.format(SingelTD),"Distance (km)",table);
	 
	 //Driving Time (hh:mm)
	 String DTime ="SELECT etl_gps_driving_time FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'"; 
	 String DriveTime = CommonFunctionLib.connectToDatamart(DTime);			 
	 System.out.println(" Drive time for this Trip id "+ TripId + " is "+DriveTime); 
	String DriveT = ConvertSecondsInHHMM(Integer.parseInt(DriveTime));
	System.out.println(DriveT +" Database value of Drive time  (hh:mm)");
	test.log(LogStatus.INFO,  DriveT +" Database value of Drive time  (hh:mm)");
	Log.info(DriveT +" Database value of Drive time  (hh:mm)");
	
	 CommonFunctionLib.verifyDBDataInTable(DriveT,"Driving Time (hh:mm)", table);
	
	 //Idle Duration (hh:mm)
	 String IdTime ="SELECT idle_duration FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'"; 
	 String IdealTime = CommonFunctionLib.connectToDatamart(IdTime);
	 System.out.println(" Ideal time for Trip id "+ TripId+ " is "+ IdealTime);
	 String IdealT = ConvertSecondsInHHMM(Integer.parseInt(IdealTime));
	 System.out.println(IdealT + " Database value of Ideal time  (hh:mm)");
	 test.log(LogStatus.INFO,  IdealT + " Database value of Ideal time  (hh:mm)");
	 Log.info(IdealT + " Database value of Ideal time (hh:mm)");
	 CommonFunctionLib.verifyDBDataInTable(IdealT,"Idle Duration (hh:mm)",table);
	  
	 //Average Speed (km/h)
	 double ASpeed = Double.parseDouble(dist)/(Double.parseDouble(DriveTime)+Double.parseDouble(IdealTime)); 
	 double AVGSpeed = (ASpeed*3600)/1000; 
	 String TripASpeed = df.format(AVGSpeed);// Average speed in Hrs with default round up
	 System.out.println(TripASpeed + " Database value of Average Speed (km/h)");
	 test.log(LogStatus.INFO,  TripASpeed + " Database value of Average Speed (km/h)");
	 Log.info(TripASpeed + " Database value of Average Speed (km/h)");
	 CommonFunctionLib.verifyDBDataInTable(TripASpeed,"Average Speed (km/h)",table);
	
	 //Fuel Consumed (ltr)
	 String FConsumed="SELECT etl_gps_fuel_consumed FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String PTFConsume = CommonFunctionLib.connectToDatamart(FConsumed); 
	 double SingelTF = Double.parseDouble(PTFConsume)/1000; 
	 String Fuel = df.format(SingelTF);//Fuel consumed in litter
	 System.out.println(Fuel + " Database value of Fuel Consumed (ltr)"); 
	 test.log(LogStatus.INFO, Fuel + " Database value of Fuel Consumed (ltr)");
	 Log.info(Fuel + " Database value of Fuel Consumed (ltr)");
	 CommonFunctionLib.verifyDBDataInTable(Fuel,"Fuel Consumed (ltr)",table);
	 
	 //verify start and end date
	 //Start date
	 String StmtStamp="SELECT start_time_stamp FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String STimeStamp = CommonFunctionLib.connectToDatamart(StmtStamp); 
	 String Sdate = CommonFunctionLib.MilisecondtoDate(STimeStamp, "'ulka.pate@atos.net'");
	 System.out.println(Sdate + " Database value of Start date"); 
	 test.log(LogStatus.INFO, Sdate + " Database value of Start date");
	 Log.info(Sdate + " Database value of Start date");
	 CommonFunctionLib.verifyDBDataInTable(Sdate,"Start Date ",table);
	 //End date
	 String EtmtStamp="SELECT end_time_stamp FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String ETimeStamp = CommonFunctionLib.connectToDatamart(EtmtStamp); 
	 String Edate = CommonFunctionLib.MilisecondtoDate(ETimeStamp,"'ulka.pate@atos.net'");	 
	 System.out.println(Edate + " Database value of End date"); 
	 test.log(LogStatus.INFO, Edate + " Database value of End date");
	 Log.info(Edate + " Database value of End date");
	 CommonFunctionLib.verifyDBDataInTable(Edate,"End Date ",table);
	 
	 //verify Alert count for trip
	 String AlertSQL="SELECT no_of_alerts FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String NoOfAlerts = CommonFunctionLib.connectToDatamart(AlertSQL); 	 
	 System.out.println(NoOfAlerts + " Database value of Alters"); 
	 test.log(LogStatus.INFO, NoOfAlerts + " Database value of Alters");
	 Log.info(NoOfAlerts + " Database value of Alters");
	 CommonFunctionLib.verifyDBDataInTable(NoOfAlerts,"Alerts ",table);
	 
	 //verify Odometer value for trip
	 String Odo="SELECT last_odometer FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String Odometer1 = CommonFunctionLib.connectToDatamart(Odo); 	
	 double odm = Double.parseDouble(Odometer1)/1000;
	 System.out.println(df.format(odm));
	 String Odometer =df.format(odm);
	 System.out.println(Odometer + " Database value of Odometer"); 
	 test.log(LogStatus.INFO, Odometer + " Database value of Odometer");
	 Log.info(Odometer + " Database value of Odometer");
	 CommonFunctionLib.verifyDBDataInTable(df.format(odm),"Odometer (km)",table);	

		}catch (Exception e) {
			test.log(LogStatus.FAIL, e.getMessage());
			Log.error("Data is not present in table..." + e.getMessage());
			String screenshotPath = getScreenshot(driver, DriverScript.TestCaseID);
			test.log(LogStatus.FAIL, test.addScreenCapture(screenshotPath));
			ExcelSheet.setCellData(e.getMessage(), TestStep, Constants.Col_TestStepOutput, Constants.Sheet_TestSteps);
			DriverScript.bResult = false;
			
			}	
}
	
	//************************ Imperial Trip values check  **************************************************************
	
	
	public static void verifyImperialTripValue() throws Exception {		
		try {//08/30/2021 13:58:01
			
			 String Startdatetime = ExcelSheet.getCellData(TestStep, Constants.Col_PageObject, Constants.Sheet_TestSteps); 
			  String User = ExcelSheet.getCellData(TestStep, Constants.Col_Parm2, Constants.Sheet_TestSteps);
			  String Enddatetime =ExcelSheet.getCellData(TestStep, Constants.Col_Parm1, Constants.Sheet_TestSteps);
			  String VIN = ExcelSheet.getCellData(TestStep, Constants.Col_Parm3, Constants.Sheet_TestSteps);
//	long St = getTripStartTime("2021.08.30 13:58:01");
//	long Et = getTripEndTime("2021.08.30 14:02:51");
			  long St = getTripStartTime(Startdatetime, User);
			  long Et = getTripEndTime(Enddatetime,User);
	String Vin = "vin = '"+ VIN +"' and";//"'XLRAE75PC0E348696'";
	String TripIdInput ="SELECT trip_id FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "
			  + Long.toString(St) +" and end_time_stamp <= " + Long.toString(Et);
	String TripId = CommonFunctionLib.connectToDatamart(TripIdInput);
	
	//Distance (km)
	String TripDistance ="SELECT etl_gps_distance FROM tripdetail.trip_statistics where "+ Vin +" start_time_stamp >= "+ Long.toString(St) +" and end_time_stamp <= "+ Long.toString(Et);
	String dist = CommonFunctionLib.connectToDatamart(TripDistance);
	double SingelTD = Double.parseDouble(dist)/1609;
	System.out.println(df.format(SingelTD));
	String t =df.format(SingelTD);
	test.log(LogStatus.INFO,  t + " Database value of Distance (km)" );
	Log.info(t + " Database value of Distance (km)");	
	String table = "";
	 CommonFunctionLib.verifyDBDataInTable(df.format(SingelTD),"Distance (km)",table);
	 
	 //Driving Time (hh:mm)
	 String DTime ="SELECT etl_gps_driving_time FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'"; 
	 String DriveTime = CommonFunctionLib.connectToDatamart(DTime);			 
	 System.out.println(" Drive time for this Trip id "+ TripId + " is "+DriveTime); 
	String DriveT = ConvertSecondsInHHMM(Integer.parseInt(DriveTime));
	System.out.println(DriveT +" Database value of Drive time  (hh:mm)");
	test.log(LogStatus.INFO,  DriveT +" Database value of Drive time  (hh:mm)");
	Log.info(DriveT +" Database value of Drive time  (hh:mm)");
	
	 CommonFunctionLib.verifyDBDataInTable(DriveT,"Driving Time (hh:mm)", table);
	
	 //Idle Duration (hh:mm)
	 String IdTime ="SELECT idle_duration FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'"; 
	 String IdealTime = CommonFunctionLib.connectToDatamart(IdTime);
	 System.out.println(" Ideal time for Trip id "+ TripId+ " is "+ IdealTime);
	 String IdealT = ConvertSecondsInHHMM(Integer.parseInt(IdealTime));
	 System.out.println(IdealT + " Database value of Ideal time  (hh:mm)");
	 test.log(LogStatus.INFO,  IdealT + " Database value of Ideal time  (hh:mm)");
	 Log.info(IdealT + " Database value of Ideal time (hh:mm)");
	 CommonFunctionLib.verifyDBDataInTable(IdealT,"Idle Duration (hh:mm)",table);
	  
	 //Average Speed (km/h)
	 double ASpeed = Double.parseDouble(dist)/(Double.parseDouble(DriveTime)+Double.parseDouble(IdealTime)); 
	 double AVGSpeed = (ASpeed*3600)/1609; 
	 String TripASpeed = df.format(AVGSpeed);// Average speed in Hrs with default round up
	 System.out.println(TripASpeed + " Database value of Average Speed (km/h)");
	 test.log(LogStatus.INFO,  TripASpeed + " Database value of Average Speed (km/h)");
	 Log.info(TripASpeed + " Database value of Average Speed (km/h)");
	 CommonFunctionLib.verifyDBDataInTable(TripASpeed,"Average Speed (km/h)",table);
	
	 //Fuel Consumed (ltr)
	 String FConsumed="SELECT etl_gps_fuel_consumed FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String PTFConsume = CommonFunctionLib.connectToDatamart(FConsumed); 
	 double SingelTF = Double.parseDouble(PTFConsume)/3785; 
	 String Fuel = df.format(SingelTF);//Fuel consumed in litter
	 System.out.println(Fuel + " Database value of Fuel Consumed (ltr)"); 
	 test.log(LogStatus.INFO, Fuel + " Database value of Fuel Consumed (ltr)");
	 Log.info(Fuel + " Database value of Fuel Consumed (ltr)");
	 CommonFunctionLib.verifyDBDataInTable(Fuel,"Fuel Consumed (ltr)",table);
	 
	 //verify start and end date
	 //Start date
	 String StmtStamp="SELECT start_time_stamp FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String STimeStamp = CommonFunctionLib.connectToDatamart(StmtStamp); 
	 String Sdate = CommonFunctionLib.MilisecondtoDate(STimeStamp, "'ulka.pate@atos.net'");
	 System.out.println(Sdate + " Database value of Start date"); 
	 test.log(LogStatus.INFO, Sdate + " Database value of Start date");
	 Log.info(Sdate + " Database value of Start date");
	 CommonFunctionLib.verifyDBDataInTable(Sdate,"Start Date ",table);
	 //End date
	 String EtmtStamp="SELECT end_time_stamp FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String ETimeStamp = CommonFunctionLib.connectToDatamart(EtmtStamp); 
	 String Edate = CommonFunctionLib.MilisecondtoDate(ETimeStamp,"'ulka.pate@atos.net'");	 
	 System.out.println(Edate + " Database value of End date"); 
	 test.log(LogStatus.INFO, Edate + " Database value of End date");
	 Log.info(Edate + " Database value of End date");
	 CommonFunctionLib.verifyDBDataInTable(Edate,"End Date ",table);
	 
	 //verify Alert count for trip
	 String AlertSQL="SELECT no_of_alerts FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String NoOfAlerts = CommonFunctionLib.connectToDatamart(AlertSQL); 	 
	 System.out.println(NoOfAlerts + " Database value of Alters"); 
	 test.log(LogStatus.INFO, NoOfAlerts + " Database value of Alters");
	 Log.info(NoOfAlerts + " Database value of Alters");
	 CommonFunctionLib.verifyDBDataInTable(NoOfAlerts,"Alerts ",table);
	 
	 //verify Odometer value for trip
	 String Odo="SELECT last_odometer FROM tripdetail.trip_statistics where trip_id=" +"'"+TripId+"'";  
	 String Odometer1 = CommonFunctionLib.connectToDatamart(Odo); 	
	 double odm = Double.parseDouble(Odometer1)/1609;
	 System.out.println(df.format(odm));
	 String Odometer =df.format(odm);
	 System.out.println(Odometer + " Database value of Odometer"); 
	 test.log(LogStatus.INFO, Odometer + " Database value of Odometer");
	 Log.info(Odometer + " Database value of Odometer");
	 CommonFunctionLib.verifyDBDataInTable(df.format(odm),"Odometer (km)",table);	

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
	
	public static long getTripEndTime(String Enddate, String Usr) throws Exception {
		try {//String Rptdate ="2021.08.17 09:21:26";08/23/2021 11:08:05
			String Enddatetime =Enddate; //"2021.08.23 11:08:05";
			String User =Usr;
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
	
	public static long getTripStartTime(String Startdate, String Usr) throws Exception {
		try {//String Rptdate ="2021.08.17 09:21:26"; 	08/23/2021 11:04:42
			String Startdatetime =Startdate;//"2021.08.23 11:04:42";
			String User =Usr;
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
