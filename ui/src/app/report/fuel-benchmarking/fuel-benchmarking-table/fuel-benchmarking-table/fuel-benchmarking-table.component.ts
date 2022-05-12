import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { ChartType } from 'chart.js';
import { Label, MultiDataSet } from 'ng2-charts';
import { ProgressBarMode } from '@angular/material/progress-bar';
import { ThemePalette } from '@angular/material/core';
import { ReportService } from 'src/app/services/report.service';
import { ReportMapService } from '../../../report-map.service';

@Component({
  selector: 'app-fuel-benchmarking-table',
  templateUrl: './fuel-benchmarking-table.component.html',
  styleUrls: ['./fuel-benchmarking-table.component.less']
})
export class FuelBenchmarkingTableComponent implements OnInit {
  searchExpandPanel: boolean = true;
  mode: ProgressBarMode = 'determinate';
  color: ThemePalette = 'primary';
  displayedColumnsUpdated: any = [];
  @Input() test;
  @Input() startDateRange: any;
  @Input() endDateRange: any;
  @Input() selectionValueBenchmarkBY: any;
  @Input() benchmarkSelectionChange: any;
  @Input() vehicleGroupSelected: any;
  @Input() prefUnitFormat: any;
  @Input() translationData: any;
  @Input() reportPrefData: any;
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
  firstColumn: string[] = ['numberOfActiveVehicles', 'convertedTotalFuelConsumed', 'convertedTotalMileage', 'convertedAvgFuelConsumption', 'ranking', 'fuelConsumption'];
  doughnutChartLabels: Label[] = ['High', 'Medium', 'Low'];
  doughnutChartData: MultiDataSet = [
    [55, 25, 20]
  ];
  doughnutChartType: ChartType = 'doughnut';
  public pieChartType: ChartType = 'pie';
  chartType: any;

  constructor(private reportService: ReportService, private reportMapService: ReportMapService) { }

  ngOnInit() {
    this.dataSource = [{
      "period": this.translationData.lblNoOfActiveVehicles || "Number of Active Vehicles",
    }, {
      "period": this.translationData.lblTotalFuelConsumed || "Total Fuel Consumed",
    }, {
      "period": this.translationData.lblTotalMileage || "Total Mileage",
    }, {
      "period": this.translationData.lblAverageFuelConsumption || "Average Fuel Consumption",
    }, {
      "period": this.translationData.lblRanking || "Ranking",
    }, {
      "period": this.translationData.lblFuelConsumption || "Fuel Consumption",
    }];
    this.doughnutChartLabels = [this.translationData.lblHigh || 'High', this.translationData.lblMedium || 'Medium', this.translationData.lblLow || 'Low'];
    this.loadBenchmarkTable();
  }

  loadBenchmarkTable() {
    if (this.benchmarkSelectionChange && this.displayedColumns.length > 1) {
      this.displayedColumns = this.displayedColumns.splice(0, 1)
      this.benchmarkSelectionChange = false;
    }
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
        this.test.pop(row);
        if (this.displayedColumns.length > 1) {
          this.displayedColumns.splice(index, 1);
          this.displayedColumnsUpdated = [...this.displayedColumns];
        }
        return true;
      }
    }
  }

  addColumn(data: any, column: any) {
    if (this.displayedColumns.length < 5) {
      // if (!this.displayedColumns.includes(column)) {
      //   this.displayedColumns.push(column);
      // }
      if (!this.displayedColumnsUpdated.includes(column)) {
        if(this.displayedColumnsUpdated.length == 0){
          this.displayedColumnsUpdated.push('period');
        }
         this.displayedColumnsUpdated.push(column);
      }
      this.displayedColumns = [...this.displayedColumnsUpdated];
      for (let colIndx in this.firstColumn) {
        if (this.firstColumn[colIndx] == 'ranking') {
          let rakingSortedData = data.fuelBenchmarkDetails[this.firstColumn[colIndx]].sort((a, b) => (a.fuelConsumption > b.fuelConsumption) ? 1 : ((b.fuelConsumption > a.fuelConsumption) ? -1 : 0));
          for (let row of rakingSortedData) {
            row["ltrVal"] = this.reportMapService.getFuelConsumedUnits(row.fuelConsumption, this.prefUnitFormat, true); // no UOM applicable
          }
          this.dataSource[colIndx][column] = rakingSortedData;
        } else if (this.firstColumn[colIndx] == 'fuelConsumption') {
          let indCol = Number(colIndx) - 1;
          this.dataSource[colIndx][column] = this.updateDoughnutChartData(this.dataSource[indCol][column]);
        } else if (this.firstColumn[colIndx] == 'numberOfActiveVehicles') {
          this.dataSource[colIndx][column] = data.fuelBenchmarkDetails.numberOfActiveVehicles + "/" + data.fuelBenchmarkDetails.numberOfTotalVehicles;
        }
        else {
          this.dataSource[colIndx][column] = data.fuelBenchmarkDetails[this.firstColumn[colIndx]];
        }
      }
    }
  }

  updateDoughnutChartData(rakingData) {
    let highthresholdValue;
    let lowthresholdValue;
    for (let pref1 of this.reportPrefData.subReportUserPreferences) {
      for (let pref of pref1.subReportUserPreferences) {
        if (pref.key == "rp_fb_component_highfuelefficiency") {
          highthresholdValue = pref.thresholdValue;
        } else if (pref.key == "rp_fb_component_lowfuelefficiency") {
          lowthresholdValue = pref.thresholdValue;
        } else if (pref.key == "rp_fb_chart_fuelconsumption") {
          if (pref.chartType == "P") {
            this.chartType = 'pie';
          } else {
            this.chartType = 'doughnut';
          }
        }
      }
    }
    let high = 0;
    let medium = 0;
    let low = 0;
    if (rakingData && rakingData.length > 0) {
      for (let ranking of rakingData) {
        if (parseFloat(ranking.ltrVal) > highthresholdValue && parseFloat(ranking.ltrVal) < lowthresholdValue) {
          medium++;
        } else if (parseFloat(ranking.ltrVal) <= highthresholdValue) {
          high++;
        } else if (parseFloat(ranking.ltrVal) >= lowthresholdValue) {
          low++;
        } else {
          high++;
        }
      }
    }
    let total = high + medium + low;
    let totalparts = 0;
    if (total != 0) {
      totalparts = 100 / total;
    }
    let highVal = totalparts * high;
    let mediumVal = totalparts * medium;
    let lowVal = totalparts * low;
    let testArr = [];
    testArr.push(highVal.toFixed(2));
    testArr.push(mediumVal.toFixed(2));
    testArr.push(lowVal.toFixed(2));
    let test: MultiDataSet = [];
    test.push(testArr);
    return test;
  }

}