import { Component, Input, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-preferences',
  templateUrl: './fleet-overview-preferences.component.html',
  styleUrls: ['./fleet-overview-preferences.component.less']
})

export class FleetOverviewPreferencesComponent implements OnInit {
  @Input() translationData: any = {};
  updateMsgVisible: boolean = false;
  displayMessage: any = 'pref added';
  showLoadingIndicator: any = false; 
  editFleetOverviewFlag: boolean = false;
  showFleetOverview: boolean = false;
  editLogbookFlag: boolean = false;
  showLogbook: boolean = false;
  prefDetail: any = {};
  reportDetail: any = [];

  constructor() { }

  ngOnInit() {
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail'));
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  onClose() {
    this.updateMsgVisible = false;
  }

  editFleetOverviewPreferences(){
    this.editFleetOverviewFlag = true;
    this.showFleetOverview = false;
  }

  updateEditFleetOverviewFlag(retObj: any){
    if(retObj){
      this.editFleetOverviewFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editFleetOverviewFlag = false; // hard coded
    }
  }

  editLogbookPreferences(){
    this.editLogbookFlag = true;
    this.showLogbook = false;
  }

  updateEditLogbookFlag(retObj: any){
    if(retObj){
      this.editLogbookFlag = retObj.flag;
      if(retObj.msg && retObj.msg != ''){
        this.successMsgBlink(retObj.msg);
      }
    }else{
      this.editLogbookFlag = false; // hard coded
    }
  }

  successMsgBlink(msg: any){
    this.updateMsgVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.updateMsgVisible = false;
    }, 5000);
  }

}