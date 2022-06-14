import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChildren, ViewChild, ElementRef, Inject } from '@angular/core';
import { FormBuilder, FormGroup,FormControl, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DomSanitizer } from '@angular/platform-browser';
import { AlertService } from 'src/app/services/alert.service';
import { CorridorService } from 'src/app/services/corridor.service';
import { GeofenceService } from 'src/app/services/landmarkGeofence.service';
import { LandmarkGroupService } from 'src/app/services/landmarkGroup.service';
import { POIService } from 'src/app/services/poi.service';
import { CommonTableComponent } from 'src/app/shared/common-table/common-table.component';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { CustomValidators } from 'src/app/shared/custom.validators';
import { CreateNotificationsAlertComponent } from './create-notifications-alert/create-notifications-alert.component';
import { Options } from '@angular-slider/ngx-slider';
import { PeriodSelectionFilterComponent } from './period-selection-filter/period-selection-filter.component';
import { AlertAdvancedFilterComponent } from './alert-advanced-filter/alert-advanced-filter.component';
import { ReportMapService } from '../../../report/report-map.service';
import { TranslationService } from '../../../services/translation.service';
import { OrganizationService } from '../../../services/organization.service';
import { SimpleChanges } from '@angular/core';
import { DataTableComponent } from 'src/app/shared/data-table/data-table.component';
import { ConfigService } from '@ngx-config/core';
import { ReplaySubject } from 'rxjs';
import { Util } from 'src/app/shared/util';
import * as moment from 'moment';
import { MapFunctionsService } from '../../landmarks/manage-corridor/map-functions.service';
import { MAT_DATE_FORMATS } from '@angular/material/core';
declare var H: any;

@Component({
  selector: 'app-create-edit-view-alerts',
  templateUrl: './create-edit-view-alerts.component.html',
  styleUrls: ['./create-edit-view-alerts.component.less']
})
export class CreateEditViewAlertsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() actionType: any;
  @Input() translationData: any = {};
  @Input() selectedRowData: any;
  @Input() alertCategoryTypeMasterData: any = [];
  @Input() alertCategoryTypeFilterData: any = [];
  @Input() associatedVehicleData: any = [];
  @Input() finalVehicleGroupList: any = [];
  @Input() vehicleDisplayPreference: any = '';
  alertCategoryList: any = [];
  alertTypeList: any = [];
  vehicleGroupList: any = [];
  vehicleList: any = [];
  accountInfo:any = {};
  initData: any = [];
  // columnCodes = ['vin','vehicleName', 'vehicleGroupName', 'subscriptionType'];
  // columnLabels = ['VIN','VehicleName', 'VehicleGroupName', 'Status'];
  columnCodes = ['vin','vehicleName', 'subscriptionType'];
  columnLabels = ['VIN','VehicleName', 'Status'];
  @ViewChild('gridComp') gridComp: DataTableComponent;
 
  options: Options = {
    floor: 0,
    ceil: 100000
  };
  alertTypeObject: any;
  displayedColumnsVehicles: string[] = ['vin', 'vehicleName', 'vehicleGroupName', 'subcriptionStatus']
  displayedColumnsPOI: string[] = ['select', 'icon', 'name', 'categoryName', 'subCategoryName', 'address'];
  displayedColumnsGeofence: string[] = ['select', 'name', 'categoryName', 'subCategoryName', 'address'];
  displayedColumnsGroup: string[] = ['select', 'name', 'poiCount', 'geofenceCount'];
  displayedColumnsCorridor: string[] = ['select', 'corridoreName', 'startPoint', 'endPoint', 'distance', 'width'];
  selectedPOI = new SelectionModel(true, []);
  selectedGeofence = new SelectionModel(true, []);
  selectedGroup = new SelectionModel(true, []);
  selectedCorridor = new SelectionModel(true, []);
  vehiclesDataSource: any = new MatTableDataSource([]);
  poiDataSource: any = new MatTableDataSource([]);
  geofenceDataSource: any = new MatTableDataSource([]);
  groupDataSource: any = new MatTableDataSource([]);
  corridorDataSource: any = new MatTableDataSource([]);
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  dialogRef: MatDialogRef<CommonTableComponent>;
  alertCreatedMsg: any = '';
  breadcumMsg: any = '';
  alertForm: FormGroup;
  accountOrganizationId: number;
  accountId: number;
  accountRoleId: number;
  userType: string;
  selectedApplyOn: string;
  selectedApplyOnPeriod: string = 'A';
  criticalLevel: boolean= false;
  warningLevel: boolean= false;
  advisoryLevel: boolean= false;
  openAdvancedFilter: boolean= false;
  poiGridData = [];
  geofenceGridData = [];
  groupGridData = [];
  corridorGridData = [];
  selectedPOIList: any = [];
  selectedGeofenceList: any = [];
  selectedGroupList: any = [];
  selectedCorridorList: any = [];
  isDuplicateAlert: boolean= false;
  private platform: any;
  map: any;
  geofenceData: any;
  marker: any;
  markerArray: any = [];
  geoMarkerArray: any = [];
  polyPoints: any = [];
  alertTypeByCategoryList: any= [];
  vehicleByVehGroupList: any= [];
  vehicleListForTable: any= [];
  alert_category_selected: string= '';
  alert_type_selected: string= '';
  vehicle_group_selected: number;
  alertCategoryName: string= '';
  alertTypeName: string= '';
  isCriticalLevelSelected: boolean= false;
  isWarningLevelSelected: boolean= false;
  isAdvisoryLevelSelected: boolean= false;
  labelForThreshold: string= '';
  unitForThreshold: string= '';
  unitTypeEnum: string= '';
  panelOpenState: boolean = false;
  notifications: any= [];
  unitTypes: any= [];
  isUnsubscribedVehicle: boolean= false;
  poiWidth : number = 100;
  poiWidthKm : number = 0.1;
  sliderValue : number = 0;
  alertFeatures: any= [];
  periodForm: any;
  alertFilterRefs: any = [];
  ui: any;
  from: any;
  to: any;
  isExpandedOpen: boolean = false;
  isExpandedOpenAlert: boolean = true;
  filterDetailsErrorMsg:any= '';
  filterDetailsCheck:boolean = false;
  isNotificationFormValid: boolean= true;
  isNotifyEmailValid:  boolean= true;
  isAdvancedAlertPayload:  boolean= true;
  isFormValidate :  boolean= true;
  isEnteringZone: boolean= true;
  isValidityCalender: boolean= true;
  isFiltersDetailsValidate: boolean= true;
  criticalThreshold: any ='';
  warningThreshold: any ='';
  advisoryThreshold: any ='';
  localStLanguage: any;
  accountPrefObj: any;
  vehicleGrpByVehicleList: any = [];
  internalSelection: boolean = false;
  globalSearchFilterData: any = JSON.parse(localStorage.getItem("globalSearchFilterData"));
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  selectionTab: any;
  startDateValue: any;
  endDateValue: any;
  //last3MonthDate: any;
  todayDate: any;

  @Input() prefTimeFormat: any; //-- coming from pref setting
  @Input() prefTimeZone: any; //-- coming from pref setting
  @Input() prefDateFormat: any; //-- coming from pref setting
  @Input() prefUnitFormat: any; //-- coming from pref setting
  map_key: any = '';
  singleVehicle = [];
  showLoadingIndicator: boolean = false;
  periodSelectedDateTime = [];

  @ViewChild(CreateNotificationsAlertComponent)
  notificationComponent: CreateNotificationsAlertComponent;

  @ViewChild(PeriodSelectionFilterComponent)
  periodSelectionComponent: PeriodSelectionFilterComponent;

  @ViewChild(AlertAdvancedFilterComponent)
  alertAdvancedComponent: AlertAdvancedFilterComponent;

  typesOfLevel: any= [
                      {
                        levelType : 'C',
                        value: 'Critical'
                      },
                      {
                        levelType : 'W',
                        value: 'Warning'
                      }, 
                      {
                        levelType : 'A',
                        value: 'Advisory'
                      }
                    ];

  
  @ViewChild("map")
  private mapElement: ElementRef;
  
  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  
  public filteredVehicles: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  
  
  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private _formBuilder: FormBuilder,
              private poiService: POIService,
              private geofenceService: GeofenceService, 
              private landmarkGroupService: LandmarkGroupService, 
              private domSanitizer: DomSanitizer, 
              private dialog: MatDialog,
              private alertService: AlertService,
              private corridorService: CorridorService,
              private dialogService: ConfirmDialogService,
              private el: ElementRef,
              private reportMapService: ReportMapService,
              private translationService: TranslationService,
              private organizationService: OrganizationService,
              private _configService: ConfigService,
              private mapFunctions: MapFunctionsService ) 
  {
    // this.map_key = _configService.getSettings("hereMap").api_key;
    this.map_key = localStorage.getItem("hereMapsK");
    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });  
   }

  ngOnInit() {
    //this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    if(localStorage.getItem('contextOrgId')){
      this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
    }
    else{
      this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    } 
    let today = new Date();
    let nextWeek = new Date(today.getFullYear(), today.getMonth(), today.getDate()+7);
  
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.alertForm = this._formBuilder.group({
      alertName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      alertCategory: ['', [Validators.required]],
      alertType: ['', [Validators.required]],
      applyOn: ['G', [Validators.required]],
      // vehicleGroup: [''],
      vehicleGroup: [''],
      vehicle: [''],
      statusMode: ['A', [Validators.required]],
      alertLevel: ['C', [Validators.required]],
      criticalLevelThreshold: [''],
      warningLevelThreshold: [''],
      advisoryLevelThreshold: [''],
      mondayPeriod: [''],
      unitType: [''],
      widthInput: [''],
      searchForLevelPOI: [''],
      alertLevelValue:[''],
      searchCorridor:[''],
      levelType: [''],
      fullorCustom: ['A'],
      fromDate: new FormControl(today),
      fromTimeRange: ['00:00'],
      toDate: new FormControl(nextWeek),
      toTimeRange:['23:59']
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('alertName')  
      ]
    });

    // this.loadFilterDataBasedOnPrivileges();

    if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'create'){
      this.breadcumMsg = this.getBreadcum();
    }

    this.selectedApplyOn = 'G';
    this.alertForm.controls.widthInput.setValue(0.1);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    let _langCode = this.localStLanguage ? this.localStLanguage.code  :  "EN-GB";
    
    this.showLoadingIndicator = true;
    this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
      if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){ // account pref
        this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        this.showLoadingIndicator = false;
      }else{ // org pref
        this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
          this.proceedStep(prefData, orgPref);
          this.showLoadingIndicator = false;
        }, (error) => { // failed org API
          this.showLoadingIndicator = false;
          let pref: any = {};
          this.proceedStep(prefData, pref);
        });
      }

      // this.loadFilterDataBasedOnPrivileges();
      let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
      if(vehicleDisplayId) {
        let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        if(vehicledisplay.length != 0) {
          this.vehicleDisplayPreference = vehicledisplay[0].name;
        }
      }  
      if(this.actionType == 'create')
      {
        this.sliderChanged();
      }
    });
    if(this.finalVehicleGroupList && this.finalVehicleGroupList.length > 0){
      this.showLoadingIndicator = true;
      let _vehGrpList: any = {
        'vehicleGroupIds': this.finalVehicleGroupList.map(i => i.vehicleGroupId)
      } 
      this.alertService.getSubscribeNonSubsucribeVehicles(_vehGrpList).subscribe((vehicleGrpList: any) => {
        if(vehicleGrpList && vehicleGrpList.vehicleandGroupDetails && vehicleGrpList.vehicleandGroupDetails.length > 0){
          this.vehicleGrpByVehicleList = vehicleGrpList.vehicleandGroupDetails.slice();
        }
        this.stepForword();
        this.showLoadingIndicator = false;
      }, (error) => {
        this.stepForword();
        this.showLoadingIndicator = false;
      });
    }

    if (this.actionType == 'view') {
      this.openAdvancedFilter = true;
      if(this.selectedRowData.alertLandmarkRefs.length > 0){
        this.poiWidth =this.selectedRowData.alertLandmarkRefs[0].distance;
        this.sliderChanged();
      }
    }
  }

  stepForword(){
    this.loadFilterDataBasedOnPrivileges();
    if(this.actionType == 'create'){
      this.sliderChanged();
    }
  }

public ngAfterViewInit() {
  
}
  
proceedStep(prefData: any, preference: any){
  let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
  if(_search.length > 0){
    //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
    this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
    //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
    this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
    this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
    this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
  }else{
    //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
    this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
    //this.prefTimeZone = prefData.timezone[0].value;
    this.prefTimeZone = prefData.timezone[0].name;
    this.prefDateFormat = prefData.dateformat[0].name;
    this.prefUnitFormat = prefData.unit[0].name;
  }
  this.setDefaultStartEndTime();
  this.setPrefFormatDate();
  this.setDefaultTodayDate();

  //console.log(this.prefUnitFormat);
}
setDefaultStartEndTime() {
  this.setPrefFormatTime();
}
setPrefFormatTime() {
  if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "" && ((this.globalSearchFilterData.startTimeStamp || this.globalSearchFilterData.endTimeStamp) !== "")) {
    if (this.prefTimeFormat == this.globalSearchFilterData.filterPrefTimeFormat) { // same format
      this.selectedStartTime = this.globalSearchFilterData.startTimeStamp;
      this.selectedEndTime = this.globalSearchFilterData.endTimeStamp;
      this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.startTimeStamp}:00` : this.globalSearchFilterData.startTimeStamp;
      this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.endTimeStamp}:59` : this.globalSearchFilterData.endTimeStamp;
    } else { // different format
      if (this.prefTimeFormat == 12) { // 12
        this.selectedStartTime = this._get12Time(this.globalSearchFilterData.startTimeStamp);
        this.selectedEndTime = this._get12Time(this.globalSearchFilterData.endTimeStamp);
        this.startTimeDisplay = `${this.selectedStartTime}:00 AM`;
        this.endTimeDisplay =  `${this.selectedEndTime}:59 PM`;
      } else { // 24
        this.selectedStartTime = this.get24Time(this.globalSearchFilterData.startTimeStamp);
        this.selectedEndTime = this.get24Time(this.globalSearchFilterData.endTimeStamp);
        this.startTimeDisplay = `${this.selectedStartTime}:00`;
        this.endTimeDisplay = `${this.selectedEndTime}:59`;
      }
    }
  } else {
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = '00:00:00';
      this.endTimeDisplay = '23:59:59';
      this.selectedStartTime = "00:00";
      this.selectedEndTime = "23:59";
    } else {
      this.startTimeDisplay = '12:00:00 AM';
      this.endTimeDisplay = '11:59:59 PM';
      this.selectedStartTime = "12:00 AM";
      this.selectedEndTime = "11:59 PM";
    }
  }

}

_get12Time(_sTime: any) {
  let _x = _sTime.split(':');
  let _yy: any = '';
  if (_x[0] >= 12) { // 12 or > 12
    if (_x[0] == 12) { // exact 12
      _yy = `${_x[0]}:${_x[1]} PM`;
    } else { // > 12
      let _xx = (_x[0] - 12);
      _yy = `${_xx}:${_x[1]} PM`;
    }
  } else { // < 12
    _yy = `${_x[0]}:${_x[1]} AM`;
  }
  return _yy;
}

