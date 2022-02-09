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
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../services/data-interchange.service';

@Component({
  selector: 'app-driver-time-management',
  templateUrl: './driver-time-management.component.html',
  styleUrls: ['./driver-time-management.component.less']
})
export class DriverTimeManagementComponent implements OnInit, OnDestroy {
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;

  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  selectionTab: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  driverTimeForm: FormGroup;
  translationData: any = {};
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
  lastMonthDate: any;
  todayDate: any;
  onLoadData: any = [];
  tableInfoObj: any = {};
  tableDetailsInfoObj: any = {};
  totalDriverCount : number = 0;
  tripTraceArray: any = [];
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  accountPrefObj: any;
  displayedColumns = ['driverName', 'driverId', 'startTime', 'endTime', 'driveTime', 'workTime', 'serviceTime', 'restTime', 'availableTime'];
  detaildisplayedColumns = ['startTime', 'driveTime', 'workTime', 'serviceTime', 'restTime', 'availableTime'];
  
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
  summaryObj:any=[];
  singleVehicle: any = [];

  reportPrefData: any = [];
  reportId:number;
  showField: any = {
    driverId:true,
    driverName:true,
    endTime:true,
    startTime:true,
    workTime:true,
    availableTime:true,
    serviceTime:true,
    restTime:true,
    driveTime:true
  };
  showDetailsField: any = {
    endTime:true,
    startTime:true,
    workTime:true,
    availableTime:true,
    serviceTime:true,
    restTime:true,
    driveTime:true,
    specificdetailchart : true
  }
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
      key: 'rp_dtm_report_alldetails_driverid',
      value: 'driverId'
    },
    {
      key: 'rp_dtm_report_alldetails_drivername',
      value: 'driverName'
    },
    {
      key: 'rp_dtm_report_alldetails_endtime',
      value: 'endTime'
    },
    {
      key: 'rp_dtm_report_alldetails_starttime',
      value: 'startTime'
    },
    {
      key: 'rp_dtm_report_alldetails_worktime',
      value: 'workTime'
    },
    {
      key: 'rp_dtm_report_alldetails_availabletime',
      value: 'availableTime'
    },
    {
      key: 'rp_dtm_report_alldetails_servicetime',
      value: 'serviceTime'
    },
    {
      key: 'rp_dtm_report_alldetails_resttime',
      value: 'restTime'
    },
    {
      key: 'rp_dtm_report_alldetails_drivetime',
      value: 'driveTime'
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
      value: 'endTime'
    },
    {
      key: 'rp_dtm_report_bydriver_date',
      value: 'startTime'
    },
    {
      key: 'rp_dtm_report_bydriver_worktime',
      value: 'workTime'
    },
    {
      key: 'rp_dtm_report_bydriver_availabletime',
      value: 'availableTime'
    },
    {
      key: 'rp_dtm_report_bydriver_servicetime',
      value: 'serviceTime'
    },
    {
      key: 'rp_dtm_report_bydriver_resttime',
      value: 'restTime'
    },
    {
      key: 'rp_dtm_report_bydriver_drivetime',
      value: 'driveTime'
    },
    {
      key: 'rp_dtm_report_chart_zoomchart',
      value: 'specificdetailchart'
    }
  ];
  allDriversSelected = true;
  allDriverData : any;
  graphPayload : any;
  noRecordFound: boolean = false;

  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredDriver: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService, 
  private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private organizationService: OrganizationService, private dataInterchangeService: DataInterchangeService) { 
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if(prefResp && (prefResp.type == 'drive time report') && prefResp.prefdata){
        this.displayedColumns = ['driverName', 'driverId', 'startTime', 'endTime', 'driveTime', 'workTime', 'serviceTime', 'restTime', 'availableTime'];
        this.reportPrefData = prefResp.prefdata;
        this.resetPref();
        this.preparePrefData(this.reportPrefData);
        this.onSearch();
      }
    });
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
    });
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

        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if(vehicleDisplayId) {
          let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if(vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }  
      });
    
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      // this.prefTimeFormat = parseInt(_search[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      // this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].value;
      this.prefTimeZone = prefData.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = prefData.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      // this.prefTimeFormat = parseInt(prefData.timeformat[0].value.split(" ")[0]);
      this.prefTimeFormat = Number(prefData.timeformat[0].name.split("_")[1].substring(0,2));
      // this.prefTimeZone = prefData.timezone[0].value;
      this.prefTimeZone = prefData.timezone[0].name;
      this.prefDateFormat = prefData.dateformat[0].name;
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.getReportPreferences();
  }

  getReportPreferences(){
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      let repoId: any = reportListData.filter(i => i.name == 'Drive Time Management');
      if(repoId.length > 0){
        this.reportId = repoId[0].id; 
        this.getDriveTimeReportPreferences();
      }else{
        console.error("No report id found!")
      }
    }, (error)=>{
      //console.log('Report not found...', error);
      reportListData = [{name: 'Drive Time Management', id: this.reportId}];
      // this.getDriveTimeReportPreferences();
    });
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
        this.selectedStartTime = "12:00 AM";
        this.selectedEndTime = "11:59 PM";
      }
    }
  }

  getDriveTimeReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((data : any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetPref();
      this.preparePrefData(this.reportPrefData);
      this.setDisplayColumnBaseOnPref();
      this.getOnLoadData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetPref();
      this.preparePrefData(this.reportPrefData);
      this.setDisplayColumnBaseOnPref();
      this.getOnLoadData();
    });
  }

  allDriverPrefData: any = [];
  specificDriverPrefData: any = [];
  chartPrefData: any = [];
  
  resetPref(){
    this.allDriverPrefData = [];
    this.specificDriverPrefData = [];
    this.chartPrefData = [];
  }  

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.key.includes('rp_dtm_report_chart_')){
              this.chartPrefData.push(_data);
            }else if(item.key.includes('rp_dtm_report_alldetails_')){
              this.allDriverPrefData.push(_data);
           }else if(item.key.includes('rp_dtm_report_bydriver_')){
            this.specificDriverPrefData.push(_data);
           }
          });
        }
      });
      this.setDisplayColumnBaseOnPref();
    }
  }

  setDisplayColumnBaseOnPref(){
    let filterAllDrvPref = this.allDriverPrefData.filter(i => i.state == 'I');
    let filterSpecificDrvPref = this.specificDriverPrefData.filter(i => i.state == 'I');
    
    if(filterAllDrvPref.length > 0){
      filterAllDrvPref.forEach(element => {
        let search = this.prefMapData.filter(i => i.key == element.key);
        if(search.length > 0){
          let index = this.displayedColumns.indexOf(search[0].value);
          if (index > -1) {
              let _value = search[0]['value'];
              this.displayedColumns.splice(index, 1);
              this.showField[_value] = false;
          }
        }
      });
    }

    if(filterSpecificDrvPref.length > 0){
      filterSpecificDrvPref.forEach(item => {
        let _search = this.prefMapData.filter(i => i.key == item.key);
        if(_search.length > 0){
          let detailIndex = this.detaildisplayedColumns.indexOf(_search[0].value);
          if (detailIndex > -1) {
            let _detailvalue = _search[0]['value'];
            this.detaildisplayedColumns.splice(detailIndex, 1);
            this.showDetailsField[_detailvalue] = false;
          }
        }
      });
    }
  }

  ngOnDestroy(){
   // //console.log("component destroy...");
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

 

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //////console.log("process translationData:: ", this.translationData)
  }

  vehicleDD = [];
  onVehicleGroupChange(event: any){
    //if(event.value){
    this.internalSelection = true; 
    this.driverTimeForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    this.driverTimeForm.get('driver').setValue(''); //- reset driver dropdown
    if(parseInt(event.value) == 0){ //-- all group
      //.filter(i => i.vehicleGroupId != 0);
      this.driverTimeForm.get('vehicle').setValue(0);
      this.driverTimeForm.get('driver').setValue(0);
      let vehicleData = this.vehicleListData.slice();
      this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
      //console.log("vehicleDD 1", this.vehicleDD);
    }else{

      //this.vehicleListData = this.vehicleListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      let search = this.vehicleListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      if(search.length > 0){
        this.vehicleDD = [];
        search.forEach(element => {
          this.vehicleDD.push(element);
          //console.log("vehicleDD 2", this.vehicleDD);  
        });
      }
    
    }
  //}else {
    // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event));
    // this.driverTimeForm.get('vehicleGroup').setValue(parseInt(this.searchFilterpersistData.vehicleGroupDropDownValue));
    // this.driverTimeForm.get('vehicle').setValue(parseInt(this.searchFilterpersistData.vehicleDropDownValue));
    // this.driverTimeForm.get('driver').setValue(this.searchFilterpersistData.driverDropDownValue);
 // }
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

  driverDD = [];
  onVehicleChange(event: any){
    if(event.value==0){
      this.driverDD = this.driverListData;
     // this.driverListData = this.finalDriverList;
    }else{
     // let selectedVin = this.vehicleListData.filter(i=>i.vehicleId === parseInt(event.value))[0]['vin'];
     // this.driverListData = this.driverListData.filter(i => i.driverID == parseInt(event.value));
      let search = this.driverListData.filter(i => i.driverID == parseInt(event.value));
      if(search.length > 0){
        this.driverDD = [];
        search.forEach(element => {
          this.driverDD.push(element); 
          //console.log("driverDD 1", this.driverDD);

        });
      }
    }
    
    
    this.searchFilterpersistData["vehicleDropDownValue"] = event.value;
    this.setGlobalSearchData(this.searchFilterpersistData)
    this.internalSelection = true; 
    // this.searchFilterpersistData["vehicleDropDownValue"] = event.value;
    // this.setGlobalSearchData(this.searchFilterpersistData)
  }

  onDriverChange(event: any){
    this.searchFilterpersistData["driverDropDownValue"] = event.value;
    this.setGlobalSearchData(this.searchFilterpersistData)
    this.internalSelection = true; 
  }

  onSearch(){
    // let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    // let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone); 
  
    let _vehicelIds = [];
    let _driverIds =[];
    let _hashDriverIds = [];
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

      _hashDriverIds = this.driverListData.map(data=>data.hashedDriverID); 
      _hashDriverIds.shift();
    }
    else {
      this.allDriversSelected = false;
      _driverIds = this.driverListData.filter(item => item.driverID == (this.driverTimeForm.controls.driver.value)).map(data=>data.driverID);
      _hashDriverIds = this.driverListData.filter(item => item.driverID == (this.driverTimeForm.controls.driver.value)).map(data=>data.hashedDriverID);
    }
    
   
 
   // let _driverData = this.driverListData.map(data=>data.driverID);
    let searchDataParam = {
      "startDateTime":_startTime,
      "endDateTime":_endTime,
      "viNs": _vehicelIds,
      "driverIds":_driverIds,
      "HashedDriverIds":_hashDriverIds
    }
    if(_vehicelIds.length > 0){
      this.showLoadingIndicator = true;
      this.reportService.getDriverTimeDetails(searchDataParam).subscribe((_tripData: any) => {
        if(_tripData.length == 0) {
          this.noRecordFound = true;
        } else {
          this.noRecordFound = false;
        }
        this.hideloader();
        let tripData = _tripData;
        // _tripData["driverActivities"]= [
        //   {
        //     "driverId": "NL B000384974000000",
        //     "driverName": "Hero Honda",
        //     "vin": "XLR0998HGFFT76657",
        //     "activityDate": 1624272277000,
        //     "startTime": 1624273297000,
        //     "endTime": 1624273297000,
        //     "code": 3,
        //     "restTime": 995000,
        //     "availableTime": 0,
        //     "workTime": -940935000,
        //     "driveTime": 2665000,
        //     "serviceTime": -938270000
        //   },
        //   {
        //     "driverId": "PH B110000123456021",
        //     "driverName": "Namita1 Patil",
        //     "vin": "XLR0998HGFFT76657",
        //     "activityDate": 1623330688000,
        //     "startTime": 1623333767000,
        //     "endTime": 1623333767000,
        //     "code": 0,
        //     "restTime": 1033000,
        //     "availableTime": 646000,
        //     "workTime": 0,
        //     "driveTime": 0,
        //     "serviceTime": 646000
        //   },
        //   {
        //     "driverId": "PH B110000123456021",
        //     "driverName": "Namita1 Patil",
        //     "vin": "XLR0998HGFFT76657",
        //     "activityDate": 1623330688000,
        //     "startTime": 1623333767000,
        //     "endTime": 1623333767000,
        //     "code": 0,
        //     "restTime": 1033000,
        //     "availableTime": 646000,
        //     "workTime": 0,
        //     "driveTime": 0,
        //     "serviceTime": 646000
        //   }
        // ]
        if(this.allDriversSelected){
          this.onSearchData = tripData;
          this.driverSelected = false;
          
          this.setGeneralDriverValue();

          let updatedDriverData = this.makeDetailDriverList(tripData.driverActivities);
          this.totalDriverCount = updatedDriverData.length;
          this.allDriverData = [];
          this.allDriverData = updatedDriverData;
          let tableData = updatedDriverData.map(item =>item.cummulativeDriverList)
          this.updateDataSource(tableData);
        }
        else{
         // this.driverSelected = true;
          this.driverDetails = [];
          this.driverDetails = [...tripData.driverActivities];
          let updatedDriverData = this.makeDetailDriverList(tripData.driverActivities);
          this.totalDriverCount = updatedDriverData.length;
          this.detailConvertedData = [];
          this.detailConvertedData = this.reportMapService.getDriverDetailsTimeDataBasedOnPref(this.driverDetails, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
          this.setGeneralDriverDetailValue(updatedDriverData[0]["cummulativeDriverList"]);
          this.graphPayload = {
            "startDateTime": _startTime,
            "endDateTime": _endTime,
            "driverId": _driverIds[0],
            "hashedDriverId": _hashDriverIds[0]
    
          }
        }
       

      }, (error)=>{
        ////console.log(error);
        this.hideloader();
        this.onSearchData = [];
        this.tableInfoObj = {};
        this.driverSelected = false;
        this.allDriversSelected = true;
        this.initData = [];
        this.noRecordFound = true;
        this.updateDataSource(this.initData);
      });
    }
  }

  makeDetailDriverList(driverData: any){
    let _driverArr: any = [];
    let _cummulativeDriverList : any;
    let _arr: any = driverData.map(item => item.driverId).filter((value, index, self) => self.indexOf(value) === index);
    if(_arr.length > 0){
      _arr.forEach(element => {
        let _data = driverData.filter(i => i.driverId == element);
        if(_data.length > 0){
          let startTime = _data[0]['startTime'];
          let endTime = _data[_data.length-1]['endTime'];
          let restTime = 0;
          let serviceTime = 0;
          let availableTime = 0;
          let workTime = 0;
          let driveTime = 0;

          _data.forEach(element => {
            
             restTime  +=  element.restTime;
             serviceTime  +=  element.serviceTime;
             availableTime  +=  element.availableTime;
             workTime  +=  element.workTime;
             driveTime  +=  element.driveTime;

            
              
          });

          _cummulativeDriverList = {
            driverId: _data[0].driverId,
            driverName: _data[0].driverName,
            startTime: startTime,
            endTime: endTime,
            restTime: restTime,
            serviceTime: serviceTime,
            availableTime: availableTime,
            workTime: workTime,
            driveTime: driveTime,
          }
          let _updateCummulative = this.reportMapService.getDriverTimeDataBasedOnPref(_cummulativeDriverList, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
       
          _driverArr.push({
              
            cummulativeDriverList:_updateCummulative,
            driverDetailList: _data
          });
        }
      });
        
    }
    return _driverArr;
  }
 
  onReset(){
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.onSearchData = [];
    this.vehicleGroupListData = this.vehicleGroupListData;
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    //console.log("vehicleGroupListData 1", this.vehicleGroupListData);
    //this.updateDataSource(this.tripData);
    this.resetdriverTimeFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
    this.tableInfoObj = {};
    this.allDriversSelected = true;
    this.initData=[];
    this.noRecordFound = false;
    this.updateDataSource(this.initData);
    this.driverSelected = false;
    //this.advanceFilterOpen = false;
   // this.selectedPOI.clear();
  }

  resetdriverTimeFormControlValue(){
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== ""){
      this.driverTimeForm.get('vehicle').setValue(this.searchFilterpersistData.vehicleDropDownValue);
      this.driverTimeForm.get('vehicleGroup').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
      this.driverTimeForm.get('driver').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
    }else{
      this.driverTimeForm.get('vehicleGroup').setValue('');
      this.driverTimeForm.get('vehicle').setValue('');
      this.driverTimeForm.get('driver').setValue('');
    }
    //this.driverTimeForm.get('vehicle').setValue(0);


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
      "startDateTime": Util.getMillisecondsToUTCDate(defaultStartValue, this.prefTimeZone),
      "endDateTime": Util.getMillisecondsToUTCDate(defaultEndValue, this.prefTimeZone)
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
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); 
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);     
    // let currentStartTime = Util.convertDateToUtc(this.startDateValue); //_last3m.getTime();
    // let currentEndTime = Util.convertDateToUtc(this.endDateValue); // _yesterday.getTime();
    //this.resetdriverTimeFormControlValue();
    //let driverList  = this.onLoadData.driverList.filter(i => (i.activityDateTime >= currentStartTime) && (i.activityDateTime <= currentEndTime)).map(data=>data.driverID);
    let driverList = [];
    this.onLoadData?.driverList?.forEach(element => {
      if(element.activityDateTime && element.activityDateTime.length > 0){
        let search =  element.activityDateTime.filter(item => (item >= currentStartTime) && (item <= currentEndTime)).map(data=>data.driverID);
        if(search.length > 0){
          driverList.push(element.driverID);
        }
      }
    });
    
    let filteredDriverList = [];
    let filteredVehicleList = [];
    let filteredVehicleGroupList = [];
    let vehicleGroupList = []
    let vinList = []
    let finalVehicleList = []
    
    let distinctVin = [];
    this.driverDD = [];
    this.vehicleDD = [];
    this.vehicleGroupListData=[];
    let finalVinList=[];
    ////console.log(driverList.length)
    let distinctDriver;
    if( driverList && driverList.length > 0){
      distinctDriver = driverList.filter((value, index, self) => self.indexOf(value) === index);
      distinctDriver = distinctDriver.filter(i => i !== "")
      if(distinctDriver.length > 0){
        distinctDriver.forEach(element => {
          vinList= this.onLoadData.driverList.filter(i => i.driverID === element).map(data=>data.vin);
          let _item = this.onLoadData.driverList.filter(i => i.driverID === element)
          let _namePresent =  this.checkIfNamePresent(_item);
          if(_item.length > 0 && _namePresent){
            filteredDriverList.push(_item[0]); //-- unique VIN data added 
            _item.forEach(element => {
              finalDriverList.push(element)
            });
          }
          vinList.forEach(vin =>{
            finalVinList.push(vin);
          });
        });
      }
      vinList=finalVinList;
      ////console.log(filteredDriverList)
      ////console.log(finalDriverList)
         //TODO: plz verify fleet-utilisation for below logic
      this.singleVehicle = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i=> i.groupType == 'S');
      if(vinList.length > 0){
        distinctVin = vinList.filter((value, index, self) => self.indexOf(value) === index);
        if(distinctVin && distinctVin.length>0){
          distinctVin.forEach(element => {
           // filteredVehicleList = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element);
            let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element  && i.groupType != 'S')
            if(_item.length > 0){
              filteredVehicleList.push(_item[0]); //-- unique VIN data added 
              //this.vehicleGroupListData.sort(this.compare);
             // this.vehicleDD.sort(this.compare);
             // this.driverDD.sort(this.compare);
             // this.resetVehicleGroupFilter();
              //this.resetVehicleFilter();
              //this.resetDriverFilter();
              _item.forEach(element => {
                finalVehicleList.push(element)
              });
            }
            
          });
        }
      }

      ////console.log(filteredVehicleList);
      ////console.log(finalVehicleList);
      this.driverListData = filteredDriverList;
      this.vehicleListData = filteredVehicleList;
      this.vehicleGroupListData = finalVehicleList;
      //console.log("vehicleGroupListData 2", this.vehicleGroupListData);
      this.vehicleGroupListData.sort(this.compare);
      this.resetVehicleGroupFilter();
      if(this.vehicleGroupListData.length >0){
        this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
        this.resetVehicleGroupFilter();
        this.vehicleListData.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
        this.resetVehicleFilter();

      }
          if(this.driverListData.length>1){
          this.driverListData.unshift({ driverID: 0, firstName: this.translationData.lblAll || 'All' });
          this.resetDriverFilter();
          }
          // if(this.vehicleListData.length>0){
          //   this.vehicleListData.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll || 'All' });
          // }
          let vehicleData = this.vehicleListData.slice();
          this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
          //console.log("vehicleDD 3", this.vehicleDD);
          this.vehicleDD.sort(this.compareVin);
          this.resetVehicleFilter();
          this.driverDD = this.driverListData;
          //console.log("driverDD 2", this.driverDD);
          this.driverDD.sort(this.compareName);
          this.resetDriverFilter();

          this.driverTimeForm.get('vehicleGroup').setValue(0);
          this.driverTimeForm.get('vehicle').setValue(0);
          this.driverTimeForm.get('driver').setValue(0);


    }
    
   
    /////////////////////////////////////
 

        //////console.log("finalVINDataList:: ", finalVINDataList); 
  // this.setVehicleGroupAndVehiclePreSelection();
  }

  setGeneralDriverValue(){
    this.fromDisplayDate = this.formStartDate(this.startDateValue);
    this.toDisplayDate = this.formStartDate(this.endDateValue);
    this.selectedVehicleGroup = this.vehicleGroupListData.filter(item => item.vehicleGroupId == parseInt(this.driverTimeForm.controls.vehicleGroup.value))[0]["vehicleGroupName"];
    this.selectedVehicle = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.driverTimeForm.controls.vehicle.value))[0]["vehicleName"];
    this.onSearchData.driverActivities.forEach(element => {
      if (element.driveTime >= 0)
        this.totalDriveTime += element.driveTime;
      if (element.workTime >= 0)
        this.totalWorkTime += element.workTime;
      if (element.restTime >= 0)
        this.totalRestTime += element.restTime;
      if (element.totalAvailableTime >= 0)
        this.totalAvailableTime += element.availableTime
    });
      this.tableInfoObj= {
        driveTime: Util.getHhMmTimeFromMS(this.totalDriveTime),
        workTime: Util.getHhMmTimeFromMS(this.totalWorkTime),
        restTime: Util.getHhMmTimeFromMS(this.totalRestTime),
        availableTime: Util.getHhMmTimeFromMS(this.totalAvailableTime)
      }
  }

  checkIfNamePresent(_item){
    if(_item[0].firstName != "" || _item[0].lastName != ""){
      return true;
    }
    return false;
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
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  exportAsExcelFile(){      
  const title = this.translationData.lblDriverTimeReport;
  const summary = this.translationData.lblSummarySection;
  const detail = this.translationData.lblAllDetails;
  // const header = ['Driver Name', 'Driver Id', 'Start Time', 'End Time', 'Drive Time(hh:mm)', 'Work Time(hh:mm)', 'Service Time(hh:mm)', 'Rest Time(hh:mm)', 'Available Time(hh:mm)'];
  // const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name', 'Drivers Count', 'Total Drive Time(hh:mm)', 'Total Work Time(hh:mm)', 'Total Available Time(hh:mm)', 'Total Rest Time(hh:mm)'];
  const header = this.getPDFExcelHeader();
  const summaryHeader = this.getExcelSummaryHeader();

  this.summaryObj=[
    [this.translationData.lblDriverTimeReport, this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.fromDisplayDate, this.toDisplayDate,
      this.selectedVehicleGroup, this.selectedVehicle, this.totalDriverCount, this.tableInfoObj.driveTime, 
      this.tableInfoObj.workTime, this.tableInfoObj.availableTime, this.tableInfoObj.restTime
    ]
  ];
  const summaryData= this.summaryObj;
  
  //Create workbook and worksheet
  let workbook = new Workbook();
  let worksheet = workbook.addWorksheet('Driver Time Report');
  //Add Row and formatting
  let titleRow = worksheet.addRow([title]);
  worksheet.addRow([]);
  titleRow.font = { name: 'sans-serif', family: 4, size: 14, underline: 'double', bold: true }
 
  worksheet.addRow([]);  
  let subTitleRow = worksheet.addRow([summary]);
  let summaryRow = worksheet.addRow(summaryHeader);  
  summaryData.forEach(element => {  
    worksheet.addRow(element);   
  });      
  worksheet.addRow([]);
  summaryRow.eachCell((cell, number) => {
    cell.fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFFFFF00' },
      bgColor: { argb: 'FF0000FF' }      
    }
    cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
  })  
  worksheet.addRow([]);   
  let subTitleDetailRow = worksheet.addRow([detail]);
  let headerRow = worksheet.addRow(header);
  headerRow.eachCell((cell, number) => {
    cell.fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFFFFF00' },
      bgColor: { argb: 'FF0000FF' }
    }
    cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
  })

 this.initData.forEach(item => {     
    worksheet.addRow([item.driverName,item.driverId, item.startTime,item.endTime,
      item.driveTime,item.workTime, item.serviceTime, item.restTime,item.availableTime]);   
  }); 
  worksheet.mergeCells('A1:D2'); 
  subTitleRow.font = { name: 'sans-serif', family: 4, size: 11, bold: true }
  subTitleDetailRow.font = { name: 'sans-serif', family: 4, size: 11, bold: true }
  for (var i = 0; i < header.length; i++) {    
    worksheet.columns[i].width = 20;      
  }
  for (var j = 0; j < summaryHeader.length; j++) {  
    worksheet.columns[j].width = 20; 
  }
  worksheet.addRow([]); 
  workbook.xlsx.writeBuffer().then((data) => {
    let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    fs.saveAs(blob, 'Driver_Time_Report.xlsx');
 })
  //this.matTableExporter.exportTable('xlsx', {fileName:'Driver_Time_Report', sheet: 'sheet_name'});
}

