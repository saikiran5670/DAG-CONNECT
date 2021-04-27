import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { Form, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../../../shared/custom.validators';
import { HereService } from 'src/app/services/here.service';
import { FormBuilder } from '@angular/forms';
declare var H: any;

@Component({
  selector: 'app-create-edit-view-geofence',
  templateUrl: './create-edit-view-geofence.component.html',
  styleUrls: ['./create-edit-view-geofence.component.less']
})
export class CreateEditViewGeofenceComponent implements OnInit {
  
  @Input() translationData: any;
  map: any; 
  @ViewChild("map")
  public mapElement: ElementRef;
  breadcumMsg: any = ''; 
  // @Input() actionType: any;
  // poiFormGroup: FormGroup;
  // form: Form;
  title = 'here-project';
  private platform: any;
  private search: any;
//  map: any; 
 private ui: any; 
 lat: any = '37.7397';  
 lng: any = '-121.4252'; 
 query : any;
 public geocoder: any;
 public position: string;
 public locations: Array<any>;
poiFlag:boolean = true;
  
  constructor(private here: HereService,) {
    this.query = "starbucks";
    this.platform = new H.service.Platform({
        "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });

   }

  ngOnInit(): void {
  }

  getBreadcum(type: any){
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home' } / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblLandmark ? this.translationData.lblLandmark : "Landmark"} / ${(type == 'view') ? (this.translationData.lblViewPOI ? this.translationData.lblViewPOI : 'View POI Details') : (type == 'edit') ? (this.translationData.lblEditPOI ? this.translationData.lblEditPOI : 'Edit POI Details') : (this.translationData.lblPOIDetails ? this.translationData.lblPOIDetails : 'Add New POI')}`;
  }

  public ngAfterViewInit() {
    let defaultLayers = this.platform.createDefaultLayers();
//Step 2: initialize a map - this map is centered over Europe
 let map = new H.Map(this.mapElement.nativeElement,
  defaultLayers.vector.normal.map,{
  center: {lat:50, lng:5},
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
                  console.log(this.locations[0].Location.Address);
                  poiFlag = false;
              }, error => {
                  console.error(error);
              });
             }
            }
         
                 
          
    });

    
}

}
