import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { NgxMaterialTimepickerComponent } from 'ngx-material-timepicker';
import { Util } from '../../../shared/util';

@Component({
  selector: 'app-search-criteria',
  templateUrl: './search-criteria.component.html',
  styleUrls: ['./search-criteria.component.less']
})
export class SearchCriteriaComponent implements OnInit, OnDestroy {
  @Input() translationData:any = {};
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  searchForm: FormGroup;
  searchExpandPanel: boolean = true;
  selectionTab: string = 'today';
  startDateValue: any;
  endDateValue: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  prefTimeFormat: any = 12;
  prefTimeZone: any;
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  globalSearchFilterData: any = JSON.parse(localStorage.getItem("globalSearchFilterData"));
  internalSelection: boolean = false;
  vehicleDD: any = [];
  vehicleGrpDD: any = [];
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  todayDate: any;
  last3MonthDate: any;

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {

    this.searchForm = this.formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []]
    });
  }

  selectionTimeRange(selection: any) {
    this.internalSelection = true;
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
    this.searchForm.reset();
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  setDefaultStartEndTime() {
    this.setPrefFormatTime();
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
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

  getTodayDate() {
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
  }

  getYesterdaysDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate() - 1);
    return date;
  }

  getLastWeekDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate() - 7);
    return date;
  }

  getLastMonthDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 1);
    return date;
  }

  getLast3MonthDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 3);
    return date;
  }

  // resetTripFormControlValue() {
  //   if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
  //     if (this._state) {
  //       if (this.vehicleDD.length > 0) {
  //         let _v = this.vehicleDD.filter(i => i.vin == this._state.vehicleData.vin);
  //         if (_v.length > 0) {
  //           let id = _v[0].vehicleId;
  //           this.tripForm.get('vehicle').setValue(id);
  //         }
  //       }
  //     } else {
  //       this.tripForm.get('vehicle').setValue(this.globalSearchFilterData.vehicleDropDownValue);
  //     }
  //     this.tripForm.get('vehicleGroup').setValue(this.globalSearchFilterData.vehicleGroupDropDownValue);
  //   } else {
  //     this.tripForm.get('vehicle').setValue('');
  //     this.tripForm.get('vehicleGroup').setValue(0);
  //   }
  // }

  // filterDateData() {
  //   let distinctVIN: any = [];
  //   let finalVINDataList: any = [];
  //   this.vehicleListData = [];
  //   this.vehicleGrpDD = [];
  //   /* --- comment code as per discus with Atul --- */
  //   // let _last3m = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
  //   // let _yesterday = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
  //   // let currentStartTime = Util.convertDateToUtc(_last3m); //_last3m.getTime();
  //   // let currentEndTime = Util.convertDateToUtc(_yesterday); // _yesterday.getTime();
  //   /* --- comment code as per discus with Atul --- */
  //   let currentStartTime = Util.convertDateToUtc(this.startDateValue);  // extra addded as per discuss with Atul
  //   let currentEndTime = Util.convertDateToUtc(this.endDateValue); // extra addded as per discuss with Atul
  //   //console.log(currentStartTime + "<->" + currentEndTime);
  //   if (this.wholeTripData.vinTripList.length > 0) {
  //     let filterVIN: any = this.wholeTripData.vinTripList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);
  //     if (filterVIN.length > 0) {
  //       distinctVIN = filterVIN.filter((value, index, self) => self.indexOf(value) === index);
  //       ////console.log("distinctVIN:: ", distinctVIN);
  //       if (distinctVIN.length > 0) {
  //         distinctVIN.forEach(element => {
  //           let _item = this.wholeTripData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element);
  //           if (_item.length > 0) {
  //             this.vehicleListData.push(_item[0]); //-- unique VIN data added 
  //             _item.forEach(element => {
  //               finalVINDataList.push(element)
  //             });
  //           }
  //         });
  //       }
  //     } else {
  //       this.tripForm.get('vehicle').setValue('');
  //       this.tripForm.get('vehicleGroup').setValue('');
  //     }
  //   }
  //   this.vehicleGroupListData = finalVINDataList;
  //   if (this.vehicleGroupListData.length > 0) {
  //     let _s = this.vehicleGroupListData.map(item => item.vehicleGroupId).filter((value, index, self) => self.indexOf(value) === index);
  //     if (_s.length > 0) {
  //       _s.forEach(element => {
  //         let count = this.vehicleGroupListData.filter(j => j.vehicleGroupId == element);
  //         if (count.length > 0) {
  //           this.vehicleGrpDD.push(count[0]); //-- unique Veh grp data added
  //         }
  //       });
  //     }
  //     //this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
  //     this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
  //     // this.resetTripFormControlValue();
  //   }
  //   //this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
  //   this.vehicleDD = this.vehicleListData.slice();
  //   if (this.vehicleDD.length > 0) {
  //     this.resetTripFormControlValue();
  //   }
  //   this.setVehicleGroupAndVehiclePreSelection();
  //   if (this.showBack) {
  //     this.onSearch();
  //   }
  // }

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
          this.startTimeDisplay = this.selectedStartTime;
          this.endTimeDisplay = this.selectedEndTime;
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
        this.startTimeDisplay = '12:00 AM';
        this.endTimeDisplay = '11:59 PM';
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
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
      this.searchForm.get('vehicle').setValue('');
    }
    else {
      this.searchForm.get('vehicleGroup').setValue(parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue));
    }
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedEndTime = selectedTime;
    if (this.prefTimeFormat == 24) {
      this.endTimeDisplay = selectedTime + ':59';
    }
    else {
      this.endTimeDisplay = selectedTime;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  onVehicleChange(event: any) {
    this.internalSelection = true;
  }

  ngOnDestroy() {
    this.globalSearchFilterData["vehicleGroupDropDownValue"] = this.searchForm.controls.vehicleGroup.value;
    this.globalSearchFilterData["vehicleDropDownValue"] = this.searchForm.controls.vehicle.value;
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

  startTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedStartTime = selectedTime;
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = selectedTime + ':00';
    }
    else {
      this.startTimeDisplay = selectedTime;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
    // this.resetTripFormControlValue(); // extra addded as per discuss with Atul
    // this.filterDateData(); // extra addded as per discuss with Atul
  }

  onReset() {

  }

  onSearch() {

  }

}
