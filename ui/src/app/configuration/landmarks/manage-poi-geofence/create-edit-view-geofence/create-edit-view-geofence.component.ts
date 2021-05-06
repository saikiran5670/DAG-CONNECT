import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../shared/custom.validators';
import { HereService } from 'src/app/services/here.service';
import { FormBuilder } from '@angular/forms';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { GeofenceService } from '../../../../services/landmarkGeofence.service';
declare var H: any;

@Component({
  selector: 'app-create-edit-view-geofence',
  templateUrl: './create-edit-view-geofence.component.html',
  styleUrls: ['./create-edit-view-geofence.component.less']
})
export class CreateEditViewGeofenceComponent implements OnInit {
  displayedColumns: string[] = ['select', 'icon', 'name', 'categoryName', 'subCategoryName', 'address'];
  selectedPOI = new SelectionModel(true, []);
  dataSourceForPOI: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() categoryList: any;
  @Input() subCategoryList: any;
  @Output() createViewEditPoiEmit = new EventEmitter<object>();
  @Input() translationData: any;
  @Input() selectedElementData: any;
  @Output() backToPage = new EventEmitter<any>();
  breadcumMsg: any = '';
  @Input() actionType: any;
  @Input() poiData: any;
  polygonGeofenceFormGroup: FormGroup;
  circularGeofenceFormGroup: FormGroup;
  private platform: any;
  map: any;
  private ui: any;
  lat: any = '37.7397';
  lng: any = '-121.4252';
  query: any;
  public geocoder: any;
  public position: string;
  public locations: Array<any>;
  poiFlag: boolean = true;
  data: any;
  address: 'chaitali';
  zip: any;
  city: any;
  country: any;
  userCreatedMsg: any = '';
  hereMapService: any;
  organizationId: number;
  localStLanguage: any;
  polygoanGeofence: boolean = false;
  circularGeofence: boolean = false;
  types = ['Regular', 'Global'];
  duplicateCircularGeofence: boolean = false;
  duplicatePolygonGeofence: boolean = false;
  geoSelectionFlag: boolean = false;
  hereMap: any;
  markerArray: any = [];

  @ViewChild("map")
  public mapElement: ElementRef;

  constructor(private here: HereService, private _formBuilder: FormBuilder, private geofenceService: GeofenceService) {
    this.query = "starbucks";
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
  }

  ngOnInit(): void {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.circularGeofenceFormGroup = this._formBuilder.group({
      circularName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      type: ['', []],
      radius: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
    },
      {
        validator: [
          CustomValidators.specialCharValidationForName('circularName'),
          CustomValidators.specialCharValidationForName('radius'),
        ]
      });

    this.polygonGeofenceFormGroup = this._formBuilder.group({
      name: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      type: ['', []],
      category: ['', [Validators.required]],
      subCategory: ['', []],
      address: new FormControl({ value: null, disabled: true }),
      zip: new FormControl({ value: null, disabled: true }),
      city: new FormControl({ value: null, disabled: true }),
      country: new FormControl({ value: null, disabled: true })
    },
      {
        validator: [
          CustomValidators.specialCharValidationForName('name'),
        ]
      });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if (this.actionType == 'create') {
      this.geoSelectionFlag = true;
    }
    if (this.actionType == 'view' || this.actionType == 'edit') {
      if (this.selectedElementData && this.selectedElementData.type == 'C') { //-- circular geofence
        this.circularGeofence = true;
        this.setDefaultCircularGeofenceFormValue();
        this.loadGridData(this.poiData);
      } else { //-- polygon geofence
        this.polygoanGeofence = true;
        this.setDefaultPolygonGeofenceFormValue();
      }
    }
  }

  loadGridData(tableData: any) {
    let selectedGeofenceList: any = [];
    this.selectedElementData.latitude = 48.8569817;
    this.selectedElementData.longitude = 2.4509036;
    if (this.actionType == 'view') {
      tableData.forEach((row: any) => {
        let search = [this.selectedElementData].filter((item: any) => (item.latitude == row.latitude) && (item.longitude == row.longitude));
        if (search.length > 0) {
          selectedGeofenceList.push(row);
        }
      });
      tableData = selectedGeofenceList;
      this.displayedColumns = ['icon', 'name', 'categoryName', 'subCategoryName', 'address'];
    }
    this.updatePOIDatasource(tableData);
    if (this.actionType == 'edit') {
      this.selectTableRows();
    }
  }

  selectTableRows() {
    this.dataSourceForPOI.data.forEach((row: any) => {
      let search = [this.selectedElementData].filter((item: any) => (item.latitude == row.latitude) && (item.longitude == row.longitude));
      if (search.length > 0) {
        this.selectedPOI.select(row);
      }
    });
  }

