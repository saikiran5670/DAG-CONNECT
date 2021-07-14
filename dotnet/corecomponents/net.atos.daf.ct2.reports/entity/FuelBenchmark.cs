using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FuelBenchmark
    {
        public string VIN { get; set; }
        public string Vehicle_Group_Name { get; set; }
        public int Vehicle_group_id { get; set; }
        public int Number_of_Active_Vehicles { get; set; }
        public int Number_of_Total_Vehicles { get; set; }
        public string Total_fuel_consumed { get; set; }
        public string Total_Mileage { get; set; }
        public string Average_fuel_consumption { get; set; }
        public List<Ranking> VehicleRanking { get; set; }
        public string Fuel_consumption_High { get; set; }
        public string Fuel_consumption_Medium { get; set; }
        public string Fuel_consumption_Low { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }

    }
}
