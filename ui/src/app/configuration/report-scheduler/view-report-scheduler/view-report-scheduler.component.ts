import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import * as moment from 'moment';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportSchedulerService } from 'src/app/services/report.scheduler.service';
import { TranslationService } from 'src/app/services/translation.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-view-report-scheduler',
  templateUrl: './view-report-scheduler.component.html',
  styleUrls: ['./view-report-scheduler.component.less']
})
export class ViewReportSchedulerComponent implements OnInit {

  @Input() translationData: any = {};
  @Input() selectedRowData: any;
  @Input() prefTimeFormat: any;
  @Input() prefTimeZone: any;
  @Input() prefDateFormat: any;
  @Input() completePrefData: any;
  @Output() backToPage = new EventEmitter<any>();
  @Output() editReportSchedule = new EventEmitter<any>();
  @Input() adminAccessType : any;

  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  startDate: any;
  endDate: any;
  month: any;
  quarter: any;
  weekdays= [];
  months= [];
  language: string= "";
  vehicleGroupName: string= "";
  vehicleName: string= "";
  accountPrefObj: any;
  localStLanguage: any;
  accountOrganizationId: any;
  languageCodeList: any;
  displayedColumns= ['reportName', 'startDate', 'endDate', 'action'];
  scheduledReportList: any= [];
  dataSource: any; 
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  breadcumMsg: string = "";

