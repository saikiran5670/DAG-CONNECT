import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChildren, ViewChild, ElementRef, Inject } from '@angular/core';
import { FormBuilder, FormGroup,FormControl, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { NgxMaterialTimepickerComponent } from 'ngx-material-timepicker';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { ReportMapService } from '../../../report/report-map.service';
import { TranslationService } from '../../../services/translation.service';
import { OrganizationService } from 'src/app/services/organization.service';
import { Util } from 'src/app/shared/util';
import * as moment from 'moment';
import { MAT_DATE_FORMATS } from '@angular/material/core';

@Component({
    selector: 'app-create-edit-view-rule',
    templateUrl: './create-edit-view-rule.component.html',
    styleUrls: ['./create-edit-view-rule.component.less']
  })
  
  export class CreateEditViewRuleComponent implements OnInit {
    breadcumMsg: any;
  localStLanguage: any;
  accountPrefObj: any;
  startTimeDisplay: string;
  endTimeDisplay: string;

    constructor(private _formBuilder: FormBuilder, private reportMapService: ReportMapService, 
      private translationService: TranslationService, private organizationService: OrganizationService,){

    }

    translationData: any = {};
    @Output() backToPage = new EventEmitter<any>();
    @Input() actionType: any;
    @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
    @Output() createViewEditRuleEmit = new EventEmitter<object>();
    @Input() prefTimeFormat: any; //-- coming from pref setting
    @Input() prefTimeZone: any; //-- coming from pref setting
    @Input() prefDateFormat: any; //-- coming from pref setting
    @Input() prefUnitFormat: any; //-- coming from pref setting
    // @Input() translationData: any = {};
    @Input() selectedRowData: any;
    initData: any = [];
    ruleForm: FormGroup;
    isExpandedOpenAlert: boolean = true;
    oemList: any= [];
    minDate:any;
    maxDate:any;
    minTime:any;
    tcuBrands: any = [];
    vehicleGenerationList: any = [];
    dataFrequencyList: any = [];
    connectStateList: any = [];
    targetSystemList: any = [];
    actionArgumentList: any = [];
    actionList: any = [];
    showLoadingIndicator: boolean = false;
    // prefTimeFormat: "12";
    // prefTimeZone: any; //-- coming from pref setting
    startDateValue: any;
    endDateValue: any;
    todayDate: any;
    accountOrganizationId: number;
    accountId: number;
    accountRoleId: number;
    selectedStartTime: any = '00:00';
    selectedEndTime: any = '23:59';
    

    ngOnInit() {
      console.log(this.prefTimeZone, 'pref');
      if(localStorage.getItem('contextOrgId')){
        this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
      }
      else{
        this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
      } 
      // let today = new Date();
      // let nextWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate()+7);
      this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
      this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
      this.ruleForm = this._formBuilder.group({
        description: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
        status: ['A', [Validators.required]],
        startDate: [''],
        startTime: [''],
        endDate: [''],
        endTime: [''],
        OEM: [''],
        tcuBrand: [''],
        vehicleGeneration: [''],
        dataFrequency: [''],
        connectState: [''],
        targetSystem: ['',[Validators.required]],
        action: ['', [Validators.required]],
        actionArgument: ['']
      },
      {
        validator: [
          CustomValidators.specialCharValidationForName('description')  
        ]
      });
      if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'create'){
        // this.breadcumMsg = this.getBreadcum();
      }
      this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    let _langCode = this.localStLanguage ? this.localStLanguage.code  :  "EN-GB";
      this.dataFrequencyList = [{value:'high'}, {value: 'low'}]
      this.setDefaultTodayDate();
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
        if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
          this.showLoadingIndicator = false;
        }else{ // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
            this.proceedStep(prefData, orgPref);
            this.showLoadingIndicator = false;
          }, (error) => { // failed org API
            this.showLoadingIndicator = false;
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }
    });
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
      // 
      // this.setPrefFormatDate();
      this.setDefaultTodayDate();
      this.setDefaultStartEndTime();
    
      //console.log(this.prefUnitFormat);
    }

    setDefaultStartEndTime() {
        if (this.prefTimeFormat == 24) {
          this.startTimeDisplay = '00:00:00';
          this.endTimeDisplay = '23:59:59';
          this.selectedStartTime = "00:00";
          this.selectedEndTime = "23:59";
        } else {
          this.startTimeDisplay = '12:00:00 AM';
          this.endTimeDisplay = '11:59:59 PM';
          this.selectedStartTime = "12:00 AM";
          this.selectedEndTime = "11:59 PM";
        }
      }
  
    getBreadcum() {
      let page = '';
      if(this.actionType == 'edit')
        page = (this.translationData.lblEditAlertDetails ? this.translationData.lblEditAlertDetails : 'Edit Alert Details') ;
      else if(this.actionType === 'view')
        page = (this.translationData.lblViewAlertDetails ? this.translationData.lblViewAlertDetails : 'View Alert Details');
      else if(this.actionType === 'create' || this.actionType === 'duplicate')
        page = (this.translationData.lblCreateNewAlert ? this.translationData.lblCreateNewAlert : 'Create New Alert');
      
      return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
      ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
      ${this.translationData.lblAlerts ? this.translationData.lblAlerts : "Alerts"} / 
      ${page}`;
    }

    toBack(){
      let emitObj = {
        stepFlag: false,
      }
      this.createViewEditRuleEmit.emit(emitObj);
    }

    onStatusChange(event){

    }

    setDefaultTodayDate() {
      this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
      this.todayDate = this.getTodayDate();
    }

    getTodayDate() {
      console.log(this.prefTimeZone, 'pref');
      let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
      _todayDate.setHours(0);
      _todayDate.setMinutes(0);
      _todayDate.setSeconds(0);
      return _todayDate;
    }

    setStartEndDateTime(date: any, timeObj: any, type: any) {
      return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
    }
    changeStartDateEvent (event: MatDatepickerInputEvent<any>) {}
    onChangeTcuBrand(event){}
    onCancel(){}
    onReset(){}
    onCreateUpdate(){}
    startTimeChanged(event){
     
    }
    changeEndDateEvent(event: MatDatepickerInputEvent<any>){
        
    }
    endTimeChanged(event){}
  }
