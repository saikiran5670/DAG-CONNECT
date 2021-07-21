import { Injectable, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { HereService } from 'src/app/services/here.service';
import { ReportService } from 'src/app/services/report.service';

declare var H: any;

@Injectable({
  providedIn: 'root'
})
export class MapService {

    map_key = "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw";
    ui: any;
    group = new H.map.Group();
    platform: any;
    map: any;
    hereMap: any;
    disableGroup = new H.map.Group();
    public mapElement: ElementRef;
    mapGroup;
    startAddressPositionLat: number = 0; // = {lat : 18.50424,long : 73.85286};
    startAddressPositionLong: number = 0;
    startMarker: any;
    endMarker: any;
    defaultLayers : any; 
    endAddressPositionLat: number = 0;
    endAddressPositionLong: number = 0;

    constructor(private hereService: HereService, private ReportService: ReportService) {
        this.platform = new H.service.Platform({
          "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
        });
      }

      getHhMmTime(totalSeconds: any){
        let data: any = "00:00";
        let hours = Math.floor(totalSeconds / 3600);
        totalSeconds %= 3600;
        let minutes = Math.floor(totalSeconds / 60);
        let seconds = totalSeconds % 60;
        data = `${(hours >= 10) ? hours : ('0'+hours)}:${(minutes >= 10) ? minutes : ('0'+minutes)}`;
        return data;
      }
      
      viewselectedroutes(_selectedRoutes:any, _displayRouteView?: any,trackType?: any, ){

        if(_selectedRoutes && _selectedRoutes.length > 0){
          _selectedRoutes.forEach(elem => {
            this.startAddressPositionLat = elem.startpositionlattitude;
            this.startAddressPositionLong = elem.startpositionlongitude;
            this.endAddressPositionLat= elem.endpositionlattitude;
            this.endAddressPositionLong= elem.endpositionlongitude;
            let houseMarker = this.createHomeMarker();
            let markerSize = { w: 26, h: 32 };
            const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
            this.startMarker = new H.map.Marker({ lat:this.startAddressPositionLat, lng:this.startAddressPositionLong },{ icon:icon });
            let endMarker = this.createEndMarker();
            const iconEnd = new H.map.Icon(endMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
            this.endMarker = new H.map.Marker({ lat:this.endAddressPositionLat, lng:this.endAddressPositionLong },{ icon:iconEnd });
            this.group.addObjects([this.startMarker, this.endMarker]);
            
            if(elem.liveFleetPosition.length > 1){ // required 2 points atleast to draw polyline
              let liveFleetPoints: any = elem.liveFleetPosition;
              liveFleetPoints.sort((a, b) => parseInt(a.id) - parseInt(b.id)); // sorted in Asc order based on Id's 
              if(_displayRouteView == 'C'){ // classic route
                let blueColorCode: any = '#436ddc';
                this.showClassicRoute(liveFleetPoints, trackType, blueColorCode);
              }else if(_displayRouteView == 'F' || _displayRouteView == 'CO'){ // fuel consumption/CO2 emissiom route
                let filterDataPoints: any = this.getFilterDataPoints(liveFleetPoints, _displayRouteView);
                filterDataPoints.forEach((element) => {
                  this.drawPolyline(element, trackType);
              
                });
              }
            }
            this.hereMap.addObject(this.group);
          })
        }
      }
      
      getFilterDataPoints(_dataPoints: any, _displayRouteView: any){
        //-----------------------------------------------------------------//
        // Fuel Consumption	Green	 	Orange	 	Red	 
        // VehicleSerie	Min	Max	Min	Max	Min	Max
        // LF	0	100	100	500	500	infinity
        // CF	0	100	100	500	500	infinity
        // XF	0	100	100	500	500	infinity
        // XG	0	100	100	500	500	infinity
        
        // CO2	A	 	B	 	C	 	D	 	E	 	F	 
        // VehicleSerie	Min	Max	Min	Max	Min	Max	Min	Max	Min	Max	Min	Max
        // LF	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
        // CF	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
        // XF	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
        // XG	0	270	270	540	540	810	810	1080	1080	1350	1350	infinity
        //--------------------------------------------------------------------//
      
        let innerArray: any = [];
        let outerArray: any = [];
        let finalDataPoints: any = [];
        _dataPoints.forEach((element) => { 
          let elemChecker: any = 0;
          if(_displayRouteView == 'F'){ //------ fuel consumption
            elemChecker = element.fuelconsumtion;
            if(elemChecker <= 100){
              element.color = '#57A952'; // green
            }else if(elemChecker > 100 && elemChecker <= 500){ 
              element.color = '#FFA500'; // orange
            }else{ 
              element.color = '#FF010F';  // red 
            }
          }else{ //---- co2 emission
            elemChecker = element.co2Emission;
            if(elemChecker <= 270){
              element.color = '#01FE75'; // light green
            }else if(elemChecker > 270 && elemChecker <= 540){ // green
              element.color = '#57A952'; 
            }else if(elemChecker > 540 && elemChecker <= 810){ // green-brown
              element.color = '#867B3F'; 
            }else if(elemChecker > 810 && elemChecker <= 1080){ // red-brown
              element.color = '#9C6236'; 
            }else if(elemChecker > 1080 && elemChecker <= 1350){ // brown
              element.color = '#C13F28'; 
            }else{ // red
              element.color = '#FF010F'; 
            }
          }
          finalDataPoints.push(element);
        });
    
        let curColor: any = '';
        finalDataPoints.forEach((element, index) => {
          innerArray.push(element);
          if(index != 0){
            if(curColor != element.color){
              outerArray.push({dataPoints: innerArray, color: curColor});
              innerArray = [];
              curColor = element.color;
              innerArray.push(element);
            }else if(index == (finalDataPoints.length - 1)){ // last point
              outerArray.push({dataPoints: innerArray, color: curColor}); 
            }
          }else{ // 0
            curColor = element.color;
          }
        });
    
        return outerArray;
      }
    

      selectionPolylineRoute(dataPoints: any, _index: any, checkStatus?: any){
        let lineString: any = new H.geo.LineString();
        dataPoints.map((element) => {
          lineString.pushPoint({lat: element.gpsLatitude, lng: element.gpsLongitude});  
        });
    
        let _style: any = {
          lineWidth: 4, 
          strokeColor: checkStatus ? 'blue' : 'grey'
        }
        let polyline = new H.map.Polyline(
          lineString, { style: _style }
        );
        polyline.setData({id: _index});
        
        this.disableGroup.addObject(polyline);
       }

      drawPolyline(finalDatapoints: any, trackType?: any){
        var lineString = new H.geo.LineString();
        finalDatapoints.dataPoints.map((element) => {
          lineString.pushPoint({lat: element.gpsLatitude, lng: element.gpsLongitude});  
        });
      
        let _style: any = {
          lineWidth: 4, 
          strokeColor: finalDatapoints.color
        }
        if(trackType == 'dotted'){
          _style.lineDash = [2,2];
        }
        let polyline = new H.map.Polyline(
          lineString, { style: _style }
        );
        this.group.addObject(polyline);
      }
      showClassicRoute(dataPoints: any, _trackType: any, _colorCode: any){
        let lineString: any = new H.geo.LineString();
        dataPoints.map((element) => {
          lineString.pushPoint({lat: element.gpsLatitude, lng: element.gpsLongitude});  
        });
    
        let _style: any = {
          lineWidth: 4, 
          strokeColor: _colorCode
        }
        if(_trackType == 'dotted'){
          _style.lineDash = [2,2];
        }
        let polyline = new H.map.Polyline(
          lineString, { style: _style }
        );
        
        this.group.addObject(polyline);
       }
      viewSelectedRoutes(_selectedRoutes, accountOrganizationId?) {
        let corridorName = '';
        let startAddress = '';
        let endAddress = '';
        
        this.hereMap.removeLayer(this.defaultLayers.vector.normal.traffic);
        this.hereMap.removeLayer(this.defaultLayers.vector.normal.truck);

     // var group = new H.map.Group();
     this.mapGroup.removeAll();
     this.hereMap.removeObjects(this.hereMap.getObjects())
        // if(this.routeOutlineMarker){
        //   this.hereMap.removeObjects([this.routeOutlineMarker, this.routeCorridorMarker]);
        //   this.routeOutlineMarker = null;
        // }
        if (_selectedRoutes) {
          for (var i in _selectedRoutes) {
            if (accountOrganizationId) {
    
              this.startAddressPositionLat = _selectedRoutes[i].startLat;
              this.startAddressPositionLong = _selectedRoutes[i].startLong;
              this.endAddressPositionLat = _selectedRoutes[i].endLat;
              this.endAddressPositionLong = _selectedRoutes[i].endLong;
              corridorName = _selectedRoutes[i].corridoreName;
              startAddress = _selectedRoutes[i].startPoint;
              endAddress = _selectedRoutes[i].endPoint;
    
            } else {
              this.startAddressPositionLat = _selectedRoutes[i].startPositionlattitude;
              this.startAddressPositionLong = _selectedRoutes[i].startPositionLongitude;
              this.endAddressPositionLat = _selectedRoutes[i].endPositionLattitude;
              this.endAddressPositionLong = _selectedRoutes[i].endPositionLongitude;
    
              // this.startAddressPositionLat =  19.14045;
              // this.startAddressPositionLong = 72.88235;
              // this.endAddressPositionLat=  19.03261;
              // this.endAddressPositionLong= 73.02961;
           
            }
    
            //create and add start marker
            let houseMarker = this.createHomeMarker();
            let markerSize = { w: 26, h: 32 };
            const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
            this.startMarker = new H.map.Marker({ lat: this.startAddressPositionLat, lng: this.startAddressPositionLong }, { icon: icon });
            this.mapGroup.addObject(this.startMarker);
    
            //create and add end marker
            let endMarker = this.createEndMarker();
            const iconEnd = new H.map.Icon(endMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
            this.endMarker = new H.map.Marker({ lat: this.endAddressPositionLat, lng: this.endAddressPositionLong }, { icon: iconEnd });
            let endMarkerHtml = `<div style="font-size:11px;font-family:Times New Roman">
            <table>
            <tr><td><b>Corridor Name:</b></td> <td>${corridorName} </td></tr>
            <tr><td><b>Start Point:</b></td><td>${startAddress}</td></tr>
            <tr><td><b>End Point:</b></td><td>${endAddress}</td></tr>
            </table>
            </div>`
            this.endMarker.setData(endMarkerHtml);
            this.mapGroup.addObject(this.endMarker);
    
            if(accountOrganizationId){
            // add end tooltip
            let bubble;
            this.endMarker.addEventListener('pointerenter',  (evt)=> {
              // event target is the marker itself, group is a parent event target
              // for all objects that it contains
              bubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
                // read custom data
                content:`<div style="font-size:12px;font-family:Times New Roman">
                <table>
                <tr><td><b>Corridor Name:</b></td> <td>${corridorName} </td></tr>
                <tr><td><b>Start Point:</b></td><td>${startAddress}</td></tr>
                <tr><td><b>End Point:</b></td><td>${endAddress}</td></tr>
                </table>
                </div>`
              });
              // show info bubble
              this.ui.addBubble(bubble);
            }, false);
            this.endMarker.addEventListener('pointerleave', (evt)=> {
              this.ui.removeBubble(bubble);
              bubble.close();
              bubble.dispose();
            }, false);
          }
            //this.group.addObjects([this.startMarker, this.endMarker]);
   
            else {
              //this.calculateTruckRoute();
    
            }
            //this.removeBubble();
    
            // this.hereMap.getViewModel().setLookAtData({ bounds: group.getBoundingBox()});
            // let successRoute = this.calculateAB('view');
          }
         
        }
      }

      initMap(mapElement) {
        //Step 2: initialize a map - this map is centered over Europe
        this.defaultLayers  = this.platform.createDefaultLayers();
        this.hereMap = new H.Map(mapElement.nativeElement,
          this.defaultLayers.vector.normal.map, {
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
        this.ui = H.ui.UI.createDefault(this.hereMap, this.defaultLayers);
        var group = new H.map.Group();
        this.mapGroup = group;
      }

      clearRoutesFromMap() { 
        this.mapGroup.removeAll();
        this.startMarker = null; this.endMarker = null;
        this.hereMap.removeLayer(this.defaultLayers.vector.normal.traffic);
        this.hereMap.removeLayer(this.defaultLayers.vector.normal.truck);
        //this.transportOnceChecked = false;
       // this.trafficOnceChecked = false;
        this.ui.getBubbles().forEach(bub =>this.ui.removeBubble(bub));
      }
    plotStartPoint(_locationId) {
        let geocodingParameters = {
          searchText: _locationId,
        };
        this.hereService.getLocationDetails(geocodingParameters).then((result) => {
          this.startAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
          this.startAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
          let houseMarker = this.createHomeMarker();
          let markerSize = { w: 26, h: 32 };
          const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
    
          this.startMarker = new H.map.Marker({ lat: this.startAddressPositionLat, lng: this.startAddressPositionLong }, { icon: icon });
          var group = new H.map.Group();
          this.hereMap.addObject(this.startMarker);
          //this.hereMap.getViewModel().setLookAtData({zoom: 8});
          //this.hereMap.setZoom(8);
          this.hereMap.setCenter({ lat: this.startAddressPositionLat, lng: this.startAddressPositionLong }, 'default');
          this.checkRoutePlot();
    
        });
      }

      checkRoutePlot() {
        if (this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0) {
          //this.calculateTruckRoute();
        }
      }
    
      plotEndPoint(_locationId) {
        let geocodingParameters = {
          searchText: _locationId,
        };
        this.hereService.getLocationDetails(geocodingParameters).then((result) => {
          this.endAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
          this.endAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
          let houseMarker = this.createEndMarker();
          let markerSize = { w: 26, h: 32 };
          const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
    
          this.endMarker = new H.map.Marker({ lat: this.endAddressPositionLat, lng: this.endAddressPositionLong }, { icon: icon });
          this.hereMap.addObject(this.endMarker);
          // this.hereMap.getViewModel().setLookAtData({bounds: this.endMarker.getBoundingBox()});
          //this.hereMap.setZoom(8);
          this.hereMap.setCenter({ lat: this.endAddressPositionLat, lng: this.endAddressPositionLong }, 'default');
          this.checkRoutePlot();
    
        });
    
      }

      createHomeMarker() {
        const homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#0D7EE7"/>
        <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
        <path fill-rule="evenodd" clip-rule="evenodd" d="M7.75 13.3394H5.5L13 6.58936L20.5 13.3394H18.25V19.3394H13.75V14.8394H12.25V19.3394H7.75V13.3394ZM16.75 11.9819L13 8.60687L9.25 11.9819V17.8394H10.75V13.3394H15.25V17.8394H16.75V11.9819Z" fill="#436DDC"/>
        </svg>`
        return homeMarker;
      }
    
      createEndMarker() {
        const endMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M25 13.2979C25 22.6312 13 30.6312 13 30.6312C13 30.6312 1 22.6312 1 13.2979C1 10.1153 2.26428 7.06301 4.51472 4.81257C6.76516 2.56213 9.8174 1.29785 13 1.29785C16.1826 1.29785 19.2348 2.56213 21.4853 4.81257C23.7357 7.06301 25 10.1153 25 13.2979Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M12.9998 29.9644C18.6665 25.2977 24.3332 19.5569 24.3332 13.2977C24.3332 7.03846 19.2591 1.96436 12.9998 1.96436C6.74061 1.96436 1.6665 7.03846 1.6665 13.2977C1.6665 19.5569 7.6665 25.631 12.9998 29.9644Z" fill="#D50017"/>
        <path d="M13 22.9644C18.5228 22.9644 23 18.7111 23 13.4644C23 8.21765 18.5228 3.96436 13 3.96436C7.47715 3.96436 3 8.21765 3 13.4644C3 18.7111 7.47715 22.9644 13 22.9644Z" fill="white"/>
        <path d="M13 18.9644C16.3137 18.9644 19 16.5019 19 13.4644C19 10.4268 16.3137 7.96436 13 7.96436C9.68629 7.96436 7 10.4268 7 13.4644C7 16.5019 9.68629 18.9644 13 18.9644Z" stroke="#D50017" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>`
        return endMarker;
      }
}