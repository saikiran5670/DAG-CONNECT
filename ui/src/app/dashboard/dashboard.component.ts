import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ReportService } from '../services/report.service';
import { TranslationService } from '../services/translation.service';
import { OrganizationService } from '../services/organization.service';
import { MessageService } from '../services/message.service';
import { DashboardService } from '../services/dashboard.service';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less'],
})
export class DashboardComponent implements OnInit {
  localStLanguage: any;
  accountOrganizationId: any = 0;
  globalSearchFilterData: any = JSON.parse(localStorage.getItem("globalSearchFilterData"));
  accountId: any;
  accountPrefObj : any;
  showLoadingIndicator : boolean = false;
  finalVinList : any =[];
  preference : any;
  noDataFound: boolean = false;
  dashboardPrefData: any;
  translationData: any = {};
  prefDetail: any = {};
  reportDetail: any = [];

  constructor(public httpClient: HttpClient,private translationService: TranslationService,private reportService : ReportService, private organizationService: OrganizationService,
    private messageService : MessageService,private dashboardService : DashboardService) {
      this.sendMessage();
    }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    let _langCode = this.localStLanguage ? this.localStLanguage.code  :  "EN-GB";
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail')); 

    if(localStorage.getItem('contextOrgId'))
      this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    else 
      this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;

    let translationObj = {
      id: 0,
      code: _langCode,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 1 //-- for dashboard
    }
   
    this.globalSearchFilterData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.showLoadingIndicator = true;
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      if(this.prefDetail){
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ 
          this.preference = this.accountPrefObj.accountPreference;
          this.loadReportData();
        }else{ 
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.preference = orgPref;
            this.loadReportData();
          }, (error) => { 
            this.preference = {};
            this.loadReportData();
          });
        }
      }
    });
  }

  loadReportData() {
    if(this.reportDetail){
      let repoId: any= this.reportDetail.filter(i => i.name == 'Dashboard');
      let reportId;
      if (repoId.length > 0) {
        reportId = repoId[0].id;
      } 
      else {
        reportId = 18;
      }
      this.showLoadingIndicator = true;
      this.dashboardService.getDashboardPreferences(reportId).subscribe((_prefData: any) => {
        this.dashboardPrefData = _prefData['userPreferences'];
        this.getVinsForDashboard();
      }, (error) => {
        this.dashboardPrefData = [];
        this.getVinsForDashboard();
      });
    }
  }
  
  sendMessage(): void {
    this.messageService.sendMessage('refreshTimer');
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  proceedStep(preference: any){
    this.preference = preference;
    this.loadReportData();
  }

  getVinsForDashboard(){
    this.dashboardService.getVinsForDashboard(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.noDataFound = false;
      this.processVins(tripData);
    }, (error) => {
      this.hideloader();
      this.noDataFound = true;
    });
  }

  processVins(tripData){
    let _vinList = tripData['vinTripList'].map(x=>x.vin);
    if(_vinList.length > 0){
      this.finalVinList = _vinList.filter((value, index, self) => self.indexOf(value) === index);
    }
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }
}