import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportSchedulerService } from 'src/app/services/report.scheduler.service';
import { TranslationService } from 'src/app/services/translation.service';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { Util } from '../../../shared/util';
import * as moment from 'moment-timezone';

@Component({
  selector: 'app-create-edit-view-report-scheduler',
  templateUrl: './create-edit-view-report-scheduler.component.html',
  styleUrls: ['./create-edit-view-report-scheduler.component.less']
})
export class CreateEditViewReportSchedulerComponent implements OnInit {

  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Input() reportSchedulerParameterData: any;
  @Output() backToPage = new EventEmitter<any>();
  
  breadcumMsg: any = '';
  reportSchedulerForm: FormGroup;
  accountOrganizationId: any;
  accountId: any;
  userType: any= "";
  scheduleCreatedMsg: any= '';
  ReportTypeList: any= [];
  VehicleGroupList: any= [];
  LanguageCodeList: any= [];
  VehicleList: any= [];
  DriverList: any= [];
  RecipientList: any= [];
  selectedIndex: number = 0;
  tabVisibilityStatus: boolean = true;
  selectionTab: string = 'D';
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  dispatchTimeDisplay: any = '23:59:59';
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  selectedDispatchTime: any = '23:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  accountPrefObj: any;
  localStLanguage: any;
  weekdays: any= [];
  months: any= [];
  quarters: any= [];
  biweeklyStartDateValue: any;
  biweeklyEndDateValue: any;
  monthlyStartDateValue: any;
  monthlyEndDateValue: any;
  selectedMonth: any;
  quarterlylyStartDateValue: any;
  quarterlyEndDateValue: any;
  selectedQuarter: any;
  dispatchHours: any= [];
  showDriverList: boolean= true;
  keyword = 'email';
  isInvalidEmail: boolean = false;
  invalidEmail: string;
  only10Emails: boolean= false;
  recipientEmailList: any= [];

  constructor(private _formBuilder: FormBuilder, 
              private reportSchedulerService: ReportSchedulerService,
              private translationService: TranslationService,
              private organizationService: OrganizationService,
              @Inject(MAT_DATE_FORMATS) private dateFormats) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.reportSchedulerForm = this._formBuilder.group({
      reportType : ['', [Validators.required]],
      vehicleGroup : [0, [Validators.required]],
      language : ['', [Validators.required]],
      vehicle : [0, [Validators.required]],
      recipientEmail : ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      driver : [0, []],
      mailSubject : ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      mailDescription : ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      dailyStartTime : new FormControl({value: '', disabled: true}),
      dailyEndTime : new FormControl({value: '', disabled: true}),
      reportDispatchTime : [23, []],
      weeklyStartDay :['', []],
      weeklyEndDay : ['', []],
      biweeklyStartDate: new FormControl({value: '', disabled: true}),
      biweeklyStartDay: ['', []],
      biweeklyStartTime : new FormControl({value: '', disabled: true}),
      biweeklyEndDate: ['', []],
      biweeklyEndDay: ['', []],
      biweeklyEndTime: new FormControl({value: '', disabled: true}),
      month: ['', []],
      monthlyStartDate: new FormControl({value: '', disabled: true}),
      monthlyEndDate: new FormControl({value: '', disabled: true}),
      monthlyStartTime: new FormControl({value: '', disabled: true}),
      monthlyEndTime: new FormControl({value: '', disabled: true}),
      quarter: ['', []],
      quarterlyStartDate: new FormControl({value: '', disabled: true}),
      quarterlyEndDate: new FormControl({value: '', disabled: true}),
      quarterlyStartTime: new FormControl({value: '', disabled: true}),
      quarterlyEndTime: new FormControl({value: '', disabled: true}),
    });

    this.weekdays= [{id : 0, value : 'Sunday'},{id : 1, value : 'Monday'},{id : 2, value : 'Tuesday'},{id : 3, value : 'Wednesday'},{id : 4, value : 'Thursday'},{id : 5, value : 'Friday'},{id : 6, value : 'Saturday'}];
    this.months= [{id : 0, value : 'January'},{id : 1, value : 'February'},{id : 2, value : 'March'},{id : 3, value : 'April'},{id : 4, value : 'May'},{id : 5, value : 'June'},
                  {id : 6, value : 'July'},{id : 7, value : 'August'},{id : 8, value : 'September'},{id : 9, value : 'October'},{id : 10, value : 'November'},{id : 11, value : 'December'}]
    this.quarters= [{id : 0, value : 'Quarter1 (Jan-Mar)'}, {id : 1, value : 'Quarter2 (Apr-Jun)'},
                    {id : 2, value : 'Quarter3 (Jul-Sept)'},{id : 3, value : 'Quarter4 (Oct-Dec)'}]                  
    for(let i = 1; i < 24; i++){
      this.dispatchHours.push(i);
    }

