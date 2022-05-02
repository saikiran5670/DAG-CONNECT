import { Component, ElementRef, Inject, Input, OnInit, ViewChild, Output,EventEmitter } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ReportService } from '../../../services/report.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { Util } from '../../../shared/util';
import { ReportMapService } from '../../report-map.service';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';
import * as Highcharts from 'highcharts';
import ColumnRange from 'highcharts/highcharts-more';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import html2canvas from 'html2canvas';
import { MessageService } from 'src/app/services/message.service';
import { DomSanitizer } from '@angular/platform-browser';
ColumnRange(Highcharts);

@Component({
  selector: 'app-driver-time-detail',
  templateUrl: './driver-time-detail.component.html',
  styleUrls: ['./driver-time-detail.component.less'],
  template: `<highcharts-chart 
  [Highcharts]="Highcharts"
  [constructorType]="'chart'"
  [options]="chartOptions"
  [callbackFunction]="chartCallback"
  [(update)]="updateFlag"
  [oneToOne]="oneToOneFlag"
  [runOutsideAngular]="runOutsideAngularFlag"
  (chartInstance)="getInstance($event)"
  style="width: 100%; height: 400px; display: block;"
></highcharts-chart>`
})
export class DriverTimeDetailComponent implements OnInit {
   Highcharts = Highcharts;
  @Input() translationData : any;
  @Input() driverSelected : boolean;
  @Input() driverDetails : any;
  @Input() detailConvertedData : any;
  @Input() showField: any;
  @Input() graphPayload : any;
  @Input() prefTimeZone : any;
  @Input() prefDateFormat : any;
  @Input() prefTimeFormat : any;
  @Input() driverTableInfoObj : any;
  initData = [];
  chartData = [];
  searchExpandPanel: boolean = true;
  chartExpandPanel : boolean = true;
  tableExpandPanel: boolean = true;
  noDetailsExpandPanel : boolean = true;
  generalExpandPanel : boolean = true;
  totalDriveTime: string = '';
  totalWorkTime: string = '';
  totalAvailableTime: string = '';
  totalRestTime: string = '';
  totalServiceTime: string = '';
  brandimagePath: any;

  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  onSearchData: any = [];
  showLoadingIndicator: boolean = false;
  @Input() displayedColumns:any;// = ['specificdetailstarttime', 'specificdetaildrivetime', 'specificdetailworktime', 'specificdetailservicetime', 'specificdetailresttime', 'specificdetailavailabletime'];
  @Output() backToMainPage = new EventEmitter<any>();
  dayWiseSummary:  {
    startTime: string;
    driveTime: number;
    workTime: number;
    availableTime: number;
    serviceTime: number;
    restTime: number;
  }
  dateFormats: any;
  dayWiseSummaryList: any =[];
  chartOptions: any;
  chart: Highcharts.Chart;
  getInstance(chart: Highcharts.Chart) {
    this.chart = chart;
  }
  barChartLegend = true;
  barChartLabels: string[] = [];

  canvas: any;
  ctx: any;
  //chartPlugins = [crosshair];
  zoomMsg : boolean = true;
  summaryObj:any=[];

  constructor(private reportMapService:ReportMapService, private reportService: ReportService, private messageService: MessageService, private _sanitizer: DomSanitizer) {}

