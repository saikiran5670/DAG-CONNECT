import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-eco-score-report-preferences',
  templateUrl: './eco-score-report-preferences.component.html',
  styleUrls: ['./eco-score-report-preferences.component.less']
})
export class EcoScoreReportPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setEcoScoreFlag = new EventEmitter<any>();
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  reportId: any;
  initData: any = [];
  generalColumnData: any = [];
  generalGraphColumnData: any = [];
  driverPerformanceColumnData: any = [];
  driverPerformanceGraphColumnData: any = [];
  selectionForGeneralColumns = new SelectionModel(true, []);
  selectionForGeneralGraphColumns = new SelectionModel(true, []);
  selectionForDriverPerformanceColumns = new SelectionModel(true, []);
  selectionForDriverPerformanceGraphColumns = new SelectionModel(true, []);

  constructor(private reportService: ReportService, private router: Router) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'EcoScore Report');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 10; //- hard coded for Eco-Score Report
    }
    this.translationUpdate();
    this.loadEcoScoreReportPreferences();
  }

  translationUpdate(){
    this.translationData = {
      
    }
  }

  loadEcoScoreReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.initData = [];
      this.resetColumnData();
    });
  }

  resetColumnData(){
    this.generalColumnData = [];
    this.generalGraphColumnData = [];
    this.driverPerformanceColumnData = [];
    this.driverPerformanceGraphColumnData = [];
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.name.includes('EcoScore.General.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 17);   
              }
              this.generalColumnData.push(_data);
            }else if(item.name.includes('EcoScore.GeneralGraph.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 22);   
              }
              this.generalGraphColumnData.push(_data);
            }else if(item.name.includes('EcoScore.DriverPerformance.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 27);   
              }
              this.driverPerformanceColumnData.push(_data);
            }else if(item.name.includes('EcoScore.DriverPerformanceGraph.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 32);   
              }
              this.driverPerformanceGraphColumnData.push(_data);
            }
          });
        }
      });
    }
    this.setColumnCheckbox();
  }

  setColumnCheckbox(){
    this.selectionForGeneralColumns.clear();
    this.selectionForGeneralGraphColumns.clear();
    this.selectionForDriverPerformanceColumns.clear();
    this.selectionForDriverPerformanceGraphColumns.clear();
    
    this.generalColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForGeneralColumns.select(element);
      }
    });
    
    // this.detailColumnData.forEach(element => {
    //   if(element.state == 'A'){
    //     this.selectionForDetailsColumns.select(element);
    //   }
    // });

    // this.chartsColumnData.forEach(element => {
    //   if(element.state == 'A'){
    //     this.selectionForChartsColumns.select(element);
    //   }
    // });
    // if(this.summaryColumnData.length > 0 && this.chartsColumnData.length > 0 && this.calenderColumnData.length > 0 && this.detailColumnData.length > 0){
    //   this.setDefaultFormValues();
    // }
  }
  
  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  masterToggleForGeneralColumns(){
    if(this.isAllSelectedForGeneralColumns()){
      this.selectionForGeneralColumns.clear();
    }else{
      this.generalColumnData.forEach(row => { this.selectionForGeneralColumns.select(row) });
    }
  }

  isAllSelectedForGeneralColumns(){
    const numSelected = this.selectionForGeneralColumns.selected.length;
    const numRows = this.generalColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeneralColumns(row?: any){

  }

  checkboxClicked(event: any, rowData: any){
    
  }

  onCancel(){

  }

  onReset(){

  }

  onConfirm(){
    
  }

}
