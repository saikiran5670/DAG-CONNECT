import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { POIService } from 'src/app/services/poi.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';
import { MatTableExporterDirective } from 'mat-table-exporter';
import { GeofenceService } from 'src/app/services/landmarkGeofence.service';
import { QueryList } from '@angular/core';
import { ViewChildren } from '@angular/core';
import { LandmarkCategoryService } from 'src/app/services/landmarkCategory.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { isNgTemplate } from '@angular/compiler';
import { ElementRef } from '@angular/core';

declare var H: any;
const createGpx = require('gps-to-gpx').default;

@Component({
  selector: 'app-manage-poi-geofence',
  templateUrl: './manage-poi-geofence.component.html',
  styleUrls: ['./manage-poi-geofence.component.less']
})
export class ManagePoiGeofenceComponent implements OnInit {
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  showLoadingIndicator: any = false;
  @Input() translationData: any;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  displayedColumnsPoi = ['All', 'Icon', 'name', 'categoryName', 'subCategoryName', 'address', 'Actions'];
  displayedColumnsGeo = ['All', 'name', 'categoryName', 'subCategoryName', 'Actions'];
  poidataSource: any;
  geofencedataSource: any;
  accountOrganizationId: any = 0;
  accountId: any = 0;
  localStLanguage: any;
  poiInitData: any = [];
  geoInitData: any = [];
  data: any = [];
  selectedElementData: any;
  titleVisible: boolean = false;
  poiCreatedMsg: any = '';
  actionType: any;
  roleID: any;
  platform: any;
  showMap: boolean = false;
  createEditViewPoiFlag: boolean = false;
  createEditViewGeofenceFlag: boolean = false;
  mapFlag: boolean = false;
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  selectedpois = new SelectionModel(true, []);
  selectedgeofences = new SelectionModel(true, []);
  @Output() tabVisibility: EventEmitter<boolean> = new EventEmitter();
  categoryList: any = [];
  subCategoryList: any = [];
  // private _snackBar: any;
  initData: any[];
  importPOIClicked: boolean = false;
  importGeofenceClicked : boolean = false;
  importClicked: boolean = false;
  impportTitle = "Import POI";
  importTranslationData: any = {};
  xmlObject : any = {};
  map: any;
  templateTitle = ['OrganizationId', 'CategoryId', 'CategoryName', 'SubCategoryId', 'SubCategoryName',
    'POIName', 'Address', 'City', 'Country', 'Zipcode', 'Latitude', 'Longitude', 'Distance', 'State', 'Type'];
  templateValue = [
    [36, 10, 'CategoryName', 8, 'SubCategoryName', "PoiTest",
      'Pune', 'Pune', 'India', '411057', 51.07, 57.07, 12, 'Active', 'POI']];
  tableColumnList = ['organizationId', 'categoryId',  'subCategoryId', 
    'poiName', 'latitude', 'longitude', 'returnMessage'];
  tableColumnName = ['OrganizationId', 'CategoryId',  'SubCategoryId',
    'POIName', 'Latitude', 'Longitude', 'Fail Reason'];
  tableTitle = 'Rejected POI Details';
  @Output() showImportCSV: EventEmitter<any> = new EventEmitter();
  selectedCategoryId = null;
  selectedSubCategoryId = null;
  allCategoryPOIData : any;
  defaultGpx : any;

  @ViewChild("map")
  public mapElement: ElementRef;
  markerArray: any = [];
  hereMap: any;
  
  constructor( 
    private dialogService: ConfirmDialogService,
    private poiService: POIService,
    private geofenceService: GeofenceService,
    private landmarkCategoryService: LandmarkCategoryService,
    private _snackBar: MatSnackBar
    ) {
      
      this.platform = new H.service.Platform({
        "apikey": "BmrUv-YbFcKlI4Kx1ev575XSLFcPhcOlvbsTxqt0uqw"
      });
   }

  ngOnInit(): void {
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    // this.initData = this.mockData();
    this.hideloader();
    this.loadPoiData();
    this.loadGeofenceData();
    this.loadLandmarkCategoryData();
  }

