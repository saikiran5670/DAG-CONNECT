import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { OrganizationService } from '../../services/organization.service';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource } from '@angular/material/table';
import { Util } from '../../shared/util';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-measurement-campaign-status',
  templateUrl: './measurement-campaign-status.component.html',
  styleUrls: ['./measurement-campaign-status.component.less']
})
export class MeasurementCampaignStatusComponent implements OnInit {

  intendedTimeDisplay = '';
  appliedTimeDisplay = ''
  translationData: any = {};
  prefDetail: any = {};
  accountPrefObj: any;
  accountOrganizationId: any;
  showTableResult = true;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  displayedColumns: string[] = ['select', 'vin', 'intendedDateTime', 'targetSystem', 'status', 'targetSystemStatus', 'appliedDateTime', 'action'];
  selectedEcoScore = new SelectionModel(true, []);
  initData: any = [];
  dataSource: any;
  filterValue: string;
  viewPageData: any;
  viewPage = false;
  showLoadingIndicator = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  showMessage = false;
  model = {
    vin: '',
    target: '',
    status: '',
    maxset: 50,
    intendedDate: '',
    intendedTime: '',
    appliedDate:'',
    appliedTime:''
  }

  targetSystemValues = [
    'BVMS ',
    'AM2M'
  ]

  statusValues = [
    'Success',
    'Failed',
    'Intended',
    'Working'
  ]

  maxResultValues = [
    50,
    100,
    500,
    1000
  ]

