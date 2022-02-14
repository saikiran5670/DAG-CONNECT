import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { MultiDataSet, Label, Color } from 'ng2-charts';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
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
  prefData : any;
  preference : any;
  noDataFound: boolean = false;
  dashboardPrefData: any;
  //--------- Pie chart ------------------//
  // doughnutChartData: MultiDataSet = [[55, 25, 20]];
  // doughnutChartLabels: Label[] = ['BMW', 'Ford', 'Tesla'];
  // doughnutChartType: ChartType = 'doughnut';
  // barChartColors: Color[] = [
  //   {
  //     backgroundColor: ['#eb8171', '#f7d982', '#54a9d8'],
  //   },
  // ];
  // public doughnut_barOptions: ChartOptions = {
  //   responsive: true,
  //   legend: {
  //     position: 'bottom',
  //     labels: {
  //       //fontSize: 10,
  //       usePointStyle: true,
  //     },
  //   },
  //   cutoutPercentage: 50,
  // };
  // public barChartLegend = true;

  // //--------------- bar graph ----------//
  // public stack_barChartData: ChartDataSets[] = [
  //   {
  //     data: [20, 10, 20, 10, 20],
  //     label: 'BMW',
  //     stack: 'a',
  //     backgroundColor: '#eb8171',
  //     hoverBackgroundColor: '#eb8171',
  //     barThickness: 30,
  //   },
  //   {
  //     data: [10, 10, 10, 10, 10],
  //     label: 'Ford',
  //     stack: 'a',
  //     backgroundColor: '#f7d982',
  //     hoverBackgroundColor: '#f7d982',
  //     barThickness: 30,
  //   },
  //   {
  //     data: [10, 20, 10, 20, 10],
  //     label: 'Tesla',
  //     stack: 'a',
  //     backgroundColor: '#54a9d8',
  //     hoverBackgroundColor: '#54a9d8',
  //     barThickness: 30,
  //   },
  // ];
  // public stack_barChartLabels: Label[] = [
  //   '2016',
  //   '2017',
  //   '2018',
  //   '2019',
  //   '2020',
  // ];
  // public stack_barChartOptions: ChartOptions = {
  //   responsive: true,
  //   legend: {
  //     position: 'bottom',
  //   },
  //   scales: {
  //     xAxes: [
  //       {
  //         stacked: true,
  //         time: {
  //           unit: 'month',
  //         },
  //         // gridLines: {
  //         //   display: false,
  //         // }
  //       },
  //     ],
  //     yAxes: [
  //       {
  //         stacked: true,
  //         ticks: {
  //           beginAtZero: true,
  //           //    max: 0
  //         },
  //       },
  //     ],
  //   },
  // };
  // public stack_barChartPlugins = [];
  // public stack_barChartLegend = true;
  // public stack_barChartType: ChartType = 'bar';
  //----- bar graph 2 -------------------------//
  // public stack_barChartData_overdue: ChartDataSets[] = [
  //   {
  //     data: [15],
  //     label: 'BMW',
  //     stack: 'a',
  //     backgroundColor: '#eb8171',
  //     hoverBackgroundColor: '#eb8171',
  //     barThickness: 30,
  //   },
  // ];
  // public stack_barChartLabels_overdue: Label[] = ['2020'];
  translationData: any = {};

  constructor(public httpClient: HttpClient,private translationService: TranslationService,private reportService : ReportService, private organizationService: OrganizationService,
    private messageService : MessageService,private dashboardService : DashboardService) {
      this.sendMessage();
     
    }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    let _langCode = this.localStLanguage ? this.localStLanguage.code  :  "EN-GB";
    
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
   // this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.showLoadingIndicator = true;
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.translationService.getPreferences(_langCode).subscribe((prefData: any) => {
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        }else{ // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }
      });
    });
   
  }

  loadReportData() {
    let reportListData;
    this.showLoadingIndicator = true;
    this.reportService.getReportDetails().subscribe((reportList: any) => {
      reportListData = reportList.reportDetails;
      let repoId: any= reportListData.filter(i => i.name == 'Dashboard');
      let reportId;
      if (repoId.length > 0) {
        reportId= repoId[0].id;
      } 
      else {
        reportId = 18;
      }
      this.dashboardService.getDashboardPreferences(reportId).subscribe((prefData: any) => {
        this.dashboardPrefData = prefData['userPreferences'];
        if(this.dashboardPrefData.subReportUserPreferences && this.dashboardPrefData.subReportUserPreferences.length > 0) {
          let userPref = this.dashboardPrefData.subReportUserPreferences.find(x => x.key == "rp_db_dashboard_vehicleutilization");
         if(userPref && userPref.subReportUserPreferences.length > 0) {
           let timebasedutilizationrate = userPref.subReportUserPreferences.find(x => x.key == "rp_db_dashboard_vehicleutilization_timebasedutilizationrate").thresholdValue;
           let distancebasedutilizationrate = userPref.subReportUserPreferences.find(x => x.key == "rp_db_dashboard_vehicleutilization_distancebasedutilizationrate").thresholdValue;
           localStorage.setItem("liveFleetMileageThreshold", timebasedutilizationrate);
           localStorage.setItem("liveFleetUtilizationThreshold", distancebasedutilizationrate);         
          }
        }

        this.getVinsForDashboard();
      }, (error) => {
        this.dashboardPrefData = [];
        this.getVinsForDashboard();
      });
    }, (error) => {
      //console.log('Report not found...', error);
      this.hideloader();
      reportListData = [];
    });

  }
  
  sendMessage(): void {
    // send message to subscribers via observable subject
    this.messageService.sendMessage('refreshTimer');
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  proceedStep(prefData: any, preference: any){
    this.prefData = prefData;
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
      //console.log('No data found for this organisation dashboard...');
    });
  }

  processVins(tripData){
    let _vinList = tripData['vinTripList'].map(x=>x.vin);
    if(_vinList.length > 0){
      this.finalVinList = _vinList.filter((value, index, self) => self.indexOf(value) === index);
    }
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

}
