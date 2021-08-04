import { Component, EventEmitter, OnInit, Output } from '@angular/core';
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
  // accountPrefObj;
  // accountOrganizationId;
  // accountId;
  translationData: any = {};
  search:boolean = false;
  performanceTypeLst = [
    { label: "Engine Load Collective",  value: "E" },
    { label: "Road Speed Collective",  value: "R" },
    { label: "Brake Behavior",  value: "B" }
  ];

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

  getMenuTranslations(translationObj) {
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  showSearchResult(data) {
    let performaceObj = this.performanceTypeLst.filter((item)=>item.value == data.performanceType);
    this.searchResult = data;
    this.searchResult['performanceTypeLabel'] = performaceObj[0].label;
    this.searchResult['lbl'] = 'lbl' + performaceObj[0].label.replace(/ /g, '');
    this.search = true;
    let payload =  {
      "vin": this.searchResult.vin,
      "performanceType": this.searchResult.performanceType,
      "startDateTime": this.searchResult.utcStartDateTime,
      "endDateTime": this.searchResult.utcEndDateTime
    }
    this.reportService.vehicleperformancechart(payload).subscribe((res:any) => {
      console.log("res", res)
      this.searchResult = { ...this.searchResult, ...res.vehPerformanceSummary}
      this.searchResult.vehPerformanceCharts = res.vehPerformanceCharts;
    })
    console.log("this.searchResult", this.searchResult)
  }
  
  hideSearchResult() {
    this.search = false;
  }

}
