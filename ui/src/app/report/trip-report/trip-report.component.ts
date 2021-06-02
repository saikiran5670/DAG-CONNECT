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
import { ReportMapService } from './report-map.service';
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { POIService } from '../../services/poi.service';
//var jsPDF = require('jspdf');

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
  prefTimeFormat: any = 12; //-- coming from pref setting
  prefDateFormat: any = ''; //-- coming from pref setting
  accountPrefObj: any;
  advanceFilterOpen: boolean = false;
  userPOIList: any = [];
  
  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private poiService: POIService) {
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
      menuId: 6 //-- for Trip Report
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
        this.prefTimeFormat = parseInt(prefData.timeformat.filter(i => i.id == this.accountPrefObj.accountPreference.timeFormatId)[0].value.split(" ")[0]);
        this.prefDateFormat = prefData.dateformat.filter(i => i.id == this.accountPrefObj.accountPreference.dateFormatTypeId)[0].value;
        this.setDefaultStartEndTime();
        this.setPrefFormatDate();
        this.setDefaultTodayDate();
        this.loadWholeTripData();
      });
    });
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

  loadWholeTripData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeTripData = tripData;
      this.filterDateData();
      this.loadUserPOI();
    }, (error)=>{
      this.hideloader();
      let _tripData = {"code":200,"message":"VIN fetched successfully for given date range of 90 days","vinTripList":[{"vin":"XLR0998HGFFT5566","startTimeStamp":1616161806919,"endTimeStamp":1616161833919},{"vin":"XLR0998HGFFT5566","startTimeStamp":1616963841000,"endTimeStamp":1616964049000},{"vin":"XLR0998HGFFT5566","startTimeStamp":1620654606919,"endTimeStamp":1616914996000},{"vin":"XLR0998HGFFT5566","startTimeStamp":1621432206919,"endTimeStamp":1621432217919},{"vin":"XLR0998HGFFT5566","startTimeStamp":1622166606919,"endTimeStamp":1622166802919},{"vin":"XLRAS47MS0E808088","startTimeStamp":1621432206919,"endTimeStamp":1621432351919},{"vin":"XLRAS47MS0E808088","startTimeStamp":1622166606919,"endTimeStamp":1622166632919}],"vehicleDetailsWithAccountVisibiltyList":[{"vehicleGroupId":0,"accountId":40,"objectType":"V","groupType":"S","functionEnum":"","organizationId":12,"accessType":"F","vehicleGroupName":"","vehicleId":32,"vehicleName":"08: FFT7788","vin":"XLR0998HGFFT7788","registrationNo":"1-DAF-23.989898"},{"vehicleGroupId":16,"accountId":40,"objectType":"V","groupType":"G","functionEnum":"N","organizationId":12,"accessType":"F","vehicleGroupName":"JTC Fleet Group","vehicleId":31,"vehicleName":"Rolls Royce","vin":"XLR0998HGFFT5566","registrationNo":"PLOI097OII"},{"vehicleGroupId":16,"accountId":40,"objectType":"V","groupType":"G","functionEnum":"N","organizationId":12,"accessType":"F","vehicleGroupName":"JTC Fleet Group","vehicleId":32,"vehicleName":"08: FFT7788","vin":"XLR0998HGFFT7788","registrationNo":"1-DAF-23.989898"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":31,"vehicleName":"Rolls Royce","vin":"XLR0998HGFFT5566","registrationNo":"PLOI097OII"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":32,"vehicleName":"08: FFT7788","vin":"XLR0998HGFFT7788","registrationNo":"1-DAF-23.989898"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":41,"vehicleName":"Mercedes Benz","vin":"XLRAS47MS0E808088","registrationNo":"XCVFDG23544"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":42,"vehicleName":"22:","vin":"XLRAS47MS0E808099","registrationNo":"Pb-016-V"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":43,"vehicleName":"14:","vin":"XLRAS47MS0E808000","registrationNo":"XCvFDG23544"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":45,"vehicleName":"16: Vehicle_V2","vin":"XLRAS47MS0E808011","registrationNo":"XCVFDG237845"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":68,"vehicleName":"23: Vehicle_VDemo","vin":"XLRAS47MS0E808321","registrationNo":"XCVFDG2332112"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester","vehicleId":87,"vehicleName":"24:","vin":"XLRAS47MS0E808399","registrationNo":"XCVFDG23321"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":31,"vehicleName":"Rolls Royce","vin":"XLR0998HGFFT5566","registrationNo":"PLOI097OII"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":32,"vehicleName":"08: FFT7788","vin":"XLR0998HGFFT7788","registrationNo":"1-DAF-23.989898"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":41,"vehicleName":"Mercedes Benz","vin":"XLRAS47MS0E808088","registrationNo":"XCVFDG23544"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":42,"vehicleName":"22:","vin":"XLRAS47MS0E808099","registrationNo":"Pb-016-V"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":43,"vehicleName":"14:","vin":"XLRAS47MS0E808000","registrationNo":"XCvFDG23544"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":45,"vehicleName":"16: Vehicle_V2","vin":"XLRAS47MS0E808011","registrationNo":"XCVFDG237845"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":68,"vehicleName":"23: Vehicle_VDemo","vin":"XLRAS47MS0E808321","registrationNo":"XCVFDG2332112"},{"vehicleGroupId":22,"accountId":40,"objectType":"V","groupType":"D","functionEnum":"O","organizationId":12,"accessType":"V","vehicleGroupName":"VG001Tester","vehicleId":87,"vehicleName":"24:","vin":"XLRAS47MS0E808399","registrationNo":"XCVFDG23321"},{"vehicleGroupId":23,"accountId":40,"objectType":"V","groupType":"G","functionEnum":"","organizationId":12,"accessType":"F","vehicleGroupName":"VG001Tester123","vehicleId":0,"vehicleName":"","vin":"","registrationNo":""},{"vehicleGroupId":24,"accountId":40,"objectType":"V","groupType":"G","functionEnum":"","organizationId":12,"accessType":"F","vehicleGroupName":"VG002Tester","vehicleId":0,"vehicleName":"","vin":"","registrationNo":""},{"vehicleGroupId":24,"accountId":40,"objectType":"V","groupType":"G","functionEnum":"","organizationId":12,"accessType":"V","vehicleGroupName":"VG002Tester","vehicleId":0,"vehicleName":"","vin":"","registrationNo":""}]};
      this.wholeTripData = _tripData;
      this.filterDateData();
      this.loadUserPOI();
    });
  }

  loadUserPOI(){
    this.poiService.getPois(this.accountOrganizationId).subscribe((poiData: any) => {
      this.userPOIList = poiData; 
    }, (error) => {
      this.userPOIList = [];
    });
  }

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
        this.tripData = this.getConvertTableDateTime(_tripData.tripData);
        this.setTableInfo();
        this.updateDataSource(this.tripData);
      }, (error)=>{
        //console.log(error);
        this.hideloader();
        this.tripData = [];
        //this.tableInfoObj = {};
        let _tripData = [{"id":11903,"tripId":"ae6e42d3-4ba1-49eb-8fe1-704a2271bc49","vin":"XLR0998HGFFT5566","startTimeStamp":1587143959831,"endTimeStamp":1587143959831,"distance":139,"idleDuration":53,"averageSpeed":2663,"averageWeight":655350,"odometer":298850780,"startPosition":"NA","endPosition":"NA","fuelConsumed":166.896551724138,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.66896551724138,"liveFleetPosition":[],"startPositionLattitude":50.96831131,"startPositionLongitude":-1.388581276,"endPositionLattitude":50.9678421,"endPositionLongitude":-1.388695598},{"id":12576,"tripId":"11c2c2c1-c56f-42ce-9e62-31685ed5d2ae","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":22,"idleDuration":3,"averageSpeed":6545,"averageWeight":655350,"odometer":298981400,"startPosition":"NA","endPosition":"NA","fuelConsumed":50,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":0.5,"liveFleetPosition":[],"startPositionLattitude":50.81643677,"startPositionLongitude":-0.7481001616,"endPositionLattitude":50.81661987,"endPositionLongitude":-0.74804914},{"id":13407,"tripId":"ce3c49fb-2291-4052-9d1b-3b2ee343ab33","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":213,"idleDuration":39,"averageSpeed":3461,"averageWeight":655350,"odometer":74677630,"startPosition":"NA","endPosition":"NA","fuelConsumed":91,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":0.91,"liveFleetPosition":[],"startPositionLattitude":51.39526367,"startPositionLongitude":-1.229614377,"endPositionLattitude":51.39541626,"endPositionLongitude":-1.231176734},{"id":12582,"tripId":"6adb296a-549e-4d50-af9d-3bfbc6fc3e4b","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":4,"idleDuration":14,"averageSpeed":3130,"averageWeight":655350,"odometer":10327065,"startPosition":"NA","endPosition":"NA","fuelConsumed":175,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.75,"liveFleetPosition":[],"startPositionLattitude":41.71875763,"startPositionLongitude":26.35817528,"endPositionLattitude":41.71875,"endPositionLongitude":26.35810089},{"id":12587,"tripId":"cc5b9533-1d94-4af8-8cc8-903b0dcd5514","vin":"XLR0998HGFFT5566","startTimeStamp":0,"endTimeStamp":0,"distance":65,"idleDuration":10,"averageSpeed":8000,"averageWeight":655350,"odometer":14747690,"startPosition":"NA","endPosition":"NA","fuelConsumed":105,"drivingTime":0,"alert":0,"events":0,"fuelConsumed100Km":1.05,"liveFleetPosition":[],"startPositionLattitude":43.00225067,"startPositionLongitude":22.80965805,"endPositionLattitude":43.00209045,"endPositionLongitude":22.8104248}];
        this.tripData = this.getConvertTableDateTime(_tripData);
        this.setTableInfo();
        this.updateDataSource(this.tripData);
      });
    }
  }

  getConvertTableDateTime(data: any){
    data.forEach(element => {
      if(element.startTimeStamp != 0){
        element.convertedStartTime = this.formStartDate(new Date(element.startTimeStamp));
      }
      else{
        element.convertedStartTime = 0;
      }

      if(element.endTimeStamp != 0){
        element.convertedEndTime = this.formStartDate(new Date(element.endTimeStamp));
      }else{
        element.convertedEndTime = 0;
      }
    });
    return data;
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
    switch(this.prefDateFormat){
      case 'dd/mm/yyyy': {
        _date = `${_d}/${_m}/${_y} ${h}:${m}:${s}`;
        break;
      }
      case 'mm/dd/yyyy': {
        _date = `${_m}/${_d}/${_y} ${h}:${m}:${s}`;
        break;
      }
      case 'dd-mm-yyyy': {
        _date = `${_d}-${-m}-${_y} ${h}:${m}:${s}`;
        break;
      }
      case 'mm-dd-yyyy': {
        _date = `${_m}-${_d}-${_y} ${h}:${m}:${s}`;
        break;
      }
      default:{
        _date = `${_m}/${_d}/${_y} ${h}:${m}:${s}`;
      }
    }
    return _date;
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
  }

  resetTripFormControlValue(){
    this.tripForm.get('vehicle').setValue('');
    this.tripForm.get('vehicleGroup').setValue(0);
  }

  onVehicleGroupChange(event: any){
    this.tripForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    if(parseInt(event.value) == 0){ //-- all group
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
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
    this.matTableExporter.exportTable('xlsx', {fileName:'Trip_Report', sheet: 'sheet_name'});
  }

  exportAsPDFFile(){
   
    var doc = new jsPDF();

    doc.setFontSize(18);
    doc.text('Trip Details', 11, 8);
    doc.setFontSize(11);
    doc.setTextColor(100);

    let pdfColumns = [['Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

  let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.convertedStartTime);
      tempObj.push(e.convertedEndTime);
      tempObj.push( e.distance);
      tempObj.push( e.idleDuration);
      tempObj.push( e.averageSpeed);
      tempObj.push(e.averageWeight);
      tempObj.push(e.startPosition);
      tempObj.push(e.endPosition);
      tempObj.push(e.fuelConsumed100Km);
      tempObj.push(e.drivingTime);
      tempObj.push(e.alert);
      tempObj.push(e.events);

      prepare.push(tempObj);
    });
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        console.log(data.column.index)
      }
    })
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

  setStartEndDateTime(date: any, timeObj: any, type: any){
    date.setHours(timeObj.split(":")[0]);
    date.setMinutes(timeObj.split(":")[1]);
    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    let _last3m = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let _yesterday = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    let currentStartTime = _last3m.getTime();
    let currentEndTime = _yesterday.getTime();
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

  onAdvanceFilterOpen(){
    this.advanceFilterOpen = !this.advanceFilterOpen;
  }

  onDisplayChange(event: any){

  }

  changeUserPOISelection(event: any, poiData: any){
    //console.log(this.selectedPOI.selected);
  }

  onMapModeChange(event: any){

  }

  onMapRepresentationChange(event: any){

  }
}
