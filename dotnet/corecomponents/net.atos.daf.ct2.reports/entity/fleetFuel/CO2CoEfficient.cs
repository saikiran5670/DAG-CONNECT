using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class CO2Coefficient
    {
        public int Id { get; set; }
        public string Discription { get; set; }
        public string Fuel_Type { get; set; }
        public double Coeffficient { get; set; }
    }
}
