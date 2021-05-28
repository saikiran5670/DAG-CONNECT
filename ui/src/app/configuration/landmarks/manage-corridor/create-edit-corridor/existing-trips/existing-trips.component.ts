import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { CorridorService } from '../../../../../services/corridor.service';
// import { AccountService } from '../../../services/account.service';
import { CustomValidators } from '../../../../../shared/custom.validators';
import { NgxMaterialTimepickerComponent, NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import * as moment from 'moment';
import { POIService } from 'src/app/services/poi.service';
import {
  CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData
} from 'ng2-completer';
import { ConfigService } from '@ngx-config/core';
import { HereService } from 'src/app/services/here.service';
import { Options } from '@angular-slider/ngx-slider';
declare var H: any;


@Component({
  selector: 'app-existing-trips',
  templateUrl: './existing-trips.component.html',
  styleUrls: ['./existing-trips.component.less']
})
export class ExistingTripsComponent implements OnInit {
  @Input() translationData: any;
  @Input() exclusionList :  any;
  @Input() actionType: any; 
  @Input() selectedElementData : any;
  @Output() backToPage = new EventEmitter<any>();
  @Output() backToCreate = new EventEmitter<any>();
  @Output() backToReject = new EventEmitter<any>();

  endDate = new FormControl();
  startDate = new FormControl();
  startTime = new FormControl();
  @Input() ngxTimepicker: NgxMaterialTimepickerComponent;
  @Input() disabled: boolean;
  @Input() value: string = '11:00 PM';
  @Input() format: number = 12;
  selectedStartTime: any = '12:00 AM'
  selectedEndTime: any = '11:59 PM'
  selectedStartDateStamp: any;
  selectedEndDateStamp: any;
  startTimeUTC: any;
  endTimeUTC: any;
  timeValue: any = 0;
  // range = new FormGroup({
  //   start: new FormControl(),
  //   end: new FormControl()
  // });
  // translationData: any;
  OrgId: any = 0;
  // @Output() backToPage = new EventEmitter<any>();
  // displayedColumns: string[] = ['select', 'firstName', 'emailId', 'roles', 'accountGroups'];
  selectedAccounts = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  displayedColumns = ['All', 'DriverName', 'distance', 'date', 'startPoint', 'endPoint'];
  existingTripData: any = [];
  createEditStatus = false;
  accountOrganizationId: any = 0;
  corridorCreatedMsg: any = '';
  // actionType: string;
  titleVisible: boolean = false;
  titleFailVisible: boolean = false;
  showMap: boolean = false;
  map: any;
  initData: any = [];
  // dataSource: any;
  markerArray: any = [];
  showLoadingIndicator: boolean;
  selectedCorridors = new SelectionModel(true, []);
  public position: string;
  public locations: Array<any>;
  setEndAddress: any = "";
  setStartAddress: any = ""; 
  // @Input() translationData: any;
  // @Input() selectedRowData: any;
  // @Input() actionType: any;
  // userCreatedMsg: any = '';
  // duplicateEmailMsg: boolean = false;
  // breadcumMsg: any = '';
  // existingTripForm: FormGroup;
  // groupTypeList: any = [];
  // showUserList: boolean = true;
  // @Input() translationData: any;
  @Input() vehicleGroupList: any;
  vinList: any = [];
  vinListSelectedValue: any = [];
  vehicleGroupIdsSet: any = [];
  localStLanguage: any;


  getAttributeData : any;
  getExclusionList : any;
  getVehicleSize : any;
  additionalData : any;

  breadcumMsg: any = '';
  existingTripForm: FormGroup;
  corridorTypeList = [{id:1,value:'Route Calculating'},{id:2,value:'Existing Trips'}];
  trailerList = [0,1,2,3,4];
  selectedCorridorTypeId : any = 46;
  selectedTrailerId = 0;
  private platform: any;
  // map: any;
  private ui: any;
  lat: any = '37.7397';
  lng: any = '-121.4252';
  @ViewChild("map")
  public mapElement: ElementRef;
  hereMapService: any;
  organizationId: number;
  corridorId : number = 0;
  // localStLanguage: any;
  accountId: any = 0;
  hereMap: any;
  distanceinKM = 0;
  viaRouteCount : boolean = false;
  transportDataChecked : boolean= false;
  trafficFlowChecked : boolean = false;
  corridorWidth : number = 100;
  corridorWidthKm : number = 0.1;
  sliderValue : number = 0;
  min : number = 0;
  max : number = 10000;
  map_key : string = "";
  map_id: string = "";
  map_code : string="";
  mapGroup ;
  searchStr : string = "";
  searchEndStr : string = "";
  searchViaStr : string = "";
  corridorName : string = "";
  startAddressPositionLat :number = 0; // = {lat : 18.50424,long : 73.85286};
  startAddressPositionLong :number = 0; // = {lat : 18.50424,long : 73.85286};
  startMarker : any;
  endMarker :any;
  routeCorridorMarker : any;
  routeOutlineMarker : any;
  endAddressPositionLat : number = 0;
  endAddressPositionLong : number = 0;

  
  // value: number = 100;
  options: Options = {
    floor: 0,
    ceil: 10000
  };
  searchStrError : boolean = false;
  searchEndStrError : boolean = false;
  strPresentStart: boolean = false;
  strPresentEnd: boolean = false;

  createFlag : boolean = true;

  constructor(private here: HereService,private _formBuilder: FormBuilder,private corridorService : CorridorService, private poiService: POIService,private completerService: CompleterService, private config: ConfigService) {

    this.map_key =  config.getSettings("hereMap").api_key;
    this.map_id =  config.getSettings("hereMap").app_id;
    this.map_code =  config.getSettings("hereMap").app_code;


   this.platform = new H.service.Platform({
     "apikey": this.map_key
   });
   this.configureAutoCompleteForLocationSearch();

   }

  ngOnInit(): void {
    this.vehicleGroupList.forEach(item => {
      this.vehicleGroupIdsSet.push(item.vehicleGroupId);
      this.vehicleGroupIdsSet = [...new Set(this.vehicleGroupIdsSet)];
    });
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.OrgId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.existingTripForm = this._formBuilder.group({
      // userGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidator]],
      corridorType:['Regular'],
      label: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      widthInput : ['', [Validators.required]],
      endaddress : ['', [Validators.required]],
      startaddress : ['', [Validators.required]],
      viaroute1: [''],
      viaroute2: [''],
      // trailer:["Regular"],
      // tollRoad:['Regular'],
      // motorWay:['Regular'],
      // boatFerries:['Regular'],
      // railFerries:['Regular'],
      // tunnels:['Regular'],
      // dirtRoad:['Regular'],
      // vehicleHeight:[''],
      // vehicleWidth: [''],
      // vehicleLength : [''],
      // limitedWeight: [''],
      // weightPerAxle: [''],


      vehicleGroup: ['', [Validators.required]],
      vehicle: ['', [Validators.required]],
      startDate: ['', [Validators.required]],
      startTime: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
      endTime: ['', [Validators.required]],
      // userGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
      {
        validator: [
          // CustomValidators.specialCharValidationForName('userGroupName'),
          // CustomValidators.specialCharValidationForNameWithoutRequired('userGroupDescription')
        ]
      });
    this.loadExistingTripData();
    this.setDefaultTodayDate();
  }

  public ngAfterViewInit() {
    this.initMap();
  }

  // setDefaultStartEndTime(){
  //   this.selectedStartTime = "00:00";
  //   this.selectedEndTime = "23:59";
  // }

  setDefaultTodayDate(){
    // this.selectionTab = 'today';
    // this.startDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedStartTime, 'start');
    // this.endDateValue = this.setStartEndDateTime(this.getTodayDate(), this.selectedEndTime, 'end');
    // this.last3MonthDate = this.getLast3MonthDate();
    // this.todayDate = this.getTodayDate();
    let todayDate = new Date(); //-- UTC
    // return todayDate;
    this.selectedStartDateStamp = todayDate;
    this.selectedEndDateStamp = todayDate;
    this.existingTripForm.controls.startDate.setValue(this.selectedStartDateStamp);
    this.existingTripForm.controls.endDate.setValue(this.selectedEndDateStamp);
    console.log("------defaults dates--",this.selectedStartDateStamp)

  }
  
  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    //Step 2: initialize a map - this map is centered over Europe
    this.hereMap = new H.Map(this.mapElement.nativeElement,
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
    var ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
    var group = new H.map.Group();
    this.mapGroup = group;
  }



  subscribeWidthValue(){
    this.existingTripForm.get("widthInput").valueChanges.subscribe(x => {
      console.log(x)
      this.corridorWidthKm = Number(x);
      this.corridorWidth = this.corridorWidthKm  * 1000;
      this.calculateAB();
   })
  }
  setCorridorData(){
    let _selectedElementData = this.selectedElementData;
    if(_selectedElementData){
      this.corridorId = _selectedElementData.id;
      if(this.corridorId){
          this.corridorService.getCorridorFullList(this.organizationId,this.corridorId).subscribe((data)=>{
              console.log(data)
              if(data[0]["corridorProperties"]){
                 this.additionalData =  data[0]["corridorProperties"];
                 this.setAdditionalData();
              
              }
          })
      }
      this.corridorName = _selectedElementData.corridoreName;
      this.existingTripForm.controls.label.setValue(_selectedElementData.corridoreName);
      this.searchStr = _selectedElementData.startPoint;
      this.searchEndStr = _selectedElementData.endPoint;
      this.startAddressPositionLat = _selectedElementData.startLat;
      this.startAddressPositionLong = _selectedElementData.startLong;
      this.endAddressPositionLat = _selectedElementData.endLat;
      this.endAddressPositionLong = _selectedElementData.endLong;
      this.corridorWidth = _selectedElementData.width;
      this.corridorWidthKm = this.corridorWidth / 1000;
      this.plotStartPoint(this.searchStr);
      this.plotEndPoint(this.searchEndStr);
      this.calculateAB()
    }
  }
  vehicleHeightValue: number = 0;
  vehicleWidthValue: number = 0;
  vehicleLengthValue: number = 0;
  vehicleLimitedWtValue: number = 0;
  vehicleWtPerAxleValue: number = 0;
  setAdditionalData(){
    let _data = this.additionalData;
    this.getAttributeData = _data["attribute"];
    this.getExclusionList = _data["exclusion"];
    // this.combustibleChecked = this.getAttributeData["isCombustible"];
    // this.corrosiveChecked = this.getAttributeData["isCorrosive"];
    // this.explosiveChecked = this.getAttributeData["isExplosive"];
    // this.flammableChecked = this.getAttributeData["isFlammable"];
    // this.gasChecked = this.getAttributeData["isGas"];
    // this.organicChecked = this.getAttributeData["isOrganic"];
    // this.othersChecked = this.getAttributeData["isOther"];
    // this.poisonChecked = this.getAttributeData["isPoision"];
    // this.poisonInhaleChecked = this.getAttributeData["isPoisonousInhalation"];
    // this.radioactiveChecked = this.getAttributeData["isRadioActive"];
    // this.waterHarmChecked = this.getAttributeData["isWaterHarm"];
    this.selectedTrailerId = this.getAttributeData["noOfTrailers"];
    this.trafficFlowChecked = _data["isTrafficFlow"];
    this.transportDataChecked = _data["isTransportData"];
    this.getVehicleSize = _data["vehicleSize"];
    this.vehicleHeightValue = this.getVehicleSize.vehicleHeight;
    this.vehicleWidthValue = this.getVehicleSize.vehicleWidth;
    this.vehicleLengthValue = this.getVehicleSize.vehicleLength;
    this.vehicleLimitedWtValue = this.getVehicleSize.vehicleLimitedWeight;
    this.vehicleWtPerAxleValue = this.getVehicleSize.vehicleWeightPerAxle;


    this.existingTripForm.controls.vehicleHeight.setValue(this.getVehicleSize.vehicleHeight);
    this.existingTripForm.controls.vehicleWidth.setValue(this.getVehicleSize.vehicleWidth);
    this.existingTripForm.controls.vehicleLength.setValue(this.getVehicleSize.vehicleLength);
    this.existingTripForm.controls.limitedWeight.setValue(this.getVehicleSize.vehicleLimitedWeight);
    this.existingTripForm.controls.weightPerAxle.setValue(this.getVehicleSize.vehicleWeightPerAxle);
    // this.tollRoadId = this.getExclusionList["tollRoadType"];
    // this.boatFerriesId = this.getExclusionList["boatFerriesType"];
    // this.dirtRoadId = this.getExclusionList["dirtRoadType"];
    // this.motorWayId = this.getExclusionList["mortorway"];
    // this.tunnelId = this.getExclusionList["tunnelsType"];
    // this.railFerriesId = this.getExclusionList["railFerriesType"];

    this.initiateDropDownValues();

  }

  initiateDropDownValues(){
    // this.existingTripForm.controls.trailer.setValue(this.selectedTrailerId);
    // this.trailerValue = this.selectedTrailerId;
    // this.existingTripForm.controls.tollRoad.setValue(this.tollRoadId);
    // this.tollRoadValue = this.exclusionList.filter(e=> e.enum === this.tollRoadId)[0].value;
    // this.existingTripForm.controls.motorWay.setValue(this.motorWayId);
    // this.motorWayValue = this.exclusionList.filter(e=> e.enum === this.motorWayId)[0].value;
    // this.existingTripForm.controls.boatFerries.setValue(this.boatFerriesId);
    // this.boatFerriesValue = this.exclusionList.filter(e=> e.enum === this.boatFerriesId)[0].value;
    // this.existingTripForm.controls.railFerries.setValue(this.railFerriesId);
    // this.railFerriesValue = this.exclusionList.filter(e=> e.enum === this.railFerriesId)[0].value;
    // this.existingTripForm.controls.tunnels.setValue(this.tunnelId);
    // this.tunnelValue = this.exclusionList.filter(e=> e.enum === this.tunnelId)[0].value;
    // this.existingTripForm.controls.dirtRoad.setValue(this.dirtRoadId);
    // this.dirtRoadValue = this.exclusionList.filter(e=> e.enum === this.dirtRoadId)[0].value;
    this.existingTripForm.controls.widthInput.setValue(this.corridorWidthKm);
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

  createViaMarker(){
    const viaMarker = `<svg width="26" height="32" viewBox="0 0 26 32" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M25 13C25 22.3333 13 30.3333 13 30.3333C13 30.3333 1 22.3333 1 13C1 9.8174 2.26428 6.76515 4.51472 4.51472C6.76516 2.26428 9.8174 1 13 1C16.1826 1 19.2348 2.26428 21.4853 4.51472C23.7357 6.76515 25 9.8174 25 13Z" stroke="#0D7EE7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.9998 29.6665C18.6665 24.9998 24.3332 19.2591 24.3332 12.9998C24.3332 6.74061 19.2591 1.6665 12.9998 1.6665C6.74061 1.6665 1.6665 6.74061 1.6665 12.9998C1.6665 19.2591 7.6665 25.3332 12.9998 29.6665Z" fill="#0D7EE7"/>
    <path d="M13 22.6665C18.5228 22.6665 23 18.4132 23 13.1665C23 7.9198 18.5228 3.6665 13 3.6665C7.47715 3.6665 3 7.9198 3 13.1665C3 18.4132 7.47715 22.6665 13 22.6665Z" fill="white"/>
    <path d="M19.7616 12.6263L14.0759 6.94057C13.9169 6.78162 13.7085 6.70215 13.5 6.70215C13.2915 6.70215 13.0831 6.78162 12.9241 6.94057L7.23842 12.6263C6.92053 12.9444 6.92053 13.4599 7.23842 13.778L12.9241 19.4637C13.0831 19.6227 13.2915 19.7021 13.5 19.7021C13.7085 19.7021 13.9169 19.6227 14.0759 19.4637L19.7616 13.778C20.0795 13.4599 20.0795 12.9444 19.7616 12.6263ZM13.5 18.3158L8.38633 13.2021L13.5 8.08848L18.6137 13.2021L13.5 18.3158ZM11.0625 12.999V15.0303C11.0625 15.1425 11.1534 15.2334 11.2656 15.2334H12.0781C12.1904 15.2334 12.2812 15.1425 12.2812 15.0303V13.4053H14.3125V14.7695C14.3125 14.8914 14.4123 14.9731 14.5169 14.9731C14.5644 14.9731 14.6129 14.9564 14.6535 14.9188L16.7916 12.9452C16.8787 12.8647 16.8787 12.7271 16.7916 12.6466L14.6535 10.673C14.6129 10.6357 14.5644 10.6187 14.5169 10.6187C14.4123 10.6187 14.3125 10.7004 14.3125 10.8223V12.1865H11.875C11.4263 12.1865 11.0625 12.5504 11.0625 12.999Z" fill="#0D7EE7"/>
    </svg>`

    return viaMarker;
  }

  loadExistingTripData() {
    // this.showLoadingIndicator = true;
    // this.corridorService.getCorridorList(this.accountOrganizationId).subscribe((data : any) => {
    //   this.initData = data;
    //   this.hideloader();
    //   this.updatedTableData(this.initData);
    // }, (error) => {
    //   this.initData = [];
    //   this.hideloader();
    //   this.updatedTableData(this.initData);
    // });
  }
  
  suggestionData :  any;
  dataService : any;
  private configureAutoCompleteForLocationSearch() {
    let searchParam = this.searchEndStr !== null ? this.searchEndStr : this.searchStr != null ? this.searchStr : this.searchViaStr;
    let AUTOCOMPLETION_URL = 'https://autocomplete.geocoder.cit.api.here.com/6.2/suggest.json' + '?' +
    '&maxresults=5' +  // The upper limit the for number of suggestions to be included in the response.  Default is set to 5.
    '&app_id=' + this.map_id + // TODO: Store this configuration in Config File.
    '&app_code=' + this.map_code +  // TODO: Store this configuration in Config File.
    '&query='+searchParam; 
    this.suggestionData = this.completerService.remote(
      AUTOCOMPLETION_URL,
      "label",
      "label");
    this.suggestionData.dataField("suggestions");
    this.dataService = this.suggestionData;
    console.log(this.dataService);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }
  vehicleGroupSelection(vehicleGroupValue: any) {
    this.vinList = [];
    // console.log("----vehicleGroupList---",this.vehicleGroupList)
    this.vehicleGroupList.forEach(item => {
      // this.vehicleGroupIdsSet.push(item.vehicleGroupId)
      if (item.vehicleGroupId == vehicleGroupValue.value) {
        this.vinList.push(item.vin)
      }
    });
  }

  // ------------- Map Functions ------------------------//
  plotStartPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.here.getLocationDetails(geocodingParameters).then((result) => {
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

  plotEndPoint(_locationId){
    let geocodingParameters = {
		  searchText: _locationId ,
		};
    this.here.getLocationDetails(geocodingParameters).then((result) => {
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

  viaAddressPositionLat : any;
  viaAddressPositionLong : any;
  viaRoutePlottedObject : any = [];
  viaMarker : any;
  plotViaPoint(_viaRouteList){
    this.viaRoutePlottedObject = [];
    if(this.viaMarker){
      this.hereMap.removeObjects([this.viaMarker]);
      this.viaMarker = null;
    }
    if(_viaRouteList.length >0){
      for(var i in _viaRouteList){

        let geocodingParameters = {
          searchText: _viaRouteList[i],
        };
        this.here.getLocationDetails(geocodingParameters).then((result) => {
          this.viaAddressPositionLat  = result[0]["Location"]["DisplayPosition"]["Latitude"];
          this.viaAddressPositionLong = result[0]["Location"]["DisplayPosition"]["Longitude"];
          let viaMarker = this.createViaMarker();
          let markerSize = { w: 26, h: 32 };
          const icon = new H.map.Icon(viaMarker, { size: markerSize, anchor: { x: Math.round(markerSize.w / 2), y: Math.round(markerSize.h / 2) } });
      
          this.viaMarker = new H.map.Marker({lat:this.viaAddressPositionLat, lng:this.viaAddressPositionLong},{icon:icon});
          this.hereMap.addObject(this.viaMarker);
         // this.hereMap.getViewModel().setLookAtData({bounds: this.endMarker.getBoundingBox()});
          //this.hereMap.setZoom(8);
          this.hereMap.setCenter({lat:this.viaAddressPositionLat, lng:this.viaAddressPositionLong}, 'default');
          this.viaRoutePlottedObject.push({
            "viaRoutName": _viaRouteList[i],
            "latitude": this.viaAddressPositionLat,
            "longitude": this.viaAddressPositionLat
          });
        this.checkRoutePlot();
    
        });
      }  
      
    }
    else{
      this.checkRoutePlot();

    }
    

    
  }

  calculateAB(){
    let viaPoints = [];
    for(var i in this.viaRoutePlottedObject){
      viaPoints.push(`${this.viaRoutePlottedObject[i]["latitude"]},${this.viaRoutePlottedObject[i]["longitude"]}`)
    }
    let routeRequestParams = {}
    if(viaPoints.length > 0){
      
    routeRequestParams = {
      'routingMode': 'fast',
      'transportMode': 'truck',
      'origin': `${this.startAddressPositionLat},${this.startAddressPositionLong}`, 
      'via': viaPoints,//`${this.viaAddressPositionLat},${this.viaAddressPositionLong}`,
      'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`, 
      'return': 'polyline'
    };
    }
    else{
      
    routeRequestParams = {
      'routingMode': 'fast',
      'transportMode': 'truck',
      'origin': `${this.startAddressPositionLat},${this.startAddressPositionLong}`,
      'destination': `${this.endAddressPositionLat},${this.endAddressPositionLong}`, 
      'return': 'polyline'
    };
    }
    console.log(viaPoints)
    // if(viaPoints.length>0){
    //   routeRequestParams["via"] = new H.service.Url.MultiValueQueryParameter(viaPoints);
    // }
    this.here.calculateRoutePoints(routeRequestParams).then((data)=>{
      
       this.addRouteShapeToMap(data);
    },(error)=>{
       console.error(error);
    })
  }

  addRouteShapeToMap(result){
    var group = new H.map.Group();
    // if(this.routeOutlineMarker){
    //   this.hereMap.removeObjects([this.routeOutlineMarker, this.routeCorridorMarker]);
    //   this.routeOutlineMarker = null;
    // }
    result.routes[0].sections.forEach((section) =>{
      let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);

      // Create a polyline to display the route:
      // let routeLine = new H.map.Polyline(linestring, {
      //   style: { strokeColor: '#436ddc', lineWidth: 3 } //b5c7ef
      // });
      // this.hereMap.addObject(routeLine);
      // this.hereMap.getViewModel().setLookAtData({bounds: routeLine.getBoundingBox()});
     // if (this.corridorWidthKm > 0) {
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
        this.hereMap.addObjects([this.routeOutlineMarker, this.routeCorridorMarker]);
        this.hereMap.getViewModel().setLookAtData({ bounds: this.routeCorridorMarker.getBoundingBox() });

      // }
      // else{
      //   this.routeOutlineMarker = null;
      //   this.routeCorridorMarker = null;

      // }

    });
  
    // // Add the polyline to the map
    // this.map.addObject(group);
    // // And zoom to its bounding rectangle
    // this.map.getViewModel().setLookAtData({
    //   bounds: group.getBoundingBox()
    // });
  }


  sliderChanged(){
    // this.corridorWidth = _event.value;
     this.corridorWidthKm = this.corridorWidth / 1000;
     this.existingTripForm.controls.widthInput.setValue(this.corridorWidthKm);
     this.checkRoutePlot();
     //this.calculateRouteFromAtoB();
 }

 checkRoutePlot(){
   if(this.startAddressPositionLat != 0 && this.endAddressPositionLat != 0){
     this.calculateAB();
   }
 }
 changeSliderInput(){
   this.corridorWidthKm = this.existingTripForm.controls.widthInput.value;
   this.corridorWidth = this.corridorWidthKm * 1000;
 }
 
 formatLabel(value:number){
   return value;
 }

 addViaRoute(){
   this.viaRouteCount = true;
 }

 removeViaRoute(){
   this.viaRouteCount = false;
 }

 transportDataCheckedFn(_checked){
   this.transportDataChecked = _checked;
 }

 
 trafficFlowCheckedFn(_checked){
   this.trafficFlowChecked = _checked;
 }

 attributeCheck(_checked, type) {
  //  switch (type) {
  //    case 'explosive':
  //      this.explosiveChecked = _checked;
  //      break;
  //    case 'gas':
  //      this.gasChecked = _checked;
  //      break;
  //    case 'flammable':
  //      this.flammableChecked = _checked;
  //      break;
  //    case 'combustible':
  //      this.combustibleChecked = _checked;
  //      break;
  //    case 'organic':
  //      this.organicChecked = _checked;
  //      break;
  //    case 'poison':
  //      this.poisonChecked = _checked;
  //      break;
  //    case 'radioactive':
  //      this.radioactiveChecked = _checked;
  //      break;
  //    case 'corrosive':
  //      this.corrosiveChecked = _checked;
  //      break;
  //    case 'poisonInhale':
  //      this.poisonInhaleChecked = _checked;
  //      break;
  //    case 'waterHarm':
  //      this.waterHarmChecked = _checked;
  //      break;
  //    case 'others':
  //      this.othersChecked = _checked;
  //      break;
  //    default:
  //      break;
  //  }
 }

 trailerSelected(_event){
   this.selectedTrailerId = _event.value;
 }

 exclusionSelected(_event,type){
  //  console.log(this.exclusionList);
  //  console.log(_event)
  //  switch (type) {
  //    case 'tollRoad':
  //        this.tollRoadId = _event.value;
  //      break;
  //      case 'motorWay':
  //        this.motorWayId = _event.value;
  //      break;
       
  //      case 'boatFerries':
  //        this.boatFerriesId = _event.value;
  //      break;
  //      case 'railFerries':
  //        this.railFerriesId = _event.value;
  //      break;
  //      case 'tunnel':
  //        this.tunnelId = _event.value;
  //      break;
  //      case 'dirtRoad':
  //        this.dirtRoadId = _event.value;
  //      break;
  //    default:
  //      break;
  //  }
 }

 createCorridorClicked(){
  
   var corridorObj = {
     "id": this.corridorId ? this.corridorId : 0,
     "organizationId": this.organizationId,
     "corridorType": "R",
     "corridorLabel":this.existingTripForm.controls.label.value,
     "startAddress": this.searchStr,
     "startLatitude": this.startAddressPositionLat,
     "startLongitude": this.startAddressPositionLong,
     "endAddress": this.searchEndStr,
     "endLatitude": this.endAddressPositionLat,
     "endLongitude": this.endAddressPositionLong,
     "width": this.corridorWidth,
     "viaAddressDetails": this.viaRoutePlottedObject,
     "transportData": this.transportDataChecked,
     "trafficFlow": this.trafficFlowChecked,
     "state": "A",
     "created_At": 0,
     "created_By": this.organizationId,
     "modified_At": 0,
     "modified_By": this.organizationId,
     "attribute": {
       "trailer": this.selectedTrailerId,
      //  "explosive": this.explosiveChecked,
      //  "gas": this.gasChecked,
      //  "flammable": this.flammableChecked,
      //  "combustible": this.combustibleChecked,
      //  "organic": this.organicChecked,
      //  "poision": this.poisonChecked,
      //  "radioActive": this.radioactiveChecked,
      //  "corrosive": this.corrosiveChecked,
      //  "poisonousInhalation": this.poisonInhaleChecked,
      //  "waterHarm": this.waterHarmChecked,
      //  "other": this.othersChecked
     },
     "exclusion": {
      //  "tollRoad": this.tollRoadId,
      //  "mortorway": this.motorWayId,
      //  "boatFerries":this.boatFerriesId,
      //  "railFerries": this.railFerriesId,
      //  "tunnels": this.tunnelId,
      //  "dirtRoad":this.dirtRoadId,
     },
     "vehicleSize": {
       "vehicleSizeHeight":this.existingTripForm.controls.vehicleHeight.value ? this.existingTripForm.controls.vehicleHeight.value : 0,
       "vehicleSizeWidth": this.existingTripForm.controls.vehicleWidth.value ? this.existingTripForm.controls.vehicleWidth.value : 0,
       "vehicleSizeLength": this.existingTripForm.controls.vehicleLength.value ? this.existingTripForm.controls.vehicleLength.value : 0,
       "vehicleSizeLimitedWeight": this.existingTripForm.controls.limitedWeight.value ? this.existingTripForm.controls.limitedWeight.value : 0,
       "vehicleSizeWeightPerAxle": this.existingTripForm.controls.weightPerAxle.value ? this.existingTripForm.controls.weightPerAxle.value : 0,
     }
   }
   console.log(corridorObj)
   this.corridorService.createRouteCorridor(corridorObj).subscribe((responseData)=>{
     if(responseData.code === 200){
         let emitObj = {
           booleanFlag: false,
           successMsg: "create",
           fromCreate:true,
         }  
         this.backToCreate.emit(emitObj);
     }
   },(error)=>{
       if(error.status === 409){
         let emitObj = {
           booleanFlag: false,
           successMsg: "duplicate",
           fromCreate:true,
         }  
         this.backToReject.emit(emitObj);
       }
   })
 }

 getSuggestion(_event){
   let startValue = _event.target.value;
   
  
   console.log(_event)
 }

 backToCorridorList(){
   let emitObj = {
     booleanFlag: false,
     successMsg: "",
   }  
   this.backToPage.emit(emitObj);
 }

 resetValues(){
   if(this.actionType === 'create'){
       
  //  this.tollRoadId = 'D';
  //  this.motorWayId ='D';
  //  this.railFerriesId = 'D';
  //  this.tunnelId ='D';
  //  this.dirtRoadId = 'D';
  //  this.boatFerriesId = 'D';
  //  this.explosiveChecked = false;
  //  this.gasChecked = false;
  //  this.flammableChecked  = false;
  //  this.combustibleChecked  = false;
  //  this.organicChecked  = false;
  //  this.poisonChecked  = false;
  //  this.radioactiveChecked  = false;
  //  this.corrosiveChecked  = false;
  //  this.poisonInhaleChecked  = false;
  //  this.waterHarmChecked  = false;
  //  this.othersChecked  = false;
   this.transportDataChecked = false;
   this.trafficFlowChecked = false;
   this.corridorWidth = 100;
   this.corridorWidthKm = 0.1;
   this.existingTripForm.controls.vehicleHeight.setValue("");
   this.existingTripForm.controls.vehicleLength.setValue("");
   this.existingTripForm.controls.vehicleWidth.setValue("");
   this.existingTripForm.controls.limitedWeight.setValue("");
   this.existingTripForm.controls.weightPerAxle.setValue("");
   this.existingTripForm.controls.startaddress.setValue("");
   this.existingTripForm.controls.endaddress.setValue("");
   }
   else{
     this.setAdditionalData();
   }
 }

 clearMap(){
   if(this.startMarker && this.endMarker ){
   this.hereMap.removeObjects([this.startMarker,this.endMarker,this.routeOutlineMarker,this.routeCorridorMarker]);

   }

 }


  onStartFocus(){
    this.searchStrError = true;
    this.strPresentStart = false;
    this.searchStr = null;
    this.startAddressPositionLat = 0;
    this.hereMap.removeObjects(this.hereMap.getObjects());
    
    if(this.searchEndStr){
      this.plotEndPoint(this.searchEndStr);
    }
    
  }
  onEndFocus(){
    this.searchEndStrError = true;
    this.strPresentEnd = false;
    this.searchEndStr = null;
    this.endAddressPositionLat = 0;
    this.hereMap.removeObjects(this.hereMap.getObjects());
    
    if(this.searchStr){
      this.plotStartPoint(this.searchStr);
    }
  }

  onSelected(selectedAddress: CompleterItem){
    //console.log(item.title)
    if(this.searchStr){
       this.searchStrError = false;
       this.strPresentStart = true;
    }
    if(selectedAddress){
      let postalCode = selectedAddress["originalObject"]["label"];
      this.plotStartPoint(postalCode)
    }

  }

  onEndSelected(selectedAddress: CompleterItem){
    
    if(this.searchEndStr){
      this.searchEndStrError = false;
      this.strPresentEnd = true;
      }
    if(selectedAddress){
      let locationId = selectedAddress["originalObject"]["label"]
      this.plotEndPoint(locationId)
    }

  }


  masterToggleForCorridor() {
    this.markerArray = [];
    if (this.isAllSelectedForCorridor()) {
      this.selectedCorridors.clear();
      this.showMap = false;
    }
    else {
      this.dataSource.data.forEach((row) => {
        this.selectedCorridors.select(row);
        this.markerArray.push(row);
      });
      this.showMap = true;
    }
    // this.addPolylineToMap();
  }

  isAllSelectedForCorridor() {
    const numSelected = this.selectedCorridors.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForCorridor(row?: any): string {
    if (row)
      return `${this.isAllSelectedForCorridor() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedCorridors.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  checkboxClicked(event: any, row: any) {
    
    let startAddress =  row.startPositionlattitude + "," + row.startPositionLongitude;
    let endAddress =  row.endPositionlattitude + "," + row.endPositionLongitude;
    // this.position = row.startPositionlattitude + "," + row.startPositionLongitude;
    this.here.getAddressFromLatLng(startAddress).then(result => {
      this.locations = <Array<any>>result;
      
      console.log("---location--",this.locations)
      this.setStartAddress = this.locations[0].Location.Address.Label;
      console.log("--hey Chckbox clicked startAddress---",this.setStartAddress)
      
      this.existingTripForm.get('startaddress').setValue(this.setStartAddress);
      // // //console.log(this.locations[0].Location.Address);
      // let pos = this.locations[0].Location.DisplayPosition;
      // // //console.log(data);
      // this.data = data;
      // thisRef.poiFlag = false;
      // thisRef.setAddressValues(data, pos);
    }, error => {
      // console.error(error);
    });
    
    this.here.getAddressFromLatLng(endAddress).then(result => {
      this.locations = <Array<any>>result;
      
      console.log("---location--",this.locations)
      this.setEndAddress = this.locations[0].Location.Address.Label;
      console.log("--hey Chckbox clicked endAddress---",this.setEndAddress)
      this.existingTripForm.get('endaddress').setValue(this.setEndAddress);
      // // //console.log(this.locations[0].Location.Address);
      // let pos = this.locations[0].Location.DisplayPosition;
      // // //console.log(data);
      // this.data = data;
      // thisRef.poiFlag = false;
      // thisRef.setAddressValues(data, pos);
    }, error => {
      // console.error(error);
    });



    this.showMap = this.selectedCorridors.selected.length > 0 ? true : false;
    //console.log(this.selectedpois.selected.length)
    //console.log(row);
    if (event.checked) { //-- add new marker
      this.markerArray.push(row);
    } else { //-- remove existing marker
      //It will filter out checked points only
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
    }
    // this.addPolylineToMap();
  }

  updatedTableData(tableData: any) {
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    console.log("------dataSource--", this.dataSource)
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getNewTagData(data: any) {
    let currentDate = new Date().getTime();
    if (data.length > 0) {
      data.forEach(row => {
        let createdDate = parseInt(row.createdAt);
        let nextDate = createdDate + 86400000;
        if (currentDate > createdDate && currentDate < nextDate) {
          row.newTag = true;
        }
        else {
          row.newTag = false;
        }
      });
      let newTrueData = data.filter(item => item.newTag == true);
      newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
      let newFalseData = data.filter(item => item.newTag == false);
      Array.prototype.push.apply(newTrueData, newFalseData);
      return newTrueData;
    }
    else {
      return data;
    }
  }
  timeChanged(selectedTime: any) {
    this.selectedStartTime = selectedTime;
    this.concateStartDateTimeInUTC(this.selectedStartDateStamp, this.selectedStartTime);
  }
  endtimeChanged(endTime: any) {
    this.selectedEndTime = endTime;
    this.concateEndDateTimeInUTC(this.selectedEndDateStamp, this.selectedEndTime);
  }

  selectedStartDate(startDate: any) {
    this.selectedStartDateStamp = moment(startDate.target.value).format('DD/MM/YYYY');
    // console.log("---selectedStartDate---",this.selectedStartDateStamp)
    // let dateTime = moment(this.selectedStartDateStamp + ' ' + this.selectedStartTime, 'DD/MM/YYYY HH:mm');
    // console.log(dateTime.format('DD-MM-YYYY HH:mm'))
    // this.startTimeUTC = moment.utc(dateTime).valueOf();
    // console.log("--startTimeUTC----UTC format",this.startTimeUTC)
    this.concateStartDateTimeInUTC(this.selectedStartDateStamp, this.selectedStartTime);
  }
  concateStartDateTimeInUTC(selectedDate: any, selectedTime: any) {
    let dateTime = moment(selectedDate + ' ' + selectedTime, 'DD/MM/YYYY HH:mm');
    // console.log("actual date and time value----",dateTime.format('DD-MM-YYYY HH:mm'))
    this.startTimeUTC = moment.utc(dateTime).valueOf();
    console.log("--startTimeUTC----UTC format", this.startTimeUTC)
  }
  selectedEndDate(endDate: any) {
    this.selectedEndDateStamp = moment(endDate.target.value).format('DD/MM/YYYY');
    // console.log("---selectedEndDate---",this.selectedEndDateStamp)
    // let dateTime = moment(this.selectedEndDateStamp + ' ' + this.selectedEndTime, 'DD/MM/YYYY HH:mm');
    // console.log(dateTime.format('DD-MM-YYYY HH:mm'))
    // this.endTimeUTC = moment.utc(dateTime).valueOf();
    // console.log("--endTimeUTC----UTC format",this.endTimeUTC)
    this.concateEndDateTimeInUTC(this.selectedEndDateStamp, this.selectedEndTime);
  }
  concateEndDateTimeInUTC(selectedDate: any, selectedTime: any) {
    let dateTime = moment(selectedDate + ' ' + selectedTime, 'DD/MM/YYYY HH:mm');
    let concateDateAndTime = dateTime.format('DD-MM-YYYY HH:mm');
    // console.log("actual date and time value----",dateTime.format('DD-MM-YYYY HH:mm'))
    this.endTimeUTC = moment.utc(dateTime).valueOf();
    console.log("--endTimeUTC----UTC format", this.endTimeUTC)
  }
  vinSelection(vinSelectedValue: any) {
    this.vinListSelectedValue = vinSelectedValue.value;
    console.log("------vins selection--",this.vinListSelectedValue)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  dateSelection(getDateValue:any) {
    console.log("---start date value--",this.existingTripForm.controls.startDate.value)
    let testDate= moment().format('DD/MM/YYYY HH:mm');
    // console.log("---getValue--",testDate);

    switch (getDateValue) {
      case "today" : 
      // let a = moment().subtract(1, 'day');
      this.selectedStartDateStamp = testDate;
      
      // let testValue = this.existingTripForm.get('startDate').setValue(testDate);
      // let a = moment(startDate.target.value).format('DD/MM/YYYY');
      // console.log("---from today--",testDate,"convertedValue--",testValue)
      // return a;
      break;
      
      case "yesterday" : 
      let b = moment().subtract(2, 'day');
      console.log("---from today--",b)
      return b;
      break;
      
      case "lastWeek" : ""
      break;

      case "lastMonth" : ""
      break;

      case "last3Months": ""
      break;
      
      default: 
      return "hey default";
   }
  }

  onReset() {

  }

  onSearch() {
    console.log("---Search calling---")
    // this.poiService.getalltripdetails(this.accountOrganizationId).subscribe((data: any) => {
    //     VIN: 5A65654
    // start date: 1616961846000
    // end date: 1616963318000

    //     VIN: NBVGF1254KLJ55
    // start date: 1604327461000
    // end date: 1604336647000

    //----------This is only test data-----
    // this.startTimeUTC = 1616961846000;
    // this.endTimeUTC = 1616963318000;
    // this.vinListSelectedValue= "5A65654";

    this.poiService.getalltripdetails(this.startTimeUTC, this.endTimeUTC, this.vinListSelectedValue).subscribe((existingTripDetails: any) => {
      console.log("--existingTripData----", existingTripDetails)
      this.showLoadingIndicator = true;
      this.initData = existingTripDetails.tripData;
      console.log("--initData----", this.initData.length)
      this.hideloader();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedTableData(this.initData);
    });
    //   }

  }


}