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

@Component({
  selector: 'app-eco-score-report',
  templateUrl: './eco-score-report.component.html',
  styleUrls: ['./eco-score-report.component.less']
})
export class EcoScoreReportComponent implements OnInit, OnDestroy {
  generalColumnData: any = [];
  generalGraphColumnData: any = [];
  driverPerformanceColumnData: any = [];
  driverPerformanceGraphColumnData: any = [];
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  selectionTab: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59'; 
  ecoScoreForm: FormGroup;
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
  reportId:number = 10;
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

  getReportPreferences(){
    let reportListData: any = [];
    this.reportService.getReportDetails().subscribe((reportList: any)=>{
      reportListData = reportList.reportDetails;
      this.getEcoScoreReportPreferences(reportListData);
    }, (error)=>{
      console.log('Report not found...', error);
      reportListData = [{name: 'EcoScore Report', id: 10}]; // hard coded
      this.getEcoScoreReportPreferences(reportListData);
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
        this.selectedStartTime = "00:00";
        this.selectedEndTime = "23:59";
      }
    }
  }

  getEcoScoreReportPreferences(prefData: any){
    let repoId: any = prefData.filter(i => i.name == 'EcoScore Report');
    this.reportService.getReportUserPreference(repoId.length > 0 ? repoId[0].id : 10).subscribe((data : any) => {
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

  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    console.log(this.translationData);
  }

  onVehicleGroupChange(event: any){
    if(event.value){
      this.internalSelection = true; 
    this.ecoScoreForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    this.ecoScoreForm.get('driver').setValue(''); //- reset vehicle dropdown
    this.driverListData = this.finalDriverList;
    this.vehicleListData = this.finalVehicleList;
    
    if(parseInt(event.value) == 0){ //-- all group
      this.ecoScoreForm.get('vehicle').setValue(0);
      this.ecoScoreForm.get('driver').setValue(0);

    }else{

      this.vehicleListData = this.finalVehicleList.filter(i => i.vehicleGroupId == parseInt(event.value));
    }
  }else {
    this.ecoScoreForm.get('vehicleGroup').setValue(parseInt(this.searchFilterpersistData.vehicleGroupDropDownValue));
    this.ecoScoreForm.get('vehicle').setValue(parseInt(this.searchFilterpersistData.vehicleDropDownValue));
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
  }

  onDriverChange(event: any){
  }

  allDriversSelected = true;

  onSearch(){
    this.driverSelected = false;
    this.ecoScoreDriver = false;
    let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
    let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
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
      this.showLoadingIndicator = true;
        this.reportService.getEcoScoreProfiles(true).subscribe((profiles: any) => {
          this.profileList = profiles.profiles;
          let obj = this.profileList.find(o => o.isDeleteAllowed === false);
          this.targetProfileId = obj.profileId;
          this.targetProfileSelected = this.targetProfileId;
        
          let searchDataParam = {
            "startDateTime":_startTime,
            "endDateTime":_endTime,
            // "viNs": [
            //   "M4A14528","M4A1114","M4A1117","XLR0998HGFFT76657","XLRASH4300G1472w0"
            //   ],
            "viNs": _vehicelIds,
            //"driverIds":_driverIds
            "minTripDistance": _minTripVal,
            "minDriverTotalDistance": _minDriverDist,
            "targetProfileId": this.targetProfileId,
            "reportId": 10
          }

      this.reportService.getEcoScoreDetails(searchDataParam).subscribe((_tripData: any) => {
        this.hideloader();
        let tripData = _tripData; 
      
            // let test = {"driverRanking" : [{"driverId": "NL B000384974000000",
            // "driverName": "Hero Honda",
            // "ecoScoreRanking": 4.171307300509338,
            // "ecoScoreRankingColor": "Red",
            // "ranking": 4},
            // {"driverId": "NL N110000225456008",
            // "driverName": "Johan PT",
            // "ecoScoreRanking": 7.5,
            // "ecoScoreRankingColor": "Green",
            // "ranking": 2},
            // {"driverId": " NL N110000323456008",
            // "driverName": "Johan PU",
            // "ecoScoreRanking": 5.5,
            // "ecoScoreRankingColor": "Red",
            // "ranking": 3},
            // {"driverId": "P 0000000542878012",
            // "driverName": "Driver4 DriverL4",
            // "ecoScoreRanking": 3.6,
            // "ecoScoreRankingColor": "Amber",
            // "ranking": 5},
            // {"driverId": "NL N110000233456008",
            // "driverName": "Johan PV",
            // "ecoScoreRanking": 3.1,
            // "ecoScoreRankingColor": "Amber",
            // "ranking": 6},
            // {"driverId": "NL B000384974000000",
            // "driverName": "Hero Honda2",
            // "ecoScoreRanking": 7.7,
            // "ecoScoreRankingColor": "Green",
            // "ranking": 1}]};
        if(this.allDriversSelected){
          this.onSearchData = tripData;
          this.setGeneralDriverValue();

         // this.initData = this.reportMapService.getDriverTimeDataBasedOnPref(tripData.driverActivities, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
          this.setTableInfo();
          this.updateDataSource(tripData.driverRanking);
          this.setDataForAll();
        }
        else{
          this.ecoScoreDriver = true;
          this.ecoScoreDriverDetails ={};
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
    }, (error)=>{
      this.hideloader();
      this.onSearchData = [];
      this.tableInfoObj = {};
    });
    }    
  }

  setDataForAll(){    
  }

  setSingleDriverData(){
  }

  onReset(){
    this.internalSelection = true;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.onSearchData = [];
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
    let currentStartTime = Util.convertDateToUtc(this.startDateValue); //_last3m.getTime();
    let currentEndTime = Util.convertDateToUtc(this.endDateValue); // _yesterday.getTime();
   
let groupIdArray = [];
let finalGroupDataList = [];
    if(this.onLoadData.vehicleDetailsWithAccountVisibiltyList.length > 0){
      groupIdArray  = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.map(data=>data.vehicleGroupId);
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
      }
      }
    }
    this.resetEcoScoreFormControlValue();
    this.selectedVehicleGroup = this.vehicleGroupListData[0].vehicleGroupName;
    if(this.vehicleListData.length >0)
    this.selectedVehicle = this.vehicleListData[0].vehicleName;  
  }

  setGeneralDriverValue(){
    this.fromDisplayDate = this.formStartDate(this.startDateValue);
    this.toDisplayDate = this.formStartDate(this.endDateValue);
    this.selectedVehicleGroup = this.vehicleGroupListData.filter(item => item.vehicleGroupId == parseInt(this.ecoScoreForm.controls.vehicleGroup.value))[0]["vehicleGroupName"];
    this.selectedVehicle = this.vehicleListData.filter(item => item.vehicleId == parseInt(this.ecoScoreForm.controls.vehicle.value))[0]["vehicleName"];
    this.selectedDriverId = this.driverListData.filter(item => item.driverID == parseInt(this.ecoScoreForm.controls.driver.value))[0]["firstName"];
    this.selectedDriverName = this.driverListData.filter(item => item.driverID == parseInt(this.ecoScoreForm.controls.driver.value))[0]["firstName"];
    this.selectedDriverOption='';
    this.selectedDriverOption += (this.ecoScoreForm.controls.minTripCheck.value === true) ? (this.translationData.lblInclude || 'Include') : (this.translationData.lblExclude || 'Exclude');
    this.selectedDriverOption += ' ' + (this.translationData.lblShortTrips || 'Short Trips') + ' ';
    this.selectedDriverOption += (this.ecoScoreForm.controls.minDriverCheck.value === true) ?  (this.translationData.lblInclude || 'Include') : (this.translationData.lblExclude || 'Exclude');
    this.selectedDriverOption += ' ' + (this.translationData.lblMinDriverTotDist || 'Minimum Driver Total Distance');
  }
  setTableInfo(){
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
          console.log(JSON.stringify(a)+' '+JSON.stringify(b));
          if(a !== undefined && a !== null && b !== undefined && b !== null)
            return this.compare(a[sort.active], b[sort.active], isAsc);
          else
            return 1;
        });
       }
      this.dataSource.filterPredicate = function(data, filter: any){
        console.log(data);
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
    const title = 'Eco Score Report';
    const summary = 'Summary Section';
    const detail = 'Detail Section';
    const header = ['Ranking', 'Driver Name', 'Driver ID', 'Eco-Score'];
    const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Vehicle Group', 'Vehicle Name', 'Driver ID', 'Driver Name', 'Driver Option'];
    let summaryObj=[
      ['Eco Score Report', new Date(), this.fromDisplayDate, this.toDisplayDate, this.selectedVehicleGroup, 
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

    let pdfColumns = [['Ranking', 'Driver Name', 'Driver Id', 'Eco-Score']]
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

  pageSizeUpdated(_evt){
  }

  onDriverSelected(_row){
    console.log(_row);
    this.selectedDriverData = _row;
    this.selectedDriverData = {
      startDate: this.fromDisplayDate,
      endDate: this.toDisplayDate,
      vehicleGroup: this.selectedVehicleGroup,
      vehicleName: this.selectedVehicle,
      driverId: _row.driverId,
      driverName: _row.driverName,
      driverOption: this.selectedDriverOption

    }
    // let setId = (this.driverListData.filter(elem=>elem.driverID === _row.driverId)[0]['driverID']);
    // this.ecoScoreForm.get('driver').setValue(setId);
    // this.onSearch(); 
    let searchDataParam = {
      "startDateTime":1204336888377,
      "endDateTime":1820818919744,
      "viNs": [
        "XLR0998HGFFT76666","XLR0998HGFFT76657","XLRASH4300G1472w0","XLR0998HGFFT74600"
        ],
      "driverId":"NL B000384974000000",
      "minTripDistance":0,
      "minDriverTotalDistance": 0,
      "targetProfileId": 2,
      "reportId": 10,
      "uoM": "Metric"
    }  
    this.reportService.getEcoScoreSingleDriver(searchDataParam).subscribe((_driverDetails: any) => {  
      this.ecoScoreDriverDetails = _driverDetails;          
    //  console.log(JSON.stringify(_driverDetails));
     console.log(_driverDetails);
     this.reportService.getEcoScoreSingleDriverTrendLines(searchDataParam).subscribe((_trendLine: any) => {  
      this.ecoScoreDriverDetailsTrendLine = _trendLine;
      // console.log("trendlines "+JSON.stringify(_trendLine));
      console.log("trendlines "+_trendLine);
      this.driverSelected = true;
      this.compareEcoScore = false;
      this.ecoScoreDriver = true;
     }, (error)=>{
     
     });
    }, (error)=>{
    
    });

    // this.reportService.getEcoScoreSingleDriverTrendLines(searchDataParam).subscribe((_trendLine: any) => {  
    //   this.ecoScoreDriverDetailsTrendLine = _trendLine;
    //   // console.log("trendlines "+JSON.stringify(_trendLine));
    //   console.log("trendlines "+_trendLine);
    //  }, (error)=>{
     
    //  });
    
    // this.driverSelected = true;
    // this.compareEcoScore = false;
    // this.ecoScoreDriver = true;
  }

  backToMainPage(){
    this.compareEcoScore = false;
    this.driverSelected = false;
    this.ecoScoreDriver = false;
    this.updateDataSource(this.initData);
    this.ecoScoreForm.get('driver').setValue(0);
  }

  setGeneralDriverDetailValue(){
    this.totalDriveTime = 0;
    this.totalWorkTime = 0;
    this.totalRestTime = 0;
    this.totalAvailableTime= 0;
    this.totalServiceTime = 0;

    this.fromDisplayDate = this.formStartDate(this.startDateValue);
    this.toDisplayDate = this.formStartDate(this.endDateValue);
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
      let _startTime = Util.convertDateToUtc(this.startDateValue); // this.startDateValue.getTime();
      let _endTime = Util.convertDateToUtc(this.endDateValue); // this.endDateValue.getTime();
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
            //_drivers=JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"},{"driverName":"Driver2 DriverL1","driverId":"NL B000171984000002"},{"driverName":"Hero Honda","driverId":"NL B000384974000000"}],"compareDrivers":{"dataAttributeId":221,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":234,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":235,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""},{"driverId":"NL B000171984000002","value":47.472527472527474,"color":""},{"driverId":"NL B000384974000000","value":7.356973995271868,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":236,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""},{"driverId":"NL B000171984000002","value":455,"color":""},{"driverId":"NL B000384974000000","value":42300,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":237,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":238,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":2,"color":""},{"driverId":"NL B000384974000000","value":1,"color":""}],"subCompareDrivers":[]},{"dataAttributeId":239,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":0,"rangeValueType":"","score":[{"driverId":"NL B000171984000002","value":0,"color":""},{"driverId":"NL B000384974000000","value":0,"color":""}],"subCompareDrivers":[]}]},{"dataAttributeId":243,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":244,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":10,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"},{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":4.24,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":245,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":377,"color":"Green"},{"driverId":"NL B000384974000000","value":14663,"color":"Green"}],"subCompareDrivers":[{"dataAttributeId":246,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[{"dataAttributeId":247,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":248,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":249,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]}]},{"dataAttributeId":250,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":251,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":252,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":3.3455882352941178,"color":"Green"},{"driverId":"NL B000384974000000","value":16.478379431242697,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":253,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":1.9279661016949152,"color":"Green"},{"driverId":"NL B000384974000000","value":13.157076205287714,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":254,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":255,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":0,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":256,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":42.3728813559322,"color":"Amber"},{"driverId":"NL B000384974000000","value":20.15552099533437,"color":"Red"}],"subCompareDrivers":[]},{"dataAttributeId":257,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[{"driverId":"NL B000171984000002","value":100,"color":"Green"},{"driverId":"NL B000384974000000","value":648,"color":"Green"}],"subCompareDrivers":[]}]},{"dataAttributeId":261,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0.00010416666666666667,"color":"Green"},{"driverId":"NL B000384974000000","value":0.003449074074074074,"color":"Green"}],"subCompareDrivers":[]},{"dataAttributeId":263,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[{"driverId":"NL B000171984000002","value":0,"color":"Red"},{"driverId":"NL B000384974000000","value":7.58,"color":"Green"}],"subCompareDrivers":[]}]}]}}');
            //_drivers = JSON.parse('{"code":200,"message":"Eco-Score Report details fetched successfully.","drivers":[{"driverName":"Driver5 DriverL5","driverId":"NL B000171984000002"},{"driverName":"Johan PT","driverId":"NL N110000225456008"}],"compareDrivers":{"dataAttributeId":275,"name":"EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":276,"name":"EcoScore.General","key":"rp_general","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":277,"name":"EcoScore.General.AverageGrossweight","key":"rp_averagegrossweight","target":28.78,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":278,"name":"EcoScore.General.Distance","key":"rp_distance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":279,"name":"EcoScore.General.NumberOfTrips","key":"rp_numberoftrips","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":280,"name":"EcoScore.General.NumberOfVehicles","key":"rp_numberofvehicles","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":281,"name":"EcoScore.General.AverageDistancePerDay","key":"rp_averagedistanceperday","target":452.66,"rangeValueType":"D","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":285,"name":"EcoScore.DriverPerformance","key":"rp_driverperformance","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[{"dataAttributeId":286,"name":"EcoScore.DriverPerformance.EcoScore","key":"rp_ecoscore","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":287,"name":"EcoScore.DriverPerformance.FuelConsumption","key":"rp_fuelconsumption","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[{"dataAttributeId":288,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage","key":"rp_cruisecontrolusage","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[{"dataAttributeId":289,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)","key":"rp_CruiseControlUsage30","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":290,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)","key":"rp_cruisecontroldistance50","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]},{"dataAttributeId":291,"name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)","key":"rp_cruisecontroldistance75","target":0,"rangeValueType":"","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":292,"name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)","key":"rp_ptousage","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":293,"name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration","key":"rp_ptoduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]},{"dataAttributeId":294,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed","key":"rp_averagedrivingspeed","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":295,"name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed","key":"rp_averagespeed","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":296,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)","key":"rp_heavythrottling","target":48.9,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":297,"name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration","key":"rp_heavythrottleduration","target":3560,"rangeValueType":"T","score":[],"subCompareDrivers":[]},{"dataAttributeId":298,"name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)","key":"rp_idling","target":23.7,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":299,"name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration","key":"rp_idleduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":300,"name":"EcoScore.DriverPerformance.BrakingScore","key":"rp_brakingscore","target":7.5,"rangeValueType":"D","score":[],"subCompareDrivers":[{"dataAttributeId":301,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBraking(%)","key":"rp_harshbraking","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":302,"name":"EcoScore.DriverPerformance.BrakingScore.HarshBrakeDuration","key":"rp_harshbrakeduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]},{"dataAttributeId":303,"name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)","key":"rp_braking","target":0,"rangeValueType":"D","score":[],"subCompareDrivers":[]},{"dataAttributeId":304,"name":"EcoScore.DriverPerformance.BrakingScore.BrakeDuration","key":"rp_brakeduration","target":0,"rangeValueType":"T","score":[],"subCompareDrivers":[]}]},{"dataAttributeId":305,"name":"EcoScore.DriverPerformance.AnticipationScore","key":"rp_anticipationscore","target":7.5,"rangeValueType":"D","score":[],"subCompareDrivers":[]}]}]}}');
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
}