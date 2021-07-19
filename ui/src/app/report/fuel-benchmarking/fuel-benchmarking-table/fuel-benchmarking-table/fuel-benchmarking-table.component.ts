import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableExporterDirective } from 'mat-table-exporter';

@Component({
  selector: 'app-fuel-benchmarking-table',
  templateUrl: './fuel-benchmarking-table.component.html',
  styleUrls: ['./fuel-benchmarking-table.component.less']
})
export class FuelBenchmarkingTableComponent implements OnInit {

  searchExpandPanel: boolean = true;

  @Input() test;
  @Input() startDateRange: any;
  @Input() endDateRange: any;
  @Input() selectionValueBenchmarkBY: any;
  initData: any = [];
  responseDataTP: any = {}
  // displayedColumns: string[] = ['period','range1','range2','range3'];
  dataSource: any = new MatTableDataSource([]);
  tabledataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
@ViewChild(MatPaginator) paginator: MatPaginator;
@ViewChild(MatSort) sort: MatSort;
  displayedColumns: string[] = ['period', 'range1'];
  timerangeColumn: string[] = ['timerangeColumn'];
  firstColumn: string[] = ['ActiveVehicle','TotalFuelConsumed','TotalMileage','AverageFuelConumption','Ranking','TotalFuelConsumed'];
  // firstPeriodColumn: string[] = ['Number of Active Vehicles','Total Fuel Consumed','Total Mileage',
  // 'Total Mileage','Average Fuel Consumption','Ranking','Fuel Consumption'];


  constructor() { }

  ngOnInit(): void {

  this.loadBenchmarkTable();
  
  }

  loadBenchmarkTable() {
    
    console.log("this.selectionValueBenchmarkBY",this.selectionValueBenchmarkBY)
    if(this.selectionValueBenchmarkBY == "timePeriods") {
       
      // console.log("---from TP selection")
      // // console.log("---selectedVehicleGroup",selectedVehicleGroup)
      //   //Response payload for time period
    
      //   this.responseDataTP = 
      //   {
      //   VechileGroupID: "VehicleGroup1",
      //     vehicleGroupName : "value",
      //     ActiveVehicle : 4/4,
      //     TotalFuelConsumed : "59.00 gal",
      //     TotalMileage : "1360.70 km",
      //     AverageFuelConumption: "1.60 mpg",
      //     Ranking : [
      //       {
      //         "Vehicle Name": 18.09,
      //         "VIN": "VIN1",
      //         "FuelConsumption": "1.50",
              
      //       }
      //     ]
      //   }

    // console.log("---responseDataTP--",this.responseDataTP)
    
          this.dataSource =  [{
            "period": "Number of Active Vehicles",
            // "range1": "4/4",
            // "range2": "4/7",
            // "range3": "4/7"
           
          }, {
            "period": "Total Fuel Consumed",
            // "range1": "59.00 gal",
            // "range2": "60.70 gal",
            // "range3": "61.10 gal"
          }, {
            "period": "Total Mileage",
            // "range1": "1360.70 km",
            // "range2": "1319.33 km",
            // "range3": "X232.15 km"
           
          }, {
            "period": "Average Fuel Consumption",
            // "range1": "1.60 mpg",
            // "range2": "6.00 mpg",
            // "range3": "3.73 mpg"
           
          }, {
            "period": "Ranking",
            // "range1": "Consumption (Ltrs/ 100 km)",
            // "range2": "Consumption (Ltrs/ 100 km)",
            // "range3": "Consumption (Ltrs/ 100 km)"
           
          }, {
            "period": "Fuel Consumption",
            // "range1": "Xyzzz",
            // "range2": "Xyzzz",
            // "range3": "Xyzzz"
          }
        ];


        
        console.log("this.test",this.test)
        let index = 1;
        for(let row of this.test) {
          this.addColumn(row, 'range'+index);
          index++;
        }
    }else if(this.selectionValueBenchmarkBY == "vehicleGroup"){
      console.log("---from VG selection")
    }

  }
  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.tabledataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.tabledataSource.paginator = this.paginator;
      this.tabledataSource.sort = this.sort;
    });
  }

  removeColumn(index) {
    for(let row of this.dataSource) {
      delete row[this.displayedColumns[index]];
    }
    this.displayedColumns.splice(index, 1)
  }

  addColumn(data, column) {
    if(!this.displayedColumns.includes(column)) {
      this.displayedColumns.push(column)
    }
    for(let colIndx in this.firstColumn) {
      this.dataSource[colIndx][column] = data[this.firstColumn[colIndx]];
    }
  }

}