get24Time(_time: any) {
  let _x = _time.split(':');
  let _y = _x[1].split(' ');
  let res: any = '';
  if (_y[1] == 'PM') { // PM
    let _z: any = parseInt(_x[0]) + 12;
    res = `${(_x[0] == 12) ? _x[0] : _z}:${_y[0]}`;
  } else { // AM
    res = `${_x[0]}:${_y[0]}`;
  }
  return res;
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
setDefaultTodayDate() {
  if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
    if (this.globalSearchFilterData.timeRangeSelection !== "") {
      this.selectionTab = this.globalSearchFilterData.timeRangeSelection;
    } else {
      this.selectionTab = 'today';
    }
    let startDateFromSearch = new Date(this.globalSearchFilterData.startDateStamp);
    let endDateFromSearch = new Date(this.globalSearchFilterData.endDateStamp);
    this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
    //this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
  } else {
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
   // this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
  }
}
getTodayDate() {
  let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
  _todayDate.setHours(0);
  _todayDate.setMinutes(0);
  _todayDate.setSeconds(0);
  return _todayDate;
}

  toBack() {
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.backToPage.emit(emitObj);
  }
  getUnique(arr, comp) {

    // store the comparison  values in array
    const unique =  arr.map(e => e[comp])

      // store the indexes of the unique objects
      .map((e, i, final) => final.indexOf(e) === i && i)

      // eliminate the false indexes & return unique objects
    .filter((e) => arr[e]).map(e => arr[e]);

    return unique;
  }

  loadFilterDataBasedOnPrivileges(){  
    //this.alertService.getAlertFilterDataBasedOnPrivileges(this.accountId, this.accountRoleId).subscribe((data: any) => {
      // this.alertCategoryTypeMasterData = data["enumTranslation"];
      // this.alertCategoryTypeFilterData = data["alertCategoryFilterRequest"];
      // this.associatedVehicleData = data["associatedVehicleRequest"];

      let alertTypeMap = new Map();
      this.alertCategoryTypeFilterData.forEach(element => {
        alertTypeMap.set(element.featureKey, element.featureKey);
      });

      if(alertTypeMap != undefined){
        this.alertCategoryTypeMasterData.forEach(element => {
          if(alertTypeMap.get(element.key)){
            element["value"]= this.translationData[element["key"]];
            this.alertTypeList.push(element);
          }
        });
      }
     
      if(this.alertTypeList.length != 0){
        this.alertCategoryTypeMasterData.forEach(element => {
          this.alertTypeList.forEach(item => {
            if(item.parentEnum == element.enum && element.parentEnum == ""){
              element["value"]= this.translationData[element["key"]];
              this.alertCategoryList.push(element);
            }
          });
        });
        this.alertCategoryList = this.getUnique(this.alertCategoryList, "enum")
      }

      if(this.actionType == 'edit' || this.actionType == 'duplicate'){
        this.selectedApplyOn = this.selectedRowData.applyOn;
        this.setDefaultValue();
        if(this.selectedRowData.notifications.length != 0)
        this.panelOpenState= true;
      }
      else if(this.actionType == 'view'){
        this.alert_category_selected = this.selectedRowData.category;
        this.selectedApplyOn = this.selectedRowData.applyOn;
        this.alertCategoryName = this.translationData[this.alertCategoryTypeMasterData.filter(item => item.enum == this.alert_category_selected)[0].key];
        this.alertTypeName = this.translationData[this.alertCategoryTypeMasterData.filter(item => (item.enum == this.selectedRowData.type && item.parentEnum == this.alert_category_selected))[0].key];
        this.onChangeAlertType(this.selectedRowData.type);
        if(this.selectedRowData.alertUrgencyLevelRefs[0].periodType == 'C'){
          this.from = this.formStartDate(new Date(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelStartDate),  this.prefTimeFormat, this.prefDateFormat).split(" ");
          this.to = this.formStartDate(new Date(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelEndDate),  this.prefTimeFormat, this.prefDateFormat).split(" ");
        }
        this.convertValuesToPrefUnit();
        if(this.selectedRowData.notifications.length != 0)
          this.panelOpenState= true;
      }
      //this.showLoadingIndicator = false;
    //});
  }

  formStartDate(date: any, prefTimeFormat: any, prefDateFormat:any) {
    let h = (date.getHours() < 10) ? ('0' + date.getHours()) : date.getHours();
    let m = (date.getMinutes() < 10) ? ('0' + date.getMinutes()) : date.getMinutes();
    let s = (date.getSeconds() < 10) ? ('0' + date.getSeconds()) : date.getSeconds();
    let _d = (date.getDate() < 10) ? ('0' + date.getDate()) : date.getDate();
    let _m = ((date.getMonth() + 1) < 10) ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1);
    let _y = (date.getFullYear() < 10) ? ('0' + date.getFullYear()) : date.getFullYear();
    let _date: any;
    let _time: any;
    if (prefTimeFormat == 12) {
      if (date.getHours() == 12) {
        _time = ((date.getHours() == 12 || date.getMinutes() > 0 || date.getSeconds() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours() - 12}:${m}:${s} PM` : `${(date.getHours() == 0) ? 12 : h}:${m}:${s} AM`;
      }
      else {
        _time = (date.getHours() > 12 || (date.getHours() == 12 && date.getMinutes() > 0 && date.getSeconds() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours() - 12}:${m}:${s} PM` : `${(date.getHours() == 0) ? 12 : h}:${m}:${s} AM`;
      }
    }
    else {
      _time = `${h}:${m}:${s}`;
    }
    switch (prefDateFormat) {
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
      default: {
        _date = `${_m}/${_d}/${_y} ${_time}`;
      }
    }
    return _date;
  }

  updateVehiclesDataSource(tableData: any){
    // tableData.forEach(element =>{
    //   element.subscriptionType = "Subscribed";
    // });
    this.gridComp.updatedTableData(tableData);
    // this.vehiclesDataSource= new MatTableDataSource(tableData);
    // this.vehiclesDataSource.filterPredicate = function(data: any, filter: string): boolean {
    //   return (
    //     data.vehicleName.toString().toLowerCase().includes(filter) ||
    //     data.vehicleGroupName.toString().toLowerCase().includes(filter) ||
    //     data.subcriptionStatus.toString().toLowerCase().includes(filter)
    //   );
    // };
    // setTimeout(()=>{
    //   this.vehiclesDataSource.paginator = this.paginator.toArray()[0];
    //   this.vehiclesDataSource.sort = this.sort.toArray()[0];
    // });
  }

  onChangeAlertCategory(value){
    this.alert_category_selected= value;
    this.alertForm.get('alertType').setValue('');
    this.alertTypeByCategoryList= this.alertTypeList.filter(item => item.parentEnum == value);
    //On 12-08-2021 removed the alert type "Excessive under utilization in days" by adding below line as discussed this will not need anymore.
    this.alertTypeByCategoryList = this.alertTypeByCategoryList.filter(item => item.enum != 'Y');
  }

  onChangeAlertType(value: any, flag?: boolean){
    this.vehicleGroupList = [];
    this.vehicleByVehGroupList= [];
    this.vehicleListForTable = [];
    this.unitTypes= [];
    // this.alertFilterRefs = []; need to check
    this.alert_type_selected= value;
    if(this.panelOpenState && this.notificationComponent.openAdvancedFilter){
      this.notificationComponent.setAlertType(this.alert_type_selected);
    }
    
    //Render vehicle group and vehicle dropdowns based on alert type
    let alertTypeObj = this.alertCategoryTypeMasterData.filter(item => item.enum == this.alert_type_selected && item.parentEnum == this.alert_category_selected)[0];
    this.alertTypeObject = alertTypeObj;
    if(this.actionType != 'view'){
      this.alertTypeName = this.alertTypeList.filter(item => item.enum == this.alert_type_selected)[0].value;
      this.getVehiclesForAlertType(alertTypeObj);
      this.getVehicleGroupsForAlertType(alertTypeObj);
    }

    if(flag){ // default selection after alert type change
      this.alertForm.get('vehicleGroup').setValue('ALL');
      this.onChangeVehicleGroup('ALL');
    }
    //----------------------------------------------------------------------------------------------------------

    if(this.alert_category_selected === 'L' && (this.alert_type_selected === 'N' || this.alert_type_selected === 'X' || this.alert_type_selected === 'C' ||this.alert_type_selected === 'S')){
      if(this.actionType == 'edit' || this.actionType == 'duplicate'){
        this.alertForm.get('alertLevel').setValue(this.selectedRowData.alertUrgencyLevelRefs[0].urgencyLevelType);
      }
      this.loadMap();
      if(this.alert_type_selected === 'N' || this.alert_type_selected === 'X'){ //Entering zone & Exiting Zone
        this.loadPOIData();
        this.loadGeofenceData();
        this.loadGroupData();
        this.selectedPOI.clear();
        this.selectedGeofence.clear();
        this.selectedGroup.clear();
      }
      else if(this.alert_type_selected === 'C'){ // Exiting Corridor
        if(this.actionType == 'edit' || this.actionType == 'duplicate'){
          this.alertForm.get('alertLevel').setValue(this.selectedRowData.alertUrgencyLevelRefs[0].urgencyLevelType);
        }
        this.loadCorridorData();
        this.selectedCorridor.clear();       
      }
    }
    else if(this.alert_category_selected == 'R'){ // Repair and maintenance
      if(this.alert_type_selected === 'O'){ // Status Change to Stop Now
        this.alertForm.get('alertLevel').setValue('C');
      }
      else if(this.alert_type_selected === 'E'){ // Status Change to Service Now
        this.alertForm.get('alertLevel').setValue('W');
      }
    }
    else if((this.alert_category_selected == 'L' && (this.alert_type_selected == 'Y' || this.alert_type_selected == 'H' || this.alert_type_selected == 'D' || this.alert_type_selected == 'U' || this.alert_type_selected == 'G')) ||
            (this.alert_category_selected == 'F' && (this.alert_type_selected == 'P' || this.alert_type_selected == 'L' || this.alert_type_selected == 'T' || this.alert_type_selected == 'I' || this.alert_type_selected == 'A' || this.alert_type_selected == 'F'))){

      if(this.actionType == 'edit' || this.actionType == 'duplicate' || this.actionType == 'view'){
        this.selectedRowData.alertUrgencyLevelRefs.forEach(element => {
          if(element.urgencyLevelType == 'C'){
            this.isCriticalLevelSelected= true;
            // this.alertForm.get('criticalLevelThreshold').setValue(element.thresholdValue);
          }
          else if(element.urgencyLevelType == 'W'){
            this.isWarningLevelSelected= true;
            // let threshold;
            // if(this.alert_category_selected+this.alert_type_selected == 'LU'){
            //   threshold = this.reportMapService.getConvertedTime(element.thresholdValue,this.unitTypeEnum);
            // }
            // this.alertForm.get('warningLevelThreshold').setValue(element.thresholdValue);
          }
          else if(element.urgencyLevelType == 'A'){
            this.isAdvisoryLevelSelected= true;
            // this.alertForm.get('advisoryLevelThreshold').setValue(element.thresholdValue);
          }          
        });
      }
        
      // if(this.alert_category_selected+this.alert_type_selected == 'LD' || this.alert_category_selected+this.alert_type_selected == 'LG'){        
      //   this.unitTypes= [
      //                     {
      //                       enum : 'K', 
      //                       value : this.translationData.lblKilometer ? this.translationData.lblKilometer : 'Kilometer'
      //                     },
      //                     {
      //                       enum : 'L',
      //                       value : this.translationData.lblMiles ? this.translationData.lblMiles : 'Miles'
      //                     }
      //                   ];
      // }
      if(this.alert_category_selected+this.alert_type_selected == 'LU' || this.alert_category_selected+this.alert_type_selected == 'FI'){
        this.unitTypes= [
          {
            enum : 'H', 
            value : this.translationData.lblHours ? this.translationData.lblHours : ''
          },
          {
            enum : 'T',
            value : this.translationData.lblMinutes ? this.translationData.lblMinutes : ''
          },
          {
            enum : 'S',
            value : this.translationData.lblSeconds ? this.translationData.lblSeconds : ''
          }
        ];
      }

      switch(this.alert_category_selected+this.alert_type_selected){
        //On 12-08-2021 removed the alert type "Excessive under utilization in days" as discussed this will not need anymore.
        // case "LY": { //Excessive under utilization in days
        //   this.labelForThreshold= this.translationData.lblPeriod ? this.translationData.lblPeriod : "Period";
        //   this.unitForThreshold= this.translationData.lblDays ? this.translationData.lblDays : "Days";
        //   this.unitTypeEnum= "D";
        //   break;
        // }
        case "LH": { //Excessive under utilization in hours
          this.labelForThreshold= this.translationData.lblPeriod ? this.translationData.lblPeriod : "";
          this.unitForThreshold= this.translationData.lblHours ? this.translationData.lblHours : "";
          this.unitTypeEnum= "H";
          break;
        }
        case "LD": { //Excessive distance done
          this.labelForThreshold= this.translationData.lblDistance ? this.translationData.lblDistance : "";
          this.unitForThreshold= this.prefUnitFormat == 'dunit_Metric' ? this.translationData.lblkm : this.translationData.lblmile;
         // this.unitForThreshold= this.translationData.lbl ? this.translationData.lblKilometer : "Kilometer"; //km/miles
          if(this.prefUnitFormat == 'dunit_Metric'){
            this.unitTypeEnum= "K";  }
            else{
              this.unitTypeEnum= "L";
            }
          if(this.actionType == 'edit' || this.actionType == 'duplicate' || this.actionType == 'view'){
            this.alertForm.get('unitType').setValue(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);                  
            // this.onChangeUnitType(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);      
          }
          else{                
            this.alertForm.get('unitType').setValue(this.unitTypeEnum);
          }
          break;
        }
        case "LU": { //Excessive Driving duration
          this.labelForThreshold= this.translationData.lblDuration ? this.translationData.lblDuration : "";
          this.unitForThreshold= this.translationData.lblHours ? this.translationData.lblHours : "";
          this.unitTypeEnum= "H";
          if(this.actionType == 'edit' || this.actionType == 'duplicate' || this.actionType == 'view'){
            this.alertForm.get('unitType').setValue(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);                  
            this.onChangeUnitType(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);      
          }
          else{                
            this.alertForm.get('unitType').setValue(this.unitTypeEnum);
          }
          break;
        }
        case "LG": { //Excessive Global Mileage
          this.labelForThreshold= this.translationData.lblMileage ? this.translationData.lblMileage : "";
          // this.unitForThreshold= this.translationData.lblKilometer ? this.translationData.lblKilometer : "Kilometer"; //km/miles 
          this.unitForThreshold= this.prefUnitFormat == 'dunit_Metric' ? this.translationData.lblkm : this.translationData.lblmile;
          if(this.prefUnitFormat == 'dunit_Metric'){
          this.unitTypeEnum= "K";  }
          else{
            this.unitTypeEnum= "L";
          }
          if(this.actionType == 'edit' || this.actionType == 'duplicate' || this.actionType == 'view'){
            this.alertForm.get('unitType').setValue(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);                  
            // this.onChangeUnitType(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);      
          }
          else{                
            this.alertForm.get('unitType').setValue(this.unitTypeEnum);
          }
          break;
        }
        case "FP": { //Fuel Increase During stop
          this.labelForThreshold= this.translationData.lblPercentage ? this.translationData.lblPercentage : "";
          this.unitForThreshold= "%";
          this.unitTypeEnum= "P";
          break;
        }
        case "FL": { //Fuel loss during stop
          this.labelForThreshold= this.translationData.lblPercentage ? this.translationData.lblPercentage : "";
          this.unitForThreshold= "%"
          this.unitTypeEnum= "P";
          break;
        }
        case "FT": { //Fuel loss during trip
          this.labelForThreshold= this.translationData.lblPercentage ? this.translationData.lblPercentage : "";
          this.unitForThreshold= "%"
          this.unitTypeEnum= "P";
          break;
        }
        case "FI": { //Excessive Average Idling
          this.labelForThreshold= this.translationData.lblDuration ? this.translationData.lblDuration : "";
          this.unitForThreshold= this.translationData.lblSeconds ? this.translationData.lblSeconds : "";
          this.unitTypeEnum= "S";
          if(this.actionType == 'edit' || this.actionType == 'duplicate' || this.actionType == 'view'){
            this.alertForm.get('unitType').setValue(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);                  
            this.onChangeUnitType(this.selectedRowData.alertUrgencyLevelRefs[0].unitType);      
          }
          else{                
            this.alertForm.get('unitType').setValue(this.unitTypeEnum);
          }
          break;
        }
        case "FA": { //Excessive Average speed
          this.labelForThreshold= this.translationData.lblSpeed ? this.translationData.lblSpeed : "";
          this.unitForThreshold= this.prefUnitFormat == 'dunit_Metric' ? this.translationData.lblkilometerperhour : this.translationData.lblMilesPerHour;
          // this.unitForThreshold= this.translationData.lblkilometerperhour ? this.translationData.lblkilometerperhour : "km/h";
          // this.unitTypeEnum= "E";
          if(this.prefUnitFormat == 'dunit_Metric'){
            this.unitTypeEnum= "A";  }
            else{
              this.unitTypeEnum= "B";
            }
          break;
        }
        case "FF": { //Fuel Consumed
          this.labelForThreshold= this.translationData.lblFuelConsumed ? this.translationData.lblFuelConsumed : "";
          // this.unitForThreshold= this.translationData.lblLiters ? this.translationData.lblLiters : "Liters";
          // this.unitTypeEnum= "L";
           this.unitForThreshold= "%";
           this.unitTypeEnum= "P";
          break;
        }
      }
    }
    

  }

  updateVehiclesList(alertTypeObj: any){
    this.vehicleByVehGroupList = [];
    this.vehicleListForTable = [];
    let vehicles = this.getUnique(this.alertCategoryTypeFilterData.filter(item => item.featureKey == alertTypeObj.key), "vehicleId");
    vehicles.forEach(element => {
      let veh = this.associatedVehicleData.filter(item => item.vehicleId == element.vehicleId && item.vehicleGroupId == this.selectedRowData.vehicleGroupId);
      if(veh.length > 0){
        this.vehicleByVehGroupList.push(veh[0]);
      }
    });
        //subscribed vehicles
        this.vehicleByVehGroupList.forEach(element => {
          element["subcriptionStatus"] = true;
          this.vehicleListForTable.push(element);
        });
    
        //non-subscribed vehicles
        this.getUnique(this.associatedVehicleData, "vehicleId").forEach(element => {
          let isDuplicateVehicle= false;
          for(let i = 0; i< this.vehicleByVehGroupList.length; i++){
            if(element.vehicleId == this.vehicleByVehGroupList[i].vehicleId){
                isDuplicateVehicle= true;
                break;
            }
          }
          if(!isDuplicateVehicle){
            element["subcriptionStatus"] = false;
            this.vehicleListForTable.push(element);
          }
        });
        
        this.updateVehiclesDataSource(this.vehicleListForTable);
  }

  getVehicleGroupsForAlertType(alertTypeObj: any){
     this.vehicleByVehGroupList.forEach(element => {
       let vehicleGroupDetails = element.vehicleGroupDetails.split(",");
       vehicleGroupDetails.forEach(item => {
         let itemSplit = item.split("~");
         if(itemSplit[2] != 'S') {
          let vehicleGroupObj = {
            "vehicleGroupId": parseInt(itemSplit[0]),
            "vehicleGroupName": itemSplit[1],
            "vehicleId": parseInt(element.vehicleId)
          }
          this.vehicleGroupList.push(vehicleGroupObj);
          //console.log("vehicleGroupList 1", this.vehicleGroupList);
        } else {
          this.singleVehicle.push(element);
        }
       });
     });
     this.vehicleGroupList = this.getUnique(this.vehicleGroupList, "vehicleGroupId");
     this.vehicleGroupList = this.removeNaNrecords(this.vehicleGroupList); 
     //console.log("vehicleGroupList 2", this.vehicleGroupList); 
     this.vehicleGroupList.sort(this.compareHere);
     this.resetVehicleGroupFilter();

    //  this.vehicleGroupList.forEach(element => {
    //    element.vehicleGroupId = parseInt(element.vehicleGroupId);
    //  });
  }

  removeNaNrecords(data: any){
    let _data: any = data.filter(i => i.vehicleGroupId >= 0); // remove NaN records
    return _data.slice();
  }
 
  resetVehicleGroupFilter(){
    this.filteredVehicleGroups.next(this.vehicleGroupList.slice());
	}

  resetVehiclesFilter(){
    this.filteredVehicles.next(this.vehicleByVehGroupList.slice());
  }

  getVehiclesForAlertType(alertTypeObj: any){
    this.vehicleByVehGroupList = [];
    this.vehicleListForTable = [];
    let vehicleConcernList: any  =  [];
    let featuresData = this.alertCategoryTypeFilterData.filter(item => item.featureKey == alertTypeObj.key);
    if(featuresData.length == 1 && featuresData[0].subscriptionType == 'O'){ // org based - all subscribe
      this.associatedVehicleData.forEach(element => {    
        element.subscriptionType = "Subscribed";    
        this.vehicleByVehGroupList.push(element);
      });
      vehicleConcernList = this.filterVehiclesFromResp(); // #20710
      vehicleConcernList.forEach(element => {
        element.subscriptionType = "Subscribed";    
      });
    }
    else{ //if subscriptionType == 'v'
      //if(featuresData.length > 0){
        // featuresData.forEach(element => {
        //   let vehicle = this.associatedVehicleData.filter(item => item.vehicleId == element.vehicleId);
        //   if(vehicle.length > 0){
        //     this.vehicleByVehGroupList.push(vehicle[0]);
        //     ////console.log("vehicleByVehGroupList 5", this.vehicleByVehGroupList);
        //     this.vehicleByVehGroupList.sort(this.compareVehicleList);
        //     this.resetVehiclesFilter();
        //   }
        // });

        if(featuresData.length > 0){
          this.vehicleByVehGroupList = this.associatedVehicleData.slice();
          let _orgSub: any = featuresData.filter(i => i.subscriptionType == 'O'); // find org based subscription
          if(_orgSub && _orgSub.length > 0){ // all vehicle subscribe
            this.vehicleByVehGroupList.forEach(elem => {
              elem.subscriptionType = "Subscribed";
            });
            vehicleConcernList = this.filterVehiclesFromResp(); // #20710
            vehicleConcernList.forEach(element => {
              element.subscriptionType = "Subscribed";    
            });
          }else{ // all vehicle subscribe
            this.vehicleByVehGroupList.forEach(_el => {
              let _find: any = featuresData.filter(j => j.vehicleId == _el.vehicleId && j.subscriptionType == 'V');
              if(_find && _find.length > 0){ // find 'V'
                _el.subscriptionType = "Subscribed";
              }else{ // non-subscribe
                _el.subscriptionType = "Non-Subscribed";
              }
            });

            vehicleConcernList = this.filterVehiclesFromResp(); // #20710
            vehicleConcernList.forEach(_el => {
              let _find: any = featuresData.filter(j => j.vehicleId == _el.vehicleId && j.subscriptionType == 'V');
              if(_find && _find.length > 0){ // find 'V'
                _el.subscriptionType = "Subscribed";
              }else{ // non-subscribe
                _el.subscriptionType = "Non-Subscribed";
              }
            });
          }
          this.vehicleByVehGroupList.sort(this.compareVehicleList);
          this.resetVehiclesFilter();
        }
      //}
    }

    //this.vehicleListForTable = this.vehicleByVehGroupList.slice();
    this.vehicleListForTable = vehicleConcernList.slice();
 
    //subscribed vehicles
    // this.vehicleByVehGroupList.forEach(element => {
    //   ////console.log("vehicleByVehGroupList 6", this.vehicleByVehGroupList);
    //   element["subcriptionStatus"] = true;
    //   this.vehicleListForTable.push(element);
    // });

    // //non-subscribed vehicles
    // if(featuresData[0].subscriptionType != 'O'){
    //   this.associatedVehicleData.forEach(element => {
    //     let isDuplicateVehicle = false;
    //     for(let i = 0; i< this.vehicleByVehGroupList.length; i++){
    //       if(element.vehicleId == this.vehicleByVehGroupList[i].vehicleId){
    //           isDuplicateVehicle= true;
    //           break;
    //       }
    //     }
    //     if(!isDuplicateVehicle){
    //       element["subcriptionStatus"] = false;
    //       this.vehicleListForTable.push(element);
    //     }
    //   });
    // }
    this.updateVehiclesDataSource(this.vehicleListForTable);
  }

  filterVehiclesFromResp(grpId?: any){
    let _vehicleConcernList: any = [];
    let grpData: any = [];
    if(grpId){ // search by veh-grp-id
      grpData = this.vehicleGrpByVehicleList.filter(i => parseInt(i.vehicleGroupId) == parseInt(grpId));
    }else{ // all veh-grp
      grpData = this.vehicleGrpByVehicleList.slice();
    }

    grpData.forEach(_elem => {
      if(_elem.vehicleDetails && _elem.vehicleDetails != ''){
        let _vehDetails = _elem.vehicleDetails.split(",");
        _vehDetails.forEach(item => {
          let itemSplit = item.split("~");
          if(itemSplit.length > 1) {
            let vehObj: any;
            vehObj = {
              "vehicleId": parseInt(itemSplit[0]),
              "vehicleName": itemSplit[1],
              "vin": itemSplit[2]
            }
            _vehicleConcernList.push(vehObj);
          }
        });
      }
    });
    _vehicleConcernList = this.removeDuplicates(_vehicleConcernList, 'vehicleId'); // remove deplicate vehicle
    return _vehicleConcernList.slice();
  }

  removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject  = {};
    for(var i in originalArray) {
       lookupObject[originalArray[i][prop]] = originalArray[i];
    }
    for(i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
     return newArray;
  }

  getUniqueVINs(vinList: any){
    let uniqueVINList = [];
    for(let vin of vinList){
      let vinPresent = uniqueVINList.map(element => element.vin).indexOf(vin.vin);
      if(vinPresent == -1) {
        uniqueVINList.push(vin);
      }
    }
    return uniqueVINList;
  }


  onChangeVehicleGroup(value: any){
    this.vehicleListForTable = [];
    this.vehicleByVehGroupList = [];
    let vehicleConcernList: any = [];
    if(this.actionType == 'edit' || this.actionType == 'duplicate'){
      // this.onChangeAlertType(this.selectedRowData.type); //commenting this line as map was overriding on table for edit
      this.vehicleByVehGroupList = [];
      this.vehicleListForTable = [];
    }
    this.alertForm.get('vehicle').setValue('');    
    let alertTypeObj = this.alertCategoryTypeMasterData.filter(item => item.enum == this.alert_type_selected && item.parentEnum == this.alert_category_selected)[0];
    if(value == 'ALL'){
      this.getVehiclesForAlertType(alertTypeObj);
      this.vehicleByVehGroupList = this.getUniqueVINs([...this.vehicleByVehGroupList, ...this.singleVehicle]);
    }
    else{
      //converted vehicle group selection into int val.
      this.vehicle_group_selected = parseInt(value);
      let featuresData = this.alertCategoryTypeFilterData.filter(item => item.featureKey == alertTypeObj.key);
      if(featuresData.length == 1 && featuresData[0].subscriptionType == 'O'){ // org based -> all veh subscribe
        this.vehicleByVehGroupList = this.associatedVehicleData.filter(item => item.vehicleGroupDetails.includes(this.vehicle_group_selected+"~"));
        this.vehicleByVehGroupList.forEach(elem => {
          elem.subscriptionType = "Subscribed";
        });

        vehicleConcernList = this.filterVehiclesFromResp(this.vehicle_group_selected); // #20710
        vehicleConcernList.forEach(element => {
          element.subscriptionType = "Subscribed";    
        });
      }
      else{ //if subscriptionType == 'v'
        // featuresData.forEach(element => {
        //   let vehicle= this.associatedVehicleData.filter(item => item.vehicleId == element.vehicleId && item.vehicleGroupDetails.includes(this.vehicle_group_selected+"~"));
        //   if(vehicle.length > 0){
        //     this.vehicleByVehGroupList.push(vehicle[0]);
        //   }
        //});
        if(featuresData.length > 0){
          let vehicle = this.associatedVehicleData.filter(item => item.vehicleGroupDetails.includes(this.vehicle_group_selected+"~"));
          if(vehicle.length > 0){
            this.vehicleByVehGroupList = vehicle.slice();
          }

          let _orgSub: any = featuresData.filter(i => i.subscriptionType == 'O'); // find org based subscription
          if(_orgSub && _orgSub.length > 0){ // all vehicle subscribe
            this.vehicleByVehGroupList.forEach(elem => {
              elem.subscriptionType = "Subscribed";
            });
            
            vehicleConcernList = this.filterVehiclesFromResp(this.vehicle_group_selected); // #20710
            vehicleConcernList.forEach(element => {
              element.subscriptionType = "Subscribed";    
            });

          }else{ // all vehicle subscribe
            this.vehicleByVehGroupList.forEach(_el => {
              let _find: any = featuresData.filter(j => j.vehicleId == _el.vehicleId && j.subscriptionType == 'V');
              if(_find && _find.length > 0){ // find 'V'
                _el.subscriptionType = "Subscribed";
              }else{ // non-subscribe
                _el.subscriptionType = "Non-Subscribed";
              }
            });

            vehicleConcernList = this.filterVehiclesFromResp(this.vehicle_group_selected); // #20710
            vehicleConcernList.forEach(_el => {
              let _find: any = featuresData.filter(j => j.vehicleId == _el.vehicleId && j.subscriptionType == 'V');
              if(_find && _find.length > 0){ // find 'V'
                _el.subscriptionType = "Subscribed";
              }else{ // non-subscribe
                _el.subscriptionType = "Non-Subscribed";
              }
            });

            // featuresData.forEach(element => {
            //   let vehicle: any = this.associatedVehicleData.filter(item => item.vehicleId == element.vehicleId && item.vehicleGroupDetails.includes(this.vehicle_group_selected+"~"));
            //   if(vehicle.length > 0){
            //     this.vehicleByVehGroupList.push(vehicle[0]);
            //   }
            // });
          }
        }
      }

      //subscribed vehicles
      // this.vehicleByVehGroupList.forEach(element => {
      //   element["subcriptionStatus"] = true;
      //   this.vehicleListForTable.push(element);
      // });
      
      //this.vehicleListForTable = this.vehicleByVehGroupList;
      this.vehicleListForTable = vehicleConcernList.slice()

      //Commented because only vehicles from that group should be displayed.
      //non-subscribed vehicles
      // if(featuresData[0].subscriptionType != 'O'){  
      //   this.getUnique(this.associatedVehicleData, "vehicleId").forEach(element => {
      //     let isDuplicateVehicle= false;
      //     for(let i = 0; i< this.vehicleByVehGroupList.length; i++){
      //       if(element.vehicleId == this.vehicleByVehGroupList[i].vehicleId){
      //           isDuplicateVehicle= true;
      //           break;
      //       }
      //     }
      //     if(!isDuplicateVehicle){
      //       element["subcriptionStatus"] = false;
      //       this.vehicleListForTable.push(element);
      //     }
      //   });
      // }
      this.updateVehiclesDataSource(this.vehicleListForTable);
    }
  }

  onChangeVehicle(value){
    this.vehicle_group_selected= value;
    let vehicleSelected= this.vehicleByVehGroupList.filter(item => item.vehicleId == value);
    this.updateVehiclesDataSource(vehicleSelected);
  }

  onChangeUnitType(value: any){
    let _s = this.unitTypes.filter(item => item.enum == value);
    if(_s && _s.length > 0){
      this.unitForThreshold = _s[0].value;
    }else{
      this.unitForThreshold = '';
    }
    this.unitTypeEnum = value;
  }

  loadMap() {
    if(this.alert_type_selected == 'C'){
      setTimeout(() => {
        this.mapFunctions.initMap(this.mapElement, this.translationData);
        }, 0);
    }
    else{
      let defaultLayers = this.platform.createDefaultLayers();
      setTimeout(() => {
        this.map = new H.Map(this.mapElement.nativeElement,
          defaultLayers.raster.normal.map, {
          center: { lat: 51.43175839453286, lng: 5.519981221425336 },
          zoom: 4,
          pixelRatio: window.devicePixelRatio || 1
        });
        window.addEventListener('resize', () => this.map.getViewPort().resize());
        var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
        this.ui = H.ui.UI.createDefault(this.map, defaultLayers);
        
        this.ui.removeControl("mapsettings");
        // create custom one
        var ms = new H.ui.MapSettingsControl({
            baseLayers : [ { 
              label: this.translationData.lblNormal , layer: defaultLayers.raster.normal.map
            },{
              label: this.translationData.lblSatellite , layer: defaultLayers.raster.satellite.map
            }, {
              label: this.translationData.lblTerrain , layer: defaultLayers.raster.terrain.map
            }
            ],
          layers : [{
                label: this.translationData.lblLayerTraffic , layer: defaultLayers.vector.normal.traffic
            },
            {
                label: this.translationData.lblLayerIncidents , layer: defaultLayers.vector.normal.trafficincidents
            }
          ]
        });
        this.ui.addControl("customized", ms);
      }, 1000);
    }
}

