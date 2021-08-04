import { Injectable,Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { HereService } from '../services/here.service';
import { Util } from '../shared/util';
import { ConfigService } from '@ngx-config/core';
import { elementEventFullName } from '@angular/compiler/src/view_compiler/view_compiler';

declare var H: any;

@Injectable({
  providedIn: 'root'
})
export class ReportMapService {
  platform: any;
  clusteringLayer: any;
  markerClusterLayer: any = [];
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
  disableGroup = new H.map.Group();
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
    this.hereMap.removeObjects(this.hereMap.getObjects());
    this.group.removeAll();
    this.disableGroup.removeAll();
    this.startMarker = null; 
    this.endMarker = null; 
    if(this.clusteringLayer){
      this.clusteringLayer.dispose();
      this.hereMap.removeLayer(this.clusteringLayer);
      this.clusteringLayer = null;
    }
    if(this.markerClusterLayer && this.markerClusterLayer.length > 0){
      this.markerClusterLayer.forEach(element => {
        this.hereMap.removeLayer(element);
      });
      this.markerClusterLayer = [];
    }
    if(this.overlayLayer){
      this.hereMap.removeLayer(this.overlayLayer);
      this.overlayLayer = null;
    }
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
    let locMarkup = `<svg width="25" height="39" viewBox="0 0 25 39" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M22.9991 12.423C23.2909 20.9156 12.622 28.5702 12.622 28.5702C12.622 28.5702 1.45279 21.6661 1.16091 13.1735C1.06139 10.2776 2.11633 7.46075 4.09368 5.34265C6.07103 3.22455 8.8088 1.9787 11.7047 1.87917C14.6006 1.77965 17.4175 2.83459 19.5356 4.81194C21.6537 6.78929 22.8995 9.52706 22.9991 12.423Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.6012 37.9638C12.6012 37.9638 22.5882 18.1394 22.3924 12.444C22.1967 6.74858 17.421 2.29022 11.7255 2.48596C6.03013 2.6817 1.57177 7.45742 1.76751 13.1528C1.96325 18.8482 12.6012 37.9638 12.6012 37.9638Z" fill="#00529C"/>
    <path d="M12.3824 21.594C17.4077 21.4213 21.3486 17.4111 21.1845 12.637C21.0204 7.86293 16.8136 4.13277 11.7882 4.30549C6.76283 4.4782 2.82198 8.48838 2.98605 13.2625C3.15013 18.0366 7.357 21.7667 12.3824 21.594Z" fill="white"/>
    </svg>`;
    let markerSize = { w: 25, h: 39 };
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
            content:`<table style='width: 350px;'>
            <tr>
              <td style='width: 100px;'>POI Name:</td> <td><b>${element.poiName}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Category:</td> <td><b>${element.categoryName}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Sub-Category:</td> <td><b>${element.subCategoryName != '' ? element.subCategoryName : '-'}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Address:</td> <td><b>${element.poiAddress != '' ? element.poiAddress : '-'}</b></td>
            </tr>
          </table>`
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
    let lat: any = 51.43175839453286; // DAF Netherland lat
    let lng: any = 5.519981221425336; // DAF Netherland lng
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
    let markerSize = { w: 26, h: 32 };
    const icon = new H.map.Icon(marker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
    let poiMarker = new H.map.Marker(coordinates, {icon:icon});
    let bubble: any = '';
    poiMarker.addEventListener('pointerenter', event => {
        bubble = new H.ui.InfoBubble(event.target.getGeometry(), {
          content: `<p> ${data.title}<br> ${data.vicinity}</p>`
        });
        _ui.addBubble(bubble);
    }, false);
    poiMarker.addEventListener('pointerleave', evt => {
      bubble.close();
    }, false);
    this.group.addObject(poiMarker);
  }

  createMarker(poiType: any){
    let homeMarker: any = '';
    switch(poiType){
      case 'Hotel':{
        homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M12.9998 29.6667C18.6665 25.0001 24.3332 19.2593 24.3332 13.0001C24.3332 6.74085 19.2591 1.66675 12.9998 1.66675C6.74061 1.66675 1.6665 6.74085 1.6665 13.0001C1.6665 19.2593 7.6665 25.3334 12.9998 29.6667Z" fill="#00529C"/>
        <path d="M13 22.6667C18.5228 22.6667 23 18.4135 23 13.1667C23 7.92004 18.5228 3.66675 13 3.66675C7.47715 3.66675 3 7.92004 3 13.1667C3 18.4135 7.47715 22.6667 13 22.6667Z" fill="white"/>
        <path d="M10.575 13C11.471 13 12.2 12.2523 12.2 11.3333C12.2 10.4144 11.471 9.66667 10.575 9.66667C9.67902 9.66667 8.95 10.4144 8.95 11.3333C8.95 12.2523 9.67902 13 10.575 13ZM17.725 10.3333H13.175C12.9954 10.3333 12.85 10.4825 12.85 10.6667V13.6667H8.3V9.33333C8.3 9.14917 8.15456 9 7.975 9H7.325C7.14544 9 7 9.14917 7 9.33333V16.6667C7 16.8508 7.14544 17 7.325 17H7.975C8.15456 17 8.3 16.8508 8.3 16.6667V15.6667H18.7V16.6667C18.7 16.8508 18.8454 17 19.025 17H19.675C19.8546 17 20 16.8508 20 16.6667V12.6667C20 11.3779 18.9815 10.3333 17.725 10.3333Z" fill="#00529C"/>
        </svg>`;
        break;
      }
      case 'Petrol Station':{
        homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M12.9998 29.6667C18.6665 25.0001 24.3332 19.2593 24.3332 13.0001C24.3332 6.74085 19.2591 1.66675 12.9998 1.66675C6.74061 1.66675 1.6665 6.74085 1.6665 13.0001C1.6665 19.2593 7.6665 25.3334 12.9998 29.6667Z" fill="#00529C"/>
        <path d="M13 22.6667C18.5228 22.6667 23 18.4135 23 13.1667C23 7.92004 18.5228 3.66675 13 3.66675C7.47715 3.66675 3 7.92004 3 13.1667C3 18.4135 7.47715 22.6667 13 22.6667Z" fill="white"/>
        <path d="M15.875 17.5H8.375C8.16875 17.5 8 17.6687 8 17.875V18.625C8 18.8313 8.16875 19 8.375 19H15.875C16.0813 19 16.25 18.8313 16.25 18.625V17.875C16.25 17.6687 16.0813 17.5 15.875 17.5ZM19.5594 9.51484L17.6609 7.61641C17.5156 7.47109 17.2766 7.47109 17.1313 7.61641L16.8664 7.88125C16.7211 8.02656 16.7211 8.26562 16.8664 8.41094L17.75 9.29453V10.75C17.75 11.4086 18.2398 11.9523 18.875 12.0437V15.8125C18.875 16.1219 18.6219 16.375 18.3125 16.375C18.0031 16.375 17.75 16.1219 17.75 15.8125V15.0625C17.75 13.9234 16.8266 13 15.6875 13H15.5V8.5C15.5 7.67266 14.8273 7 14 7H10.25C9.42266 7 8.75 7.67266 8.75 8.5V16.75H15.5V14.125H15.6875C16.2055 14.125 16.625 14.5445 16.625 15.0625V15.7141C16.625 16.5977 17.2578 17.4016 18.1367 17.493C19.1445 17.5938 20 16.8016 20 15.8125V10.5766C20 10.1781 19.8406 9.79609 19.5594 9.51484ZM14 11.5H10.25V8.5H14V11.5Z" fill="#00529C"/>
        </svg>`;
        break;
      }
      case 'Parking':{
        homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M13.3333 29.6667C19 25.0001 24.6667 19.2593 24.6667 13.0001C24.6667 6.74085 19.5926 1.66675 13.3333 1.66675C7.07411 1.66675 2 6.74085 2 13.0001C2 19.2593 8 25.3334 13.3333 29.6667Z" fill="#00529C"/>
        <path d="M13 23C18.5228 23 23 18.7467 23 13.5C23 8.25329 18.5228 4 13 4C7.47715 4 3 8.25329 3 13.5C3 18.7467 7.47715 23 13 23Z" fill="white"/>
        <path d="M13 7C9.13391 7 6 10.1339 6 14C6 17.8661 9.13391 21 13 21C16.8661 21 20 17.8661 20 14C20 10.1339 16.8661 7 13 7ZM13 19.1935C10.1362 19.1935 7.80645 16.8638 7.80645 14C7.80645 11.1362 10.1362 8.80645 13 8.80645C15.8638 8.80645 18.1935 11.1362 18.1935 14C18.1935 16.8638 15.8638 19.1935 13 19.1935ZM13.9032 10.8387H11.1935C10.944 10.8387 10.7419 11.0408 10.7419 11.2903V16.7097C10.7419 16.9592 10.944 17.1613 11.1935 17.1613H12.0968C12.3463 17.1613 12.5484 16.9592 12.5484 16.7097V15.3548H13.9032C15.1502 15.3548 16.1613 14.3438 16.1613 13.0968C16.1613 11.8498 15.1502 10.8387 13.9032 10.8387ZM13.9032 13.5484H12.5484V12.6452H13.9032C14.1522 12.6452 14.3548 12.8478 14.3548 13.0968C14.3548 13.3457 14.1522 13.5484 13.9032 13.5484Z" fill="#00529C"/>
        </svg>`;
        break;
      }
      case 'Railway Station':{
        homeMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M12.9998 29.6667C18.6665 25.0001 24.3332 19.2593 24.3332 13.0001C24.3332 6.74085 19.2591 1.66675 12.9998 1.66675C6.74061 1.66675 1.6665 6.74085 1.6665 13.0001C1.6665 19.2593 7.6665 25.3334 12.9998 29.6667Z" fill="#00529C"/>
        <path d="M13 22.6667C18.5228 22.6667 23 18.4135 23 13.1667C23 7.92004 18.5228 3.66675 13 3.66675C7.47715 3.66675 3 7.92004 3 13.1667C3 18.4135 7.47715 22.6667 13 22.6667Z" fill="white"/>
        <g clip-path="url(#clip0)">
        <path d="M18 10.1429V15.8571C18 17.0137 16.6245 18 15.0977 18L16.5035 19.1098C16.6363 19.2147 16.5617 19.4286 16.3929 19.4286H9.60714C9.43799 19.4286 9.36402 19.2144 9.4965 19.1098L10.9023 18C9.37991 18 8 17.0168 8 15.8571V10.1429C8 8.9594 9.42857 8 10.8571 8H15.1429C16.5938 8 18 8.9594 18 10.1429ZM12.4643 13.1786V10.6786C12.4643 10.3827 12.2244 10.1429 11.9286 10.1429H9.60714C9.31127 10.1429 9.07143 10.3827 9.07143 10.6786V13.1786C9.07143 13.4744 9.31127 13.7143 9.60714 13.7143H11.9286C12.2244 13.7143 12.4643 13.4744 12.4643 13.1786ZM16.9286 13.1786V10.6786C16.9286 10.3827 16.6887 10.1429 16.3929 10.1429H14.0714C13.7756 10.1429 13.5357 10.3827 13.5357 10.6786V13.1786C13.5357 13.4744 13.7756 13.7143 14.0714 13.7143H16.3929C16.6887 13.7143 16.9286 13.4744 16.9286 13.1786ZM15.8571 14.4286C15.2654 14.4286 14.7857 14.9083 14.7857 15.5C14.7857 16.0917 15.2654 16.5714 15.8571 16.5714C16.4489 16.5714 16.9286 16.0917 16.9286 15.5C16.9286 14.9083 16.4489 14.4286 15.8571 14.4286ZM10.1429 14.4286C9.55112 14.4286 9.07143 14.9083 9.07143 15.5C9.07143 16.0917 9.55112 16.5714 10.1429 16.5714C10.7346 16.5714 11.2143 16.0917 11.2143 15.5C11.2143 14.9083 10.7346 14.4286 10.1429 14.4286Z" fill="#00529C"/>
        </g>
        <defs>
        <clipPath id="clip0">
        <rect width="10" height="11.4286" fill="white" transform="translate(8 8)"/>
        </clipPath>
        </defs>
        </svg>`;
        break;
      }
    }
    return homeMarker;
  }

  viewSelectedRoutes(_selectedRoutes: any, _ui: any, trackType?: any, _displayRouteView?: any, _displayPOIList?: any, _searchMarker?: any, _herePOI?: any, row?: any){
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
      _selectedRoutes.forEach(elem => {
        this.startAddressPositionLat = elem.startPositionLattitude;
        this.startAddressPositionLong = elem.startPositionLongitude;
        this.endAddressPositionLat= elem.endPositionLattitude;
        this.endAddressPositionLong= elem.endPositionLongitude;
        this.corridorWidth = 1000; //- hard coded
        this.corridorWidthKm = this.corridorWidth/1000;
        let houseMarker = this.createHomeMarker();
        let markerSize = { w: 26, h: 32 };
        const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.startMarker = new H.map.Marker({ lat:this.startAddressPositionLat, lng:this.startAddressPositionLong },{ icon:icon });
        let endMarker = this.createEndMarker();
        const iconEnd = new H.map.Icon(endMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.endMarker = new H.map.Marker({ lat:this.endAddressPositionLat, lng:this.endAddressPositionLong },{ icon:iconEnd });
        this.group.addObjects([this.startMarker, this.endMarker]);
        var startBubble;
        this.startMarker.addEventListener('pointerenter', function (evt) {
          // event target is the marker itself, group is a parent event target
          // for all objects that it contains
          startBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
            // read custom data
            content:`<table style='width: 350px;'>
              <tr>
                <td style='width: 100px;'>Start Location:</td> <td><b>${elem.startPosition}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>Start Date:</td> <td><b>${elem.convertedStartTime}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>Total Alerts:</td> <td><b>${elem.alert}</b></td>
              </tr>
            </table>`
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
            content:`<table style='width: 350px;'>
              <tr>
                <td style='width: 100px;'>End Location:</td> <td><b>${elem.endPosition}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>End Date:</td> <td><b>${elem.convertedEndTime}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>Total Alerts:</td> <td><b>${elem.alert}</b></td>
              </tr>
            </table>`
          });
          // show info bubble
          _ui.addBubble(endBubble);
        }, false);
        this.endMarker.addEventListener('pointerleave', function(evt) {
          endBubble.close();
        }, false);

        //this.calculateAtoB(trackType);
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
        if(elem.id == row.id){
          let grp= new H.map.Group();
          grp.addObjects([this.startMarker, this.endMarker]);
          this.hereMap.addObject(grp);
          this.hereMap.getViewModel().setLookAtData({
            bounds: grp.getBoundingBox()
          });
        }
        
        // this.hereMap.setCenter({lat: this.startAddressPositionLat, lng: this.startAddressPositionLong}, 'default');
      });
      
      this.makeCluster(_selectedRoutes, _ui);
    }else{
      if(_displayPOIList.length > 0 || (_searchMarker && _searchMarker.lat && _searchMarker.lng) || (_herePOI && _herePOI.length > 0)){
        this.hereMap.addObject(this.group);
      }
    }
   }

   makeCluster(_selectedRoutes: any, _ui: any){
    if(_selectedRoutes.length > 9){
      this.setInitialCluster(_selectedRoutes, _ui); 
    }else{
      this.afterPlusClick(_selectedRoutes, _ui);
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

  setInitialCluster(data: any, ui: any){
    let dataPoints = data.map((item) => {
      return new H.clustering.DataPoint(item.startPositionLattitude, item.startPositionLongitude);
    });
    var noiseSvg =
    '<svg xmlns="http://www.w3.org/2000/svg" height="50px" width="50px">' +
    '<circle cx="20px" cy="20px" r="20" fill="red" />' +
    '<text x="20" y="35" font-size="30pt" font-family="arial" font-weight="bold" text-anchor="middle" fill="white" textContent="!">{text}</text></svg>';
  
    // var noiseIcon = new H.map.Icon(noiseSvg, {
    //   size: { w: 22, h: 22 },
    //   anchor: { x: 11, y: 11 }
    // });
    
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
        minWeight: 10
      },
      theme: {
        getClusterPresentation: (markerCluster: any) => {
  
          // Use cluster weight to change icon size:
          var svgString = clusterSvgTemplate.replace('{radius}', markerCluster.getWeight());
          if(data && data.length > 9){
            svgString = svgString.replace('{text}', '+');
          }else{
            svgString = svgString.replace('{text}', markerCluster.getWeight());
          }
  
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
          //clusterMarker.setZIndex(10);
          //let infoBubble: any
          // clusterMarker.addEventListener("tap",  (event) => {
  
          //   var point = event.target.getGeometry(),
          //     screenPosition = this.hereMap.geoToScreen(point),
          //     t = event.target,
          //     data = t.getData(),
          //     tooltipContent = "<table border='1'><thead><th>Action</th><th>Latitude</th><th>Longitude</th></thead><tbody>"; 
          //     var chkBxId = 0;
          //   data.forEachEntry(
          //     (p) => 
          //     { 
          //       tooltipContent += "<tr>";
          //       tooltipContent += "<td><input type='checkbox' id='"+ chkBxId +"' onclick='infoBubbleCheckBoxClick("+ chkBxId +","+ p.getPosition().lat +", "+ p.getPosition().lng +")'></td>" + "<td>" + p.getPosition().lat + "</td><td>" + p.getPosition().lng + "</td>";
          //       tooltipContent += "</tr>";
          //       chkBxId++;
          //       //alert(chkBxId);
          //     }
          //   ); 
          //   tooltipContent += "</tbody></table>";
            
          //   // function infoBubbleCheckBoxClick(chkBxId, latitude, longitude){
          //   //   // Get the checkbox
          //   //   let checkBox: any = document.getElementById(chkBxId);
          //   //   if (checkBox.checked == true){
          //   //     alert("Latitude:" + latitude + " Longitude:" + longitude + " Enabled")
          //   //   } else {
          //   //     alert("Latitude:" + latitude + " Longitude:" + longitude + " Disabled")
          //   //   }
          //   // }

          //   infoBubble = new H.ui.InfoBubble(this.hereMap.screenToGeo(screenPosition.x, screenPosition.y), { content: tooltipContent });
          //   ui.addBubble(infoBubble);
          // });
          
          
          // clusterMarker.addEventListener("pointerleave", (event) => { 
          //   if(infoBubble)
          //   {
          //     ui.removeBubble(infoBubble);
          //     infoBubble = null;
          //   }
          // });				
  
          return clusterMarker;
        },
        getNoisePresentation: (noisePoint) => {
          //let infoBubble: any;
          var noiseSvgString = noiseSvg.replace('{radius}', noisePoint.getWeight());
          if(data && data.length > 9){
            noiseSvgString = noiseSvgString.replace('{text}', '+');
          }else{
            noiseSvgString = noiseSvgString.replace('{text}', noisePoint.getWeight());
          }
  
          var w, h;
          var weight = noisePoint.getWeight();
  
          //Set cluster size depending on the weight
          if (weight <= 6)
          {
            w = 30;
            h = 30;
          }
          else if (weight <= 12) {
            w = 40;
            h = 40;
          }
          else {
            w = 50;
            h = 50;
          }
  
          var noiseIcon = new H.map.Icon(noiseSvgString, {
            size: { w: w, h: h },
            anchor: { x: (w/2), y: (h/2) }
          });
  
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
  
          // noiseMarker.addEventListener("tap", (event) => { 
            
          //   var point = event.target.getGeometry();
          //   var tooltipContent = ["Latitude: ", point.lat, ", Longitude: ", point.lng].join("");
  
          //   var screenPosition = this.hereMap.geoToScreen(point);
  
          //   infoBubble = new H.ui.InfoBubble(this.hereMap.screenToGeo(screenPosition.x, screenPosition.y), { content: tooltipContent });
          //   ui.addBubble(infoBubble);
          
          // });
          
          // noiseMarker.addEventListener("pointerleave", (event) => { 
          //   if(infoBubble)
          //   {
          //     ui.removeBubble(infoBubble);
          //     infoBubble = null;
          //   }
          // });
          
  
          return noiseMarker;
        }
      }
    });
  
    // // Create a layer tha will consume objects from our clustering provider
    this.clusteringLayer = new H.map.layer.ObjectLayer(clusteredDataProvider);
  
    // // To make objects from clustering provder visible,
    // // we need to add our layer to the map
    this.hereMap.addLayer(this.clusteringLayer, 100); // set z-index to cluster
    clusteredDataProvider.addEventListener('tap', (event) => {
      // Log data bound to the marker that has been tapped:
      //console.log(event.target.getData(), data)
      this.afterPlusClick(data, ui);
    });
  }

  setMarkerCluster(data: any, ui: any){
    let dataPoints = data.map((item) => {
      return new H.clustering.DataPoint(item.startPositionLattitude, item.startPositionLongitude);
    });
    var noiseSvg =
    '<svg xmlns="http://www.w3.org/2000/svg" height="50px" width="50px">' +
    '<circle cx="20px" cy="20px" r="20" fill="red" />' +
    '<text x="20" y="35" font-size="30pt" font-family="arial" font-weight="bold" text-anchor="middle" fill="white" textContent="!">!</text>{text}</svg>';
  
    // var noiseIcon = new H.map.Icon(noiseSvg, {
    //   size: { w: 22, h: 22 },
    //   anchor: { x: 11, y: 11 }
    // });
    
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
        minWeight: 2
      },
      theme: {
        getClusterPresentation: (markerCluster: any) => {
          // Use cluster weight to change icon size:
          var svgString = clusterSvgTemplate.replace('{radius}', markerCluster.getWeight());
          //svgString = svgString.replace('{text}', markerCluster.getWeight());
          if(data && data.length > 9){
            svgString = svgString.replace('{text}', '+');
          }else{
            svgString = svgString.replace('{text}', markerCluster.getWeight());
          }
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
          clusterMarker.addEventListener("tap",  (event) => {
            this.removedDisabledGroup();
            data.forEach((element, _index) => {
              let liveFleetPoints: any = element.liveFleetPosition;
              liveFleetPoints.sort((a, b) => parseInt(a.id) - parseInt(b.id)); 
              this.selectionPolylineRoute(liveFleetPoints, _index);   
            });
            this.hereMap.addObject(this.disableGroup);
  
            var point = event.target.getGeometry(),
              screenPosition = this.hereMap.geoToScreen(point),
              t = event.target,
              _data = t.getData(),
              tooltipContent = "<table class='cust-table' border='1'><thead><th></th><th>Trip</th><th>Start Date</th><th>End Date</th></thead><tbody>"; 
              var chkBxId = 0;
              _data.forEachEntry(
              (p) => 
              { 
                tooltipContent += "<tr>";
                tooltipContent += "<td><input type='checkbox' class='checkbox' id='"+ chkBxId +"'></td>"+ "<td>"+ (chkBxId+1) +"</td>" + "<td>" + data[chkBxId].convertedStartTime + "</td><td>" + data[chkBxId].convertedEndTime + "</td>";
                tooltipContent += "</tr>";
               chkBxId++;
              }
            ); 
            tooltipContent += "</tbody></table>";

            infoBubble = new H.ui.InfoBubble(this.hereMap.screenToGeo(screenPosition.x, screenPosition.y), { content: tooltipContent, 
              onStateChange: (event) => {​​​
                this.removedDisabledGroup();
              }​​​
            });

            ui.addBubble(infoBubble);

            document.querySelectorAll('.checkbox').forEach((item: any) => {
              item.addEventListener('click', event => {
                //handle click
                this.infoBubbleCheckBoxClick(item.id, data, item.checked);
              })
            })
          });
          
          return clusterMarker;
        },
        getNoisePresentation: (noisePoint) => {
          let infoBubble: any;

          var noiseSvgString = clusterSvgTemplate.replace('{radius}', noisePoint.getWeight());
          if(data && data.length > 9){
            noiseSvgString = noiseSvgString.replace('{text}', '+');
          }else{
            noiseSvgString = noiseSvgString.replace('{text}', noisePoint.getWeight());
          }
          var w, h;
          var weight = noisePoint.getWeight();
  
          //Set cluster size depending on the weight
          if (weight <= 6)
          {
            w = 35;
            h = 35;
          }
          else if (weight <= 12) {
            w = 40;
            h = 40;
          }
          else {
            w = 50;
            h = 50;
          }

          var noiseIcon = new H.map.Icon(noiseSvgString, {
            size: { w: w, h: h },
            anchor: { x: (w/2), y: (h/2) }
          });
  
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
  
          // noiseMarker.addEventListener("tap", (event) => { 
            
          //   var point = event.target.getGeometry();
          //   var tooltipContent = ["Latitude: ", point.lat, ", Longitude: ", point.lng].join("");
  
          //   var screenPosition = this.hereMap.geoToScreen(point);
  
          //   infoBubble = new H.ui.InfoBubble(this.hereMap.screenToGeo(screenPosition.x, screenPosition.y), { content: tooltipContent });
          //   ui.addBubble(infoBubble);
          
          // });

          // noiseMarker.addEventListener("tap",  (event) => {
          //   this.removedDisabledGroup();
          //   data.forEach((element, _index) => {
          //     let liveFleetPoints: any = element.liveFleetPosition;
          //     liveFleetPoints.sort((a, b) => parseInt(a.id) - parseInt(b.id)); 
          //     this.selectionPolylineRoute(liveFleetPoints, _index);   
          //   });
          //   this.hereMap.addObject(this.disableGroup);
  
          //   var point = event.target.getGeometry(),
          //     screenPosition = this.hereMap.geoToScreen(point),
          //     t = event.target,
          //     _data = t.getData(),
          //     tooltipContent = "<table class='cust-table' border='1'><thead><th></th><th>Trip</th><th>Start Date</th><th>End Date</th></thead><tbody>"; 
          //     var chkBxId = 0;
          //     _data.forEachEntry(
          //     (p) => 
          //     { 
          //       tooltipContent += "<tr>";
          //       tooltipContent += "<td><input type='checkbox' class='checkbox' id='"+ chkBxId +"'></td>"+ "<td>"+ (chkBxId+1) +"</td>" + "<td>" + data[chkBxId].convertedStartTime + "</td><td>" + data[chkBxId].convertedEndTime + "</td>";
          //       tooltipContent += "</tr>";
          //      chkBxId++;
          //     }
          //   ); 
          //   tooltipContent += "</tbody></table>";

          //   infoBubble = new H.ui.InfoBubble(this.hereMap.screenToGeo(screenPosition.x, screenPosition.y), { content: tooltipContent, 
          //     onStateChange: (event) => {​​​
          //       this.removedDisabledGroup();
          //     }​​​
          //   });

          //   ui.addBubble(infoBubble);

          //   document.querySelectorAll('.checkbox').forEach((item: any) => {
          //     item.addEventListener('click', event => {
          //       //handle click
          //       this.infoBubbleCheckBoxClick(item.id, data, item.checked);
          //     })
          //   })
          // });
          
          // noiseMarker.addEventListener("pointerleave", (event) => { 
          //   if(infoBubble)
          //   {
          //     ui.removeBubble(infoBubble);
          //     infoBubble = null;
          //   }
          // });
          return noiseMarker;
        }
      }
    });

    // Create a layer tha will consume objects from our clustering provider
    let _markerClusterLayer = new H.map.layer.ObjectLayer(clusteredDataProvider);
    // // To make objects from clustering provder visible,
    // // we need to add our layer to the map
    this.markerClusterLayer.push(_markerClusterLayer);
    this.hereMap.addLayer(_markerClusterLayer, 100);
  }

  removedDisabledGroup(){
    this.disableGroup.removeAll();
    //this.disableGroup = null;
  }

  // function infoBubbleCheckBoxClick(chkBxId, latitude, longitude){
            //   // Get the checkbox
            //   let checkBox: any = document.getElementById(chkBxId);
            //   if (checkBox.checked == true){
            //     alert("Latitude:" + latitude + " Longitude:" + longitude + " Enabled")
            //   } else {
            //     alert("Latitude:" + latitude + " Longitude:" + longitude + " Disabled")
            //   }
            // }

  afterPlusClick(_selectedRoutes: any, _ui: any){
    this.hereMap.removeLayer(this.clusteringLayer);
    // this.hereMap.setCenter({lat: _selectedRoutes[0].startPositionLattitude, lng: _selectedRoutes[0].startPositionLongitude}, 'default');
    // this.hereMap.setZoom(10);
    if(_selectedRoutes.length > 1){
      let _arr = _selectedRoutes.filter((elem, index) => _selectedRoutes.findIndex(obj => obj.startPositionLattitude === elem.startPositionLattitude && obj.startPositionLongitude === elem.startPositionLongitude) === index);
      let _a: any = [];
      _arr.forEach(i=> {
        let b: any = _selectedRoutes.filter(j => i.startPositionLattitude == j.startPositionLattitude && i.startPositionLongitude == j.startPositionLongitude)
        _a.push(b);
      }); 
      if(_a.length > 0){
        let _check: any = false;
        _a.forEach(element => {
          if(element.length > 1){
            _check = true;
            this.setMarkerCluster(element, _ui); // cluster route marker    
          }
        });
        if(!_check){
          // TODO: cluster all element
        }
      }
    }
  }

  checkPolylineSelection(chkBxId: any, _checked: any){
    let _a = this.disableGroup.getObjects();
    if(_a && _a.length > 0){
      _a.forEach(element => {
        if((chkBxId) == element.data.id){
          element.setStyle({
              lineWidth: 4, 
              strokeColor: _checked ? 'transparent' : 'grey'
          });
        } 
      });
    }
  }

  infoBubbleCheckBoxClick(chkBxId, _data, _checked: any){
    var checkBox: any = document.getElementById(chkBxId);
    //console.log(_data)
    //if (_checked){
      //alert(" Enabled")
      //this.removedDisabledGroup();
      this.checkPolylineSelection(parseInt(chkBxId), _checked);
      // let liveFleetPoints: any = _data[parseInt(checkBox)].liveFleetPosition;
      // liveFleetPoints.sort((a, b) => parseInt(a.id) - parseInt(b.id)); 
      //this.selectionPolylineRoute(liveFleetPoints, _checked);   

      // _data.forEach((element, index) => {
      //   let liveFleetPoints: any = element.liveFleetPosition;
      //   liveFleetPoints.sort((a, b) => parseInt(a.id) - parseInt(b.id)); 
      //   let _c: any = false;
      //   if((index+1) == parseInt(chkBxId) && _checked){
      //     _c = true;
      //   }
      //   this.selectionPolylineRoute(liveFleetPoints, _c);   
      // });
    // } else {
    //   //alert(" Disabled")
    //   this.selectionPolylineRoute(_checkedData, _checked);
    // }
    //this.hereMap.addObject(this.disableGroup);
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

  // trip report data-conversion
  convertTripReportDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.convertedStartTime = this.getStartTime(element.startTimeStamp, dateFormat, timeFormat, timeZone,true);
      element.convertedEndTime = this.getEndTime(element.endTimeStamp, dateFormat, timeFormat, timeZone,true);
      element.convertedAverageWeight = this.convertWeightUnits(element.averageWeight, unitFormat, false);
      element.convertedAverageSpeed = this.convertSpeedUnits(element.averageSpeed, unitFormat);
      element.convertedFuelConsumed100Km = this.getFuelConsumedUnits(element.fuelConsumed100Km, unitFormat, true);
      element.convertedDistance = this.convertDistanceUnits(element.distance, unitFormat);
      element.convertedDrivingTime = this.getHhMmTime(element.drivingTime);
      element.convertedIdleDuration = this.getHhMmTime(element.idleDuration);
      element.convertedOdometer = this.convertDistanceUnits(element.odometer, unitFormat);
    });
    return gridData;
  }

  // fuel deviation report data-conversion 
  convertFuelDeviationDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any, translationData: any){
    let _id: number = 0;
    gridData.forEach(element => {
      let eventData: any = this.getEventTooltip(element, translationData); 
      element.id = _id++;
      element.svgIcon = eventData.svgIcon,
      element.eventTooltip = eventData.eventText,
      element.eventDate = this.getStartTime(element.eventTime, dateFormat, timeFormat, timeZone, true);
      element.convertedOdometer = this.convertDistanceUnits(element.odometer, unitFormat);
      element.convertedStartDate = this.getStartTime(element.startTimeStamp, dateFormat, timeFormat, timeZone, true);
      element.convertedEndDate = this.getEndTime(element.endTimeStamp, dateFormat, timeFormat, timeZone, true);
      element.convertedDistance = this.convertDistanceUnits(element.distance, unitFormat);
      element.convertedIdleDuration = this.getHhMmTime(element.idleDuration);
      element.convertedAverageSpeed = this.convertSpeedUnits(element.averageSpeed, unitFormat);
      element.convertedAverageWeight = this.convertWeightUnits(element.averageWeight, unitFormat, true);
      element.convertedFuelConsumed = this.getFuelConsumedUnits(element.fuelConsumed, unitFormat, false);
      element.convertedDrivingTime = this.getHhMmTime(element.drivingTime);
    });
    return gridData;
  }

  getEventTooltip(elem: any, transData: any){
    let _eventTxt: any = '';
    let _svgIcon: any = 'fuel-desc-run';
    switch(`${elem.fuelEventType}${elem.vehicleActivityType}`){
      case 'IS': {
        _eventTxt = transData.lblFuelIncreaseDuringStop || 'Fuel Increase During Stop';
        _svgIcon = 'fuel-incr-stop';
        break;
      }
      case 'DS': {
        _eventTxt = transData.lblFuelDecreaseDuringStop || 'Fuel Decrease During Stop';
        _svgIcon = 'fuel-desc-stop';
        break;
      }
      case 'IR': {
        _eventTxt = transData.lblFuelIncreaseDuringRun || 'Fuel Increase During Run';
        _svgIcon = 'fuel-incr-run';
        break;
      }
      case 'DR': {
        _eventTxt = transData.lblFuelDecreaseDuringRun || 'Fuel Decrease During Run';
        _svgIcon = 'fuel-desc-run';
        break;
      }
    }
    return { eventText: _eventTxt, svgIcon: _svgIcon };
  }

  miliLitreToLitre(_data: any){
      return (_data/1000).toFixed(2);
  }
  

  miliLitreToGallon(_data: any){
    let litre: any = this.miliLitreToLitre(_data);
    let gallon: any = litre/3.780;
    return gallon.toFixed(2);
  }

  convertFuelConsumptionMlmToLtr100km(_data: any){
    return (_data*100).toFixed(2);
  }

  convertFuelConsumptionMlmToMpg(_data: any){
    let data: any = 1.6/(_data * 3.78);
    return (data).toFixed(2);
  }

  convertKgToPound(_data: any){
    return (_data*2.2).toFixed(2);
  }

  convertKgToTonnes(_data: any){
    return (_data/1000).toFixed(2);
  }

  convertWeightUnits(data: any, unitFormat: any, tonFlag?: boolean){
    let _data: any;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _data = tonFlag ? this.convertKgToTonnes(data) : data; //-- kg/ton
        break;
      }
      case 'dunit_Imperial': {
        _data = tonFlag ? this.convertKgToTonnes(data) : this.convertKgToPound(data); //-- pound/ton
        break;
      }
      default: {
        _data = tonFlag ? this.convertKgToTonnes(data) : data; //-- kg/ton
      }
    }
    return _data;
  }

  convertSpeedUnits(data: any, unitFormat: any){
    let _data: any;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _data = this.convertSpeedMmsToKmph(data); // kmph
        break;
      }
      case 'dunit_Imperial': {
        _data = this.convertSpeedMmsToMph(data); // mph
        break;
      }
      default: {
        _data = this.convertSpeedMmsToKmph(data); // kmph
      }
    }
    return _data;
  }

  convertSpeedMmsToKmph(_data: any){
    return (_data*3600).toFixed(2);
  }

  convertSpeedMmsToMph(_data: any){
    let kmph: any = this.convertSpeedMmsToKmph(_data);
    let mph: any = kmph/1.6;
    return mph.toFixed(2);
  }

  // convertSpeedMsToKmph(){

  // }

  // convertSpeedMsToMph(){
    
  // }

  convertDistanceUnits(data: any, unitFormat: any){
    let _data: any;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _data = this.meterToKm(data); // km
        break;
      }
      case 'dunit_Imperial': {
        _data = this.meterToMile(data); // mile
        break;
      }
      default: {
        _data = this.meterToKm(data); // km
      }
    }
    return _data;
  }

  meterToKm(_data: any){
    return (_data/1000).toFixed(2);
  }

  meterToMile(_data: any){
    let km: any = this.meterToKm(_data);
    let mile = km/1.6;
    return mile.toFixed(2);
  }

  // Fleet utilisation data conversions
  getConvertedFleetDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.convertedStopTime = this.getStartTime(element.StopTime, dateFormat, timeFormat, timeZone,true);
      element.convertedAverageWeight = this.convertWeightUnits(element.averageWeightPerTrip, unitFormat, true);
      element.convertedAverageSpeed = this.convertSpeedUnits(element.averageSpeed, unitFormat);
      element.convertedAverageDistance = this.convertDistanceUnits(element.averageDistancePerDay, unitFormat);
      element.convertedDistance = this.convertDistanceUnits(element.distance, unitFormat);
      element.convertedDrivingTime = this.getHhMmTime(element.drivingTime);
      element.convertedTripTime = this.getHhMmTime(element.tripTime);
      element.convertedIdleDuration = this.getHhMmTime(element.idleDuration);
    });
    return gridData;
  }


  // Fuel Benchmarking data conversions
  getConvertedFuelBenchmarkingData(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    // gridData.forEach(element => {
      gridData = JSON.parse(gridData);
      gridData.fuelBenchmarkDetails.convertedAvgFuelConsumption = this.getFuelConsumedUnits(gridData.fuelBenchmarkDetails.averageFuelConsumption, unitFormat, true);
      gridData.fuelBenchmarkDetails.convertedTotalFuelConsumed = this.getFuelConsumedUnits(gridData.fuelBenchmarkDetails.totalFuelConsumed, unitFormat, false);
      gridData.fuelBenchmarkDetails.convertedTotalMileage = this.convertDistanceUnits(gridData.fuelBenchmarkDetails.totalMileage, unitFormat);

    // });
    if(unitFormat == 'dunit_Imperial') {
      gridData.fuelBenchmarkDetails.convertedTotalMileage = gridData.fuelBenchmarkDetails.convertedTotalMileage + " mi"
      gridData.fuelBenchmarkDetails.convertedTotalFuelConsumed = gridData.fuelBenchmarkDetails.convertedTotalFuelConsumed + " gal"
      gridData.fuelBenchmarkDetails.convertedAvgFuelConsumption = gridData.fuelBenchmarkDetails.convertedAvgFuelConsumption + " mpg"
    }else if(unitFormat == 'dunit_Metric') {
      gridData.fuelBenchmarkDetails.convertedTotalMileage = gridData.fuelBenchmarkDetails.convertedTotalMileage + " km"
      gridData.fuelBenchmarkDetails.convertedTotalFuelConsumed = gridData.fuelBenchmarkDetails.convertedTotalFuelConsumed + " ltr"
      gridData.fuelBenchmarkDetails.convertedAvgFuelConsumption = gridData.fuelBenchmarkDetails.convertedAvgFuelConsumption + " ltr/100km"
    }
    return gridData;
  }



  getConvertedFleetFuelDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.convertedAverageSpeed = this.convertSpeedUnits(element.averageSpeed, unitFormat);
      element.convertedAverageDistance = this.convertDistanceUnits(element.averageDistancePerDay, unitFormat);
      element.convertedDistance = this.convertDistanceUnits(element.distance, unitFormat);
      element.convertedIdleDuration = this.getHhMmTime(element.idleDuration);
      element.convertedFuelConsumed100Km = this.getFuelConsumptionUnits(element.fuelConsumed, unitFormat);
      element.convertedFuelConsumption = this.getFuelConsumedUnits(element.fuelConsumption, unitFormat);
      element.dpaScore = parseFloat(element.dpaScore);
      element.dpaScore = element.dpaScore.toFixed(2)*1;
      element.dpaScore = element.dpaScore.toFixed(2);

            //for 2 decimal points
            // element.convertedDistance = parseFloat(element.convertedDistance);
            // element.convertedDistance = element.convertedDistance.toFixed(2)*1;
            // element.convertedAverageDistance = parseFloat(element.convertedAverageDistance);
            // element.convertedAverageDistance = element.convertedAverageDistance.toFixed(2)*1;
            // element.convertedAverageSpeed =parseFloat(element.convertedAverageSpeed);
            // element.convertedAverageSpeed =element.convertedAverageSpeed.toFixed(2)*1;
            // element.averageGrossWeightComb =parseFloat(element.averageGrossWeightComb);
            // element.averageGrossWeightComb =element.averageGrossWeightComb.toFixed(2)*1;
            // element.fuelConsumed =parseFloat(element.fuelConsumed);
            // element.fuelConsumed =element.fuelConsumed.toFixed(2)*1;
            // element.fuelConsumption =parseFloat(element.fuelConsumption);
            // element.fuelConsumption =element.fuelConsumption.toFixed(2)*1;
            // element.cO2Emission =parseFloat(element.cO2Emission);
            // element.cO2Emission =element.cO2Emission.toFixed(2)*1;
            // element.harshBrakeDuration = parseFloat(element.harshBrakeDuration);
            // element.harshBrakeDuration =element.harshBrakeDuration.toFixed(2)*1;
            // element.heavyThrottleDuration = parseFloat(element.heavyThrottleDuration);
            // element.heavyThrottleDuration= element.heavyThrottleDuration.toFixed(2)*1;
            // element.dpaScore = parseFloat(element.dpaScore);
            // element.dpaScore = element.dpaScore.toFixed(2)*1;
    });
    return gridData;
  }
  getFuelConsumptionUnits(fuelConsumption: any, unitFormat: any, litreFlag?: boolean){
    let _fuelConsumption: any = 0;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _fuelConsumption =   this.miliLitreToLitre(fuelConsumption); //-- Ltr/100Km / ltr
        break;
      }
      case 'dunit_Imperial':{
        _fuelConsumption =  this.miliLitreToGallon(fuelConsumption); // mpg / gallon
        break;
      }
      default: {
        _fuelConsumption =  this.miliLitreToLitre(fuelConsumption); // Ltr/100Km / ltr
      }
    }
    return _fuelConsumption; 
  }

 

  getDriverTimeDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    //gridData.forEach(element => {
      gridData.driverName = gridData.driverName;
      gridData.driverId = gridData.driverId;
      gridData.startTime = this.getStartTime(gridData.startTime, dateFormat, timeFormat, timeZone);
      gridData.endTime = this.getEndTime(gridData.endTime, dateFormat, timeFormat, timeZone);
      gridData.driveTime = this.getHhMmTime(gridData.driveTime);
      gridData.workTime = this.getHhMmTime(gridData.workTime);
      gridData.serviceTime = this.getHhMmTime(gridData.serviceTime);
      gridData.restTime = this.getHhMmTime(gridData.restTime);
      gridData.availableTime = this.getHhMmTime(gridData.availableTime);
   // });
    return gridData;
  }

  
  getDriverDetailsTimeDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    let _gridData = [];
    gridData.forEach(element => {
      _gridData.push({
        driverName : element.driverName,
      driverId : element.driverId,
      startTime : this.getStartTime(element.activityDate, dateFormat, timeFormat, timeZone,false),
      driveTime : this.getHhMmTime(element.driveTime),
      workTime : this.getHhMmTime(element.workTime),
      serviceTime : this.getHhMmTime(element.serviceTime),
      restTime : this.getHhMmTime(element.restTime),
      availableTime : this.getHhMmTime(element.availableTime),
      })
      
    });
    return _gridData;
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

 


  getFuelConsumedUnits(fuelConsumed: any, unitFormat: any, litreFlag?: boolean){
    let _fuelConsumed: any = 0;
    switch(unitFormat){
      case 'dunit_Metric': { 
        _fuelConsumed = litreFlag ? this.convertFuelConsumptionMlmToLtr100km(fuelConsumed) : this.miliLitreToLitre(fuelConsumed); //-- Ltr/100Km / ltr
        break;
      }
      case 'dunit_Imperial':{
        _fuelConsumed = litreFlag ? this.convertFuelConsumptionMlmToMpg(fuelConsumed) : this.miliLitreToGallon(fuelConsumed); // mpg / gallon
        break;
      }
      default: {
        _fuelConsumed = litreFlag ? this.convertFuelConsumptionMlmToLtr100km(fuelConsumed) : this.miliLitreToLitre(fuelConsumed); // Ltr/100Km / ltr
      }
    }
    return _fuelConsumed; 
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

  convertMsToSeconds(ms: any){
    return ms/1000;
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
