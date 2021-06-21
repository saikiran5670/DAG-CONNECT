import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReportService } from '../../services/report.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { ReportMapService } from '../report-map.service';
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { LandmarkCategoryService } from '../../services/landmarkCategory.service'; 
//var jsPDF = require('jspdf');
import * as moment from 'moment-timezone';
import { Util } from '../../shared/util';
import { ChartType } from 'chart.js';
import { MultiDataSet, Label } from 'ng2-charts';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-fleet-utilisation',
  templateUrl: './fleet-utilisation.component.html',
  styleUrls: ['./fleet-utilisation.component.less']
})
export class FleetUtilisationComponent implements OnInit {
  tripReportId: any = 1;
  selectionTab: any;
  reportPrefData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  tripForm: FormGroup;
  displayedColumns = ['vehicle_name', 'vin', 'RegPlateNumber', 'distance', 'NumberOfTrips', 'TripTime', 'drivingTime', 'idleDuration', 'averageSpeed', 'AverageWeightPerTrip', 'AverageDistancePerDay', 'odometer'];
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
  selectedPOI = new SelectionModel(true, []);
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
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  advanceFilterOpen: boolean = false;
  showField: any = {
    vehicleName: true,
    vin: true,
    regNo: true
  };
  prefMapData: any = [
    {
      key: 'da_report_details_vehiclename',
      value: 'vehiclename'
    },
    {
      key: 'da_report_details_averagespeed',
      value: 'averageSpeed'
    },
    {
      key: 'da_report_details_drivingtime',
      value: 'drivingTime'
    },
    {
      key: 'da_report_details_alerts',
      value: 'alert'
    },
    {
      key: 'da_report_details_averageweight',
      value: 'averageWeight'
    },
    {
      key: 'da_report_details_events',
      value: 'events'
    },
    {
      key: 'da_report_details_distance',
      value: 'distance'
    },
    {
      key: 'da_report_details_enddate',
      value: 'endTimeStamp'
    },
    {
      key: 'da_report_details_endposition',
      value: 'endPosition'
    },
    {
      key: 'da_report_details_fuelconsumed',
      value: 'fuelConsumed100Km'
    },
    {
      key: 'da_report_details_idleduration',
      value: 'idleDuration'
    },
    {
      key: 'da_report_details_odometer',
      value: 'odometer'
    },
    {
      key: 'da_report_details_registrationnumber',
      value: 'registrationnumber'
    },
    {
      key: 'da_report_details_startdate',
      value: 'startTimeStamp'
    },
    {
      key: 'da_report_details_vin',
      value: 'vin'
    },
    {
      key: 'da_report_details_startposition',
      value: 'startPosition'
    }
  ];
  doughnutChartLabels: Label[] = ['Percentage of vehicles with distance done under 10500km', 'Percentage of vehicles with distance done above 10500km'];
  doughnutChartData: MultiDataSet = [
    [20, 80]
  ];
  doughnutChartType: ChartType = 'doughnut';

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService) {
    this.defaultTranslation();
   }

  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  ngOnInit(): void {
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
        this.prefTimeFormat = parseInt(prefData.timeformat.filter(i => i.id == this.accountPrefObj.accountPreference.timeFormatId)[0].value.split(" ")[0]);
        this.prefTimeZone = prefData.timezone.filter(i => i.id == this.accountPrefObj.accountPreference.timezoneId)[0].value;
        this.prefDateFormat = prefData.dateformat.filter(i => i.id == this.accountPrefObj.accountPreference.dateFormatTypeId)[0].name;
        this.prefUnitFormat = prefData.unit.filter(i => i.id == this.accountPrefObj.accountPreference.unitId)[0].name;
        this.setDefaultStartEndTime();
        this.setPrefFormatDate();
        this.setDefaultTodayDate();
        this.getFleetPreferences();
      });
    });
  }

  getFleetPreferences(){
    this.reportService.getUserPreferenceReport(5, this.accountId, this.accountOrganizationId).subscribe((data: any) => {
      this.reportPrefData = data["userPreferences"];
      this.setDisplayColumnBaseOnPref();
      this.loadWholeTripData();
    }, (error) => {
      this.reportPrefData = [];
      this.setDisplayColumnBaseOnPref();
      this.loadWholeTripData();
    });
  }

  setDisplayColumnBaseOnPref(){
    let filterPref = this.reportPrefData.filter(i => i.state == 'I');
    if(filterPref.length > 0){
      filterPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key);
        if(search.length > 0){
          let index = this.displayedColumns.indexOf(search[0].value);
          if (index > -1) {
              this.displayedColumns.splice(index, 1);
          }
        }

        if(element.key == 'da_report_details_vehiclename'){
          this.showField.vehicleName = false;
        }else if(element.key == 'da_report_details_vin'){
          this.showField.vin = false;
        }else if(element.key == 'da_report_details_registrationnumber'){
          this.showField.regNo = false;
        }
      });
    }
  }


  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    ////console.log("process translationData:: ", this.translationData)
  }

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
    }, (error)=>{
      this.hideloader();
      this.wholeTripData.vinTripList = [];
      this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
      //this.loadUserPOI();
    });
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    let _last3m = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let _yesterday = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    let currentStartTime = Util.convertDateToUtc(_last3m); //_last3m.getTime();
    let currentEndTime = Util.convertDateToUtc(_yesterday); // _yesterday.getTime();
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
    if(this.vehicleGroupListData.length > 0){
      this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.resetTripFormControlValue();
    }
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
  }

  onSearch(){
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if(_vinData.length > 0){
      this.showLoadingIndicator = true;

      // Dummy data

      let fleetData =[
        {
          "vehicle_name":"Vehicle 1",
          "vin":"XLR0998HGFFT5566",
          "StopTime":1587143959831,
          "NumberOfTrips":15,
          "distance":139,
          "idleDuration":353,
          "averageSpeed":2663,
          "odometer":298850780,
          "AverageDistancePerDay":50,
          "AverageWeightPerTrip":5000,
          "drivingTime":0,
          "TripTime":0,
          "RegPlateNumber":"",
        },
        {
          "vehicle_name":"Vehicle 2",
          "vin":"XLR0998HGFFT5566",
          "StopTime":1587143959831,
          "NumberOfTrips":7,
          "distance":144,
          "idleDuration":253,
          "averageSpeed":2663,
          "odometer":298850780,
          "AverageDistancePerDay":50,
          "AverageWeightPerTrip":5000,
          "drivingTime":0,
          "TripTime":0,
          "RegPlateNumber":"",
        },{
          "vehicle_name":"Vehicle 3",
          "vin":"XLR0998HGFFT5566",
          "StopTime":1587143959831,
          "NumberOfTrips":5,
          "distance":133,
          "idleDuration":258,
          "averageSpeed":2663,
          "odometer":298850780,
          "AverageDistancePerDay":50,
          "AverageWeightPerTrip":5000,
          "drivingTime":0,
          "TripTime":0,
          "RegPlateNumber":"",
        },{
          "vehicle_name":"Vehicle 4",
          "vin":"XLR0998HGFFT5566",
          "StopTime":1587143959831,
          "NumberOfTrips":10,
          "distance":130,
          "idleDuration":255,
          "averageSpeed":2663,
          "odometer":298850780,
          "AverageDistancePerDay":50,
          "AverageWeightPerTrip":5000,
          "drivingTime":0,
          "TripTime":0,
          "RegPlateNumber":"",
        },{
          "vehicle_name":"Vehicle 5",
          "vin":"XLR0998HGFFT5566",
          "StopTime":1587143959831,
          "NumberOfTrips":14,
          "distance":150,
          "idleDuration":553,
          "averageSpeed":2663,
          "odometer":298850780,
          "AverageDistancePerDay":50,
          "AverageWeightPerTrip":5000,
          "drivingTime":0,
          "TripTime":0,
          "RegPlateNumber":"",
        }
      ];
      this.tripData = this.reportMapService.getConvertedFleetDataBasedOnPref(fleetData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
      this.setTableInfo();
      this.updateDataSource(this.tripData);
      this.hideloader();

      // Dummy data ends

      // this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
      //   this.hideloader();
      //   //_tripData = [{"id":11903,"tripId":"ae6e42d3-4ba1-49eb-8fe1-704a2271bc49","vin":"XLR0998HGFFT5566","startTimeStamp":1587143959831,"endTimeStamp":1587143959831,"distance":139,"idleDuration":53,"averageSpeed":2663,"averageWeight":655350,"odometer":298850780,"startPosition":"NA","endPosition":"NA","fuelConsumed":166.896551724138,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.66896551724138,"liveFleetPosition":[],"startPositionLattitude":50.96831131,"startPositionLongitude":-1.388581276,"endPositionLattitude":50.9678421,"endPositionLongitude":-1.388695598},{"id":12576,"tripId":"11c2c2c1-c56f-42ce-9e62-31685ed5d2ae","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":22,"idleDuration":3,"averageSpeed":6545,"averageWeight":655350,"odometer":298981400,"startPosition":"NA","endPosition":"NA","fuelConsumed":50,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":0.5,"liveFleetPosition":[],"startPositionLattitude":50.81643677,"startPositionLongitude":-0.7481001616,"endPositionLattitude":50.81661987,"endPositionLongitude":-0.74804914},{"id":13407,"tripId":"ce3c49fb-2291-4052-9d1b-3b2ee343ab33","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":213,"idleDuration":39,"averageSpeed":3461,"averageWeight":655350,"odometer":74677630,"startPosition":"NA","endPosition":"NA","fuelConsumed":91,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":0.91,"liveFleetPosition":[],"startPositionLattitude":51.39526367,"startPositionLongitude":-1.229614377,"endPositionLattitude":51.39541626,"endPositionLongitude":-1.231176734},{"id":12582,"tripId":"6adb296a-549e-4d50-af9d-3bfbc6fc3e4b","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":4,"idleDuration":14,"averageSpeed":3130,"averageWeight":655350,"odometer":10327065,"startPosition":"NA","endPosition":"NA","fuelConsumed":175,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.75,"liveFleetPosition":[],"startPositionLattitude":41.71875763,"startPositionLongitude":26.35817528,"endPositionLattitude":41.71875,"endPositionLongitude":26.35810089},{"id":12587,"tripId":"cc5b9533-1d94-4af8-8cc8-903b0dcd5514","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":65,"idleDuration":10,"averageSpeed":8000,"averageWeight":655350,"odometer":14747690,"startPosition":"NA","endPosition":"NA","fuelConsumed":105,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.05,"liveFleetPosition":[],"startPositionLattitude":43.00225067,"startPositionLongitude":22.80965805,"endPositionLattitude":43.00209045,"endPositionLongitude":22.8104248}];
      //   this.tripData = this.reportMapService.getConvertedFleetDataBasedOnPref(_tripData.tripData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
      //   this.setTableInfo();
      //   this.updateDataSource(this.tripData);
      // }, (error)=>{
      //   //console.log(error);
      //   this.hideloader();
      //   this.tripData = [];
      //   this.tableInfoObj = {};
      //   this.updateDataSource(this.tripData);
      // });
    }
  }

  onReset(){
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.tripData = [];
    this.vehicleGroupListData = this.vehicleGroupListData;
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.updateDataSource(this.tripData);
    this.resetTripFormControlValue();
    this.tableInfoObj = {};
    this.advanceFilterOpen = false;
    this.selectedPOI.clear();
  }

  sumOfColumns(columnName : any){
    let sum: any = 0;
    switch(columnName){
      case 'distance': { 
        let s = this.tripData.forEach(element => {
         sum += parseFloat(element.convertedDistance);

        });
        break;
      }case 'NumberOfVehicles': { 
        sum = this.tripData.length + 1;
        break;
      } case 'NumberOfTrips': { 
        let s = this.tripData.forEach(element => {
          sum += element.NumberOfTrips;
         });
        break;
      }  case 'AverageDistancePerDay': { 
        let s = this.tripData.forEach(element => {
         sum += parseFloat(element.convertedAverageDistance);
        });
        break;
      } case 'idleDuration': { 
        let s = this.tripData.forEach(element => {
          let time: any = 0;
          time += (element.idleDuration);
          let data: any = "00:00";
          let hours = Math.floor(time / 3600);
          time %= 3600;
          let minutes = Math.floor(time / 60);
          let seconds = time % 60;
          data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
          sum = data;
        });
        break;
      }
    }
    return sum; 
  }

  resetTripFormControlValue(){
    this.tripForm.get('vehicle').setValue('');
    this.tripForm.get('vehicleGroup').setValue(0);
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
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  onVehicleGroupChange(event: any){
    this.tripForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    if(parseInt(event.value) == 0){ //-- all group
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    }else{
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
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
    let _d = (date.getDate() < 10) ? ('0'+date.getDate()): date.getDate();
    let _m = ((date.getMonth()+1) < 10) ? ('0'+(date.getMonth()+1)): (date.getMonth()+1);
    let _y = (date.getFullYear() < 10) ? ('0'+date.getFullYear()): date.getFullYear();
    let _date: any;
    let _time: any;
    if(this.prefTimeFormat == 12){
      _time = (date.getHours() > 12 || (date.getHours() == 12 && date.getMinutes() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours()-12}:${m} PM` : `${(date.getHours() == 0) ? 12 : h}:${m} AM`;
    }else{
      _time = `${h}:${m}:${s}`;
    }
    switch(this.prefDateFormat){
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
      default:{
        _date = `${_m}/${_d}/${_y} ${_time}`;
      }
    }
    return _date;
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


  setPrefFormatTime(){
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = '00:00:00';
      this.endTimeDisplay = '23:59:59';
    }else{
      this.startTimeDisplay = '12:00 AM';
      this.endTimeDisplay = '11:59 PM';
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

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    //this.endDateValue = event.value._d;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
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

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
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

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
    //let todayDate = new Date();
    // let _date = moment.utc(todayDate.getTime());
    // let _tz = moment.utc().tz('Europe/London');
    // let __tz = moment.utc(todayDate.getTime()).tz('Europe/London').isDST();
    // var timedifference = new Date().getTimezoneOffset(); //-- difference from the clients timezone from UTC time.
    // let _tzOffset = this.getUtcOffset(todayDate);
    // let dt = moment(todayDate).toDate();
  }

  // getUtcOffset(date) {
  //   return moment(date)
  //     .subtract(
  //       moment(date).utcOffset(), 
  //       'seconds')
  //     .utc()
  // }

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

  exportAsExcelFile(){
    this.matTableExporter.exportTable('xlsx', {fileName:'Trip_Fleet_Utilisation', sheet: 'sheet_name'});
  }

  exportAsPDFFile(){
   
    var doc = new jsPDF();

    doc.setFontSize(18);
    doc.text('Trip Fleet Utilisation Details', 11, 8);
    doc.setFontSize(11);
    doc.setTextColor(100);

    let pdfColumns = [['Vehicle Name', 'VIN', 'RegPlateNumber', 'Distance', 'Number Of Trips', 'Trip Time', 'Driving Time', 'Idle Duration', 'Average Speed', 'Average Weight Per Trip', 'Average Distance Per Day', 'Odometer']];

  let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.vehicle_name);
      tempObj.push(e.vin);
      tempObj.push(e.RegPlateNumber);
      tempObj.push(e.convertedDistance);
      tempObj.push(e.NumberOfTrips);
      tempObj.push(e.convertedTripTime);
      tempObj.push(e.convertedDrivingTime);
      tempObj.push(e.convertedIdleDuration);
      tempObj.push(e.convertedAverageSpeed);
      tempObj.push(e.convertedAverageWeight);
      tempObj.push(e.convertedAverageDistance);
      tempObj.push(e.odometer);

      prepare.push(tempObj);
    });
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        //console.log(data.column.index)
      }
    })
    // below line for Download PDF document  
    doc.save('tripFleetUtilisationTable.pdf');

    
    // var data = document.getElementById('myChart');  
    // const divHeight = data.clientHeight
    // const divWidth = data.clientWidth
    // const ratio = divHeight / divWidth;
    // html2canvas(data,
    //   {
    //     height: window.outerHeight + window.innerHeight,
    //     width: window.outerWidth + window.innerWidth,
    //     windowHeight: window.outerHeight + window.innerHeight,
    //     windowWidth: window.outerWidth + window.innerWidth,
    //     scrollX: 0,
    //     scrollY: 0
    //   }
    //   ).then(canvas => {  
    //   var pdf = new jsPDF("l", "mm", "a4"); 
    //   var imgData = canvas.toDataURL('image/png');
    //   var width = pdf.internal.pageSize.getWidth();
    //   var height = pdf.internal.pageSize.getHeight();
    //   pdf.addImage(imgData, 'PNG', 0, 0, width, height*ratio); 
    //   pdf.save('tripFleetUtilisation.pdf'); 
    // });  

// To merge both pdf


  //   const merge = require('easy-pdf-merge');
  //   merge(['tripFleetUtilisationTable.pdf', 'tripFleetUtilisation.pdf'], 'Ouput.pdf', function (err) {
  //     if (err) {
  //         return console.log(err)
  //     }
  //     console.log('Successfully merged!')
  // });
  }

  pageSizeUpdated(_event) {
    // setTimeout(() => {
    //   document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    // }, 100);
  }

}
