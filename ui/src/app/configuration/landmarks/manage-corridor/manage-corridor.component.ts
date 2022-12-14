import { SelectionModel } from '@angular/cdk/collections';
import { Component, Input, Output, EventEmitter,OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfigService } from '@ngx-config/core';
import { FleetMapService } from 'src/app/live-fleet/current-fleet/fleet-map.service';
import { CorridorService } from 'src/app/services/corridor.service';
import { HereService } from 'src/app/services/here.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { Util } from 'src/app/shared/util';
import { MapFunctionsService } from './map-functions.service';
import { CompleterCmp, CompleterData, CompleterItem, CompleterService, RemoteData } from 'ng2-completer';
declare var H: any;

@Component({
  selector: 'app-manage-corridor',
  templateUrl: './manage-corridor.component.html',
  styleUrls: ['./manage-corridor.component.less']
})

export class ManageCorridorComponent implements OnInit {
  adminAccessType: any = {};
  @Input() translationData: any = {};
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  displayedColumns = ['All', 'corridoreName', 'startPoint', 'endPoint', 'distance', 'width', 'action'];
  createEditStatus = false;
  accountOrganizationId: any = 0;
  searchMarker: any = {};
  corridorCreatedMsg: any = '';
  trackType: any = 'snail';
  suggestionData : any;
  actionType : string;
  titleVisible : boolean = false;
  titleFailVisible: boolean = false;
  showMap: boolean = false;
  map: any;
  initData: any = [];
  dataSource: any;
  markerArray: any = [];
  corridorNameList = [];
  userPOIList: any = [];
  herePOIList: any = [];
  displayPOIList: any = [];
  dataService: any;
  routeType = 'R';
  displayRouteView: any = 'C';
  corridorTypeId = 46;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @ViewChild("map")
  public mapElement: ElementRef;
  platform: any;
  selectedElementData: any;
  showLoadingIndicator: boolean;
  localStLanguage: any;
  map_key: any = '';
  selectedCorridors = new SelectionModel(true, []);
  filterValue: string;
  searchStr: any;
  tripTraceArray: any;
  startMarker: any;


  constructor(
    private dialogService: ConfirmDialogService,
    private hereService: HereService,
    private fleetMapService:FleetMapService,
    private corridorService : CorridorService,
    private _snackBar: MatSnackBar,
    private mapFunctions: MapFunctionsService,
    private completerService: CompleterService,
    private _configService: ConfigService)  {
      // this.map_key = _configService.getSettings("hereMap").api_key;
      this.map_key = localStorage.getItem("hereMapsK");
      this.platform = new H.service.Platform({
        "apikey": this.map_key
      });
      this.configureAutoSuggest();
  }
  private configureAutoSuggest(){
    let searchParam = this.searchStr != null ? this.searchStr : '';
    let URL = 'https://autocomplete.search.hereapi.com/v1/autocomplete?'+'apiKey='+this.map_key +'&limit=5'+'&q='+searchParam ;
  // let URL = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json'+'?'+ '&apiKey='+this.map_key+'&limit=5'+'&query='+searchParam ;
    this.suggestionData = this.completerService.remote(
    URL,'title','title');
    this.suggestionData.dataField("items");
    this.dataService = this.suggestionData;
  }
  onSearchFocus(){
    this.searchStr = null;
  }
  onSearchSelected(selectedAddress: CompleterItem) {
    if (selectedAddress) {
      let id = selectedAddress["originalObject"]["id"];
      let qParam = 'apiKey=' + this.map_key + '&id=' + id;
      this.hereService.lookUpSuggestion(qParam).subscribe((data: any) => {
        this.searchMarker = {};
        if (data && data.position && data.position.lat && data.position.lng) {
          this.searchMarker = {
            lat: data.position.lat,
            lng: data.position.lng,
            from: 'search'
          };
          this.showSearchMarker(this.searchMarker);
              }
      });
    }
  }

  ngOnInit(){
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.adminAccessType = JSON.parse(localStorage.getItem("accessType"));
    this.loadCorridorData();
  }

