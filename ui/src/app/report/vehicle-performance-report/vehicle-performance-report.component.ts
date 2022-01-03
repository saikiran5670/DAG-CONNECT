import { Component, OnInit, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ReportService } from 'src/app/services/report.service';
import { TranslationService } from 'src/app/services/translation.service';
import { SearchCriteriaComponent } from './search-criteria/search-criteria.component';

@Component({
  selector: 'app-vehicle-performance-report',
  templateUrl: './vehicle-performance-report.component.html',
  styleUrls: ['./vehicle-performance-report.component.css']
})
export class VehiclePerformanceReportComponent implements OnInit {
  @ViewChild('searchCriteria') searchCriteria : SearchCriteriaComponent
  detailsExpandPanel: boolean = true;
  chartsExpandPanel: boolean = true;
  searchResult: any = {};
  localStLanguage;
  translationData: any = {};
  search:boolean = false;
  showLoadingIndicator:boolean = false;
  xaxisVaues = [];
  yaxisVaues = [];
  pieChartLabels = [];
  pieChartData = [];
  pieChartColors = [];
  piechartTitle = '';
  bubbleHeatchartTitle = '';
  vehicleDisplayPreference = 'dvehicledisplay_Name';
  accountInfo:any = {};
  defualtData = [
    [
      0,
      5,
      0
    ],
    [
      0,
      15,
      0
    ],
    [
      0,
      25,
      0
    ],
    [
      0,
      35,
      0
    ],
    [
      0,
      45,
      0
    ],
    [
      0,
      55,
      0
    ],
    [
      0,
      65,
      0
    ],
    [
      0,
      75,
      0
    ],
    [
      0,
      85,
      0
    ],
    [
      0,
      95,
      0
    ],
  ]
  performanceTypeLst = [
    { name: "Engine Load Collective",  value: "E" },
    { name: "Road Speed Collective",  value: "S" },
    { name: "Brake Behavior",  value: "B" }
  ];
  colorToLegends = {
    'A':'#75923C',
    'D':'#4AB0BA',
    'E':'#E46D0A',
    'H':'#FF0000',
    'M':'#FFFF00',
    'N':'#CC0000',
    'O':'#00B050',
    'P':'#FFC000',
    'S':'#EEECE1',
    'T':'#FFC000',
    'U':'#659FA4'
  };
  
  chartXaxis;
  chartYaxis;
  legends = [];

  commonAxis = {
    type: 'numeric',
    tickPlacement: 'between',
    max: 100,
    tickAmount: 10,
    tooltip: {
      enabled: false,
    }
  }

  xaxisLabels: any = {}
  yaxisLabels: any = {}
  xaxisE:any = {}
  yaxisE:any = {}
  xaxisS:any = {}
  yaxisS:any = {}
  xaxisB:any = {}
  yaxisB:any = {}
  

  

