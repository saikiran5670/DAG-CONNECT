import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ReportService } from 'src/app/services/report.service';
import { FleetFuelPreferencesComponent } from './fleet-fuel-preferences/fleet-fuel-preferences.component';

@Component({
  selector: 'app-reports-preferences',
  templateUrl: './reports-preferences.component.html',
  styleUrls: ['./reports-preferences.component.less']
})

export class ReportsPreferencesComponent implements OnInit {
  @Input() translationData: any = {};
  @ViewChild('fleetFuelPerferencesVehicle') fleetFuelPerferencesVehicle: FleetFuelPreferencesComponent;
  @ViewChild('fleetFuelPerferencesDriver') fleetFuelPerferencesDriver: FleetFuelPreferencesComponent;
  displayMessage: any = '';
  updateMsgVisible: boolean = false;
  showLoadingIndicator: any = false;
  showTripReport: boolean = false;
  showFleetUtilisationReport: boolean = false;
  editTripFlag: boolean = false;
  editFleetUtilisationFlag: boolean = false;
  reportListData: any = [];
  showDriverTimePerferences: boolean = false;
  editDriverTimePerferencesFlag: boolean = false;
  showEcoScorePerferences: boolean = false;
  editEcoScorePerferencesFlag: boolean = false;
  showFuelBenchmarkPerferences: boolean = false;
  editFuelBenchmarkPerferencesFlag: boolean = false;
  showFuelDeviationPerferences: boolean = false;
  editFuelDeviationPerferencesFlag: boolean = false;
  showFleetFuelPerferences: boolean = false;
  editFleetFuelPerferencesFlag: boolean = false;

  constructor( private reportService: ReportService ) { }

  ngOnInit() {
    this.loadReportData();
  }

  loadReportData(){
    this.showLoadingIndicator = true;
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      this.hideloader();
      this.reportListData = reportList.reportDetails;
    }, (error)=>{
      console.log('Report not found...', error);
      this.hideloader();
      this.reportListData = [];
    });
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
    this.showTripReport = false;
  }

  successMsgBlink(msg: any){
    this.updateMsgVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.updateMsgVisible = false;
    }, 5000);
  }

  editFleetUtilisationPreferences(){
    this.editFleetUtilisationFlag = true;
    this.showFleetUtilisationReport = false;
  }

  updateEditFleetUtilFlag(retObj: any){
    if(retObj){
      this.editFleetUtilisationFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editFleetUtilisationFlag = false; // hard coded
    }
  }

  updateEditTripReportFlag(retObj: any){
    if(retObj){
      this.editTripFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editTripFlag = false; // hard coded
    }
  }

  editFleetFuelPerferences() {
    this.editFleetFuelPerferencesFlag = true;
    this.showFleetFuelPerferences = false;
  }

  updateFleetFuelPerferencesFlag(retObj: any) {
    if (retObj) {
      if (retObj.msg && retObj.msg != '') {
        this.successMsgBlink(retObj.msg);
      }
    }
    this.editFleetFuelPerferencesFlag = false;
    this.showFleetFuelPerferences = false;
  }

  onCancel(){
    this.updateFleetFuelPerferencesFlag({flag: false, msg: ''});
    this.onReset();
  }

  onReset(){
    this.fleetFuelPerferencesVehicle.setColumnCheckbox();
    this.fleetFuelPerferencesDriver.setColumnCheckbox();
  }

  onConfirm() {
    let vehicleObj = this.fleetFuelPerferencesVehicle.onConfirm();
    let driverObj = this.fleetFuelPerferencesDriver.onConfirm();
    let objData: any = {
      reportId: this.reportListData.filter(i => i.name == 'Fleet Fuel Report')[0].id,
      attributes: [...vehicleObj, ...driverObj]
    };
    this.reportService.updateReportUserPreference(objData).subscribe((res: any) => {
      this.updateFleetFuelPerferencesFlag({ flag: false, msg: this.getSuccessMsg() });
      setTimeout(() => {
        window.location.reload();
      }, 1000);
    });
  }

  getSuccessMsg() {
    if (this.translationData.lblDetailssavesuccessfully)
      return this.translationData.lblDetailssavesuccessfully;
    else
      return ("Details save successfully");
  }

  editDriverTimePerferences(){
    this.editDriverTimePerferencesFlag = true;
    this.showDriverTimePerferences = false;
  }

  editEcoScorePerferences(){
    this.editEcoScorePerferencesFlag = true;
    this.showEcoScorePerferences = false;
  }

  editFuelBenchmarkPerferences(){
    this.editFuelBenchmarkPerferencesFlag = true;
    this.showFuelBenchmarkPerferences = false;
  }

  editFuelDeviationPerferences(){
    this.editFuelDeviationPerferencesFlag = true;
    this.showFuelDeviationPerferences = false;
  }

  updateEditDriverTimeFlag(retObj: any){
    if(retObj){
      this.editDriverTimePerferencesFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editDriverTimePerferencesFlag = false; // hard coded
    }
  }

  updateEcoScoreReportFlag(retObj: any){
    if(retObj){
      this.editEcoScorePerferencesFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editEcoScorePerferencesFlag = false; // hard coded
    }
  }

  updateFuelBenchmarkReportFlag(retObj: any){
    if(retObj){
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }
    this.editFuelBenchmarkPerferencesFlag = false; // hard coded
    this.showFuelBenchmarkPerferences = false;
  }

  updateFuelDeviationReportFlag(retObj: any){
    if(retObj){
      this.editFuelDeviationPerferencesFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editFuelDeviationPerferencesFlag = false; // hard coded
    }
  }
  
  onTabChanged(event) {
    // event.stopPropogation();
    // event.preventDefault();
  }

}
