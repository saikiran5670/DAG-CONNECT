import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-fuel-benchmarking-table',
  templateUrl: './fuel-benchmarking-table.component.html',
  styleUrls: ['./fuel-benchmarking-table.component.less']
})

// export interface Employee {
//   period : number,	
//   fromDate :string,	
//   toDate :string
// }


export class FuelBenchmarkingTableComponent implements OnInit {

  @Input() startDateRange: any;
  @Input() endDateRange: any;
  @Input() selectionValueBenchmarkBY: any;
  // displayedColumns: string[] = ['period','range1','range2','range3'];
  dataSource: any[];
  displayedColumns: string[] = ['period','range1'];
  firstPeriodColumn: string[] = ['Number of Active Vehicles','Total Fuel Consumed','Total Mileage',
  'Total Mileage','Average Fuel Consumption','Ranking','Fuel Consumption'];

  constructor() { }

  ngOnInit(): void {

  this.loadBenchmarkTable();
    
  }

  loadBenchmarkTable() {
    
    console.log("this.selectionValueBenchmarkBY",this.selectionValueBenchmarkBY)
    if(this.selectionValueBenchmarkBY == "timePeriods") {
       
      console.log("---from TP selection")
      // console.log("---selectedVehicleGroup",selectedVehicleGroup)
        //Response payload for time period
    
        let responseDataTP = 
        [{
        VechileGroupID: "VehicleGroup1",
          vehicleGroupName : "value",
          ActiveVehicle : 4/4,
          TotalFuelConsumed : "59.00 gal",
          TotalMileage : "1360.70 km",
          AverageFuelConumption: "1.60 mpg",
          Ranking : [
            {
              "Vehicle Name": 18.09,
              "VIN": "VIN1",
              "FuelConsumption": "1.50",
              
            }
          ]
        }, 
        {
          VechileGroupID: "VehicleGroup2",
            vehicleGroupName : "value",
            "ActiveVehicle": "5/5",
            "TotalFuelConsumed": "60.00 gal",
            "TotalMileage": "1111.70 km",
            "AverageFuelConumption": "2.60 mpg",
            "Ranking": [
              {
                "Vehicle Name": "12.09",
                "VIN": "VIN1",
                "FuelConsumption": "1.30",
                
              }
            ]
           
          
          }]

    console.log("---responseDataTP--",responseDataTP)
    
          this.dataSource =  [{
            "period": "Number of Active Vehicles",
            "range1": "4/4",
            // "range2": "4/7",
            // "range3": "4/7"
           
          }, {
            "period": "Total Fuel Consumed",
            "range1": "59.00 gal",
            // "range2": "60.70 gal",
            // "range3": "61.10 gal"
          }, {
            "period": "Total Mileage",
            "range1": "1360.70 km",
            // "range2": "1319.33 km",
            // "range3": "X232.15 km"
           
          }, {
            "period": "Average Fuel Consumption",
            "range1": "1.60 mpg",
            // "range2": "6.00 mpg",
            // "range3": "3.73 mpg"
           
          }, {
            "period": "Ranking",
            "range1": "Consumption (Ltrs/ 100 km)",
            // "range2": "Consumption (Ltrs/ 100 km)",
            // "range3": "Consumption (Ltrs/ 100 km)"
           
          }, {
            "period": "Fuel Consumption",
            "range1": "Xyzzz",
            // "range2": "Xyzzz",
            // "range3": "Xyzzz"
          }
        ];


        this.dataSource["responseData"] = responseDataTP;
        console.log("responseData in data source",this.dataSource)
    }else if(this.selectionValueBenchmarkBY == "vehicleGroup"){
      console.log("---from VG selection")
    }



  }
}
