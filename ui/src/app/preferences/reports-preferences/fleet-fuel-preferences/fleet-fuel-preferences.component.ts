import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-fleet-fuel-preferences',
  templateUrl: './fleet-fuel-preferences.component.html',
  styleUrls: ['./fleet-fuel-preferences.component.css']
})
export class FleetFuelPreferencesComponent implements OnInit {
  @Input() tabName: string;
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setDriverTimeFlag = new EventEmitter<any>();
  initData;
  summaryColumnData = [];
  vehicleRankingColumnData = [];
  chartsColumnData = [];
  vehicleDetialsColumnData = [];
  singleVehicleDetialsColumnData = [];
  chartIndex: any = {};
  selectionForSummaryColumns = new SelectionModel(true, []);
  selectionForVehicleRankingColumns = new SelectionModel(true, []);
  selectionForChartsColumns = new SelectionModel(true, []);
  selectionForVehicleDetialsColumns = new SelectionModel(true, []);
  selectionForSingleVehicleDetialsColumns = new SelectionModel(true, []);

  constructor(private resportService: ReportService) { }

  ngOnInit(): void {
    this.loadFleetFuelPreferences();
  }

  loadFleetFuelPreferences() {
    let reportId: any = this.reportListData.filter(i => i.name == 'Fleet Fuel Report')[0].id;
    this.resportService.getReportUserPreference(reportId).subscribe((res) => {
      if (this.tabName == 'Vehicle') {
        this.initData = res['userPreferences']['subReportUserPreferences'][1];
      } else {
        this.initData = res['userPreferences']['subReportUserPreferences'][0];
      }
      console.log("Fleet Fuel Report ", this.initData)
      this.resetColumnData();
      this.preparePrefData(this.initData);
    });
  }

  resetColumnData() {
    this.summaryColumnData = [];
    this.vehicleRankingColumnData = [];
    this.chartsColumnData = [];
    this.vehicleDetialsColumnData = [];
    this.singleVehicleDetialsColumnData = [];
  }

  setColumnCheckbox() {
    this.selectionForSummaryColumns.clear();
    this.selectionForVehicleRankingColumns.clear();
    this.selectionForChartsColumns.clear();
    this.selectionForVehicleDetialsColumns.clear();
    this.selectionForSingleVehicleDetialsColumns.clear();

    this.summaryColumnData.forEach(element => {
      if (element.state == 'A') {
        this.selectionForSummaryColumns.select(element);
      }
    });

    this.vehicleRankingColumnData.forEach(element => {
      if (element.state == 'A') {
        this.selectionForVehicleRankingColumns.select(element);
      }
    });

    this.chartsColumnData.forEach(element => {
      if (element.state == 'A') {
        this.selectionForChartsColumns.select(element);
      }
    });
    
    this.vehicleRankingColumnData.forEach(element => {
      if (element.state == 'A') {
        this.selectionForVehicleDetialsColumns.select(element);
      }
    });
    
    this.singleVehicleDetialsColumnData.forEach(element => {
      if (element.state == 'A') {
        this.selectionForSingleVehicleDetialsColumns.select(element);
      }
    });
    // if (this.summaryColumnData.length > 0 && this.chartsColumnData.length > 0 && this.calenderColumnData.length > 0 && this.detailColumnData.length > 0) {
    //   this.setDefaultFormValues();
    // }
    // this.validateRequiredField();
  }

  preparePrefData(prefData: any) {
    prefData.subReportUserPreferences.forEach(section => {
      section.subReportUserPreferences.forEach(element => {
        let _data: any;
        if (element.name.includes('Driver.Summary') || element.name.includes('Vehicle.Summary')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name, 15);
          }
          this.summaryColumnData.push(_data);
        } else if (element.name.includes('Driver.Chart') || element.name.includes('Vehicle.Chart')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name, 14);
          }
          let index: any;
          switch (element.key) {
            case 'da_report_charts_distanceperday': {
              index = this.chartIndex.distanceIndex = 0;
              break;
            }
            case 'da_report_charts_numberofvehiclesperday': {
              index = this.chartIndex.vehicleIndex = 1;
              break;
            }
            case 'da_report_charts_mileagebasedutilization': {
              index = this.chartIndex.mileageIndex = 2;
              break;
            }
            case 'da_report_charts_timebasedutilization': {
              index = this.chartIndex.timeIndex = 3;
              break;
            }
          }
          this.chartsColumnData[index] = _data;
        } else if (element.name.includes('Driver.VehicleDetails') || element.name.includes('Vehicle.VehicleDetails')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name, 20);
          }
          this.vehicleDetialsColumnData.push(_data);
        } else if (element.name.includes('Driver.SingleVehicleDetails') || element.name.includes('Vehicle.SingleVehicleDetails')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name, 15);
          }
          this.singleVehicleDetialsColumnData.push(_data);
        } else if (element.name.includes('Vehicle.VehicleRanking')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name, 15);
          }
          this.vehicleRankingColumnData.push(_data);
        }
      });
    });
    this.setColumnCheckbox();
  }

  getName(name: any, index: any) {
    let updatedName = name.split('.')[-1];
    return updatedName;
  }

  masterToggle(event, section) {

  }

}
