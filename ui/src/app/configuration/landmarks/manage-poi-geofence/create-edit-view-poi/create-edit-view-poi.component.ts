import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Form, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../shared/custom.validators';
import { ElementRef } from '@angular/core';
import { HereService } from 'src/app/services/here.service';
import { ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { POIService } from 'src/app/services/poi.service';
import { LandmarkCategoryService } from '../../../../services/landmarkCategory.service';

declare var H: any;

@Component({
  selector: 'app-create-edit-view-poi',
  templateUrl: './create-edit-view-poi.component.html',
  styleUrls: ['./create-edit-view-poi.component.less']
})
export class CreateEditViewPoiComponent implements OnInit {
  @Output() createViewEditPoiEmit = new EventEmitter<object>();
  @Input() createStatus: boolean;
  @Input() translationData: any;
  @Input() selectedElementData: any;
  @Input() viewFlag: boolean;
  @Input() categoryList: any;
  @Input() subCategoryList: any;
  @Output() backToPage = new EventEmitter<any>();
  breadcumMsg: any = '';
  @Input() actionType: any;
  poiFormGroup: FormGroup;
  form: Form;
  title = 'here-project';
  private platform: any;
  private search: any;
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
  latitude: any;
  longitude: any;
  localStLanguage: any;
  initData: any = [];
  showLoadingIndicator: any = false;
  actualLattitude: any;
  actuallongitude: any;
  poiInitdata: any = [];
  userName: string = '';
  state: any;
  selectedMarker: any;
  searchData: any = [];
  activeSearchList: any = false;
  duplicatePOIName: any = false;
  duplicatePOINameMsg: any = '';
  @Output() createEditViewPOIEmit = new EventEmitter<object>();

  @ViewChild("map")
  public mapElement: ElementRef;

  // @ViewChild('map') mapElement: ElementRef;

  constructor(private here: HereService, private landmarkCategoryService: LandmarkCategoryService, private _formBuilder: FormBuilder, private POIService: POIService) {
    this.query = "starbucks";
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.poiFormGroup = this._formBuilder.group({
      name: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc, Validators.min(1), Validators.max(100)]],
      category: ['', [Validators.required]],
      subcategory: [''],
      address: [''],
      zip: [''],
      city: [''],
      country: [''],
      lattitude: ['', [Validators.required]],
      longitude: ['', [Validators.required]]
    },
      {
        validator: [
          CustomValidators.specialCharValidationForName('name'),
        ]
      });
    this.breadcumMsg = this.getBreadcum(this.actionType);
    if (this.actionType == 'view' || this.actionType == 'edit') {
      this.setDefaultValue();
    }
  }

  toBack() {
    let emitObj = {
      stepFlag: false,
      msg: ""
    }
    this.backToPage.emit(emitObj);
  }

  getBreadcum(type: any) {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblLandmarks ? this.translationData.lblLandmarks : "Landmarks"} / ${(type == 'view') ? (this.translationData.lblViewPOI ? this.translationData.lblViewPOI : 'View POI Details') : (type == 'edit') ? (this.translationData.lblEditPOI ? this.translationData.lblEditPOI : 'Edit POI Details') : (this.translationData.lblAddNewPOI ? this.translationData.lblAddNewPOI : 'Add New POI')}`;
  }

  public ngAfterViewInit() {
    let defaultLayers = this.platform.createDefaultLayers();
    //Step 2: initialize a map - this map is centered over Europe
    this.map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 50, lng: 5 },
      // center: {lat:37.37634, lng:-122.03405},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    // add a resize listener to make sure that the map occupies the whole container
    window.addEventListener('resize', () => this.map.getViewPort().resize());

    // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));

    // Create the default UI components
    var ui = H.ui.UI.createDefault(this.map, defaultLayers);
   
    var searchbox = ui.getControl("searchbox");
    if (this.actionType == 'edit' || this.actionType == 'view') {
      this.removeMapObjects();
      this.drawMarkerOnMap();
      // let getSelectedLatitude = this.poiFormGroup.get("lattitude").value;
      // let getSelectedLongitude = this.poiFormGroup.get("longitude").value;
      // this.selectedMarker = new H.map.Marker({ lat: getSelectedLatitude, lng: getSelectedLongitude });
      // this.map.addObject(this.selectedMarker);
    }
    if(this.actionType != 'view'){
      var bubble = new H.ui.InfoBubble({ lng: 13.4050, lat: 52.5200 }, {
          content: '<b>Click on map to create POI position</b>'
      });
      // Add info bubble to the UI:
      ui.addBubble(bubble);
      this.setUpClickListener(this.map, behavior, this.selectedMarker, this.here, this.poiFlag, this.data, this, bubble, ui);
    }
  }

  drawMarkerOnMap(){
    let getSelectedLatitude = this.selectedElementData.latitude;//this.poiFormGroup.get("lattitude").value;
    let getSelectedLongitude = this.selectedElementData.longitude;//this.poiFormGroup.get("longitude").value;
    this.selectedMarker = new H.map.Marker({ lat: getSelectedLatitude, lng: getSelectedLongitude });
    this.map.addObject(this.selectedMarker);
  }

  removeMapObjects(){
    this.map.removeObjects(this.map.getObjects());
  }

  searchValue(event: any) {
    this.activeSearchList = true;
    if(event.target.value == "") {
      this.activeSearchList = false;
    }
    ////console.log("----search value called--",event.target.value);
    let inputData = event.target.value;
          // "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
      // var a = https://places.ls.hereapi.com/places/v1/autosuggest?at=40.74917,-73.98529&q=chrysler&apiKey="BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw";

      this.POIService.getAutoSuggestMap(inputData).subscribe((res: any) => {
     let newData = res.results;
          this.searchData = newData;
       });
       
  }

  SearchListItems(item){
   
// ////console.log("you clicked on:" +item.title);
// //console.log(item.position);
this.map.setCenter({lat:item.position[0], lng:item.position[1]});
this.map.setZoom(14);
  }

  setUpClickListener(map, behavior, selectedMarker, here, poiFlag, data, thisRef, bubble, ui) {
    // obtain the coordinates and display
    // let actionTypeGlobal = this.actionType;
    //  thisRef.UpdatedPoiFlag = thisRef.poiFlag;
    map.addEventListener('tap', function (evt) {
      // let selectedMakerOnClick = selectedMarker;
      // //console.log("----UpdatedPoiFlag---",thisRef.poiFlag)
      // //console.log(ui.getBubbles());
      ui.removeBubble(bubble);
      if (thisRef.poiFlag) {
        thisRef.setNewMapMarker(map, behavior, selectedMarker, here, poiFlag, data, thisRef, evt)
      } else {
        // //console.log("---this----", this)
        // //console.log("---thisRef----", thisRef)
        let getSelectedLatitude = thisRef.poiFormGroup.get("lattitude").value;
        let getSelectedLongitude = thisRef.poiFormGroup.get("longitude").value;
        let existingMarker = new H.map.Marker({ lat: getSelectedLatitude, lng: getSelectedLongitude });
        // this.map.addObject(this.selectedMarker);

        if (existingMarker) {
          // //console.log("---existingMarker----", existingMarker)
          map.removeObjects(map.getObjects())
          thisRef.setNewMapMarker(map, behavior, selectedMarker, here, poiFlag, data, thisRef, evt)
        }
        // //console.log("--second click triggered in MAP")
      }
    });
  }


  setNewMapMarker(map, behavior, selectedMarker, here, poiFlag, data, thisRef, evt) {
    let actionTypeGlobal = this.actionType;
    if (actionTypeGlobal == 'edit') {
      this.poiFlag = false;
      // //console.log("----when edit selected marker--",selectedMarker)
      // map.removeObject(selectedMarker);
      map.removeObjects(map.getObjects());
      //             thisRef.setNewMapMarker(map,behavior,selectedMarker, here, poiFlag, data,thisRef,evt)
      // map.removeObjects(searchMarkers);
    }
    var coord = map.screenToGeo(evt.currentPointer.viewportX,
      evt.currentPointer.viewportY);
    let x = Math.abs(coord.lat.toFixed(4));
    let y = Math.abs(coord.lng.toFixed(4));
    this.actualLattitude = x;
    this.actuallongitude = y;
    //console.log("latitude=" + x);
    //console.log("longi=" + y);

    let locations = new H.map.Marker({ lat: x, lng: y }, {
      // mark the object as volatile for the smooth dragging
      volatility: true
    });


    // Ensure that the marker can receive drag events
    locations.draggable = true;
    map.addObject(locations);

    // disable the default draggability of the underlying map
    // and calculate the offset between mouse and target's position
    // when starting to drag a marker object:
    map.addEventListener('dragstart', function (ev) {
      var target = ev.target,
        pointer = ev.currentPointer;
      if (target instanceof H.map.Marker) {
        var targetPosition = map.geoToScreen(target.getGeometry());
        target['offset'] = new H.math.Point(pointer.viewportX - targetPosition.x, pointer.viewportY - targetPosition.y);
        behavior.disable();
      }
    }, false);


    // re-enable the default draggability of the underlying map
    // when dragging has completed
    map.addEventListener('dragend', function (ev) {
      var target = ev.target;
      if (target instanceof H.map.Marker) {
        behavior.enable();
      }
    }, false);

    // Listen to the drag event and move the position of the marker
    // as necessary
    map.addEventListener('drag', function (ev) {
      var target = ev.target,
        pointer = ev.currentPointer;
      if (target instanceof H.map.Marker) {
        target.setGeometry(map.screenToGeo(pointer.viewportX - target['offset'].x, pointer.viewportY - target['offset'].y));
      }

      var coord = map.screenToGeo(evt.currentPointer.viewportX,
        evt.currentPointer.viewportY);
      let x = Math.abs(coord.lat.toFixed(4));
      let y = Math.abs(coord.lng.toFixed(4));
      //console.log(" updated - latitude=" + x);
      //console.log("updated - longi=" + y);

      let dataUpdated = thisRef.locations[0].Location.Address;
      // //console.log(this.locations[0].Location.Address);
      let position = thisRef.locations[0].Location.DisplayPosition;
      this.data = dataUpdated;
      // //console.log("---while dragging location and data--",dataUpdated,"---position---",position);

      thisRef.setAddressValues(dataUpdated, position);
      this.actualLattitude = x;
      this.actuallongitude = y;

      this.position = this.actualLattitude + "," + this.actuallongitude;
      // //console.log("---updated positions with lat long----",this.position);
      if (this.position) {

        here.getAddressFromLatLng(this.position).then(result => {
          this.locations = <Array<any>>result;
          data = this.locations[0].Location.Address;
          // //console.log(this.locations[0].Location.Address);
          let pos = this.locations[0].Location.DisplayPosition;
          // //console.log(data);
          this.data = data;
          thisRef.poiFlag = false;
          thisRef.setAddressValues(data, pos);
        }, error => {
          // console.error(error);
        });
      }

    }, false);


    this.position = this.actualLattitude + "," + this.actuallongitude;
    // //console.log("---initial positions with lat long----", this.position);
    if (this.position) {
      // //console.log("---thisRef--POIFLAG--", thisRef.poiFlag)
      this.poiFlag = false;
      thisRef.poiFlag = false;
      // //console.log("---this--POIFLAG--", this.poiFlag)
      here.getAddressFromLatLng(this.position).then(result => {
        this.locations = <Array<any>>result;
        data = this.locations[0].Location.Address;
        // //console.log(this.locations[0].Location.Address);
        let pos = this.locations[0].Location.DisplayPosition;
        // //console.log(data);
        this.data = data;
        this.poiFlag = false;
        thisRef.setAddressValues(data, pos);
      }, error => {
        // console.error(error);
      });
    }
    //  return this.data;

  }

  setAddressValues(addressVal, positions) {
    //     //console.log("this is in setAddress()");
    //console.log(addressVal);
    this.address = addressVal.Label;
    this.zip = addressVal.PostalCode;
    this.city = addressVal.City;
    this.state = addressVal.State;
    this.country = addressVal.Country;
    // var nameArr = positions.split(',');
    let pos = positions;
    // //console.log(this.lattitude);
    this.poiFormGroup.get("address").setValue(this.address);
    this.poiFormGroup.get("zip").setValue(this.zip);
    this.poiFormGroup.get("city").setValue(this.city);
    this.poiFormGroup.get("country").setValue(this.country);
    this.poiFormGroup.get("lattitude").setValue(positions.Latitude);
    this.poiFormGroup.get("longitude").setValue(positions.Longitude);
    //console.log("poiformgroup=" + this.poiFormGroup);
    // this.poiFormGroup.get("category").setValue(this.selectedCategoryType);
  }

  onCancel() {
    let emitObj = {
      stepFlag: false,
      successMsg: this.userCreatedMsg,
    }
    this.backToPage.emit(emitObj);
  }

  onCategoryChange() {

  }

  onSubCategoryChange() {

  }

  getUserCreatedMessage() {
    this.userName = `${this.poiFormGroup.controls.name.value}`;
    if (this.actionType == 'create') {
      if (this.translationData.lblNewPOICreatedSuccessfully)
        return this.translationData.lblNewPOICreatedSuccessfully.replace('$', this.userName);
      else
        return ("New POI '$' Created Successfully").replace('$', this.userName);
    } else {
      if (this.translationData.lblPOIDetailsUpdatedSuccessfully)
        return this.translationData.lblPOIDetailsUpdatedSuccessfully.replace('$', this.userName);
      else
        return ("'$' POI Details Updated Successfully").replace('$', this.userName);
    }
  }

  setDefaultValue() {
    this.poiFormGroup.get("name").setValue(this.selectedElementData.name);
    this.poiFormGroup.get("address").setValue(this.selectedElementData.address);
    this.poiFormGroup.get("city").setValue(this.selectedElementData.city);
    this.poiFormGroup.get("zip").setValue(this.selectedElementData.zipcode);
    this.poiFormGroup.get("lattitude").setValue(this.selectedElementData.latitude);
    this.poiFormGroup.get("longitude").setValue(this.selectedElementData.longitude);
    this.poiFormGroup.get("country").setValue(this.selectedElementData.country);
    this.poiFormGroup.get("category").setValue(this.selectedElementData.categoryId);
    this.poiFormGroup.get("subcategory").setValue(this.selectedElementData.subCategoryId);
  }

  getDuplicateCategoryMsg(poiName: any){
    if(this.translationData.lblDuplicatePOINameMsg)
      this.duplicatePOINameMsg = this.translationData.lblDuplicatePOINameMsg.replace('$', poiName);
    else
      this.duplicatePOINameMsg = ("POI name '$' already exists.").replace('$', poiName);
  }

  onCreatePoi() {

    let subcatId = this.poiFormGroup.controls.subcategory.value;
    if(subcatId == "")
    {
      subcatId = 0;
    }
    let zip = this.poiFormGroup.controls.zip.value;
    if(zip == null)
    {
      zip = "";
    }
    let objData = {
      id: 0,
      organizationId: this.organizationId,
      categoryId: this.poiFormGroup.controls.category.value,
      subCategoryId: subcatId,
      //  categoryId: 5,
      // subCategoryId: 7,
      name: this.poiFormGroup.controls.name.value,
      address: this.poiFormGroup.controls.address.value,
      city: this.poiFormGroup.controls.city.value,
      country: this.poiFormGroup.controls.country.value,
      zipcode: zip,
      latitude: this.poiFormGroup.controls.lattitude.value,
      longitude: this.poiFormGroup.controls.longitude.value,
      state: this.state,
      createdBy: 0
    }

    if (this.actionType == 'create') {
      this.POIService.createPoi(objData).subscribe((res: any) => {
        this.POIService.getPois(this.organizationId).subscribe((data: any) => {
          this.poiInitdata = data;
          this.userCreatedMsg = this.getUserCreatedMessage();
          let emitObj = {
            stepFlag: false,
            successMsg: this.userCreatedMsg,
            tableData: this.poiInitdata,
          }
          this.backToPage.emit(emitObj);

        });
      }, (error) => {
        if(error.status == 409){
          this.duplicatePOIName = true;
          this.getDuplicateCategoryMsg(this.poiFormGroup.controls.name.value.trim());
        }
      });
    }
    else { //-- update
      let objData = {
        id: this.selectedElementData.id,
        icon: this.selectedElementData.icon,
        organizationId: this.selectedElementData.organizationId,
        categoryId: this.poiFormGroup.controls.category.value,
        subCategoryId: this.poiFormGroup.controls.subcategory.value,
        name: this.poiFormGroup.controls.name.value,
        address: this.poiFormGroup.controls.address.value,
        city: this.poiFormGroup.controls.city.value,
        country: this.poiFormGroup.controls.country.value,
        zipcode: this.poiFormGroup.controls.zip.value,
        latitude: this.poiFormGroup.controls.lattitude.value,
        longitude: this.poiFormGroup.controls.longitude.value,
        state: this.selectedElementData.state,
        createdBy: 0
      }

      this.POIService.updatePoi(objData).subscribe((data: any) => {
        this.POIService.getPois(this.organizationId).subscribe((data: any) => {
          this.poiInitdata = data;
          this.userCreatedMsg = this.getUserCreatedMessage();
          let emitObj = {
            stepFlag: false,
            successMsg: this.userCreatedMsg,
            tableData: this.poiInitdata,
          }
          this.backToPage.emit(emitObj);

        });
      });

    }

  }

  onReset(){
    //-- reset POI here
    this.setDefaultValue();
    this.removeMapObjects();
    this.drawMarkerOnMap();
  }

}