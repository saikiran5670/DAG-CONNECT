using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public static class ReportTemplateContants
    {
        public const string REPORT_TEMPLATE_FLEET_UTILISATION = @"<!doctype html>
<html>
<head>
    <style>
.detailsDiv {{
  border: none;
  background-color: lightblue;
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
    </style>
</head>
<body>
    <table style='width: 100%;'>
        <tr>
            <td><img style='margin:20px 00px' align='left' width='180px' height='80px' src='{0}'></td>
            <td><h2 style='text-align: center'>[lblFleetUtilisationReportDetails]</h2></td>
            <td><img style='margin:0px 0px' align='right' width='180px' height='80px' src='{23}'></td>
        </tr>
    </table>

    <div class='detailsDiv'>
        <table style='width: 100%;'>
            <tr>
                <td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblFrom] : </strong>  {2}</p></td>
                <td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblVehicleGroup] : </strong>  {3}</p></td>
            </tr>
            <tr>
                <td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblTo] : </strong>  {4}</p></td>
                <td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblVehicleName] : </strong>  {5}</p></td>
            </tr>
        </table>
    </div><br /><br />
    <table style='width: 100%;'>
        <tr>
            <td>
                <div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
                    <div fxLayoutAlign='left'>
                        <span>[lblNumberOfVehicles]</span>
                    </div>
                    <div fxLayout='column' fxLayoutAlign='left'>
                        <span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{6}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
                    <div class='areaWidth min-width-35-per' fxLayout='column' fxLayoutAlign='left'>
                        <span>[lblTotalDistance]</span>
                    </div>
                    <div fxLayout='column' fxLayoutAlign='left'>
                        <span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{7}{8}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
                    <div class='areaWidth min-width-35-per' fxLayout='column' fxLayoutAlign='left'>
                        <span>[lblNumberOfTrips]</span>
                    </div>
                    <div fxLayout='column' fxLayoutAlign='left'>
                        <span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{9}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
                    <div fxLayout='column' fxLayoutAlign='right'>
                        <span>[lblAverageDistancePerDay]</span>
                    </div>
                    <div fxLayout='column' fxLayoutAlign='right'>
                        <span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{10} {11}</span>
                    </div>
                </div>
            </td>
            <td>
                <div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
                    <div fxLayout='column' fxLayoutAlign='right'>
                        <span>[lblIdleDuration]</span>
                    </div>
                    <div fxLayout='column' fxLayoutAlign='right'>
                        <span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{12} {13}</span>
                    </div>
                </div>
            </td>
        </tr>
    </table><br /><br />
    
                      <table class='reportDetailsTable'>
                          <thead>
                          <th>[lblVehicleName]</th>
                          <th>[lblVIN]</th>
                          <th>[lblRegistrationNumber]</th>
                          <th>[lblDistance]({14})</th>
                          <th>[lblNumberOfTrips]</th>
                          <th>[lblTripTime]({15})</th>
                          <th>[lblDrivingTime] ({16})</th>
                          <th>[lblIdleDuration] ({17})</th>
                          <th>[lblStopTime] ({18})</th>
                          <th>[lblAverageSpeed] ({19})</th>
                          <th>[lblAverageWeight] ({20})</th>
                          <th>[lblAverageDistance] ({21})</th>
                          <th>[lblOdometer] ({22})</th>
                          </thead>
                          {1}
                      </table>
                  
</body>
</html>";
        public const string REPORT_TEMPLATE_FLEET_FUEL = @"