PoiCheckboxClicked(event: any, row: any) {
  if(event.checked){ //-- add new marker
    this.markerArray.push(row);
    this.moveMapToSelectedPOI(this.map, row.latitude, row.longitude);
  }else{ //-- remove existing marker
    //It will filter out checked points only
    let arr = this.markerArray.filter(item => item.id != row.id);
    this.markerArray = arr;
  }
  this.addMarkerOnMap(this.ui);
    
  }
  
  addMarkerOnMap(ui){
    this.map.removeObjects(this.map.getObjects());
    this.markerArray.forEach(element => {
      let marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
      this.map.addObject(marker);
      // this.createResizableCircle(this.circularGeofenceFormGroup.controls.radius.value ? parseInt(this.circularGeofenceFormGroup.controls.radius.value) : 0, element);
      this.createResizableCircle(this.alertForm.controls.widthInput.value * 1000,this.ui,element);

      //For tooltip on info bubble

      var bubble;
      var transPOIName = this.translationData.lblPOIName;
      var transCategory = this.translationData.lblCategory;
      var SubCategory = this.translationData.lblSubCategory;
      var Address = this.translationData.lblAddress;
      
      marker.addEventListener('pointerenter', function (evt) {
        // event target is the marker itself, group is a parent event target
        // for all objects that it contains
        bubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
          // read custom data
          content:`<div style='width:310px;' class='font-14-px line-height-21px'>
          <span class='font-helvetica-md'>${transPOIName}:</span> ${element.name}<br>
          <span class='font-helvetica-md'>${transCategory}:</span> ${element.categoryName}<br>
          <span class='font-helvetica-md'>${SubCategory}:</span> ${element.subCategoryName}<br>
          <span class='font-helvetica-md'>${Address}:</span> ${element.address}
          </div>`
        });
        // show info bubble
        ui.addBubble(bubble);
      }, false);
      marker.addEventListener('pointerleave', function(evt) {
        bubble.close();
      }, false);



    });
    this.geoMarkerArray.forEach(element => {
      if(element.type == "C"){
      this.marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
      this.map.addObject(this.marker);
      this.createResizableCircle(element.distance, this.ui,element);
      }
      else if(element.type == "O"){
        this.polyPoints = [];
        element.nodes.forEach(item => {
        this.polyPoints.push(item.latitude.toFixed(4));
        this.polyPoints.push(item.longitude.toFixed(4));
        this.polyPoints.push(0);
        });
        this.createResizablePolygon(this.map,this.polyPoints,this,this.ui, element);
      }

  });
  }

  geofenceCheckboxClicked(event: any, row: any) {

    if(event.checked){ 
      this.geoMarkerArray.push(row);
      this.addMarkersAndSetViewBoundsGeofence(this.map, row);
    }else{ 
      let arr = this.geoMarkerArray.filter(item => item.id != row.id);
      this.geoMarkerArray = arr;
    }
    this.addCircleOnMap(event);
  // });
    }

    addCircleOnMap(event: any){
      if(event.checked == false){
    this.map.removeObjects(this.map.getObjects());
  }
//adding circular geofence points on map
    this.geoMarkerArray.forEach(element => {
      if(element.type == "C"){
      this.marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
      this.map.addObject(this.marker);
      
      this.createResizableCircle(element.distance, this.ui, element);
      }
      // "PolygonGeofence"
      else{
        this.polyPoints = [];
        element.nodes.forEach(item => {
        this.polyPoints.push(item.latitude);
        this.polyPoints.push(item.longitude);
        this.polyPoints.push(0);
        });
        this.createResizablePolygon(this.map,this.polyPoints,this,this.ui, element);
      }

  });
  //adding poi geofence points on map
  this.markerArray.forEach(element => {
    let marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
    this.map.addObject(marker);
  });

    }

  createResizableCircle(_radius: any,ui:any, rowData: any) {
    var circle = new H.map.Circle(
      { lat: rowData.latitude, lng: rowData.longitude },

      _radius,//85000,
      {
        style: { fillColor: 'rgba(138, 176, 246, 0.7)', lineWidth: 0 }
      }
    ),
      circleOutline = new H.map.Polyline(
        circle.getGeometry().getExterior(),
        {
          style: { lineWidth: 8, strokeColor: 'rgba(255, 0, 0, 0)' }
        }
      ),
      circleGroup = new H.map.Group({
        volatility: true, // mark the group as volatile for smooth dragging of all it's objects
        objects: [circle, circleOutline]
      }),
      circleTimeout;

    circle.draggable = true;
    circleOutline.draggable = true;
    circleOutline.getGeometry().pushPoint(circleOutline.getGeometry().extractPoint(0));
    this.map.addObject(circleGroup);

    var bubble;
    var transCategory = this.translationData.lblCategory;
    var SubCategory = this.translationData.lblSubCategory;
    var GeofenceName = this.translationData.lblGeofence;
    circle.addEventListener('pointerenter', function (evt) {
      // event target is the marker itself, group is a parent event target
      // for all objects that it contains
      bubble =  new H.ui.InfoBubble({lat:rowData.latitude,lng:rowData.longitude}, {
        // read custom data
        content:`<div style='width:310px;' class='font-14-px line-height-21px'>
        <span class='font-helvetica-md'>${GeofenceName}:</span> ${rowData.name}<br>
        <span class='font-helvetica-md'>${transCategory}:</span> ${rowData.categoryName}<br>
        <span class='font-helvetica-md'>${SubCategory}:</span> ${rowData.subCategoryName}<br>
        </div>`
      });
      // show info bubble
      ui.addBubble(bubble);
    }, false);
    circle.addEventListener('pointerleave', function(evt) {
      bubble.close();
    }, false);
    }
  
    createResizablePolygon(map: any, points: any, thisRef: any, ui: any, rowData: any){
          var svgCircle = '<svg width="50" height="20" version="1.1" xmlns="http://www.w3.org/2000/svg">' +
          '<circle cx="10" cy="10" r="7" fill="transparent" stroke="red" stroke-width="4"/>' +
          '</svg>',
            polygon = new H.map.Polygon(
              new H.geo.Polygon(new H.geo.LineString(points)),
              {
                style: {fillColor: 'rgba(150, 100, 0, .8)', lineWidth: 0}
              }
            ),
            verticeGroup = new H.map.Group({
              visibility: false
            }),
            mainGroup = new H.map.Group({
              volatility: true, // mark the group as volatile for smooth dragging of all it's objects
              objects: [polygon, verticeGroup]
            }),
            polygonTimeout;
      
        // ensure that the polygon can receive drag events
        polygon.draggable = true;
      
        // create markers for each polygon's vertice which will be used for dragging
        polygon.getGeometry().getExterior().eachLatLngAlt(function(lat, lng, alt, index) {
          var vertice = new H.map.Marker(
            {lat, lng},
            {
              icon: new H.map.Icon(svgCircle, {anchor: {x: 10, y: 10}})
            }
          );
          vertice.draggable = true;
          vertice.setData({'verticeIndex': index})
          verticeGroup.addObject(vertice);
        });
      
        // add group with polygon and it's vertices (markers) on the map
        map.addObject(mainGroup);

        var bubble;
        var transCategory = this.translationData.lblCategory;
        var SubCategory = this.translationData.lblSubCategory;
        var GeofenceName = this.translationData.lblGeofence;
        // event listener for main group to show markers if moved in with mouse (or touched on touch devices)
        mainGroup.addEventListener('pointerenter', function(evt) {
          if (polygonTimeout) {
            clearTimeout(polygonTimeout);
            polygonTimeout = null;
          }
          // show vertice markers
          verticeGroup.setVisibility(true);
          
          bubble =  new H.ui.InfoBubble({ lat: rowData.latitude, lng: rowData.longitude } , {
            // read custom data
            content:`<div style='width:310px;' class='font-14-px line-height-21px'>
              <span class='font-helvetica-md'>${GeofenceName}:</span> ${rowData.name}<br>
              <span class='font-helvetica-md'>${transCategory}:</span> ${rowData.categoryName}<br>
              <span class='font-helvetica-md'>${SubCategory}:</span> ${rowData.subCategoryName}<br>
            </div>`
          });
          // show info bubble
          ui.addBubble(bubble);
        }, true);
        

        // event listener for main group to show markers if moved in with mouse (or touched on touch devices)
        mainGroup.addEventListener('pointerenter', function(evt) {
          if (polygonTimeout) {
            clearTimeout(polygonTimeout);
            polygonTimeout = null;
          }
      
          // show vertice markers
          verticeGroup.setVisibility(true);
        }, true);
      
        // event listener for main group to hide vertice markers if moved out with mouse (or released finger on touch devices)
        // the vertice markers are hidden on touch devices after specific timeout
        mainGroup.addEventListener('pointerleave', function(evt) {
          var timeout = (evt.currentPointer.type == 'touch') ? 1000 : 0;
      
          // hide vertice markers
          polygonTimeout = setTimeout(function() {
            verticeGroup.setVisibility(false);
          }, timeout);
        }, true);
      
        if(thisRef.actionType == 'create'){ //-- only for create polygon geofence
          // event listener for vertice markers group to change the cursor to pointer
          verticeGroup.addEventListener('pointerenter', function(evt) {
            document.body.style.cursor = 'pointer';
          }, true);
        
          // event listener for vertice markers group to change the cursor to default
          verticeGroup.addEventListener('pointerleave', function(evt) {
            document.body.style.cursor = 'default';
          }, true);
        
          // event listener for vertice markers group to resize the geo polygon object if dragging over markers
          verticeGroup.addEventListener('drag', function(evt) {
            var pointer = evt.currentPointer,
                geoLineString = polygon.getGeometry().getExterior(),
                geoPoint = map.screenToGeo(pointer.viewportX, pointer.viewportY);
           // set new position for vertice marker
            evt.target.setGeometry(geoPoint);
        
            // set new position for polygon's vertice
            geoLineString.removePoint(evt.target.getData()['verticeIndex']);
            geoLineString.insertPoint(evt.target.getData()['verticeIndex'], geoPoint);
            polygon.setGeometry(new H.geo.Polygon(geoLineString));
        
            // stop propagating the drag event, so the map doesn't move
            evt.stopPropagation();
          }, true);
  
          verticeGroup.addEventListener('dragend', function (ev) {
            var coordinate = map.screenToGeo(ev.currentPointer.viewportX,
              ev.currentPointer.viewportY);
              let nodeIndex = ev.target.getData()['verticeIndex'];
            let _position = coordinate.lat.toFixed(4) + "," + coordinate.lng.toFixed(4);
              if(_position){
                thisRef.hereService.getAddressFromLatLng(_position).then(result => {
                  let locations = <Array<any>>result;
                  let data = locations[0].Location.Address;
                  let pos = locations[0].Location.DisplayPosition;
                  thisRef.setAddressValues('updatePoint', data, pos, nodeIndex);
                }, error => {
                  // console.error(error);
                });
              }
  
          }, false);
        }
    }

    moveMapToSelectedPOI(map, lat, lon){
      map.setCenter({lat:lat, lng:lon});
      map.setZoom(16);
    }
  
    addMarkersAndSetViewBoundsGeofence(map, row) {
      let group = new H.map.Group();
      let locationObjArray= [];
      row.nodes.forEach(element => {
        locationObjArray.push(new H.map.Marker({lat:element.latitude, lng:element.longitude}))
      });    
    
      // add markers to the group
      group.addObjects(locationObjArray);
      map.addObject(group);
    
      // get geo bounding box for the group and set it to the map
      map.getViewModel().setLookAtData({
        bounds: group.getBoundingBox()
      });
    }  

    corridorCheckboxClicked(event, row){
      this.mapFunctions.clearRoutesFromMap();
      if(event.checked){ //-- add new marker
        this.markerArray.push(row);
      }else{ //-- remove existing marker
        //It will filter out checked points only
        let arr = this.markerArray.filter(item => item.id != row.id);
        this.markerArray = arr;
        }
        // this.addPolylineToMap();
        this.mapFunctions.viewSelectedRoutes(this.markerArray,this.accountOrganizationId);
    }
  
    addPolylineToMap(){
      var lineString = new H.geo.LineString();
      this.markerArray.forEach(element => {
      lineString.pushPoint({lat : element.startLat, lng: element.startLong});
      lineString.pushPoint({lat : element.endLat, lng: element.endLong});
      // lineString.pushPoint({lat:48.8567, lng:2.3508});
      // lineString.pushPoint({lat:52.5166, lng:13.3833});
      });
      
      let group= new H.map.Group();
      group.addObjects([new H.map.Polyline(
        lineString, { style: { lineWidth: 4 }}
      )]);
      this.map.addObject(group);

      this.map.getViewModel().setLookAtData({
        bounds: group.getBoundingBox()
      });
    }
  


  getSVGIcon(){
    let markup = '<svg xmlns="http://www.w3.org/2000/svg" width="28px" height="36px" >' +
    '<path d="M 19 31 C 19 32.7 16.3 34 13 34 C 9.7 34 7 32.7 7 31 C 7 29.3 9.7 ' +
    '28 13 28 C 16.3 28 19 29.3 19 31 Z" fill="#000" fill-opacity=".2"></path>' +
    '<path d="M 13 0 C 9.5 0 6.3 1.3 3.8 3.8 C 1.4 7.8 0 9.4 0 12.8 C 0 16.3 1.4 ' +
    '19.5 3.8 21.9 L 13 31 L 22.2 21.9 C 24.6 19.5 25.9 16.3 25.9 12.8 C 25.9 9.4 24.6 ' +
    '6.1 22.1 3.8 C 19.7 1.3 16.5 0 13 0 Z" fill="#fff"></path>' +
    '<path d="M 13 2.2 C 6 2.2 2.3 7.2 2.1 12.8 C 2.1 16.1 3.1 18.4 5.2 20.5 L ' +
    '13 28.2 L 20.8 20.5 C 22.9 18.4 23.8 16.2 23.8 12.8 C 23.6 7.07 20 2.2 ' +
    '13 2.2 Z" fill="${COLOR}"></path><text transform="matrix( 1 0 0 1 13 18 )" x="0" y="0" fill-opacity="1" ' +
    'fill="#fff" text-anchor="middle" font-weight="bold" font-size="13px" font-family="arial" style="fill:black"></text></svg>';
    
    let locMarkup = '<svg height="24" version="1.1" width="24" xmlns="http://www.w3.org/2000/svg" xmlns:cc="http://creativecommons.org/ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"><g transform="translate(0 -1028.4)"><path d="m12 0c-4.4183 2.3685e-15 -8 3.5817-8 8 0 1.421 0.3816 2.75 1.0312 3.906 0.1079 0.192 0.221 0.381 0.3438 0.563l6.625 11.531 6.625-11.531c0.102-0.151 0.19-0.311 0.281-0.469l0.063-0.094c0.649-1.156 1.031-2.485 1.031-3.906 0-4.4183-3.582-8-8-8zm0 4c2.209 0 4 1.7909 4 4 0 2.209-1.791 4-4 4-2.2091 0-4-1.791-4-4 0-2.2091 1.7909-4 4-4z" fill="#55b242" transform="translate(0 1028.4)"/><path d="m12 3c-2.7614 0-5 2.2386-5 5 0 2.761 2.2386 5 5 5 2.761 0 5-2.239 5-5 0-2.7614-2.239-5-5-5zm0 2c1.657 0 3 1.3431 3 3s-1.343 3-3 3-3-1.3431-3-3 1.343-3 3-3z" fill="#ffffff" transform="translate(0 1028.4)"/></g></svg>';
    
    //let icon = new H.map.Icon(markup.replace('${COLOR}', '#55b242'));
    let icon = new H.map.Icon(locMarkup);
    return icon;
  }
  
  setDefaultValue(){
     //console.log(this.selectedRowData);
    this.alertForm.get('alertName').setValue(this.selectedRowData.name);
    this.alertForm.get('alertCategory').setValue(this.selectedRowData.category);

    this.onChangeAlertCategory(this.selectedRowData.category);
    
    this.alertForm.get('alertType').setValue(this.selectedRowData.type);
    this.onChangeAlertType(this.selectedRowData.type);
    this.alert_type_selected = this.selectedRowData.type;
    this.alertForm.get('applyOn').setValue(this.selectedRowData.applyOn);
    this.selectedApplyOn = this.selectedRowData.applyOn;
    if(this.selectedRowData.alertLandmarkRefs.length > 0){
    this.poiWidth =this.selectedRowData.alertLandmarkRefs[0].distance;
    this.sliderChanged();
    }
    if(this.selectedRowData.applyOn == 'G'){
      this.alertForm.get('vehicleGroup').setValue(this.selectedRowData.vehicleGroupId);
      this.onChangeVehicleGroup(this.selectedRowData.vehicleGroupId);
    }
    else{
      this.alertForm.get('vehicle').setValue(this.selectedRowData.vehicleGroupId);
      this.onChangeVehicle(this.selectedRowData.vehicleGroupId);

    }
    
    this.alertForm.get('statusMode').setValue(this.selectedRowData.state);
    // this.onChangeAlertType(this.selectedRowData.type);
    this.convertValuesToPrefUnit();
    this.selectedApplyOnPeriod = this.selectedRowData.alertUrgencyLevelRefs[0].periodType;
    if(this.selectedApplyOnPeriod == 'C'){
      this.from =this.convertToFromTime(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelStartDate);
      this.to = this.convertToFromTime(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelEndDate);
      
      // this.from = Util.convertUtcToDateFormat(new Date(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelStartDate), this.prefTimeZone);
      // this.to = Util.convertUtcToDateFormat(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelEndDate,'DD/MM/YYYY HH:MM').split(" ");
      this.alertForm.get('fromDate').setValue(new Date(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelStartDate));
      this.alertForm.get('toDate').setValue(new Date(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelEndDate));
      

      this.alertForm.get('fromTimeRange').setValue(this.from);
      this.alertForm.get('toTimeRange').setValue(this.to);
    }
    this.alertForm.get('fullorCustom').setValue(this.selectedApplyOnPeriod);
    let editVal:any = {value: this.selectedApplyOnPeriod}
    this.onApplyPeriodOnChange(editVal);
  }
 
