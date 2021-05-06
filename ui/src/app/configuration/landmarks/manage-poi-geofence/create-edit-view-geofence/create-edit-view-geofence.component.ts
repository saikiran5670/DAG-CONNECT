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
  accountId: any = 0;
  polygonPointArray: any = [];

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
    this.accountId = parseInt(localStorage.getItem("accountId"));
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
    //this.initMap();
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if (this.actionType == 'create') {
      this.geoSelectionFlag = true;
    }
    // if (this.actionType == 'view' || this.actionType == 'edit') {
    //   if (this.selectedElementData && this.selectedElementData.type == 'C') { //-- circular geofence
    //     this.circularGeofence = true;
    //     this.setDefaultCircularGeofenceFormValue();
    //     this.loadGridData(this.poiData);
    //     this.drawCircularGeofence();
    //   } else { //-- polygon geofence
    //     this.polygoanGeofence = true;
    //     this.setDefaultPolygonGeofenceFormValue();
    //   }
    // }
  }

  drawCircularGeofence(){
    this.markerArray = [];
    this.markerArray.push(this.selectedElementData);
    this.addMarkerOnMap();
  }

  loadGridData(tableData: any) {
    let selectedGeofenceList: any = [];
    // this.selectedElementData.latitude = 42.02308;
    // this.selectedElementData.longitude = 12.46952;
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
      let cirGeoCreateObjData: any = [];
      this.selectedPOI.selected.forEach(item => {
        let obj: any = {
          id: 0,
          organizationId: this.circularGeofenceFormGroup.controls.type.value == 'Regular' ? this.organizationId : 0,
          categoryId: item.categoryId,
          subCategoryId: item.subCategoryId,
          name: this.circularGeofenceFormGroup.controls.circularName.value.trim(),
          type: "C", //- for circular geofence
          address: item.address,
          city: item.city,
          country: item.country,
          zipcode: item.zipcode,
          latitude: item.latitude,
          longitude: item.longitude,
          distance: parseInt(this.circularGeofenceFormGroup.controls.radius.value),
          tripId: 0,
          createdBy: this.accountId //-- login account-id
        };
        cirGeoCreateObjData.push(obj);
      });

      this.geofenceService.createCircularGeofence(cirGeoCreateObjData).subscribe((cirGeoCreateData: any) => {
        this.getAllGeofenceData();
      }, (error) => {
        if (error.status == 409) {
          this.duplicateCircularGeofence = true;
        }
      });
    }
    else { //-- update
      let cirGeoUpdateObjData: any; 
      this.selectedPOI.selected.forEach(item => {
        cirGeoUpdateObjData = {
          id: this.selectedElementData.id, //-- circular geoId
          categoryId: item.categoryId,
          subCategoryId: item.subCategoryId,
          name: this.circularGeofenceFormGroup.controls.circularName.value.trim(),
          modifiedBy: this.accountId,
          organizationId: this.circularGeofenceFormGroup.controls.type.value == 'Regular' ? this.organizationId : 0
        };
      });
      this.geofenceService.updateCircularGeofence(cirGeoUpdateObjData).subscribe((cirGeoUpdateData: any) => {
        this.getAllGeofenceData();
      }, (error) => {
        if (error.status == 409) {
          this.duplicateCircularGeofence = true;
        }
      });
    }
  }

  getAllGeofenceData(){
    this.geofenceService.getAllGeofences(this.organizationId).subscribe((geoData: any) => {
      let geoInitData = geoData["geofenceList"];
      geoInitData = geoInitData.filter(item => item.type == "C" || item.type == "O");
      let geofenceCreatedUpdateMsg = this.circularGeofence ? this.getCircularGeofenceCreatedUpdatedMessage() : this.getPolygonGeofenceCreatedUpdatedMessage();
      let emitObj = { stepFlag: false, gridData: geoInitData, successMsg: geofenceCreatedUpdateMsg };
      this.backToPage.emit(emitObj);
    }, (error)=>{
      let emitObj = { stepFlag: false, successMsg: '' };
      this.backToPage.emit(emitObj);
    });
  }

  getCircularGeofenceCreatedUpdatedMessage() {
    let geoName = `${this.circularGeofenceFormGroup.controls.circularName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblCircularGeofenceCreatedSuccessfully)
        return this.translationData.lblCircularGeofenceCreatedSuccessfully.replace('$', geoName);
      else
        return ("Circular Geofence '$' Created Successfully").replace('$', geoName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblCircularGeofenceUpdatedSuccessfully)
        return this.translationData.lblCircularGeofenceUpdatedSuccessfully.replace('$', geoName);
      else
        return ("Circular Geofence '$' Updated Successfully").replace('$', geoName);
    }
    else{
      return '';
    }
  }

  getPolygonGeofenceCreatedUpdatedMessage() {
    let geoName = `${this.polygonGeofenceFormGroup.controls.name.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblPolygonGeofenceCreatedSuccessfully)
        return this.translationData.lblPolygonGeofenceCreatedSuccessfully.replace('$', geoName);
      else
        return ("Polygon Geofence '$' Created Successfully").replace('$', geoName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblPolygonGeofenceUpdatedSuccessfully)
        return this.translationData.lblPolygonGeofenceUpdatedSuccessfully.replace('$', geoName);
      else
        return ("Polygon Geofence '$' Updated Successfully").replace('$', geoName);
    }
    else{
      return '';
    }
  }

  onCreateUpdatePolygonGeofence() {
    if (this.actionType == 'create') { //-- create
      let nodePoints: any = [];
      this.polygonPointArray.forEach(element => {
        nodePoints.push({
          id: 0,
          landmarkId: 0,
          seqNo: element.seqNo, //-- node seq. number
          latitude: element.latitude, //-- node lat
          longitude: element.longitude, //-- node lng
          createdBy: this.accountId //-- login account-id
        });
      });
      let polyCreateObjData: any = {
        id: 0,
        organizationId: this.polygonGeofenceFormGroup.controls.type.value == 'Regular' ? this.organizationId : 0,
        categoryId: parseInt(this.polygonGeofenceFormGroup.controls.category.value),
        subCategoryId: parseInt(this.polygonGeofenceFormGroup.controls.subCategory.value),
        name: this.polygonGeofenceFormGroup.controls.name.value.trim(),
        type: "O", //-- polygon geofence
        address: this.polygonGeofenceFormGroup.controls.address.value.trim(),
        city: this.polygonGeofenceFormGroup.controls.city.value.trim(),
        country: this.polygonGeofenceFormGroup.controls.country.value.trim(),
        zipcode: this.polygonGeofenceFormGroup.controls.zip.value.trim(),
        latitude: 0, //-- first node lat
        longitude: 0, //-- first node lng
        distance: 0,
        tripId: 0,
        createdBy: this.accountId,
        nodes: nodePoints
      };

      this.geofenceService.createPolygonGeofence(polyCreateObjData).subscribe((createPolyData: any) => {
        this.getAllGeofenceData();
      }, (error) => {
        if (error.status == 409) {
          this.duplicatePolygonGeofence = true;
        }
      });
    } else { //-- update
      let polyUpdateObjData: any = {
        id: this.selectedElementData.id, //-- polygon geoId
        categoryId: parseInt(this.polygonGeofenceFormGroup.controls.category.value),
        subCategoryId: parseInt(this.polygonGeofenceFormGroup.controls.subCategory.value),
        name: this.polygonGeofenceFormGroup.controls.name.value.trim(),
        modifiedBy: this.accountId,
        organizationId: this.polygonGeofenceFormGroup.controls.type.value == 'Regular' ? this.organizationId : 0,
      };

      this.geofenceService.updatePolygonGeofence(polyUpdateObjData).subscribe((updatePolyData: any) => {
        this.getAllGeofenceData();
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
    this.drawCircularGeofence();
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
    if (this.actionType == 'view' || this.actionType == 'edit') {
      if (this.selectedElementData && this.selectedElementData.type == 'C') { //-- circular geofence
        this.circularGeofence = true;
        this.setDefaultCircularGeofenceFormValue();
        this.loadGridData(this.poiData);
        this.drawCircularGeofence();
      } else { //-- polygon geofence
        this.polygoanGeofence = true;
        this.setDefaultPolygonGeofenceFormValue();
      }
    }
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
      let marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
      this.hereMap.addObject(marker);
      this.createResizableCircle(this.circularGeofenceFormGroup.controls.radius.value ? parseInt(this.circularGeofenceFormGroup.controls.radius.value) : 0, element);
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

}
