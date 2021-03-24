import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogRef, MatDialogConfig } from '@angular/material/dialog';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { VehicleService } from '../../services/vehicle.service';
import { UserDetailTableComponent } from '../../admin/user-management/new-user-step/user-detail-table/user-detail-table.component';

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
  displayedColumns: any = ['groupName', 'vehicleCount', 'action'];
  showLoadingIndicator: boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dialogRef: MatDialogRef<UserDetailTableComponent>;

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
      lblNoRecordFound: "No Record Found"
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
      menuId: 24 //-- for veh grp mgnt
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
    });
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

  }

  deleteVehicleGroup(rowData: any){

  }

}
