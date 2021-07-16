import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReportService } from '../../../services/report.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-eco-score-report-preferences',
  templateUrl: './eco-score-report-preferences.component.html',
  styleUrls: ['./eco-score-report-preferences.component.less']
})
export class EcoScoreReportPreferencesComponent implements OnInit {
  @Input() editFlag: any;
  @Input() reportListData: any;
  @Input() translationData: any;
  @Output() setEcoScoreFlag = new EventEmitter<any>();
  localStLanguage: any;
  accountId: any;
  accountOrganizationId: any;
  roleID: any;
  reportId: any;
  initData: any = [];
  generalColumnData: any = [];
  generalGraphColumnData: any = [];
  driverPerformanceColumnData: any = [];
  driverPerformanceGraphColumnData: any = [];
  selectionForGeneralColumns = new SelectionModel(true, []);
  selectionForGeneralGraphColumns = new SelectionModel(true, []);
  selectionForDriverPerformanceColumns = new SelectionModel(true, []);
  selectionForDriverPerformanceGraphColumns = new SelectionModel(true, []);

  constructor(private reportService: ReportService, private router: Router) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    let repoId: any = this.reportListData.filter(i => i.name == 'EcoScore Report');
    if(repoId.length > 0){
      this.reportId = repoId[0].id; 
    }else{
      this.reportId = 10; //- hard coded for Eco-Score Report
    }
    this.translationUpdate();
    this.loadEcoScoreReportPreferences();
  }

  translationUpdate(){
    this.translationData = {
      
    }
  }

  loadEcoScoreReportPreferences(){
    this.reportService.getReportUserPreference(this.reportId).subscribe((prefData : any) => {
      this.initData = prefData['userPreferences'];
      this.resetColumnData();
      this.preparePrefData(this.initData);
    }, (error)=>{
      this.initData = [];
      this.resetColumnData();
      // let prefData = {
      //   "dataAttributeId":221,
      //   "name":"EcoScore",
      //   "key":"rp_ecoscore",
      //   "state":"A",
      //   "chartType":"",
      //   "thresholdType":"",
      //   "thresholdValue":0,
      //   "subReportUserPreferences":[
      //      {
      //         "dataAttributeId":234,
      //         "name":"EcoScore.General",
      //         "key":"rp_general",
      //         "state":"A",
      //         "chartType":"",
      //         "thresholdType":"",
      //         "thresholdValue":0,
      //         "subReportUserPreferences":[
      //            {
      //               "dataAttributeId":235,
      //               "name":"EcoScore.General.AverageGrossweight",
      //               "key":"rp_averagegrossweight",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":236,
      //               "name":"EcoScore.General.Distance",
      //               "key":"rp_distance",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":237,
      //               "name":"EcoScore.General.NumberOfTrips",
      //               "key":"rp_numberoftrips",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":238,
      //               "name":"EcoScore.General.NumberOfVehicles",
      //               "dataAttributeType":"A",
      //               "key":"rp_numberofvehicles",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":239,
      //               "name":"EcoScore.General.AverageDistancePerDay",
      //               "key":"rp_averagedistanceperday",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            }
      //         ]
      //      },
      //      {
      //         "dataAttributeId":240,
      //         "name":"EcoScore.GeneralGraph",
      //         "key":"rp_generalgraph",
      //         "state":"A",
      //         "chartType":"",
      //         "thresholdType":"",
      //         "thresholdValue":0,
      //         "subReportUserPreferences":[
      //            {
      //               "dataAttributeId":241,
      //               "name":"EcoScore.GeneralGraph.PieChart",
      //               "key":"rp_piechart",
      //               "state":"I",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":242,
      //               "name":"EcoScore.GeneralGraph.BarGraph",
      //               "key":"rp_bargraph",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            }
      //         ]
      //      },
      //      {
      //         "dataAttributeId":243,
      //         "name":"EcoScore.DriverPerformance",
      //         "key":"rp_driverperformance",
      //         "state":"A",
      //         "chartType":"",
      //         "thresholdType":"",
      //         "thresholdValue":0,
      //         "subReportUserPreferences":[
      //            {
      //               "dataAttributeId":244,
      //               "name":"EcoScore.DriverPerformance.EcoScore",
      //               "key":"rp_ecoscore",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":245,
      //               "name":"EcoScore.DriverPerformance.FuelConsumption",
      //               "key":"rp_fuelconsumption",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
      //                  {
      //                     "dataAttributeId":246,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage",
      //                     "key":"rp_cruisecontrolusage",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
      //                        {
      //                           "dataAttributeId":247,
      //                           "name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage30-50km/h(%)",
      //                           "key":"rp_CruiseControlUsage30",
      //                           "state":"A",
      //                           "chartType":"",
      //                           "thresholdType":"",
      //                           "thresholdValue":0,
      //                           "subReportUserPreferences":[
                                   
      //                           ]
      //                        },
      //                        {
      //                           "dataAttributeId":248,
      //                           "name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage50-75km/h(%)",
      //                           "key":"rp_cruisecontroldistance50",
      //                           "state":"A",
      //                           "chartType":"",
      //                           "thresholdType":"",
      //                           "thresholdValue":0,
      //                           "subReportUserPreferences":[
                                   
      //                           ]
      //                        },
      //                        {
      //                           "dataAttributeId":249,
      //                           "name":"EcoScore.DriverPerformance.FuelConsumption.CruiseControlUsage.CruiseControlUsage>75km/h(%)",
      //                           "key":"rp_cruisecontroldistance75",
      //                           "state":"A",
      //                           "chartType":"",
      //                           "thresholdType":"",
      //                           "thresholdValue":0,
      //                           "subReportUserPreferences":[
                                   
      //                           ]
      //                        }
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":250,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.PTOUsage(%)",
      //                     "key":"rp_ptousage",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":251,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.PTODuration",
      //                     "key":"rp_ptoduration",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":252,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.AverageDrivingSpeed",
      //                     "key":"rp_averagedrivingspeed",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":253,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.AverageSpeed",
      //                     "key":"rp_averagespeed",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":254,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottling(%)",
      //                     "key":"rp_heavythrottling",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":255,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.HeavyThrottleDuration",
      //                     "key":"rp_heavythrottleduration",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":256,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.Idling(%)",
      //                     "key":"rp_idling",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  },
      //                  {
      //                     "dataAttributeId":257,
      //                     "name":"EcoScore.DriverPerformance.FuelConsumption.IdleDuration",
      //                     "key":"rp_idleduration",
      //                     "state":"A",
      //                     "chartType":"",
      //                     "thresholdType":"",
      //                     "thresholdValue":0,
      //                     "subReportUserPreferences":[
                             
      //                     ]
      //                  }
      //               ]
      //            },
      //            {
      //               "dataAttributeId":261,
      //               "name":"EcoScore.DriverPerformance.BrakingScore.Braking(%)",
      //               "key":"rp_braking",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":263,
      //               "name":"EcoScore.DriverPerformance.AnticipationScore",
      //               "key":"rp_anticipationscore",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            }
      //         ]
      //      },
      //      {
      //         "dataAttributeId":264,
      //         "name":"EcoScore.DriverPerformanceGraph",
      //         "key":"rp_driverperformancegraph",
      //         "state":"A",
      //         "chartType":"",
      //         "thresholdType":"",
      //         "thresholdValue":0,
      //         "subReportUserPreferences":[
      //            {
      //               "dataAttributeId":265,
      //               "name":"EcoScore.DriverPerformanceGraph.PieChart",
      //               "key":"rp_piechart",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            },
      //            {
      //               "dataAttributeId":266,
      //               "name":"EcoScore.DriverPerformanceGraph.BarGraph",
      //               "key":"rp_bargraph",
      //               "state":"A",
      //               "chartType":"",
      //               "thresholdType":"",
      //               "thresholdValue":0,
      //               "subReportUserPreferences":[
                       
      //               ]
      //            }
      //         ]
      //      }
      //   ]
      // };
      // this.initData = prefData;
      // this.resetColumnData();
      // this.preparePrefData(this.initData);
    });
  }

  resetColumnData(){
    this.generalColumnData = [];
    this.generalGraphColumnData = [];
    this.driverPerformanceColumnData = [];
    this.driverPerformanceGraphColumnData = [];
  }

  preparePrefData(prefData: any){
    if(prefData && prefData.subReportUserPreferences && prefData.subReportUserPreferences.length > 0){
      prefData.subReportUserPreferences.forEach(element => {
        if(element.subReportUserPreferences && element.subReportUserPreferences.length > 0){
          element.subReportUserPreferences.forEach(item => {
            let _data: any = item;
            if(item.name.includes('EcoScore.General.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 17);   
              }
              this.generalColumnData.push(_data);
            }else if(item.name.includes('EcoScore.GeneralGraph.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 22);   
              }
              this.generalGraphColumnData.push(_data);
            }else if(item.name.includes('EcoScore.DriverPerformance.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 27);   
              }
              this.driverPerformanceColumnData.push(_data);
            }else if(item.name.includes('EcoScore.DriverPerformanceGraph.')){
              if(this.translationData[item.key]){
                _data.translatedName = this.translationData[item.key];  
              }else{
                _data.translatedName = this.getName(item.name, 32);   
              }
              this.driverPerformanceGraphColumnData.push(_data);
            }
          });
        }
      });
    }
    this.setColumnCheckbox();
  }

  setColumnCheckbox(){
    this.selectionForGeneralColumns.clear();
    this.selectionForGeneralGraphColumns.clear();
    this.selectionForDriverPerformanceColumns.clear();
    this.selectionForDriverPerformanceGraphColumns.clear();
    
    this.generalColumnData.forEach(element => {
      if(element.state == 'A'){
        this.selectionForGeneralColumns.select(element);
      }
    });
    
    // this.detailColumnData.forEach(element => {
    //   if(element.state == 'A'){
    //     this.selectionForDetailsColumns.select(element);
    //   }
    // });

    // this.chartsColumnData.forEach(element => {
    //   if(element.state == 'A'){
    //     this.selectionForChartsColumns.select(element);
    //   }
    // });
    // if(this.summaryColumnData.length > 0 && this.chartsColumnData.length > 0 && this.calenderColumnData.length > 0 && this.detailColumnData.length > 0){
    //   this.setDefaultFormValues();
    // }
  }
  
  getName(name: any, _count: any) {
    let updatedName = name.slice(_count);
    return updatedName;
  }

  masterToggleForGeneralColumns(){
    if(this.isAllSelectedForGeneralColumns()){
      this.selectionForGeneralColumns.clear();
    }else{
      this.generalColumnData.forEach(row => { this.selectionForGeneralColumns.select(row) });
    }
  }

  isAllSelectedForGeneralColumns(){
    const numSelected = this.selectionForGeneralColumns.selected.length;
    const numRows = this.generalColumnData.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeneralColumns(row?: any){

  }

  checkboxClicked(event: any, rowData: any){
    
  }

  onCancel(){

  }

  onReset(){

  }

  onConfirm(){
    
  }

}
