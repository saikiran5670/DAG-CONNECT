using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FuelBenchmarkDetails
    {
        public int NumberOfActiveVehicles { get; set; }
        public int NumberOfTotalVehicles { get; set; }
        public double TotalFuelConsumed { get; set; }
        public double TotalMileage { get; set; }
        public double AverageFuelConsumption { get; set; }
        public IEnumerable<Ranking> Ranking { get; set; }
    }

    public class FuelBenchmarkConsumption
    {
        public int Numbersofactivevehicle { get; set; }
        public int Totalnumberofvehicle { get; set; }
        public double Totalmileage { get; set; }
        public double Totalfuelconsumed { get; set; }
        public double Averagefuelconsumption { get; set; }
    }

    public class FuelBenchmarkFilter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
    }

}
