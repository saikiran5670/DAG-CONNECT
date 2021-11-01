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
import { MAT_CHECKBOX_CONTROL_VALUE_ACCESSOR } from '@angular/material/checkbox';
import { ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-eco-score-report',
  templateUrl: './eco-score-report.component.html',
  styleUrls: ['./eco-score-report.component.less']
})
export class EcoScoreReportComponent implements OnInit, OnDestroy {

  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  generalColumnData: any = [];
  generalGraphColumnData: any = [];
  driverPerformanceColumnData: any = [];
  driverPerformanceGraphColumnData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectionTab: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  ecoScoreForm: FormGroup;
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
  prefTimeFormat: any;
  prefTimeZone: any;
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy';
  prefUnitFormat: any = 'dunit_Metric';
  accountPrefObj: any;
  displayedColumns = ['select', 'ranking', 'driverName', 'driverId', 'ecoScoreRanking'];
  detaildisplayedColumns = ['specificdetailstarttime', 'specificdetaildrivetime', 'specificdetailworktime', 'specificdetailservicetime', 'specificdetailresttime', 'specificdetailavailabletime'];
  fromDisplayDate: any;
  toDisplayDate : any;
  selectedVehicleGroup : string;
  selectedVehicle : string;
  driverSelected : boolean = false;
  selectedDriverData: any;
  totalDriveTime : Number = 0;
  totalWorkTime : Number = 0;
  totalRestTime : Number = 0;
  totalAvailableTime : Number = 0;
  totalServiceTime : Number = 0;
  driverDetails : any= [];
  detailConvertedData : any;
  reportPrefData: any = [];
  reportId:number ;
  minTripCheck: any;
  minTripValue: any;
  minDriverCheck: any;
  minDriverValue: any;
  minTripInputCheck: boolean = false;
  minDriverInputCheck: boolean = false;
  compareDriverEcoScore: any;
  compareDriverEcoScoreSearchParam: any;
  profileList: any =[];
  targetProfileId: Number;
  showField: any = {
    select: true,
    ranking: true,
    driverId: true,
    driverName: true,
    ecoScoreRanking: true,
  };
  finalDriverList : any = [];
  finalVehicleList : any =[];
  selectedEcoScore = new SelectionModel(true, []);
  selectedDriversEcoScore = [];
  selectedDriverOption: any;
  selectedDriverId: String;
  selectedDriverName: String;
  ecoScoreDriver: boolean = false;
  compareEcoScore: boolean = false;
  compareButton: boolean = false;
  targetProfileSelected: Number;
  ecoScoreDriverDetails: any;
  ecoScoreDriverDetailsTrendLine: any;
  prefObj: any;
  titleVisible : boolean = false;
  feautreCreatedMsg : any = '';
  trendLineSearchDataParam: any;
  noSingleDriverData: boolean=false;
  isSearched: boolean=false;
  singleVehicle: any = [];
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
  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredDriver: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private translationService: TranslationService,
  private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService, private organizationService: OrganizationService) {

  }

  ngOnInit(): void {
    this.searchFilterpersistData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.isSearched=MAT_CHECKBOX_CONTROL_VALUE_ACCESSOR;
    this.ecoScoreForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      driver: ['', [Validators.required]],
      startDate: ['', []],
      endDate: ['', []],
      startTime: ['', []],
      endTime: ['', []],
      minTripCheck: [false, []],
      minTripValue: [{value:'', disabled: true}],
      minDriverCheck: [false, []],
      minDriverValue: [{value:'', disabled: true}],
      profile: ['', []]
    });
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "ECO Score Report",
      value: "",
      filter: "",
      menuId: 15
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

        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if(vehicleDisplayId) {
          let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if(vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }

       });
    });
    this.isSearched=true;
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
    this.getReportPreferences();
    this.prefObj={
      prefTimeFormat: this.prefTimeFormat,
      prefTimeZone: this.prefTimeZone,
      prefDateFormat: this.prefDateFormat,
      prefUnitFormat: this.prefUnitFormat
    }
  }

  getReportPreferences(){
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      let repoId: any = reportListData.filter(i => i.name == 'EcoScore Report');
      if(repoId.length > 0){
        this.reportId = repoId[0].id;
        this.getEcoScoreReportPreferences();
      }else{
        console.error("No report id found!")
      }
    }, (error)=>{
      console.log('Report not found...', error);
      reportListData = [{name: 'EcoScore Report', id: this.reportId}];
      // this.getEcoScoreReportPreferences();
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

  getEcoScoreReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((data : any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetColumnData();
      this.preparePrefData(this.reportPrefData);
      //this.setDisplayColumnBaseOnPref();
      this.getOnLoadData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetColumnData();
      this.preparePrefData(this.reportPrefData);
      (this.reportPrefData);
      //this.setDisplayColumnBaseOnPref();
      this.getOnLoadData();
    });
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.name.includes('EcoScore.General.')){
              this.generalColumnData.push(_data);
            }else if(item.name.includes('EcoScore.GeneralGraph.')){
              this.generalGraphColumnData.push(_data);
            }else if(item.name.includes('EcoScore.DriverPerformance.')){
              let index: any;
              switch(item.name){
                case 'EcoScore.DriverPerformance.EcoScore':{
                  index = 0;
                  break;
                }
                case 'EcoScore.DriverPerformance.FuelConsumption':{
                  index = 1;
                  break;
                }
                case 'EcoScore.DriverPerformance.BrakingScore':{
                  index = 2;
                  break;
                }
                case 'EcoScore.DriverPerformance.AnticipationScore':{
                  index = 3;
                  break;
                }
              }
              this.driverPerformanceColumnData[index] = _data;
            }else if(item.name.includes('EcoScore.DriverPerformanceGraph.')){
              this.driverPerformanceGraphColumnData.push(_data);
            }
          });
        }
      });
    }
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
      });
    }
  }

  ngOnDestroy(){
    console.log("component destroy...");
    this.searchFilterpersistData["vehicleGroupDropDownValue"] = this.ecoScoreForm.controls.vehicleGroup.value;
    this.searchFilterpersistData["vehicleDropDownValue"] = this.ecoScoreForm.controls.vehicle.value;
    this.searchFilterpersistData["driverDropDownValue"] = this.ecoScoreForm.controls.driver.value;
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
    console.log(this.translationData);
  }

  vehicleDD = [];
  onVehicleGroupChange(event: any){
    // if(event.value){
    this.internalSelection = true;
    this.ecoScoreForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    this.ecoScoreForm.get('driver').setValue(''); //- reset vehicle dropdown
    // this.driverListData = this.finalDriverList;
    // this.vehicleListData = this.finalVehicleList;

    if(parseInt(event.value) == 0){ //-- all group
      this.ecoScoreForm.get('vehicle').setValue(0);
      this.ecoScoreForm.get('driver').setValue(0);
      let vehicleData = this.vehicleListData.slice();
      this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
      console.log("vehicleDD 1", this.vehicleDD);
    }else{
      let search = this.vehicleListData.filter(i => i.vehicleGroupId == parseInt(event.value));
      if(search.length > 0){
        this.vehicleDD = [];
        search.forEach(element => {
          this.vehicleDD.push(element);
          console.log("vehicleDD 2", this.vehicleDD);
        });
      }
      // this.vehicleListData = this.finalVehicleList.filter(i => i.vehicleGroupId == parseInt(event.value));
    }
  // }else {
  //   this.ecoScoreForm.get('vehicleGroup').setValue(parseInt(this.searchFilterpersistData.vehicleGroupDropDownValue));
  //   this.ecoScoreForm.get('vehicle').setValue(parseInt(this.searchFilterpersistData.vehicleDropDownValue));
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
      console.log("driverDD 1", this.driverDD);
      // this.driverListData = this.finalDriverList;
    }else{
      // let selectedVin = this.vehicleListData.filter(i=>i.vehicleId === parseInt(event.value))[0]['vin'];
      // this.driverListData = this.finalDriverList.filter(i => i.vin == selectedVin);
      let search = this.driverListData.filter(i => i.driverID == parseInt(event.value));
      if(search.length > 0){
        this.driverDD = [];
        search.forEach(element => {
          this.driverDD.push(element);
          console.log("driverDD 2", this.driverDD);
        });
      }
    }

    this.searchFilterpersistData["vehicleDropDownValue"] = event.value;
    this.setGlobalSearchData(this.searchFilterpersistData)
    this.internalSelection = true;
  }

  onDriverChange(event: any){ }

  allDriversSelected = true;

  onSearch(){
    this.driverSelected = false;
    this.ecoScoreDriver = false;
    this.isSearched=false;
    // let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    // let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);

    let _vehicelIds = [];
    let _driverIds =[];
    var _minTripVal =0;
    let _minDriverDist=0;
    if (parseInt(this.ecoScoreForm.controls.vehicle.value) === 0) {
      _vehicelIds = this.vehicleListData.map(data => data.vin);
      _vehicelIds.shift();

    }
    else {
      _vehicelIds = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value)).map(data => data.vin);
      if(_vehicelIds.length > 0){
        _vehicelIds = _vehicelIds.filter((value, index, self) => self.indexOf(value) === index);
      }
    }

    if (parseInt(this.ecoScoreForm.controls.driver.value) === 0) {
      this.allDriversSelected = true;
      _driverIds = this.driverListData.map(data=>data.driverID);
      _driverIds.shift();
    }
    else {
      this.allDriversSelected = false
      _driverIds = this.driverListData.filter(item => item.driverID == (this.ecoScoreForm.controls.driver.value)).map(data=>data.driverID);
    }
    if(this.ecoScoreForm.get('minTripCheck').value){
      _minTripVal = Number(this.ecoScoreForm.get('minTripValue').value);
      _minTripVal = this.checkForConversion(_minTripVal);
    }
    if(this.ecoScoreForm.get('minDriverCheck').value){
      _minDriverDist = Number(this.ecoScoreForm.get('minDriverValue').value);
      _minDriverDist = this.checkForConversion(_minDriverDist);
    }

    if(_vehicelIds.length > 0){
      if(this.allDriversSelected){
      this.showLoadingIndicator = true;
        this.reportService.getEcoScoreProfiles(true).subscribe((profiles: any) => {
          this.profileList = profiles.profiles;
          let obj = this.profileList.find(o => o.isDeleteAllowed === false);
          if(obj) this.targetProfileId = obj.profileId;
          this.targetProfileSelected = this.targetProfileId;

          let searchDataParam = {
            "startDateTime":_startTime,
            "endDateTime":_endTime,
            "viNs": _vehicelIds,
            "minTripDistance": _minTripVal,
            "minDriverTotalDistance": _minDriverDist,
            "targetProfileId": this.targetProfileId,
            "reportId": 10
          }
        this.reportService.getEcoScoreDetails(searchDataParam).subscribe((_ecoScoreDriverData: any) => {
        this.hideloader();
          this.setGeneralDriverValue();
          this.updateDataSource(_ecoScoreDriverData.driverRanking);

        }, (error)=>{
          this.isSearched=true;
          this.hideloader();
          this.tableInfoObj = {};
        });
        }, (error)=>{
          this.isSearched=true;
          this.hideloader();
          this.tableInfoObj = {};
        });
        }
       else {
        this.setGeneralDriverValue();
        this.loadSingleDriverDetails();
      }
    }
  }

  onReset(){
    this.internalSelection = true;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.vehicleGroupListData = this.vehicleGroupListData;
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    this.resetEcoScoreFormControlValue();
    this.filterDateData();
    this.initData = [];
    this.tableInfoObj = {};
    this.onSearch();
  }

  resetColumnData(){
    this.generalColumnData = [];
    this.generalGraphColumnData = [];
    this.driverPerformanceColumnData = [];
    this.driverPerformanceGraphColumnData = [];
  }

  resetEcoScoreFormControlValue(){
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== ""){
      if(this.searchFilterpersistData.vehicleDropDownValue !== '')
        this.ecoScoreForm.get('vehicle').setValue(this.searchFilterpersistData.vehicleDropDownValue);
      else
        this.ecoScoreForm.get('vehicle').setValue(0);
      if(this.searchFilterpersistData.vehicleGroupDropDownValue !== '')
        this.ecoScoreForm.get('vehicleGroup').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
      else
        this.ecoScoreForm.get('vehicleGroup').setValue(0);
      if(this.searchFilterpersistData.vehicleGroupDropDownValue !== '')
        this.ecoScoreForm.get('driver').setValue(this.searchFilterpersistData.vehicleGroupDropDownValue);
      else
        this.ecoScoreForm.get('driver').setValue(0);
    }else{
      this.ecoScoreForm.get('vehicleGroup').setValue(0);
      this.ecoScoreForm.get('vehicle').setValue(0);
      this.ecoScoreForm.get('driver').setValue(0);
    }
   // this.ecoScoreForm.get('vehicle').setValue(0);
    this.ecoScoreForm.get('minDriverCheck').setValue(false);
    this.ecoScoreForm.get('minTripCheck').setValue(false);
    this.ecoScoreForm.get('minDriverValue').setValue('');
    this.ecoScoreForm.get('minTripValue').setValue('');
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  getOnLoadData(){
    let defaultStartValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let defaultEndValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    let loadParam = {
      "reportId": 10,
      "accountId": this.accountId,
      "organizationId": this.accountOrganizationId,
      "startDateTime":Util.getMillisecondsToUTCDate(defaultStartValue, this.prefTimeZone),
      "endDateTime":Util.getMillisecondsToUTCDate(defaultEndValue, this.prefTimeZone)
    }
    this.showLoadingIndicator = true;
    this.reportService.getDefaultDriverParameterEcoScore(loadParam).subscribe((initData: any) => {
       this.hideloader();
      this.onLoadData = initData;
      this.filterDateData();
    }, (error)=>{
      this.hideloader();
    });
  }
  setGlobalSearchData(globalSearchFilterData:any) {
    this.searchFilterpersistData["modifiedFrom"] = "EcoScoreReport";
    localStorage.setItem("globalSearchFilterData", JSON.stringify(globalSearchFilterData));
  }

  filterDateData(){
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    let distinctGroupId : any = [];
    let distinctDriverId : any = [];
    let finalDriverList : any = [];
    let currentStartTime =  Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone); //_last3m.getTime();
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone); // _yesterday.getTime();
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
        vinList=finalVinList;
      }
      this.singleVehicle = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i=> i.groupType == 'S');
      if(vinList.length > 0){
        distinctVin = vinList.filter((value, index, self) => self.indexOf(value) === index);
        if(distinctVin && distinctVin.length>0){
          distinctVin.forEach(element => {
           // filteredVehicleList = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element);
            let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element && i.groupType != 'S')
            if(_item.length > 0){
              filteredVehicleList.push(_item[0]); //-- unique VIN data added
              //this.vehicleGroupListData.sort(this.compareGrpName);
              //this.vehicleDD.sort(this.compared);
             // this.driverDD.sort(this.compared);
            //  this.resetVehicleGroupFilter();
              //this.resetVehicleFilter();
              //this.resetDriverFilter();
              _item.forEach(element => {
                finalVehicleList.push(element)
              });

            }

          });
        }
      }


      this.driverListData = filteredDriverList;
      this.vehicleListData = filteredVehicleList;
      this.vehicleGroupListData = finalVehicleList;
      console.log("vehicleGroupListData 1", this.vehicleGroupListData);
      this.vehicleGroupListData.sort(this.compareGrpName);
      this.resetVehicleGroupFilter();
      if(this.vehicleGroupListData.length >0){
        this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll  });
        this.resetVehicleGroupFilter();
      }
      if(this.vehicleListData.length>0 && this.vehicleListData[0].vehicleId != 0){
        this.vehicleListData.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll  });
        this.resetVehicleFilter();
      }
      if(this.driverListData.length>0){
        this.driverListData.unshift({ driverID: 0, firstName: this.translationData.lblAll  });
        this.resetDriverFilter();
      }
      let vehicleData = this.vehicleListData.slice();
      this.vehicleDD = this.getUniqueVINs([...vehicleData]);
      this.vehicleDD.sort(this.compareVin);
      this.resetVehicleFilter();
      this.driverDD = this.driverListData;
      this.driverDD.sort(this.compareName);
      this.resetDriverFilter();

      this.ecoScoreForm.get('vehicleGroup').setValue(0);
    }

