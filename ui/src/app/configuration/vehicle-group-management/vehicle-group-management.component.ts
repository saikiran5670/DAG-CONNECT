import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { VehicleService } from '../../services/vehicle.service';
import { UserDetailTableComponent } from '../../admin/user-management/new-user-step/user-detail-table/user-detail-table.component';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-vehicle-group-management',
  templateUrl: './vehicle-group-management.component.html',
  styleUrls: ['./vehicle-group-management.component.less']
})

export class VehicleGroupManagementComponent implements OnInit {
  grpTitleVisible: boolean = false;
  vehicleGrpCreatedMsg: any = '';
  createViewEditStatus: boolean = false;
  translationData: any;
  localStLanguage: any;
  accountOrganizationId: any;
  initData: any = [];
  dataSource: any;
  actionBtn:any; 
  displayedColumns: any = ['groupName', 'vehicleCount', 'action'];
  showLoadingIndicator: boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  actionType: any = '';
  selectedRowData: any = [];
  vehicleListData: any = [];

  constructor(private dialogService: ConfirmDialogService,
    private translationService: TranslationService,
    private vehicleService: VehicleService,
    private dialog: MatDialog) { 
      this.defaultTranslation();  
  }

  defaultTranslation() {
    this.translationData = {
      lblVehicleGroupManagement: "Vehicle Group Management",
      lblSearch: "Search",
      lblNewVehicleGroup: "New Vehicle Group",
      lblNoRecordFound: "No Record Found",
      lblOptional: "(Optional)"
    }
  }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 27 //-- for vehicle group mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.loadVehicleGroupData();
    });
  }

  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  loadVehicleGroupData(){
    this.showLoadingIndicator = true;
    this.vehicleService.getVehicleGroupList(this.accountOrganizationId).subscribe((vehGrpData: any) => {
      this.hideloader();
      this.updateDataSource(vehGrpData);
      this.initData = vehGrpData;
    }, (error) => {
      if(error.status == 404){
        this.initData = [];
        this.hideloader();
        this.updateDataSource(this.initData);
      }
    });
  }

  updateDataSource(tableData: any){
    this.initData = tableData;
    if(this.initData.length > 0){
      this.initData = this.getNewTagData(this.initData);
    }
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortdata = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any)=>{
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
    });
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName: any) {
    if(columnName == "groupName"){
      if(!(a instanceof Number)) a = a.toString().toUpperCase();
      if(!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    return ( a < b ? -1 : 1) * (isAsc ? 1: -1);
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
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

  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  onClose(){
    this.grpTitleVisible = false;
  }

  onNewVehicleGroup(){
    this.actionType = 'create';
    this.getVehicleList();
  }

  getVehicleList(rowData?: any){
    this.vehicleService.getVehicle(this.accountOrganizationId).subscribe((vehList: any) => {
      this.vehicleListData = vehList;
      if(this.actionType != 'create'){
        this.selectedRowData = rowData;
        this.vehicleService.getVehicleListById(rowData.groupId).subscribe((selectedVehList: any) => {
          this.selectedRowData.selectedVehicleList = selectedVehList;
          this.createViewEditStatus = true;
        }, (error) => {
          //console.log("error:: ", error);
          if(error.status == 404){
            this.selectedRowData.selectedVehicleList = [];
            this.createViewEditStatus = true;
          }
        });
      }
      else{
        this.createViewEditStatus = true;
      }
    });
  }

  onVehicleClick(rowData: any){
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${rowData.groupName} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    let objData = {
      groupId: rowData.groupId,
      groupType: rowData.groupType,
      functionEnum: rowData.functionEnum,
      organizationId: rowData.organizationId
    }
    this.vehicleService.getVehiclesDetails(objData).subscribe((vehList: any) => {
      this.callToCommonTable(vehList, colsList, colsName, tableTitle);
    });
  }

  callToCommonTable(tableData: any, colsList: any, colsName: any, tableTitle: any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      tableData: tableData,
      colsList: colsList,
      colsName:colsName,
      tableTitle: tableTitle
    }
    this.dialogRef = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }

  editViewVehicleGroup(rowData: any, type: any){
    this.actionType = type;
    this.getVehicleList(rowData);
  }

  deleteVehicleGroup(rowData: any){
    const options = {
      title: this.translationData.lblDelete || "Delete",
      message: this.translationData.lblAreyousureyouwanttodeleteVehicleGroup || "Are you sure you want to delete '$' Vehicle Group?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.openDeleteDialog(options, rowData);
  }

  openDeleteDialog(options: any, item: any) {
    // Model for delete
    let name = item.groupName;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
      if (res) {
        this.vehicleService.deleteVehicleGroup(item.groupId).subscribe((d: any) => {
          this.showSuccessMessage(this.getDeleteMsg(name));
          this.loadVehicleGroupData();
        });
      }
    });
  }

  showSuccessMessage(msg: any){
    this.vehicleGrpCreatedMsg = msg;
    this.grpTitleVisible = true;
    setTimeout(() => {
      this.grpTitleVisible = false;
    }, 5000);
  }

  getDeleteMsg(vehGrpName: any){
    if(this.translationData.lblVehicleGroupDelete)
      return this.translationData.lblVehicleGroupDelete.replace('$', vehGrpName);
    else
      return ("Vehicle Group '$' was successfully deleted").replace('$', vehGrpName);
  }

  onVehicleGroupCreation(item: any){
    //this.createViewEditStatus = !this.createViewEditStatus;
    this.createViewEditStatus = item.stepFlag;
    if(item.successMsg && item.successMsg != ''){
      this.showSuccessMessage(item.successMsg);
    }
    if(item.gridData){
     this.initData = item.gridData; 
    }
    this.updateDataSource(this.initData);
  }

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'VehicleGroupMgmt_Data', sheet: 'sheet_name'});
}

exportAsPdf() {
  let DATA = document.getElementById('vehicleGroupMgmtData');
    
  html2canvas( DATA , { onclone: (document) => {
    this.actionBtn = document.getElementsByClassName('action');
    for (let obj of this.actionBtn) {
      obj.style.visibility = 'hidden';  }       
  }})
  .then(canvas => {  
    
      let fileWidth = 208;
      let fileHeight = canvas.height * fileWidth / canvas.width;
      
      const FILEURI = canvas.toDataURL('image/png')
      let PDF = new jsPDF('p', 'mm', 'a4');
      let position = 0;
      PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)
      
      PDF.save('VehicleGroupMgmt_Data.pdf');
      PDF.output('dataurlnewwindow');
  });     
}

}