import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import 'jspdf-autotable';
import { HereService } from '../../../services/here.service'; 
import { Router } from '@angular/router';
import { OrganizationService } from '../../../services/organization.service';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { FleetMapService } from '../fleet-map.service'
import { DataInterchangeService } from '../../../services/data-interchange.service';
import { CompleterItem, CompleterService } from 'ng2-completer';

declare var H: any;

@Component({
  selector: 'app-live-fleet-map',
  templateUrl: './live-fleet-map.component.html',
  styleUrls: ['./live-fleet-map.component.less']
})
export class LiveFleetMapComponent implements OnInit {
  showMap: boolean = false;
  showBack: boolean = false;
  showMapPanel: boolean = false;
  advanceFilterOpen: boolean = false;
  mapFilterForm: FormGroup;
  userPOIList: any = [];
  herePOIList: any = [];
  displayPOIList: any = [];
  internalSelection: boolean = false;
  herePOIArr: any = [];
  map_key: any = '';
  platform: any = '';
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  dataSource: any = new MatTableDataSource([]);
  selectedTrip = new SelectionModel(true, []);
  selectedPOI = new SelectionModel(true, []);
  selectedGlobalPOI = new SelectionModel(true, []);
  selectedHerePOI = new SelectionModel(true, []);
  trackType: any = 'snail';
  displayRouteView: any = 'C';
  @Input() translationData:any;
  @Input()  detailsData : any;
  @Input() preferenceObject : any;
  @ViewChild("map") public mapElement: ElementRef;
  accountId:any;
  accountPrefObj : any;
  tripTraceArray = [];
  searchMarker: any = {};
  dataService: any;
  searchStr: null;
  suggestionData : any;
  @Input() filterData : any;
  @Input() filterPOIData : any;
  globalPOIList : any = [];
  displayGlobalPOIList : any =[];
  prefTimeFormat: any; //-- coming from pref setting
  prefTimeZone: any; //-- coming from pref setting
  prefDateFormat: any = 'ddateformat_mm/dd/yyyy'; //-- coming from pref setting
  prefUnitFormat: any = 'dunit_Metric'; //-- coming from pref setting
  prefDetail: any = {};
  @Input() fromVehicleHealth: boolean = false;

  constructor(
    private _formBuilder: FormBuilder,
    private router: Router,
    private organizationService: OrganizationService, 
    private hereService: HereService,
    private fleetMapService:FleetMapService,
    private dataInterchangeService:DataInterchangeService,
    private completerService: CompleterService) {
    // this.map_key = _configService.getSettings("hereMap").api_key;
    this.map_key = localStorage.getItem("hereMapsK"); 
    this.platform = new H.service.Platform({
      "apikey": this.map_key
    });
    this.configureAutoSuggest();
    const navigation = this.router.getCurrentNavigation();
    this.dataInterchangeService.detailDataInterface$.subscribe(vehicleResponse => {
    this.tripTraceArray = [];
    this.fleetMapService.clearRoutesFromMap();
    if (vehicleResponse) {
      if(!vehicleResponse.vehicleDetailsFlag){
        this.tripTraceArray = [];
        this.tripTraceArray = vehicleResponse.data;
        this.showIcons = true;
      }
      else{
        this.tripTraceArray = [];
        if(vehicleResponse.data.length === undefined){ // 16665 - changes added to convert data to array
          this.tripTraceArray.push(vehicleResponse.data);
        }
        else if(vehicleResponse.data.length > 0){
          this.tripTraceArray = vehicleResponse.data; //1665 - data wasn't mapped correctly
        }
        this.showIcons = false;
      }
    }
    else {
      this.showIcons = true;
    }
    this.mapIconData();
    })
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.accountPrefObj = JSON.parse(localStorage.getItem('accountInfo'));
    this.prefDetail = JSON.parse(localStorage.getItem('prefDetail'));
    if(this.prefDetail){
      if(this.accountPrefObj.accountPreference && this.accountPrefObj.accountPreference != ''){
        this.proceedStep(this.accountPrefObj.accountPreference);
      }else{ 
        this.organizationService.getOrganizationPreference(this.accountOrganizationId).subscribe((orgPref: any)=>{
          this.proceedStep(orgPref);
        }, (error) => { 
          this.proceedStep({});
        });
      }
    }
    this.mapFilterForm = this._formBuilder.group({
      routeType: ['', []],
      trackType: ['', []]
    });
    this.mapFilterForm.get('trackType').setValue('snail');
    this.mapFilterForm.get('routeType').setValue('C');
    setTimeout(() => {
      this.fleetMapService.initMap(this.mapElement, this.translationData);
      this.fleetMapService.clearRoutesFromMap();
      if(this.fromVehicleHealth){
        console.log("deployment check!");
      }
      else
        this.tripTraceArray = this.detailsData;
      this.showMap = true;
      this.makeHerePOIList();
      this.loadUserPOI();
      this.loadGlobalPOI();
      this.mapIconData();
    }, 0);
  }

