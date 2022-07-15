import { Component, ElementRef, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { Label, Color, SingleDataSet } from 'ng2-charts';
import { ChartOptions, ChartType, ChartDataSets } from 'chart.js';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { ReportService } from '../../services/report.service';
import { TranslationService } from '../../services/translation.service';
import { OrganizationService } from '../../services/organization.service';
import { Util } from '../../shared/util';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ReportMapService } from '../report-map.service';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { CompleterItem, CompleterService } from 'ng2-completer';
import { ConfigService } from '@ngx-config/core';
import { HereService } from '../../services/here.service';
import { MatIconRegistry } from "@angular/material/icon";
import { DomSanitizer } from '@angular/platform-browser';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import html2canvas from 'html2canvas';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import { DatePipe } from '@angular/common';
import { ReplaySubject } from 'rxjs';
import { DataInterchangeService } from '../../services/data-interchange.service';
import { MessageService } from '../../services/message.service';


declare var H: any;

@Component({
  selector: 'app-fuel-deviation-report',
  templateUrl: './fuel-deviation-report.component.html',
  styleUrls: ['./fuel-deviation-report.component.less'],
  providers: [DatePipe]
})

export class FuelDeviationReportComponent implements OnInit {
  vehicleDisplayPreference = 'dvehicledisplay_VehicleName';
  dataService: any;
  searchMarker: any = {};
  eventIconMarker: any;
  searchStr: string = "";
  suggestionData: any;
  selectedEventMarkers: any = [];
  defaultLayers: any;
  hereMap: any;
  ui: any;
  mapGroup: any;
  map: any;
  @ViewChild("map")
  public mapElement: ElementRef;
  map_key: any = '';
  platform: any = '';
  searchExpandPanel: boolean = true;
  fuelDeviationData: any = [];
  summarySectionData: any = {};
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  vehicleGrpDD: any = [];
  showMap: any = false;
  dataSource: any = new MatTableDataSource([]);
  selectedFuelDeviationEntry = new SelectionModel(true, []);
  showMapPanel: boolean = false;
  fuelDeviationForm: FormGroup;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  reportPrefData: any = [];
  fuelTableDetailsPrefData: any = [];
  fuelSummaryPrefData = [];
  fuelChartsPrefData = [];
  summaryExpandPanel: any = false;
  chartExpandPanel: any = false;
  showSummaryPanel: any = false;
  showChartPanel: any = false;
  vehicleDD: any = [];
  singleVehicle: any = [];
  selectionTab: any;
  showLoadingIndicator: boolean = false;
  wholeFuelDeviationData: any = [];
  tableInfoObj: any = {};
  fuelDeviationReportId: number;
  displayedColumns = ['All', 'fuelEventType', 'convertedDifference', 'vehicleName', 'vin', 'registrationNo', 'eventTime', 'odometer', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed', 'drivingTime', 'alerts'];
  pdfDisplayedColumns = ['All', 'fuelEventType', 'convertedDifference', 'vehicleName', 'vin', 'registrationNo', 'eventTime', 'odometer', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed', 'drivingTime', 'alerts'];
  startDateValue: any;
  tableExpandPanel: boolean = true;
  last3MonthDate: any;
  todayDate: any;
  endDateValue: any;
  translationData: any = {};
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  globalSearchFilterData: any = JSON.parse(localStorage.getItem("globalSearchFilterData"));
  accountId: any;
  accountPrefObj: any;
  selectedStartTime: any = '00:00';
  selectedEndTime: any = '23:59';
  startTimeDisplay: any = '00:00:00';
  endTimeDisplay: any = '23:59:59';
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  chartLabelDateFormat: any = 'MM/DD/YYYY';
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  internalSelection: boolean = false;
  fuelDeviationChartLabels = [];
  excelSummaryData: any = [];
  brandimagePath: any;
  maxStartTime: any;
 selectedStartTimeValue: any ='00:00';
 selectedEndTimeValue: any ='11:59';
 endTimeStart:any;
  prefMapData: any = [
    {
      key: 'rp_fd_details_averageweight',
      value: 'averageWeight'
    },
    {
      key: 'rp_fd_details_enddate',
      value: 'endTimeStamp'
    },
    {
      key: 'rp_fd_details_fuelconsumed',
      value: 'fuelConsumed'
    },
    {
      key: 'rp_fd_details_startdate',
      value: 'startTimeStamp'
    },
    {
      key: 'rp_fd_details_drivingtime',
      value: 'drivingTime'
    },
    {
      key: 'rp_fd_details_startposition',
      value: 'startPosition'
    },
    {
      key: 'rp_fd_details_difference',
      value: 'convertedDifference'
    },
    {
      key: 'rp_fd_details_alerts',
      value: 'alerts'
    },
    {
      key: 'rp_fd_details_idleduration',
      value: 'idleDuration'
    },
    {
      key: 'rp_fd_details_endposition',
      value: 'endPosition'
    },
    {
      key: 'rp_fd_details_regplatenumber',
      value: 'registrationNo'
    },
    {
      key: 'rp_fd_details_odometer',
      value: 'odometer'
    },
    {
      key: 'rp_fd_details_averagespeed',
      value: 'averageSpeed'
    },
    {
      key: 'rp_fd_details_distance',
      value: 'distance'
    },
    {
      key: 'rp_fd_details_date',
      value: 'eventTime'
    },
    {
      key: 'rp_fd_details_type',
      value: 'fuelEventType'
    },
    {
      key: 'rp_fd_details_vin',
      value: 'vin'
    },
    {
      key: 'rp_fd_details_vehiclename',
      value: 'vehicleName'
    }
  ];
  summaryBlock: any = {
    fuelIncrease: false,
    fuelDecrease: false,
    fuelVehicleEvent: false
  }
  fuelDeviationDChartLabels: Label[] = [];
  fuelDeviationDChartData: any = [];
  fuelDeviationDChartType: ChartType = 'doughnut';
  fuelDeviationDChartColors: Color[] = [
    {
      backgroundColor: ['#65C3F7', '#F4AF85']
    }
  ];
  chartLegend = true;
  public fuelDeviationDChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom'
    },
    cutoutPercentage: 70
  };
  fuelDeviationPChartOptions: ChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom'
    }
  };
  fuelDeviationPChartType: ChartType = 'pie';
  fuelDeviationPChartLabels: Label[] = [];
  fuelDeviationPChartData: SingleDataSet = [];
  fuelDeviationPChartLegend = true;
  fuelDeviationPChartPlugins = [];
  fuelDeviationPChartColors: Color[] = [
    {
      backgroundColor: ['#65C3F7', '#F4AF85']
    }
  ];
  fuelDeviationChart: any = {
    fuelIncreaseEvent: {
      state: false,
      lineChart: false
    },
    fuelDecreaseEvent: {
      state: false,
      lineChart: false
    },
    fuelDeviationEvent: {
      state: false,
      dChart: false
    }
  };
  _xIncLine: any = [];
  _yIncLine: any = [];
  fuelIncLineChartData: ChartDataSets[] = [];
  fuelIncLineChartLabels: Label[] = this.fuelDeviationChartLabels;
  fuelIncLineChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
      onClick: null
    },
    scales: {
      yAxes: [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          steps: 10,
          stepSize: 1,
          beginAtZero: true,
        },
        scaleLabel: {
          display: true,
          labelString: 'Values(Fuel Increase Events)'
        }
      }],
      xAxes: [{
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }]
    }
  };
  fuelIncLineChartColors: Color[] = [
    {
      borderColor: '#65C3F7',
      backgroundColor: 'rgba(255,255,0,0)',
    },
  ];
  fuelIncLineChartLegend = true;
  fuelIncLineChartPlugins = [];
  fuelIncLineChartType = 'line';
  _xDecLine: any = [];
  _yDecLine: any = [];
  fuelDecLineChartData: ChartDataSets[] = [];
  fuelDecLineChartLabels: Label[] = this.fuelDeviationChartLabels;
  fuelDecLineChartOptions = {
    responsive: true,
    legend: {
      position: 'bottom',
      onClick: null
    },
    scales: {
      yAxes: [{
        id: "y-axis-1",
        position: 'left',
        type: 'linear',
        ticks: {
          steps: 10,
          stepSize: 1,
          beginAtZero: true,
        },
        scaleLabel: {
          display: true,
          labelString: 'Values(Fuel Decrease Events)'
        }
      }], xAxes: [{
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }]
    }
  };
  fuelDecLineChartColors: Color[] = [
    {
      borderColor: '#F4AF85',
      backgroundColor: 'rgba(255,255,0,0)',
    },
  ];
  fuelDecLineChartLegend = true;
  fuelDecLineChartPlugins = [];
  fuelDecLineChartType = 'line';
  fuelIncBarChartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom',
      onClick: null
    },
    scales: {
      yAxes: [
        {
          id: "y-axis",
          position: 'left',
          type: 'linear',
          ticks: {
            beginAtZero: true,
            steps: 10,
            stepSize: 1,
          },
          scaleLabel: {
            display: true,
            labelString: 'Values (Fuel Increase Events)'
          }
        }],
      xAxes: [{
        barThickness: 2,
        gridLines: {
          drawOnChartArea: false
        },
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }]
    }
  };
  fuelIncBarChartLabels: Label[] = this.fuelDeviationChartLabels;
  fuelIncBarChartType: ChartType = 'bar';
  fuelIncBarChartLegend = true;
  fuelIncBarChartPlugins = [];
  fuelIncBarChartData: any[] = [];
  fuelDecBarChartOptions: any = {
    responsive: true,
    legend: {
      position: 'bottom',
      onClick: null
    },
    scales: {
      yAxes: [
        {
          id: "y-axis",
          position: 'left',
          type: 'linear',
          ticks: {
            beginAtZero: true,
            steps: 10,
            stepSize: 1,
          },
          scaleLabel: {
            display: true,
            labelString: 'Values (Fuel Decrease Events)'
          }
        }],
      xAxes: [{
        barThickness: 2,
        gridLines: {
          drawOnChartArea: false
        },
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }]
    }
  };
  fuelDecBarChartLabels: Label[] = this.fuelDeviationChartLabels;
  fuelDecBarChartType: ChartType = 'bar';
  fuelDecBarChartLegend = true;
  fuelDecBarChartPlugins = [];
  fuelDecBarChartData: any[] = [];
  noRecordFound: boolean = false;

  public filteredVehicleGroups: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  public filteredVehicle: ReplaySubject<String[]> = new ReplaySubject<String[]>(1);
  prefDetail: any = {};
  reportDetail: any = [];
  vehVinRegChecker: any = [
    { key: 'rp_fd_details_vehiclename', attr: 'vehicleName' },
    { key: 'rp_fd_details_vin', attr: 'vin' },
    { key: 'rp_fd_details_regplatenumber', attr: 'registrationNo' }
  ];

  constructor(@Inject(MAT_DATE_FORMATS) private dateFormats, private organizationService: OrganizationService, private _formBuilder: FormBuilder, private translationService: TranslationService, private reportService: ReportService, private reportMapService: ReportMapService, private completerService: CompleterService, private configService: ConfigService, private hereService: HereService, private matIconRegistry: MatIconRegistry,private domSanitizer: DomSanitizer,private datePipe: DatePipe, private dataInterchangeService: DataInterchangeService,  private messageService: MessageService) {
    // this.map_key = this.configService.getSettings("hereMap").api_key;
    this.map_key = localStorage.getItem("hereMapsK");
    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });
    this.dataInterchangeService.prefSource$.subscribe((prefResp: any) => {
      if (prefResp && (prefResp.type == 'fuel deviation report') && prefResp.prefdata) {
        this.displayedColumns = ['All', 'fuelEventType', 'convertedDifference', 'vehicleName', 'vin', 'registrationNo', 'eventTime', 'odometer', 'startTimeStamp', 'endTimeStamp', 'distance', 'idleDuration', 'averageSpeed', 'averageWeight', 'startPosition', 'endPosition', 'fuelConsumed', 'drivingTime', 'alerts'];
        this.reportPrefData = prefResp.prefdata;
        this.resetFuelDeviationPrefData();
        this.getTranslatedColumnName(this.reportPrefData);
        this.onSearch();
      }
    });
    this.configureAutoSuggest();
    this.setIcons();
  }

  setIcons() {
    this.matIconRegistry.addSvgIcon("fuel-incr-run", this.domSanitizer.bypassSecurityTrustResourceUrl("assets/images/icons/fuelDeviationIcons/fuel-increase-run.svg"));
    this.matIconRegistry.addSvgIcon("fuel-desc-stop", this.domSanitizer.bypassSecurityTrustResourceUrl("assets/images/icons/fuelDeviationIcons/fuel-decrease-stop.svg"));
  }

  ngOnDestroy() {
    this.setFilterValues();
  }

  @HostListener('window:beforeunload', ['$event'])
  reloadWindow($event: any) {
    this.setFilterValues();
  }

  setFilterValues() {
    this.globalSearchFilterData["vehicleGroupDropDownValue"] = this.fuelDeviationForm.controls.vehicleGroup.value;
    this.globalSearchFilterData["vehicleDropDownValue"] = this.fuelDeviationForm.controls.vehicle.value;
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

  ngOnInit() {
    this.globalSearchFilterData = JSON.parse(localStorage.getItem("globalSearchFilterData"));
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    this.reportDetail = JSON.parse(localStorage.getItem('reportDetail'));
    this.fuelDeviationForm = this._formBuilder.group({
      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
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
      menuId: 12 //-- for Fuel Deviation Report
    }
    
    let menuId = 'menu_12_' + this.localStLanguage.code;
    if (!localStorage.getItem(menuId)) {
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);
      });
    } else {
      this.translationData = JSON.parse(localStorage.getItem(menuId));
    }

      if (this.prefDetail) {
        if (this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != '') {
          this.proceedStep(this.accountPrefObj.accountPreference);
        } else {
          this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(orgPref);
          }, (error) => {
            this.proceedStep({});
          });
        }
        let vehicleDisplayId = this.accountPrefObj.accountPreference.vehicleDisplayId;
        if (vehicleDisplayId) {
          let vehicledisplay = this.prefDetail.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
          if (vehicledisplay.length != 0) {
            this.vehicleDisplayPreference = vehicledisplay[0].name;
          }
        }
      }
    

    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this.domSanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
    });

  }

  initMap() {
    this.defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      this.defaultLayers.raster.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, this.defaultLayers);
    this.mapGroup = new H.map.Group();
    this.ui.removeControl("mapsettings");
    var ms = new H.ui.MapSettingsControl({
      baseLayers: [{
        label: this.translationData.lblNormal, layer: this.defaultLayers.raster.normal.map
      }, {
        label: this.translationData.lblSatellite, layer: this.defaultLayers.raster.satellite.map
      }, {
        label: this.translationData.lblTerrain, layer: this.defaultLayers.raster.terrain.map
      }
      ],
      layers: [{
        label: this.translationData.lblLayerTraffic, layer: this.defaultLayers.vector.normal.traffic
      },
      {
        label: this.translationData.lblLayerIncidents, layer: this.defaultLayers.vector.normal.trafficincidents
      }
      ]
    });
    this.ui.addControl("customized", ms);
  }

  clearRoutesFromMap() {
    this.hereMap.removeObjects(this.hereMap.getObjects());
    this.mapGroup.removeAll();
    this.eventIconMarker = null;
  }

  private configureAutoSuggest() {
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?' + 'apiKey=' + this.map_key + '&limit=5' + '&q=' + searchParam;
    this.suggestionData = this.completerService.remote(
      URL, 'title', 'title');
    this.suggestionData.dataField("items");
    this.dataService = this.suggestionData;
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    let langCode =this.localStLanguage? this.localStLanguage.code : 'EN-GB';
    let menuId = 'menu_12_'+ langCode;
    localStorage.setItem(menuId, JSON.stringify(this.translationData));
  }

  proceedStep(preference: any) {
    let _search = this.prefDetail.timeformat.filter(i => i.id == preference.timeFormatId);
    if (_search.length > 0) {
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = this.prefDetail.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = this.prefDetail.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = this.prefDetail.unit.filter(i => i.id == preference.unitId)[0].name;
    } else {
      this.prefTimeFormat = Number(this.prefDetail.timeformat[0].name.split("_")[1].substring(0, 2));
      this.prefTimeZone = this.prefDetail.timezone[0].name;
      this.prefDateFormat = this.prefDetail.dateformat[0].name;
      this.prefUnitFormat = this.prefDetail.unit[0].name;
    }
    this.setDefaultStartEndTime();
    this.setPrefFormatDate();
    this.setDefaultTodayDate();
    this.getReportPreferences();
  }

  getReportPreferences() {
    if (this.reportDetail) {
      let repoId: any = this.reportDetail.filter(i => i.name == 'Fuel Deviation Report');
      if (repoId.length > 0) {
        this.fuelDeviationReportId = repoId[0].id;
        this.getFuelDeviationReportPreferences();
      } else {
        console.error("No report id found!")
      }
    }
  }

  getFuelDeviationReportPreferences() {
    this.reportService.getReportUserPreference(this.fuelDeviationReportId).subscribe((data: any) => {
      this.reportPrefData = data["userPreferences"];
      this.resetFuelDeviationPrefData();
      this.getTranslatedColumnName(this.reportPrefData);
      this.loadFuelDeviationData();
    }, (error) => {
      this.reportPrefData = [];
      this.resetFuelDeviationPrefData();
      this.loadFuelDeviationData();
    });
  }

  hideloader() {
    this.showLoadingIndicator = false;
  }

  loadFuelDeviationData() {
    this.showLoadingIndicator = true;
    this.reportService.getVINFromTripFueldeviation(this.accountId, this.accountOrganizationId).subscribe((tripData: any) => {
      this.hideloader();
      this.wholeFuelDeviationData = tripData;
      this.filterDateData();
    }, (error) => {
      this.hideloader();
      this.wholeFuelDeviationData.vinTripList = [];
      this.wholeFuelDeviationData.vehicleDetailsWithAccountVisibiltyList = [];
      this.filterDateData();
    });
  }

  filterDateData() {
    let distinctVIN: any = [];
    let finalVINDataList: any = [];
    this.vehicleListData = [];
    this.vehicleGrpDD = [];
    let currentStartTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let currentEndTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    if (this.wholeFuelDeviationData.vinTripList.length > 0) {
      let vinArray = [];
      this.wholeFuelDeviationData.vinTripList.forEach(element => {
        if (element.endTimeStamp && element.endTimeStamp.length > 0) {
          let search = element.endTimeStamp.filter(item => (item >= currentStartTime) && (item <= currentEndTime));
          if (search.length > 0) {
            vinArray.push(element.vin);
          }
        }
      });
      this.singleVehicle = this.wholeFuelDeviationData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.groupType == 'S');
      if (vinArray.length > 0) {
        distinctVIN = vinArray.filter((value, index, self) => self.indexOf(value) === index);
        if (distinctVIN.length > 0) {
          distinctVIN.forEach(element => {
            let _item = this.wholeFuelDeviationData.vehicleDetailsWithAccountVisibiltyList.filter(i => i.vin === element && i.groupType != 'S');
            if (_item.length > 0) {
              this.vehicleListData.push(_item[0]); //-- unique VIN data added
              _item.forEach(element => {
                finalVINDataList.push(element)
              });
            }
          });
        }
      } else {
        this.fuelDeviationForm.get('vehicle').setValue('');
        this.fuelDeviationForm.get('vehicleGroup').setValue('');
      }
    }
    this.vehicleGroupListData = finalVINDataList;
    if (this.vehicleGroupListData.length > 0) {
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
      this.vehicleGrpDD.unshift({ vehicleGroupId: 0, vehicleGroupName: this.translationData.lblAll });
      this.resetVehicleGroupFilter();
    }
    let vehicleData = this.vehicleListData.slice();
    this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
    this.vehicleDD.sort(this.compareVin);
    this.resetVehicleFilter();
    if (this.vehicleDD.length > 0) {
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll, registrationNo: this.translationData.lblAll, vin: this.translationData.lblAll });
      this.resetVehicleFilter();
      this.resetFuelDeviationFormControlValue();
    }
    this.setVehicleGroupAndVehiclePreSelection();
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

  setVehicleGroupAndVehiclePreSelection() {
    if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      this.onVehicleGroupChange(this.globalSearchFilterData.vehicleGroupDropDownValue, false);
    }
  }

  vehVinRegCheck(item: any) {
    let _s = this.vehVinRegChecker.filter(i => i.key == item.key);
    if (_s.length > 0) {
      let index = this.vehVinRegChecker.map(i => i.attr).indexOf(_s[0].attr); // find index
      if (index > -1) {
        this.vehVinRegChecker.splice(index, 1); // removed
      }
    }
  }

  setDisplayColumnBaseOnPref() {
    if (this.fuelSummaryPrefData.length > 0) {
      this.summaryBlock.fuelIncrease = this.fuelSummaryPrefData[0].state == 'A' ? true : false;
      this.summaryBlock.fuelDecrease = this.fuelSummaryPrefData[1].state == 'A' ? true : false;
      this.summaryBlock.fuelVehicleEvent = this.fuelSummaryPrefData[2].state == 'A' ? true : false;
    }
    if (this.fuelTableDetailsPrefData.length > 0) { //- Table details
      let filterPref = this.fuelTableDetailsPrefData.filter(i => i.state == 'I'); // removed unchecked
      if (filterPref.length > 0) {
        filterPref.forEach(element => {
          this.vehVinRegCheck(element);
          let search = this.prefMapData.filter(i => i.key == element.key); // present or not
          if (search.length > 0) {
            let index = this.displayedColumns.indexOf(search[0].value); // find index
            if (index > -1) {
              this.displayedColumns.splice(index, 1); // removed
            }
          }
        });
      }
    }
    if (this.fuelChartsPrefData.length > 0) {
      this.fuelDeviationChart.fuelIncreaseEvent = {
        state: this.fuelChartsPrefData[0].state == 'A' ? true : false,
        lineChart: (this.fuelChartsPrefData[0].chartType == 'L') ? true : false
      }
      this.fuelDeviationChart.fuelDecreaseEvent = {
        state: this.fuelChartsPrefData[1].state == 'A' ? true : false,
        lineChart: (this.fuelChartsPrefData[1].chartType == 'L') ? true : false
      }
      this.fuelDeviationChart.fuelDeviationEvent = {
        state: this.fuelChartsPrefData[2].state == 'A' ? true : false,
        dChart: (this.fuelChartsPrefData[2].chartType == 'D') ? true : false
      }
    }
  }

  getTranslatedColumnName(prefData: any) {
    if (prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0) {
      prefData.subReportUserPreferences.forEach(element => {
        if (element.subReportUserPreferences && element.subReportUserPreferences.length > 0) {
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if (item.key.includes('rp_fd_summary_')) {
              let _index: any;
              switch (item.key) {
                case 'rp_fd_summary_fuelincreaseevents': {
                  _index = 0;
                  break;
                }
                case 'rp_fd_summary_fueldecreaseevents': {
                  _index = 1;
                  break;
                }
                case 'rp_fd_summary_vehiclewithfuelevents': {
                  _index = 2;
                  break;
                }
              }
              this.fuelSummaryPrefData[_index] = _data;
            } else if (item.key.includes('rp_fd_chart_')) {
              let index: any;
              switch (item.key) {
                case 'rp_fd_chart_fuelincreaseevents': {
                  index = 0;
                  break;
                }
                case 'rp_fd_chart_fueldecreaseevents': {
                  index = 1;
                  break;
                }
                case 'rp_fd_chart_fueldeviationevent': {
                  index = 2;
                  break;
                }
              }
              this.fuelChartsPrefData[index] = _data;
            } else if (item.key.includes('rp_fd_details_')) {
              this.fuelTableDetailsPrefData.push(_data);
            }
          });
        }
      });
      this.setDisplayColumnBaseOnPref();
    }
  }

  resetFuelDeviationPrefData() {
    this.fuelSummaryPrefData = [];
    this.fuelChartsPrefData = [];
    this.fuelTableDetailsPrefData = [];
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
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
    } else {
      this.selectionTab = 'today';
      this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
      this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
      this.last3MonthDate = this.getLast3MonthDate();
      this.todayDate = this.getTodayDate();
    }
  }

  getLast3MonthDate() {
    var date = Util.getUTCDate(this.prefTimeZone);
    date.setMonth(date.getMonth() - 3);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return date;
  }

  getTodayDate() {
    let _todayDate: any = Util.getUTCDate(this.prefTimeZone);
    _todayDate.setHours(0);
    _todayDate.setMinutes(0);
    _todayDate.setSeconds(0);
    return _todayDate;
  }

  setStartEndDateTime(date: any, timeObj: any, type: any) {
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
  }

  changeEndDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
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
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1)+ "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.selectedEndTime;
      this.endTimeStart = this.selectedStartTime; 
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
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
  }

  setPrefFormatDate() {
    switch (this.prefDateFormat) {
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats.display.dateInput = "DD/MM/YYYY";
        this.chartLabelDateFormat = 'DD/MM/YYYY';
        this.dateFormats.parse.dateInput = "DD/MM/YYYY";
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat = 'MM/DD/YYYY';
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats.display.dateInput = "DD-MM-YYYY";
        this.chartLabelDateFormat = 'DD-MM-YYYY';
        this.dateFormats.parse.dateInput = "DD-MM-YYYY";
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats.display.dateInput = "MM-DD-YYYY";
        this.chartLabelDateFormat = 'MM-DD-YYYY';
        this.dateFormats.parse.dateInput = "MM-DD-YYYY";
        break;
      }
      default: {
        this.dateFormats.display.dateInput = "MM/DD/YYYY";
        this.chartLabelDateFormat = 'MM/DD/YYYY';
        this.dateFormats.parse.dateInput = "MM/DD/YYYY";
      }
    }
  }

  setDefaultStartEndTime() {
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
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
  }

  resetFuelDeviationFormControlValue() {
    if (!this.internalSelection && this.globalSearchFilterData.modifiedFrom !== "") {
      this.fuelDeviationForm.get('vehicle').setValue(this.globalSearchFilterData.vehicleDropDownValue);
      this.fuelDeviationForm.get('vehicleGroup').setValue(this.globalSearchFilterData.vehicleGroupDropDownValue);
      this.onVehicleGroupChange({'value': this.globalSearchFilterData.vehicleGroupDropDownValue}, true);
    }else{
      this.fuelDeviationForm.get('vehicle').setValue(0);
      this.fuelDeviationForm.get('vehicleGroup').setValue(0);
    }
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

  onVehicleGroupChange(event: any, flag?: any) {
    let _val: any;
    if (flag && (event.value || event.value == 0)) { // from internal veh-grp DD event
      this.internalSelection = true;
      _val = parseInt(event.value);
      this.fuelDeviationForm.get('vehicle').setValue(_val == 0 ? 0 : '');
    }
    else { // from local-storage
      _val = parseInt(this.globalSearchFilterData.vehicleGroupDropDownValue);
      this.fuelDeviationForm.get('vehicleGroup').setValue(_val);
    }
    if (_val == 0) { //-- all group
      this.vehicleDD = [];
      let vehicleData = this.vehicleListData.slice();
      this.vehicleDD = this.getUniqueVINs([...this.singleVehicle, ...vehicleData]);
      this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll, registrationNo: this.translationData.lblAll, vin: this.translationData.lblAll });
      this.resetVehicleFilter();
    } else {
      let search = this.vehicleGroupListData.filter(i => i.vehicleGroupId == _val);
      if (search.length > 0) {
        this.vehicleDD = [];
        search.forEach(element => {
          this.vehicleDD.push(element);
        });
        this.vehicleDD.unshift({ vehicleId: 0, vehicleName: this.translationData.lblAll , registrationNo: this.translationData.lblAll , vin: this.translationData.lblAll  });
      }
      this.resetVehicleFilter();
    }
  }

  onVehicleChange(event: any) {
    this.internalSelection = true;
  }

  onReset() {
    this.internalSelection = false;
    this.setDefaultStartEndTime();
    this.setDefaultTodayDate();
    this.selectedEventMarkers = [];
    this.eventIconMarker = null;
    this.fuelDeviationData = [];
    this.summarySectionData = {};
    this.resetChartData();
    this.vehicleListData = [];
    this.noRecordFound = false;
    this.updateDataSource(this.fuelDeviationData);
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
    this.tableInfoObj = {};
  }

  onSearch() {
    this.selectedEventMarkers = [];
    this.eventIconMarker = null;
    this.summarySectionData = {};
    let _startTime = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    let _vinData: any = [];
    if (parseInt(this.fuelDeviationForm.controls.vehicle.value) == 0) { // all vin data
      _vinData = this.vehicleDD.filter(item => item.vehicleId != 0).map(i => i.vin);
    } else { // single vin data
      _vinData = this.vehicleDD.filter(item => item.vehicleId == parseInt(this.fuelDeviationForm.controls.vehicle.value)).map(i => i.vin);
    }
    if (_vinData.length > 0) {
      this.showLoadingIndicator = true;
      let reportDataObj = {
        startDateTime: _startTime,
        endDateTime: _endTime,
        viNs: _vinData
      }
      this.reportService.getFuelDeviationReportDetails(reportDataObj).subscribe((_fuelDeviationData: any) => {
        ////console.log(_fuelDeviationData);
        if(_fuelDeviationData.data.length == 0) {
          this.noRecordFound = true;
        } else {
          this.noRecordFound = false;
        }
        this.reportService.getFuelDeviationReportCharts(reportDataObj).subscribe((_fuelDeviationChartData: any) => {
          this.hideloader();
          this.resetChartData();
          this.setChartsSection(_fuelDeviationChartData);
          this.setSummarySection(_fuelDeviationData.data);
          this.fuelDeviationData = this.reportMapService.convertFuelDeviationDataBasedOnPref(_fuelDeviationData.data, this.prefDateFormat, this.prefTimeFormat, this.prefUnitFormat, this.prefTimeZone, this.translationData);
          this.setTableInfo();
          this.updateDataSource(this.fuelDeviationData);
        }, (error) => {
          this.hideloader();
          this.resetChartData();
        });
      }, (error) => {
        this.hideloader();
        this.fuelDeviationData = [];
        this.tableInfoObj = {};
        this.summarySectionData = {};
        this.noRecordFound = true;
        this.updateDataSource(this.fuelDeviationData);
      });
    }
  }

  resetChartData() {
    this.fuelDeviationDChartData = [];
    this.fuelDeviationDChartLabels = [];
    this.fuelDeviationPChartData = [];
    this.fuelDeviationPChartLabels = [];
    this.fuelIncLineChartData = [];
    this.fuelDecLineChartData = [];
    this.fuelIncBarChartData = [];
    this.fuelDecBarChartData = [];
    this.fuelIncBarChartLabels = [];
    this.fuelDecBarChartLabels = [];
    this._xIncLine = [];
    this._yIncLine = [];
    this._xDecLine = [];
    this._yDecLine = [];
  }

  setSummarySection(data: any) {
    if (data && data.length > 0) {
      let _totalIncCount: any = data.filter(i => i.fuelEventType == 'I');
      let _totalDecCount: any = data.filter(i => i.fuelEventType == 'D');
      let vinData: any = data.map(i => i.vin);
      let vinCount: any = vinData.filter((value, index, self) => self.indexOf(value) === index);
      this.summarySectionData = {
        totalIncCount: _totalIncCount.length,
        totalDecCount: _totalDecCount.length,
        totalVehCount: vinCount.length
      };
      this.fuelDeviationDChartData = [_totalIncCount.length, _totalDecCount.length];
      this.fuelDeviationDChartLabels = [this.translationData.lblFuelIncreaseEvent, this.translationData.lblFuelDecreaseEvent];
      this.fuelDeviationPChartData = [_totalIncCount.length, _totalDecCount.length];
      this.fuelDeviationPChartLabels = [this.translationData.lblFuelIncreaseEvent, this.translationData.lblFuelDecreaseEvent];
    }
  }

  setChartsSection(_chartData: any) {
    if (_chartData && _chartData.data && _chartData.data.length > 0) {
      _chartData.data.forEach(element => {
        //let _d = this.reportMapService.getStartTime(element.date, this.prefDateFormat, this.prefTimeZone, this.prefTimeZone, false);
        let resultDate = this.datePipe.transform(element.date, 'MM/dd/yyyy');
        this._yIncLine.push({ x: resultDate, y: element.increaseEvent });
        this._yDecLine.push({ x: resultDate, y: element.decreaseEvent });
      });
      this.assignDataToCharts();
    }
  }

  assignDataToCharts() {
    let startDate = Util.getMillisecondsToUTCDate(this.startDateValue, this.prefTimeZone);
    let endDate = Util.getMillisecondsToUTCDate(this.endDateValue, this.prefTimeZone);
    this.fuelDeviationChartLabels = [startDate, endDate];
    if (this.fuelIncLineChartOptions.scales.yAxes.length > 0) {
      this.fuelIncLineChartOptions.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblValuesFuelIncreaseEvents || 'Values(Fuel Increase Events)';
    }
    this.fuelIncLineChartLabels = this.fuelDeviationChartLabels;
    this.fuelDecLineChartLabels = this.fuelDeviationChartLabels;
    this.fuelIncBarChartLabels = this.fuelDeviationChartLabels;
    this.fuelDecBarChartLabels = this.fuelDeviationChartLabels;
    if (this.fuelIncLineChartType == 'line') {
      this.fuelIncLineChartOptions.scales.xAxes = [{
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }];
      this.fuelIncLineChartData = [
        { data: this._yIncLine, label: this.translationData.lblFuelIncreaseEvents },
      ];
    }
    if (this.fuelDecLineChartType == 'line') {
      this.fuelDecLineChartOptions.scales.xAxes = [{
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }];
      this.fuelDecLineChartOptions.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblValuesFuelDecreaseEvents || 'Values(Fuel Decrease Events)';
      this.fuelDecLineChartData = [
        { data: this._yDecLine, label: this.translationData.lblFuelDecreaseEvents },
      ];
    }
    if (this.fuelIncBarChartType == 'bar') {
      this.fuelIncBarChartOptions.scales.xAxes = [{
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }];
      this.fuelIncBarChartOptions.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblValuesFuelIncreaseEvents || 'Values(Fuel Increase Events)';
      this.fuelIncBarChartData = [
        {
          label: this.translationData.lblFuelIncreaseEvents,
          type: 'bar',
          backgroundColor: '#65C3F7',
          hoverBackgroundColor: '#65C3F7',
          yAxesID: "y-axis",
          data: this._yIncLine
        }
      ];
    }
    if (this.fuelDecBarChartType == 'bar') {
      this.fuelDecBarChartOptions.scales.xAxes = [{
        type: 'time',
        time:
        {
          tooltipFormat: this.chartLabelDateFormat,
          unit: 'day',
          stepSize: 1,
          displayFormats: {
            day: this.chartLabelDateFormat,
          },
        }
      }];
      this.fuelDecBarChartOptions.scales.yAxes[0].scaleLabel.labelString = this.translationData.lblValuesFuelDecreaseEvents || 'Values(Fuel Decrease Events)';
      this.fuelDecBarChartData = [
        {
          label: this.translationData.lblFuelDecreaseEvents,
          type: 'bar',
          backgroundColor: '#F4AF85',
          hoverBackgroundColor: '#F4AF85',
          yAxesID: "y-axis",
          data: this._yDecLine
        }
      ];
    }
  }

  setTableInfo() {
    let vehName: any = '';
    let vehGrpName: any = '';
    let vehGrpCount = this.vehicleGrpDD.filter(i => i.vehicleGroupId == parseInt(this.fuelDeviationForm.controls.vehicleGroup.value));
    if (vehGrpCount.length > 0) {
      vehGrpName = vehGrpCount[0].vehicleGroupName;
    }
    let vehCount = this.vehicleDD.filter(i => i.vehicleId == parseInt(this.fuelDeviationForm.controls.vehicle.value));
    if (vehCount.length > 0) {
      vehName = (this.vehVinRegChecker.length > 0 && this.vehVinRegChecker[0].attr == 'vin') ? vehCount[0].vin : (this.vehVinRegChecker[0].attr == 'registrationNo') ? vehCount[0].registrationNo : vehCount[0].vehicleName;
    }
    this.tableInfoObj = {
      fromDate: this.formStartDate(this.startDateValue),
      endDate: this.formStartDate(this.endDateValue),
      vehGroupName: vehGrpName,
      vehicleName: vehName
    }
  }

  formStartDate(date: any) {
    return this.reportMapService.formStartDate(date, this.prefTimeFormat, this.prefDateFormat);
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.showMap = false;
    this.selectedFuelDeviationEntry.clear();
    this.summaryExpandPanel = false;
    this.chartExpandPanel = false;
    if (this.initData.length > 0) {
      this.showChartPanel = true;
      this.showSummaryPanel = true;
      this.summaryExpandPanel = true;
      this.chartExpandPanel = true;
      if (!this.showMapPanel) { //- map panel not shown already
        this.showMapPanel = true;
        setTimeout(() => {
          this.initMap();
        }, 0);
      } else {
        this.clearRoutesFromMap();
      }
    }
    else {
      this.showMapPanel = false;
      this.showChartPanel = false;
      this.showSummaryPanel = false;
    }
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.filterPredicate = function (data: any, filter: string): boolean {
        return (
          data.convertedDifference.toString().toLowerCase().includes(filter) ||
          data.vehicleName.toString().toLowerCase().includes(filter) ||
          data.vin.toString().toLowerCase().includes(filter) ||
          data.registrationNo.toString().toLowerCase().includes(filter) ||
          data.eventDate.toString().toLowerCase().includes(filter) ||
          data.convertedOdometer.toString().toLowerCase().includes(filter) ||
          data.convertedStartDate.toString().toLowerCase().includes(filter) ||
          data.convertedEndDate.toString().toLowerCase().includes(filter) ||
          data.convertedDistance.toString().toLowerCase().includes(filter) ||
          data.convertedIdleDuration.toString().toLowerCase().includes(filter) ||
          data.convertedAverageSpeed.toString().toLowerCase().includes(filter) ||
          data.convertedAverageWeight.toString().toLowerCase().includes(filter) ||
          data.startPosition.toString().toLowerCase().includes(filter) ||
          data.endPosition.toString().toLowerCase().includes(filter) ||
          data.convertedFuelConsumed.toString().toLowerCase().includes(filter) ||
          data.convertedDrivingTime.toString().toLowerCase().includes(filter) ||
          data.alerts.toString().toLowerCase().includes(filter)
        );
      };
    });
  }

  changeStartDateEvent(event: MatDatepickerInputEvent<any>) {
    this.internalSelection = true;
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
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1) + "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.selectedEndTime;
      this.endTimeStart = this.selectedStartTime; 
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
    this.resetFuelDeviationFormControlValue(); // extra addded as per discuss with Atul
    this.filterDateData(); // extra addded as per discuss with Atul
  }

  getStartTimeChanged(time: any){
    this.selectedStartTimeValue = time;
  }

  getEndTimeChanged(time: any){
    this.selectedEndTimeValue = time;
  }

  startTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedStartTime = this.selectedStartTimeValue;
    if (this.prefTimeFormat == 24) {
      this.startTimeDisplay = this.selectedStartTimeValue + ':00';
    }
    else {
      this.startTimeDisplay = this.selectedStartTimeValue;
    }
    this.startDateValue = this.setStartEndDateTime(this.startDateValue, this.selectedStartTime, 'start');
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1) + "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
    this.maxStartTime = this.selectedEndTime;
    this.endTimeStart = this.selectedStartTime; 
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
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
  }

  endTimeChanged(selectedTime: any) {
    this.internalSelection = true;
    this.selectedEndTime = this.selectedEndTimeValue;
    if (this.prefTimeFormat == 24) {
      this.endTimeDisplay = this.selectedEndTimeValue + ':59';
    }
    else {
      this.endTimeDisplay = this.selectedEndTimeValue;
    }
    this.endDateValue = this.setStartEndDateTime(this.endDateValue, this.selectedEndTime, 'end');
    let startDate1 = this.startDateValue.getFullYear() + "/" + (this.startDateValue.getMonth() + 1) + "/" + this.startDateValue.getDate();
    let endDate1 = this.endDateValue.getFullYear() + "/" + (this.endDateValue.getMonth() + 1) + "/" + this.endDateValue.getDate();
    if(startDate1 == endDate1){
      this.maxStartTime = this.selectedEndTime;
      this.endTimeStart = this.selectedStartTime; 
    }
    else{
      this.maxStartTime = this.selectedEndTime;
      if (this.prefTimeFormat == 24) {
        this.maxStartTime = '23:59';
      }
      else{
        this.maxStartTime = '11:59';
      }
      this.endTimeStart = "00:00";
    }
    this.resetFuelDeviationFormControlValue();
    this.filterDateData();
  }

  onSearchFocus() {
    this.searchStr = null;
  }

  onSearchSelected(selectedAddress: CompleterItem) {
    if (selectedAddress) {
      let id = selectedAddress["originalObject"]["id"];
      let qParam = 'apiKey=' + this.map_key + '&id=' + id;
      this.hereService.lookUpSuggestion(qParam).subscribe((data: any) => {
        this.searchMarker = {};
        if (data && data.position && data.position.lat && data.position.lng) {
          let searchMarker = {
            lat: data.position.lat,
            lng: data.position.lng,
            from: 'search'
          }
          this.setMapToLocation(searchMarker);
        }
      });
    }
  }

  setMapToLocation(_position: any) {
    this.hereMap.setCenter({ lat: _position.lat, lng: _position.lng }, 'default');
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getAllSummaryData() {
    this.excelSummaryData = [
      [this.translationData.lblFuelDeviationReport || 'Fuel Deviation Report', this.reportMapService.getStartTime(Date.now(), this.prefDateFormat, this.prefTimeFormat, this.prefTimeZone, true), this.tableInfoObj.fromDate, this.tableInfoObj.endDate,
      this.tableInfoObj.vehGroupName, this.tableInfoObj.vehicleName, this.summarySectionData.totalIncCount,
      this.summarySectionData.totalDecCount, this.summarySectionData.totalVehCount
      ]
    ];
  }

  getPDFExcelHeader() {
    let col: any = [];
    let unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm) : (this.translationData.lblmile);
    let unitValkmh = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkmh) : (this.translationData.lblmph);
    let unitValton = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblton) : (this.translationData.lblton);
    let unitValgallon = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblltr) : (this.translationData.lblgallon);
    col = [`${this.translationData.lblType}`, `${this.translationData.lblDifference} (%)`, `${this.translationData.lblVehicleName}`, `${this.translationData.lblVIN}`, `${this.translationData.lblRegPlateNumber}`, `${this.translationData.lblDate}`, `${this.translationData.lblOdometer} (${unitValkm})`, `${this.translationData.lblStartDate}`, `${this.translationData.lblEndDate}`, `${this.translationData.lblDistance} (${unitValkm})`, `${this.translationData.lblIdleDuration} (${this.translationData.lblhhmm})`, `${this.translationData.lblAverageSpeed} (${unitValkmh})`, `${this.translationData.lblAverageWeight || 'Average Weight'} (${unitValton})`, `${this.translationData.lblStartPosition}`, `${this.translationData.lblEndPosition}`, `${this.translationData.lblFuelConsumed || 'Fuel Consumed'} (${unitValgallon})`, `${this.translationData.lblDrivingTime} (${this.translationData.lblhhmm})`, `${this.translationData.lblAlerts}`];
    return col;
  }

  exportAsExcelFile() {
    this.getAllSummaryData();
    const title = this.translationData.lblFuelDeviationReport;
    const summary = this.translationData.lblSummarySection;
    const detail = this.translationData.lblDetailSection;
    const header = this.getPDFExcelHeader();
    const summaryHeader = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime || 'Report Start Time'}`, `${this.translationData.lblReportEndTime || 'Report End Time'}`, `${this.translationData.lblVehicleGroup}`, (this.vehVinRegChecker.length > 0 && this.vehVinRegChecker[0].attr == 'vin') ? (this.translationData.lblVIN) : (this.vehVinRegChecker[0].attr == 'registrationNo') ? (this.translationData.lblRegPlateNumber) : (this.translationData.lblVehicle), `${this.translationData.lblFuelIncreaseEvents}`, `${this.translationData.lblFuelDecreaseEvents}`, `${this.translationData.lblVehiclesWithFuelEvents}`];
    const summaryData = this.excelSummaryData;
    let workbook = new Workbook();
    let worksheet = workbook.addWorksheet('Fuel Deviation Report');
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
      worksheet.addRow([item.eventTooltip, item.convertedDifference, item.vehicleName, item.vin,
      item.registrationNo, item.eventDate, item.convertedOdometer, item.convertedStartDate,
      item.convertedEndDate, item.convertedDistance, item.convertedIdleDuration,
      item.convertedAverageSpeed, item.convertedAverageWeight, item.startPosition, item.endPosition, item.convertedFuelConsumed,
      item.convertedDrivingTime, item.alerts]);
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
      fs.saveAs(blob, 'Fuel_Deviation_Report.xlsx');
   })
  }

  // getPDFHeaders(){
  //   let displayArray: any = [];
  //   this.displayedColumns.forEach(i => {
  //     let _s = this.prefMapData.filter(item => item.value == i);
  //     if (_s.length > 0){
  //       displayArray.push(this.translationData[_s[0].key] ? this.translationData[_s[0].key] : _s[0].value);
  //     }
  //   })
  //   return [displayArray];
  // }

  exportAsPDFFile(){
  //var doc = new jsPDF('p', 'mm', 'a4');

  var imgleft;
  if (this.brandimagePath != null) {
    imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
  } else {
    imgleft = "/assets/Daf-NewLogo.png";
    // let defaultIcon: any = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
    // let sanitizedData: any= this.domSanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
    // imgleft = sanitizedData.changingThisBreaksApplicationSecurity;
  }  

  var doc = new jsPDF('p', 'mm', 'a4');
  let pdfColumns = this.getPDFExcelHeader(); // this.getPDFHeaders()
  let tranHeaderNamePdf = this.translationData.lblFuelDeviationDetails;
  let prepare = []
    this.initData.forEach(e=>{
      var tempObj = [];
      this.pdfDisplayedColumns.forEach(element => {
        switch (element) {
          case 'fuelEventType': {
            tempObj.push(e.eventTooltip);
            break;
          }
          case 'convertedDifference': {
            tempObj.push(e.convertedDifference);
            break;
          }
          case 'vehicleName': {
            tempObj.push(e.vehicleName);
            break;
          }
          case 'vin': {
            tempObj.push(e.vin);
            break;
          }
          case 'registrationNo': {
            tempObj.push(e.registrationNo);
            break;
          }
          case 'eventTime': {
            tempObj.push(e.eventDate);
            break;
          }
          case 'odometer': {
            tempObj.push(e.convertedOdometer);
            break;
          }
          case 'startTimeStamp': {
            tempObj.push(e.convertedStartDate);
            break;
          }
          case 'endTimeStamp': {
            tempObj.push(e.convertedEndDate);
            break;
          }
          case 'distance': {
            tempObj.push(e.convertedDistance);
            break;
          }
          case 'idleDuration': {
            tempObj.push(e.convertedIdleDuration);
            break;
          }
          case 'averageSpeed': {
            tempObj.push(e.convertedAverageSpeed);
            break;
          }
          case 'averageWeight': {
            tempObj.push(e.convertedAverageWeight);
            break;
          }
          case 'startPosition': {
            tempObj.push(e.startPosition);
            break;
          }
          case 'endPosition': {
            tempObj.push(e.endPosition);
            break;
          }
          case 'fuelConsumed': {
            tempObj.push(e.convertedFuelConsumed);
            break;
          }
          case 'drivingTime': {
            tempObj.push(e.convertedDrivingTime);
            break;
          }
          case 'alerts': {
            tempObj.push(e.alerts);
            break;
          }
        }
      })
      prepare.push(tempObj);
    });
    let DATA: any;
    if (this.chartExpandPanel) { // charts expand
      DATA = document.getElementById('fuelSummaryCharts'); // summary + charts
    } else {
      DATA = document.getElementById('fuelSummary'); // only summary
    }
    html2canvas(DATA)
      .then(canvas => {
        (doc as any).autoTable({
          styles: {
            cellPadding: 0.5,
            fontSize: 12
          },
          didDrawPage: function (data) {
            doc.setFontSize(14);
            var fileTitle = tranHeaderNamePdf;
            // var img = "/assets/logo.png";
            // doc.addImage(img, 'JPEG', 10, 10, 0, 0);
            doc.addImage(imgleft, 'JPEG', 10, 10, 0, 16);

            var img = "/assets/logo_daf.png";
            doc.text(fileTitle, 14, 35);
            doc.addImage(img, 'JPEG', 150, 10, 0, 10);
          },
          margin: {
            bottom: 20,
            top: 30
          }
        });
        let fileWidth = 170;
        let fileHeight = canvas.height * fileWidth / canvas.width;
        const FILEURI = canvas.toDataURL('image/png')
        doc.addImage(FILEURI, 'PNG', 10, 40, fileWidth, fileHeight);
        doc.addPage('a1', 'p');
        (doc as any).autoTable({
          head: [pdfColumns],
          body: prepare,
          theme: 'striped',
          didDrawCell: data => {
          }
        })
        doc.save('FuelDeviationReport.pdf');
      });
  }

  drawEventMarkersOnMap(markerData: any) {
    this.clearRoutesFromMap();
    markerData.forEach(element => {
      let markerPositionLat = element.eventLatitude;
      let markerPositionLng = element.eventLongitude;
      let eventIcon = this.getEventIcons(element);
      let markerSize = { w: 22, h: 22 };
      let icon = new H.map.Icon(eventIcon, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      this.eventIconMarker = new H.map.Marker({ lat: markerPositionLat, lng: markerPositionLng }, { icon: icon });
      this.mapGroup.addObject(this.eventIconMarker);
      let eventDescText: any = this.reportMapService.getEventTooltip(element, this.translationData);
      let iconInfoBubble: any;
      this.eventIconMarker.addEventListener('pointerenter', (evt) => {
        iconInfoBubble = new H.ui.InfoBubble(evt.target.getGeometry(), {
          // read custom data
          content: `<table style='width: 300px;' class='font-14-px line-height-21px font-helvetica-lt'>
            <tr>
              <td style='width: 100px;'>${this.translationData.lblDate}:</td> <td class='font-helvetica-md'>${element.eventDate}</td>
            </tr>
            <tr>
              <td style='width: 100px;'>${(this.vehVinRegChecker.length > 0 && this.vehVinRegChecker[0].attr == 'vin') ? (this.translationData.lblVIN) : (this.vehVinRegChecker[0].attr == 'registrationNo') ? (this.translationData.lblRegPlateNumber) : (this.translationData.lblVehicleName)}:</td> <td class='font-helvetica-md'>${element[this.vehVinRegChecker[0].attr]}</td>
            </tr>
            <tr>
              <td style='width: 100px;'>${this.translationData.lblPosition}:</td> <td class='font-helvetica-md'>${element.geoLocationAddress}</td>
            </tr>
            <tr>
              <td style='width: 100px;'>${this.translationData.lblEventDescription}:</td> <td class='font-helvetica-md'>${eventDescText.eventText}</td>
            </tr>
            <tr>
              <td style='width: 100px;'>${this.translationData.lblDifference}:</td> <td class='font-helvetica-md'>${element.convertedDifference}%</td>
            </tr>
          </table>`
        });
        this.ui.addBubble(iconInfoBubble); // show info bubble
      }, false);
      this.eventIconMarker.addEventListener('pointerleave', (evt) => {
        iconInfoBubble.close(); // hide info bubble
      }, false);
    });
    this.hereMap.addObject(this.mapGroup);
    this.hereMap.getViewModel().setLookAtData({
      bounds: this.mapGroup.getBoundingBox()
    });
  }

  getEventIcons(eventElement: any) {
    let icon: any = '';
    let colorCode: any = (eventElement.fuelEventType == 'I') ? '#00AE10' : '#D50017';
    switch (eventElement.fuelEventType) {
      case 'I': { // increase
        icon = `<svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
        <circle cx="11" cy="11" r="11" fill="${colorCode}"/>
        <path d="M6.54448 6.74266C5.3089 6.80137 5 7.78474 5 8.26908V16.6057C5 17.6155 5.94055 17.956 6.41082 18H15.5886C16.7529 18 17.0143 17.0117 16.9994 16.5176V4.35029C16.9875 3.36399 16.0935 3.03914 15.648 3H10.1829C9.17306 3.01174 8.82159 3.97358 8.77209 4.45303V4.90802C8.77209 5.61252 8.08896 6.66928 6.54448 6.74266Z" fill="white"/>
        <rect x="5" y="5.34229" width="3.31245" height="1.90539" rx="0.952697" transform="rotate(-45 5 5.34229)" fill="white"/>
        <rect x="11" y="5" width="4" height="1" rx="0.5" fill="${colorCode}"/>
        <path d="M7.27959 14.719L10.9497 8.48971L14.6199 14.719L10.9497 13.4237L7.27959 14.719Z" fill="${colorCode}"/>
        </svg>`;
        break;
      }
      case 'D': { // decrease
        icon = `<svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
        <circle cx="11" cy="11" r="11" fill="${colorCode}"/>
        <path d="M6.54448 6.74266C5.3089 6.80137 5 7.78474 5 8.26908V16.6057C5 17.6155 5.94055 17.956 6.41082 18H15.5886C16.7529 18 17.0143 17.0117 16.9994 16.5176V4.35029C16.9875 3.36399 16.0935 3.03914 15.648 3H10.1829C9.17306 3.01174 8.82159 3.97358 8.77209 4.45303V4.90802C8.77209 5.61252 8.08896 6.66928 6.54448 6.74266Z" fill="white"/>
        <rect x="5" y="5.34229" width="3.31245" height="1.90539" rx="0.952697" transform="rotate(-45 5 5.34229)" fill="white"/>
        <rect x="11" y="5" width="4" height="1" rx="0.5" fill="${colorCode}"/>
        <path d="M14.6198 10.1599L10.9497 16.3892L7.27951 10.1599L10.9497 11.4552L14.6198 10.1599Z" fill="${colorCode}"/>
        </svg>`;
        break;
      }
    }
    return icon;
  }

  masterToggleForFuelDeviationEntry() {
    this.selectedEventMarkers = [];
    if (this.isAllSelectedForFuelEntry()) { // remove all event markers
      this.selectedFuelDeviationEntry.clear();
      this.showMap = false;
      this.drawEventMarkersOnMap(this.selectedEventMarkers);
    }
    else { // add all event markers
      this.dataSource.data.forEach((row) => {
        this.selectedFuelDeviationEntry.select(row);
        this.selectedEventMarkers.push(row);
      });
      this.showMap = true;
      this.drawEventMarkersOnMap(this.selectedEventMarkers);
    }
  }

  isAllSelectedForFuelEntry() {
    const numSelected = this.selectedFuelDeviationEntry.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForFuelEntry(row?: any): string {
    if (row)
      return `${this.isAllSelectedForFuelEntry() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedFuelDeviationEntry.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  pageSizeUpdated(_event) { }

  fuelEntryCheckboxClicked(event: any, row: any) {
    this.showMap = this.selectedFuelDeviationEntry.selected.length > 0 ? true : false;
    if (event.checked) { // add event marker
      this.selectedEventMarkers.push(row);
      this.drawEventMarkersOnMap(this.selectedEventMarkers);
    }
    else { // remove event marker
      let _arr = this.selectedEventMarkers.filter(item => item.id != row.id);
      this.selectedEventMarkers = _arr.slice();
      this.drawEventMarkersOnMap(this.selectedEventMarkers);
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

  resetVehicleGroupFilter() {
    this.filteredVehicleGroups.next(this.vehicleGrpDD.slice());
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

}