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
  displayedColumnsGeo = ['All', 'geofenceName', 'categoryName', 'subCategoryName', 'Actions'];
  poidataSource: any;
  geofencedataSource: any;
  accountOrganizationId: any = 0;
  localStLanguage: any;
  poiInitData: any = [];
  geoInitData: any = [];
  data: any = [];
  selectedElementData: any;
  titleVisible: boolean = false;
  poiCreatedMsg: any = '';
  actionType: any;
  roleID: any;
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
  allCategoryData : any =[];

  constructor( 
    private dialogService: ConfirmDialogService,
    private poiService: POIService,
    private geofenceService: GeofenceService,
    private landmarkCategoryService: LandmarkCategoryService,
    private _snackBar: MatSnackBar
    ) {
    
   }

  ngOnInit(): void {
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
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
    this.geofenceService.getAllGeofences(this.accountOrganizationId).subscribe((data: any) => {
      this.geoInitData = data["geofenceList"];
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
      this.getCategoryDetails();
    }, (error) => {
      this.subCategoryList = [];
      this.getCategoryDetails();
    });
  }

  getCategoryDetails() {
    this.landmarkCategoryService.getLandmarkCategoryDetails().subscribe((categoryData: any) => {
      this.hideloader();
      //let data = this.createImageData(categoryData.categories);
      this.allCategoryData = categoryData.categories;
    }, (error) => {
      this.hideloader();
      this.initData = [];
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
    this.geofenceService.getGeofenceDetails(this.accountOrganizationId, rowData.geofenceId).subscribe((geoData: any) => {
      this.selectedElementData = geoData[0];
      this.actionType = type;
      this.tabVisibility.emit(false);
      this.createEditViewGeofenceFlag = true;
    }, (error) => {
      console.log('Not valid geofence data...')
    });
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
  }


  deleteGeofence(rowData: any){
    let GeofenceId = rowData.geofenceId;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.geofenceName);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        let delObjData: any = {
          id: [GeofenceId]
        }
        this.geofenceService.deleteGeofence(delObjData).subscribe((delData: any) => {
          this.successMsgBlink(this.getDeletMsg(rowData.geofenceName)); 
          this.loadGeofenceData();
          this.loadPoiData();
        });
      }
    });
  }

  bulkDeleteGeofence(){
    let geoId: any = []; // this.selectedgeofences.selected.map(item => item.geofenceId);
    let geofencesList: any = '';
    this.selectedgeofences.selected.forEach(item => {
      geoId.push(item.geofenceId);
      geofencesList += geofencesList + ', ';
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
            id: geoId
          }
          this.geofenceService.deleteGeofence(delObjData).subscribe((delData: any) => {
            this.successMsgBlink(this.getDeletMsg(geofencesList)); 
            this.loadGeofenceData();
            this.loadPoiData();
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
    this.isAllSelectedForPOI()
      ? this.selectedpois.clear()
      : this.poidataSource.data.forEach((row) =>
        this.selectedpois.select(row)
      );
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
}