  constructor(private translationService: TranslationService, private reportService: ReportService) {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountInfo = JSON.parse(localStorage.getItem("accountInfo"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 13 //-- for Trip Report
    }
    this.getMenuTranslations(translationObj);
    this.getPreferences();
    this.getAxisLabels();
  }

  getAxisLabels() {
    this.xaxisLabels = {
      offsetX: -8,
      offsetY: 0,
      formatter: (value, index) => {
        let newIndex = (index/10)-1;
        if(this.xaxisVaues[newIndex]) {
          return this.xaxisVaues[newIndex];
        }
        if(value == 0) {
          return '';
        }
        return value;
      },
      style: {
        cssClass: 'apexcharts-xaxis-label',
      }
    };
  
    this.yaxisLabels = {
      offsetX: 5,
      offsetY: 15,
      formatter: (value, index) => {
        if(index !== 0) {
          let newIndex = index - 1;
          return this.yaxisVaues[newIndex];
        }
        return '';
      }
    };
    this.xaxisE = {
      ...this.commonAxis,
      title: {
        text: this.translationData.lblRpm,
      },
      labels: this.xaxisLabels
    }
  
    this.yaxisE = {
      ...this.commonAxis,
      title: {
        text: '%',
        offsetX: 5,
      },
      labels: this.yaxisLabels
    }
  
    this.xaxisS = {
      ...this.commonAxis,
      title: {
        text: this.translationData.lblKmh,
      },
      labels: this.xaxisLabels,
    }
  
    this.yaxisS = {
      ...this.commonAxis,
      title: {
        text: this.translationData.lblRpm,
        offsetX: 5,
      },
      labels: this.yaxisLabels
    }
  
  
    this.xaxisB = {
      ...this.commonAxis,
      title: {
        text: this.translationData.lblKmh,
      },
      labels: this.xaxisLabels,
    }
  
    this.yaxisB = {
      ...this.commonAxis,
      title: {
        text: this.translationData.lblMs2,
        offsetX: 5,
      },
      labels: this.yaxisLabels
    }
  }

  ngOnInit(): void {
  }

  getPreferences() {
    this.translationService.getPreferences(this.localStLanguage.code).subscribe((prefData: any) => {
      let vehicleDisplayId = this.accountInfo.accountPreference.vehicleDisplayId;
      if(vehicleDisplayId) {
        let vehicledisplay = prefData.vehicledisplay.filter((el) => el.id == vehicleDisplayId);
        if(vehicledisplay.length != 0) {
          this.vehicleDisplayPreference = vehicledisplay[0].name;
        }
      }
      this.searchCriteria.getPreferences(prefData)
    });
  }

  kpi() {
    this.reportService.kpi().subscribe((kpiList: any) => {
      this.searchResult.kpi = kpiList;
      if (kpiList.length > 0) {
        this.performanceTypeLst = [];
        for (let kpi of kpiList) {
          if (kpi.type == 'Y') {
            this.performanceTypeLst.push(kpi);
          } else {
            let newkpi = kpi;
            newkpi['color'] = this.colorToLegends[kpi.value];
            newkpi['transName'] = this.translationData[kpi.name];
            this.legends.push(newkpi);
          }
        }
      }
    });
  }

  getMenuTranslations(translationObj) {
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.kpi();
      this.processTranslation(data);
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  showSearchResult(data) {
    this.hideSearchResult();
    this.showLoadingIndicator = true;
    let performaceObj = this.performanceTypeLst.filter((item)=>item.value == data.performanceType);
    this.searchResult = data;
    this.searchResult['performanceTypeLabel'] = this.translationData[performaceObj[0].name];
    this.searchResult['lbl'] = 'lbl' + this.translationData[performaceObj[0].name]?.replace(/ /g, '');
    let payload =  {
      "vin": this.searchResult.vin,
      "performanceType": this.searchResult.performanceType,
      "startDateTime": this.searchResult.utcStartDateTime,
      "endDateTime": this.searchResult.utcEndDateTime
    }
    let req1 = this.reportService.chartTemplate(payload);
    let req2 = this.reportService.chartData(payload);
    this.updateChartTitles(this.searchResult.performanceType);
    forkJoin([req1, req2]).subscribe((res:any) => {
      if(res[1] && res[1].matrixData.length > 0) {
        this.searchResult = { ...this.searchResult, ...res[0].vehPerformanceSummary}
        this.searchResult.vehPerformanceCharts = res[0].vehPerformanceCharts;
        this.searchResult.kpiData = res[1].kpiData;
        this.searchResult.matrixData = res[1].matrixData;
        this.searchResult.bubbleData = this.transformDataForBubbleChart(res[1].matrixData);
        this.chartXaxis = this["xaxis"+ this.searchResult.performanceType];
        this.chartYaxis = this["yaxis"+ this.searchResult.performanceType];
        this.xaxisVaues = this.processXaxis(res[0].vehPerformanceCharts);
        this.yaxisVaues = this.processYaxis(res[0].vehPerformanceCharts);
        this.generatePieChartData(res[1].kpiData);
        this.search = true;
      } else {
        this.search = false;
      }
      this.showLoadingIndicator = false;
    }, (err) => {
      this.showLoadingIndicator = false;
    });    
  }
  
  hideSearchResult() {
    this.search = false;
  }

  updateChartTitles(performanceType) {
    if(performanceType == 'E') {
      this.piechartTitle = this.translationData.lblEngineOperationalPerformance || "Engine Operational Performance";
      this.bubbleHeatchartTitle = this.translationData.lblEngineLoadDistribution || "Engine Load Distribution";
    } else if(performanceType == 'S') {
      this.piechartTitle = this.translationData.lblRoadSpeedPerformance+" (%)";
      this.bubbleHeatchartTitle = this.translationData.lblRoadSpeedDistribution || "Road Speed Distribution";
    } else {
      this.piechartTitle = this.translationData.lblBrakePerformance+" (%)";
      this.bubbleHeatchartTitle = this.translationData.lblBrakeBehavior || "Brake Behavior Distribution";
    }
  }

  processXaxis(data) {
    if(data.length > 0) {
      let xaxisObj = data.filter((item)=>item.index == -1);
      if(xaxisObj[0] && xaxisObj[0].axisvalues) {
        let tempArr = xaxisObj[0].axisvalues.split(',')
        tempArr = tempArr.map(el => el.replace(/'/g, ''));
        console.log(tempArr);
        return tempArr;
      }
    }
    return [];
  }

  processYaxis(data) {
    if(data.length > 0) {
      let tempArr = [];
      for(let row of data) {
        if(row.index != -1) {
          tempArr.push(row.range);
        }
      }
      return tempArr;
    }
    return [];
  }

  transformDataForBubbleChart(bubbleData) {
    let bubbleChartData = [];
    bubbleChartData.push(...this.defualtData);
    for(let bubble of bubbleData) {
      let newArr = [];
      newArr.push((bubble.xindex*10) + 5 < 0 ? 0 : (bubble.xindex*10) + 5);
      newArr.push((bubble.yindex*10) + 5 < 0 ? 0 : (bubble.yindex*10) + 5);
      newArr.push(Math.abs(bubble.value));
      bubbleChartData.push(newArr);
    }
    return bubbleChartData;
  }

  generatePieChartData(pieData) {
    this.pieChartData = [];
    this.pieChartLabels = [];
    this.pieChartColors = [];
    for (let pie of pieData) {
      let pieLabel = pie.label.trim();
      if (pieLabel != '') {
        let labelObj = this.legends.filter(item => item.value == pieLabel);
        if (labelObj && labelObj[0]) {
          this.pieChartLabels.push(this.translationData[labelObj[0].name]);
        } else {
          this.pieChartLabels.push(pieLabel);
        }
        this.pieChartData.push((pie.value).toFixed(2));
        this.pieChartColors.push(this.colorToLegends[pieLabel]);
      }
    }
   // if(this.searchResult.performanceType != 'E') {
   //   this.updateDataPercent(pieData);
  //  }
  }

  updateDataPercent(pieData) {
    let sumOfNum = this.pieChartData.reduce((a, b) => a + b);
    let percentPerNum = 100 / sumOfNum;
    let tempArr = [ ...this.pieChartData ];
    this.pieChartData = [];
    for(let tA of tempArr) {
      let percent = tA*percentPerNum;
      this.pieChartData.push(percent.toFixed(2));
    }
  }

}
