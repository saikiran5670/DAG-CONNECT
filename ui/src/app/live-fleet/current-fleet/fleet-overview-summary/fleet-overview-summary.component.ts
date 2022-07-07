import { Component, OnInit, OnDestroy, Input, SimpleChanges, ChangeDetectorRef, ViewChild } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet } from 'ng2-charts';
import { Subscription } from 'rxjs';
import { MessageService } from 'src/app/services/message.service';
import { ReportService } from 'src/app/services/report.service';
import { Util } from 'src/app/shared/util';
import { FleetMapService } from '../fleet-map.service';
import { OrganizationService } from '../../../services/organization.service';
import { TranslationService } from '../../../services/translation.service';
import { ReportMapService } from 'src/app/report/report-map.service';
import { DataInterchangeService } from 'src/app/services/data-interchange.service';

@Component({
  selector: 'app-fleet-overview-summary',
  templateUrl: './fleet-overview-summary.component.html',
  styleUrls: ['./fleet-overview-summary.component.less']
})
export class FleetOverviewSummaryComponent implements OnInit, OnDestroy {
  @Input() translationData: any = {};
  @Input() detailsData: any = [];
  @Input() filterData: any = {};
  @Input() totalVehicleCount: number;
  @Input() dashboardPref: any;
  fleetSummary: any;
  criticalAlert: number = 0;
  mileageDone: string = '';
  drivers: number = 0;
  driveTime: any = '';
  noAction: number = 0;
  serviceNow: number = 0;
  stopNow: number = 0;
  vehicleGroup: string = '';
  barChartLabels: Label[] = [];
  barChartType: ChartType = 'bar';
  barChartLegend = true;
  barChartPlugins = [];
  lineChartType = 'horizontalBar';
  doughnutChartType: ChartType = 'doughnut';
  messages: any[] = [];
  subscription: Subscription;
  summaryData: any = [];
  movedVehicle: number = 0;
  totalVehicle: number = 0;
  mileageRate: number = 0;
  utilizationRate: number = 0;
  prefTimeZone:any;
  prefTimeFormat:any;
  //preferemces value
  prefUnitFormat: any = 'dunit_Metric';
  unitValkm: string = '';
  filterInvoked: boolean = false;
  showLoadingIndicator: any = false;
  distanceThreshold: number;
  timeThreshold: number;
  totalDriveTime: number = 0;
  prefDetail: any = {};
  @ViewChild('mep') mep: any;
  isSummaryOpened: boolean=false;
  currentData: any;
  todayData: any;

  constructor(private messageService: MessageService, private reportService: ReportService, private fleetMapService: FleetMapService, private organizationService: OrganizationService, private translationService: TranslationService, private cdref: ChangeDetectorRef, private reportMapService: ReportMapService, private dataService: DataInterchangeService) {
      
    this.dataService.fleetOverViewSource$.subscribe(data => {
        let data$=JSON.parse(JSON.stringify(data));
        this.currentData = data$;
        if(this.mep && this.mep.expanded){
          this.loadSummaryPartTwo(data$, false);
        }
      });
    this.dataService.fleetOverViewSourceToday$.subscribe(data => {
      let data$=JSON.parse(JSON.stringify(data));
      this.todayData=data$;
      if(this.mep && this.mep.expanded){
        this.currentData = data$;
        this.getSummary();
      } 
    });
  }

