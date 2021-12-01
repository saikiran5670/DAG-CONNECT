import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ReportService } from 'src/app/services/report.service';
import { DataInterchangeService } from '../../../services/data-interchange.service';

@Component({
  selector: 'app-fleet-fuel-preferences',
  templateUrl: './fleet-fuel-preferences.component.html',
  styleUrls: ['./fleet-fuel-preferences.component.less']
})
export class FleetFuelPreferencesComponent implements OnInit {
  @Input() tabName: string;
  @Input() tabIndex: any;
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any = {};
  @Input() generalPreferences;
  @Output() setFuelFleetFlag = new EventEmitter<any>();
  reportId;
  initData;
  summaryColumnData = [];
  vehicleRankingColumnData = [];
  chartsColumnData = [];
  vehicleDetailsColumnData = [];
  singleVehicleDetailsColumnData = [];
  chartIndex: any = {};
  selectionForSummaryColumns = new SelectionModel(true, []);
  selectionForVehicleRankingColumns = new SelectionModel(true, []);
  selectionForChartsColumns = new SelectionModel(true, []);
  selectionForVehicleDetailsColumns = new SelectionModel(true, []);
  selectionForSingleVehicleDetailsColumns = new SelectionModel(true, []);
  fleetFuelForm = new FormGroup({});
  reportData:any;
  unitId;
  showLoadingIndicator: boolean = false;
  lineBarDD: any = [{
    type: 'L',
    name: 'Line Chart'
  }, {
    type: 'B',
    name: 'Bar Chart'
  }];

  chartDefaultValue = {
    "rp_ff_report_vehicle_chart_fuelconsumption" : "L",
    "rp_ff_report_vehicle_chart_co2emission" : "L",
    "rp_ff_report_vehicle_chart_numberoftrips" : "B",
    "rp_ff_report_vehicle_chart_distance" : "L",
    "rp_ff_report_vehicle_chart_idledurationtotaltime" : "L",
    "rp_ff_report_vehicle_chart_fuelconsumed" : "L",
    "rp_ff_report_driver_chart_numberoftrips" : "B",
    "rp_ff_report_driver_chart_distance" : "L",
    "rp_ff_report_driver_chart_fuelconsumed" : "L",
    "rp_ff_report_driver_chart_idledurationtotaltime" : "L",
    "rp_ff_report_driver_chart_fuelconsumption" : "L",
    "rp_ff_report_driver_chart_co2emission" : "L"
  }

  constructor(private reportService: ReportService, private dataInterchangeService: DataInterchangeService) { }

  ngOnInit(): void {
    let accountPreference = JSON.parse(localStorage.getItem('accountInfo')).accountPreference;
    this.unitId = accountPreference.unitId
    let repoId = this.reportListData.filter(i => i.name == 'Fleet Fuel Report');
    if (repoId.length > 0) {
      this.reportId = repoId[0].id;
      this.loadFleetFuelPreferences();
    } else {
      console.error("No report id found!")
    }
  }

  loadFleetFuelPreferences(reloadFlag?: any) {
    this.showLoadingIndicator = true;
    this.reportService.getReportUserPreference(this.reportId).subscribe((res:any) => {
      this.showLoadingIndicator = false;
      this.reportData = res.userPreferences;
      if(reloadFlag){ // close pref tab & goto fuel report
        let _dataObj: any = {
          prefdata: this.reportData,
          type: 'fuel report',
          tab: this.tabIndex == 0 ? 'Vehicle' : 'Driver'
        }
        this.dataInterchangeService.getPrefData(_dataObj);
        this.dataInterchangeService.closedPrefTab(false); // closed pref tab
      }
      if (this.tabName == 'Vehicle') {
        this.initData = res['userPreferences']['subReportUserPreferences'].filter((item) => item.name.includes('Vehicle'));
      } else {
        this.initData = res['userPreferences']['subReportUserPreferences'].filter((item) => item.name.includes('Driver'));
      }
      this.resetColumnData();
      this.preparePrefData(this.initData[0]);
    }, (error) => {
      this.showLoadingIndicator=false;
    });
  }

  resetColumnData() {
    this.summaryColumnData = [];
    this.vehicleRankingColumnData = [];
    console.log("vehicleRankingColumnData", this.vehicleRankingColumnData);
    this.chartsColumnData = [];
    this.vehicleDetailsColumnData = [];
    console.log("vehicleDetailsColumnData", this.vehicleDetailsColumnData);
    this.singleVehicleDetailsColumnData = [];    
    console.log("singleVehicleDetailsColumnData", this.singleVehicleDetailsColumnData);
  }

