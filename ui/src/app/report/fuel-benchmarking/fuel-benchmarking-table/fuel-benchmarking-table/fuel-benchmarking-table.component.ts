import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { ChartType } from 'chart.js';
import { Label, MultiDataSet } from 'ng2-charts';
import {ProgressBarMode} from '@angular/material/progress-bar';
import { ThemePalette } from '@angular/material/core';
import { ReportService } from 'src/app/services/report.service';


@Component({
  selector: 'app-fuel-benchmarking-table',
  templateUrl: './fuel-benchmarking-table.component.html',
  styleUrls: ['./fuel-benchmarking-table.component.less']
})
export class FuelBenchmarkingTableComponent implements OnInit {

  searchExpandPanel: boolean = true;
  mode: ProgressBarMode = 'determinate';
  color: ThemePalette = 'primary';
  @Input() test;
  @Input() startDateRange: any;
  @Input() endDateRange: any;
  @Input() selectionValueBenchmarkBY: any;
  @Input() benchmarkSelectionChange: any;
  @Input() vehicleGroupSelected:any;
  //vehicleHeaderCount :any = 0;
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
  firstColumn: string[] = ['numberOfActiveVehicles', 'totalFuelConsumed', 'totalMileage', 'averageFuelConsumption', 'ranking', 'fuelConsumption'];

  doughnutChartLabels: Label[] = ['High', 'Medium', 'Low'];
  doughnutChartData: MultiDataSet = [
    [55, 25, 20]
  ];
  doughnutChartType: ChartType = 'doughnut';
  reportPrefData;
  accountOrganizationId;
  accountId;

  constructor(private reportService: ReportService) { }

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
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.getUserPreferenceReport();
    
  }

  loadBenchmarkTable() {
    //to check if benchmark selection chage
    if (this.benchmarkSelectionChange && this.displayedColumns.length > 1) {
      this.displayedColumns = this.displayedColumns.splice(0, 1)
      this.benchmarkSelectionChange = false;
    }

    //Building Headings and Data for benchmark Selections
    if (this.selectionValueBenchmarkBY == "timePeriods") {
      this.tableHeadingwithRange = this.startDateRange + " to " + this.endDateRange;
    } else if (this.selectionValueBenchmarkBY == "vehicleGroups") {
      this.tableHeadingwithRange = this.vehicleGroupSelected;
    }

    for (let row of this.test) {
      this.addColumn(JSON.parse(row), this.tableHeadingwithRange);
    }
  }

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
    //this.vehicleHeaderCount--;
  }

  addColumn(data, column) {
    if (this.displayedColumns.length < 5) {
      if (!this.displayedColumns.includes(column)) {
        this.displayedColumns.push(column);
      }
      for (let colIndx in this.firstColumn) {
        if(this.firstColumn[colIndx] == 'ranking') {
          let rakingSortedData = data.fuelBenchmarkDetails[this.firstColumn[colIndx]].sort((a,b) => (a.fuelConsumption > b.fuelConsumption) ? 1 : ((b.fuelConsumption > a.fuelConsumption) ? -1 : 0));
          for(let row of rakingSortedData) {
            row["ltrVal"] = (row.fuelConsumption/1000).toFixed(2);
          }
          this.dataSource[colIndx][column] = rakingSortedData;
        } else if(this.firstColumn[colIndx] == 'fuelConsumption') {
          let indCol = Number(colIndx) - 1;
          this.dataSource[colIndx][column] = this.updateDoughnutChartData(this.dataSource[indCol][column]);
        }else if(this.firstColumn[colIndx] == 'numberOfActiveVehicles'){
          console.log("total vehicles", data);
          this.dataSource[colIndx][column] = data.fuelBenchmarkDetails.numberOfActiveVehicles + "/" + data.fuelBenchmarkDetails.numberOfTotalVehicles;
        }
        else {
          this.dataSource[colIndx][column] = data.fuelBenchmarkDetails[this.firstColumn[colIndx]];
        }
      }
    }
    //this.vehicleHeaderCount++;
  }
  
  getUserPreferenceReport() {
    this.reportService.getUserPreferenceReport(6, this.accountId, this.accountOrganizationId).subscribe((data: any) => {
      this.reportPrefData = data["userPreferences"];
      this.loadBenchmarkTable();
    });
  }

  updateDoughnutChartData(rakingData) {
    let highthresholdValue;
    let lowthresholdValue;
    for (let pref of this.reportPrefData) {
      if (pref.key == "da_report_component_highfuelefficiency") {
        highthresholdValue = pref.thresholdValue;
      } else if (pref.key == "da_report_component_lowfuelefficiency") {
        lowthresholdValue = pref.thresholdValue;
      } else if (pref.key == "rp_fb_chart_fuelconsumption") {
        if(pref.chartType == "P") {
          this.doughnutChartType = 'pie';
         } else {
          this.doughnutChartType = 'doughnut';
         }
      }
    }
    let high = 0;
    let medium = 0;
    let low = 0;
    if (rakingData && rakingData.length > 0) {
      for (let ranking of rakingData) {
        if (highthresholdValue <= parseFloat(ranking.ltrVal)) {
          high++;
        } else if (lowthresholdValue >= parseFloat(ranking.ltrVal)) {
          low++;
        } else {
          medium++;
        }
      }
    }
    let total = high + medium + low;
    let totalparts = 0;
    if(total != 0) {
      totalparts = 100 / total;
    }
    let highVal = totalparts*high;
    let mediumVal = totalparts * medium;
    let lowVal = totalparts * low;
    let testArr = [];
    testArr.push(highVal);
    testArr.push(mediumVal);
    testArr.push(lowVal);
    let test: MultiDataSet = [];
    test.push(testArr);
    return test;
  }
  
}
