import { Component, Input, OnInit, OnDestroy, Inject, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { NgxMaterialTimepickerComponent } from 'ngx-material-timepicker';
import { OrganizationService } from 'src/app/services/organization.service';
import { ReportService } from 'src/app/services/report.service';
import { UtilsService } from 'src/app/services/utils.service';
import { Util } from '../../../shared/util';

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

  localStLanguage;
  accountPrefObj;
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
  vehicleGrpDD: any = [];
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  


  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private formBuilder: FormBuilder, private organizationService: OrganizationService, private utilsService: UtilsService, private reportService: ReportService) {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    if(!this.utilsService.isEmpty(localStorage.getItem("globalSearchFilterData"))) {
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
    } else { // org pref
      this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
        this.proceedStep(prefData, orgPref);
        console.log("orgPref", orgPref)
      }, (error) => { // failed org API
        let pref: any = {};
        this.proceedStep(prefData, pref);
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
    if(date){
      let _x = timeObj.split(":")[0];
      let _y = timeObj.split(":")[1];
      if (this.prefTimeFormat == 12) {
        if (_y.split(' ')[1] == 'AM' && _x == 12) {
          date.setHours(0);
        } else {
          date.setHours(_x);
        }
        date.setMinutes(_y.split(' ')[0]);
      } else {
        date.setHours(_x);
        date.setMinutes(_y);
      }

      date.setSeconds(type == 'start' ? '00' : '59');
      return date;
    }
  }

  setPrefFormatDate() {
    switch (this.prefDateFormat) {
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
      default: {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
      }
    }
  }

  getTodayDate() {
    if(this.prefTimeZone) {
      let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
      return _todayDate;
    }
    return null;
  }

  getYesterdaysDate() {
    if(this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setDate(date.getDate() - 1);
      return date;
    }
    return null;
  }

  getLastWeekDate() {
    if(this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setDate(date.getDate() - 7);
      return date;
    }
    return null;
  }

  getLastMonthDate() {
    if(this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setMonth(date.getMonth() - 1);
      return date;
    }
    return null;
  }

  getLast3MonthDate() {
    if(this.prefTimeZone) {
      var date = Util.getUTCDate(this.prefTimeZone);
      date.setMonth(date.getMonth() - 3);
      return date;
    }
    return null;
  }

  proceedStep(prefData: any, preference: any) {
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
      this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeZone = prefData.timezone[0].value;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.resetDropdownValues();
  }

  loadWholeTripData() {
    this.reportService.getVINFromTrip(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
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
    // let currentStartTime = Util.convertDateToUtc(this.searchForm.get('startDate').value);  // extra addded as per discuss with Atul
    // let currentEndTime = Util.convertDateToUtc(this.searchForm.get('endDate').value); // extra addded as per discuss with Atul
    if (this.wholeTripData && this.wholeTripData.vinTripList && this.wholeTripData.vinTripList.length > 0) {
      let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
      if (filterVIN.length > 0) {
        distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
        if (distinctVIN.length > 0) {
          distinctVIN.forEach(element => {
            let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element);
            if (_item.length > 0) {
              this.vehicleListData.push(_item[0]); //-- unique VIN data added 
              _item.forEach(element => {
                finalVINDataList.push(element)
              });
            }
          });
        }
      } else {
        if(this.searchForm) {
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
          }
        });
      }
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
    }
    this.vehicleDD = this.vehicleListData.slice();
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
        this.searchForm.get('startTime').setValue("00:00");
        this.searchForm.get('endTime').setValue("23:59");
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
        this.vehicleDD = this.vehicleListData.slice();
      } else {
        let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
        if (search.length > 0) {
          this.vehicleDD = [];
          search.forEach(element => {
            this.vehicleDD.push(element);
          });
        }
      }
      this.searchForm.get('vehicleName').setValue('');
      this.searchForm.get('vehicleName').enable();
    }
    else {
      this.searchForm.get('vehicleGroup').setValue(parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue));
    }
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    this.searchForm.get('startDate').setValue(this.setStartEndDateTime(event.value._d, this.searchForm.get('startTime').value, 'start'));
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    this.searchForm.get("endDate").setValue(this.setStartEndDateTime(event.value._d, this.searchForm.get('endTime').value, 'end'));
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.searchForm.get('endTime').setValue(selectedTime);
    if (this.prefTimeFormat == 24) {
      this.endTimeDisplay = selectedTime + ':59';
    }
    else {
      this.endTimeDisplay = selectedTime;
    }
    this.searchForm.get("endDate").setValue(this.setStartEndDateTime(this.searchForm.get("endDate").value, this.searchForm.get('endTime').value, 'end'));
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  onVehicleChange(event: any) {
    // this.internalSelection = true;
  }

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
    this.searchForm.get('startTime').setValue(selectedTime);
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = selectedTime + ':00';
    } else {
      this.startTimeDisplay = selectedTime;
    }
    this.searchForm.get('startDate').setValue(this.setStartEndDateTime(this.searchForm.get('startDate').value, this.searchForm.get('startTime').value, 'start'));
    this.resetDropdownValues(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  formStartDate(date: any) {
    let h = (date.getHours() < 10) ? ('0' + date.getHours()) : date.getHours();
    let m = (date.getMinutes() < 10) ? ('0' + date.getMinutes()) : date.getMinutes();
    let s = (date.getSeconds() < 10) ? ('0' + date.getSeconds()) : date.getSeconds();
    let _d = (date.getDate() < 10) ? ('0' + date.getDate()) : date.getDate();
    let _m = ((date.getMonth() + 1) < 10) ? ('0' + (date.getMonth() + 1)) : (date.getMonth() + 1);
    let _y = (date.getFullYear() < 10) ? ('0' + date.getFullYear()) : date.getFullYear();
    let _date: any;
    let _time: any;
    if (this.prefTimeFormat == 12) {
      _time = (date.getHours() > 12 || (date.getHours() == 12 && date.getMinutes() > 0)) ? `${date.getHours() == 12 ? 12 : date.getHours() - 12}:${m} PM` : `${(date.getHours() == 0) ? 12 : h}:${m} AM`;
    } else {
      _time = `${h}:${m}:${s}`;
    }
    switch (this.prefDateFormat) {
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
    if(this.searchForm.valid) {
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
      
      // let utcStartDateTime = Util.convertDateToUtc(this.searchForm.get('startDate').value);
      // let utcEndDateTime = Util.convertDateToUtc(this.searchForm.get('endDate').value);
     
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

}
