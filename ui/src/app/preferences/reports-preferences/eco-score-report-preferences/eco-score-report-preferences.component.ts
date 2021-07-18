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
      rp_ecoscore: 'Eco Score',
      rp_general: 'General',
      rp_distance: 'Distance',
      rp_averagedistanceperday: 'Average Distance Per Day',
      rp_numberofvehicles: 'Number Of Vehicles',
      rp_averagegrossweight: 'Average Gross Weight',
      rp_numberoftrips: 'Number Of Trips',
      rp_generalgraph: 'General Graph',
      rp_piechart: 'Pie Chart',
      rp_bargraph: 'Bar Graph',
      rp_driverperformancegraph: 'Driver Performance Graph',
      rp_driverperformance: 'Driver Performance',
      rp_anticipationscore: 'Anticipation Score (%)',
      rp_fuelconsumption: 'Fuel Consumption',
      rp_cruisecontrolusage: 'Cruise Control Usage (%)',
      rp_CruiseControlUsage30: 'Cruise Control Usage 30-50 km/h (%)',
      rp_cruisecontroldistance50: 'Cruise Control Usage 50-75 km/h (%)',
      rp_cruisecontroldistance75: 'Cruise Control Usage >75 km/h (%)',
      rp_heavythrottling: 'Heavy Throttling (%)',
      rp_heavythrottleduration: 'Heavy Throtting Duration (hh:mm:ss)',
      rp_ptousage: 'PTO Usage (%)',
      rp_ptoduration: 'PTO Duration (hh:mm:ss)',
      rp_averagespeed: 'Average Speed (mph)',
      rp_idleduration: 'Idle duration (hh:mm:ss)',
      rp_averagedrivingspeed: 'Average Driving Speed (mph)',
      rp_idling: 'Idling (%)',
      rp_brakingscore: 'Braking Score',
      rp_braking: 'Braking (%)',
      rp_harshbraking: 'Harsh Braking (%)',
      rp_harshbrakeduration: 'Harsh Brake Duration (hh:mm:ss)',
      rp_brakeduration: 'Brake Duration (hh:mm:ss)'
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

              let index: any;
              switch(item.name){
                case 'EcoScore.DriverPerformance.EcoScore':{
                  index = 0;
                  break;
                }
                case 'EcoScore.DriverPerformance.FuelConsumption':{
                  index = 1;
                  break;
                }
                case 'EcoScore.DriverPerformance.BrakingScore':{
                  index = 2;
                  break;
                }
                case 'EcoScore.DriverPerformance.AnticipationScore':{
                  index = 3;
                  break;
                }
              }
              this.driverPerformanceColumnData[index] = _data;
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

    this.generalGraphColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForGeneralGraphColumns.select(element);
      }
    });

    this.driverPerformanceGraphColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForDriverPerformanceGraphColumns.select(element);
      }
    });
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

  masterToggleForGeneralGraphColumns(){
    if(this.isAllSelectedForGeneralGraphColumns()){
      this.selectionForGeneralGraphColumns.clear();
    }else{
      this.generalGraphColumnData.forEach(row => { this.selectionForGeneralGraphColumns.select(row) });
    }
  }

  isAllSelectedForGeneralGraphColumns(){
    const numSelected = this.selectionForGeneralGraphColumns.selected.length;
    const numRows = this.generalGraphColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeneralGraphColumns(row?: any){

  }

  masterToggleForDriverPerformanceGraphColumns(){
    if(this.isAllSelectedForDriverPerformanceGraphColumns()){
      this.selectionForDriverPerformanceGraphColumns.clear();
    }else{
      this.driverPerformanceGraphColumnData.forEach(row => { this.selectionForDriverPerformanceGraphColumns.select(row) });
    }
  }

  isAllSelectedForDriverPerformanceGraphColumns(){
    const numSelected = this.selectionForDriverPerformanceGraphColumns.selected.length;
    const numRows = this.driverPerformanceGraphColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForDriverPerformanceGraphColumns(row?: any){

  }

  checkboxClicked(event: any, rowData: any){
    
  }

  onCancel(){
    this.setEcoScoreFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
  }

  onReset(){
    this.setColumnCheckbox();
  }

  onConfirm(){
    
  }

  getSuccessMsg(){
    if(this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

}
