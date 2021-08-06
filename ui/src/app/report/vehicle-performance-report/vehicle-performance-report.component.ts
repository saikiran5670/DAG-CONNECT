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
    'H':'',
    'M':'',
    'N':'#CC0000',
    'O':'#00B050',
    'P':'#FFC000',
    'S':'#EEECE1',
    'T':'',
    'U':'#659FA4'
  };
  
  chartXaxis;
  chartYaxis;
  legends = [];

  xaxisE = {
    type: 'numeric',
    tickPlacement: 'between',
    max: 100,
    tickAmount: 10,
    title: {
      text: 'RPM',
    },
    labels: {
      offsetX: 0,
      offsetY: 0,
      formatter: (value, index) => {
        console.log(`yaxis -> value ${value} & index ${index}` )
        // return value;
        if(value !== 0) {
          let newIndex = (index/10)-1;
          return this.xaxisVaues[newIndex];
        }
        return '';
      },
      style: {
        // colors: [],
        // fontSize: '12px',
        // fontFamily: 'Helvetica, Arial, sans-serif',
        // fontWeight: 400,
        cssClass: 'apexcharts-xaxis-label',
      }
    },
    tooltip: {
      enabled: false,
    }
  }

  yaxisE = {
    type: 'numeric',
    tickPlacement: 'between',
    max: 100,
    tickAmount: 10,
    labels: {
      offsetX: 0,
      offsetY: 15,
      formatter: (value, index) => {
        console.log(`yaxis -> value ${value} & index ${index}`)
        if(index !== 0) {
          return value;
        }
        return ' ';
      }
    },
    title: {
      text: '%',
    },
  }

  xaxisS = {
    type: 'category',
    tickPlacement: 'between',
    title: {
      text: 'RPM',
    },
    labels: {
      formatter: (value) => {
        if(value == 9) {
          return ">2100";
        }
        return value;
      }
    },
  }

  yaxisS = {
    max: 100,
    tickAmount: 10,
    title: {
      text: '%',
    },
  }

  xaxisB = {
    type: 'category',
    tickPlacement: 'between',
    title: {
      text: 'RPM',
    },
    labels: {
      formatter: (value) => {
        if(value == 9) {
          return ">2100";
        }
        return value;
      }
    },
  }

  yaxisB = {
    max: 100,
    tickAmount: 10,
    title: {
      text: '%',
    },
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
    this.kpi();
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
            this.legends.push(newkpi);
          }
        }
      }
    });
  }

  getMenuTranslations(translationObj) {
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  showSearchResult(data) {
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
    console.log("legends", this.legends)
    forkJoin([req1, req2]).subscribe((res:any) => {
      console.log("res", res)
      this.searchResult = { ...this.searchResult, ...res[0].vehPerformanceSummary}
      this.searchResult.vehPerformanceCharts = res[0].vehPerformanceCharts;
      this.searchResult.kpiData = res[1].kpiData;
      this.searchResult.matrixData = res[1].matrixData;
      this.searchResult.bubbleData = this.transformDataForBubbleChart(res[1].matrixData);
      this.chartXaxis = this["xaxis"+ this.searchResult.performanceType];
      this.chartYaxis = this["yaxis"+ this.searchResult.performanceType];
      this.xaxisVaues = this.processXaxis(res[0].vehPerformanceCharts)
      this.search = true;
      this.showLoadingIndicator = false;
      this.generatePieChartData(res[1].kpiData);
      console.log("searchResult", this.searchResult)
    }, (err) => {
      this.showLoadingIndicator = false;
    });    
  }
  
  hideSearchResult() {
    this.search = false;
  }

  processXaxis(data) {
    let xaxisObj = data.filter((item)=>item.index == -1);
    if(xaxisObj[0] && xaxisObj[0].axisvalues) {
      let tempArr = xaxisObj[0].axisvalues.split(',')
      tempArr = tempArr.map(el => el.replace(/'/g, ''));
      console.log(tempArr);
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
