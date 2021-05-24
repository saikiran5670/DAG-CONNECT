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

  tollRoadId = 'D';
  motorWayId ='D';
  railFerriesId = 'D';
  tunnelId ='D';
  dirtRoadId = 'D';
  boatFerriesId = 'D';
  

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
  constructor(private here: HereService,private formBuilder: FormBuilder, private corridorService : CorridorService,
    private completerService: CompleterService, private config: ConfigService) {
     this.map_key =  config.getSettings("hereMap").api_key;
     this.map_id =  config.getSettings("hereMap").app_id;
     this.map_code =  config.getSettings("hereMap").app_code;


    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });
    this.configureAutoCompleteForLocationSearch();
   }

  ngOnInit(){
    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.corridorFormGroup = this.formBuilder.group({
      corridorType:['Regular'],
      label: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      startaddress: ['', [Validators.required]],
      endaddress:  ['', [Validators.required]],
      widthInput : ['', [Validators.required]],
      viaroute1: ['', [Validators.required]],
      viaroute2: ['', [Validators.required]],
      trailer:["Regular"],
      tollRoad:['Regular'],
      motorWay:['Regular'],
      boatFerries:['Regular'],
      railFerries:['Regular'],
      tunnels:['Regular'],
      dirtRoad:['Regular'],
      vehicleHeight:['', [Validators.required]],
      vehicleWidth: ['', [Validators.required]],
      vehicleLength : ['', [Validators.required]],
      limitedWeight: ['', [Validators.required]],
      weightPerAxle: ['', [Validators.required]]

    });
    this.initiateDropDownValues();
    if((this.actionType === 'edit' || this.actionType === 'view') && this.selectedElementData){
      this.setCorridorData();
    }
    this.subscribeWidthValue()
   
    //this.configureAutoCompleteForLocationSearch();
  }

  subscribeWidthValue(){
    this.corridorFormGroup.get("widthInput").valueChanges.subscribe(x => {
      console.log(x)
      this.corridorWidthKm = Number(x);
      this.corridorWidth = this.corridorWidthKm  * 1000;
      this.calculateAB();
   })
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
      this.corridorWidth = _selectedElementData.width;
      this.corridorWidthKm = this.corridorWidth / 1000;
    }
  }
  vehicleHeightValue: number = 0;
  vehicleWidthValue: number = 0;
  vehicleLengthValue: number = 0;
  vehicleLimitedWtValue: number = 0;
  vehicleWtPerAxleValue: number = 0;
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

  initMap(){
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

  sliderChanged(){
     // this.corridorWidth = _event.value;
      this.corridorWidthKm = this.corridorWidth / 1000;
      this.corridorFormGroup.controls.widthInput.setValue(this.corridorWidthKm);
      this.checkRoutePlot();
      //this.calculateRouteFromAtoB();
  }

  checkRoutePlot(){
    if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0 && this.corridorWidth != 0){
      this.calculateAB();
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
    console.log(this.exclusionList);
    console.log(_event)
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
  }

  createCorridorClicked(){
   
    var corridorObj = {
      "id": this.corridorId ? this.corridorId : 0,
      "organizationId": this.organizationId,
      "corridorType": "R",
      "corridorLabel":this.corridorFormGroup.controls.label.value,
      "startAddress": this.corridorFormGroup.controls.startaddress.value,
      "startLatitude": this.startAddressPositionLat,
      "startLongitude": this.startAddressPositionLong,
      "endAddress": this.corridorFormGroup.controls.endaddress.value,
      "endLatitude": this.endAddressPositionLat,
      "endLongitude": this.endAddressPositionLong,
      "width": this.corridorWidth,
      "viaAddressDetails": [],
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
        
    this.tollRoadId = 'D';
    this.motorWayId ='D';
    this.railFerriesId = 'D';
    this.tunnelId ='D';
    this.dirtRoadId = 'D';
    this.boatFerriesId = 'D';
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
    if(this.startMarker && this.endMarker ){
    this.hereMap.removeObjects([this.startMarker,this.endMarker,this.routeOutlineMarker,this.routeCorridorMarker]);

    }

  }

  onStartFocus(){
    this.searchStr = null;
    this.startAddressPositionLat = 0;
    if(this.startMarker){
      this.hereMap.removeObjects([this.startMarker,this.routeOutlineMarker,this.routeCorridorMarker]);
    }
  }
  onEndFocus(){
    this.searchEndStr = null;
    this.endAddressPositionLat = 0;
    if(this.endMarker){
      this.hereMap.removeObjects([this.endMarker,this.routeOutlineMarker,this.routeCorridorMarker]);
    }
  }

  onSelected(selectedAddress: CompleterItem){
    //console.log(item.title)
    if(selectedAddress){
      let postalCode = selectedAddress["originalObject"]["label"];
      this.plotStartPoint(postalCode)
    }

  }

  onEndSelected(selectedAddress: CompleterItem){
    if(selectedAddress){
      let locationId = selectedAddress["originalObject"]["label"]
      this.plotEndPoint(locationId)
    }

  }

  resetToEditData(){
    this.setCorridorData();
  }


  // ------------- Map Functions ------------------------//
  plotStartPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.here.getLocationDetails(geocodingParameters).then((result) => {
      this.startAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.startAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createHomeMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
  
      this.startMarker = new H.map.Marker({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong},{icon:icon});
      this.hereMap.addObject(this.startMarker);
      //this.hereMap.getViewModel().setLookAtData({bounds: this.startMarker.getBoundingBox()});
      this.hereMap.setZoom(2);
      this.hereMap.setCenter({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong}, 'default');
      this.checkRoutePlot();

    });
  }

  plotEndPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.here.getLocationDetails(geocodingParameters).then((result) => {
      this.endAddressPositionLat  = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.endAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createEndMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
  
      this.endMarker = new H.map.Marker({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong},{icon:icon});
      this.hereMap.addObject(this.endMarker);
     // this.hereMap.getViewModel().setLookAtData({bounds: this.endMarker.getBoundingBox()});
      //this.hereMap.setZoom(2);
      this.hereMap.setCenter({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong}, 'default');
      this.checkRoutePlot();

    });
    
  }

  suggestionData :  any;
  dataService : any;
  private configureAutoCompleteForLocationSearch() {
    let searchParam = this.searchEndStr !== null ? this.searchEndStr : this.searchStr;
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

  calculateAB(){
    let routeRequestParams = {
      'routingMode': 'fast',
      'transportMode': 'truck',
      'origin': `${this.startAddressPositionLat},${this.startAddressPositionLong}`, 
      'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`, 
      'return': 'polyline'
    };
    this.here.calculateRoutePoints(routeRequestParams).then((data)=>{
      
       this.addRouteShapeToMap(data);
      console.log(data)
    },(error)=>{
       console.error(error);
    })
  }

  addRouteShapeToMap(result){
    var group = new H.map.Group();
    if(this.routeOutlineMarker){
      this.hereMap.removeObjects([this.routeOutlineMarker, this.routeCorridorMarker]);

    }
    result.routes[0].sections.forEach((section) =>{
      let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);

      // Create a polyline to display the route:
      // let routeLine = new H.map.Polyline(linestring, {
      //   style: { strokeColor: '#436ddc', lineWidth: 3 } //b5c7ef
      // });
      // this.hereMap.addObject(routeLine);
      // this.hereMap.getViewModel().setLookAtData({bounds: routeLine.getBoundingBox()});
      if (this.corridorWidthKm > 0) {
        this.routeOutlineMarker = new H.map.Polyline(linestring, {
          style: {
            lineWidth: this.corridorWidthKm,
            strokeColor: '#b5c7ef',
          }
        });
        // Create a patterned polyline:
        this.routeCorridorMarker = new H.map.Polyline(linestring, {
          style: {
            lineWidth: 3,
            strokeColor: '#436ddc'
          }
        }
        );
        // create a group that represents the route line and contains
        // outline and the pattern
        var routeLine = new H.map.Group();
        // routeLine.addObjects([routeOutline, routeArrows]);
        this.hereMap.addObjects([this.routeOutlineMarker, this.routeCorridorMarker]);
        this.hereMap.getViewModel().setLookAtData({ bounds: this.routeCorridorMarker.getBoundingBox() });

      }
      else{
        this.routeOutlineMarker = null;
        this.routeCorridorMarker = null;

      }

    });
  
    // // Add the polyline to the map
    // this.map.addObject(group);
    // // And zoom to its bounding rectangle
    // this.map.getViewModel().setLookAtData({
    //   bounds: group.getBoundingBox()
    // });
  }
  
}
