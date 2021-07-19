import { Component, Input, OnInit } from '@angular/core';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-reports-preferences',
  templateUrl: './reports-preferences.component.html',
  styleUrls: ['./reports-preferences.component.less']
})

export class ReportsPreferencesComponent implements OnInit {
  @Input() translationData: any = {};
  displayMessage: any = '';
  updateMsgVisible: boolean = false;
  showLoadingIndicator: any = false;
  showTripReport: boolean = false;
  showFleetUtilisationReport: boolean = false;
  editTripFlag: boolean = false;
  editFleetUtilisationFlag: boolean = false;
  reportListData: any = [];
  showDriverTimePerferences: boolean = false;
  editDriverTimePerferencesFlag:boolean = false;
  showEcoScorePerferences: boolean = false;
  editEcoScorePerferencesFlag:boolean = false;
  showFuelBenchmarkPerferences: boolean = false;
  editFuelBenchmarkPerferencesFlag:boolean = false;
  
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
      this.editFuelBenchmarkPerferencesFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editFuelBenchmarkPerferencesFlag = false; // hard coded
    }
  }

}
