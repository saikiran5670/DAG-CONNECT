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

declare var H: any;

@Component({
  selector: 'app-alert-advanced-filter',
  templateUrl: './alert-advanced-filter.component.html',
  styleUrls: ['./alert-advanced-filter.component.css']
})
export class AlertAdvancedFilterComponent implements OnInit {
  @Input() translationData: any = [];
  @Input() alert_category_selected : any;
  @Input() alert_type_selected : any;
  @Input() selectedRowData : any;
  @Input() actionType :any;
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
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
  selectedPoiSite: any;
  marker: any;
  markerArray: any = [];
  geoMarkerArray: any = [];
  map: any;
  polyPoints: any = [];
  poiDataSource: any = new MatTableDataSource([]);
  geofenceDataSource: any = new MatTableDataSource([]);
  groupDataSource: any = new MatTableDataSource([]);
  selectedPOI = new SelectionModel(true, []);
  private platform: any;
  poiGridData = [];
  
  @ViewChild("map")
  private mapElement: ElementRef;
  constructor(private _formBuilder: FormBuilder,private poiService: POIService,private domSanitizer: DomSanitizer,) {
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
      duration: ['']
    })
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
    }, 1000);
    this.loadPOIData();
  }

  onChangeDistance(event: any){
    if(event.checked){
      this.isDistanceSelected= true;
    }
    else{
      this.isDistanceSelected= false;
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
        this.poiDataSource.paginator = this.paginator.toArray()[1];
        this.poiDataSource.sort = this.sort.toArray()[1];
      });
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
          let search = rowData.alertLandmarkRefs.filter(item => item.refId == row.id && item.landmarkType == "P");
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

}
