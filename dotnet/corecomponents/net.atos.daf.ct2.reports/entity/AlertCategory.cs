using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class AlertCategory
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class FilterProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class DriverFilter
    {
        public string DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int OrganizationId { get; set; }
    }
}
