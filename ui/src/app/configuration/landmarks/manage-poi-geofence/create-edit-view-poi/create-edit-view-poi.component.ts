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
 query : any;
 public geocoder: any;
 public position: string;
 public locations: Array<any>;
poiFlag:boolean = true;
data: any;
address: 'chaitali';
zip: any;
city: any;
country: any;
userCreatedMsg: any = ''; 
hereMapService: any;
organizationId: number;
localStLanguage: any;
initData: any = [];
showLoadingIndicator: any = false;
categoryList: any = [];
lattitude: any;
longitude: any;
subCategoryList: any = [];
poiInitdata: any = [];
userName: string = '';
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
  
    ngOnInit(): void {
      this.localStLanguage = JSON.parse(localStorage.getItem("language"));
      this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));

      this.poiFormGroup = this._formBuilder.group({
        name: ['', [ Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
        category: ['', [ Validators.required]],
        sub_category: ['', [ Validators.required]],
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

      this.loadInitData();
      this.loadLandmarkCategoryData();

    }

    loadInitData() {
 


   
    }

    loadLandmarkCategoryData(){
      this.showLoadingIndicator = true;
      let objData = {
        type:'C',
        Orgid: this.organizationId
      }
      this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((parentCategoryData: any) => {
        this.categoryList = parentCategoryData.categories;
        this.getSubCategoryData();
      }, (error) => {
        this.categoryList = [];
        this.getSubCategoryData();
      }); 
    }

    getSubCategoryData(){
      let objData = {
        type:'S',
        Orgid: this.organizationId
      }
      this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((subCategoryData: any) => {
        this.subCategoryList = subCategoryData.categories;
        this.getCategoryDetails();
      }, (error) => {
        this.subCategoryList = [];
        this.getCategoryDetails();
      });
    }
  
    getCategoryDetails(){
      this.landmarkCategoryService.getLandmarkCategoryDetails().subscribe((categoryData: any) => {
        this.hideloader();
        //let data = this.createImageData(categoryData.categories);
      }, (error) => {
        this.hideloader();
        this.initData = [];
      });
    }
    
    hideloader() {
      // Setting display of spinner
      this.showLoadingIndicator = false;
    }

    toBack(){
      let emitObj = {
        stepFlag: false,
        msg: ""
      }    
      this.createViewEditPoiEmit.emit(emitObj);    
    }
  
    getBreadcum(type: any){
      return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / ${(type == 'view') ? (this.translationData.lblViewPOI ? this.translationData.lblViewPOI : 'View POI Details') : (type == 'edit') ? (this.translationData.lblEditPOI ? this.translationData.lblEditPOI : 'Edit POI Details') : (this.translationData.lblPOIDetails ? this.translationData.lblPOIDetails : 'Add New POI')}`;
    }

    public ngAfterViewInit() {
      let defaultLayers = this.platform.createDefaultLayers();
  //Step 2: initialize a map - this map is centered over Europe
   this.map = new H.Map(this.mapElement.nativeElement,
    defaultLayers.vector.normal.map,{
    center: {lat:50, lng:5},
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

  this.setUpClickListener(this.map, this.here, this.poiFlag, this.data,this);
  

  }
  



   setUpClickListener(map, here, poiFlag, data,thisRef) {
    // obtain the coordinates and display
    map.addEventListener('tap', function (evt) {
      if(poiFlag){
      var coord = map.screenToGeo(evt.currentPointer.viewportX,
              evt.currentPointer.viewportY); 
              let x = Math.abs(coord.lat.toFixed(4));
              let y = Math.abs(coord.lng.toFixed(4));
              console.log("latitude=" +x);
              console.log("longi=" +y);
            
              let locations = new H.map.Marker({lat:x, lng:y});

              map.addObject(locations);
             
              this.position = x +","+y;             
              console.log(this.position);
              if(this.position) {
                
            here.getAddressFromLatLng(this.position).then(result => {
                    this.locations = <Array<any>>result;
                    data = this.locations[0].Location.Address;
                    // console.log(this.locations[0].Location.Address);
                    console.log(data);
                    this.data = data;
                    poiFlag = false;
                    thisRef.setAddressValues(data,this.position);
                }, error => {
                    console.error(error);
                });
               }
              //  return this.data;
 
              }   
              
      });
      // console.log(this.hereMapService);
      // this.setAddressValues(this.hereMapService);

  }

 

  setAddressValues(addressVal,positions){
//     console.log("this is in setAddress()");
// console.log(addressVal);
this.address = addressVal.Label;
this.zip = addressVal.PostalCode;
this.city = addressVal.City;
// this.state
this.country = addressVal.Country;
var nameArr = positions.split(',');
// console.log(nameArr[0]);
this.poiFormGroup.get("address").setValue(this.address);
this.poiFormGroup.get("zip").setValue(this.zip);
this.poiFormGroup.get("city").setValue(this.city);
this.poiFormGroup.get("country").setValue(this.country);
this.poiFormGroup.get("lattitude").setValue(nameArr[0]);
this.poiFormGroup.get("longitude").setValue(nameArr[1]);
// this.poiFormGroup.get("category").setValue(this.selectedCategoryType);
}

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: this.userCreatedMsg,
    }    
    this.backToPage.emit(emitObj);
  }

  onCategoryChange(){

  }

  onSubCategoryChange(){

  }

  getUserCreatedMessage() {
    this.userName = `${this.poiFormGroup.controls.name.value}`;
    if (this.actionType == 'create') {
      if (this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', this.userName);
      else
        return ("New POI '$' Created Successfully").replace('$', this.userName);
    } else {
      if (this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', this.userName);
      else
        return ("New POI Details '$' Updated Successfully").replace('$', this.userName);
    }
  }

  onCreatePoi(){
    if(this.actionType == 'create'){
    console.log(this.poiFormGroup.controls);
      let objData = {
        id: 0,
        organizationId: this.organizationId,
        // categoryId: this.poiFormGroup.controls.category.value,
        // subCategoryId: this.poiFormGroup.controls.category.value,
         categoryId: 5,
        subCategoryId: 7,
        name: this.poiFormGroup.controls.name.value,
        address: this.poiFormGroup.controls.address.value,
        city: this.poiFormGroup.controls.city.value,
        country: this.poiFormGroup.controls.country.value,
        zipcode: this.poiFormGroup.controls.zip.value,
        latitude: this.poiFormGroup.controls.lattitude,
        longitude: this.poiFormGroup.controls.longitude,
        state: "MH",
        createdBy: 0
      }

      this.POIService.createPoi(objData).subscribe((res: any) => {
        console.log("created");
        console.log(res);
      this.POIService.getPois(this.organizationId).subscribe((data : any) => {
this.poiInitdata = data;
this.userCreatedMsg = this.getUserCreatedMessage();
        let emitObj = {
          stepFlag: false,
          successMsg: this.userCreatedMsg,
          tableData: this.poiInitdata,
        }    
        this.createViewEditPoiEmit.emit(emitObj); 

      });
      });
  }
  }
}