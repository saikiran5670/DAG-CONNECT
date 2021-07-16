import { Component, OnInit } from '@angular/core';

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

  displayedColumns: string[] = ['period','range1'];
  // displayedColumns: string[] = ['period','range1','range2','range3'];
  dataSource: any[];

  constructor() { }

  ngOnInit(): void {

    this.dataSource =  [{
      "period": "Number of Active Vehicles",
      "range1": "4/4",
      "range2": "4/7",
      "range3": "4/7"
     
    }, {
      "period": "Total Fuel Consumed",
      "range1": "59.00 gal",
      "range2": "60.70 gal",
      "range3": "61.10 gal"
    }, {
      "period": "Total Mileage",
      "range1": "1360.70 km",
      "range2": "1319.33 km",
      "range3": "X232.15 km"
     
    }, {
      "period": "Average Fuel Consumption",
      "range1": "1.60 mpg",
      "range2": "6.00 mpg",
      "range3": "3.73 mpg"
     
    }, {
      "period": "Ranking",
      "range1": "Consumption (Ltrs/ 100 km)",
      "range2": "Consumption (Ltrs/ 100 km)",
      "range3": "Consumption (Ltrs/ 100 km)"
     
    }, {
      "period": "Fuel Consumption",
      "range1": "Xyzzz",
      "range2": "Xyzzz",
      "range3": "Xyzzz"
    }
  ];
  }

}
