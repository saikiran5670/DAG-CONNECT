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
  iconsGroup: any;
  startAddressPositionLat :number = 0; // = {lat : 18.50424,long : 73.85286};
  startAddressPositionLong :number = 0; // = {lat : 18.50424,long : 73.85286};
  startMarker : any;
  endMarker :any;
  rippleMarker:any;
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
  prefTimeZone: any;
  
  alertMarker : any;
  vehicleIconMarker : any;

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

  private createSVGMarker(_value,_health) {
    let healthColor = this.getHealthUpdateForDriving(_health);
    let direction = this.getDirectionIconByBearings(_value);
    let markerSvg = this.createDrivingMarkerSVG(direction,healthColor);
    let rippleSize = { w: 50, h: 50 };
    let rippleMarker = this.createRippleMarker(direction);
    const iconRipple = new H.map.DomIcon(rippleMarker, { size: rippleSize, anchor: { x:-(Math.round(rippleSize.w / 2)), y: -(Math.round(rippleSize.h / 2) )} });
    this.rippleMarker = new H.map.DomMarker({ lat:this.endAddressPositionLat, lng:this.endAddressPositionLong },{ icon:iconRipple });
 
    return `<svg width="34" height="41" viewBox="0 0 34 41" fill="none" xmlns="http://www.w3.org/2000/svg">
		<style type="text/css">.st0{fill:#FFFFFF;}.st1{fill:#1D884F;}.st2{fill:#F4C914;}.st3{fill:#176BA5;}.st4{fill:#DB4F60;}.st5{fill:#7F7F7F;}.st6{fill:#808281;}.hidden{display:none;}.cls-1{isolation:isolate;}.cls-2{opacity:0.3;mix-blend-mode:multiply;}.cls-3{fill:#fff;}.cls-4{fill:none;stroke:#db4f60;stroke-width:3px;}.cls-4,.cls-6{stroke-miterlimit:10;}.cls-5,.cls-6{fill:#db4f60;}.cls-6{stroke:#fff;}</style>
		${markerSvg}
		</svg>`;
  }
  private getDirectionIconByBearings = function (brng) {
    //var brng= 317.888;
    brng = 315;
    let iconWd = 34;
    let iconCenter = iconWd / 2;
    let iconCentery = 41 / 2;
    let rippleX = -20 ,rippleY = -25;
    let outerRotation = `rotate(0 ${iconCenter} ${iconCenter})`;

    let innerRotation = `translate(0 0) scale(1  1) rotate(0 ${iconCenter} ${iconCenter})`;
    let image = "";
    //let bearings = ["NE", "E", "SE", "S", "SW", "W", "NW", "N"];
    let bearings = ["S","SE","E","NE","N","NW","W","SW"];

    let index = -1;
    if (!isNaN(brng))
      index = brng;
    else
      return { outer: outerRotation, inner: innerRotation };
    if (index < 0)
      index = index + 360;
    index = index / 45;
    index = parseInt(index.toString());

    let direction = (bearings[index]);
    let degree = 360 - brng;
    outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(1)`; // translate(0 8) scale(1.2)
    switch (direction) {
      case "N":
        outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.8)`;
        innerRotation = `translate(13 13) scale(0.8)`;

        break;
      case "NE":
        outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.8)`; // translate(0 8) scale(1.2)
        innerRotation = `translate(9 14) scale(0.8)`;
        break;
      case "E":
        outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.)`; 
        innerRotation = `translate(6 12) scale(0.8)`;
        rippleX =  -25;
        rippleY = -25;
        break;
      case "SE":
        outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.9)`; // translate(0 8) scale(1.2)
        innerRotation = `translate(6 7) scale(0.9)`;
        rippleX =  -26;
        rippleY = -25;
        break;
      case "S":
        outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.8)`; 
        innerRotation = `translate(6 7) scale(0.8)`;
        rippleX  = -27;
        rippleY = -30;
        break;
      case "SW":
        outerRotation = `translate(4,4) rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.8)`; // translate(0 8) scale(1.2)
        innerRotation = `translate(13 8) scale(0.8)`;

        break;
      case "W":
       // outerRotation = `rotate(180 ${iconCenter} ${iconCenter}) scale(1)`; //translate(20 4) scale(1.2)
        outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.8)`;
        innerRotation = `translate(12 6) scale(0.8)`;
        rippleX  = -21;
        rippleY = -31;
        break;
      case "NW":
        outerRotation = `rotate(${degree} ${iconCenter} ${iconCenter}) scale(0.9)`; // translate(0 8) scale(1.2)
        innerRotation = `translate(10 8) scale(0.9)`;
        rippleX  = -24;
        rippleY = -28;
        break;
      default:
        break;
    }
    return { outer: outerRotation, inner: innerRotation,rippleX : rippleX,rippleY:rippleY };
  }

  private createRippleMarker(direction?){
    let rippleIcon = `<div class='rippleSVG' style='left:${direction.rippleX}px;top:${direction.rippleY}px'></div>`
   
    return rippleIcon;
  }
  private createDrivingMarkerSVG(direction: any,healthColor:any): string {
    return `
      <g id="svg_15">
			<g id="svg_1" transform="${direction.outer}">
      <path d="M32.5 16.75C32.5 29 16.75 39.5 16.75 39.5C16.75 39.5 1 29 1 16.75C1 12.5728 2.65937 8.56677 5.61307 5.61307C8.56677 2.65937 12.5728 1 16.75 1C20.9272 1 24.9332 2.65937 27.8869 5.61307C30.8406 8.56677 32.5 12.5728 32.5 16.75Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 38.625C24.1875 32.5 31.625 24.9652 31.625 16.75C31.625 8.53477 24.9652 1.875 16.75 1.875C8.53477 1.875 1.875 8.53477 1.875 16.75C1.875 24.9652 9.75 32.9375 16.75 38.625Z" fill="#00529C"/>
      <path d="M16.75 29.4375C23.9987 29.4375 29.875 23.8551 29.875 16.9688C29.875 10.0824 23.9987 4.5 16.75 4.5C9.50126 4.5 3.625 10.0824 3.625 16.9688C3.625 23.8551 9.50126 29.4375 16.75 29.4375Z" fill="white"/>
      <g clip-path="url(#clip0)">
      <path d="M11.7041 22.1148C10.8917 22.1148 10.2307 21.4539 10.2307 20.6415C10.2307 19.8291 10.8917 19.1682 11.7041 19.1682C12.5164 19.1682 13.1773 19.8291 13.1773 20.6415C13.1773 21.4539 12.5164 22.1148 11.7041 22.1148ZM11.7041 19.974C11.3359 19.974 11.0365 20.2735 11.0365 20.6416C11.0365 21.0096 11.3359 21.3091 11.7041 21.3091C12.0721 21.3091 12.3715 21.0096 12.3715 20.6416C12.3715 20.2735 12.0721 19.974 11.7041 19.974Z" fill="#00529C"/>
      <path d="M21.7961 22.1148C20.9838 22.1148 20.3228 21.4539 20.3228 20.6415C20.3228 19.8291 20.9838 19.1682 21.7961 19.1682C22.6085 19.1682 23.2694 19.8291 23.2694 20.6415C23.2694 21.4539 22.6085 22.1148 21.7961 22.1148ZM21.7961 19.974C21.4281 19.974 21.1285 20.2735 21.1285 20.6416C21.1285 21.0096 21.4281 21.3091 21.7961 21.3091C22.1642 21.3091 22.4637 21.0096 22.4637 20.6416C22.4637 20.2735 22.1642 19.974 21.7961 19.974Z" fill="#00529C"/>
      <path d="M18.819 10.5846H14.6812C14.4587 10.5846 14.2783 10.4043 14.2783 10.1817C14.2783 9.9592 14.4587 9.77881 14.6812 9.77881H18.819C19.0415 9.77881 19.2219 9.9592 19.2219 10.1817C19.2219 10.4042 19.0415 10.5846 18.819 10.5846Z" fill="#00529C"/>
      <path d="M19.6206 22.2772H13.8795C13.6569 22.2772 13.4766 22.0969 13.4766 21.8743C13.4766 21.6518 13.6569 21.4714 13.8795 21.4714H19.6206C19.8431 21.4714 20.0235 21.6518 20.0235 21.8743C20.0235 22.0968 19.8431 22.2772 19.6206 22.2772Z" fill="#00529C"/>
      <path d="M19.6206 19.8119H13.8795C13.6569 19.8119 13.4766 19.6315 13.4766 19.409C13.4766 19.1864 13.6569 19.0061 13.8795 19.0061H19.6206C19.8431 19.0061 20.0235 19.1864 20.0235 19.409C20.0235 19.6315 19.8431 19.8119 19.6206 19.8119Z" fill="#00529C"/>
      <path d="M19.6206 21.0445H13.8795C13.6569 21.0445 13.4766 20.8642 13.4766 20.6417C13.4766 20.4191 13.6569 20.2388 13.8795 20.2388H19.6206C19.8431 20.2388 20.0235 20.4191 20.0235 20.6417C20.0235 20.8642 19.8431 21.0445 19.6206 21.0445Z" fill="#00529C"/>
      <path d="M25.5346 14.0678H23.552C23.2742 14.0678 23.0491 14.2929 23.0491 14.5707V15.6681L22.7635 15.9697V10.1753C22.7635 9.20234 21.9722 8.41099 20.9993 8.41099H12.5009C11.528 8.41099 10.7365 9.20233 10.7365 10.1753V15.9696L10.451 15.6681V14.5707C10.451 14.2929 10.2259 14.0678 9.94814 14.0678H7.96539C7.68767 14.0678 7.4625 14.2929 7.4625 14.5707V15.8683C7.4625 16.1461 7.68767 16.3712 7.96539 16.3712H9.73176L10.1695 16.8335C9.49853 17.0833 9.01905 17.73 9.01905 18.4873V23.7339C9.01905 24.0117 9.24416 24.2368 9.52194 24.2368H10.1291V25.4026C10.1291 26.1947 10.7734 26.839 11.5655 26.839C12.3575 26.839 13.0018 26.1947 13.0018 25.4026V24.2368H20.4981V25.4026C20.4981 26.1947 21.1424 26.839 21.9345 26.839C22.7266 26.839 23.3709 26.1947 23.3709 25.4026V24.2368H23.9781C24.2558 24.2368 24.481 24.0117 24.481 23.7339V18.4873C24.481 17.73 24.0015 17.0834 23.3306 16.8336L23.7683 16.3712H25.5346C25.8124 16.3712 26.0375 16.1461 26.0375 15.8683V14.5707C26.0375 14.2929 25.8123 14.0678 25.5346 14.0678ZM9.4452 15.3655H8.46828V15.0736H9.4452V15.3655ZM11.7422 10.1753C11.7422 9.75712 12.0826 9.41677 12.5009 9.41677H20.9992C21.4173 9.41677 21.7576 9.75715 21.7576 10.1753V10.9469H11.7422V10.1753ZM21.7577 11.9526V16.723H17.2529V11.9526H21.7577ZM11.7422 11.9526H16.2471V16.723H11.7422V11.9526ZM11.996 25.4025C11.996 25.6399 11.8027 25.8331 11.5655 25.8331C11.3281 25.8331 11.1349 25.6399 11.1349 25.4025V24.2368H11.996V25.4025ZM22.3651 25.4025C22.3651 25.6399 22.1718 25.8331 21.9345 25.8331C21.6972 25.8331 21.5039 25.6399 21.5039 25.4025V24.2368H22.3651V25.4025ZM23.4752 18.4873V23.231H10.0248V18.4873C10.0248 18.0692 10.3652 17.7288 10.7834 17.7288H22.7166C23.1348 17.7288 23.4752 18.0692 23.4752 18.4873ZM25.0317 15.3655H24.0549V15.0736H25.0317V15.3655Z" fill="#00529C" stroke="#00529C" stroke-width="0.2"/>
      </g>
      <path d="M32.5 16.75C32.5 29 16.75 39.5 16.75 39.5C16.75 39.5 1 29 1 16.75C1 12.5728 2.65937 8.56677 5.61307 5.61307C8.56677 2.65937 12.5728 1 16.75 1C20.9272 1 24.9332 2.65937 27.8869 5.61307C30.8406 8.56677 32.5 12.5728 32.5 16.75Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
      <path d="M16.75 38.625C24.1875 32.5 31.625 24.9652 31.625 16.75C31.625 8.53477 24.9652 1.875 16.75 1.875C8.53477 1.875 1.875 8.53477 1.875 16.75C1.875 24.9652 9.75 32.9375 16.75 38.625Z" fill="${healthColor}"/>
      <path d="M16.75 29.4375C23.9987 29.4375 29.875 23.8551 29.875 16.9688C29.875 10.0824 23.9987 4.5 16.75 4.5C9.50126 4.5 3.625 10.0824 3.625 16.9688C3.625 23.8551 9.50126 29.4375 16.75 29.4375Z" fill="white"/>
      </g>
      <defs>
      <clipPath id="clip0">
      <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 8.4375)"/>
      </clipPath>
      </defs>
      <g  transform="${direction.inner}">
      <path d="M4.70411 14.1148C3.89167 14.1148 3.23071 13.4539 3.23071 12.6415C3.23071 11.8291 3.89167 11.1682 4.70411 11.1682C5.51639 11.1682 6.17729 11.8291 6.17729 12.6415C6.17729 13.4539 5.51639 14.1148 4.70411 14.1148ZM4.70411 11.974C4.33592 11.974 4.03649 12.2735 4.03649 12.6416C4.03649 13.0096 4.33592 13.3091 4.70411 13.3091C5.07214 13.3091 5.37151 13.0096 5.37151 12.6416C5.37151 12.2735 5.07208 11.974 4.70411 11.974Z" fill="${healthColor}"/>
      <path d="M14.7961 14.1148C13.9838 14.1148 13.3228 13.4539 13.3228 12.6415C13.3228 11.8291 13.9838 11.1682 14.7961 11.1682C15.6085 11.1682 16.2694 11.8291 16.2694 12.6415C16.2694 13.4539 15.6085 14.1148 14.7961 14.1148ZM14.7961 11.974C14.4281 11.974 14.1285 12.2735 14.1285 12.6416C14.1285 13.0096 14.4281 13.3091 14.7961 13.3091C15.1642 13.3091 15.4637 13.0096 15.4637 12.6416C15.4637 12.2735 15.1642 11.974 14.7961 11.974Z" fill="${healthColor}"/>
      <path d="M11.819 2.58459H7.68121C7.45865 2.58459 7.27832 2.40425 7.27832 2.1817C7.27832 1.9592 7.45865 1.77881 7.68121 1.77881H11.819C12.0415 1.77881 12.2219 1.9592 12.2219 2.1817C12.2219 2.4042 12.0415 2.58459 11.819 2.58459Z" fill="${healthColor}"/>
      <path d="M12.6206 14.2772H6.87945C6.6569 14.2772 6.47656 14.0969 6.47656 13.8743C6.47656 13.6518 6.6569 13.4714 6.87945 13.4714H12.6206C12.8431 13.4714 13.0235 13.6518 13.0235 13.8743C13.0235 14.0968 12.8431 14.2772 12.6206 14.2772Z" fill="${healthColor}"/>
      <path d="M12.6206 11.8119H6.87945C6.6569 11.8119 6.47656 11.6315 6.47656 11.409C6.47656 11.1864 6.6569 11.0061 6.87945 11.0061H12.6206C12.8431 11.0061 13.0235 11.1864 13.0235 11.409C13.0235 11.6315 12.8431 11.8119 12.6206 11.8119Z" fill="${healthColor}"/>
      <path d="M12.6206 13.0445H6.87945C6.6569 13.0445 6.47656 12.8642 6.47656 12.6417C6.47656 12.4191 6.6569 12.2388 6.87945 12.2388H12.6206C12.8431 12.2388 13.0235 12.4191 13.0235 12.6417C13.0235 12.8642 12.8431 13.0445 12.6206 13.0445Z" fill="${healthColor}"/>
      <path d="M18.5346 6.06783H16.552C16.2742 6.06783 16.0491 6.29293 16.0491 6.57072V7.66811L15.7635 7.96969V2.1753C15.7635 1.20234 14.9722 0.410986 13.9993 0.410986H5.50091C4.52796 0.410986 3.73649 1.20233 3.73649 2.1753V7.96961L3.45103 7.66811V6.57072C3.45103 6.29293 3.22593 6.06783 2.94814 6.06783H0.96539C0.687667 6.06783 0.4625 6.29292 0.4625 6.57072V7.86835C0.4625 8.14614 0.687667 8.37124 0.96539 8.37124H2.73176L3.16945 8.83351C2.49853 9.08331 2.01905 9.73 2.01905 10.4873V15.7339C2.01905 16.0117 2.24416 16.2368 2.52194 16.2368H3.12909V17.4026C3.12909 18.1947 3.77337 18.839 4.56545 18.839C5.35754 18.839 6.00181 18.1947 6.00181 17.4026V16.2368H13.4981V17.4026C13.4981 18.1947 14.1424 18.839 14.9345 18.839C15.7266 18.839 16.3709 18.1947 16.3709 17.4026V16.2368H16.9781C17.2558 16.2368 17.481 16.0117 17.481 15.7339V10.4873C17.481 9.72999 17.0015 9.08335 16.3306 8.83356L16.7683 8.37124H18.5346C18.8124 8.37124 19.0375 8.14613 19.0375 7.86835V6.57072C19.0375 6.29292 18.8123 6.06783 18.5346 6.06783ZM2.4452 7.36546H1.46828V7.07361H2.4452V7.36546ZM4.74222 2.1753C4.74222 1.75712 5.08264 1.41677 5.50085 1.41677H13.9992C14.4173 1.41677 14.7576 1.75715 14.7576 2.1753V2.94688H4.74222V2.1753ZM14.7577 3.95261V8.72298H10.2529V3.95261H14.7577ZM4.74222 3.95261H9.24711V8.72298H4.74222V3.95261ZM4.99603 17.4025C4.99603 17.6399 4.80273 17.8331 4.56545 17.8331C4.32813 17.8331 4.13487 17.6399 4.13487 17.4025V16.2368H4.99603V17.4025ZM15.3651 17.4025C15.3651 17.6399 15.1718 17.8331 14.9345 17.8331C14.6972 17.8331 14.5039 17.6399 14.5039 17.4025V16.2368H15.3651V17.4025ZM16.4752 10.4873V15.231H3.02483V10.4873C3.02483 10.0692 3.36522 9.72881 3.78336 9.72881H15.7166C16.1348 9.72881 16.4752 10.0692 16.4752 10.4873ZM18.0317 7.36546H17.0549V7.07361H18.0317V7.36546Z" fill="${healthColor}" stroke="${healthColor}" stroke-width="0.2"/>
      
    </g>
		
		</g>`;
  }

    viewSelectedRoutes(_selectedRoutes: any, _ui: any, trackType?: any, _displayRouteView?: any, _displayPOIList?: any, _searchMarker?: any, _herePOI?: any,alertsChecked?: boolean,showIcons?:boolean){
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
    if(showIcons && _selectedRoutes && _selectedRoutes.length > 0){
      this.drawIcons(_selectedRoutes,_ui);
    
      this.hereMap.addObject(this.group);
      
      //this.makeCluster(_selectedRoutes, _ui);
    }
    else if(!showIcons && _selectedRoutes && _selectedRoutes.length > 0){
      _selectedRoutes.forEach(elem => {
        this.startAddressPositionLat = elem.startPositionLattitude;
        this.startAddressPositionLong = elem.startPositionLongitude;
        this.endAddressPositionLat= elem.latestReceivedPositionLattitude;
        this.endAddressPositionLong= elem.latestReceivedPositionLongitude;
        this.corridorWidth = 1000; //- hard coded
        this.corridorWidthKm = this.corridorWidth/1000;
        let houseMarker = this.createHomeMarker();
        let markerSize = { w: 26, h: 32 };
        const icon = new H.map.Icon(houseMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.startMarker = new H.map.Marker({ lat:this.startAddressPositionLat, lng:this.startAddressPositionLong },{ icon:icon });
         
      
        let endMarkerSize = { w: 34, h: 40 };;
        let endMarker = this.createSVGMarker(elem.latestReceivedPositionHeading,elem.vehicleHealthStatusType);
        const iconEnd = new H.map.Icon(endMarker, { size: endMarkerSize, anchor: { x: Math.round(endMarkerSize.w / 2), y: Math.round(endMarkerSize.h / 2) } });
        this.endMarker = new H.map.Marker({ lat:this.endAddressPositionLat, lng:this.endAddressPositionLong },{ icon:iconEnd });
        this.group.addObjects([this.startMarker,this.rippleMarker, this.endMarker]);
        // end start marker
        // var startBubble;
        // this.startMarker.addEventListener('pointerenter', function (evt) {
        //   // event target is the marker itself, group is a parent event target
        //   // for all objects that it contains
        //   startBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
        //     // read custom data
        //     content:`<table style='width: 350px;'>
        //       <tr>
        //         <td style='width: 100px;'>Start Location:</td> <td><b>${elem.startPosition}</b></td>
        //       </tr>
        //       <tr>
        //         <td style='width: 100px;'>Start Date:</td> <td><b>${elem.convertedStartTime}</b></td>
        //       </tr>
        //       <tr>
        //         <td style='width: 100px;'>Total Alerts:</td> <td><b>${elem.alert}</b></td>
        //       </tr>
        //     </table>`
        //   });
        //   // show info bubble
        //   _ui.addBubble(startBubble);
        // }, false);
        // this.startMarker.addEventListener('pointerleave', function(evt) {
        //   startBubble.close();
        // }, false);

        // var endBubble;
        // this.endMarker.addEventListener('pointerenter', function (evt) {
        //   // event target is the marker itself, group is a parent event target
        //   // for all objects that it contains
        //   endBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
        //     // read custom data
        //     content:`<table style='width: 350px;'>
        //       <tr>
        //         <td style='width: 100px;'>End Location:</td> <td><b>${elem.endPosition}</b></td>
        //       </tr>
        //       <tr>
        //         <td style='width: 100px;'>End Date:</td> <td><b>${elem.convertedEndTime}</b></td>
        //       </tr>
        //       <tr>
        //         <td style='width: 100px;'>Total Alerts:</td> <td><b>${elem.alert}</b></td>
        //       </tr>
        //     </table>`
        //   });
        //   // show info bubble
        //   _ui.addBubble(endBubble);
        // }, false);
        // this.endMarker.addEventListener('pointerleave', function(evt) {
        //   endBubble.close();
        // }, false);

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
        
      if(alertsChecked){
        if(elem.fleetOverviewAlert.length > 0){
          this.drawAlerts(elem.fleetOverviewAlert,_ui);
        }
      }
        this.hereMap.addObject(this.group);
        this.hereMap.setCenter({lat: this.startAddressPositionLat, lng: this.startAddressPositionLong}, 'default');
        
      });
   
      this.makeCluster(_selectedRoutes, _ui);
    }else{
      if(_displayPOIList.length > 0 || (_searchMarker && _searchMarker.lat && _searchMarker.lng) || (_herePOI && _herePOI.length > 0)){
        this.hereMap.addObject(this.group);
      }
    }
   }

   drawAlerts(_alertDetails,_ui){
    if(_alertDetails.length > 0){
      let _fillColor = '#D50017';
      let _level = 'Critical';
      let _type = 'Logistics Alerts';
      let alertList  = _alertDetails.map(data=>data.alertId);
      let distinctAlert = alertList.filter((value, index, self) => self.indexOf(value) === index);
      let finalAlerts = [];
      distinctAlert.forEach(element => {
        let _currentElem = _alertDetails.find(item=> item.level === 'C' && item.alertId === element);
        if(_currentElem == undefined){
         _currentElem = _alertDetails.find(item=> item.alertId === element);

        }
        finalAlerts.push(_currentElem);

      });
      finalAlerts.forEach(element => {
        switch (element.level) {
          case 'C':
            case 'Critical':{
            _fillColor = '#D50017';
            _level = 'Critical'
          }
          break;
          case 'W':
            case 'Warning':{
            _fillColor = '#FC5F01';
            _level = 'Warning'
          }
          break;
          case 'A':
            case 'Advisory':{
            _fillColor = '#FFD80D';
            _level = 'Advisory'
          }
          break;
          default:
            break;
        }
        switch (element.type) {
          case 'L':
            case 'Logistics Alerts':{
            _type = 'Logistics Alerts'
          }
          break;
          case 'F':
            case 'Fuel and Driver Performance':{
            _type='Fuel and Driver Performance'
          }
          break;
          case 'R':
            case 'Repair and Maintenance':{
            _type='Repair and Maintenance'

          }
          break;
          default:
            break;
        }
        let _alertMarker = `<svg width="23" height="20" viewBox="0 0 23 20" fill="none" xmlns="http://www.w3.org/2000/svg">
        <mask id="path-1-outside-1" maskUnits="userSpaceOnUse" x="0.416748" y="0.666748" width="23" height="19" fill="black">
        <rect fill="white" x="0.416748" y="0.666748" width="23" height="19"/>
        <path d="M11.7501 4.66675L4.41675 17.3334H19.0834L11.7501 4.66675Z"/>
        </mask>
        <path d="M11.7501 4.66675L4.41675 17.3334H19.0834L11.7501 4.66675Z" fill="${_fillColor}"/>
        <path d="M11.7501 4.66675L13.4809 3.66468L11.7501 0.675021L10.0192 3.66468L11.7501 4.66675ZM4.41675 17.3334L2.6859 16.3313L0.947853 19.3334H4.41675V17.3334ZM19.0834 17.3334V19.3334H22.5523L20.8143 16.3313L19.0834 17.3334ZM10.0192 3.66468L2.6859 16.3313L6.1476 18.3355L13.4809 5.66882L10.0192 3.66468ZM4.41675 19.3334H19.0834V15.3334H4.41675V19.3334ZM20.8143 16.3313L13.4809 3.66468L10.0192 5.66882L17.3526 18.3355L20.8143 16.3313Z" fill="white" mask="url(#path-1-outside-1)"/>
        <path d="M12.4166 14H11.0833V15.3333H12.4166V14Z" fill="white"/>
        <path d="M12.4166 10H11.0833V12.6667H12.4166V10Z" fill="white"/>
        </svg>
        `
        let markerSize = { w: 23, h: 20 };
        const icon = new H.map.Icon(_alertMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
        this.alertMarker = new H.map.Marker({ lat:element.latitude, lng: element.longitude },{ icon:icon });
        this.group.addObject(this.alertMarker);
        let _time = Util.convertUtcToDateFormat(element.time,'DD/MM/YYYY hh:mm:ss');

        //alert tooltip
        var startBubble;
        this.alertMarker.addEventListener('pointerenter', function (evt) {
          // event target is the marker itself, group is a parent event target
          // for all objects that it contains
          startBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
            // read custom data
            content:`<table style='width: 300px; font-size:12px;'>
              <tr>
                <td style='width: 100px;'>Alert Name:</td> <td><b>${element.name}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>Alert Type:</td> <td><b>${_type}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>Alert Level:</td> <td><b>${_level}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>Alert Location:</td> <td><b>${element.geolocationAddress}</b></td>
              </tr>
              <tr>
                <td style='width: 100px;'>Alert Time:</td> <td><b>${_time}</b></td>
              </tr>
            </table>`
          });
          // show info bubble
          _ui.addBubble(startBubble);
        }, false);
        this.alertMarker.addEventListener('pointerleave', function(evt) {
          startBubble.close();
        }, false);

      });
    }
   }

   setMapToLocation(_position){
    this.hereMap.setCenter({lat: _position.lat, lng: _position.lng}, 'default');

   }
   
   drawIcons(_selectedRoutes,_ui){
    _selectedRoutes.forEach(elem => {
      this.startAddressPositionLat = elem.startPositionLattitude;
      this.startAddressPositionLong = elem.startPositionLongitude;
      this.endAddressPositionLat= elem.latestReceivedPositionLattitude;
      this.endAddressPositionLong= elem.latestReceivedPositionLongitude;
      let _vehicleMarkerDetails = this.setIconsOnMap(elem,_ui);
      let _vehicleMarker = _vehicleMarkerDetails['icon'];
      let _alertConfig = _vehicleMarkerDetails['alertConfig'];
      let _type = 'No Warning';
      if(_alertConfig){
        _type = _alertConfig.type;
      }
      let markerSize = { w: 34, h: 40 };
      let icon = new H.map.Icon(_vehicleMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      this.vehicleIconMarker = new H.map.Marker({ lat:this.endAddressPositionLat, lng:this.endAddressPositionLong },{ icon:icon });
    
      this.group.addObject(this.vehicleIconMarker);
      let _healthStatus = '',_drivingStatus = '';
      // icon tooltip
      switch (elem.vehicleHealthStatusType) {
        case 'T': // stop now;
        case 'Stop Now':
          _healthStatus = 'Stop Now';
          break;
        case 'V': // service now;
        case 'Service Now':
          _healthStatus = 'Service Now';
          break;
        case 'N': // no action;
        case 'No Action':
          _healthStatus = 'No Action';
          break
        default:
          break;
      }
      switch (elem.vehicleDrivingStatusType) {
        case 'N': 
        case 'Never Moved':
          _drivingStatus = 'Never Moved';
          break;
        case 'D':
          case 'Driving':
          _drivingStatus = 'Driving';
          break;
        case 'I': // no action;
        case 'Idle':
          _drivingStatus = 'Idle';
          break;
        case 'U': // no action;
        case 'Unknown':
          _drivingStatus = 'Unknown';
          break;
        case 'S': // no action;
        case 'Stopped':
          _drivingStatus = 'Stopped';
          break
        
        default:
          break;
      }
      let activatedTime = Util.convertUtcToDateFormat(elem.startTimeStamp,'DD/MM/YYYY hh:mm:ss');
      let _driverName = elem.driverName ? elem.driverName : elem.driver1Id;
      let _vehicleName = elem.vid ? elem.vid : elem.vin;
      let iconBubble;
      this.vehicleIconMarker.addEventListener('pointerenter', function (evt) {
        // event target is the marker itself, group is a parent event target
        // for all objects that it contains
        iconBubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
          // read custom data
          content:`<table style='width: 300px; font-size:12px;'>
            <tr>
              <td style='width: 100px;'>Vehicle:</td> <td><b>${_vehicleName}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Driving Status:</td> <td><b>${_drivingStatus}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Current Mileage:</td> <td><b>${elem.odometerVal}</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Next Service in:</td> <td><b>-${elem.distanceUntilNextService} km</b></td>
            </tr>
            <tr>
              <td style='width: 100px;'>Health Status:</td> <td><b>${_healthStatus}</b></td>
            </tr>
            <tr class='warningClass'>
              <td style='width: 100px;'>Warning Name:</td> <td><b>${_type}</b></td>
            </tr>
            <tr>
            <td style='width: 100px;'>Activated Time:</td> <td><b>${activatedTime}</b></td>
            </tr>
            <tr>
            <td style='width: 100px;'>Driver Name:</td> <td><b>${_driverName}</b></td>
            </tr>
          </table>`
        });
        // show info bubble
        _ui.addBubble(iconBubble);
      }, false);
      this.vehicleIconMarker.addEventListener('pointerleave', function(evt) {
        iconBubble.close();
      }, false);
    });
    
      
   }

  setIconsOnMap(element,_ui) {
    let _drivingStatus = false;
    let healthColor = '#606060';
    let _alertConfig = undefined;
    element.vehicleDrivingStatusType = 'D'
    if (element.vehicleDrivingStatusType === 'D' || element.vehicleDrivingStatusType === 'Driving') {
      _drivingStatus = true
    }
    switch (element.vehicleHealthStatusType) {
      case 'T': // stop now;
      case 'Stop Now':
        healthColor = '#D50017'; //red
        break;
      case 'V': // service now;
      case 'Service Now':
        healthColor = '#FC5F01'; //orange
        break;
      case 'N': // no action;
      case 'No Action':
        healthColor = '#606060'; //grey
        if (_drivingStatus) {
          healthColor = '#00AE10'; //green
        }
        break
      default:
        break;
    }
    let _vehicleIcon : any;
    // if(_drivingStatus){

    //   let direction = this.getDirectionIconByBearings(element.latestReceivedPositionHeading);
    //   let markerSvg = this.createDrivingMarkerSVG(direction,healthColor);
      
    //   if(element.vehicleDrivingStatusType === 'D' || element.vehicleDrivingStatusType === 'Driving'){
        
    //     let rippleSize = { w: 50, h: 50 };
    //     let rippleMarker = this.createRippleMarker(direction);
    //     const iconRipple = new H.map.DomIcon(rippleMarker, { size: rippleSize, anchor: { x:(Math.round(rippleSize.w / 2)), y: (Math.round(rippleSize.h / 2) )} });
    //     this.rippleMarker = new H.map.DomMarker({ lat:element.latestReceivedPositionLattitude, lng:element.latestReceivedPositionLongitude },{ icon:iconRipple });
    //     this.group.addObject(this.rippleMarker);
  
    //     }
    //   _vehicleIcon =  `<svg width="34" height="40" viewBox="0 0 34 40" fill="none" xmlns="http://www.w3.org/2000/svg">
    //   <style type="text/css">.st0{fill:#FFFFFF;}.st1{fill:#1D884F;}.st2{fill:#F4C914;}.st3{fill:#176BA5;}.st4{fill:#DB4F60;}.st5{fill:#7F7F7F;}.st6{fill:#808281;}.hidden{display:none;}.cls-1{isolation:isolate;}.cls-2{opacity:0.3;mix-blend-mode:multiply;}.cls-3{fill:#fff;}.cls-4{fill:none;stroke:#db4f60;stroke-width:3px;}.cls-4,.cls-6{stroke-miterlimit:10;}.cls-5,.cls-6{fill:#db4f60;}.cls-6{stroke:#fff;}</style>
    //   ${markerSvg}
    //   </svg>`;

    
    // }
    // else{
      let _alertFound = undefined ;
      
      if(element.fleetOverviewAlert.length > 0){
        _alertFound = element.fleetOverviewAlert.find(item=>{
          item.latitude == element.latestReceivedPositionLattitude && item.longitude == element.latestReceivedPositionLongitude})
      }
      
      if(_alertFound){
        _alertConfig = this.getAlertConfig(_alertFound);
        _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M32.5 24.75C32.5 37 16.75 47.5 16.75 47.5C16.75 47.5 1 37 1 24.75C1 20.5728 2.65937 16.5668 5.61307 13.6131C8.56677 10.6594 12.5728 9 16.75 9C20.9272 9 24.9332 10.6594 27.8869 13.6131C30.8406 16.5668 32.5 20.5728 32.5 24.75Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M16.75 46.625C24.1875 40.5 31.625 32.9652 31.625 24.75C31.625 16.5348 24.9652 9.875 16.75 9.875C8.53477 9.875 1.875 16.5348 1.875 24.75C1.875 32.9652 9.75 40.9375 16.75 46.625Z" fill="${healthColor}"/>
        <path d="M16.75 37.4375C23.9987 37.4375 29.875 31.8551 29.875 24.9688C29.875 18.0824 23.9987 12.5 16.75 12.5C9.50126 12.5 3.625 18.0824 3.625 24.9688C3.625 31.8551 9.50126 37.4375 16.75 37.4375Z" fill="white"/>
        <g clip-path="url(#clip0)">
        <path d="M11.7041 30.1148C10.8917 30.1148 10.2307 29.4539 10.2307 28.6415C10.2307 27.8291 10.8917 27.1682 11.7041 27.1682C12.5164 27.1682 13.1773 27.8291 13.1773 28.6415C13.1773 29.4539 12.5164 30.1148 11.7041 30.1148ZM11.7041 27.974C11.3359 27.974 11.0365 28.2735 11.0365 28.6416C11.0365 29.0096 11.3359 29.3091 11.7041 29.3091C12.0721 29.3091 12.3715 29.0096 12.3715 28.6416C12.3715 28.2735 12.0721 27.974 11.7041 27.974Z" fill="${healthColor}"/>
        <path d="M21.7961 30.1148C20.9838 30.1148 20.3228 29.4539 20.3228 28.6415C20.3228 27.8291 20.9838 27.1682 21.7961 27.1682C22.6085 27.1682 23.2694 27.8291 23.2694 28.6415C23.2694 29.4539 22.6085 30.1148 21.7961 30.1148ZM21.7961 27.974C21.4281 27.974 21.1285 28.2735 21.1285 28.6416C21.1285 29.0096 21.4281 29.3091 21.7961 29.3091C22.1642 29.3091 22.4637 29.0096 22.4637 28.6416C22.4637 28.2735 22.1642 27.974 21.7961 27.974Z" fill="${healthColor}"/>
        <path d="M18.819 18.5846H14.6812C14.4587 18.5846 14.2783 18.4043 14.2783 18.1817C14.2783 17.9592 14.4587 17.7788 14.6812 17.7788H18.819C19.0415 17.7788 19.2219 17.9592 19.2219 18.1817C19.2219 18.4042 19.0415 18.5846 18.819 18.5846Z" fill="${healthColor}"/>
        <path d="M19.6206 30.2772H13.8795C13.6569 30.2772 13.4766 30.0969 13.4766 29.8743C13.4766 29.6518 13.6569 29.4714 13.8795 29.4714H19.6206C19.8431 29.4714 20.0235 29.6518 20.0235 29.8743C20.0235 30.0968 19.8431 30.2772 19.6206 30.2772Z" fill="${healthColor}"/>
        <path d="M19.6206 27.8119H13.8795C13.6569 27.8119 13.4766 27.6315 13.4766 27.409C13.4766 27.1864 13.6569 27.0061 13.8795 27.0061H19.6206C19.8431 27.0061 20.0235 27.1864 20.0235 27.409C20.0235 27.6315 19.8431 27.8119 19.6206 27.8119Z" fill="${healthColor}"/>
        <path d="M19.6206 29.0445H13.8795C13.6569 29.0445 13.4766 28.8642 13.4766 28.6417C13.4766 28.4191 13.6569 28.2388 13.8795 28.2388H19.6206C19.8431 28.2388 20.0235 28.4191 20.0235 28.6417C20.0235 28.8642 19.8431 29.0445 19.6206 29.0445Z" fill="${healthColor}"/>
        <path d="M25.5346 22.0678H23.552C23.2742 22.0678 23.0491 22.2929 23.0491 22.5707V23.6681L22.7635 23.9697V18.1753C22.7635 17.2023 21.9722 16.411 20.9993 16.411H12.5009C11.528 16.411 10.7365 17.2023 10.7365 18.1753V23.9696L10.451 23.6681V22.5707C10.451 22.2929 10.2259 22.0678 9.94814 22.0678H7.96539C7.68767 22.0678 7.4625 22.2929 7.4625 22.5707V23.8683C7.4625 24.1461 7.68767 24.3712 7.96539 24.3712H9.73176L10.1695 24.8335C9.49853 25.0833 9.01905 25.73 9.01905 26.4873V31.7339C9.01905 32.0117 9.24416 32.2368 9.52194 32.2368H10.1291V33.4026C10.1291 34.1947 10.7734 34.839 11.5655 34.839C12.3575 34.839 13.0018 34.1947 13.0018 33.4026V32.2368H20.4981V33.4026C20.4981 34.1947 21.1424 34.839 21.9345 34.839C22.7266 34.839 23.3709 34.1947 23.3709 33.4026V32.2368H23.9781C24.2558 32.2368 24.481 32.0117 24.481 31.7339V26.4873C24.481 25.73 24.0015 25.0834 23.3306 24.8336L23.7683 24.3712H25.5346C25.8124 24.3712 26.0375 24.1461 26.0375 23.8683V22.5707C26.0375 22.2929 25.8123 22.0678 25.5346 22.0678ZM9.4452 23.3655H8.46828V23.0736H9.4452V23.3655ZM11.7422 18.1753C11.7422 17.7571 12.0826 17.4168 12.5009 17.4168H20.9992C21.4173 17.4168 21.7576 17.7571 21.7576 18.1753V18.9469H11.7422V18.1753ZM21.7577 19.9526V24.723H17.2529V19.9526H21.7577ZM11.7422 19.9526H16.2471V24.723H11.7422V19.9526ZM11.996 33.4025C11.996 33.6399 11.8027 33.8331 11.5655 33.8331C11.3281 33.8331 11.1349 33.6399 11.1349 33.4025V32.2368H11.996V33.4025ZM22.3651 33.4025C22.3651 33.6399 22.1718 33.8331 21.9345 33.8331C21.6972 33.8331 21.5039 33.6399 21.5039 33.4025V32.2368H22.3651V33.4025ZM23.4752 26.4873V31.231H10.0248V26.4873C10.0248 26.0692 10.3652 25.7288 10.7834 25.7288H22.7166C23.1348 25.7288 23.4752 26.0692 23.4752 26.4873ZM25.0317 23.3655H24.0549V23.0736H25.0317V23.3655Z" fill="#D50017" stroke="#D50017" stroke-width="0.2"/>
        </g>
        <mask id="path-11-outside-1" maskUnits="userSpaceOnUse" x="17.6667" y="0.666748" width="23" height="19" fill="black">
        <rect fill="white" x="17.6667" y="0.666748" width="23" height="19"/>
        <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z"/>
        </mask>
        <path d="M29.0001 4.66675L21.6667 17.3334H36.3334L29.0001 4.66675Z" fill="${_alertConfig.color}"/>
        <path d="M29.0001 4.66675L30.7309 3.66468L29.0001 0.675021L27.2692 3.66468L29.0001 4.66675ZM21.6667 17.3334L19.9359 16.3313L18.1979 19.3334H21.6667V17.3334ZM36.3334 17.3334V19.3334H39.8023L38.0643 16.3313L36.3334 17.3334ZM27.2692 3.66468L19.9359 16.3313L23.3976 18.3355L30.7309 5.66882L27.2692 3.66468ZM21.6667 19.3334H36.3334V15.3334H21.6667V19.3334ZM38.0643 16.3313L30.7309 3.66468L27.2692 5.66882L34.6026 18.3355L38.0643 16.3313Z" fill="white" mask="url(#path-11-outside-1)"/>
        <path d="M29.6666 14H28.3333V15.3333H29.6666V14Z" fill="white"/>
        <path d="M29.6666 10H28.3333V12.6667H29.6666V10Z" fill="white"/>
        <defs>
        <clipPath id="clip0">
        <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 16.4375)"/>
        </clipPath>
        </defs>
        </svg>`;
      }
      else{
        _vehicleIcon = `<svg width="40" height="49" viewBox="0 0 40 49" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M32.5 17.5C32.5 29.75 16.75 40.25 16.75 40.25C16.75 40.25 1 29.75 1 17.5C1 13.3228 2.65937 9.31677 5.61307 6.36307C8.56677 3.40937 12.5728 1.75 16.75 1.75C20.9272 1.75 24.9332 3.40937 27.8869 6.36307C30.8406 9.31677 32.5 13.3228 32.5 17.5Z" stroke="${healthColor}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M16.75 39.375C24.1875 33.25 31.625 25.7152 31.625 17.5C31.625 9.28475 24.9652 2.62498 16.75 2.62498C8.53477 2.62498 1.875 9.28475 1.875 17.5C1.875 25.7152 9.75 33.6875 16.75 39.375Z" fill="${healthColor}"/>
        <path d="M16.75 30.1875C23.9987 30.1875 29.875 24.605 29.875 17.7187C29.875 10.8324 23.9987 5.24998 16.75 5.24998C9.50126 5.24998 3.625 10.8324 3.625 17.7187C3.625 24.605 9.50126 30.1875 16.75 30.1875Z" fill="white"/>
        <g clip-path="url(#clip0)">
        <path d="M11.7041 22.8649C10.8917 22.8649 10.2307 22.2039 10.2307 21.3916C10.2307 20.5792 10.8917 19.9183 11.7041 19.9183C12.5164 19.9183 13.1773 20.5792 13.1773 21.3916C13.1773 22.204 12.5164 22.8649 11.7041 22.8649ZM11.7041 20.7241C11.3359 20.7241 11.0365 21.0235 11.0365 21.3916C11.0365 21.7597 11.3359 22.0591 11.7041 22.0591C12.0721 22.0591 12.3715 21.7597 12.3715 21.3916C12.3715 21.0235 12.0721 20.7241 11.7041 20.7241Z" fill="${healthColor}"/>
        <path d="M21.7961 22.8649C20.9838 22.8649 20.3228 22.2039 20.3228 21.3916C20.3228 20.5792 20.9838 19.9183 21.7961 19.9183C22.6085 19.9183 23.2694 20.5792 23.2694 21.3916C23.2694 22.204 22.6085 22.8649 21.7961 22.8649ZM21.7961 20.7241C21.4281 20.7241 21.1285 21.0235 21.1285 21.3916C21.1285 21.7597 21.4281 22.0591 21.7961 22.0591C22.1642 22.0591 22.4637 21.7597 22.4637 21.3916C22.4637 21.0235 22.1642 20.7241 21.7961 20.7241Z" fill="${healthColor}"/>
        <path d="M18.819 11.3345H14.6812C14.4587 11.3345 14.2783 11.1542 14.2783 10.9317C14.2783 10.7092 14.4587 10.5288 14.6812 10.5288H18.819C19.0415 10.5288 19.2219 10.7092 19.2219 10.9317C19.2219 11.1542 19.0415 11.3345 18.819 11.3345Z" fill="${healthColor}"/>
        <path d="M19.6206 23.0272H13.8795C13.6569 23.0272 13.4766 22.8468 13.4766 22.6243C13.4766 22.4018 13.6569 22.2214 13.8795 22.2214H19.6206C19.8431 22.2214 20.0235 22.4018 20.0235 22.6243C20.0235 22.8468 19.8431 23.0272 19.6206 23.0272Z" fill="${healthColor}"/>
        <path d="M19.6206 20.5619H13.8795C13.6569 20.5619 13.4766 20.3815 13.4766 20.159C13.4766 19.9364 13.6569 19.7561 13.8795 19.7561H19.6206C19.8431 19.7561 20.0235 19.9364 20.0235 20.159C20.0235 20.3815 19.8431 20.5619 19.6206 20.5619Z" fill="${healthColor}"/>
        <path d="M19.6206 21.7945H13.8795C13.6569 21.7945 13.4766 21.6142 13.4766 21.3916C13.4766 21.1691 13.6569 20.9887 13.8795 20.9887H19.6206C19.8431 20.9887 20.0235 21.1691 20.0235 21.3916C20.0235 21.6142 19.8431 21.7945 19.6206 21.7945Z" fill="${healthColor}"/>
        <path d="M25.5346 14.8178H23.552C23.2742 14.8178 23.0491 15.0429 23.0491 15.3207V16.4181L22.7635 16.7197V10.9253C22.7635 9.95231 21.9722 9.16096 20.9993 9.16096H12.5009C11.528 9.16096 10.7365 9.9523 10.7365 10.9253V16.7196L10.451 16.4181V15.3207C10.451 15.0429 10.2259 14.8178 9.94814 14.8178H7.96539C7.68767 14.8178 7.4625 15.0429 7.4625 15.3207V16.6183C7.4625 16.8961 7.68767 17.1212 7.96539 17.1212H9.73176L10.1695 17.5835C9.49853 17.8333 9.01905 18.48 9.01905 19.2373V24.4839C9.01905 24.7617 9.24416 24.9868 9.52194 24.9868H10.1291V26.1526C10.1291 26.9447 10.7734 27.5889 11.5655 27.5889C12.3575 27.5889 13.0018 26.9447 13.0018 26.1526V24.9868H20.4981V26.1526C20.4981 26.9447 21.1424 27.5889 21.9345 27.5889C22.7266 27.5889 23.3709 26.9447 23.3709 26.1526V24.9868H23.9781C24.2558 24.9868 24.481 24.7617 24.481 24.4839V19.2373C24.481 18.48 24.0015 17.8333 23.3306 17.5835L23.7683 17.1212H25.5346C25.8124 17.1212 26.0375 16.8961 26.0375 16.6183V15.3207C26.0375 15.0429 25.8123 14.8178 25.5346 14.8178ZM9.4452 16.1154H8.46828V15.8236H9.4452V16.1154ZM11.7422 10.9253C11.7422 10.5071 12.0826 10.1667 12.5009 10.1667H20.9992C21.4173 10.1667 21.7576 10.5071 21.7576 10.9253V11.6969H11.7422V10.9253ZM21.7577 12.7026V17.4729H17.2529V12.7026H21.7577ZM11.7422 12.7026H16.2471V17.4729H11.7422V12.7026ZM11.996 26.1525C11.996 26.3898 11.8027 26.5831 11.5655 26.5831C11.3281 26.5831 11.1349 26.3898 11.1349 26.1525V24.9867H11.996V26.1525ZM22.3651 26.1525C22.3651 26.3898 22.1718 26.5831 21.9345 26.5831C21.6972 26.5831 21.5039 26.3898 21.5039 26.1525V24.9867H22.3651V26.1525ZM23.4752 19.2373V23.981H10.0248V19.2373C10.0248 18.8191 10.3652 18.4788 10.7834 18.4788H22.7166C23.1348 18.4788 23.4752 18.8191 23.4752 19.2373ZM25.0317 16.1154H24.0549V15.8236H25.0317V16.1154Z" fill="${healthColor}" stroke="${healthColor}" stroke-width="0.2"/>
        </g>
        <defs>
        <clipPath id="clip0">
        <rect width="18.375" height="18.375" fill="white" transform="translate(7.5625 9.18748)"/>
        </clipPath>
        </defs>
        </svg>`
      }
    
   // }
    return {icon: _vehicleIcon,alertConfig:_alertConfig};
  }

  getAlertConfig(_currentAlert){
    let _alertConfig = {color : '#D50017' , level :'Critical', type : ''};
    let _fillColor = '#D50017';
    let _level = 'Critical';
    let _type = '';
      switch (_currentAlert.level) {
        case 'C':
          case 'Critical':{
          _fillColor = '#D50017';
          _level = 'Critical'
        }
        break;
        case 'W':
          case 'Warning':{
          _fillColor = '#FC5F01';
          _level = 'Warning'
        }
        break;
        case 'A':
          case 'Advisory':{
          _fillColor = '#FFD80D';
          _level = 'Advisory'
        }
        break;
        default:
          break;
      }
      switch (_currentAlert.categoryType) {
        case 'L':
          case 'Logistics Alerts':{
          _type = 'Logistics Alerts'
        }
        break;
        case 'F':
          case 'Fuel and Driver Performance':{
          _type='Fuel and Driver Performance'
        }
        break;
        case 'R':
          case 'Repair and Maintenance':{
          _type='Repair and Maintenance'

        }
        break;
        default:
          break;
      }
      return {color : _fillColor , level : _level, type : _type};
  }

  getHealthUpdateForDriving(_health){
    let  healthColor = '#D50017';
    switch (_health) {
      case 'T': // stop now;
      case 'Stop Now':
        healthColor = '#D50017'; //red
        break;
      case 'V': // service now;
      case 'Service Now':
        healthColor = '#FC5F01'; //orange
        break;
      case 'N': // no action;
      case 'No Action':
          healthColor = '#00AE10'; //green
        break
      default:
        break;
    }
    return healthColor;
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
    this.hereMap.setCenter({lat: _selectedRoutes[0].startPositionLattitude, lng: _selectedRoutes[0].startPositionLongitude}, 'default');
    this.hereMap.setZoom(10);
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

  getConvertedFleetFuelDataBasedOnPref(gridData: any, dateFormat: any, timeFormat: any, unitFormat: any, timeZone: any){
    gridData.forEach(element => {
      element.convertedAverageSpeed = this.getAvergSpeed(element.averageSpeed, unitFormat);
      element.convertedAverageDistance = this.getDistance(element.averageDistancePerDay, unitFormat);
      element.convertedDistance = this.getDistance(element.distance, unitFormat);
      element.convertedIdleDuration = this.getHhMmTime(element.idleDuration);
      element.convertedFuelConsumed100Km = this.getFuelConsumed(element.fuelConsumption, unitFormat);
      // element.dpaScore = element.dpaScore.toFixed(2);

            //for 2 decimal points
            element.convertedDistance = parseFloat(element.convertedDistance);
            element.convertedDistance = element.convertedDistance.toFixed(2)*1;
            element.convertedAverageDistance = parseFloat(element.convertedAverageDistance);
            element.convertedAverageDistance = element.convertedAverageDistance.toFixed(2)*1;
            element.convertedAverageSpeed =parseFloat(element.convertedAverageSpeed);
            element.convertedAverageSpeed =element.convertedAverageSpeed.toFixed(2)*1;
            element.averageGrossWeightComb =parseFloat(element.averageGrossWeightComb);
            element.averageGrossWeightComb =element.averageGrossWeightComb.toFixed(2)*1;
            element.fuelConsumed =parseFloat(element.fuelConsumed);
            element.fuelConsumed =element.fuelConsumed.toFixed(2)*1;
            element.fuelConsumption =parseFloat(element.fuelConsumption);
            element.fuelConsumption =element.fuelConsumption.toFixed(2)*1;
            element.cO2Emission =parseFloat(element.cO2Emission);
            element.cO2Emission =element.cO2Emission.toFixed(2)*1;
            element.harshBrakeDuration = parseFloat(element.harshBrakeDuration);
            element.harshBrakeDuration =element.harshBrakeDuration.toFixed(2)*1;
            element.heavyThrottleDuration = parseFloat(element.heavyThrottleDuration);
            element.heavyThrottleDuration= element.heavyThrottleDuration.toFixed(2)*1;
            element.dpaScore = parseFloat(element.dpaScore);
            element.dpaScore = element.dpaScore.toFixed(2)*1;
    });
    return gridData;
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