convertToFromTime(milliseconds: any){ 
  let hours = `0${new Date(milliseconds).getHours()}`.slice(-2);
  let minutes = `0${new Date(milliseconds).getMinutes()}`.slice(-2);
  let seconds = `0${new Date(milliseconds).getSeconds()}`.slice(-2);
  let time:any = `${hours}:${minutes}`
  return time;
}

  convertValuesToPrefUnit(){
    let threshold;
    this.selectedRowData.alertUrgencyLevelRefs.forEach(element => {
            if(this.alert_category_selected+this.alert_type_selected == 'LU' || this.alert_category_selected+this.alert_type_selected == 'LH' ||
            this.alert_category_selected+this.alert_type_selected == 'FI' ){
              threshold = this.reportMapService.getConvertedTime(element.thresholdValue,this.unitTypeEnum);
              if(element.urgencyLevelType == 'C'){
                this.alertForm.get('criticalLevelThreshold').setValue(threshold);
              }
              else if(element.urgencyLevelType == 'W'){
                this.alertForm.get('warningLevelThreshold').setValue(threshold);
              }
              else{
                this.alertForm.get('advisoryLevelThreshold').setValue(threshold);
              }
            }

            if(this.alert_category_selected+this.alert_type_selected == 'LD' || this.alert_category_selected+this.alert_type_selected == 'LG'){
              if(this.prefUnitFormat == 'dunit_Metric'){
                this.unitTypeEnum= "K";  }
                else{
                  this.unitTypeEnum= "L";
                }
              threshold = this.reportMapService.getDistance(element.thresholdValue,this.prefUnitFormat);
              if(element.urgencyLevelType == 'C'){
                this.alertForm.get('criticalLevelThreshold').setValue(threshold);
              }
              else if(element.urgencyLevelType == 'W'){
                this.alertForm.get('warningLevelThreshold').setValue(threshold);
              }
              else{
                this.alertForm.get('advisoryLevelThreshold').setValue(threshold);
              }
            }

            if(this.alert_category_selected+this.alert_type_selected == 'FA'){
              threshold = this.reportMapService.getConvertedSpeed(element.thresholdValue,this.unitTypeEnum);
              if(element.urgencyLevelType == 'C'){
                this.alertForm.get('criticalLevelThreshold').setValue(threshold);
              }
              else if(element.urgencyLevelType == 'W'){
                this.alertForm.get('warningLevelThreshold').setValue(threshold);
              }
              else{
                this.alertForm.get('advisoryLevelThreshold').setValue(threshold);
              }
            }

            if(this.alert_category_selected+this.alert_type_selected == 'FL' || this.alert_category_selected+this.alert_type_selected == 'FT' ||
            this.alert_category_selected+this.alert_type_selected == 'FP' || this.alert_category_selected+this.alert_type_selected == 'FF'){
              if(element.urgencyLevelType == 'C'){
                this.alertForm.get('criticalLevelThreshold').setValue(element.thresholdValue);
              }
              else if(element.urgencyLevelType == 'W'){
                this.alertForm.get('warningLevelThreshold').setValue(element.thresholdValue);
              }
              else{
                this.alertForm.get('advisoryLevelThreshold').setValue(element.thresholdValue);
              }
            }
        
    });
  }

  getBreadcum() {
    let page = '';
    if(this.actionType == 'edit')
      page = (this.translationData.lblEditAlertDetails ? this.translationData.lblEditAlertDetails : 'Edit Alert Details') ;
    else if(this.actionType === 'view')
      page = (this.translationData.lblViewAlertDetails ? this.translationData.lblViewAlertDetails : 'View Alert Details');
    else if(this.actionType === 'create' || this.actionType === 'duplicate')
      page = (this.translationData.lblCreateNewAlert ? this.translationData.lblCreateNewAlert : 'Create New Alert');
    
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblAlerts ? this.translationData.lblAlerts : "Alerts"} / 
    ${page}`;
  }

  loadPOIData() {
    this.showLoadingIndicator = true;
    this.poiService.getPois(this.accountOrganizationId).subscribe((poilist: any) => {
      if(poilist.length > 0){
        poilist.forEach(element => {
          if(element.icon && element.icon != '' && element.icon.length > 0){
            element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + element.icon);
          }else{
            element.icon = '';
          }
        });
        this.poiGridData = poilist;
        this.updatePOIDataSource(this.poiGridData);
        if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'duplicate')
        this.loadPOISelectedData(this.poiGridData);
      }
      
    });
    this.showLoadingIndicator = false;
  }

  loadPOISelectedData(tableData: any){
    this.selectedPOIList = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && item.landmarkType == "P");
        if (search.length > 0) {
          this.selectedPOIList.push(row);
          setTimeout(() => {
            this.PoiCheckboxClicked({checked : true}, row);  
          }, 1000);
        }
      });
      tableData = this.selectedPOIList;
      this.displayedColumnsPOI= ['icon', 'name', 'categoryName', 'subCategoryName', 'address'];
      this.updatePOIDataSource(tableData);
    }
    else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
      this.selectPOITableRows(this.selectedRowData);
    }
  }

  selectPOITableRows(rowData: any, event?:any){
    if(event){
      this.poiDataSource.data.forEach((row: any) => {
        let search = rowData.landmarks.filter(item => item.landmarkid == row.id && item.type == "P");
        if(search.length > 0) {
          if(event.checked)
            this.selectedPOI.select(row);
          else
            this.selectedPOI.deselect(row);  
          this.PoiCheckboxClicked(event,row);
        }
      });
    }
    else{
      this.poiDataSource.data.forEach((row: any) => {
        let search = rowData.alertLandmarkRefs.filter(item => item.refId == row.id && item.landmarkType == "P");
        if(search.length > 0) {
          this.selectedPOI.select(row);
          setTimeout(() => {
            this.PoiCheckboxClicked({checked : true}, row);  
          }, 1000);
        }
      });
    }
  }

  loadGeofenceData() {
    this.showLoadingIndicator = true;
    // this.geofenceService.getAllGeofences(this.accountOrganizationId).subscribe((geofencelist: any) => {
    this.geofenceService.getGeofenceDetails(this.accountOrganizationId).subscribe((geofencelist: any) => {
      // this.geofenceGridData = geofencelist.geofenceList;
      this.geofenceGridData = geofencelist;
     this.geofenceGridData = this.geofenceGridData.filter(item => item.type == "C" || item.type == "O");
      this.updateGeofenceDataSource(this.geofenceGridData);
      if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'duplicate')
        this.loadGeofenceSelectedData(this.geofenceGridData);
    });
    this.showLoadingIndicator = false;
  }

  loadGeofenceSelectedData(tableData: any){
    this.selectedGeofenceList= [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && (item.landmarkType == "C" || item.landmarkType == "O"));
        if (search.length > 0) {
          this.selectedGeofenceList.push(row);
          setTimeout(() => {
            this.geofenceCheckboxClicked({checked : true}, row);  
          }, 1000);
        }
      });
      tableData = this.selectedGeofenceList;
      this.displayedColumnsGeofence= ['name', 'categoryName', 'subCategoryName', 'address'];
      this.updateGeofenceDataSource(tableData);
    }
    else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
      this.selectGeofenceTableRows(this.selectedRowData);
    }
  }

  selectGeofenceTableRows(rowData: any, event?: any){
    if(event){
      this.geofenceDataSource.data.forEach((row: any) => {
        let search = rowData.landmarks.filter(item => item.landmarkid == row.id && (item.type == "C" || item.type == "O"));
        if (event && search.length > 0) {
          if(event.checked)
            this.selectedGeofence.select(row);
          else
            this.selectedGeofence.deselect(row);
          this.geofenceCheckboxClicked(event,row);
        }
      });
    }
    else{
      this.geofenceDataSource.data.forEach((row: any) => {
        let search = rowData.alertLandmarkRefs.filter(item => item.refId == row.id && (item.landmarkType == "C" || item.landmarkType == "O"));
        if(search.length > 0) {
          this.selectedGeofence.select(row);
          setTimeout(() => {
            this.geofenceCheckboxClicked({checked : true}, row);  
          }, 1000);
        }
      });
    }
  }

  loadGroupData(){
    this.showLoadingIndicator = true;
    let objData = { 
      organizationid : this.accountOrganizationId,
   };

    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((data: any) => {
      if(data){
        this.groupGridData = data["groups"];
        this.updateGroupDatasource(this.groupGridData);
        if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'duplicate'){
          this.loadGroupSelectedData(this.groupGridData);
        }
      }
      this.showLoadingIndicator = false;
    }, (error) => {
      ////console.log(error)
      this.showLoadingIndicator = false;
    });
  }

  loadGroupSelectedData(tableData: any){
    this.selectedGroupList= [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && item.landmarkType == 'G');
        if (search.length > 0) {
          this.selectedGroupList.push(row);
        }
      });
      tableData = this.selectedGroupList;
      this.displayedColumnsGroup= ['name', 'poiCount', 'geofenceCount'];
      this.updateGroupDatasource(tableData);
    }
    else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
      this.selectGroupTableRows();
    }
  }

  selectGroupTableRows(){
    this.groupDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && item.landmarkType == 'G');
      if (search.length > 0) {
        this.selectedGroup.select(row);
      }
    });
  }

  loadCorridorData(){
    this.showLoadingIndicator = true;
    this.corridorService.getCorridorList(this.accountOrganizationId).subscribe((data : any) => {
      this.corridorGridData = data;
      this.updateCorridorDatasource(this.corridorGridData);
      if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'duplicate'){
        this.loadCorridorSelectedData(this.corridorGridData);
      }
      this.showLoadingIndicator = false;
    }, (error) => {
      this.showLoadingIndicator = false;
    });
  }

  loadCorridorSelectedData(tableData: any){
    this.selectedCorridorList= [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && (item.landmarkType == "R" || item.landmarkType == "E"));
        if (search.length > 0) {
          this.selectedCorridorList.push(row);
          setTimeout(() => {
            this.corridorCheckboxClicked({checked : true}, row);  
          }, 1000);
        }
      });
      tableData = this.selectedCorridorList;
      this.displayedColumnsCorridor= ['corridoreName', 'startPoint', 'endPoint', 'distance', 'width'];
      this.updateCorridorDatasource(tableData);
    }
    else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
      this.selectCorridorTableRows();
    }
  }

  selectCorridorTableRows(){
    this.corridorDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && (item.landmarkType == "R" || item.landmarkType == "E"));
      if (search.length > 0) {
        this.selectedCorridor.select(row);
        setTimeout(() => {
          this.corridorCheckboxClicked({checked : true}, row);  
        }, 1000);
      }
    });
  }

  updatePOIDataSource(tableData: any){
    this.poiDataSource= new MatTableDataSource(tableData);
    this.poiDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.categoryName.toString().toLowerCase().includes(filter) ||
        data.subCategoryName.toString().toLowerCase().includes(filter) || 
        data.address.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.poiDataSource.paginator = this.paginator.toArray()[0];
      this.poiDataSource.sort = this.sort.toArray()[0];
      this.poiDataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any): any => {
          let columnName = sort.active;
          return this.compareOnalerteditview(a[sort.active], b[sort.active], isAsc, columnName);
        });
       }
    });
  }

  updateGeofenceDataSource(tableData: any){
    this.geofenceDataSource = new MatTableDataSource(tableData);
    this.geofenceDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.categoryName.toString().toLowerCase().includes(filter) ||
        data.subCategoryName.toString().toLowerCase().includes(filter) ||
        data.address.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.geofenceDataSource.paginator = this.paginator.toArray()[1];
      this.geofenceDataSource.sort = this.sort.toArray()[1];
      // this.geofenceDataSource.sortData = (data: String[], sort: MatSort) => {
      //   const isAsc = sort.direction === 'asc';
      //   return data.sort((a: any, b: any) => {
      //     return this.compare(a[sort.active], b[sort.active], isAsc);
      //   });
      //  }
      this.geofenceDataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any): any => {
          let columnName = sort.active;
          return this.compareOnalerteditview(a[sort.active], b[sort.active], isAsc, columnName);
        });
       }
    }, 2000);
  }

  compareOnalerteditview(a: any, b: any, isAsc: boolean, columnName:any) {
    if(!(a instanceof Number)) a = a ? a.replace(/\s/g, '').replace(/[^\w\s]/gi, 'z').toString().toUpperCase() : '';
    if(!(b instanceof Number)) b = b ? b.replace(/\s/g, '').replace(/[^\w\s]/gi, 'z').toString().toUpperCase() : '';
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }


  compareVehicleGroupList(a: any | String, b: any | String, isAsc: boolean) {
    a = parseInt(a.vehicleGroupId);
    b = parseInt(b.vehicleGroupId);
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  compareVehicleList(a: any | String, b: any | String, isAsc: boolean) {
    a = a.vehicleId;
    b = b.vehicleId;
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean) {
    if(!(a instanceof Number)) a = a.toUpperCase();
    if(!(b instanceof Number)) b = b.toUpperCase();
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  compareHere(a,b){
    if (a.vehicleGroupName < b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }

  updateGroupDatasource(tableData: any){
    this.groupDataSource = new MatTableDataSource(tableData);
    this.groupDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.name.toString().toLowerCase().includes(filter) ||
        data.poiCount.toString().toLowerCase().includes(filter) ||
        data.geofenceCount.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.groupDataSource.paginator = this.paginator.toArray()[2];
      this.groupDataSource.sort = this.sort.toArray()[2];
      this.groupDataSource.sortData = (data: String[], sort: MatSort) => {
         
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any): any => {
        let columnName = sort.active;
        if ( columnName == 'name') {
          return this.compareOnalerteditview(a[sort.active], b[sort.active], isAsc, columnName);
        } else {

          return this.comparenumber(a[sort.active], b[sort.active], isAsc);
        }
        });  
     }
    });
  }

  updateCorridorDatasource(tableData: any){
    this.corridorDataSource = new MatTableDataSource(tableData);
    this.corridorDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.corridoreName.toString().toLowerCase().includes(filter) ||
        data.startPoint.toString().toLowerCase().includes(filter) ||
        data.endPoint.toString().toLowerCase().includes(filter) ||
        data.distance.toString().toLowerCase().includes(filter) ||
        data.width.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.corridorDataSource.paginator = this.paginator.toArray()[0];
      this.corridorDataSource.sort = this.sort.toArray()[0];
      this.corridorDataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any): any => {
          let columnName = sort.active;
          if (columnName == 'corridoreName' ||  columnName == 'startPoint' || columnName == 'endPoint') {
            return this.compareCorridor(a[sort.active], b[sort.active], isAsc, columnName);
          } else {
            return this.comparenumber(a[sort.active], b[sort.active], isAsc);
          }
          
          
        });
       }
    });
  }

  compareCorridor(a: any, b: any, isAsc: boolean, columnName:any) {
    if(!(a instanceof Number)) a = a.replace(/\s/g, '').replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
    if(!(b instanceof Number)) b = b.replace(/\s/g, '').replace(/[^\w\s]/gi, 'z').toString().toUpperCase();
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  comparenumber(a: Number | String, b: Number | String, isAsc: boolean) {
    if(!(a instanceof Number)) a = a;
    if(!(b instanceof Number)) b = b;
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  onPOIClick(row: any){
    const colsList = ['icon', 'landmarkname', 'categoryname', 'subcategoryname', 'address'];
    const colsName = [this.translationData.lblicon, this.translationData.lblName, this.translationData.lblCategory, this.translationData.lblSubCategory, this.translationData.lblAddress];
    const tableTitle = this.translationData.lblPOI;
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
    };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => item.type == "P");
      if(this.selectedRowData.length > 0){
        this.selectedRowData.forEach(element => {
          if(element.icon && element.icon != '' && element.icon.length > 0){
            let TYPED_ARRAY = new Uint8Array(element.icon);
            let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
            let base64String = btoa(STRING_CHAR);
            element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + base64String);
          }else{
            element.icon = '';
          }
        });
        this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
      }
    });
  }

  onGeofenceClick(row: any){
    const colsList = ['landmarkname', 'categoryname', 'subcategoryname'];
    const colsName = ['Name', this.translationData.lblCategory, this.translationData.lblSubCategory];
    const tableTitle = this.translationData.lblGeofence;
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
   };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => (item.type == "C" || item.type == "O"));
      this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
    });
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: colsList,
      colsName: colsName,
      tableTitle: tableTitle,
      translationData: this.translationData
    }
    this.dialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
  }

  onChangeCriticalLevel(event){
    if(event.checked){
      this.isCriticalLevelSelected= true;
    }
    else{
      this.isCriticalLevelSelected= false;
      this.alertForm.get('criticalLevelThreshold').setValue('');
    }
  }
  
  onChangeWarningLevel(event){
    if(event.checked){
      this.isWarningLevelSelected= true;
    }
    else{
      this.isWarningLevelSelected= false;
      this.alertForm.get('warningLevelThreshold').setValue('');
    }
  }

  onChangeAdvisoryLevel(event){
    if(event.checked){
      this.isAdvisoryLevelSelected= true;
    }
    else{
      this.isAdvisoryLevelSelected= false;
      this.alertForm.get('advisoryLevelThreshold').setValue('');
    }
  }

  onReset(){ //-- Reset
    this.selectedPOI.clear();
    this.selectedGeofence.clear();
    // this.selectPOITableRows(this.selectedRowData);
    // this.selectGeofenceTableRows(this.selectedRowData);
    this.setDefaultValue();
    if(this.alert_type_selected == 'S'){ //hours of service
      this.periodSelectionComponent.setDefaultValues();
    }
    if(this.panelOpenState){
      this.notificationComponent.onReset();
    }
  }

  onCancel(){
    let emitObj = {
      actionFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onClosePanel(evt){
    this.panelOpenState = evt;
  }

  onApplyOnChange(event){   
    this.selectedApplyOn = event.value;
    if(this.selectedApplyOn != 'G'){
      // this.updateVehiclesList(this.alertTypeObject);
      this.getVehiclesForAlertType(this.alertTypeObject);
      this.getVehicleGroupsForAlertType(this.alertTypeObject);
    }else{
      this.alertForm.get('vehicleGroup').setValue('ALL');
      this.onChangeVehicleGroup('ALL');
    }
  }

  onClickAdvancedFilter(){
    this.openAdvancedFilter = !this.openAdvancedFilter;
  }

  onBackToPage(objData){
    // this.createEditStatus = objData.actionFlag;
    // this.viewStatus = objData.actionFlag;
    // if(objData.successMsg && objData.successMsg != ''){
    //   this.successMsgBlink(objData.successMsg);
    // }
    // this.loadScheduledReports();
    this.isNotificationFormValid= objData.isValidInput;
  }
  
  onNotifyEmailValid(objData){ 
    this.isNotifyEmailValid = objData.isValidInput;
  }
  onAdvancedAlertPayload(objData){ 
    this.isAdvancedAlertPayload = objData.isValidInput;
  }  
  onValidityCalender(objData){ 
    this.isValidityCalender = objData.isValidInput;
    this.isExpandedOpen=true;
  }


  convertThresholdValuesBasedOnUnits(){
    if(this.isCriticalLevelSelected){
      this.criticalThreshold =Number(this.alertForm.get('criticalLevelThreshold').value);
      if(this.alert_category_selected+this.alert_type_selected == 'LU' || this.alert_category_selected+this.alert_type_selected == 'LH' || this.alert_category_selected+this.alert_type_selected == 'FI'){
      this.criticalThreshold =this.reportMapService.getTimeInSeconds(this.criticalThreshold, this.unitTypeEnum);
      }
      else if(this.alert_category_selected+this.alert_type_selected == 'LD' || this.alert_category_selected+this.alert_type_selected == 'LG'){
        this.criticalThreshold =this.reportMapService.getConvertedDistanceToMeter(this.criticalThreshold, this.unitTypeEnum);
        }
        else if(this.alert_category_selected+this.alert_type_selected == 'FA'){
          this.criticalThreshold =this.reportMapService.getConvertedSpeedToMeterPerSec(this.criticalThreshold, this.unitTypeEnum);
          }
    }
    if(this.isWarningLevelSelected){
      this.warningThreshold = Number(this.alertForm.get('warningLevelThreshold').value);
      if(this.alert_category_selected+this.alert_type_selected == 'LU' || this.alert_category_selected+this.alert_type_selected == 'LH' || this.alert_category_selected+this.alert_type_selected == 'FI'){
      this.warningThreshold =this.reportMapService.getTimeInSeconds(this.warningThreshold, this.unitTypeEnum);
      }
      else if(this.alert_category_selected+this.alert_type_selected == 'LD' || this.alert_category_selected+this.alert_type_selected == 'LG'){
        this.warningThreshold =this.reportMapService.getConvertedDistanceToMeter(this.warningThreshold, this.unitTypeEnum);
        }
      else if(this.alert_category_selected+this.alert_type_selected == 'FA'){
          this.warningThreshold =this.reportMapService.getConvertedSpeedToMeterPerSec(this.warningThreshold, this.unitTypeEnum);
      }
    }
    if(this.isAdvisoryLevelSelected){
      this.advisoryThreshold =Number(this.alertForm.get('advisoryLevelThreshold').value);
      if(this.alert_category_selected+this.alert_type_selected == 'LU' || this.alert_category_selected+this.alert_type_selected == 'LH' || this.alert_category_selected+this.alert_type_selected == 'FI'){
      this.advisoryThreshold =this.reportMapService.getTimeInSeconds(this.advisoryThreshold, this.unitTypeEnum); 
      }
      else if(this.alert_category_selected+this.alert_type_selected == 'LD' || this.alert_category_selected+this.alert_type_selected == 'LG'){
        this.advisoryThreshold =this.reportMapService.getConvertedDistanceToMeter(this.advisoryThreshold, this.unitTypeEnum);
        }
      else if(this.alert_category_selected+this.alert_type_selected == 'FA'){
       this.advisoryThreshold =this.reportMapService.getConvertedSpeedToMeterPerSec(this.advisoryThreshold, this.unitTypeEnum);
      }
    }
  }

  onCreateUpdate(){  
    this.convertThresholdValuesBasedOnUnits();
    this.alertForm.markAllAsTouched();    
    if (!this.alertForm.valid) {      
      this.alertForm.markAllAsTouched();
      this.scrollToFirstInvalidControl();
    }
    else 
    {
     
      //this.alertForm.controls["vehicle"].setValidators([Validators.required]);
      this.alertForm.markAllAsTouched();
     // this.alertForm.controls["criticalLevelThreshold"].setValidators([Validators.required]);         
      this.scrollToVehicleInvalidControl();      
      this.alertForm.markAllAsTouched();
      this.scrollToFiltersDetailsInvalidControl();      
    }
     // Entering Zone, Exiting Zone
     if(this.alert_category_selected == 'L' && (this.alert_type_selected == 'N' || this.alert_type_selected == 'X')){
      if ((this.selectedPOI.selected.length == 0) && (this.selectedGeofence.selected.length == 0) && (this.selectedGroup.selected.length == 0)){
         const invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'searchForLevelPOI' + '"]');
         if (invalidControl) {          
           invalidControl.scrollIntoView({ behavior: 'smooth', block: 'center' }); 
           this.isEnteringZone =false; 
           this.isExpandedOpen=true;         
         }        
       }
       else{
        this.isEnteringZone =true; 
       }
    }
    // Exiting Corridor
    if(this.alert_category_selected == 'L' && this.alert_type_selected === 'C'){ 
      if(this.selectedCorridor.selected.length == 0){
        const invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'searchCorridor' + '"]');
        if (invalidControl) {          
          invalidControl.scrollIntoView({ behavior: 'smooth', block: 'center' }); 
          this.isEnteringZone =false;   
          this.isExpandedOpen=true;      
        }  
      } else{
        this.isEnteringZone =true; 
       }
    }

    if(this.isFormValidate)
    {
    let urgencylevelStartDate = 0;
    let urgencylevelEndDate = 0;
    if(this.panelOpenState){
      this.notifications= this.notificationComponent.getNotificationDetails();
      ////console.log(this.notifications); 
    }
    if(this.alert_category_selected == 'L' && this.alert_type_selected === 'S'){ //Hours of Service
      let alertTimingRefHoursOfService = this.periodSelectionComponent.getAlertTimingPayload();
      if(alertTimingRefHoursOfService.length>0)
      {
        this.isValidityCalender
      }
    }
    let periodType = 'A';   
 
    if(this.selectedApplyOnPeriod == 'C'){
     // this.alertTimingDetail = this.periodSelectionComponent.getAlertTimingPayload();
      urgencylevelStartDate = Util.convertDateToUtc(this.setStartEndDateTime(this.alertForm.controls.fromDate.value, this.alertForm.controls.fromTimeRange.value, "start"));
      urgencylevelEndDate = Util.convertDateToUtc(this.setStartEndDateTime(this.alertForm.controls.toDate.value, this.alertForm.controls.toTimeRange.value, "end"));;
      // this.alertTimingDetail.forEach(element => {
      //   element["type"] = "F";
      //   element["startDate"] =urgencylevelStartDate;
      //   element["endDate"] =urgencylevelEndDate;
      // });
    }
    else{
       // this.alertTimingDetail = [];
        urgencylevelStartDate = 0;
        urgencylevelEndDate = 0;
      }

      if(this.openAdvancedFilter == true){
        let alertAdvancedPayload = this.alertAdvancedComponent.getAdvancedFilterAlertPayload();
        this.alertFilterRefs = [];
        if(alertAdvancedPayload!=undefined){
        this.alertFilterRefs =alertAdvancedPayload["advancedAlertPayload"];
        urgencylevelStartDate =urgencylevelStartDate;
        urgencylevelEndDate = urgencylevelEndDate;
        }
        if(this.alertFilterRefs.length > 0 ){
          this.alertFilterRefs=  alertAdvancedPayload["advancedAlertPayload"].filter(i=>i!=undefined);
          periodType =this.selectedApplyOnPeriod == 'A' ? 'A' : 'C';     
          // if(this.alertFilterRefs.length>0){
          //   periodType = this.alertFilterRefs[0].alertTimingDetails.length == 0 ? 'A' : 'C'; 
          // }
        }
       
      }
      else{
        let alertAdvancedPayload =[];
        this.alertFilterRefs = [];
        if(alertAdvancedPayload!=undefined){
        this.alertFilterRefs =[];
        urgencylevelStartDate = urgencylevelStartDate;
        urgencylevelEndDate = urgencylevelEndDate;
        }   
        periodType = this.selectedApplyOnPeriod == 'A' ? 'A' : 'C';         
      }
 
      // if(this.openAdvancedFilter == true){
      //   let alertAdvancedPayload = this.alertAdvancedComponent.getAdvancedFilterAlertPayload();
      //   this.alertFilterRefs = [];
      //   if(alertAdvancedPayload!=undefined){
      //   this.alertFilterRefs =alertAdvancedPayload["advancedAlertPayload"];
      //   urgencylevelStartDate = alertAdvancedPayload["urgencylevelStartDate"];
      //   urgencylevelEndDate = alertAdvancedPayload["urgencylevelEndDate"];
      //   }
      //   if(this.alertFilterRefs.length > 0 ){
      //     this.alertFilterRefs=  alertAdvancedPayload["advancedAlertPayload"].filter(i=>i!=undefined);
      //     if(this.alertFilterRefs.length>0){
      //       periodType = this.alertFilterRefs[0].alertTimingDetails.length == 0 ? 'A' : 'C'; 
      //     }
      //   }
       
      // }
      // else{

      // }
   if(this.isNotifyEmailValid && this.isAdvancedAlertPayload && this.isEnteringZone && this.isValidityCalender && this.isFiltersDetailsValidate)
   {
    this.isDuplicateAlert= false;
    let alertUrgencyLevelRefs= [];
    let alertLandmarkRefs= [];
    let alertTimingRefHoursOfService= [];
    // let alertFilterRefs: any= [];
    let alertTimingRefAdvancedAlert= [];
    let urgenyLevelObj= {};
    if(this.actionType == 'create' || this.actionType == 'duplicate'){
      urgenyLevelObj = {
        "urgencyLevelType": this.alertForm.get('alertLevel').value,
        "thresholdValue": '0',
        "unitType": "N",
        "dayType": [
          false, false, false, false, false, false, false
        ],
        "periodType": periodType,
        "urgencylevelStartDate": urgencylevelStartDate,
        "urgencylevelEndDate": urgencylevelEndDate,
        "alertFilterRefs": this.alertFilterRefs,
        "alertTimingDetails" : alertTimingRefHoursOfService
      }
    }
    else if(this.actionType == 'edit'){
      urgenyLevelObj = {
        "urgencyLevelType": this.alertForm.get('alertLevel').value,
        "thresholdValue": '0',
        "unitType": "N",
        "dayType": [
          false, false, false, false, false, false, false
        ],
        "periodType": periodType,
        "urgencylevelStartDate": urgencylevelStartDate,
        "urgencylevelEndDate": urgencylevelEndDate,
        "id": this.selectedRowData.alertUrgencyLevelRefs[0].id,	
        "alertId": this.selectedRowData.id,
        "alertFilterRefs": this.alertFilterRefs,
        "alertTimingDetails" : alertTimingRefHoursOfService
      }
    }
    if((this.alert_category_selected == 'L' && 
        (this.alert_type_selected == 'N' || this.alert_type_selected == 'X' || this.alert_type_selected == 'C' || this.alert_type_selected == 'S')) || 
      this.alert_category_selected == 'R'){

      // if(this.actionType == 'create' || this.actionType == 'duplicate'){
      //   urgenyLevelObj = {
      //     "urgencyLevelType": this.alertForm.get('alertLevel').value,
      //     "thresholdValue": '0',
      //     "unitType": "N",
      //     "dayType": [
      //       false, false, false, false, false, false, false
      //     ],
      //     "periodType": periodType,
      //     "urgencylevelStartDate": urgencylevelStartDate,
      //     "urgencylevelEndDate": urgencylevelEndDate,
      //     "alertFilterRefs": this.alertFilterRefs,
      //     "alertTimingDetails" : alertTimingRefHoursOfService
      //   }
      // }
      // else if(this.actionType == 'edit'){
      //   urgenyLevelObj = {
      //     "urgencyLevelType": this.alertForm.get('alertLevel').value,
      //     "thresholdValue": '0',
      //     "unitType": "N",
      //     "dayType": [
      //       false, false, false, false, false, false, false
      //     ],
      //     "periodType": periodType,
      //     "urgencylevelStartDate": urgencylevelStartDate,
      //     "urgencylevelEndDate": urgencylevelEndDate,
      //     "id": this.selectedRowData.alertUrgencyLevelRefs[0].id,	
      //     "alertId": this.selectedRowData.id,
      //     "alertFilterRefs": this.alertFilterRefs,
      //     "alertTimingDetails" : alertTimingRefHoursOfService
      //   }
      // }
      if(this.alert_category_selected == 'L' && this.alert_type_selected === 'S'){ //Hours of Service
        alertTimingRefHoursOfService= this.periodSelectionComponent.getAlertTimingPayload();
        if(this.actionType == 'edit'){
          alertTimingRefHoursOfService.forEach(element => {
            element["refId"] = this.selectedRowData.alertUrgencyLevelRefs[0].id;;
          })
        }
        urgenyLevelObj["alertTimingDetails"] = alertTimingRefHoursOfService;
        // this.periodForm = this.periodSelectionComponent.periodSelectionForm;
      }
      alertUrgencyLevelRefs.push(urgenyLevelObj);

      // Entering Zone, Exiting Zone
      if(this.alert_category_selected == 'L' && (this.alert_type_selected == 'N' || this.alert_type_selected == 'X')){
       if(this.selectedPOI.selected.length > 0){
          if(this.actionType == 'create' || this.actionType == 'duplicate'){
            this.selectedPOI.selected.forEach(element => {
              let tempObj= {
                "landmarkType": "P",
                "refId": element.id,
                "distance": this.poiWidth,
                "unitType": "N"
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
          else if(this.actionType == 'edit'){
            this.selectedPOI.selected.forEach(element => {
              let poiLandmarkRefArr = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == element.id); 
              let tempObj= {
                "landmarkType": "P",
                "refId": element.id,
                "distance": this.poiWidth,
                "unitType": "N",
                "id": poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0,	
                "alertId": this.selectedRowData.id,
                "state": element.state == 'Active' ? 'A' : 'I'
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
        }
        if(this.selectedGeofence.selected.length > 0){
          if(this.actionType == 'create' || this.actionType == 'duplicate'){
            this.selectedGeofence.selected.forEach(element => {
              let tempObj= {
                "landmarkType": element.type,
                "refId": element.id,
                "distance": element.distance,
                "unitType": "N"
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
          else if(this.actionType == 'edit'){
            this.selectedGeofence.selected.forEach(element => {
              let geofenceLandmarkRefArr = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == element.id); 
              let tempObj= {
                "landmarkType": element.type,
                "refId": element.id,
                "distance": element.distance,
                "unitType": "N",
                "id": geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0,	
                "alertId": this.selectedRowData.id,
                "state": element.state == 'Active' ? 'A' : 'I'
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
        }
        if(this.selectedGroup.selected.length > 0){
          if(this.actionType == 'create' || this.actionType == 'duplicate'){
            this.selectedGroup.selected.forEach(element => {
              let tempObj= {
                "landmarkType": "G",
                "refId": element.id,
                "distance": 0,
                "unitType": "N"
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
          else if(this.actionType == 'edit'){
            this.selectedGroup.selected.forEach(element => {
              let groupLandmarkRefArr = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == element.id && item.landmarkType == 'G'); 
              let tempObj= {
                "landmarkType": "G",
                "refId": element.id,
                "distance": 0,
                "unitType": "N",
                "id": groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0,
                "alertId": this.selectedRowData.id,
                "state": 'A'
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
        }
      }
      else if(this.alert_category_selected == 'L' && this.alert_type_selected === 'C'){ // Exiting Corridor
        if(this.selectedCorridor.selected.length > 0){
          if(this.actionType == 'create' || this.actionType == 'duplicate'){
            this.selectedCorridor.selected.forEach(element => {
              let tempObj= {
                "landmarkType": element.corridorType,
                "refId": element.id,
                "distance": element.distance,
                "unitType": "N"
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
          else if(this.actionType == 'edit'){
            this.selectedCorridor.selected.forEach(element => {
              let corridorLandmarkRefArr = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == element.id); 
              let tempObj= {
                "landmarkType": element.corridorType,
                "refId": element.id,
                "distance": element.distance,
                "unitType": "N",
                "id": corridorLandmarkRefArr.length > 0 ? corridorLandmarkRefArr[0].id : 0,
                "alertId": this.selectedRowData.id,
                "state": element.state
              }
              alertLandmarkRefs.push(tempObj);
            });
          }
        }
      }
    }
    else{
      if(this.isCriticalLevelSelected){
        let criticalUrgenyLevelObj= {};      
        if(this.actionType == 'create' || this.actionType == 'duplicate'){
           criticalUrgenyLevelObj = {
            "urgencyLevelType": "C",
            "thresholdValue": this.criticalThreshold.toString(),
            "unitType": this.unitTypeEnum,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": periodType,
            "urgencylevelStartDate": urgencylevelStartDate,
            "urgencylevelEndDate": urgencylevelEndDate,
            "alertFilterRefs": this.alertFilterRefs,
            "alertTimingDetails" : alertTimingRefHoursOfService 
          }
        }
        else if(this.actionType == 'edit'){
          let urgencyLevelRefArr = this.selectedRowData.alertUrgencyLevelRefs.filter(item => item.urgencyLevelType == 'C'); 
            criticalUrgenyLevelObj = {
            "urgencyLevelType": "C",
            "thresholdValue": this.criticalThreshold.toString(),
            "unitType": this.unitTypeEnum,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": periodType,
            "urgencylevelStartDate": urgencylevelStartDate,
            "urgencylevelEndDate": urgencylevelEndDate,
            "id": urgencyLevelRefArr.length > 0 ? urgencyLevelRefArr[0].id : 0,
            "alertId": this.selectedRowData.id,
            "alertFilterRefs": this.alertFilterRefs,
            "alertTimingDetails" : alertTimingRefHoursOfService
          }
        }
        alertUrgencyLevelRefs.push(criticalUrgenyLevelObj);
      }
      if(this.isWarningLevelSelected){
        let warningUrgenyLevelObj= {};
        if(this.actionType == 'create' || this.actionType == 'duplicate'){
          warningUrgenyLevelObj = {
            "urgencyLevelType": "W",
            "thresholdValue": this.warningThreshold.toString(),
            "unitType": this.unitTypeEnum,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": periodType,
            "urgencylevelStartDate": urgencylevelStartDate,
            "urgencylevelEndDate": urgencylevelEndDate,
            "alertFilterRefs": this.alertFilterRefs,
            "alertTimingDetails" : alertTimingRefHoursOfService
          }
        }
        else if(this.actionType == 'edit'){
          let urgencyLevelRefArr = this.selectedRowData.alertUrgencyLevelRefs.filter(item => item.urgencyLevelType == 'W'); 
          warningUrgenyLevelObj = {
            "urgencyLevelType": "W",
            "thresholdValue": this.warningThreshold.toString(),
            "unitType": this.unitTypeEnum,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": periodType,
            "urgencylevelStartDate": urgencylevelStartDate,
            "urgencylevelEndDate": urgencylevelEndDate,
            "id": urgencyLevelRefArr.length > 0 ? urgencyLevelRefArr[0].id : 0,
            "alertId": this.selectedRowData.id,
            "alertFilterRefs": this.alertFilterRefs,
            "alertTimingDetails" : alertTimingRefHoursOfService
          }
        }
        alertUrgencyLevelRefs.push(warningUrgenyLevelObj);
      }
      if(this.isAdvisoryLevelSelected){
        let advisoryUrgenyLevelObj = {};
        if(this.actionType == 'create' || this.actionType == 'duplicate'){
          advisoryUrgenyLevelObj= {
            "urgencyLevelType": "A",
            "thresholdValue": this.advisoryThreshold.toString(),
            "unitType": this.unitTypeEnum,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": periodType,
            "urgencylevelStartDate": 0,
            "urgencylevelEndDate": 0,
            "alertFilterRefs": this.alertFilterRefs,
            "alertTimingDetails" : alertTimingRefHoursOfService
          }
        }
        else if(this.actionType == 'edit'){
          let urgencyLevelRefArr = this.selectedRowData.alertUrgencyLevelRefs.filter(item => item.urgencyLevelType == 'A');
          advisoryUrgenyLevelObj= {
            "urgencyLevelType": "A",
            "thresholdValue": this.advisoryThreshold.toString(),
            "unitType": this.unitTypeEnum,
            "dayType": [
              false, false, false, false, false, false, false
            ],
            "periodType": periodType,
            "urgencylevelStartDate": 0,
            "urgencylevelEndDate": 0,
            "id": urgencyLevelRefArr.length > 0 ? urgencyLevelRefArr[0].id : 0,
            "alertId": this.selectedRowData.id,
            "alertFilterRefs": this.alertFilterRefs,
            "alertTimingDetails" : alertTimingRefHoursOfService
          }
        }
        alertUrgencyLevelRefs.push(advisoryUrgenyLevelObj);
      }
    }

    if(this.actionType == 'create' || this.actionType == 'duplicate'){
        let createAlertObjData= {
          "organizationId": this.accountOrganizationId,
          "name": this.alertForm.get('alertName').value.trim(),
          "category": this.alert_category_selected,
          "type": this.alert_type_selected,
          "validityPeriodType": "A",
          "validityStartDate": 0,
          "validityEndDate": 0,
          "vehicleGroupId": this.vehicle_group_selected,
          "state": this.alertForm.get('statusMode').value,
          "applyOn": this.alertForm.get('applyOn').value,
          "createdBy": this.accountId,
          "notifications": this.notifications,
          "alertUrgencyLevelRefs": alertUrgencyLevelRefs,
          "alertLandmarkRefs": alertLandmarkRefs
        }
        this.showLoadingIndicator=true;
        this.alertService.createAlert(createAlertObjData).subscribe((data) => {
          if(data){
            this.alertCreatedMsg = this.getAlertCreatedMessage();
            let emitObj = { actionFlag: false, successMsg: this.alertCreatedMsg };
            this.backToPage.emit(emitObj);
          }  
          this.showLoadingIndicator=false;
        }, (error) => {
          this.showLoadingIndicator=false;
          if(error.status == 409 && error.error == 'Duplicate alert name')
          {
            this.isDuplicateAlert= true;
            const invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'alertName' + '"]');
            invalidControl.focus();
          }
          else if(error.status == 409 && error.error.includes('Duplicate notification recipient label')){
            this.notificationComponent.duplicateRecipientLabel();
          }
        })      
    }
    else if(this.actionType == 'edit'){
      let editAlertObjData= {
        "organizationId": this.accountOrganizationId,
        "name": this.alertForm.get('alertName').value.trim(),
        "category": this.alert_category_selected,
        "type": this.alert_type_selected,
        "validityPeriodType": "A",
        "validityStartDate": 0,
        "validityEndDate": 0,
        "vehicleGroupId": this.vehicle_group_selected,
        "state": this.alertForm.get('statusMode').value,
        "applyOn": this.alertForm.get('applyOn').value,
        "createdBy": this.accountId,
        "id": this.selectedRowData.id,
        "modifiedBy": this.accountId,
        "notifications": this.notifications,
        "alertUrgencyLevelRefs": alertUrgencyLevelRefs,
        "alertLandmarkRefs": alertLandmarkRefs
      }
      this.showLoadingIndicator=true;
      this.alertService.updateAlert(editAlertObjData).subscribe((data) => {
        if(data){
          this.alertCreatedMsg = this.getAlertCreatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.alertCreatedMsg };
          this.backToPage.emit(emitObj);
        }  
        this.showLoadingIndicator=false;
      }, (error) => {
        this.showLoadingIndicator=false;
        if(error.status == 409)
          this.isDuplicateAlert= true;
      })

    }
  }
}
}

  private scrollToFirstInvalidControl() {     
    //const invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'alertName' + '"]');
     const invalidControl: HTMLElement = this.el.nativeElement.querySelector(
        "form .ng-invalid"
      );  
    if (invalidControl) {  
      //invalidControl.scrollIntoView({ behavior: 'smooth' });
      this.isExpandedOpenAlert=true;
      invalidControl.scrollIntoView({ behavior: 'smooth', block: 'center' });
      this.isFormValidate=false;   
    }
    else{     
      this.isFormValidate=true;
     }
    
  }
  
  private scrollToVehicleInvalidControl() {     
    let invalidControl: HTMLElement ;   
    if(this.selectedApplyOn == 'G'){
      
    if((this.alertForm.controls.vehicleGroup.value == '' || this.alertForm.controls.vehicleGroup.value == 'ALL'))
    {
      invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'vehicleGroup' + '"]');
    }    
    
   }
    else {       
        if((this.alertForm.controls.vehicle.value == '' || this.isUnsubscribedVehicle)){
          this.alertForm.get('vehicle').setValue('');  
         invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'vehicle' + '"]');
        }   
    } 
     if (invalidControl) {  
         this.isExpandedOpenAlert=true;
         invalidControl.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'nearest' });     
         this.isFormValidate=false;
     }
     else{
      this.isFormValidate=true;
     }
    
  }
  
  
  private scrollToFiltersDetailsInvalidControl() {     
    let invalidControl: HTMLElement ;  
    this.filterDetailsErrorMsg='';
    // if( (this.alert_category_selected != 'R' && this.alert_type_selected != 'S' &&
    // (this.selectedPOI.selected.length == 0 && this.selectedGeofence.selected.length == 0 && this.selectedGroup.selected.length == 0) &&
    // (this.selectedCorridor.selected.length == 0)))
   if((this.alert_category_selected === 'L' && (this.alert_type_selected === 'H' || this.alert_type_selected === 'Y' || this.alert_type_selected === 'D' || this.alert_type_selected === 'U' || this.alert_type_selected === 'G')) || 
   (this.alert_category_selected === 'F' && (this.alert_type_selected === 'P' || this.alert_type_selected === 'L' || this.alert_type_selected === 'T' || this.alert_type_selected === 'I' || this.alert_type_selected === 'A' || this.alert_type_selected === 'F' )))    
      {
     if(this.filterDetailsCheck)
     {
        if((!this.isWarningLevelSelected && !this.isAdvisoryLevelSelected && !this.isCriticalLevelSelected))
        {
          this.isExpandedOpen=true;    
          this.filterDetailsCheck=true;
          this.isFiltersDetailsValidate=false;   
          invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'levelType' + '"]');
          this.filterDetailsErrorMsg ='Please select atleast one alerts level';
        }
        else if((!this.isWarningLevelSelected && !this.isAdvisoryLevelSelected && this.isCriticalLevelSelected && (this.alertForm.get('criticalLevelThreshold').value == '' || this.alertForm.get('criticalLevelThreshold').value == null)))
        {
          this.alertForm.get('criticalLevelThreshold').setValue('');  
          invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'criticalLevelThreshold' + '"]');
          this.filterDetailsErrorMsg= 'Critical level threshold cannot be empty';
        }
        else if((this.isWarningLevelSelected && !this.isAdvisoryLevelSelected && !this.isCriticalLevelSelected && (this.alertForm.get('warningLevelThreshold').value == '' || this.alertForm.get('warningLevelThreshold').value == null))){
          this.alertForm.get('warningLevelThreshold').setValue('');  
          invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'warningLevelThreshold' + '"]');
          this.filterDetailsErrorMsg= 'Warning level threshold cannot be empty';
          }
        else if((!this.isWarningLevelSelected && this.isAdvisoryLevelSelected && !this.isCriticalLevelSelected && (this.alertForm.get('advisoryLevelThreshold').value == '' || this.alertForm.get('advisoryLevelThreshold').value == null))){
          this.alertForm.get('advisoryLevelThreshold').setValue('');  
          invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'advisoryLevelThreshold' + '"]');
          this.filterDetailsErrorMsg= 'Advisory level threshold cannot be empty';
        }
        this.filterDetailsCheck=false;
      }
      else if((!this.isCriticalLevelSelected && !this.isWarningLevelSelected && !this.isAdvisoryLevelSelected)){
        this.isExpandedOpen=true;    
        this.filterDetailsCheck=true;
        this.isFiltersDetailsValidate=false;   
        invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'levelType' + '"]');
        this.filterDetailsErrorMsg ='Please select atleast one alerts level';
      }
       else if((this.isCriticalLevelSelected && ((this.alertForm.get('criticalLevelThreshold').value == ''))))
        {
         // this.alertForm.get('criticalLevelThreshold').setValue(''); 
          invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'criticalLevelThreshold' + '"]');
        }
        else if((this.isWarningLevelSelected && (this.alertForm.get('warningLevelThreshold').value == '')) ||
        ((this.isWarningLevelSelected && this.isCriticalLevelSelected) && Number(this.alertForm.get('warningLevelThreshold').value) >= Number(this.alertForm.get('criticalLevelThreshold').value))) 
        {
         // this.alertForm.get('warningLevelThreshold').setValue('');  
          invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'warningLevelThreshold' + '"]');
          this.filterDetailsErrorMsg =this.translationData.lblWarningValueShouldbeLess;
        }
        else if((this.isAdvisoryLevelSelected && (this.alertForm.get('advisoryLevelThreshold').value == '')) ||
        ((this.isAdvisoryLevelSelected && this.isWarningLevelSelected ) && Number(this.alertForm.get('advisoryLevelThreshold').value) >= Number(this.alertForm.get('warningLevelThreshold').value )) ||
        ((this.isAdvisoryLevelSelected && this.isCriticalLevelSelected) && Number(this.alertForm.get('advisoryLevelThreshold').value) >= Number(this.alertForm.get('criticalLevelThreshold').value )))
        {
         // this.alertForm.get('advisoryLevelThreshold').setValue(''); 
        invalidControl = this.el.nativeElement.querySelector('[formcontrolname="' + 'advisoryLevelThreshold' + '"]');
        this.filterDetailsErrorMsg =this.translationData.lblAdvisoryValueShouldBeLess;
      }           
    }
    else{
      invalidControl=null;  
      this.isFiltersDetailsValidate=true;  
    }
    if (invalidControl) {         
      // invalidControl.focus()
     invalidControl.scrollIntoView({ behavior: 'smooth', block: 'center' });
     this.isFiltersDetailsValidate=false;
    }else{
      this.isFiltersDetailsValidate=true;
     }
    
  }
  

  getAlertCreatedMessage() {
    let alertName = `${this.alertForm.controls.alertName.value}`;
    if(this.actionType == 'create' || this.actionType == 'duplicate') {
      // if(this.translationData.lblAlertCreatedSuccessfully)
      //   return this.translationData.lblAlertCreatedSuccessfully.replace('$', alertName);
      // else
      //   return ("Alert '$' Created Successfully").replace('$', alertName);
      return ("New Alert '$' Created Successfully").replace('$', alertName);
    }else if(this.actionType == 'edit') {
      // if (this.translationData.lblAlertUpdatedSuccessfully)
      //   return this.translationData.lblAlertUpdatedSuccessfully.replace('$', alertName);
      // else
      //   return ("Alert '$' Updated Successfully").replace('$', alertName);
      return ("Alert '$' Updated Successfully").replace('$', alertName);
    }
    else{
      return '';
    }
  }
  // urgencylevelStartDate = Util.convertDateToUtc(this.setStartEndDateTime(this.alertForm.controls.fromDate.value, this.alertForm.controls.fromTimeRange.value, "start"));
  // urgencylevelEndDate = Util.convertDateToUtc(this.setStartEndDateTime(this.alertForm.controls.toDate.value, this.alertForm.controls.toTimeRange.value, "end"));;

  onApplyPeriodOnChange(event: any){
    this.selectedApplyOnPeriod = event.value;
    this.periodSelectedDateTime=[];
    if(this.selectedApplyOnPeriod=='C')
    {
     this.periodSelectedDateTime=[{
      'startDate':new Date(this.alertForm.controls.fromDate.value),
      'fromTime': this.alertForm.controls.fromTimeRange.value,
      'endDate':new Date(this.alertForm.controls.toDate.value),
      'toTime':  this.alertForm.controls.toTimeRange.value
     }]
    }
  }
  applyFilterForVehicles(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.vehiclesDataSource.filter = filterValue;
  }

  applyFilterForPOI(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.poiDataSource.filter = filterValue;
  }

  applyFilterForGeofence(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.geofenceDataSource.filter = filterValue;
  }

  applyFilterForGroup(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.groupDataSource.filter = filterValue;
  }

  applyFilterForCorridor(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.corridorDataSource.filter = filterValue;
  }

  masterToggleForPOI() {
    this.isAllSelectedForPOI()
      ? this.selectedPOI.clear()
      : this.poiDataSource.data.forEach((row) =>
        this.selectedPOI.select(row)
      );
  }

  isAllSelectedForPOI() {
    const numSelected = this.selectedPOI.selected.length;
    const numRows = this.poiDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForPOI(row?: any): string {
    if (row)
      return `${this.isAllSelectedForPOI() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedPOI.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  masterToggleForGeofence() {
    this.isAllSelectedForGeofence()
      ? this.selectedGeofence.clear()
      : this.geofenceDataSource.data.forEach((row) =>
        this.selectedGeofence.select(row)
      );
  }

  isAllSelectedForGeofence() {
    const numSelected = this.selectedGeofence.selected.length;
    const numRows = this.geofenceDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeofence(row?: any): string {
    if (row)
      return `${this.isAllSelectedForGeofence() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedGeofence.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  masterToggleForGroup(event) {
    this.isAllSelectedForGroup()
      ? this.selectedGroup.clear()
      : this.groupDataSource.data.forEach((row) =>
        this.selectedGroup.select(row)
      );

      this.groupDataSource.data.forEach(row => {
        this.onGroupSelect(event, row);
      });
  }

  isAllSelectedForGroup() {
    const numSelected = this.selectedGroup.selected.length;
    const numRows = this.groupDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForGroup(row?: any): string {
    if (row)
      return `${this.isAllSelectedForGroup() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedGroup.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  masterToggleForCorridor() {
    this.isAllSelectedForCorridor()
      ? this.selectedCorridor.clear()
      : this.corridorDataSource.data.forEach((row) =>
        this.selectedCorridor.select(row)
      );
  }

  isAllSelectedForCorridor() {
    const numSelected = this.selectedCorridor.selected.length;
    const numRows = this.corridorDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForCorridor(row?: any): string {
    if (row)
      return `${this.isAllSelectedForCorridor() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedCorridor.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onGroupSelect(event: any, row: any){
    let groupDetails= [];
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
    };
    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupData) => {
      groupDetails = groupData["groups"][0];
      this.selectPOITableRows(groupDetails, event);
      this.selectGeofenceTableRows(groupDetails, event);
    });
  }

  onAddNotification(){
     this.panelOpenState = !this.panelOpenState;
     this.criticalThreshold = this.alertForm.get('criticalLevelThreshold').value;
     this.warningThreshold = this.alertForm.get('warningLevelThreshold').value;
     this.advisoryThreshold = this.alertForm.get('advisoryLevelThreshold').value; 
     if(this.isCriticalLevelSelected && this.criticalThreshold == ''){
      this.criticalLevel = false;
    }
    else if(this.isCriticalLevelSelected && this.criticalThreshold != ''){
      this.criticalLevel = true;
    }
    if(this.isWarningLevelSelected && this.warningThreshold == ''){
      this.warningLevel = false;
    }
    else if(this.isWarningLevelSelected && this.warningThreshold != ''){
      this.warningLevel = false;
    } 

    if(this.isAdvisoryLevelSelected && this.advisoryThreshold == ''){
      this.advisoryLevel = false;
    } 
    else if(this.isAdvisoryLevelSelected && this.advisoryThreshold != ''){
      this.advisoryLevel = false;
    }     
  }

  onDeleteNotification(){
    this.notifications= [];
    this.panelOpenState = !this.panelOpenState; 
  //   const options = {
  //     title: this.translationData.lblDeleteAlertNotification,
  //     message: this.translationData.lblAreousureyouwanttodeleteNotification,
  //     cancelText: this.translationData.lblCancel,
  //     confirmText: this.translationData.lblDelete
  //   };
  //   let name = this.selectedRowData.name;
  //   this.dialogService.DeleteModelOpen(options, name);
  //   this.dialogService.confirmedDel().subscribe((res) => {
  //   if (res) {
  //     this.notifications= [];
  //     this.panelOpenState = !this.panelOpenState;    
  //   }
  //  });
  }

  sliderChanged(){
    if(this.prefUnitFormat == 'dunit_Metric'){
     this.poiWidthKm = this.poiWidth / 1000;
    }
    else{
      this.poiWidthKm = this.poiWidth / 1609;
    }
     this.alertForm.controls.widthInput.setValue(this.poiWidthKm);
     if(this.markerArray.length > 0){
     this.addMarkerOnMap(this.ui);
     }
 }

 changeSliderInput(){
  this.poiWidthKm = this.alertForm.controls.widthInput.value;
  if(this.prefUnitFormat == 'dunit_Metric'){
  this.poiWidth = this.poiWidthKm * 1000;
  }
  else{
    this.poiWidthKm = this.poiWidth * 1609;
  }
}

keyPressNumbers(event) {    
  // var limit = parseInt(event.currentTarget.maxLength);
  // var max = parseInt(event.currentTarget.max);
  // var min = parseInt(event.currentTarget.min);
  // var exclude = /Backspace|Enter/;  
  // var value = Number.parseFloat(event.target.value + '' + event.key);
  // var parts = event.target.value.split('.');
  // if (parts.length == 2 && parts[1].length >= 2){
  //    event.preventDefault();
  // }
  // if(event.key=='-' || value < min) {
  //   event.preventDefault();
  // }

  let charCode = (event.which) ? event.which : event.keyCode;
  let number = event.target.value.split('.');
  if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
  }
  //just one dot
  if(number.length>1 && charCode == 46){
       return false;
  }
  var caratPos = event.target.selectionStart;
  var dotPos = event.target.value.indexOf(".");
  if( caratPos > dotPos && dotPos>-1 && (number[1].length > 1)){
      return false;
  }
    return true;     
}

onKey(event: any) { // without type info
  // let values += event.target.value + ' | ';
 //  //console.log('Key Up',event);
}

onChange($event){
this.criticalThreshold =Number(this.alertForm.controls.criticalLevelThreshold.value);
this.warningThreshold = Number(this.alertForm.controls.warningLevelThreshold.value);
this.advisoryThreshold =Number(this.alertForm.controls.advisoryLevelThreshold.value);
}

ngOnChanges(changes: SimpleChanges) {
  // let d =this.criticalLevel;
  // let e = this.warningLevel;
  // let f = this.advisoryLevel;
  for (const d in changes) {
    const chng1 = changes[d];
    const cur1  = JSON.stringify(chng1.currentValue);

  }}

  filterVehicleGroups(vehicleSearch){
    //console.log("filterVehicleGroups called");
    if(!this.vehicleGroupList){
      return;
    }
    if(!vehicleSearch){
      this.resetVehicleGroupFilter();
      return;
     } else{
       vehicleSearch = vehicleSearch.toLowerCase();
     }
     this.filteredVehicleGroups.next(
       this.vehicleGroupList.filter(item=> item.vehicleGroupName.toLowerCase().indexOf(vehicleSearch) > -1)
     );
     //console.log("this.filteredVehicleGroups", this.filteredVehicleGroups);
}

filterVehicles(VehicleSearch) {
    if (!this.vehicleByVehGroupList) {
      return;
    }
    if (!VehicleSearch) {
      this.resetVehiclesFilter();
      return;
    } else {
      VehicleSearch = VehicleSearch.toLowerCase();
    }
    let filterby = '';
    switch (this.vehicleDisplayPreference) {
      case 'dvehicledisplay_VehicleIdentificationNumber':
        filterby = "vin";
        break;
      case 'dvehicledisplay_VehicleName':
        filterby = "vehicleName";
        break;
      case 'dvehicledisplay_VehicleRegistrationNumber':
        filterby = "registrationNo";
        break;
      default:
        filterby = "vin";
    }
    this.filteredVehicles.next(
      this.vehicleByVehGroupList.filter(item => {
        if(filterby == 'registrationNo') {
          let ofilterby = (item['registrationNo'])? 'registrationNo' :'vehicleName';
          return item[ofilterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        } else {
          return item[filterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        }    
      })
    );
  }

setStartEndDateTime(date: any, timeObj: any, type: any){
  let _x = timeObj.split(":")[0];
  let _y = timeObj.split(":")[1];
  date = moment(date);
  let newDate = date._d;
  newDate.setHours(_x);
  newDate.setMinutes(_y);
  newDate.setSeconds(type == 'start' ? '00' : '59');
  return newDate;
}
  
}