  getSummary(){
    let selectedStartTime = '';
    let selectedEndTime = '';
    if(this.prefTimeFormat == 24){
      selectedStartTime = "00:00";
      selectedEndTime = "23:59";
    } else{      
      selectedStartTime = "12:00 AM";
      selectedEndTime = "11:59 PM";
    }
    let startDateValue = this.setStartEndDateTime(Util.getUTCDate(this.prefTimeZone), selectedStartTime, 'start');
    let endDateValue = this.setStartEndDateTime(Util.getUTCDate(this.prefTimeZone), selectedEndTime, 'end');
    let _startTime = Util.getMillisecondsToUTCDate(startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(endDateValue, this.prefTimeZone);
    let vinSet = new Set();
    let fleetDetailToday = JSON.parse(JSON.stringify(this.dataService.fleetOverViewDetailDataToday));
    if(fleetDetailToday && fleetDetailToday.fleetOverviewDetailList && fleetDetailToday.fleetOverviewDetailList.length > 0){
      fleetDetailToday.fleetOverviewDetailList.forEach(element => {
        vinSet.add(element.vin);
      });
    }
    //API call for summary
    let objData = {
      "viNs": Array.from(vinSet),        
      "startDateTime": _startTime,        
      "endDateTime": _endTime        
    }
    
    // if(vinSet && vinSet.size > 0){
      this.showLoadingIndicator=true;
      this.reportService.getFleetOverviewSummary(objData).subscribe((summary: any) => {
        if(summary) {
          this.resetSummaryPartOne();
          this.fleetSummary = summary;
        }
        if(this.currentData){
          this.totalVehicle = this.currentData.visibleVinsCount;
          this.stepForword(this.currentData.fleetOverviewDetailList, true);
        }
        this.showLoadingIndicator=false;
      }, (error) => {
        this.loadSummaryPartTwo(this.currentData, false);
        this.showLoadingIndicator=false;
        this.resetSummaryPartOne();
      });
    // }

  }

  panelClick(event: any){
    this.getSummary();
    this.isSummaryOpened=true;
  }

  ngAfterViewInit() {
    this.cdref.detectChanges();
  }

  ngOnDestroy() {
    if(this.subscription)
      this.subscription.unsubscribe();
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes && changes.fleetSummary){
      this.fleetSummary = changes.fleetSummary.currentValue;
    }
    if (changes && changes.detailsData && changes.detailsData.currentValue) {
      this.detailsData = changes.detailsData.currentValue;     
      this.stepForword(this.detailsData, true);
    }
    if (changes && changes.totalVehicleCount) {
      // this.filterData = changes.filterData.currentValue;
      this.totalVehicle = Number(changes.totalVehicleCount.currentValue);
      this.updateVehicleGraph();
    }
  } 

