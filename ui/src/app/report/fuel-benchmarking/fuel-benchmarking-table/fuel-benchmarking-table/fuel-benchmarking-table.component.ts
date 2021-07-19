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
  headerArray: any = ["Period"];
  // displayedColumns: string[] = ['period','range1','range2','range3'];
  dataSource: any = new MatTableDataSource([]);
  tabledataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
@ViewChild(MatPaginator) paginator: MatPaginator;
@ViewChild(MatSort) sort: MatSort;
tableHeadingwithRange:any = "";
  displayedColumns: string[] = ['period'];
  timerangeColumn: string[] = ['timerangeColumn'];
  firstColumn: string[] = ['numberOfActiveVehicles','totalFuelConsumed','totalMileage','averageFuelConsumption','ranking','totalFuelConsumed'];
  // firstPeriodColumn: string[] = ['Number of Active Vehicles','Total Fuel Consumed','Total Mileage',
  // 'Total Mileage','Average Fuel Consumption','Ranking','Fuel Consumption'];


  constructor() { }

  ngOnInit(): void {

    this.dataSource =  [{
      "period": "Number of Active Vehicles",

     
    }, {
      "period": "Total Fuel Consumed",
    }, {
      "period": "Total Mileage",
     
    }, {
      "period": "Average Fuel Consumption",
     
    }, {
      "period": "Ranking",
     
    }, {
      "period": "Fuel Consumption",
    }
  ];

    this.loadBenchmarkTable();
    
  }
  
  loadBenchmarkTable() {
    console.log("-------this.selectedTime from loadBenchmarkTable--", this.startDateRange)
    console.log("-------this.end Time from loadBenchmarkTable--", this.endDateRange)
    this.tableHeadingwithRange = this.startDateRange + "to" + this.endDateRange;
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
    
      


        
        console.log("this.test",this.test)
        // let index = 1;
        for(let row of this.test) {
          this.addColumn(row, this.tableHeadingwithRange);
          // index++;
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
    // if(this.test.length > 1){

      for(let row of this.dataSource) {
        let removingColumn = this.displayedColumns[index];
        if( removingColumn !== "period"){

          delete row[this.displayedColumns[index]];
          this.test.pop(row)
        }
      }
      if(this.displayedColumns.length > 1){
      this.displayedColumns.splice(index, 1)
      }
    // }
  }

  addColumn(data, column) {
    console.log("----startDateRange from addColumn--",this.startDateRange)
    console.log("----endDateRange--from addColumn--",this.endDateRange)
    if(this.displayedColumns.length <5){

   
    if(!this.displayedColumns.includes(column)) {
      this.headerArray.push(this.startDateRange + "to" + this.endDateRange)
      console.log("--headerArray--",this.headerArray)
      this.displayedColumns.push(column)
    }
    for(let colIndx in this.firstColumn) {
      this.dataSource[colIndx][column] = data[this.firstColumn[colIndx]];
    }
  }
}
}