  setDefaultCircularGeofenceFormValue() {
    this.circularGeofenceFormGroup.get('circularName').setValue(this.selectedElementData.name);
    this.circularGeofenceFormGroup.get('type').setValue((this.selectedElementData.organizationId == 0) ? this.types[1] : this.types[0]);
    this.circularGeofenceFormGroup.get('radius').setValue(this.selectedElementData.distance);
  }

  setDefaultPolygonGeofenceFormValue() {
    this.polygonGeofenceFormGroup.get('name').setValue(this.selectedElementData.name);
    this.polygonGeofenceFormGroup.get('type').setValue((this.selectedElementData.organizationId == 0) ? this.types[1] : this.types[0]);
    this.polygonGeofenceFormGroup.get('category').setValue(this.selectedElementData.categoryId);
    this.polygonGeofenceFormGroup.get('subCategory').setValue(this.selectedElementData.subCategoryId);
    this.polygonGeofenceFormGroup.get('address').setValue(this.selectedElementData.address);
    this.polygonGeofenceFormGroup.get('zip').setValue(this.selectedElementData.zipcode);
    this.polygonGeofenceFormGroup.get('city').setValue(this.selectedElementData.city);
    this.polygonGeofenceFormGroup.get('country').setValue(this.selectedElementData.country);
  }

