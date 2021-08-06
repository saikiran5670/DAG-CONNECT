import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ReportService } from 'src/app/services/report.service';
import { TranslationService } from 'src/app/services/translation.service';

@Component({
  selector: 'app-vehicle-performance-report',
  templateUrl: './vehicle-performance-report.component.html',
  styleUrls: ['./vehicle-performance-report.component.css']
})
export class VehiclePerformanceReportComponent implements OnInit {
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

  xaxisLabels = {
    offsetX: 0,
    offsetY: 0,
    formatter: (value, index) => {
      if(value !== 0) {
        let newIndex = (index/10)-1;
        return this.xaxisVaues[newIndex];
      }
      return '';
    },
    style: {
      cssClass: 'apexcharts-xaxis-label',
    }
  };

  yaxisLabels = {
    offsetX: 0,
    offsetY: 15,
    formatter: (value, index) => {
      if(index !== 0) {
        let newIndex = index - 1;
        return this.yaxisVaues[newIndex];
      }
      return '';
    }
  };

  xaxisE = {
    ...this.commonAxis,
    title: {
      text: 'RPM',
    },
    labels: this.xaxisLabels
  }

  yaxisE = {
    ...this.commonAxis,
    title: {
      text: '%',
    },
    labels: this.yaxisLabels
  }

  xaxisS = {
    ...this.commonAxis,
    title: {
      text: 'KM/H',
    },
    labels: this.xaxisLabels,
  }

  yaxisS = {
    ...this.commonAxis,
    title: {
      text: 'RPM',
    },
    labels: this.yaxisLabels
  }


  xaxisB = {
    ...this.commonAxis,
    title: {
      text: 'KM/H',
    },
    labels: this.xaxisLabels,
  }

  yaxisB = {
    ...this.commonAxis,
    title: {
      text: 'M/S2',
    },
    labels: this.yaxisLabels
  }

  constructor(private translationService: TranslationService, private reportService: ReportService) {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
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
  }

  ngOnInit(): void {
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
    this.searchResult['lbl'] = 'lbl' + this.translationData[performaceObj[0].name].replace(/ /g, '');
    let payload =  {
      "vin": this.searchResult.vin,
      "performanceType": this.searchResult.performanceType,
      "startDateTime": this.searchResult.utcStartDateTime,
      "endDateTime": this.searchResult.utcEndDateTime
    }
    let req1 = this.reportService.chartTemplate(payload);
    let req2 = this.reportService.chartData(payload);
    forkJoin([req1, req2]).subscribe((res:any) => {
      this.searchResult = { ...this.searchResult, ...res[0].vehPerformanceSummary}
      this.searchResult.vehPerformanceCharts = res[0].vehPerformanceCharts;
      this.searchResult.kpiData = res[1].kpiData;
      this.searchResult.matrixData = res[1].matrixData;
      this.searchResult.bubbleData = this.transformDataForBubbleChart(res[1].matrixData);
      this.chartXaxis = this["xaxis"+ this.searchResult.performanceType];
      this.chartYaxis = this["yaxis"+ this.searchResult.performanceType];
      this.xaxisVaues = this.processXaxis(res[0].vehPerformanceCharts);
      this.yaxisVaues = this.processYaxis(res[0].vehPerformanceCharts);
      this.search = true;
      this.showLoadingIndicator = false;
      this.generatePieChartData(res[1].kpiData);
      console.log("searchResult", this.searchResult)
      console.log("xaxisVaues", this.xaxisVaues)
      console.log("yaxisVaues", this.yaxisVaues)
    }, (err) => {
      this.showLoadingIndicator = false;
    });    
  }
  
  hideSearchResult() {
    this.search = false;
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
    for(let bubble of bubbleData) {
      let newArr = [];
      newArr.push(bubble.xindex*10);
      newArr.push(bubble.yindex*10);
      newArr.push(bubble.value);
      bubbleChartData.push(newArr);
    }
    return bubbleChartData;
  }

  generatePieChartData(pieData) {
    this.pieChartData = [];
    this.pieChartLabels = [];
    for (let pie of pieData) {
      if (pie.label.trim() != '') {
        let labelObj = this.legends.filter(item => item.value == pie.label.trim());
        if (labelObj && labelObj[0]) {
          this.pieChartLabels.push(this.translationData[labelObj[0].name]);
        } else {
          this.pieChartLabels.push(pie.label.trim());
        }
        this.pieChartData.push(pie.value);
      }
    }
  }

}
