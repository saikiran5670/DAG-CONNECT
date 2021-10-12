import { Component, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
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
  openScheduler: boolean = false;
  selectedEndTime;
  selectedStartTime: any;
  public schedulerForm: FormGroup;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  startDateValue: any;
  endDateValue: any;
  last3MonthDate: any;
  @Output() backToPage = new EventEmitter<any>();
  @Input() selectedVehicleUpdateDetailsData: any;
  @Input() selectedVehicleUpdateDetails: any;
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @ViewChild(MdePopoverTrigger, { static: false }) trigger: MdePopoverTrigger;
  @Input('mdePopoverPositionX') positionX;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dialogRef: MatDialogRef<ReleaseNoteComponent>;

  constructor(private translationService: TranslationService, public fb: FormBuilder, private dialog: MatDialog, private dialogService: ConfirmDialogService,
    private otaSoftwareService: OtaSoftwareUpdateService) {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
  }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
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
    });
  }

  ngOnChanges() {
    this.loadVehicleDetailsData(this.selectedVehicleUpdateDetailsData);
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
    if (this.selectedVehicleUpdateDetailsData) {
      this.selectedVehicleUpdateDetailsData.campaigns.forEach(element => {
        if (element.endDate) {
          var date = new Date(element.endDate).toISOString();
          element.endDate = moment(parseInt(element.endDate)).format('MM/DD/YYYY');
        } else {
          element.endDate = '-';
        }
        if (element.scheduleDateTime) {
          //  let shceduledDateTime = new Date(element.scheduleDateTime).toISOString();
          element.scheduleDateTime = moment(parseInt(element.scheduleDateTime)).format('MM/DD/YYYY HH:mm:ss');
        } else {
          element.scheduleDateTime = '-';
        }

      });
      this.initData = this.selectedVehicleUpdateDetailsData.campaigns;
      this.selectedVin = this.selectedVehicleUpdateDetails.vin;
      this.updateDataSource(this.initData);
    }
    this.hideloader();
  }

  updateDataSource(tableData: any) {
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      // this.dataSource.sortData = (data: String[], sort: MatSort) => {
      //   const isAsc = sort.direction === 'asc';
      //   return data.sort((a: any, b: any) => {
      //     return this.compare(a[sort.active], b[sort.active], isAsc);
      //   });
      //  }
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

  }
  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  releaseNote(releaseNoteData: any, vin) { //--- single opt-in/out mode
    this.showReleaseNoteDailog(releaseNoteData, vin);
  }

  showReleaseNoteDailog(releaseNoteData: any, vin: any) {
    const dialogReleaseNote = new MatDialogConfig();
    dialogReleaseNote.disableClose = true;
    dialogReleaseNote.autoFocus = true;
    this.otaSoftwareService.getsoftwarereleasenotes(releaseNoteData['campaignID'], 'XLR000000BE000080').subscribe((value: any) => {
      dialogReleaseNote.data = {
        translationData: this.translationData,
        releaseNoteData: releaseNoteData,
        vin: vin,
        message: value.message,
        releaseNotes: value.releaseNotes
      }
      this.dialogRef = this.dialog.open(ReleaseNoteComponent, dialogReleaseNote);

    });

    // this.dialogRef.afterClosed().subscribe(res => {
    //   if(res){
    //     console.log(res, 'gfhgfhjg');
    //     // if(res.tableData && res.tableData.length > 0){
    //     //   this.selectedConsentType = 'All';
    //     // if(res.consentMsg) { 
    //     //   if(dialogConfig.data.consentType == 'H' || dialogConfig.data.consentType == 'I') {
    //     //     var msg = res.tableData.length + " drivers were successfully Opted-In.";
    //     //   } else if(dialogConfig.data.consentType == 'U') {
    //     //     var msg = res.tableData.length + " drivers were successfully Opted-Out.";
    //     //   }
    //     // }
    //     // this.successMsgBlink(msg);
    //     // // if(res.consentMsg && res.consentMsg != ''){
    //     // //   this.successMsgBlink(res.consentMsg);
    //     // }
    //   }
    // });
  }
  OpenScheduler(rowData: any) { //--- single opt-in/out mode
    this.showScheduler(rowData);
    this.openScheduler = true;
  }
  showScheduler(rowData) {

  }
  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    // this.internalSelection = true;
    // this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  startTimeChanged(selectedTime: any) {
    // this.internalSelection = true;
    // this.selectedStartTime = selectedTime;
    // if (this.prefTimeFormat == 24) {
    //   this.startTimeDisplay = selectedTime + ':00';
    // }
    // else {
    //    this.startTimeDisplay = selectedTime;
    //  }
    // this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

}