// let groupIdArray = [];
// let finalGroupDataList = [];
//     if(this.onLoadData.vehicleDetailsWithAccountVisibiltyList.length > 0){
//       groupIdArray  = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.map(data=>data.vehicleGroupId);
//     if(groupIdArray.length > 0){
//       distinctGroupId = groupIdArray.filter((value, index, self) => self.indexOf(value) === index);

//       if(distinctGroupId.length > 0){
//         distinctGroupId.forEach(element => {
//           let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vehicleGroupId === element);
//           if(_item.length > 0){
//             finalGroupDataList.push(_item[0])
//           }
//         });
//         this.vehicleGroupListData = finalGroupDataList;
//       }
//         this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll  });
//       this.finalVehicleList = [];
//       this.finalVehicleList = this.onLoadData.vehicleDetailsWithAccountVisibiltyList;
//       this.vehicleListData =[];
//       let vinList = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.map(data=>data.vin);
//       if(vinList.length>0){
//         let distinctVIN = vinList.filter((value, index, self) => self.indexOf(value) === index);
//         if(distinctVIN.length > 0){
//           distinctVIN.forEach(element => {
//             let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element);
//             if(_item.length > 0){
//               finalVINDataList.push(_item[0])
//             }
//           });
//       }
//       }
//       this.vehicleListData = finalVINDataList;
//       if(this.vehicleListData.length>0 && this.vehicleListData[0].vehicleId != 0)
//       this.vehicleListData.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll  });
//     }
//     }
//     if(this.onLoadData.driverList.length > 0){
//       let driverIds: any = this.onLoadData.driverList.map(data=> data.vin);
//       if(driverIds.length > 0){
//           distinctDriverId = driverIds.filter((value, index, self) => self.indexOf(value) === index);

