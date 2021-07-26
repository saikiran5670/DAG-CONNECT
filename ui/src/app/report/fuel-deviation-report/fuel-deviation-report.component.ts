import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { ReportService } from '../../services/report.service';
import { TranslationService } from '../../services/translation.service';
import { OrganizationService } from '../../services/organization.service';
import { Util } from '../../shared/util';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ReportMapService } from '../report-map.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { CompleterItem, CompleterService } from 'ng2-completer';
import { ConfigService } from '@ngx-config/core';
import { HereService } from '../../services/here.service';

declare var H: any;

@Component({
  selector: 'app-fuel-deviation-report',
  templateUrl: './fuel-deviation-report.component.html',
  styleUrls: ['./fuel-deviation-report.component.less']
})

export class FuelDeviationReportComponent implements OnInit {
  dataService: any;
  searchMarker: any = {};
  vehicleIconMarker: any;
  searchStr: string = "";
  suggestionData: any;
  selectedMarker: any;
  defaultLayers: any;
  hereMap:any;
  ui: any;
  mapGroup : any;
  map: any;
  lat: any = '37.7397';
  lng: any = '-121.4252';
  @ViewChild("map")
  public mapElement: ElementRef;
  map_key: any = '';
  platform: any = '';
  searchExpandPanel: boolean = true;
  fuelDeviationData: any = [];
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  vehicleGrpDD: any = [];
  showMap: any = false;
  dataSource: any = new MatTableDataSource([]);
  selectedFuelDeviationEntry = new SelectionModel(true, []);
  showMapPanel: boolean = false;
  fuelDeviationForm: FormGroup;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  reportPrefData: any = [];
  fuelTableDetailsPrefData: any = [];
  fuelSummaryPrefData = [];
  fuelChartsPrefData = [];
  summaryExpandPanel: any = false;
  chartExpandPanel: any = false;
  showSummaryPanel: any = false;
  showChartPanel: any = false;
  vehicleDD: any = [];
  selectionTab: any;
  showLoadingIndicator: boolean = false;
  wholeFuelDeviationData: any = [];
  tableInfoObj: any = {};
  fuelDeviationReportId: any = 7; // hard coded for fuel deviation report pref.
  displayedColumns = ['All', 'vin', 'odometer', 'vehicleName', 'registrationNo', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed100Km', 'drivingTime', 'alert', 'events'];
  startDateValue: any;
  tableExpandPanel: boolean = true;
  last3MonthDate: any;
  todayDate: any;
  endDateValue: any;
  translationData: any;
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  globalSearchFilterData: any = JSON.parse(localStorage.getItem("globalSearchFilterData"));
  accountId: any;
  accountPrefObj: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  internalSelection: boolean = false;
  prefMapData: any = [
    {
      key: 'rp_fd_details_averageweight',
      value: 'averageweight'
    },
    {
      key: 'rp_fd_details_enddate',
      value: 'enddate'
    },
    {
      key: 'rp_fd_details_fuelconsumed',
      value: 'fuelconsumed'
    },
    {
      key: 'rp_fd_details_startdate',
      value: 'startdate'
    },
    {
      key: 'rp_fd_details_drivingtime',
      value: 'drivingtime'
    },
    {
      key: 'rp_fd_details_startposition',
      value: 'startposition'
    },
    {
      key: 'rp_fd_details_difference',
      value: 'difference'
    },
    {
      key: 'rp_fd_details_alerts',
      value: 'alerts'
    },
    {
      key: 'rp_fd_details_idleduration',
      value: 'idleduration'
    },
    {
      key: 'rp_fd_details_endposition',
      value: 'endposition'
    },
    {
      key: 'rp_fd_details_regplatenumber',
      value: 'regplatenumber'
    },
    {
      key: 'rp_fd_details_odometer',
      value: 'odometer'
    },
    {
      key: 'rp_fd_details_averagespeed',
      value: 'averagespeed'
    },
    {
      key: 'rp_fd_details_distance',
      value: 'distance'
    },
    {
      key: 'rp_fd_details_date',
      value: 'date'
    },
    {
      key: 'rp_fd_details_type',
      value: 'type'
    },
    {
      key: 'rp_fd_details_vin',
      value: 'vin'
    },
    {
      key: 'rp_fd_details_vehiclename',
      value: 'vehiclename'
    }
  ];

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private organizationService: OrganizationService, private _formBuilder: FormBuilder, private translationService: TranslationService, private reportService: ReportService, private reportMapService: ReportMapService, private completerService: CompleterService, private configService: ConfigService, private hereService: HereService) { 
    this.map_key = this.configService.getSettings("hereMap").api_key;
    this.platform = new H.service.Platform({
      "apikey": this.map_key // "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
    this.configureAutoSuggest();
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  ngOnDestroy() {
    this.globalSearchFilterData["vehicleGroupDropDownValue"] = this.fuelDeviationForm.controls.vehicleGroup.value;
    this.globalSearchFilterData["vehicleDropDownValue"] = this.fuelDeviationForm.controls.vehicle.value;
    this.globalSearchFilterData["timeRangeSelection"] = this.selectionTab;
    this.globalSearchFilterData["startDateStamp"] = this.startDateValue;
    this.globalSearchFilterData["endDateStamp"] = this.endDateValue;
    this.globalSearchFilterData.testDate = this.startDateValue;
    this.globalSearchFilterData.filterPrefTimeFormat = this.prefTimeFormat;
    if (this.prefTimeFormat == 24) {
      let _splitStartTime = this.startTimeDisplay.split(':');
      let _splitEndTime = this.endTimeDisplay.split(':');
      this.globalSearchFilterData["startTimeStamp"] = `${_splitStartTime[0]}:${_splitStartTime[1]}`;
      this.globalSearchFilterData["endTimeStamp"] = `${_splitEndTime[0]}:${_splitEndTime[1]}`;
    } else {
      this.globalSearchFilterData["startTimeStamp"] = this.startTimeDisplay;
      this.globalSearchFilterData["endTimeStamp"] = this.endTimeDisplay;
    }
    this.setGlobalSearchData(this.globalSearchFilterData);
  }

  setGlobalSearchData(globalSearchFilterData: any) {
    this.globalSearchFilterData["modifiedFrom"] = "TripReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  ngOnInit() {
    this.globalSearchFilterData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.fuelDeviationForm = this._formBuilder.group({
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
      menuId: 6 //-- for Fuel Deviation Report
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

   // Map Functions
   initMap(){
    this.defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      this.defaultLayers.raster.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      //center:{lat:41.881944, lng:-87.627778},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, this.defaultLayers);
    this.mapGroup = new H.map.Group();

    this.ui.removeControl("mapsettings");
    // create custom one
    var ms = new H.ui.MapSettingsControl( {
        baseLayers : [ { 
          label:"Normal", layer:this.defaultLayers.raster.normal.map
        },{
          label:"Satellite", layer:this.defaultLayers.raster.satellite.map
        }, {
          label:"Terrain", layer:this.defaultLayers.raster.terrain.map
        }
        ],
      layers : [{
            label: "Layer.Traffic", layer: this.defaultLayers.vector.normal.traffic
        },
        {
            label: "Layer.Incidents", layer: this.defaultLayers.vector.normal.trafficincidents
        }
    ]
      });
      this.ui.addControl("customized", ms);
  }

  clearRoutesFromMap(){
    this.hereMap.removeObjects(this.hereMap.getObjects());
    this.mapGroup.removeAll();
    this.vehicleIconMarker = null;
   }

  private configureAutoSuggest(){
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?'+'apiKey='+this.map_key +'&limit=5'+'&q='+searchParam;
    this.suggestionData = this.completerService.remote(
    URL,'title','title');
    this.suggestionData.dataField("items");
    this.dataService = this.suggestionData;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    ////console.log("process translationData:: ", this.translationData)
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
    this.getReportPreferences();
  }

  getReportPreferences(){
    this.reportService.getReportUserPreference(this.fuelDeviationReportId).subscribe((data : any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetFuelDeviationPrefData();
      this.getTranslatedColumnName(this.reportPrefData);
      this.setDisplayColumnBaseOnPref();
      this.loadFuelDeviationData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetFuelDeviationPrefData();
      this.setDisplayColumnBaseOnPref();
      this.loadFuelDeviationData();
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  loadFuelDeviationData(){
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeFuelDeviationData = tripData;
      this.filterDateData();
    }, (error)=>{
      this.hideloader();
      this.wholeFuelDeviationData.vinTripList = [];
      this.wholeFuelDeviationData.vehicleDetailsWithAccountVisibiltyList = [];
      this.filterDateData();
    });
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];
    let currentStartTime = Util.convertDateToUtc(this.startDateValue);
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); 
    if(this.wholeFuelDeviationData.vinTripList.length > 0){
      let filterVIN: any = this.wholeFuelDeviationData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
      if(filterVIN.length > 0){
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
        ////console.log("distinctVIN:: ", distinctVIN);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            let _item = this.wholeFuelDeviationData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element); 
            if(_item.length > 0){
              this.vehicleListData.push(_item[0]); //-- unique VIN data added 
              _item.forEach(element => {
                finalVINDataList.push(element)
              });
            }
          });
        }
      }else{
        this.fuelDeviationForm.get('vehicle').setValue('');
        this.fuelDeviationForm.get('vehicleGroup').setValue('');
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    if(this.vehicleGroupListData.length > 0){
      let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
      if(_s.length > 0){
        _s.forEach(element => {
          let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
          if(count.length > 0){
            this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
          }
        });
      }
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
    }
    this.vehicleDD = this.vehicleListData.slice();
    if(this.vehicleDD.length > 0){
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
      this.resetFuelDeviationFormControlValue();
    }
    this.setVehicleGroupAndVehiclePreSelection();
  }

  setVehicleGroupAndVehiclePreSelection() {
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      this.onVehicleGroupChange(this.globalSearchFilterData.vehicleGroupDropDownValue)
    }
  }

  setDisplayColumnBaseOnPref(){
    let filterPref = this.fuelTableDetailsPrefData.filter(i => i.state == 'I'); // removed unchecked
    if(filterPref.length > 0){
      filterPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key); // present or not
        if(search.length > 0){
          let index = this.displayedColumns.indexOf(search[0].value); // find index
          if (index > -1) {
            this.displayedColumns.splice(index, 1); // removed
          }
        }
      });
    }
  }

