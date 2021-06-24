import { Component, ElementRef, Inject, Input, OnInit, ViewChild } from '@angular/core';
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

@Component({
  selector: 'app-driver-time-management',
  templateUrl: './driver-time-management.component.html',
  styleUrls: ['./driver-time-management.component.less']
})
export class DriverTimeManagementComponent implements OnInit {

  
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
  searchFilterpersistData = JSON.parse(localStorage.getItem("globalSearchFilterData"));

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
  private _formBuilder: FormBuilder, private reportService: ReportService, private reportMapService: ReportMapService) { 
    this.defaultTranslation()
  }


  ngOnInit(): void {
    
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
        this.prefTimeFormat = parseInt(prefData.timeformat.filter(i => i.id == this.accountPrefObj.accountPreference.timeFormatId)[0].value.split(" ")[0]);
        this.prefTimeZone = prefData.timezone.filter(i => i.id == this.accountPrefObj.accountPreference.timezoneId)[0].value;
        this.prefDateFormat = prefData.dateformat.filter(i => i.id == this.accountPrefObj.accountPreference.dateFormatTypeId)[0].name;
        this.prefUnitFormat = prefData.unit.filter(i => i.id == this.accountPrefObj.accountPreference.unitId)[0].name;
        this.setDefaultStartEndTime();
        this.setPrefFormatDate();
        this.getReportPreferences();
      });
    });
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
    if(event.value || event.value == 0){

    this.driverTimeForm.get('vehicle').setValue(''); //- reset vehicle dropdown
    if(parseInt(event.value) == 0){ //-- all group
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);

    }else{
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event.value));
    }
    this.searchFilterpersistData["vehicleGroupDropDownValue"] = event.value;
    this.searchFilterpersistData["vehicleDropDownValue"] = '';
    this.setGlobalSearchData(this.searchFilterpersistData)
  }else {
    // this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId == parseInt(event));
    this.driverTimeForm.get('vehicleGroup').setValue(parseInt(this.searchFilterpersistData.vehicleGroupDropDownValue));
    this.driverTimeForm.get('vehicle').setValue(parseInt(this.searchFilterpersistData.vehicleDropDownValue));
  }
  }

  onVehicleChange(event: any){
    this.searchFilterpersistData["vehicleDropDownValue"] = event.value;
    this.setGlobalSearchData(this.searchFilterpersistData)
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
    }

    if (parseInt(this.driverTimeForm.controls.driver.value) === 0) {
      this.allDriversSelected = true;
      _driverIds = this.vehicleListData.map(data=>data.driverID);
      _driverIds.shift();
    }
    else {
      this.allDriversSelected = false
      _driverIds = this.driverListData.filter(item => item.driverID == (this.driverTimeForm.controls.driver.value)).map(data=>data.driverID);
    }
    
   
 
   // let _driverData = this.driverListData.map(data=>data.driverID);
    let searchDataParam = {
      "StartDateTime":_startTime,
      "EndDateTime":_endTime,
      "VINs": _vehicelIds,
      "DriverIds":_driverIds
    }
    if(_vehicelIds.length > 0){
      this.showLoadingIndicator = true;
      //this.reportService.getDriverTimeDetails(searchDataParam).subscribe((_tripData: any) => {
        this.hideloader();
        let tripData = {
          "driverActivities": [
            {
              "driverId": "NL B000384974000000",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            {
              "driverId": "D2",
              "driverName": "Ayrton Senna",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 1,
              "restTime": 0,
              "availableTime": 1218000,
              "workTime": 0,
              "driveTime": 0,
              "serviceTime": 1218000
            },
            {
              "driverId": "UK DB08176162022802",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            {
              "driverId": "D2",
              "driverName": "Ayrton Senna",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 1,
              "restTime": 0,
              "availableTime": 1218000,
              "workTime": 0,
              "driveTime": 0,
              "serviceTime": 1218000
            },
            {
              "driverId": "UK DB08176162022802",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            {
              "driverId": "D2",
              "driverName": "Ayrton Senna",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 1,
              "restTime": 0,
              "availableTime": 1218000,
              "workTime": 0,
              "driveTime": 0,
              "serviceTime": 1218000
            },
            {
              "driverId": "UK DB08176162022802",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            {
              "driverId": "D2",
              "driverName": "Ayrton Senna",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 1,
              "restTime": 0,
              "availableTime": 1218000,
              "workTime": 0,
              "driveTime": 0,
              "serviceTime": 1218000
            },
            {
              "driverId": "UK DB08176162022802",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            {
              "driverId": "D2",
              "driverName": "Ayrton Senna",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 1,
              "restTime": 0,
              "availableTime": 1218000,
              "workTime": 0,
              "driveTime": 0,
              "serviceTime": 1218000
            },
            {
              "driverId": "UK DB08176162022802",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            {
              "driverId": "D2",
              "driverName": "Ayrton Senna",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 0,
              "endTime": 0,
              "code": 1,
              "restTime": 0,
              "availableTime": 1218000,
              "workTime": 0,
              "driveTime": 0,
              "serviceTime": 1218000
            }
          ],
          "code": 200,
          "message": "Trip fetched successfully for requested Filters"
        }

        if(this.allDriversSelected){
          this.onSearchData = tripData;
          this.setGeneralDriverValue();

          this.initData = this.reportMapService.getDriverTimeDataBasedOnPref(tripData.driverActivities, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
          this.setTableInfo();
          this.updateDataSource(this.initData);
          this.setDataForAll();
        }
        else{
          this.driverDetails =   [
            {
              "driverId": "UK DB08176162022802",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1604338846000,
              "startTime": 1604338846000,
              "endTime": 1604337628000,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            {
              "driverId": "UK DB08176162022802",
              "driverName": "Helloupdated Helloupdated",
              "vin": "RERAE75PC0E261011",
              "activityDate": 1624373306931,
              "startTime": 1604338846000,
              "endTime": 1604337628000,
              "code": 3,
              "restTime": 0,
              "availableTime": 0,
              "workTime": 0,
              "driveTime": 1218000,
              "serviceTime": 1218000
            },
            
          ]
          let updateData = this.driverDetails;
          
          this.setGeneralDriverDetailValue();
          this.detailConvertedData = this.reportMapService.getDriverDetailsTimeDataBasedOnPref(updateData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);
          this.setSingleDriverData();
        }
       

      // }, (error)=>{
      //   //console.log(error);
      //   this.hideloader();
      //   this.onSearchData = [];
      //   this.tableInfoObj = {};
      //  // this.updateDataSource(this.tripData);
      // });
    }
  }

  setDataForAll(){
    
  }

  setSingleDriverData(){

  }

  onReset(){
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.onSearchData = [];
    this.vehicleGroupListData = this.vehicleGroupListData;
    this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0);
    //this.updateDataSource(this.tripData);
    this.resetdriverTimeFormControlValue();
    this.filterDateData(); // extra addded as per discuss with Atul
    this.tableInfoObj = {};
    //this.advanceFilterOpen = false;
   // this.selectedPOI.clear();
  }

  resetdriverTimeFormControlValue(){
    this.driverTimeForm.get('vehicleGroup').setValue(0);
    this.driverTimeForm.get('vehicle').setValue(0);
    this.driverTimeForm.get('driver').setValue(0);
    this.searchFilterpersistData["vehicleGroupDropDownValue"] = 0;
    this.searchFilterpersistData["vehicleDropDownValue"] = '';
    this.searchFilterpersistData["driverDropDownValue"] = '';
    this.setGlobalSearchData(this.searchFilterpersistData);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  getOnLoadData(){
    let defaultStartValue = this.setStartEndDateTime(this.getLast3MonthDate(), this.selectedStartTime, 'start');
    let defaultEndValue = this.setStartEndDateTime(this.getYesterdaysDate(), this.selectedEndTime, 'end');
    this.startDateValue = defaultStartValue;
    this.endDateValue = defaultEndValue;
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
     
      this.setDefaultTodayDate();
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
    if(this.onLoadData.vehicleDetailsWithAccountVisibiltyList.length > 0){
      distinctGroupId  = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.map(data=>data.vehicleGroupId);
      //this.vehicleGroupListData = distinctGroupId;
    if(distinctGroupId.length > 0){
      distinctVIN = distinctGroupId.filter((value, index, self) => self.indexOf(value) === index);

      if(distinctVIN.length > 0){
        distinctVIN.forEach(element => {
          let _item = this.onLoadData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vehicleGroupId === element); 
          if(_item.length > 0){
            finalVINDataList.push(_item[0])
          }
        });
        this.vehicleGroupListData = finalVINDataList;

        ////console.log("finalVINDataList:: ", finalVINDataList); 
      }
      this.vehicleGroupListData.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll || 'All' });
      this.vehicleListData = this.vehicleGroupListData.filter(i => i.vehicleGroupId != 0) ;
  //this.vehicleListData = this.vehicleListData.filter(i => (i.activityDateTime >= currentStartTime) && (i.activityDateTime <= currentEndTime));
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

        ////console.log("finalVINDataList:: ", finalVINDataList); 
      }
      }

    }
    this.resetdriverTimeFormControlValue();
    this.selectedVehicleGroup = this.vehicleGroupListData[0].vehicleGroupName;
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

    doc.setFontSize(18);
    doc.text('Driver Details', 11, 8);
    doc.setFontSize(11);
    doc.setTextColor(100);

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
    
    this.driverDetails =   [
              {
                "driverId": "UK DB08176162022802",
                "driverName": "Helloupdated Helloupdated",
                "vin": "RERAE75PC0E261011",
                "activityDate": 1604338846000,
                "startTime": 1604338846000,
                "endTime": 1604337628000,
                "code": 3,
                "restTime": 0,
                "availableTime": 0,
                "workTime": 0,
                "driveTime": 14400,
                "serviceTime": 10800
              },
              {
                "driverId": "UK DB08176162022802",
                "driverName": "Helloupdated Helloupdated",
                "vin": "RERAE75PC0E261011",
                "activityDate": 1624370044000,
                "startTime": 1604338846000,
                "endTime": 1604337628000,
                "code": 3,
                "restTime": 0,
                "availableTime": 0,
                "workTime": 0,
                "driveTime": 10800,
                "serviceTime": 14400
              },
              {
                "driverId": "UK DB08176162022802",
                "driverName": "Helloupdated Helloupdated",
                "vin": "RERAE75PC0E261011",
                "activityDate": 1624427508387,
                "startTime": 1604338846000,
                "endTime": 1604337628000,
                "code": 3,
                "restTime": 0,
                "availableTime": 0,
                "workTime": 0,
                "driveTime": 10800,
                "serviceTime": 14400
              },
              
            ]

            let updateData = this.driverDetails;
            this.setGeneralDriverDetailValue();
            this.detailConvertedData = this.reportMapService.getDriverDetailsTimeDataBasedOnPref(updateData, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat,  this.prefTimeZone);

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

  setPrefFormatTime(){
    if(this.searchFilterpersistData.modifiedFrom !== "" &&  ((this.searchFilterpersistData.startTimeStamp || this.searchFilterpersistData.endTimeStamp) !== "") ) {
      console.log("---if fleetUtilizationSearchData exist")
      this.selectedStartTime = this.searchFilterpersistData.startTimeStamp;
      this.selectedEndTime = this.searchFilterpersistData.endTimeStamp;
      this.startTimeDisplay = `${this.searchFilterpersistData.startTimeStamp+":00"}`;
      this.endTimeDisplay = `${this.searchFilterpersistData.endTimeStamp+":59"}`;
    }else {
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = '00:00:00';
      this.endTimeDisplay = '23:59:59';
    }else{
      this.startTimeDisplay = '12:00 AM';
      this.endTimeDisplay = '11:59 PM';
    }
  }
}

  setDefaultStartEndTime(){
    this.setPrefFormatTime();
    if(this.searchFilterpersistData.modifiedFrom == ""){
      this.selectedStartTime = "00:00";
      this.selectedEndTime = "23:59";
    }
  }

  setDefaultTodayDate(){
    if(this.searchFilterpersistData.modifiedFrom !== "") {
      console.log("---if searchFilterpersistData startDateStamp exist")
      if(this.searchFilterpersistData.timeRangeSelection !== ""){
        this.selectionTab = this.searchFilterpersistData.timeRangeSelection;

        let startDateFromSearch = new Date(this.searchFilterpersistData.startDateStamp);
        let endDateFromSearch =new Date(this.searchFilterpersistData.endDateStamp);
        this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.searchFilterpersistData.startTimeStamp, 'start');
        this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.searchFilterpersistData.endTimeStamp, 'end');
        // this.globalSearchFilterData["timeRangeSelection"] = this.searchFilterpersistData.timeRangeSelection;
        // this.setGlobalSearchData(this.globalSearchFilterData);
        // this.selectionTimeRange(this.selectionTab)
      }else {
        this.selectionTab = 'today';
        let startDateFromSearch = new Date(this.searchFilterpersistData.startDateStamp);
        let endDateFromSearch =new Date(this.searchFilterpersistData.endDateStamp);
        console.log(typeof(startDateFromSearch));
        this.startDateValue = this.setStartEndDateTime(startDateFromSearch, this.searchFilterpersistData.startTimeStamp, 'start');
        this.endDateValue = this.setStartEndDateTime(endDateFromSearch, this.searchFilterpersistData.endTimeStamp, 'end');
      }
      
    }else {
    this.selectionTab = 'today';
    this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    this.last3MonthDate = this.getLast3MonthDate();
    this.todayDate = this.getTodayDate();
  }
  }

  setVehicleGroupAndVehiclePreSelection() {
    if(this.searchFilterpersistData.vehicleDropDownValue !== "") {
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
    if(type == "start"){
      console.log("--date type--",date)
      console.log("--date type--",timeObj)
      this.searchFilterpersistData["startDateStamp"] = date;
      this.searchFilterpersistData.testDate = date;
      this.searchFilterpersistData["startTimeStamp"] = timeObj;
      this.setGlobalSearchData(this.searchFilterpersistData)
      // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
      // console.log("---time after function called--",timeObj)
    }else if(type == "end") {
      this.searchFilterpersistData["endDateStamp"] = date;
      this.searchFilterpersistData["endTimeStamp"] = timeObj;
      this.setGlobalSearchData(this.searchFilterpersistData)
      // localStorage.setItem("globalSearchFilterData", JSON.stringify(this.globalSearchFilterData));
    }
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
    this.searchFilterpersistData["timeRangeSelection"] = this.selectionTab;
    this.setGlobalSearchData(this.searchFilterpersistData);
    this.resetdriverTimeFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>){
    //this.startDateValue = event.value._d;
    this.startDateValue = this.setStartEndDateTime(event.value._d, this.selectedStartTime, 'start');
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>){
    //this.endDateValue = event.value._d;
    this.endDateValue = this.setStartEndDateTime(event.value._d, this.selectedEndTime, 'end');
  }

  startTimeChanged(selectedTime: any) {
    this.selectedStartTime = selectedTime;
    if(this.prefTimeFormat == 24){
      this.startTimeDisplay = selectedTime + ':00';
    }
    else{
      this.startTimeDisplay = selectedTime;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
  }

  endTimeChanged(selectedTime: any) {
    this.selectedEndTime = selectedTime;
    if(this.prefTimeFormat == 24){
      this.endTimeDisplay = selectedTime + ':59';
    }
    else{
      this.endTimeDisplay = selectedTime;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
  }



}
