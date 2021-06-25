import { SelectionModel } from '@angular/cdk/collections';
import { QueryList } from '@angular/core';
import { ElementRef } from '@angular/core';
import { ViewChild } from '@angular/core';
import { ViewChildren } from '@angular/core';
import { Input } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AnyMxRecord } from 'dns';
import { POIService } from 'src/app/services/poi.service';
import { DomSanitizer } from '@angular/platform-browser';
import { LandmarkGroupService } from 'src/app/services/landmarkGroup.service';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { CommonTableComponent } from 'src/app/shared/common-table/common-table.component';
import { GeofenceService } from 'src/app/services/landmarkGeofence.service';
import { Options } from '@angular-slider/ngx-slider';
import { PeriodSelectionFilterComponent } from '../period-selection-filter/period-selection-filter.component';
import { Util } from 'src/app/shared/util';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

declare var H: any;

@Component({
  selector: 'app-alert-advanced-filter',
  templateUrl: './alert-advanced-filter.component.html',
  styleUrls: ['./alert-advanced-filter.component.less']
})
export class AlertAdvancedFilterComponent implements OnInit {
  @Input() translationData: any = [];
  @Input() alert_category_selected : any;
  @Input() alert_type_selected : any;
  @Input() selectedRowData : any;
  @Input() actionType :any;
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  dialogRef: MatDialogRef<CommonTableComponent>;
  selectedGeofence = new SelectionModel(true, []);
  selectedGroup = new SelectionModel(true, []);
  alertAdvancedFilterForm: FormGroup;
  displayedColumnsPOI: string[] = ['select', 'icon', 'name', 'categoryName', 'subCategoryName', 'address'];
  displayedColumnsGeofence: string[] = ['select', 'name', 'categoryName', 'subCategoryName'];
  displayedColumnsGroup: string[] = ['select', 'name', 'poiCount', 'geofenceCount'];
  localStLanguage: any;
  organizationId: number;
  accountId: number;
  isDistanceSelected: boolean= false;
  isOccurenceSelected: boolean= false;
  isDurationSelected: boolean= false;
  isPoiSelected: boolean= false;
  selectedPoiSite: any;
  marker: any;
  tableRowData: any = [];
  alertTimingDetail: any =[];
  groupArray: any = [];
  markerArray: any = [];
  geoMarkerArray: any = [];
  filterTypeArray: any =[];
  map: any;
  polyPoints: any = [];
  poiDataSource: any = new MatTableDataSource([]);
  geofenceDataSource: any = new MatTableDataSource([]);
  groupDataSource: any = new MatTableDataSource([]);
  selectedPOI = new SelectionModel(true, []);
  private platform: any;
  poiGridData = [];
  geofenceGridData = [];
  groupGridData = [];
  poiWidth : number = 100;
  poiWidthKm : number = 0.1;
  sliderValue : number = 0;
  selectedApplyOn: string = 'A';
  advancedAlertPayload: any = [];
  filterType: any;
  selectedDistance: any;
  selectedDuration :any;
  selectedOccurance : any;
  rowData : any;
  distanceVal: any =[];
  occurenceVal: any =[];
  durationVal :any = [];
  thresholdVal: any;
  from: any;
  to: any;
  options: Options = {
    floor: 0,
    ceil: 10000
  };
  @ViewChild(PeriodSelectionFilterComponent)
  periodSelectionComponent: PeriodSelectionFilterComponent;
  
