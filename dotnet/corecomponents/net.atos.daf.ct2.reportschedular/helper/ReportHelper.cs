using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace net.atos.daf.ct2.reportscheduler.helper
{
    public static class ReportHelper
    {
        public static string ToDataTableAndGenerateHTML<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(DisplayNameHelper.GetDisplayName(prop) ?? prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return GenerateHTMLString(dataTable);
        }

        public static string GenerateHTMLString(DataTable reportData)
        {
            if (reportData.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                //Table start.
                sb.Append("<table>");

                //Adding HeaderRow.
                sb.Append("<tr>");
                foreach (DataColumn column in reportData.Columns)
                {
                    sb.Append("<th>" + column.ColumnName + "</th>");
                }
                sb.Append("</tr>");

                //Adding DataRow.
                foreach (DataRow row in reportData.Rows)
                {
                    sb.Append("<tr>");
                    foreach (DataColumn column in reportData.Columns)
                    {
                        sb.Append("<td>" + row[column.ColumnName].ToString() + "</td>");
                    }
                    sb.Append("</tr>");
                }

                //Table end.
                sb.Append("</table>");
                //ltTable.Text = sb.ToString();
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
