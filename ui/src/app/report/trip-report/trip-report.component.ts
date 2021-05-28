import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { ReportMapService } from './report-map.service';
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
// import * as jsPDF from 'jspdf';
import 'jspdf-autotable';
var jsPDF = require('jspdf');

declare var H: any;

@Component({
  selector: 'app-trip-report',
  templateUrl: './trip-report.component.html',
  styleUrls: ['./trip-report.component.less']
})

export class TripReportComponent implements OnInit {
  
  selectionTab: any;
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  tripForm: FormGroup;
  displayedColumns = ['All', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed100Km', 'drivingTime', 'alert', 'events'];
  translationData: any;
  // hereMap: any;
  // platform: any;
  // ui: any;
  @ViewChild("map")
  public mapElement: ElementRef;
  showMap: boolean = false;
  showMapPanel: boolean = false;
  searchExpandPanel: boolean = true;
  tableExpandPanel: boolean = true;
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  dataSource: any = new MatTableDataSource([]);
  selectedTrip = new SelectionModel(true, []);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tripData: any = [];
  showLoadingIndicator: boolean = false;
  startDateValue: any;
  endDateValue: any;
  last3MonthDate: any;
  todayDate: any;
  wholeTripData: any = [];
  tableInfoObj: any = {};
  tripTraceArray: any = [];

  constructor(private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService) {
    // this.platform = new H.service.Platform({
    //   "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    // });
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.tripForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []]
    });
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 6 //-- for Trip Report
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.setDefaultStartEndTime();
      this.setDefaultTodayDate();
      this.loadWholeTripData();
    });
  }

  setDefaultStartEndTime(){
    this.selectedStartTime = "00:00";
    this.selectedEndTime = "23:59";
  }

  setDefaultTodayDate(){
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
  }

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
    }, (error)=>{
      this.hideloader();
      this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
      this.wholeTripData.vinTripList = [];
    });
  }

  // dummyData(){
  //   [ 
  //     {
  //       "id": 214681,
  //       "tripId": "3",
  //       "vin": "V12003",
  //       "startTimeStamp": 0,
  //       "endTimeStamp": 0,
  //       "distance": 0,
  //       "idleDuration": 0,
  //       "averageSpeed": 0,
  //       "averageWeight": 0,
  //       "odometer": 0,
  //       "startPosition": "NA",
  //       "endPosition": "NA",
  //       "fuelConsumed": 0,
  //       "drivingTime": 0,
  //       "alert": 0,
  //       "events": 0,
  //       "fuelConsumed100Km": 0,
  //       "liveFleetPosition": [],
  //       "startPositionLattitude": 0,
  //       "startPositionLongitude": 0,
  //       "endPositionLattitude": 0,
  //       "endPositionLongitude": 0
  //     },
  //     {
  //       "id": 214681,
  //       "tripId": "3",
  //       "vin": "V12003",
  //       "startTimeStamp": 0,
  //       "endTimeStamp": 0,
  //       "distance": 0,
  //       "idleDuration": 0,
  //       "averageSpeed": 0,
  //       "averageWeight": 0,
  //       "odometer": 0,
  //       "startPosition": "NA",
  //       "endPosition": "NA",
  //       "fuelConsumed": 0,
  //       "drivingTime": 0,
  //       "alert": 0,
  //       "events": 0,
  //       "fuelConsumed100Km": 0,
  //       "liveFleetPosition": [],
  //       "startPositionLattitude": 0,
  //       "startPositionLongitude": 0,
  //       "endPositionLattitude": 0,
  //       "endPositionLongitude": 0
  //     }
  //   ]
  // }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    ////console.log("process translationData:: ", this.translationData)
  }

  public ngAfterViewInit() {
    // setTimeout(() => {
     //this.reportMapService.initMap(this.mapElement);
    // }, 0);
  }

  onSearch(){
    let _startTime = this.startDateValue.getTime();
    let _endTime = this.endDateValue.getTime();
    let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if(_vinData.length > 0){
      this.showLoadingIndicator = true;
      this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
        this.hideloader();
        this.tripData = _tripData.tripData;
        this.setTableInfo();
        this.updateDataSource(this.tripData);
      }, (error)=>{
        //console.log(error);
        this.hideloader();
        this.tripData = [];
        this.tableInfoObj = {};
        this.updateDataSource(this.tripData);
      });
    }
  }

  setTableInfo(){
    let vehName: any = '';
    let vehGrpName: any = '';
    let vin: any = '';
    let plateNo: any = '';
    this.vehicleGroupListData.forEach(element => {
      if(element.vehicleId == parseInt(this.tripForm.controls.vehicle.value)){
        vehName = element.vehicleName;
        vin = element.vin;
        plateNo = element.registrationNo;
      }
      if(parseInt(this.tripForm.controls.vehicleGroup.value) != 0){
        if(element.vehicleGroupId == parseInt(this.tripForm.controls.vehicleGroup.value)){
          vehGrpName = element.vehicleGroupName;
        }
      }
    });

    if(parseInt(this.tripForm.controls.vehicleGroup.value) == 0){
      vehGrpName = this.translationData.lblAll || 'All';
    }
    this.tableInfoObj = {
      fromDate: this.formStartDate(this.startDateValue),
      endDate: this.formStartDate(this.endDateValue),
      vehGroupName: vehGrpName,
      vehicleName: vehName,
      vin: vin,
      regNo: plateNo
    }
  }

  formStartDate(date: any){
    let h = (date.getHours() < 10) ? ('0'+date.getHours()) : date.getHours(); 
    let m = (date.getMinutes() < 10) ? ('0'+date.getMinutes()) : date.getMinutes(); 
    let s = (date.getSeconds() < 10) ? ('0'+date.getSeconds()) : date.getSeconds(); 
    let _date = `${date.getMonth()+1}/${date.getDate()}/${date.getFullYear()} ${h}:${m}:${s}`;
    return _date;
  }

  onReset(){
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.updateDataSource(this.tripData);
    this.filterDateData();
    this.resetTripFormControlValue();
    this.tableInfoObj = {};
  }

  resetTripFormControlValue(){
    this.tripForm.get('vehicle').setValue('');
    this.tripForm.get('vehicleGroup').setValue('');
  }

  onVehicleGroupChange(event: any){
    this.tripForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    if(parseInt(event.value) == 0){ //-- all group
      this.vehicleListData = this.vehicleGroupListData;
    }else{
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
    }
  }

  onVehicleChange(event: any){

  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.showMap = false;
    this.selectedTrip.clear();
    if(this.initData.length > 0){
      if(!this.showMapPanel){ //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.reportMapService.initMap(this.mapElement);
        }, 0);
      }else{
        this.reportMapService.clearRoutesFromMap();
      }
    }
    else{
      this.showMapPanel = false;
    }
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  exportAsExcelFile(){
    this.matTableExporter.exportTable('xlsx', {fileName:'Package_Data', sheet: 'sheet_name'});
  }

  exportAsPDFFile(){
   
    var doc = new jsPDF();

    doc.setFontSize(18);
    doc.text('My Team Detail', 11, 8);
    doc.setFontSize(11);
    doc.setTextColor(100);


    (doc as any).autoTable({
      head: this.displayedColumns,
      body: this.initData,
      theme: 'plain',
      didDrawCell: data => {
        console.log(data.column.index)
      }
    })

    // below line for Open PDF document in new tab
    doc.output('dataurlnewwindow')

    // below line for Download PDF document  
    doc.save('tripReport.pdf');
  }

  masterToggleForTrip() {
    this.tripTraceArray = [];
    if(this.isAllSelectedForTrip()){
      this.selectedTrip.clear();
      this.reportMapService.clearRoutesFromMap();
      this.showMap = false;
    }
    else{
      this.dataSource.data.forEach((row) => {
        this.selectedTrip.select(row);
        this.tripTraceArray.push(row);
      });
      this.showMap = true;
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray);
    }
  }

  isAllSelectedForTrip() {
    const numSelected = this.selectedTrip.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForTrip(row?: any): string {
    if (row)
      return `${this.isAllSelectedForTrip() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedTrip.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  pageSizeUpdated(_event) {
    // setTimeout(() => {
    //   document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    // }, 100);
  }

  tripCheckboxClicked(event: any, row: any) {
    this.showMap = this.selectedTrip.selected.length > 0 ? true : false;
    if(event.checked){ //-- add new marker
      this.tripTraceArray.push(row);
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray);
    }
    else{ //-- remove existing marker
      let arr = this.tripTraceArray.filter(item => item.id != row.id);
      this.tripTraceArray = arr;
      this.reportMapService.viewSelectedRoutes(this.tripTraceArray);
    }
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  startTimeChanged(selectedTime: any) {
    this.selectedStartTime = selectedTime;
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
    this.resetTripFormControlValue();
    this.filterDateData();
  }

  endTimeChanged(selectedTime: any) {
    this.selectedEndTime = selectedTime;
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
    this.resetTripFormControlValue();
    this.filterDateData();
  }

  getTodayDate(){
    let todayDate = new Date(); //-- UTC
    return todayDate;
  }

  getYesterdaysDate() {
    var date = new Date();
    date.setDate(date.getDate()-1);
    return date;
  }

  getLastWeekDate() {
    var date = new Date();
    date.setDate(date.getDate()-7);
    return date;
  }

  getLastMonthDate(){
    let date = new Date();
    date.setMonth(date.getMonth()-1);
    return date;
  }

  getLast3MonthDate(){
    let date = new Date();
    date.setMonth(date.getMonth()-3);
    return date;
  }

  selectionTimeRange(selection: any){
    switch(selection){
      case 'today': {
        this.selectionTab = 'today';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'yesterday': {
        this.selectionTab = 'yesterday';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastweek': {
        this.selectionTab = 'lastweek';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastWeekDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastMonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
    }
    this.resetTripFormControlValue();
    this.filterDateData();
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<Date>){
    ////console.log("start:: ", event.value)
    this.startDateValue = event.value;
    this.resetTripFormControlValue();
    this.filterDateData();
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<Date>){
    ////console.log("end: ", event.value)
    this.endDateValue = event.value;
    this.resetTripFormControlValue();
    this.filterDateData();
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){
    date.setHours(timeObj.split(":")[0]);
    date.setMinutes(timeObj.split(":")[1]);
    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    let currentStartTime = this.startDateValue.getTime();
    let currentEndTime = this.endDateValue.getTime();
    //console.log(currentStartTime + "<->" + currentEndTime);
    if(this.wholeTripData.vinTripList.length > 0){
      let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
      if(filterVIN.length > 0){
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
        ////console.log("distinctVIN:: ", distinctVIN);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element); 
            if(_item.length > 0){
              finalVINDataList.push(_item[0])
            }
          });
          ////console.log("finalVINDataList:: ", finalVINDataList); 
        }
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    this.vehicleListData = finalVINDataList;
  }

}
