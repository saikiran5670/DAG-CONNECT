import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { NavigationExtras, Router } from '@angular/router';
import { ReportMapService } from 'src/app/report/report-map.service';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportService } from 'src/app/services/report.service';
import { TranslationService } from 'src/app/services/translation.service';

@Component({
  selector: 'app-vehicle-details',
  templateUrl: './vehicle-details.component.html',
  styleUrls: ['./vehicle-details.component.less']
})
export class VehicleDetailsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() filterData: any; 
  @Input() selectedElementData: any;
  @Input() translationData: any = {};
  @Input() levelList: any;
  @Input() categoryList: any;
  @Input() vehInfoPrefData: any;
  @Input() todayFlagClicked: any;
  vehicleGroups: any;
  vehicleGroupId: any;
  gridData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  accountPrefObj: any;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  mileagewithUnit: any;
  nextservicing: any;
  alertLength : any;
  vehicleDisplayPreference: any= 'dvehicledisplay_VehicleIdentificationNumber';
  
  constructor(private router: Router, private dataInterchangeService: DataInterchangeService,@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService,  private reportMapService: ReportMapService,  private organizationService: OrganizationService) { }
  
  
  // processTranslation(transData: any) {
  //   this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  // }
  ngOnInit(): void {
   // this.reportService.getFilterDetails().subscribe((data: any) => {
    //   this.filterData = data;
    //   this.levelList = [];
    // });
    // let translationObj = {
    //   id: 0,
    //   code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
    //   type: "Menu",
    //   name: "",
    //   value: "",
    //   filter: "",
    //   menuId: 10 //-- for fleet utilisation
    // }
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    // this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
    //   this.processTranslation(data);
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
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
    // });
 
  }

  checkForPreference(fieldKey) {
    if (this.vehInfoPrefData) {
      let filterData = this.vehInfoPrefData.filter(item => item.key.includes('rp_fo_fleetoverview_'+fieldKey));
      if (filterData.length > 0) {
        if (filterData[0].state == 'A') {
          return true;
        } else {
          return false;
        }
      }
    }
    return true;
  }
  

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    let vehicleDisplayId = preference.vehicleDisplayId;
    if(vehicleDisplayId) {
      let vehicledisplay = prefData?.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
      if(vehicledisplay.length != 0) {
        this.vehicleDisplayPreference = vehicledisplay[0].name;
      }
    }
    this.selectedElementData.fleetOverviewAlert.forEach(item => {
      this.levelList.forEach(element => {
        if(item.level ==element.value)
        {         
         item.originLevel = element.name;
        }
       });              
    }); 
    this.selectedElementData.fleetOverviewAlert.forEach(item => {
      // this.categoryList.forEach(element => {
      //   if(item.categoryType ==element.value)
      //   {         
      //    item.categoryType = element.name;
      //   }
      //  });      
      if(this.filterData && this.filterData.alertType && this.filterData.alertType.length > 0){
        this.filterData.alertType.forEach(element => {
          if(item.type == element.value){        
            item.type = this.translationData[element.name]; 
          //  item.type = element.name;
          }
        });
      }        
    }); 
    this.gridData = this.selectedElementData;
    this.alertLength = this.gridData.fleetOverviewAlert ? this.gridData.fleetOverviewAlert.length : 0;
    this.gridData.fleetOverviewAlert.forEach(element => {
      if (this.filterData && this.filterData.alertLevel && this.filterData.alertLevel.length > 0) {
        this.filterData.alertLevel.forEach(item => {
        if(item.value == element.level){        
          element.originLevel = this.translationData[item.name];
        }
      });
    }
    });
    this.mileagewithUnit = this.reportMapService.getDistance(this.selectedElementData.odometerVal,this.prefUnitFormat);
    this.nextservicing = this.reportMapService.getDistance(this.selectedElementData.distanceUntilNextService,this.prefUnitFormat)
    

  }

  timeConversion(time: any){
    let newDate = ''
    if(time){
      newDate = this.reportMapService.getStartTime(time, this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true);
    }
    else{
      newDate = '';
    }
    return newDate;

  }

  toBack() {
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.backToPage.emit(emitObj);
  }

  gotoHealthStatus(){
    let obj = {
      fromVehicleHealth : true
    }
    this.dataInterchangeService.gethealthDetails(this.selectedElementData);
  }

  gotoLogBook(){
    this.vehicleGroups =this.filterData.vehicleGroups;   
    for(let i=0; i< this.vehicleGroups.length; i++)
    {
      if(this.vehicleGroups[i].vin == this.selectedElementData.vin)
      {
        this.vehicleGroupId = this.vehicleGroups[i].vehicleGroupId;
      }
    }   
   this.selectedElementData.vehicleGroupId = this.vehicleGroupId; 
  //  if(this.selectedElementData.startTimeStamp != 0 && this.selectedElementData.endTimeStamp != 0) {
    // this.selectedElementData.endDate = this.selectedElementData.endTimeStamp;
    this.selectedElementData.startDate =  this.selectedElementData.fleetOverviewAlert[this.selectedElementData.fleetOverviewAlert.length-1].time;//this.selectedElementData.startTimeStamp;
    this.selectedElementData.endDate = this.selectedElementData.fleetOverviewAlert[0].time;//this.selectedElementData.latestProcessedMessageTimeStamp;
  //  }
   this.selectedElementData.todayFlag = this.todayFlagClicked;

    const navigationExtras: NavigationExtras = {
      state: {
        fromVehicleDetails: true,
        data: this.selectedElementData
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }

}
