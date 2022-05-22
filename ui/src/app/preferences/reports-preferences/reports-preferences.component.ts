import { Component, Input, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core'; 
import { ReportService } from 'src/app/services/report.service';
import { FleetFuelPreferencesComponent } from './fleet-fuel-preferences/fleet-fuel-preferences.component';
import { Router } from '@angular/router';

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
  requestSent: boolean = false;
  tabIndex: any = 0;
  prefDetail: any = {};
  reportDetail: any = [];
  tripReport: boolean = false;
  fleetUtilisationReport: boolean = false;
  driveTimeReport: boolean = false;
  ecoScoreReport: boolean = false;
  fuelBenchmarkingReport: boolean = false;
  fleetFuelReport: boolean = false;
  fuelDeviationReport: boolean = false;

  constructor(private reportService: ReportService, private router: Router, private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail')); 
    this.showHideReports();
  }

  ngAfterViewInit(){
    this.cdr.detectChanges();
  }

  hideloader() {
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
      this.editFleetUtilisationFlag = false; 
    }
  }

  updateEditTripReportFlag(retObj: any){
    if(retObj){
      this.editTripFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editTripFlag = false; 
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
    if(!this.requestSent) {
      this.requestSent = true;
      let vehicleObj = this.fleetFuelPerferencesVehicle.onConfirm();
      let driverObj = this.fleetFuelPerferencesDriver.onConfirm();
      let objData: any = {
        reportId: this.reportDetail.filter(i => i.name == 'Fleet Fuel Report')[0].id,
        attributes: [...vehicleObj, ...driverObj]
      };
      this.reportService.updateReportUserPreference(objData).subscribe((res: any) => {
        this.showLoadingIndicator = false;
        let _reloadFlag: any = false;
        if ((this.router.url).includes("fleetfuelreport")) {
          _reloadFlag = true
          this.fleetFuelPerferencesVehicle.loadFleetFuelPreferences(_reloadFlag); 
        }
        this.updateFleetFuelPerferencesFlag({ flag: false, msg: this.getSuccessMsg() });
        this.requestSent = false;
      }, (error) => {
        this.showLoadingIndicator = false;
      });
    }
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
    this.tabIndex = event.index;
  }

  validateRequiredField() {
    if(this.fleetFuelPerferencesVehicle) {
      let VehicleDetailsV = this.fleetFuelPerferencesVehicle.validateRequiredField('VehicleDetails');
      let SingleVehicleDetailsV = this.fleetFuelPerferencesVehicle.validateRequiredField('SingleVehicleDetails');
      let VehicleDetailsD = this.fleetFuelPerferencesDriver.validateRequiredField('VehicleDetails');
      let SingleVehicleDetailsD = this.fleetFuelPerferencesDriver.validateRequiredField('SingleVehicleDetails');
      if(VehicleDetailsV || SingleVehicleDetailsV || VehicleDetailsD || SingleVehicleDetailsD) {
        return true;
      }
      return false;
    }
    return false;
  }

  showHideReports(){
    let reportMenu = JSON.parse(localStorage.getItem('accountFeatures'));
    if(reportMenu && reportMenu.menus){
      reportMenu.menus.forEach(menu => {
        for(var key in menu){
          if(menu && menu.key && menu.key == 'lblReport'){
            menu.subMenus.forEach(subMenu => {
              for(var key in subMenu){
                if(subMenu && subMenu.key){
                  if(subMenu.key == 'lblFuelDeviationReport')
                    this.fuelDeviationReport = true;
                  else if(subMenu.key == 'lblTripReport')
                    this.tripReport = true;
                  else if(subMenu.key == 'lblFleetFuelReport')
                    this.fleetFuelReport = true;
                  else if(subMenu.key == 'lblFleetUtilisation')
                    this.fleetUtilisationReport = true;
                  else if(subMenu.key == 'lblFuelBenchmarking')
                    this.fuelBenchmarkingReport = true;
                  else if(subMenu.key == 'lblDriveTimeManagement')
                    this.driveTimeReport = true;
                  else if(subMenu.key == 'lblECOScoreReport')
                    this.ecoScoreReport = true;
                }
              }
            });
          }
        }
      });
    }
  }
}