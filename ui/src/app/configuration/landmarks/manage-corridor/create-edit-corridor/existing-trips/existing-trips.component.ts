import { Component, ElementRef, EventEmitter, Inject, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { CorridorService } from '../../../../../services/corridor.service';
import { TranslationService } from '../../../../../../app/services/translation.service';
// import { AccountService } from '../../../services/account.service';
import { CustomValidators } from '../../../../../shared/custom.validators';
import { OrganizationService } from 'src/app/services/organization.service'
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import * as moment from 'moment';
import { POIService } from 'src/app/services/poi.service';
import {
  CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData
} from 'ng2-completer';
import { ConfigService } from '@ngx-config/core';
import { HereService } from 'src/app/services/here.service';
import { Options } from '@angular-slider/ngx-slider';
declare var H: any;
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { MapFunctionsService } from '../../map-functions.service';
import { ReplaySubject } from 'rxjs';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-existing-trips',
  templateUrl: './existing-trips.component.html',
  styleUrls: ['./existing-trips.component.less']
})
export class ExistingTripsComponent implements OnInit {
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @Input() translationData: any = {};
  @Input() exclusionList: any;
  @Input() actionType: any;
  @Input() selectedElementData: any;
  @Output() backToPage = new EventEmitter<any>();
  @Output() backToCreate = new EventEmitter<any>();
  @Output() backToReject = new EventEmitter<any>();

  endDate = new FormControl();
  startDate = new FormControl();
  startTime = new FormControl();
  // @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @Input() disabled: boolean;
  @Input() value: string = '11:00 PM';
  @Input() format: number = 12;
  @Input() vehicleGroupList: any;
  @Input() vinTripList: any;
  //internalSelection: boolean = false;
  vehicleGrpDD: any = [];
  vehicleDD: any = [];
  vehicleListData: any = [];
  vehicleGroupListData: any = [];
  accountPrefObj: any;
  prefTimeZone: any;
  newVehicleGrpList: any = [];
  prefUnitFormat: any = 'dunit_Metric';
  singleVehicle = [];
  selectedStartTime: any = '12:00 AM'
  selectedEndTime: any = '11:59 PM'
  selectedStartDateStamp: any;
  selectedEndDateStamp: any;
  startTimeUTC: any;
  endTimeUTC: any;
  selectionTab: any;
  timeValue: any;
  OrgId: any = 0;
  startAddressLatitudePoints: any = [];
  startAddressLongitudePoints: any = [];
  endAddressLatitudePoints: any = [];
  endAddressLongitudePoints: any = [];
  selectedAccounts = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  displayedColumns = ['All', 'DriverName', 'distance', 'startTimeStamp', 'startAddress', 'endAddress'];
  existingTripData: any = [];
  dataColValue: any = [];
  createEditStatus = false;
  corridorDistance: any = 0;
  accountOrganizationId: any = 0;
  corridorCreatedMsg: any = '';
  covertedDateValue: any = [];
  // actionType: string;
  titleVisible: boolean = false;
  titleFailVisible: boolean = false;
  showMap: boolean = false;
  map: any;
  initData: any = [];
  // dataSource: any;
  markerArray: any = [];
  tripsSelection: any = [];
  showLoadingIndicator: boolean;
  selectedCorridors = new SelectionModel(true, []);
  public position: string;
  public locations: Array<any>;
  setEndAddress: any = "";
  setStartAddress: any = "";
  vinList: any = [];
  vinListSelectedValue: any = [];
  vehicleGroupIdsSet: any = [];
  localStLanguage: any;
  // createExistingTripObj :any = {
  // id : 1,
  // sequence :"first"
  // }
  selectedTrips: any = [];
  internalNodePoints: any = [];
  getAttributeData: any;
  getExclusionList: any;
  getVehicleSize: any;
  additionalData: any;

  breadcumMsg: any = '';
  existingTripForm: FormGroup;
  corridorTypeList = [{ id: 1, value: 'Route Calculating' }, { id: 2, value: 'Existing Trips' }];
  trailerList = [0, 1, 2, 3, 4];
  selectedCorridorTypeId: any = 46;
  selectedTrailerId = 0;
  private platform: any;
  // map: any;
  private ui: any;
  lat: any = '37.7397';
  lng: any = '-121.4252';
  @ViewChild("map")
  public mapElement: ElementRef;
  hereMapService: any;
  organizationId: number;
  corridorId: number = 0;
  // localStLanguage: any;
  accountId: any = JSON.parse(localStorage.getItem("accountId"));
  filterValue: string;
  hereMap: any;
  distanceinKM = 0;
  viaRouteCount: boolean = false;
  transportDataChecked: boolean = false;
  trafficFlowChecked: boolean = false;
  corridorWidth: number = 100;
  corridorWidthKm: number = 0.1;
  sliderValue: number = 0;
  min: number = 0;
  max: number = 10000;
  map_key: string = "";
  map_id: string = "";
  map_code: string = "";
  mapGroup;
  searchStr: string = "";
  searchEndStr: string = "";
  searchViaStr: string = "";
  corridorName: string = "";
  startAddressPositionLat: number = 0; // = {lat : 18.50424,long : 73.85286};
  startAddressPositionLong: number = 0; // = {lat : 18.50424,long : 73.85286};
  startMarker: any;
  endMarker: any;
  routeCorridorMarker: any;
  routeOutlineMarker: any;
  endAddressPositionLat: number = 0;
  endAddressPositionLong: number = 0;


  // value: number = 100;
  options: Options = {
    floor: 0,
    ceil: 10000
  };
  searchStrError: boolean = false;
  searchEndStrError: boolean = false;
  strPresentStart: boolean = false;
  strPresentEnd: boolean = false;
  startAddressGroup: any = [];
  endAddressGroup: any = [];
  createFlag: boolean = true;
  showMapSection: boolean = false;

