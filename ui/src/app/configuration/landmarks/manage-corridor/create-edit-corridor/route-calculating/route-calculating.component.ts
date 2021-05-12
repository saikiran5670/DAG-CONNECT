import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Form, FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../../shared/custom.validators';
import { HereService } from 'src/app/services/here.service';
declare var H: any;

@Component({
  selector: 'app-route-calculating',
  templateUrl: './route-calculating.component.html',
  styleUrls: ['./route-calculating.component.less']
})
export class RouteCalculatingComponent implements OnInit {
  @Input() translationData: any;

  @Input() actionType: any;
  @Output() backToPage = new EventEmitter<any>();
  breadcumMsg: any = '';
  corridorFormGroup: FormGroup;
  corridorTypeList = [{id:1,value:'Route Calculating'},{id:2,value:'Existing Trips'}];
  selectedCorridorTypeId : any = 1;
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
  
  constructor(private here: HereService,private formBuilder: FormBuilder) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(): void {

    this.organizationId = parseInt(localStorage.getItem("accountOrganizationId"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.corridorFormGroup =  new FormGroup({
      label: new FormControl(),
      startaddress:new FormControl(),
      endaddress: new FormControl(),
      widthInput :new FormControl(),
      viaroute1: new FormControl(),
      viaroute2: new FormControl()

  

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
      this.distanceinKM = distanceinMtr/1000;
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
  attributeCheck(_checked,type){
    switch (type) {
      case 'explosive':
        this.explosiveChecked = _checked;
        console.log(this.explosiveChecked)
        break;
    
      default:
        break;
    }
  }

}
