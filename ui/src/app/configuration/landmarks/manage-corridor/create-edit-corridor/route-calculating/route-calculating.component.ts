import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../../shared/custom.validators';
import { HereService } from 'src/app/services/here.service';
import { CorridorService } from '../../../../../services/corridor.service';
import {
  CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData
} from 'ng2-completer';
import { ConfigService } from '@ngx-config/core';
import { Options } from '@angular-slider/ngx-slider';

declare var H: any;

@Component({
  selector: 'app-route-calculating',
  templateUrl: './route-calculating.component.html',
  styleUrls: ['./route-calculating.component.less']
})
export class RouteCalculatingComponent implements OnInit {
  @Input() translationData: any;
  @Input() exclusionList :  any;
  @Input() actionType: any; 
  @Input() selectedElementData : any;
  @Output() backToPage = new EventEmitter<any>();
  @Output() backToCreate = new EventEmitter<any>();
  @Output() backToReject = new EventEmitter<any>();


  breadcumMsg: any = '';
  corridorFormGroup: FormGroup;
  corridorTypeList = [{id:1,value:'Route Calculating'},{id:2,value:'Existing Trips'}];
  trailerList = [0,1,2,3,4];
  selectedCorridorTypeId : any = 46;
  selectedTrailerId = 0;
  private platform: any;
  map: any;
  private ui: any;
  lat: any = '37.7397';
  lng: any = '-121.4252';
  @ViewChild("map")
  public mapElement: ElementRef;
  hereMapService: any;
  organizationId: number;
  corridorId : number = 0;
  localStLanguage: any;
  accountId: any = 0;
  hereMap: any;
  distanceinKM = 0;
  viaRouteCount : boolean = false;
  transportDataChecked : boolean= false;
  trafficFlowChecked : boolean = false;
  corridorWidth : number = 100;
  corridorWidthKm : number = 0.1;
  sliderValue : number = 0;
  min : number = 0;
  max : number = 10000;
  map_key : string = "";
  map_id: string = "";
  map_code : string="";
  mapGroup ;
  searchStr : string = "";
  searchEndStr : string = "";
  searchViaStr : string = "";
  corridorName : string = "";
  startAddressPositionLat :number = 0; // = {lat : 18.50424,long : 73.85286};
  startAddressPositionLong :number = 0; // = {lat : 18.50424,long : 73.85286};
  startMarker : any;
  endMarker :any;
  routeCorridorMarker : any;
  routeOutlineMarker : any;
  endAddressPositionLat : number = 0;
  endAddressPositionLong : number = 0;
  
  explosiveChecked :boolean = false;
  gasChecked :boolean = false;
  flammableChecked : boolean = false;
  combustibleChecked : boolean = false;
  organicChecked : boolean = false;
  poisonChecked : boolean = false;
  radioactiveChecked : boolean = false;
  corrosiveChecked : boolean = false;
  poisonInhaleChecked : boolean = false;
  waterHarmChecked : boolean = false;
  othersChecked : boolean = false;

  tollRoadId : string = undefined;
  motorWayId : string = undefined;
  railFerriesId : string = undefined;
  tunnelId: string = undefined;
  dirtRoadId : string = undefined;
  boatFerriesId : string = undefined;
  

  getAttributeData : any;
  getExclusionList : any;
  getVehicleSize : any;
  additionalData : any;

  
  tollRoadValue : any ;
  motorWayValue : any;
  boatFerriesValue : any;
  railFerriesValue : any;
  tunnelValue : any;
  dirtRoadValue :any;
  trailerValue : any;

  value: number = 100;
  options: Options = {
    floor: 0,
    ceil: 10000
  };
  searchStrError : boolean = false;
  searchEndStrError : boolean = false;
  strPresentStart: boolean = false;
  strPresentEnd: boolean = false;

  
  viaAddressPositionLat : any;
  viaAddressPositionLong : any;
  viaRoutePlottedPoints : any = [];
  viaMarker : any;

  
  vehicleHeightValue: number = 0;
  vehicleWidthValue: number = 0;
  vehicleLengthValue: number = 0;
  vehicleLimitedWtValue: number = 0;
  vehicleWtPerAxleValue: number = 0;

