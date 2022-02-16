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
  prefDetail: any = {};
  constructor(private router: Router, private dataInterchangeService: DataInterchangeService, @Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService,  private reportMapService: ReportMapService,  private organizationService: OrganizationService) { }
  
  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    if(this.prefDetail){
      if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ 
        this.proceedStep(this.accountPrefObj.accountPreference);
      }else{ 
        this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
          this.proceedStep(orgPref);
        }, (error) => { 
          this.proceedStep({});
        });
      }
    }
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
      this.filterData.alertLevel.forEach(item => {
        if(item.value == element.level){        
          element.originLevel = this.translationData[item.name]; 
        }
      });
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
