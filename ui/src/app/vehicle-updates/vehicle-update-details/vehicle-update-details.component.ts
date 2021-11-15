import { Component, EventEmitter,Inject, Input, OnChanges, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ReleaseNoteComponent } from './release-note/release-note.component';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { OtaSoftwareUpdateService } from 'src/app/services/ota-softwareupdate.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { MdePopoverTrigger } from '@material-extended/mde';
import { Util } from '../../shared/util';
import * as moment from 'moment-timezone';
import { ScheduleConfirmComponent } from './schedule-confirm/schedule-confirm.component';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { OrganizationService } from '../../services/organization.service';



@Component({
  selector: 'app-vehicle-update-details',
  templateUrl: './vehicle-update-details.component.html',
  styleUrls: ['./vehicle-update-details.component.less'],
  encapsulation: ViewEncapsulation.None
})
export class VehicleUpdateDetailsComponent implements OnInit, OnChanges {
  public selectedIndex: number = 0;
  dataSource: any;
  displayedColumns: string[] = ['campaignId', 'subject', 'affectedSystem(s)', 'type', 'category', 'status', 'endDate', 'scheduledDateTime', 'action'];
  translationData: any = {};
  localStLanguage: any;
  initData: any = [];
  showLoadingIndicator: boolean = false;
  accountOrganizationId: any = 0;
  accountOrganizationSetting: any;
  breadcumMsg: any = '';
  selectedVin: any;
  selectedVehicalName: any;
  openSchedulerFlag: boolean = false;
  backdropClose: boolean = false;
  campaignOverFlag: boolean = false;
  selectedScheduledTime: any = '12:00 AM'
  public schedulerForm: FormGroup;
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  scheduledTime: any;
  scheduledDate: any;
  prefTimeFormat: any = 12; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_dd/mm/yyyy'; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  schedulerData: any ={
    campaignName: "",
    vehicalName: "",
    baseLineId: "",
    scheduleDateTime: "",
    vin: '',
    campaignID: '',
    scheduledDate:'',
    scheduledTime:''

  }
  today= new Date();
  @Output() backToPage = new EventEmitter<any>();
  @Input() selectedVehicleUpdateDetailsData: any;
  @Input() selectedVehicleUpdateDetails: any;
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @ViewChild(MdePopoverTrigger) trigger: MdePopoverTrigger;
  @Input('mdePopoverPositionX') positionX;
  @Input() prefDefaultTimeFormat: any;
  @Input() prefDefaultDateFormat: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dialogRef: MatDialogRef<ReleaseNoteComponent>;
  dialogRefConfirm: MatDialogRef<ScheduleConfirmComponent>;
  accountId: number;
  accountRoleId: number;
  accountPrefObj: any;
  selectedStartTime: string;
  startDateValue: any;
  todayDate: any;
  titleVisible : boolean = false;
  displayMessage : any = '';
  formattedDate: any;
  adminRight:boolean;
  
 
  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats,private translationService: TranslationService, public fb: FormBuilder, private dialog: MatDialog, private dialogService: ConfirmDialogService,
    private otaSoftwareService: OtaSoftwareUpdateService, private organizationService: OrganizationService) {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
  }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 17 //-- for alerts
    }
    this.schedulerForm = this.fb.group({
      date: [''],
      time: ['']
    });
    this.breadcumMsg = this.getBreadcum();    
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
        if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        } else { // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }  
      });
    });
  }
  proceedStep(prefData: any, preference: any) {
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
    } else {
      //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
  }

  ngOnChanges() {
    this.loadVehicleDetailsData(this.selectedVehicleUpdateDetailsData);
  }

  setDefaultStartEndTime() {
    this.setPrefFormatTime();
  }

  setPrefFormatTime() {
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = '00:00:00';
      this.selectedStartTime = "00:00";
    } else {
      this.startTimeDisplay = '12:00:00 AM';
      this.selectedStartTime = "12:00 AM";
    
    }
  }

  setDefaultTodayDate() {
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.todayDate = this.getTodayDate();
  }

  getTodayDate() {
    let todayDate = new Date(); //-- UTC
    return todayDate;
  }
  
  setPrefFormatDate() {
    switch (this.prefDateFormat) {
      case 'dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        break;
      }
      default: {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
      }
    }
  }
  formStartDate(date: any, prefTimeFormat: any, prefDateFormat:any) {
    let h = (date.getHours() < 10) ? ('0' + date.getHours()) : date.getHours();
    let m = (date.getMinutes() < 10) ? ('0' + date.getMinutes()) : date.getMinutes();
    let s = (date.getSeconds() < 10) ? ('0' + date.getSeconds()) : date.getSeconds();
    let _d = (date.getDate() < 10) ? ('0' + date.getDate()) : date.getDate();
    let _m = ((date.getMonth() + 1) < 10) ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1);
    let _y = (date.getFullYear() < 10) ? ('0' + date.getFullYear()) : date.getFullYear();
    let _date: any;
    let _time: any;
    if (prefTimeFormat == 12) {
      if (date.getHours() == 12) {
        _time = ((date.getHours() == 12 || date.getMinutes() > 0 || date.getSeconds() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours() - 12}:${m}:${s} PM` : `${(date.getHours() == 0) ? 12 : h}:${m}:${s} AM`;
      }
      else {
        _time = (date.getHours() > 12 || (date.getHours() == 12 && date.getMinutes() > 0 && date.getSeconds() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours() - 12}:${m}:${s} PM` : `${(date.getHours() == 0) ? 12 : h}:${m}:${s} AM`;
      }
    }
    else {
      _time = `${h}:${m}:${s}`;
    }
    switch (prefDateFormat) {
      case 'ddateformat_dd/mm/yyyy': {
        _date = `${_d}/${_m}/${_y} ${_time}`;
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        _date = `${_m}/${_d}/${_y} ${_time}`;
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        _date = `${_d}-${_m}-${_y} ${_time}`;
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        _date = `${_m}-${_d}-${_y} ${_time}`;
        break;
      }
      default: {
        _date = `${_m}/${_d}/${_y} ${_time}`;
      }
    }
    return _date;
  }


  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  loadVehicleDetailsData(selectedVehicleUpdateDetailsData: any) {
    this.showLoadingIndicator = true;
    if (selectedVehicleUpdateDetailsData) {
      var todaysDate = moment();
      todaysDate = Util.convertUtcToDateFormat(todaysDate,this.dateFormats.display.dateInput, this.prefTimeZone);
      selectedVehicleUpdateDetailsData.campaigns.forEach(element => {
        if (element.endDate) {        
          element.endDate = Util.convertUtcToDateFormat(element.endDate,this.dateFormats.display.dateInput, this.prefTimeZone);
         if(moment(element.endDate).isBefore(todaysDate)){
            element.campaignOverFlag = true;
         }
        } else {
          element.campaignOverFlag = false;
          element.endDate = '-';
        }
        if (element.scheduleDateTime) {
          element.scheduleDateTime = new Date( element.scheduleDateTime);
         // console.log('Default:',this.prefDefaultTimeFormat, this.prefDefaultDateFormat);
          element.scheduleDateTime = this.formStartDate(element.scheduleDateTime, this.prefDefaultTimeFormat, this.prefDefaultDateFormat);
        } else {
          element.scheduleDateTime = '-';
        }

      });
      this.initData = selectedVehicleUpdateDetailsData.campaigns;
      this.selectedVin = this.selectedVehicleUpdateDetails.vin;
      this.adminRight = this.selectedVehicleUpdateDetails.isAdminRight;
      this.selectedVehicalName = this.selectedVehicleUpdateDetails.vehicleName;
      this.updateDataSource(this.initData);
    }
    this.hideloader();
  }

  updateDataSource(tableData: any) {
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }
  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} /  
    ${this.translationData.lblVehicleUpdate ? this.translationData.lblVehicleUpdate : 'Vehical Updates'} / 
    ${this.translationData.lblVehicleUpdateDetails ? this.translationData.lblVehicleUpdateDetails : 'Vehical Update Details'}`;
  }


  toBack() {
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.backToPage.emit(emitObj);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  
  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  releaseNote(releaseNoteData: any) { //--- single opt-in/out mode
    this.showReleaseNoteDailog(releaseNoteData, this.selectedVin);
  }

  showReleaseNoteDailog(releaseNoteData: any, vin) {
    const dialogReleaseNote = new MatDialogConfig();
    dialogReleaseNote.disableClose = true;
    dialogReleaseNote.autoFocus = true;
    this.otaSoftwareService.getsoftwarereleasenotes(releaseNoteData, vin).subscribe((value: any) => {
      dialogReleaseNote.data = {
        translationData: this.translationData,
        releaseNoteData: releaseNoteData,
        vin: vin,
        message: value.message,
        releaseNotes: value.releaseNotes
      }
      this.dialogRef = this.dialog.open(ReleaseNoteComponent, dialogReleaseNote);

    }, (error) => {
      this.hideloader();
      console.log("error:: ", error)
    });
  }

  openScheduler(rowData: any) { 
  this.schedulerData.campaignName = rowData.campaignSubject;
  this.schedulerData.baseLineId = rowData.baselineAssignmentId;
  this.schedulerData.campaignID = rowData.campaignID;
  this.schedulerData.vin = this.selectedVin;
  }
 
  changeScheduleDateEvent (event: MatDatepickerInputEvent<any>) {
    this.scheduledDate = this.setStartEndDateTime(event.value._d, this.scheduledTime, 'start');
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    let _x = timeObj.split(":")[0];
    let _y = timeObj.split(":")[1];
    if (this.prefTimeFormat == 12) {
      if(_y.split(' ')[1] == 'AM'){
        if (_x == 12) {
          date.setHours(0);
        } else {
          date.setHours(_x);
        }
      }
      else if(_y.split(' ')[1] == 'PM'){               
         if(_x != 12){
           date.setHours(parseInt(_x) + 12);
         }
         else{
          date.setHours(_x);
         }
      }     
      date.setMinutes(_y.split(' ')[0]);
    } else {
      date.setHours(_x);
      date.setMinutes(_y);
    }

    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

  
  startTimeChanged(selectedTime: any) {
    this.trigger.openPopover();
    if(selectedTime){
      this.scheduledTime = selectedTime;
    }
  }

  onSubmitScheduler(){
    this.trigger.closePopover();
    this.showConfirmDailog(this.schedulerData);
   
}

onCancel(){
  this.trigger.closePopover();
  // this.trigger.destroyPopover();
}
showConfirmDailog(schedulerData: any) {
  let scheduledDateTime = this.scheduledDate +  this.scheduledTime;
  this.formattedDate = moment(this.scheduledDate).format(this.dateFormats.display.dateInput).toString();
  const isoDate = moment(this.formattedDate +' '+ this.scheduledTime).toISOString();
  this.schedulerData.scheduleDateTime = isoDate;
  const dialogScheduler = new MatDialogConfig();
  dialogScheduler.disableClose = true;
  dialogScheduler.autoFocus = true;
  dialogScheduler.data = {
      translationData: this.translationData,
      campaignName: schedulerData.campaignName,
      vehicalName: this.selectedVehicalName,
      baseLineId: schedulerData.baseLineId,
      scheduleDateTime: isoDate,
      scheduledDate:this.formattedDate,
      scheduledTime:this.scheduledTime
    }

    this.dialogRefConfirm = this.dialog.open(ScheduleConfirmComponent, dialogScheduler);
    this.dialogRefConfirm.afterClosed().subscribe(res => {
      this.showLoadingIndicator = true;
      if(res){ 
        this.otaSoftwareService.getschedulesoftwareupdate(this.schedulerData).subscribe((sheduleData: any) => {
          // this.hideloader();
          let successMsg =`${this.schedulerData.campaignID} ${this.formattedDate} ${this.scheduledTime} scheduled successfully.`
          this.successMsgBlink(successMsg);
          this.otaSoftwareService.getvehicleupdatedetails(this.selectedVehicleUpdateDetails.vin).subscribe((data: any) =>{
            this.hideloader();
            if (data  && data.vehicleUpdateDetails && data.vehicleUpdateDetails !== null) {
              this.loadVehicleDetailsData(data)
            }

          }, (error) => {
            this.hideloader();
          })
          

        }, (error) => {
          this.hideloader();
          // let successMsg =`${this.schedulerData.campaignId} ${this.formattedDate} ${this.scheduledTime} scheduled successfully.`
          // this.successMsgBlink(successMsg);
          console.log("error:: ", error);
        });
      }
    });
  }
  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }
  onClose() {
    this.titleVisible = false;
  }


}
