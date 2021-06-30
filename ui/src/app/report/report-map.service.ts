import { Injectable,Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { HereService } from '../services/here.service';
import { Util } from '../shared/util';
import { ConfigService } from '@ngx-config/core';

declare var H: any;

@Injectable({
  providedIn: 'root'
})
export class ReportMapService {
  platform: any;
  clusteringLayer: any;
  overlayLayer: any;
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
  map_key : string = "";
  defaultLayers : any;
  herePOISearch: any = '';
  entryPoint: any = '';

  constructor(private hereSerive : HereService, private _configService: ConfigService) {
    this.map_key =  _configService.getSettings("hereMap").api_key;
    this.platform = new H.service.Platform({
      "apikey": this.map_key // "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
    this.herePOISearch = this.platform.getPlacesService();
    this.entryPoint = H.service.PlacesService.EntryPoint;
  }

  initMap(mapElement: any){
    this.defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(mapElement.nativeElement,
      this.defaultLayers.raster.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      //center:{lat:41.881944, lng:-87.627778},
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, this.defaultLayers);
    var group = new H.map.Group();
    this.mapGroup = group;

    this.ui.removeControl("mapsettings");
    // create custom one
    var ms = new H.ui.MapSettingsControl( {
        baseLayers : [ { 
          label:"Normal", layer:this.defaultLayers.raster.normal.map
        },{
          label:"Satellite", layer:this.defaultLayers.raster.satellite.map
        }, {
          label:"Terrain", layer:this.defaultLayers.raster.terrain.map
        }
        ],
      layers : [{
            label: "Layer.Traffic", layer: this.defaultLayers.vector.normal.traffic
        },
        {
            label: "Layer.Incidents", layer: this.defaultLayers.vector.normal.trafficincidents
        }
    ]
      });
      this.ui.addControl("customized", ms);
  }

  clearRoutesFromMap(){
    this.hereMap.removeObjects(this.hereMap.getObjects())
    this.group.removeAll();
    this.startMarker = null; 
    this.endMarker = null; 
    if(this.clusteringLayer)
      this.hereMap.removeLayer(this.clusteringLayer);
    if(this.overlayLayer)
      this.hereMap.removeLayer(this.overlayLayer);
  }

  getUI(){
    return this.ui;
  }

    getHereMap() {
      return this.hereMap
    }

  getPOIS(){
    //let hexString = (35).toString(16);
    let tileProvider = new H.map.provider.ImageTileProvider({
      // We have tiles only for zoom levels 12â€“15,
      // so on all other zoom levels only base map will be visible
      min: 0,
      max: 26,
      opacity: 0.5,
      getURL: function (column, row, zoom) {
          return `https://1.base.maps.ls.hereapi.com/maptile/2.1/maptile/newest/normal.day/${zoom}/${column}/${row}/256/png8?apiKey=BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw&pois`;
        }
    });
    this.overlayLayer = new H.map.layer.TileLayer(tileProvider, {
      // Let's make it semi-transparent
      //opacity: 0.5
    });
    this.hereMap.addLayer(this.overlayLayer);
  
   // let poi = this.platform.createDefaultLayers({pois:true});
    //     let routeURL = 'https://1.base.maps.ls.hereapi.com/maptile/2.1/maptile/newest/normal.day/11/525/761/256/png8?apiKey=BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw&pois';
      
    // var normalLayer = this.platform.getMapTileService({type: 'base'}).createTileLayer(
    //   routeURL
    // );   
   // this.hereMap.addLayer(poi.raster.normal.map);

    // this.hereSerive.getHerePois().subscribe(data=>{
    //   console.log(data)
    // });
   
  }

  getCategoryPOIIcon(){
    let locMarkup = '<svg height="24" version="1.1" width="24" xmlns="http://www.w3.org/2000/svg" xmlns:cc="http://creativecommons.org/ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"><g transform="translate(0 -1028.4)"><path d="m12 0c-4.4183 2.3685e-15 -8 3.5817-8 8 0 1.421 0.3816 2.75 1.0312 3.906 0.1079 0.192 0.221 0.381 0.3438 0.563l6.625 11.531 6.625-11.531c0.102-0.151 0.19-0.311 0.281-0.469l0.063-0.094c0.649-1.156 1.031-2.485 1.031-3.906 0-4.4183-3.582-8-8-8zm0 4c2.209 0 4 1.7909 4 4 0 2.209-1.791 4-4 4-2.2091 0-4-1.791-4-4 0-2.2091 1.7909-4 4-4z" fill="#55b242" transform="translate(0 1028.4)"/><path d="m12 3c-2.7614 0-5 2.2386-5 5 0 2.761 2.2386 5 5 5 2.761 0 5-2.239 5-5 0-2.7614-2.239-5-5-5zm0 2c1.657 0 3 1.3431 3 3s-1.343 3-3 3-3-1.3431-3-3 1.343-3 3-3z" fill="#ffffff" transform="translate(0 1028.4)"/></g></svg>';
    let markerSize = { w: 26, h: 26 };
    const icon = new H.map.Icon(locMarkup, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
    return icon;
  }

  showCategoryPOI(categotyPOI: any, _ui: any){
    categotyPOI.forEach(element => {
      if(element.latitude && element.longitude){
        let categoryPOIMarker = new H.map.Marker({lat: element.latitude, lng: element.longitude},{icon: this.getCategoryPOIIcon()});
        this.group.addObject(categoryPOIMarker);
        let bubble: any;
        categoryPOIMarker.addEventListener('pointerenter', function (evt) {
          bubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
            content:`<div>
            <b>POI Name:</b> ${element.poiName}<br>
            <b>Category:</b> ${element.categoryName}<br>
            <b>Address:</b> ${element.poiAddress}
            </div>`
          });
          // show info bubble
          _ui.addBubble(bubble);
        }, false);
        categoryPOIMarker.addEventListener('pointerleave', function(evt) {
          bubble.close();
        }, false);
      }
    });
  }

  showSearchMarker(markerData: any){
    if(markerData && markerData.lat && markerData.lng){
      let selectedMarker = new H.map.Marker({ lat: markerData.lat, lng: markerData.lng });
      if(markerData.from && markerData.from == 'search'){
        this.hereMap.setCenter({lat: markerData.lat, lng: markerData.lng}, 'default');
      }
      this.group.addObject(selectedMarker);
    }
  }

  showHereMapPOI(POIArr: any, selectedRoutes: any, _ui: any){
    let lat: any = 53.000751; // DAF Netherland lat
    let lng: any = 6.580920; // DAF Netherland lng
    if(selectedRoutes && selectedRoutes.length > 0){
      lat = selectedRoutes[selectedRoutes.length - 1].startPositionLattitude;
      lng = selectedRoutes[selectedRoutes.length - 1].startPositionLongitude;
    }
    if(POIArr.length > 0){
      POIArr.forEach(element => {
        this.herePOISearch.request(this.entryPoint.SEARCH, { 'at': lat + "," + lng, 'q': element }, (data) => {
          //console.log(data);
          for(let i = 0; i < data.results.items.length; i++) {
            this.dropMapPOIMarker({ "lat": data.results.items[i].position[0], "lng": data.results.items[i].position[1] }, data.results.items[i], element, _ui);
          }
        }, error => {
          console.log('ERROR: ' + error);
        });
      });
      if(selectedRoutes && selectedRoutes.length == 0){
        this.hereMap.setCenter({lat: lat, lng: lng}, 'default');
      }
    }
  }

  dropMapPOIMarker(coordinates: any, data: any, poiType: any, _ui: any) {
    let marker = this.createMarker(poiType);
    let markerSize = { w: 26, h: 26 };
    const icon = new H.map.Icon(marker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
    let poiMarker = new H.map.Marker(coordinates, {icon:icon});
    poiMarker.addEventListener('tap', event => {
        let bubble = new H.ui.InfoBubble(event.target.getGeometry(), {
          content: `<p> ${data.title}<br> ${data.vicinity}</p>`
        });
        _ui.addBubble(bubble);
    }, false);
    this.group.addObject(poiMarker);
  }

  createMarker(poiType: any){
    let homeMarker: any = '';
    switch(poiType){
      case 'Hotel':{
        homeMarker = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 50 50"><path fill="#ff00f3" d="M25,1.801c-9.703,0-17.602,7.895-17.602,17.598c0,11.227,10.711,22.117,16.461,27.969L25,48.527l1.137-1.16	c5.754-5.848,16.465-16.734,16.465-27.969C42.602,9.695,34.703,1.801,25,1.801z M31.295,27.399h-2.191V20.46h-8.207v6.939h-2.191	V11.898h2.191v6.617h8.207v-6.617h2.191V27.399z" style=""></path></svg>`;
        break;
      }
      case 'Petrol Station':{
        homeMarker = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 50 50"><path fill="#008400" d="M 25 1.8007812 C 15.297 1.8007812 7.3984375 9.6954375 7.3984375 19.398438 C 7.3984375 30.625437 18.109375 41.515188 23.859375 47.367188 L 25 48.527344 L 26.136719 47.367188 C 31.890719 41.519188 42.601562 30.633437 42.601562 19.398438 C 42.601562 9.6954375 34.703 1.8007812 25 1.8007812 z M 19.882812 11.898438 L 25.855469 11.898438 C 28.841469 11.898438 30.904297 13.970516 30.904297 16.978516 C 30.903297 19.943516 28.786547 22.007813 25.810547 22.007812 L 22.072266 22.007812 L 22.072266 27.400391 L 19.882812 27.400391 L 19.882812 11.898438 z M 22.072266 13.810547 L 22.072266 20.105469 L 25.285156 20.105469 C 27.412156 20.105469 28.658203 18.977234 28.658203 16.990234 C 28.658203 14.927234 27.455156 13.810547 25.285156 13.810547 L 22.072266 13.810547 z" style="
        "></path></svg>`;
        break;
      }
      case 'Parking':{
        homeMarker = `<svg xmlns="http://www.w3.org/2000/svg" id="Layer_1" x="0" y="0" version="1.1" viewBox="0 0 128 128" xml:space="preserve" style="
        "><circle cx="64" cy="64" r="55" fill="#f14e4e" style="
        "></circle><path fill="#FFF" d="M64,122C32,122,6,96,6,64S32,6,64,6s58,26,58,58S96,122,64,122z M64,12c-28.7,0-52,23.3-52,52s23.3,52,52,52 s52-23.3,52-52S92.7,12,64,12z" style="
        "></path><path fill="#FFF" d="M54,92c-1.7,0-3-1.3-3-3V44c0-1.7,1.3-3,3-3h12c9.4,0,17,7.6,17,17s-7.6,17-17,17h-9v14 C57,90.7,55.7,92,54,92z M57,69h9c6.1,0,11-4.9,11-11s-4.9-11-11-11h-9V69z"></path></svg>`;
        break;
      }
      case 'Railway Station':{
        homeMarker = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 50 50"><path fill="#0414a0" d="M 25 1.8007812 C 15.297 1.8007812 7.3984375 9.6954375 7.3984375 19.398438 C 7.3984375 30.625437 18.109375 41.515188 23.859375 47.367188 L 25 48.527344 L 26.136719 47.367188 C 31.890719 41.519188 42.601562 30.633437 42.601562 19.398438 C 42.601562 9.6954375 34.703 1.8007812 25 1.8007812 z M 19.683594 11.898438 L 25.720703 11.898438 C 28.782703 11.898438 30.769531 13.734797 30.769531 16.591797 C 30.769531 18.707797 29.620031 20.395297 27.707031 21.029297 L 31.240234 27.398438 L 28.695312 27.398438 L 25.462891 21.373047 L 21.873047 21.373047 L 21.873047 27.398438 L 19.683594 27.398438 L 19.683594 11.898438 z M 21.873047 13.798828 L 21.873047 19.492188 L 25.494141 19.492188 C 27.417141 19.493187 28.513672 18.473484 28.513672 16.646484 C 28.513672 14.863484 27.342922 13.798828 25.419922 13.798828 L 21.873047 13.798828 z" style=""></path></svg>`;
        break;
      }
    }
    return homeMarker;
  }

  viewSelectedRoutes(_selectedRoutes: any, _ui: any, trackType?: any, _displayRouteView?: any, _displayPOIList?: any, _searchMarker?: any, _herePOI?: any){
    this.clearRoutesFromMap();
    if(_herePOI){
      this.showHereMapPOI(_herePOI, _selectedRoutes, _ui);
    }
    if(_searchMarker){
      this.showSearchMarker(_searchMarker);
    }
    if(_displayPOIList && _displayPOIList.length > 0){ 
      this.showCategoryPOI(_displayPOIList, _ui); //-- show category POi
    }
    if(_selectedRoutes && _selectedRoutes.length > 0){
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

        //this.calculateAtoB(trackType);
        if(_selectedRoutes[i].liveFleetPosition.length > 1){ // required 2 points atleast to draw polyline
          let liveFleetPoints: any = _selectedRoutes[i].liveFleetPosition;
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
        this.hereMap.setCenter({lat: this.startAddressPositionLat, lng: this.startAddressPositionLong}, 'default');
      }
      this.setMarkerCluster(_selectedRoutes, _ui, this.hereMap); // cluster route marker
    }else{
      if(_displayPOIList.length > 0 || (_searchMarker && _searchMarker.lat && _searchMarker.lng) || (_herePOI && _herePOI.length > 0)){
        this.hereMap.addObject(this.group);
      }
    }
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

   getFilterDataPoints(_dataPoints: any, _displayRouteView: any){
    let fuelThreshold: any = 400; // hard coded
    let co2Threshold: any = 1; // hard coded
    let threshold: any = 0;
    let innerArray: any = [];
    let outerArray: any = [];
    let finalDataPoints: any = [];
    _dataPoints.forEach((element) => { 
      let elemChecker: any;
      if(_displayRouteView == 'F'){ // fuel consumption
        threshold = fuelThreshold;
        elemChecker = element.fuelconsumtion;
      }else{ // co2 emission
        threshold = co2Threshold;
        elemChecker = element.co2Emission;
      }
      
      if(elemChecker < threshold){
        element.color = '#12a802'; // green
      }else{
        element.color = '#f2f200'; // yellow  and #FFBF00 - Amber
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

  setMarkerCluster(data: any, ui: any, hereMap: any){
    let dataPoints = data.map((item) => {
      return new H.clustering.DataPoint(item.startPositionLattitude, item.startPositionLongitude);
    });
    var noiseSvg =
    '<svg xmlns="http://www.w3.org/2000/svg" height="50px" width="50px">' +
    '<circle cx="20px" cy="20px" r="20" fill="red" />' +
    '<text x="20" y="35" font-size="30pt" font-family="arial" font-weight="bold" text-anchor="middle" fill="white" textContent="!">!</text></svg>';
  
    var noiseIcon = new H.map.Icon(noiseSvg, {
      size: { w: 22, h: 22 },
      anchor: { x: 11, y: 11 }
    });
    
    var clusterSvgTemplate =
    '<svg xmlns="http://www.w3.org/2000/svg" height="50px" width="50px"><circle cx="25px" cy="25px" r="20" fill="red" stroke-opacity="0.5" />' +
    '<text x="24" y="32" font-size="14pt" font-family="arial" font-weight="bold" text-anchor="middle" fill="white">{text}</text>' +
    '</svg>';
    // // Create a clustering provider with custom options for clusterizing the input
    let clusteredDataProvider = new H.clustering.Provider(dataPoints, {
      clusteringOptions: {
        // Maximum radius of the neighbourhood
        eps: 32,
        // minimum weight of points required to form a cluster
        minWeight: 9
      },
      theme: {
        getClusterPresentation: (markerCluster: any) => {
  
          // Use cluster weight to change icon size:
          var svgString = clusterSvgTemplate.replace('{radius}', markerCluster.getWeight());
          svgString = svgString.replace('{text}', markerCluster.getWeight());
  
          var w, h;
          var weight = markerCluster.getWeight();
  
          //Set cluster size depending on the weight
          if (weight <= 6)
          {
            w = 35;
            h = 35;
          }
          else if (weight <= 12) {
            w = 50;
            h = 50;
          }
          else {
            w = 75;
            h = 75;
          }
  
          var clusterIcon = new H.map.Icon(svgString, {
            size: { w: w, h: h },
            anchor: { x: (w/2), y: (h/2) }
          });
  
          // Create a marker for clusters:
          var clusterMarker = new H.map.Marker(markerCluster.getPosition(), {
            icon: clusterIcon,
            // Set min/max zoom with values from the cluster, otherwise
            // clusters will be shown at all zoom levels:
            min: markerCluster.getMinZoom(),
            max: markerCluster.getMaxZoom()
          });
  
          // Bind cluster data to the marker:
          clusterMarker.setData(markerCluster);
          let infoBubble: any
          clusterMarker.addEventListener("pointerenter",  (event) => {
  
            var point = event.target.getGeometry(),
              screenPosition = hereMap.geoToScreen(point),
              t = event.target,
              data = t.getData(),
              tooltipContent = "<table border='1'><thead><th>Action</th><th>Latitude</th><th>Longitude</th></thead><tbody>"; 
              var chkBxId = 0;
            data.forEachEntry(
              (p) => 
              { 
                tooltipContent += "<tr>";
                tooltipContent += "<td><input type='checkbox' id='"+ chkBxId +"' onclick='infoBubbleCheckBoxClick("+ chkBxId +","+ p.getPosition().lat +", "+ p.getPosition().lng +")'></td>" + "<td>" + p.getPosition().lat + "</td><td>" + p.getPosition().lng + "</td>";
                tooltipContent += "</tr>";
                chkBxId++;
                //alert(chkBxId);
              }
            ); 
            tooltipContent += "</tbody></table>";
            
            // function infoBubbleCheckBoxClick(chkBxId, latitude, longitude){
            //   // Get the checkbox
            //   let checkBox: any = document.getElementById(chkBxId);
            //   if (checkBox.checked == true){
            //     alert("Latitude:" + latitude + " Longitude:" + longitude + " Enabled")
            //   } else {
            //     alert("Latitude:" + latitude + " Longitude:" + longitude + " Disabled")
            //   }
            // }

            infoBubble = new H.ui.InfoBubble(hereMap.screenToGeo(screenPosition.x, screenPosition.y), { content: tooltipContent });
            ui.addBubble(infoBubble);
          });
          
          
          clusterMarker.addEventListener("pointerleave", (event) => { 
            if(infoBubble)
            {
              ui.removeBubble(infoBubble);
              infoBubble = null;
            }
          });				
  
          return clusterMarker;
        },
        getNoisePresentation: (noisePoint) => {
          let infoBubble: any;
  
          // Create a marker for noise points:
          var noiseMarker = new H.map.Marker(noisePoint.getPosition(), {
            icon: noiseIcon,
  
            // Use min zoom from a noise point to show it correctly at certain
            // zoom levels:
            min: noisePoint.getMinZoom(),
            max: 20
          });
  
          // Bind cluster data to the marker:
          noiseMarker.setData(noisePoint);
  
          noiseMarker.addEventListener("pointerenter", (event) => { 
            
            var point = event.target.getGeometry();
            var tooltipContent = ["Latitude: ", point.lat, ", Longitude: ", point.lng].join("");
  
            var screenPosition = hereMap.geoToScreen(point);
  
            infoBubble = new H.ui.InfoBubble(hereMap.screenToGeo(screenPosition.x, screenPosition.y), { content: tooltipContent });
            ui.addBubble(infoBubble);
          
          });
          
          noiseMarker.addEventListener("pointerleave", (event) => { 
            if(infoBubble)
            {
              ui.removeBubble(infoBubble);
              infoBubble = null;
            }
          });
          
  
          return noiseMarker;
        }
      }
    });
  
    // // Create a layer tha will consume objects from our clustering provider
    this.clusteringLayer = new H.map.layer.ObjectLayer(clusteredDataProvider);
  
    // // To make objects from clustering provder visible,
    // // we need to add our layer to the map
    hereMap.addLayer(this.clusteringLayer);
    clusteredDataProvider.addEventListener('tap', (event) => {
      // Log data bound to the marker that has been tapped:
      console.log(event.target.getData())
    });
  }

  infoBubbleCheckBoxClick(chkBxId, latitude, longitude){
    var checkBox: any = document.getElementById(chkBxId);
    if (checkBox.checked == true){
      alert("Latitude:" + latitude + " Longitude:" + longitude + " Enabled")
    } else {
      alert("Latitude:" + latitude + " Longitude:" + longitude + " Disabled")
    }
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

  calculateAtoB(trackType?: any){
    let routeRequestParams = {
      'routingMode': 'fast',
      'transportMode': 'truck',
      'origin': `${this.startAddressPositionLat},${this.startAddressPositionLong}`, 
      'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`, 
      'return': 'polyline'
    };
    this.hereSerive.calculateRoutePoints(routeRequestParams).then((data: any)=>{
      this.addRouteShapeToMap(data, trackType);
    },(error)=>{
       console.error(error);
    })
  }

  addRouteShapeToMap(result: any, trackType?: any){
    result.routes[0].sections.forEach((section) =>{
      let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);
        this.routeOutlineMarker = new H.map.Polyline(linestring, {
          style: {
            lineWidth: this.corridorWidthKm,
            strokeColor: '#b5c7ef',
          }
        });
        // Create a patterned polyline:
        if(trackType && trackType == 'dotted'){
          this.routeCorridorMarker = new H.map.Polyline(linestring, {
            style: {
              lineWidth: 3,
              strokeColor: '#436ddc',
              lineDash:[2,2]
            }
          });
        }else{
          this.routeCorridorMarker = new H.map.Polyline(linestring, {
            style: {
              lineWidth: 3,
              strokeColor: '#436ddc'
            }
          });
        }
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

  getConvertedDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.convertedStartTime = this.getStartTime(element.startTimeStamp, dateFormat, timeFormat, timeZone,true);
      element.convertedEndTime = this.getEndTime(element.endTimeStamp, dateFormat, timeFormat, timeZone,true);
      element.convertedAverageWeight = this.getAvrgWeight(element.averageWeight, unitFormat);
      element.convertedAverageSpeed = this.getAvergSpeed(element.averageSpeed, unitFormat);
      element.convertedFuelConsumed100Km = this.getFuelConsumed(element.fuelConsumed100Km, unitFormat);
      element.convertedDistance = this.getDistance(element.distance, unitFormat);
      element.convertedDrivingTime = this.getHhMmTime(element.drivingTime);
      element.convertedIdleDuration = this.getHhMmTime(element.idleDuration);
    });
    return gridData;
  }

  getConvertedFleetDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.convertedStopTime = this.getStartTime(element.StopTime, dateFormat, timeFormat, timeZone,true);
      element.convertedAverageWeight = this.getAvrgWeight(element.averageWeightPerTrip, unitFormat);
      element.convertedAverageSpeed = this.getAvergSpeed(element.averageSpeed, unitFormat);
      element.convertedAverageDistance = this.getDistance(element.averageDistancePerDay, unitFormat);
      element.convertedDistance = this.getDistance(element.distance, unitFormat);
      element.convertedDrivingTime = this.getHhMmTime(element.drivingTime);
      element.convertedTripTime = this.getHhMmTime(element.tripTime);
      element.convertedIdleDuration = this.getHhMmTime(element.idleDuration);
    });
    return gridData;
  }

  getDriverTimeDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.driverName = element.driverName;
      element.driverId = element.driverId;
      element.startTime = this.getStartTime(element.startTime, dateFormat, timeFormat, timeZone);
      element.endTime = this.getEndTime(element.endTime, dateFormat, timeFormat, timeZone);
      element.driveTime = this.getHhMmTime(element.driveTime);
      element.workTime = this.getHhMmTime(element.workTime);
      element.serviceTime = this.getHhMmTime(element.serviceTime);
      element.restTime = this.getHhMmTime(element.restTime);
      element.availableTime = this.getHhMmTime(element.availableTime);
    });
    return gridData;
  }

  
  getDriverDetailsTimeDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.driverName = element.driverName;
      element.driverId = element.driverId;
      element.activityDate= this.getStartTime(element.activityDate, dateFormat, timeFormat, timeZone,false);
      element.driveTime = this.getHhMmTime(element.driveTime);
      element.workTime = this.getHhMmTime(element.workTime);
      element.serviceTime = this.getHhMmTime(element.serviceTime);
      element.restTime = this.getHhMmTime(element.restTime);
      element.availableTime = this.getHhMmTime(element.availableTime);
    });
    return gridData;
  }
  getStartTime(startTime: any, dateFormat: any, timeFormat: any, timeZone: any, addTime?:boolean){
    let sTime: any = 0;
    if(startTime != 0){
      sTime = this.formStartEndDate(Util.convertUtcToDate(startTime, timeZone), dateFormat, timeFormat, addTime);
    }
    return sTime;
  }

  getEndTime(endTime: any, dateFormat: any, timeFormat: any, timeZone: any, addTime?:boolean){
    let eTime: any = 0;
    if(endTime != 0){
      eTime = this.formStartEndDate(Util.convertUtcToDate(endTime, timeZone), dateFormat, timeFormat, addTime);
    }
    return eTime;
  }

  getDistance(distance: any, unitFormat: any){
    // distance in meter
    let _distance: any = 0;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _distance = (distance/1000).toFixed(2); //-- km
        break;
      }
      case 'dunit_Imperial':
      case 'dunit_USImperial': {
        _distance = (distance/1609.344).toFixed(2); //-- mile
        break;
      }
      default: {
        _distance = distance.toFixed(2);
      }
    }
    return _distance;    
  }

  getAvrgWeight(avgWeight: any, unitFormat: any ){
    let _avgWeight: any = 0;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _avgWeight = avgWeight.toFixed(2); //-- kg/count
        break;
      }
      case 'dunit_Imperial':{
        _avgWeight =(avgWeight * 2.2).toFixed(2); //pounds/count
        break;
      }
      case 'dunit_USImperial': {
        _avgWeight = (avgWeight * 2.2 * 1.009).toFixed(2); //-- imperial * 1.009
        break;
      }
      default: {
        _avgWeight = avgWeight.toFixed(2);
      }
    }
    return _avgWeight;    
  }

  getAvergSpeed(avgSpeed: any, unitFormat: any){
    let _avgSpeed : any = 0;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _avgSpeed = (avgSpeed * 360).toFixed(2); //-- km/h(converted from m/ms)
        break;
      }
      case 'dunit_Imperial':{
        _avgSpeed = (avgSpeed * 360/1.6).toFixed(2); //miles/h
        break;
      }
      case 'dunit_USImperial': {
        _avgSpeed = (avgSpeed * 360/1.6).toFixed(2); //-- miles/h
        break;
      }
      default: {
        _avgSpeed = avgSpeed.toFixed(2);
      }
    }
    return _avgSpeed;    
  }

  getFuelConsumed(fuelConsumed: any, unitFormat: any){
    let _fuelConsumed: any = 0;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _fuelConsumed = (fuelConsumed / 100).toFixed(2); //-- ltr/km(converted from ml/m)
        break;
      }
      case 'dunit_Imperial':{
        _fuelConsumed = (fuelConsumed * (1.6/370)).toFixed(2); //gallons/miles
        break;
      }
      case 'dunit_USImperial': {
        _fuelConsumed = (fuelConsumed * (1.6/370) * 1.2).toFixed(2); //-- imperial * 1.2
        break;
      }
      default: {
        _fuelConsumed = fuelConsumed.toFixed(2);
      }
    }
    return _fuelConsumed; 
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

  formStartEndDate(date: any, dateFormat: any, timeFormat: any, addTime?:boolean){
    // let h = (date.getHours() < 10) ? ('0'+date.getHours()) : date.getHours(); 
    // let m = (date.getMinutes() < 10) ? ('0'+date.getMinutes()) : date.getMinutes(); 
    // let s = (date.getSeconds() < 10) ? ('0'+date.getSeconds()) : date.getSeconds(); 
    // let _d = (date.getDate() < 10) ? ('0'+date.getDate()): date.getDate();
    // let _m = ((date.getMonth()+1) < 10) ? ('0'+(date.getMonth()+1)): (date.getMonth()+1);
    // let _y = (date.getFullYear() < 10) ? ('0'+date.getFullYear()): date.getFullYear();
    let date1 = date.split(" ")[0];
    let time1 = date.split(" ")[1];
    let h = time1.split(":")[0];
    let m = time1.split(":")[1];
    let s = time1.split(":")[2];
    let _d = date1.split("/")[2];
    let _m = date1.split("/")[1];
    let _y = date1.split("/")[0];
    let _date: any;
    let _time: any;
    if(timeFormat == 12){
      _time = (h > 12 || (h == 12 && m > 0)) ? `${h == 12 ? 12 : h-12}:${m} PM` : `${(h == 0) ? 12 : h}:${m} AM`;
    }else{
      _time = `${h}:${m}:${s}`;
    }
    switch(dateFormat){
      case 'ddateformat_dd/mm/yyyy': {
        if(addTime)
        _date = `${_d}/${_m}/${_y} ${_time}`;
        else
        _date = `${_d}/${_m}/${_y}`;

        break;
      }
      case 'ddateformat_mm/dd/yyyy': {
        if(addTime)
        _date = `${_m}/${_d}/${_y} ${_time}`;
        else
        _date = `${_m}/${_d}/${_y}`;
        break;
      }
      case 'ddateformat_dd-mm-yyyy': {
        if(addTime)
        _date = `${_d}-${_m}-${_y} ${_time}`;
        else
        _date = `${_d}-${_m}-${_y}`;

        break;
      }
      case 'ddateformat_mm-dd-yyyy': {
        if(addTime)
        _date = `${_m}-${_d}-${_y} ${_time}`;
        else
        _date = `${_m}-${_d}-${_y}`;

        break;
      }
      default:{
        if(addTime)
        _date = `${_m}/${_d}/${_y} ${_time}`;
        else
        _date = `${_m}/${_d}/${_y}`;

      }
    }
    return _date;
  }
   
}