  getTranslatedColumnName(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.key.includes('rp_fd_summary_')){
              this.fuelSummaryPrefData.push(_data);
            }else if(item.key.includes('rp_fd_chart_')){
              let index: any;
              switch(item.key){
                case 'rp_fd_chart_fuelincreaseevents':{
                  index = 0;
                  break;
                }
                case 'rp_fd_chart_fueldecreaseevents':{
                  index = 1;
                  break;
                }
                case 'rp_fd_chart_fueldeviationevent':{
                  index = 2;
                  break;
                }
              }
              this.fuelChartsPrefData[index] = _data;
            }else if(item.key.includes('rp_fd_details_')){
              this.fuelTableDetailsPrefData.push(_data);
            }
          });
        }
      });
    }
  }

  resetFuelDeviationPrefData(){
    this.fuelSummaryPrefData = [];
    this.fuelChartsPrefData = [];
    this.fuelTableDetailsPrefData = [];
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      if(this.globalSearchFilterData.timeRangeSelection !== ""){
        this.selectionTab = this.globalSearchFilterData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.globalSearchFilterData.startDateStamp);
      let endDateFromSearch = new Date(this.globalSearchFilterData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
    }else {
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
  }
}

getLast3MonthDate(){
  var date = Util.getUTCDate(this.prefTimeZone);
  date.setMonth(date.getMonth()-3);
  return date;
}

