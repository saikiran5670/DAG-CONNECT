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
import { filter } from 'rxjs/operators';

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
  displayedColumns = ['All', 'startDate', 'endDate', 'distance', 'idleDuration', 'avgSpeed', 'avgWeight', 'startPosition', 'endPosition', 'fuelConsumption', 'drivingTime', 'alerts', 'events'];
  translationData: any;
  hereMap: any;
  platform: any;
  ui: any;
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
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tripData: any = [];
  showLoadingIndicator: boolean = false;
  startDateValue: any;
  endDateValue: any;
  last3MonthDate: any;
  todayDate: any;
  wholeTripData: any = [];

  constructor(private translationService: TranslationService, private _formBuilder: FormBuilder, private reportService: ReportService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
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
     this.wholeTripData = tripData;
     this.filterDateData();
    });

    this.tripData = [{
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 128.9, 
      idleDuration: '00:12', 
      avgSpeed: 54.5, 
      avgWeight: 6.45,
      startPosition: 'DAF Nederland S',
      endPosition: 'DAF Nederland E',
      fuelConsumption: 123.5,
      drivingTime: '00:23',
      alerts: 20,
      events: 30
    },
    {
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 123.9, 
      idleDuration: '00:18', 
      avgSpeed: 32.5, 
      avgWeight: 7.45,
      startPosition: 'DAF Nederland S',
      endPosition: 'DAF Nederland E',
      fuelConsumption: 123.5,
      drivingTime: '00:23',
      alerts: 20,
      events: 30
    },
    {
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 18.9, 
      idleDuration: '00:02', 
      avgSpeed: 5.2, 
      avgWeight: 3.0,
      startPosition: 'DAF Nederland S',
      endPosition: 'DAF Nederland E',
      fuelConsumption: 123.5,
      drivingTime: '00:23',
      alerts: 20,
      events: 30
    },
    {
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 128.9, 
      idleDuration: '00:12', 
      avgSpeed: 54.5, 
      avgWeight: 6.45,
      startPosition: 'DAF Nederland S',
      endPosition: 'DAF Nederland E',
      fuelConsumption: 123.5,
      drivingTime: '00:23',
      alerts: 20,
      events: 30
    },
    {
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 123.9, 
      idleDuration: '00:18', 
      avgSpeed: 32.5, 
      avgWeight: 7.45,
      startPosition: 'DAF Nederland S',
      endPosition: 'DAF Nederland E',
      fuelConsumption: 123.5,
      drivingTime: '00:23',
      alerts: 20,
      events: 30
    }];
    this.updateDataSource(this.tripData);
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  public ngAfterViewInit() {
    // setTimeout(() => {
    // this.initMap();
    // }, 0);
  }

  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
  }

  onSearch(){
    let _startTime = this.startDateValue.getTime();
    let _endTime = this.endDateValue.getTime();
    let _vinData = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.tripForm.controls.vehicle.value));
    if(_vinData.length > 0){
      this.reportService.getTripDetails(_startTime, _endTime, _vinData[0].vin).subscribe((_tripData: any) => {
        console.log("current trip data:: ", _tripData);
      });
    }
  }

  onReset(){
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.filterDateData();
    this.tripData = [];
    this.resetTripFormControlValue();
  }

  resetTripFormControlValue(){
    this.tripForm.setValue({
      vehicle: '',
      vehicleGroup: ''
   });
  }

  onVehicleGroupChange(event: any){

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
    if(this.initData.length > 0){
      this.showMapPanel = true;
      setTimeout(() => {
        this.initMap();
      }, 0);
    }
    else{
      this.showMapPanel = false;
    }
    this.hideloader(); //-- hide loader
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  exportAsExcelFile(){

  }

  exportAsPDFFile(){
    
  }

  masterToggleForTrip() {
    if(this.isAllSelectedForTrip()){
      this.selectedTrip.clear();
    }
    else{
      this.dataSource.data.forEach((row) =>{
        this.selectedTrip.select(row);
      });
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
    if(event.checked){ 
      
    }else{

    }
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  startTimeChanged(selectedTime: any) {
    this.selectedStartTime = selectedTime;
  }

  endTimeChanged(selectedTime: any) {
    this.selectedEndTime = selectedTime;
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
        //this.startDateValue = this.getTodayDate();
        //this.endDateValue = this.getTodayDate();
        // this.tripForm.get('startDate').setValue(this.getTodayDate());
        // this.tripForm.get('endDate').setValue(this.getTodayDate());
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'yesterday': {
        this.selectionTab = 'yesterday';
        // this.startDateValue = this.getYesterdaysDate();
        // this.endDateValue = this.getTodayDate();
        // this.tripForm.get('startDate').setValue(this.getYesterdaysDate());
        // this.tripForm.get('endDate').setValue(this.getTodayDate());
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastweek': {
        this.selectionTab = 'lastweek';
        // this.startDateValue = this.getLastWeekDate();
        // this.endDateValue = this.getTodayDate();
        // this.tripForm.get('startDate').setValue(this.getLastWeekDate());
        // this.tripForm.get('endDate').setValue(this.getTodayDate());
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastWeekDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        // this.startDateValue = this.getLastMonthDate();
        // this.endDateValue = this.getTodayDate();
        // this.tripForm.get('startDate').setValue(this.getLastMonthDate());
        // this.tripForm.get('endDate').setValue(this.getTodayDate());
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastMonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        // this.startDateValue = this.getLast3MonthDate();
        // this.endDateValue = this.getTodayDate();
        // this.tripForm.get('startDate').setValue(this.getLast3MonthDate());
        // this.tripForm.get('endDate').setValue(this.getTodayDate());
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        break;
      }
    }
    this.filterDateData();
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<Date>){
    //console.log("start:: ", event.value)
    this.startDateValue = event.value;
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<Date>){
    //console.log("end: ", event.value)
    this.endDateValue = event.value;
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
    console.log(currentStartTime + "<->" + currentEndTime);
    let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
    if(filterVIN.length > 0){
      distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
      console.log("distinctVIN:: ", distinctVIN);
      if(distinctVIN.length > 0){
        distinctVIN.forEach(element => {
          let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element); 
          if(_item.length > 0){
            finalVINDataList.push(_item[0])
          }
        });
        console.log("finalVINDataList:: ", finalVINDataList); 
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    this.vehicleListData = finalVINDataList;
  }

}