//       if(distinctDriverId.length > 0){
//         distinctDriverId.forEach(element => {
//           let _item = this.onLoadData.driverList.filter(i => i.vin === element);
//           if(_item.length > 0){
//             finalDriverList.push(_item[0])
//           }
//         });
//         this.driverListData = finalDriverList.filter(i => (i.activityDateTime >= currentStartTime) && (i.activityDateTime <= currentEndTime));

//         this.driverListData.unshift({ driverID: 0, firstName: this.translationData.lblAll  });
//         this.finalDriverList = this.driverListData;
//       }
//       }
//     }
//     this.resetEcoScoreFormControlValue();
//     this.selectedVehicleGroup = this.vehicleGroupListData[0].vehicleGroupName;
//     if(this.vehicleListData.length >0)
//     this.selectedVehicle = this.vehicleListData[0].vehicleName;
  }
  compareGrpName(a, b) {
    if (a.vehicleGroupName< b.vehicleGroupName) {
      return -1;
    }
    if (a.vehicleGroupName > b.vehicleGroupName) {
      return 1;
    }
    return 0;
  }

  setGeneralDriverValue(){
    this.fromDisplayDate = this.formStartDate(this.startDateValue);
    this.toDisplayDate = this.formStartDate(this.endDateValue);
    this.selectedVehicleGroup = this.vehicleGroupListData.filter(item => item.vehicleGroupId == parseInt(this.ecoScoreForm.controls.vehicleGroup.value))[0]["vehicleGroupName"];
    this.selectedVehicle = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value))[0]["vehicleName"];
    this.selectedDriverId = this.driverListData.filter(item => (item.driverID).toString() == (this.ecoScoreForm.controls.driver.value))[0]["driverID"];
    let driverFirstName = this.driverListData.filter(item => (item.driverID).toString() == (this.ecoScoreForm.controls.driver.value))[0]["firstName"];
    let driverLastName = this.driverListData.filter(item => (item.driverID).toString() == (this.ecoScoreForm.controls.driver.value))[0]["lastName"];
    this.selectedDriverName = driverFirstName +" "+ driverLastName;
    this.selectedDriverOption='';
    this.selectedDriverOption += (this.ecoScoreForm.controls.minTripCheck.value === true) ? (this.translationData.lblInclude ) : (this.translationData.lblExclude );
    this.selectedDriverOption += ' ' + (this.translationData.lblShortTrips ) + ' ';
    this.selectedDriverOption += (this.ecoScoreForm.controls.minDriverCheck.value === true) ?  (this.translationData.lblInclude ) : (this.translationData.lblExclude );
    this.selectedDriverOption += ' ' + (this.translationData.lblMinDriverTotDist );
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
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
          if(a !== undefined && a !== null && b !== undefined && b !== null)
            return this.compare(a[sort.active], b[sort.active], isAsc);
          else
            return 1;
        });
       }
      this.dataSource.filterPredicate = function(data, filter: any){
        return data.driverId.toLowerCase().includes(filter) ||
               data.driverName.toLowerCase().includes(filter) ||
               data.ecoScoreRanking.toString().toLowerCase().includes(filter) ||
               data.ranking.toString().toLowerCase().includes(filter)
      }
      this.showLoadingIndicator=false;
    });
  }

  compare(a: any, b: any, isAsc: boolean) {
    //console.log(a+' '+b);
    if(a === undefined || a === null || b === undefined || b === null)
      return 1;
    if(a !== undefined && a !== null && isNaN(a) && !(a instanceof Number)) a = a.toString().toLowerCase();
    if(a !== undefined && a !== null && isNaN(a) && isNaN(b) && !(b instanceof Number)) b = b.toString().toLowerCase();

    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
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
    const title = 'Eco Score Report';
    const summary = 'Summary Section';
    const detail = 'Detail Section';
    // const header = ['Ranking', 'Driver Name', 'Driver ID', 'Eco-Score'];
    const header =  this.getPDFExcelHeader();
    // const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name', 'Driver ID', 'Driver Name', 'Driver Option'];
    const summaryHeader = this.getExcelSummaryHeader();
    let summaryObj=[
      ['Eco Score Report', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.fromDisplayDate, this.toDisplayDate, this.selectedVehicleGroup,
      this.selectedVehicle, this.selectedDriverId, this.selectedDriverName, this.selectedDriverOption
      ]
    ];
    const summaryData= summaryObj;

    //Create workbook and worksheet
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Eco Score Report');
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
      worksheet.addRow([item.ranking, item.driverName, item.driverId, item.ecoScoreRanking]);
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
      fs.saveAs(blob, 'Eco-Score_Report.xlsx');
   })
    //this.matTableExporter.exportTable('xlsx', {fileName:'Eco-Score_Report', sheet: 'sheet_name'});
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
          var fileTitle = "Eco Score Report";
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

    // let pdfColumns = [['Ranking', 'Driver Name', 'Driver Id', 'Eco-Score']]
    let pdfColumns = this.getPDFExcelHeader();
    pdfColumns = [pdfColumns];
  let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.ranking);
      tempObj.push(e.driverName);
      tempObj.push(e.driverId);
      tempObj.push(e.ecoScoreRanking);
      prepare.push(tempObj);
    });
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {}
    })
    // below line for Download PDF document
    doc.save('EcoScoreReport.pdf');
  }

  getPDFExcelHeader(){
    let col: any = [];
    col = [`${this.translationData.lblRanking || 'Ranking'}`, `${this.translationData.lblDriverName || 'Driver Name'}`, `${this.translationData.lblDriverId || 'Driver Id'}`, `${this.translationData.lblEcoScore || 'Eco-Score' }`];
    return col;
  }

  getExcelSummaryHeader(){
    let col: any = [];
    col = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime || 'Report Start Time'}`, `${this.translationData.lblReportEndTime || 'Report End Time' }`, `${this.translationData.lblVehicleGroup || 'Vehicle Group' }`, `${this.translationData.lblVehicleName || 'Vehicle Name' }`, `${this.translationData.lblDriverId|| 'Driver ID' }`, `${this.translationData.lblDriverName || 'Driver Name' }`, `${this.translationData.lblDriverOption || 'Driver Option' }`];
    return col;
  }

  pageSizeUpdated(_evt){
  }

  onDriverSelected(_row){
    this.selectedDriverId = _row.driverId;
    this.selectedDriverName = _row.driverName;
    this.loadSingleDriverDetails();
    this.isSearched=false;
  }

  loadSingleDriverDetails(){
    this.selectedDriverData = {
      startDate: this.fromDisplayDate,
      endDate: this.toDisplayDate,
      vehicleGroup: this.selectedVehicleGroup,
      vehicleName: this.selectedVehicle,
      driverId: this.selectedDriverId,
      driverName: this.selectedDriverName,
      driverOption: this.selectedDriverOption
    }
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    // let _startTime = Util.convertDateToUtc(this.startDateValue);
    // let _endTime = Util.convertDateToUtc(this.endDateValue);
    let _vehicelIds = [];
    let _driverIds =[];
    let _minTripVal =0;
    let _minDriverDist=0;
    let _prefUnit="Metric";

    _driverIds = this.selectedEcoScore.selected.map(a => a.driverId);
    if (parseInt(this.ecoScoreForm.controls.vehicle.value) === 0) {
      _vehicelIds = this.vehicleListData.map(data => data.vin);
      _vehicelIds.shift();
    }
    else {
      _vehicelIds = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value)).map(data => data.vin);
      if(_vehicelIds.length > 0){
        _vehicelIds = _vehicelIds.filter((value, index, self) => self.indexOf(value) === index);
      }
    }
    if(this.ecoScoreForm.get('minTripCheck').value){
      _minTripVal = Number(this.ecoScoreForm.get('minTripValue').value);
    }
    if(this.ecoScoreForm.get('minDriverCheck').value){
      _minDriverDist = Number(this.ecoScoreForm.get('minDriverValue').value);
    }
    if(this.prefUnitFormat === 'dunit_Metric')
      _prefUnit = 'Metric';
    else if(this.prefUnitFormat === 'dunit_Imperial')
      _prefUnit = 'Imperial';

  let searchDataParam = {
    "startDateTime": _startTime,
    "endDateTime": _endTime,
    "viNs": _vehicelIds,
    "driverId": this.selectedDriverId,
    "minTripDistance": _minTripVal,
    "minDriverTotalDistance": _minDriverDist,
    "targetProfileId": 2,
    "reportId": 10,
    "uoM": _prefUnit
  }
  this.trendLineSearchDataParam = searchDataParam;
    this.reportService.getEcoScoreSingleDriver(searchDataParam).subscribe((_driverDetails: any) => {
    this.noSingleDriverData = false;
      // this.ecoScoreDriverDetails = {"code":200,"message":"Eco-Score Report details fetched successfully.","overallPerformance":{"ecoScore":{"dataAttributeId":286,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","limitType":"N","limitValue":10,"targetValue":10,"score":"6"},"fuelConsumption":{"dataAttributeId":287,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","limitType":"X","limitValue":0,"targetValue":0,"score":"0.0"},"anticipationScore":{"dataAttributeId":305,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","limitType":"N","limitValue":5,"targetValue":8,"score":"6.7"},"brakingScore":{"dataAttributeId":300,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","limitType":"N","limitValue":5,"targetValue":8,"score":"6.0"}},"singleDriver":[{"headerType":"Overall_Driver","vin":"","vehicleName":"","registrationNo":""},{"headerType":"Overall_Company","vin":"","vehicleName":"","registrationNo":""},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","vehicleName":"Vehicle 111","registrationNo":"XCVFDG23555"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","vehicleName":"Vehicle 111","registrationNo":"XCVFDG23555"}],"singleDriverKPIInfo":{"dataAttributeId":275,"name":"EcoScore","key":"rp_ecoscore","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subSingleDriver":[{"dataAttributeId":276,"name":"EcoScore.General","key":"rp_general","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subSingleDriver":[{"dataAttributeId":277,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","limitType":"","limitValue":31.994,"targetValue":31.994,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"356.3"},{"headerType":"Overall_Company","vin":"","value":"356.3"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"356.3"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"356.3"}],"subSingleDriver":[]},{"dataAttributeId":278,"name":"EcoScore.General.Distance","key":"rp_distance","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"headerType":"Overall_Driver","vin":"","value":"758.1"},{"headerType":"Overall_Company","vin":"","value":"758.1"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"758.1"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"758.1"}],"subSingleDriver":[]},{"dataAttributeId":279,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"headerType":"Overall_Driver","vin":"","value":"69.0"},{"headerType":"Overall_Company","vin":"","value":"69.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"69.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"69.0"}],"subSingleDriver":[]},{"dataAttributeId":280,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[{"headerType":"Overall_Driver","vin":"","value":"69.0"},{"headerType":"Overall_Company","vin":"","value":"69.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"69.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"69.0"}],"subSingleDriver":[]},{"dataAttributeId":281,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","limitType":"","limitValue":468.172,"targetValue":468.172,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"758.1"},{"headerType":"Overall_Company","vin":"","value":"758.1"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"758.1"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"758.1"}],"subSingleDriver":[]}]},{"dataAttributeId":285,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","limitType":"","limitValue":0,"targetValue":0,"rangeValueType":"","score":[],"subSingleDriver":[{"dataAttributeId":286,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","limitType":"N","limitValue":10,"targetValue":10,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"6.3"},{"headerType":"Overall_Company","vin":"","value":"6.3"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"6.3"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"6.3"}],"subSingleDriver":[]},{"dataAttributeId":287,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"0.0"},{"headerType":"Overall_Company","vin":"","value":"0.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"0.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"0.0"}],"subSingleDriver":[{"dataAttributeId":288,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"77.8"},{"headerType":"Overall_Company","vin":"","value":"77.8"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"77.8"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"77.8"}],"subSingleDriver":[{"dataAttributeId":289,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"0.3"},{"headerType":"Overall_Company","vin":"","value":"0.3"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"0.3"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"0.3"}],"subSingleDriver":[]},{"dataAttributeId":290,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"1.6"},{"headerType":"Overall_Company","vin":"","value":"1.6"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"1.6"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"1.6"}],"subSingleDriver":[]},{"dataAttributeId":291,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","limitType":"N","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"76.0"},{"headerType":"Overall_Company","vin":"","value":"76.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"76.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"76.0"}],"subSingleDriver":[]}]},{"dataAttributeId":292,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"0.0"},{"headerType":"Overall_Company","vin":"","value":"0.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"0.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"0.0"}],"subSingleDriver":[]},{"dataAttributeId":293,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"headerType":"Overall_Driver","vin":"","value":"0.0"},{"headerType":"Overall_Company","vin":"","value":"0.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"0.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"0.0"}],"subSingleDriver":[]},{"dataAttributeId":294,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"67.0"},{"headerType":"Overall_Company","vin":"","value":"67.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"67.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"67.0"}],"subSingleDriver":[]},{"dataAttributeId":295,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"63.3"},{"headerType":"Overall_Company","vin":"","value":"63.3"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"63.3"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"63.3"}],"subSingleDriver":[]},{"dataAttributeId":296,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","limitType":"X","limitValue":50,"targetValue":50,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"0.5"},{"headerType":"Overall_Company","vin":"","value":"0.5"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"0.5"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"0.5"}],"subSingleDriver":[]},{"dataAttributeId":297,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","limitType":"X","limitValue":2880,"targetValue":2880,"rangeValueType":"T","score":[{"headerType":"Overall_Driver","vin":"","value":"195.0"},{"headerType":"Overall_Company","vin":"","value":"195.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"195.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"195.0"}],"subSingleDriver":[]},{"dataAttributeId":298,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","limitType":"X","limitValue":20,"targetValue":20,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"5.5"},{"headerType":"Overall_Company","vin":"","value":"5.5"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"5.5"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"5.5"}],"subSingleDriver":[]},{"dataAttributeId":299,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"headerType":"Overall_Driver","vin":"","value":"2366.0"},{"headerType":"Overall_Company","vin":"","value":"2366.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"2366.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"2366.0"}],"subSingleDriver":[]}]},{"dataAttributeId":300,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","limitType":"N","limitValue":8,"targetValue":8,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"6.0"},{"headerType":"Overall_Company","vin":"","value":"6.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"6.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"6.0"}],"subSingleDriver":[{"dataAttributeId":301,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)","key":"rp_harshbraking","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"21.1"},{"headerType":"Overall_Company","vin":"","value":"21.1"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"21.1"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"21.1"}],"subSingleDriver":[]},{"dataAttributeId":302,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration","key":"rp_harshbrakeduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"headerType":"Overall_Driver","vin":"","value":"272.0"},{"headerType":"Overall_Company","vin":"","value":"272.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"272.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"272.0"}],"subSingleDriver":[]},{"dataAttributeId":303,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"3.2"},{"headerType":"Overall_Company","vin":"","value":"3.2"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"3.2"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"3.2"}],"subSingleDriver":[]},{"dataAttributeId":304,"name":"EcoScore.DriverPerformance.BrakingScore.BrakeDuration","key":"rp_brakeduration","limitType":"X","limitValue":0,"targetValue":0,"rangeValueType":"T","score":[{"headerType":"Overall_Driver","vin":"","value":"0.0"},{"headerType":"Overall_Company","vin":"","value":"0.0"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"0.0"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"0.0"}],"subSingleDriver":[]}]},{"dataAttributeId":305,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","limitType":"N","limitValue":8,"targetValue":8,"rangeValueType":"D","score":[{"headerType":"Overall_Driver","vin":"","value":"6.7"},{"headerType":"Overall_Company","vin":"","value":"6.7"},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76666","value":"6.7"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76666","value":"6.7"}],"subSingleDriver":[]}]}]},"averageGrossWeightChart":{"xAxisLabel":["0-10 t","10-20 t","20-30 t","30-40 t","40-50 t",">50 t"],"chartDataSet":[{"data":[18.21,81.79,0,0,0,0],"label":"Overall Driver"},{"data":[18.21,81.79,0,0,0,0],"label":"Vehicle 111"}]},"averageDrivingSpeedChart":{"xAxisLabel":["0-15 mph","15-30 mph","30-45 mph","45-50 mph",">50 mph"],"chartDataSet":[{"data":[2.08,0,21.44,17.25,59.22],"label":"Overall Driver"},{"data":[2.08,0,21.44,17.25,59.22],"label":"Vehicle 111"}]}}
      this.ecoScoreDriverDetails = _driverDetails;
      // this.ecoScoreDriverDetails = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","overallPerformance":{"ecoScore":{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","limitType":"N","limitValue":10,"targetValue":10,"score":"4"},"fuelConsumption":{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","limitType":"X","limitValue":0,"targetValue":0,"score":"114.0"},"anticipationScore":{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","limitType":"N","limitValue":5,"targetValue":7.5,"score":"6.3"},"brakingScore":{"dataAttributeId":258,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","limitType":"N","limitValue":5,"targetValue":7.5,"score":"1.4"}},"singleDriver":[{"headerType":"Overall_Driver","vin":"","vehicleName":"","registrationNo":""},{"headerType":"Overall_Company","vin":"","vehicleName":"","registrationNo":""},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76657","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Driver","vin":"NL B000171984000002","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Driver","vin":"NL B000171984000002","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Driver","vin":"NL B000384974000000","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76657","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"}],"singleDriverKPIInfo":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","targetValue":0,"rangeValueType":"","score":[],"subSingleDriver":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","targetValue":0,"rangeValueType":"","score":[],"subSingleDriver":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""},{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""}],"subSingleDriver":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""},{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""}],"subSingleDriver":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subSingleDriver":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subSingleDriver":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","targetValue":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":0,"color":""},{"driverId":"NL B000384974000000","value":0,"color":""}],"subSingleDriver":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","targetValue":0,"rangeValueType":"","score":[],"subSingleDriver":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","targetValue":10,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"},{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":377,"color":"Green"},{"driverId":"NL B000384974000000","value":14663,"color":"Green"}],"subSingleDriver":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":3.3455882352941178,"color":"Green"},{"driverId":"NL B000384974000000","value":16.478379431242697,"color":"Green"}],"subSingleDriver":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":1.9279661016949152,"color":"Green"},{"driverId":"NL B000384974000000","value":13.157076205287714,"color":"Green"}],"subSingleDriver":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","targetValue":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","targetValue":3560,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","targetValue":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":42.3728813559322,"color":"Amber"},{"driverId":"NL B000384974000000","value":20.15552099533437,"color":"Red"}],"subSingleDriver":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","targetValue":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":100,"color":"Green"},{"driverId":"NL B000384974000000","value":648,"color":"Green"}],"subSingleDriver":[]}]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","targetValue":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0.00010416666666666667,"color":"Green"},{"driverId":"NL B000384974000000","value":0.003449074074074074,"color":"Green"}],"subSingleDriver":[]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","targetValue":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":7.58,"color":"Green"}],"subSingleDriver":[]}]}]},"averageGrossWeightChart":{"xAxisLabel":["0-10 t","10-20 t","20-30 t","30-40 t","40-50 t",">50 t"],"chartDataSet":[{"data":[100,0,0,0,0,0],"label":"Overall Driver"},{"data":[100,0,0,0,0,0],"label":"Veh 001"}]},"averageDrivingSpeedChart":{"xAxisLabel":["0-30 kmph","30-50 kmph","50-75 kmph","75-85 kmph",">85 kmph"],"chartDataSet":[{"data":[25.4,44.83,29.76,0,0],"label":"Overall Driver"},{"data":[25.4,44.83,29.76,0,0],"label":"Veh 001"}]}}');
      // this.ecoScoreDriverDetails = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","overallPerformance":{"ecoScore":{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","limitType":"N","limitValue":10,"targetValue":10,"score":"4"},"fuelConsumption":{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","limitType":"X","limitValue":0,"targetValue":0,"score":"114.0"},"anticipationScore":{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","limitType":"N","limitValue":5,"targetValue":7.5,"score":"6.3"},"brakingScore":{"dataAttributeId":258,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","limitType":"N","limitValue":5,"targetValue":7.5,"score":"1.4"}},"singleDriver":[{"headerType":"Overall_Driver","vin":"","vehicleName":"","registrationNo":""},{"headerType":"Overall_Company","vin":"","vehicleName":"","registrationNo":""},{"headerType":"VIN_Driver","vin":"XLR0998HGFFT76657","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Driver","vin":"NL B000171984000002","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Driver","vin":"NL B000171984000002","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Driver","vin":"NL B000384974000000","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"},{"headerType":"VIN_Company","vin":"XLR0998HGFFT76657","vehicleName":"Veh 001","registrationNo":"PLOI098OOO"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""},{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""},{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":0,"color":""},{"driverId":"NL B000384974000000","value":0,"color":""}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":10,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"},{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":377,"color":"Green"},{"driverId":"NL B000384974000000","value":14663,"color":"Green"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":3.3455882352941178,"color":"Green"},{"driverId":"NL B000384974000000","value":16.478379431242697,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":1.9279661016949152,"color":"Green"},{"driverId":"NL B000384974000000","value":13.157076205287714,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":42.3728813559322,"color":"Amber"},{"driverId":"NL B000384974000000","value":20.15552099533437,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":100,"color":"Green"},{"driverId":"NL B000384974000000","value":648,"color":"Green"}],"subCompareDrivers":[]}]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0.00010416666666666667,"color":"Green"},{"driverId":"NL B000384974000000","value":0.003449074074074074,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":7.58,"color":"Green"}],"subCompareDrivers":[]}]}]},"averageGrossWeightChart":{"xAxisLabel":["0-10 t","10-20 t","20-30 t","30-40 t","40-50 t",">50 t"],"chartDataSet":[{"data":[100,0,0,0,0,0],"label":"Overall Driver"},{"data":[100,0,0,0,0,0],"label":"Veh 001"}]},"averageDrivingSpeedChart":{"xAxisLabel":["0-30 kmph","30-50 kmph","50-75 kmph","75-85 kmph",">85 kmph"],"chartDataSet":[{"data":[25.4,44.83,29.76,0,0],"label":"Overall Driver"},{"data":[25.4,44.83,29.76,0,0],"label":"Veh 001"}]}}');
      this.reportService.getEcoScoreSingleDriverTrendLines(searchDataParam).subscribe((_trendLine: any) => {
        // this.ecoScoreDriverDetailsTrendLine = JSON.parse('{"code":200,"message":"Eco-Score Trendline Report details fetched successfully.","trendlines":[{"vin":"Overall","vehicleName":"Overall","kpiInfo":{"ecoScoreCompany":{"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","uoM":"","data":{"07/08/2021":"6.3"}},"ecoScore":{"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","uoM":"","data":{"07/08/2021":"6.3"}},"fuelConsumption":{"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","uoM":"mpg","data":{"07/08/2021":"999918.6"}},"cruiseControlUsage":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","uoM":"%","data":{"07/08/2021":"77.8"}},"cruiseControlUsage3050":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","uoM":"mph(%)","data":{"07/08/2021":"0.3"}},"cruiseControlUsage5075":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","uoM":"mph(%)","data":{"07/08/2021":"1.6"}},"cruiseControlUsage75":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","uoM":"mph(%)","data":{"07/08/2021":"76.0"}},"ptoUsage":{"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","uoM":"%","data":{"07/08/2021":"0.0"}},"ptoDuration":{"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:00:00"}},"averageDrivingSpeed":{"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","uoM":"mph","data":{"07/08/2021":"41.6"}},"averageSpeed":{"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","uoM":"mph","data":{"07/08/2021":"39.3"}},"heavyThrottling":{"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","uoM":"%","data":{"07/08/2021":"0.5"}},"heavyThrottleDuration":{"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:03:15"}},"idling":{"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","uoM":"%","data":{"07/08/2021":"5.5"}},"idleDuration":{"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:39:26"}},"brakingScore":{"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","uoM":"","data":{"07/08/2021":"6.0"}},"harshBraking":{"name":"EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)","key":"rp_harshbraking","uoM":"%","data":{"07/08/2021":"21.1"}},"harshBrakeDuration":{"name":"EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration","key":"rp_harshbrakeduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:04:32"}},"brakeDuration":{"name":"EcoScore.DriverPerformance.BrakingScore.BrakeDuration","key":"rp_brakeduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:00:00"}},"braking":{"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","uoM":"%","data":{"07/08/2021":"3.2"}},"anticipationScore":{"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","uoM":"","data":{"07/08/2021":"6.7"}}}},{"vin":"XLR0998HGFFT76666","vehicleName":"Vehicle 111","kpiInfo":{"ecoScoreCompany":{"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","uoM":"","data":{"07/08/2021":"6.3"}},"ecoScore":{"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","uoM":"","data":{"07/08/2021":"6.3"}},"fuelConsumption":{"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","uoM":"mpg","data":{"07/08/2021":"999918.6"}},"cruiseControlUsage":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","uoM":"%","data":{"07/08/2021":"77.8"}},"cruiseControlUsage3050":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","uoM":"mph(%)","data":{"07/08/2021":"0.3"}},"cruiseControlUsage5075":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","uoM":"mph(%)","data":{"07/08/2021":"1.6"}},"cruiseControlUsage75":{"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","uoM":"mph(%)","data":{"07/08/2021":"76.0"}},"ptoUsage":{"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","uoM":"%","data":{"07/08/2021":"0.0"}},"ptoDuration":{"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:00:00"}},"averageDrivingSpeed":{"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","uoM":"mph","data":{"07/08/2021":"41.6"}},"averageSpeed":{"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","uoM":"mph","data":{"07/08/2021":"39.3"}},"heavyThrottling":{"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","uoM":"%","data":{"07/08/2021":"0.5"}},"heavyThrottleDuration":{"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:03:15"}},"idling":{"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","uoM":"%","data":{"07/08/2021":"5.5"}},"idleDuration":{"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:39:26"}},"brakingScore":{"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","uoM":"","data":{"07/08/2021":"6.0"}},"harshBraking":{"name":"EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)","key":"rp_harshbraking","uoM":"%","data":{"07/08/2021":"21.1"}},"harshBrakeDuration":{"name":"EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration","key":"rp_harshbrakeduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:04:32"}},"brakeDuration":{"name":"EcoScore.DriverPerformance.BrakingScore.BrakeDuration","key":"rp_brakeduration","uoM":"hh:mm:ss","data":{"07/08/2021":"00:00:00"}},"braking":{"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","uoM":"%","data":{"07/08/2021":"3.2"}},"anticipationScore":{"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","uoM":"","data":{"07/08/2021":"6.7"}}}}]}');
        this.ecoScoreDriverDetailsTrendLine = _trendLine;
        // console.log("trendlines "+_trendLine);
        this.driverSelected = true;
        this.compareEcoScore = false;
        this.ecoScoreDriver = true;
     }, (error)=>{
        this.hideloader();
     });
    }, (error)=>{
      this.isSearched=true;
      this.ecoScoreDriver = true;
      this.noSingleDriverData = true;
      this.hideloader();
    });
  }

  backToMainPage(){
    this.compareEcoScore = false;
    this.driverSelected = false;
    this.ecoScoreDriver = false;
    this.updateDataSource(this.initData);
    this.ecoScoreForm.get('driver').setValue(0);
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
  }

  setDefaultTodayDate(){
    if(!this.internalSelection && this.searchFilterpersistData.modifiedFrom !== "") {
      if(this.searchFilterpersistData.timeRangeSelection !== ""){
        this.selectionTab = this.searchFilterpersistData.timeRangeSelection;
      }else{
        this.selectionTab = 'today';
      }
      if(this.searchFilterpersistData.startDateStamp !== '' && this.searchFilterpersistData.endDateStamp !== ''){
      let startDateFromSearch = new Date(this.searchFilterpersistData.startDateStamp);
      let endDateFromSearch = new Date(this.searchFilterpersistData.endDateStamp);
      this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.selectedEndTime, 'end');
      } else {
        this.selectionTab = 'today';
        this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
        this.last3MonthDate = this.getLast3MonthDate();
        this.todayDate = this.getTodayDate();
      }
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
      this.onVehicleGroupChange(this.searchFilterpersistData.vehicleGroupDropDownValue || { value : 0 });
    }
  }

  setDefaultDateToFetch(){
  }

  getTodayDate(){
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    return _todayDate;
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

  getLast3MonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-3);
    return date;
  }

  getLast6MonthDate(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-6);
    return date;
  }

  getLastYear(){
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth()-12);
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
      case 'last6month': {
        this.selectionTab = 'last6month';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLast6MonthDate(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
      case 'lastYear': {
        this.selectionTab = 'lastYear';
        this.setDefaultStartEndTime();
        this.startDateValue = this.setStartEndDateTime(this.getLastYear(), this.selectedStartTime, 'start');
        this.endDateValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
        break;
      }
    }
    this.resetEcoScoreFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
    this.resetEcoScoreFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    this.internalSelection = true;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
    this.resetEcoScoreFormControlValue(); // extra addded as per discuss with Atul
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
    this.resetEcoScoreFormControlValue(); // extra addded as per discuss with Atul
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
    this.resetEcoScoreFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData();
  }

  onProfileChange(event: any){
    this.targetProfileId = event.value;
    this.onSearch();
  }

  onCompare(event: any){
    const numSelected = this.selectedEcoScore.selected.length;

    if(numSelected > 4){
      return;
    } else {
      let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
      let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);

      // let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
      // let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
      let _vehicelIds = [];
      let _driverIds =[];
      var _minTripVal =0;
      let _minDriverDist=0;

      _driverIds = this.selectedEcoScore.selected.map(a => a.driverId);
      //_vehicelIds = this.selectedEcoScore.selected.map(a => a.vin);
      if (parseInt(this.ecoScoreForm.controls.vehicle.value) === 0) {
        _vehicelIds = this.vehicleListData.map(data => data.vin);
        _vehicelIds.shift();
      }
      else {
        _vehicelIds = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value)).map(data => data.vin);
        if(_vehicelIds.length > 0){
          _vehicelIds = _vehicelIds.filter((value, index, self) => self.indexOf(value) === index);
        }
      }
      if(this.ecoScoreForm.get('minTripCheck').value){
        _minTripVal = Number(this.ecoScoreForm.get('minTripValue').value);
      }
      if(this.ecoScoreForm.get('minDriverCheck').value){
        _minDriverDist = Number(this.ecoScoreForm.get('minDriverValue').value);
      }

      let searchDataParam = {
        "startDateTime":_startTime,
        "endDateTime":_endTime,
        "viNs": _vehicelIds,
        "driverIds":_driverIds,
        "minTripDistance":_minTripVal,
        "minDriverTotalDistance": _minDriverDist,
        "targetProfileId": 2,
        "reportId": 10
      }

        // let searchDataParam = {
        //   "startDateTime":_startTime,
        //   "endDateTime":_endTime,
        //   "viNs": [ "M4A14528","M4A1114","M4A1117","XLR0998HGFFT76657","XLRASH4300G1472w0"],
        //   "driverIds": ["NL B000171984000002", "P 0000000542878012","NL B000384974000000"],
        //   "minTripDistance":_minTripVal,
        //   "minDriverTotalDistance": _minDriverDist,
        //   "targetProfileId": 2,
        //   "reportId": 10
        // }
        //searchDataParam = {"startDateTime":1617993000674,"endDateTime":1625855399674,"viNs":["XLR0998HGFFT74597","XLR0998HGFFT74599","XLR0998HGFFT74601","XLR0998HGFFT75550","XLR0998HGFFT74606","XLR0998HGFFT74598","XLR0998HGFFT74592","XLR0998HGFFT74607","XLRTEH4300G328155","XLR0998HGFFT76666","XLR0998HGFFT74603","XLR0998HGFFT74604","XLR0998HGFFT74602","XLRASH4300G1472w0","XLR0998HGFFT74600","XLR0998HGFFT74605"],"driverIds":["NL B000171984000002","NL N110000225456008","NL N110000323456008"],"minTripDistance":0,"minDriverTotalDistance":0,"targetProfileId":2,"reportId":10};
        if(_vehicelIds.length > 0){
          this.showLoadingIndicator = true;
          this.reportService.getEcoScoreDriverCompare(searchDataParam).subscribe((_drivers: any) => {
            //_drivers = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":0,"color":""},{"driverId":"NL B000384974000000","value":0,"color":""}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":10,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":377,"color":"Green"},{"driverId":"NL B000384974000000","value":14663,"color":"Green"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":3.3455882352941178,"color":"Green"},{"driverId":"NL B000384974000000","value":16.478379431242697,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":1.9279661016949152,"color":"Green"},{"driverId":"NL B000384974000000","value":13.157076205287714,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":42.3728813559322,"color":"Amber"},{"driverId":"NL B000384974000000","value":20.15552099533437,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":100,"color":"Green"},{"driverId":"NL B000384974000000","value":648,"color":"Green"}],"subCompareDrivers":[]}]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0.00010416666666666667,"color":"Green"},{"driverId":"NL B000384974000000","value":0.003449074074074074,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":7.58,"color":"Green"}],"subCompareDrivers":[]}]}]}}');
            //  _drivers=JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"},{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""},{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""},{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":0,"color":""},{"driverId":"NL B000384974000000","value":0,"color":""}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":10,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"},{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":377,"color":"Green"},{"driverId":"NL B000384974000000","value":14663,"color":"Green"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":3.3455882352941178,"color":"Green"},{"driverId":"NL B000384974000000","value":16.478379431242697,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":1.9279661016949152,"color":"Green"},{"driverId":"NL B000384974000000","value":13.157076205287714,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":42.3728813559322,"color":"Amber"},{"driverId":"NL B000384974000000","value":20.15552099533437,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":100,"color":"Green"},{"driverId":"NL B000384974000000","value":648,"color":"Green"}],"subCompareDrivers":[]}]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0.00010416666666666667,"color":"Green"},{"driverId":"NL B000384974000000","value":0.003449074074074074,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":7.58,"color":"Green"}],"subCompareDrivers":[]}]}]}}');
            //  _drivers = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Driver5 DriverL5","driverId":"NL B000171984000002"},{"driverName":"Johan PT","driverId":"NL N110000225456008"}],"compareDrivers":{"dataAttributeId":275,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":276,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":277,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":28.78,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":278,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":279,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":280,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":281,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":452.66,"rangeValueType":"D","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":285,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":286,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":287,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[{"dataAttributeId":288,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[{"dataAttributeId":289,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":290,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":291,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":292,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":293,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]},{"dataAttributeId":294,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":295,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":296,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":297,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[],"subCompareDrivers":[]},{"dataAttributeId":298,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":299,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":300,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","target":7.5,"rangeValueType":"D","score":[],"subCompareDrivers":[{"dataAttributeId":301,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)","key":"rp_harshbraking","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":302,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration","key":"rp_harshbrakeduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]},{"dataAttributeId":303,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":304,"name":"EcoScore.DriverPerformance.BrakingScore.BrakeDuration","key":"rp_brakeduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":305,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[],"subCompareDrivers":[]}]}]}}');
            this.compareDriverEcoScoreSearchParam = _drivers;
            this.compareEcoScore = true;
      }, (error)=>{
        this.hideloader();
      });
    }
  }
}

  masterToggleForEcoScore(){
    this.isAllSelectedForEcoScore()
    ? this.selectedEcoScore.clear()
    : this.dataSource.data.forEach((row) =>
      this.selectedEcoScore.select(row)
    );
  }

  isAllSelectedForEcoScore() {
    const numSelected = this.selectedEcoScore.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  setStyle(row: any){
    // if(row.ecoScore < 5)
    //   return {'width': + ((row.ecoScore / 10) * 100) +'%', 'height': '18px', 'background-color': '#f44336'};
    // else if(row.ecoScore > 7.5)
    //   return {'width': + ((row.ecoScore / 10) * 100) +'%', 'height': '18px', 'background-color': '#33cc33'};
    // else
    //   return {'width': + ((row.ecoScore / 10) * 100) +'%', 'height': '18px', 'background-color': '#ff9900'};

      return {'width': + ((row.ecoScoreRanking / 10) * 100) +'%', 'height': '18px', 'background-color': (row.ecoScoreRankingColor === 'Amber'?'Orange':row.ecoScoreRankingColor)};
  }

  rowSelected(event: any, row: any){
    if(event.checked){
      this.selectedEcoScore.select(row);
      const numSelected = this.selectedEcoScore.selected.length;
      if(numSelected <= 4)
        this.selectedDriversEcoScore.push(row);
    } else {
      this.selectedEcoScore.deselect(row);
      const index: number = this.selectedDriversEcoScore.indexOf(row);
      if(index !== -1)
        this.selectedDriversEcoScore.splice(index, 1);
    }
    this.toggleCompareButton();
  }

  deselectDriver(driver: any){
    const index: number = this.selectedDriversEcoScore.indexOf(driver);
    if(index !== -1)
      this.selectedDriversEcoScore.splice(index, 1);
    this.selectedEcoScore.deselect(driver);
    this.toggleCompareButton();
  }

  toggleCompareButton(){
    if(this.selectedEcoScore.selected.length > 1 && this.selectedEcoScore.selected.length <5)
      this.compareButton = true;
    else
      this.compareButton = false;
  }

  validateMinTripVal(){
    if(this.minTripCheck)
      this.ecoScoreForm.controls['minTripValue'].enable();
    else
      this.ecoScoreForm.controls['minTripValue'].disable();
    if(this.minTripCheck && (this.ecoScoreForm.controls.minTripValue.value === null || this.ecoScoreForm.controls.minTripValue.value === '' || this.ecoScoreForm.controls.minTripValue.value < 0 || this.ecoScoreForm.controls.minTripValue.value > 100))
      this.minTripInputCheck = true;
    else
      this.minTripInputCheck = false;
  }
  validateMinDistVal(){
    if(this.minDriverCheck)
      this.ecoScoreForm.controls['minDriverValue'].enable();
    else
      this.ecoScoreForm.controls['minDriverValue'].disable();
    if(this.minDriverCheck && (this.ecoScoreForm.controls.minDriverValue.value === null || this.ecoScoreForm.controls.minDriverValue.value === '' || this.ecoScoreForm.controls.minDriverValue.value < 0 || this.ecoScoreForm.controls.minDriverValue.value > 100))
      this.minDriverInputCheck = true;
    else
      this.minDriverInputCheck = false;
  }

  checkForConversion(val){
    if(this.prefUnitFormat === 'dunit_Imperial')
      return (val * 0.621371).toFixed(2);
    return val;
  }

  vehicleLimitExceeds(item: any){
    if(item.limitExceeds) {
      this.successMsgBlink();
    }
  }

  successMsgBlink(){
    this.titleVisible = true;
    this.feautreCreatedMsg = this.translationData.lblVehicleLimitExceeds;
    setTimeout(() => {
      this.titleVisible = false;
    }, 25000);
  }

  onClose(){
    this.titleVisible = false;
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
    console.log("filterVehicleGroups called");
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
    console.log("this.filteredVehicleGroups", this.filteredVehicleGroups);

  }

  filterVehicle(search){
    console.log("vehicle dropdown called");
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
    console.log("filtered vehicles", this.filteredVehicle);
  }

  filterDriver(DriverSearch){
    console.log("vehicle dropdown called");
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
    console.log("filtered vehicles", this.filteredVehicle);
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
