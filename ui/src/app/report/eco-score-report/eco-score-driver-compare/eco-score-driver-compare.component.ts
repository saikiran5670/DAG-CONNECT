import { EventEmitter } from '@angular/core';
import { Component, Input, OnInit, Output, ViewEncapsulation, AfterViewInit, ElementRef } from '@angular/core';
import {
  AngularGridInstance,
  Column,
  FieldType,
  GridOption,
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
  //performance table
  angularGrid!: AngularGridInstance;
  dataViewObj: any;
  gridObj: any;
  gridOptions!: GridOption;
  columnDefinitions!: Column[];
  datasetHierarchical: any[] = [];
  compareDriverCount: number = 0;
  columnPerformance: any=[];
  //General table
  angularGridGen!: AngularGridInstance;
  dataViewObjGen: any;
  gridObjGen: any;
  gridOptionsGen!: GridOption;
  columnDefinitionsGen!: Column[];
  datasetGen: any[] = [];
  columnGeneral: any=[];
  //common details
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
  
  constructor(private reportService: ReportService, private elementRef:ElementRef) { }

  ngOnInit() {
    this.translationUpdate();
    this.showTable = true;
    this.driverDetails = this.compareEcoScore.drivers;    
    this.driverHeaders();
    this.tableColumns();
    this.defineGridGen();
    this.defineGrid();
    this.loadData();
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
      { key:'rp_CruiseControlUsage30' , value:'Cruise Control Usage 30-50 km/h(%)' },
      { key:'rp_CruiseControlUsage75' , value:'Cruise Control Usage > 75 km/h(%)' },
      { key:'rp_CruiseControlUsage50' , value:'Cruise Control Usage 50-75 km/h(%)' },
      { key:'rp_cruisecontrolusage' , value:'Cruise Control Usage' },
      { key:'rp_cruisecontroldistance50' , value:'Cruise Control Usage 50-75 km/h(%)' },
      { key:'rp_cruisecontroldistance30' , value:'Cruise Control Usage 30-50 km/h(%)' },
      { key:'rp_cruisecontroldistance75' , value:'Cruise Control Usage > 75 km/h(%)' },
      { key:'rp_harshbraking' , value:'Harsh Braking(%)' },
      { key:'rp_harshbrakeduration' , value:'Harsh Brake Duration' },
      { key:'rp_brakeduration' , value:'Brake Duration' },
      { key:'rp_brakingscore' , value:'Braking Score' }
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
      if(this.driverDetails.length > 0){
        this.driver1= '<span style="font-weight:700">'+this.driverDetails[0].driverName+'</span><br/><span class="backBtnCss driver1">X</span><span style="font-weight:normal">('+this.driverDetails[0].driverId+')</span>';
        this.driverG1= '<span style="font-weight:700">'+this.driverDetails[0].driverName+'</span><br/><span class="backBtnCss driverG1">X</span><span style="font-weight:normal">('+this.driverDetails[0].driverId+')</span>';
      }
      if(this.driverDetails.length > 1){
        this.driver2= '<span style="font-weight:700">'+this.driverDetails[1].driverName+'</span><br/><span class="backBtnCss driver2">X</span><span style="font-weight:normal">('+this.driverDetails[1].driverId+')</span>';
        this.driverG2= '<span style="font-weight:700">'+this.driverDetails[1].driverName+'</span><br/><span class="backBtnCss driverG2">X</span><span style="font-weight:normal">('+this.driverDetails[1].driverId+')</span>';
      }
      if(this.driverDetails.length > 2){
        this.driver3= '<span style="font-weight:700">'+this.driverDetails[2].driverName+'</span><br/><span class="backBtnCss driver3">X</span><span style="font-weight:normal">('+this.driverDetails[2].driverId+')</span>';
        this.driverG3= '<span style="font-weight:700">'+this.driverDetails[2].driverName+'</span><br/><span class="backBtnCss driverG3">X</span><span style="font-weight:normal">('+this.driverDetails[2].driverId+')</span>';
      }
      if(this.driverDetails.length > 3){
        this.driver4= '<span style="font-weight:700">'+this.driverDetails[3].driverName+'</span><br/><span class="backBtnCss driver4">X</span><span style="font-weight:normal">('+this.driverDetails[3].driverId+')</span>';
        this.driverG4= '<span style="font-weight:700">'+this.driverDetails[3].driverName+'</span><br/><span class="backBtnCss driverG4">X</span><span style="font-weight:normal">('+this.driverDetails[3].driverId+')</span>';
      }
    }
  }

  tableColumns(){
    this.columnDefinitions = [
      {
        id: 'category', name: 'Category', field: 'key',
        type: FieldType.string, width: 150, formatter: this.treeFormatter
      },
      {
        id: 'target', name: 'Target', field: 'target',
        type: FieldType.string, minWidth: 90, maxWidth: 100
      }
    ];
    this.columnDefinitionsGen = [
      {
        id: 'categoryG', name: 'Category', field: 'key',
        type: FieldType.string, width: 150, formatter: this.treeFormatter,// maxWidth: 400
      }
    ];
    
    this.columnPerformance.push({columnId: 'category'});
    this.columnPerformance.push({columnId: 'target'});
    this.columnGeneral.push({columnId: 'categoryG'})
    if(this.driverDetails !== undefined && this.driverDetails !== null){
      for(var i=1; i<=this.driverDetails.length;i++){
        this.columnPerformance.push({columnId: 'driver'+i});
        this.columnGeneral.push({columnId: 'driverG'+i});
      }
      this.compareDriverCount = this.driverDetails.length;
      if(this.driverDetails.length > 0){
        let driver1= '<span style="font-weight:700">'+this.driverDetails[0].driverName+'</span><br/><span class="backBtnCss driver1">X</span><span style="font-weight:normal">('+this.driverDetails[0].driverId+')</span>';
        let driverG1= '<span style="font-weight:700">'+this.driverDetails[0].driverName+'</span><br/><span class="backBtnCss driverG1">X</span><span style="font-weight:normal">('+this.driverDetails[0].driverId+')</span>';
        this.columnDefinitions.push({
          id: 'driver1', name: driver1, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore0, maxWidth: 250
        });
        this.columnDefinitionsGen.push({
          id: 'driverG1', name: driverG1, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore0, maxWidth: 250
        });
      }
      if(this.driverDetails.length > 1){
        let driver2= '<span style="font-weight:700">'+this.driverDetails[1].driverName+'</span><br/><span class="backBtnCss driver2">X</span><span style="font-weight:normal">('+this.driverDetails[1].driverId+')</span>';
        let driverG2= '<span style="font-weight:700">'+this.driverDetails[1].driverName+'</span><br/><span class="backBtnCss driverG2">X</span><span style="font-weight:normal">('+this.driverDetails[1].driverId+')</span>';
        this.columnDefinitions.push({
          id: 'driver2', name: driver2, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore1, maxWidth: 250
        });
        this.columnDefinitionsGen.push({
          id: 'driverG2', name: driverG2, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore1, maxWidth: 250
        });
      }
      if(this.driverDetails.length > 2){
        let driver3= '<span style="font-weight:700">'+this.driverDetails[2].driverName+'</span><br/><span class="backBtnCss driver3">X</span><span style="font-weight:normal">('+this.driverDetails[2].driverId+')</span>';
        let driverG3= '<span style="font-weight:700">'+this.driverDetails[2].driverName+'</span><br/><span class="backBtnCss driverG3">X</span><span style="font-weight:normal">('+this.driverDetails[2].driverId+')</span>';
        this.columnDefinitions.push({
          id: 'driver3', name: driver3, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore2, maxWidth: 250
        });
        this.columnDefinitionsGen.push({
          id: 'driverG3', name: driverG3, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore2, maxWidth: 250
        });
      }
      if(this.driverDetails.length > 3){
        let driver4= '<span style="font-weight:700">'+this.driverDetails[3].driverName+'</span><br/><span class="backBtnCss driver4">X</span><span style="font-weight:normal">('+this.driverDetails[3].driverId+')</span>';
        let driverG4= '<span style="font-weight:700">'+this.driverDetails[3].driverName+'</span><br/><span class="backBtnCss driverG4">X</span><span style="font-weight:normal">('+this.driverDetails[3].driverId+')</span>';
        this.columnDefinitions.push({
          id: 'driver4', name: driver4, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore3, maxWidth: 250
        });
        this.columnDefinitionsGen.push({
          id: 'driverG4', name: driverG4, field: 'score',
          type: FieldType.number, minWidth: 90, formatter: this.getScore3, maxWidth: 250
        });
      }
    }
  }

  defineGrid() {
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
        columns: this.columnPerformance
      }
    };
  }

  defineGridGen() {
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
        columns: this.columnGeneral
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

  treeFormatter: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if (value === null || value === undefined || dataContext === undefined) {
      return '';
    }
    var foundValue = this.translationData.filter(obj=>obj.key === value);
    if(foundValue === undefined || foundValue === null || foundValue.length === 0)
      value = value;
    else
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
    if (value === null || value === undefined)
    return '';
    value = value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    const spacer = `<span style="display:inline-block; width:${(15 * dataContext[treeLevelPropName])}px;"></span>`;

    if (data[idx + 1] && data[idx + 1][treeLevelPropName] > data[idx][treeLevelPropName]) {
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
    if(value !== undefined && value !== null && value.length > 0){
      let color = value[0].color === 'Amber'?'Orange':value[0].color;
      return '<span style="color:' + color + '">' + value[0].value + "</span>";
    }
    return '';
  }
  
  getScore1: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 1){
      let color = value[1].color === 'Amber'?'Orange':value[1].color;
      return '<span style="color:' + color + '">' + value[1].value + "</span>";
    }
    return '';
  }

  getScore2: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 2){
      let color = value[2].color === 'Amber'?'Orange':value[2].color;
      return '<span style="color:' + color + '">' + value[2].value + "</span>";
    }
    return '';
  }

  getScore3: Formatter = (row, cell, value, columnDef, dataContext, grid) => {
    if(value !== undefined && value !== null && value.length > 3){
      let color = value[3].color === 'Amber'?'Orange':value[3].color;
      return '<span style="color:' + color + '">' + value[3].value + "</span>";
    }
    return '';
  }

  loadData() {
    let res = (JSON.stringify(this.compareEcoScore)).replace(/dataAttributeId/g, "id");
    let fin = JSON.parse(res);
    this.datasetHierarchical = fin.compareDrivers.subCompareDrivers[1].subCompareDrivers;
    this.datasetGen = fin.compareDrivers.subCompareDrivers[0].subCompareDrivers; 
 }

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