  updateVehicleGraph() {
    this.barChartData = [
      { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
    ];
  }

  ngOnInit() {
    let accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    if(this.prefDetail){
      if (accountPrefObj.accountPreference && accountPrefObj.accountPreference != '') { 
        this.proceedStep(accountPrefObj.accountPreference);
      } else { 
        this.organizationService.getOrganizationPreference(accountOrganizationId).subscribe((orgPref: any) => {
          this.proceedStep(orgPref);
        }, (error) => { 
          this.proceedStep({});
        });
      }
    }
  }

  proceedStep(preference: any) {
    let _search = this.prefDetail.timeformat.filter(i => i.id == preference.timeFormatId); 
    if (_search.length > 0) {
      this.prefUnitFormat = this.prefDetail.unit.filter(i => i.id == preference.unitId)[0].name;
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = this.prefDetail.timezone.filter(i => i.id == preference.timezoneId)[0].name;
    } else {
      this.prefUnitFormat = this.prefDetail.unit[0].name;
      this.prefTimeZone = this.prefDetail.timezone[0].name;
      this.prefTimeFormat = Number(this.prefDetail.timeformat[0].name.split("_")[1].substring(0,2));    
    }
    this.unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm) : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile);
    this.setFleetThreshold();
  }

  loadSummaryPartTwo(data$: any, isToday: boolean){
    if(data$){
      // this.fleetSummary = data$.fleetOverviewSummary;
      this.totalVehicle = data$.visibleVinsCount;
      this.stepForword(data$.fleetOverviewDetailList, isToday);
    }
  }

  loadData() {
    let localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.showLoadingIndicator = true;
    let selectedStartTime = '';
    let selectedEndTime = '';
    if(this.prefTimeFormat == 24){
      selectedStartTime = "00:00";
      selectedEndTime = "23:59";
    } else{      
      selectedStartTime = "12:00 AM";
      selectedEndTime = "11:59 PM";
    }
    let startDateValue = this.setStartEndDateTime(Util.getUTCDate(this.prefTimeZone), selectedStartTime, 'start');
    let endDateValue = this.setStartEndDateTime(Util.getUTCDate(this.prefTimeZone), selectedEndTime, 'end');
    let _startTime = Util.getMillisecondsToUTCDate(startDateValue, this.prefTimeZone);
    let _endTime = Util.getMillisecondsToUTCDate(endDateValue, this.prefTimeZone);
    let objData = {
      "groupId": ['all'],
      "alertLevel": ['all'],
      "alertCategory": ['all'],
      "healthStatus": ['all'],
      "otherFilter": ['all'],
      "driverId": ['all'],
      "days": 0,
      "languagecode": localStLanguage.code,
      "StartDateTime":_startTime,
      "EndDateTime":_endTime
    }
    this.reportService.getFleetOverviewDetails(objData).subscribe((data: any) => {
      this.totalVehicle = data.visibleVinsCount;
      //let filterData = this.fleetMapService.processedLiveFLeetData(data.fleetOverviewDetailList);
      this.stepForword(data.fleetOverviewDetailList);
      this.hideloader();
    }, (error) => {
      this.resetSummary();
      this.hideloader();
    });
  }
  setStartEndDateTime(date: any, timeObj: any, type: any){   
    return this.reportMapService.setStartEndDateTime(date, timeObj, type, this.prefTimeFormat);
  }

  stepForword(filterData: any, flag?: boolean) {
    if (filterData && filterData.length > 0) {
      this.summaryData = filterData;
      // this.unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
      this.refreshData(flag);
      this.barChartLabels = [this.translationData.lblMovedVehicle, this.translationData.lblTotalVehicle];
      this.doughnutChartLabelsMileage = [(this.translationData.lblFleetMileageRate), ''];
      this.doughnutChartLabelsUtil = [(this.translationData.lblFleetUtilizationRate), '', ''];
    } else {
      if (flag) {
        this.criticalAlert = 0;
        this.mileageDone = '00 ' + this.unitValkm;
        this.driveTime = '00' + (this.translationData.lblhh) + ' 00' + (this.translationData.lblmm);
        this.drivers = 0;
      }
      this.resetSummary();
    }
    if (this.totalVehicle) {
      this.barChartLabels = [this.translationData.lblMovedVehicle, this.translationData.lblTotalVehicle];
      this.barChartData = [
        { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
      ];
    }
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  activeObj: number;
  setFleetThreshold() {
    if (this.dashboardPref && this.dashboardPref.subReportUserPreferences && this.dashboardPref.subReportUserPreferences.length > 0) {
      let userPref = this.dashboardPref.subReportUserPreferences.find(x => x.key == "rp_db_dashboard_vehicleutilization");
      if (userPref && userPref.subReportUserPreferences.length > 0) {
        let timeObj = userPref.subReportUserPreferences.find(x => x.key == "rp_db_dashboard_vehicleutilization_timebasedutilizationrate");
        let distanceObj = userPref.subReportUserPreferences.find(x => x.key == "rp_db_dashboard_vehicleutilization_distancebasedutilizationrate");
        if (distanceObj)
          this.distanceThreshold = this.reportMapService.convertDistanceUnits(distanceObj.thresholdValue, this.prefUnitFormat);
        if (timeObj)
          this.timeThreshold = timeObj.thresholdValue;
      }
    }
  }

  barChartOptions: ChartOptions = {
    responsive: true,
    // We use these empty structures as placeholders for dynamic theming.
    scales: {
      xAxes: [{
        ticks: {
          beginAtZero: true,
          stepSize: 1
        },
        gridLines: {
          display: true,
          drawBorder: true,
          offsetGridLines: false
        }
      }], yAxes: [{
        gridLines: {
          display: true,
          drawBorder: true,
          offsetGridLines: false
        }
      }]
    },
    plugins: {
      datalabels: {
        anchor: 'end',
        align: 'end',
      }
    },
    legend: {
      display: false
    },
  };

  barChartData: ChartDataSets[] = [
    { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
  ];

  barChartColors: Color[] = [
    {
      backgroundColor: ["#75c3f0", "#168cd0"]
    }
  ];

  doughnutColors: Color[] = [
    {
      backgroundColor: [
        "#75c3f0",
        "#AAAAAA"
      ],
      hoverBackgroundColor: [
        "#75c3f0",
        "#AAAAAA"
      ],
      hoverBorderColor: [
        "#75c3f0",
        "#ffffff"
      ]
    }
  ];
  // Doughnut - Fleet Mileage Rate
  doughnutChartLabelsMileage: Label[] = [];
  doughnutChartDataMileage: MultiDataSet = [[0, 0]];
  doughnutChartOptionsMileage: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 70,
    tooltips: {
      filter: function (item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  // Doughnut - Fleet Utilization Rate
  doughnutChartLabelsUtil: Label[] = [];
  doughnutChartDataUtil: MultiDataSet = [[0, 0]];

  doughnutChartOptionsUtil: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 70,
    tooltips: {
      filter: function (item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  refreshData(flag?: boolean) {
    let tripDistance = 0;
    this.noAction = 0;
    this.serviceNow = 0;
    this.stopNow = 0;

      if (flag) {
        this.movedVehicle=0;
        if(this.fleetSummary){
        //drivers count
          this.drivers = this.fleetSummary.driverCount;
          this.criticalAlert = this.fleetSummary.criticalAlertCount;
          this.totalDriveTime = this.fleetSummary.drivingTime;
          if(this.todayData && this.todayData.fleetOverviewDetailList){
            this.todayData.fleetOverviewDetailList.forEach(element => {
              if (element.vehicleDrivingStatusType && element.vehicleDrivingStatusType != 'N') {
                this.movedVehicle += 1;
              }
            });
          }
        }
      }
      tripDistance = this.fleetSummary? this.fleetSummary.drivingDistance :0;
      if (this.summaryData) {
        
        this.summaryData.forEach(element => {
            // if (flag && (element.vehicleDrivingStatusType && element.vehicleDrivingStatusType != 'N')) {
            //   this.movedVehicle += 1;
            // }

          if (element.vehicleHealthStatusType) {
            if (element.vehicleHealthStatusType === 'N') {
              this.noAction += 1;
            } else if (element.vehicleHealthStatusType === 'V') {
              this.serviceNow += 1;
            } else if (element.vehicleHealthStatusType === 'T') {
              this.stopNow += 1;
            }
          }
        });
      }
  
    let milDone: any = this.getDistance(tripDistance, this.prefUnitFormat);
    this.mileageDone = milDone + ' ' + this.unitValkm;
    let totDriveTime = Util.getHhMmTime(this.totalDriveTime).split(':'); //driving time is coming in seconds
    this.driveTime = flag ? totDriveTime[0] + (this.translationData.lblhh ) + ' ' +totDriveTime[1] + (this.translationData.lblmm) : this.driveTime;

    this.barChartData = [
      { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
    ];

    this.doughnutChartDataMileage = [[0, 0]];
    if(this.distanceThreshold) this.doughnutChartDataMileage = [[0, 100]];
    if (milDone && this.distanceThreshold) {
      this.mileageRate = Number.parseFloat(((Number.parseFloat(milDone) / this.distanceThreshold) * 100).toFixed(2));
      let thresholdLeft = (100 - this.mileageRate > 0) ? 100 - this.mileageRate : 0;
      this.doughnutChartDataMileage = [[this.mileageRate, thresholdLeft]];
    }
    //Fleet Utilization rate
    this.doughnutChartDataUtil = [[0, 0]];
    if(this.timeThreshold) this.doughnutChartDataUtil = [[0, 100]];
    if (this.totalDriveTime && this.timeThreshold) {
      this.utilizationRate = Number(((this.totalDriveTime / (this.timeThreshold/1000)) * 100).toFixed(2));
      let thresholdLeft = (100 - this.utilizationRate > 0) ? 100 - this.utilizationRate : 0;
      this.doughnutChartDataUtil = [[this.utilizationRate, thresholdLeft]];
    }
  }

  resetSummary() {
    this.noAction = 0;
    this.serviceNow = 0;
    this.stopNow = 0;
    this.doughnutChartDataMileage = [[0, 0]];
    this.doughnutChartDataUtil = [[0, 0]];
  }

  resetSummaryPartOne(){
    this.criticalAlert = 0;
    this.mileageDone = '00 ' + this.unitValkm;
    this.driveTime = '00' + (this.translationData.lblhh) + ' 00' + (this.translationData.lblmm);
    this.drivers = 0;
  }

  getDistance(distance: any, unitFormat: any) {
    // distance in meter
    let _distance: any = 0;
    switch (unitFormat) {
      case 'dunit_Metric': {
        _distance = (distance / 1000).toFixed(2); //-- km
        break;
      }
      case 'dunit_Imperial':
      case 'dunit_USImperial': {
        _distance = (distance / 1609.344).toFixed(2); //-- mile
        break;
      }
      default: {
        _distance = distance.toFixed(2);
      }
    }
    return _distance;
  }
}
