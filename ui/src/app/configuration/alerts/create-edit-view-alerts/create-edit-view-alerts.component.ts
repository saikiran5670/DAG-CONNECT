import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChildren, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DomSanitizer } from '@angular/platform-browser';
import { AlertService } from 'src/app/services/alert.service';
import { CorridorService } from 'src/app/services/corridor.service';
import { GeofenceService } from 'src/app/services/landmarkGeofence.service';
import { LandmarkGroupService } from 'src/app/services/landmarkGroup.service';
import { POIService } from 'src/app/services/poi.service';
import { CommonTableComponent } from 'src/app/shared/common-table/common-table.component';
import { CustomValidators } from 'src/app/shared/custom.validators';

declare var H: any;

@Component({
  selector: 'app-create-edit-view-alerts',
  templateUrl: './create-edit-view-alerts.component.html',
  styleUrls: ['./create-edit-view-alerts.component.less']
})
export class CreateEditViewAlertsComponent implements OnInit {
  @Output() backToPage = new EventEmitter<any>();
  @Input() actionType: any;
  @Input() translationData: any = [];
  @Input() selectedRowData: any;
  @Input() alertCategoryList: any;
  @Input() alertTypeList: any;
  @Input() vehicleGroupList: any;
  @Input() vehicleList: any;
  displayedColumnsVehicles: string[] = ['vehicleName', 'vehicleGroupName', 'subcriptionStatus']
  displayedColumnsPOI: string[] = ['select', 'icon', 'name', 'categoryName', 'subCategoryName', 'address'];
  displayedColumnsGeofence: string[] = ['select', 'name', 'categoryName', 'subCategoryName'];
  displayedColumnsGroup: string[] = ['select', 'name', 'poiCount', 'geofenceCount'];
  displayedColumnsCorridor: string[] = ['select', 'corridoreName', 'startPoint', 'endPoint', 'distance', 'width'];
  selectedPOI = new SelectionModel(true, []);
  selectedGeofence = new SelectionModel(true, []);
  selectedGroup = new SelectionModel(true, []);
  selectedCorridor = new SelectionModel(true, []);
  vehiclesDataSource: any = new MatTableDataSource([]);
  poiDataSource: any = new MatTableDataSource([]);
  geofenceDataSource: any = new MatTableDataSource([]);
  groupDataSource: any = new MatTableDataSource([]);
  corridorDataSource: any = new MatTableDataSource([]);
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  dialogRef: MatDialogRef<CommonTableComponent>;
  alertCreatedMsg: any = '';
  breadcumMsg: any = '';
  alertForm: FormGroup;
  accountOrganizationId: number;
  accountId: number;
  userType: string;
  selectedApplyOn: string;
  openAdvancedFilter: boolean= false;
  poiGridData = [];
  geofenceGridData = [];
  groupGridData = [];
  corridorGridData = [];
  isDuplicateAlert: boolean= false;
  private platform: any;
  map: any;
  geofenceData: any;
  marker: any;
  markerArray: any = [];
  geoMarkerArray: any = [];
  polyPoints: any = [];
  alertTypeByCategoryList: any= [];
  vehicleByVehGroupList: any= [];
  alert_category_selected: string= '';
  alert_type_selected: string= '';
  vehicle_group_selected: number= 26;
  alertTypeName: string= '';
  isCriticalLevelSelected: boolean= false;
  isWarningLevelSelected: boolean= false;
  isAdvisoryLevelSelected: boolean= false;
  isSundaySelected: boolean= false;
  isMondaySelected: boolean= false;
  isTuesdaySelected: boolean= false;
  isWednesdaySelected: boolean= false;
  isThursdaySelected: boolean= false;
  isFridaySelected: boolean= false;
  isSaturdaySelected: boolean= false;
  labelForThreshold: string= '';
  unitForThreshold: string= '';
  panelOpenState: boolean = false;
  notifications: any= [];
  typesOfLevel: any= [
                      {
                        levelType : 'C',
                        value: 'Critical'
                      },
                      {
                        levelType : 'W',
                        value: 'Warning'
                      }, 
                      {
                        levelType : 'A',
                        value: 'Advisory'
                      }
                    ];

  
  @ViewChild("map")
  private mapElement: ElementRef;
  