getPDFExcelHeader(){
  let col: any = [];
  col = [`${this.translationData.lblDriverName || 'Driver Name'}`, `${this.translationData.lblDriverId || 'Driver Id'}`, `${this.translationData.lblStartTime || 'Start Time'}`, `${this.translationData.lblEndTime || 'End Time' }`, `${this.translationData.lblDriveTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblWorkTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblServiceTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblRestTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblAvailableTime + ' (' + this.translationData.lblhhmm + ')' }`];
  return col;
}

getExcelSummaryHeader(){
  let col: any = [];
  col = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime || 'Report Start Time'}`, `${this.translationData.lblReportEndTime || 'Report End Time' }`, `${this.translationData.lblVehicleGroup || 'Vehicle Group' }`, `${this.translationData.lblVehicleName || 'Vehicle Name' }`, `${this.translationData.lblDriversCount|| 'Drivers Count' }`, `${this.translationData.lblTotalDriveTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalWorkTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalAvailableTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalRestTime + ' (' + this.translationData.lblhhmm + ')' }`];
  return col;
}
  exportAsPDFFile(){
   
    var doc = new jsPDF('p', 'mm', 'a3');

    (doc as any).autoTable({
      styles: {
          cellPadding: 0.5,
          fontSize: 12
      },       
      didDrawPage: function(data) {     
          // Header
          doc.setFontSize(16);
          var fileTitle = this.translationData.lblDriverTimeReport;
          var img = "/assets/logo.png";
          doc.addImage(img, 'JPEG',10,8,0,0);
 
          var img = "/assets/logo_daf.png"; 
          doc.text(fileTitle, 14, 35);
          doc.addImage(img, 'JPEG',250, 10, 0, 8);            
      },
      margin: {
        bottom: 30, 
        top:40
      }
  });

   // let pdfColumns = [['Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

    // let pdfColumns = [['Driver Name', 'Driver Id', 'Start Time', 'End Time', 'Drive Time', 'Work Time', 'Service Time', 'Rest Time', 'Available Time']]
    let pdfColumns = this.getPDFExcelHeader();
    pdfColumns = [pdfColumns];
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
        ////console.log(data.column.index)
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
    this.driverDetails = [];
    this.allDriverData.forEach(element => {
      if(element.cummulativeDriverList.driverId === _row.driverId){
        ////console.log(element.driverDetailList)
        this.driverDetails = [...element.driverDetailList];
        this.setGeneralDriverDetailValue(element.cummulativeDriverList);
     }
    });

    let hashedId = (this.driverListData.filter(elem=>elem.driverID === _row.driverId)[0]['hashedDriverID']);
 //   this.driverDetails = this.allDriverData.map(item=>item.driverDetailList).filter(i=>i.driverID === _row.driverId)
 
    this.detailConvertedData = this.reportMapService.getDriverDetailsTimeDataBasedOnPref(this.driverDetails, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
    this.driverSelected = true;
    this.graphPayload = {   
      "startDateTime":Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone),//this.startDateValue,
      "endDateTime":  Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone), //this.endDateValue,
      "driverId": _row.driverId,
      "hashedDriverId": hashedId
    }
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

  }

  backToMainPage(){
    this.driverSelected = false;
    this.allDriversSelected = true;
    this.updateDataSource(this.initData);
    this.driverTimeForm.get('driver').setValue(0);
  }

  setGeneralDriverDetailValue(_totalValue){
    this.fromDisplayDate = this.formStartDate(this.startDateValue);
    this.toDisplayDate = this.formStartDate(this.endDateValue);
   
      this.tableDetailsInfoObj= {
        fromDisplayDate : this.fromDisplayDate,
        toDisplayDate : this.toDisplayDate,
        fromDisplayOnlyDate :  this.fromDisplayDate.split(" ")[0],
        toDisplayOnlyDate : this.toDisplayDate.split(" ")[0],
        selectedDriverName: _totalValue['driverName'],
        selectedDriverId : _totalValue['driverId'],
        driveTime: _totalValue.driveTime,
        workTime: _totalValue.workTime,
        restTime: _totalValue.restTime,
        availableTime: _totalValue.availableTime,
        serviceTime: _totalValue.serviceTime,

      }
  }
  //********************************** Date Time Functions *******************************************//
  setPrefFormatDate(){
    switch(this.prefDateFormat){
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
      default:{
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
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
    // if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
    //   ////console.log("---if searchFilterpersistData startDateStamp exist")
    //   if(this.searchFilterpersistData.timeRangeSelection !== ""){
    //     this.selectionTab = this.searchFilterpersistData.timeRangeSelection;
    //   }else{
    //     this.selectionTab = 'today';
    //   }
    //   let startDateFromSearch = new Date(this.searchFilterpersistData.startDateStamp);
    //   let endDateFromSearch = new Date(this.searchFilterpersistData.endDateStamp);
    //   this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
    //   this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
    // }else{
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
    this.lastMonthDate = this.getLastMonthDate();
    // }
  }

  setVehicleGroupAndVehiclePreSelection() {
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
      // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
      this.onVehicleGroupChange(this.searchFilterpersistData.vehicleGroupDropDownValue || { value : 0 });
    }
  }

  setDefaultDateToFetch(){

  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    _todayDate.setHours(0);
    _todayDate.setMinutes(0);
    _todayDate.setSeconds(0);
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
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-30);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  getLast3MonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setDate(date.getDate()-90);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }
  setStartEndDateTime(date: any, timeObj: any, type: any){
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
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
    let dateTime: any = '';
    if(event.value._d.getTime() >= this.lastMonthDate.getTime()){ // CurTime > lastMonthDate
      if(event.value._d.getTime() <= this.endDateValue.getTime()){ // CurTime < endDateValue
        dateTime = event.value._d;
      }else{
        dateTime = this.endDateValue; 
      }
    }else{ 
      dateTime = this.lastMonthDate;
    }
    this.startDateValue = this.setStartEndDateTime(dateTime, this.selectedStartTime, 'start');
    this.resetdriverTimeFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul

    //this.startDateValue = event.value._d;
    // this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    let dateTime: any = '';
    if(event.value._d.getTime() <= this.todayDate.getTime()){ // EndTime > todayDate
      if(event.value._d.getTime() >= this.startDateValue.getTime()){ // EndTime < startDateValue
        dateTime = event.value._d;
      }else{
        dateTime = this.startDateValue; 
      }
    }else{ 
      dateTime = this.todayDate;
    }
    this.endDateValue = this.setStartEndDateTime(dateTime, this.selectedEndTime, 'end');
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

  compare(a, b) {
    if (a.vehicleGroupName < b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }
  compareName(a, b) {
    if (a.firstName < b.firstName) {
      return -1;
    }
    if (a.firstName > b.firstName) {
      return 1;
    }
    return 0;
  }
  compareVin(a, b) {
    if (a.vin< b.vin) {
      return -1;
    }
    if (a.vin > b.vin) {
      return 1;
    }
    return 0;
  }
  
    filterVehicleGroups(vehicleSearch){
    //console.log("filterVehicleGroups called");
    if(!this.vehicleGroupListData){
      return;
    }
    if(!vehicleSearch){
      this.resetVehicleGroupFilter();
      return;
    } else {
      vehicleSearch = vehicleSearch.toLowerCase();
    }
    this.filteredVehicleGroups.next(
      this.vehicleGroupListData.filter(item => item.vehicleGroupName.toLowerCase().indexOf(vehicleSearch) > -1)
    );
    //console.log("this.filteredVehicleGroups", this.filteredVehicleGroups);

  }

  filterVehicle(search){
    //console.log("vehicle dropdown called");
    if(!this.vehicleDD){
      return;
    }
    if(!search){
      this.resetVehicleFilter();
      return;
    }else{
      search = search.toLowerCase();
    }
    this.filteredVehicle.next(
      this.vehicleDD.filter(item => item.vin?.toLowerCase()?.indexOf(search) > -1)
    );
    //console.log("filtered vehicles", this.filteredVehicle);
  }

  filterDriver(DriverSearch){
    //console.log("vehicle dropdown called");
    if(!this.driverDD){
      return;
    }
    if(!DriverSearch){
      this.resetDriverFilter();
      return;
    }else{
      DriverSearch = DriverSearch.toLowerCase();
    }
    this.filteredVehicle.next(
      this.driverDD.filter(item => item.firstName.toLowerCase().indexOf(DriverSearch) > -1)
    );
    //console.log("filtered vehicles", this.filteredVehicle);
  }
  
  resetVehicleFilter(){
    this.filteredVehicle.next(this.vehicleDD.slice());
  }
  
   resetVehicleGroupFilter(){
    this.filteredVehicleGroups.next(this.vehicleGroupListData.slice());
  }

  resetDriverFilter(){
    this.filteredDriver.next(this.driverDD.slice());
  }

}
