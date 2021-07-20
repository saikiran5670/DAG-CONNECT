import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { ChartType } from 'chart.js';
import { Label, MultiDataSet } from 'ng2-charts';
import {ProgressBarMode} from '@angular/material/progress-bar';
import { ThemePalette } from '@angular/material/core';

@Component({
  selector: 'app-fuel-benchmarking-table',
  templateUrl: './fuel-benchmarking-table.component.html',
  styleUrls: ['./fuel-benchmarking-table.component.less']
})
export class FuelBenchmarkingTableComponent implements OnInit {

  searchExpandPanel: boolean = true;
  mode: ProgressBarMode = 'determinate';
  color: ThemePalette = 'primary';
  bufferValue = 75;
  @Input() test;
  @Input() startDateRange: any;
  @Input() endDateRange: any;
  @Input() selectionValueBenchmarkBY: any;
  @Input() benchmarkSelectionChange: any;
  vehicleHeaderCount :any = 0;
  initData: any = [];
  responseDataTP: any = {}
  headerArray: any = ["Period"];
  dataSource: any = new MatTableDataSource([]);
  tabledataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tableHeadingwithRange: any = "";
  displayedColumns: string[] = ['period'];
  timerangeColumn: string[] = ['timerangeColumn'];
  firstColumn: string[] = ['numberOfActiveVehicles', 'totalFuelConsumed', 'totalMileage', 'averageFuelConsumption', 'ranking', 'totalFuelConsumed'];

  doughnutChartLabels: Label[] = ['High', 'Medium', 'Low'];
  doughnutChartData: MultiDataSet = [
    [55, 25, 20]
  ];
  doughnutChartType: ChartType = 'doughnut';
  
  constructor() { }

  ngOnInit(): void {
    this.dataSource = [{
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
    }];
    this.loadBenchmarkTable();
  }

  loadBenchmarkTable() {
    //to check if benchmark selection chage
    if(this.benchmarkSelectionChange && this.displayedColumns.length > 1){
      this.displayedColumns = this.displayedColumns.splice(0,1)
      this.benchmarkSelectionChange = false;
    }

    //Building Headings and Data for benchmark Selections
    if (this.selectionValueBenchmarkBY == "timePeriods") {
      this.tableHeadingwithRange = this.startDateRange + " to " + this.endDateRange;

    } else if (this.selectionValueBenchmarkBY == "vehicleGroups") {
      this.tableHeadingwithRange = `Vehicle Group ${this.vehicleHeaderCount}`;
      console.log("---from VG selection")
      
    }
   
      
      for (let row of this.test) {
        this.addColumn(JSON.parse(row), this.tableHeadingwithRange);
      }
  }

  // updateDataSource(tableData: any) {
  //   this.initData = tableData;
  //   this.tabledataSource = new MatTableDataSource(tableData);
  //   setTimeout(() => {
  //     this.tabledataSource.paginator = this.paginator;
  //     this.tabledataSource.sort = this.sort;
  //   });
  // }

  removeColumn(index) {
    for (let row of this.dataSource) {
      let removingColumn = this.displayedColumns[index];
      if (removingColumn !== "period") {
        delete row[this.displayedColumns[index]];
        this.test.pop(row)
      }
    }
    if (this.displayedColumns.length > 1) {
      this.displayedColumns.splice(index, 1)
    }
    this.vehicleHeaderCount--;
  }

  addColumn(data, column) {
    if (this.displayedColumns.length < 5) {
      if (!this.displayedColumns.includes(column)) {
        // this.headerArray.push(column);
        this.displayedColumns.push(column);
      }
      for (let colIndx in this.firstColumn) {
        this.dataSource[colIndx][column] = data.fuelBenchmarkDetails[this.firstColumn[colIndx]];
      }
    }
    this.vehicleHeaderCount++;
  }
}
