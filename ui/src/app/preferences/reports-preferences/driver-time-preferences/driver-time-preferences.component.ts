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
  allDriverTableData:any = [];
  chartData:any = [];
  specificDriverData:any = [];
  selectionForAllDriver = new SelectionModel(true, []);
  selectionForDriver = new SelectionModel(true, []);
  selectionForChart = new SelectionModel(true, []);

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
      rp_dtm_report: 'Report',
      rp_dtm_report_chart: 'Charts',
      rp_dtm_report_chart_zoomchart: 'Zoom Chart',
      rp_dtm_report_alldetails: 'All Details',
      rp_dtm_report_alldetails_drivername: 'Driver Name',
      rp_dtm_report_alldetails_driverid: 'Driver Id',
      rp_dtm_report_alldetails_starttime: 'Start Time',
      rp_dtm_report_alldetails_endtime: 'End Time',
      rp_dtm_report_alldetails_drivetime: 'Drive Time',
      rp_dtm_report_alldetails_worktime: 'Work Time',
      rp_dtm_report_alldetails_availabletime: 'Available Time',
      rp_dtm_report_alldetails_resttime: 'Rest Time',
      rp_dtm_report_alldetails_servicetime: 'Service Time',
      rp_dtm_report_bydriver: 'By Driver',
      rp_dtm_report_bydriver_date: 'Date',
      rp_dtm_report_bydriver_drivetime: 'Drive Time',
      rp_dtm_report_bydriver_worktime: 'Work Time',
      rp_dtm_report_bydriver_availabletime: 'Available Time',
      rp_dtm_report_bydriver_resttime: 'Rest Time',
      rp_dtm_report_bydriver_servicetime: 'Service Time'
    }
  }

  loadDriveTimePreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
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
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.key.includes('rp_dtm_report_chart_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 13);   
              }
              this.chartData.push(_data);
            }else if(item.key.includes('rp_dtm_report_alldetails_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 18);   
              }
              this.allDriverTableData.push(_data);
            }else if(item.key.includes('rp_dtm_report_bydriver_')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 16);   
              }
              this.specificDriverData.push(_data);
            }
          });
        }
      });
      this.setColumnCheckbox();
    }
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

  masterToggleForAllDetailsColumns(){
    if(this.isAllSelectedForAllDetailsColumns()){
      this.selectionForAllDriver.clear();
    }else{
      this.allDriverTableData.forEach(row => { this.selectionForAllDriver.select(row) });
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
    }else{
      this.specificDriverData.forEach(row => { this.selectionForDriver.select(row) });
    }
  }

  masterToggleForChartsColumns(){
    if(this.isAllSelectedForAllChartsColumns()){
      this.selectionForChart.clear();
    }else{
      this.chartData.forEach(row => { this.selectionForChart.select(row) });
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
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm(){
    let _allDriverArr: any = [];
    let _specificDriverArr: any = [];
    let _chartArr: any = [];
    let _parentDataAttr: any = [];

    this.allDriverTableData.forEach(element => {
      let sSearch = this.selectionForAllDriver.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(sSearch.length > 0){
        _allDriverArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _allDriverArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    this.chartData.forEach(element => {
      let sSearch = this.selectionForChart.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(sSearch.length > 0){
        _chartArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _chartArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    this.specificDriverData.forEach(element => {
      let sSearch = this.selectionForDriver.selected.filter(item => item.dataAttributeId == element.dataAttributeId);
      if(sSearch.length > 0){
        _specificDriverArr.push({ dataAttributeId: element.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }else{
        _specificDriverArr.push({ dataAttributeId: element.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      }
    });

    if(this.initData && this.initData.subReportUserPreferences && this.initData.subReportUserPreferences.length > 0){
      _parentDataAttr.push({ dataAttributeId: this.initData.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
      this.initData.subReportUserPreferences.forEach(elem => {
        if(elem.key.includes('rp_dtm_report_chart')){
          if(this.selectionForChart.selected.length == this.chartData.length){ // parent selected
            _parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }else{
            _parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }
        }else if(elem.key.includes('rp_dtm_report_alldetails')){
          if(this.selectionForAllDriver.selected.length == this.allDriverTableData.length){ // parent selected
            _parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }else{
            _parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }
        }else if(elem.key.includes('rp_dtm_report_bydriver')){
          if(this.selectionForDriver.selected.length == this.specificDriverData.length){ // parent selected
            _parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "A", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }else{
            _parentDataAttr.push({ dataAttributeId: elem.dataAttributeId, state: "I", preferenceType: "D", chartType: "", thresholdType: "", thresholdValue: 0 });
          }
        }
      });
    }

    let objData: any = {
      reportId: this.reportId,
      attributes: [..._allDriverArr, ..._chartArr, ..._specificDriverArr, ..._parentDataAttr] //-- merge data
    }

    this.reportService.updateReportUserPreference(objData).subscribe((_prefData: any) => {
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