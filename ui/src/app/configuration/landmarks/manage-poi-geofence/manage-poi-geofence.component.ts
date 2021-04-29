import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter,Input, OnInit, Output, ViewChild } from '@angular/core';
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
  titleVisible : boolean = false;
  poiCreatedMsg : any = '';
  actionType: any;
  roleID: any;
  createEditViewPoiFlag: boolean = false;
  createEditViewGeofenceFlag: boolean = false;
  mapFlag: boolean = false;
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();
  selectedpois = new SelectionModel(true, []);
  selectedgeofences = new SelectionModel(true, []);
  @Output() tabVisibility: EventEmitter<boolean> =   new EventEmitter();
  categoryList: any = [];
  subCategoryList: any = [];
  private _snackBar: any;
  initData: any[];

  constructor( 
    private dialogService: ConfirmDialogService,
    private poiService: POIService,
    private geofenceService: GeofenceService,
    private landmarkCategoryService: LandmarkCategoryService
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

  loadPoiData(){
    this.showLoadingIndicator = true;
    this.poiService.getPois(this.accountOrganizationId).subscribe((data : any) => {
      this.poiInitData = data;
      console.log(this.poiInitData);
      this.hideloader();
      this.updatedPOITableData(this.poiInitData);
    }, (error) => {
      this.poiInitData = [];
      this.hideloader();
      this.updatedPOITableData(this.poiInitData);
    });
  }

  updatedPOITableData(tableData : any) {
    tableData = this.getNewTagData(tableData);
    this.poidataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.poidataSource.paginator = this.paginator.toArray()[0];
      this.poidataSource.sort = this.sort.toArray()[0];
    });
  }

  loadGeofenceData(){
    this.showLoadingIndicator = true;
    this.geofenceService.getAllGeofences(this.accountOrganizationId).subscribe((data : any) => {
      this.geoInitData = data["geofenceList"];
      console.log(this.geoInitData);
      this.hideloader();
      this.updatedGeofenceTableData(this.geoInitData);
    }, (error) => {
      this.geoInitData = [];
      this.hideloader();
      this.updatedGeofenceTableData(this.geoInitData);
    });
  }

  updatedGeofenceTableData(tableData : any) {
    tableData = this.getNewTagData(tableData);
    this.geofencedataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.geofencedataSource.paginator = this.paginator.toArray()[1];
      this.geofencedataSource.sort = this.sort.toArray()[1];
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


  loadLandmarkCategoryData(){
    this.showLoadingIndicator = true;
    let objData = {
      type:'C',
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

  getSubCategoryData(){
    let objData = {
      type:'S',
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

  getCategoryDetails(){
    this.landmarkCategoryService.getLandmarkCategoryDetails().subscribe((categoryData: any) => {
      this.hideloader();
      //let data = this.createImageData(categoryData.categories);
    }, (error) => {
      this.hideloader();
      this.initData = [];
    });
  }

  onCategoryChange(){

  }

  onSubCategoryChange(){

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
    console.log("createEditView() method called");
  }

  onGeofenceSelection() {
    this.tabVisibility.emit(false);
    this.createEditViewGeofenceFlag = true;
    this.actionType = 'create';
    console.log("--geofence selection--",this.createEditViewGeofenceFlag)
  }

  editViewPoi(rowData: any, type: any){
    this.actionType = type;
    this.selectedElementData = rowData;
    this.createEditViewPoiFlag = true;
  }

  successMsgBlink(msg: any){
    this.titleVisible = true;
    this.poiCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
    }, 5000);
  }

  checkCreationForPoi(item: any){
    this.createEditViewPoiFlag = !this.createEditViewPoiFlag;
    this.createEditViewPoiFlag = item.stepFlag;
    if(item.successMsg) {
      this.successMsgBlink(item.successMsg);
    }
    if(item.tableData) {
      this.poiInitData = item.tableData;
    }
    this.loadPoiData;
  }

  deletePoi(rowData: any){
    let poiId = rowData.id;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.code);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      // this.poidataSource.deletePoi(poiId).subscribe((data) => {
      //   this.openSnackBar('Item delete', 'dismiss');
      //   this.loadPoiData();
      // })
      //   this.successMsgBlink(this.getDeletMsg(rowData.code));
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
      this.geofenceService.deleteGeofence(GeofenceId).subscribe((data) => {
        this.openSnackBar('Item delete', 'dismiss');
        this.loadGeofenceData();
        this.loadPoiData();
      })
        this.successMsgBlink(this.getDeletMsg(rowData.geofenceName));
      }
    });
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

  getDeletMsg(name: any){
    if(this.translationData.lblGeofencewassuccessfullydeleted)
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
    this.poiService.downloadPOIForExcel().subscribe((poiData)=>{
        
    const myworksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(poiData);
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
    FileSaver.saveAs(data, fileName + '_exported'+ EXCEL_EXTENSION);
  }

  exportGeofenceAsExcelFile(){
    this.matTableExporter.exportTable('csv', {fileName:'GeofenceData', sheet: 'sheet_name'});

  }
}
