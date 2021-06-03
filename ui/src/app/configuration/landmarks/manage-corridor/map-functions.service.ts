import { Injectable,Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { HereService } from 'src/app/services/here.service';
import { CorridorService } from 'src/app/services/corridor.service';

declare var H: any;

@Injectable({
  providedIn: 'root'
})
export class MapFunctionsService {

  platform: any;
  map: any;
  hereMap: any;
  public mapElement: ElementRef;
  mapGroup ;
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
  additionalData = [];
  map_key = "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw";
  constructor(private hereService : HereService, private corridorService: CorridorService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

   
  
  getAttributeData = [];
  getExclusionList = [];
  hazardousMaterial = [];
  tunnelId = undefined;
  selectedTrailerId = undefined;
  trafficFlowChecked = false;
  transportDataChecked = false;
  vehicleHeightValue = 0
  vehicleWidthValue = 0
  vehicleLengthValue =0
  vehicleLimitedWtValue = 0
  vehicleWtPerAxleValue =0

  
  // public ngAfterViewInit() {
  //   this.initMap();
  // }

  initMap(mapElement){
    let defaultLayers = this.platform.createDefaultLayers();
    //Step 2: initialize a map - this map is centered over Europe
    this.hereMap = new H.Map(mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
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
    this.ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
    var group = new H.map.Group();
    this.mapGroup = group;
  }

  clearRoutesFromMap(){
    var group = new H.map.Group();
    group.removeAll();
    this.hereMap.removeObjects(this.hereMap.getObjects())
    this.startMarker = null; this.endMarker = null;
  }
  
  group = new H.map.Group();

  viaRoutePlottedPoints = [];

  viewSelectedRoutes(_selectedRoutes,accountOrganizationId?){
    var group = new H.map.Group();
    group.removeAll();
    this.hereMap.removeObjects(this.hereMap.getObjects())
    // if(this.routeOutlineMarker){
    //   this.hereMap.removeObjects([this.routeOutlineMarker, this.routeCorridorMarker]);
    //   this.routeOutlineMarker = null;
    // }
    if(_selectedRoutes){
      for(var i in _selectedRoutes){
        this.startAddressPositionLat = _selectedRoutes[i].startLat;
        this.startAddressPositionLong = _selectedRoutes[i].startLong;
        this.endAddressPositionLat= _selectedRoutes[i].endLat;
        this.endAddressPositionLong= _selectedRoutes[i].endLong;
        this.corridorWidth = _selectedRoutes[i].width;
        this.corridorWidthKm = this.corridorWidth/1000;
        let corridorName = _selectedRoutes[i].corridoreName;
        let startAddress = _selectedRoutes[i].startPoint;
        let endAddress = _selectedRoutes[i].endPoint;




        let houseMarker = this.createHomeMarker();
        let markerSize = { w: 26, h: 32 };
        const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.startMarker = new H.map.Marker({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong},{icon:icon});

        let endMarker = this.createEndMarker();
        const iconEnd = new H.map.Icon(endMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
  
        this.endMarker = new H.map.Marker({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong},{icon:iconEnd});
        let endMarkerHtml = `<div style="font-size:11px;font-family:Times New Roman">
        <table>
        <tr><td><b>Corridor Name:</b></td> <td>${corridorName} </td></tr>
        <tr><td><b>Start Point:</b></td><td>${startAddress}</td></tr>
        <tr><td><b>End Point:</b></td><td>${endAddress}</td></tr>
        <tr><td><b>Width:</b></td><td>${this.corridorWidthKm} km</td></tr>
        </table>
        </div>`
        this.endMarker.setData(endMarkerHtml);
       
        this.group.addObjects([this.startMarker,this.endMarker]);
        this.hereMap.addObject(this.group)
        if(accountOrganizationId){
          if(_selectedRoutes[i].id){
            this.corridorService.getCorridorFullList(accountOrganizationId,_selectedRoutes[i].id).subscribe((data)=>{
                console.log(data)
                if(data[0]["corridorProperties"]){
                   this.additionalData =  data[0]["corridorProperties"];
                   this.setAdditionalData();
                   if( data[0].viaAddressDetail.length > 0){
                    this.viaRoutePlottedPoints =  data[0].viaAddressDetail;
                    //this.plotViaStopPoints();
                  }
                this.calculateTruckRoute();

                }
            })
          }
        }
        else{
        this.calculateTruckRoute();

        }
        this.addInfoBubble(group);
        
       // this.hereMap.getViewModel().setLookAtData({ bounds: group.getBoundingBox()});
       // let successRoute = this.calculateAB('view');
      }
    }
  }

  setAdditionalData(){
    let _data = this.additionalData;
    this.getAttributeData = _data["attribute"];
    this.getExclusionList = _data["exclusion"];
    this.getAttributeData["isCombustible"] ? this.hazardousMaterial.push('combustible'):'';
    this.getAttributeData["isCorrosive"] ? this.hazardousMaterial.push('corrosive'):'';
    this.getAttributeData["isExplosive"] ? this.hazardousMaterial.push('explosive'):'';
    this.getAttributeData["isFlammable"] ? this.hazardousMaterial.push('flammable'):'';
    this.getAttributeData["isGas"] ? this.hazardousMaterial.push('gas'):'';
    this.getAttributeData["isOrganic"] ? this.hazardousMaterial.push('organic'):'';
    this.getAttributeData["isOther"]? this.hazardousMaterial.push('other'):'';
    this.getAttributeData["isPoision"] ? this.hazardousMaterial.push('poison'):'';
    this.getAttributeData["isPoisonousInhalation"] ? this.hazardousMaterial.push('poisonousInhalation'):'';
    this.getAttributeData["isRadioActive"] ? this.hazardousMaterial.push('radioactive'):'';
    this.getAttributeData["isWaterHarm"]? this.hazardousMaterial.push('harmfulToWater'):'';

    
    this.selectedTrailerId = this.getAttributeData["noOfTrailers"];
    this.trafficFlowChecked = _data["isTrafficFlow"];
    this.transportDataChecked = _data["isTransportData"];
    this.vehicleHeightValue = _data["vehicleSize"].vehicleHeight;
    this.vehicleWidthValue = _data["vehicleSize"].vehicleWidth;
    this.vehicleLengthValue = _data["vehicleSize"].vehicleLength;
    this.vehicleLimitedWtValue = _data["vehicleSize"].vehicleLimitedWeight;
    this.vehicleWtPerAxleValue =_data["vehicleSize"].vehicleWeightPerAxle;

    this.tunnelId = this.getExclusionList["tunnelsType"];

    // this.tollRoadId = this.getExclusionList["tollRoadType"];
    // this.boatFerriesId = this.getExclusionList["boatFerriesType"];
    // this.dirtRoadId = this.getExclusionList["dirtRoadType"];
    // this.motorWayId = this.getExclusionList["mortorway"];
    // this.railFerriesId = this.getExclusionList["railFerriesType"];

  }

  plotStartPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.hereService.getLocationDetails(geocodingParameters).then((result) => {
      this.startAddressPositionLat = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.startAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createHomeMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
  
      this.startMarker = new H.map.Marker({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong},{icon:icon});
      var group = new H.map.Group();
      this.hereMap.addObject(this.startMarker);
      //this.hereMap.getViewModel().setLookAtData({zoom: 8});
      //this.hereMap.setZoom(8);
      this.hereMap.setCenter({lat:this.startAddressPositionLat, lng:this.startAddressPositionLong}, 'default');
      this.checkRoutePlot();

    });
  }

  checkRoutePlot(){
    if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0 && this.corridorWidth != 0){
      this.calculateTruckRoute();
    }
  }

  plotEndPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.hereService.getLocationDetails(geocodingParameters).then((result) => {
      this.endAddressPositionLat  = result[0]["Location"]["DisplayPosition"]["Latitude"];
      this.endAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
      let houseMarker = this.createEndMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
  
      this.endMarker = new H.map.Marker({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong},{icon:icon});
      this.hereMap.addObject(this.endMarker);
     // this.hereMap.getViewModel().setLookAtData({bounds: this.endMarker.getBoundingBox()});
      //this.hereMap.setZoom(8);
      this.hereMap.setCenter({lat:this.endAddressPositionLat, lng:this.endAddressPositionLong}, 'default');
      this.checkRoutePlot();

    });
    
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

   /////////////////////////// v8 calculate ////////////////////
   routePoints:any;
   calculateTruckRoute(){
     let lineWidth = this.corridorWidthKm;
     let routeRequestParams = 
     'origin='+`${this.startAddressPositionLat},${this.startAddressPositionLong}`+
     '&destination='+ `${this.endAddressPositionLat},${this.endAddressPositionLong}`+
     '&return=polyline,summary,travelSummary'+
     '&routingMode=fast'+
     '&transportMode=truck'+
     '&apikey='+this.map_key
 
     if(this.viaRoutePlottedPoints.length>0){
       this.viaRoutePlottedPoints.forEach(element => {
       routeRequestParams += '&via='+ `${element["latitude"]},${element["longitude"]}`
       });
     }
 
     if(this.selectedTrailerId){
       routeRequestParams += '&truck[trailerCount]='+ this.selectedTrailerId;
     }
     if(this.tunnelId){
       routeRequestParams += '&truck[tunnelCategory]='+ this.tunnelId;
     }
     if(this.vehicleHeightValue){
       routeRequestParams += '&truck[height]='+ this.vehicleHeightValue;
     }
     if(this.vehicleWidthValue){
       routeRequestParams += '&truck[width]='+ this.vehicleWidthValue;
     }
     if(this.vehicleLengthValue){
       routeRequestParams += '&truck[length]='+ this.vehicleLengthValue;
     }
     if(this.vehicleLimitedWtValue){
       routeRequestParams += '&truck[grossWeight]='+ this.vehicleLimitedWtValue;
     }
     if(this.vehicleWtPerAxleValue){
       routeRequestParams += '&truck[weightPerAxle]='+ this.vehicleWtPerAxleValue;
     }
 
     if(this.hazardousMaterial.length > 0){
       routeRequestParams += '&truck[shippedHazardousGoods]=' + this.hazardousMaterial.join();
     }
     this.routePoints= [];
     this.hereService.getTruckRoutes(routeRequestParams).subscribe((data)=>{
       if(data && data.routes){
 
         this.routePoints = data.routes[0];
           this.addTruckRouteShapeToMap(lineWidth);
         }
       
     })
 
   }
 
   addTruckRouteShapeToMap(lineWidth?){
     let pathWidth= this.corridorWidthKm * 10;
     
     if(this.routePoints.sections){
     this.routePoints.sections.forEach((section) => {
       // decode LineString from the flexible polyline
       let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);
   
        // Create a corridor width to display the route:
        let corridorPath = new H.map.Polyline(linestring, {
         style:  {
           lineWidth: pathWidth,
           strokeColor: '#b5c7ef'
         }
       });
       // Create a polyline to display the route:
       let polylinePath = new H.map.Polyline(linestring, {
         style:  {
           lineWidth: 3,
           strokeColor: '#436ddc'
         }
       });
   
       // Add the polyline to the map
       this.mapGroup.addObjects([corridorPath,polylinePath]);
       this.hereMap.addObject(this.mapGroup);
       // And zoom to its bounding rectangle
      //  this.hereMap.getViewModel().setLookAtData({
      //     bounds: this.mapGroup.getBoundingBox()
      //  });
     });
   }
   }
   
  ui: any;
  addInfoBubble(markerGroup) {

    var group = new H.map.Group();
  
    this.hereMap.addObject(group);
  
    // add 'tap' event listener, that opens info bubble, to the group
    this.hereMap.addEventListener('tap',  (evt)=> {
      // event target is the marker itself, group is a parent event target
      // for all objects that it contains
      var bubble = new H.ui.InfoBubble(evt.target.getGeometry(), {
        // read custom data
        content: evt.target.getData()
      });
      // show info bubble
      if(evt.target.getData()){
      this.ui.addBubble(bubble);

      }
    }, false);
  }
}
