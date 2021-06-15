import { Component, ElementRef, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { ReportService } from '../../services/report.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { Util } from '../../shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';

@Component({
  selector: 'app-driver-time-management',
  templateUrl: './driver-time-management.component.html',
  styleUrls: ['./driver-time-management.component.less']
})
export class DriverTimeManagementComponent implements OnInit {

  
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectionTab: any;

  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  driverTimeForm: FormGroup;
  translationData: any;
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  driverListData: any = [];
  searchExpandPanel: boolean = true;
  tableExpandPanel: boolean = true;

  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tripData: any = [];
  showLoadingIndicator: boolean = false;
  defaultStartValue: any;
  defaultEndValue: any;
  startDateValue: any;
  endDateValue: any;
  last3MonthDate: any;
  todayDate: any;
  onLoadData: any = [];
  tableInfoObj: any = {};
  tripTraceArray: any = [];
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService) { 
    this.defaultTranslation()
  }


  ngOnInit(): void {
    
    this.showLoadingIndicator = true;

    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.driverTimeForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      driver: ['', [Validators.required]],
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
      menuId: 14 
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
        this.prefTimeFormat = parseInt(prefData.timeformat.filter(i => i.id == this.accountPrefObj.accountPreference.timeFormatId)[0].value.split(" ")[0]);
        this.prefTimeZone = prefData.timezone.filter(i => i.id == this.accountPrefObj.accountPreference.timezoneId)[0].value;
        this.prefDateFormat = prefData.dateformat.filter(i => i.id == this.accountPrefObj.accountPreference.dateFormatTypeId)[0].name;
        this.prefUnitFormat = prefData.unit.filter(i => i.id == this.accountPrefObj.accountPreference.unitId)[0].name;
        this.setDefaultStartEndTime();
        this.setPrefFormatDate();
        this.getOnLoadData();
       // this.getReportPreferences();
      });
    });
  }

  
  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    ////console.log("process translationData:: ", this.translationData)
  }

  
  resetdriverTimeFormControlValue(){
    this.driverTimeForm.get('vehicle').setValue('');
    this.driverTimeForm.get('vehicleGroup').setValue(0);
  }

  onVehicleGroupChange(event: any){
    this.driverTimeForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    if(parseInt(event.value) == 0){ //-- all group
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);

    }else{
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
    }
  }

  onVehicleChange(event: any){

  }

  onDriverChange(event: any){

  }

  onSearch(){
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.driverTimeForm.controls.vehicle.value));
    if(_vinData.length > 0){
      this.showLoadingIndicator = true;
      this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
        this.hideloader();
        //_tripData = [{"id":11903,"tripId":"ae6e42d3-4ba1-49eb-8fe1-704a2271bc49","vin":"XLR0998HGFFT5566","startTimeStamp":1587143959831,"endTimeStamp":1587143959831,"distance":139,"idleDuration":53,"averageSpeed":2663,"averageWeight":655350,"odometer":298850780,"startPosition":"NA","endPosition":"NA","fuelConsumed":166.896551724138,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.66896551724138,"liveFleetPosition":[],"startPositionLattitude":50.96831131,"startPositionLongitude":-1.388581276,"endPositionLattitude":50.9678421,"endPositionLongitude":-1.388695598},{"id":12576,"tripId":"11c2c2c1-c56f-42ce-9e62-31685ed5d2ae","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":22,"idleDuration":3,"averageSpeed":6545,"averageWeight":655350,"odometer":298981400,"startPosition":"NA","endPosition":"NA","fuelConsumed":50,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":0.5,"liveFleetPosition":[],"startPositionLattitude":50.81643677,"startPositionLongitude":-0.7481001616,"endPositionLattitude":50.81661987,"endPositionLongitude":-0.74804914},{"id":13407,"tripId":"ce3c49fb-2291-4052-9d1b-3b2ee343ab33","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":213,"idleDuration":39,"averageSpeed":3461,"averageWeight":655350,"odometer":74677630,"startPosition":"NA","endPosition":"NA","fuelConsumed":91,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":0.91,"liveFleetPosition":[],"startPositionLattitude":51.39526367,"startPositionLongitude":-1.229614377,"endPositionLattitude":51.39541626,"endPositionLongitude":-1.231176734},{"id":12582,"tripId":"6adb296a-549e-4d50-af9d-3bfbc6fc3e4b","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":4,"idleDuration":14,"averageSpeed":3130,"averageWeight":655350,"odometer":10327065,"startPosition":"NA","endPosition":"NA","fuelConsumed":175,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.75,"liveFleetPosition":[],"startPositionLattitude":41.71875763,"startPositionLongitude":26.35817528,"endPositionLattitude":41.71875,"endPositionLongitude":26.35810089},{"id":12587,"tripId":"cc5b9533-1d94-4af8-8cc8-903b0dcd5514","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":65,"idleDuration":10,"averageSpeed":8000,"averageWeight":655350,"odometer":14747690,"startPosition":"NA","endPosition":"NA","fuelConsumed":105,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.05,"liveFleetPosition":[],"startPositionLattitude":43.00225067,"startPositionLongitude":22.80965805,"endPositionLattitude":43.00209045,"endPositionLongitude":22.8104248}];
       // this.tripData = this.reportMapService.getConvertedDataBasedOnPref(_tripData.tripData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
        //this.setTableInfo();
        //this.updateDataSource(this.tripData);
      }, (error)=>{
        //console.log(error);
        this.hideloader();
        this.tripData = [];
        this.tableInfoObj = {};
       // this.updateDataSource(this.tripData);
      });
    }
  }

  onReset(){
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleGroupListData = this.vehicleGroupListData;
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    //this.updateDataSource(this.tripData);
    this.resetTripFormControlValue();
    this.tableInfoObj = {};
    //this.advanceFilterOpen = false;
   // this.selectedPOI.clear();
  }

  resetTripFormControlValue(){
    this.driverTimeForm.get('vehicle').setValue(0);
    this.driverTimeForm.get('vehicleGroup').setValue(0);
    this.driverTimeForm.get('driver').setValue(0);

  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  getOnLoadData(){
    let defaultStartValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let defaultEndValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    this.startDateValue = defaultStartValue;
    this.endDateValue = defaultEndValue;
    let loadParam = {
      "ReportId": 10,
      "AccountId": this.accountId,
      "OrganizationId": this.accountOrganizationId,
      "StartDateTime": Util.convertDateToUtc(defaultStartValue),
      "EndDateTime": Util.convertDateToUtc(defaultEndValue)
    }
    this.showLoadingIndicator = true;
    this.reportService.getDefaultDriverParameter(loadParam).subscribe((initData: any) => {
      this.hideloader();
      this.onLoadData = initData;
      this.filterDateData();
     
      this.setDefaultTodayDate();
    }, (error)=>{
      this.hideloader();
      //this.wholeTripData.vinTripList = [];
     // this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
      //this.loadUserPOI();
    });
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    let distinctGroupId : any = [];
    let distinctDriverId : any = [];
    let finalDriverList : any = [];
    let _last3m = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let _yesterday = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    let currentStartTime = Util.convertDateToUtc(_last3m); //_last3m.getTime();
    let currentEndTime = Util.convertDateToUtc(_yesterday); // _yesterday.getTime();
    //console.log(currentStartTime + "<->" + currentEndTime);
    // if(this.onLoadData.vehicleDetailsWithAccountVisibiltyList > 0){
    //   let filterVIN: any = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);

    // }
    if(this.onLoadData.vehicleDetailsWithAccountVisibiltyList.length > 0){
      distinctGroupId  = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.map(data=>data.vehicleGroupId);
      //this.vehicleGroupListData = distinctGroupId;
    if(distinctGroupId.length > 0){
      distinctVIN = distinctGroupId.filter((value, index, self) => self.indexOf(value) === index);

      if(distinctVIN.length > 0){
        distinctVIN.forEach(element => {
          let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vehicleGroupId === element); 
          if(_item.length > 0){
            finalVINDataList.push(_item[0])
          }
        });
        this.vehicleGroupListData = finalVINDataList;

        ////console.log("finalVINDataList:: ", finalVINDataList); 
      }
      this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
      this.vehicleListData.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });

    }
    }
    if(this.onLoadData.driverList.length > 0){
      let driverIds: any = this.onLoadData.driverList.map(data=> data.vin);
      if(driverIds.length > 0){
          distinctDriverId = driverIds.filter((value, index, self) => self.indexOf(value) === index);
          
      if(distinctDriverId.length > 0){
        distinctDriverId.forEach(element => {
          let _item = this.onLoadData.driverList.filter(i => i.vin === element); 
          if(_item.length > 0){
            finalDriverList.push(_item[0])
          }
        });
        this.driverListData = finalDriverList;
        this.driverListData.unshift({ driverID: 0, firstName: this.translationData.lblAll || 'All' });

        ////console.log("finalVINDataList:: ", finalVINDataList); 
      }
      }

    }
    this.resetTripFormControlValue();

   
  }


  //********************************** Date Time Functions *******************************************//
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

  setPrefFormatTime(){
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = '00:00:00';
      this.endTimeDisplay = '23:59:59';
    }else{
      this.startTimeDisplay = '12:00 AM';
      this.endTimeDisplay = '11:59 PM';
    }
  }

  setDefaultStartEndTime(){
    this.setPrefFormatTime();
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

  setDefaultDateToFetch(){

  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
  }

  getYesterdaysDate() {
    //var date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-1);
    return date;
  }

  getLastWeekDate() {
    // var date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-7);
    return date;
  }

  getLastMonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-1);
    return date;
  }

  getLast3MonthDate(){
    // let date = new Date();
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
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
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastweek': {
        this.selectionTab = 'lastweek';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastWeekDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastMonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
    }
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    //this.endDateValue = event.value._d;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
  }

  startTimeChanged(selectedTime: any) {
    this.selectedStartTime = selectedTime;
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = selectedTime + ':00';
    }
    else{
      this.startTimeDisplay = selectedTime;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
  }

  endTimeChanged(selectedTime: any) {
    this.selectedEndTime = selectedTime;
    if(this.prefTimeFormat == 24){
      this.endTimeDisplay = selectedTime + ':59';
    }
    else{
      this.endTimeDisplay = selectedTime;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
  }



}
