import { Component, Input, OnInit, OnDestroy, Inject, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { NgxMaterialTimepickerComponent } from 'ngx-material-timepicker';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportService } from 'src/app/services/report.service';
import { UtilsService } from 'src/app/services/utils.service';
import { Util } from '../../../shared/util';
import { ReportMapService } from '../../report-map.service';
import { ReplaySubject } from 'rxjs';
import { TranslationService } from 'src/app/services/translation.service';

@Component({
  selector: 'app-search-criteria',
  templateUrl: './search-criteria.component.html',
  styleUrls: ['./search-criteria.component.less']
})
export class SearchCriteriaComponent implements OnInit, OnDestroy {
  @Input() translationData: any = {};
  @Input() performanceTypeLst;
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @Output() showSearchResult = new EventEmitter();
  @Output() hideSearchResult = new EventEmitter();
  accountPrefObj;
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  accountOrganizationId;
  accountId;
  searchForm: FormGroup;
  searchExpandPanel: boolean = true;
  formSubmitted: boolean = false;
  selectionTab: string = 'today';
  prefTimeFormat: any = 12;
  prefTimeZone: any;
  prefDateFormat;
  prefUnitFormat;
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  globalSearchFilterData: any = {};
  internalSelection: boolean = false;
  todayDate: any;
  last3MonthDate: any;
  wholeTripData: any = [];
  vehicleDD: any = [];
  singleVehicle: any = [];
  vehicleGrpDD: any = [];
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  maxStartTime: any;
  selectedStartTimeValue: any ='00:00';
  selectedEndTimeValue: any ='11:59';
  endTimeStart:any;
  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private formBuilder: FormBuilder, private organizationService: OrganizationService, private utilsService: UtilsService, private reportService: ReportService, private reportMapService: ReportMapService, private translationService: TranslationService) {
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    if (!this.utilsService.isEmpty(localStorage.getItem("globalSearchFilterData"))) {
      this.globalSearchFilterData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    }
  }

  ngOnInit(): void {
    this.searchForm = this.formBuilder.group({
      vehicleGroup: [0, [Validators.required]],
      vehicleName: ['', [Validators.required]],
      performanceType: ['E', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['00:00', []],
      endTime: ['23:59', []]
    });
  }

  getPreferences(prefData) {
    if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') { // account pref
      this.proceedStep(prefData, this.accountPrefObj.accountPreference);
    } else {
      this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
        this.proceedStep(prefData, orgPref);
      }, (error) => {
        this.proceedStep(prefData, {});
      });
    }
    this.loadWholeTripData();
  }

  selectionTimeRange(selection: any) {
    this.internalSelection = true;
    switch (selection) {
      case 'today': {
        this.selectionTab = 'today';
        this.setDefaultStartEndTime();
        this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.getTodayDate(), this.searchForm.get('startTime').value, 'start'));
        this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.getTodayDate(), this.searchForm.get('endTime').value, 'end'));
        break;
      }
      case 'yesterday': {
        this.selectionTab = 'yesterday';
        this.setDefaultStartEndTime();
        this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.getYesterdaysDate(), this.searchForm.get('startTime').value, 'start'));
        this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.getYesterdaysDate(), this.searchForm.get('endTime').value, 'end'));
        break;
      }
      case 'lastweek': {
        this.selectionTab = 'lastweek';
        this.setDefaultStartEndTime();
        this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.getLastWeekDate(), this.searchForm.get('startTime').value, 'start'));
        this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.getYesterdaysDate(), this.searchForm.get('endTime').value, 'end'));
        break;
      }
      case 'lastmonth': {
        this.selectionTab = 'lastmonth';
        this.setDefaultStartEndTime();
        this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.getLastMonthDate(), this.searchForm.get('startTime').value, 'start'));
        this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.getYesterdaysDate(), this.searchForm.get('endTime').value, 'end'));
        break;
      }
      case 'last3month': {
        this.selectionTab = 'last3month';
        this.setDefaultStartEndTime();
        this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.getLast3MonthDate(), this.searchForm.get('startTime').value, 'start'));
        this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.getYesterdaysDate(), this.searchForm.get('endTime').value, 'end'));
        break;
      }
    }
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  resetDropdownValues() {
    if (!this.internalSelection && !this.utilsService.isEmpty(this.globalSearchFilterData)) {
      this.searchForm.get('vehicleGroup').setValue(this.globalSearchFilterData["vehicleGroupDropDownValue"]);
      this.searchForm.get('vehicleName').setValue(this.globalSearchFilterData["vehicleDropDownValue"]);
    } else {
      this.searchForm.get('vehicleGroup').setValue(0);
      this.searchForm.get('vehicleName').setValue('');
    }
    this.searchForm.get('performanceType').setValue('E');
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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

  getTodayDate() {
    if (this.prefTimeZone) {
      let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
      _todayDate.setHours(0);
      _todayDate.setMinutes(0);
      _todayDate.setSeconds(0);
      return _todayDate;
    }
    return null;
  }

  getYesterdaysDate() {
    if (this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setDate(date.getDate() - 1);
      return date;
    }
    return null;
  }

  getLastWeekDate() {
    if (this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setDate(date.getDate() - 7);
      return date;
    }
    return null;
  }

  getLastMonthDate() {
    if (this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setMonth(date.getMonth() - 1);
      return date;
    }
    return null;
  }

  getLast3MonthDate() {
    if (this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setMonth(date.getMonth() - 3);
      date.setHours(0);
      date.setMinutes(0);
      date.setSeconds(0);
      return date;
    }
    return null;
  }

  proceedStep(prefData: any, preference: any) {
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
    if (vehicleDisplayId) {
      let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
      if (vehicledisplay.length != 0) {
        this.vehicleDisplayPreference = vehicledisplay[0].name;
      }
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.resetDropdownValues();
  }

  loadWholeTripData() {
    this.reportService.getVINFromTripVehicleperformance(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
    this.wholeTripData = tripData;
      this.filterDateData();
    }, (error) => {
      this.wholeTripData.vinTripList = [];
      this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
    });
  }

  filterDateData() {
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];

    let currentStartTime = Util.getMillisecondsToUTCDate(this.searchForm.get('startDate').value, this.prefTimeZone);
    let currentEndTime = Util.getMillisecondsToUTCDate(this.searchForm.get('endDate').value, this.prefTimeZone);
    if (this.wholeTripData && this.wholeTripData.vinTripList && this.wholeTripData.vinTripList.length > 0) {
      let vinArray = [];
      this.wholeTripData.vinTripList.forEach(element => {
        if (element.endTimeStamp && element.endTimeStamp.length > 0) {
          let search = element.endTimeStamp.filter(item => (item >= currentStartTime) && (item <= currentEndTime));
          if (search.length > 0) {
            vinArray.push(element.vin);
          }
        }
      });
      this.singleVehicle = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.groupType == 'S');
      if (vinArray.length > 0) {
        distinctVIN = vinArray.filter((value, index, self) => self.indexOf(value) === index);
        if (distinctVIN.length > 0) {
          distinctVIN.forEach(element => {
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element && i.groupType != 'S');
            if (_item.length > 0) {
              this.vehicleListData.push(_item[0]); //-- unique VIN data added
              _item.forEach(element => {
                finalVINDataList.push(element)
              });
            }
          });
        }
      } else {
        if (this.searchForm) {
          this.searchForm.get('vehicleName').setValue('');
          this.searchForm.get('vehicleGroup').setValue('');
        }
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    if (this.vehicleGroupListData && this.vehicleGroupListData.length > 0) {
      let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
      if (_s.length > 0) {
        _s.forEach(element => {
          let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
          if (count.length > 0) {
            this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
            this.vehicleGrpDD.sort(this.compare);
            this.resetVehicleGroupFilter();
          }
        });
      }
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.resetVehicleGroupFilter();
    }
    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    this.vehicleDD.sort(this.compareVin);
    this.resetVehicleFilter();
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

  setDefaultStartEndTime() {
    if (!this.internalSelection && !this.utilsService.isEmpty(this.globalSearchFilterData)) {
      if (this.prefTimeFormat == this.globalSearchFilterData.filterPrefTimeFormat) { // same format
        this.searchForm.get('startTime').setValue(this.globalSearchFilterData.startTimeStamp);
        this.searchForm.get('endTime').setValue(this.globalSearchFilterData.endTimeStamp);
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.startTimeStamp}:00` : this.globalSearchFilterData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.globalSearchFilterData.endTimeStamp}:59` : this.globalSearchFilterData.endTimeStamp;
      } else { // different format
        if (this.prefTimeFormat == 12) { // 12
          this.searchForm.get('startTime').setValue(this._get12Time(this.globalSearchFilterData.startTimeStamp));
          this.searchForm.get('endTime').setValue(this._get12Time(this.globalSearchFilterData.endTimeStamp));
          this.startTimeDisplay = this.searchForm.get('startTime').value;
          this.endTimeDisplay = this.searchForm.get('endTime').value;
        } else { // 24
          this.searchForm.get('startTime').setValue(this.get24Time(this.globalSearchFilterData.startTimeStamp));
          this.searchForm.get('endTime').setValue(this.get24Time(this.globalSearchFilterData.endTimeStamp));
          this.startTimeDisplay = `${this.searchForm.get('startTime').value}:00`;
          this.endTimeDisplay = `${this.searchForm.get('endTime').value}:59`;
        }
      }
    } else {
      if (this.prefTimeFormat == 24) {
        this.startTimeDisplay = '00:00:00';
        this.endTimeDisplay = '23:59:59';
        this.searchForm.get('startTime').setValue("00:00");
        this.searchForm.get('endTime').setValue("23:59");
      } else {
        this.startTimeDisplay = '12:00 AM';
        this.endTimeDisplay = '11:59 PM';
        this.searchForm.get('startTime').setValue("12:00 AM");
        this.searchForm.get('endTime').setValue("11:59 PM");
      }
    }

  }

  setDefaultTodayDate() {
    if (!this.internalSelection && !this.utilsService.isEmpty(this.globalSearchFilterData)) {
      if (this.utilsService.isEmpty(this.globalSearchFilterData.timeRangeSelection)) {
        this.selectionTab = 'today';
      } else {
        this.selectionTab = this.globalSearchFilterData.timeRangeSelection;
      }
      let startDateFromSearch = new Date(this.globalSearchFilterData.startDateStamp);
      let endDateFromSearch = new Date(this.globalSearchFilterData.endDateStamp);
      this.searchForm.get('startDate').setValue(this.setStartEndDateTime(startDateFromSearch, this.searchForm.get('startTime').value, 'start'));
      this.searchForm.get("endDate").setValue(this.setStartEndDateTime(endDateFromSearch, this.searchForm.get('endTime').value, 'end'));
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
    } else {
      this.selectionTab = 'today';
      this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.getTodayDate(), this.searchForm.get('startTime').value, 'start'));
      this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.getTodayDate(), this.searchForm.get('endTime').value, 'end'));
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
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

  onVehicleGroupChange(event: any) {
    if (event.value || event.value == 0) {
      this.internalSelection = true;
      if (parseInt(event.value) == 0) { //-- all group
        let vehicleData = this.vehicleListData.slice();
        this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
        //console.log("vehicleDD 2", this.vehicleDD);
        this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      } else {
        let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        const arrayUniqueByKey = [...new Map(search.map(item => [item['vehicleId'], item])).values()];
        if (arrayUniqueByKey.length > 0) {
          this.vehicleDD = [];
          arrayUniqueByKey.forEach(element => {
            this.vehicleDD.push(element);
          });
          this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
        }
      }
      this.searchForm.get('vehicleName').setValue('');
      this.searchForm.get('vehicleName').enable();
    }
    else {
      this.searchForm.get('vehicleGroup').setValue(parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue));
    }
    this.resetVehicleFilter();
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    let dateTime: any = '';
    if (event.value._d.getTime() >= this.last3MonthDate.getTime()) { // CurTime > Last3MonthTime
      if (event.value._d.getTime() <= this.searchForm.get("endDate").value.getTime()) { // CurTime < endDateValue
        dateTime = event.value._d;
      } else {
        dateTime = this.searchForm.get("endDate").value;
      }
    } else {
      dateTime = this.last3MonthDate;
    }
    this.searchForm.get('startDate').setValue(this.setStartEndDateTime(dateTime, this.searchForm.get('startTime').value, 'start'));
    let startDate1 = this.searchForm.get('startDate').value.getFullYear() + "/" + (this.searchForm.get('startDate').value.getMonth() + 1) + "/" + this.searchForm.get('startDate').value.getDate();
    let endDate1 = this.searchForm.get('endDate').value.getFullYear() + "/" + (this.searchForm.get('endDate').value.getMonth() + 1) + "/" + this.searchForm.get('endDate').value.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.searchForm.get('endTime').value;
      this.endTimeStart = this.searchForm.get('startTime').value; 
    }
    else{
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    let dateTime: any = '';
    if (event.value._d.getTime() <= this.todayDate.getTime()) { // EndTime > todayDate
      if (event.value._d.getTime() >= this.searchForm.get("startDate").value.getTime()) { // EndTime < startDateValue
        dateTime = event.value._d;
      } else {
        dateTime = this.searchForm.get("startDate").value;
      }
    } else {
      dateTime = this.todayDate;
    }
    this.searchForm.get("endDate").setValue(this.setStartEndDateTime(dateTime, this.searchForm.get('endTime').value, 'end'));
    let startDate1 = this.searchForm.get('startDate').value.getFullYear() + "/" + (this.searchForm.get('startDate').value.getMonth() + 1) + "/" + this.searchForm.get('startDate').value.getDate();
    let endDate1 = this.searchForm.get('endDate').value.getFullYear() + "/" + (this.searchForm.get('endDate').value.getMonth() + 1) + "/" + this.searchForm.get('endDate').value.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.searchForm.get('endTime').value;
      this.endTimeStart = this.searchForm.get('startTime').value; 
    }
    else{
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.searchForm.get('endTime').setValue(this.selectedEndTimeValue);
    if (this.prefTimeFormat == 24) {
      this.endTimeDisplay = this.selectedEndTimeValue + ':59';
    }
    else {
      this.endTimeDisplay = this.selectedEndTimeValue;
    }
    this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.searchForm.get("endDate").value, this.searchForm.get('endTime').value, 'end'));
    let startDate1 = this.searchForm.get('startDate').value.getFullYear() + "/" + (this.searchForm.get('startDate').value.getMonth() + 1) + "/" + this.searchForm.get('startDate').value.getDate();
    let endDate1 = this.searchForm.get('endDate').value.getFullYear() + "/" + (this.searchForm.get('endDate').value.getMonth() + 1) + "/" + this.searchForm.get('endDate').value.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.searchForm.get('endTime').value;
      this.endTimeStart = this.searchForm.get('startTime').value; 
    }
    else{
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  onVehicleChange(event: any) { }

  ngOnDestroy() {
    this.setGlobalSearchData();
  }

  setGlobalSearchData() {
    this.globalSearchFilterData["vehicleGroupDropDownValue"] = this.searchForm.get('vehicleGroup').value;
    this.globalSearchFilterData["vehicleDropDownValue"] = this.searchForm.get('vehicleName').value;
    this.globalSearchFilterData["timeRangeSelection"] = this.selectionTab;
    this.globalSearchFilterData["startDateStamp"] = this.searchForm.get('startDate').value;
    this.globalSearchFilterData["endDateStamp"] = this.searchForm.get("endDate").value;
    this.globalSearchFilterData.testDate = this.searchForm.get('startDate').value;
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
    this.globalSearchFilterData["modifiedFrom"] = "VehiclePerformanceReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
  }

  startTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.searchForm.get('startTime').setValue(this.selectedStartTimeValue);
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = this.selectedStartTimeValue + ':00';
    } else {
      this.startTimeDisplay = this.selectedStartTimeValue;
    }
    this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.searchForm.get('startDate').value, this.searchForm.get('startTime').value, 'start'));
    let startDate1 = this.searchForm.get('startDate').value.getFullYear() + "/" + (this.searchForm.get('startDate').value.getMonth() + 1) + "/" + this.searchForm.get('startDate').value.getDate();
    let endDate1 = this.searchForm.get('endDate').value.getFullYear() + "/" + (this.searchForm.get('endDate').value.getMonth() + 1) + "/" + this.searchForm.get('endDate').value.getDate();
    if(startDate1 == endDate1){
    this.maxStartTime = this.searchForm.get('endTime').value;
    this.endTimeStart = this.searchForm.get('startTime').value; 
    }
    else{
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  formStartDate(date: any) {
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat);
  }

  getStartTimeChanged(time: any){
    this.selectedStartTimeValue = time;
  }

  getEndTimeChanged(time: any){
    this.selectedEndTimeValue = time;
  }

  onReset() {
    this.formSubmitted = false;
    this.resetDropdownValues();
    this.selectionTimeRange('today');
    this.searchForm.get('vehicleGroup').setValue(0);
    this.searchForm.get('performanceType').setValue('E');
    this.hideSearchResult.emit();
  }

  onSearch() {
    this.formSubmitted = true;
    if (this.searchForm.valid) {
      let vehName: any = '';
      let vehGrpName: any = '';
      let vin;
      let registrationNo;
      let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.searchForm.get('vehicleGroup').value));
      if (vehGrpCount.length > 0) {
        vehGrpName = vehGrpCount[0].vehicleGroupName;
      }
      let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.searchForm.get('vehicleName').value));
      if (vehCount.length > 0) {
        vehName = vehCount[0].vehicleName;
        vin = vehCount[0].vin;
        registrationNo = vehCount[0].registrationNo;
      }
      let utcStartDateTime = Util.getMillisecondsToUTCDate(this.searchForm.get('startDate').value, this.prefTimeZone);
      let utcEndDateTime = Util.getMillisecondsToUTCDate(this.searchForm.get('endDate').value, this.prefTimeZone);
      let searchData = {
        utcStartDateTime: utcStartDateTime,
        utcEndDateTime: utcEndDateTime,
        startDate: this.formStartDate(this.searchForm.get('startDate').value),
        endDate: this.formStartDate(this.searchForm.get('endDate').value),
        vehicleGroupId: this.searchForm.get('vehicleGroup').value,
        vehicleNameId: this.searchForm.get('vehicleName').value,
        vehicleGroup: vehGrpName,
        vehicleName: vehName,
        performanceType: this.searchForm.get('performanceType').value,
        vin: vin,
        registrationNo: registrationNo,
      }
      this.showSearchResult.emit(searchData);
      this.setGlobalSearchData();
    }
  }

  compare(a, b) {
    if (a.vehicleGroupName < b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
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

  filterVehicleGroups(vehicleSearch) {
    if (!this.vehicleGrpDD) {
      return;
    }
    if (!vehicleSearch) {
      this.resetVehicleGroupFilter();
      return;
    } else {
      vehicleSearch = vehicleSearch.toLowerCase();
    }
    this.filteredVehicleGroups.next(
      this.vehicleGrpDD.filter(item => item.vehicleGroupName.toLowerCase().indexOf(vehicleSearch) > -1)
    );
  }

  filterVehicle(VehicleSearch) {
    if (!this.vehicleDD) {
      return;
    }
    if (!VehicleSearch) {
      this.resetVehicleFilter();
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
    this.filteredVehicle.next(
      this.vehicleDD.filter(item => {
        if(filterby == 'registrationNo') {
          let ofilterby = (item['registrationNo'])? 'registrationNo' :'vehicleName';
          return item[ofilterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        } else {
          return item[filterby]?.toLowerCase()?.indexOf(VehicleSearch) > -1;
        }    
      })
    );
    ////console.log("filtered vehicles", this.filteredVehicle);
  }

  resetVehicleFilter() {
    this.filteredVehicle.next(this.vehicleDD.slice());
  }

  resetVehicleGroupFilter() {
    this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
  }

}