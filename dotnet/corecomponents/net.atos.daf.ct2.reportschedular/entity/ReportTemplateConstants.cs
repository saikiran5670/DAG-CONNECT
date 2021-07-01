using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public static class ReportTemplateContants
    {
        public const string REPORT_TEMPLATE = @"
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
		<img style='margin:20px 50px' align='right' width='180px' height='80px' src='{1}'><br/><br/><br/><br/>
	
    <h2 style='margin:50px 50px'>[lblTripReportDetails]</h2>
	<div class='detailsDiv'>
	  <table  style='width: 100%;'>
		<tr>
			<td style='width: 30%;'><p><strong>[lblFrom] : </strong>  {2}</P></td>
			<td style='width: 30%;'><p><strong>[lblVehicleGroup] : </strong>  {3}</P></td>
			<td style='width: 30%;'><p><strong>[lblVehicleVIN] : </strong>  {4}</P></td>
		</tr>
		<tr>
			<td style='width: 30%;'><p><strong>[lblTo] : </strong>  {5}</P></td>
			<td style='width: 30%;'><p><strong>[lblVehicleName] : </strong>  {6}</P></td>
			<td style='width: 30%;'><p><strong>[lblRegPlateNumber] : </strong>  {7}</P></td>
		</tr>
	  </table>
	</div><br/><br/>
	
	<table class='reportDetailsTable'>
		<thead>
			<th>[lblStartDate]</th>
			<th>[lblEndDate]</th>
			<th>[lblDistance]</th>
			<th>[lblIdleDuration]</th>
			<th>[lblAverageSpeed]</th>
			<th>[lblAverageWeight]</th>
			<th>[lblStartPosition]</th>
			<th>[lblEndPosition]</th>
			<th>[lblFuelConsumption]</th>
			<th>[lblDrivingTime]</th>
			<th>[lblAlerts]</th>
			<th>[lblEvents]</th>
		</thead>
		{8}
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
