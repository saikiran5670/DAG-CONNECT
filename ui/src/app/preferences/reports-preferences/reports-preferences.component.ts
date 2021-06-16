import { SelectionModel } from '@angular/cdk/collections';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-reports-preferences',
  templateUrl: './reports-preferences.component.html',
  styleUrls: ['./reports-preferences.component.less']
})

export class ReportsPreferencesComponent implements OnInit {
  localStLanguage: any;
  displayMessage: any = '';
  @Input() translationData: any = {};
  updateMsgVisible: boolean = false;
  initData: any = [];
  displayedColumns = ['All','translatedName'];
  showLoadingIndicator: any = false;
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  accountId: number;
  accountOrganizationId: number;
  roleID: number;
  selectionForColumns = new SelectionModel(true, []);
  showTripReport: boolean = false;
  showFleetUtilisationReport: boolean = false;
  editTripFlag: boolean = false;
  editFleetUtilisationFlag: boolean = false;
  tripReportId = 0; //- Trip report
  reqField: boolean = false;
  reportListData: any = [];

  constructor(  private reportService: ReportService, private router: Router) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    this.translationUpdate();
    this.loadReportData();
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

  loadReportData(){
    this.showLoadingIndicator = true;
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      this.reportListData = reportList.reportDetails;
      this.tripReportId = this.reportListData.filter(i => i.name == 'Trip Report')[0].id;
      this.reportService.getUserPreferenceReport(this.tripReportId, this.accountId, this.accountOrganizationId).subscribe((data : any) => {
        this.initData = data["userPreferences"];
        this.initData = this.getTranslatedColumnName(this.initData);
        this.setColumnCheckbox();
        this.validateRequiredField();
        this.hideloader();
        this.updatedTableData(this.initData);
      }, (error) => {
        this.initData = [];
        this.hideloader();
        this.updatedTableData(this.initData);
      });
    }, (err)=>{
      console.log('Report not found...');
      this.reportListData = [];
    });
  }

  getTranslatedColumnName(prefData: any){
    prefData.forEach(element => {
      if(this.translationData[element.key]){
        element.translatedName = this.translationData[element.key];  
      }else{
        element.translatedName = this.getName(element.name);   
      }
    });
    return prefData;
  }

  setColumnCheckbox(){
    this.selectionForColumns.clear();
    this.initData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForColumns.select(element);
      }
    });
  }

  updatedTableData(tableData : any) {
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
    });
  }

  getName(name: any) {
    let updatedName = name.slice(15);
    return updatedName;
  }

  
  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  onClose() {
    this.updateMsgVisible = false;
  }

  editTripReportPreferences(){
    this.editTripFlag = true;
  }

  isAllSelectedForColumns(){
    const numSelected = this.selectionForColumns.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForColumns(){
    if(this.isAllSelectedForColumns()){
      this.selectionForColumns.clear();
      this.validateRequiredField();
    }else{
      this.dataSource.data.forEach(row => { this.selectionForColumns.select(row) });
      this.validateRequiredField();
    }
  }

  validateRequiredField(){
    let _flag = true;
    if(this.selectionForColumns.selected.length > 0){
      let _search = this.selectionForColumns.selected.filter(i => (i.key == 'da_report_details_vehiclename' || i.key == 'da_report_details_vin' || i.key == 'da_report_details_registrationnumber'));
      if(_search.length){
        _flag = false;
      }
    }
    this.reqField = _flag;
  }

  checkboxLabelForColumns(row?: any): string{
    if(row)
      return `${this.isAllSelectedForColumns() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForColumns.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  onCancel(){
    this.editTripFlag = false;
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onReset(){
    this.setColumnCheckbox();
    this.validateRequiredField();
  }

  onConfirm(){
    let _dataArr: any = [];
    this.initData.forEach(element => {
      let search = this.selectionForColumns.selected.filter(item => item.dataAtrributeId == element.dataAtrributeId);
      if(search.length > 0){
        _dataArr.push({ dataAttributeId: element.dataAtrributeId, state: "A" });
      }else{
        _dataArr.push({ dataAttributeId: element.dataAtrributeId, state: "I" });
      }
    });

    let objData: any = {
      accountId: this.accountId,
      reportId: this.tripReportId,
      organizationId: this.accountOrganizationId,
      createdAt: 0,
      modifiedAt: 0,
      type: "D", //-- For trip report
      chartType: "D", //-- 'chartType' based on dashboard pref chart-Type. like Doughnut/Pie/Line/Bar etc
      atributesShowNoShow: _dataArr
    }
    this.reportService.createTripReportPreference(objData).subscribe((tripPrefData: any) => {
      this.loadReportData();
      this.successMsgBlink(this.getSuccessMsg());
      this.editTripFlag = false;
      if((this.router.url).includes("tripreport")){
        this.reloadCurrentComponent();
      }
    }, (error) => {
      console.log(error);
    });
  }

  reloadCurrentComponent(){
    window.location.reload(); //-- reload screen
  }

  successMsgBlink(msg: any){
    this.updateMsgVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.updateMsgVisible = false;
    }, 5000);
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  checkboxClicked(event: any, rowData: any){
    this.validateRequiredField();
  }

  editFleetUtilisationPreferences(){
    this.editFleetUtilisationFlag = true;
  }

  updateEditFleetUtilFlag(flag: any){
    this.editFleetUtilisationFlag = flag;
  }

}
