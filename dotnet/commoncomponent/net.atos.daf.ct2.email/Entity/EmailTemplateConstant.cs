using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.email.Entity
{
    public static class EmailTemplateConstant
    {
        public const string EMAIL_TEMPLATE =
        @"<tr>
            <td>{0}</td>
            <td><a class='button' target='_blank' href='{1}'>[lblDownloadReportButton]</a></td>
            <td><a target='_blank' href='{2}'>[lblUnsubscribe]</a></td>
        </tr> ";
    }
}
