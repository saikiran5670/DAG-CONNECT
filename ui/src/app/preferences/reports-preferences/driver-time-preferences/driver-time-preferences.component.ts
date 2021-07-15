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
    this.translationData = {
      da_report: 'Report',
      da_report_chart: 'Charts',
      da_report_alldetails: 'All Details',
      da_report_alldetails_drivername: 'Driver Name',
      da_report_alldetails_driverid: 'Driver Id',
      da_report_alldetails_starttime: 'Start Time',
      da_report_alldetails_endtime: 'End Time',
      da_report_alldetails_drivetime: 'Drive Time',
      da_report_alldetails_worktime: 'Work Time',
      da_report_alldetails_availabletime: 'Available Time',
      da_report_alldetails_resttime: 'Rest Time',
      da_report_alldetails_servicetime: 'Service Time',
      da_report_chart_zoomchart: 'Zoom Chart',
      da_report_bydriver: 'By Driver',
      da_report_bydriver_date: 'Date',
      da_report_bydriver_drivetime: 'Drive Time',
      da_report_bydriver_worktime: 'Work Time',
      da_report_bydriver_availabletime: 'Available Time',
      da_report_bydriver_resttime: 'Rest Time',
      da_report_bydriver_servicetime: 'Service Time'
    }
  }

  loadDriveTimePreferences(){
    this.reportService.getUserPreferenceReport(this.reportId, this.accountId, this.accountOrganizationId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.initData = [];
    });
  }

  resetColumnData(){
    this.specificDriverData = [];
    this.allDriverTableData = [];
    this.chartData = [];
  }

  preparePrefData(prefData: any){
    prefData.forEach(element => {
      let _data: any;
      if(element.key.includes('da_report_alldetails_')){
         _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 25);   
        }
        this.allDriverTableData.push(_data);
      }else if(element.key.includes('da_report_chart_')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 30);   
        }
        this.chartData.push(_data);
      }else if(element.key.includes('da_report_bydriver_')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 30);   
        }
        this.specificDriverData.push(_data)
      }
    });
    //this.snipData();
    this.setColumnCheckbox();
  }

  allDetailsData: any = [];
  allChartsData: any = [];
  allSpecificData: any = [];
  snipData(){
    this.allDetailsData = this.allDriverTableData.length > 0 ? this.allDriverTableData[0] : [];
    this.allDriverTableData = this.allDriverTableData.filter((i, index) => index != 0);
    this.allChartsData = this.chartData.length > 0 ? this.chartData[0] : [];
    this.chartData = this.chartData.filter((i, index) => index != 0);
    this.allSpecificData = this.specificDriverData.length > 0 ? this.specificDriverData[0] : [];
    this.specificDriverData = this.specificDriverData.filter((i, index) => index != 0);
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
    this.selectionForChart.clear();
    
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
    
    this.chartData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForChart.select(element);
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
      this.specificDriverData.forEach(row => { this.selectionForDriver.select(row) });
      this.validateRequiredField();
    }
  }

  masterToggleForChartsColumns(){
    if(this.isAllSelectedForAllChartsColumns()){
      this.selectionForChart.clear();
      this.validateRequiredField();
    }else{
      this.chartData.forEach(row => { this.selectionForChart.select(row) });
      this.validateRequiredField();
    }
  }

  isAllSelectedForAllChartsColumns(){
    const numSelected = this.selectionForChart.selected.length;
    const numRows = this.chartData.length;
    return numSelected === numRows;
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

    //_allDriverArr.push({ dataAttributeId: this.allDetailsData.dataAtrributeId, state: (this.selectionForAllDriver.selected.length == this.allDriverTableData.length) ? 'A' : 'I', type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });

    this.chartData.forEach(element => {
      let sSearch = this.selectionForChart.selected.filter(item => item.dataAtrributeId == element.dataAtrributeId);
      if(sSearch.length > 0){
        _chartArr.push({ dataAttributeId: element.dataAtrributeId, state: "A", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _chartArr.push({ dataAttributeId: element.dataAtrributeId, state: "I", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    //_chartArr.push({ dataAttributeId: this.allChartsData.dataAtrributeId, state: (this.selectionForChart.selected.length == this.chartData.length) ? 'A' : 'I', type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });

    this.specificDriverData.forEach(element => {
      let sSearch = this.selectionForDriver.selected.filter(item => item.dataAtrributeId == element.dataAtrributeId);
      if(sSearch.length > 0){
        _specificDriverArr.push({ dataAttributeId: element.dataAtrributeId, state: "A", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _specificDriverArr.push({ dataAttributeId: element.dataAtrributeId, state: "I", type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    //_specificDriverArr.push({ dataAttributeId: this.allSpecificData.dataAtrributeId, state: (this.selectionForDriver.selected.length == this.specificDriverData.length) ? 'A' : 'I', type: "D", chartType: "", thresholdType: "", thresholdValue: 0 });

    let objData: any = {
      accountId: this.accountId,
      reportId: this.reportId,
      organizationId: this.accountOrganizationId,
      createdAt: 0,
      modifiedAt: 0,
      atributesShowNoShow: [..._allDriverArr,..._chartArr, ..._specificDriverArr] //-- merge data
    }
    this.reportService.createReportUserPreference(objData).subscribe((prefData: any) => {
      this.loadDriveTimePreferences();
      this.setDriverTimeFlag.emit({ flag: false, msg: this.getSuccessMsg() });
      if((this.router.url).includes("drivetimemanagement")){
        this.reloadCurrentComponent();
      }
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