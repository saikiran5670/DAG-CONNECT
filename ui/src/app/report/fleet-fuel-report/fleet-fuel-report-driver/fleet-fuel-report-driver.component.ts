
import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { TranslationService } from '../../../services/translation.service';
import { OrganizationService } from '../../../services/organization.service';
import { Util } from '../../../shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { ReportService } from '../../../services/report.service';

@Component({
  selector: 'app-fleet-fuel-report-driver',
  templateUrl: './fleet-fuel-report-driver.component.html',
  styleUrls: ['./fleet-fuel-report-driver.component.less']
})
export class FleetFuelReportDriverComponent implements OnInit {
  @Input() translationData: any;
  tripForm: FormGroup;
  fleetFuelonSearchData: any = {};
  selectionTab: any;
  internalSelection: boolean = false;
  startDateValue: any = 0;
  endDateValue: any = 0;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  vehicleDD: any = [];
  vehicleGrpDD: any = [];
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  todayDate: any;
  last3MonthDate: any;
  searchExpandPanel: boolean = true;
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  fleetFuelSearchData: any = {};
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  accountPrefObj : any;
  tableExpandPanel: boolean = true;
  initData: any = [];
  reportPrefData: any = [];
  
  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private reportService: ReportService,private _formBuilder: FormBuilder,private translationService: TranslationService,private organizationService: OrganizationService) {  }

  ngOnInit(): void {
    this.fleetFuelSearchData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    // console.log("----globalSearchFilterData---",this.fleetFuelSearchData)
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
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
      menuId: 10 //-- for fleet utilisation
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
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
    });
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.getFleetPreferences();
  }
  onSearch(){
  }

  onReset(){
  }

  getFleetPreferences(){
    this.reportService.getUserPreferenceReport(5, this.accountId, this.accountOrganizationId).subscribe((data: any) => {
      // data= {"userPreferences":[{"dataAtrributeId":140,"name":"Report.General.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_general_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":123,"name":"Report.General.TotalDistance","description":"","type":"A","reportReferenceType":"","key":"da_report_general_totaldistance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":141,"name":"Report.General.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":18,"name":"Report.General.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_general_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":99,"name":"Report.General.NumberOfVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberofvehicles","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":140,"name":"Report.General.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_general_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":123,"name":"Report.General.TotalDistance","description":"","type":"A","reportReferenceType":"","key":"da_report_general_totaldistance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":141,"name":"Report.General.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberoftrips","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":18,"name":"Report.General.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_general_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":99,"name":"Report.General.NumberOfVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_general_numberofvehicles","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":142,"name":"Report.Charts.DistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_distanceperday","state":"A","chartType":"L","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":143,"name":"Report.Charts.NumberOfVehiclesPerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_numberofvehiclesperday","state":"A","chartType":"L","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":144,"name":"Report.Charts.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_mileagebasedutilization","state":"A","chartType":"P","thresholdType":"U","thresholdValue":99000},{"dataAtrributeId":145,"name":"Report.Charts.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_charts_timebasedutilization","state":"A","chartType":"P","thresholdType":"U","thresholdValue":65100000},{"dataAtrributeId":151,"name":"Report.CalendarView.TotalTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_totaltrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":149,"name":"Report.CalendarView.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_drivingtime","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":148,"name":"Report.CalendarView.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_distance","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":152,"name":"Report.CalendarView.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_mileagebasedutilization","state":"I","chartType":"P","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":146,"name":"Report.CalendarView.AverageWeight","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_averageweight","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":147,"name":"Report.CalendarView.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":150,"name":"Report.CalendarView.ActiveVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_activevehicles","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":153,"name":"Report.CalendarView.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_timebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":151,"name":"Report.CalendarView.TotalTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_totaltrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":149,"name":"Report.CalendarView.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_drivingtime","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":148,"name":"Report.CalendarView.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_distance","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":152,"name":"Report.CalendarView.MileageBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_mileagebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":146,"name":"Report.CalendarView.AverageWeight","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_averageweight","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":147,"name":"Report.CalendarView.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_idleduration","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":150,"name":"Report.CalendarView.ActiveVehicles","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_activevehicles","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":153,"name":"Report.CalendarView.TimeBasedUtilization","description":"","type":"A","reportReferenceType":"","key":"da_report_calendarview_timebasedutilization","state":"I","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":135,"name":"Report.Details.VehicleName","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vehiclename","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":107,"name":"Report.Details.RegistrationNumber","description":"","type":"A","reportReferenceType":"","key":"da_report_details_registrationnumber","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":98,"name":"Report.Details.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_details_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":17,"name":"Report.Details.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":24,"name":"Report.Details.AverageSpeed","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagespeed","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":55,"name":"Report.Details.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":84,"name":"Report.Details.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_details_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":119,"name":"Report.Details.StopTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_stoptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":100,"name":"Report.Details.Odometer","description":"","type":"A","reportReferenceType":"","key":"da_report_details_odometer","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":28,"name":"Report.Details.AverageWeightPerTrip","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averageweightpertrip","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":137,"name":"Report.Details.VIN","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vin","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":44,"name":"Report.Details.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_details_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":132,"name":"Report.Details.TripTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_triptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":135,"name":"Report.Details.VehicleName","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vehiclename","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":107,"name":"Report.Details.RegistrationNumber","description":"","type":"A","reportReferenceType":"","key":"da_report_details_registrationnumber","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":98,"name":"Report.Details.NumberOfTrips","description":"","type":"A","reportReferenceType":"","key":"da_report_details_numberoftrips","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":17,"name":"Report.Details.AverageDistancePerDay","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagedistanceperday","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":24,"name":"Report.Details.AverageSpeed","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averagespeed","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":55,"name":"Report.Details.DrivingTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_drivingtime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":84,"name":"Report.Details.IdleDuration","description":"","type":"A","reportReferenceType":"","key":"da_report_details_idleduration","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":119,"name":"Report.Details.StopTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_stoptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":100,"name":"Report.Details.Odometer","description":"","type":"A","reportReferenceType":"","key":"da_report_details_odometer","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":28,"name":"Report.Details.AverageWeightPerTrip","description":"","type":"A","reportReferenceType":"","key":"da_report_details_averageweightpertrip","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":137,"name":"Report.Details.VIN","description":"","type":"A","reportReferenceType":"","key":"da_report_details_vin","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":44,"name":"Report.Details.Distance","description":"","type":"A","reportReferenceType":"","key":"da_report_details_distance","state":"A","chartType":"","thresholdType":" ","thresholdValue":0},{"dataAtrributeId":132,"name":"Report.Details.TripTime","description":"","type":"A","reportReferenceType":"","key":"da_report_details_triptime","state":"A","chartType":"","thresholdType":" ","thresholdValue":0}],"code":200,"message":""}
      
      this.reportPrefData = data["userPreferences"];
      this.resetPref();
      this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetPref();
      this.preparePrefData(this.reportPrefData);
      this.loadWholeTripData();
    });
  }

  resetPref(){
    //this.summaryColumnData = [];
    //this.chartsColumnData = [];
    //this.calenderColumnData = [];
    //this.detailColumnData = [];
  }

  preparePrefData(prefData: any){
  }

  loadWholeTripData(){
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.fleetFuelSearchData.modifiedFrom !== "") {
      //console.log("---if fleetFuelSearchData startDateStamp exist")
      if(this.fleetFuelSearchData.timeRangeSelection !== ""){
        this.selectionTab = this.fleetFuelSearchData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.fleetFuelSearchData.startDateStamp);
      let endDateFromSearch = new Date(this.fleetFuelSearchData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
    }else{
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
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

selectionTimeRange(selection: any){
  this.internalSelection = true;
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

setPrefFormatTime(){
  if(!this.internalSelection && this.fleetFuelSearchData.modifiedFrom !== "" &&  ((this.fleetFuelSearchData.startTimeStamp || this.fleetFuelSearchData.endTimeStamp) !== "") ) {
    if(this.prefTimeFormat == this.fleetFuelSearchData.filterPrefTimeFormat){ // same format
      this.selectedStartTime = this.fleetFuelSearchData.startTimeStamp;
      this.selectedEndTime = this.fleetFuelSearchData.endTimeStamp;
      this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fleetFuelSearchData.startTimeStamp}:00` : this.fleetFuelSearchData.startTimeStamp;
      this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.fleetFuelSearchData.endTimeStamp}:59` : this.fleetFuelSearchData.endTimeStamp;  
    }else{ // different format
      if(this.prefTimeFormat == 12){ // 12
        this.selectedStartTime = this._get12Time(this.fleetFuelSearchData.startTimeStamp);
        this.selectedEndTime = this._get12Time(this.fleetFuelSearchData.endTimeStamp);
        this.startTimeDisplay = this.selectedStartTime; 
        this.endTimeDisplay = this.selectedEndTime;
      }else{ // 24
        this.selectedStartTime = this.get24Time(this.fleetFuelSearchData.startTimeStamp);
        this.selectedEndTime = this.get24Time(this.fleetFuelSearchData.endTimeStamp);
        this.startTimeDisplay = `${this.selectedStartTime}:00`; 
        this.endTimeDisplay = `${this.selectedEndTime}:59`;
      }
    }
  }else {
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = '00:00:00';
      this.endTimeDisplay = '23:59:59';
      this.selectedStartTime = "00:00";
      this.selectedEndTime = "23:59";
    } else{
      this.startTimeDisplay = '12:00 AM';
      this.endTimeDisplay = '11:59 PM';
      this.selectedStartTime = "00:00";
      this.selectedEndTime = "23:59";
    }
  }

}
_get12Time(_sTime: any){
  let _x = _sTime.split(':');
  let _yy: any = '';
  if(_x[0] >= 12){ // 12 or > 12
    if(_x[0] == 12){ // exact 12
      _yy = `${_x[0]}:${_x[1]} PM`;
    }else{ // > 12
      let _xx = (_x[0] - 12);
      _yy = `${_xx}:${_x[1]} PM`;
    }
  }else{ // < 12
    _yy = `${_x[0]}:${_x[1]} AM`;
  }
  return _yy;
}

get24Time(_time: any){
  let _x = _time.split(':');
  let _y = _x[1].split(' ');
  let res: any = '';
  if(_y[1] == 'PM'){ // PM
    let _z: any = parseInt(_x[0]) + 12;
    res = `${(_x[0] == 12) ? _x[0] : _z}:${_y[0]}`;
  }else{ // AM
    res = `${_x[0]}:${_y[0]}`;
  }
  return res;
}


setDefaultStartEndTime(){
  this.setPrefFormatTime();
  // if(this.internalSelection && this.fleetFuelSearchData.modifiedFrom == ""){
  //   this.selectedStartTime = "00:00";
  //   this.selectedEndTime = "23:59";
  // }
}
setStartEndDateTime(date: any, timeObj: any, type: any){
  if(type == "start"){
    console.log("--date type--",date)
    console.log("--date type--",timeObj)
    // this.fleetFuelSearchData["startDateStamp"] = date;
    // this.fleetFuelSearchData.testDate = date;
    // this.fleetFuelSearchData["startTimeStamp"] = timeObj;
    // this.setGlobalSearchData(this.fleetFuelSearchData)
    // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
    // console.log("---time after function called--",timeObj)
  }else if(type == "end") {
    // this.fleetFuelSearchData["endDateStamp"] = date;
    // this.fleetFuelSearchData["endTimeStamp"] = timeObj;
    // this.setGlobalSearchData(this.fleetFuelSearchData)
    // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
  }

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

getYesterdaysDate(){ 
  var date = Util.getUTCDate(this.prefTimeZone);
  date.setDate(date.getDate()-1);
  return date;

}

getLastMonthDate(){
  var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-1);
    return date;
}


getLast3MonthDate(){
  var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
}

getLastWeekDate(){
  var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-7);
    return date;
}

onVehicleChange(event: any){
  this.internalSelection = true; 
  // this.fleetFuelSearchData["vehicleDropDownValue"] = event.value;
  // this.setGlobalSearchData(this.fleetFuelSearchData)
}

startTimeChanged(selectedTime: any) {
  this.internalSelection = true;
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
  this.internalSelection = true;
  this.selectedEndTime = selectedTime;
  if(this.prefTimeFormat == 24){
    this.endTimeDisplay = selectedTime + ':59';
  }
  else{
    this.endTimeDisplay = selectedTime;
  }
  this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
}

changeStartDateEvent(event: MatDatepickerInputEvent<any>){
  this.internalSelection = true;
  //this.startDateValue = event.value._d;
  this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');

}

changeEndDateEvent(event: MatDatepickerInputEvent<any>){
  //this.endDateValue = event.value._d;
  this.internalSelection = true;
  this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
}

onVehicleGroupChange(event: any){
}

processTranslation(transData: any){

}

}

