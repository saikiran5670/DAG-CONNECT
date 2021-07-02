import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportSchedulerService } from 'src/app/services/report.scheduler.service';
import { TranslationService } from 'src/app/services/translation.service';
import { CustomValidators } from 'src/app/shared/custom.validators';

@Component({
  selector: 'app-create-edit-view-report-scheduler',
  templateUrl: './create-edit-view-report-scheduler.component.html',
  styleUrls: ['./create-edit-view-report-scheduler.component.less']
})
export class CreateEditViewReportSchedulerComponent implements OnInit {

  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
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
  selectedIndex: number = 0;
  tabVisibilityStatus: boolean = true;
  selectionTab: string = 'daily';
  dailyStartTimeDisplay: any = '00:00:00';
  dailyEndTimeDisplay: any = '23:59:59';
  dispatchTimeDisplay: any = '23:59:59';
  selectedDailyStartTime: any = '00:00';
  selectedDailyEndTime: any = '23:59'; 
  selectedDispatchTime: any = '23:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  accountPrefObj: any;
  localStLanguage: any;
  weekdays: any= [];
  

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
      vehicleGroup : ['', [Validators.required]],
      language : ['', [Validators.required]],
      vehicle : ['', [Validators.required]],
      recipientEmail : ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      driver : ['', [Validators.required]],
      mailSubject : ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      mailDescription : ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      dailyStartTime : new FormControl({value: '', disabled: true}),
      dailyEndTime : new FormControl({value: '', disabled: true}),
      reportDispatchTime : ['', []],
      weeklyStartDay : new FormControl({value: 1, disabled: true}),
      weeklyEndDay : new FormControl({value: 7, disabled: true})
    });

    this.weekdays=[{id : 1, value : 'Monday'},{id : 2, value : 'Tuesday'},{id : 3, value : 'Wednesday'},{id : 4, value : 'Thursday'},{id : 5, value : 'Friday'},{id : 6, value : 'Saturday'},{id : 7, value : 'Sunday'}]
    
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
        this.dailyStartTimeDisplay = '00:00:00';
        this.dailyEndTimeDisplay = '23:59:59';
        this.dispatchTimeDisplay = '23:59:59';
        this.selectedDailyStartTime = "00:00";
        this.selectedDailyEndTime = "23:59";
        this.selectedDispatchTime = "23:59";
      } else{
        this.dailyStartTimeDisplay = '12:00 AM';
        this.dailyEndTimeDisplay = '11:59 PM';
        this.dispatchTimeDisplay = '11:59 PM';
        this.selectedDailyStartTime = "00:00";
        this.selectedDailyEndTime = "23:59";
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

  onCreateUpdate(){
    if(this.actionType == 'create'){ //-- create schedule
      let createdObj: any = {
      
      }     
      //this.reportSchedulerService.addLandmarkCategory(createdObj).subscribe((createdData: any) => {
        //if(createdData){
          this.scheduleCreatedMsg = this.getScheduleCreatedUpdatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.scheduleCreatedMsg };
          this.backToPage.emit(emitObj);
      //   }
      // }, (error) => {
        
      // });
    }else{ //-- update category
      let updatedObj: any = {
      }
      // this.reportSchedulerService.updateLandmarkCategory(updatedObj).subscribe((updatedData: any) => {
      //   if(updatedData){
          this.scheduleCreatedMsg = this.getScheduleCreatedUpdatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.scheduleCreatedMsg };
          this.backToPage.emit(emitObj);
      //   }
      // }, (error) => {
        
      // });
    }
  }

  getScheduleCreatedUpdatedMessage() {
    //let categoryName = `${this.categoryForm.controls.categoryName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblScheduleCreatedSuccessfully)
        return this.translationData.lblScheduleCreatedSuccessfully;
      else
        return ("New Report Schedule Created Successfully");
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblScheduleUpdatedSuccessfully)
        return this.translationData.lblScheduleUpdatedSuccessfully;
      else
        return ("Report Schedule Updated Successfully");
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
    
  }
 
}
