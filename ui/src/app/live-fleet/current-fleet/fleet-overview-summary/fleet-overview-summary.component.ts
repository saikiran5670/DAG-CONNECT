import { Component, OnInit,Input, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { Color, Label, MultiDataSet } from 'ng2-charts';
import { Subscription } from 'rxjs';
import { MessageService } from 'src/app/services/message.service';
import { ReportService } from 'src/app/services/report.service';
import { Util } from 'src/app/shared/util';
import { FleetMapService } from '../fleet-map.service';
import { OrganizationService } from '../../../services/organization.service';
import { TranslationService } from '../../../services/translation.service';

@Component({
  selector: 'app-fleet-overview-summary',
  templateUrl: './fleet-overview-summary.component.html',
  styleUrls: ['./fleet-overview-summary.component.less']
})
export class FleetOverviewSummaryComponent implements OnInit {
  @Input() translationData: any = {};
  @Input() detailsData: any = [];
  @Input() filterData: any = {};
  @Input() totalVehicleCount: number;
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
  summaryData: any=[];
  movedVehicle: number = 0;
  totalVehicle: number = 0;
  mileageRate: number = 0;
  utilizationRate: number = 0;
  //preferemces value
  prefUnitFormat: any = 'dunit_Metric';
  unitValkm: string = '';
  filterInvoked: boolean = false;
  showLoadingIndicator: any = false;

  constructor(private messageService: MessageService, private reportService: ReportService, private fleetMapService: FleetMapService, private organizationService: OrganizationService, private translationService: TranslationService, private cdref: ChangeDetectorRef,) {
    //this.loadData();
    this.subscription = this.messageService.getMessage().subscribe(message => {
      if (message.key.indexOf("refreshData") < 0 && message.key.indexOf("refreshTimer") < 0) {
        this.filterInvoked = true;
        this.vehicleGroup = message.key[0].vehicleGroup;
        if(message.key[0].vehicleGroup && message.key[0].vehicleGroup === 'all')
          this.vehicleGroup +=' Groups';
        if(JSON.stringify(message.key[0].data).indexOf("HttpErrorResponse") !== -1 || JSON.stringify(message.key[0].data).indexOf("No Result Found") !== -1){
          this.resetSummary();
        } else {
          this.summaryData = message.key[0].data;
          //this.refreshData();
          this.stepForword(this.summaryData);
        }
      } else if (!this.filterInvoked && message.key.indexOf("refreshData") !== -1){
        this.loadData();
      }
    });
  }

  ngAfterViewInit(){
    this.cdref.detectChanges();
  }

  ngOnChanges(changes: SimpleChanges) {
    if(changes && changes.detailsData && changes.detailsData.currentValue){
      this.detailsData = changes.detailsData.currentValue;
      this.stepForword(this.detailsData, true);
    }
    if(changes && changes.totalVehicleCount){
      // this.filterData = changes.filterData.currentValue;
      this.totalVehicle = Number(changes.totalVehicleCount.currentValue);
      this.updateVehicleGraph();
    }

  } 
  updateVehicleGraph(){
    // let distinctVIN = [];
    // if(this.filterData && this.filterData.vehicleGroups && this.filterData.vehicleGroups.length > 0){
    //  let vehIds = this.filterData.vehicleGroups.map(i => i.vehicleId);
    //  if(vehIds && vehIds.length > 0){
    //   distinctVIN = vehIds.filter((value, index, self) => self.indexOf(value) === index);
    //  }
    // }
    // this.totalVehicle = distinctVIN.length;
    this.barChartData = [
      { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
    ];
  } 

  ngOnInit() {
    let localStLanguage = JSON.parse(localStorage.getItem("language"));
    let accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    //let accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    let accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.translationService.getPreferences(localStLanguage.code).subscribe((prefData: any) => {
        if(accountPrefObj.accountPreference && accountPrefObj.accountPreference != ''){ // account pref
          this.proceedStep(prefData, accountPrefObj.accountPreference);
        }else{ // org pref
          this.organizationService.getOrganizationPreference(accountOrganizationId).subscribe((orgPref: any) => {
            this.proceedStep(prefData, orgPref);
          }, (error) => { // failed org API
            let pref: any = {};
            this.proceedStep(prefData, pref);
          });
        }
      });
  }

  proceedStep(prefData: any, preference: any){
    let _search = prefData.unit.filter(i => i.id == preference.unitId);
    if(_search.length > 0){
     this.prefUnitFormat = prefData.unit.filter(i => i.id == preference.unitId)[0].name;  
    }else{
      this.prefUnitFormat = prefData.unit[0].name;
    }
    this.unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm) : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile);
    this.stepForword(this.detailsData,true);
  }

  loadData(){
    let localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.showLoadingIndicator = true;
    let objData = {
      "groupId": ['all'],
      "alertLevel": ['all'],
      "alertCategory": ['all'],
      "healthStatus": ['all'],
      "otherFilter": ['all'],
      "driverId": ['all'],
      "days": 0,
      "languagecode": localStLanguage.code
    }
    this.reportService.getFleetOverviewDetails(objData).subscribe((data: any) => {
     this.totalVehicle = data.visibleVinsCount;
      let filterData = this.fleetMapService.processedLiveFLeetData(data.fleetOverviewDetailList);
      this.stepForword(filterData);
      //this.summaryData = filterData;
      // this.unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm ) : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile) : (this.translationData.lblmile);
      // this.refreshData();
      // this.barChartLabels = [this.translationData.lblMovedVehicle, this.translationData.lblTotalVehicle];
      // this.doughnutChartLabelsMileage = [(this.translationData.lblFleetMileageRate ), ''];
      // this.doughnutChartLabelsUtil = [(this.translationData.lblFleetUtilizationRate), '', ''];
      this.hideloader();
    }, (error) => {
      this.resetSummary();
      this.hideloader();
    });
  }

  stepForword(filterData: any, flag?: boolean){
    if(filterData && filterData.length > 0){
      this.summaryData = filterData;
      // this.unitValkm = (this.prefUnitFormat == 'dunit_Metric') ? (this.translationData.lblkm || 'km') : (this.prefUnitFormat == 'dunit_Imperial') ? (this.translationData.lblmile || 'mile') : (this.translationData.lblmile || 'mile');
      this.refreshData(flag);
      this.barChartLabels = [this.translationData.lblMovedVehicle, this.translationData.lblTotalVehicle];
      this.doughnutChartLabelsMileage = [(this.translationData.lblFleetMileageRate), ''];
      this.doughnutChartLabelsUtil = [(this.translationData.lblFleetUtilizationRate), '', ''];
    }else{
      if(flag){
      this.criticalAlert = 0;
      this.mileageDone = '00 ' + this.unitValkm;
      this.driveTime = '00' + (this.translationData.lblhh) + ' 00' + (this.translationData.lblmm);
      this.drivers = 0;
      }
      this.resetSummary();
    }
    if(this.totalVehicle){
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

  barChartOptions: ChartOptions = {
    responsive: true,
    // We use these empty structures as placeholders for dynamic theming.
    scales: { xAxes: [{
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
    }] },
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

  // // events
  // chartClicked({ event, active }: { event: MouseEvent, active: {}[] }): void {
  //   //console.log(event, active);
  // }

  // chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
  //   //console.log(event, active);
  // }

  barChartColors: Color[] = [
    {
      backgroundColor: [ "#75c3f0", "#168cd0" ]
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
   doughnutChartDataMileage: MultiDataSet = [ [0, 0] ];
   doughnutChartOptionsMileage: ChartOptions = {
    responsive: true,
    legend: {
      display: false
    },
    cutoutPercentage: 70,
    tooltips: {
      filter: function(item, data) {
        var label = data.labels[item.index];
        if (label) return true;
        return false;
      }
    }
  };

  // Doughnut - Fleet Utilization Rate
  doughnutChartLabelsUtil: Label[] = [];
  doughnutChartDataUtil: MultiDataSet = [ [0, 0] ];

  doughnutChartOptionsUtil: ChartOptions = {
   responsive: true,
   legend: {
     display: false
   },
   cutoutPercentage: 70,
   tooltips: {
     filter: function(item, data) {
       var label = data.labels[item.index];
       if (label) return true;
       return false;
     }
   }
 };

 refreshData(flag?: boolean){
  let totalDriveTime=0;
  let tripDistance=0;
  //this.movedVehicle=0;
  this.noAction=0;
  this.serviceNow=0;
  this.stopNow=0;
  
  //this.totalVehicle=0;
  //this.criticalAlert=0;
  //this.mileageDone = this.summaryData.reduce((val, elem) => val + elem.tripDistance, 0);
  if(this.summaryData){
    if(flag){
      this.movedVehicle = 0;
      let drivers = this.summaryData.filter((elem) => elem.driver1Id);
      let driversUnknown = this.summaryData.filter((elem) => elem.driver1Id=='');
      let uniqueDrivers = [...new Set(drivers)];
      this.drivers = uniqueDrivers.length + driversUnknown.length;
    }
    // let drivers = this.summaryData.filter((elem) => elem.driver1Id);
    // let uniqueDrivers = [...new Set(drivers)];
    // this.drivers = uniqueDrivers.length;

    let vins = this.summaryData.filter((elem) => elem.vin);
    let uniqueVin = [...new Set(vins)];
    //this.totalVehicle = uniqueVin.length;
    let criticalCount = 0;
    
    this.summaryData.forEach(element => {

      if (element.tripDistance) {
        tripDistance += element.tripDistance;
      }
      if(flag){
        if (element.drivingTime)
          totalDriveTime += element.drivingTime;

        if (element.vehicleDrivingStatusType && element.vehicleDrivingStatusType != 'N') {
          this.movedVehicle += 1;
        }

        if(element.fleetOverviewAlert.length > 0){         
          let isCritical = true;
          element.fleetOverviewAlert.forEach(ele => {
            if(isCritical){
              if(ele.level === 'C' ){
                criticalCount += ele.level === 'C' ? 1 : 0;   
                isCritical = false; 
                return;             
              }             
            }                         
          });        
          if(criticalCount > 0){
            this.criticalAlert = criticalCount;
          }  
        }  

      }
      // if(element.drivingTime)
      //   totalDriveTime += element.drivingTime;
      // if(element.tripDistance){
      //   tripDistance += element.tripDistance;
      // }
      // if(element.vehicleDrivingStatusType && element.vehicleDrivingStatusType === 'D'){
      //   this.movedVehicle += 1;
      // }
      if(element.vehicleHealthStatusType){
        if(element.vehicleHealthStatusType === 'N'){
          this.noAction += 1;
        } else if(element.vehicleHealthStatusType === 'V'){
            this.serviceNow += 1;
        } else if(element.vehicleHealthStatusType === 'T'){
            this.stopNow += 1;
        }
        // if(element.fleetOverviewAlert.length > 0){         
        //   let isCritical = true;
        //   element.fleetOverviewAlert.forEach(ele => {
        //     if(isCritical){
        //       if(ele.level === 'C' ){
        //         criticalCount += ele.level === 'C' ? 1 : 0;   
        //         isCritical = false; 
        //         return;             
        //       }             
        //     }                         
        //   });        
        //   if(criticalCount > 0){
        //     this.criticalAlert = criticalCount;
        //   }  
        // }  
      }
    });
  }
  //this.mileageDone = (this.prefUnitFormat == 'dunit_Metric' ? tripDistance : (tripDistance * 0.621371)) + ' ' + this.unitValkm;
  let milDone:any = this.getDistance(tripDistance, this.prefUnitFormat);
  
  this.mileageDone = milDone + ' ' + this.unitValkm;
  let totDriveTime = Util.getHhMmTimeFromMS(totalDriveTime).split(':'); //driving time is coming in ms
  this.driveTime = flag ? totDriveTime[0] + (this.translationData.lblhh ) + ' ' +totDriveTime[1] + (this.translationData.lblmm) : this.driveTime;

  this.barChartData = [
    { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
  ];

  // let totDriveTime = Util.getHhMmTime(totalDriveTime).split(':');
  // this.driveTime = totDriveTime[0] + (this.translationData.lblhh ) + ' ' +totDriveTime[1] + (this.translationData.lblmm);
  // this.barChartData = [
  //   { data: [this.movedVehicle, this.totalVehicle], label: '', barThickness: 16, barPercentage: 0.5 }
  // ];
  //Fleet Mileage rate
  let liveFleetMileageVal:any= localStorage.getItem('liveFleetMileageThreshold');
  let liveFleetUtilizationThresholdVal = localStorage.getItem('liveFleetUtilizationThreshold');
  //let milRate = (Number.parseFloat(this.mileageDone)/Number.parseInt(liveFleetMileageVal)) * 100;
   this.mileageRate = Number.parseFloat(((Number.parseFloat(milDone)/Number.parseInt(liveFleetMileageVal)) * 100).toFixed(2));
  this.doughnutChartDataMileage = [ [this.mileageRate, 100-this.mileageRate] ];
  //Fleet Utilization rate
  let utilRate:any = ((this.movedVehicle/Number.parseInt(liveFleetUtilizationThresholdVal)) * 100).toFixed(2);
  this.utilizationRate = utilRate;
  this.doughnutChartDataUtil = [ [this.utilizationRate, 100-this.utilizationRate] ];
 }

 resetSummary(){
  //this.movedVehicle=0;
  this.noAction=0;
  this.serviceNow=0;
  this.stopNow=0;
  //this.totalVehicle=0;
  //this.criticalAlert=0;
  //this.barChartData = [ { data : [0, 0] } ];
  this.doughnutChartDataMileage = [ [0 , 0] ];
  this.doughnutChartDataUtil = [ [0, 0] ];
  //this.mileageDone = '00 ' + this.unitValkm;
  //this.driveTime = '00' + (this.translationData.lblhh) + ' 00' + (this.translationData.lblmm);
  //this.drivers = 0;
 }

 getDistance(distance: any, unitFormat: any){
  // distance in meter
  let _distance: any = 0;
  switch(unitFormat){
    case 'dunit_Metric': {
      _distance = (distance/1000).toFixed(2); //-- km
      break;
    }
    case 'dunit_Imperial':
    case 'dunit_USImperial': {
      _distance = (distance/1609.344).toFixed(2); //-- mile
      break;
    }
    default: {
      _distance = distance.toFixed(2);
    }
  }
  return _distance;
}
}