  constructor(private _formBuilder: FormBuilder,
              private poiService: POIService,
              private geofenceService: GeofenceService, 
              private landmarkGroupService: LandmarkGroupService, 
              private domSanitizer: DomSanitizer, 
              private dialog: MatDialog,
              private alertService: AlertService,
              private corridorService: CorridorService) 
  {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(): void {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.userType= localStorage.getItem("userType");
    this.alertForm = this._formBuilder.group({
      alertName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      alertCategory: ['', [Validators.required]],
      alertType: ['', [Validators.required]],
      applyOn: ['G', [Validators.required]],
      vehicleGroup: [''],
      vehicle: [''],
      statusMode: ['A', [Validators.required]],
      alertLevel: ['C', [Validators.required]],
      criticalLevel: [''],
      criticalLevelThreshold: [''],
      warningLevel: [''],
      warningLevelThreshold: [''],
      advisoryLevel: [''],
      advisoryLevelThreshold: [''],
      mondayPeriod: ['']
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('alertName')  
      ]
    });

    this.selectedApplyOn= 'G';
    if(this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    if(this.actionType == 'view' || this.actionType == 'edit'){
      this.breadcumMsg = this.getBreadcum();
    }

    // this.alertTypeByCategoryList= this.alertTypeList;
    this.vehicleGroupList = this.getUnique(this.vehicleList, "vehicleGroupId");
    this.vehicleGroupList= this.vehicleGroupList.filter(item=> item.vehicleGroupId != 0);
    this.vehicleByVehGroupList= this.getUnique(this.vehicleList, "vehicleId");

    if(this.vehicleList.length > 0){
      this.updateVehiclesDataSource(this.vehicleList.filter(item => item.subcriptionStatus == false));
    }
    

    if(this.alertCategoryList.length== 0 || this.alertTypeList.length == 0 || this.vehicleList.length == 0)
      this.loadFiltersData();
  }

  getUnique(arr, comp) {

    // store the comparison  values in array
    const unique =  arr.map(e => e[comp])

      // store the indexes of the unique objects
      .map((e, i, final) => final.indexOf(e) === i && i)

      // eliminate the false indexes & return unique objects
    .filter((e) => arr[e]).map(e => arr[e]);

    return unique;
  }

  loadFiltersData(){
    this.alertService.getAlertFilterData(this.accountId, this.accountOrganizationId).subscribe((data) => {
      let filterData = data["enumTranslation"];
      filterData.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });
      this.alertCategoryList= filterData.filter(item => item.type == 'C');
      this.alertTypeList= filterData.filter(item => item.type == 'T');
      this.vehicleList= data["vehicleGroup"];
      if(this.vehicleList.length > 0){
        this.updateVehiclesDataSource(this.vehicleList.filter(item => item.subcriptionStatus == false));
      }
      this.vehicleGroupList = this.getUnique(this.vehicleList, "vehicleGroupId");
      this.vehicleByVehGroupList= this.getUnique(this.vehicleList, "vehicleId");
    }, (error) => {

    })
  }

  updateVehiclesDataSource(tableData: any){
    this.vehiclesDataSource= new MatTableDataSource(tableData);
    this.vehiclesDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.vehicleName.toString().toLowerCase().includes(filter) ||
        data.vehicleGroupName.toString().toLowerCase().includes(filter) ||
        data.subcriptionStatus.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.vehiclesDataSource.paginator = this.paginator.toArray()[0];
      this.vehiclesDataSource.sort = this.sort.toArray()[0];
    });
  }

  onChangeAlertCategory(event){
    this.alert_category_selected= event.value;
    this.alertForm.get('alertType').setValue('');
    this.alertTypeByCategoryList= this.alertTypeList.filter(item => item.parentEnum == event.value);
  }

  onChangeAlertType(event){
    this.alert_type_selected= event.value;
    if(this.alert_category_selected === 'L' && (this.alert_type_selected === 'N' || this.alert_type_selected === 'X' || this.alert_type_selected === 'C')){
      this.loadMap();
      if(this.alert_type_selected === 'N' || this.alert_type_selected === 'X'){ //Entering zone & Exiting Zone
        this.loadPOIData();
        this.loadGeofenceData();
        this.loadGroupData();
      }
      else if(this.alert_type_selected === 'C'){ // Exiting Corridor
        this.loadCorridorData();
      }
    }
    else if(this.alert_category_selected == 'R'){ // Repair and maintenance
      this.alertTypeName = this.alertTypeList.filter(item => item.enum == this.alert_type_selected)[0].value;
      if(this.alert_type_selected === 'O'){ // Status Change to Stop Now
        this.alertForm.get('alertLevel').setValue('C');
      }
      else if(this.alert_type_selected === 'E'){ // Status Change to Service Now
        this.alertForm.get('alertLevel').setValue('W');
      }
    }
    else if((this.alert_category_selected == 'L' && (this.alert_type_selected == 'Y' || this.alert_type_selected == 'H' || this.alert_type_selected == 'D' || this.alert_type_selected == 'U' || this.alert_type_selected == 'G')) ||
            (this.alert_category_selected == 'F' && (this.alert_type_selected == 'P' || this.alert_type_selected == 'L' || this.alert_type_selected == 'T' || this.alert_type_selected == 'I' || this.alert_type_selected == 'A' || this.alert_type_selected == 'F'))){

      switch(this.alert_category_selected+this.alert_type_selected){
        case "LY": { //Excessive under utilization in days
          this.labelForThreshold= this.translationData.lblPeriod ? this.translationData.lblPeriod : "Period";
          this.unitForThreshold= this.translationData.lblDays ? this.translationData.lblDays : "Days";
          break;
        }
        case "LH": { //Excessive under utilization in hours
          this.labelForThreshold= this.translationData.lblPeriod ? this.translationData.lblPeriod : "Period";
          this.unitForThreshold= this.translationData.lblHours ? this.translationData.lblHours : "Hours";
          break;
        }
        case "LD": { //Excessive distance done
          this.labelForThreshold= this.translationData.lblDistance ? this.translationData.lblDistance : "Distance";
          this.unitForThreshold= "" //km/miles
          break;
        }
        case "LU": { //Excessive Driving duration
          this.labelForThreshold= this.translationData.lblDuration ? this.translationData.lblDuration : "Duration";
          this.unitForThreshold= ""
          break;
        }
        case "LG": { //Excessive Global Mileage
          this.labelForThreshold= this.translationData.lblMileage ? this.translationData.lblMileage : "Mileage";
          this.unitForThreshold= "" //km/miles
          break;
        }
        case "FP": { //Fuel Increase During stop
          this.labelForThreshold= this.translationData.lblPercentage ? this.translationData.lblPercentage : "Percentage";
          this.unitForThreshold= "%";
          break;
        }
        case "FL": { //Fuel loss during stop
          this.labelForThreshold= this.translationData.lblPercentage ? this.translationData.lblPercentage : "Percentage";
          this.unitForThreshold= "%"
          break;
        }
        case "FT": { //Fuel loss during trip
          this.labelForThreshold= this.translationData.lblPercentage ? this.translationData.lblPercentage : "Percentage";
          this.unitForThreshold= ""
          break;
        }
        case "FI": { //Excessive Average Idling
          this.labelForThreshold= this.translationData.lblDuration ? this.translationData.lblDuration : "Duration";
          this.unitForThreshold= this.translationData.lblSeconds ? this.translationData.lblSeconds : "Seconds";
          break;
        }
        case "FA": { //Excessive Average speed
          this.labelForThreshold= this.translationData.lblDSpeed ? this.translationData.lblSpeed : "Speed";
          this.unitForThreshold= this.translationData.lblkilometerperhour ? this.translationData.lblkilometerperhour : "km/h";
          break;
        }
        case "FF": { //Fuel Consumed
          this.labelForThreshold= this.translationData.lblFuelConsumed ? this.translationData.lblFuelConsumed : "Fuel Consumed";
          this.unitForThreshold= this.translationData.lblLiters ? this.translationData.lblLiters : "Liters";
          break;
        }
      }
    } 
  }

  onChangeVehicleGroup(event){
    
    if(event.value == 'ALL'){
      this.vehicleByVehGroupList = this.getUnique(this.vehicleList, "vehicleId");
    }
    else{
      this.vehicleByVehGroupList= this.vehicleList.filter(item => item.vehicleGroupId == event.value)
      this.vehicle_group_selected= event.value;
    }
    this.updateVehiclesDataSource(this.vehicleByVehGroupList);
  }

  onChangeVehicle(event){
    this.vehicle_group_selected= event.value;
  }

  loadMap() {
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
    
}

