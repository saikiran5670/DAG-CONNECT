import { SelectionModel } from '@angular/cdk/collections';
import { Component, Input, Output, EventEmitter,OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { CorridorService } from 'src/app/services/corridor.service';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { MapFunctionsService } from './map-functions.service';
declare var H: any;

@Component({
  selector: 'app-manage-corridor',
  templateUrl: './manage-corridor.component.html',
  styleUrls: ['./manage-corridor.component.less']
})

export class ManageCorridorComponent implements OnInit {
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  @Input() translationData: any;
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  displayedColumns = ['All', 'corridoreName', 'startPoint', 'endPoint', 'distance', 'width', 'action'];
  createEditStatus = false;
  accountOrganizationId: any = 0;
  corridorCreatedMsg: any = '';
  actionType : string;
  titleVisible : boolean = false;
  titleFailVisible: boolean = false;
  showMap: boolean = false;
  map: any;
  initData: any = [];
  dataSource: any;
  markerArray: any = [];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @ViewChild("map")
  public mapElement: ElementRef;
  platform: any;
  selectedElementData: any;
  showLoadingIndicator: boolean;
  localStLanguage: any;
  selectedCorridors = new SelectionModel(true, []);

  
  constructor( 
    private dialogService: ConfirmDialogService, 
    private corridorService : CorridorService,
    private _snackBar: MatSnackBar,
    private mapFunctions: MapFunctionsService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
   }

  ngOnInit(){ 
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.loadCorridorData();
  }

  loadCorridorData(){
    this.showLoadingIndicator = true;
    this.corridorService.getCorridorList(this.accountOrganizationId).subscribe((data : any) => {
      this.initData = data;
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
    });
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
    this.mapFunctions.initMap(this.mapElement);
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
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditStatus = true;
  }

  deleteCorridor(rowData: any){
    let corridorId = rowData.id;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.corridoreName);
    let deleteMsg :string = "";
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.corridorService.deleteCorridor(corridorId).subscribe((data) => {
        this.openSnackBar('Item delete', 'dismiss');
        this.loadCorridorData();
        if(data.code === 200){
          this.successMsgBlink(this.getDeletMsg(rowData.corridoreName));
        }
      },
      (error)=>{
        if(error.status === 500){
          const options = {
            title: this.translationData.lblDelete || "Delete",
            message: this.translationData.lblAreyousureyouwanttodelete || "Alert exists for corridor. You cannot deleted this corridor if there is an alert set for it. To remove this Corridor, first remove connected alerts.",
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
      //console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      //console.log('The snackbar action was triggered!');
    });
  }

  getDeletMsg(name: any) {
    if (this.translationData.lblCorridorwassuccessfullydeleted)
      return this.translationData.lblCorridorwassuccessfullydeleted.replace('$', name);
    else
      return ("Corridor '$' was successfully deleted").replace('$', name);
  }

  getNoDeletMsg(name: any) {
    if (this.translationData.lblCorridorwassuccessfullydeleted)
      return this.translationData.lblCorridorwassuccessfullydeleted.replace('$', name);
    else
      return ("Corridor '$' cannot be deleted, it is associated with alert").replace('$', name);
  }
  masterToggleForCorridor() {
    this.markerArray = [];
    if(this.isAllSelectedForCorridor()){
      this.selectedCorridors.clear();
      this.mapFunctions.clearRoutesFromMap();
      this.showMap = false;
    }
    else{
      this.dataSource.data.forEach((row) =>{
        this.selectedCorridors.select(row);
        this.markerArray.push(row);
      });
      this.mapFunctions.viewSelectedRoutes(this.markerArray);
      this.showMap = true;
    }
    console.log(this.markerArray);
    
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
    //console.log(this.selectedpois.selected.length)
    //console.log(row);
    if(event.checked){ //-- add new marker
      this.markerArray.push(row);
      
    this.mapFunctions.viewSelectedRoutes(this.markerArray);
    }else{ //-- remove existing marker
      //It will filter out checked points only
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
      this.mapFunctions.clearRoutesFromMap();

      }

     // this.addPolylineToMap();
  }

  addPolylineToMap(){
    console.log(this.markerArray);
    var lineString = new H.geo.LineString();
    this.markerArray.forEach(element => {
      console.log(element.startLat)
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
      var _msg = "Corridor created successfully!"
      this.successMsgBlink(_msg);
    }
    else if(_eventObj.successMsg=="reject"){
        var _msg = "Corridor label exists!"
        this.failureMsgBlink(_msg);
    }
    this.loadCorridorData();
    setTimeout(() => {
      this.mapFunctions.initMap(this.mapElement);
      }, 0);
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }
}
