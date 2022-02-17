import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, NgZone } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { ReportService } from 'src/app/services/report.service';
import { MessageService } from 'src/app/services/message.service';
import { Subscription } from 'rxjs';
import { DataInterchangeService} from '../../services/data-interchange.service'; 
import { Router } from '@angular/router';
import { FleetMapService } from './fleet-map.service';
import { DashboardService } from 'src/app/services/dashboard.service';

declare var H: any;

@Component({
  selector: 'app-current-fleet',
  templateUrl: './current-fleet.component.html',
  styleUrls: ['./current-fleet.component.less']
})
export class CurrentFleetComponent implements OnInit {

  private platform: any;
  public userPreferencesFlag: boolean = false;
  localStLanguage: any;
  accountOrganizationId: any;
  translationData: any = {};
  clickOpenClose:string;
  currentFleetReportId: number;
  detailsData =[];
  messages: any[] = [];
  subscription: Subscription;
  isOpen: boolean = false;
  obj: any = {
    fromVehicleHealth: false,
    isOpen: false,
    selectedElementData: []
  };
  healthData: any = [];
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  preferenceObject : any;
  _state: any;
  filterData : any;
  filterPOIData : any;
  showLoadingIndicator: boolean = false;
  totalVehicleCount: number;
  dashboardPref: any;
  reportDetail: any = [];
  
  constructor(private translationService: TranslationService,
    private reportService: ReportService,
    private messageService: MessageService,
    private dataInterchangeService: DataInterchangeService,
    private router: Router, private fleetMapService: FleetMapService,
    private dashboardService : DashboardService) { 
      this.subscription = this.messageService.getMessage().subscribe(message => {
        if (message.key.indexOf("refreshData") !== -1) {
          this.refreshData();
        }
      });
      this.sendMessage();
      this.dataInterchangeService.healthData$.subscribe(data => {
        this.healthData = data;
        this.isOpen = true;
      });
      const navigation = this.router.getCurrentNavigation();
      this._state = navigation.extras.state as {
        fromVehicleDetails: boolean,
        data: any
      };
    }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail'));  
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 
    }
    this.showLoadingIndicator = true;
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.getFleetOverviewPreferences();
    });
   }

  getFleetOverviewPreferences(){
    if(this.reportDetail){
      let repoId: any = this.reportDetail.filter(i => i.name == 'Fleet Overview');
      let repoIdDB: any = this.reportDetail.find(i => i.name == 'Dashboard');
      if(repoId.length > 0){
        this.currentFleetReportId = repoId[0].id; 
        this.callPreferences();
      }
      if(repoIdDB){
        this.dashboardService.getDashboardPreferences(repoIdDB.id).subscribe((prefData: any) => {
          this.dashboardPref = prefData['userPreferences'];
        }, (error) => { });
      }
    }
  }

  callPreferences(){
    this.reportService.getReportUserPreference(this.currentFleetReportId).subscribe((data : any) => {
      this.hideLoader();
      let _preferencesData = data['userPreferences'];
      this.getTranslatedColumnName(_preferencesData);
      this.getFilterPOIData();
    }, (error)=>{;
      this.hideLoader();
      this.getFilterPOIData();
    });
  }

  hideLoader(){
    this.showLoadingIndicator = false;
  }

  timerPrefData: any = [];
  vehInfoPrefData: any = [];
  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        //if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          //element.subReportUserPreferences.forEach(item => {
            let _data: any = element;
            // if(element.key.includes('rp_fo_fleetoverview_settimer_')){
            //   this.timerPrefData.push(_data);
            // }else 
            if(element.key.includes('rp_fo_fleetoverview_generalvehicleinformation_')){
              let index: any;
             switch(element.key){
               case 'rp_fo_fleetoverview_generalvehicleinformation_currentmileage':{
                 index = 0;
                 break;
               }
               case 'rp_fo_fleetoverview_generalvehicleinformation_nextservicein':{
                 index = 1;
                 break;
               }
               case 'rp_fo_fleetoverview_generalvehicleinformation_healthstatus':{
                 index = 2;
                 break;
               }
             }
              this.vehInfoPrefData[index] = _data;
            }
          //});
        //}
      });
    }
  }

  getFleetOverviewDetails(){
    // this.clickOpenClose='Click to Open';
    this.clickOpenClose = this.translationData.lblClickToOpen || 'Click to Open';
    this.showLoadingIndicator = true;
    let objData = {
      "groupId": ["all"],
      "alertLevel": ["all"],
      "alertCategory": ["all"],
      "healthStatus": ["all"],
      "otherFilter": ["all"],
      "driverId": ["all"],
      "days": 0,
      "languagecode":"cs-CZ"
    }
    this.reportService.getFleetOverviewDetails(objData).subscribe((data: any) => {
      this.totalVehicleCount = data.visibleVinsCount;
      this.hideLoader();
      let processedData = this.fleetMapService.processedLiveFLeetData(data.fleetOverviewDetailList);
      this.detailsData = processedData;
      this.getFilterData();
      let _dataObj = {
        vehicleDetailsFlag: false,
        data: this.detailsData
      }
      this.dataInterchangeService.getVehicleData(_dataObj);
      // if (this._state && this._state.data) {
      //   this.userPreferencesSetting();
      //   this.toBack();
      // }
    }, (err) => {
      this.hideLoader();
      this.getFilterData();
      this.detailsData = [];
    });
    if (this._state && this._state.data) {
      this.userPreferencesSetting();
      this.toBack();
    }
  }

  getFilterPOIData(){
    this.showLoadingIndicator = true;
    this.reportService.getFilterPOIDetails().subscribe((data: any) => {
      this.hideLoader();
      this.filterPOIData = data;
      this.getFleetOverviewDetails();
    }, (error) => {
      this.hideLoader();
      this.getFleetOverviewDetails();
    });
  }

  getFilterData(){
    this.showLoadingIndicator = true;
    this.reportService.getFilterDetails().subscribe((data: any) => {
      if(data) this.hideLoader();
      this.filterData = data;
    }, (error) => {
      this.hideLoader();
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }
  
  userPreferencesSetting(event?: any) {
    this.userPreferencesFlag = !this.userPreferencesFlag;
    let summary = document.getElementById("summary");
    let sidenav = document.getElementById("sidenav");
    if(this.userPreferencesFlag){
    summary.style.width = '67%';
    sidenav.style.width = '32%';
    this.clickOpenClose=this.translationData.lblClickToHide ? this.translationData.lblClickToHide :'Click To Hide';
    }
    else{
      summary.style.width = '100%';
      sidenav.style.width = '0%';
      this.clickOpenClose=this.translationData.lblClickToOpen ? this.translationData.lblClickToOpen :'Click To Open';
    }
  } 

  sendMessage(): void {
    this.messageService.sendMessage('refreshTimer');
  }
  
  refreshData(){}

  toBack(item?: any){
    this.obj = {
      fromVehicleHealth : true,
      isOpen: this.isOpen,
      selectedElementData: (this._state && this._state.data) ? this._state.data : this.healthData
    }
    this.isOpen = false;   
  }

}