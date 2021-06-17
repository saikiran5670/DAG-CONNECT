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

}