  ngOnInit(): void {
    // this.createChart(this.chartData);
    this.showLoadingIndicator = true;
    this.setPrefFormatDate();
    this.messageService.brandLogoSubject.subscribe(value => {
      if (value != null && value != "") {
        this.brandimagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + value);
      } else {
        this.brandimagePath = null;
      }
    });
  }

  ngOnChanges(){
    this.reportService.getDriverChartDetails(this.graphPayload).subscribe((data : any)=>{
      this.showLoadingIndicator = false;
      // data = JSON.parse('{"driverActivitiesChartData":[{"activityDate":1651098087000,"code":2,"duration":13000,"startTime":1651098074000,"endTime":1651098087000},{"activityDate":1651098141000,"code":2,"duration":9000,"startTime":1651098132000,"endTime":1651098141000},{"activityDate":1651098499000,"code":2,"duration":75000,"startTime":1651098424000,"endTime":1651098499000},{"activityDate":1651098699000,"code":2,"duration":84000,"startTime":1651098615000,"endTime":1651098699000},{"activityDate":1651098869000,"code":2,"duration":10000,"startTime":1651098859000,"endTime":1651098869000},{"activityDate":1651098910000,"code":2,"duration":26000,"startTime":1651098884000,"endTime":1651098910000},{"activityDate":1651100352000,"code":2,"duration":138000,"startTime":1651100291000,"endTime":1651100352000},{"activityDate":1651100452000,"code":2,"duration":31000,"startTime":1651100421000,"endTime":1651100452000},{"activityDate":1651102139000,"code":2,"duration":17000,"startTime":1651102122000,"endTime":1651102139000},{"activityDate":1651103586000,"code":2,"duration":13000,"startTime":1651103573000,"endTime":1651103586000},{"activityDate":1651103640000,"code":2,"duration":10000,"startTime":1651103630000,"endTime":1651103640000},{"activityDate":1651103997000,"code":2,"duration":75000,"startTime":1651103922000,"endTime":1651103997000},{"activityDate":1651104198000,"code":2,"duration":84000,"startTime":1651104114000,"endTime":1651104198000},{"activityDate":1651104368000,"code":2,"duration":10000,"startTime":1651104358000,"endTime":1651104368000},{"activityDate":1651104408000,"code":2,"duration":26000,"startTime":1651104408000,"endTime":1651104408000},{"activityDate":1651105790000,"code":2,"duration":77000,"startTime":1651105713000,"endTime":1651105790000},{"activityDate":1651105951000,"code":2,"duration":31000,"startTime":1651105920000,"endTime":1651105951000},{"activityDate":1651107638000,"code":2,"duration":17000,"startTime":1651107621000,"endTime":1651107638000},{"activityDate":1651110393000,"code":2,"duration":138000,"startTime":1651110332000,"endTime":1651110393000},{"activityDate":1651110493000,"code":2,"duration":31000,"startTime":1651110462000,"endTime":1651110493000},{"activityDate":1651112180000,"code":2,"duration":17000,"startTime":1651112163000,"endTime":1651112180000},{"activityDate":1651113627000,"code":2,"duration":13000,"startTime":1651113614000,"endTime":1651113627000},{"activityDate":1651113680000,"code":2,"duration":9000,"startTime":1651113671000,"endTime":1651113680000},{"activityDate":1651114038000,"code":2,"duration":75000,"startTime":1651113963000,"endTime":1651114038000},{"activityDate":1651114239000,"code":2,"duration":84000,"startTime":1651114155000,"endTime":1651114239000},{"activityDate":1651114409000,"code":2,"duration":10000,"startTime":1651114399000,"endTime":1651114409000},{"activityDate":1651114449000,"code":2,"duration":26000,"startTime":1651114423000,"endTime":1651114449000},{"activityDate":1651115892000,"code":2,"duration":138000,"startTime":1651115831000,"endTime":1651115892000},{"activityDate":1651115992000,"code":2,"duration":31000,"startTime":1651115961000,"endTime":1651115992000},{"activityDate":1651117679000,"code":2,"duration":17000,"startTime":1651117662000,"endTime":1651117679000},{"activityDate":1651119126000,"code":2,"duration":14000,"startTime":1651119112000,"endTime":1651119126000},{"activityDate":1651119179000,"code":2,"duration":9000,"startTime":1651119170000,"endTime":1651119179000},{"activityDate":1651119537000,"code":2,"duration":75000,"startTime":1651119462000,"endTime":1651119537000},{"activityDate":1651119738000,"code":2,"duration":85000,"startTime":1651119653000,"endTime":1651119738000},{"activityDate":1651119908000,"code":2,"duration":11000,"startTime":1651119897000,"endTime":1651119908000},{"activityDate":1651119947000,"code":2,"duration":25000,"startTime":1651119947000,"endTime":1651119947000},{"activityDate":1651121329000,"code":2,"duration":76000,"startTime":1651121253000,"endTime":1651121329000},{"activityDate":1651121490000,"code":2,"duration":31000,"startTime":1651121459000,"endTime":1651121490000},{"activityDate":1651123178000,"code":2,"duration":18000,"startTime":1651123160000,"endTime":1651123178000},{"activityDate":1651124624000,"code":2,"duration":13000,"startTime":1651124611000,"endTime":1651124624000},{"activityDate":1651124678000,"code":2,"duration":9000,"startTime":1651124669000,"endTime":1651124678000},{"activityDate":1651125035000,"code":2,"duration":75000,"startTime":1651124960000,"endTime":1651125035000},{"activityDate":1651125236000,"code":2,"duration":84000,"startTime":1651125152000,"endTime":1651125236000},{"activityDate":1651125406000,"code":2,"duration":10000,"startTime":1651125396000,"endTime":1651125406000},{"activityDate":1651125447000,"code":2,"duration":27000,"startTime":1651125420000,"endTime":1651125447000},{"activityDate":1651126828000,"code":2,"duration":77000,"startTime":1651126751000,"endTime":1651126828000},{"activityDate":1651126989000,"code":2,"duration":31000,"startTime":1651126958000,"endTime":1651126989000},{"activityDate":1651128676000,"code":2,"duration":17000,"startTime":1651128659000,"endTime":1651128676000},{"activityDate":1651130123000,"code":2,"duration":13000,"startTime":1651130110000,"endTime":1651130123000},{"activityDate":1651130176000,"code":2,"duration":9000,"startTime":1651130167000,"endTime":1651130176000},{"activityDate":1651130534000,"code":2,"duration":75000,"startTime":1651130459000,"endTime":1651130534000},{"activityDate":1651130735000,"code":2,"duration":84000,"startTime":1651130651000,"endTime":1651130735000},{"activityDate":1651130905000,"code":2,"duration":10000,"startTime":1651130895000,"endTime":1651130905000},{"activityDate":1651130945000,"code":2,"duration":26000,"startTime":1651130945000,"endTime":1651130945000},{"activityDate":1651132388000,"code":2,"duration":138000,"startTime":1651132327000,"endTime":1651132388000},{"activityDate":1651132488000,"code":2,"duration":31000,"startTime":1651132457000,"endTime":1651132488000},{"activityDate":1651134175000,"code":2,"duration":17000,"startTime":1651134158000,"endTime":1651134175000},{"activityDate":1651135621000,"code":2,"duration":13000,"startTime":1651135608000,"endTime":1651135621000},{"activityDate":1651135675000,"code":2,"duration":9000,"startTime":1651135666000,"endTime":1651135675000},{"activityDate":1651136033000,"code":2,"duration":75000,"startTime":1651135958000,"endTime":1651136033000},{"activityDate":1651136233000,"code":2,"duration":84000,"startTime":1651136149000,"endTime":1651136233000},{"activityDate":1651136403000,"code":2,"duration":10000,"startTime":1651136393000,"endTime":1651136403000},{"activityDate":1651136443000,"code":2,"duration":25000,"startTime":1651136418000,"endTime":1651136443000},{"activityDate":1651137886000,"code":2,"duration":138000,"startTime":1651137825000,"endTime":1651137886000},{"activityDate":1651137986000,"code":2,"duration":31000,"startTime":1651137955000,"endTime":1651137986000},{"activityDate":1651139673000,"code":2,"duration":17000,"startTime":1651139656000,"endTime":1651139673000},{"activityDate":1651141120000,"code":2,"duration":13000,"startTime":1651141107000,"endTime":1651141120000},{"activityDate":1651141173000,"code":2,"duration":9000,"startTime":1651141164000,"endTime":1651141173000},{"activityDate":1651141531000,"code":2,"duration":75000,"startTime":1651141456000,"endTime":1651141531000},{"activityDate":1651141732000,"code":2,"duration":84000,"startTime":1651141648000,"endTime":1651141732000},{"activityDate":1651141902000,"code":2,"duration":10000,"startTime":1651141892000,"endTime":1651141902000},{"activityDate":1651141942000,"code":2,"duration":26000,"startTime":1651141942000,"endTime":1651141942000},{"activityDate":1651143324000,"code":2,"duration":77000,"startTime":1651143247000,"endTime":1651143324000},{"activityDate":1651143485000,"code":2,"duration":31000,"startTime":1651143454000,"endTime":1651143485000},{"activityDate":1651145172000,"code":2,"duration":17000,"startTime":1651145155000,"endTime":1651145172000},{"activityDate":1651146619000,"code":2,"duration":13000,"startTime":1651146606000,"endTime":1651146619000},{"activityDate":1651146672000,"code":2,"duration":9000,"startTime":1651146663000,"endTime":1651146672000},{"activityDate":1651147030000,"code":2,"duration":75000,"startTime":1651146955000,"endTime":1651147030000},{"activityDate":1651147231000,"code":2,"duration":84000,"startTime":1651147147000,"endTime":1651147231000},{"activityDate":1651147401000,"code":2,"duration":10000,"startTime":1651147391000,"endTime":1651147401000},{"activityDate":1651147441000,"code":2,"duration":26000,"startTime":1651147441000,"endTime":1651147441000},{"activityDate":1651148883000,"code":2,"duration":137000,"startTime":1651148823000,"endTime":1651148883000},{"activityDate":1651148984000,"code":2,"duration":32000,"startTime":1651148952000,"endTime":1651148984000},{"activityDate":1651150671000,"code":2,"duration":17000,"startTime":1651150654000,"endTime":1651150671000},{"activityDate":1651152117000,"code":2,"duration":13000,"startTime":1651152104000,"endTime":1651152117000},{"activityDate":1651152171000,"code":2,"duration":9000,"startTime":1651152162000,"endTime":1651152171000},{"activityDate":1651152529000,"code":2,"duration":76000,"startTime":1651152453000,"endTime":1651152529000},{"activityDate":1651152729000,"code":2,"duration":84000,"startTime":1651152645000,"endTime":1651152729000},{"activityDate":1651152899000,"code":2,"duration":10000,"startTime":1651152889000,"endTime":1651152899000},{"activityDate":1651152939000,"code":2,"duration":26000,"startTime":1651152939000,"endTime":1651152939000},{"activityDate":1651154321000,"code":2,"duration":77000,"startTime":1651154244000,"endTime":1651154321000},{"activityDate":1651154482000,"code":2,"duration":31000,"startTime":1651154451000,"endTime":1651154482000},{"activityDate":1651156169000,"code":2,"duration":17000,"startTime":1651156152000,"endTime":1651156169000},{"activityDate":1651157616000,"code":2,"duration":13000,"startTime":1651157603000,"endTime":1651157616000},{"activityDate":1651157669000,"code":2,"duration":9000,"startTime":1651157660000,"endTime":1651157669000},{"activityDate":1651158027000,"code":2,"duration":75000,"startTime":1651157952000,"endTime":1651158027000},{"activityDate":1651158228000,"code":2,"duration":84000,"startTime":1651158144000,"endTime":1651158228000},{"activityDate":1651158398000,"code":2,"duration":10000,"startTime":1651158388000,"endTime":1651158398000},{"activityDate":1651158437000,"code":2,"duration":25000,"startTime":1651158412000,"endTime":1651158437000},{"activityDate":1651159880000,"code":2,"duration":137000,"startTime":1651159820000,"endTime":1651159880000},{"activityDate":1651159981000,"code":2,"duration":32000,"startTime":1651159949000,"endTime":1651159981000},{"activityDate":1651161668000,"code":2,"duration":17000,"startTime":1651161651000,"endTime":1651161668000},{"activityDate":1651163114000,"code":2,"duration":13000,"startTime":1651163101000,"endTime":1651163114000},{"activityDate":1651163168000,"code":2,"duration":9000,"startTime":1651163159000,"endTime":1651163168000},{"activityDate":1651163526000,"code":2,"duration":76000,"startTime":1651163450000,"endTime":1651163526000},{"activityDate":1651163726000,"code":2,"duration":84000,"startTime":1651163642000,"endTime":1651163726000},{"activityDate":1651163896000,"code":2,"duration":10000,"startTime":1651163886000,"endTime":1651163896000},{"activityDate":1651163936000,"code":2,"duration":26000,"startTime":1651163936000,"endTime":1651163936000},{"activityDate":1651165318000,"code":2,"duration":77000,"startTime":1651165241000,"endTime":1651165318000},{"activityDate":1651165479000,"code":2,"duration":31000,"startTime":1651165448000,"endTime":1651165479000},{"activityDate":1651167166000,"code":2,"duration":17000,"startTime":1651167149000,"endTime":1651167166000},{"activityDate":1651168613000,"code":2,"duration":13000,"startTime":1651168600000,"endTime":1651168613000},{"activityDate":1651168666000,"code":2,"duration":9000,"startTime":1651168657000,"endTime":1651168666000},{"activityDate":1651169024000,"code":2,"duration":75000,"startTime":1651168949000,"endTime":1651169024000},{"activityDate":1651169225000,"code":2,"duration":84000,"startTime":1651169141000,"endTime":1651169225000},{"activityDate":1651169395000,"code":2,"duration":10000,"startTime":1651169385000,"endTime":1651169395000},{"activityDate":1651169434000,"code":2,"duration":25000,"startTime":1651169434000,"endTime":1651169434000},{"activityDate":1651170817000,"code":2,"duration":77000,"startTime":1651170740000,"endTime":1651170817000},{"activityDate":1651170978000,"code":2,"duration":32000,"startTime":1651170946000,"endTime":1651170978000},{"activityDate":1651172665000,"code":2,"duration":17000,"startTime":1651172648000,"endTime":1651172665000},{"activityDate":1651174111000,"code":2,"duration":13000,"startTime":1651174098000,"endTime":1651174111000},{"activityDate":1651174165000,"code":2,"duration":9000,"startTime":1651174156000,"endTime":1651174165000},{"activityDate":1651174523000,"code":2,"duration":75000,"startTime":1651174448000,"endTime":1651174523000},{"activityDate":1651174723000,"code":2,"duration":84000,"startTime":1651174639000,"endTime":1651174723000},{"activityDate":1651174893000,"code":2,"duration":10000,"startTime":1651174883000,"endTime":1651174893000},{"activityDate":1651174933000,"code":2,"duration":26000,"startTime":1651174933000,"endTime":1651174933000},{"activityDate":1651176376000,"code":2,"duration":137000,"startTime":1651176316000,"endTime":1651176376000},{"activityDate":1651176477000,"code":2,"duration":32000,"startTime":1651176445000,"endTime":1651176477000},{"activityDate":1651178164000,"code":2,"duration":17000,"startTime":1651178147000,"endTime":1651178164000},{"activityDate":1651179610000,"code":2,"duration":13000,"startTime":1651179597000,"endTime":1651179610000},{"activityDate":1651179664000,"code":2,"duration":9000,"startTime":1651179655000,"endTime":1651179664000},{"activityDate":1651180022000,"code":2,"duration":75000,"startTime":1651179947000,"endTime":1651180022000},{"activityDate":1651180222000,"code":2,"duration":84000,"startTime":1651180138000,"endTime":1651180222000},{"activityDate":1651180392000,"code":2,"duration":10000,"startTime":1651180382000,"endTime":1651180392000},{"activityDate":1651180432000,"code":2,"duration":26000,"startTime":1651180406000,"endTime":1651180432000},{"activityDate":1651181875000,"code":2,"duration":138000,"startTime":1651181814000,"endTime":1651181875000},{"activityDate":1651181975000,"code":2,"duration":31000,"startTime":1651181944000,"endTime":1651181975000},{"activityDate":1651098132000,"code":3,"duration":45000,"startTime":1651098087000,"endTime":1651098132000},{"activityDate":1651098424000,"code":3,"duration":283000,"startTime":1651098141000,"endTime":1651098424000},{"activityDate":1651098615000,"code":3,"duration":116000,"startTime":1651098499000,"endTime":1651098615000},{"activityDate":1651098859000,"code":3,"duration":160000,"startTime":1651098699000,"endTime":1651098859000},{"activityDate":1651098884000,"code":3,"duration":15000,"startTime":1651098869000,"endTime":1651098884000},{"activityDate":1651100214000,"code":3,"duration":188000,"startTime":1651100026000,"endTime":1651100214000},{"activityDate":1651100421000,"code":3,"duration":69000,"startTime":1651100352000,"endTime":1651100421000},{"activityDate":1651102122000,"code":3,"duration":1670000,"startTime":1651100452000,"endTime":1651102122000},{"activityDate":1651103573000,"code":3,"duration":1434000,"startTime":1651102139000,"endTime":1651103573000},{"activityDate":1651103630000,"code":3,"duration":44000,"startTime":1651103586000,"endTime":1651103630000},{"activityDate":1651103922000,"code":3,"duration":282000,"startTime":1651103640000,"endTime":1651103922000},{"activityDate":1651104114000,"code":3,"duration":117000,"startTime":1651103997000,"endTime":1651104114000},{"activityDate":1651104358000,"code":3,"duration":160000,"startTime":1651104198000,"endTime":1651104358000},{"activityDate":1651104382000,"code":3,"duration":14000,"startTime":1651104368000,"endTime":1651104382000},{"activityDate":1651105713000,"code":3,"duration":188000,"startTime":1651105525000,"endTime":1651105713000},{"activityDate":1651105851000,"code":3,"duration":61000,"startTime":1651105790000,"endTime":1651105851000},{"activityDate":1651105920000,"code":3,"duration":69000,"startTime":1651105851000,"endTime":1651105920000},{"activityDate":1651107621000,"code":3,"duration":1670000,"startTime":1651105951000,"endTime":1651107621000},{"activityDate":1651107670000,"code":3,"duration":32000,"startTime":1651107670000,"endTime":1651107670000},{"activityDate":1651110255000,"code":3,"duration":188000,"startTime":1651110067000,"endTime":1651110255000},{"activityDate":1651110462000,"code":3,"duration":69000,"startTime":1651110393000,"endTime":1651110462000},{"activityDate":1651112163000,"code":3,"duration":1670000,"startTime":1651110493000,"endTime":1651112163000},{"activityDate":1651113614000,"code":3,"duration":1434000,"startTime":1651112180000,"endTime":1651113614000},{"activityDate":1651113671000,"code":3,"duration":44000,"startTime":1651113627000,"endTime":1651113671000},{"activityDate":1651113963000,"code":3,"duration":283000,"startTime":1651113680000,"endTime":1651113963000},{"activityDate":1651114155000,"code":3,"duration":117000,"startTime":1651114038000,"endTime":1651114155000},{"activityDate":1651114399000,"code":3,"duration":160000,"startTime":1651114239000,"endTime":1651114399000},{"activityDate":1651114423000,"code":3,"duration":14000,"startTime":1651114409000,"endTime":1651114423000},{"activityDate":1651115754000,"code":3,"duration":188000,"startTime":1651115566000,"endTime":1651115754000},{"activityDate":1651115961000,"code":3,"duration":69000,"startTime":1651115892000,"endTime":1651115961000},{"activityDate":1651117662000,"code":3,"duration":1670000,"startTime":1651115992000,"endTime":1651117662000},{"activityDate":1651119112000,"code":3,"duration":1433000,"startTime":1651117679000,"endTime":1651119112000},{"activityDate":1651119170000,"code":3,"duration":44000,"startTime":1651119126000,"endTime":1651119170000},{"activityDate":1651119462000,"code":3,"duration":283000,"startTime":1651119179000,"endTime":1651119462000},{"activityDate":1651119653000,"code":3,"duration":116000,"startTime":1651119537000,"endTime":1651119653000},{"activityDate":1651119897000,"code":3,"duration":159000,"startTime":1651119738000,"endTime":1651119897000},{"activityDate":1651119922000,"code":3,"duration":14000,"startTime":1651119908000,"endTime":1651119922000},{"activityDate":1651121253000,"code":3,"duration":189000,"startTime":1651121064000,"endTime":1651121253000},{"activityDate":1651121390000,"code":3,"duration":61000,"startTime":1651121329000,"endTime":1651121390000},{"activityDate":1651121459000,"code":3,"duration":69000,"startTime":1651121390000,"endTime":1651121459000},{"activityDate":1651123144000,"code":3,"duration":1654000,"startTime":1651123144000,"endTime":1651123144000},{"activityDate":1651123160000,"code":3,"duration":16000,"startTime":1651123144000,"endTime":1651123160000},{"activityDate":1651124611000,"code":3,"duration":1433000,"startTime":1651123178000,"endTime":1651124611000},{"activityDate":1651124669000,"code":3,"duration":45000,"startTime":1651124624000,"endTime":1651124669000},{"activityDate":1651124960000,"code":3,"duration":282000,"startTime":1651124678000,"endTime":1651124960000},{"activityDate":1651125152000,"code":3,"duration":117000,"startTime":1651125035000,"endTime":1651125152000},{"activityDate":1651125396000,"code":3,"duration":160000,"startTime":1651125236000,"endTime":1651125396000},{"activityDate":1651125420000,"code":3,"duration":14000,"startTime":1651125406000,"endTime":1651125420000},{"activityDate":1651126563000,"code":3,"duration":0,"startTime":1651126563000,"endTime":1651126563000},{"activityDate":1651126751000,"code":3,"duration":188000,"startTime":1651126563000,"endTime":1651126751000},{"activityDate":1651126889000,"code":3,"duration":61000,"startTime":1651126828000,"endTime":1651126889000},{"activityDate":1651126958000,"code":3,"duration":69000,"startTime":1651126889000,"endTime":1651126958000},{"activityDate":1651128659000,"code":3,"duration":1670000,"startTime":1651126989000,"endTime":1651128659000},{"activityDate":1651130110000,"code":3,"duration":1434000,"startTime":1651128676000,"endTime":1651130110000},{"activityDate":1651130167000,"code":3,"duration":44000,"startTime":1651130123000,"endTime":1651130167000},{"activityDate":1651130459000,"code":3,"duration":283000,"startTime":1651130176000,"endTime":1651130459000},{"activityDate":1651130651000,"code":3,"duration":117000,"startTime":1651130534000,"endTime":1651130651000},{"activityDate":1651130895000,"code":3,"duration":160000,"startTime":1651130735000,"endTime":1651130895000},{"activityDate":1651130919000,"code":3,"duration":14000,"startTime":1651130905000,"endTime":1651130919000},{"activityDate":1651132250000,"code":3,"duration":189000,"startTime":1651132061000,"endTime":1651132250000},{"activityDate":1651132457000,"code":3,"duration":69000,"startTime":1651132388000,"endTime":1651132457000},{"activityDate":1651134158000,"code":3,"duration":1670000,"startTime":1651132488000,"endTime":1651134158000},{"activityDate":1651135608000,"code":3,"duration":1433000,"startTime":1651134175000,"endTime":1651135608000},{"activityDate":1651135666000,"code":3,"duration":45000,"startTime":1651135621000,"endTime":1651135666000},{"activityDate":1651135958000,"code":3,"duration":283000,"startTime":1651135675000,"endTime":1651135958000},{"activityDate":1651136149000,"code":3,"duration":116000,"startTime":1651136033000,"endTime":1651136149000},{"activityDate":1651136393000,"code":3,"duration":160000,"startTime":1651136233000,"endTime":1651136393000},{"activityDate":1651136418000,"code":3,"duration":15000,"startTime":1651136403000,"endTime":1651136418000},{"activityDate":1651137748000,"code":3,"duration":188000,"startTime":1651137560000,"endTime":1651137748000},{"activityDate":1651137955000,"code":3,"duration":69000,"startTime":1651137886000,"endTime":1651137955000},{"activityDate":1651139656000,"code":3,"duration":1670000,"startTime":1651137986000,"endTime":1651139656000},{"activityDate":1651141107000,"code":3,"duration":1434000,"startTime":1651139673000,"endTime":1651141107000},{"activityDate":1651141164000,"code":3,"duration":44000,"startTime":1651141120000,"endTime":1651141164000},{"activityDate":1651141456000,"code":3,"duration":283000,"startTime":1651141173000,"endTime":1651141456000},{"activityDate":1651141648000,"code":3,"duration":117000,"startTime":1651141531000,"endTime":1651141648000},{"activityDate":1651141892000,"code":3,"duration":160000,"startTime":1651141732000,"endTime":1651141892000},{"activityDate":1651141916000,"code":3,"duration":14000,"startTime":1651141902000,"endTime":1651141916000},{"activityDate":1651143247000,"code":3,"duration":188000,"startTime":1651143059000,"endTime":1651143247000},{"activityDate":1651143385000,"code":3,"duration":61000,"startTime":1651143324000,"endTime":1651143385000},{"activityDate":1651143454000,"code":3,"duration":69000,"startTime":1651143385000,"endTime":1651143454000},{"activityDate":1651145155000,"code":3,"duration":1670000,"startTime":1651143485000,"endTime":1651145155000},{"activityDate":1651146606000,"code":3,"duration":1434000,"startTime":1651145172000,"endTime":1651146606000},{"activityDate":1651146663000,"code":3,"duration":44000,"startTime":1651146619000,"endTime":1651146663000},{"activityDate":1651146955000,"code":3,"duration":283000,"startTime":1651146672000,"endTime":1651146955000},{"activityDate":1651147147000,"code":3,"duration":117000,"startTime":1651147030000,"endTime":1651147147000},{"activityDate":1651147391000,"code":3,"duration":160000,"startTime":1651147231000,"endTime":1651147391000},{"activityDate":1651147415000,"code":3,"duration":14000,"startTime":1651147401000,"endTime":1651147415000},{"activityDate":1651148746000,"code":3,"duration":189000,"startTime":1651148557000,"endTime":1651148746000},{"activityDate":1651148952000,"code":3,"duration":69000,"startTime":1651148883000,"endTime":1651148952000},{"activityDate":1651150654000,"code":3,"duration":1670000,"startTime":1651148984000,"endTime":1651150654000},{"activityDate":1651152104000,"code":3,"duration":1433000,"startTime":1651150671000,"endTime":1651152104000},{"activityDate":1651152162000,"code":3,"duration":45000,"startTime":1651152117000,"endTime":1651152162000},{"activityDate":1651152453000,"code":3,"duration":282000,"startTime":1651152171000,"endTime":1651152453000},{"activityDate":1651152645000,"code":3,"duration":116000,"startTime":1651152529000,"endTime":1651152645000},{"activityDate":1651152889000,"code":3,"duration":160000,"startTime":1651152729000,"endTime":1651152889000},{"activityDate":1651152913000,"code":3,"duration":14000,"startTime":1651152899000,"endTime":1651152913000},{"activityDate":1651154244000,"code":3,"duration":132000,"startTime":1651154112000,"endTime":1651154244000},{"activityDate":1651154382000,"code":3,"duration":61000,"startTime":1651154321000,"endTime":1651154382000},{"activityDate":1651154451000,"code":3,"duration":69000,"startTime":1651154382000,"endTime":1651154451000},{"activityDate":1651156152000,"code":3,"duration":1670000,"startTime":1651154482000,"endTime":1651156152000},{"activityDate":1651157603000,"code":3,"duration":1434000,"startTime":1651156169000,"endTime":1651157603000},{"activityDate":1651157660000,"code":3,"duration":44000,"startTime":1651157616000,"endTime":1651157660000},{"activityDate":1651157952000,"code":3,"duration":283000,"startTime":1651157669000,"endTime":1651157952000},{"activityDate":1651158144000,"code":3,"duration":117000,"startTime":1651158027000,"endTime":1651158144000},{"activityDate":1651158388000,"code":3,"duration":160000,"startTime":1651158228000,"endTime":1651158388000},{"activityDate":1651158412000,"code":3,"duration":14000,"startTime":1651158398000,"endTime":1651158412000},{"activityDate":1651159743000,"code":3,"duration":189000,"startTime":1651159554000,"endTime":1651159743000},{"activityDate":1651159949000,"code":3,"duration":69000,"startTime":1651159880000,"endTime":1651159949000},{"activityDate":1651161651000,"code":3,"duration":1670000,"startTime":1651159981000,"endTime":1651161651000},{"activityDate":1651163101000,"code":3,"duration":1433000,"startTime":1651161668000,"endTime":1651163101000},{"activityDate":1651163159000,"code":3,"duration":45000,"startTime":1651163114000,"endTime":1651163159000},{"activityDate":1651163450000,"code":3,"duration":282000,"startTime":1651163168000,"endTime":1651163450000},{"activityDate":1651163642000,"code":3,"duration":116000,"startTime":1651163526000,"endTime":1651163642000},{"activityDate":1651163886000,"code":3,"duration":160000,"startTime":1651163726000,"endTime":1651163886000},{"activityDate":1651163910000,"code":3,"duration":14000,"startTime":1651163896000,"endTime":1651163910000},{"activityDate":1651165241000,"code":3,"duration":188000,"startTime":1651165053000,"endTime":1651165241000},{"activityDate":1651165379000,"code":3,"duration":61000,"startTime":1651165318000,"endTime":1651165379000},{"activityDate":1651165448000,"code":3,"duration":69000,"startTime":1651165379000,"endTime":1651165448000},{"activityDate":1651167149000,"code":3,"duration":1670000,"startTime":1651165479000,"endTime":1651167149000},{"activityDate":1651168600000,"code":3,"duration":1434000,"startTime":1651167166000,"endTime":1651168600000},{"activityDate":1651168657000,"code":3,"duration":44000,"startTime":1651168613000,"endTime":1651168657000},{"activityDate":1651168949000,"code":3,"duration":283000,"startTime":1651168666000,"endTime":1651168949000},{"activityDate":1651169141000,"code":3,"duration":117000,"startTime":1651169024000,"endTime":1651169141000},{"activityDate":1651169385000,"code":3,"duration":160000,"startTime":1651169225000,"endTime":1651169385000},{"activityDate":1651169409000,"code":3,"duration":14000,"startTime":1651169395000,"endTime":1651169409000},{"activityDate":1651170551000,"code":3,"duration":0,"startTime":1651170551000,"endTime":1651170551000},{"activityDate":1651170740000,"code":3,"duration":189000,"startTime":1651170551000,"endTime":1651170740000},{"activityDate":1651170877000,"code":3,"duration":60000,"startTime":1651170817000,"endTime":1651170877000},{"activityDate":1651170946000,"code":3,"duration":69000,"startTime":1651170877000,"endTime":1651170946000},{"activityDate":1651172648000,"code":3,"duration":1670000,"startTime":1651170978000,"endTime":1651172648000},{"activityDate":1651174098000,"code":3,"duration":1433000,"startTime":1651172665000,"endTime":1651174098000},{"activityDate":1651174156000,"code":3,"duration":45000,"startTime":1651174111000,"endTime":1651174156000},{"activityDate":1651174448000,"code":3,"duration":283000,"startTime":1651174165000,"endTime":1651174448000},{"activityDate":1651174639000,"code":3,"duration":116000,"startTime":1651174523000,"endTime":1651174639000},{"activityDate":1651174883000,"code":3,"duration":160000,"startTime":1651174723000,"endTime":1651174883000},{"activityDate":1651174907000,"code":3,"duration":14000,"startTime":1651174893000,"endTime":1651174907000},{"activityDate":1651176239000,"code":3,"duration":189000,"startTime":1651176050000,"endTime":1651176239000},{"activityDate":1651176445000,"code":3,"duration":69000,"startTime":1651176376000,"endTime":1651176445000},{"activityDate":1651178147000,"code":3,"duration":1670000,"startTime":1651176477000,"endTime":1651178147000},{"activityDate":1651179597000,"code":3,"duration":1433000,"startTime":1651178164000,"endTime":1651179597000},{"activityDate":1651179655000,"code":3,"duration":45000,"startTime":1651179610000,"endTime":1651179655000},{"activityDate":1651179947000,"code":3,"duration":283000,"startTime":1651179664000,"endTime":1651179947000},{"activityDate":1651180138000,"code":3,"duration":116000,"startTime":1651180022000,"endTime":1651180138000},{"activityDate":1651180382000,"code":3,"duration":160000,"startTime":1651180222000,"endTime":1651180382000},{"activityDate":1651180406000,"code":3,"duration":14000,"startTime":1651180392000,"endTime":1651180406000},{"activityDate":1651181737000,"code":3,"duration":188000,"startTime":1651181549000,"endTime":1651181737000},{"activityDate":1651181944000,"code":3,"duration":69000,"startTime":1651181875000,"endTime":1651181944000},{"activityDate":1651098910000,"code":7,"duration":0,"startTime":1651098910000,"endTime":1651098910000},{"activityDate":1651100026000,"code":7,"duration":1116000,"startTime":1651100026000,"endTime":1651100026000},{"activityDate":1651105525000,"code":7,"duration":1117000,"startTime":1651105525000,"endTime":1651105525000},{"activityDate":1651110067000,"code":7,"duration":2397000,"startTime":1651110067000,"endTime":1651110067000},{"activityDate":1651114449000,"code":7,"duration":0,"startTime":1651114449000,"endTime":1651114449000},{"activityDate":1651115566000,"code":7,"duration":1117000,"startTime":1651115566000,"endTime":1651115566000},{"activityDate":1651121064000,"code":7,"duration":1117000,"startTime":1651121064000,"endTime":1651121064000},{"activityDate":1651125447000,"code":7,"duration":0,"startTime":1651125447000,"endTime":1651125447000},{"activityDate":1651126563000,"code":7,"duration":1116000,"startTime":1651125447000,"endTime":1651126563000},{"activityDate":1651132061000,"code":7,"duration":1116000,"startTime":1651132061000,"endTime":1651132061000},{"activityDate":1651136443000,"code":7,"duration":0,"startTime":1651136443000,"endTime":1651136443000},{"activityDate":1651137560000,"code":7,"duration":1117000,"startTime":1651137560000,"endTime":1651137560000},{"activityDate":1651143059000,"code":7,"duration":1117000,"startTime":1651143059000,"endTime":1651143059000},{"activityDate":1651148557000,"code":7,"duration":1116000,"startTime":1651148557000,"endTime":1651148557000},{"activityDate":1651154112000,"code":7,"duration":1173000,"startTime":1651154112000,"endTime":1651154112000},{"activityDate":1651158437000,"code":7,"duration":0,"startTime":1651158437000,"endTime":1651158437000},{"activityDate":1651159554000,"code":7,"duration":1117000,"startTime":1651159554000,"endTime":1651159554000},{"activityDate":1651165053000,"code":7,"duration":1117000,"startTime":1651165053000,"endTime":1651165053000},{"activityDate":1651170551000,"code":7,"duration":1117000,"startTime":1651169434000,"endTime":1651170551000},{"activityDate":1651176050000,"code":7,"duration":1117000,"startTime":1651176050000,"endTime":1651176050000},{"activityDate":1651180432000,"code":7,"duration":0,"startTime":1651180432000,"endTime":1651180432000},{"activityDate":1651181549000,"code":7,"duration":1117000,"startTime":1651181549000,"endTime":1651181549000}],"code":200,"message":"Driver details fetched successfully for requested Filters"}');
      this.createChart(data);
    })
    // this.updateDataSource(this.detailConvertedData);
    this.setGraphData();
  }

  createChart(data){
    let _data = data['driverActivitiesChartData'];
    // this.chartData = _data.sort((a, b) => parseInt(a.activityDate) - parseInt(b.activityDate));
    let _series = [];

    // let restArray =_data.filter(item => item.code === 0);
    // let availableArray =_data.filter(item => item.code === 1);
    // let workArray =_data.filter(item => item.code === 2);
    // let driveArray =_data.filter(item => item.code === 3);

    // let restData = [];
    // restArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor: '#8ac543',
    //   }
    //   restData.push(restObj)
    // });
  
    // let availableData = [];
    // availableArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor : '#dddee2'
    //   }
    //   availableData.push(restObj)
    // });

    // let workData = [];
    // workArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor : '#e85c2a'
    //   }
    //   workData.push(restObj)
    // });

    // let driveData = [];
    // driveArray.forEach(element => {
    //   let _startTime = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
    //   let _endTime = Util.convertUtcToDateTZ(element.endTime,this.prefTimeZone);
    //   let restObj={
    //     x :  this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
    //     y : [_startTime,_endTime],
    //     fillColor : '#29539b'
    //   }
    //   driveData.push(restObj)
    // });
    
    let driveData=[], workData=[], restData=[], availableData=[];
    let startTime: any;
    let currentArray;
    let newObj=[];
    this.dayWiseSummaryList=[];
    _data.forEach(element => {
      // let _startTime1 = Util.convertUtcToDateTZ(element.startTime,this.prefTimeZone);
      // let _startTime = Util.convertUtcToHour(element.startTime,this.prefTimeZone);
      // let _endTime = Util.convertUtcToHour(element.endTime,this.prefTimeZone);
      // let _startTime1 = Util.getMillisecondsToUTCDate(new Date(element.startTime), this.prefTimeZone);
      // let _startTime = Util.convertMilisecondsToHHMM(_startTime1);
      // let _endTime1 = Util.getMillisecondsToUTCDate(new Date(element.endTime), this.prefTimeZone);
      // let _endTime = Util.convertMilisecondsToHHMM(_endTime1);
      // let _startTimeDate = Util.convertUtcToDateStart(element.startTime, this.prefTimeZone);
      var now = new Date(element.startTime);
      var utc = new Date(now.getTime() + now.getTimezoneOffset() * 60000);

      // let _startTimeDate = Util.getMillisecondsToUTCDate(utc, this.prefTimeZone);
      // console.log('actual:tzconv =>'+element.startTime+':'+ _startTimeDate );

     
    let _startTimeDate = this.reportMapService.formStartendDate(Util.convertUtcToDate(element.startTime, this.prefTimeZone), '', 24, false, false, false, true);
    let _startTime = this.reportMapService.formStartendDate(Util.convertUtcToDate(element.startTime, this.prefTimeZone), '', 24, false, true, true, false)
    let _endTime = this.reportMapService.formStartendDate(Util.convertUtcToDate(element.endTime, this.prefTimeZone), '', 24, false, true, true, false);

      // let _startTimeDate = element.startTime;
      let isValid=true;
      if(_startTime == _endTime || (_startTime) > (_endTime)){  
        isValid=false;
      }
      // //console.log(this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false));
      if(isValid && element.duration > 0){
        // //console.log('start'+element.startTime+' end '+element.endTime+' activity'+element.activityDate+' duration'+element.duration);
        
        // startTime=Util.convertUtcToDateFormat2(_startTimeDate, this.prefTimeZone);------
      //   let formatDate = Util.convertUtcToDateAndTimeFormat(_startTimeDate, this.prefTimeZone,this.dateFormats);
      //  startTime = formatDate[0];
      let tooltip;
      let day, month, year;
      if(_startTimeDate){
         day=(_startTimeDate.toString()).substring(0,2);
         month=(_startTimeDate.toString()).substring(2,4);
         year=(_startTimeDate.toString()).substring(4);
        tooltip= month+'/'+day+'/'+year;
      }
      //  console.log(newStartDate+' '+startTime);
        // startTime=this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false);
        // //console.log('sta'+_startTime+' end'+_endTime+' sa'+Util.convertUtcToDateStart(element.startTime, this.prefTimeZone)+' ac'+Util.convertUtcToDateFormat2(element.activityDate, this.prefTimeZone));
        // console.log(_startTimeDate+' '+startTime);
        let newStartDate = (Util.convertUtcToDate(element.startTime, this.prefTimeZone));
        let newdt = new Date(newStartDate);
//         newStartDate.set({hour:0,minute:0,second:0,millisecond:0});
// newStartDate.toISOString();
// newStartDate.format();
console.log(Date.UTC(year, month, day));
        // console.log('convert12.00:'+newStartDate+' tooltipdate:'+startTime+' actual:tzconv =>'+element.startTime+':'+ _startTimeDate +' drstarttm:'+_startTime+' drendTm:'+_endTime);
        let restObj={
          x: Date.UTC(year, month-1, day),
          // x :  Number(_startTimeDate.toString().substring(0,4)),
          // x: 1234567890,
          // x: _startTimeDate,
          // actualDate: this.reportMapService.getStartTime(element.activityDate,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false),
          actualDate: tooltip,
          duration: element.duration,
          low : _startTime,
          high:_endTime
        }
        const found = this.dayWiseSummaryList.some(el => el.startTime === startTime);
        if (!found) this.dayWiseSummaryList.push({ startTime: startTime, restTime: 0,  availableTime: 0, workTime: 0, driveTime: 0});
        currentArray=this.dayWiseSummaryList.filter(el => el.startTime === startTime)[0];
        // //console.log(currentArray[0].date+ ' ' + currentArray[0].restTime + ' ' + currentArray[0].workTime + ' ' + currentArray[0].availableTime + ' ' + currentArray[0].serviceTime);
        if(element.code === 0){
          restObj['color']='#8ac543';
          restObj['type']=this.translationData.lblRest;
          // restData.push(restObj);
          newObj.push(restObj);
          currentArray['restTime'] = currentArray.restTime + element.duration;
        } else if(element.code === 1){
          restObj['color']='#dddee2';
          restObj['type']=this.translationData.lblAvailable;
          // availableData.push(restObj);
          newObj.push(restObj);
          currentArray['availableTime']= currentArray.availableTime + element.duration;
        } else if(element.code === 2){
          restObj['color']='#fc5f01';
          restObj['type']=this.translationData.lblWork;
          // workData.push(restObj);
          newObj.push(restObj);
          currentArray['workTime']= currentArray.workTime + element.duration;
        } else if(element.code === 3){
          restObj['color']='#00529b';
          restObj['type']=this.translationData.lblDrive;
          // driveData.push(restObj);
          newObj.push(restObj);
          currentArray['driveTime']= currentArray.driveTime + element.duration;
        }
        // //console.log(currentArray.date+ ' ' + currentArray.restTime + ' ' + currentArray.workTime + ' ' + currentArray.availableTime + ' ' + currentArray.serviceTime);
    }
    });
    let totDriveTime=0;
    let totAvailableTime=0;
    let totWorkTime=0;
    let totRestTime=0;
    let totServiceTime=0;
    let transFrom = this.translationData.lblFrom;
    let transTo = this.translationData.lblTo;
    let transDuration = this.translationData.lblDuration;

    this.dayWiseSummaryList.forEach(element => {
      totDriveTime += element.driveTime;
      totAvailableTime += element.availableTime;
      totWorkTime += element.workTime;
      totRestTime += element.restTime;
      totServiceTime += element.availableTime + element.workTime + element.driveTime;
      element['serviceTime'] = Util.getHhMmTimeFromMS(element.availableTime + element.workTime + element.driveTime);
      element['restTime'] = Util.getHhMmTimeFromMS(element.restTime);
      element['availableTime'] = Util.getHhMmTimeFromMS(element.availableTime);
      element['workTime'] = Util.getHhMmTimeFromMS(element.workTime);
      element['driveTime'] = Util.getHhMmTimeFromMS(element.driveTime);
      // //console.log(element);
    });
    this.totalDriveTime = Util.getHhMmTimeFromMS(totDriveTime);
    this.totalAvailableTime = Util.getHhMmTimeFromMS(totAvailableTime);
    this.totalWorkTime = Util.getHhMmTimeFromMS(totWorkTime);
    this.totalRestTime = Util.getHhMmTimeFromMS(totRestTime);
    this.totalServiceTime = Util.getHhMmTimeFromMS(totServiceTime);
    const tz=this.prefTimeZone;
    this.updateDataSource(this.dayWiseSummaryList);
      // //console.log("newObj" +JSON.stringify(newObj));
      this.chartOptions = {
        title: {
          enabled: false,
          text: ''
        },
        chart: {
          type: 'columnrange',
          inverted: true,
          zoomType: 'y',
          panning: true,
          panKey: 'shift'
        },
        credits: {
          enabled: false
        },
        style: {
          fontFamily: 'fontAwesome'
        },
        tooltip:{
          formatter(e){
            var symbol = '';
            if (this.point) {
                switch ( this.point.type) {
                    case 'Work':
                        symbol = '<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/work.svg" style="width: 16px; height: 16px;" />';
                        break;
                    case 'Rest':
                        symbol = '<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/rest.svg" style="width: 16px; height: 16px;" />';
                        break;
                    case 'Drive':
                        symbol = '<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/drive.svg" style="width: 16px; height: 16px;" />';
                        break;
                    case 'Available':                      
                        symbol='<img matTooltip="activity" class="mr-1" src="/assets/activityIcons/available.svg" style="width: 16px; height: 16px;" />'
                        break;                            
                    }
                }
                return (
               '<div class="driveChartTT" style="border: 0px;"><div style="font-weight: bold;"><span style="font-size: 15px;">' +
                symbol + '</span>'+ this.point.type +'</div>'+
               '<div>'+transFrom+':'+ this.point.actualDate +'&nbsp;&nbsp;'+ this.point.low +'</div>'+
               '<div>'+transTo+':'+ this.point.actualDate+'&nbsp;&nbsp;'+this.point.high +'</div>'+
               '<div>'+transDuration+':' + Util.getTimeHHMMSS(this.point.duration)+'</div>'+
               '</div>'
            )
        },  useHTML: true},
        series: [{
          data: newObj
        }],
        //   data: [{"x":1626805800000,"actualDate":"07/21/2021","duration":604000,"low":1.5,"high":8.53,"color":"blue","type":"Work"},{"x":1626805800000,"actualDate":"07/21/2021","duration":263926000,"low":7.8,"high":8.27,"color":"blue","type":"Work"},{"x":1626892200000,"actualDate":"07/22/2021","duration":44000,"low":2.53,"high":6.54,"color":"orange","type":"Drive"},{"x":1626892200000,"actualDate":"07/22/2021","duration":44000,"low":15.53,"high":22.54,"color":"orange","type":"Drive"}]
        // }],
        //   data: [{"x":1625423400000,"duration":10899000,"low":0.1,"high":0.21,"color":"orange","type":"Drive"},{"x":1625423400000,"duration":1998000,"low":0.21,"high":0.25,"color":"orange","type":"Drive"}]
        // }],
          // data: JSON.stringify(newObj)        }],
        // series: [{
        //   data: [{
        //     color: "blue",
        //       high: 15,
        //       low: 12,
        //       x: 1625509800000,
        //   },{
        //     color: "red",
        //     high: 17,
        //     low: 16,
        //     x: 1625337000000,
        //   }
        // ]
        // }],
        // series : [{
        //   data : [{
        //     low: 1262336400000,
        //     high: 1262347200000,
        //     color: 'blue',
        //     x: 1262336400000
        //   }]
        // }],
        // series: _seriesN,
        // series : [{data:[ {
        //           x: 2,
        //           low: 1262336400000,//Date.UTC(2010, 0, 1, 9, 0, 0),
        //           high: 1262347200000,//Date.UTC(2010, 0, 1, 12, 0, 0),
        //           color: 'blue'
        //         }]}],
        // series: newObj,
        // series: [{
        //   data: [{
        //       x: 0,
        //       low: Date.UTC(2010, 0, 1, 9, 0, 0),
        //       high: Date.UTC(2010, 0, 1, 12, 0, 0),
        //       color: 'blue'
        //     }
    
        //   ]
        // }],
        plotOptions: {
          series: {
            pointWidth: 16,
          }
        },
        xAxis: {
          type: 'datetime',
          // categories: ['4/21/2022','4/22/2022','4/23/2022','4/24/2022','4/25/2022','4/26/2022','4/27/2022','4/28/2022'],
          tickInterval: 24 * 3600 * 1000,
      labels: {
        format: '{value:%m/%e/%Y}'
      },
        //   labels: {
        //     formatter: function(s, a) {
        //         //return Util.convertUtcToDateFormat2(this.value, tz);
        //         // let date1 = new Date(this.value);
        //         // return this.point.actualDate;
        //         // if(this.value){
        //         //   let day=((this.value).toString()).substring(0,2);
        //         //   let month=((this.value).toString()).substring(2,4);
        //         //   let year=((this.value).toString()).substring(4);
        //         //   return month+'/'+day+'/'+year;
        //         // }
        //         this.point;
        //         // return (date1.getMonth() + 1) + "/" + date1.getDate() + "/" + date1.getFullYear() ;
        //         return this.value;
        //     },
        //     align: 'center',
        //     alignTicks: true
        // },
        // lineWidth: 2,
        // tickInterval: 24*3600*1000*1,
        // visible: false,
        // tickInterval: 3600*1000*24,
        // lineWidth: 0,
        //   minorGridLineWidth: 0,
        //   lineColor: 'transparent',
        },
        yAxis: {
          type: 'numeric',
          min: 0,
          max: 24,
          tickInterval: 1,
          lineWidth: 0,
          minorGridLineWidth: 0,
          lineColor: 'transparent',
          title: false,
        },
        toolbar: {          
        },
        legend: {
          enabled: false
        },
      //   legend :{
      //     useHTML: true,
      //     symbolWidth: 0,
      //     labelFormatter: function () {
      //                  return '<div><div class="legend-color" style="background: rgb(0, 82, 155); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Drive &nbsp;&nbsp;</div>'+
      //                  '<div class="legend-color" style="background: rgb(252, 95, 1); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Work &nbsp;&nbsp;</div>'+
      //                  '<div class="legend-color" style="background: rgb(138, 197, 67); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Rest &nbsp;&nbsp;</div>'+
      //                  '<div class="legend-color" style="background: rgb(221, 222, 226); display: inline-block; height: 17px; width: 30px;"></div><div style="display: inline-block; font: 16px Arial,Helvetica,sans-serif; font-weight: bold;"> Available &nbsp;&nbsp;</div></div>'
      //              }

      // },
      }
      // this.setGraphData();
      // this.showLoadingIndicator=false;
  }

  setPrefFormatDate(){
    switch(this.prefDateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        this.dateFormats = "DD/MM/YYYY"; 
        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        this.dateFormats = "MM/DD/YYYY";
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        this.dateFormats = "DD-MM-YYYY";  
        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        this.dateFormats = "MM-DD-YYYY";
        break;
      }
      default: {
        this.dateFormats = "MM/DD/YYYY";
      }
    }
  }


  setGraphData(){
    let dateArray = this.detailConvertedData.map(data=>data.startTime);
    let driveTimeArray = this.detailConvertedData.map(data=>data.driveTime);
    let workTimeArray = this.detailConvertedData.map(data=>data.workTime);
    let restTimeArray = this.detailConvertedData.map(data=>data.restTime);
    let availableTimeArray = this.detailConvertedData.map(data=>data.availableTime);
    
    this.barChartLabels = dateArray;
  }

  getActualDate(_utc: any){
    let date=this.reportMapService.getStartTime(_utc,this.prefDateFormat,this.prefTimeFormat,this.prefTimeZone,false,false);
    //console.log(date);
    return date;
  }

  // totalDriveTime =0;
  // totalWorkTime =0;
  // totalRestTime =0;
  // totalAvailableTime =0;
  selectedDriverName = '';
  selectedDriverId = '';
  tableInfoObj = {};
 
  onZoomReset(){
    //console.log('reset')
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  public chartClicked({ event, active }: { event: MouseEvent, active: {}[] }): void {
    ////console.log(event, active);
  }

  public chartHovered({ event, active }: { event: MouseEvent, active: {}[] }): void {
    ////console.log(event, active);
  }
  
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getPDFExcelHeader(){
    let col: any = [];
    col = [`${this.translationData.lblDate || 'Date'}`, `${this.translationData.lblDriveTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblWorkTime + ' (' + this.translationData.lblhhmm + ')'}`, `${this.translationData.lblServiceTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblRestTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblAvailableTime + ' (' + this.translationData.lblhhmm + ')' }`];
    return col;
  }
  
  getExcelSummaryHeader(){
    let col: any = [];
    col = [`${this.translationData.lblReportName || 'Report Name'}`, `${this.translationData.lblReportCreated || 'Report Created'}`, `${this.translationData.lblReportStartTime || 'Report Start Time'}`, `${this.translationData.lblReportEndTime || 'Report End Time' }`, `${this.translationData.lblDriverName || 'Driver Name' }`, `${this.translationData.lblDriverId || 'Driver Id' }`, `${this.translationData.lblTotalDriveTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalWorkTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalAvailableTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalRestTime + ' (' + this.translationData.lblhhmm + ')' }`, `${this.translationData.lblTotalServiceTime + ' (' + this.translationData.lblhhmm + ')' }`];
    return col;
  }

  exportAsExcelFile(){    
  const title = this.translationData.lblDriverTimeReportDetails;
  const summary = this.translationData.lblSummarySection;
  const detail = this.translationData.lblAllDetails;
  // const header = ['Date', 'Drive Time(hh:mm)', 'Work Time(hh:mm)', 'Service Time(hh:mm)', 'Rest Time(hh:mm)', 'Available Time(hh:mm)'];
  // const summaryHeader = ['Report Name', 'Report Created', 'Report Start Time', 'Report End Time', 'Driver Name', 'Driver Id', 'Total Drive Time(hh:mm)', 'Total Work Time(hh:mm)', 'Total Available Time(hh:mm)', 'Total Rest Time(hh:mm)', 'Total Service Time(hh:mm)'];
  const header = this.getPDFExcelHeader();
  const summaryHeader = this.getExcelSummaryHeader();
  this.summaryObj=[
    [this.translationData.lblDriverTimeReportDetails, new Date(), this.driverTableInfoObj.fromDisplayDate, this.driverTableInfoObj.toDisplayDate,
    this.driverDetails.vehicleGroupName, this.driverDetails.vin, this.driverTableInfoObj.driveTime, this.driverTableInfoObj.workTime, 
    this.driverTableInfoObj.availableTime, this.driverTableInfoObj.restTime, this.driverTableInfoObj.serviceTime
    ]
  ];
  const summaryData= this.summaryObj;
  
  //Create workbook and worksheet
  let workbook = new Workbook();
  let worksheet = workbook.addWorksheet('Driver Details Time Report');
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
    worksheet.addRow([item.startTime,item.driveTime, item.workTime,item.serviceTime,
      item.restTime, item.availableTime,]);   
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
    fs.saveAs(blob, 'Driver_Details_Time_Report.xlsx');
 })
  // this.matTableExporter.exportTable('xlsx', {fileName:'Driver_Details_Time_Report', sheet: 'sheet_name'});
}

  exportAsPDFFile(){
    var imgleft;
    if (this.brandimagePath != null) {
      imgleft = this.brandimagePath.changingThisBreaksApplicationSecurity;
    } else {
      imgleft = "/assets/Daf-NewLogo.png";
      // let defaultIcon: any = "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 dZTWn+gq/wcifZTYZGl3fQAAAABJRU5ErkJggg==";
      // let sanitizedData: any= this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpeg;base64,' + defaultIcon);
      // imgleft = sanitizedData.changingThisBreaksApplicationSecurity;
    }

   
    var doc = new jsPDF();

    // doc.setFontSize(18);
    // doc.text(this.translationData.lblDriverTimeReportDetails, 11, 8);
    doc.setFontSize(11);
    doc.setTextColor(100);

    let src;
    let ohref;
    let summaryHeight;
    let chartHeight;
    let oWidth = 175;
    let summaryHref;
    let chartHref;

    let summaryArea = document.getElementById('summaryArea');
    let chartArea = document.getElementById('chartArea');

    html2canvas(summaryArea).then((canvas) => {
      summaryHeight = (canvas.height * oWidth) / canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      summaryHref = canvas.toDataURL('image/png');
    });
    html2canvas(chartArea).then((canvas) => {
      chartHeight = (canvas.height * oWidth) / canvas.width;
      //oWidth= canvas.width;
      src = canvas.toDataURL();
      chartHref = canvas.toDataURL('image/png');
    });
   // let pdfColumns = [['Start Date', 'End Date', 'Distance', 'Idle Duration', 'Average Speed', 'Average Weight', 'Start Position', 'End Position', 'Fuel Consumed100Km', 'Driving Time', 'Alert', 'Events']];

    // let pdfColumns = [['Date', 'Drive Time', 'Work Time', 'Service Time', 'Rest Time', 'Available Time']]
    let pdfColumns = this.getPDFExcelHeader();
    pdfColumns = [pdfColumns];
    let prepare = []
    this.initData.forEach(e=>{
      var tempObj =[];
      tempObj.push(e.startTime);
      tempObj.push(e.driveTime);
      tempObj.push(e.workTime);
      tempObj.push(e.serviceTime);
      tempObj.push(e.restTime);
      tempObj.push(e.availableTime);

      prepare.push(tempObj);
    });
    let fileTitle = this.translationData.lblDriverTimeReportDetails;
    html2canvas(chartArea, { scale: 2 }).then(() => {
      (doc as any).autoTable({
        styles: {
          cellPadding: 0.5,
          fontSize: 12,
        },
        didDrawPage: function (data) {
          // Header
          doc.setFontSize(14);
          // var fileTitle = this.translationData.lblPdfTitle;
          // var fileTitle = 'pdf';
          // var img = '/assets/logo.png';
          // doc.addImage(img, 'JPEG', 10, 10, 0, 0);
          doc.addImage(imgleft, 'JPEG', 10, 10, 0, 17.5);

          var img = '/assets/logo_daf.png';
          doc.text(fileTitle, 14, 35);
          doc.addImage(img, 'JPEG', 150, 10, 0, 10);
        },
        margin: {
          bottom: 30,
          top: 40,
        },
      });
    // doc.addPage('a4', 'p');
    doc.addImage(summaryHref, 'PNG', 10, 40, oWidth, summaryHeight);
    doc.addPage('a4', 'p');
    doc.addImage(chartHref, 'PNG', 10, 40, oWidth, chartHeight);
    doc.addPage('a2', 'p');
    (doc as any).autoTable({
      head: pdfColumns,
      body: prepare,
      theme: 'striped',
      didDrawCell: data => {
        ////console.log(data.column.index)
      }
    });
    doc.save('DriverDetailsTimeReport.pdf');
  });

    // below line for Download PDF document  
    // doc.save('DriverDetailsTimeReport.pdf');
  }

  pageSizeUpdated(_evt){}
  
  backToMainPageCall(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToMainPage.emit(emitObj);
  }
}