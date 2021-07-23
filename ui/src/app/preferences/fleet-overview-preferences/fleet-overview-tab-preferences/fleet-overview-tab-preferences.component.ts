import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-fleet-overview-tab-preferences',
  templateUrl: './fleet-overview-tab-preferences.component.html',
  styleUrls: ['./fleet-overview-tab-preferences.component.less']
})
export class FleetOverviewTabPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setFleetOverviewFlag = new EventEmitter<any>();
  reportId: any;
  initData: any = [];
  timerPrefData: any = [];
  vehInfoPrefData: any = [];
  selectionForSetTimerColumns = new SelectionModel(true, []);
  selectionForVehInfoColumns = new SelectionModel(true, []);
  fleetOverviewForm: FormGroup;

  constructor(private reportService: ReportService, private router: Router, private _formBuilder: FormBuilder) { }

  ngOnInit() { 
    let repoId: any = this.reportListData.filter(i => i.name == 'Fleet Overview');
    this.fleetOverviewForm = this._formBuilder.group({
      refreshTime: []
    });
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 17; //- hard coded for Fleet Overview - Logbook -13
    }
    this.translationUpdate();
    this.loadTripReportPreferences();
   }

   translationUpdate(){
    this.translationData = {
      rp_fo_fleetoverview: 'Fleet Overview',
      rp_fo_fleetoverview_settimer: 'Set Timer',
      rp_fo_fleetoverview_settimer_pagerefresh: 'Page Refresh Time',
      rp_fo_fleetoverview_generalvehicleinformation: 'General Vehicle Information',
      rp_fo_fleetoverview_generalvehicleinformation_currentmileage: 'Current Mileage',
      rp_fo_fleetoverview_generalvehicleinformation_nextservicein: 'Next Service In',
      rp_fo_fleetoverview_generalvehicleinformation_healthstatus: 'Health Status'
    }
   }

   loadTripReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.getTranslatedColumnName(this.initData);
    }, (error)=>{
      this.initData = [];
      this.resetColumnData();
    });
   }

   getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any;
            if(item.key.includes('rp_fo_fleetoverview_settimer_')){
              _data = item;
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 23);   
              }
              this.timerPrefData.push(_data);
            }else if(item.key.includes('rp_fo_fleetoverview_generalvehicleinformation_')){
              _data = item;
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 40);   
              }
              this.vehInfoPrefData.push(_data);
            }
          });
        }
      });
      if (this.timerPrefData.length > 0 && this.vehInfoPrefData.length > 0) {
        this.setDefaultFormValues();
        this.setColumnCheckbox();
      }
    }
  }

  setDefaultFormValues() {
    this.fleetOverviewForm.get('refreshTime').setValue(this.timerPrefData[0].thresholdValue != '' ? this.timerPrefData[0].thresholdValue : 0);
  }

  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  setColumnCheckbox(){
    this.selectionForSetTimerColumns.clear();
    this.selectionForVehInfoColumns.clear();
    this.timerPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForSetTimerColumns.select(element);
      }
    });
    this.vehInfoPrefData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForVehInfoColumns.select(element);
      }
    });
  }

   resetColumnData(){
     this.timerPrefData = [];
     this.vehInfoPrefData = [];
   }

   onCancel(){
    this.setFleetOverviewFlag.emit({flag: false, msg: ''});
    this.setColumnCheckbox();
    this.setDefaultFormValues();
  }

  onReset(){
    this.setColumnCheckbox();
    this.setDefaultFormValues();
  }

  onConfirm(){ }

  keyPressNumbers(event: any){
    var charCode = (event.which) ? event.which : event.keyCode;
    // Only Numbers 0-9
    if ((charCode < 48 || charCode > 57)) {
      event.preventDefault();
      return false;
    } else {
      return true;
    }
  }

  isAllSelectedForTimerColumns(){
    const numSelected = this.selectionForSetTimerColumns.selected.length;
    const numRows = this.timerPrefData.length;
    return numSelected === numRows;
  }

  masterToggleForTimerColumns(){
    if(this.isAllSelectedForTimerColumns()){
      this.selectionForSetTimerColumns.clear();
    }else{
      this.timerPrefData.forEach(row => { this.selectionForSetTimerColumns.select(row) });
    }
  }

  isAllSelectedForVehInfoColumns(){
    const numSelected = this.selectionForVehInfoColumns.selected.length;
    const numRows = this.vehInfoPrefData.length;
    return numSelected === numRows;
  }

  masterToggleForVehInfoColumns(){
    if(this.isAllSelectedForVehInfoColumns()){
      this.selectionForVehInfoColumns.clear();
    }else{
      this.vehInfoPrefData.forEach(row => { this.selectionForVehInfoColumns.select(row) });
    }
  }

  checkboxLabelForColumns(rowData?: any){
  }

  checkboxClicked(event: any, rowData: any){

  }

}
