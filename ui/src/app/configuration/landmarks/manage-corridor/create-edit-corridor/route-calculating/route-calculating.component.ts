import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../../shared/custom.validators';
import { HereService } from 'src/app/services/here.service';
import { CorridorService } from '../../../../../services/corridor.service';
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
  @Output() backToPage = new EventEmitter<any>();
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
  localStLanguage: any;
  accountId: any = 0;
  hereMap: any;
  distanceinKM = 0;
  viaRouteCount : boolean = false;
  transportDataChecked : boolean= false;
  trafficFlowChecked : boolean = false;
  corridorWidth : number;
  sliderValue : number = 0;
  min : number = 0;
  max : number = 10000;

  constructor(private here: HereService,private formBuilder: FormBuilder, private corridorService : CorridorService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(): void {
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

  

    // add a resize listener to make sure that the map occupies the whole container
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());

    // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));


    // Create the default UI components
    var ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
  }

  addPolylineToMap(){
    var lineString = new H.geo.LineString();
    // lineString.pushPoint({lat : this.startAddressPosition.lat, lng: this.startAddressPosition.long});
    // lineString.pushPoint({lat : this.endAddressPosition.lat, lng: this.endAddressPosition.long});
    lineString.pushPoint({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong});
    lineString.pushPoint({lat:this.endAddressPositionLat , lng:this.endAddressPositionLong});
   // console.log(this.startAddressPosition,this.endAddressPosition)
    this.hereMap.addObject(new H.map.Polyline(
      lineString, { style: { lineWidth: 4 }}
    ));
  }
  sliderChanged(_event){
      let distanceinMtr = _event.value;
      this.corridorWidth = _event.value;
      this.distanceinKM = distanceinMtr/1000;
      this.corridorFormGroup.controls.widthInput.setValue(this.distanceinKM);
  }

  changeSliderInput(){
    this.distanceinKM = this.corridorFormGroup.controls.widthInput.value;
    this.sliderValue = this.distanceinKM * 1000;
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
    console.log(this.transportDataChecked)
  }

  
  trafficFlowCheckedFn(_checked){
    this.trafficFlowChecked = _checked;
  }

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

  tollRoadId = 'D';
  motorWayId ='D';
  railFerriesId = 'D';
  tunnelId ='D';
  dirtRoadId = 'D';
  boatFerriesId = 'D';
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

  startAddressPositionLat :number = 0; // = {lat : 18.50424,long : 73.85286};
  startAddressPositionLong :number = 0; // = {lat : 18.50424,long : 73.85286};

  startAddressFocusOut(){
    if (this.corridorFormGroup.controls.startaddress.value != '') {
      this.here.getAddress(this.corridorFormGroup.controls.startaddress.value).then((result) => {
        console.log(result)
        this.startAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"],
        this.startAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"]
      });
    }
  }

  endAddressPositionLat : number = 0;
  endAddressPositionLong : number = 0;

  endAddressFocusOut(){
    if (this.corridorFormGroup.controls.endaddress.value != '') {
      this.here.getAddress(this.corridorFormGroup.controls.endaddress.value).then((result) => {
        console.log(result)
        this.endAddressPositionLat  = result[0]["Location"]["DisplayPosition"]["Latitude"],
        this.endAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"]
      });
    this.addPolylineToMap();

    }
  }
  createCorridorClicked(){
   
    var corridorObj = {
      "id": 0,
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
      "created_By": 0,
      "modified_At": 0,
      "modified_By": 0,
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
      console.log(responseData);
    })
  }

  backToCorridorList(){
    let emitObj = {
      booleanFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  resetValues(){
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
    this.corridorFormGroup.controls.vehicleHeight.setValue("")
  }
}
