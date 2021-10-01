import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ConfigService } from '@ngx-config/core';
import { OrganizationService } from 'src/app/services/organization.service';

declare var H: any;

@Component({
  selector: 'app-common-map',
  templateUrl: './common-map.component.html',
  styleUrls: ['./common-map.component.css']
})
export class CommonMapComponent implements OnInit {
  title = 'here-project';
  private platform: any;
  private search: any;
 map: any; 
 private ui: any; 
 lat: any = '37.7397';  
 lng: any = '-121.4252'; 
 query : any;
 map_key: any = '';

  @ViewChild("map")
  public mapElement: ElementRef;

  public constructor(private organizationService: OrganizationService, private _configService: ConfigService) {
    this.query = "starbucks";
    this.map_key = _configService.getSettings("hereMap").api_key;
      this.platform = new H.service.Platform({
          "apikey": this.map_key
      });
  }

  ngOnInit(): void {
  //   this.platform = new H.service.Platform({
  //     "app_id": "N9i3iDX6Tgp0xxnrRFyD"
  //     // "app_code": this.appCode
  // });
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

//Step 3: make the map interactive
// MapEvents enables the event system
// Behavior implements default interactions for pan/zoom (also on mobile touch environments)
var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

// Create the default UI components
var ui = H.ui.UI.createDefault(map, defaultLayers);

// Now use the map as required...
// window.onload = function () {
 this.addMarkersToMap(map)
// }

this.setUpClickListener(map);

}

 setUpClickListener(map) {
  // obtain the coordinates and display in console.
  map.addEventListener('tap', function (evt) {
    var coord = map.screenToGeo(evt.currentPointer.viewportX,
            evt.currentPointer.viewportY);
            // let x = Math.abs(coord.lat.toFixed(4)) + ((coord.lat > 0) ? 'N' : 'S');
            // let y = Math.abs(coord.lng.toFixed(4)) + ((coord.lng > 0) ? 'E' : 'W')
            let x = Math.abs(coord.lat.toFixed(4));
            let y = Math.abs(coord.lng.toFixed(4));
            console.log("latitude=" +x);
            console.log("longi=" +y);
            let locations = new H.map.Marker({lat:x, lng:y});
            map.addObject(locations);

    });
}

 addMarkersToMap(map) {
    // var parisMarker = new H.map.Marker({lat:48.8567, lng:2.3508});
    // map.addObject(parisMarker);

    var romeMarker = new H.map.Marker({lat:41.9, lng: 12.5});
    map.addObject(romeMarker);

    var berlinMarker = new H.map.Marker({lat:52.5166, lng:13.3833});
    map.addObject(berlinMarker);

    var madridMarker = new H.map.Marker({lat:40.4, lng: -3.6833});
    map.addObject(madridMarker);

    var londonMarker = new H.map.Marker({lat:51.5008, lng:-0.1224});
    map.addObject(londonMarker);
}

}