    this.ReportTypeList = this.reportSchedulerParameterData["reportType"];
    this.VehicleGroupList = this.getUnique(this.reportSchedulerParameterData["associatedVehicle"], "vehicleGroupId");
    this.VehicleList = this.getUnique(this.reportSchedulerParameterData["associatedVehicle"], "vehicleId");
    this.DriverList = this.reportSchedulerParameterData["driverDetail"];
    this.LanguageCodeList = JSON.parse(localStorage.getItem("languageCodeList"));
    this.RecipientList = this.reportSchedulerParameterData["receiptEmails"];

    this.breadcumMsg = this.getBreadcum();
    if(this.actionType == 'edit' || this.actionType == 'view'){
      this.setDefaultValues();
    }

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
  }

  getUnique(arr, comp) {

    // store the comparison  values in array
    const unique =  arr.map(e => e[comp])
  
      // store the indexes of the unique objects
      .map((e, i, final) => final.indexOf(e) === i && i)
  
      // eliminate the false indexes & return unique objects
    .filter((e) => arr[e]).map(e => arr[e]);
  
    return unique;
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
    }
    this.setPrefFormatTime();
    this.setPrefFormatDate();
    // this.setDefaultTodayDate();
  }

  setPrefFormatTime(){
      if(this.prefTimeFormat == 24){
        this.startTimeDisplay = '00:00:00';
        this.endTimeDisplay = '23:59:59';
        this.dispatchTimeDisplay = '23:59:59';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
        this.selectedDispatchTime = "23:59";
      } else{
        this.startTimeDisplay = '12:00 AM';
        this.endTimeDisplay = '11:59 PM';
        this.dispatchTimeDisplay = '11:59 PM';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
        this.selectedDispatchTime = "23:59";
      }
  }

  setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        break;
      }
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
      }
    }
  }


  setDefaultValues(){
    // this.imageMaxMsg = false;
    // this.imageEmptyMsg = false;
    // this.categoryForm.get('categoryName').setValue(this.selectedRowData.subCategoryId == 0 ? this.selectedRowData.parentCategoryName : this.selectedRowData.subCategoryName);
    // this.categoryForm.get('type').setValue(this.selectedRowData.organizationId ? (this.selectedRowData.organizationId  > 0 ? 'Regular': 'Global' ) : 'Global');
    // this.categoryForm.get('categoryDescription').setValue(this.selectedRowData.description);
    // this.selectedCategoryType = this.selectedRowData.subCategoryId == 0 ? 'category' : 'subcategory';
    // this.categoryForm.get('parentCategory').setValue(this.selectedRowData.parentCategoryId);
    // //this.categoryForm.get('uploadFile').setValue(this.selectedRowData.icon);
  }
  
  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblReportScheduler : "ReportScheduler"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditScheduleDetails ? this.translationData.lblEditScheduleDetails : 'Edit Schedule Details') : (this.actionType == 'view') ? (this.translationData.lblViewScheduleDetails ? this.translationData.lblViewScheduleDetails : 'View Schedule Details') : (this.translationData.lblCreateScheduleDetails ? this.translationData.lblScheduleNewReport : 'Schedule New Report')}`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onReset(){
    this.setDefaultValues();
  }

  getScheduleCreatedUpdatedMessage() {
    let reportName = `${this.ReportTypeList.filter(item => item.id == this.reportSchedulerForm.controls.reportType.value)[0].reportName}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblScheduleCreatedSuccessfully)
        return this.translationData.lblScheduleCreatedSuccessfully.replace('$', reportName);
      else
        return ("New Report Schedule for '$' Created Successfully").replace('$', reportName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblScheduleUpdatedSuccessfully)
        return this.translationData.lblScheduleUpdatedSuccessfully.replace('$', reportName);
      else
        return ("Report Schedule for '$' Updated Successfully").replace('$', reportName);
    }
    else{
      return '';
    }
  }

  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }

  selectionTimeRange(timeRange: string){
    this.selectionTab = timeRange;
    switch(timeRange){
      case 'W': {
        this.reportSchedulerForm.get('weeklyStartDay').setValue(1);
        this.reportSchedulerForm.get('weeklyEndDay').setValue(0);
        break;
      }
      case 'B': {
        this.biweeklyEndDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        this.reportSchedulerForm.get('biweeklyEndDay').setValue(this.weekdays.filter(item => item.id == (this.biweeklyEndDateValue.getDay()))[0].value);

        let startDate = new Date();
        startDate.setDate(startDate.getDate() - 13);
        this.biweeklyStartDateValue = (this.setStartEndDateTime(startDate, this.selectedStartTime, 'start'));
        this.reportSchedulerForm.get('biweeklyStartDay').setValue(this.weekdays.filter(item => item.id == (this.biweeklyStartDateValue.getDay()))[0].value);
        break;
      }
      case 'M': {
        this.reportSchedulerForm.get('month').setValue((new Date()).getMonth());
        this.selectedMonth = this.reportSchedulerForm.controls.month.value;
        let date = new Date();
        let year = date.getFullYear();
        this.monthlyStartDateValue = new Date(year, this.selectedMonth, 1);
        this.monthlyEndDateValue = new Date(year, this.selectedMonth + 1, 0);
        break;
      }
      case 'Q': {
        let date = new Date();
        let currentMonth = date.getMonth();
        let year = date.getFullYear();

        if(currentMonth >=0 && currentMonth<=2){
          this.reportSchedulerForm.get('quarter').setValue(0);
          this.quarterlylyStartDateValue = new Date(year, 0 /*January*/ , 1);
          this.quarterlyEndDateValue = new Date(year, 3, 0);
        }
        else if(currentMonth >=3 && currentMonth<=5){
          this.reportSchedulerForm.get('quarter').setValue(1);
          this.quarterlylyStartDateValue = new Date(year, 3 /*April*/ , 1);
          this.quarterlyEndDateValue = new Date(year, 6, 0);
        }
        else if(currentMonth >=6 && currentMonth<=8){
          this.reportSchedulerForm.get('quarter').setValue(2);
          this.quarterlylyStartDateValue = new Date(year, 6 /*July*/ , 1);
          this.quarterlyEndDateValue = new Date(year, 9, 0);
        }
        else if(currentMonth >=9 && currentMonth<=11){
          this.reportSchedulerForm.get('quarter').setValue(3);
          this.quarterlylyStartDateValue = new Date(year, 9 /*October*/ , 1);
          this.quarterlyEndDateValue = new Date(year, 12, 0);
        }
        this.selectedQuarter = this.reportSchedulerForm.controls.quarter.value;
        break;
      }

    }
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){
    let _x = timeObj.split(":")[0];
    let _y = timeObj.split(":")[1];
    if(this.prefTimeFormat == 12){
      if(_y.split(' ')[1] == 'AM' && _x == 12) {
        date.setHours(0);
      }else{
        date.setHours(_x);
      }
      date.setMinutes(_y.split(' ')[0]);
    }else{
      date.setHours(_x);
      date.setMinutes(_y);
    }

    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
  }

  getDateFromDay(day: number){
    var d = new Date();
    d.setDate(d.getDate() + ((7-d.getDay())%7+day) % 7);
    return d;
  }

  reportDispatchTimeChanged(selectedTime){
    this.selectedDispatchTime = selectedTime;
    if(this.prefTimeFormat == 24){
      this.dispatchTimeDisplay = selectedTime + ':00';
    }
    else{
      this.dispatchTimeDisplay = selectedTime;
    }
    //this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
  }

  onChangeWeekDay(event){
    let weeklyEndDay: number;
    if(event.value == 0)
      weeklyEndDay = 6;
    else
      weeklyEndDay = event.value - 1;

    this.reportSchedulerForm.get('weeklyEndDay').setValue(weeklyEndDay);
  }

  onChangeBiweeklyEndDate(event){
    this.biweeklyEndDateValue = this.setStartEndDateTime(event.value, this.selectedEndTime, 'end');
    //this.reportSchedulerForm.get('biweeklyEndDate').setValue(this.biweeklyEndDateValue)
    this.reportSchedulerForm.get('biweeklyEndDay').setValue(this.weekdays.filter(item => item.id == (this.biweeklyEndDateValue.getDay()))[0].value);

    let startDate = event.value;
    startDate.setDate(startDate.getDate() - 13);
    this.biweeklyStartDateValue = this.setStartEndDateTime(startDate, this.selectedStartTime, 'start');
    this.reportSchedulerForm.get('biweeklyStartDay').setValue(this.weekdays.filter(item => item.id == (this.biweeklyStartDateValue.getDay()))[0].value);
  }

  onChangeMonth(event){
    this.selectedMonth = event.value;
    let date = new Date();
        let year = date.getFullYear();
        this.reportSchedulerForm.get('monthlyStartDate').setValue(new Date(year, this.selectedMonth, 1));
        this.reportSchedulerForm.get('monthlyEndDate').setValue(new Date(year, this.selectedMonth + 1, 0));

  }

  onChangeQuarter(event){
    this.selectedQuarter = event.value;
    let date = new Date();
    let currentMonth = date.getMonth();
    let year = date.getFullYear();

    if(this.selectedQuarter == 0){
      this.quarterlylyStartDateValue = new Date(year, 0 /*January*/ , 1);
      this.quarterlyEndDateValue = new Date(year, 3, 0);
    }
    else if(this.selectedQuarter == 1){
      this.quarterlylyStartDateValue = new Date(year, 3 /*April*/ , 1);
      this.quarterlyEndDateValue = new Date(year, 6, 0);
    }
    else if(this.selectedQuarter == 2){
      this.quarterlylyStartDateValue = new Date(year, 6 /*July*/ , 1);
      this.quarterlyEndDateValue = new Date(year, 9, 0);
    }
    else if(this.selectedQuarter == 3){
      this.quarterlylyStartDateValue = new Date(year, 9 /*October*/ , 1);
      this.quarterlyEndDateValue = new Date(year, 12, 0);
    }
    
  }

  onChangeReportType(event){
    this.showDriverList = this.ReportTypeList.filter(item => item.id == event.value)[0].isDriver == 'Y' ? true : false;
  }

  onChangeVehicleGroup(event){
    this.VehicleList = this.reportSchedulerParameterData["associatedVehicle"].filter(item => item.vehicleGroupId == event.value);
  }

  validateRecipientEmails(event : any){
    this.only10Emails= false;
    this.isInvalidEmail= false;
    this.invalidEmail = '';
    this.recipientEmailList = event.target.value.split(",");
    if(this.recipientEmailList.length <= 10){
      let pattern=/[a-zA-Z0-9-_.]{1,}@[a-zA-Z0-9-_.]{2,}[.]{1}[a-zA-Z]{2,}/
      this.recipientEmailList.forEach(element => {
      if(!pattern.test(element.trim())){
        this.isInvalidEmail = true;    
        this.invalidEmail += element +",";
        return;
      }
    });
    }
    else{
      this.only10Emails = true;
    }
    
  }

  onCreateUpdate(){
    let startDate: any, endDate: any, nextScheduledRunDate: any;
    let scheduledReport= [];
    let scheduledReportRecipient= [];
    let scheduledReportVehicleRef= [];
    let scheduledReportDriverRef= [];

    switch(this.selectionTab){
      case 'D': {
        startDate = Util.convertDateToUtc(this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start'));
        endDate = Util.convertDateToUtc(this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end'));

        let nextDate= new Date();
        nextDate.setDate(nextDate.getDate()+1);
        nextScheduledRunDate = Util.convertDateToUtc(this.setStartEndDateTime(nextDate, this.reportSchedulerForm.controls.reportDispatchTime.value+":00", 'start'));
        break;
      }
      case 'W': {
        let startDateFromDay = this.getDateFromDay(this.reportSchedulerForm.controls.weeklyStartDay.value);
        startDate = Util.convertDateToUtc(this.setStartEndDateTime(startDateFromDay, this.selectedStartTime, 'start'));

        let endDateFromDay = startDateFromDay;
        endDateFromDay.setDate(endDateFromDay.getDate()+6);
        endDate = Util.convertDateToUtc(this.setStartEndDateTime(endDateFromDay, this.selectedEndTime, 'end'));

        let nextDateFromDay= endDateFromDay;
        nextDateFromDay.setDate(nextDateFromDay.getDate()+1);
        nextScheduledRunDate = Util.convertDateToUtc(this.setStartEndDateTime(nextDateFromDay, this.selectedStartTime, 'start'));
        break;
      }
      case 'B': {
        startDate = Util.convertDateToUtc(this.setStartEndDateTime(this.reportSchedulerForm.controls.biweeklyStartDate.value, this.selectedStartTime, 'start'));
        endDate = Util.convertDateToUtc(this.setStartEndDateTime(this.reportSchedulerForm.controls.biweeklyEndDate.value, this.selectedEndTime, 'end'));

        let nextDate= this.reportSchedulerForm.controls.biweeklyEndDate.value;
        nextDate.setDate(nextDate.getDate()+1);
        nextScheduledRunDate = Util.convertDateToUtc(this.setStartEndDateTime(nextDate, this.reportSchedulerForm.controls.reportDispatchTime.value+":00", 'start'));
        break;
      }
      case 'M': {
        startDate = Util.convertDateToUtc(this.setStartEndDateTime(this.reportSchedulerForm.controls.monthlyStartDate.value, this.selectedStartTime, 'start'));
        endDate = Util.convertDateToUtc(this.setStartEndDateTime(this.reportSchedulerForm.controls.monthlyEndDate.value, this.selectedEndTime, 'end'));

        let nextDate= this.reportSchedulerForm.controls.monthlyEndDate.value;
        nextDate.setDate(nextDate.getDate()+1);
        nextScheduledRunDate = Util.convertDateToUtc(this.setStartEndDateTime(nextDate, this.reportSchedulerForm.controls.reportDispatchTime.value+":00", 'start'));
        break;
      }
      case 'Q': {
        startDate = Util.convertDateToUtc(this.setStartEndDateTime(this.reportSchedulerForm.controls.quarterlyStartDate.value, this.selectedStartTime, 'start'));
        endDate = Util.convertDateToUtc(this.setStartEndDateTime(this.reportSchedulerForm.controls.quarterlyEndDate.value, this.selectedEndTime, 'end'));

        let nextDate= this.reportSchedulerForm.controls.quarterlyEndDate.value;
        nextDate.setDate(nextDate.getDate()+1);
        nextScheduledRunDate = Util.convertDateToUtc(this.setStartEndDateTime(nextDate, this.reportSchedulerForm.controls.reportDispatchTime.value+":00", 'start'));
        break;
      }

    }
    if(this.actionType == 'create'){ //-- create schedule
      let scheduledReportObj = 
        {
          "id": 0,
          "scheduleReportId": 0,
          "report": 0,
          "token": "",
          "startDate": 0,
          "endDate": 0,
          "downloadedAt": 0,
          "validTill": 0,
          "createdAt": 0
        }
      scheduledReport.push(scheduledReportObj);

      this.recipientEmailList.forEach(element => {
        let scheduledReportRecipientObj = 
        {
          "id": 0,
          "scheduleReportId": 0,
          "email": element,
          "state": "A",
          "createdAt": 0,
          "modifiedAt": 0
        }
        scheduledReportRecipient.push(scheduledReportRecipientObj);
      });
     

      let scheduledReportVehicleRef = [
        {
          "scheduleReportId": 0,
          "vehicleGroupId": this.reportSchedulerForm.controls.vehicleGroup.value,
          "vehicleId": this.reportSchedulerForm.controls.vehicle.value,
          "state": "A",
          "createdAt": 0,
          "createdBy": this.accountId,
          "modifiedAt": 0,
          "modifiedBy": 0
        }
      ]

      let scheduledReportDriverRef = [
        {
          "scheduleReportId": 0,
          "driverId": this.reportSchedulerForm.controls.driver.value,
          "state": "A",
          "createdAt": 0,
          "createdBy": this.accountId,
          "modifiedAt": 0,
          "modifiedBy": 0
        }
      ]

      let createObj: any = {
        "id": 0,
        "organizationId": this.accountOrganizationId,
        "reportId": this.reportSchedulerForm.controls.reportType.value,
        "frequencyType": this.selectionTab,
        "status": "A",
        "type": "",
        "startDate": startDate,
        "endDate": endDate,
        "code": this.reportSchedulerForm.controls.language.value,
        "lastScheduleRunDate": 0,
        "nextScheduleRunDate": nextScheduledRunDate,
        "createdAt": 0,
        "createdBy": this.accountId,
        "modifiedAt": 0,
        "modifiedBy": 0,
        "mailSubject": this.reportSchedulerForm.controls.mailSubject.value,
        "mailDescription": this.reportSchedulerForm.controls.mailDescription.value,
        "reportDispatchTime": this.reportSchedulerForm.controls.reportDispatchTime.value,
        "scheduledReport": scheduledReport,
        "scheduledReportRecipient": scheduledReportRecipient,
        "scheduledReportVehicleRef": scheduledReportVehicleRef,
        "scheduledReportDriverRef": scheduledReportDriverRef
      }
      
      this.reportSchedulerService.createReportScheduler(createObj).subscribe((createdData: any) => {
        if(createdData){
          this.scheduleCreatedMsg = this.getScheduleCreatedUpdatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.scheduleCreatedMsg };
          this.backToPage.emit(emitObj);
         }
       }, (error) => {
        
       });
    }else{ //-- update category
      let updateObj: any = {
      }
      // this.reportSchedulerService.updateLandmarkCategory(updateObj).subscribe((updatedData: any) => {
      //   if(updatedData){
          this.scheduleCreatedMsg = this.getScheduleCreatedUpdatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.scheduleCreatedMsg };
          this.backToPage.emit(emitObj);
      //   }
      // }, (error) => {
        
      // });
    }
  }

}