  setColumnCheckbox() {
    this.selectionForSummaryColumns.clear();
    this.selectionForVehicleRankingColumns.clear();
    this.selectionForChartsColumns.clear();
    this.selectionForVehicleDetailsColumns.clear();
    this.selectionForSingleVehicleDetailsColumns.clear();

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
    
    this.vehicleDetailsColumnData.forEach(element => {
      if (element.state == 'A') {
        this.selectionForVehicleDetailsColumns.select(element);
      }
    });
    
    this.singleVehicleDetailsColumnData.forEach(element => {
      if (element.state == 'A') {
        this.selectionForSingleVehicleDetailsColumns.select(element);
      }
    });
    // Object.keys(this.fleetFuelForm.controls).forEach(key => {
    //   this.fleetFuelForm.controls[key].setValue(this.chartDefaultValue[key]);
    // });
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
          this.chartsColumnData.push(_data);
          this.fleetFuelForm.addControl(element.key, new FormControl(element.chartType != '' ? element.chartType : this.chartDefaultValue[element.key], Validators.required));
        } else if (element.name.includes('Driver.VehicleDetails') || element.name.includes('Vehicle.VehicleDetails')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name, 20);
          }
          this.vehicleDetailsColumnData.push(_data);
        } else if (element.name.includes('Driver.SingleVehicleDetails') || element.name.includes('Vehicle.SingleVehicleDetails')) {
          _data = element;
          if (this.translationData[element.key]) {
            _data.translatedName = this.translationData[element.key];
          } else {
            _data.translatedName = this.getName(element.name, 15);
          }
          this.singleVehicleDetailsColumnData.push(_data);
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

  masterToggle(section){
    if(this.isAllSelected(section)){
      this["selectionFor"+section+"Columns"].clear();
    }else{
      let lowerCaseSection = section.charAt(0).toLowerCase() + section.substring(1);
      this[lowerCaseSection+"ColumnData"].forEach(row => { this["selectionFor"+section+"Columns"].select(row) });
    }
  }

  validateRequiredField(section) {
    let lowerCaseSection = section.toLowerCase();
    let lowerCaeTagName = this.tabName.toLowerCase();
    let selectedElements = this["selectionFor"+section+"Columns"].selected;
    if (selectedElements.length > 0) {
      let _search =selectedElements.filter(i => (i.key == `rp_ff_report_${lowerCaeTagName}_${lowerCaseSection}_vehiclename` || i.key == `rp_ff_report_${lowerCaeTagName}_${lowerCaseSection}_vin` || i.key == `rp_ff_report_${lowerCaeTagName}_${lowerCaseSection}_vehicleregistrationno`));
      if (_search.length == 0) {
        return true;
      }
      return false;
    }
    return true;
  }

  isAllSelected(section){
    const numSelected = this["selectionFor"+section+"Columns"].selected.length;
    let lowerCaseSection = section.charAt(0).toLowerCase() + section.substring(1);
    const numRows = this[lowerCaseSection+"ColumnData"].length;
    return numSelected === numRows;
  }

  onlineBarDDChange(event) {

  }

  getSaveObject(columnData, selectionData) {
    let saveArr = [];
    this[columnData].forEach(element => {
      let sSearch = this[selectionData].selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(sSearch.length > 0){
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
      }else{
        saveArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0, reportId: element.reportId });
      }
    });
    return saveArr;
  }

  onConfirm() {
    let _summaryArr: any = [];
    let _vehicleRankingArr: any = [];
    let _chartsArr: any = [];
    let _vehicleDetailsArr: any = [];
    let _singleVehicleDetailsArr: any = [];
    let parentArr=[];

    _summaryArr = this.getSaveObject('summaryColumnData', 'selectionForSummaryColumns');
    _vehicleRankingArr = this.getSaveObject('vehicleRankingColumnData', 'selectionForVehicleRankingColumns');
    _vehicleDetailsArr = this.getSaveObject('vehicleDetailsColumnData', 'selectionForVehicleDetailsColumns');
    _singleVehicleDetailsArr = this.getSaveObject('singleVehicleDetailsColumnData', 'selectionForSingleVehicleDetailsColumns');
    
    if (this.tabName == 'Vehicle') {
      parentArr.push({ dataAttributeId: this.reportData.dataAttributeId, state: "A", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: this.reportData.reportId  })
    }
    parentArr.push({ dataAttributeId: this.initData[0].dataAttributeId, state: "A", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: this.initData[0].reportId  })
    this.initData[0].subReportUserPreferences.forEach(element => {
         if(element.name.includes('Report.Driver.Summary') || element.name.includes('Report.Vehicle.Summary') ){
          if(this.selectionForSummaryColumns.selected.length == this.summaryColumnData.length){
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }else{
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }
         }else if(element.name.includes('Report.Driver.Chart') || element.name.includes('Report.Vehicle.Chart') ){
          if(this.selectionForChartsColumns.selected.length == this.chartsColumnData.length){
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "C", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }else{
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "C", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }
         }else if(element.name.includes('Report.Driver.VehicleDetails') || element.name.includes('Report.Vehicle.VehicleDetails') ){
          if(this.selectionForVehicleDetailsColumns.selected.length == this.vehicleDetailsColumnData.length){
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }else{
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }
         }else if(element.name.includes('Report.Driver.SingleVehicleDetails') || element.name.includes('Report.Vehicle.SingleVehicleDetails') ){
          if(this.selectionForSingleVehicleDetailsColumns.selected.length == this.singleVehicleDetailsColumnData.length){
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }else{
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }
        }
        else if(element.name.includes('Report.Vehicle.VehicleRanking') ){
          if(this.selectionForVehicleRankingColumns.selected.length == this.vehicleRankingColumnData.length){
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }else{
            parentArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: '', thresholdType: "", thresholdValue: 0, reportId: element.reportId  })
          }
        }
    });
    this.chartsColumnData.forEach((element, index) => {
      let cSearch = this.selectionForChartsColumns.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(cSearch.length > 0){
        _chartsArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "C", chartType: this.fleetFuelForm.get([element.key]).value, thresholdType: "", thresholdValue: 0, reportId: element.reportId  });
      }else{
        _chartsArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "C", chartType: this.fleetFuelForm.get([element.key]).value, thresholdType: "", thresholdValue: 0, reportId: element.reportId  });
      }
    });
   // console.log("save Object", [..._summaryArr, ..._vehicleRankingArr, ..._vehicleDetailsArr, ..._singleVehicleDetailsArr, ..._chartsArr, ...parentArr])
    return [..._summaryArr, ..._vehicleRankingArr, ..._vehicleDetailsArr, ..._singleVehicleDetailsArr, ..._chartsArr, ...parentArr];
  }

  getUnits(key) {
  let unitObj = this.generalPreferences.unit.filter(item => item.id == this.unitId);
  if(key === "rp_ff_report_vehicle_vehicleranking_consumption"){
    if(unitObj[0].value == 'Imperial') {
      return '(mpg(miles per gallon))';
    } else {
      return '(lts/100km)';
    }
  }
  if(key === "rp_ff_report_vehicle_vehicledetails_cruisecontroldistance3050"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)';
    } else {
      return 'km/h (%)';
    }
  }

  if(key === "rp_ff_report_vehicle_vehicledetails_cruisecontroldistance5075"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h (%)';
    }
  }

  if(key === "rp_ff_report_vehicle_vehicledetails_cruisecontroldistance75"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h (%)';
    }
  }
  
  if(key === "rp_ff_report_vehicle_singlevehicledetails_cruisecontroldistance5075"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h (%)';
    }
  }


  if(key === "rp_ff_report_vehicle_singlevehicledetails_cruisecontroldistance3050"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }

  if(key === "rp_ff_report_vehicle_singlevehicledetails_cruisecontroldistance75"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }

  if(key === "rp_ff_report_driver_vehicledetails_cruisecontroldistance3050"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }

  if(key === "rp_ff_report_driver_vehicledetails_cruisecontroldistance75"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }

  if(key === "rp_ff_report_driver_vehicledetails_cruisecontroldistance5075"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }

  if(key === "rp_ff_report_driver_singlevehicledetails_cruisecontroldistance3050"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }

  if(key === "rp_ff_report_driver_singlevehicledetails_cruisecontroldistance5075"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }
  if(key === "rp_ff_report_driver_singlevehicledetails_cruisecontroldistance75"){
    if(unitObj[0].value == 'Imperial'){
      return '(mph)(%)';
    } else {
      return 'km/h(%)';
    }
  }
  if(key === "rp_ff_report_driver_vehicledetails_co2emmision"){
    if(unitObj[0].value == 'Imperial'){
      return 'gr/km';
    } else {
      return 'gr/km';
    }
  }
  }
 }
