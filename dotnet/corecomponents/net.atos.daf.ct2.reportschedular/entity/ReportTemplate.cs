using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.entity
{
    public static class ReportTemplate
    {
        public const string REPORT_TEMPLATE = @"<html>
                                                    <head></head>
                                                    <body>
                                                    </br>
                                                    <span><img src='data:image/gif;base64,{0}' /><div><span style='text-align: center;'><h1>{1}</h1></span>
                                                    </br>{2}
                                                    </br>{3}
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
