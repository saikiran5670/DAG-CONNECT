import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

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

  @ViewChild("map")
  public mapElement: ElementRef;

  public constructor() {
    this.query = "starbucks";
      this.platform = new H.service.Platform({
          "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
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
    let map = new H.Map(
        this.mapElement.nativeElement,
        defaultLayers.vector.normal.map,
        {
            zoom: 10,
            // center: { lat: 37.7397, lng: -121.4252 }
            center: { lat : this.lat, lng: this.lng }
        }
    );
    this.search = new H.places.Search(this.platform.getPlacesService());
    // this.search = new H.places.Search(this.platform.getPlacesService()); 
    let behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
    this.ui = H.ui.UI.createDefault(this.map, defaultLayers);
}

public places(query: string) {
  this.map.removeObjects(this.map.getObjects());
  this.search.request({ "q": query, "at": this.lat + "," + this.lng }, {}, data => {
      for(let i = 0; i < data.results.items.length; i++) {
          this.dropMarker({ "lat": data.results.items[i].position[0], "lng": data.results.items[i].position[1] }, data.results.items[i]);
      }
  }, error => {
      console.error(error);
  });
}

private dropMarker(coordinates: any, data: any) {
  let marker = new H.map.Marker(coordinates);
  marker.setData("<p>" + data.title + "<br>" + data.vicinity + "</p>");
  marker.addEventListener('tap', event => {
      let bubble =  new H.ui.InfoBubble(event.target.getPosition(), {
          content: event.target.getData()
      });
      this.ui.addBubble(bubble);
  }, false);
  this.map.addObject(marker);
}


}
