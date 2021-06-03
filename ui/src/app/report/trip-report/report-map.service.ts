import { Injectable,Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { HereService } from '../../services/here.service';

declare var H: any;

@Injectable({
  providedIn: 'root'
})
export class ReportMapService {
  platform: any;
  map: any;
  ui: any
  hereMap: any;
  public mapElement: ElementRef;
  mapGroup: any;
  startAddressPositionLat :number = 0; // = {lat : 18.50424,long : 73.85286};
  startAddressPositionLong :number = 0; // = {lat : 18.50424,long : 73.85286};
  startMarker : any;
  endMarker :any;
  routeCorridorMarker : any;
  routeOutlineMarker : any;
  endAddressPositionLat : number = 0;
  endAddressPositionLong : number = 0;
  corridorWidth : number = 100;
  corridorWidthKm : number = 0.1;
  group = new H.map.Group();

  constructor(private hereSerive : HereService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  initMap(mapElement: any){
    let defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      //center:{lat:41.881944, lng:-87.627778},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
    var group = new H.map.Group();
    this.mapGroup = group;
  }

  clearRoutesFromMap(){
    this.hereMap.removeObjects(this.hereMap.getObjects())
    this.group.removeAll();
    this.startMarker = null; 
    this.endMarker = null;
  }

  getUI(){
    return this.ui;
  }

  viewSelectedRoutes(_selectedRoutes: any, _ui: any){
    this.clearRoutesFromMap();
    if(_selectedRoutes){
      for(var i in _selectedRoutes){
        this.startAddressPositionLat = _selectedRoutes[i].startPositionLattitude;
        this.startAddressPositionLong = _selectedRoutes[i].startPositionLongitude;
        this.endAddressPositionLat= _selectedRoutes[i].endPositionLattitude;
        this.endAddressPositionLong= _selectedRoutes[i].endPositionLongitude;
        this.corridorWidth = 1000; //- hard coded
        this.corridorWidthKm = this.corridorWidth/1000;
        let houseMarker = this.createHomeMarker();
        let markerSize = { w: 26, h: 32 };
        const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.startMarker = new H.map.Marker({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong},{icon:icon});
        let endMarker = this.createEndMarker();
        const iconEnd = new H.map.Icon(endMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.endMarker = new H.map.Marker({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong},{icon:iconEnd});
        this.group.addObjects([this.startMarker,this.endMarker]);
        var startBubble;
        this.startMarker.addEventListener('pointerenter', function (evt) {
          // event target is the marker itself, group is a parent event target
          // for all objects that it contains
          startBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
            // read custom data
            content:`<div>
            <b>Start Location:</b> ${_selectedRoutes[i].startPosition}<br>
            <b>Start Date:</b> ${_selectedRoutes[i].convertedStartTime}<br>
            <b>Total Alerts:</b> ${_selectedRoutes[i].alert}
            </div>`
          });
          // show info bubble
          _ui.addBubble(startBubble);
        }, false);
        this.startMarker.addEventListener('pointerleave', function(evt) {
          startBubble.close();
        }, false);

        var endBubble;
        this.endMarker.addEventListener('pointerenter', function (evt) {
          // event target is the marker itself, group is a parent event target
          // for all objects that it contains
          endBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
            // read custom data
            content:`<div>
            <b>End Location:</b> ${_selectedRoutes[i].endPosition}<br>
            <b>End Date:</b> ${_selectedRoutes[i].convertedEndTime}<br>
            <b>Total Alerts:</b> ${_selectedRoutes[i].alert}
            </div>`
          });
          // show info bubble
          _ui.addBubble(endBubble);
        }, false);
        this.endMarker.addEventListener('pointerleave', function(evt) {
          endBubble.close();
        }, false);

        this.calculateAtoB();
      }
    }
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

  calculateAtoB(){
    let routeRequestParams = {
      'routingMode': 'fast',
      'transportMode': 'truck',
      'origin': `${this.startAddressPositionLat},${this.startAddressPositionLong}`, 
      'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`, 
      'return': 'polyline'
    };
    this.hereSerive.calculateRoutePoints(routeRequestParams).then((data: any)=>{
      this.addRouteShapeToMap(data);
    },(error)=>{
       console.error(error);
    })
  }

  addRouteShapeToMap(result: any){
    result.routes[0].sections.forEach((section) =>{
      let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);
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
        this.group.addObjects([this.routeOutlineMarker, this.routeCorridorMarker]);
        this.hereMap.addObject(this.group);
        this.hereMap.setCenter({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong}, 'default');
        // this.hereMap.getViewModel().setLookAtData({ bounds: this.routeCorridorMarker.getBoundingBox() });
    });
  }
  
}