  menuTranslation = [
    { 
      code: "EN-GB",
      filter: "",
      id: 1,
      menuId: 0,
      name: "lblSearchVin",
      type: "L",
      value: "Search VIN" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 2,
      menuId: 0,
      name: "lblTargetSystem",
      type: "L",
      value: "Target System" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 3,
      menuId: 0,
      name: "lblStatus",
      type: "L",
      value: "Status" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 4,
      menuId: 0,
      name: "lblMaxResultSet",
      type: "L",
      value: "Max Result Set" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 5,
      menuId: 0,
      name: "lblSearchParameters",
      type: "L",
      value: "Search Parameters" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 6,
      menuId: 0,
      name: "lblIntendedDate",
      type: "L",
      value: "Intended Date" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 7,
      menuId: 0,
      name: "lblTime",
      type: "L",
      value: "Time" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 8,
      menuId: 0,
      name: "lblAppliedDate",
      type: "L",
      value: "Applied Date" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 9,
      menuId: 0,
      name: "lblHome",
      type: "L",
      value: "Home" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 10,
      menuId: 0,
      name: "lblAdmin",
      type: "L",
      value: "Admin" 
    },

    { 
      code: "EN-GB",
      filter: "",
      id: 11,
      menuId: 0,
      name: "lblMeasurementCampaignStatus",
      type: "L",
      value: "Measurement Campaign Status" 
    },

    { 
      code: "EN-GB",
      filter: "",
      id: 11,
      menuId: 0,
      name: "lblViewDetails",
      type: "L",
      value: "View Details" 
    },
    
    { 
      code: "EN-GB",
      filter: "",
      id: 12,
      menuId: 0,
      name: "lblBack",
      type: "L",
      value: "Back" 
    },

    { 
      code: "EN-GB",
      filter: "",
      id: 13,
      menuId: 0,
      name: "lblIntendedAction",
      type: "L",
      value: "Intended Action" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 14,
      menuId: 0,
      name: "lblIntendedActionArgument",
      type: "L",
      value: "Intended Action Argument" 
    },

    { 
      code: "EN-GB",
      filter: "",
      id: 15,
      menuId: 0,
      name: "lblAppliedSystem",
      type: "L",
      value: "Applied System" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 16,
      menuId: 0,
      name: "lblAppliedAction",
      type: "L",
      value: "Applied Action" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 17,
      menuId: 0,
      name: "lblAppliedActionArgument",
      type: "L",
      value: "Applied Action Argument" 
    },

    { 
      code: "EN-GB",
      filter: "",
      id: 18,
      menuId: 0,
      name: "lblCancel",
      type: "L",
      value: "Cancel" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 19,
      menuId: 0,
      name: "lblReInterpret",
      type: "L",
      value: "Re-Interpret" 
    },
    { 
      code: "EN-GB",
      filter: "",
      id: 20,
      menuId: 0,
      name: "lblRuleReInterpret",
      type: "L",
      value: "Rule Re-Interpret" 
    }
    

  ]

  
ELEMENT_DATA: any = [
  {vin: 'XLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Intended', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'KLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'MLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'XLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Intended', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'KLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'MLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'XLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Intended', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'KLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'MLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'XLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Intended', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'KLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'MLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'XLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Intended', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'KLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'MLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'XLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Intended', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'KLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'},
  {vin: 'MLRTEM45578965480', intendedDateTime: '02/01/2022', targetSystem: 'BVMS', status: 'Success', targetSystemStatus: '-', appliedDateTime: '11-03-2022, 11:59:59'}
];

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private organizationService: OrganizationService, public dialog: MatDialog) { }

  ngOnInit(): void {
    // will be change
    // this.dataSource = this.ELEMENT_DATA;
    // will be change

    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0; 
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));

    // Will be changed into translation service
    this.processTranslation();
    if (this.prefDetail) {
      if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { // account pref
        this.proceedStep(this.accountPrefObj.accountPreference);
      } else { // org pref
        this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
          this.proceedStep(orgPref);
        }, (error) => {
          this.proceedStep({});
        });
      }
    }

    this.updateDataSource(this.ELEMENT_DATA);
    
  }

  processTranslation() {
    this.translationData = this.menuTranslation.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  proceedStep(preference: any) {
    let _search = this.prefDetail.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = this.prefDetail.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = this.prefDetail.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = this.prefDetail.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
      this.prefTimeFormat = Number(this.prefDetail.timeformat[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = this.prefDetail.timezone[0].name;
      this.prefDateFormat = this.prefDetail.dateformat[0].name;
      this.prefUnitFormat = this.prefDetail.unit[0].name;
    }
    this.setPrefFormatDate();
  }

  setPrefFormatDate() {
    switch (this.prefDateFormat) {
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        this.dateFormats.parse.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        this.dateFormats.parse.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.dateFormats.parse.dateInput = "MM-DD-YYYY";
        break;
      }
      default: {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
      }
    }
  }


  intendedDateEvent(e) {}

  intendedtimeChanged(e){
    if(this.prefTimeFormat == 12) {
      this.intendedTimeDisplay = e; 
    } else {
      this.intendedTimeDisplay = e + ':59';
    }
  }

  targetSystemchange(e, t){
    this.model.target = e.value;
  }

  statusChange(e, t){}

  changeAppliedDateEvent(e) {}

  appliedtimeChanged(e){
    if(this.prefTimeFormat == 12){
      this.appliedTimeDisplay = e;
    } else {
      this.appliedTimeDisplay = e + ':59';  
    }
  }

  maxResultchange(e, t){
    this.model.maxset = e.value
  }

  onSubmitMeasurement() {
    console.log('Form Submit')
    console.log(this.model)
  this.showLoadingIndicator = false; // will be change at logic time

  }

  onReset() {
  this.intendedTimeDisplay = '';
  this.appliedTimeDisplay = '';
  this.model.target = '';
  this.model.maxset = 50;
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(tableData);

    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
  //     this.dataSource.filterPredicate = function(data, filter: any){
  //       return data.vehicleName.toString().toLowerCase().includes(filter) ||
  //           data.vin?.toString().toLowerCase().includes(filter) ||
  //           data.registrationNumber?.toString().toLowerCase().includes(filter) ||
  //           data.convertedDistance?.toString().toLowerCase().includes(filter) ||
  //           data.numberOfTrips?.toString().toLowerCase().includes(filter)  ||
  //           data.convertedTripTime?.toString().toLowerCase().includes(filter) ||
  //           data.convertedDrivingTime?.toString().toLowerCase().includes(filter)  ||
  //           data.convertedIdleDuration?.toString().toLowerCase().includes(filter) ||
  //           data.convertedStopTime?.toString().toLowerCase().includes(filter) ||
  //           data.convertedAverageDistance?.toString().toLowerCase().includes(filter) ||
  //           data.convertedAverageSpeed?.toString().toLowerCase().includes(filter) ||
  //           data.convertedAverageWeight?.toString().toLowerCase().includes(filter) ||
  //           data.convertedOdometer?.toString().toLowerCase().includes(filter)
  //  }

      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
            let columnName = sort.active;
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
      
      });

      Util.applySearchFilter(this.dataSource, this.displayedColumns ,this.filterValue );
  }

  compare(a: any, b: any, isAsc: boolean, columnName:any) {
    if(columnName ==  "vin" || columnName == "targetSystem" || columnName == "status" || columnName == "targetSystemStatus"){
      if(!(a instanceof Number)) a = a?a.replace(/[^\w\s]/gi, 'z').toUpperCase(): "";
      if(!(b instanceof Number)) b = b?b.replace(/[^\w\s]/gi, 'z').toUpperCase(): "";
    }

      return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }


  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  rowSelected(event: any, row: any) {
    if (event.checked) {
      this.selectedEcoScore.select(row);
    } else {
      this.selectedEcoScore.deselect(row);
    }

    console.log(this.selectedEcoScore.selected)
  }

  exportAsExcelFile(){}

  exportAsPDFFile(){}



  pageSizeUpdated(e) {}

  onClose() {
    this.showMessage = false;
  }

  viewData(data) {
    this.viewPage = true;
    this.viewPageData = data;
  }

  backFromviewPage(e){
    this.viewPage = false;
  }

  reInterpretDialog() {
    const dialogRef = this.dialog.open(DialogMeasurementStatusDialog, {
      width: '40%',
      disableClose: true,
      data: {
        selectedvin: this.selectedEcoScore.selected,
        translationDataDialog: this.translationData
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
      if (result == 'submit') {
        this.showMessage = true;
      }
    });
  }

}


// Measurement Dialog Component

@Component({
  selector: 'dialog-measurement-status-dialog',
  templateUrl: 'measurement-status-dialog/dialog-measurement-status-dialog.html',
  styleUrls: ['./measurement-status-dialog/dialog-measurement-status-dialog.less']
})
export class DialogMeasurementStatusDialog {
  vinList: any;
  dataList: any;
  dataTranslation: any;
  showMoreString = false;
  showMoreLink = false;
  submit = 'submit';
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  ngOnInit() {
    console.log('dialog',this.data.selectedvin)
    this.dataTranslation = this.data.translationDataDialog;
    this.dataList = this.data.selectedvin;
    this.morelessfunctionality(this.data.selectedvin);
  }

  morelessfunctionality(data) {
    if (data.length > 5) {
      this.vinList = data.slice(0, 5);
      this.showMoreLink = true;
      this.showMoreString = true;
    } else {
      this.vinList = data;
    }
  }

  showMore() {
    this.vinList = this.dataList;
    this.showMoreString = false;
  }

  showLess() {
    this.vinList = this.dataList.slice(0, 5);
    this.showMoreString = true;
  }
}
