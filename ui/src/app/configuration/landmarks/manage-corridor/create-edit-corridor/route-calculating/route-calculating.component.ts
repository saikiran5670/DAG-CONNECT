import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../../shared/custom.validators';
import { HereService } from 'src/app/services/here.service';
import { CorridorService } from '../../../../../services/corridor.service';
import {
  CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData
} from 'ng2-completer';

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
  mapapikey = "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw";
  constructor(private here: HereService,private formBuilder: FormBuilder, private corridorService : CorridorService,
    private completerService: CompleterService) {
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

  private createOuterMainIcon(markerSvg){
    return `<svg width="80" height="80" viewbox="0,0,80,80" xmlns="http://www.w3.org/2000/svg">
	${markerSvg}
		</svg>`
  }
  private createDrivingMarkerSVG(embeddedVehicleIcon: string): string {
		return `<g id="svg_15">
			<g id="svg_1">
				<path stroke="#db4f60" fill="#FFFFFF" stroke-width="3" stroke-miterlimit="10" d="m6.04673,9.43231c-5.18654,5.35713 -5.04859,13.90421 0.30854,19.09075c5.35713,5.18655 13.90495,5.04785 19.09149,-0.30927l9.39111,-9.70039l-9.70039,-9.39037c-5.35638,-5.18654 -13.90421,-5.04785 -19.09075,0.30928l0,0z" id="path1978"/>
			</g>
		
			${embeddedVehicleIcon}
		
			<g id="svg_8" class="hidden">
				<g id="svg_11" stroke="null">
					<circle id="svg_12" r="6.236538" cy="8.9" cx="26.9" class="st0" stroke="null"/>
					<path id="svg_13" d="m26.9,15.8c-3.78173,0 -6.9,-3.11827 -6.9,-6.9s3.11827,-6.9 6.9,-6.9s6.9,3.11827 6.9,6.9s-3.11827,6.9 -6.9,6.9zm0,-12.47308c-3.05192,0 -5.57308,2.52116 -5.57308,5.57308c0,3.05192 2.52116,5.57308 5.57308,5.57308s5.57308,-2.52116 5.57308,-5.57308c0,-3.05192 -2.52116,-5.57308 -5.57308,-5.57308z" class="st4" stroke="null"/>
				</g>
				<path id="svg_14" d="m29.95192,10.49231l-0.59711,-0.66346c-0.39808,-0.46443 -0.66346,-0.9952 -0.66346,-1.79135l0,-0.8625c0,-0.66346 -0.46443,-1.19423 -1.06154,-1.39327c0,0 0,0 0,-0.06635c0,-0.39807 -0.33173,-0.7298 -0.72981,-0.7298c-0.39808,0 -0.72981,0.33173 -0.72981,0.7298c0,0 0,0 0,0.06635c-0.59711,0.13269 -1.06154,0.72981 -1.06154,1.39327l0,0.8625c0,0.79615 -0.26538,1.32692 -0.66346,1.79135l-0.59711,0.66346c-0.26539,0.46442 0.13269,1.06154 0.66346,1.06154l1.725,0c0,0.39807 0.33173,0.7298 0.72981,0.7298c0.39807,0 0.72981,-0.33173 0.72981,-0.7298l1.5923,0c0.53077,-0.06635 0.8625,-0.59712 0.66347,-1.06154l-0.00001,0z" class="st5" stroke="null"/>
			</g>
		</g>`;
	}

  createHomeMarker(){
    const homeMarker = `<svg width="80" height="80" viewbox="0,0,80,80" xmlns="http://www.w3.org/2000/svg">
	
    <g id="svg_15">
        <g id="svg_1" transform="rotate(90 20 20)">
          <path stroke="#417ee7" fill="#FFFFFF" stroke-width="3" stroke-miterlimit="10" d="m6.04673,9.43231c-5.18654,5.35713 -5.04859,13.90421 0.30854,19.09075c5.35713,5.18655 13.90495,5.04785 19.09149,-0.30927l9.39111,-9.70039l-9.70039,-9.39037c-5.35638,-5.18654 -13.90421,-5.04785 -19.09075,0.30928l0,0z" id="path1978"/>
        </g>
      <svg fill="#417ee7"  xmlns="http://www.w3.org/2000/svg"  viewBox="0 0 80 80" width="80px" height="80px">
      <g id="house" transform="translate(13,8)">
      <path d="M 8 1.320313 L 0.660156 8.132813 L 1.339844 8.867188 L 2 8.253906 L 2 14 L 7 14 L 7 9 L 9 9 L 9 14 L 14 14 L 14 8.253906 L 14.660156 8.867188 L 15.339844 8.132813 Z M 8 2.679688 L 13 7.328125 L 13 13 L 10 13 L 10 8 L 6 8 L 6 13 L 3 13 L 3 7.328125 Z"/>
      
      </g>
      </svg>
      </g>
      </svg>`
return homeMarker;
  }

  createEndMarker(){
    const homeMarker = `<svg width="80" height="80" viewbox="0,0,80,80" xmlns="http://www.w3.org/2000/svg">
	
    <g id="svg_15">
        <g id="svg_1" transform="rotate(90 20 20)">
          <path stroke="#e62e2d" fill="#FFFFFF" stroke-width="3" stroke-miterlimit="10" d="m6.04673,9.43231c-5.18654,5.35713 -5.04859,13.90421 0.30854,19.09075c5.35713,5.18655 13.90495,5.04785 19.09149,-0.30927l9.39111,-9.70039l-9.70039,-9.39037c-5.35638,-5.18654 -13.90421,-5.04785 -19.09075,0.30928l0,0z" id="path1978"/>
        </g>
      <svg fill="#e62e2d"  xmlns="http://www.w3.org/2000/svg"  viewBox="0 0 80 80" width="80px" height="80px">
      <g id="house" transform="translate(13,8)">
      <path d="M 8 1.320313 L 0.660156 8.132813 L 1.339844 8.867188 L 2 8.253906 L 2 14 L 7 14 L 7 9 L 9 9 L 9 14 L 14 14 L 14 8.253906 L 14.660156 8.867188 L 15.339844 8.132813 Z M 8 2.679688 L 13 7.328125 L 13 13 L 10 13 L 10 8 L 6 8 L 6 13 L 3 13 L 3 7.328125 Z"/>
      
      </g>
      </svg>
      </g>
      </svg>`
return homeMarker;
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
  startMarker : any;
  endMarker :any;
  startAddressFocusOut(){
    if (this.corridorFormGroup.controls.startaddress.value != '') {
      this.here.getAddress(this.corridorFormGroup.controls.startaddress.value).then((result) => {
        console.log(result)
        this.startAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
        this.startAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
        let houseMarker = this.createHomeMarker();
        let markerSize = { w: 80, h: 80 };
        const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
    
        this.startMarker = new H.map.Marker({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong},{icon:icon});
        this.hereMap.addObject(this.startMarker);
        this.hereMap.setZoom(8);

        this.hereMap.setCenter({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong}, 'default');
      });
    }
  }

  endAddressPositionLat : number = 0;
  endAddressPositionLong : number = 0;

  endAddressFocusOut(){
    if (this.corridorFormGroup.controls.endaddress.value != '') {
      this.here.getAddress(this.corridorFormGroup.controls.endaddress.value).then((result) => {
        console.log(result)
        this.endAddressPositionLat  = result[0]["Location"]["DisplayPosition"]["Latitude"];
        this.endAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
        let houseMarker = this.createEndMarker();
        let markerSize = { w: 80, h: 80 };
        const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
    
        this.endMarker = new H.map.Marker({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong},{icon:icon});
        this.hereMap.addObject(this.endMarker);
        this.hereMap.setZoom(8);

        this.hereMap.setCenter({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong}, 'default');

      });
   // this.addPolylineToMap();

    }
  }

  drawStartMarker(){
    
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
    
    this.configureAutoCompleteForLocationSearch(startValue);
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
    this.corridorFormGroup.controls.vehicleHeight.setValue("");
    this.corridorFormGroup.controls.vehicleLength.setValue("");
    this.corridorFormGroup.controls.vehicleWidth.setValue("");
    this.corridorFormGroup.controls.limitedWeight.setValue("");
    this.corridorFormGroup.controls.weightPerAxle.setValue("");
    this.corridorFormGroup.controls.startaddress.setValue("");
    this.corridorFormGroup.controls.endaddress.setValue("");
    this.clearMap();
  }

  clearMap(){
    this.hereMap.removeObject(this.startMarker);
    this.hereMap.removeObject(this.endMarker);

  }

  suggestionData :  any;
  dataService : any;
  private configureAutoCompleteForLocationSearch(findValue) {
    let AUTOCOMPLETION_URL = 'https://autocomplete.geocoder.cit.api.here.com/6.2/suggest.json' + '?' +
      // The upper limit the for number of suggestions to be included in the response.  Default is set to 5.
      // The search text which is the basis of the query
      '&beginHighlight=' + encodeURIComponent('<mark>') + //  Mark the beginning of the match in a token.
      '&endHighlight=' + encodeURIComponent('</mark>') + //  Mark the end of the match in a token.
      '&maxresults=5' +
      'apiKey=' + this.mapapikey
    '&query=' + encodeURIComponent(findValue);   // The search text which is the basis of the query
    this.suggestionData = this.completerService.remote(
      AUTOCOMPLETION_URL,
      "label",
      "label");
    this.suggestionData.dataField("suggestions");
    this.dataService = this.suggestionData;
    console.log(this.dataService)
    // this.dataService = this.completerService.local(this.searchData, 'color', 'color');
  }
}
