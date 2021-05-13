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
  selectedCorridorTypeId : any = 46;
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
console.log(this.exclusionList)
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

  startAddressPosition = {lat : 18.50424,long : 73.85286};
  startAddressFocusOut(){
    if (this.corridorFormGroup.controls.startaddress.value != '') {
      this.here.getAddress(this.corridorFormGroup.controls.startaddress.value).then((result) => {
        console.log(result)
        this.startAddressPosition.lat = result[0]["Location"]["DisplayPosition"]["Latitude"];
        this.startAddressPosition.long = result[0]["Location"]["DisplayPosition"]["Longitude"];
      });
      console.log(this.startAddressPosition);
    }
  }

  endAddressPosition = {lat : 18.50424,long : 73.85286};
  endAddressFocusOut(){
    if (this.corridorFormGroup.controls.endaddress.value != '') {
      this.here.getAddress(this.corridorFormGroup.controls.endaddress.value).then((result) => {
        console.log(result)
        this.endAddressPosition.lat = result[0]["Location"]["DisplayPosition"]["Latitude"];
        this.endAddressPosition.long = result[0]["Location"]["DisplayPosition"]["Longitude"];
      });
      console.log(this.endAddressPosition);

    }
  }
  createCorridorClicked(){
   
    var corridorObj = {
      "id": 0,
      "organizationId": this.organizationId,
      "corridorType": 46,
      "corridorLabel":this.corridorFormGroup.controls.label.value,
      "startAddress": this.corridorFormGroup.controls.startaddress.value,
      "startLatitude": this.startAddressPosition.lat,
      "startLongitude": this.startAddressPosition.long,
      "endAddress": this.corridorFormGroup.controls.endaddress.value,
      "endLatitude": this.endAddressPosition.lat,
      "endLongitude": this.endAddressPosition.long,
      "width": this.corridorWidth,
      "viaAddressDetails": [
        {
          "viaRoutName": "string",
          "latitude": 0,
          "longitude": 0
        }
      ],
      "transportData": this.transportDataChecked,
      "trafficFlow": this.trafficFlowChecked,
      "state": "string",
      "created_At": 0,
      "created_By": 0,
      "modified_At": 0,
      "modified_By": 0,
      "attribute": {
        "trailer": 0,
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
        "tollRoad": this.corridorFormGroup.controls.tollRoad.value,
        "mortorway": this.corridorFormGroup.controls.motorWay.value,
        "boatFerries":this.corridorFormGroup.controls.boatFerries.value,
        "railFerries": this.corridorFormGroup.controls.railFerries.value,
        "tunnels": this.corridorFormGroup.controls.tunnels.value,
        "dirtRoad":this.corridorFormGroup.controls.dirtRoad.value,
      },
      "vehicleSize": {
        "vehicleSizeHeight":this.corridorFormGroup.controls.vehicleHeight.value,
        "vehicleSizeWidth": this.corridorFormGroup.controls.vehicleWidth.value,
        "vehicleSizeLength": this.corridorFormGroup.controls.vehicleLength.value,
        "vehicleSizeLimitedWeight": this.corridorFormGroup.controls.limitedWeight.value,
        "vehicleSizeWeightPerAxle": this.corridorFormGroup.controls.weightPerAxle.value,
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
}
