import { Injectable,Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { HereService } from '../../services/here.service';
import { Util } from '../../shared/util';
import { ConfigService } from '@ngx-config/core';
import { elementEventFullName } from '@angular/compiler/src/view_compiler/view_compiler';

declare var H: any;

@Injectable({
  providedIn: 'root'
})


export class FleetMapService {
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
    this.group = new H.map.Group();
    //this.mapGroup = group;
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

  showCategoryPOI(categotyPOI: any,_ui:any){
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
          this.ui.addBubble(bubble);
        }, false);
        categoryPOIMarker.addEventListener('pointerleave', function(evt) {
          bubble.close();
        }, false);
      }
    });
    this.hereMap.addObject(this.group);

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

}