getTodayDate(){
  let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
  return _todayDate;
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

changeEndDateEvent(event: MatDatepickerInputEvent<any>){
  this.internalSelection = true;
  this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
  this.resetFuelDeviationFormControlValue();
  this.filterDateData();
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
  }

  setPrefFormatTime(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "" && ((this.globalSearchFilterData.startTimeStamp || this.globalSearchFilterData.endTimeStamp) !== "") ) {
      if(this.prefTimeFormat == this.globalSearchFilterData.filterPrefTimeFormat){ // same format
        this.selectedStartTime = this.globalSearchFilterData.startTimeStamp;
        this.selectedEndTime = this.globalSearchFilterData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.startTimeStamp}:00` : this.globalSearchFilterData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.endTimeStamp}:59` : this.globalSearchFilterData.endTimeStamp;  
      }else{ // different format
        if(this.prefTimeFormat == 12){ // 12
          this.selectedStartTime = this._get12Time(this.globalSearchFilterData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.globalSearchFilterData.endTimeStamp);
          this.startTimeDisplay = this.selectedStartTime; 
          this.endTimeDisplay = this.selectedEndTime;
        }else{ // 24
          this.selectedStartTime = this.get24Time(this.globalSearchFilterData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.globalSearchFilterData.endTimeStamp);
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
    this.resetFuelDeviationFormControlValue(); 
    this.filterDateData(); 
  }

  resetFuelDeviationFormControlValue(){
    if(!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== ""){
      this.fuelDeviationForm.get('vehicle').setValue(this.globalSearchFilterData.vehicleDropDownValue);
      this.fuelDeviationForm.get('vehicleGroup').setValue(this.globalSearchFilterData.vehicleGroupDropDownValue);
    }else{
      this.fuelDeviationForm.get('vehicle').setValue(0);
      this.fuelDeviationForm.get('vehicleGroup').setValue(0);
    }
  }

  getYesterdaysDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-1);
    return date;
  }

  getLastWeekDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-7);
    return date;
  }

  getLastMonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-1);
    return date;
  }

  onVehicleGroupChange(event: any){
    let _val: any;
    if(event.value || event.value == 0){ // from internal veh-grp DD event
      this.internalSelection = true; 
      _val = parseInt(event.value); 
      this.fuelDeviationForm.get('vehicle').setValue(_val == 0 ? 0 : '');
    }
    else { // from local-storage
      _val = parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue);
      this.fuelDeviationForm.get('vehicleGroup').setValue(_val);
    }

    if(_val == 0){ //-- all group
      this.vehicleDD = [];
      this.vehicleDD = this.vehicleListData.slice();
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
    }else{
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == _val);
      if(search.length > 0){
        this.vehicleDD = [];
        search.forEach(element => {
          this.vehicleDD.push(element);  
        });
      }
    }
  }

  onVehicleChange(event: any){
    this.internalSelection = true;
  }

  onReset(){
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.fuelDeviationData = [];
    this.vehicleListData = [];
    this.updateDataSource(this.fuelDeviationData);
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
    this.tableInfoObj = {};
  }

  onSearch(){
    //this.internalSelection = true;
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _vinData: any = [];
    if(parseInt(this.fuelDeviationForm.controls.vehicle.value) == 0){ // all vin data
      _vinData = this.vehicleDD.filter(item => item.vehicleId != 0).map(i => i.vin);
    }else{ // single vin data
      _vinData = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.fuelDeviationForm.controls.vehicle.value)).map(i => i.vin);
    }
    if(_vinData.length > 0){
      this.showLoadingIndicator = true;
      let reportDataObj = {
        startDateTime: _startTime,
        endDateTime: _endTime,
        viNs: _vinData
      }
      this.reportService.getFuelDeviationReportDetails(reportDataObj).subscribe((_fuelDeviationData: any) => {
        //console.log(_fuelDeviationData);
        this.hideloader();
        //this.fuelDeviationData = this.reportMapService.getConvertedDataBasedOnPref(_fuelDeviationData.data, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
        //this.setTableInfo();
        this.updateDataSource(this.fuelDeviationData);
      }, (error)=>{
        //console.log(error);
        this.hideloader();
        this.fuelDeviationData = [];
        this.tableInfoObj = {};
        this.updateDataSource(this.fuelDeviationData);
      });
    }
  }

  setTableInfo(){
    let vehName: any = '';
    let vehGrpName: any = '';
    let vin: any = '';
    let plateNo: any = '';
    let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.fuelDeviationForm.controls.vehicleGroup.value));
    if(vehGrpCount.length > 0){
      vehGrpName = vehGrpCount[0].vehicleGroupName;
    }
    let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.fuelDeviationForm.controls.vehicle.value));
    if(vehCount.length > 0){
      vehName = vehCount[0].vehicleName;
      vin = vehCount[0].vin;
      plateNo = vehCount[0].registrationNo;
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

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.showMap = false;
    this.selectedFuelDeviationEntry.clear();
    if(this.initData.length > 0){
      this.showChartPanel = true;
      this.showSummaryPanel = true;
      if(!this.showMapPanel){ //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.initMap();
        }, 0);
      }else{
        this.clearRoutesFromMap();
      }
    }
    else{
      this.showMapPanel = false;
      this.showChartPanel = false;
      this.showSummaryPanel = false;
    }
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetFuelDeviationFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
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
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
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
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
  }

  onSearchFocus(){
    this.searchStr = null;
  }

  onSearchSelected(selectedAddress: CompleterItem){
    if(selectedAddress){
      let id = selectedAddress["originalObject"]["id"];
      let qParam = 'apiKey='+this.map_key + '&id='+ id;
      this.hereService.lookUpSuggestion(qParam).subscribe((data: any) => {
        this.searchMarker = {};
        if(data && data.position && data.position.lat && data.position.lng){
          let searchMarker = {
            lat: data.position.lat,
            lng: data.position.lng,
            from: 'search'
          }
          this.setMapToLocation(searchMarker);
        }
      });
    }
  }

  setMapToLocation(_position){
    this.hereMap.setCenter({lat: _position.lat, lng: _position.lng}, 'default');
  }

}