  loadPoiData() {
    this.showLoadingIndicator = true;
    this.poiService.getPois(this.accountOrganizationId).subscribe((data: any) => {
      this.poiInitData = data;
      // console.log("poiData=" +this.poiInitData);
      this.hideloader();
      this.allCategoryPOIData = this.poiInitData;
      this.updatedPOITableData(this.poiInitData);
    }, (error) => {
      this.poiInitData = [];
      this.hideloader();
      this.updatedPOITableData(this.poiInitData);
    });
  }

  public ngAfterViewInit() {
    let defaultLayers = this.platform.createDefaultLayers();
    this.map = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map, {
      center: { lat: 50, lng: 5 },
      zoom: 4,
      pixelRatio: window.devicePixelRatio || 1
    });
    window.addEventListener('resize', () => this.map.getViewPort().resize());

    var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));

     var ui = H.ui.UI.createDefault(this.map, defaultLayers);
  }
  

  checkboxClicked(event: any, row: any) {
    this.showMap = this.selectedpois.selected.length > 0 ? true : false;
    console.log(this.selectedpois.selected.length)
    console.log(row);
    if(event.checked){ //-- add new marker
      this.markerArray.push(row);
    }else{ //-- remove existing marker
      //It will filter out checked points only
      let arr = this.markerArray.filter(item => item.id != row.id);
      this.markerArray = arr;
      }
      this.addMarkerOnMap(); 
    }
    
    addMarkerOnMap(){
      this.map.removeObjects(this.map.getObjects());
      this.markerArray.forEach(element => {
        let marker = new H.map.Marker({ lat: element.latitude, lng: element.longitude }, { icon: this.getSVGIcon() });
        this.map.addObject(marker);
        
      });
    }
    
  updatedPOITableData(tableData: any) {
    tableData = this.getNewTagData(tableData);
    this.poidataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.poidataSource.paginator = this.paginator.toArray()[0];
      this.poidataSource.sort = this.sort.toArray()[0];
    });
  }

  loadGeofenceData() {
    this.showLoadingIndicator = true;
    this.geofenceService.getGeofenceDetails(this.accountOrganizationId).subscribe((geoListData: any) => {
      this.geoInitData = geoListData;
      this.geoInitData = this.geoInitData.filter(item => item.type == "C" || item.type == "O");
      this.hideloader();
      this.updatedGeofenceTableData(this.geoInitData);
    }, (error) => {
      this.geoInitData = [];
      this.hideloader();
      this.updatedGeofenceTableData(this.geoInitData);
    });
  }

  updatedGeofenceTableData(tableData: any) {
    tableData = this.getNewTagData(tableData);
    this.geofencedataSource = new MatTableDataSource(tableData);
    setTimeout(() => {
      this.geofencedataSource.paginator = this.paginator.toArray()[1];
      this.geofencedataSource.sort = this.sort.toArray()[1];
    }, 1000);
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

  loadLandmarkCategoryData() {
    this.showLoadingIndicator = true;
    let objData = {
      type: 'C',
      Orgid: this.accountOrganizationId
    }
    this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((parentCategoryData: any) => {
      this.categoryList = parentCategoryData.categories;
      this.getSubCategoryData();
    }, (error) => {
      this.categoryList = [];
      this.getSubCategoryData();
    });
  }

  getSubCategoryData() {
    let objData = {
      type: 'S',
      Orgid: this.accountOrganizationId
    }
    this.landmarkCategoryService.getLandmarkCategoryType(objData).subscribe((subCategoryData: any) => {
      this.subCategoryList = subCategoryData.categories;
    }, (error) => {
      this.subCategoryList = [];
    });
  }

  onCategoryChange(_event) {
    this.selectedCategoryId = _event.value;
    this.updateSelectionData();
  }

  applyFilterOnCategory(_event){
    this.selectedCategoryId = _event.value;
    if(this.selectedCategoryId == "all"){
      this.poidataSource.filter = '';
    }
    else{
    this.poidataSource.filterPredicate = function(data, filter: any): boolean {
      return data.categoryId == filter;
    }; this.poidataSource.filter = this.selectedCategoryId; 
      }
    
  }

  applyFilterOnSubCategory(_event){
    this.selectedSubCategoryId = _event.value;
    if(this.selectedSubCategoryId == "all"){
      this.poidataSource.filter = '';
    }
    else{
    this.poidataSource.filterPredicate = function(data, filter: any): boolean {
      return data.subCategoryId == filter;
    }; this.poidataSource.filter = this.selectedSubCategoryId; 
      }
  }


  onSubCategoryChange(_event) {
    this.selectedSubCategoryId = _event.value;
    //this.updateSelectionData();

  }

  updateSelectionData() {
    let poiCategoryData = [];
    if (this.selectedCategoryId) {

      poiCategoryData = this.allCategoryPOIData.filter((e) => {
        return (e.subCategoryId === this.selectedCategoryId);
      });
    }
    if (this.selectedSubCategoryId) {
      poiCategoryData = this.allCategoryPOIData.filter((e) => {
        return (e.subCategoryId === this.selectedSubCategoryId);
      });
    }
    if (this.selectedCategoryId && this.selectedSubCategoryId) {
      poiCategoryData = this.allCategoryPOIData.filter((e) => {
        return (e.parentCategoryId === this.selectedCategoryId && e.subCategoryId === this.selectedSubCategoryId);
      });
    }
    //console.log(poiCategoryData)
  }
  // mockData() {
  //   this.data = [
  //     {
  //       name: "Global List",
  //       category: "Dealers1",
  //       subcategory: "Sub-dealer1",
  //       address: "American city, Pratt, North"
  //     },
  //     {
  //       name: "Global List",
  //       category: "Dealers2",
  //       subcategory: "Sub-dealer2",
  //       address: "American city, Pratt, North"
  //     },
  //     {
  //       name: "Global List",
  //       category: "Dealers3",
  //       subcategory: "Sub-dealer3",
  //       address: "American city, Pratt, North"
  //     }
  //   ]
  //   return this.data;
  //   console.log(this.data);

  // }

  createEditView() {
    this.tabVisibility.emit(false);
    this.createEditViewPoiFlag = true;
    this.actionType = 'create';
  }

  onGeofenceSelection() {
    this.tabVisibility.emit(false);
    this.createEditViewGeofenceFlag = true;
    this.actionType = 'create';
  }

  editViewPoi(rowData: any, type: any) {
    this.tabVisibility.emit(false);
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditViewPoiFlag = true;
  }

  editViewGeofence(rowData: any, type: any) {
    this.selectedElementData = rowData;
    this.actionType = type;
    this.tabVisibility.emit(false);
    this.createEditViewGeofenceFlag = true;
  }

  successMsgBlink(msg: any) {
    this.titleVisible = true;
    this.poiCreatedMsg = msg;
    setTimeout(() => {
      this.titleVisible = false;
    }, 5000);
  }

  checkCreationForPoi(item: any) {
    // this.createEditViewPoiFlag = !this.createEditViewPoiFlag;
    this.tabVisibility.emit(true);
    this.createEditViewPoiFlag = item.stepFlag;
    if (item.successMsg && item.successMsg != '') {
      this.successMsgBlink(item.successMsg);
    }
    if (item.tableData) {
      this.poiInitData = item.tableData;
    }
    //this.loadPoiData();
    this.allCategoryPOIData = this.poiInitData;
    this.updatedPOITableData(this.poiInitData);
    this.updatedGeofenceTableData(this.geoInitData);
  }

  checkCreationForGeofence(item: any) {
    this.tabVisibility.emit(true);
    this.createEditViewGeofenceFlag = item.stepFlag;
    if(item.successMsg && item.successMsg != '') {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.geoInitData = item.tableData;
    }
    //this.loadGeofenceData();
    this.allCategoryPOIData = this.poiInitData;
    this.updatedPOITableData(this.poiInitData);
    this.updatedGeofenceTableData(this.geoInitData);
    this.selectedgeofences.clear();
  }

  onClose() {

  }

  deletePoi(rowData: any) {
    let poiId = {
      id: [rowData.id]
    };
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        this.poiService.deletePoi(poiId).subscribe((data) => {
          this.openSnackBar('Item delete', 'dismiss');
          this.loadPoiData();
        })
        this.successMsgBlink(this.getDeletMsg(rowData.name));
      }
    });
  }

  deleteMultiplePoi()
  {
    let poiId = 
    { 
      id: this.selectedpois.selected.map(item=>item.id)
    }
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    let name = this.selectedpois.selected[0].name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.poiService.deletePoi(poiId).subscribe((data) => {
        this.openSnackBar('Item delete', 'dismiss');
        this.loadPoiData();
      })
        this.successMsgBlink(this.getDeletMsg(name));
      }
    });
    this.markerArray = [];
    this.showMap = false;
    // console.log(this.markerArray)
  }


  deleteGeofence(rowData: any){
    let geofenceId = rowData.id;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        let delObjData: any = {
          geofenceIds: [geofenceId],
          modifiedBy: this.accountId
        }
        this.geofenceService.deleteGeofence(delObjData).subscribe((delData: any) => {
          this.successMsgBlink(this.getDeletMsg(rowData.name)); 
          this.loadGeofenceData();
          this.loadPoiData();
          this.selectedgeofences.clear();
        });
      }
    });
  }

  bulkDeleteGeofence(){
    let geoId: any = [];
    let geofencesList: any = '';
    this.selectedgeofences.selected.forEach(item => {
      geoId.push(item.id);
      geofencesList += item.name + ', ';
    });

    if(geofencesList != ''){
      geofencesList = geofencesList.slice(0, -2);
    }

    if(geoId.length > 0){ //- bulk delete geofences
      const options = {
        title: this.translationData.lblDelete || "Delete",
        message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
        cancelText: this.translationData.lblCancel || "Cancel",
        confirmText: this.translationData.lblDelete || "Delete"
      };
      this.dialogService.DeleteModelOpen(options, geofencesList);
      this.dialogService.confirmedDel().subscribe((res) => {
        if (res) {
          let delObjData: any = {
            geofenceIds: geoId,
            modifiedBy: this.accountId
          }
          this.geofenceService.deleteGeofence(delObjData).subscribe((delData: any) => {
            this.successMsgBlink(this.getDeletMsg(geofencesList)); 
            this.loadGeofenceData();
            this.loadPoiData();
            this.selectedgeofences.clear();
          });
        }
      });
    }
    else{
      console.log("geofence id not found...");
    }
  }

  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      console.log('The snackbar action was triggered!');
    });
  }

  getDeletMsg(name: any) {
    if (this.translationData.lblGeofencewassuccessfullydeleted)
      return this.translationData.lblGeofencewassuccessfullydeleted.replace('$', name);
    else
      return ("Geofence '$' was successfully deleted").replace('$', name);
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  poiApplyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.poidataSource.filter = filterValue;
  }

  geoApplyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.geofencedataSource.filter = filterValue;
  }

  masterToggleForPOI() {
    if(this.isAllSelectedForPOI()){
      this.selectedpois.clear();
      this.showMap = false;
    }  else {
      this.poidataSource.data.forEach((row) =>
      this.selectedpois.select(row)
      );
      this.showMap = true ;
    } 
  }

  isAllSelectedForPOI() {
    const numSelected = this.selectedpois.selected.length;
    const numRows = this.poidataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForPOI(row?: any): string {
    if (row)
      return `${this.isAllSelectedForPOI() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedpois.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  masterToggleForGeo() {
    this.isAllSelectedForGeo()
      ? this.selectedgeofences.clear()
      : this.geofencedataSource.data.forEach((row) =>
        this.selectedgeofences.select(row)
      );
  }

  isAllSelectedForGeo() {
    const numSelected = this.selectedgeofences.selected.length;
    const numRows = this.geofencedataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForGeo(row?: any): string {
    if (row)
      return `${this.isAllSelectedForGeo() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedgeofences.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }
  pageSizeUpdated(_event) {
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }
  public exportAsExcelFile(): void {
    let json: any[], excelFileName: string = 'POIData';
    this.poiService.downloadPOIForExcel().subscribe((poiData) => {
      const result = poiData.map(({ organizationId, id, categoryId, subCategoryId, type, city, country, zipcode, latitude, longitude, distance, state, createdBy, createdAt, icon, ...rest }) => ({ ...rest }));
      const myworksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(result);
      const myworkbook: XLSX.WorkBook = { Sheets: { 'data': myworksheet }, SheetNames: ['data'] };
      const excelBuffer: any = XLSX.write(myworkbook, { bookType: 'xlsx', type: 'array' });
      this.saveAsExcelFile(excelBuffer, excelFileName);
    })
  }

  private saveAsExcelFile(buffer: any, fileName: string): void {
    const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
    const EXCEL_EXTENSION = '.xlsx';
    const data: Blob = new Blob([buffer], {
      type: EXCEL_TYPE
    });
    FileSaver.saveAs(data, fileName + '_exported' + EXCEL_EXTENSION);
  }

  exportGeofenceAsExcelFile() {
    this.matTableExporter.exportTable('xlsx', { fileName: 'GeofenceData', sheet: 'sheet_name' });

  }

  updateImportView(_event) {
    this.importPOIClicked = _event;
    this.importGeofenceClicked = _event;
    this.tabVisibility.emit(true);
    this.importClicked = _event;

  }

  importPOIExcel() {
    this.importClicked = true;
    this.importPOIClicked = true;
    this.showImportCSV.emit(true);
    this.tabVisibility.emit(false);

    this.processTranslationForImport();
  }

  importGeofence(){
    this.importClicked = true;
    this.importGeofenceClicked = true;
    this.showImportCSV.emit(true);
    this.tabVisibility.emit(false);
    this.generateGPXFile();
    this.processTranslationForImportGeofence();
  }

  generateGPXFile(){
    this.defaultGpx = `<?xml version="1.0" encoding="UTF-8"?>
    <gpx version="1.1">
      <metadata>
        <id>156</id>
        <organizationId>6</organizationId>,
        <categoryId>0</categoryId>,
        <subCategoryId>0</subCategoryId>,
        <geofencename>Test Geofence1</geofencename>,
        <type>O</type>,
        <address>Pune</address>,
        <city>Pune</city>,
        <country>India</country>,
        <zipcode>400501</zipcode>,
        <latitude>18.52050580488341</latitude>,
        <longitude>73.86056772285173</longitude>,
        <distance>0</distance>,
        <tripId>0</tripId>,
        <createdBy>0</createdBy>,
      </metadata>
      <trk>
        <name>RUN</name>
        <trkseg>
          <trkpt lat="18.52050580488341" lon="73.86056772285173"></trkpt>
          <trkpt lat="18.560710817234337" lon="74.30724364900217"></trkpt>
        </trkseg>
      </trk>
    </gpx>`
  }

  processTranslationForImport() {
    if (this.translationData) {
      this.importTranslationData.importTitle = this.translationData.lblImportNewPOI || 'Import New POI';
      this.importTranslationData.downloadTemplate = this.translationData.lbldownloadTemplate || 'Download a Template';
      this.importTranslationData.downloadTemplateInstruction = this.translationData.lbldownloadTemplateInstruction || 'Each line is required to have at least X column: POI Name, Latitude, Longitude and Category separated by either a column or semicolon. You can also optionally specify a description and a XXXX for each POI.';
      this.importTranslationData.selectUpdatedFile = this.translationData.lblselectUpdatedFile || 'Upload Updated Excel File';
      this.importTranslationData.browse = this.translationData.lblbrowse || 'Browse';
      this.importTranslationData.uploadButtonText = this.translationData.lbluploadPackage || 'Upload';
      this.importTranslationData.selectFile = this.translationData.lblPleaseSelectAFile || 'Please select a file';
      this.importTranslationData.totalSizeMustNotExceed = this.translationData.lblTotalSizeMustNotExceed || 'The total size must not exceed';
      this.importTranslationData.emptyFile = this.translationData.lblEmptyFile || 'Empty File';
      this.importTranslationData.importedFileDetails = this.translationData.lblImportedFileDetails || 'Imported file details';
      this.importTranslationData.new = this.translationData.lblNew || 'new';
      this.importTranslationData.fileType = this.translationData.lblPOI || 'POI';
      this.importTranslationData.fileTypeMultiple = this.translationData.lblPOI || 'POI';
      this.importTranslationData.imported = this.translationData.lblimport || 'Imported';
      this.importTranslationData.rejected = this.translationData.lblrejected || 'Rejected';
      this.importTranslationData.existError = this.translationData.lblNamealreadyexists || 'POI name already exists';
      this.importTranslationData.input1mandatoryReason = this.translationData.lblNameMandatoryReason || '$ is mandatory input';
      this.tableTitle = this.translationData.lblTableTitle || 'Rejected POI Details';
      this.tableColumnName = [this.translationData.lblOrganizationId || 'OrganizationId',
                              this.translationData.lblCategoryId || 'CategoryId',
                              this.translationData.lblSubCategoryId || 'SubCategoryId',
                              this.translationData.lblPOIName || 'POIName',
                              this.translationData.lblLatitude || 'Latitude',
                              this.translationData.lblLongitude || 'Longitude',
                              this.translationData.lblFailReason || 'Fail Reason'];
    }
  }

  processTranslationForImportGeofence() {
    this.tableColumnList = ['organizationId', 'geofenceName', 'type', 'latitude', 'longitude', 'distance', 'returnMessage'];

    if (this.translationData) {
      this.importTranslationData.importTitle = this.translationData.lblImportGeofence || 'Import Geofence';
      this.importTranslationData.downloadTemplate = this.translationData.lbldownloadTemplate || 'Download a Template';
      this.importTranslationData.downloadTemplateInstruction = this.translationData.lbldownloadTemplateInstruction || 'Please fill required details and upload updated file again.';
      this.importTranslationData.selectUpdatedFile = this.translationData.lblselectUpdatedGeofenceFile || 'Upload Updated .GPX File';
      this.importTranslationData.browse = this.translationData.lblbrowse || 'Browse';
      this.importTranslationData.uploadButtonText = this.translationData.lbluploadPackage || 'Upload';
      this.importTranslationData.selectFile = this.translationData.lblPleaseSelectAFile || 'Please select a file';
      this.importTranslationData.totalSizeMustNotExceed = this.translationData.lblTotalSizeMustNotExceed || 'The total size must not exceed';
      this.importTranslationData.emptyFile = this.translationData.lblEmptyFile || 'Empty File';
      this.importTranslationData.importedFileDetails = this.translationData.lblImportedFileDetails || 'Imported file details';
      this.importTranslationData.new = this.translationData.lblNew || 'new';
      this.importTranslationData.fileType = this.translationData.lblGeofence || 'Geofence';
      this.importTranslationData.fileTypeMultiple = this.translationData.lblGeofence || 'Geofences';
      this.importTranslationData.imported = this.translationData.lblimport || 'Imported';
      this.importTranslationData.rejected = this.translationData.lblrejected || 'Rejected';
      this.importTranslationData.existError = this.translationData.lblGeofenceNamealreadyexists || 'Geofence name already exists';
      this.importTranslationData.input1mandatoryReason = this.translationData.lblNameMandatoryReason || "$ is mandatory input";
      this.importTranslationData.valueCannotExceed = this.translationData.lblValueCannotExceed || 'Geofence name can be upto 50 characters';
      this.importTranslationData.distanceGreaterThanZero = this.translationData.lbldistanceGreaterThanZero || 'Distance should be greater than zero';
      this.importTranslationData.nodesAreRequired = this.translationData.lblnodesAreRequired || 'Nodes are required';
      this.importTranslationData.typeCanEitherBeCorO = this.translationData.lbltypeCanEitherBeCorO || 'Geofence type can either be C or O';
      this.importTranslationData.organizationIdCannotbeZero = this.translationData.lblorganizationIdCannotbeZero || 'Organization Id cannot be zero';
      this.tableTitle = this.translationData.lblGeofenceTableTitle || 'Rejected Geofence Details';
      this.tableColumnName = [this.translationData.lblOrganizationId || 'Organization Id',
                              this.translationData.lblGeofenceName|| 'Geofence Name',
                              this.translationData.lblGeofenceType|| 'Type',
                              this.translationData.lblLatitude || 'Latitude',
                              this.translationData.lblLongitude || 'Longitude',
                              this.translationData.lblDistance || 'Distance',
                              this.translationData.lblFailReason || 'Fail Reason'];
    }
  }
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
}