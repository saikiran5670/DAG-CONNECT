import { SelectionModel } from '@angular/cdk/collections';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
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
  showReport: boolean = false;
  editFlag: boolean = false;
  tripReportId = 1; //- Trip report

  constructor(  private reportService: ReportService, ) { }

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
    this.translationData.da_report_details_registrationnumber = 'Registration Number';
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
    this.reportService.getUserPreferenceReport(this.tripReportId, this.accountId, this.accountOrganizationId).subscribe((data : any) => {
      this.initData = data["userPreferences"];
      this.initData = this.getTranslatedColumnName(this.initData);
      this.setColumnCheckbox();
      this.hideloader();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedTableData(this.initData);
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
    this.editFlag =  true;
  }

  isAllSelectedForColumns(){
    const numSelected = this.selectionForColumns.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForColumns(){
    if(this.isAllSelectedForColumns()){
      this.selectionForColumns.clear();
    }else{
      this.dataSource.data.forEach(row => { this.selectionForColumns.select(row) });
    }
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
    this.editFlag = false;
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
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
      this.editFlag = false;
      window.location.reload(); //-- reload screen
    }, (error) => {
      console.log(error);
    });
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

}