  updatePOIDatasource(tableData: any) {
    this.dataSourceForPOI = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSourceForPOI.paginator = this.paginator;
      this.dataSourceForPOI.sort = this.sort;
    });
  }

  setCircularType() {
    this.circularGeofenceFormGroup.get('type').setValue(this.types[0]);
  }

  setPolygonType() {
    this.polygonGeofenceFormGroup.get('type').setValue(this.types[0]);
  }

  getBreadcum(type: any) {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / ${(type == 'view') ? (this.translationData.lblViewGeofenceDetails ? this.translationData.lblViewGeofenceDetails : 'View Geofence Details') : (type == 'edit') ? (this.translationData.lblEditGeofenceDetails ? this.translationData.lblEditGeofenceDetails : 'Edit Geofence Details') : (this.translationData.lblAddNewGeofence ? this.translationData.lblAddNewGeofence : 'Add New Geofence')}`;
  }

  setUpClickListener(map, here, poiFlag) {
    // obtain the coordinates and display
    map.addEventListener('tap', function (evt) {
      if (poiFlag) {
        var coord = map.screenToGeo(evt.currentPointer.viewportX,
          evt.currentPointer.viewportY);
        let x = Math.abs(coord.lat.toFixed(4));
        let y = Math.abs(coord.lng.toFixed(4));
        console.log("latitude=" + x);
        console.log("longi=" + y);

        let locations = new H.map.Marker({ lat: x, lng: y });

        map.addObject(locations);

        this.position = x + "," + y;
        console.log(this.position);
        if (this.position) {
          here.getAddressFromLatLng(this.position).then(result => {
            this.locations = <Array<any>>result;
            console.log(this.locations[0].Location.Address);
            poiFlag = false;
          }, error => {
            console.error(error);
          });
        }
      }



    });


  }

  setAddressValues(addressVal, positions) {
    //     console.log("this is in setAddress()");
    // console.log(addressVal);
    this.address = addressVal.Label;
    this.zip = addressVal.PostalCode;
    this.city = addressVal.City;
    this.country = addressVal.Country;
    var nameArr = positions.split(',');
    // console.log(nameArr[0]);
    this.polygonGeofenceFormGroup.get("address").setValue(this.address);
    this.polygonGeofenceFormGroup.get("zip").setValue(this.zip);
    this.polygonGeofenceFormGroup.get("city").setValue(this.city);
    this.polygonGeofenceFormGroup.get("country").setValue(this.country);
    this.polygonGeofenceFormGroup.get("lattitude").setValue(nameArr[0]);
    this.polygonGeofenceFormGroup.get("longitude").setValue(nameArr[1]);
  }

  onCancel() {
    let emitObj = {
      stepFlag: false,
      successMsg: this.userCreatedMsg,
    }
    this.backToPage.emit(emitObj);
  }

  onCreateUpdateCircularGeofence() {
    if (this.actionType == 'create') { //-- create
      let cirGeoCreateObjData: any = [
        {
          id: 0,
          organizationId: 0,
          categoryId: 0,
          subCategoryId: 0,
          name: "string",
          type: "string",
          address: "string",
          city: "string",
          country: "string",
          zipcode: "string",
          latitude: 0,
          longitude: 0,
          distance: 0,
          tripId: 0,
          createdBy: 0
        }
      ];

      this.geofenceService.createCircularGeofence(cirGeoCreateObjData).subscribe((cirGeoCreateData: any) => {

      }, (error) => {
        if (error.status == 409) {
          this.duplicateCircularGeofence = true;
        }
      });
    }
    else { //-- update
      let cirGeoUpdateObjData: any = {
        id: 0,
        categoryId: 0,
        subCategoryId: 0,
        name: "string",
        modifiedBy: 0,
        organizationId: 0
      };
      this.geofenceService.updateCircularGeofence(cirGeoUpdateObjData).subscribe((cirGeoUpdateData: any) => {

      }, (error) => {
        if (error.status == 409) {
          this.duplicateCircularGeofence = true;
        }
      });
    }
  }

  onCreateUpdatePolygonGeofence() {
    if (this.actionType == 'create') { //-- create
      let polyCreateObjData: any = {
        id: 0,
        organizationId: 0,
        categoryId: 0,
        subCategoryId: 0,
        name: "string",
        type: "string",
        address: "string",
        city: "string",
        country: "string",
        zipcode: "string",
        latitude: 0,
        longitude: 0,
        distance: 0,
        tripId: 0,
        createdBy: 0,
        nodes: [
          {
            id: 0,
            landmarkId: 0,
            seqNo: 0,
            latitude: 0,
            longitude: 0,
            createdBy: 0
          }
        ]
      };

      this.geofenceService.createPolygonGeofence(polyCreateObjData).subscribe((createPolyData: any) => {

      }, (error) => {
        if (error.status == 409) {
          this.duplicatePolygonGeofence = true;
        }
      });
    } else { //-- update
      let polyUpdateObjData: any = {
        id: 0,
        categoryId: 0,
        subCategoryId: 0,
        name: "string",
        modifiedBy: 0,
        organizationId: 0
      };

      this.geofenceService.updatePolygonGeofence(polyUpdateObjData).subscribe((updatePolyData: any) => {

      }, (error) => {
        if (error.status == 409) {
          this.duplicatePolygonGeofence = true;
        }
      });
    }

  }

  onCircularReset() {
    this.selectedPOI.clear();
    this.setDefaultCircularGeofenceFormValue();
    this.selectTableRows();
  }

  onPolygonReset() {
    this.setDefaultPolygonGeofenceFormValue();
  }

  applyPOIFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSourceForPOI defaults to lowercase matches
    this.dataSourceForPOI.filter = filterValue;
  }

  masterToggleForPOI() {
    // this.isAllSelectedForPOI()
    //   ? this.selectedPOI.clear()
    //   : this.dataSourceForPOI.data.forEach((row) =>
    //     this.selectedPOI.select(row)
    //   );
    this.markerArray = [];
    if(this.isAllSelectedForPOI()){
      this.selectedPOI.clear();
    }
    else{
      this.dataSourceForPOI.data.forEach((row: any) =>{
        this.selectedPOI.select(row);
        this.markerArray.push(row);
      });
    }
    this.addMarkerOnMap();
  }

  isAllSelectedForPOI() {
    const numSelected = this.selectedPOI.selected.length;
    const numRows = this.dataSourceForPOI.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForPOI(row?: any): string {
    if (row)
      return `${this.isAllSelectedForPOI() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedPOI.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  onCategoryChange(event: any) {

  }

  onSubCategoryChange(event: any) {

  }

  geoSelection(type: any) {
    this.geoSelectionFlag = false;
    if (type == 'circular') {
      this.circularGeofence = true;
      this.setCircularType();
      this.updatePOIDatasource(this.poiData);
    } else { //-- polygon
      this.polygoanGeofence = true;
      this.setPolygonType();
    }
  }

  toBack() {
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.backToPage.emit(emitObj);
  }

  public ngAfterViewInit() {
    this.initMap();
  }
  
  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    //Step 2: initialize a map - this map is centered over Europe
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 50, lng: 5 },
      //center:{lat:41.881944, lng:-87.627778},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });

    // var svgMarkup = '<svg  width="210" height="24" xmlns="http://www.w3.org/2000/svg">' +
    //   '<rect stroke="black" fill="${FILL}" x="1" y="1" width="220" height="220" />' +
    //   '<text x="12" y="18" font-size="12pt" font-family="Arial" font-weight="bold" ' +
    //   'text-anchor="start" fill="${STROKE}" >Create Polygon Geofence</text></svg>';
    // // Add the first marker
    // var bearsIcon = new H.map.Icon(
    //   svgMarkup.replace('${FILL}', '#E5CDC7').replace('${STROKE}', '#393C49')),
    //   bearsMarker = new H.map.Marker({ lat: 60, lng: 5 },
    //     { icon: bearsIcon });

    // map.addObject(bearsMarker);


    // add a resize listener to make sure that the map occupies the whole container
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());

    // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));

    //===added code here=======
    /*
    //Step 4, initilize drag for map objects.
    map.addEventListener('dragstart', (ev) => {
      const target = ev.target;
      if (target instanceof H.map.Circle) {
          behavior.disable();
      }
    }, false);
    map.addEventListener('drag', (ev) => {
      const target = ev.target,
          pointer = ev.currentPointer;
      if (target instanceof H.map.Circle) {
          target.setCenter(map.screenToGeo(pointer.viewportX, pointer.viewportY));
      }
    }, false);
    
    map.addEventListener('dragend', (ev) => {
      const target = ev.target;
      if (target instanceof H.map.Circle) {
          behavior.enable();
      }
    }, false);  */
    //=========end========

    // Create the default UI components
    var ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
  }

  createResizableCircle(_radius: any, rowData: any) {
    var circle = new H.map.Circle(
      // The central point of the circle
      { lat: rowData.latitude, lng: rowData.longitude },
      // The radius of the circle in meters
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

    // ensure that the objects can receive drag events
    circle.draggable = true;
    circleOutline.draggable = true;

    // extract first point of the circle outline polyline's LineString and
    // push it to the end, so the outline has a closed geometry
    circleOutline.getGeometry().pushPoint(circleOutline.getGeometry().extractPoint(0));

    // add group with circle and it's outline (polyline)
    //this.hereMap.removeObjects(this.hereMap.getObjects());
    this.hereMap.addObject(circleGroup);

    // event listener for circle group to show outline (polyline) if moved in with mouse (or touched on touch devices)
    // circleGroup.addEventListener('pointerenter', function (evt) {
    //   var currentStyle = circleOutline.getStyle(),
    //     newStyle = currentStyle.getCopy({
    //       strokeColor: 'rgb(255, 0, 0)'
    //     });

    //   if (circleTimeout) {
    //     clearTimeout(circleTimeout);
    //     circleTimeout = null;
    //   }
    //   // show outline
    //   circleOutline.setStyle(newStyle);
    // }, true);

    // event listener for circle group to hide outline if moved out with mouse (or released finger on touch devices)
    // the outline is hidden on touch devices after specific timeout
    // circleGroup.addEventListener('pointerleave', function (evt) {
    //   var currentStyle = circleOutline.getStyle(),
    //     newStyle = currentStyle.getCopy({
    //       strokeColor: 'rgba(255, 0, 0, 0)'
    //     }),
    //     timeout = (evt.currentPointer.type == 'touch') ? 1000 : 0;

    //   circleTimeout = setTimeout(function () {
    //     circleOutline.setStyle(newStyle);
    //   }, timeout);
    //   document.body.style.cursor = 'default';
    // }, true);

    // event listener for circle group to change the cursor if mouse position is over the outline polyline (resizing is allowed)
    // circleGroup.addEventListener('pointermove', function (evt) {
    //   if (evt.target instanceof H.map.Polyline) {
    //     document.body.style.cursor = 'pointer';
    //   } else {
    //     document.body.style.cursor = 'default'
    //   }
    // }, true);
    //   map.addEventListener('dragstart', (ev) => {
    //     const target = ev.target;
    //     if (target instanceof H.map.Circle) {
    //         behavior.disable();
    //     }
    // }, false);
    // event listener for circle group to resize the geo circle object if dragging over outline polyline
    // circleGroup.addEventListener('drag', function (evt) {
    //   var pointer = evt.currentPointer,
    //     distanceFromCenterInMeters = circle.getCenter().distance(this.hereMap.screenToGeo(pointer.viewportX, pointer.viewportY));

    //   // if resizing is alloved, set the circle's radius
    //   if (evt.target instanceof H.map.Polyline) {
    //     circle.setRadius(distanceFromCenterInMeters);

    //     // use circle's updated geometry for outline polyline
    //     var outlineLinestring = circle.getGeometry().getExterior();

    //     // extract first point of the outline LineString and push it to the end, so the outline has a closed geometry
    //     outlineLinestring.pushPoint(outlineLinestring.extractPoint(0));
    //     circleOutline.setGeometry(outlineLinestring);

    //     // prevent event from bubling, so map doesn't receive this event and doesn't pan
    //     evt.stopPropagation();
    //   }
    // }, true);
  }

  onChangeRadius(event: any){
    if(this.markerArray.length > 0){
      this.addMarkerOnMap();
    }
  }

  onChangeCheckbox(event: any, row: any){
    if(event){
      this.selectedPOI.toggle(row);
    }
    if(event.checked){ //-- add new marker
      this.markerArray.push(row);
    }else{ //-- remove existing marker
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
    }
    this.addMarkerOnMap();
  }

  addMarkerOnMap(){
    this.hereMap.removeObjects(this.hereMap.getObjects());
    this.markerArray.forEach(element => {
      var marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude });
      this.hereMap.addObject(marker);
      this.createResizableCircle(this.circularGeofenceFormGroup.controls.radius.value ? parseInt(this.circularGeofenceFormGroup.controls.radius.value) : 0, element);
    });
  }

}
