import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit, Output, ViewEncapsulation, AfterViewInit, ElementRef } from '@angular/core';
import { AngularSlickgridModule } from 'angular-slickgrid';
import {
  AngularGridInstance,
  Column,
  FieldType,
  Filters,
  Formatters,
  GridOption,
  findItemInTreeStructure,
  Formatter,
} from 'angular-slickgrid';
import { ReportService } from '../../../services/report.service';


@Component({
  selector: 'app-eco-score-driver-compare',
  templateUrl: './eco-score-driver-compare.component.html',
  styleUrls: ['./eco-score-driver-compare.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class EcoScoreDriverCompareComponent implements OnInit {
  @Input() translationData: any;
  @Input() compareEcoScore: any;
  @Output() backToMainPage = new EventEmitter<any>();
  generalExpandPanel: boolean = true;
  angularGrid!: AngularGridInstance;
  dataViewObj: any;
  gridObj: any;
  gridOptions!: GridOption;
  columnDefinitions!: Column[];
  datasetHierarchical: any[] = [];
  compareDriverCount: number = 0;

  //General table
  angularGridGen!: AngularGridInstance;
  dataViewObjGen: any;
  gridObjGen: any;
  gridOptionsGen!: GridOption;
  columnDefinitionsGen!: Column[];
  datasetGen: any[] = [];

  driverDetails: any=[];
  driver1:string = '';
  driver2:string = '';
  driver3:string = '';
  driver4:string = '';
  driverG1:string = '';
  driverG2:string = '';
  driverG3:string = '';
  driverG4:string = '';

  searchString = '';
  displayColumns: string[];
  displayData: any[];
  showTable: boolean;

  ngOnInit() {
   this.translationUpdate();
    this.showTable = true;
    this.driverDetails = this.compareEcoScore.drivers;
    this.driverHeaders();
    this.defineGridGen();
    this.defineGrid();
    console.log(JSON.stringify(this.compareEcoScore));

    this.mockDataset();
    // this.reportService.getEcoScoreDriverCompare(this.compareEcoScore).subscribe((_drivers: any) => {
    //   this.driverDetails = _drivers.drivers;
    //   this.defineGrid();
    //   this.defineGridGen();

    //   let res = (JSON.stringify(_drivers)).replace(/dataAttributeId/g, "id");
    //   let fin = JSON.parse(res);
    //   this.datasetHierarchical = fin.compareDrivers.subCompareDrivers[1].subCompareDrivers;
    //   this.datasetGen = fin.compareDrivers.subCompareDrivers[0].subCompareDrivers;  
    // });

    // mock a dataset
    //this.datasetHierarchical = this.mockDataset();
  }

  translationUpdate(){
    this.translationData = [
      { key:'rp_general' , value:'General' },
      { key:'rp_averagegrossweight' , value:'Average Gross Weight' },
      { key:'rp_distance' , value:'Distance' },
      { key:'rp_numberoftrips' , value:'Number of Trips' },
      { key:'rp_numberofvehicles' , value:'Number of vehicles' },
      { key:'rp_averagedistanceperday' , value:'Average distance per day' },
      { key:'rp_driverperformance' , value:'Driver Performance' },
      { key:'rp_ecoscore' , value:'Eco Score' },
      { key:'rp_fuelconsumption' , value:'Fuel Consumption' },
      { key:'rp_braking' , value:'Braking(%)' },
      { key:'rp_anticipationscore' , value:'Anticipation Score' },
      { key:'rp_averagedrivingspeed' , value:'Average Driving Speed' },
      { key:'rp_idleduration' , value:'Idle Duration' },
      { key:'rp_idling' , value:'Idling(%)' },
      { key:'rp_heavythrottleduration' , value:'Heavy Throttle Duration' },
      { key:'rp_heavythrottling' , value:'Heavy Throttling(%)' },
      { key:'rp_averagespeed' , value:'Average Speed' },
      { key:'rp_ptoduration' , value:'PTO Duration' },
      { key:'rp_ptousage' , value:'PTO Usage(%)' },
      { key:'rp_CruiseControlUsage30' , value:'Cruise Control Usage 30-75km/h(%)' },
      { key:'rp_CruiseControlUsage75' , value:'Cruise Control Usage>75km/h(%)' },
      { key:'rp_CruiseControlUsage50' , value:'Cruise Control Usage 50-75km/h(%)' },
      { key:'rp_cruisecontrolusage' , value:'Cruise Control Usage' },
      { key:'rp_cruisecontroldistance50' , value:'Cruise Control Usage 50-75km/h(%)' },
      { key:'rp_cruisecontroldistance30' , value:'Cruise Control Usage 30-75km/h(%)' },
      { key:'rp_cruisecontroldistance75' , value:'Cruise Control Usage>75km/h(%)' },
     ];

    // this.translationData.rp_general = 'General';
    // this.translationData.rp_averagegrossweight = 'Average Gross Weight';
    // this.translationData.rp_distance = 'Distance';
    // this.translationData.rp_numberoftrips = 'Number of Trips';
    // this.translationData.rp_numberofvehicles = 'Number of vehicles';
    // this.translationData.rp_averagedistanceperday = 'Average distance per day';
    // this.translationData.rp_driverperformance = 'Driver Performance';
    // this.translationData.rp_ecoscore = 'Eco Score';
    // this.translationData.rp_fuelconsumption = 'Fuel Consumption';
    // this.translationData.rp_braking = 'Braking(%)';
    // this.translationData.rp_anticipationscore = 'Anticipation Score';
    // this.translationData.rp_averagedrivingspeed = 'Average Driving Speed';
    // this.translationData.rp_idleduration = 'Idle Duration';
    // this.translationData.rp_idling = 'Idling(%)';
    // this.translationData.rp_heavythrottleduration = 'Heavy Throttle Duration';
    // this.translationData.rp_heavythrottling = 'Heavy Throttling(%)';
    // this.translationData.rp_averagespeed = 'Average Speed';
    // this.translationData.rp_averagedrivingspeed = 'Average Driving Speed';
    // this.translationData.rp_ptoduration = 'PTO Duration';
    // this.translationData.rp_ptousage = 'PTO Usage(%)';
    // this.translationData.rp_cruisecontroldistance30 = 'Cruise Control Usage 30-75km/h(%)';
    // this.translationData.rp_cruisecontroldistance75 = 'Cruise Control Usage>75km/h(%)';
    // this.translationData.rp_cruisecontroldistance50 = 'Cruise Control Usage 50-75km/h(%)';
    // this.translationData.rp_cruisecontrolusage = 'Cruise Control Usage';
    // this.translationData.rp_fuelconsumption = 'Fuel Consumption';
  }

  driverHeaders(){    
    if((this.driverDetails !== undefined && this.driverDetails !== null)){
      this.compareDriverCount = this.driverDetails.length;
      if(this.driverDetails.length > 0){
        this.driver1= '<span style="font-weight:700">'+this.driverDetails[0].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[0].driverId+')</span>';
        this.driverG1= '<span style="font-weight:700">'+this.driverDetails[0].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[0].driverId+')</span>';
      }
      if(this.driverDetails.length > 1){
        this.driver2= '<span style="font-weight:700">'+this.driverDetails[1].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[1].driverId+')</span>';
        this.driverG2= '<span style="font-weight:700">'+this.driverDetails[1].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[1].driverId+')</span>';
      }
      if(this.driverDetails.length > 2){
        this.driver3= '<span style="font-weight:700">'+this.driverDetails[2].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[2].driverId+')</span>';
        this.driverG3= '<span style="font-weight:700">'+this.driverDetails[2].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[2].driverId+')</span>';
      }
      if(this.driverDetails.length > 3){
        this.driver4= '<span style="font-weight:700">'+this.driverDetails[3].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[3].driverId+')</span>';
        this.driverG4= '<span style="font-weight:700">'+this.driverDetails[3].driverName+'</span><br/><span style="font-weight:normal">('+this.driverDetails[3].driverId+')</span>';
      }
    }
  }

  defineGrid() {    
    this.columnDefinitions = [
      {
        id: 'category', name: 'Category', field: 'key',
        type: FieldType.string, width: 150, formatter: this.treeFormatter
      },
      {
        id: 'target', name: 'Target', field: 'target',
        type: FieldType.string, minWidth: 90, maxWidth: 100
      },
      {
        id: 'driver1', name: this.driver1, field: 'score',
        type: FieldType.number, minWidth: 90, formatter: this.getScore0, maxWidth: 250
      },
      {
        id: 'driver2', name: this.driver2, field: 'score',
         minWidth: 90,
        type: FieldType.number, formatter: this.getScore1, maxWidth: 250
      },
      {
        id: 'driver3', name: this.driver3, field: 'score',
        type: FieldType.number, minWidth: 90, formatter: this.getScore2, maxWidth: 250
      },
      {
        id: 'driver4', name: this.driver4, field: 'score',
         minWidth: 90,
        type: FieldType.number, formatter: this.getScore3, maxWidth: 250
      }
    ];

    this.gridOptions = {
      autoResize: {
        containerId: 'container-DriverPerformance',
        sidePadding: 10
      },
      autoTooltipOptions: {
        enableForCells: true,
        enableForHeaderCells: true,
        maxToolTipLength: 200
      },
      enableAutoSizeColumns: true,
      enableAutoResize: true,
      forceFitColumns: true,
      enableExport: false,
      enableHeaderMenu: false,
      enableContextMenu: false,
      enableGridMenu: false,
      enableFiltering: true,
      enableTreeData: true, // you must enable this flag for the filtering & sorting to work as expected
      treeDataOptions: {
        columnId: 'category',
        childrenPropName: 'subCompareDrivers'
      },
      multiColumnSort: false, // multi-column sorting is not supported with Tree Data, so you need to disable it
      // change header/cell row height for salesforce theme
      headerRowHeight: 45,
      rowHeight: 40,
      showCustomFooter: true,

      // use Material Design SVG icons
      contextMenu: {
        iconCollapseAllGroupsCommand: 'mdi mdi-arrow-collapse',
        iconExpandAllGroupsCommand: 'mdi mdi-arrow-expand',
        iconClearGroupingCommand: 'mdi mdi-close',
        iconCopyCellValueCommand: 'mdi mdi-content-copy',
        iconExportCsvCommand: 'mdi mdi-download',
        iconExportExcelCommand: 'mdi mdi-file-excel-outline',
        iconExportTextDelimitedCommand: 'mdi mdi-download',
      },
      headerMenu: {
        hideColumnHideCommand: true,
        hideClearFilterCommand: true,
        hideColumnResizeByContentCommand: true
      },
      presets: {
        columns: [
          {columnId: 'category',},
          {columnId: 'target',},
          {columnId: 'driver1'},
          {columnId: 'driver2'},
          {columnId: 'driver3'},
          {columnId: 'driver4'}
        ]
      }
    };
  }

  defineGridGen() {
    this.columnDefinitionsGen = [
      {
        id: 'categoryG', name: 'Category', field: 'key',
        type: FieldType.string, width: 150, formatter: this.treeFormatter,// maxWidth: 400
      },
      {
        id: 'driverG1', name: this.driverG1, field: 'score',
        type: FieldType.number, minWidth: 90, formatter: this.getScore0, //maxWidth: 200
      },
      {
        id: 'driverG2', name: this.driverG2, field: 'score',
        type: FieldType.number, minWidth: 90, formatter: this.getScore1, //maxWidth: 200
      },
      {
        id: 'driverG3', name: this.driverG3, field: 'score',
        type: FieldType.number, minWidth: 90, formatter: this.getScore2, //maxWidth: 200
      },
      {
        id: 'driverG4', name: this.driverG4, field: 'score',
        type: FieldType.number, minWidth: 90, formatter: this.getScore3, //maxWidth: 200
      }
    ];

    this.gridOptionsGen = {
      autoResize: {
        containerId: 'container-General',
        sidePadding: 10
      },
      autoTooltipOptions: {
        enableForCells: true,
        enableForHeaderCells: true,
        maxToolTipLength: 200
      },
      autoHeight: true,
      enableAutoSizeColumns: true,
      enableAutoResize: true,
      forceFitColumns: true,
      enableExport: false,
      enableHeaderMenu: false,
      enableContextMenu: false,
      enableGridMenu: false,
      enableFiltering: true,
      enableTreeData: true, // you must enable this flag for the filtering & sorting to work as expected
      treeDataOptions: {
        columnId: 'categoryG',
        childrenPropName: 'subCompareDrivers'
      },
      multiColumnSort: false,
      headerRowHeight: 45,
      rowHeight: 40,
      showCustomFooter: true,

      // use Material Design SVG icons
      contextMenu: {
        iconCollapseAllGroupsCommand: 'mdi mdi-arrow-collapse',
        iconExpandAllGroupsCommand: 'mdi mdi-arrow-expand',
        iconClearGroupingCommand: 'mdi mdi-close',
        iconCopyCellValueCommand: 'mdi mdi-content-copy',
        iconExportCsvCommand: 'mdi mdi-download',
        iconExportExcelCommand: 'mdi mdi-file-excel-outline',
        iconExportTextDelimitedCommand: 'mdi mdi-download',
      },
      headerMenu: {
        hideColumnHideCommand: true,
        hideClearFilterCommand: true,
        hideColumnResizeByContentCommand: true
      },
      presets: {
        columns: [
          {columnId: 'categoryG',},
          {columnId: 'driverG1'},
          {columnId: 'driverG2'},
          {columnId: 'driverG3'},
          {columnId: 'driverG4'}
        ]
      }
    };
  }

  angularGridReady(angularGrid: AngularGridInstance) {
    this.angularGrid = angularGrid;
    this.gridObj = angularGrid.slickGrid;
    this.dataViewObj = angularGrid.dataView;
  }

  angularGridReadyGen(angularGrid: AngularGridInstance) {
    this.angularGridGen = angularGrid;
    this.gridObjGen = angularGrid.slickGrid;
    this.dataViewObjGen = angularGrid.dataView;
  }

  clearSearch() {
    this.searchString = '';
    this.updateFilter();
  }

  searchStringChanged() {
    this.updateFilter();
  }

  updateFilter() {
    this.angularGrid.filterService.updateFilters([{ columnId: 'file', searchTerms: [this.searchString] }], true, false, true);
  }

  treeFormatter: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    var foundValue = this.translationData.filter(obj=>obj.key === value);
    value = foundValue[0].value;
    const gridOptions = grid.getOptions() as GridOption;
    const treeLevelPropName = gridOptions.treeDataOptions && gridOptions.treeDataOptions.levelPropName || '__treeLevel';
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    const dataView = grid.getData();
    const data = dataView.getItems();
    const identifierPropName = dataView.getIdPropertyName() || 'id';
    const idx = dataView.getIdxById(dataContext[identifierPropName]);

    value = value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    const spacer = `<span style="display:inline-block; width:${(15 * dataContext[treeLevelPropName])}px;"></span>`;

    if (data[idx + 1] && data[idx + 1][treeLevelPropName] > data[idx][treeLevelPropName]) {
      //const folderPrefix = `<span class="mdi icon color-alt-warning ${dataContext.__collapsed ? 'mdi-folder' : 'mdi-folder-open'}"></span>`;
      if (dataContext.__collapsed) {
        return `${spacer} <span class="slick-group-toggle collapsed" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
      } else {
        return `${spacer} <span class="slick-group-toggle expanded" level="${dataContext[treeLevelPropName]}"></span> &nbsp;${value}`;
      }
    } else {
      return `${spacer} <span class="slick-group-toggle" level="${dataContext[treeLevelPropName]}"></span>&nbsp;${value}`;
    }
  }
  
  getScore0: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 0)
      return '<span style="color:' + value[0].color + '">' + value[0].value + "</span>";
    return '';
  }
  
  getScore1: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 1)
      return '<span style="color:' + value[1].color + '">' + value[1].value + "</span>";
    return '';
  }

  getScore2: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 2)
      return '<span style="color:' + value[2].color + '">' + value[2].value + "</span>";
    return '';
  }

  getScore3: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 3)
      return '<span style="color:' + value[3].color + '">' + value[3].value + "</span>";
    return '';
  }

  collapseAll() {
    this.angularGrid.treeDataService.toggleTreeDataCollapse(true);
  }

  expandAll() {
    this.angularGrid.treeDataService.toggleTreeDataCollapse(false);
  }

  mockDataset() {
    let res = (JSON.stringify(this.compareEcoScore)).replace(/dataAttributeId/g, "id");
    let fin = JSON.parse(res);
    this.datasetHierarchical = fin.compareDrivers.subCompareDrivers[1].subCompareDrivers;
    this.datasetGen = fin.compareDrivers.subCompareDrivers[0].subCompareDrivers; 
 }

  constructor(private reportService: ReportService, private elementRef:ElementRef) { }

  backToMainPageCall(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToMainPage.emit(emitObj);
  }

  ngAfterViewInit() {
    // this.elementRef.nativeElement.querySelector('.driver1')
    //                               .addEventListener('click', this.removeHeader.bind(this));
    // this.elementRef.nativeElement.querySelector('.driver2')
    //                               .addEventListener('click', this.removeHeader.bind(this));
    // this.elementRef.nativeElement.querySelector('.driver3')
    //                               .addEventListener('click', this.removeHeader.bind(this));
    // this.elementRef.nativeElement.querySelector('.driver4')
    //                               .addEventListener('click', this.removeHeader.bind(this));
    // this.elementRef.nativeElement.querySelector('.driverG1')
    //                               .addEventListener('click', this.removeHeader.bind(this));
    // this.elementRef.nativeElement.querySelector('.driverG2')
    //                               .addEventListener('click', this.removeHeader.bind(this));
    // this.elementRef.nativeElement.querySelector('.driverG3')
    //                               .addEventListener('click', this.removeHeader.bind(this));
    // this.elementRef.nativeElement.querySelector('.driverG4')
    //                               .addEventListener('click', this.removeHeader.bind(this));
  }

  removeHeader(event: any){
    if(event.srcElement.outerHTML.indexOf("driver1") !== -1){
      this.removeColumn("driver1");
    } else if(event.srcElement.outerHTML.indexOf("driver2") !== -1){
      this.removeColumn("driver2");
    } else if(event.srcElement.outerHTML.indexOf("driver3") !== -1){
      this.removeColumn("driver3");
    } else if(event.srcElement.outerHTML.indexOf("driver4") !== -1){
      this.removeColumn("driver4");
    } else if(event.srcElement.outerHTML.indexOf("driverG1") !== -1){
      this.removeColumnG("driverG1");
    } else if(event.srcElement.outerHTML.indexOf("driverG2") !== -1){
      this.removeColumnG("driverG2");
    } else if(event.srcElement.outerHTML.indexOf("driverG3") !== -1){
      this.removeColumnG("driverG3");
    } else if(event.srcElement.outerHTML.indexOf("driverG4") !== -1){
      this.removeColumnG("driverG4");
    } 
  }

  removeColumn(driverId){
    this.angularGrid.gridService.hideColumnById(driverId);
    //this.dataViewObj.refresh();
  }

  removeColumnG(driverId){
    this.angularGridGen.gridService.hideColumnById(driverId);
    //this.dataViewObjGen.refresh();
  }
}