  @ViewChild("map")
  private mapElement: ElementRef;
  openAdvancedFilter: boolean;
  constructor(private _formBuilder: FormBuilder,private poiService: POIService,
              private domSanitizer: DomSanitizer,
              private landmarkGroupService: LandmarkGroupService,
              private dialog: MatDialog,
              private dialogService: ConfirmDialogService,
              private geofenceService: GeofenceService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(): void {
    console.log(this.displayedColumnsPOI[0]);
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId= parseInt(localStorage.getItem("accountId"));
    this.alertAdvancedFilterForm = this._formBuilder.group({
      poiSite: [''],
      distance: [''],
      occurences: [''],
      duration: [''],
      widthInput: [''],
      fullorCustom: ['A'],
      fromDate: [''],
      fromTimeRange: ['00:00'],
      toDate: [''],
      toTimeRange:['23:59']
    })

    
    
    this.alertAdvancedFilterForm.controls.widthInput.setValue(0.1);
    if(this.actionType == 'edit' || this.actionType == 'duplicate' || this.actionType == 'view'){
      this.setDefaultAdvanceAlert();
    }
  }

  setDefaultAdvanceAlert(){
    this.loadMapData();
    this.loadPOIData();
    this.loadGeofenceData();
    this.loadGroupData();
    this.selectedApplyOn = this.selectedRowData.alertUrgencyLevelRefs[0].periodType;
    if(this.selectedApplyOn == 'C'){
      this.from = Util.convertUtcToDateFormat(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelStartDate,'DD/MM/YYYY HH:MM').split(" ");
      this.to = Util.convertUtcToDateFormat(this.selectedRowData.alertUrgencyLevelRefs[0].urgencylevelEndDate,'DD/MM/YYYY HH:MM').split(" ");
      this.alertAdvancedFilterForm.get('fromDate').setValue(this.from[0]);
      this.alertAdvancedFilterForm.get('toDate').setValue(this.to[0]);

      this.alertAdvancedFilterForm.get('fromTimeRange').setValue(this.from[1]);
      this.alertAdvancedFilterForm.get('toTimeRange').setValue(this.to[1]);
    }
      this.alertAdvancedFilterForm.get('fullorCustom').setValue(this.selectedApplyOn);
      let Data = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.forEach(element => {
        if(element.filterType == 'N' && element.thresholdValue !=0)
        {
          this.isOccurenceSelected =true;
          this.alertAdvancedFilterForm.get('occurences').setValue(element.thresholdValue);
        }
        if(element.filterType == 'T')
        {
          this.isDistanceSelected =true;
          this.alertAdvancedFilterForm.get('distance').setValue(element.thresholdValue);
        }
        if(element.filterType == 'D')
        {
          this.isDurationSelected =true;
          this.alertAdvancedFilterForm.get('duration').setValue(element.thresholdValue);
        }
        if(element.landmarkType == 'P' || element.landmarkType == 'O' || element.landmarkType == 'C' || element.landmarkType == 'G')
        {

            this.isPoiSelected= true;
            // this.loadMapData();
            // this.loadPOIData();
            // this.loadGeofenceData();
            // this.loadGroupData();
          
        }
        if(this.actionType == 'view'){
          let arr1 = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => (item.filterType == 'N' && item.thresholdValue != 0));
          this.occurenceVal.push(arr1);
          let arr2 = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'T');
          this.distanceVal.push(arr2);
          let arr3 = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'D');
          this.durationVal.push(arr3);
        }
        
      });
  }

  onChangeDistance(event: any){
    if(event.checked){
      this.isDistanceSelected= true;
    }
    else{
      this.isDistanceSelected= false;
    }
  }

  onApplyOnChange(event: any){
    this.selectedApplyOn = event.value;
  }

  loadMapData(){
    let defaultLayers = this.platform.createDefaultLayers();
    setTimeout(() => {
      this.map = new H.Map(
        this.mapElement.nativeElement,
        defaultLayers.vector.normal.map,
        {
          center: { lat: 51.43175839453286, lng: 5.519981221425336 },
          zoom: 4,
          pixelRatio: window.devicePixelRatio || 1
        }
      );
      window.addEventListener('resize', () => this.map.getViewPort().resize());
      var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
      var ui = H.ui.UI.createDefault(this.map, defaultLayers);  
    }, 5000);
  }

  onChangePOI(event: any){
    if(event.checked){
      this.isPoiSelected= true;
      this.loadMapData();
      this.loadPOIData();
      this.loadGeofenceData();
      this.loadGroupData();
    }
    else{
      this.isPoiSelected= false;
    }
  }

  onChangeOccurence(event: any)
  {
    if(event.checked){
      this.isOccurenceSelected= true;
    }
    else{
      this.isOccurenceSelected= false;
    }
  }

  onChangeDuration(event: any)
  {
    if(event.checked){
      this.isDurationSelected= true;
    }
    else{
      this.isDurationSelected= false;
    }
  }

  onRadioButtonChange(event: any){
    this.selectedPoiSite = event.value;
  }

  loadPOIData() {
    this.poiService.getPois(this.organizationId).subscribe((poilist: any) => {
      if(poilist.length > 0){
        poilist.forEach(element => {
          if(element.icon && element.icon != '' && element.icon.length > 0){
            element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + element.icon);
          }else{
            element.icon = '';
          }
        });
        this.poiGridData = poilist;
        this.updatePOIDataSource(this.poiGridData);
        if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'duplicate')
        this.loadPOISelectedData(this.poiGridData);
      }
      
    });
  }

  PoiCheckboxClicked(event: any, row: any) {
    if(event.checked){ //-- add new marker
      this.markerArray.push(row);
    }else{ //-- remove existing marker
      //It will filter out checked points only
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
    }
    this.addMarkerOnMap();
      
    }

    loadGeofenceData() {
      this.geofenceService.getGeofenceDetails(this.organizationId).subscribe((geofencelist: any) => {
        this.geofenceGridData = geofencelist;
       this.geofenceGridData = this.geofenceGridData.filter(item => item.type == "C" || item.type == "O");
        this.updateGeofenceDataSource(this.geofenceGridData);
        if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'duplicate')
          this.loadGeofenceSelectedData(this.geofenceGridData);
      });
    }

    loadGeofenceSelectedData(tableData: any){
      let selectedGeofenceList: any = [];
      if(this.actionType == 'view'){
        tableData.forEach((row: any) => {
          let search = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == row.id && (item.landmarkType == "C" || item.landmarkType == "O"));
          if (search.length > 0) {
            selectedGeofenceList.push(row);
            setTimeout(() => {
              this.geofenceCheckboxClicked({checked : true}, row);  
            }, 1000);
          }
        });
        tableData = selectedGeofenceList;
        this.displayedColumnsGeofence= ['name', 'categoryName', 'subCategoryName'];
        this.updateGeofenceDataSource(tableData);
      }
      else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
        this.selectGeofenceTableRows(this.selectedRowData);
      }
    }

    updateGeofenceDataSource(tableData: any){
      this.geofenceDataSource = new MatTableDataSource(tableData);
      this.geofenceDataSource.filterPredicate = function(data: any, filter: string): boolean {
        return (
          data.name.toString().toLowerCase().includes(filter) ||
          data.categoryName.toString().toLowerCase().includes(filter) ||
          data.subCategoryName.toString().toLowerCase().includes(filter)
        );
      };
      setTimeout(()=>{
        this.geofenceDataSource.paginator = this.paginator.toArray()[1];
        this.geofenceDataSource.sort = this.sort.toArray()[1];
      },2000);
    }

    loadGroupData(){
      let objData = { 
        organizationid : this.organizationId,
     };
  
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((data: any) => {
        if(data){
          this.groupGridData = data["groups"];
          this.updateGroupDatasource(this.groupGridData);
          if(this.actionType == 'view' || this.actionType == 'edit' || this.actionType == 'duplicate'){
            this.loadGroupSelectedData(this.groupGridData);
          }
        }
      }, (error) => {
        //console.log(error)
      });
    }

    updateGroupDatasource(tableData: any){
      this.groupDataSource = new MatTableDataSource(tableData);
      this.groupDataSource.filterPredicate = function(data: any, filter: string): boolean {
        return (
          data.name.toString().toLowerCase().includes(filter) ||
          data.poiCount.toString().toLowerCase().includes(filter) ||
          data.geofenceCount.toString().toLowerCase().includes(filter)
        );
      };
      setTimeout(()=>{
        this.groupDataSource.paginator = this.paginator.toArray()[2];
        this.groupDataSource.sort = this.sort.toArray()[2];
      },2000);
    }

    loadGroupSelectedData(tableData: any){
      let selectedGroupList: any = [];
      if(this.actionType == 'view'){
        tableData.forEach((row: any) => {
          let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && item.landmarkType == 'G');
          if (search.length > 0) {
            selectedGroupList.push(row);
          }
        });
        tableData = selectedGroupList;
        this.displayedColumnsGroup= ['name', 'poiCount', 'geofenceCount'];
        this.updateGroupDatasource(tableData);
      }
      else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
        this.selectGroupTableRows();
      }
    }

    selectGroupTableRows(){
      this.groupDataSource.data.forEach((row: any) => {
        let search = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == row.id && item.landmarkType == 'G');
        if (search.length > 0) {
          this.selectedGroup.select(row);
        }
      });
    }

    applyFilterForPOI(filterValue: string) {
      filterValue = filterValue.trim(); // Remove whitespace
      filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
      this.poiDataSource.filter = filterValue;
    }
  
    applyFilterForGeofence(filterValue: string) {
      filterValue = filterValue.trim(); // Remove whitespace
      filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
      this.geofenceDataSource.filter = filterValue;
    }
  
    applyFilterForGroup(filterValue: string) {
      filterValue = filterValue.trim(); // Remove whitespace
      filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
      this.groupDataSource.filter = filterValue;
    }

    loadPOISelectedData(tableData: any){
      let selectedPOIList: any = [];
      if(this.actionType == 'view'){
        tableData.forEach((row: any) => {
          let search = this.selectedRowData.alertLandmarkRefs.filter(item => item.refId == row.id && item.landmarkType == "P");
          if (search.length > 0) {
            selectedPOIList.push(row);
            setTimeout(() => {
              this.PoiCheckboxClicked({checked : true}, row);  
            }, 1000);
          }
        });
        tableData = selectedPOIList;
        this.displayedColumnsPOI= ['icon', 'name', 'categoryName', 'subCategoryName', 'address'];
        this.updatePOIDataSource(tableData);
      }
      else if(this.actionType == 'edit' || this.actionType == 'duplicate'){
        this.selectPOITableRows(this.selectedRowData);
      }
    }

    addMarkerOnMap(){
      this.map.removeObjects(this.map.getObjects());
      this.markerArray.forEach(element => {
        let marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
        this.map.addObject(marker);
        // this.createResizableCircle(this.circularGeofenceFormGroup.controls.radius.value ? parseInt(this.circularGeofenceFormGroup.controls.radius.value) : 0, element);
        this.createResizableCircle(this.alertAdvancedFilterForm.controls.widthInput.value * 1000,element);
      });
      this.geoMarkerArray.forEach(element => {
        if(element.type == "C"){
        this.marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
        this.map.addObject(this.marker);
        this.createResizableCircle(element.distance, element);
        }
        else if(element.type == "O"){
          this.polyPoints = [];
          element.nodes.forEach(item => {
          this.polyPoints.push(Math.abs(item.latitude.toFixed(4)));
          this.polyPoints.push(Math.abs(item.longitude.toFixed(4)));
          this.polyPoints.push(0);
          });
          this.createResizablePolygon(this.map,this.polyPoints,this);
        }
  
    });
    }

    getSVGIcon(){
      let markup = '<svg xmlns="http://www.w3.org/2000/svg" width="28px" height="36px" >' +
      '<path d="M 19 31 C 19 32.7 16.3 34 13 34 C 9.7 34 7 32.7 7 31 C 7 29.3 9.7 ' +
      '28 13 28 C 16.3 28 19 29.3 19 31 Z" fill="#000" fill-opacity=".2"></path>' +
      '<path d="M 13 0 C 9.5 0 6.3 1.3 3.8 3.8 C 1.4 7.8 0 9.4 0 12.8 C 0 16.3 1.4 ' +
      '19.5 3.8 21.9 L 13 31 L 22.2 21.9 C 24.6 19.5 25.9 16.3 25.9 12.8 C 25.9 9.4 24.6 ' +
      '6.1 22.1 3.8 C 19.7 1.3 16.5 0 13 0 Z" fill="#fff"></path>' +
      '<path d="M 13 2.2 C 6 2.2 2.3 7.2 2.1 12.8 C 2.1 16.1 3.1 18.4 5.2 20.5 L ' +
      '13 28.2 L 20.8 20.5 C 22.9 18.4 23.8 16.2 23.8 12.8 C 23.6 7.07 20 2.2 ' +
      '13 2.2 Z" fill="${COLOR}"></path><text transform="matrix( 1 0 0 1 13 18 )" x="0" y="0" fill-opacity="1" ' +
      'fill="#fff" text-anchor="middle" font-weight="bold" font-size="13px" font-family="arial" style="fill:black"></text></svg>';
      
      let locMarkup = '<svg height="24" version="1.1" width="24" xmlns="http://www.w3.org/2000/svg" xmlns:cc="http://creativecommons.org/ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"><g transform="translate(0 -1028.4)"><path d="m12 0c-4.4183 2.3685e-15 -8 3.5817-8 8 0 1.421 0.3816 2.75 1.0312 3.906 0.1079 0.192 0.221 0.381 0.3438 0.563l6.625 11.531 6.625-11.531c0.102-0.151 0.19-0.311 0.281-0.469l0.063-0.094c0.649-1.156 1.031-2.485 1.031-3.906 0-4.4183-3.582-8-8-8zm0 4c2.209 0 4 1.7909 4 4 0 2.209-1.791 4-4 4-2.2091 0-4-1.791-4-4 0-2.2091 1.7909-4 4-4z" fill="#55b242" transform="translate(0 1028.4)"/><path d="m12 3c-2.7614 0-5 2.2386-5 5 0 2.761 2.2386 5 5 5 2.761 0 5-2.239 5-5 0-2.7614-2.239-5-5-5zm0 2c1.657 0 3 1.3431 3 3s-1.343 3-3 3-3-1.3431-3-3 1.343-3 3-3z" fill="#ffffff" transform="translate(0 1028.4)"/></g></svg>';
      
      //let icon = new H.map.Icon(markup.replace('${COLOR}', '#55b242'));
      let icon = new H.map.Icon(locMarkup);
      return icon;
    }

    updatePOIDataSource(tableData: any){
      this.poiDataSource= new MatTableDataSource(tableData);
      this.poiDataSource.filterPredicate = function(data: any, filter: string): boolean {
        return (
          data.name.toString().toLowerCase().includes(filter) ||
          data.categoryName.toString().toLowerCase().includes(filter) ||
          data.subCategoryName.toString().toLowerCase().includes(filter) || 
          data.address.toString().toLowerCase().includes(filter)
        );
      };
      setTimeout(()=>{
        this.poiDataSource.paginator = this.paginator.toArray()[0];
        this.poiDataSource.sort = this.sort.toArray()[0];
      },2000);
    }

    selectPOITableRows(rowData: any, event?:any){
      if(event){
        this.poiDataSource.data.forEach((row: any) => {
          let search = rowData.landmarks.filter(item => item.landmarkid == row.id && item.type == "P");
          if(search.length > 0) {
            if(event.checked)
              this.selectedPOI.select(row);
            else
              this.selectedPOI.deselect(row);  
            this.PoiCheckboxClicked(event,row);
          }
        });
      }
      else{
        this.poiDataSource.data.forEach((row: any) => {
          let search = rowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == row.id && item.landmarkType == "P");
          if(search.length > 0) {
            this.selectedPOI.select(row);
            setTimeout(() => {
              this.PoiCheckboxClicked({checked : true}, row);  
            }, 1000);
          }
        });
      }
    }

    createResizableCircle(_radius: any, rowData: any) {
      var circle = new H.map.Circle(
        { lat: rowData.latitude, lng: rowData.longitude },
  
        _radius,//85000,
        {
          style: { fillColor: 'rgba(138, 176, 246, 0.7)', lineWidth: 0 }
        }
      ),
        circleOutline = new H.map.Polyline(
          circle.getGeometry().getExterior(),
          {
            style: { lineWidth: 8, strokeColor: 'rgba(255, 0, 0, 0)' }
          }
        ),
        circleGroup = new H.map.Group({
          volatility: true, // mark the group as volatile for smooth dragging of all it's objects
          objects: [circle, circleOutline]
        }),
        circleTimeout;
  
      circle.draggable = true;
      circleOutline.draggable = true;
      circleOutline.getGeometry().pushPoint(circleOutline.getGeometry().extractPoint(0));
      this.map.addObject(circleGroup);
      }
    
      createResizablePolygon(map: any, points: any, thisRef: any){
            var svgCircle = '<svg width="50" height="20" version="1.1" xmlns="http://www.w3.org/2000/svg">' +
            '<circle cx="10" cy="10" r="7" fill="transparent" stroke="red" stroke-width="4"/>' +
            '</svg>',
              polygon = new H.map.Polygon(
                new H.geo.Polygon(new H.geo.LineString(points)),
                {
                  style: {fillColor: 'rgba(150, 100, 0, .8)', lineWidth: 0}
                }
              ),
              verticeGroup = new H.map.Group({
                visibility: false
              }),
              mainGroup = new H.map.Group({
                volatility: true, // mark the group as volatile for smooth dragging of all it's objects
                objects: [polygon, verticeGroup]
              }),
              polygonTimeout;
        
          // ensure that the polygon can receive drag events
          polygon.draggable = true;
        
          // create markers for each polygon's vertice which will be used for dragging
          polygon.getGeometry().getExterior().eachLatLngAlt(function(lat, lng, alt, index) {
            var vertice = new H.map.Marker(
              {lat, lng},
              {
                icon: new H.map.Icon(svgCircle, {anchor: {x: 10, y: 10}})
              }
            );
            vertice.draggable = true;
            vertice.setData({'verticeIndex': index})
            verticeGroup.addObject(vertice);
          });
        
          // add group with polygon and it's vertices (markers) on the map
          map.addObject(mainGroup);
        
          // event listener for main group to show markers if moved in with mouse (or touched on touch devices)
          mainGroup.addEventListener('pointerenter', function(evt) {
            if (polygonTimeout) {
              clearTimeout(polygonTimeout);
              polygonTimeout = null;
            }
        
            // show vertice markers
            verticeGroup.setVisibility(true);
          }, true);
        
          // event listener for main group to hide vertice markers if moved out with mouse (or released finger on touch devices)
          // the vertice markers are hidden on touch devices after specific timeout
          mainGroup.addEventListener('pointerleave', function(evt) {
            var timeout = (evt.currentPointer.type == 'touch') ? 1000 : 0;
        
            // hide vertice markers
            polygonTimeout = setTimeout(function() {
              verticeGroup.setVisibility(false);
            }, timeout);
          }, true);
        
          if(thisRef.actionType == 'create'){ //-- only for create polygon geofence
            // event listener for vertice markers group to change the cursor to pointer
            verticeGroup.addEventListener('pointerenter', function(evt) {
              document.body.style.cursor = 'pointer';
            }, true);
          
            // event listener for vertice markers group to change the cursor to default
            verticeGroup.addEventListener('pointerleave', function(evt) {
              document.body.style.cursor = 'default';
            }, true);
          
            // event listener for vertice markers group to resize the geo polygon object if dragging over markers
            verticeGroup.addEventListener('drag', function(evt) {
              var pointer = evt.currentPointer,
                  geoLineString = polygon.getGeometry().getExterior(),
                  geoPoint = map.screenToGeo(pointer.viewportX, pointer.viewportY);
             // set new position for vertice marker
              evt.target.setGeometry(geoPoint);
          
              // set new position for polygon's vertice
              geoLineString.removePoint(evt.target.getData()['verticeIndex']);
              geoLineString.insertPoint(evt.target.getData()['verticeIndex'], geoPoint);
              polygon.setGeometry(new H.geo.Polygon(geoLineString));
          
              // stop propagating the drag event, so the map doesn't move
              evt.stopPropagation();
            }, true);
    
            verticeGroup.addEventListener('dragend', function (ev) {
              var coordinate = map.screenToGeo(ev.currentPointer.viewportX,
                ev.currentPointer.viewportY);
                let nodeIndex = ev.target.getData()['verticeIndex'];
              let _position = Math.abs(coordinate.lat.toFixed(4)) + "," + Math.abs(coordinate.lng.toFixed(4));
                if(_position){
                  thisRef.hereService.getAddressFromLatLng(_position).then(result => {
                    let locations = <Array<any>>result;
                    let data = locations[0].Location.Address;
                    let pos = locations[0].Location.DisplayPosition;
                    thisRef.setAddressValues('updatePoint', data, pos, nodeIndex);
                  }, error => {
                  });
                }
    
            }, false);
          }
      }

      geofenceCheckboxClicked(event: any, row: any) {

        if(event.checked){ 
          this.geoMarkerArray.push(row);
        }else{ 
          let arr = this.geoMarkerArray.filter(item => item.id != row.id);
          this.geoMarkerArray = arr;
        }
        this.addCircleOnMap(event);
      // });
        }

        addCircleOnMap(event: any){
          if(event.checked == false){
        this.map.removeObjects(this.map.getObjects());
      }
    //adding circular geofence points on map
        this.geoMarkerArray.forEach(element => {
          if(element.type == "C"){
          this.marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
          this.map.addObject(this.marker);
          
          this.createResizableCircle(element.distance, element);
          }
          // "PolygonGeofence"
          else{
            this.polyPoints = [];
            element.nodes.forEach(item => {
            this.polyPoints.push(Math.abs(item.latitude.toFixed(4)));
            this.polyPoints.push(Math.abs(item.longitude.toFixed(4)));
            this.polyPoints.push(0);
            });
            this.createResizablePolygon(this.map,this.polyPoints,this);
          }
    
      });
      //adding poi geofence points on map
      this.markerArray.forEach(element => {
        let marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
        this.map.addObject(marker);
      });
    
        }

        onPOIClick(row: any){
          const colsList = ['icon', 'landmarkname', 'categoryname', 'subcategoryname', 'address'];
          const colsName = [this.translationData.lblIcon || 'Icon', this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category', this.translationData.lblAddress || 'Address'];
          const tableTitle = this.translationData.lblPOI || 'POI';
          let objData = { 
            organizationid : this.organizationId,
            groupid : row.id
          };
            this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
            this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => item.type == "P");
            if(this.selectedRowData.length > 0){
              this.selectedRowData.forEach(element => {
                if(element.icon && element.icon != '' && element.icon.length > 0){
                  let TYPED_ARRAY = new Uint8Array(element.icon);
                  let STRING_CHAR = String.fromCharCode.apply(null, TYPED_ARRAY);
                  let base64String = btoa(STRING_CHAR);
                  element.icon = this.domSanitizer.bypassSecurityTrustUrl('data:image/jpg;base64,' + base64String);
                }else{
                  element.icon = '';
                }
              });
              this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
            }
          });
        }

        callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any) {
          const dialogConfig = new MatDialogConfig();
          dialogConfig.disableClose = true;
          dialogConfig.autoFocus = true;
          dialogConfig.data = {
            tableData: tableData,
            colsList: colsList,
            colsName: colsName,
            tableTitle: tableTitle
          }
          this.dialogRef = this.dialog.open(CommonTableComponent, dialogConfig);
        }

        onGeofenceClick(row: any){
          const colsList = ['landmarkname', 'categoryname', 'subcategoryname'];
          const colsName = ['Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category'];
          const tableTitle = this.translationData.lblGeofence || 'Geofence';
          let objData = { 
            organizationid : this.organizationId,
            groupid : row.id
         };
            this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
            this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => (item.type == "C" || item.type == "O"));
            this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
          });
        }

        onGroupSelect(event: any, row: any){
          let objData = { 
            organizationid : this.organizationId,
            groupid : row.id
          };
          let groupDetails= [];
          if(event.checked){
            this.groupArray.push(row);
            this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupData) => {
              groupDetails = groupData["groups"][0];
              this.selectPOITableRows(groupDetails, event);
              this.selectGeofenceTableRows(groupDetails, event);
            });
          }
          else{
            this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupData) => {
              groupDetails = groupData["groups"][0];
              this.selectPOITableRows(groupDetails, event);
              this.selectGeofenceTableRows(groupDetails, event);
            });
            let arr = this.groupArray.filter(item => item.id != row.id);
            this.groupArray = arr;
          }
        }

        selectGeofenceTableRows(rowData: any, event?: any){
          if(event){
            this.geofenceDataSource.data.forEach((row: any) => {
              let search = rowData.landmarks.filter(item => item.landmarkid == row.id && (item.type == "C" || item.type == "O"));
              if (event && search.length > 0) {
                if(event.checked)
                  this.selectedGeofence.select(row);
                else
                  this.selectedGeofence.deselect(row);
                this.geofenceCheckboxClicked(event,row);
              }
            });
          }
          else{
            this.geofenceDataSource.data.forEach((row: any) => {
              let search = rowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == row.id && (item.landmarkType == "C" || item.landmarkType == "O"));
              if(search.length > 0) {
                this.selectedGeofence.select(row);
                setTimeout(() => {
                  this.geofenceCheckboxClicked({checked : true}, row);  
                }, 1000);
              }
            });
          }
        }

      masterToggleForPOI() {
        this.isAllSelectedForPOI()
          ? this.selectedPOI.clear()
          : this.poiDataSource.data.forEach((row) =>
            this.selectedPOI.select(row)
          );
      }
    
      isAllSelectedForPOI() {
        const numSelected = this.selectedPOI.selected.length;
        const numRows = this.poiDataSource.data.length;
        return numSelected === numRows;
      }
    
      checkboxLabelForPOI(row?: any): string {
        if (row)
          return `${this.isAllSelectedForPOI() ? 'select' : 'deselect'} all`;
        else
          return `${this.selectedPOI.isSelected(row) ? 'deselect' : 'select'
            } row`;
      }

      masterToggleForGeofence() {
        this.isAllSelectedForGeofence()
          ? this.selectedGeofence.clear()
          : this.geofenceDataSource.data.forEach((row) =>
            this.selectedGeofence.select(row)
          );
      }
    
      isAllSelectedForGeofence() {
        const numSelected = this.selectedGeofence.selected.length;
        const numRows = this.geofenceDataSource.data.length;
        return numSelected === numRows;
      }
    
      checkboxLabelForGeofence(row?: any): string {
        if (row)
          return `${this.isAllSelectedForGeofence() ? 'select' : 'deselect'} all`;
        else
          return `${this.selectedGeofence.isSelected(row) ? 'deselect' : 'select'
            } row`;
      }
    
      masterToggleForGroup(event) {
        this.isAllSelectedForGroup()
          ? this.selectedGroup.clear()
          : this.groupDataSource.data.forEach((row) =>
            this.selectedGroup.select(row)
          );

          this.groupDataSource.data.forEach(row => {
            this.onGroupSelect(event, row);
          });
      }
    
      isAllSelectedForGroup() {
        const numSelected = this.selectedGroup.selected.length;
        const numRows = this.groupDataSource.data.length;
        return numSelected === numRows;
      }
    
      checkboxLabelForGroup(row?: any): string {
        if (row)
          return `${this.isAllSelectedForGroup() ? 'select' : 'deselect'} all`;
        else
          return `${this.selectedGroup.isSelected(row) ? 'deselect' : 'select'
            } row`;
      }

      sliderChanged(){
        this.poiWidthKm = this.poiWidth / 1000;
        this.alertAdvancedFilterForm.controls.widthInput.setValue(this.poiWidthKm);
        if(this.markerArray.length > 0){
        this.addMarkerOnMap();
        }
    }
   
    changeSliderInput(){
     this.poiWidthKm = this.alertAdvancedFilterForm.controls.widthInput.value;
     this.poiWidth = this.poiWidthKm * 1000;
   }

   getAdvancedFilterAlertPayload(){

let urgencylevelStartDate = 0;
let urgencylevelEndDate = 0;
if(this.selectedApplyOn == 'C'){
  this.alertTimingDetail = this.periodSelectionComponent.getAlertTimingPayload();
  urgencylevelStartDate = Util.convertDateToUtc(this.setStartEndDateTime(this.alertAdvancedFilterForm.controls.fromDate.value, this.alertAdvancedFilterForm.controls.fromTimeRange.value, "start"));
  urgencylevelEndDate = Util.convertDateToUtc(this.setStartEndDateTime(this.alertAdvancedFilterForm.controls.toDate.value, this.alertAdvancedFilterForm.controls.toTimeRange.value, "end"));;
  this.alertTimingDetail.forEach(element => {
    element["type"] = "F";
  });
}
else{
    this.alertTimingDetail = [];
    urgencylevelStartDate = 0;
    urgencylevelEndDate = 0;
  }
  
//Fuel Increase & Fuel Loss
     if ((this.alert_category_selected == 'F') && (this.alert_type_selected == 'P' || this.alert_type_selected == 'L' || this.alert_type_selected == 'T')) {

       if (this.actionType == 'create' || this.actionType == 'duplicate' || this.actionType == 'edit') {
         if (this.geoMarkerArray.length != 0) {
           this.geoMarkerArray.forEach(element => {
             let obj = {
               "alertUrgencyLevelId": 0,
               "filterType": "N",
               "thresholdValue": 0,
               "unitType": "N",
               "landmarkType": element.type,
               "refId": element.id,
               "positionType": "N",
               "alertTimingDetails": []
             }
             if(this.actionType == 'edit'){
              let geofenceLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
              obj["id"] = geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = element.state == 'Active' ? 'A' : 'I';
             }
            
             this.advancedAlertPayload.push(obj);
           })
         }
         if(this.markerArray.length != 0) {
           this.markerArray.forEach(element => {
             let obj = {
               "alertUrgencyLevelId": 0,
               "filterType": "N",
               "thresholdValue": 0,
               "unitType": "N",
               "landmarkType": "P",
               "refId": element.id,
               "positionType": "N",
               "alertTimingDetails": []
             }
             if(this.actionType == 'edit'){
              let poiLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
              obj["id"] = poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = element.state == 'Active' ? 'A' : 'I';
             }
             this.advancedAlertPayload.push(obj);
           });
         }

         if(this.groupArray.length != 0) {
          this.groupArray.forEach(element => {
            let obj = {
              "alertUrgencyLevelId": 0,
              "filterType": "N",
              "thresholdValue": 0,
              "unitType": "N",
              "landmarkType": "G",
              "refId": element.id,
              "positionType": "N",
              "alertTimingDetails": []
            }
            if(this.actionType == 'edit'){
              let groupLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id && item.landmarkType == 'G'); 
              obj["id"] = groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = element.state == 'Active' ? 'A' : 'I';
             }
            this.advancedAlertPayload.push(obj);
          });
        }

       }
      }

      // entering & existing zone
  if((this.alert_category_selected == 'L') && (this.alert_type_selected == 'N' || this.alert_type_selected == 'X'))
  {

    if (this.actionType == 'create' || this.actionType == 'duplicate' || this.actionType == 'edit') {
      let obj;
      this.thresholdVal = 0;
      this.filterType = "N";
      if(this.isOccurenceSelected){
        this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.occurences.value);
      obj = {
        "alertUrgencyLevelId": 0,
        "filterType": "N",
        "thresholdValue": this.thresholdVal,
        "unitType": "N",
        "landmarkType": "N",
        "refId": 0,
        "positionType": "N",
        "alertTimingDetails": this.alertTimingDetail
      }
      if(this.actionType == 'edit'){
        let noOfOccuranceRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'N'); 
        obj["id"] = noOfOccuranceRefArr.length > 0 ? noOfOccuranceRefArr[0].id : 0;
        obj["alertId"] = this.selectedRowData.id;
        obj["state"] = 'A';
        obj["alertTimingDetails"]["refId"] = noOfOccuranceRefArr.length > 0 ? noOfOccuranceRefArr[0].id : 0;
       }
       this.advancedAlertPayload.push(obj);
    }
      
    
      if(this.isDurationSelected){
      this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.duration.value);
      obj = {
        "alertUrgencyLevelId": 0,
        "filterType": "D",
        "thresholdValue": this.thresholdVal,
        "unitType": "N",
        "landmarkType": "N",
        "refId": 0,
        "positionType": "N",
        "alertTimingDetails": this.alertTimingDetail
      }
      if(this.actionType == 'edit'){
        let durationRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'D'); 
        obj["id"] = durationRefArr.length > 0 ? durationRefArr[0].id : 0;
        obj["alertId"] = this.selectedRowData.id;
        obj["state"] = 'A';
        obj["alertTimingDetails"]["refId"] = durationRefArr.length > 0 ? durationRefArr[0].id : 0;
       }
      this.advancedAlertPayload.push(obj);
    }
    if(!this.isOccurenceSelected && !this.isDurationSelected){
      obj = {
        "alertUrgencyLevelId": 0,
        "filterType": "N",
        "thresholdValue": this.thresholdVal,
        "unitType": "N",
        "landmarkType": "N",
        "refId": 0,
        "positionType": "N",
        "alertTimingDetails": this.alertTimingDetail
      }
      if(this.actionType == 'edit'){
        let periodRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs[0].refId;
        obj["id"] = periodRefArr.length > 0 ? periodRefArr[0].id : 0;
        obj["alertId"] = this.selectedRowData.id;
        obj["state"] = 'A';
        obj["alertTimingDetails"]["refId"] = periodRefArr.length > 0 ? periodRefArr[0].id : 0;
       }
      this.advancedAlertPayload.push(obj);
    }

    
    }
  }

  // excessive avg idling
  if((this.alert_category_selected == 'F') && (this.alert_type_selected == 'I')){

      if(this.actionType == 'create' || this.actionType == 'duplicate' || this.actionType == 'edit') {
        this.filterType = 'N';
        this.thresholdVal = 0;
        if(this.isOccurenceSelected == true){
          this.filterType = 'N';
          this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.occurences.value);
          if(!this.isPoiSelected){
            let obj = {
              "alertUrgencyLevelId": 0,
              "filterType": 'N',
              "thresholdValue": this.thresholdVal,
              "unitType": "N",
              "landmarkType": 'N',
              "refId": 0,
              "positionType": "N",
              "alertTimingDetails": this.alertTimingDetail
            }
            if(this.actionType == 'edit'){
              let noOfOccuranceRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'N'); 
              obj["id"] = noOfOccuranceRefArr.length > 0 ? noOfOccuranceRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = 'A';
              obj["alertTimingDetails"]["refId"] = noOfOccuranceRefArr.length > 0 ? noOfOccuranceRefArr[0].id : 0;
             }
            this.advancedAlertPayload.push(obj);
          }
        }

        if(!this.isPoiSelected && !this.isOccurenceSelected){
          let obj = {
            "alertUrgencyLevelId": 0,
            "filterType": 'N',
            "thresholdValue": 0,
            "unitType": "N",
            "landmarkType": 'N',
            "refId": 0,
            "positionType": "N",
            "alertTimingDetails": this.alertTimingDetail
          }
          if(this.actionType == 'edit'){
            let periodRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.refId;
            obj["id"] = periodRefArr.length > 0 ? periodRefArr[0].id : 0;
            obj["alertId"] = this.selectedRowData.id;
            obj["state"] = 'A';
            obj["alertTimingDetails"]["refId"] = periodRefArr.length > 0 ? periodRefArr[0].id : 0;
           }
          this.advancedAlertPayload.push(obj);
        }

        if (this.geoMarkerArray.length != 0) {
          this.geoMarkerArray.forEach(element => {
            let obj = {
              "alertUrgencyLevelId": 0,
              "filterType": this.filterType,
              "thresholdValue": this.thresholdVal,
              "unitType": "N",
              "landmarkType": element.type,
              "refId": element.id,
              "positionType": "N",
              "alertTimingDetails": this.alertTimingDetail
            }
            if(this.actionType == 'edit'){
              let geofenceLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
              obj["id"] = geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = element.state == 'Active' ? 'A' : 'I';
              obj["alertTimingDetails"]["refId"] = geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0;
             }
            this.advancedAlertPayload.push(obj);
          })
        }
        if(this.markerArray.length != 0) {
          this.markerArray.forEach(element => {
            let obj = {
              "alertUrgencyLevelId": 0,
              "filterType": this.filterType,
              "thresholdValue": this.thresholdVal,
              "unitType": "N",
              "landmarkType": "P",
              "refId": element.id,
              "positionType": "N",
              "alertTimingDetails": this.alertTimingDetail
            }
            if(this.actionType == 'edit'){
              let poiLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
              obj["id"] = poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = element.state == 'Active' ? 'A' : 'I';
              obj["alertTimingDetails"]["refId"] = poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0;
             }
            this.advancedAlertPayload.push(obj);
          });
        }

        if(this.groupArray.length != 0) {
         this.groupArray.forEach(element => {
           let obj = {
             "alertUrgencyLevelId": 0,
             "filterType": this.filterType,
             "thresholdValue": this.thresholdVal,
             "unitType": "N",
             "landmarkType": "G",
             "refId": element.id,
             "positionType": "N",
             "alertTimingDetails": this.alertTimingDetail
           }
           if(this.actionType == 'edit'){
            let groupLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id && item.landmarkType == 'G'); 
            obj["id"] = groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0;
            obj["alertId"] = this.selectedRowData.id;
            obj["state"] = element.state == 'Active' ? 'A' : 'I';
            obj["alertTimingDetails"]["refId"] = groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0;
           }
           this.advancedAlertPayload.push(obj);
         });
       }
      }

  }

    // hours of service
    if((this.alert_category_selected == 'L') && (this.alert_type_selected == 'S')){

      if (this.actionType == 'create' || this.actionType == 'duplicate' || this.actionType == 'edit') {
        let obj;
        if(this.isDurationSelected){
          this.filterType = 'D';
          this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.duration.value);
          let filterObj = {
            "type" : this.filterType,
            "val" : this.thresholdVal
          }
          this.filterTypeArray.push(filterObj);
          if(!this.isPoiSelected){
          obj = {
          "alertUrgencyLevelId": 0,
          "filterType": "D",
          "thresholdValue": this.thresholdVal,
          "unitType": "N",
          "landmarkType": 'N',
          "refId": 0,
          "positionType": "N",
          "alertTimingDetails": []
        }
        if(this.actionType == 'edit'){
          let durationRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'D'); 
          obj["id"] = durationRefArr.length > 0 ? durationRefArr[0].id : 0;
          obj["alertId"] = this.selectedRowData.id;
          obj["state"] = 'A';
          obj["alertTimingDetails"]["refId"] = durationRefArr.length > 0 ? durationRefArr[0].id : 0;
         }
        this.advancedAlertPayload.push(obj);
      }
      }
  
      if(this.isDistanceSelected){
        this.filterType = 'T';
        this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.distance.value);
        let filterObj = {
          "type" : this.filterType,
          "val" : this.thresholdVal
        }
        this.filterTypeArray.push(filterObj);
        if(!this.isPoiSelected){
        obj = {
        "alertUrgencyLevelId": 0,
        "filterType": "T",
        "thresholdValue": this.thresholdVal,
        "unitType": "N",
        "landmarkType": 'N',
        "refId": 0,
        "positionType": "N",
        "alertTimingDetails": []
      }
      if(this.actionType == 'edit'){
        let distanceRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'T'); 
        obj["id"] = distanceRefArr.length > 0 ? distanceRefArr[0].id : 0;
        obj["alertId"] = this.selectedRowData.id;
        obj["state"] = 'A';
        obj["alertTimingDetails"]["refId"] = distanceRefArr.length > 0 ? distanceRefArr[0].id : 0;
       }
      this.advancedAlertPayload.push(obj);
    }
    }
  
    if (this.geoMarkerArray.length != 0) {
      this.geoMarkerArray.forEach(element => {
        if(this.filterTypeArray.length == 0){
        let obj = {
          "alertUrgencyLevelId": 0,
          "filterType": 'N',
          "thresholdValue": 0,
          "unitType": "N",
          "landmarkType": element.type,
          "refId": element.id,
          "positionType": "N",
          "alertTimingDetails": this.alertTimingDetail
        }
        if(this.actionType == 'edit'){
          let geofenceLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
          obj["id"] = geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0;
          obj["alertId"] = this.selectedRowData.id;
          obj["state"] = element.state == 'Active' ? 'A' : 'I';
          obj["alertTimingDetails"]["refId"] = geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0;
         }
        this.advancedAlertPayload.push(obj);
      }
  
        if (this.filterTypeArray.length != 0) {
          this.filterTypeArray.forEach(item => {
            this.filterType = item.type;
            this.thresholdVal = parseInt(item.val);
            let obj = {
              "alertUrgencyLevelId": 0,
              "filterType": this.filterType,
              "thresholdValue": this.thresholdVal,
              "unitType": "N",
              "landmarkType": element.type,
              "refId": element.id,
              "positionType": "N",
              "alertTimingDetails": this.alertTimingDetail
            }
              if(this.actionType == 'edit'){
                let filterTypeRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == this.filterType); 
                obj["id"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
                obj["alertId"] = this.selectedRowData.id;
                obj["state"] = 'A';
                obj["alertTimingDetails"]["refId"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
               }
            this.advancedAlertPayload.push(obj);
          });
        }
        
      })
    }
        if(this.markerArray.length != 0) {
          this.markerArray.forEach(element => {
            if(this.filterTypeArray.length == 0){
              let obj = {
                "alertUrgencyLevelId": 0,
                "filterType": "N",
                "thresholdValue": 0,
                "unitType": "N",
                "landmarkType": "P",
                "refId": element.id,
                "positionType": "N",
                "alertTimingDetails": []
                
              }
              if(this.actionType == 'edit'){
                let poiLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
                obj["id"] = poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0;
                obj["alertId"] = this.selectedRowData.id;
                obj["state"] = element.state == 'Active' ? 'A' : 'I';
                obj["alertTimingDetails"]["refId"] = poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0;
               }
              this.advancedAlertPayload.push(obj);
            }
            if (this.filterTypeArray.length != 0) {
              this.filterTypeArray.forEach(item => {
                this.filterType = item.type;
                this.thresholdVal = parseInt(item.val);
            let obj = {
              "alertUrgencyLevelId": 0,
              "filterType": this.filterType,
              "thresholdValue": this.thresholdVal,
              "unitType": "N",
              "landmarkType": "P",
              "refId": element.id,
              "positionType": "N",
              "alertTimingDetails": this.alertTimingDetail
            }
            if(this.actionType == 'edit'){
              let filterTypeRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == this.filterType); 
              obj["id"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = 'A';
              obj["alertTimingDetails"]["refId"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
             }
            this.advancedAlertPayload.push(obj);
          });
        }
          });
        
        }
  
        if(this.groupArray.length != 0) {
         this.groupArray.forEach(element => {
          if(this.filterTypeArray.length == 0){
           let obj = {
             "alertUrgencyLevelId": 0,
             "filterType": "N",
             "thresholdValue": 0,
             "unitType": "N",
             "landmarkType": "G",
             "refId": element.id,
             "positionType": "N",
             "alertTimingDetails": []
           }
           if(this.actionType == 'edit'){
            let groupLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id && item.landmarkType == 'G'); 
            obj["id"] = groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0;
            obj["alertId"] = this.selectedRowData.id;
            obj["state"] = element.state == 'Active' ? 'A' : 'I';
            obj["alertTimingDetails"]["refId"] = groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0;
           }
           this.advancedAlertPayload.push(obj);
          }
  
          if (this.filterTypeArray.length != 0) {
            this.filterTypeArray.forEach(item => {
              this.filterType = item.type;
              this.thresholdVal = parseInt(item.val);
          let obj = {
            "alertUrgencyLevelId": 0,
            "filterType": this.filterType,
            "thresholdValue": this.thresholdVal,
            "unitType": "N",
            "landmarkType": "G",
            "refId": element.id,
            "positionType": "N",
            "alertTimingDetails": this.alertTimingDetail
          }
          if(this.actionType == 'edit'){
            let filterTypeRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == this.filterType); 
            obj["id"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
            obj["alertId"] = this.selectedRowData.id;
            obj["state"] = 'A';
            obj["alertTimingDetails"]["refId"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
           }
          this.advancedAlertPayload.push(obj);
        });
      }
         });
       }
  
      }
      }
  
      // excessive average speed
      if((this.alert_category_selected == 'F') && (this.alert_type_selected == 'A')){
  
        if (this.actionType == 'create' || this.actionType == 'duplicate'  || this.actionType == 'edit') {
          let obj;
          if(this.isDurationSelected){
            this.filterType = 'D';
            this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.duration.value);
            let filterObj = {
              "type" : this.filterType,
              "val" : this.thresholdVal
            }
            this.filterTypeArray.push(filterObj);
            if(!this.isPoiSelected){
            obj = {
            "alertUrgencyLevelId": 0,
            "filterType": "D",
            "thresholdValue": this.thresholdVal,
            "unitType": "N",
            "landmarkType": 'N',
            "refId": 0,
            "positionType": "N",
            "alertTimingDetails": this.alertTimingDetail
          }
          if(this.actionType == 'edit'){
            let durationRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'D'); 
            obj["id"] = durationRefArr.length > 0 ? durationRefArr[0].id : 0;
            obj["alertId"] = this.selectedRowData.id;
            obj["state"] = 'A';
            obj["alertTimingDetails"]["refId"] = durationRefArr.length > 0 ? durationRefArr[0].id : 0;
           }
          this.advancedAlertPayload.push(obj);
        }
        }
    
        if(this.isDistanceSelected){
          this.filterType = 'T';
          this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.distance.value);
          let filterObj = {
            "type" : this.filterType,
            "val" : this.thresholdVal
          }
          this.filterTypeArray.push(filterObj);
          if(!this.isPoiSelected){
          obj = {
          "alertUrgencyLevelId": 0,
          "filterType": "T",
          "thresholdValue": this.thresholdVal,
          "unitType": "N",
          "landmarkType": 'N',
          "refId": 0,
          "positionType": "N",
          "alertTimingDetails": this.alertTimingDetail
        }
        if(this.actionType == 'edit'){
          let distanceRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'T'); 
          obj["id"] = distanceRefArr.length > 0 ? distanceRefArr[0].id : 0;
          obj["alertId"] = this.selectedRowData.id;
          obj["state"] = 'A';
          obj["alertTimingDetails"]["refId"] = distanceRefArr.length > 0 ? distanceRefArr[0].id : 0;
         }
        this.advancedAlertPayload.push(obj);
      }
      }
    
      if(this.isOccurenceSelected){
        this.filterType = 'N';
        this.thresholdVal = parseInt(this.alertAdvancedFilterForm.controls.occurences.value);
        let filterObj = {
          "type" : this.filterType,
          "val" : this.thresholdVal
        }
        this.filterTypeArray.push(filterObj);
        if(!this.isPoiSelected){
        obj = {
        "alertUrgencyLevelId": 0,
        "filterType": "N",
        "thresholdValue": this.thresholdVal,
        "unitType": "N",
        "landmarkType": 'N',
        "refId": 0,
        "positionType": "N",
        "alertTimingDetails": this.alertTimingDetail
      }
      
      if(this.actionType == 'edit'){
        let noOfOccuranceRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == 'N'); 
        obj["id"] = noOfOccuranceRefArr.length > 0 ? noOfOccuranceRefArr[0].id : 0;
        obj["alertId"] = this.selectedRowData.id;
        obj["state"] = 'A';
        obj["alertTimingDetails"]["refId"] = noOfOccuranceRefArr.length > 0 ? noOfOccuranceRefArr[0].id : 0;
       }
      this.advancedAlertPayload.push(obj);
    }
    }

    if(!this.isDurationSelected && !this.isDistanceSelected && !this.isOccurenceSelected){
      this.filterType = 'N';
      this.thresholdVal = 0;
      let filterObj = {
        "type" : this.filterType,
        "val" : this.thresholdVal
      }
      this.filterTypeArray.push(filterObj);
      if(!this.isPoiSelected){
      obj = {
      "alertUrgencyLevelId": 0,
      "filterType": "N",
      "thresholdValue": this.thresholdVal,
      "unitType": "N",
      "landmarkType": 'N',
      "refId": 0,
      "positionType": "N",
      "alertTimingDetails": this.alertTimingDetail
    }
    if(this.actionType == 'edit'){
      let periodRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs[0].id;
      obj["id"] = periodRefArr;
      obj["alertId"] = this.selectedRowData.id;
      obj["state"] = 'A';
      obj["alertTimingDetails"]["refId"] = periodRefArr;
     }
    this.advancedAlertPayload.push(obj);
  }
  }


      if (this.geoMarkerArray.length != 0) {
        this.geoMarkerArray.forEach(element => {
          if(this.filterTypeArray.length == 0){
          let obj = {
            "alertUrgencyLevelId": 0,
            "filterType": 'N',
            "thresholdValue": 0,
            "unitType": "N",
            "landmarkType": element.type,
            "refId": element.id,
            "positionType": "N",
            "alertTimingDetails": this.alertTimingDetail
          }
          if(this.actionType == 'edit'){
            let geofenceLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
            obj["id"] = geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0;
            obj["alertId"] = this.selectedRowData.id;
            obj["state"] = element.state == 'Active' ? 'A' : 'I';
            obj["alertTimingDetails"]["refId"] = geofenceLandmarkRefArr.length > 0 ? geofenceLandmarkRefArr[0].id : 0;
           }
          this.advancedAlertPayload.push(obj);
        }
    
          if (this.filterTypeArray.length != 0) {
            this.filterTypeArray.forEach(item => {
              this.filterType = item.type;
              this.thresholdVal = parseInt(item.val);
              let obj = {
                "alertUrgencyLevelId": 0,
                "filterType": this.filterType,
                "thresholdValue": this.thresholdVal,
                "unitType": "N",
                "landmarkType": element.type,
                "refId": element.id,
                "positionType": "N",
                "alertTimingDetails": this.alertTimingDetail
              }
              if(this.actionType == 'edit'){
                let filterTypeRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == this.filterType); 
                obj["id"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
                obj["alertId"] = this.selectedRowData.id;
                obj["state"] = 'A';
                obj["alertTimingDetails"]["refId"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
               }
              this.advancedAlertPayload.push(obj);
            });
          }
          
        })
      }
          if(this.markerArray.length != 0) {
            this.markerArray.forEach(element => {
              if(this.filterTypeArray.length == 0){
                let obj = {
                  "alertUrgencyLevelId": 0,
                  "filterType": "N",
                  "thresholdValue": 0,
                  "unitType": "N",
                  "landmarkType": "P",
                  "refId": element.id,
                  "positionType": "N",
                  "alertTimingDetails": []
                  
                }
                if(this.actionType == 'edit'){
                  let poiLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id); 
                  obj["id"] = poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0;
                  obj["alertId"] = this.selectedRowData.id;
                  obj["state"] = element.state == 'Active' ? 'A' : 'I';
                  obj["alertTimingDetails"]["refId"] = poiLandmarkRefArr.length > 0 ? poiLandmarkRefArr[0].id : 0;
                 }
                this.advancedAlertPayload.push(obj);
              }
              if (this.filterTypeArray.length != 0) {
                this.filterTypeArray.forEach(item => {
                  this.filterType = item.type;
                  this.thresholdVal = parseInt(item.val);
              let obj = {
                "alertUrgencyLevelId": 0,
                "filterType": this.filterType,
                "thresholdValue": this.thresholdVal,
                "unitType": "N",
                "landmarkType": "P",
                "refId": element.id,
                "positionType": "N",
                "alertTimingDetails": this.alertTimingDetail
              }
              if(this.actionType == 'edit'){
                let filterTypeRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == this.filterType); 
                obj["id"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
                obj["alertId"] = this.selectedRowData.id;
                obj["state"] = 'A';
                obj["alertTimingDetails"]["refId"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
               }
              this.advancedAlertPayload.push(obj);
            });
          }
            });
          
          }
    
          if(this.groupArray.length != 0) {
           this.groupArray.forEach(element => {
            if(this.filterTypeArray.length == 0){
             let obj = {
               "alertUrgencyLevelId": 0,
               "filterType": "N",
               "thresholdValue": 0,
               "unitType": "N",
               "landmarkType": "G",
               "refId": element.id,
               "positionType": "N",
               "alertTimingDetails": []
             }
             if(this.actionType == 'edit'){
              let groupLandmarkRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.refId == element.id && item.landmarkType == 'G'); 
              obj["id"] = groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = element.state == 'Active' ? 'A' : 'I';
              obj["alertTimingDetails"]["refId"] = groupLandmarkRefArr.length > 0 ? groupLandmarkRefArr[0].id : 0;
             }
             this.advancedAlertPayload.push(obj);
            }
    
            if (this.filterTypeArray.length != 0) {
              this.filterTypeArray.forEach(item => {
                this.filterType = item.type;
                this.thresholdVal = parseInt(item.val);
            let obj = {
              "alertUrgencyLevelId": 0,
              "filterType": this.filterType,
              "thresholdValue": this.thresholdVal,
              "unitType": "N",
              "landmarkType": "G",
              "refId": element.id,
              "positionType": "N",
              "alertTimingDetails": this.alertTimingDetail
            }
            if(this.actionType == 'edit'){
              let filterTypeRefArr = this.selectedRowData.alertUrgencyLevelRefs[0].alertFilterRefs.filter(item => item.filterType == this.filterType); 
              obj["id"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
              obj["alertId"] = this.selectedRowData.id;
              obj["state"] = 'A';
              obj["alertTimingDetails"]["refId"] = filterTypeRefArr.length > 0 ? filterTypeRefArr[0].id : 0;
             }
            this.advancedAlertPayload.push(obj);
          });
        }
           });
         }
    
        }
        }

       return {"urgencylevelStartDate" : urgencylevelStartDate, "urgencylevelEndDate" : urgencylevelEndDate, "advancedAlertPayload" : this.advancedAlertPayload};
   
  }

  setStartEndDateTime(date: any, timeObj: any, type: any){
    let _x = timeObj.split(":")[0];
    let _y = timeObj.split(":")[1];
    
    date.setHours(_x);
    date.setMinutes(_y);
    date.setSeconds(type == 'start' ? '00' : '59');
    return date;
  }

}
