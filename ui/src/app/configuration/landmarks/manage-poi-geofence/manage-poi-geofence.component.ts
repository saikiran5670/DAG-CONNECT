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

@Component({
  selector: 'app-manage-poi-geofence',
  templateUrl: './manage-poi-geofence.component.html',
  styleUrls: ['./manage-poi-geofence.component.less']
})
export class ManagePoiGeofenceComponent implements OnInit {
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  showLoadingIndicator: any = false;
  @Input() translationData: any;
 // @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective;
  displayedColumnsPoi = ['All', 'Icon', 'name', 'categoryName', 'subCategoryName', 'address', 'Actions'];
  displayedColumnsGeo = ['All', 'geofenceName', 'categoryName', 'subCategoryName', 'Actions'];
  poidataSource: any;
  geofencedataSource: any;
  accountOrganizationId: any = 0;
  localStLanguage: any;
  initData: any = [];
  data: any = [];
  selectedElementData: any;
  titleVisible : boolean = false;
  poiCreatedMsg : any = '';
  actionType: any;
  roleID: any;
  createEditViewPoiFlag: boolean = false;
  createEditViewGeofenceFlag: boolean = false;
  mapFlag: boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  selectedpois = new SelectionModel(true, []);
  @Output() tabVisibility: EventEmitter<boolean> =   new EventEmitter();

  constructor( 
    private dialogService: ConfirmDialogService,
    private poiService: POIService,
    private geofenceService: GeofenceService,
    ) {
    
   }

  ngOnInit(): void {
    this.showLoadingIndicator = true;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    this.mockData();
    this.initData = this.data;
    // this.initData = this.mockData();
    console.log(this.mockData());
    this.hideloader();
    this.loadPoiData();
    this.loadGeofenceData();
  }

  loadPoiData(){
    this.showLoadingIndicator = true;
    this.poiService.getPois(this.accountOrganizationId).subscribe((data : any) => {
      this.initData = data;
      console.log(this.initData);
      this.hideloader();
      this.updatedPOITableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedPOITableData(this.initData);
    });
  }

  updatedPOITableData(tableData : any) {
    tableData = this.getNewTagData(tableData);
    this.poidataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.poidataSource.paginator = this.paginator;
      this.poidataSource.sort = this.sort;
    });
  }

  loadGeofenceData(){
    this.showLoadingIndicator = true;
    this.geofenceService.getAllGeofences(this.accountOrganizationId).subscribe((data : any) => {
      this.initData = data["geofenceList"];
      console.log(this.initData);
      this.hideloader();
      this.updatedGeofenceTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedGeofenceTableData(this.initData);
    });
  }

  updatedGeofenceTableData(tableData : any) {
    tableData = this.getNewTagData(tableData);
    this.geofencedataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.geofencedataSource.paginator = this.paginator;
      this.geofencedataSource.sort = this.sort;
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

  mockData() {
    this.data = [
      {
        name: "Global List",
        category: "Dealers1",
        subcategory: "Sub-dealer1",
        address: "American city, Pratt, North"
      },
      {
        name: "Global List",
        category: "Dealers2",
        subcategory: "Sub-dealer2",
        address: "American city, Pratt, North"
      },
      {
        name: "Global List",
        category: "Dealers3",
        subcategory: "Sub-dealer3",
        address: "American city, Pratt, North"
      }
    ]
    return this.data;
    console.log(this.data);

  }

  createEditView() {
    this.tabVisibility.emit(false);
    this.createEditViewPoiFlag = true;
    this.actionType = 'create';
    console.log("createEditView() method called");
  }

  onGeofenceSelection() {
    console.log("--geofence selection--")
    this.createEditViewGeofenceFlag = true;
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
      this.initData = item.tableData;
    }
    this.mockData;
  }

  deletePoi(rowData: any){
    let packageId = rowData.id;
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodelete || "Are you sure you want to delete '$' ?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.dialogService.DeleteModelOpen(options, rowData.code);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      // this.packageService.deletePackage(packageId).subscribe((data) => {
      //   this.openSnackBar('Item delete', 'dismiss');
      //   this.loadPackageData();
      // })
      //   this.successMsgBlink(this.getDeletMsg(rowData.code));
      }
    });
  }

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.poidataSource.filter = filterValue;
  }

  masterToggleForOrgRelationship() {
    this.isAllSelectedForOrgRelationship()
      ? this.selectedpois.clear()
      : this.poidataSource.data.forEach((row) =>
        this.selectedpois.select(row)
      );
  }

  isAllSelectedForOrgRelationship() {
    const numSelected = this.selectedpois.selected.length;
    const numRows = this.poidataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForOrgRelationship(row?: any): string {
    if (row)
      return `${this.isAllSelectedForOrgRelationship() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedpois.isSelected(row) ? 'deselect' : 'select'
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

  // exportGeofenceAsExcelFile(){
  //   this.matTableExporter.exportTable('csv', {fileName:'GeofenceData', sheet: 'sheet_name'});

  // }
}