  loadCorridorData(){
    this.showLoadingIndicator = true;
    this.corridorService.getCorridorList(this.accountOrganizationId).subscribe((data : any) => {
      this.initData = data;
      this.corridorNameList = data.map(e=> e.corridoreName)
      this.hideloader();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedTableData(this.initData);
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  updatedTableData(tableData : any) {
    tableData = this.getNewTagData(tableData);
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data:String[], sort: MatSort) => {
        const isAsc = sort.direction === "asc";
        let columnName = this.sort.active;
        return data.sort((a : any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc,columnName);
        });
      }
    });
    Util.applySearchFilter(this.dataSource, this.displayedColumns ,this.filterValue );
  }

  compare(a:Number | String, b: Number | String, isAsc: boolean, columnName: any){
    if(columnName == 'corridoreName'|| columnName == 'startPoint'|| columnName == 'endPoint'){
      if(!(a instanceof Number)) a = a.replace(/[^\w\s]/gi, 'z').toUpperCase();
      if(!(b instanceof Number)) b = b.replace(/[^\w\s]/gi, 'z').toUpperCase();
    }
    return (a< b ? -1 : 1) * (isAsc ? 1: -1);
  }


  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    if(data.length > 0){
      data.forEach(row => {
        let createdDate = parseInt(row.createdAt);
        let nextDate = createdDate + 86400000;
        if(currentDate > createdDate && currentDate < nextDate){
          row.newTag = true;
        }
        else{
          row.newTag = false;
        }
      });
      let newTrueData = data.filter(item => item.newTag == true);
      newTrueData.sort((userobj1, userobj2) => parseInt(userobj2.createdAt) - parseInt(userobj1.createdAt));
      let newFalseData = data.filter(item => item.newTag == false);
      Array.prototype.push.apply(newTrueData, newFalseData);
      return newTrueData;
    }
    else{
      return data;
    }
  }

  public ngAfterViewInit() {
    setTimeout(() => {
      this.initMap();
    }, 0);
  }

  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    this.map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.map.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
    var ui = H.ui.UI.createDefault(this.map, defaultLayers);
    var group = new H.map.Group();
  }

  onClose() {
    this.titleVisible = false;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  onNewCorridor(){
    this.actionType = "create";
    this.createEditStatus = true;
    this.tabVisibility.emit(false);

  }

  editViewCorridor(rowData: any, type: any){
    this.tabVisibility.emit(false);
    this.actionType = type;
    this.selectedElementData = rowData;
    if(this.selectedElementData.corridorType === 'R'){
      this.corridorTypeId = 46;
    }
    else{
      this.corridorTypeId = 45;
    }
    this.createEditStatus = true;
  }

  deleteCorridor(rowData: any){
    let corridorId = rowData.id;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodeleteCorridor + " '$'?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.corridoreName);
    let deleteMsg :string = "";
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.corridorService.deleteCorridor(corridorId).subscribe((data) => {
        // this.openSnackBar('Item delete', 'dismiss');
        this.loadCorridorData();
        if(data.code === 200){
          this.successMsgBlink(this.getDeletMsg(rowData.corridoreName));
        }
      },
      (error)=>{
        if(error.status === 500 || error.status === 409){
          const options = {
            title: this.translationData.lblDelete || "Delete",
            message: this.translationData.lblAlertsAlreadyAssociatesWithCorridorMsg || "Alert exists for corridor. You cannot deleted this corridor if there is an alert set for it. To remove this Corridor, first remove connected alerts.",
            cancelText: this.translationData.lblCancel || "Cancel",
            confirmText: 'hide-btn'
          };
          this.dialogService.DeleteModelOpen(options);
        }
      })

      }
    });
  }

  successMsgBlink(msg: any) {
    this.titleVisible = true;
    this.corridorCreatedMsg = msg;
    setTimeout(() => {
      this.titleVisible = false;
    }, 5000);
  }

  failureMsgBlink(msg: any) {
    this.titleFailVisible = true;
    this.corridorCreatedMsg = msg;
    setTimeout(() => {
      this.titleFailVisible = false;
    }, 5000);
  }
  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      ////console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      ////console.log('The snackbar action was triggered!');
    });
  }

  getcreatedMsg(name: any) {
    if (this.translationData.lblCreatedSuccessfully)
      return this.translationData.lblCorridorCreatedSuccessfully.replace('$', name);
    else
      return ("Corridor '$' was created successfully").replace('$', name);
  }

  getDeletMsg(name: any) {
    if (this.translationData.lblCorridorwassuccessfullydeleted)
      return this.translationData.lblCorridorwassuccessfullydeleted.replace('$', name);
    else
      return ("Corridor '$' was successfully deleted").replace('$', name);
  }

  getNoDeletMsg(name: any) {
    if (this.translationData.lblCorridorwassuccessfullydeleted)
      return this.translationData.lblCorridorwasnotdeleted.replace('$', name);
    else
      return ("Corridor '$' cannot be deleted, it is associated with alert").replace('$', name);
  }
  masterToggleForCorridor() {
    this.markerArray = [];
    this.mapFunctions.clearRoutesFromMap();

    if(this.isAllSelectedForCorridor()){
      this.selectedCorridors.clear();
      this.showMap = false;
    }
    else{
      this.dataSource.data.forEach((row) =>{
        this.selectedCorridors.select(row);
        this.markerArray.push(row);
      });
      this.mapFunctions.viewSelectedRoutes(this.markerArray,this.accountOrganizationId,this.translationData);
      this.showMap = true;
    }
  //  //console.log(this.markerArray);

    //this.addPolylineToMap();
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
    this.showMap = this.selectedCorridors.selected.length > 0 ? true : false;
    ////console.log(this.selectedpois.selected.length)
    ////console.log(row);
    this.mapFunctions.clearRoutesFromMap();
    if(event.checked){ //-- add new marker
      this.markerArray.push(row);
    //this.mapFunctions.viewSelectedRoutes(this.markerArray,this.accountOrganizationId);
    }else{ //-- remove existing marker
      //It will filter out checked points only
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
      }
      this.mapFunctions.viewSelectedRoutesCorridor(this.markerArray,this.accountOrganizationId,this.translationData);
    // this.mapFunctions.viewSelectedRoutes(this.markerArray,this.accountOrganizationId);

     this.addPolylineToMap();
  }

  addPolylineToMap(){
    //console.log(this.markerArray);
    var lineString = new H.geo.LineString();
    this.markerArray.forEach(element => {
      //console.log(element.startLat)
    lineString.pushPoint({lat : element.startLat, lng: element.startLong});
    lineString.pushPoint({lat : element.endLat, lng: element.endLong});
    // lineString.pushPoint({lat:48.8567, lng:2.3508});
    // lineString.pushPoint({lat:52.5166, lng:13.3833});
    });

  this.map.addObject(new H.map.Polyline(
    lineString, { style: { lineWidth: 4 }}
  ));
  }

  // addMarkerOnMap(){
  //   this.map.removeObjects(this.map.getObjects());
  //   this.markerArray.forEach(element => {
  //     let marker = new H.map.Marker({ lat: 48.8567, lng: 2.3508 }, { icon: this.getSVGIcon() });
  //     this.map.addObject(marker);
  //   });
  // }

  getSVGIcon(){
    let markup = '<svg xmlns="http://www.w3.org/2000/svg" width="28px" height="36px" >' +
    '<path d="M 19 31 C 19 32.7 16.3 34 13 34 C 9.7 34 7 32.7 7 31 C 7 29.3 9.7 ' +
    '28 13 28 C 16.3 28 19 29.3 19 31 Z" fill="#000" fill-opacity=".2"></path>' +
    '<path d="M 13 0 C 9.5 0 6.3 1.3 3.8 3.8 C 1.4 7.8 0 9.4 0 12.8 C 0 16.3 1.4 ' +
    '19.5 3.8 21.9 L 13 31 L 22.2 21.9 C 24.6 19.5 25.9 16.3 25.9 12.8 C 25.9 9.4 24.6 ' +
    '6.1 22.1 3.8 C 19.7 1.3 16.5 0 13 0 Z" fill="#fff"></path>' +
    '<path d="M 13 2.2 C 6 2.2 2.3 7.2 2.1 12.8 C 2.1 16.1 3.1 18.4 5.2 20.5 L ' +
    '13 28.2 L 20.8 20.5 C 22.9 18.4 23.8 16.2 23.8 12.8 C 23.6 7.07 20 2.2 ' +
    '13 2.2 Z" fill="${COLOR}"></path><text transform="matrix( 1 0 0 1 13 18 )" x="0" y="0" fill-opacity="1" ' +
    'fill="#fff" text-anchor="middle" font-weight="bold" font-size="13px" font-family="arial" style="fill:black"></text></svg>';

    let locMarkup = '<svg height="24" version="1.1" width="24" xmlns="http://www.w3.org/2000/svg" xmlns:cc="http://creativecommons.org/ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"><g transform="translate(0 -1028.4)"><path d="m12 0c-4.4183 2.3685e-15 -8 3.5817-8 8 0 1.421 0.3816 2.75 1.0312 3.906 0.1079 0.192 0.221 0.381 0.3438 0.563l6.625 11.531 6.625-11.531c0.102-0.151 0.19-0.311 0.281-0.469l0.063-0.094c0.649-1.156 1.031-2.485 1.031-3.906 0-4.4183-3.582-8-8-8zm0 4c2.209 0 4 1.7909 4 4 0 2.209-1.791 4-4 4-2.2091 0-4-1.791-4-4 0-2.2091 1.7909-4 4-4z" fill="#55b242" transform="translate(0 1028.4)"/><path d="m12 3c-2.7614 0-5 2.2386-5 5 0 2.761 2.2386 5 5 5 2.761 0 5-2.239 5-5 0-2.7614-2.239-5-5-5zm0 2c1.657 0 3 1.3431 3 3s-1.343 3-3 3-3-1.3431-3-3 1.343-3 3-3z" fill="#ffffff" transform="translate(0 1028.4)"/></g></svg>';

    //let icon = new H.map.Icon(markup.replace('${COLOR}', '#55b242'));
    let icon = new H.map.Icon(locMarkup);
    return icon;
  }

  onBackToPage(_eventObj) {
   this.createEditStatus = false;

    this.tabVisibility.emit(true);
    if(_eventObj.successMsg=="create"){
      var _msg = this.translationData.lblCorridorCreatedSuccessfully.replace('$', _eventObj.CreateCorridorName);//;  "Corridor '"+_eventObj.CreateCorridorName+"' created successfully!"
      this.successMsgBlink(_msg);
    }
    else if(_eventObj.successMsg=="update"){
      var _msg:any = '';
      if(this.translationData.lblCorridorUpdatedSuccessfully) {
        _msg = this.translationData.lblCorridorUpdatedSuccessfully.replace('$', _eventObj.corridorName);
      } else {
        _msg = ("'$' Corridor Updated Successfully").replace('$', _eventObj.corridorName);
      }
      this.successMsgBlink(_msg);
  }
    else if(_eventObj.successMsg=="reject"){
        var _msg = this.translationData.lblCorridorexists;//"Corridor label exists!"
        this.failureMsgBlink(_msg);
    }
    this.loadCorridorData();
    setTimeout(() => {
      this.mapFunctions.initMap(this.mapElement, this.translationData);
      }, 0);
  }


  moveMapToSelectedPOI(map, lat, lon){
    map.setCenter({lat:lat, lng:lon});
    map.setZoom(15);
  }
  getCategoryPOIIcon(){
    let locMarkup = `<svg width="25" height="39" viewBox="0 0 25 39" fill="none" xmlns="http://www.w3.org/2000/svg">
    <path d="M22.9991 12.423C23.2909 20.9156 12.622 28.5702 12.622 28.5702C12.622 28.5702 1.45279 21.6661 1.16091 13.1735C1.06139 10.2776 2.11633 7.46075 4.09368 5.34265C6.07103 3.22455 8.8088 1.9787 11.7047 1.87917C14.6006 1.77965 17.4175 2.83459 19.5356 4.81194C21.6537 6.78929 22.8995 9.52706 22.9991 12.423Z" stroke="#00529C" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
    <path d="M12.6012 37.9638C12.6012 37.9638 22.5882 18.1394 22.3924 12.444C22.1967 6.74858 17.421 2.29022 11.7255 2.48596C6.03013 2.6817 1.57177 7.45742 1.76751 13.1528C1.96325 18.8482 12.6012 37.9638 12.6012 37.9638Z" fill="#00529C"/>
    <path d="M12.3824 21.594C17.4077 21.4213 21.3486 17.4111 21.1845 12.637C21.0204 7.86293 16.8136 4.13277 11.7882 4.30549C6.76283 4.4782 2.82198 8.48838 2.98605 13.2625C3.15013 18.0366 7.357 21.7667 12.3824 21.594Z" fill="white"/>
    </svg>`;
      return locMarkup;
  }

  showSearchMarker(markerData: any){
    if(markerData && markerData.lat && markerData.lng){
      //let selectedMarker = new H.map.Marker({ lat: markerData.lat, lng: markerData.lng });
      if(markerData.from && markerData.from == 'search'){
        let locMarkup = this.getCategoryPOIIcon();
        let markerSizeIcon = { w: 25, h: 39 };
        const locIcon = new H.map.Icon(locMarkup, { size: markerSizeIcon, anchor: { x: Math.round(markerSizeIcon.w / 2), y: Math.round(markerSizeIcon.h / 2) } });
        let locMarkupIcon = new H.map.Marker({ lat: markerData.lat, lng: markerData.lng }, { icon: locIcon });
        this.map.addObject(locMarkupIcon);
        this.map.setCenter({lat: markerData.lat, lng: markerData.lng}, 'default');
      }
      //this.map.addObject(selectedMarker);
    }
  }


  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }
}
