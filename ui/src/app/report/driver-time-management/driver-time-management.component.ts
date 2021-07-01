import { Component, ElementRef, Inject, Input, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';
import { ReportService } from '../../services/report.service';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { filter } from 'rxjs/operators';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { Util } from '../../shared/util';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { ReportMapService } from '../report-map.service';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { OrganizationService } from '../../services/organization.service';

@Component({
  selector: 'app-driver-time-management',
  templateUrl: './driver-time-management.component.html',
  styleUrls: ['./driver-time-management.component.less']
})
export class DriverTimeManagementComponent implements OnInit, OnDestroy {
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectionTab: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  driverTimeForm: FormGroup;
  translationData: any;
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  accountId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  driverListData: any = [];
  searchExpandPanel: boolean = true;
  tableExpandPanel: boolean = true;
  noDetailsExpandPanel : boolean = true;
  generalExpandPanel : boolean = true;
  searchFilterpersistData :any = {};
  internalSelection: boolean = false;
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  onSearchData: any = [];
  showLoadingIndicator: boolean = false;
  defaultStartValue: any;
  defaultEndValue: any;
  startDateValue: any;
  endDateValue: any;
  last3MonthDate: any;
  todayDate: any;
  onLoadData: any = [];
  tableInfoObj: any = {};
  tableDetailsInfoObj: any = {};

  tripTraceArray: any = [];
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  displayedColumns = ['detailsdrivername', 'detailsdriverid', 'detailsstarttime', 'detailsendtime', 'detailsdrivetime', 'detailsworktime', 'detailsservicetime', 'detailsresttime', 'detailsavailabletime'];
  detaildisplayedColumns = ['specificdetailstarttime', 'specificdetaildrivetime', 'specificdetailworktime', 'specificdetailservicetime', 'specificdetailresttime', 'specificdetailavailabletime'];
  
  fromDisplayDate: any;
  toDisplayDate : any;
  selectedVehicleGroup : string;
  selectedVehicle : string;
  driverSelected : boolean = false;
  selectedDriverData = [];
  
  totalDriveTime : Number = 0;
  totalWorkTime : Number = 0;
  totalRestTime : Number = 0;
  totalAvailableTime : Number = 0;
  totalServiceTime : Number = 0;
  
  driverDetails : any= [];
  detailConvertedData : any;

  reportPrefData: any = [];
  reportId:number = 9;
  showField: any = {
    detailsdriverid:true,
    detailsdrivername:true,
    detailsendtime:true,
    detailsstarttime:true,
    detailsworktime:true,
    detailsavailabletime:true,
    detailsservicetime:true,
    detailsresttime:true,
    detailsdrivetime:true,
    specificdetailsendtime:true,
    specificdetailstarttime:true,
    specificdetailworktime:true,
    specificdetailavailabletime:true,
    specificdetailservicetime:true,
    specificdetailresttime:true,
    specificdetaildrivetime:true,
    specificdetailchart : true,


  };
  
  finalDriverList : any = [];
  finalVehicleList : any =[];
  prefMapData: any = [
    {
      key: 'da_report_alldriver_general_driverscount',
      value: 'driverscount'
    },
    {
      key: 'da_report_alldriver_general_totaldrivetime',
      value: 'totaldrivetime'
    },
    {
      key: 'da_report_alldriver_general_totalworktime',
      value: 'totalworktime'
    },
    {
      key: 'da_report_alldriver_general_totalavailabletime',
      value: 'totalavailabletime'
    },
    {
      key: 'da_report_alldriver_general_totalresttime',
      value: 'totalresttime'
    },
    {
      key: 'da_report_alldriver_details_driverid',
      value: 'detailsdriverid'
    },
    {
      key: 'da_report_alldriver_details_drivername',
      value: 'detailsdrivername'
    },
    {
      key: 'da_report_alldriver_details_endtime',
      value: 'detailsendtime'
    },
    {
      key: 'da_report_alldriver_details_starttime',
      value: 'detailsstarttime'
    },
    {
      key: 'da_report_alldriver_details_worktime',
      value: 'detailsworktime'
    },
    {
      key: 'da_report_alldriver_details_availabletime',
      value: 'detailsavailabletime'
    },
    {
      key: 'da_report_alldriver_details_servicetime',
      value: 'detailsservicetime'
    },
    {
      key: 'da_report_alldriver_details_resttime',
      value: 'detailsresttime'
    },
    {
      key: 'da_report_alldriver_details_drivetime',
      value: 'detailsdrivetime'
    },
    {
      key: 'da_report_specificdriver_general_driverid',
      value: 'gereraldriverid'
    },
    {
      key: 'da_report_specificdriver_general_drivername',
      value: 'generaldrivername'
    },
    {
      key: 'da_report_specificdriver_general_totaldrivetime',
      value: 'generaltotaldrivetime'
    },
    {
      key: 'da_report_specificdriver_general_totalworktime',
      value: 'generaltotalworktime'
    },
    {
      key: 'da_report_specificdriver_general_totalavailabletime',
      value: 'generaltotalavailabletime'
    },
    {
      key: 'da_report_specificdriver_general_totalresttime',
      value: 'generaltotalresttime'
    },
    {
      key: 'da_report_specificdriver_details_driverid',
      value: 'specificdetailsdriverid'
    },
    {
      key: 'da_report_specificdriver_details_drivername',
      value: 'specificdetailsdrivername'
    },
    {
      key: 'da_report_specificdriver_details_endtime',
      value: 'specificdetailsendtime'
    },
    {
      key: 'da_report_specificdriver_details_starttime',
      value: 'specificdetailstarttime'
    },
    {
      key: 'da_report_specificdriver_details_worktime',
      value: 'specificdetailworktime'
    },
    {
      key: 'da_report_specificdriver_details_availabletime',
      value: 'specificdetailavailabletime'
    },
    {
      key: 'da_report_specificdriver_details_servicetime',
      value: 'specificdetailservicetime'
    },
    {
      key: 'da_report_specificdriver_details_resttime',
      value: 'specificdetailresttime'
    },
    {
      key: 'da_report_specificdriver_details_drivetime',
      value: 'specificdetaildrivetime'
    },
    {
      key: 'da_report_specificdriver_details_charts',
      value: 'specificdetailchart'
    }
  ];
  
  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, 
  private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private organizationService: OrganizationService) { 
    this.defaultTranslation()
  }


  ngOnInit(): void {
    this.searchFilterpersistData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.showLoadingIndicator = true;
    
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.driverTimeForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      driver: ['', [Validators.required]],
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
      menuId: 14 
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
        // this.prefTimeFormat = parseInt(prefData.timeformat.filter(i => i.id == this.accountPrefObj.accountPreference.timeFormatId)[0].value.split(" ")[0]);
        // this.prefTimeZone = prefData.timezone.filter(i => i.id == this.accountPrefObj.accountPreference.timezoneId)[0].value;
        // this.prefDateFormat = prefData.dateformat.filter(i => i.id == this.accountPrefObj.accountPreference.dateFormatTypeId)[0].name;
        // this.prefUnitFormat = prefData.unit.filter(i => i.id == this.accountPrefObj.accountPreference.unitId)[0].name;
        // this.setDefaultStartEndTime();
        // this.setPrefFormatDate();
        // this.setDefaultTodayDate();
        // this.getReportPreferences();
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
    this.getReportPreferences();
  }

  setPrefFormatTime(){
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "" &&  ((this.searchFilterpersistData.startTimeStamp || this.searchFilterpersistData.endTimeStamp) !== "") ) {
      if(this.prefTimeFormat == this.searchFilterpersistData.filterPrefTimeFormat){ // same format
        this.selectedStartTime = this.searchFilterpersistData.startTimeStamp;
        this.selectedEndTime = this.searchFilterpersistData.endTimeStamp;
        this.startTimeDisplay = (this.prefTimeFormat == 24) ? `${this.searchFilterpersistData.startTimeStamp}:00` : this.searchFilterpersistData.startTimeStamp;
        this.endTimeDisplay = (this.prefTimeFormat == 24) ? `${this.searchFilterpersistData.endTimeStamp}:59` : this.searchFilterpersistData.endTimeStamp;  
      }else{ // different format
        if(this.prefTimeFormat == 12){ // 12
          this.selectedStartTime = this._get12Time(this.searchFilterpersistData.startTimeStamp);
          this.selectedEndTime = this._get12Time(this.searchFilterpersistData.endTimeStamp);
          this.startTimeDisplay = this.selectedStartTime; 
          this.endTimeDisplay = this.selectedEndTime;
        }else{ // 24
          this.selectedStartTime = this.get24Time(this.searchFilterpersistData.startTimeStamp);
          this.selectedEndTime = this.get24Time(this.searchFilterpersistData.endTimeStamp);
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
  getReportPreferences(){
    this.reportService.getUserPreferenceReport(this.reportId, this.accountId, this.accountOrganizationId).subscribe((data : any) => {
      this.reportPrefData = data["userPreferences"];
      
      this.setDisplayColumnBaseOnPref();
      
      this.getOnLoadData();
    }, (error) => {
      this.reportPrefData = [];
      this.setDisplayColumnBaseOnPref();
      
      this.getOnLoadData();
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
              let _value = search[0]['value'];

              this.displayedColumns.splice(index, 1);
              this.showField[_value] = false;

          }
          let detailIndex = this.detaildisplayedColumns.indexOf(search[0].value);
          this.detaildisplayedColumns.indexOf(search[0].value);
          if (index > -1) {
              let _detailvalue = search[0]['value'];
              this.detaildisplayedColumns.splice(detailIndex, 1);
              this.showField[_detailvalue] = false;
          }
        }

      //   if(element.key == 'da_report_details_vehiclename'){
      //     this.showField[element.key] = false;
      //   }else if(element.key == 'da_report_details_vin'){
      //     this.showField.vin = false;
      //   }else if(element.key == 'da_report_details_registrationnumber'){
      //     this.showField.regNo = false;
      //   }
      });
    }
  }

  ngOnDestroy(){
    console.log("component destroy...");
    this.searchFilterpersistData["vehicleGroupDropDownValue"] = this.driverTimeForm.controls.vehicleGroup.value;
    this.searchFilterpersistData["vehicleDropDownValue"] = this.driverTimeForm.controls.vehicle.value;
    this.searchFilterpersistData["driverDropDownValue"] = this.driverTimeForm.controls.driver.value;
    this.searchFilterpersistData["timeRangeSelection"] = this.selectionTab;
    this.searchFilterpersistData["startDateStamp"] = this.startDateValue;
    this.searchFilterpersistData["endDateStamp"] = this.endDateValue;
    this.searchFilterpersistData.testDate = this.startDateValue;
    this.searchFilterpersistData.filterPrefTimeFormat = this.prefTimeFormat;
    if(this.prefTimeFormat == 24){
      let _splitStartTime = this.startTimeDisplay.split(':');
      let _splitEndTime = this.endTimeDisplay.split(':');
      this.searchFilterpersistData["startTimeStamp"] = `${_splitStartTime[0]}:${_splitStartTime[1]}`;
      this.searchFilterpersistData["endTimeStamp"] = `${_splitEndTime[0]}:${_splitEndTime[1]}`;
    }else{
      this.searchFilterpersistData["startTimeStamp"] = this.startTimeDisplay;  
      this.searchFilterpersistData["endTimeStamp"] = this.endTimeDisplay;  
    }
    this.setGlobalSearchData(this.searchFilterpersistData);
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

  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    ////console.log("process translationData:: ", this.translationData)
  }

  onVehicleGroupChange(event: any){
    if(event.value){
      this.internalSelection = true; 
    this.driverTimeForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    this.driverTimeForm.get('driver').setValue(''); //- reset vehicle dropdown
    this.driverListData = this.finalDriverList;
    this.vehicleListData = this.finalVehicleList;
    //console.log(this.driverListData)
    //console.log(this.vehicleListData)

    
    if(parseInt(event.value) == 0){ //-- all group
      //.filter(i => i.vehicleGroupId != 0);
      this.driverTimeForm.get('vehicle').setValue(0);
      this.driverTimeForm.get('driver').setValue(0);

    }else{

      this.vehicleListData = this.finalVehicleList.filter(i => i.vehicleGroupId == parseInt(event.value));
      // let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      // if(search.length > 0){
      //   this.vehicleDD = [];
      //   search.forEach(element => {
      //     this.vehicleDD.push(element);  
      //   });
      // }
    }
    // this.searchFilterpersistData["vehicleGroupDropDownValue"] = event.value;
    // this.searchFilterpersistData["vehicleDropDownValue"] = '';
    // this.setGlobalSearchData(this.searchFilterpersistData)
  }else {
    // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event));
    this.driverTimeForm.get('vehicleGroup').setValue(parseInt(this.searchFilterpersistData.vehicleGroupDropDownValue));
    this.driverTimeForm.get('vehicle').setValue(parseInt(this.searchFilterpersistData.vehicleDropDownValue));
  }
  }

  onVehicleChange(event: any){
    if(event.value==0){
      this.driverListData = this.finalDriverList;
    }else{
      let selectedVin = this.vehicleListData.filter(i=>i.vehicleId === parseInt(event.value))[0]['vin'];
      this.driverListData = this.finalDriverList.filter(i => i.vin == selectedVin);
    }
    
    
    this.searchFilterpersistData["vehicleDropDownValue"] = event.value;
    this.setGlobalSearchData(this.searchFilterpersistData)
    this.internalSelection = true; 
    // this.searchFilterpersistData["vehicleDropDownValue"] = event.value;
    // this.setGlobalSearchData(this.searchFilterpersistData)
  }

  onDriverChange(event: any){

  }

  allDriversSelected = true;

  onSearch(){
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _vehicelIds = [];
    let _driverIds =[];
    if (parseInt(this.driverTimeForm.controls.vehicle.value) === 0) {
      _vehicelIds = this.vehicleListData.map(data => data.vin);
      _vehicelIds.shift();

    }
    else {
      _vehicelIds = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.driverTimeForm.controls.vehicle.value)).map(data => data.vin);
      if(_vehicelIds.length > 0){
        _vehicelIds = _vehicelIds.filter((value, index, self) => self.indexOf(value) === index);
      }
       
    }

    if (parseInt(this.driverTimeForm.controls.driver.value) === 0) {
      this.allDriversSelected = true;
      _driverIds = this.driverListData.map(data=>data.driverID);
      _driverIds.shift();
    }
    else {
      this.allDriversSelected = false
      _driverIds = this.driverListData.filter(item => item.driverID == (this.driverTimeForm.controls.driver.value)).map(data=>data.driverID);
    }
    
   
 
   // let _driverData = this.driverListData.map(data=>data.driverID);
    let searchDataParam = {
      "startDateTime":_startTime,
      "endDateTime":_endTime,
      "viNs": _vehicelIds,
      "driverIds":_driverIds
    }
    if(_vehicelIds.length > 0){
      this.showLoadingIndicator = true;
      this.reportService.getDriverTimeDetails(searchDataParam).subscribe((_tripData: any) => {
        this.hideloader();
        let tripData = _tripData;
        // let tripData = {
        //   "driverActivities": [
        //     {
        //       "driverId": "NL B000384974000000",
        //       "driverName": "Helloupdated Helloupdated",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 3,
        //       "restTime": 0,
        //       "availableTime": 0,
        //       "workTime": 0,
        //       "driveTime": 1218000,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "D2",
        //       "driverName": "Ayrton Senna",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 1,
        //       "restTime": 0,
        //       "availableTime": 1218000,
        //       "workTime": 0,
        //       "driveTime": 0,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "UK DB08176162022802",
        //       "driverName": "Helloupdated Helloupdated",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 3,
        //       "restTime": 0,
        //       "availableTime": 0,
        //       "workTime": 0,
        //       "driveTime": 1218000,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "D2",
        //       "driverName": "Ayrton Senna",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 1,
        //       "restTime": 0,
        //       "availableTime": 1218000,
        //       "workTime": 0,
        //       "driveTime": 0,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "UK DB08176162022802",
        //       "driverName": "Helloupdated Helloupdated",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 3,
        //       "restTime": 0,
        //       "availableTime": 0,
        //       "workTime": 0,
        //       "driveTime": 1218000,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "D2",
        //       "driverName": "Ayrton Senna",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 1,
        //       "restTime": 0,
        //       "availableTime": 1218000,
        //       "workTime": 0,
        //       "driveTime": 0,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "UK DB08176162022802",
        //       "driverName": "Helloupdated Helloupdated",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 3,
        //       "restTime": 0,
        //       "availableTime": 0,
        //       "workTime": 0,
        //       "driveTime": 1218000,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "D2",
        //       "driverName": "Ayrton Senna",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 1,
        //       "restTime": 0,
        //       "availableTime": 1218000,
        //       "workTime": 0,
        //       "driveTime": 0,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "UK DB08176162022802",
        //       "driverName": "Helloupdated Helloupdated",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 3,
        //       "restTime": 0,
        //       "availableTime": 0,
        //       "workTime": 0,
        //       "driveTime": 1218000,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "D2",
        //       "driverName": "Ayrton Senna",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 1,
        //       "restTime": 0,
        //       "availableTime": 1218000,
        //       "workTime": 0,
        //       "driveTime": 0,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "UK DB08176162022802",
        //       "driverName": "Helloupdated Helloupdated",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 3,
        //       "restTime": 0,
        //       "availableTime": 0,
        //       "workTime": 0,
        //       "driveTime": 1218000,
        //       "serviceTime": 1218000
        //     },
        //     {
        //       "driverId": "D2",
        //       "driverName": "Ayrton Senna",
        //       "vin": "RERAE75PC0E261011",
        //       "activityDate": 1604338846000,
        //       "startTime": 0,
        //       "endTime": 0,
        //       "code": 1,
        //       "restTime": 0,
        //       "availableTime": 1218000,
        //       "workTime": 0,
        //       "driveTime": 0,
        //       "serviceTime": 1218000
        //     }
        //   ],
        //   "code": 200,
        //   "message": "Trip fetched successfully for requested Filters"
        // }

        if(this.allDriversSelected){
          this.onSearchData = tripData;
          this.setGeneralDriverValue();

          this.initData = this.reportMapService.getDriverTimeDataBasedOnPref(tripData.driverActivities, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
          this.setTableInfo();
          this.updateDataSource(this.initData);
          this.setDataForAll();
        }
        else{
          // this.driverDetails =   [
          //   {
          //     "driverId": "UK DB08176162022802",
          //     "driverName": "Helloupdated Helloupdated",
          //     "vin": "RERAE75PC0E261011",
          //     "activityDate": 1604338846000,
          //     "startTime": 1604338846000,
          //     "endTime": 1604337628000,
          //     "code": 3,
          //     "restTime": 0,
          //     "availableTime": 0,
          //     "workTime": 0,
          //     "driveTime": 1218000,
          //     "serviceTime": 1218000
          //   },
          //   {
          //     "driverId": "UK DB08176162022802",
          //     "driverName": "Helloupdated Helloupdated",
          //     "vin": "RERAE75PC0E261011",
          //     "activityDate": 1624373306931,
          //     "startTime": 1604338846000,
          //     "endTime": 1604337628000,
          //     "code": 3,
          //     "restTime": 0,
          //     "availableTime": 0,
          //     "workTime": 0,
          //     "driveTime": 1218000,
          //     "serviceTime": 1218000
          //   },
            
          // ]
          //let updateData = tripData.driverActivities;
          this.driverDetails = tripData.driverActivities;
          this.detailConvertedData = this.reportMapService.getDriverDetailsTimeDataBasedOnPref(this.driverDetails, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
          this.setGeneralDriverDetailValue();
          this.setSingleDriverData();
        }
       

      }, (error)=>{
        //console.log(error);
        this.hideloader();
        this.onSearchData = [];
        this.tableInfoObj = {};
       // this.updateDataSource(this.tripData);
      });
    }
  }

  setDataForAll(){
    
  }

  setSingleDriverData(){

  }

  onReset(){
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.onSearchData = [];
    this.vehicleGroupListData = this.vehicleGroupListData;
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    //this.updateDataSource(this.tripData);
    this.resetdriverTimeFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
    this.tableInfoObj = {};
    this.allDriversSelected = true;
    this.initData=[];
    this.updateDataSource(this.initData);
    //this.advanceFilterOpen = false;
   // this.selectedPOI.clear();
  }

  resetdriverTimeFormControlValue(){
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== ""){
      this.driverTimeForm.get('vehicle').setValue(this.searchFilterpersistData.vehicleDropDownValue);
      this.driverTimeForm.get('vehicleGroup').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
      this.driverTimeForm.get('driver').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
    }else{
      this.driverTimeForm.get('vehicleGroup').setValue(0);
      this.driverTimeForm.get('vehicle').setValue('');
      this.driverTimeForm.get('driver').setValue('');
    }
    this.driverTimeForm.get('vehicle').setValue(0);


    // this.searchFilterpersistData["vehicleGroupDropDownValue"] = 0;
    // this.searchFilterpersistData["vehicleDropDownValue"] = '';
    // this.searchFilterpersistData["driverDropDownValue"] = '';
    // this.setGlobalSearchData(this.searchFilterpersistData);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  getOnLoadData(){
    
    let defaultStartValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let defaultEndValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    // this.startDateValue = defaultStartValue;
    // this.endDateValue = defaultEndValue;
    let loadParam = {
      "reportId": 10,
      "accountId": this.accountId,
      "organizationId": this.accountOrganizationId,
      "startDateTime": Util.convertDateToUtc(defaultStartValue),
      "endDateTime": Util.convertDateToUtc(defaultEndValue)
    }
    this.showLoadingIndicator = true;
    this.reportService.getDefaultDriverParameter(loadParam).subscribe((initData: any) => {
      this.hideloader();
      this.onLoadData = initData;
      this.filterDateData();
     
      
    }, (error)=>{
      this.hideloader();
      //this.wholeTripData.vinTripList = [];
     // this.wholeTripData.vehicleDetailsWithAccountVisibiltyList = [];
      //this.loadUserPOI();
    });

  }
  setGlobalSearchData(globalSearchFilterData:any) {
    this.searchFilterpersistData["modifiedFrom"] = "TripReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    let distinctGroupId : any = [];
    let distinctDriverId : any = [];
    let finalDriverList : any = [];
    //let _last3m = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    //let _yesterday = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    let currentStartTime = Util.convertDateToUtc(this.startDateValue); //_last3m.getTime();
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); // _yesterday.getTime();
    //console.log(currentStartTime + "<->" + currentEndTime);
    // if(this.onLoadData.vehicleDetailsWithAccountVisibiltyList > 0){
    //   let filterVIN: any = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(item => (item.startTimeStamp >= currentStartTime) && (item.endTimeStamp <= currentEndTime)).map(data => data.vin);

    // }