PoiCheckboxClicked(event: any, row: any) {
  console.log(row);
  if(event.checked){ //-- add new marker
    this.markerArray.push(row);
  }else{ //-- remove existing marker
    //It will filter out checked points only
    let arr = this.markerArray.filter(item => item.id != row.id);
    this.markerArray = arr;
  }
  this.addMarkerOnMap();
    
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
                  // console.error(error);
                });
              }
  
          }, false);
        }
    }

    corridorCheckboxClicked(event, row){
      if(event.checked){ //-- add new marker
        this.markerArray.push(row);
      }else{ //-- remove existing marker
        //It will filter out checked points only
        let arr = this.markerArray.filter(item => item.id != row.id);
        this.markerArray = arr;
        }
        this.addPolylineToMap();
    }
  
    addPolylineToMap(){
      console.log(this.markerArray)
      var lineString = new H.geo.LineString();
      this.markerArray.forEach(element => {
        console.log(element.startLat)
      lineString.pushPoint({lat : element.startLat, lng: element.startLong});
      lineString.pushPoint({lat : element.endLat, lng: element.endLong});
      // lineString.pushPoint({lat:48.8567, lng:2.3508});
      // lineString.pushPoint({lat:52.5166, lng:13.3833});
      });
  
      this.map.addObject(new H.map.Polyline(
        lineString, { style: { lineWidth: 4 }}
      ));
    }
  
    // toBack() {
    //   let emitObj = {
    //     stepFlag: false,
    //     msg: ""
    //   }
    //   this.backToPage.emit(emitObj);
    // }

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
  
  setDefaultValue(){
    // this.landmarkGroupForm.get('landmarkGroupName').setValue(this.selectedRowData.name);
    // if(this.selectedRowData.description)
    //   this.landmarkGroupForm.get('landmarkGroupDescription').setValue(this.selectedRowData.description);
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / 
    ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / 
    ${this.translationData.lblLandmarks ? this.translationData.lblLandmarks : "Landmarks"} / 
    ${(this.actionType == 'edit') ? (this.translationData.lblEditGroupDetails ? this.translationData.lblEditGroupDetails : 'Edit Group Details') : (this.translationData.lblViewGroupDetails ? this.translationData.lblViewGroupDetails : 'View Group Details')}`;
  }

  loadPOIData() {
    this.poiService.getPois(this.accountOrganizationId).subscribe((poilist: any) => {
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
        if(this.actionType == 'view' || this.actionType == 'edit')
        this.loadPOISelectedData(this.poiGridData);
      }
      
    });
  }

  loadPOISelectedData(tableData: any){
    let selectedPOIList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.id && item.type == "P");
        if (search.length > 0) {
          selectedPOIList.push(row);
        }
      });
      tableData = selectedPOIList;
      this.displayedColumnsPOI= ['icon', 'name', 'categoryName', 'subCategoryName', 'address'];
      this.updatePOIDataSource(tableData);
    }
    else if(this.actionType == 'edit' ){
      // this.selectPOITableRows(this.selectedRowData);
    }
  }

  selectPOITableRows(event:any,rowData: any){
    this.poiDataSource.data.forEach((row: any) => {
      let search = rowData.landmarks.filter(item => item.landmarkid == row.id && item.type == "P");
      if (search.length > 0) {
        if(event.checked)
          this.selectedPOI.select(row);
        else
          this.selectedPOI.deselect(row);  
        this.PoiCheckboxClicked(event,row);

      }
    });
  }

  loadGeofenceData() {
    // this.geofenceService.getAllGeofences(this.accountOrganizationId).subscribe((geofencelist: any) => {
    this.geofenceService.getGeofenceDetails(this.accountOrganizationId).subscribe((geofencelist: any) => {
      // this.geofenceGridData = geofencelist.geofenceList;
      this.geofenceGridData = geofencelist;
     this.geofenceGridData = this.geofenceGridData.filter(item => item.type == "C" || item.type == "O");
      this.updateGeofenceDataSource(this.geofenceGridData);
      if(this.actionType == 'view' || this.actionType == 'edit')
        this.loadGeofenceSelectedData(this.geofenceGridData);
    });
  }

  loadGeofenceSelectedData(tableData: any){
    let selectedGeofenceList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
        if (search.length > 0) {
          selectedGeofenceList.push(row);
        }
      });
      tableData = selectedGeofenceList;
      this.displayedColumnsGeofence= ['name', 'categoryName', 'subCategoryName'];
      this.updateGeofenceDataSource(tableData);
    }
    else if(this.actionType == 'edit' ){
      // this.selectGeofenceTableRows(this.selectedRowData);
    }
  }

  selectGeofenceTableRows(event: any,rowData: any){
    this.geofenceDataSource.data.forEach((row: any) => {
      let search = rowData.landmarks.filter(item => item.landmarkid == row.id && (item.type == "C" || item.type == "O"));
      if (search.length > 0) {
        if(event.checked)
          this.selectedGeofence.select(row);
        else
          this.selectedGeofence.deselect(row);
        this.geofenceCheckboxClicked(event,row);
      }
    });
  }

  loadGroupData(){
    let objData = { 
      organizationid : this.accountOrganizationId,
   };

    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((data: any) => {
      if(data){
        this.groupGridData = data["groups"];
        this.updateGroupDatasource(this.groupGridData);
      }
    }, (error) => {
      //console.log(error)
    });
  }

  loadGroupSelectedData(tableData: any){
    let selectedGroupList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
        if (search.length > 0) {
          selectedGroupList.push(row);
        }
      });
      tableData = selectedGroupList;
      this.displayedColumnsGeofence= ['name', 'poiCount', 'geofenceCount'];
      this.updateGroupDatasource(tableData);
    }
    else if(this.actionType == 'edit'){
      this.selectGroupTableRows();
    }
  }

  selectGroupTableRows(){
    this.groupDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.landmarks.filter(item => item.groupId == row.id);
      if (search.length > 0) {
        this.selectedGroup.select(row);
      }
    });
  }

  loadCorridorData(){
    this.corridorService.getCorridorList(this.accountOrganizationId).subscribe((data : any) => {
      this.corridorGridData = data;
      this.updateCorridorDatasource(this.corridorGridData);
    }, (error) => {
      
    });
  }

  loadCorridorSelectedData(tableData: any){
    let selectedGroupList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.landmarks.filter(item => item.landmarkid == row.geofenceId && (item.type == "C" || item.type == "O"));
        if (search.length > 0) {
          selectedGroupList.push(row);
        }
      });
      tableData = selectedGroupList;
      this.displayedColumnsCorridor= ['name', 'poiCount', 'geofenceCount'];
      this.updateCorridorDatasource(tableData);
    }
    else if(this.actionType == 'edit'){
      this.selectCorridorTableRows();
    }
  }

  selectCorridorTableRows(){
    this.corridorDataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.landmarks.filter(item => item.groupId == row.id);
      if (search.length > 0) {
        this.selectedCorridor.select(row);
      }
    });
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
      this.geofenceDataSource.paginator = this.paginator.toArray()[2];
      this.geofenceDataSource.sort = this.sort.toArray()[2];
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
      this.groupDataSource.paginator = this.paginator.toArray()[3];
      this.groupDataSource.sort = this.sort.toArray()[3];
    });
  }

  updateCorridorDatasource(tableData: any){
    this.corridorDataSource = new MatTableDataSource(tableData);
    this.corridorDataSource.filterPredicate = function(data: any, filter: string): boolean {
      return (
        data.corridoreName.toString().toLowerCase().includes(filter) ||
        data.startPoint.toString().toLowerCase().includes(filter) ||
        data.endPoint.toString().toLowerCase().includes(filter) ||
        data.distance.toString().toLowerCase().includes(filter) ||
        data.width.toString().toLowerCase().includes(filter)
      );
    };
    setTimeout(()=>{
      this.corridorDataSource.paginator = this.paginator.toArray()[1];
      this.corridorDataSource.sort = this.sort.toArray()[1];
    });
  }


  onPOIClick(row: any){
    const colsList = ['icon', 'landmarkname', 'categoryname', 'subcategoryname', 'address'];
    const colsName = [this.translationData.lblIcon || 'Icon', this.translationData.lblName || 'Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category', this.translationData.lblAddress || 'Address'];
    const tableTitle = this.translationData.lblPOI || 'POI';
    let objData = { 
      organizationid : this.accountOrganizationId,
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

  onGeofenceClick(row: any){
    const colsList = ['landmarkname', 'categoryname', 'subcategoryname'];
    const colsName = ['Name', this.translationData.lblCategory || 'Category', this.translationData.lblSubCategory || 'Sub-Category'];
    const tableTitle = this.translationData.lblGeofence || 'Geofence';
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
   };
      this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupDetails) => {
      this.selectedRowData = groupDetails["groups"][0].landmarks.filter(item => (item.type == "C" || item.type == "O"));
      this.callToCommonTable(this.selectedRowData, colsList, colsName, tableTitle);
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

  onChangeCriticalLevel(event){
    if(event.checked){
      this.isCriticalLevelSelected= true;
    }
    else{
      this.isCriticalLevelSelected= false;
      this.alertForm.get('criticalLevelThreshold').setValue('');
    }
  }
  
  onChangeWarningLevel(event){
    if(event.checked){
      this.isWarningLevelSelected= true;
    }
    else{
      this.isWarningLevelSelected= false;
      this.alertForm.get('warningLevelThreshold').setValue('');
    }
  }

  onChangeAdvisoryLevel(event){
    if(event.checked){
      this.isAdvisoryLevelSelected= true;
    }
    else{
      this.isAdvisoryLevelSelected= false;
      this.alertForm.get('advisoryLevelThreshold').setValue('');
    }
  }

  onChangeSundaySelection(event){
    if(event.checked){
      this.isSundaySelected= true;
    }
    else{
      this.isSundaySelected= false;
    }
  }

  onChangeMondaySelection(event){
    if(event.checked){
      this.isMondaySelected= true;
    }
    else{
      this.isMondaySelected= false;
    }
  }

  onChangeTuesdaySelection(event){
    if(event.checked){
      this.isTuesdaySelected= true;
    }
    else{
      this.isTuesdaySelected= false;
    }
  }

  onChangeWednesdaySelection(event){
    if(event.checked){
      this.isWednesdaySelected= true;
    }
    else{
      this.isWednesdaySelected= false;
    }
  }

  onChangeThursdaySelection(event){
    if(event.checked){
      this.isThursdaySelected= true;
    }
    else{
      this.isThursdaySelected= false;
    }
  }

  onChangeFridaySelection(event){
    if(event.checked){
      this.isFridaySelected= true;
    }
    else{
      this.isFridaySelected= false;
    }
  }

  onChangeSaturdaySelection(event){
    if(event.checked){
      this.isSaturdaySelected= true;
    }
    else{
      this.isSaturdaySelected= false;
    }
  }

  onReset(){ //-- Reset
    this.selectedPOI.clear();
    this.selectedGeofence.clear();
    // this.selectPOITableRows(this.selectedRowData);
    // this.selectGeofenceTableRows(this.selectedRowData);
    this.setDefaultValue();
  }

  onCancel(){
    let emitObj = {
      actionFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  onApplyOnChange(event){
    this.selectedApplyOn = event.value;
  }

  onClickAdvancedFilter(){
    this.openAdvancedFilter = !this.openAdvancedFilter;
  }

  onCreateUpdate(){
    this.isDuplicateAlert= false;
    let alertLandmarkRefs= [];
    let alertFilterRefs: any= [];

    // Entering Zone, Exiting Zone
    if(this.alert_category_selected == 'L' && (this.alert_type_selected == 'N' || this.alert_type_selected == 'X')){
      
      if(this.selectedPOI.selected.length > 0){
        this.selectedPOI.selected.forEach(element => {
          let tempObj= {
            "landmarkType": "P",
            "refId": element.id,
            "distance": 100,
            "unitType": ""
          }
          alertLandmarkRefs.push(tempObj);
        });
      }
      if(this.selectedGeofence.selected.length > 0){
        this.selectedGeofence.selected.forEach(element => {
          let tempObj= {
            "landmarkType": element.type,
            "refId": element.id,
            "distance": element.distance,
            "unitType": ""
          }
          alertLandmarkRefs.push(tempObj);
        });
      }
    }
    else if(this.alert_category_selected == 'L' && this.alert_type_selected === 'C'){ // Exiting Corridor
      if(this.selectedCorridor.selected.length > 0){
        this.selectedCorridor.selected.forEach(element => {
          let tempObj= {
            "landmarkType": "P",
            "refId": element.id,
            "distance": 100,
            "unitType": ""
          }
          alertLandmarkRefs.push(tempObj);
        });
      }
    }

      let alertObjData= {
        "organizationId": this.accountOrganizationId,
        "name": this.alertForm.get('alertName').value,
        "category": this.alert_category_selected,
        "type": this.alert_type_selected,
        "validityPeriodType": "A",
        "validityStartDate": 0,
        "validityEndDate": 0,
        "vehicleGroupId": this.vehicle_group_selected,
        "state": "A",
        "applyOn": this.alertForm.get('applyOn').value,
        "createdBy": this.accountId,
        "notifications": this.notifications,
        "alertUrgencyLevelRefs": [{
          "urgencyLevelType": this.alertForm.get('alertLevel').value,
          "thresholdValue": 0,
          "unitType": "N",
          "dayType": [
            false, false, false, false, false, false, false
          ],
          "periodType": "A",
          "urgencylevelStartDate": 0,
          "urgencylevelEndDate": 0,
          "alertFilterRefs": alertFilterRefs
        }],
        "alertLandmarkRefs": alertLandmarkRefs
      }

      this.alertService.createAlert(alertObjData).subscribe((data) => {
        if(data){
          this.alertCreatedMsg = this.getAlertCreatedMessage();
          let emitObj = { actionFlag: false, successMsg: this.alertCreatedMsg };
           this.backToPage.emit(emitObj);
        }  
      }, (error) => {
        if(error.status == 409)
          this.isDuplicateAlert= true;
      })
  }

  getAlertCreatedMessage() {
    let alertName = `${this.alertForm.controls.alertName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblAlertCreatedSuccessfully)
        return this.translationData.lblAlertCreatedSuccessfully.replace('$', alertName);
      else
        return ("Alert '$' Created Successfully").replace('$', alertName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblAlertUpdatedSuccessfully)
        return this.translationData.lblAlertUpdatedSuccessfully.replace('$', alertName);
      else
        return ("Alert '$' Updated Successfully").replace('$', alertName);
    }
    else{
      return '';
    }
  }

  applyFilterForVehicles(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.vehiclesDataSource.filter = filterValue;
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

  applyFilterForCorridor(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.corridorDataSource.filter = filterValue;
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

  masterToggleForGroup() {
    this.isAllSelectedForGroup()
      ? this.selectedGroup.clear()
      : this.groupDataSource.data.forEach((row) =>
        this.selectedGroup.select(row)
      );
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

  masterToggleForCorridor() {
    this.isAllSelectedForCorridor()
      ? this.selectedCorridor.clear()
      : this.corridorDataSource.data.forEach((row) =>
        this.selectedCorridor.select(row)
      );
  }

  isAllSelectedForCorridor() {
    const numSelected = this.selectedCorridor.selected.length;
    const numRows = this.corridorDataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForCorridor(row?: any): string {
    if (row)
      return `${this.isAllSelectedForCorridor() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedCorridor.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onGroupSelect(event: any, row: any){
    let groupDetails= [];
    let objData = { 
      organizationid : this.accountOrganizationId,
      groupid : row.id
    };
    this.landmarkGroupService.getLandmarkGroups(objData).subscribe((groupData) => {
      groupDetails = groupData["groups"][0];
      this.selectPOITableRows(event,groupDetails);
      this.selectGeofenceTableRows(event,groupDetails);
    });
  }
}
