import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, NgZone } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { ReportService } from 'src/app/services/report.service';
import { MessageService } from 'src/app/services/message.service';
import { Subscription } from 'rxjs';
import { DataInterchangeService} from '../../services/data-interchange.service';
import { OrganizationService } from '../../services/organization.service';
import { Router } from '@angular/router';
import { FleetMapService } from './fleet-map.service';

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
  showLoadingIndicator: boolean = false;

  // detailsData =[
  //   {
  //     "id": 8,
  //     "tripId": "52a0f631-4077-42f9-b999-cb21c6309c71",
  //     "vin": "XLR0998HGFFT76657",
  //     "startTimeStamp": 1623330691000,
  //     "endTimeStamp": 1623340800000,
  //     "driver1Id": "SK 2236526558846039",
  //     "tripDistance": 100,
  //     "drivingTime": 180,
  //     "fuelConsumption": 12,
  //     "vehicleDrivingStatusType": "N",
  //     "odometerVal": 555,
  //     "distanceUntilNextService": 3000,
  //     "latestReceivedPositionLattitude": 51.43042755,
  //     "latestReceivedPositionLongitude": 5.51616478,
  //     "latestReceivedPositionHeading": 306.552612012591,
  //     "startPositionLattitude": 51.43042755,
  //     "startPositionLongitude": 5.51616478,
  //     "startPositionHeading": 306.552612012591,
  //     "latestProcessedMessageTimeStamp": 1623340800000,
  //     "vehicleHealthStatusType": "T",
  //     "latestWarningClass": 11,
  //     "latestWarningNumber": 2,
  //     "latestWarningType": "A",
  //     "latestWarningTimestamp": 1623340800000,
  //     "latestWarningPositionLatitude": 51.43042755,
  //     "latestWarningPositionLongitude": 5.51616478,
  //     "vid": "M4A1117",
  //     "registrationNo": "PLOI098OOO",
  //     "driverFirstName": "Sid",
  //     "driverLastName": "U",
  //     "latestGeolocationAddressId": 36,
  //     "latestGeolocationAddress": "DAF, 5645 Eindhoven, Nederland",
  //     "startGeolocationAddressId": 36,
  //     "startGeolocationAddress": "DAF, 5645 Eindhoven, Nederland",
  //     "latestWarningGeolocationAddressId": 36,
  //     "latestWarningGeolocationAddress": "DAF, 5645 Eindhoven, Nederland",
  //     "latestWarningName": "Opotřebení brzdového obložení nákladního vozidla",
  //     "liveFleetPosition": [
  //       {
  //         "gpsAltitude": 17,
  //         "gpsHeading": 306.55261201259134,
  //         "gpsLatitude": 51.43042755,
  //         "gpsLongitude": 5.51616478,
  //         "fuelconsumtion": 0,
  //         "co2Emission": 0,
  //         "id": 37578
  //       },
  //       {
  //         "gpsAltitude": 21,
  //         "gpsHeading": 182.3779089956144,
  //         "gpsLatitude": 51.43199539,
  //         "gpsLongitude": 5.508029461,
  //         "fuelconsumtion": 342,
  //         "co2Emission": 0.9918,
  //         "id": 37581
  //       }
  //     ]
  //   },
  //   {
  //     "id": 4,
  //     "tripId": "tripid1",
  //     "vin": "BLRAE75PC0E272200",
  //     "startTimeStamp": 1620039161392,
  //     "endTimeStamp": 1720039161392,
  //     "driver1Id": "SK 1116526558846037",
  //     "tripDistance": 0,
  //     "drivingTime": 0,
  //     "fuelConsumption": 0,
  //     "vehicleDrivingStatusType": "N",
  //     "odometerVal": 0,
  //     "distanceUntilNextService": 0,
  //     "latestReceivedPositionLattitude": 51.32424545,
  //     "latestReceivedPositionLongitude": 5.2228899,
  //     "latestReceivedPositionHeading": 0,
  //     "startPositionLattitude": 0,
  //     "startPositionLongitude": 0,
  //     "startPositionHeading": 0,
  //     "latestProcessedMessageTimeStamp": 1720039161392,
  //     "vehicleHealthStatusType": "S",
  //     "latestWarningClass": 11,
  //     "latestWarningNumber": 2,
  //     "latestWarningType": "A",
  //     "latestWarningTimestamp": 1620039161392,
  //     "latestWarningPositionLatitude": 0,
  //     "latestWarningPositionLongitude": 0,
  //     "vid": "",
  //     "registrationNo": "PLOI098OJJ",
  //     "driverFirstName": "Neeraj",
  //     "driverLastName": "Lohumi",
  //     "latestGeolocationAddressId": 2,
  //     "latestGeolocationAddress": "5531 Bladel, Nederland",
  //     "startGeolocationAddressId": 0,
  //     "startGeolocationAddress": "",
  //     "latestWarningGeolocationAddressId": 0,
  //     "latestWarningGeolocationAddress": "",
  //     "latestWarningName": "Opotřebení brzdového obložení nákladního vozidla",
  //     "liveFleetPosition": []
  //   }
  // ];
  
  constructor(private translationService: TranslationService,
    private reportService: ReportService,
    private messageService: MessageService,
    private dataInterchangeService: DataInterchangeService,
    private organizationService: OrganizationService, private router: Router, private fleetMapService: FleetMapService) { 
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
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      let repoId: any = reportListData.filter(i => i.name == 'Fleet Overview');
      if(repoId.length > 0){
        this.currentFleetReportId = repoId[0].id; 
        this.callPreferences();
      }else{
        console.error("No report id found!")
      }
    }, (error)=>{
      console.log('Report not found...', error);
      reportListData = [{name: 'Fleet Overview', id: this.currentFleetReportId}];
      // this.callPreferences();
    });
  }

  callPreferences(){
    this.reportService.getReportUserPreference(this.currentFleetReportId).subscribe((data : any) => {
      this.hideLoader();
      let _preferencesData = data['userPreferences'];
      this.getTranslatedColumnName(_preferencesData);
      this.getFilterData();
    }, (error)=>{
      console.log('Pref not found...');
      this.hideLoader();
      this.getFilterData();
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
      this.hideLoader();
      let processedData = this.fleetMapService.processedLiveFLeetData(data);
      this.detailsData = processedData;
      let _dataObj = {
        vehicleDetailsFlag: false,
        data: this.detailsData
      }
      this.dataInterchangeService.getVehicleData(_dataObj);
      if (this._state && this._state.data) {
        this.userPreferencesSetting();
        this.toBack();
      }
    }, (err) => {
      this.hideLoader();
    });
  }

  getFilterData(){
    this.showLoadingIndicator = true;
    this.reportService.getFilterDetails().subscribe((data: any) => {
      this.hideLoader();
      this.filterData = data;
      this.getFleetOverviewDetails();
    }, (error) => {
      this.hideLoader();
      this.getFleetOverviewDetails();
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

  
  // proceedStep(prefData: any, preference: any){
  //   let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
  //   if(_search.length > 0){
  //     this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
  //     this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
  //     this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
  //     this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
  //   }else{
  //     this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
  //     this.prefTimeZone = prefData.timezone[0].value;
  //     this.prefDateFormat = prefData.dateformat[0].name;
  //     this.prefUnitFormat = prefData.unit[0].name;
  //   }
  //   this.preferenceObject = {
  //     prefTimeFormat : this.prefTimeFormat,
  //     prefTimeZone : this.prefTimeZone,
  //     prefDateFormat : this.prefDateFormat,
  //     prefUnitFormat : this.prefUnitFormat
  //   }
  // }

  sendMessage(): void {
    // send message to subscribers via observable subject
    this.messageService.sendMessage('refreshTimer');
  }
  
  refreshData(){}

  toBack(){
    this.obj ={
      fromVehicleHealth : true,
      isOpen: this.isOpen,
      selectedElementData: (this._state && this._state.data) ? this._state.data : this.healthData
    }
    this.isOpen = false;
   
 }
}
