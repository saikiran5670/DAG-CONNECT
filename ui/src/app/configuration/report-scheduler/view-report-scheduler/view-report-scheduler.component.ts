import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportSchedulerService } from 'src/app/services/report.scheduler.service';
import { TranslationService } from 'src/app/services/translation.service';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-view-report-scheduler',
  templateUrl: './view-report-scheduler.component.html',
  styleUrls: ['./view-report-scheduler.component.less']
})
export class ViewReportSchedulerComponent implements OnInit {

  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Output() backToPage = new EventEmitter<any>();

  startDate: any;
  endDate: any;
  month: any;
  quarter: any;
  weekdays= [];
  months= [];
  language: string= "";
  vehicleGroupName: string= "";
  vehicleName: string= "";
  prefTimeFormat: any= 24; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'DD/MM/YYYY'; //-- coming from pref setting
  accountPrefObj: any;
  localStLanguage: any;
  accountOrganizationId: any;
  languageCodeList: any;
  displayedColumns= ['reportName', 'startDate', 'endDate', 'action'];
  scheduledReportList: any= [];
  dataSource: any; 
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  breadcumMsg: string= ""

  constructor(private translationService: TranslationService,
              private organizationService: OrganizationService,
              private reportSchedulerService: ReportSchedulerService) { }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.languageCodeList = JSON.parse(localStorage.getItem('languageCodeList'))
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));

    this.weekdays= [{id : 0, value : 'Sunday'},{id : 1, value : 'Monday'},{id : 2, value : 'Tuesday'},{id : 3, value : 'Wednesday'},{id : 4, value : 'Thursday'},{id : 5, value : 'Friday'},{id : 6, value : 'Saturday'}];
    this.months= [{id : 0, value : 'January'},{id : 1, value : 'February'},{id : 2, value : 'March'},{id : 3, value : 'April'},{id : 4, value : 'May'},{id : 5, value : 'June'},
                  {id : 6, value : 'July'},{id : 7, value : 'August'},{id : 8, value : 'September'},{id : 9, value : 'October'},{id : 10, value : 'November'},{id : 11, value : 'December'}]

    this.breadcumMsg= this.getBreadcum();

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
      
      this.timeRangeSelection(this.selectedRowData[0].frequencyType);

    }, error => {
      this.timeRangeSelection(this.selectedRowData[0].frequencyType);
    });

    this.language= this.languageCodeList.filter(item => item.code == (this.selectedRowData[0].code).trim())[0].name;
    this.vehicleGroupName= this.selectedRowData[0].vehicleGroupAndVehicleList != "" ? "ALL" : (this.selectedRowData[0].scheduledReportVehicleRef.length == 0 ? "ALL" : this.selectedRowData[0].scheduledReportVehicleRef[0].vehicleGroupName)
    this.vehicleName= this.selectedRowData[0].vehicleGroupAndVehicleList != "" ? "ALL" : (this.selectedRowData[0].scheduledReportVehicleRef.length == 0 ? "ALL" : this.selectedRowData[0].scheduledReportVehicleRef[0].vin)

    this.scheduledReportList= this.selectedRowData[0].scheduledReport;
    this.scheduledReportList.forEach(element => {
      element.reportName=this.selectedRowData[0].reportName;
      element.startDate= Util.convertUtcToDateFormat(element.startDate, this.prefDateFormat+"  hh:mm:ss");
      element.endDate= Util.convertUtcToDateFormat(element.endDate, this.prefDateFormat+"  hh:mm:ss")
    });

    this.updateDatasource();

    // this.onDownloadReport({reportName : "Trip Report", scheduleReportId : 121, startDate : "07/07/2021 12:0:0"});
    
  }

  updateDatasource(){
    // this.scheduledReportList = data;
   
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
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
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
    ${this.translationData.lblLandmarks ? this.translationData.lblReportScheduler : "ReportScheduler"} / 
    ${this.translationData.lblViewScheduleDetails ? this.translationData.lblViewScheduleDetails : 'View Schedule Details'}`;
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  timeRangeSelection(timeRange){
    switch(timeRange){
      case 'D' : {
        // let start= Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone);
        // this.startDate= start.getHours()+":"+start.getMinutes()+":"+start.getSeconds();

        // let end= Util.convertUtcToDateNoFormat(this.selectedRowData[0].endDate, this.prefTimeZone);
        // this.endDate= end.getHours()+":"+end.getMinutes()+":"+end.getSeconds();
        this.startDate= new Date(this.selectedRowData[0].startDate);
        this.startDate= this.startDate.getHours()+":"+this.startDate.getMinutes()+":"+this.startDate.getSeconds();
        this.endDate= new Date(this.selectedRowData[0].endDate);
        this.endDate= this.endDate.getHours()+":"+this.endDate.getMinutes()+":"+this.endDate.getSeconds();
        // this.startDate= "00:00:00";
        // this.endDate= "23:59:59";
        break;
      }
      case 'W' : {
        this.startDate = this.weekdays.filter(item => item.id == (Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone).getDay()))[0].value;
        this.endDate = this.weekdays.filter(item => item.id == (Util.convertUtcToDateNoFormat(this.selectedRowData[0].endDate, this.prefTimeZone).getDay()))[0].value;
        break;
      }
      case 'B' : {
        this.startDate= Util.convertUtcToDateFormat(this.selectedRowData[0].startDate, this.prefDateFormat);
        this.endDate=  Util.convertUtcToDateFormat(this.selectedRowData[0].endDate, this.prefDateFormat);
        break;
      }
      case 'M' : {
        this.month = this.months.filter(item => item.id == (Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone)).getMonth())[0].value;
      }
      case 'Q' : {
        let currentMonth =(Util.convertUtcToDateNoFormat(this.selectedRowData[0].startDate, this.prefTimeZone)).getMonth();

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
    this.reportSchedulerService.downloadReport(row.scheduleReportId).subscribe(response => {
      let arrayBuffer= response["report"];
      var base64File = btoa(
        new Uint8Array(arrayBuffer)
          .reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      const linkSource = 'data:application/pdf;base64,' + base64File;
      const downloadLink = document.createElement("a");
      const fileName = row.reportName+"_"+row.startDate+".pdf";

      downloadLink.href = linkSource;
      downloadLink.download = fileName;
      downloadLink.click();
    }, (error) => {
      //this.downloadPDFErrorCode= error.status;
    });
  }

}