<!doctype html>
<html>
<head>
<style>
.detailsDiv {{
  border: none;
  background-color: lightblue;    
  text-align: center
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
</style>
</head>
  <body>
		<img style='margin:20px 50px' align='left' width='180px' height='80px'  src='{0}'>
		<img style='margin:20px 50px' align='right' width='220px' height='80px' src='{29}'><br/><br/><br/><br/>
	
    <h2 style='margin:50px 50px'>[lblFleetFuel]</h2>
	<div class='detailsDiv'>
	  <table  style='width: 100%;'>
		<tr>
			<td style='width: 30%;'><p><strong>[lblFrom] : </strong>  {2}</P></td>
			<td style='width: 30%;'><p><strong>[lblVehicleGroup] : </strong>  {3}</P></td>			
		</tr>
		<tr>
			<td style='width: 30%;'><p><strong>[lblTo] : </strong>  {4}</P></td>
			<td style='width: 30%;'><p><strong>[lblVehicleName] : </strong>  {5}</P></td>			
		</tr>
	  </table>
	</div><br/><br/>
<div class='detailsDiv'>
	  <table  style='width: 100%;'>
		<tr>
			<td style='width: 30%;'><p><strong>Number of Trips : </strong>  {6}</P></td>
			<td style='width: 30%;'><p><strong>Distance : </strong>  {7}{8}</P></td>		
			<td style='width: 30%;'><p><strong>Fuel Consumed : </strong>  {9} {10} </P></td>
			<td style='width: 30%;'><p><strong>Idle Duration : </strong>  {11} {12}</P></td>
			<td style='width: 30%;'><p><strong>Fuel Consumption : </strong>   {13} {14}</P></td>	
			<td style='width: 30%;'><p><strong>CO2 Emission : </strong>   {15} {16}</P></td>		
		</tr>
	  </table>
	</div><br/><br/>
	
<table class='reportDetailsTable'>
		<thead>
			<th>Ranking</th>
			<th>Vehicle</th>
			<th>VIN</th>
			<th>Plate Number</th>
			<th>Consumption ({27})</th>			
		</thead>
		{28}
	</table>
<br/>
	<table class='reportDetailsTable'>
		<thead>
			<th>Vehicle Name</th>
			<th>VIN</th>
			<th>Plate Number</th>
			<th>Distance ({17})</th>
			<th>Average distance per day ({18})</th>
			<th>Average Speed ({19})</th>
			<th>Max Speed ({20})</th>
			<th>Number Of Trips</th>
			<th>Average Gross Weight Comb ({21})</th>
			<th>Fuel Consumed ({22})</th>
			<th>Fuel Consumption ({23})</th>
			<th>CO2 Emission ({24})</th>
			<th>Idle Duration (%)</th>
			<th>PTO Duration (%)</th>
			<th>Harsh Brake Duration (%)</th>
			<th>Heavy Throttle Duration (%)</th>
			<th>Cruise Control Distance 30-50 km/h %</th>
			<th>Cruise Control Distance 50-75 km/h %</th>
			<th>Cruise Control Distance >75 km/h %</th>
			<th>Average Traffic Classification</th>
			<th>CC Fuel Consumption ({25})</th>
			<th>Fuel Consumption CC Non Active ({26})</th>
			<th>Idling Consumption</th>
			<th>DPA Score</th>
		</thead>
		{1}
	</table>

  </body>
</html>";
        public const string REPORT_TEMPLATE_FLEET_FUEL_SINGLE = @"

<!doctype html>
<html>
<head>
<style>
.detailsDiv {{
  border: none;
  background-color: lightblue;    
  text-align: center
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
</style>
</head>
  <body>
		<img style='margin:20px 50px' align='left' width='180px' height='80px'  src='{0}'>
		<img style='margin:20px 50px' align='right' width='180px' height='80px' src='{29}'><br/><br/><br/><br/>
	
    <h2 style='margin:50px 50px'>[lblFleetFuel]</h2>
	<div class='detailsDiv'>
	  <table  style='width: 100%;'>
		<tr>
			<td style='width: 30%;'><p><strong>[lblFrom] : </strong>  {2}</P></td>
			<td style='width: 30%;'><p><strong>[lblVehicleGroup] : </strong>  {3}</P></td>			
		</tr>
		<tr>
			<td style='width: 30%;'><p><strong>[lblTo] : </strong>  {4}</P></td>
			<td style='width: 30%;'><p><strong>[lblVehicleName] : </strong>  {5}</P></td>			
		</tr>
	  </table>
	</div><br/><br/>
<div class='detailsDiv'>
	  <table  style='width: 100%;'>
		<tr>
			<td style='width: 30%;'><p><strong>Number of Trips : </strong>  {6}</P></td>
			<td style='width: 30%;'><p><strong>Distance : </strong>  {7}{8}</P></td>		
			<td style='width: 30%;'><p><strong>Fuel Consumed : </strong>  {9} {10} </P></td>
			<td style='width: 30%;'><p><strong>Idle Duration : </strong>  {11} {12}</P></td>
			<td style='width: 30%;'><p><strong>Fuel Consumption : </strong>   {13} {14}</P></td>	
			<td style='width: 30%;'><p><strong>CO2 Emission : </strong>   {15} {16}</P></td>		
		</tr>
	  </table>
	</div><br/><br/>
	
<strong>All Trip Details</strong>
<br/>
<div class='detailsDiv'>
	  <table  style='width: 100%;'>
		<tr>
			<td style='width: 30%;'><p><strong>Vehicle Name : </strong>  {17}</P></td>
			<td style='width: 30%;'><p><strong>VIN : </strong>  {18}</P></td>		
			<td style='width: 30%;'><p><strong>Registration No. : </strong>  {19}</P></td>
		</tr>
	  </table>
	</div><br/>
	<table class='reportDetailsTable'>
		<thead>
			<th>Vehicle Name</th>
			<th>VIN</th>
			<th>RegistrationNo</th>
            <th>Start Date</th>
            <th>End Date</th>
            <th>Distance ({20})</th>
            <th>Start Position</th>
            <th>End Position</th>
            <th>Fuel Consumed ({21})</th>
			<th>Fuel Consumption ({22})</th>
            <th>Idle Duration (%)</th>
            <th>Cruise Control Distance 30-50 km/h %</th>
			<th>Cruise Control Distance 50-75 km/h %</th>
			<th>Cruise Control Distance >75 km/h %</th>
            <th>CO2 Emission ({23})</th>		
			<th>Heavy Throttle Duration (%)</th>
			<th>Harsh Brake Duration (%)</th>
			<th>Average Traffic Classification</th>						
			<th>CC Fuel Consumption ({24})</th>
			<th>Fuel Consumption CC Non Active ({25})</th>
			<th>Idling Consumption</th>
			<th>DPA Score</th>
            <th>Gross Weight Comb ({26})</th>	
            <th>PTO Duration (%)</th>
            <th>Max Speed ({27})</th>
            <th>Average Speed ({28})</th>			
		</thead>
		{1}
	</table>

  </body>
</html>";

        public const string REPORT_TEMPLATE_FUEL_DEVIATION = @"

<!doctype html>
<html>
<head>
<style>
.detailsDiv {{
  border: none;
  background-color: lightblue;    
  text-align: left
}}
.reportDetailsTable {{
  border-collapse: collapse;
  width: 100%;
}}

.reportDetailsTable td, #reportDetailsTable th {{
  border: 1px solid;
  padding: 8px;
}}

.reportDetailsTable tr:nth-child(even){{background-color: #ffffff;}}

.reportDetailsTable th {{
  border: 1px solid;
  padding-top: 12px;
  padding-bottom: 12px;
  text-align: center;
  background-color: lightblue;
  color: black;
}}
thead {{ display: table-header-group }}
tfoot {{ display: table-row-group }}
tr {{ page-break-inside: avoid }}
</style>
</head>
  <body>
  <table style='width: 100%;'>
  <tr>
  <td><img style='margin:20px 00px' align='left' width='180px' height='80px'  src='{0}'></td>
		<td><h2 style='text-align: center'>[lblFuelDeviationReportDetails]</h2></td>
	<td><img style='margin:0px 0px' align='right' width='180px' height='80px' src='{16}'></td>
	</tr>
	</table>
    
	<div class='detailsDiv'>
	  <table  style='width: 100%;'>
		<tr>
			<td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblFrom] : </strong>  {2}</P></td>
			<td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblVehicleGroup] : </strong>  {3}</P></td>
		</tr>
		<tr>
			<td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblTo] : </strong>  {4}</P></td>
			<td style='width: 50%;' align='left'><p style='margin-left: 15%;'><strong>[lblVehicleName] : </strong>  {5}</P></td>
			</tr>
	  </table>
	</div><br/><br/>
	<table style='width: 100%;'>
	<tr>
	<td>
	<div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
		<div fxLayoutAlign='left'>
			<span>[lblFuelIncreaseEvents]</span>
		</div>
		<div fxLayout='column' fxLayoutAlign='left'>
			<span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{6}</span>
		</div>
	</div>
	</td>
	<td>
	<div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
		<div class='areaWidth min-width-35-per' fxLayout='column' fxLayoutAlign='left'>
			<span>[lblFuelDecreaseEvents]</span>
		</div>
		<div fxLayout='column' fxLayoutAlign='left'>
			<span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{7}</span>
		</div>
	</div>
	</td>
	<td>
	<div style='padding: 20px; margin-bottom: 10px; background: #e7e7e7;'>
		<div class='areaWidth min-width-35-per' fxLayout='column' fxLayoutAlign='left'>
			<span>[lblVehiclesWithFuelEvents]</span>
		</div>
		<div fxLayout='column' fxLayoutAlign='left'>
			<span style='font: 500 14px/32px Roboto, 'Helvetica Neue', sans-serif;'>{8}</span>
		</div>
	</div>
	</td>
	</tr>
	</table><br/><br/>
	
	<table class='reportDetailsTable'>
		<thead>
			<th>[lblType]</th>
			<th>[lblDifference]</th>
			<th>[lblVehicleName]</th>
			<th>[lblVIN]</th>
			<th>[lblRegPlateNumber]</th>
			<th>[lblDate]</th>
			<th>[lblOdometer] ({9})</th>
			<th>[lblStartDate]</th>
			<th>[lblEndDate]</th>
			<th>[lblDistance] ({10})</th>
			<th>[lblIdleDuration] ({11})</th>
			<th>[lblAverageSpeed] ({12})</th>
			<th>[lblAverageWeight] ({13})</th>
			<th>[lblStartPosition]</th>
			<th>[lblEndPosition]</th>
			<th>[lblFuelConsumed] ({14})</th>
			<th>[lblDrivingTime] ({15})</th>
			<th>[lblAlerts]</th>
		</thead>
		{1}
	</table>
  </body>
</html>";
        public const string REPORT_SUMMARY_TEMPLATE = @"
            <table style='width: 100%; border-collapse: collapse;' border = '0'>                   
                   <tr>
                        <td style = 'width: 25%;' > <strong>From:</strong>{0}</td>
                        <td style = 'width: 25%;'> <strong>To:</strong> {1}</td>
                        <td style = 'width: 25%;'> <strong>Vehicle:</strong> {2} </td>
                        <td style = 'width: 25%;' > <strong>Vehicle Name:</strong> {3}</td>
                        <td style = 'width: 25%;' > <strong>Registration #:</strong> {4}</td>                        
                  </tr>   
             </table>";
    }
}