  constructor(private translationService: TranslationService,
              private organizationService: OrganizationService,
              private reportSchedulerService: ReportSchedulerService,
              private dialogService: ConfirmDialogService) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.languageCodeList = JSON.parse(localStorage.getItem('languageCodeList'))
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));

    this.weekdays= [{id : 0, value : 'Sunday'},{id : 1, value : 'Monday'},{id : 2, value : 'Tuesday'},{id : 3, value : 'Wednesday'},{id : 4, value : 'Thursday'},{id : 5, value : 'Friday'},{id : 6, value : 'Saturday'}];
    this.months= [{id : 0, value : 'January'},{id : 1, value : 'February'},{id : 2, value : 'March'},{id : 3, value : 'April'},{id : 4, value : 'May'},{id : 5, value : 'June'},
                  {id : 6, value : 'July'},{id : 7, value : 'August'},{id : 8, value : 'September'},{id : 9, value : 'October'},{id : 10, value : 'November'},{id : 11, value : 'December'}]

    this.breadcumMsg = this.getBreadcum();

    //this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
      // if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
      //   this.proceedStep(prefData, this.accountPrefObj.accountPreference);
      // }else{ // org pref
      //   this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
      //     this.proceedStep(prefData, orgPref);
      //   }, (error) => { // failed org API
      //     let pref: any = {};
      //     this.proceedStep(prefData, pref);
      //   });
      // }
      
      this.timeRangeSelection(this.selectedRowData[0].frequencyType);
      let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
      if(vehicleDisplayId) {
        let vehicledisplay = this.completePrefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        if(vehicledisplay.length != 0) {
          this.vehicleDisplayPreference = vehicledisplay[0].name;
         // this.vehicleName = this.selectedRowData[0].vehicleGroupAndVehicleList != "" ? this.selectedRowData[0].scheduledReportVehicleRef.length == 0 ? this.translationData.lblAll : this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName' ? this.selectedRowData[0].scheduledReportVehicleRef[0].vehicleName : this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber' ? this.selectedRowData[0].scheduledReportVehicleRef[0].vin : this.selectedRowData[0].scheduledReportVehicleRef[0].regno : this.translationData.lblAll;
         this.vehicleName = this.selectedRowData[0].scheduledReportVehicleRef.length == 0 || this.selectedRowData[0].scheduledReportVehicleRef.length > 1 ? 0 : this.selectedRowData[0].scheduledReportVehicleRef[0].vehicleName;
         this.vehicleName = (this.vehicleName == '0' || this.vehicleName == '' )? this.translationData.lblAll : this.vehicleName ;       
        }
      }  

    // }, error => {
    //   this.timeRangeSelection(this.selectedRowData[0].frequencyType);
    // });

    this.language = this.languageCodeList.filter(item => item.code == (this.selectedRowData[0].code).trim())[0].name;
    this.vehicleGroupName = this.selectedRowData[0].vehicleGroupAndVehicleList != "" ? this.selectedRowData[0].scheduledReportVehicleRef.length == 0 ? this.translationData.lblAll : this.selectedRowData[0].scheduledReportVehicleRef[0].vehicleGroupName: this.translationData.lblAll;
    this.vehicleGroupName =  this.vehicleGroupName == '' ? this.translationData.lblAll : this.vehicleGroupName ;       

    // this.vehicleName = this.selectedRowData[0].vehicleGroupAndVehicleList != "" ? this.selectedRowData[0].scheduledReportVehicleRef.length == 0 ? "ALL" : this.vehicleDisplayPreference == 'dvehicledisplay_VehicleName' ? this.selectedRowData[0].scheduledReportVehicleRef[0].vehicleName : this.vehicleDisplayPreference == 'dvehicledisplay_VehicleIdentificationNumber' ? this.selectedRowData[0].scheduledReportVehicleRef[0].vin : this.selectedRowData[0].scheduledReportVehicleRef[0].regno : "ALL";

    this.getScheduledReportList(); // new changes

    // this.scheduledReportList = this.selectedRowData[0].scheduledReport;
    // this.scheduledReportList.forEach(element => {
    //   element.reportName = this.selectedRowData[0].reportName;
    //   element.startDate = Util.convertUtcToDateFormat(element.startDate, this.prefDateFormat+"  HH:mm:ss", this.prefTimeZone);
    //   element.endDate = Util.convertUtcToDateFormat(element.endDate, this.prefDateFormat+"  HH:mm:ss",  this.prefTimeZone)
    // });
    // this.updateDatasource();

    //  this.onDownloadReport({reportName : "Trip Report", id : 128, startDate : "07/07/2021 12:0:0"});
  }

  getScheduledReportList(){
    this.reportSchedulerService.getScheduledReportList(this.selectedRowData[0].id).subscribe((_list: any) => {
      if(_list){
        this.scheduledReportList = _list.scheduledReport;
        this.scheduledReportList.forEach(element => {
          element.reportName = this.selectedRowData[0].reportName;
          element.startDate = Util.convertUtcToDateFormat(element.startDate, this.prefDateFormat+"  HH:mm:ss", this.prefTimeZone);
          element.endDate = Util.convertUtcToDateFormat(element.endDate, this.prefDateFormat+"  HH:mm:ss",  this.prefTimeZone)
        });
        this.updateDatasource();
      }
    }, (error) => {
      //console.log(error);
    });
  }

  updateDatasource(){
    this.dataSource = new MatTableDataSource(this.scheduledReportList);
    // this.dataSource.filterPredicate = function(data: any, filter: string): boolean {
    //   return (
    //     data.reportName.toString().toLowerCase().includes(filter) ||
    //     data.recipientList.toString().toLowerCase().includes(filter) ||
    //     data.driverList.toString().toLowerCase().includes(filter) ||
    //     data.status.toString().toLowerCase().includes(filter) 
    //   );
    // };
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
    }else{
      //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      //this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
    }
    this.setPrefFormatDate();
  }

  setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.prefDateFormat = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.prefDateFormat = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.prefDateFormat = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.prefDateFormat = "MM-DD-YYYY";
        break;
      }
      default:{
        this.prefDateFormat = "MM/DD/YYYY";
      }
    }
  }


  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblReportScheduler : "Report Scheduler"} / 
    ${this.translationData.lblViewScheduleDetails ? this.translationData.lblViewScheduleDetails : 'View Schedule Details'}`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  editReport(){
    this.editReportSchedule.emit();
  }

  onDeleteReportScheduler() {
    const options = {
      title: this.translationData.lblDeleteReportScheduler || "Delete Report Scheduler",
      message: this.translationData.lblAreousureyouwanttodeletescheduledreport || "Are you sure you want to delete this scheduled report  '$' ? ",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    let name = this.selectedRowData[0].reportName;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.reportSchedulerService.deleteScheduledReport(this.selectedRowData[0].id).subscribe((res) => {
          let emitObj = {
            stepFlag: false,
            successMsg: this.getDeletMsg(name)
          }  
          this.backToPage.emit(emitObj);
        }, error => {        });
    }
   });
  }

  getDeletMsg(reportSchedulerName: any){
    if(this.translationData.lblReportSchedulerDelete)
      return this.translationData.lblReportSchedulerDelete.replace('$', reportSchedulerName);
    else
      return ("Scheduled '$' deleted successfully ").replace('$', reportSchedulerName);
}

  timeRangeSelection(timeRange){
    switch(timeRange){
      case 'D' : {
        this.startDate= Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone);
        let _h= this.startDate.getHours();
        let _m= this.startDate.getMinutes();
        let _s= this.startDate.getSeconds();
        this.startDate= (_h > 10 ? _h : "0"+_h) + ":" + (_m > 10 ? _m : "0"+_m) + ":" + (_s > 10 ? _s : "0"+_s);
        this.endDate= Util.convertUtcToDateNoFormat(this.selectedRowData[0].endDate, this.prefTimeZone);
        this.endDate= this.endDate.getHours()+":"+this.endDate.getMinutes()+":"+this.endDate.getSeconds();
        this.startDate= "00:00:00";
        this.endDate= "23:59:59";
        break;
      }
      case 'W' : {
        this.startDate = this.weekdays.filter(item => item.id == (Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone).getDay()))[0].value;
        this.endDate= new Date(Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone));
          this.endDate.setDate(this.endDate.getDate() + 6);
        this.endDate = this.weekdays.filter(item => item.id == (this.endDate).getDay())[0].value;
        break;
      }
      case 'B' : {
        // this.endDate=  Util.convertUtcToDateFormat(this.selectedRowData[0].endDate, this.prefDateFormat, this.prefTimeZone);
        this.endDate= Util.convertUtcToDateFormat(this.selectedRowData[0].endDate, this.prefDateFormat, this.prefTimeZone);
        this.startDate= new Date(Util.convertUtcToDateNoFormat(this.selectedRowData[0].endDate, this.prefTimeZone));
        this.startDate.setDate(this.startDate.getDate() - 13);
        this.startDate= moment(this.startDate).format(this.prefDateFormat);
        break;
      }
      case 'M' : {
        this.month = this.months.filter(item => item.id == (Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone)).getMonth())[0].value;
      }
      case 'Q' : {
        let convertedDate = new Date(this.selectedRowData[0].startDate);
        let currentMonth = convertedDate.getMonth();

        if(currentMonth >=0 && currentMonth<=2){
          this.quarter= "Quarter1 (Jan-Mar)";
        }
        else if(currentMonth >=3 && currentMonth<=5){
          this.quarter= "Quarter2 (Apr-Jun)";
        }
        else if(currentMonth >=6 && currentMonth<=8){
          this.quarter= "Quarter3 (Jul-Sept)";
        }
        else if(currentMonth >=9 && currentMonth<=11){
          this.quarter= "Quarter4 (Oct-Dec)";
        }
      }
    }
    
  }

  onDownloadReport(row){
    this.reportSchedulerService.downloadReport(row.id).subscribe(response => {
      let arrayBuffer= response["report"];
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      const linkSource = 'data:application/pdf;base64,' + base64File;
      const downloadLink = document.createElement("a");
      const fileName = response["fileName"]+".pdf";

      downloadLink.href = linkSource;
      downloadLink.download = fileName;
      downloadLink.click();
    }, (error) => {
      //this.downloadPDFErrorCode= error.status;
    });
  }

}
