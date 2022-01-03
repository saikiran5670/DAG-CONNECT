import { Injectable, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { HereService } from 'src/app/services/here.service';
import { CorridorService } from 'src/app/services/corridor.service';
import { ConfigService } from '@ngx-config/core';
import { decode, encode } from '../../../services/flexible-polyline';

declare var H: any;

@Injectable({
  providedIn: 'root'
})
export class MapFunctionsService {

  platform: any;
  map: any;
  hereMap: any;
  public mapElement: ElementRef;
  mapGroup;
  startAddressPositionLat: number = 0; // = {lat : 18.50424,long : 73.85286};
  startAddressPositionLong: number = 0; // = {lat : 18.50424,long : 73.85286};
  startMarker: any;
  endMarker: any;
  polyLineArray:any = [];
  routeCorridorMarker: any;
  routeOutlineMarker: any;
  endAddressPositionLat: number = 0;
  endAddressPositionLong: number = 0;
  corridorWidth: number = 100;
  noRoutesLabel: boolean = false;

  corridorWidthKm: number = 0.1;
  additionalData = [];
  
  corridorPath : any;
  polylinePath : any;
  polyLinePathArray : any = [];
  organizationId:any;
  corridorId : any;
  tollRoadChecked = false;
  motorwayChecked = false;
  boatFerriesChecked = false;
  railFerriesChecked =false;
  tunnelsChecked=false;
  dirtRoadChecked = false;
  exclusions = [];

  map_key: any = '';
  ui: any;

  constructor(private hereService: HereService, private corridorService: CorridorService, private _configService: ConfigService) {
    this.map_key = _configService.getSettings("hereMap").api_key;
    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });
  }



  getAttributeData = [];
  getExclusionList = [];
  hazardousMaterial = [];
  tunnelId = undefined;
  selectedTrailerId = undefined;
  trafficFlowChecked = false;
  transportDataChecked = false;
  trafficOnceChecked = false;
  transportOnceChecked = false;
  vehicleHeightValue = 0
  vehicleWidthValue = 0
  vehicleLengthValue = 0
  vehicleLimitedWtValue = 0
  vehicleWtPerAxleValue = 0
  defaultLayers : any; 


  // public ngAfterViewInit() {
  //   this.initMap();
  // }

  initMap(mapElement: any, translationData?: any) {
    this.defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(mapElement.nativeElement,
      this.defaultLayers.raster.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, this.defaultLayers);
    this.ui.removeControl("mapsettings");
    // create custom one
    var ms = new H.ui.MapSettingsControl({
        baseLayers : [ { 
          label: translationData ? translationData.lblNormal || "Normal" : "Normal", layer: this.defaultLayers.raster.normal.map
        },{
          label: translationData ? translationData.lblSatellite || "Satellite" : "Satellite", layer: this.defaultLayers.raster.satellite.map
        }, {
          label: translationData ? translationData.lblTerrain || "Terrain" : "Terrain", layer: this.defaultLayers.raster.terrain.map
        }
        ],
      layers : [{
            label: translationData ? translationData.lblLayerTraffic || "Layer.Traffic" : "Layer.Traffic", layer: this.defaultLayers.vector.normal.traffic
        },
        {
            label: translationData ? translationData.lblLayerIncidents || "Layer.Incidents" : "Layer.Incidents", layer: this.defaultLayers.vector.normal.trafficincidents
        }
      ]
    });
    this.ui.addControl("customized", ms);

    var group = new H.map.Group();
    this.mapGroup = group;
  }

  // clearRoutesFromMap() {
  //   var group = new H.map.Group();
  //   group.removeAll();
  //   this.hereMap.removeObjects(this.hereMap.getObjects())
  //   this.startMarker = null; this.endMarker = null;
  // }
  clearRoutesFromMap() { 
    this.mapGroup.removeAll();
    this.startMarker = null; this.endMarker = null;
    this.hereMap.removeLayer(this.defaultLayers.vector.normal.traffic);
    this.hereMap.removeLayer(this.defaultLayers.vector.normal.truck);
    this.transportOnceChecked = false;
    this.trafficOnceChecked = false;
    this.ui.getBubbles().forEach(bub =>this.ui.removeBubble(bub));
  }

  // code changed for bug 16249
  // added to remove selected polyline  // functions removed as points are preserved from lat long line : 246
  // removeCorridor(_id){
  //   if(this.polyLinePathArray.length >0){
  //     let filteredPolyline = this.polyLinePathArray.filter(elem => elem.id != _id);
  //     this.polyLinePathArray = filteredPolyline;
  //   }
    
  // }

  // clearPolylines(){
  //   this.polyLinePathArray = [];
  // }

  group = new H.map.Group();
  viaRoutePlottedPoints = [];

  viewSelectedRoutes(_selectedRoutes, accountOrganizationId?, isRCorridor?, translationData?: any) {
    let corridorName = '';
    let startAddress = '';
    let endAddress = '';
    let transcorridorname = translationData.lblCorridorName;
    let transstartpoint = translationData.lblStartPoint;
    let transendpoint = translationData.lblEndPoint;
    let transwidth = translationData.lblWidth;
    this.organizationId = accountOrganizationId;
    this.hereMap.removeLayer(this.defaultLayers.vector.normal.traffic);
    this.hereMap.removeLayer(this.defaultLayers.vector.normal.truck);
    this.transportOnceChecked = false;
    this.trafficOnceChecked = false;
    this.viaRoutePlottedPoints = [];
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
          if(_selectedRoutes[i].trips && _selectedRoutes[i].trips.length > 0){ // for existing trip
            this.startAddressPositionLat = _selectedRoutes[i].trips[0].startLatitude;
            this.startAddressPositionLong = _selectedRoutes[i].trips[0].startLongitude;
            this.endAddressPositionLat = _selectedRoutes[i].trips[0].endLatitude;
            this.endAddressPositionLong = _selectedRoutes[i].trips[0].endLongitude;
            startAddress = _selectedRoutes[i].trips[0].startPosition;
            endAddress = _selectedRoutes[i].trips[0].endPosition;
          }else{ // for routing calculating
            this.startAddressPositionLat = _selectedRoutes[i].startLat;
            this.startAddressPositionLong = _selectedRoutes[i].startLong;
            this.endAddressPositionLat = _selectedRoutes[i].endLat;
            this.endAddressPositionLong = _selectedRoutes[i].endLong;
            startAddress = _selectedRoutes[i].startPoint;
            endAddress = _selectedRoutes[i].endPoint;
          }

          this.corridorWidth = _selectedRoutes[i].width;
          this.corridorWidthKm = this.corridorWidth / 1000;
          corridorName = _selectedRoutes[i].corridoreName;
          this.corridorId = _selectedRoutes[i].id;
        } else {
          this.startAddressPositionLat = _selectedRoutes[i].startPositionlattitude;
          this.startAddressPositionLong = _selectedRoutes[i].startPositionLongitude;
          this.endAddressPositionLat = _selectedRoutes[i].endPositionLattitude;
          this.endAddressPositionLong = _selectedRoutes[i].endPositionLongitude;

          // this.startAddressPositionLat =  19.14045;
          // this.startAddressPositionLong = 72.88235;
          // this.endAddressPositionLat=  19.03261;
          // this.endAddressPositionLong= 73.02961;
          this.corridorWidth = 100;
          this.corridorWidthKm = this.corridorWidth / 1000;
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
        let endMarkerHtml = `<div class='font-14-px line-height-21px font-helvetica-lt'>
        <table>
        <tr><td class='font-helvetica-md'>${transcorridorname}:</td> <td>${corridorName} </td></tr>
        <tr><td class='font-helvetica-md'>${transstartpoint}:</td><td>${startAddress}</td></tr>
        <tr><td class='font-helvetica-md'>${transendpoint}:</td><td>${endAddress}</td></tr>
        <tr><td class='font-helvetica-md'>${transwidth}:</td><td>${this.corridorWidthKm} km</td></tr>
        </table>
        </div>`
        this.endMarker.setData(endMarkerHtml);
        this.mapGroup.addObject(this.endMarker);
        //this.hereMap.addObject(this.mapGroup)
        this.hereMap.getViewModel().setLookAtData({
          bounds: this.mapGroup.getBoundingBox()
        });

        if(accountOrganizationId){
        // add end tooltip
        let bubble;
        this.endMarker.addEventListener('pointerenter',  (evt)=> {
          // event target is the marker itself, group is a parent event target
          // for all objects that it contains
          bubble =  new H.ui.InfoBubble(evt.target.getGeometry(), {
            // read custom data
            content: evt.target.getData()
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
        if(isRCorridor && _selectedRoutes[i].corridorType && _selectedRoutes[i].corridorType == 'R'){
          let gpsLineString:any = [];
          _selectedRoutes[i].viaAddressDetail.forEach(element => {
            gpsLineString.push(element.latitude, element.longitude, 0);
          });
          if(_selectedRoutes[i].viaAddressDetail.length > 0){
            // this.viaRouteCount = true;
            this.viaRoutePlottedPoints = _selectedRoutes[i].viaAddressDetail.filter( e => e.type == "V");
            this.viaRoutePlottedPoints.forEach(element => {
              element["viaRoutName"] = element.corridorViaStopName;
            });
            this.plotViaStopPoints();
          }
          this.addTruckRouteShapeToMapEdit(gpsLineString);
        } else {
        if (accountOrganizationId) {
          if (_selectedRoutes[i].id) {
            this.corridorService.getCorridorFullList(accountOrganizationId, _selectedRoutes[i].id).subscribe((data) => {
              //console.log(data)
              if (data[0]["corridorProperties"]) {
                this.additionalData = data[0]["corridorProperties"];
                this.setAdditionalData();
              }
                  if(this.trafficOnceChecked){
                    this.hereMap.addLayer(this.defaultLayers.vector.normal.traffic);
                  }
                  if(this.transportOnceChecked){
                    this.hereMap.addLayer(this.defaultLayers.vector.normal.truck);
                  }
                
                if (data[0].viaAddressDetail.length > 0) {
                  this.viaRoutePlottedPoints = [];
                  this.viaRoutePlottedPoints = data[0].viaAddressDetail;
                  this.plotViaStopPoints();
                }
                else{
                  this.viaRoutePlottedPoints = [];
                }

                if(data[0].trips && data[0].trips.length > 0){ // For existing trip
                  this.startAddressPositionLat = data[0].trips[0].startLatitude;
                  this.startAddressPositionLong = data[0].trips[0].startLongitude;
                  this.endAddressPositionLat = data[0].trips[0].endLatitude;
                  this.endAddressPositionLong = data[0].trips[0].endLongitude;
                }else{ // For route calculating
                  this.startAddressPositionLat = data[0].startLat;
                  this.startAddressPositionLong = data[0].startLong;
                  this.endAddressPositionLat = data[0].endLat;
                  this.endAddressPositionLong = data[0].endLong;
                }
                this.calculateTruckRoute();

            })
          }
        }
        else {
          this.calculateTruckRoute();

        }
      }
        //this.removeBubble();

        // this.hereMap.getViewModel().setLookAtData({ bounds: group.getBoundingBox()});
        // let successRoute = this.calculateAB('view');
      }
     
    }
  }

  viewSelectedRoutesCorridor(_selectedRoutes, accountOrganizationId?, translationData?: any){
        this.viewSelectedRoutes(_selectedRoutes, accountOrganizationId, true, translationData);
  }

  addTruckRouteShapeToMapEdit(gpsLineString){
        let co = [[19.14012, 72.88097, 0], [19.14012, 72.88097, 0]];
        let ob = {
          precision : 5,
          thirdDim : 0,
          thirdDimPrecision: 0,
          polyline: co
        };
        let lineVal = encode(ob);
        let linestring = H.geo.LineString.fromFlexiblePolyline(lineVal);
        linestring.Y = gpsLineString;
        this.corridorPath = new H.map.Polyline(linestring, {
          style:  {
            lineWidth: this.corridorWidthKm * 10,
            strokeColor: 'rgba(181, 199, 239, 0.6)'
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
        this.mapGroup.addObjects([this.corridorPath,polylinePath]);
        this.hereMap.addObject(this.mapGroup);
        // And zoom to its bounding rectangle
        this.hereMap.getViewModel().setLookAtData({
          bounds: this.mapGroup.getBoundingBox()
        });
    //   });
    // }
  }

  removeBubble(){
    this.hereMap.addEventListener('tap', (evt) => {
    this.ui.getBubbles().forEach(bub =>this.ui.removeBubble(bub));

    })
  }
  viaAddressPositionLat;
  viaAddressPositionLong;
  viaMarker: any;

  plotViaStopPoints() {
    for (var i in this.viaRoutePlottedPoints){
      this.viaAddressPositionLat = this.viaRoutePlottedPoints[i]["latitude"];
      this.viaAddressPositionLong = this.viaRoutePlottedPoints[i]["longitude"];
      let viaMarker = this.createViaMarker();
      let markerSize = { w: 26, h: 32 };
      const icon = new H.map.Icon(viaMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });

      this.viaMarker = new H.map.Marker({ lat: this.viaAddressPositionLat, lng: this.viaAddressPositionLong }, { icon: icon });
      this.mapGroup.addObject(this.viaMarker);
    }

  }
  setAdditionalData() {
    let _data = this.additionalData;
    this.getAttributeData = _data["attribute"];
    this.getExclusionList = _data["exclusion"];
    this.hazardousMaterial = [];
    this.exclusions = [];

    this.getAttributeData["isCombustible"] ? this.hazardousMaterial.push('combustible') : '';
    this.getAttributeData["isCorrosive"] ? this.hazardousMaterial.push('corrosive') : '';
    this.getAttributeData["isExplosive"] ? this.hazardousMaterial.push('explosive') : '';
    this.getAttributeData["isFlammable"] ? this.hazardousMaterial.push('flammable') : '';
    this.getAttributeData["isGas"] ? this.hazardousMaterial.push('gas') : '';
    this.getAttributeData["isOrganic"] ? this.hazardousMaterial.push('organic') : '';
    this.getAttributeData["isOther"] ? this.hazardousMaterial.push('other') : '';
    this.getAttributeData["isPoision"] ? this.hazardousMaterial.push('poison') : '';
    this.getAttributeData["isPoisonousInhalation"] ? this.hazardousMaterial.push('poisonousInhalation') : '';
    this.getAttributeData["isRadioActive"] ? this.hazardousMaterial.push('radioactive') : '';
    this.getAttributeData["isWaterHarm"] ? this.hazardousMaterial.push('harmfulToWater') : '';


    this.selectedTrailerId = this.getAttributeData["noOfTrailers"];
    this.trafficFlowChecked = _data["isTrafficFlow"];
    this.transportDataChecked = _data["isTransportData"];
    this.trafficFlowChecked = _data["isTrafficFlow"];
    if(this.trafficFlowChecked){
      this.trafficOnceChecked= true;
      //this.hereMap.addLayer(this.defaultLayers.vector.normal.traffic);
    }
    this.transportDataChecked = _data["isTransportData"];
    if(this.transportDataChecked){
      this.transportOnceChecked = true;
      //this.hereMap.addLayer(this.defaultLayers.vector.normal.truck);
    }
    this.vehicleHeightValue = _data["vehicleSize"].vehicleHeight;
    this.vehicleWidthValue = _data["vehicleSize"].vehicleWidth;
    this.vehicleLengthValue = _data["vehicleSize"].vehicleLength;
    this.vehicleLimitedWtValue = _data["vehicleSize"].vehicleLimitedWeight;
    this.vehicleWtPerAxleValue = _data["vehicleSize"].vehicleWeightPerAxle;

    this.getExclusionList["tunnelsType"] == 'A'  ? this.exclusions.push('tunnel') :'';
    this.getExclusionList["tollRoadType"] == 'A'  ? this.exclusions.push('tollRoad') :'';
    this.getExclusionList["boatFerriesType"] == 'A'  ? this.exclusions.push('ferry') :'';
    this.getExclusionList["dirtRoadType"] == 'A'  ? this.exclusions.push('dirtRoad') :'';
    this.getExclusionList["mortorway"] == 'A'  ? this.exclusions.push('controlledAccessHighway') :'';
    this.getExclusionList["railFerriesType"] == 'A'  ? this.exclusions.push('carShuttleTrain') :'';

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
    if (this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0 && this.corridorWidth != 0) {
      this.calculateTruckRoute();
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

  createViaMarker() {
    const viaMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.6665C18.6665 24.9998 24.3332 19.2591 24.3332 12.9998C24.3332 6.74061 19.2591 1.6665 12.9998 1.6665C6.74061 1.6665 1.6665 6.74061 1.6665 12.9998C1.6665 19.2591 7.6665 25.3332 12.9998 29.6665Z" fill="#0D7EE7"/>
    <path d="M13 22.6665C18.5228 22.6665 23 18.4132 23 13.1665C23 7.9198 18.5228 3.6665 13 3.6665C7.47715 3.6665 3 7.9198 3 13.1665C3 18.4132 7.47715 22.6665 13 22.6665Z" fill="white"/>
    <path d="M19.7616 12.6263L14.0759 6.94057C13.9169 6.78162 13.7085 6.70215 13.5 6.70215C13.2915 6.70215 13.0831 6.78162 12.9241 6.94057L7.23842 12.6263C6.92053 12.9444 6.92053 13.4599 7.23842 13.778L12.9241 19.4637C13.0831 19.6227 13.2915 19.7021 13.5 19.7021C13.7085 19.7021 13.9169 19.6227 14.0759 19.4637L19.7616 13.778C20.0795 13.4599 20.0795 12.9444 19.7616 12.6263ZM13.5 18.3158L8.38633 13.2021L13.5 8.08848L18.6137 13.2021L13.5 18.3158ZM11.0625 12.999V15.0303C11.0625 15.1425 11.1534 15.2334 11.2656 15.2334H12.0781C12.1904 15.2334 12.2812 15.1425 12.2812 15.0303V13.4053H14.3125V14.7695C14.3125 14.8914 14.4123 14.9731 14.5169 14.9731C14.5644 14.9731 14.6129 14.9564 14.6535 14.9188L16.7916 12.9452C16.8787 12.8647 16.8787 12.7271 16.7916 12.6466L14.6535 10.673C14.6129 10.6357 14.5644 10.6187 14.5169 10.6187C14.4123 10.6187 14.3125 10.7004 14.3125 10.8223V12.1865H11.875C11.4263 12.1865 11.0625 12.5504 11.0625 12.999Z" fill="#0D7EE7"/>
    </svg>`

    return viaMarker;
  }
  /////////////////////////// v8 calculate ////////////////////
  routePoints: any;
  calculateTruckRoute() {
    let lineWidth = this.corridorWidthKm;
    let routeRequestParams = {}
    routeRequestParams = {
      'origin':`${this.startAddressPositionLat},${this.startAddressPositionLong}`,
      'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`,
      'return':'polyline,summary,travelSummary',
      'routingMode':'fast',
      'transportMode':'truck',
      'apikey':this.map_key

    }
    
    if(this.viaRoutePlottedPoints.length>0){
      let waypoints = [];
      for(var i in this.viaRoutePlottedPoints){
        waypoints.push(`${this.viaRoutePlottedPoints[i]["latitude"]},${this.viaRoutePlottedPoints[i]["longitude"]}`)
      }
      routeRequestParams['via'] = new H.service.Url.MultiValueQueryParameter( waypoints )
      
    }

    if (this.selectedTrailerId) {
      routeRequestParams['truck[trailerCount]'] = this.selectedTrailerId;
    }
    if (this.tunnelId) {
      routeRequestParams['truck[tunnelCategory]']= this.tunnelId;
    }
    if (this.vehicleHeightValue) {
      routeRequestParams['truck[height]'] = Math.round(this.vehicleHeightValue);
    }
    if (this.vehicleWidthValue) {
      routeRequestParams['truck[width]'] = Math.round(this.vehicleWidthValue);
    }
    if (this.vehicleLengthValue) {
      routeRequestParams['truck[length]']= Math.round(this.vehicleLengthValue);
    }
    if (this.vehicleLimitedWtValue) {
      routeRequestParams['truck[grossWeight]'] = Math.round(this.vehicleLimitedWtValue);
    }
    if (this.vehicleWtPerAxleValue) {
      routeRequestParams['truck[weightPerAxle]'] = Math.round(this.vehicleWtPerAxleValue);
    }

    if (this.hazardousMaterial.length > 0) {
      routeRequestParams['truck[shippedHazardousGoods]']= this.hazardousMaterial.join();
    }
    
    if(this.exclusions.length>0){
      routeRequestParams['avoid[features]'] = this.exclusions.join();

    }
    this.routePoints = [];
    this.hereService.calculateRoutePoints(routeRequestParams).then((data:any)=>{
      if(data && data.routes){
        if(data.routes.length == 0){
        }
        else{
          this.routePoints = data.routes[0];
          this.addTruckRouteShapeToMap(lineWidth);
        }
        
        }
      
    })

  }

  addTruckRouteShapeToMap(lineWidth?) {
    let pathWidth = lineWidth ? (lineWidth * 10) : (this.corridorWidthKm * 10);

    if (this.routePoints.sections) {
      this.routePoints.sections.forEach((section) => {
        // decode LineString from the flexible polyline
        let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);

        // Create a corridor width to display the route:
        this.corridorPath = new H.map.Polyline(linestring, {
          style: {
            lineWidth: pathWidth > 100 ? 100 : pathWidth, // max-100
            strokeColor: 'rgba(181, 199, 239, 0.6)'
          }
        });
        // Create a polyline to display the route:
        this.polylinePath = new H.map.Polyline(linestring, {
          style: {
            lineWidth: 3,
            strokeColor: '#436ddc'
          }
        });

         this.polyLineArray.push(this.corridorPath);
         this.mapGroup.addObjects([this.corridorPath, this.polylinePath]);

        // functions removed as points are preserved from lat long line : 246 - were added for 16249
        // if(this.organizationId){ // to store all the polylines 16249
        //   this.polyLinePathArray.push({
        //     'id' : this.corridorId,
        //     'corridorPath' : this.corridorPath,
        //     'polylinePath' : this.polylinePath
        //   })
        // }
        // if(this.polyLinePathArray.length > 0){
        //   for(var i in this.polyLinePathArray){
        //     this.mapGroup.addObjects([this.polyLinePathArray[i]['corridorPath'], this.polyLinePathArray[i]['polylinePath']]);

        //   }
        // }
        // else{
        //   this.mapGroup.addObjects([this.corridorPath, this.polylinePath]);

        // }
        // added for 16249 till here!
        
        // Add the polyline to the map
        // if (this.viaMarker) {
        //   this.mapGroup.addObject(this.viaMarker);
        // }
        this.hereMap.addObject(this.mapGroup);
        this.hereMap.getViewModel().setLookAtData({
          bounds: this.mapGroup.getBoundingBox()
       });
        // And zoom to its bounding rectangle
        //  this.hereMap.getViewModel().setLookAtData({
        //     bounds: this.mapGroup.getBoundingBox()
        //  });
      });
    }
  }

  addInfoBubble(markerGroup) {

    var group = new H.map.Group();

    this.hereMap.addObject(group);

    // add 'tap' event listener, that opens info bubble, to the group
    this.hereMap.addEventListener('tap', (evt) => {
      // event target is the marker itself, group is a parent event target
      // for all objects that it contains
      var bubble = new H.ui.InfoBubble(evt.target.getGeometry(), {
        // read custom data
        content: evt.target.getData()
      });
      // show info bubble
      if (evt.target.getData()) {
        this.ui.addBubble(bubble);

      }
    }, false);
  }

  updateWidth(_width,fromExistingTrip?){
    this.corridorWidthKm = _width;
    let setWidth = _width*10;
   // this.addTruckRouteShapeToMap();
    //let geoLineString = this.corridorPath.getGeometry();

  if(fromExistingTrip && this.polyLineArray.length > 0) {
    this.polyLineArray.forEach(getAllPaths => {
      getAllPaths.setStyle({
        lineWidth: setWidth,
        strokeColor: 'rgba(181, 199, 239, 0.6)'
      });
    });
  }else {
    if(this.corridorPath){
      this.corridorPath.setStyle({
        lineWidth: setWidth,
        strokeColor: 'rgba(181, 199, 239, 0.6)'
      });
    }
  

    
    
    //this.corridorPath.setStyle( this.corridorPath.getStyle().getCopy({linewidth:_width}));
    //console.log(geoLineString)
    //this.corridorPath.setGeometry(geoLineString);
  }
}
}