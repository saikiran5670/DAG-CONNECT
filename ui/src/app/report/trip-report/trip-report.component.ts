import { SelectionModel } from '@angular/cdk/collections';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../services/translation.service';

declare var H: any;

@Component({
  selector: 'app-trip-report',
  templateUrl: './trip-report.component.html',
  styleUrls: ['./trip-report.component.less']
})

export class TripReportComponent implements OnInit {
  displayedColumns = ['All', 'startDate', 'endDate', 'distance', 'idleDuration', 'avgSpeed', 'avgWeight'];
  translationData: any;
  hereMap: any;
  platform: any;
  ui: any;
  @ViewChild("map")
  public mapElement: ElementRef;
  showMap: boolean = false;
  showMapPanel: boolean = false;
  searchExpandPanel: boolean = true;
  tableExpandPanel: boolean = true;
  initData: any = [];
  localStLanguage: any;
  accountOrganizationId: any;
  vehicleGroupListData: any = [];
  vehicleListData: any = [];
  dataSource: any = new MatTableDataSource([]);
  selectedTrip = new SelectionModel(true, []);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  tripData: any = [];

  constructor(private translationService: TranslationService) {
    this.platform = new H.service.Platform({
      "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
    });
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearchReportParameters: 'Search Report Parameters'
    }    
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 6 //-- for Trip Report
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.loadTripData();
    });
  }

  loadTripData(){
    this.tripData = [{
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 128.9, 
      idleDuration: '00:12', 
      avgSpeed: 54.5, 
      avgWeight: 6.45
    },
    {
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 123.9, 
      idleDuration: '00:18', 
      avgSpeed: 32.5, 
      avgWeight: 7.45
    },
    {
      startDate: '01/01/2021 00:00:00', 
      endDate: '01/01/2021 23:59:59', 
      distance: 18.9, 
      idleDuration: '00:02', 
      avgSpeed: 5.2, 
      avgWeight: 3.0
    }];
    this.updateDataSource(this.tripData);
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  public ngAfterViewInit() {
    // setTimeout(() => {
    // this.initMap();
    // }, 0);
  }

  initMap(){
    let defaultLayers = this.platform.createDefaultLayers();
    this.hereMap = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 51.43175839453286, lng: 5.519981221425336 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.hereMap.getViewPort().resize());
    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.ui = H.ui.UI.createDefault(this.hereMap, defaultLayers);
  }

  onSearch(){

  }

  onReset(){

  }

  onVehicleGroupChange(event: any){

  }

  onVehicleChange(event: any){

  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // dataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  updateDataSource(tableData: any) {
    this.initData = tableData;
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  exportAsExcelFile(){

  }

  exportAsPDFFile(){
    
  }

  masterToggleForTrip() {
    //this.geoMarkerArray = [];
    if(this.isAllSelectedForTrip()){
      this.selectedTrip.clear();
      //this.showMap = (this.selectedTrip.selected.length > 0 || this.selectedpois.selected.length > 0) ? true : false;
    }
    else{
      this.dataSource.data.forEach((row) =>{
        this.selectedTrip.select(row);
        //this.geoMarkerArray.push(row);
      });
      //this.showMap = (this.selectedTrip.selected.length > 0 || this.selectedpois.selected.length > 0) ? true : false;
    }
    // this.removeMapObjects(); //-- remove all object first
    // if(this.selectedTrip.selected.length > 0){ //-- add geofences
    //   this.addCirclePolygonOnMap();
    // }
    // if(this.selectedpois.selected.length > 0){ //-/ add poi
    //   this.addMarkerOnMap(this.ui);
    // }
  }

  isAllSelectedForTrip() {
    const numSelected = this.selectedTrip.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForTrip(row?: any): string {
    if (row)
      return `${this.isAllSelectedForTrip() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedTrip.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  tripCheckboxClicked(event: any, row: any) {
    if(event.checked){ 
      //this.geoMarkerArray.push(row);
    }else{ 
      // let arr = this.geoMarkerArray.filter(item => item.id != row.id);
      // this.geoMarkerArray = arr;
    }
    // this.showMap = (this.selectedgeofences.selected.length > 0 || this.selectedpois.selected.length > 0) ? true : false;
    // this.removeMapObjects();
    // this.addCirclePolygonOnMap();
    // if(this.selectedpois.selected.length > 0){ //-- poi selected
    //   this.addMarkerOnMap(this.ui);
    // }
  }

}