//     this.onLoadData = 
//     {
//       "driverList": [{
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "PL 1821230147770000",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040129000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "PL 1821230147770000",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040323000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "PL 1821230147770000",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040403000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "PL 1821230147770000",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040424000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "PL 1821230147770000",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040554000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "PL 1821230147770000",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040555000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "P  0000000542878012",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040682000
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "NL P110000123475456",
//       "firstName": "Nitish",
//       "lastName": "Wadiya",
//       "activityDateTime": 1623888060000
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "NL P110000123475456",
//       "firstName": "Nitish",
//       "lastName": "Wadiya",
//       "activityDateTime": 1623974340000
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "NL P110000123465123",
//       "firstName": "Ved",
//       "lastName": "Kumar",
//       "activityDateTime": 1623888060000
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "NL P110000123465123",
//       "firstName": "Ved",
//       "lastName": "Kumar",
//       "activityDateTime": 1623974340000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "NL B000362316000001",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039782000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "NL B000362316000001",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039884000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "NL B000362316000001",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039902000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039955000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039978000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039996000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040155000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040157000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040584000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040596000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "NL B000171984000002",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040600000
//   }, {
//       "vin": "XLR0998HGFFT75550",
//       "driverID": "B  B116526558846001",
//       "firstName": "John",
//       "lastName": "Joshep",
//       "activityDateTime": 1624321000000
//   }, {
//       "vin": "XLR0998HGFFT74601",
//       "driverID": "B  B116366983456001",
//       "firstName": "John",
//       "lastName": "Peterr",
//       "activityDateTime": 1624321000000
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "B  B116366983456001",
//       "firstName": "John",
//       "lastName": "Peterr",
//       "activityDateTime": 1623833073000
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "B  B116366983456001",
//       "firstName": "John",
//       "lastName": "Peterr",
//       "activityDateTime": 1623862833000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160617000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160622000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160693000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160706000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160850000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160857000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160894000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160900000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160962000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160974000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160979000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160985000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161083000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161101000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161195000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161200000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161214000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161219000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161432000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161461000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161494000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161507000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161511000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161522000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161550000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622162611000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622163203000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622164809000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622165055000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166273000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166331000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166419000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166516000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166525000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166535000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166541000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166558000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166572000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166580000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166581000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166856000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167767000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167772000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167828000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168005000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168077000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168085000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168087000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168088000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168303000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168343000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168458000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160617000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160622000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160693000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160706000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160850000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160857000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160894000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160900000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160962000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160974000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160979000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160985000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161083000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161101000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161195000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161200000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161214000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161219000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161432000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161461000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161494000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161507000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161511000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161522000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161550000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622162611000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622163203000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622164809000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622165055000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166273000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166331000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166419000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166516000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166525000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166535000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166541000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166558000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166572000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166580000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166581000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166856000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167767000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167772000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167828000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168005000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168077000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168085000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168087000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168088000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168303000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168343000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168458000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168495000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168510000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168516000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168523000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168817000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168868000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171725000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171728000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171962000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171982000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172042000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172049000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172144000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172173000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172181000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172196000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172199000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172205000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172222000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172229000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172240000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172246000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172353000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172360000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172372000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172380000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172394000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172407000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172410000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172416000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172417000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172440000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172441000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172508000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172510000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172533000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172536000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172542000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172551000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172571000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172584000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172589000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172708000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172741000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172939000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172963000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173065000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173092000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173171000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173185000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173228000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173234000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173235000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173264000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173418000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173427000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173452000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173460000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622175853000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622175894000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622176218000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622176224000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622176239000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177174000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177185000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177299000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177303000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177482000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177496000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177585000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177589000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177609000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622177612000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622178114000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622178653000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622179085000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622185693000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622185726000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622185855000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622185903000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622186732000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622186812000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622186826000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622186839000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622186946000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187132000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187155000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187159000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187174000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187423000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187441000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187569000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187570000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187657000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187667000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187923000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187950000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187978000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622187986000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189319000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189325000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189593000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189601000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189749000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189765000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189907000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189914000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189943000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189947000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622189999000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190009000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190015000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190020000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190021000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190028000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190128000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190142000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190145000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190151000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190161000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190166000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190183000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190198000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190201000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190213000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190487000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190513000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190518000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190522000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190671000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190706000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190835000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190917000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190927000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190930000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190933000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622190945000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192642000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192661000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192692000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192703000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192779000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192786000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192826000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192839000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192905000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192915000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192920000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622192928000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193043000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193064000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193095000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193121000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193159000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193171000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193173000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193182000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193230000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622193240000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194129000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194179000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194236000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194242000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194246000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194251000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194261000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194293000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194451000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194464000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194465000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194477000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194680000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194704000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194798000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194802000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194887000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622194915000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195794000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195803000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195851000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195869000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195933000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195938000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195942000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195961000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195973000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622195990000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199275000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199276000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199280000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199350000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199419000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199434000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199450000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622199468000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200611000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200619000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200664000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200677000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200789000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200801000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200833000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200839000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200840000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200845000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200846000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200889000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200890000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200945000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200956000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200961000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622200977000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201081000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201122000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201128000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201136000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201186000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201191000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201201000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622201213000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202217000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202248000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202367000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202380000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202381000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202397000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202400000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202405000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202414000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622202441000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203035000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203062000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203186000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203188000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203189000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203273000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203284000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203534000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203599000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203628000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622203636000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622204379000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622204386000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206094000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206108000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206109000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206132000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206152000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206202000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206203000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206483000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206493000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206535000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206541000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206577000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206588000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206607000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206612000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206617000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206628000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206740000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206753000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206756000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206762000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206775000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206780000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206794000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206801000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206802000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622206819000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622207669000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622207672000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622207711000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622207714000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622210854000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622210857000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622210874000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622210893000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622210992000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622211011000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622211054000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622211073000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622211146000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622211154000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213807000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213854000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213865000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213874000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213879000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213952000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213978000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213981000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622213989000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214076000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214084000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214102000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214108000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214120000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214126000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214129000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214134000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214161000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214173000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214231000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214244000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214245000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214254000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214395000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214410000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214411000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214416000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214583000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214589000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214867000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622214882000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622215034000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622215042000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622215060000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622215073000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622216926000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622216957000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622216976000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220165000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220174000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220307000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220314000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220330000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220531000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220852000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220866000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220878000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220937000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220944000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220953000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220958000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220965000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220969000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622220970000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221530000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221543000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221676000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221680000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221685000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221802000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221804000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221816000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622221817000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222049000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222355000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222470000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222489000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222504000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222507000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222571000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222593000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222728000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622222763000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622223727000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622223739000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622229728000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622229756000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622229822000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622229836000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622230540000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622230584000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234042000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234050000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234125000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234133000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234230000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234252000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234276000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234282000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234291000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234296000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234364000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234371000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234434000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234445000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234446000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234449000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234592000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234650000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234675000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234682000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234710000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622234715000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236342000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236436000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236438000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236545000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236597000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236600000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236676000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236720000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236747000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236753000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236765000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236777000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236778000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236783000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236852000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236864000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236901000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236908000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236990000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236995000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622236996000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237014000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237086000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237115000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237119000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237331000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237341000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237368000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237373000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237389000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237402000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237403000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237438000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237439000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237731000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237804000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237844000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622237882000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238183000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238206000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238290000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238297000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238384000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238393000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238741000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622238776000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622245622000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622245631000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622245981000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622245984000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622245985000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622246059000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622246071000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622246253000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622246259000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622246288000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622246370000
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899115477
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899116085
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899432930
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899755490
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899810475
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899885912
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616900121603
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616900282533
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616901533945
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616901557529
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616901790251
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616901790896
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616902123173
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616902124237
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903475961
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903476547
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903477188
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903477946
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903478693
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903565712
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903590938
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903616098
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903776482
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903777444
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903778506
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903888482
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616903966641
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616904092191
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616907992933
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616907994697
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616907996198
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616911526837
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616911548974
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913414278
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913506071
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913792612
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913793445
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616915352394
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616915352895
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616915353487
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917666469
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917667028
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917668207
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917670515
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917671023
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917828958
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917870576
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917871107
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917888931
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917889038
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917935567
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917936653
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616917996000
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918014376
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918015112
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918056478
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918057174
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918342394
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918343120
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918808947
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918817673
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616919441184
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616919441829
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920118241
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920166897
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920170730
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920452644
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920453294
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920888556
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920928912
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921510876
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921519805
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921922392
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921922770
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616922388704
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616922397537
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616922829965
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616922904405
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923231817
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923232412
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923307380
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923308527
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923483163
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923674473
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923744565
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923784941
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923785161
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923830265
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923900573
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923914501
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616923986208
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924061513
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924137093
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924408627
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924519352
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924589714
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924665259
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924665819
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924740560
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924816506
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616925103770
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616925409923
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616925539523
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616925729842
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926036755
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926137610
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926329321
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926717027
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926888276
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926953508
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927414683
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927746629
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928007835
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928108320
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928178772
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928218536
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928649932
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928975850
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616929283800
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616929592872
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616929898740
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616930182955
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616930491250
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616930800123
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931052895
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931360276
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931666040
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931971094
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932252744
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932558620
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932864249
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616933170629
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616933484725
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616933792813
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616934098420
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616934349085
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616934655714
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616934961984
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616935257129
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616935565495
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616935875232
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616936181533
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616936465353
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616936774352
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616937020741
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616939595761
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616939891101
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616940205308
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616940514111
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616940819064
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616941099974
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616941408682
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616941717784
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616942023225
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616942303886
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616942610258
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616942915209
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616943222552
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616943528218
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616943833877
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616944140199
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616944390433
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616944696575
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945002496
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945308067
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945589706
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945895833
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616946201671
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616946507967
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616946788979
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616947095335
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616947401016
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616947706690
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948013318
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948321310
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948631015
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948937940
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616949244087
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616949501325
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616949699337
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616952390562
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616952391146
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616952681230
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616952987823
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616953293827
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616953600393
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616953883374
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954193269
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954498805
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954805977
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616955112423
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616955419425
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616955729845
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956035585
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956341504
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956593165
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956898558
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616957204197
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616957525981
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616957831635
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958137967
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958443757
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958700063
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959006818
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959312613
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959593499
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959899952
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616960205493
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616960361538
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962160412
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962431829
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616963012813
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616963112873
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616964974698
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965080995
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965512965
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965648373
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616966757846
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616966758242
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616968166159
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616968348379
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616968660016
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616968699704
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616969409398
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616969670122
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616969770574
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616971118508
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616971379347
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616971711570
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616971842193
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973211553
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973481908
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973673007
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973863453
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616974059474
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616974396374
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616974467164
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616974813411
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975149991
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975220006
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975290442
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975556827
//   }, {
//       "vin": "XLR0998HGFFT74602",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975777012
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616889636205
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616889807139
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616889947548
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616890498803
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616890580192
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616890881036
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616890881478
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616891755866
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616892011694
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616892317744
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616892625180
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616892929951
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616893236231
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616893542283
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616893848786
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616894154180
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616894435987
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616894742135
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616895049290
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616895050168
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616897313246
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616897393917
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616897709718
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616898016447
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616898272345
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616898583587
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616898893748
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899035572
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899230653
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899276190
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899351207
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616899492188
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616900244849
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616900319915
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616904869490
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616904870850
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616905149535
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616905456849
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616905767505
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616906078661
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616906385349
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616906605287
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616906989538
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616907210543
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616907516262
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616907826401
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616908112287
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616908304773
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616908504049
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616908641547
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616908786236
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913331387
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913381734
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913578845
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616913817859
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616914127305
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616914397183
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616914978297
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616915181897
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616915377939
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616915378754
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616916011533
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616916012189
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918562187
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616918562844
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616919142537
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616919142947
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616919433813
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616919739554
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920045734
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920352293
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920659116
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616920964321
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921246453
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921467076
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921657343
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921657599
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921733232
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616921873829
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616922009532
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616922050047
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924297821
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924304224
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924559710
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924600707
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924866667
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616924867349
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616925423073
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616925694719
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926000756
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926311771
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926617599
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616926899062
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927185109
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927490710
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927822535
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927833061
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927933314
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616927933711
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928008601
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616928255314
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616929672578
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616929686538
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616929898441
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616930163958
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616930451405
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616930761728
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616930867001
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931057813
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931395428
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931435637
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931505946
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616931616602
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932630888
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932631332
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932631784
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932632305
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616932739061
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616933049727
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616933355942
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616933661938
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616933971446
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616934189098
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616934495131
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616934801130
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616935107086
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616935413110
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616935720320
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616936026180
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616936306270
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616936306508
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616936857760
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616937149190
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616937455559
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616937761654
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616938068693
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616938379705
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616938380058
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616938600622
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616938705503
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616938895599
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616939141334
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616939211822
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616939257932
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616941068159
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616941389608
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616941705978
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616941956725
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616942262426
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616942568693
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616942819564
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616943251306
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616943626986
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616943933176
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616944148130
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616944455631
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616944765526
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616944866442
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945056845
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945358902
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945497097
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945665320
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616945705063
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948897750
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948898214
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948898602
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616948898951
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616949103928
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616949174605
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616949185209
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954052728
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954053175
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954053564
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954054001
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954054597
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954360630
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954669060
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616954977821
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616955284735
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616955590626
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616955882437
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956189395
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956344391
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956656259
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956882197
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956952966
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616956962972
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616957129125
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616957300045
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616957610401
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616957751120
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958182592
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958183081
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958353067
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958353401
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616958784171
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959095571
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959311130
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959617150
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616959925114
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616960234384
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616960540269
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616960766488
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616961072186
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616961344646
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616961681604
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616961948026
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616961983006
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962238724
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962349809
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962540570
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962551005
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962651795
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616962696451
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616963193884
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616963274390
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616963414303
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616963724956
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616964031099
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616964247164
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616964553869
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616964869906
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965176314
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965176569
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965246042
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965246315
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965256910
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965326597
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965408701
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965417489
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965518094
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965723903
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616965979876
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616966085626
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616972303667
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616972377761
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616972686401
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616972731281
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973353382
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973354108
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973548824
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616973871330
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616974182428
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616974488634
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616974801127
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975020651
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975331863
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975640661
//   }, {
//       "vin": "XLR0998HGFFT74604",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1616975948782
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160617000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160622000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160693000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160706000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160850000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160857000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160894000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160900000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160962000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160974000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160979000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622160985000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161083000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161101000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161195000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161200000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161214000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161219000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161432000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161461000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161494000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161507000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161511000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161522000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622161550000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622162611000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622163203000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622164809000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622165055000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166273000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166331000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166419000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166516000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166525000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166535000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166541000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166558000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166572000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166580000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166581000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622166856000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167767000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167772000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622167828000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168005000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168077000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168085000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168087000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168088000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168303000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168343000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168458000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168495000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168510000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168516000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168523000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168817000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622168868000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171725000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171728000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171962000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622171982000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172042000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172049000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172144000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172173000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172181000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172196000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172199000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172205000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172222000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172229000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172240000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172246000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172353000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172360000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172372000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172380000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172394000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172407000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172410000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172416000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172417000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172440000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172441000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172508000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172510000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172533000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172536000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172542000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172551000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172571000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172584000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172589000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172708000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172741000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172939000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622172963000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173065000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "*",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1622173092000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039943000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039955000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039978000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039996000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040155000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040157000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040570000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040584000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040596000
//   }, {
//       "vin": "XLR0998HGFFT74597",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040600000
//   }, {
//       "vin": "XLR0998HGFFT74599",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040682000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039782000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039884000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039902000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620039931000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040067000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040094000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040129000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040323000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040403000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040424000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040554000
//   }, {
//       "vin": "XLR0998HGFFT76638",
//       "driverID": "",
//       "firstName": null,
//       "lastName": null,
//       "activityDateTime": 1620040555000
//   }
// ],
// "vehicleDetailsWithAccountVisibiltyList": [{
//       "vehicleGroupId": 0,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "S",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "",
//       "vehicleId": 12,
//       "vehicleName": "Vehicle 222 - Updated at 13:17 IST",
//       "vin": "XLR0998HGFFT74597",
//       "registrationNo": "PLOI045OII"
//   }, {
//       "vehicleGroupId": 0,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "S",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "",
//       "vehicleId": 35,
//       "vehicleName": "SK vehicles 2",
//       "vin": "XLR0998HGFFT74599",
//       "registrationNo": "BTXR98"
//   }, {
//       "vehicleGroupId": 0,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "S",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "",
//       "vehicleId": 67,
//       "vehicleName": "Test Veh1",
//       "vin": "XLR0998HGFFT74601",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 0,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "S",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "",
//       "vehicleId": 69,
//       "vehicleName": "Test Veh2",
//       "vin": "XLR0998HGFFT75550",
//       "registrationNo": "BTXR422"
//   }, {
//       "vehicleGroupId": 6,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "RTC Fleet Group",
//       "vehicleId": 11,
//       "vehicleName": "Vehicle 111",
//       "vin": "XLR0998HGFFT76638",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 6,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "RTC Fleet Group",
//       "vehicleId": 12,
//       "vehicleName": "Vehicle 222 - Updated at 13:17 IST",
//       "vin": "XLR0998HGFFT74597",
//       "registrationNo": "PLOI045OII"
//   }, {
//       "vehicleGroupId": 6,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "RTC Fleet Group",
//       "vehicleId": 34,
//       "vehicleName": "Vehicle new validation 60 char. Vehicle new valida",
//       "vin": "XLR0998HGFFT74598",
//       "registrationNo": "BTXR421"
//   }, {
//       "vehicleGroupId": 6,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "RTC Fleet Group",
//       "vehicleId": 35,
//       "vehicleName": "SK vehicles 2",
//       "vin": "XLR0998HGFFT74599",
//       "registrationNo": "BTXR98"
//   }, {
//       "vehicleGroupId": 6,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "RTC Fleet Group",
//       "vehicleId": 48,
//       "vehicleName": "SK VIN1",
//       "vin": "XLR0998HGFFT74592",
//       "registrationNo": "BTXR99"
//   }, {
//       "vehicleGroupId": 6,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "RTC Fleet Group",
//       "vehicleId": 67,
//       "vehicleName": "Test Veh1",
//       "vin": "XLR0998HGFFT74601",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 6,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "RTC Fleet Group",
//       "vehicleId": 69,
//       "vehicleName": "Test Veh2",
//       "vin": "XLR0998HGFFT75550",
//       "registrationNo": "BTXR422"
//   }, {
//       "vehicleGroupId": 26,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "Vehicle123",
//       "vehicleId": 11,
//       "vehicleName": "Vehicle 111",
//       "vin": "XLR0998HGFFT76638",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 26,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "Vehicle123",
//       "vehicleId": 12,
//       "vehicleName": "Vehicle 222 - Updated at 13:17 IST",
//       "vin": "XLR0998HGFFT74597",
//       "registrationNo": "PLOI045OII"
//   }, {
//       "vehicleGroupId": 26,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "Vehicle123",
//       "vehicleId": 34,
//       "vehicleName": "Vehicle new validation 60 char. Vehicle new valida",
//       "vin": "XLR0998HGFFT74598",
//       "registrationNo": "BTXR421"
//   }, {
//       "vehicleGroupId": 144,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "SK Vehicle Group 1",
//       "vehicleId": 12,
//       "vehicleName": "Vehicle 222 - Updated at 13:17 IST",
//       "vin": "XLR0998HGFFT74597",
//       "registrationNo": "PLOI045OII"
//   }, {
//       "vehicleGroupId": 144,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "SK Vehicle Group 1",
//       "vehicleId": 48,
//       "vehicleName": "SK VIN1",
//       "vin": "XLR0998HGFFT74592",
//       "registrationNo": "BTXR99"
//   }, {
//       "vehicleGroupId": 144,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "SK Vehicle Group 1",
//       "vehicleId": 71,
//       "vehicleName": "PH Vehicle 1",
//       "vin": "XLR0998HGFFT74603",
//       "registrationNo": "BTXR51"
//   }, {
//       "vehicleGroupId": 144,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "SK Vehicle Group 1",
//       "vehicleId": 72,
//       "vehicleName": "PH Vehicle 2",
//       "vin": "XLR0998HGFFT74604",
//       "registrationNo": "BTXR52"
//   }, {
//       "vehicleGroupId": 179,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "SK vehicle group",
//       "vehicleId": 68,
//       "vehicleName": "SK vehicle 2",
//       "vin": "XLR0998HGFFT74602",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 183,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "V",
//       "vehicleGroupName": "VG 1122",
//       "vehicleId": 11,
//       "vehicleName": "Vehicle 111",
//       "vin": "XLR0998HGFFT76638",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 183,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "V",
//       "vehicleGroupName": "VG 1122",
//       "vehicleId": 12,
//       "vehicleName": "Vehicle 222 - Updated at 13:17 IST",
//       "vin": "XLR0998HGFFT74597",
//       "registrationNo": "PLOI045OII"
//   }, {
//       "vehicleGroupId": 183,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "V",
//       "vehicleGroupName": "VG 1122",
//       "vehicleId": 34,
//       "vehicleName": "Vehicle new validation 60 char. Vehicle new valida",
//       "vin": "XLR0998HGFFT74598",
//       "registrationNo": "BTXR421"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 11,
//       "vehicleName": "Vehicle 111",
//       "vin": "XLR0998HGFFT76638",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 12,
//       "vehicleName": "Vehicle 222 - Updated at 13:17 IST",
//       "vin": "XLR0998HGFFT74597",
//       "registrationNo": "PLOI045OII"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 34,
//       "vehicleName": "Vehicle new validation 60 char. Vehicle new valida",
//       "vin": "XLR0998HGFFT74598",
//       "registrationNo": "BTXR421"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 35,
//       "vehicleName": "SK vehicles 2",
//       "vin": "XLR0998HGFFT74599",
//       "registrationNo": "BTXR98"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 48,
//       "vehicleName": "SK VIN1",
//       "vin": "XLR0998HGFFT74592",
//       "registrationNo": "BTXR99"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 64,
//       "vehicleName": "test vehicle 1",
//       "vin": "XLR0998HGFFT74333",
//       "registrationNo": "BTXR33"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 66,
//       "vehicleName": "Test Veh3",
//       "vin": "XLR0998HGFFT74600",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 67,
//       "vehicleName": "Test Veh1",
//       "vin": "XLR0998HGFFT74601",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 68,
//       "vehicleName": "SK vehicle 2",
//       "vin": "XLR0998HGFFT74602",
//       "registrationNo": "BTXR45"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 69,
//       "vehicleName": "Test Veh2",
//       "vin": "XLR0998HGFFT75550",
//       "registrationNo": "BTXR422"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 71,
//       "vehicleName": "PH Vehicle 1",
//       "vin": "XLR0998HGFFT74603",
//       "registrationNo": "BTXR51"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 72,
//       "vehicleName": "PH Vehicle 2",
//       "vin": "XLR0998HGFFT74604",
//       "registrationNo": "BTXR52"
//   }, {
//       "vehicleGroupId": 184,
//       "accountId": 15,
//       "objectType": "V",
//       "groupType": "G",
//       "functionEnum": "",
//       "organizationId": 10,
//       "accessType": "F",
//       "vehicleGroupName": "PH Vehicle group 040621",
//       "vehicleId": 73,
//       "vehicleName": "PH Vehicle 3",
//       "vin": "XLR0998HGFFT74605",
//       "registrationNo": "BTXR53"
//   }
// ]
// };
let groupIdArray = [];
let finalGroupDataList = [];
    if(this.onLoadData.vehicleDetailsWithAccountVisibiltyList.length > 0){
      groupIdArray  = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.map(data=>data.vehicleGroupId);
      //this.vehicleGroupListData = distinctGroupId;
    if(groupIdArray.length > 0){
      distinctGroupId = groupIdArray.filter((value, index, self) => self.indexOf(value) === index);

      if(distinctGroupId.length > 0){
        distinctGroupId.forEach(element => {
          let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vehicleGroupId === element); 
          if(_item.length > 0){
            finalGroupDataList.push(_item[0])
          }
        });
        this.vehicleGroupListData = finalGroupDataList;

        ////console.log("finalVINDataList:: ", finalVINDataList); 
      }
      this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.finalVehicleList = [];
      this.finalVehicleList = this.onLoadData.vehicleDetailsWithAccountVisibiltyList;
      this.vehicleListData =[];
      let vinList = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.map(data=>data.vin);
      if(vinList.length>0){
        let distinctVIN = vinList.filter((value, index, self) => self.indexOf(value) === index);
        if(distinctVIN.length > 0){
          distinctVIN.forEach(element => {
            let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element); 
            if(_item.length > 0){
              finalVINDataList.push(_item[0])
            }
          });
      }
      }
      this.vehicleListData = finalVINDataList;
      if(this.vehicleListData.length>0 && this.vehicleListData[0].vehicleId != 0)
      this.vehicleListData.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
    }
    }
    if(this.onLoadData.driverList.length > 0){
      let driverIds: any = this.onLoadData.driverList.map(data=> data.vin);
      if(driverIds.length > 0){
          distinctDriverId = driverIds.filter((value, index, self) => self.indexOf(value) === index);
          
      if(distinctDriverId.length > 0){
        distinctDriverId.forEach(element => {
          let _item = this.onLoadData.driverList.filter(i => i.vin === element); 
          if(_item.length > 0){
            finalDriverList.push(_item[0])
          }
        });
        this.driverListData = finalDriverList.filter(i => (i.activityDateTime >= currentStartTime) && (i.activityDateTime <= currentEndTime));
      
        this.driverListData.unshift({ driverID: 0, firstName: this.translationData.lblAll || 'All' });
        this.finalDriverList = this.driverListData;

        ////console.log("finalVINDataList:: ", finalVINDataList); 
      }
      }

    }
    this.resetdriverTimeFormControlValue();
    this.selectedVehicleGroup = this.vehicleGroupListData[0].vehicleGroupName;
    if(this.vehicleListData.length >0)
    this.selectedVehicle = this.vehicleListData[0].vehicleName;

   
  }

  setGeneralDriverValue(){
    this.fromDisplayDate = Util.convertUtcToDateFormat(this.startDateValue,'DD/MM/YYYY HH:MM:SS');
    this.toDisplayDate = Util.convertUtcToDateFormat(this.endDateValue,'DD/MM/YYYY HH:MM:SS');
    this.selectedVehicleGroup = this.vehicleGroupListData.filter(item => item.vehicleGroupId == parseInt(this.driverTimeForm.controls.vehicleGroup.value))[0]["vehicleGroupName"];
    this.selectedVehicle = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.driverTimeForm.controls.vehicle.value))[0]["vehicleName"];
    this.onSearchData.driverActivities.forEach(element => {
    this.totalDriveTime += element.driveTime,
    this.totalWorkTime += element.workTime,
    this.totalRestTime += element.restTime,
    this.totalAvailableTime += element.availableTime
    });
      this.tableInfoObj= {
        driveTime: Util.getHhMmTime(this.totalDriveTime),
        workTime: Util.getHhMmTime(this.totalWorkTime),
        restTime: Util.getHhMmTime(this.totalRestTime),
        availableTime: Util.getHhMmTime(this.totalAvailableTime),
      }
  }

  setTableInfo(){
   
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
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

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  exportAsExcelFile(){
    this.matTableExporter.exportTable('xlsx', {fileName:'Driver_Time_Report', sheet: 'sheet_name'});
  }

  exportAsPDFFile(){
   
    var doc = new jsPDF();

    (doc as any).autoTable({
      styles: {
          cellPadding: 0.5,
          fontSize: 12
      },       
      didDrawPage: function(data) {     
          // Header
          doc.setFontSize(14);
          var fileTitle = "Driver Details";
          var img = "/assets/logo.png";
          doc.addImage(img, 'JPEG',10,10,0,0);
 
          var img = "/assets/logo_daf.png"; 
          doc.text(fileTitle, 14, 35);
          doc.addImage(img, 'JPEG',150, 10, 0, 10);            
      },
      margin: {
          bottom: 20, 
          top:30 
      }
  });

   // let pdfColumns = [['Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

    let pdfColumns = [['Driver Name', 'Driver Id', 'Start Time', 'End Time', 'Drive Time', 'Work Time', 'Service Time', 'Rest Time', 'Available Time']]
  let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.driverName);
      tempObj.push(e.driverId);
      tempObj.push(e.startTime);
      tempObj.push(e.endTime);
      tempObj.push(e.driveTime);
      tempObj.push(e.workTime);
      tempObj.push(e.serviceTime);
      tempObj.push(e.restTime);
      tempObj.push(e.availableTime);

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
    doc.save('DriverTimeReport.pdf');
  }


  pageSizeUpdated(_evt){

  }


  onDriverSelected(_row){
    this.selectedDriverData = _row;
    let setId = (this.driverListData.filter(elem=>elem.driverID === _row.driverId)[0]['driverID']);
    this.driverTimeForm.get('driver').setValue(setId);
    this.onSearch();
    // this.driverDetails = 
    //   [
    //           {
    //             "driverId": "UK DB08176162022802",
    //             "driverName": "Helloupdated Helloupdated",
    //             "vin": "RERAE75PC0E261011",
    //             "activityDate": 1604338846000,
    //             "startTime": 1604338846000,
    //             "endTime": 1604337628000,
    //             "code": 3,
    //             "restTime": 0,
    //             "availableTime": 0,
    //             "workTime": 0,
    //             "driveTime": 14400,
    //             "serviceTime": 10800
    //           },
    //           {
    //             "driverId": "UK DB08176162022802",
    //             "driverName": "Helloupdated Helloupdated",
    //             "vin": "RERAE75PC0E261011",
    //             "activityDate": 1624370044000,
    //             "startTime": 1604338846000,
    //             "endTime": 1604337628000,
    //             "code": 3,
    //             "restTime": 0,
    //             "availableTime": 0,
    //             "workTime": 0,
    //             "driveTime": 10800,
    //             "serviceTime": 14400
    //           },
    //           {
    //             "driverId": "UK DB08176162022802",
    //             "driverName": "Helloupdated Helloupdated",
    //             "vin": "RERAE75PC0E261011",
    //             "activityDate": 1624427508387,
    //             "startTime": 1604338846000,
    //             "endTime": 1604337628000,
    //             "code": 3,
    //             "restTime": 0,
    //             "availableTime": 0,
    //             "workTime": 0,
    //             "driveTime": 10800,
    //             "serviceTime": 14400
    //           },
              
    //         ]
            // this.driverDetails = _row;
            // let updateData = this.driverDetails;
            // this.setGeneralDriverDetailValue();
            // this.detailConvertedData = this.reportMapService.getDriverDetailsTimeDataBasedOnPref(updateData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);

            this.driverSelected = true;
  }

  backToMainPage(){
    this.driverSelected = false;
    this.updateDataSource(this.initData);
    this.driverTimeForm.get('driver').setValue(0);


  }

  setGeneralDriverDetailValue(){
    this.totalDriveTime = 0;
    this.totalWorkTime = 0;
    this.totalRestTime = 0;
    this.totalAvailableTime= 0;
    this.totalServiceTime = 0;

    this.fromDisplayDate = Util.convertUtcToDateFormat(this.startDateValue,'DD/MM/YYYY HH:MM:SS');
    this.toDisplayDate = Util.convertUtcToDateFormat(this.endDateValue,'DD/MM/YYYY HH:MM:SS');
    this.driverDetails.forEach(element => {
    this.totalDriveTime += element.driveTime,
    this.totalWorkTime += element.workTime,
    this.totalRestTime += element.restTime,
    this.totalAvailableTime += element.availableTime,
    this.totalServiceTime += element.serviceTime
    });
      this.tableDetailsInfoObj= {
        fromDisplayDate : this.fromDisplayDate,
        toDisplayDate : this.toDisplayDate,
        fromDisplayOnlyDate :  this.fromDisplayDate.split(" ")[0],
        toDisplayOnlyDate : this.toDisplayDate.split(" ")[0],
        selectedDriverName: this.driverDetails[0]['driverName'],
        selectedDriverId : this.driverDetails[0]['driverId'],
        driveTime: Util.getHhMmTime(this.totalDriveTime),
        workTime: Util.getHhMmTime(this.totalWorkTime),
        restTime: Util.getHhMmTime(this.totalRestTime),
        availableTime: Util.getHhMmTime(this.totalAvailableTime),
        serviceTime: Util.getHhMmTime(this.totalServiceTime)

      }
  }
  //********************************** Date Time Functions *******************************************//
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
    // if(this.searchFilterpersistData.modifiedFrom == ""){
    //   this.selectedStartTime = "00:00";
    //   this.selectedEndTime = "23:59";
    // }
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
      //console.log("---if searchFilterpersistData startDateStamp exist")
      if(this.searchFilterpersistData.timeRangeSelection !== ""){
        this.selectionTab = this.searchFilterpersistData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      let startDateFromSearch = new Date(this.searchFilterpersistData.startDateStamp);
      let endDateFromSearch = new Date(this.searchFilterpersistData.endDateStamp);
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

  setVehicleGroupAndVehiclePreSelection() {
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
      this.onVehicleGroupChange(this.searchFilterpersistData.vehicleGroupDropDownValue)
    }
  }

  setDefaultDateToFetch(){

  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
  }

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
    // this.searchFilterpersistData["timeRangeSelection"] = this.selectionTab;
    // this.setGlobalSearchData(this.searchFilterpersistData);
    this.resetdriverTimeFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetdriverTimeFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul

    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    //this.endDateValue = event.value._d;
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetdriverTimeFormControlValue(); // extra addded as per discuss with Atul
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
    this.resetdriverTimeFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData();// extra addded as per discuss with Atul
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
    this.resetdriverTimeFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData();
  }



}
