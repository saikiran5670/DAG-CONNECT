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
  @Input() selectedElementData: any;
  @Input() translationData: any;
  @Input() levelList: any;
  @Input() categoryList: any;
  @Input() vehInfoPrefData: any;
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
  
  constructor(private router: Router, private dataInterchangeService: DataInterchangeService,@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService,  private reportMapService: ReportMapService,  private organizationService: OrganizationService) { }
  
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }
  ngOnInit(): void {
   // this.reportService.getFilterDetails().subscribe((data: any) => {
    //   this.filterData = data;
    //   this.levelList = [];
    // });
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 10 //-- for fleet utilisation
    }
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
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
    });
 
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.selectedElementData.fleetOverviewAlert.forEach(item => {
      this.levelList.forEach(element => {
        if(item.level ==element.value)
        {         
         item.level = element.name;
        }
       });              
    }); 
    this.selectedElementData.fleetOverviewAlert.forEach(item => {
      this.categoryList.forEach(element => {
        if(item.categoryType ==element.value)
        {         
         item.categoryType = element.name;
        }
       });              
    }); 
    this.gridData = this.selectedElementData;
    this.alertLength = this.gridData.fleetOverviewAlert ? this.gridData.fleetOverviewAlert.length : 0;
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
    const navigationExtras: NavigationExtras = {
      state: {
        fromVehicleDetails: true,
        data: this.selectedElementData
      }
    };
    this.router.navigate(['fleetoverview/logbook'], navigationExtras);
  }

}
