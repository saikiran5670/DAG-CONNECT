using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public static class ReportTemplateContants
    {
        public const string REPORT_TEMPLATE_FLEET_UTILISATION = @"
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
		<img style='margin:20px 50px' align='right' width='180px' height='80px' src='{23}'><br/><br/><br/><br/>
	
    <h2 style='margin:50px 50px'>[lblTripReportDetails]</h2>
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
			<td style='width: 30%;'><p><strong>Number of Vehicles : </strong>  {6}</P></td>
			<td style='width: 30%;'><p><strong>Total Distance : </strong>  {7}{8}</P></td>		
			<td style='width: 30%;'><p><strong>Number of Trips : </strong>  {9} </P></td>
			<td style='width: 30%;'><p><strong>Avg. Distance per day : </strong> {10} {11}</P></td>
			<td style='width: 30%;'><p><strong>Idle Duration : </strong>  {12} {13}</P></td>			
		</tr>
	  </table>
	</div><br/><br/>
	
	<table class='reportDetailsTable'>
		<thead>
			<th>Vehicle Name</th>
			<th>VIN</th>
			<th>Plate Number</th>
			<th>Distance ({14})</th>
			<th>Number Of Trips</th>
			<th>Trip Time ({15})</th>
			<th>Driving Time ({16})</th>
			<th>Idle Duration ({17})</th>
			<th>Stop Time ({18})</th>
			<th>Average distance per day ({19})</th>
			<th>Average Speed ({20})</th>
			<th>Average Weight Per Trip ({21})</th>
			<th>Odometer ({22})</th>
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
		<img style='margin:20px 50px' align='right' width='180px' height='80px' src='{34}'><br/><br/><br/><br/>
	
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
			<th>Consumption ({32})</th>			
		</thead>
		{33}
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
			<th>Idle Duration ({25})</th>
			<th>PTO Duration ({26})</th>
			<th>Harsh Brake Duration ({27})</th>
			<th>Heavy Throttle Duration ({28})</th>
			<th>Cruise Control Distance 30-50 km/h</th>
			<th>Cruise Control Distance 50-75 km/h</th>
			<th>Cruise Control Distance >75 km/h</th>
			<th>Average Traffic Classification</th>
			<th>CC Fuel Consumption ({29})</th>
			<th>Fuel Consumption CC Non Active ({30})</th>
			<th>Idling Consumption ({31})</th>
			<th>DPA Score</th>
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