  createFlag : boolean = true;
  routeDistance: number = 0;

  constructor(private hereService: HereService,private formBuilder: FormBuilder, private corridorService : CorridorService,
    private completerService: CompleterService, private config: ConfigService) {
     this.map_key =  config.getSettings("hereMap").api_key;
     this.map_id =  config.getSettings("hereMap").app_id;
     this.map_code =  config.getSettings("hereMap").app_code;


    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });
    //this.configureAutoCompleteForLocationSearch();
    this.configureAutoSuggest()
   }

  ngOnInit(){
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.corridorFormGroup = this.formBuilder.group({
      corridorType:['Regular'],
      label: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      widthInput : ['', [Validators.required]],
      viaroute1: [''],
      viaroute2: [''],
      trailer:["Regular"],
      tollRoad:['Regular'],
      motorWay:['Regular'],
      boatFerries:['Regular'],
      railFerries:['Regular'],
      tunnels:['Regular'],
      dirtRoad:['Regular'],
      vehicleHeight:[''],
      vehicleWidth: [''],
      vehicleLength : [''],
      limitedWeight: [''],
      weightPerAxle: ['']

    },
    {
      validator: [
        CustomValidators.specialCharValidationForNameWithoutRequired('label'),
        CustomValidators.numberFieldValidation('vehicleHeight',50),
        CustomValidators.numberFieldValidation('vehicleWidth',50),
        CustomValidators.numberFieldValidation('vehicleLength',300),
        CustomValidators.numberFieldValidation('limitedWeight',1000),
        CustomValidators.numberFieldValidation('weightPerAxle',1000),
        CustomValidators.numberFieldValidation('widthInput',10)

      ]});
    //this.initiateDropDownValues();
    if((this.actionType === 'edit' || this.actionType === 'view') && this.selectedElementData){
      this.setCorridorData();
      this.createFlag = false;
      this.strPresentStart = true;
      this.strPresentEnd = true;
    }
    this.subscribeWidthValue();
    //this.configureAutoCompleteForLocationSearch();
  }

  subscribeWidthValue(){
    this.corridorFormGroup.get("widthInput").valueChanges.subscribe(x => {
      console.log(x)
      this.corridorWidthKm = Number(x);
      this.corridorWidth = this.corridorWidthKm  * 1000;
      //this.calculateAB();
      let drawWidth = this.corridorWidthKm*10;
      if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0){
        this.addTruckRouteShapeToMap(drawWidth);
      }
   });

  }

  vehicleSizeFocusOut(){
    if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0){
      this.calculateTruckRoute();
    }
  }
  setCorridorData(){
    let _selectedElementData = this.selectedElementData;
    if(_selectedElementData){
      this.corridorId = _selectedElementData.id;
      if(this.corridorId){
          this.corridorService.getCorridorFullList(this.organizationId,this.corridorId).subscribe((data)=>{
              console.log(data)
              if(data[0]["corridorProperties"]){
                 this.additionalData =  data[0]["corridorProperties"];
                 this.setAdditionalData();
              
              }
          })
      }
      this.corridorName = _selectedElementData.corridoreName;
      this.corridorFormGroup.controls.label.setValue(_selectedElementData.corridoreName);
      this.searchStr = _selectedElementData.startPoint;
      this.searchEndStr = _selectedElementData.endPoint;
      this.startAddressPositionLat = _selectedElementData.startLat;
      this.startAddressPositionLong = _selectedElementData.startLong;
      this.endAddressPositionLat = _selectedElementData.endLat;
      this.endAddressPositionLong = _selectedElementData.endLong;
      this.corridorWidth = _selectedElementData.width;
      this.corridorWidthKm = this.corridorWidth / 1000;
      this.plotStartPoint(this.searchStr);
      this.plotEndPoint(this.searchEndStr);
      this.calculateTruckRoute()
    }
  }

  setAdditionalData(){
    let _data = this.additionalData;
    this.getAttributeData = _data["attribute"];
    this.getExclusionList = _data["exclusion"];
    this.combustibleChecked = this.getAttributeData["isCombustible"];
    this.corrosiveChecked = this.getAttributeData["isCorrosive"];
    this.explosiveChecked = this.getAttributeData["isExplosive"];
    this.flammableChecked = this.getAttributeData["isFlammable"];
    this.gasChecked = this.getAttributeData["isGas"];
    this.organicChecked = this.getAttributeData["isOrganic"];
    this.othersChecked = this.getAttributeData["isOther"];
    this.poisonChecked = this.getAttributeData["isPoision"];
    this.poisonInhaleChecked = this.getAttributeData["isPoisonousInhalation"];
    this.radioactiveChecked = this.getAttributeData["isRadioActive"];
    this.waterHarmChecked = this.getAttributeData["isWaterHarm"];
    this.selectedTrailerId = this.getAttributeData["noOfTrailers"];
    this.trafficFlowChecked = _data["isTrafficFlow"];
    this.transportDataChecked = _data["isTransportData"];
    this.getVehicleSize = _data["vehicleSize"];
    this.vehicleHeightValue = this.getVehicleSize.vehicleHeight;
    this.vehicleWidthValue = this.getVehicleSize.vehicleWidth;
    this.vehicleLengthValue = this.getVehicleSize.vehicleLength;
    this.vehicleLimitedWtValue = this.getVehicleSize.vehicleLimitedWeight;
    this.vehicleWtPerAxleValue = this.getVehicleSize.vehicleWeightPerAxle;


    this.corridorFormGroup.controls.vehicleHeight.setValue(this.getVehicleSize.vehicleHeight);
    this.corridorFormGroup.controls.vehicleWidth.setValue(this.getVehicleSize.vehicleWidth);
    this.corridorFormGroup.controls.vehicleLength.setValue(this.getVehicleSize.vehicleLength);
    this.corridorFormGroup.controls.limitedWeight.setValue(this.getVehicleSize.vehicleLimitedWeight);
    this.corridorFormGroup.controls.weightPerAxle.setValue(this.getVehicleSize.vehicleWeightPerAxle);
    this.tollRoadId = this.getExclusionList["tollRoadType"];
    this.boatFerriesId = this.getExclusionList["boatFerriesType"];
    this.dirtRoadId = this.getExclusionList["dirtRoadType"];
    this.motorWayId = this.getExclusionList["mortorway"];
    this.tunnelId = this.getExclusionList["tunnelsType"];
    this.railFerriesId = this.getExclusionList["railFerriesType"];

    this.initiateDropDownValues();

  }

  initiateDropDownValues(){
    this.corridorFormGroup.controls.trailer.setValue(this.selectedTrailerId);
    this.trailerValue = this.selectedTrailerId;
    this.corridorFormGroup.controls.tollRoad.setValue(this.tollRoadId);
    this.tollRoadValue = this.exclusionList.filter(e=> e.enum === this.tollRoadId)[0].value;
    this.corridorFormGroup.controls.motorWay.setValue(this.motorWayId);
    this.motorWayValue = this.exclusionList.filter(e=> e.enum === this.motorWayId)[0].value;
    this.corridorFormGroup.controls.boatFerries.setValue(this.boatFerriesId);
    this.boatFerriesValue = this.exclusionList.filter(e=> e.enum === this.boatFerriesId)[0].value;
    this.corridorFormGroup.controls.railFerries.setValue(this.railFerriesId);
    this.railFerriesValue = this.exclusionList.filter(e=> e.enum === this.railFerriesId)[0].value;
    this.corridorFormGroup.controls.tunnels.setValue(this.tunnelId);
    this.tunnelValue = this.exclusionList.filter(e=> e.enum === this.tunnelId)[0].value;
    this.corridorFormGroup.controls.dirtRoad.setValue(this.dirtRoadId);
    this.dirtRoadValue = this.exclusionList.filter(e=> e.enum === this.dirtRoadId)[0].value;
    this.corridorFormGroup.controls.widthInput.setValue(this.corridorWidthKm);
 }

  public ngAfterViewInit() {
    this.initMap();
  }

  initMap() {
    let defaultLayers = this.platform.createDefaultLayers();
    //Step 2: initialize a map - this map is centered over Europe
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      //center:{lat:41.881944, lng:-87.627778},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    // add a resize listener to make sure that the map occupies the whole container
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    // Create the default UI components
    var ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
    var group = new H.map.Group();
    this.mapGroup = group;
  }

  sliderChanged(){
     // this.corridorWidth = _event.value;
      this.corridorWidthKm = this.corridorWidth / 1000;
      this.corridorFormGroup.controls.widthInput.setValue(this.corridorWidthKm);
      let drawWidth = this.corridorWidthKm*10;
      if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0){
        this.addTruckRouteShapeToMap(drawWidth);
      }
      //this.checkRoutePlot();
     // this.updateWidth()
    //   if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0){
    //  // this.calculateAB();
    //  this.updateWidth()

    // }

      //this.calculateRouteFromAtoB();
  }

  checkRoutePlot(){
    if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0){
     // this.calculateAB();
     // this.calculateNewRoute()
     this.calculateTruckRoute();
    }
  }
  changeSliderInput(){
    this.corridorWidthKm = this.corridorFormGroup.controls.widthInput.value;
    this.corridorWidth = this.corridorWidthKm * 1000;
  }
  
  formatLabel(value:number){
    return value;
  }

  addViaRoute(){
    this.viaRouteCount = true;
  }

  removeViaRoute(){
    this.viaRouteCount = false;
  }

  transportDataCheckedFn(_checked){
    this.transportDataChecked = _checked;
  }

  
  trafficFlowCheckedFn(_checked){
    this.trafficFlowChecked = _checked;
  }

  hazardousMaterial : string = '';
  attributeCheck(_checked, type) {
    switch (type) {
      case 'explosive':
        this.explosiveChecked = _checked;
        break;
      case 'gas':
        this.gasChecked = _checked;
        break;
      case 'flammable':
        this.flammableChecked = _checked;
        break;
      case 'combustible':
        this.combustibleChecked = _checked;
        break;
      case 'organic':
        this.organicChecked = _checked;
        break;
      case 'poison':
        this.poisonChecked = _checked;
        break;
      case 'radioactive':
        this.radioactiveChecked = _checked;
        break;
      case 'corrosive':
        this.corrosiveChecked = _checked;
        break;
      case 'poisonInhale':
        this.poisonInhaleChecked = _checked;
        break;
      case 'waterHarm':
        this.waterHarmChecked = _checked;
        break;
      case 'others':
        this.othersChecked = _checked;
        break;
      default:
        break;
    }
  }

  trailerSelected(_event){
    this.selectedTrailerId = _event.value;
  }

  exclusionSelected(_event,type){
    switch (type) {
      case 'tollRoad':
          this.tollRoadId = _event.value;
        break;
        case 'motorWay':
          this.motorWayId = _event.value;
        break;
        
        case 'boatFerries':
          this.boatFerriesId = _event.value;
        break;
        case 'railFerries':
          this.railFerriesId = _event.value;
        break;
        case 'tunnel':
          this.tunnelId = _event.value;
        break;
        case 'dirtRoad':
          this.dirtRoadId = _event.value;
        break;
      default:
        break;
    }
    this.vehicleSizeFocusOut();
  }

  createCorridorClicked(){
   
    var corridorObj = {
      "id": this.corridorId ? this.corridorId : 0,
      "organizationId": this.organizationId,
      "corridorType": "R",
      "corridorLabel":this.corridorFormGroup.controls.label.value,
      "startAddress": this.searchStr,
      "startLatitude": this.startAddressPositionLat,
      "startLongitude": this.startAddressPositionLong,
      "endAddress": this.searchEndStr,
      "endLatitude": this.endAddressPositionLat,
      "endLongitude": this.endAddressPositionLong,
      "width": this.corridorWidth,
      "distance":this.routeDistance,
      "viaAddressDetails": this.viaRoutePlottedPoints,
      "transportData": this.transportDataChecked,
      "trafficFlow": this.trafficFlowChecked,
      "state": "A",
      "created_At": 0,
      "created_By": this.organizationId,
      "modified_At": 0,
      "modified_By": this.organizationId,
      "attribute": {
        "trailer": this.selectedTrailerId,
        "explosive": this.explosiveChecked,
        "gas": this.gasChecked,
        "flammable": this.flammableChecked,
        "combustible": this.combustibleChecked,
        "organic": this.organicChecked,
        "poision": this.poisonChecked,
        "radioActive": this.radioactiveChecked,
        "corrosive": this.corrosiveChecked,
        "poisonousInhalation": this.poisonInhaleChecked,
        "waterHarm": this.waterHarmChecked,
        "other": this.othersChecked
      },
      "exclusion": {
        "tollRoad": this.tollRoadId,
        "mortorway": this.motorWayId,
        "boatFerries":this.boatFerriesId,
        "railFerries": this.railFerriesId,
        "tunnels": this.tunnelId,
        "dirtRoad":this.dirtRoadId,
      },
      "vehicleSize": {
        "vehicleSizeHeight":this.corridorFormGroup.controls.vehicleHeight.value ? this.corridorFormGroup.controls.vehicleHeight.value : 0,
        "vehicleSizeWidth": this.corridorFormGroup.controls.vehicleWidth.value ? this.corridorFormGroup.controls.vehicleWidth.value : 0,
        "vehicleSizeLength": this.corridorFormGroup.controls.vehicleLength.value ? this.corridorFormGroup.controls.vehicleLength.value : 0,
        "vehicleSizeLimitedWeight": this.corridorFormGroup.controls.limitedWeight.value ? this.corridorFormGroup.controls.limitedWeight.value : 0,
        "vehicleSizeWeightPerAxle": this.corridorFormGroup.controls.weightPerAxle.value ? this.corridorFormGroup.controls.weightPerAxle.value : 0,
      }
    }
    console.log(corridorObj)
    this.corridorService.createRouteCorridor(corridorObj).subscribe((responseData)=>{
      if(responseData.code === 200){
          let emitObj = {
            booleanFlag: false,
            successMsg: "create",
            fromCreate:true,
          }  
          this.backToCreate.emit(emitObj);
      }
    },(error)=>{
        if(error.status === 409){
          let emitObj = {
            booleanFlag: false,
            successMsg: "duplicate",
            fromCreate:true,
          }  
          this.backToReject.emit(emitObj);
        }
    })
  }

  getSuggestion(_event){
    let startValue = _event.target.value;
    
   
    console.log(_event)
  }

  backToCorridorList(){
    let emitObj = {
      booleanFlag: false,
      successMsg: "",
    }  
    this.backToPage.emit(emitObj);
  }

  resetValues(){
    if(this.actionType === 'create'){
        
    this.tollRoadId = undefined;
    this.motorWayId =undefined;
    this.railFerriesId = undefined;
    this.tunnelId =undefined;
    this.dirtRoadId = undefined;
    this.boatFerriesId = undefined;
    this.explosiveChecked = false;
    this.gasChecked = false;
    this.flammableChecked  = false;
    this.combustibleChecked  = false;
    this.organicChecked  = false;
    this.poisonChecked  = false;
    this.radioactiveChecked  = false;
    this.corrosiveChecked  = false;
    this.poisonInhaleChecked  = false;
    this.waterHarmChecked  = false;
    this.othersChecked  = false;
    this.transportDataChecked = false;
    this.trafficFlowChecked = false;
    this.corridorWidth = 100;
    this.corridorWidthKm = 0.1;
    this.corridorFormGroup.controls.vehicleHeight.setValue("");
    this.corridorFormGroup.controls.vehicleLength.setValue("");
    this.corridorFormGroup.controls.vehicleWidth.setValue("");
    this.corridorFormGroup.controls.limitedWeight.setValue("");
    this.corridorFormGroup.controls.weightPerAxle.setValue("");
    this.corridorFormGroup.controls.startaddress.setValue("");
    this.corridorFormGroup.controls.endaddress.setValue("");
    }
    else{
      this.setAdditionalData();
    }
  }

  clearMap(){
    this.mapGroup.removeAll();
    this.hereMap.removeObject(this.mapGroup);
  }

  onStartFocus(){
    this.searchStrError = true;
    this.strPresentStart = false;
    this.searchStr = null;
    this.startAddressPositionLat = 0;
    this.clearMap();
    if(this.searchEndStr){
      this.plotEndPoint(this.searchEndStr);
    }
    
  }
  onEndFocus(){
    this.searchEndStrError = true;
    this.strPresentEnd = false;
    this.searchEndStr = null;
    this.endAddressPositionLat = 0;
    this.clearMap();
    if(this.searchStr){
      this.plotStartPoint(this.searchStr);
    }
  }

  onSelected(selectedAddress: CompleterItem){
    //console.log(item.title)
    if(this.searchStr){
       this.searchStrError = false;
       this.strPresentStart = true;
    }
    if(selectedAddress){
      let postalCode = selectedAddress["originalObject"]["label"];
      this.plotStartPoint(postalCode)
    }

  }

  onEndSelected(selectedAddress: CompleterItem){
    
    if(this.searchEndStr){
      this.searchEndStrError = false;
      this.strPresentEnd = true;
      }
    if(selectedAddress){
      let locationId = selectedAddress["originalObject"]["label"]
      this.plotEndPoint(locationId)
    }

  }

  viaRoutesList = [];
  onViaSelected(selectedAddress: CompleterItem){
    
    if(selectedAddress){
      let locationId = selectedAddress["originalObject"]["label"]
      this.viaRoutesList.push(locationId)
      this.plotViaPoint(this.viaRoutesList)
    }

  }

  remove(route: string): void {
    const index = this.viaRoutesList.indexOf(route);

    if (index >= 0) {
      this.viaRoutesList.splice(index, 1);
    }
    this.plotViaPoint(this.viaRoutesList)
  }
  resetToEditData(){
    this.searchStrError = false;
    this.searchEndStrError = false;
    this.setCorridorData();
  }


  // ------------- Map Functions ------------------------//
  
  createHomeMarker(){
    const homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#0D7EE7"/>
    <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
    <path fill-rule="evenodd" clip-rule="evenodd" d="M7.75 13.3394H5.5L13 6.58936L20.5 13.3394H18.25V19.3394H13.75V14.8394H12.25V19.3394H7.75V13.3394ZM16.75 11.9819L13 8.60687L9.25 11.9819V17.8394H10.75V13.3394H15.25V17.8394H16.75V11.9819Z" fill="#436DDC"/>
    </svg>`
    return homeMarker;
  }

  createEndMarker(){
    const endMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#D50017"/>
    <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
    <path d="M13 18.9644C16.3137 18.9644 19 16.5019 19 13.4644C19 10.4268 16.3137 7.96436 13 7.96436C9.68629 7.96436 7 10.4268 7 13.4644C7 16.5019 9.68629 18.9644 13 18.9644Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    </svg>`
    return endMarker;
  }

  createViaMarker(){
    const viaMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.6665C18.6665 24.9998 24.3332 19.2591 24.3332 12.9998C24.3332 6.74061 19.2591 1.6665 12.9998 1.6665C6.74061 1.6665 1.6665 6.74061 1.6665 12.9998C1.6665 19.2591 7.6665 25.3332 12.9998 29.6665Z" fill="#0D7EE7"/>
    <path d="M13 22.6665C18.5228 22.6665 23 18.4132 23 13.1665C23 7.9198 18.5228 3.6665 13 3.6665C7.47715 3.6665 3 7.9198 3 13.1665C3 18.4132 7.47715 22.6665 13 22.6665Z" fill="white"/>
    <path d="M19.7616 12.6263L14.0759 6.94057C13.9169 6.78162 13.7085 6.70215 13.5 6.70215C13.2915 6.70215 13.0831 6.78162 12.9241 6.94057L7.23842 12.6263C6.92053 12.9444 6.92053 13.4599 7.23842 13.778L12.9241 19.4637C13.0831 19.6227 13.2915 19.7021 13.5 19.7021C13.7085 19.7021 13.9169 19.6227 14.0759 19.4637L19.7616 13.778C20.0795 13.4599 20.0795 12.9444 19.7616 12.6263ZM13.5 18.3158L8.38633 13.2021L13.5 8.08848L18.6137 13.2021L13.5 18.3158ZM11.0625 12.999V15.0303C11.0625 15.1425 11.1534 15.2334 11.2656 15.2334H12.0781C12.1904 15.2334 12.2812 15.1425 12.2812 15.0303V13.4053H14.3125V14.7695C14.3125 14.8914 14.4123 14.9731 14.5169 14.9731C14.5644 14.9731 14.6129 14.9564 14.6535 14.9188L16.7916 12.9452C16.8787 12.8647 16.8787 12.7271 16.7916 12.6466L14.6535 10.673C14.6129 10.6357 14.5644 10.6187 14.5169 10.6187C14.4123 10.6187 14.3125 10.7004 14.3125 10.8223V12.1865H11.875C11.4263 12.1865 11.0625 12.5504 11.0625 12.999Z" fill="#0D7EE7"/>
    </svg>`

    return viaMarker;
  }

  plotStartPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.hereService.getLocationDetails(geocodingParameters).then((result) => {
      this.startAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.startAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createHomeMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } }); 
      this.startMarker = new H.map.Marker({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong},{icon:icon});
      this.mapGroup.addObject(this.startMarker)
      this.hereMap.addObject(this.mapGroup);
      this.hereMap.getViewModel().setLookAtData({bounds: this.mapGroup.getBoundingBox()});

      //this.hereMap.setCenter({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong}, 'default');
      this.checkRoutePlot();

    });
  }

  plotEndPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.hereService.getLocationDetails(geocodingParameters).then((result) => {
      this.endAddressPositionLat  = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.endAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createEndMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      this.endMarker = new H.map.Marker({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong},{icon:icon});
      this.mapGroup.addObject(this.endMarker)
      this.hereMap.addObject(this.mapGroup);
      this.hereMap.getViewModel().setLookAtData({bounds: this.mapGroup.getBoundingBox()});

    //  this.hereMap.addObject(this.endMarker);
    //  this.hereMap.setCenter({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong}, 'default');
      this.checkRoutePlot();

    });
    
  }

  plotViaPoint(_viaRouteList){
    this.viaRoutePlottedPoints = [];
    if(this.viaMarker){
      this.hereMap.removeObjects([this.viaMarker]);
      this.viaMarker = null;
    }
    if(_viaRouteList.length >0){
      for(var i in _viaRouteList){

        let geocodingParameters = {
          searchText: _viaRouteList[i],
        };
        this.hereService.getLocationDetails(geocodingParameters).then((result) => {
          this.viaAddressPositionLat  = result[0]["Location"]["DisplayPosition"]["Latitude"];
          this.viaAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
          let viaMarker = this.createViaMarker();
          let markerSize = { w: 26, h: 32 };
          const icon = new H.map.Icon(viaMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      
          this.viaMarker = new H.map.Marker({lat:this.viaAddressPositionLat, lng:this.viaAddressPositionLong},{icon:icon});
          this.hereMap.addObject(this.viaMarker);
         // this.hereMap.getViewModel().setLookAtData({bounds: this.endMarker.getBoundingBox()});
          //this.hereMap.setZoom(8);
          this.hereMap.setCenter({lat:this.viaAddressPositionLat, lng:this.viaAddressPositionLong}, 'default');
          this.viaRoutePlottedPoints.push({
            "viaRoutName": _viaRouteList[i],
            "latitude": this.viaAddressPositionLat,
            "longitude": this.viaAddressPositionLat
          });
        this.checkRoutePlot();
    
        });
      }  
      
    }
    else{
      this.checkRoutePlot();

    }
    

    
  }

  suggestionData :  any;
  dataService : any;
  private configureAutoCompleteForLocationSearch() {
    let searchParam = this.searchEndStr !== null ? this.searchEndStr : this.searchStr != null ? this.searchStr : this.searchViaStr;
    let AUTOCOMPLETION_URL = 'https://autocomplete.geocoder.cit.api.here.com/6.2/suggest.json' + '?' +
    '&maxresults=5' +  // The upper limit the for number of suggestions to be included in the response.  Default is set to 5.
    '&app_id=' + this.map_id + // TODO: Store this configuration in Config File.
    '&app_code=' + this.map_code +  // TODO: Store this configuration in Config File.
    '&query='+searchParam; 
    this.suggestionData = this.completerService.remote(
      AUTOCOMPLETION_URL,
      "label",
      "label");
    this.suggestionData.dataField("suggestions");
    this.dataService = this.suggestionData;
    console.log(this.dataService);
  }

  private configureAutoSuggest(){
    let searchParam = this.searchEndStr !== null ? this.searchEndStr : this.searchStr != null ? this.searchStr : this.searchViaStr;
    let URL = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json'+'?'+ '&apiKey='+this.map_key+'&limit=5'+'&query='+searchParam ;
    this.suggestionData = this.completerService.remote(
      URL,
      "label",
      "label");
    this.suggestionData.dataField("suggestions");
    this.dataService = this.suggestionData;
    console.log(this.dataService);
  }


  updateWidth(){
    let drawWidth = this.corridorWidthKm * 10;
    let allObj = this.mapGroup.getObjects()
    console.log(allObj)
    
  }

  /////////////////////////// v8 calculate ////////////////////
  routePoints:any;
  calculateTruckRoute(){
    let lineWidth = this.corridorWidthKm;
    console.log(this.viaRoutePlottedPoints)
    let routeRequestParams = 
    'origin='+`${this.startAddressPositionLat},${this.startAddressPositionLong}`+
    '&destination='+ `${this.endAddressPositionLat},${this.endAddressPositionLong}`+
    '&return=polyline,summary,travelSummary'+
    '&routingMode=fast'+
    '&transportMode=truck'+
    '&apikey='+this.map_key

    if(this.viaRoutePlottedPoints.length>0){
      this.viaRoutePlottedPoints.forEach(element => {
      routeRequestParams += '&via='+ `${element["latitude"]},${element["longitude"]}`
      });
    }

    if(this.selectedTrailerId){
      routeRequestParams += '&truck[trailerCount]='+ this.selectedTrailerId;
    }
    if(this.tunnelId){
      routeRequestParams += '&truck[tunnelCategory]='+ this.tunnelId;
    }
    if(this.corridorFormGroup.controls.vehicleHeight.value){
      routeRequestParams += '&truck[height]='+ this.corridorFormGroup.controls.vehicleHeight.value;
    }
    if(this.corridorFormGroup.controls.vehicleWidth.value){
      routeRequestParams += '&truck[width]='+ this.corridorFormGroup.controls.vehicleWidth.value;
    }
    if(this.corridorFormGroup.controls.vehicleLength.value){
      routeRequestParams += '&truck[length]='+ this.corridorFormGroup.controls.vehicleLength.value;
    }
    if(this.corridorFormGroup.controls.limitedWeight.value){
      routeRequestParams += '&truck[grossWeight]='+ this.corridorFormGroup.controls.limitedWeight.value;
    }
    if(this.corridorFormGroup.controls.weightPerAxle.value){
      routeRequestParams += '&truck[weightPerAxle]='+ this.corridorFormGroup.controls.weightPerAxle.value;
    }

    this.routePoints= [];
    this.hereService.getTruckRoutes(routeRequestParams).subscribe((data)=>{
        this.routePoints = data.routes[0];
        this.addTruckRouteShapeToMap(lineWidth)
    })

  }

  addTruckRouteShapeToMap(lineWidth?){
    let pathWidth= this.corridorWidthKm * 10;
    this.routePoints.sections.forEach((section) => {
      // decode LineString from the flexible polyline
      this.routeDistance = section.travelSummary.length;
      let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);
  
       // Create a corridor width to display the route:
       let corridorPath = new H.map.Polyline(linestring, {
        style:  {
          lineWidth: pathWidth,
          strokeColor: '#b5c7ef'
        }
      });
      // Create a polyline to display the route:
      let polylinePath = new H.map.Polyline(linestring, {
        style:  {
          lineWidth: 3,
          strokeColor: '#436ddc'
        }
      });
  
      // Add the polyline to the map
      this.mapGroup.addObjects([corridorPath,polylinePath]);
      this.hereMap.addObject(this.mapGroup);
      // And zoom to its bounding rectangle
      this.hereMap.getViewModel().setLookAtData({
         bounds: this.mapGroup.getBoundingBox()
      });
    });
  }






  //////////////////////////////////////////////////////////////
}
