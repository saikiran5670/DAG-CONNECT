using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class FuelBenchmark
    {
        public string VIN { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleGroupId { get; set; }
        public int NumberOfActiveVehicles { get; set; }
        public int NumberOfTotalVehicles { get; set; }
        public string TotalFuelConsumed { get; set; }
        public string TotalMileage { get; set; }
        public string AverageFuelConsumption { get; set; }
        public List<Ranking> VehicleRanking { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }

    }

    public class FuelBenchmarkConsumption
    {
        public int Numbersofactivevehicle { get; set; }
        public int Totalnumberofvehicle { get; set; }
        public int Totalmileage { get; set; }
        public decimal Totalfuelconsumed { get; set; }
        public decimal Averagefuelconsumption { get; set; }
    }

    public class FuelBenchmarkConsumptionParameter
    {
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public List<string> VINs { get; set; }
    }

}
