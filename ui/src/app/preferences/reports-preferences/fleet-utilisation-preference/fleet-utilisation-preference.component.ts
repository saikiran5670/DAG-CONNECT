import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';

@Component({
  selector: 'app-fleet-utilisation-preference',
  templateUrl: './fleet-utilisation-preference.component.html',
  styleUrls: ['./fleet-utilisation-preference.component.less']
})

export class FleetUtilisationPreferenceComponent implements OnInit {
  @Input() translationData: any;
  @Input() reportListData: any;
  @Input() editFlag: any;
  @Output() setFleetUtilFlag = new EventEmitter<boolean>();
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  prefTimeFormat: any = 24; //-- coming from pref setting
  reportId: any;
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  initData: any = [];
  summaryColumnData: any = [];
  chartsColumnData: any = [];
  calenderColumnData: any = [];
  detailColumnData: any = [];
  selectionForSummaryColumns = new SelectionModel(true, []);
  selectionForDetailsColumns = new SelectionModel(true, []);
  selectionForChartsColumns = new SelectionModel(true, []);
  selectionForCalenderColumns = new SelectionModel(true, []);
  timeDisplay: any = '00:00';
  lineBarDD: any = [{
    status: 'A',
    id: 1,
    name: 'Line Chart'
  },
  {
    status: 'I',
    id: 2,
    name: 'Bar Chart'
  }];
  
  donutPieDD: any = [{
    status: 'A',
    id: 1,
    name: 'Donut Chart'
  },
  {
    status: 'I',
    id: 2,
    name: 'Pie Chart'
  }];

  upperLowerDD: any = [{
    status: 'A',
    id: 1,
    name: 'Upper'
  },
  {
    status: 'I',
    id: 2,
    name: 'Lower'
  }];
  
  constructor(private reportService: ReportService) { }

  ngOnInit() { 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'Fleet Utilisation Report');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 5; //- hard coded for fleet utilisation report
    }
    this.loadFleetUtilisationPreferences();
  }

  loadFleetUtilisationPreferences(){
    this.reportService.getUserPreferenceReport(this.reportId, this.accountId, this.accountOrganizationId).subscribe((prefData: any) => {
      this.initData = prefData['userPreferences'];
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.initData = [];
    });
  }

  setColumnCheckbox(){
    this.selectionForSummaryColumns.clear();
    this.selectionForDetailsColumns.clear();
    this.selectionForChartsColumns.clear();
    
    this.summaryColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForSummaryColumns.select(element);
      }
    });
    
    this.detailColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForDetailsColumns.select(element);
      }
    });

    this.chartsColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForChartsColumns.select(element);
      }
    });
  }

  preparePrefData(prefData: any){
    prefData.forEach(element => {
      let _data: any;
      if(element.key.includes('da_report_general')){
         _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 15);   
        }
        this.summaryColumnData.push(_data);
      }else if(element.key.includes('da_report_charts')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 14);   
        }
        this.chartsColumnData.push(_data);
      }else if(element.key.includes('da_report_calendarview')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 20);   
        }
        this.calenderColumnData.push(_data);
      }else if(element.key.includes('da_report_details')){
        _data = element;
        if(this.translationData[element.key]){
          _data.translatedName = this.translationData[element.key];  
        }else{
          _data.translatedName = this.getName(element.name, 15);   
        }
        this.detailColumnData.push(_data);
      }
    });
    this.setColumnCheckbox();
  }

  getName(name: any, index: any) {
    let updatedName = name.slice(index);
    return updatedName;
  }

  masterToggleForSummaryColumns(){
    if(this.isAllSelectedForSummaryColumns()){
      this.selectionForSummaryColumns.clear();
    }else{
      this.summaryColumnData.forEach(row => { this.selectionForSummaryColumns.select(row) });
    }
  }

  isAllSelectedForSummaryColumns(){
    const numSelected = this.selectionForSummaryColumns.selected.length;
    const numRows = this.summaryColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForSummaryColumns(row?: any){

  }

  masterToggleForDetailsColumns(){
    if(this.isAllSelectedForDetailsColumns()){
      this.selectionForDetailsColumns.clear();
    }else{
      this.detailColumnData.forEach(row => { this.selectionForDetailsColumns.select(row) });
    }
  }

  isAllSelectedForDetailsColumns(){
    const numSelected = this.selectionForDetailsColumns.selected.length;
    const numRows = this.detailColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForDetailsColumns(row?: any){

  }

  masterToggleForChartsColumns(){
    if(this.isAllSelectedForChartsColumns()){
      this.selectionForChartsColumns.clear();
    }else{
      this.chartsColumnData.forEach(row => { this.selectionForChartsColumns.select(row) });
    }
  }

  isAllSelectedForChartsColumns(){
    const numSelected = this.selectionForChartsColumns.selected.length;
    const numRows = this.chartsColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForChartsColumns(row?: any){

  }

  onCancel(){
    this.setFleetUtilFlag.emit(false);
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm(){
    this.setFleetUtilFlag.emit(false);
    this.setColumnCheckbox();
  }

  onlineBarDDChange(event: any){

  }

  onDonutPieDDChange(event: any){

  }

  onUpperLowerDDChange(event: any){

  }

  onCalenderDetailDDChange(event: any){

  }

  timeChanged(selectedTime: any) {
    this.timeDisplay = selectedTime;
  }

}
