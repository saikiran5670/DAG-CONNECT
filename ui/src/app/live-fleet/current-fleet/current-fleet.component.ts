import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, NgZone } from '@angular/core';
import { TranslationService } from '../../services/translation.service';
import { ReportService } from 'src/app/services/report.service';
import { MessageService } from 'src/app/services/message.service';
import { Subscription } from 'rxjs';
import { DataInterchangeService} from '../../services/data-interchange.service';
import { OrganizationService } from '../../services/organization.service';


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
  detailsData =[];
  messages: any[] = [];
  subscription: Subscription;
  isOpen: boolean = false;
  healthData: any = [];
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  preferenceObject : any;
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
    private organizationService: OrganizationService) { 
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
      menuId: 2 
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      // this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
      //   if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
      //     this.proceedStep(prefData, this.accountPrefObj.accountPreference);
      //   }else{ // org pref
      //     this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
      //       this.proceedStep(prefData, orgPref);
      //     }, (error) => { // failed org API
      //       let pref: any = {};
      //       this.proceedStep(prefData, pref);
      //     });
      //   }
      // });
    });
    this.clickOpenClose='Click to Open';
    let objData = {
      "groupId": ["all"],
      "alertLevel": ["all"],
      "alertCategory": ["all"],
      "healthStatus": ["all"],
      "otherFilter": ["all"],
      "driverId": ["all"],
      "days": 90,
      "languagecode":"cs-CZ"
    }
    this.reportService.getFleetOverviewDetails(objData).subscribe((data:any) => {
       this.detailsData = data;
        this.dataInterchangeService.getVehicleData(data);

    });
   }

   processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }
  userPreferencesSetting(event) {
    this.userPreferencesFlag = !this.userPreferencesFlag;
    let summary = document.getElementById("summary");
    let sidenav = document.getElementById("sidenav");

    if(this.userPreferencesFlag){
    summary.style.width = '67%';
    sidenav.style.width = '32%';
    this.clickOpenClose='Click to Hide';
    }
    else{
      summary.style.width = '100%';
      sidenav.style.width = '0%';
      this.clickOpenClose='Click To Open';
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
    this.isOpen = false;
 }
}
