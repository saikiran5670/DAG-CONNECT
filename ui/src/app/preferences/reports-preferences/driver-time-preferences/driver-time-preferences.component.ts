import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-driver-time-preferences',
  templateUrl: './driver-time-preferences.component.html',
  styleUrls: ['./driver-time-preferences.component.less']
})
export class DriverTimePreferencesComponent implements OnInit {

  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setDriverTimeFlag = new EventEmitter<any>();
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  reportId: any;
  initData: any = [];
  allDriverTableData:any=[];
  chartData:any=[];
  specificDriverData:any=[];
  selectionForAllDriver = new SelectionModel(true, []);
  selectionForDriver = new SelectionModel(true, []);
  selectionForChart = new SelectionModel(true, []);
  reqField: boolean = false;

  constructor(private reportService: ReportService, private router: Router) { }

  ngOnInit(){ 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Drive Time Management');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 9; //- hard coded for Drive Time Management Report
    }
    this.translationUpdate();
    this.loadDriveTimePreferences();
  }

  translationUpdate(){
    this.translationData.da_report_details_averageweight = 'Average Weight';
    this.translationData.da_report_details_vin = 'VIN';
    this.translationData.da_report_details_vehiclename = 'Vehicle Name';
    this.translationData.da_report_details_alerts = 'Alerts';
    this.translationData.da_report_details_registrationnumber = 'Reg. Plate Number';
    this.translationData.da_report_details_events = 'Events';
    this.translationData.da_report_details_odometer = 'Odometer';
    this.translationData.da_report_details_averagespeed = 'Average Speed';
    this.translationData.da_report_details_drivingtime = 'Driving Time';
    this.translationData.da_report_details_fuelconsumed = 'Fuel consumed';
    this.translationData.da_report_details_startposition = 'Start Position';
    this.translationData.da_report_details_idleduration = 'Idle Duration';
    this.translationData.da_report_details_startdate = 'Start Date';
    this.translationData.da_report_details_distance = 'Distance';
    this.translationData.da_report_details_enddate = 'End Date';
    this.translationData.da_report_details_endposition = 'End Position';
  }

  loadDriveTimePreferences(){
    this.reportService.getUserPreferenceReport(this.reportId, this.accountId, this.accountOrganizationId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.preparePrefData(this.initData);

    //  this.initData = this.getTranslatedColumnName(this.initData);
    //  this.setColumnCheckbox();
    //  this.validateRequiredField();
    }, (error)=>{
      this.initData = [];
    });
  }

  preparePrefData(prefData: any){
    prefData.forEach(element => {
      let _data: any;
      if(element.key.includes('da_report_alldriver_details')){
         _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 25);   
        }
        this.allDriverTableData.push(_data);
      }else if(element.key.includes('da_report_specificdriver_details_charts')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 31);   
        }
        this.chartData.push(_data);
      }else if(element.key.includes('da_report_specificdriver')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 31);   
        }
        this.specificDriverData.push(_data)
      }
    });
    this.setColumnCheckbox();
  }


  getName(name: any, index: any) {
    let updatedName = name.slice(index);
    return updatedName;
  }

  zoomClicked(evt){
    this.chartData['state']
  }

  setColumnCheckbox(){
    this.selectionForAllDriver.clear();
    this.selectionForDriver.clear();
    
    this.allDriverTableData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForAllDriver.select(element);
      }
    });
    
    this.specificDriverData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForDriver.select(element);
      }
    });
  }

  allDriverCheckboxClicked(_evt,data){

  }

  validateRequiredField(){
    let _flag = true;
    // if(this.selectionForDetail.selected.length > 0){
    //   let _search = this.selectionForDetail.selected.filter(i => (i.key == 'da_report_details_vehiclename' || i.key == 'da_report_details_vin' || i.key == 'da_report_details_registrationnumber'));
    //   if(_search.length){
    //     _flag = false;
    //   }
    // }
    this.reqField = _flag;
  }

  masterToggleForAllDetailsColumns(){
    if(this.isAllSelectedForAllDetailsColumns()){
      this.selectionForAllDriver.clear();
      this.validateRequiredField();
    }else{
      this.allDriverTableData.forEach(row => { this.selectionForAllDriver.select(row) });
      this.validateRequiredField();
    }
  }

  isAllSelectedForAllDetailsColumns(){
    const numSelected = this.selectionForAllDriver.selected.length;
    const numRows = this.allDriverTableData.length;
    return numSelected === numRows;
  }

  masterToggleForSpecificColumns(){
    if(this.isAllSelectedForAllGeneralColumns()){
      this.selectionForDriver.clear();
      this.validateRequiredField();
    }else{
      this.allDriverTableData.forEach(row => { this.selectionForDriver.select(row) });
      this.validateRequiredField();
    }
  }

  isAllSelectedForAllGeneralColumns(){
    const numSelected = this.selectionForDriver.selected.length;
    const numRows = this.specificDriverData.length;
    return numSelected === numRows;
  }

  checkboxClicked(event: any, rowData: any){
    //this.validateRequiredField();
  }

  checkboxLabelForColumns(row?: any){
  
  }

  chartClicked($event, data){

  }
  onCancel(){
    this.setDriverTimeFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onReset(){
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onConfirm(){
    let _allDriverArr: any = [];
    let _specificDriverArr: any = [];
    let _chartArr:any=[];
    this.allDriverTableData.forEach(element => {
      let sSearch = this.selectionForAllDriver.selected.filter(item => item.dataAtrributeId == element.dataAtrributeId);
      if(sSearch.length > 0){
        _allDriverArr.push({ dataAttributeId: element.dataAtrributeId, state: "A", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _allDriverArr.push({ dataAttributeId: element.dataAtrributeId, state: "I", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    this.chartData.forEach(element => {
      let sSearch = this.selectionForChart.selected.filter(item => item.dataAtrributeId == element.dataAtrributeId);
      if(sSearch.length > 0){
        _chartArr.push({ dataAttributeId: element.dataAtrributeId, state: "A", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _chartArr.push({ dataAttributeId: element.dataAtrributeId, state: "I", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });
    this.specificDriverData.forEach(element => {
      let sSearch = this.selectionForDriver.selected.filter(item => item.dataAtrributeId == element.dataAtrributeId);
      if(sSearch.length > 0){
        _specificDriverArr.push({ dataAttributeId: element.dataAtrributeId, state: "A", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _specificDriverArr.push({ dataAttributeId: element.dataAtrributeId, state: "I", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    let objData: any = {
      accountId: this.accountId,
      reportId: this.reportId,
      organizationId: this.accountOrganizationId,
      createdAt: 0,
      modifiedAt: 0,
      atributesShowNoShow: [..._allDriverArr, ..._specificDriverArr] //-- merge data
    }
    this.reportService.createReportUserPreference(objData).subscribe((prefData: any) => {
      this.loadDriveTimePreferences();
      this.setDriverTimeFlag.emit({ flag: false, msg: this.getSuccessMsg() });
    });
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
  }


}