  //Integrating Time date component
  searchExpandPanel: boolean = true;
  mapExpandPanel: boolean = false;
  startDateValue: any;
  endDateValue: any;
  last3MonthDate: any;
  todayDate: any;
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any = 12; //-- coming from pref setting
  prefDateFormat: any = ''; //-- coming from pref setting
  noRecordFound: boolean = false;

  public filteredVehicleList: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private here: HereService,
    private _formBuilder: FormBuilder, private translationService: TranslationService,
    private corridorService: CorridorService, private poiService: POIService,
    private mapFunctions: MapFunctionsService, private organizationService: OrganizationService,
    private completerService: CompleterService, private config: ConfigService) {

    // this.map_key = config.getSettings("hereMap").api_key;
    // this.map_id = config.getSettings("hereMap").app_id;
    // this.map_code = config.getSettings("hereMap").app_code;
    this.map_key = localStorage.getItem("hereMapsK");

    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });
    // this.configureAutoCompleteForLocationSearch();

  }

  ngOnInit(): void {
    // //console.log("-------selectedElementData---", this.selectedElementData);
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.vehicleGroupList.forEach(item => {
      this.vehicleGroupIdsSet.push(item.vehicleGroupId);
      this.vehicleGroupIdsSet = [...new Set(this.vehicleGroupIdsSet)];
    });
    if (this.vehicleGroupIdsSet.length > 0) {
      this.vehicleGroupIdsSet.unshift(this.translationData.lblAll || 'All');
    };
    // //this.vinList = [];
    // this.vehicleGroupList.forEach(item => {
    //   this.vinList.push(item.vin)
    // });
    // if(this.vinList.length > 0){
    //   this.vinList.unshift(this.translationData.lblAll || 'All' );
    // };
    //this.filteredVehicleList.next(this.vinList);

    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.existingTripForm = this._formBuilder.group({
      // userGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      corridorType: ['Regular'],
      label: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      widthInput: ['', [Validators.required]],
      endaddress: ['', [Validators.required]],
      startaddress: ['', [Validators.required]],
      viaroute1: [''],
      viaroute2: [''],
      vehicleGroup: ['', [Validators.required]],
      vehicle: [this.vinList.length > 0 ? this.vinList[0].id : '', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []]
      // userGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
      {
        validator: [
          CustomValidators.specialCharValidationForName('label')
        ]
      });
    // let translationObj = {
    //   id: 0,
    //   code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
    //   type: "Menu",
    //   name: "",
    //   value: "",
    //   filter: "",
    //   menuId: 6 //-- for ExistingTrips
    // }
    // this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      // this.processTranslation(data);
     
      this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
        if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { // account pref
          this.proceedStep(prefData, this.accountPrefObj.accountPreference);
        } else { // org pref
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }

        // let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        // if(vehicleDisplayId) {
        //   let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        //   if(vehicledisplay.length != 0) {
        //     this.vehicleDisplayPreference = vehicledisplay[0].name;
        //   }
        // }

      });
    // });
    // this.existingTripForm.get('vehicleGroup');
    // this.existingTripForm.get('vehicle');


  }

  resetExistingTripFormControlValue() {
    if(this.vehicleGroupListData.length > 0){
      this.existingTripForm.get('vehicleGroup').setValue(0);
    }
    else {
      this.existingTripForm.get('vehicleGroup').setValue('');
    }

    this.existingTripForm.get('vehicle').setValue('');
  }

  proceedStep(prefData: any, preference: any) {
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      //this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0, 2));
      //this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
      //this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0, 2));
      //this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.filterDateData();
  }

  filterDateData() {
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    if (this.vinTripList.length > 0) {
      let vinArray = [];
      this.vinTripList.forEach(element => {
        if (element.endTimeStamp && element.endTimeStamp.length > 0) {
          let search = element.endTimeStamp.filter(item => (item >= currentStartTime) && (item <= currentEndTime));
          if (search.length > 0) {
            vinArray.push(element.vin);
          }
        }
      });
      if (vinArray.length > 0) {
        distinctVIN = vinArray.filter((value, index, self) => self.indexOf(value) === index);
        if (distinctVIN.length > 0) {
          distinctVIN.forEach(element => {
            let _item = this.vehicleGroupList.filter(i => i.vin === element);
            if (_item.length > 0) {
              this.vehicleListData.push(_item[0]); //-- unique VIN data added
              _item.forEach(element => {
                finalVINDataList.push(element);
              });
            }
          });
        }
      } else {
        this.existingTripForm.get('vehicle').setValue('');
        this.existingTripForm.get('vehicleGroup').setValue('');
      }
    }
    this.vehicleGroupListData = finalVINDataList;

    this.getVehicleGroupsForExistingTrip();
    this.resetExistingTripFormControlValue();

    // if(this.vehicleGroupListData.length > 0){
    //   let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
    //   if(_s.length > 0){
    //     _s.forEach(element => {
    //       let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
    //       if(count.length > 0){
    //         this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
    //         this.vehicleGrpDD.sort(this.compare);
    //         this.resetVehicleGroupFilter();
    //       }
    //     });
    //   }

    //   this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
    //   this.resetVehicleGroupFilter();
    // }

    // let vehicleData = this.vehicleListData.slice();
    // this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    // ////console.log("vehicleDD 1", this.vehicleDD);
    // this.vehicleDD.sort(this.compareVin);
    // this.resetVehicleSearch();

    // if(this.vehicleListData.length > 0){
    //   this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
    //   this.resetVehicleSearch();
    //   //this.resetTripFormControlValue();----later
    // };
    //this.setVehicleGroupAndVehiclePreSelection();
    // if(this.fromTripPageBack){ --------later
    //   this.onSearch();
    // }


  }



  compareVin(a, b) {
    if (a.vin < b.vin) {
      return -1;
    }
    if (a.vin > b.vin) {
      return 1;
    }
    return 0;
  }

  getUniqueVINs(vinList: any) {
    let uniqueVINList = [];
    for (let vin of vinList) {
      let vinPresent = uniqueVINList.map(element => element.vin).indexOf(vin.vin);
      if (vinPresent == -1) {
        uniqueVINList.push(vin);
      }
    }
    return uniqueVINList;
  }

  getUnique(arr, comp) {

    // store the comparison  values in array
    const unique = arr.map(e => e[comp])
      // store the indexes of the unique objects
      .map((e, i, final) => final.indexOf(e) === i && i)

      // eliminate the false indexes & return unique objects
      .filter((e) => arr[e]).map(e => arr[e]);
    return unique;
  }

  compareHere(a, b) {
    if (a.vehicleGroupName < b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }

  newVehicleList: any = [];

  getVehicleGroupsForExistingTrip() {
    this.newVehicleGrpList = [];
    this.newVehicleList = [];
    this.vinList = [];
    if (this.vehicleGroupListData.length > 0) {
      this.vehicleGroupListData.forEach(element => {
        let vehicleObj = {
          vehicleId: parseInt(element.vehicleId),
          vehicleName: element.vehicleName,
          vin: element.vin,
          vehicleRegistrationNo: element.registrationNo
        }
        this.newVehicleList.push(vehicleObj);
        this.vinList.push(vehicleObj.vin);
        let vehicleGroupDetails = element.vehicleGroupDetails.split(",");
        vehicleGroupDetails.forEach(item => {
          let itemSplit = item.split("~");
          // if(itemSplit[2] != 'S') {
          let vehicleGroupObj = {
            "vehicleGroupId": parseInt(itemSplit[0]),
            "vehicleGroupName": itemSplit[1],
            "vehicleId": parseInt(element.vehicleId),
          }
          this.newVehicleGrpList.push(vehicleGroupObj);
          // //console.log("vehicleGroupList 1", this.newVehicleGrpList);
          //  } else {
          //    this.singleVehicle.push(element);
          //  }
        });
      });
      this.newVehicleGrpList = this.getUnique(this.newVehicleGrpList, "vehicleGroupId");
      // //console.log("vehicleGroupList 2", this.newVehicleGrpList);
      this.newVehicleGrpList.sort(this.compareHere);


      // this.newVehicleGrpList.forEach(element => {
      //   element.vehicleGroupId = parseInt(element.vehicleGroupId);
      // });

      this.newVehicleGrpList.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll });
      this.resetVehicleGroupFilter();
      this.resetVehicleSearch();

    }

  }

  resetVehicleGroupFilter() {
    this.filteredVehicleGroups.next(this.newVehicleGrpList.slice());
  }

  public ngAfterViewInit() {
    //For Edit Screen
    // if((this.actionType === 'edit' || this.actionType === 'view') && this.selectedElementData){
    //   this.setCorridorData();
    //   this.createFlag = false;
    //   this.strPresentStart = true;
    //   this.strPresentEnd = true;
    // }
    // this.subscribeWidthValue();
    // this.existingTripForm.controls.widthInput.setValue(this.corridorWidthKm);
    setTimeout(() => {
      this.mapFunctions.initMap(this.mapElement, this.translationData);
    }, 0);
  }

  setDefaultStartEndTime() {
    // this.selectedStartTime = "00:00";
    // this.selectedEndTime = "23:59";
    this.setPrefFormatTime();
  }

  setPrefFormatTime() {
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

  setPrefFormatDate() {
    switch (this.prefDateFormat) {
      case 'dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        this.dateFormats.parse.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        this.dateFormats.parse.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'mm-dd-yyyy': {
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

  startTimeChanged(selectedTime: any) {
    this.selectedStartTime = selectedTime;
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = selectedTime + ':00';
    }
    else {
      this.startTimeDisplay = selectedTime;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
  }

  endTimeChanged(selectedTime: any) {
    this.selectedEndTime = selectedTime;
    if (this.prefTimeFormat == 24) {
      this.endTimeDisplay = selectedTime + ':59';
    }
    else {
      this.endTimeDisplay = selectedTime;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
  }

  getTodayDate() {
    let todayDate = new Date(); //-- UTC
    todayDate.setHours(0);
    todayDate.setMinutes(0);
    todayDate.setSeconds(0);
    return todayDate;
  }

  getYesterdaysDate() {
    var date = new Date();
    date.setDate(date.getDate() - 1);
    return date;
  }

  getLastWeekDate() {
    var date = new Date();
    date.setDate(date.getDate() - 7);
    return date;
  }

  getLastMonthDate() {
    let date = new Date();
    date.setDate(date.getDate() - 30);
    return date;
  }

  getLast3MonthDate() {
    let date = new Date();
    date.setDate(date.getDate() - 90);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  selectionTimeRange(selection: any) {
    switch (selection) {
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
    this.filterDateData();
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    let dateTime: any = '';
    if (event.value._d.getTime() >= this.last3MonthDate.getTime()) { // CurTime > Last3MonthTime
      if (event.value._d.getTime() <= this.endDateValue.getTime()) { // CurTime < endDateValue
        dateTime = event.value._d;
      } else {
        dateTime = this.endDateValue;
      }
    } else {
      dateTime = this.last3MonthDate;
    }
    this.startDateValue = this.setStartEndDateTime(dateTime, this.selectedStartTime, 'start');
    this.filterDateData();
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>) {
    let dateTime: any = '';
    if (event.value._d.getTime() <= this.todayDate.getTime()) { // EndTime > todayDate
      if (event.value._d.getTime() >= this.startDateValue.getTime()) { // EndTime < startDateValue
        dateTime = event.value._d;
      } else {
        dateTime = this.startDateValue;
      }
    } else {
      dateTime = this.todayDate;
    }
    this.endDateValue = this.setStartEndDateTime(dateTime, this.selectedEndTime, 'end');
    this.filterDateData();
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    let _x = timeObj.split(":")[0];
    let _y = timeObj.split(":")[1];
    if (this.prefTimeFormat == 12) {
      if (_y.split(' ')[1] == 'AM') {
        if (_x == 12) {
          date.setHours(0);
        } else {
          date.setHours(_x);
        }
      }
      else if (_y.split(' ')[1] == 'PM') {
        if (_x != 12) {
          date.setHours(parseInt(_x) + 12);
        }
        else {
          date.setHours(_x);
        }
      }
      date.setMinutes(_y.split(' ')[0]);
    } else {
      date.setHours(_x);
      date.setMinutes(_y);
    }

    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }


  onSearch() {
    // https://api.dev1.ct2.atos.net/poi/getalltripdetails?vin=XLR0998HGFFT76657&startdatetime=1604336137000&enddatetime=1604337449000

    //     VIN: "5A39727"
    // _startTime = 1604336137000
    //_endTime = 1604337449000
    // StartDateTime=1078724200000&EndDateTime=2078724200000&VIN=XLR0998HGFFT76657

    let _startTime = this.startDateValue.getTime();
    let _endTime = this.endDateValue.getTime();
    //For testing data
    // _startTime = 1604336137000;
    // _endTime = 1604337449000;
    // this.vinListSelectedValue= "XLR0998HGFFT76657";

    // this.vinListSelectedValue = "5A39727"
    // _startTime = 1604336137000
    // _endTime = 1604337449000
    this.poiService.getalltripdetails(_startTime, _endTime, this.vinListSelectedValue).subscribe((existingTripDetails: any) => {
      this.showLoadingIndicator = true;
      this.initData = existingTripDetails.tripData;
      if(this.initData.length == 0) {
        this.noRecordFound = true;
      } else {
        this.noRecordFound = false;
      }
      this.hideloader();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.noRecordFound = true;
      this.updatedTableData(this.initData);
    });

  }


  onReset() {
    // this.selectedStartTime = '00:00:00';
    // this.selectedEndTime= '23:59:59';
    this.startTimeDisplay = '00:00:00';
    this.endTimeDisplay = '23:59:59';
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    // this.existingTripForm.get('startTime').setValue(this.selectedStartTime);
    // this.existingTripForm.get('endTime').setValue(this.selectedEndTime);
    this.vinListSelectedValue = '';
    //this.vinList = [];
    this.initData = [];
    this.noRecordFound = false;
    this.updatedTableData(this.initData);
    this.filterDateData();
  }

  setDefaultTodayDate() {
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
  }

  initMap() {
    let defaultLayers = this.platform.createDefaultLayers();
    //Step 2: initialize a map - this map is centered over Europe
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      //center:{lat:41.881944, lng:-87.627778},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });

    // add a resize listener to make sure that the map occupies the whole container
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());

    // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));


    // Create the default UI components
    var ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
    var group = new H.map.Group();
    this.mapGroup = group;
  }



  subscribeWidthValue() {
    this.existingTripForm.get("widthInput").valueChanges.subscribe(x => {
      this.corridorWidthKm = Number(x);
      this.corridorWidth = this.corridorWidthKm * 1000;
      this.calculateAB();
    })
  }
  setCorridorData() {
    let _selectedElementData = this.selectedElementData;
    if (_selectedElementData) {
      this.corridorId = _selectedElementData.id;
      if (this.corridorId) {
        this.corridorService.getCorridorFullList(this.organizationId, this.corridorId).subscribe((data) => {
          if (data[0]["corridorProperties"]) {
            this.additionalData = data[0]["corridorProperties"];
            this.setAdditionalData();

          }
        })
      }
      this.corridorName = _selectedElementData.corridoreName;
      this.existingTripForm.controls.label.setValue(_selectedElementData.corridoreName);
      this.searchStr = _selectedElementData.startPoint;
      this.searchEndStr = _selectedElementData.endPoint;
      this.startAddressPositionLat = _selectedElementData.startLat;
      this.startAddressPositionLong = _selectedElementData.startLong;
      this.endAddressPositionLat = _selectedElementData.endLat;
      this.endAddressPositionLong = _selectedElementData.endLong;
      this.corridorWidth = _selectedElementData.width;
      this.corridorWidthKm = this.corridorWidth / 1000;
      this.plotStartPoint(this.searchStr);
      this.plotEndPoint(this.searchEndStr);
      this.calculateAB()
    }
  }
  vehicleHeightValue: number = 0;
  vehicleWidthValue: number = 0;
  vehicleLengthValue: number = 0;
  vehicleLimitedWtValue: number = 0;
  vehicleWtPerAxleValue: number = 0;
  setAdditionalData() {
    let _data = this.additionalData;
    this.getAttributeData = _data["attribute"];
    this.getExclusionList = _data["exclusion"];
    this.selectedTrailerId = this.getAttributeData["noOfTrailers"];
    this.trafficFlowChecked = _data["isTrafficFlow"];
    this.transportDataChecked = _data["isTransportData"];
    this.getVehicleSize = _data["vehicleSize"];
    this.vehicleHeightValue = this.getVehicleSize.vehicleHeight;
    this.vehicleWidthValue = this.getVehicleSize.vehicleWidth;
    this.vehicleLengthValue = this.getVehicleSize.vehicleLength;
    this.vehicleLimitedWtValue = this.getVehicleSize.vehicleLimitedWeight;
    this.vehicleWtPerAxleValue = this.getVehicleSize.vehicleWeightPerAxle;


    this.existingTripForm.controls.vehicleHeight.setValue(this.getVehicleSize.vehicleHeight);
    this.existingTripForm.controls.vehicleWidth.setValue(this.getVehicleSize.vehicleWidth);
    this.existingTripForm.controls.vehicleLength.setValue(this.getVehicleSize.vehicleLength);
    this.existingTripForm.controls.limitedWeight.setValue(this.getVehicleSize.vehicleLimitedWeight);
    this.existingTripForm.controls.weightPerAxle.setValue(this.getVehicleSize.vehicleWeightPerAxle);

    this.initiateDropDownValues();

  }

  initiateDropDownValues() {
    this.existingTripForm.controls.widthInput.setValue(this.corridorWidthKm);
  }


  createHomeMarker() {
    const homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#0D7EE7"/>
    <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
    <path fill-rule="evenodd" clip-rule="evenodd" d="M7.75 13.3394H5.5L13 6.58936L20.5 13.3394H18.25V19.3394H13.75V14.8394H12.25V19.3394H7.75V13.3394ZM16.75 11.9819L13 8.60687L9.25 11.9819V17.8394H10.75V13.3394H15.25V17.8394H16.75V11.9819Z" fill="#436DDC"/>
    </svg>`
    return homeMarker;
  }

  createEndMarker() {
    const endMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#D50017"/>
    <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
    <path d="M13 18.9644C16.3137 18.9644 19 16.5019 19 13.4644C19 10.4268 16.3137 7.96436 13 7.96436C9.68629 7.96436 7 10.4268 7 13.4644C7 16.5019 9.68629 18.9644 13 18.9644Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    </svg>`
    return endMarker;
  }

  createViaMarker() {
    const viaMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.6665C18.6665 24.9998 24.3332 19.2591 24.3332 12.9998C24.3332 6.74061 19.2591 1.6665 12.9998 1.6665C6.74061 1.6665 1.6665 6.74061 1.6665 12.9998C1.6665 19.2591 7.6665 25.3332 12.9998 29.6665Z" fill="#0D7EE7"/>
    <path d="M13 22.6665C18.5228 22.6665 23 18.4132 23 13.1665C23 7.9198 18.5228 3.6665 13 3.6665C7.47715 3.6665 3 7.9198 3 13.1665C3 18.4132 7.47715 22.6665 13 22.6665Z" fill="white"/>
    <path d="M19.7616 12.6263L14.0759 6.94057C13.9169 6.78162 13.7085 6.70215 13.5 6.70215C13.2915 6.70215 13.0831 6.78162 12.9241 6.94057L7.23842 12.6263C6.92053 12.9444 6.92053 13.4599 7.23842 13.778L12.9241 19.4637C13.0831 19.6227 13.2915 19.7021 13.5 19.7021C13.7085 19.7021 13.9169 19.6227 14.0759 19.4637L19.7616 13.778C20.0795 13.4599 20.0795 12.9444 19.7616 12.6263ZM13.5 18.3158L8.38633 13.2021L13.5 8.08848L18.6137 13.2021L13.5 18.3158ZM11.0625 12.999V15.0303C11.0625 15.1425 11.1534 15.2334 11.2656 15.2334H12.0781C12.1904 15.2334 12.2812 15.1425 12.2812 15.0303V13.4053H14.3125V14.7695C14.3125 14.8914 14.4123 14.9731 14.5169 14.9731C14.5644 14.9731 14.6129 14.9564 14.6535 14.9188L16.7916 12.9452C16.8787 12.8647 16.8787 12.7271 16.7916 12.6466L14.6535 10.673C14.6129 10.6357 14.5644 10.6187 14.5169 10.6187C14.4123 10.6187 14.3125 10.7004 14.3125 10.8223V12.1865H11.875C11.4263 12.1865 11.0625 12.5504 11.0625 12.999Z" fill="#0D7EE7"/>
    </svg>`

    return viaMarker;
  }

  // loadExistingTripData() {

  // }

  // suggestionData: any;
  // dataService: any;
  // private configureAutoCompleteForLocationSearch() {
  //   let searchParam = this.searchEndStr !== null ? this.searchEndStr : this.searchStr != null ? this.searchStr : this.searchViaStr;
  //   let AUTOCOMPLETION_URL = 'https://autocomplete.geocoder.cit.api.here.com/6.2/suggest.json' + '?' +
  //     '&maxresults=5' +  // The upper limit the for number of suggestions to be included in the response.  Default is set to 5.
  //     '&app_id=' + this.map_id + // TODO: Store this configuration in Config File.
  //     '&app_code=' + this.map_code +  // TODO: Store this configuration in Config File.
  //     '&query=' + searchParam;
  //   this.suggestionData = this.completerService.remote(
  //     AUTOCOMPLETION_URL,
  //     "label",
  //     "label");
  //   this.suggestionData.dataField("suggestions");
  //   this.dataService = this.suggestionData;
  // }

  hideloader() {
    this.showLoadingIndicator = false;
  }
  vehicleGroupSelection(vehicleGroupValue: any) {
    this.vinList = [];
    // //console.log("----vehicleGroupList---",this.vehicleGroupList)
    if (vehicleGroupValue.value == 0) {
      this.newVehicleList.forEach(item => {
        this.vinList.push(item.vin)
      });
    }
    else {
      this.newVehicleGrpList.forEach(element => {
        if (element.vehicleGroupId == parseInt(vehicleGroupValue.value)) {
          let vehicleFound: any = this.newVehicleList.filter(i => i.vehicleId == element.vehicleId);
          if (vehicleFound.length > 0) {
            this.vinList.push(vehicleFound[0].vin);
          }
        }
      });
    }

    this.resetVehicleSearch();
  }

  // ------------- Map Functions ------------------------//
  plotStartPoint(_locationId) {
    let geocodingParameters = {
      searchText: _locationId,
    };
    this.here.getLocationDetails(geocodingParameters).then((result) => {
      this.startAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.startAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createHomeMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });

      this.startMarker = new H.map.Marker({ lat: this.startAddressPositionLat, lng: this.startAddressPositionLong }, { icon: icon });
      var group = new H.map.Group();
      this.hereMap.addObject(this.startMarker);
      //this.hereMap.getViewModel().setLookAtData({zoom: 8});
      //this.hereMap.setZoom(8);
      this.hereMap.setCenter({ lat: this.startAddressPositionLat, lng: this.startAddressPositionLong }, 'default');
      this.checkRoutePlot();

    });
  }

  plotEndPoint(_locationId) {
    let geocodingParameters = {
      searchText: _locationId,
    };
    this.here.getLocationDetails(geocodingParameters).then((result) => {
      this.endAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.endAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createEndMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });

      this.endMarker = new H.map.Marker({ lat: this.endAddressPositionLat, lng: this.endAddressPositionLong }, { icon: icon });
      this.hereMap.addObject(this.endMarker);
      // this.hereMap.getViewModel().setLookAtData({bounds: this.endMarker.getBoundingBox()});
      //this.hereMap.setZoom(8);
      this.hereMap.setCenter({ lat: this.endAddressPositionLat, lng: this.endAddressPositionLong }, 'default');
      this.checkRoutePlot();

    });

  }

  viaAddressPositionLat: any;
  viaAddressPositionLong: any;
  viaRoutePlottedObject: any = [];
  viaMarker: any;
  plotViaPoint(_viaRouteList) {
    this.viaRoutePlottedObject = [];
    if (this.viaMarker) {
      this.hereMap.removeObjects([this.viaMarker]);
      this.viaMarker = null;
    }
    if (_viaRouteList.length > 0) {
      for (var i in _viaRouteList) {

        let geocodingParameters = {
          searchText: _viaRouteList[i],
        };
        this.here.getLocationDetails(geocodingParameters).then((result) => {
          this.viaAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
          this.viaAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
          let viaMarker = this.createViaMarker();
          let markerSize = { w: 26, h: 32 };
          const icon = new H.map.Icon(viaMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });

          this.viaMarker = new H.map.Marker({ lat: this.viaAddressPositionLat, lng: this.viaAddressPositionLong }, { icon: icon });
          this.hereMap.addObject(this.viaMarker);
          // this.hereMap.getViewModel().setLookAtData({bounds: this.endMarker.getBoundingBox()});
          //this.hereMap.setZoom(8);
          this.hereMap.setCenter({ lat: this.viaAddressPositionLat, lng: this.viaAddressPositionLong }, 'default');
          this.viaRoutePlottedObject.push({
            "viaRoutName": _viaRouteList[i],
            "latitude": this.viaAddressPositionLat,
            "longitude": this.viaAddressPositionLat
          });
          this.checkRoutePlot();

        });
      }

    }
    else {
      this.checkRoutePlot();

    }
  }

  calculateAB() {
    let viaPoints = [];
    for (var i in this.viaRoutePlottedObject) {
      viaPoints.push(`${this.viaRoutePlottedObject[i]["latitude"]},${this.viaRoutePlottedObject[i]["longitude"]}`)
    }
    let routeRequestParams = {}
    if (viaPoints.length > 0) {

      routeRequestParams = {
        'routingMode': 'fast',
        'transportMode': 'truck',
        'origin': `${this.startAddressPositionLat},${this.startAddressPositionLong}`,
        'via': viaPoints,//`${this.viaAddressPositionLat},${this.viaAddressPositionLong}`,
        'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`,
        'return': 'polyline'
      };
    }
    else {

      routeRequestParams = {
        'routingMode': 'fast',
        'transportMode': 'truck',
        'origin': `${this.startAddressPositionLat},${this.startAddressPositionLong}`,
        'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`,
        'return': 'polyline'
      };
    }
    this.here.calculateRoutePoints(routeRequestParams).then((data) => {

      this.addRouteShapeToMap(data);
    }, (error) => {
      console.error(error);
    })
  }

  addRouteShapeToMap(result) {
    var group = new H.map.Group();
    result.routes[0].sections.forEach((section) => {
      let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);

      this.routeOutlineMarker = new H.map.Polyline(linestring, {
        style: {
          lineWidth: this.corridorWidthKm,
          strokeColor: '#b5c7ef',
        }
      });
      // Create a patterned polyline:
      this.routeCorridorMarker = new H.map.Polyline(linestring, {
        style: {
          lineWidth: 3,
          strokeColor: '#436ddc'
        }
      }
      );
      // create a group that represents the route line and contains
      // outline and the pattern
      var routeLine = new H.map.Group();
      // routeLine.addObjects([routeOutline, routeArrows]);
      this.hereMap.addObjects([this.routeOutlineMarker, this.routeCorridorMarker]);
      this.hereMap.getViewModel().setLookAtData({ bounds: this.routeCorridorMarker.getBoundingBox() });

    });
  }

  sliderChanged() {
    // this.corridorWidth = _event.value;
    this.corridorWidthKm = this.corridorWidth / 1000;
    this.existingTripForm.controls.widthInput.setValue(this.corridorWidthKm);
    this.mapFunctions.updateWidth(this.corridorWidthKm, true);
    this.checkRoutePlot();
    //this.calculateRouteFromAtoB();
  }

  checkRoutePlot() {
    if (this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0) {
      this.calculateAB();
    }
  }
  changeSliderInput() {
    this.corridorWidthKm = this.existingTripForm.controls.widthInput.value;
    this.corridorWidth = this.corridorWidthKm * 1000;
  }

  formatLabel(value: number) {
    return value;
  }

  addViaRoute() {
    this.viaRouteCount = true;
  }

  removeViaRoute() {
    this.viaRouteCount = false;
  }

  transportDataCheckedFn(_checked) {
    this.transportDataChecked = _checked;
  }


  trafficFlowCheckedFn(_checked) {
    this.trafficFlowChecked = _checked;
  }


  trailerSelected(_event) {
    this.selectedTrailerId = _event.value;
  }


  createCorridorClicked() {

    this.tripsSelection.map((items) => {

      if (items.liveFleetPosition.length > 0) {
        items.liveFleetPosition.forEach(element => {
          let nodePointObj = {
            "id": element.id,
            "landmarkId": 0,
            "tripId": items.tripId,
            "sequenceNumber": element.id,
            "latitude": element.gpsLatitude,
            "longitude": element.gpsLongitude,
            "state": "A",
            "address": "",
            "createdAt": 0,
            "createdBy": 0,
            "modifiedAt": 0,
            "modifiedBy": 0
          }
          this.internalNodePoints.push(nodePointObj)
        });


      }

      // //console.log("------- Node points--",this.internalNodePoints)
      // //console.log("-------all slected Values--", items)
      this.startAddressLatitudePoints.push(items.startPositionlattitude)
      this.startAddressLongitudePoints.push(items.startPositionLongitude)
      this.endAddressLatitudePoints.push(items.endPositionLattitude)
      this.endAddressLongitudePoints.push(items.endPositionLongitude)
      let createExistingTripObj = {
        "id": items.id,
        "landmarkId": 0,
        "tripId": items.tripId,
        "startDate": items.startTimeStamp,
        "endDate": items.endTimeStamp,
        "driverId1": items.driverId1,
        "driverId2": items.driverId2,
        "startLatitude": items.startPositionlattitude,
        "startLongitude": items.startPositionLongitude,
        "startPosition": items.startAddress,
        "endLatitude": items.endPositionLattitude,
        "endLongitude": items.endPositionLongitude,
        "endPosition": items.endAddress,
        "distance": items.distance,
        "nodePoints": [...this.internalNodePoints],
      }
      this.corridorDistance = this.corridorDistance + items.distance;
      this.selectedTrips.push(createExistingTripObj)
    })

    var existingTripObj = {
      "id": this.corridorId ? this.corridorId : 0,
      "organizationId": this.accountOrganizationId,
      "corridorType": "E",
      "corridorLabel": this.existingTripForm.controls.label.value,
      "width": this.corridorWidth,
      "createdAt": 0,
      "createdBy": this.accountId,
      "modifiedAt": 0,
      "modifiedBy": 0,
      "description": "",
      "address": "",
      "city": "",
      "country": "",
      "zipcode": "",
      "startLatitude": this.startAddressLatitudePoints[0],
      "startLongitude": this.startAddressLongitudePoints[0],
      "distance": this.corridorDistance,
      "state": "",
      "existingTrips": [...this.selectedTrips]
    }

    //console.log("------existingTrip Create Obj--", existingTripObj)
    this.corridorService.createExistingCorridor(existingTripObj).subscribe((responseData) => {
      if (responseData.code === 200) {
        let emitObj = {
          booleanFlag: false,
          successMsg: "create",
          fromCreate: true,
          CreateCorridorName: this.existingTripForm.controls.label.value
        }
        this.backToCreate.emit(emitObj);
      }
    }, (error) => {
      if (error.status === 409) {
        let emitObj = {
          booleanFlag: false,
          successMsg: "duplicate",
          fromCreate: true,
          CreateCorridorName: this.existingTripForm.controls.label.value
        }
        this.backToReject.emit(emitObj);
      }
    })
  }

  getSuggestion(_event) {
    let startValue = _event.target.value;
  }

  backToCorridorList() {
    let emitObj = {
      booleanFlag: false,
      successMsg: "",
    }
    this.backToPage.emit(emitObj);
  }

  resetToEditData() {
    // this.setDefaultStartEndTime();
    this.searchStrError = false;
    this.searchEndStrError = false;
    this.setCorridorData();
  }

  filterVehicles(search) {
    if (!search) {
      this.resetVehicleSearch();
      return;
    } else {
      search = search.toLowerCase();
    }
    this.filteredVehicleList.next(
      this.vinList.filter(item => item.toLowerCase().indexOf(search) > -1)
    );
  }

  resetVehicleSearch() {
    this.filteredVehicleList.next(this.vinList.slice());
  }

  resetValues() {
    if (this.actionType === 'create') {
      this.transportDataChecked = false;
      this.trafficFlowChecked = false;
      this.corridorWidth = 100;
      this.corridorWidthKm = 0.1;
      this.existingTripForm.controls.vehicleHeight.setValue("");
      this.existingTripForm.controls.vehicleLength.setValue("");
      this.existingTripForm.controls.vehicleWidth.setValue("");
      this.existingTripForm.controls.limitedWeight.setValue("");
      this.existingTripForm.controls.weightPerAxle.setValue("");
      this.existingTripForm.controls.startaddress.setValue("");
      this.existingTripForm.controls.endaddress.setValue("");
    }
    else {
      this.setAdditionalData();
    }
  }

  clearMap() {
    if (this.startMarker && this.endMarker) {
      this.hereMap.removeObjects([this.startMarker, this.endMarker, this.routeOutlineMarker, this.routeCorridorMarker]);

    }

  }


  onStartFocus() {
    this.searchStrError = true;
    this.strPresentStart = false;
    this.searchStr = null;
    this.startAddressPositionLat = 0;
    this.hereMap.removeObjects(this.hereMap.getObjects());

    if (this.searchEndStr) {
      this.plotEndPoint(this.searchEndStr);
    }

  }
  onEndFocus() {
    this.searchEndStrError = true;
    this.strPresentEnd = false;
    this.searchEndStr = null;
    this.endAddressPositionLat = 0;
    this.hereMap.removeObjects(this.hereMap.getObjects());

    if (this.searchStr) {
      this.plotStartPoint(this.searchStr);
    }
  }

  onSelected(selectedAddress: CompleterItem) {
    if (this.searchStr) {
      this.searchStrError = false;
      this.strPresentStart = true;
    }
    if (selectedAddress) {
      let postalCode = selectedAddress["originalObject"]["label"];
      this.plotStartPoint(postalCode)
    }

  }

  onEndSelected(selectedAddress: CompleterItem) {
    if (this.searchEndStr) {
      this.searchEndStrError = false;
      this.strPresentEnd = true;
    }
    if (selectedAddress) {
      let locationId = selectedAddress["originalObject"]["label"]
      this.plotEndPoint(locationId)
    }

  }

  masterToggleForCorridor() {
    this.markerArray = [];
    if (this.isAllSelectedForCorridor()) {
      this.selectedCorridors.clear();
      this.mapFunctions.clearRoutesFromMap();
      this.showMap = false;
    }
    else {
      this.dataSource.data.forEach((row) => {
        this.selectedCorridors.select(row);
        this.markerArray.push(row);
      });
      this.mapFunctions.viewSelectedRoutes(this.markerArray);
      this.showMap = true;
    }
    // //console.log("---markerArray---",this.markerArray);
    this.setAllAddressValues(this.markerArray);

  }

  isAllSelectedForCorridor() {
    const numSelected = this.selectedCorridors.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForCorridor(row?: any): string {
    if (row)
      return `${this.isAllSelectedForCorridor() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedCorridors.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  checkboxClicked(event: any, row: any) {
    // this.showMapSection = true;
    let startAddress = row.startPositionlattitude + "," + row.startPositionLongitude;
    let endAddress = row.endPositionLattitude + "," + row.endPositionLongitude;
    // this.position = row.startPositionlattitude + "," + row.startPositionLongitude;

    if (event.checked) { //-- add new marker
      this.markerArray.push(row);
      this.mapFunctions.viewSelectedRoutes(this.markerArray);
      this.tripsSelection.push(row);
      //console.log("----this.tripsSelection.push(row);------", this.tripsSelection);

    } else { //-- remove existing marker
      //It will filter out checked points only
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
      this.tripsSelection = this.markerArray.filter(item => item.id !== row.id);
      //console.log("----this.tripsSelection.push(row);------", this.tripsSelection);
      this.mapFunctions.clearRoutesFromMap();
      this.mapFunctions.viewSelectedRoutes(this.markerArray);
    }
    //console.log("---markerArray--", this.markerArray)

    this.setAllAddressValues(this.markerArray);
  }

  updatedTableData(tableData: any) {
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    // //console.log("------dataSource--", this.dataSource)
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.sort.disableClear = true;
      this.dataSource.filterPredicate = function(data: any, filter: string): boolean {
        let driverName = data.driverFirstName+" "+data.driverLastName;
        let startDate = moment(data.startTimeStamp).format("DD/MM/YYYY-h:mm:ss")
        return (  
          driverName.toString().toLowerCase().includes(filter) || 
          data.distance.toString().toLowerCase().includes(filter) ||
          startDate.toString().toLowerCase().includes(filter) ||
          data.startAddress.toString().toLowerCase().includes(filter) ||
          data.endAddress.toString().toLowerCase().includes(filter)
        );
      };
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
          let columnName = sort.active;
          if(columnName !== 'DriverName'){
            return this.compare(a[sort.active], b[sort.active], isAsc , columnName);
            }else{
            const currentName = a.driverFirstName+" "+a.driverLastName;
            const nextName = b.driverFirstName+" "+b.driverLastName;
            return this.compare(currentName, nextName, isAsc , columnName);
            }
          // if(columnName === date){
          //   return this.compare(a[sort.active], b[sort.active], isAsc , date);
          // }
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
    });
    Util.applySearchFilter(this.dataSource, this.displayedColumns, this.filterValue);
  }
  compare(a: any, b: any, isAsc: boolean, columnName: any) {
    if (columnName == "startTimeStamp") {
      if (!(a instanceof Number)) a = a.toString().toUpperCase();
      if (!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    if (columnName === "distance") {
      var aa = a;
      var bb = b;
      return (aa < bb ? -1 : 1) * (isAsc ? 1 : -1);
    }
    if (columnName === "startAddress" || columnName === "endAddress") {
      if (!(a instanceof Number)) a = a.replace(/[^\w\s]/gi, 'z').toUpperCase();
      if (!(b instanceof Number)) b = b.replace(/[^\w\s]/gi, 'z').toUpperCase();
    }

    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  setAllAddressValues(markerArray: any) {
    if (this.markerArray.length > 0) {
      this.mapExpandPanel = true;
    } else {
      this.mapExpandPanel = false;

    }
    let allStartAddress = [];
    let allEndAddress = [];
    markerArray.forEach(getAllAddrress => {
      getAllAddrress.startAddress
      allStartAddress.push(getAllAddrress.startAddress);
      allStartAddress = [...new Set(allStartAddress)];

      allEndAddress.push(getAllAddrress.endAddress);
      allEndAddress = [...new Set(allEndAddress)];

    });
    this.setStartAddress = allStartAddress.toString();
    this.existingTripForm.get('startaddress').setValue(this.setStartAddress);
    this.setEndAddress = allEndAddress.toString();
    this.existingTripForm.get('endaddress').setValue(this.setEndAddress);

  }

  getNewTagData(data: any) {
    let currentDate = new Date().getTime();
    if (data.length > 0) {
      data.forEach(row => {
        let createdDate = parseInt(row.createdAt);
        let nextDate = createdDate + 86400000;
        if (currentDate > createdDate && currentDate < nextDate) {
          row.newTag = true;
        }
        else {
          row.newTag = false;
        }
      });
      let newTrueData = data.filter(item => item.newTag == true);
      newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
      let newFalseData = data.filter(item => item.newTag == false);
      Array.prototype.push.apply(newTrueData, newFalseData);
      return newTrueData;
    }
    else {
      return data;
    }
  }

  vinSelection(vinSelectedValue: any) {
    this.vinListSelectedValue = vinSelectedValue.value;
    if (vinSelectedValue.value == 'All')
      this.vinListSelectedValue = this.vinList;
    // //console.log("------vins selection--", this.vinListSelectedValue)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  setDate(date: any) {
    if (date === 0) {
      return "-";
    } else {
      var dateValue = moment(date).format("DD/MM/YYYY-h:mm:ss")
      return dateValue;
    }
  }


}
