import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit, NgZone } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { ReportService } from 'src/app/services/report.service';
import { MessageService } from 'src/app/services/message.service';
import { Subscription } from 'rxjs';
import { DataInterchangeService} from '../../services/data-interchange.service'; 
import { Router } from '@angular/router';
import { FleetMapService } from './fleet-map.service';
import { DashboardService } from 'src/app/services/dashboard.service';
import { Util } from 'src/app/shared/util';
import { ReportMapService } from 'src/app/report/report-map.service';
import { OrganizationService } from '../../services/organization.service';
declare var H: any;

@Component({
  selector: 'app-current-fleet',
  templateUrl: './current-fleet.component.html',
  styleUrls: ['./current-fleet.component.less']
})
export class CurrentFleetComponent implements OnInit, OnDestroy {

  private platform: any;
  fleetSummary : any ={};
  public userPreferencesFlag: boolean = false;
  localStLanguage: any;
  accountOrganizationId: any;
  translationData: any = {};
  clickOpenClose:string;
  currentFleetReportId: number;
  detailsData = [];
  fleetOverViewDetail: any;
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
  preferenceObject : any = {};
  _state: any;
  filterData : any;
  filterPOIData : any;
  showLoadingIndicator: boolean = false;
  totalVehicleCount: number;
  dashboardPref: any;
  reportDetail: any = [];
  prefDetail: any = {};
  vehicleGroups: any = [];
  
  constructor(private translationService: TranslationService,
    private reportService: ReportService, private reportMapService: ReportMapService,
    private messageService: MessageService,
    private dataInterchangeService: DataInterchangeService,
    private router: Router, private fleetMapService: FleetMapService,
    private dashboardService : DashboardService,
    private organizationService: OrganizationService) { 
      this.subscription = this.messageService.getMessage().subscribe(message => {
        if (!this.dataInterchangeService.isFleetOverViewFilterOpen && message.key.indexOf("refreshData") !== -1) {
          this.getFleetOverviewDetails();
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
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
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
    let menuId = 'menu_3_' + this.localStLanguage.code; 
    if(!localStorage.getItem(menuId)){
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
        this.showLoadingIndicator = false;
      });
    } else{
      this.translationData = JSON.parse(localStorage.getItem(menuId));
      this.showLoadingIndicator = false;
    }
      this.getFleetOverviewPreferences();
    
    if(this.prefDetail){
      if (accountPrefObj.accountPreference && accountPrefObj.accountPreference != '') {
        this.proceedStep(accountPrefObj.accountPreference);
      } else {
        this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
          this.proceedStep(orgPref);
        }, (error) => { 
          this.proceedStep({});
        });
      }
    }
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
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
    let selectedStartTime = '';
    let selectedEndTime = '';
    if(this.prefTimeFormat == 24){
      selectedStartTime = "00:00";
      selectedEndTime = "23:59";
    } else{      
      selectedStartTime = "12:00 AM";
      selectedEndTime = "11:59 PM";
    }
    let startDateValue = this.setStartEndDateTime(Util.getUTCDate(this.prefTimeZone), selectedStartTime, 'start');
    let endDateValue = this.setStartEndDateTime(Util.getUTCDate(this.prefTimeZone), selectedEndTime, 'end');
    let _startTime = Util.getMillisecondsToUTCDate(startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(endDateValue, this.prefTimeZone);
    let objData = {
      "groupId": ["all"],
      "alertLevel": ["all"],
      "alertCategory": ["all"],
      "healthStatus": ["all"],
      "otherFilter": ["all"],
      "driverId": ["all"],
      "days": 0,
      "languagecode":"cs-CZ",
      "StartDateTime":_startTime,
      "EndDateTime":_endTime
    }
    this.reportService.getFleetOverviewDetails(objData).subscribe((data: any) => {
      this.totalVehicleCount = data.visibleVinsCount;
      this.hideLoader();
      this.detailsData = data.fleetOverviewDetailList;
      this.fleetOverViewDetail = data;
      this.vehicleGroups = data.vehicleGroups
      this.getFilterData();
      let _dataObj = {
        vehicleDetailsFlag: false,
        data: this.detailsData
      }
      this.dataInterchangeService.getVehicleData(_dataObj);
      this.dataInterchangeService.setFleetOverViewDetails(JSON.parse(JSON.stringify(data)));
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
  setStartEndDateTime(date: any, timeObj: any, type: any){   
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_3_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
  }
  
  isFilterOpenClick: boolean=false;
  userPreferencesSetting(event?: any) {
    this.userPreferencesFlag = !this.userPreferencesFlag;
    let summary = document.getElementById("summary");
    let sidenav = document.getElementById("sidenav");
    if(this.userPreferencesFlag){
      this.isFilterOpenClick=true;
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

  
  proceedStep(preference: any){
    let _search = this.prefDetail.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = this.prefDetail.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = this.prefDetail.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = this.prefDetail.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = Number(this.prefDetail.timeformat[0].name.split("_")[1].substring(0,2));   
      this.prefTimeZone = this.prefDetail.timezone[0].name;
      this.prefDateFormat = this.prefDetail.dateformat[0].name;
      this.prefUnitFormat = this.prefDetail.unit[0].name;
    }
    this.preferenceObject = {
      prefTimeFormat : this.prefTimeFormat,
      prefTimeZone : this.prefTimeZone,
      prefDateFormat : this.prefDateFormat,
      prefUnitFormat : this.prefUnitFormat
    }
  }

  sendMessage(): void {
    this.messageService.sendMessage('refreshTimer');
  }

  toBack(item?: any){
    this.obj = {
      fromVehicleHealth : true,
      isOpen: this.isOpen,
      selectedElementData: (this._state && this._state.data) ? this._state.data : this.healthData
    }
    this.isOpen = false;   
  }

}