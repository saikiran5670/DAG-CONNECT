import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../shared/custom.validators';
import { HereService } from 'src/app/services/here.service';
import { FormBuilder } from '@angular/forms';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
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
  geoSelectionFlag: boolean = true;

  @ViewChild("map")
  public mapElement: ElementRef;

  constructor(private here: HereService, private _formBuilder: FormBuilder) {
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
      category: ['', [Validators.required]],
      sub_category: ['', [Validators.required]],
      address: [''],
      zip: [''],
      city: [''],
      country: [''],
      lattitude: [''],
      longitude: ['']
    },
      {
        validator: [
          CustomValidators.specialCharValidationForName('name'),
        ]
      });
    this.breadcumMsg = this.getBreadcum(this.actionType);
  }

  updatePOIDatasource(){
    this.dataSourceForPOI = new MatTableDataSource(this.poiData);
    setTimeout(() => {
      this.dataSourceForPOI.paginator = this.paginator;
      this.dataSourceForPOI.sort = this.sort;
    });
  }

  setType() {
    this.circularGeofenceFormGroup.get('type').setValue(this.types[0]);
  }

  getBreadcum(type: any) {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / ${(type == 'view') ? (this.translationData.lblViewPOI ? this.translationData.lblViewPOI : 'View POI Details') : (type == 'edit') ? (this.translationData.lblEditPOI ? this.translationData.lblEditPOI : 'Edit POI Details') : (this.translationData.lblPOIDetails ? this.translationData.lblPOIDetails : 'Add New POI')}`;
  }

  public ngAfterViewInit() {
    let defaultLayers = this.platform.createDefaultLayers();
    //Step 2: initialize a map - this map is centered over Europe
    let map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 50, lng: 5 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    // add a resize listener to make sure that the map occupies the whole container
    window.addEventListener('resize', () => map.getViewPort().resize());

    // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

    // Create the default UI components
    var ui = H.ui.UI.createDefault(map, defaultLayers);

    this.setUpClickListener(map, this.here, this.poiFlag);

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

  }

  onReset() {

  }

  applyPOIFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSourceForPOI defaults to lowercase matches
    this.dataSourceForPOI.filter = filterValue;
  }

  masterToggleForPOI() {
    this.isAllSelectedForPOI()
      ? this.selectedPOI.clear()
      : this.dataSourceForPOI.data.forEach((row) =>
        this.selectedPOI.select(row)
      );
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

  onCategoryChange(event: any){

  }

  onSubCategoryChange(event: any){

  }

  geoSelection(type: any){
    this.geoSelectionFlag = false;
    if(type == 'circular'){
      this.circularGeofence = true;
      this.setType();
      this.updatePOIDatasource();
    }else{ //-- polygon
      this.polygoanGeofence = true;
    }
  }

}
