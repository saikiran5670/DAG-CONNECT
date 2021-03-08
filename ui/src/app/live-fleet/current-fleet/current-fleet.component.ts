import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, NgZone } from '@angular/core';
declare var H: any;

@Component({
  selector: 'app-current-fleet',
  templateUrl: './current-fleet.component.html',
  styleUrls: ['./current-fleet.component.less']
})
export class CurrentFleetComponent implements OnInit {

  private platform: any;
  constructor(private zone: NgZone) { }
  ngOnInit() { }

  public ngAfterViewInit() {
  
    this.platform = new H.service.Platform({
      "app_id": 'devportal-demo-20180625',
      "app_code": '9v2BkviRwi9Ot26kp2IysQ'
    });

    let defaultLayers = this.platform.createDefaultLayers();
    let map = new H.Map(
      document.getElementById('here-map'),
        defaultLayers.normal.map,
        {
            zoom: 10,
            center: { lat: 18.5204, lng: 73.8567 }
        }
    );
  
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));  
    // Create the default UI components
    var ui = H.ui.UI.createDefault(map, defaultLayers);
    map.getViewPort().resize();
  }

}