  showIcons = true;
  alertsChecked : boolean = false;
  mapIconData(){
    let _ui = this.fleetMapService.getUI();
   // this.fleetMapService.setIconsOnMap(this.detailsData);

    if(this.tripTraceArray && this.tripTraceArray.length>0){
      this.tripTraceArray= this.fleetMapService.processedLiveFLeetData(this.tripTraceArray);
    }

    this.fleetMapService.clearRoutesFromMap();
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);
  }

  proceedStep(preference: any){
    let _search = this.prefDetail.timeformat.filter(i => i.id == preference.timeFormatId);
    if(_search.length > 0){
      this.prefTimeFormat = Number(_search[0].name.split("_")[1].substring(0,2));
      this.prefTimeZone = this.prefDetail.timezone.filter(i => i.id == preference.timezoneId)[0].name;
      this.prefDateFormat = this.prefDetail.dateformat.filter(i => i.id == preference.dateFormatTypeId)[0].name;
      this.prefUnitFormat = this.prefDetail.unit.filter(i => i.id == preference.unitId)[0].name;
    }else{
      this.prefTimeFormat = Number(this.prefDetail.timeformat[0].name.split("_")[1].substring(0,2)); 
      this.prefTimeZone = this.prefDetail.timezone[0].name;
      this.prefDateFormat = this.prefDetail.dateformat[0].name;
      this.prefUnitFormat = this.prefDetail.unit[0].name;
    }
    this.preferenceObject = {
          prefTimeFormat : this.prefTimeFormat,
          prefTimeZone : this.prefTimeZone,
          prefDateFormat : this.prefDateFormat,
          prefUnitFormat : this.prefUnitFormat
        }
    this.fleetMapService.setPrefObject(this.preferenceObject);
  }

  private configureAutoSuggest(){
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?'+'apiKey='+this.map_key +'&limit=5'+'&q='+searchParam;
    this.suggestionData = this.completerService.remote(
    URL,'title','title');
    this.suggestionData.dataField("items");
    this.dataService = this.suggestionData;
  }

  onSearchFocus(){
    this.searchStr = null;
  }

  onSearchSelected(selectedAddress: CompleterItem){
    if(selectedAddress){
      let id = selectedAddress["originalObject"]["id"];
      let qParam = 'apiKey='+this.map_key + '&id='+ id;
      this.hereService.lookUpSuggestion(qParam).subscribe((data: any) => {
        this.searchMarker = {};
        if(data && data.position && data.position.lat && data.position.lng){
          let searchMarker = {
            lat: data.position.lat,
            lng: data.position.lng,
            from: 'search'
          }
          this.fleetMapService.setMapToLocation(searchMarker);
          this.fleetMapService.dropMarker({ "lat": data.position.lat, "lng": data.position.lng }, data);
        }
      });
    }
  }

  changeAlertSelection(_event){
    this.alertsChecked = _event.checked
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);
  }
  makeHerePOIList(){
    this.herePOIList = [{
      key: 'Hotel',
      translatedName: this.translationData.lblHotel
    },
    {
      key: 'Parking',
      translatedName: this.translationData.lblParking
    },
    {
      key: 'Petrol Station',
      translatedName: this.translationData.lblPetrolStation
    },
    {
      key: 'Railway Station',
      translatedName: this.translationData.lblRailwayStation
    }];
  }

  loadUserPOI() {
    if(this.filterPOIData){
      if(this.filterPOIData['userPois']){
        this.userPOIList = this.makeUserCategoryPOIList(this.filterPOIData['userPois']);
      }
      else{
        this.userPOIList = [];
      }
    }
  }

  loadGlobalPOI(){
    if(this.filterPOIData){
      if(this.filterPOIData['globalPois']){
        this.globalPOIList = this.makeUserCategoryPOIList(this.filterPOIData['globalPois']);
      }
      else{
        this.globalPOIList = [];
      }
    }
  }

  makeUserCategoryPOIList(poiData: any) {
    let categoryArr: any = [];
    let _arr: any = poiData.map(item => item.categoryId).filter((value, index, self) => self.indexOf(value) === index);
    _arr.forEach(element => {
      let _data = poiData.filter(i => i.categoryId == element);
      if (_data.length > 0) {
        let subCatUniq = _data.map(i => i.subCategoryId).filter((value, index, self) => self.indexOf(value) === index);
        let _subCatArr = [];
        if (subCatUniq.length > 0) {
          subCatUniq.forEach(elem => {
            let _subData = _data.filter(i => i.subCategoryId == elem && i.subCategoryId != 0);
            if (_subData.length > 0) {
              _subCatArr.push({
                poiList: _subData,
                subCategoryName: _subData[0].subCategoryName,
                subCategoryId: _subData[0].subCategoryId,
                checked: false
              });
            }
          });
        }

        _data.forEach(data => {
          data.checked = false;
        });

        categoryArr.push({
          categoryId: _data[0].categoryId,
          categoryName: _data[0].categoryName,
          poiList: _data,
          subCategoryPOIList: _subCatArr,
          open: false,
          parentChecked: false
        });
      }
    });

    return categoryArr;
  }


   // Global Poi selection //

  changeGlobalPOISelection(event: any, poiData: any, index: any){
    if (event.checked){ // checked
      this.globalPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = true;
      });
      this.globalPOIList[index].poiList.forEach(_elem => {
        _elem.checked = true;
      });
      this.globalPOIList[index].parentChecked = true;
    }else{ // unchecked
      this.globalPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = false;
      });
      this.globalPOIList[index].poiList.forEach(_elem => {
        _elem.checked = false;
      });
      this.globalPOIList[index].parentChecked = false;
    }
    this.displayGlobalPOIList = [];
    this.selectedGlobalPOI.selected.forEach(item => {
      if(item.poiList && item.poiList.length > 0){
        item.poiList.forEach(element => {
          if(element.checked){ // only checked
            this.displayGlobalPOIList.push(element);
          }
        });
      }
    });
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);

    this.fleetMapService.showCategoryPOI(this.displayPOIList,_ui);
  }

  openClosedGlobalPOI(index: any) {
    this.globalPOIList[index].open = !this.globalPOIList[index].open;
  }
  changeGlobalSubCategory(event: any, subCatPOI: any, _index: any) {
    let _uncheckedCount: any = 0;
    this.globalPOIList[_index].subCategoryPOIList.forEach(element => {
      if (element.subCategoryId == subCatPOI.subCategoryId) {
        element.checked = event.checked ? true : false;
      }

      if (!element.checked) { // unchecked count
        _uncheckedCount += element.poiList.length;
      }
    });

    if (this.globalPOIList[_index].poiList.length == _uncheckedCount) {
      this.globalPOIList[_index].parentChecked = false; // parent POI - unchecked
      let _s: any = this.selectedGlobalPOI.selected;
      if (_s.length > 0) {
        this.selectedGlobalPOI.clear(); // clear parent category data
        _s.forEach(element => {
          if (element.categoryId != this.globalPOIList[_index].categoryId) { // exclude parent category data
            this.selectedGlobalPOI.select(element);
          }
        });
      }
    } else {
      this.globalPOIList[_index].parentChecked = true; // parent POI - checked
      let _check: any = this.selectedGlobalPOI.selected.filter(k => k.categoryId == this.globalPOIList[_index].categoryId); // already present
      if (_check.length == 0) { // not present, add it
        let _s: any = this.selectedGlobalPOI.selected;
        if (_s.length > 0) { // other element present
          this.selectedGlobalPOI.clear(); // clear all
          _s.forEach(element => {
            this.selectedGlobalPOI.select(element);
          });
        }
        this.globalPOIList[_index].poiList.forEach(_el => {
          if (_el.subCategoryId == 0) {
            _el.checked = true;
          }
        });
        this.selectedGlobalPOI.select(this.userPOIList[_index]); // add parent element
      }
    }

    this.displayGlobalPOIList = [];
    if(this.selectedPOI.selected.length > 0){
    this.selectedGlobalPOI.selected.forEach(item => {
      if (item.poiList && item.poiList.length > 0) {
        item.poiList.forEach(element => {
          if (element.subCategoryId == subCatPOI.subCategoryId) { // element match
            if (event.checked) { // event checked
              element.checked = true;
              this.displayGlobalPOIList.push(element);
            } else { // event unchecked
              element.checked = false;
            }
          } else {
            if (element.checked) { // element checked
              this.displayGlobalPOIList.push(element);
            }
          }
        });
      }
    });
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.showCategoryPOI(this.displayPOIList,_ui);
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);
    }
  }

 // user POI

  changeUserPOISelection(event: any, poiData: any, index: any){
    if (event.checked){ // checked
      this.userPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = true;
      });
      this.userPOIList[index].poiList.forEach(_elem => {
        _elem.checked = true;
      });
      this.userPOIList[index].parentChecked = true;
       if(this.selectedPOI.selected.length > 0){
         let _s: any = this.selectedPOI.selected.filter(i => i.categoryId == this.userPOIList[index].categoryId);
         if(_s.length > 0){

         }
       }
      //else{

      // }
    }else{ // unchecked
      this.userPOIList[index].subCategoryPOIList.forEach(element => {
        element.checked = false;
      });
      this.userPOIList[index].poiList.forEach(_elem => {
        _elem.checked = false;
      });
      this.userPOIList[index].parentChecked = false;
    }
    this.displayPOIList = [];
    this.selectedPOI.selected.forEach(item => {
      if(item.poiList && item.poiList.length > 0){
        item.poiList.forEach(element => {
          if(element.checked){ // only checked
            this.displayPOIList.push(element);
          }
        });
      }
    });
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);

    this.fleetMapService.showCategoryPOI(this.displayPOIList,_ui);
  }

  openClosedUserPOI(index: any) {
    this.userPOIList[index].open = !this.userPOIList[index].open;
  }

  changeSubCategory(event: any, subCatPOI: any, _index: any) {
    let _uncheckedCount: any = 0;
    this.userPOIList[_index].subCategoryPOIList.forEach(element => {
      if (element.subCategoryId == subCatPOI.subCategoryId) {
        element.checked = event.checked ? true : false;
      }

      if (!element.checked) { // unchecked count
        _uncheckedCount += element.poiList.length;
      }
    });

    if (this.userPOIList[_index].poiList.length == _uncheckedCount) {
      this.userPOIList[_index].parentChecked = false; // parent POI - unchecked
      let _s: any = this.selectedPOI.selected;
      if (_s.length > 0) {
        this.selectedPOI.clear(); // clear parent category data
        _s.forEach(element => {
          if (element.categoryId != this.userPOIList[_index].categoryId) { // exclude parent category data
            this.selectedPOI.select(element);
          }
        });
      }
    } else {
      this.userPOIList[_index].parentChecked = true; // parent POI - checked
      let _check: any = this.selectedPOI.selected.filter(k => k.categoryId == this.userPOIList[_index].categoryId); // already present
      if (_check.length == 0) { // not present, add it
        let _s: any = this.selectedPOI.selected;
        if (_s.length > 0) { // other element present
          this.selectedPOI.clear(); // clear all
          _s.forEach(element => {
            this.selectedPOI.select(element);
          });
        }
        this.userPOIList[_index].poiList.forEach(_el => {
          if (_el.subCategoryId == 0) {
            _el.checked = true;
          }
        });
        this.selectedPOI.select(this.userPOIList[_index]); // add parent element
      }
    }

    this.displayPOIList = [];
    if(this.selectedPOI.selected.length > 0){
    this.selectedPOI.selected.forEach(item => {
      if (item.poiList && item.poiList.length > 0) {
        item.poiList.forEach(element => {
          if (element.subCategoryId == subCatPOI.subCategoryId) { // element match
            if (event.checked) { // event checked
              element.checked = true;
              this.displayPOIList.push(element);
            } else { // event unchecked
              element.checked = false;
            }
          } else {
            if (element.checked) { // element checked
              this.displayPOIList.push(element);
            }
          }
        });
      }
    });
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.showCategoryPOI(this.displayPOIList,_ui);
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);
    }
  }

  ///////////////////////////////
  changeHerePOISelection(event: any, hereData: any){
    this.herePOIArr = [];
    this.selectedHerePOI.selected.forEach(item => {
      this.herePOIArr.push(item.key);
    });
    this.searchPlaces();
  }

  searchPlaces() {
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);
  }

  onMapRepresentationChange(event: any) {
    this.trackType = event.value;
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr);
  }
  onAdvanceFilterOpen() {
    this.advanceFilterOpen = !this.advanceFilterOpen;
  }


  onDisplayChange(event: any) {
    this.displayRouteView = event.value;
    let _ui = this.fleetMapService.getUI();
    this.fleetMapService.viewSelectedRoutes(this.tripTraceArray, _ui, this.trackType, this.displayRouteView, this.displayPOIList, this.searchMarker, this.herePOIArr,this.alertsChecked,this.showIcons, this.displayGlobalPOIList,this.translationData);
  }

}
