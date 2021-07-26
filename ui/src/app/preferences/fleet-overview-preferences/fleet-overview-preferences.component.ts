import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ReportService } from 'src/app/services/report.service';
import { TranslationService } from 'src/app/services/translation.service';

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
  generalPreferences: any;
  reportListData: any = [];
  editFleetOverviewFlag: boolean = false;
  showFleetOverview: boolean = false;
  editLogbookFlag: boolean = false;
  showLogbook: boolean = false;

  constructor(private reportService: ReportService, private translationService: TranslationService) { }

  ngOnInit() {
    this.loadReportData();
  }
  
  loadReportData(){
    this.showLoadingIndicator = true;
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      this.hideloader();
      this.reportListData = reportList.reportDetails;
      this.getPreferences();
    }, (error)=>{
      console.log('Report not found...', error);
      this.hideloader();
      this.reportListData = [];
    });
  }

  getPreferences() {
    let languageCode = JSON.parse(localStorage.getItem('language')).code;
    this.translationService.getPreferences(languageCode).subscribe((res: any) => {
      this.generalPreferences = res
    }, (error)=>{
      console.log('pref not present...');
    });
  }

  hideloader() {
    // Setting display of spinner
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
