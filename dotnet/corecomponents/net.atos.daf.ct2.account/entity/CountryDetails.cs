using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.account.entity
{
    public class CountryDetails
    {
        public int ID { get; set; }
        public string RegionType { get; set; }
        public string Code { get; set; }
        public int DailingCode { get; set; }
        public string Name { get; set; }
    }
}
