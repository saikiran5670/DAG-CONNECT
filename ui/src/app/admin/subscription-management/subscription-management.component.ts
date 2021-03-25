import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogService } from '../../shared/confirm-dialog/confirm-dialog.service';
import { TranslationService } from '../../services/translation.service';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DataSource, SelectionModel } from '@angular/cdk/collections';
import { SubscriptionService } from 'src/app/services/subscription.service';
import { CompileIdentifierMetadata } from '@angular/compiler';
import { UserDetailTableComponent } from '../../admin/user-management/new-user-step/user-detail-table/user-detail-table.component';

@Component({
  selector: 'app-subscription-management',
  templateUrl: './subscription-management.component.html',
  styleUrls: ['./subscription-management.component.less']
})

export class SubscriptionManagementComponent implements OnInit {

  options=['Select Status','All','Active','Expired'];
  subscriptionRestData: any = [];
  displayedColumns = ['subscriptionId','packageCode', 'name', 'orgName', 'type', 'count', 'subscriptionStartDate', 'subscriptionEndDate', 'isActive', 'action'];
  vehicleDiaplayColumns = ['name', 'vin', 'licensePlateNumber'];
  openVehicleFlag: boolean = false;
  selectedElementData: any;
  subscriptionCreatedMsg : any = '';
  titleVisible : boolean = false;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  initData: any = [];
  vehicleData: any = [];
  accountOrganizationId: any = 0;
  localStLanguage: any;
  dataSource: any; 
  orgID: any;
  changedOrgId: any;
  translationData: any;
  createEditViewSubscriptionFlag: boolean = false;
  actionType: any;
  // dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  dialogRef: MatDialogRef<UserDetailTableComponent>;
  selectionForSubscription = new SelectionModel(true, []);
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  userType: any = localStorage.getItem("userType");
  organizationList: any = [];
  TypeList: any = [
    {
      name: 'Organization',
      value: 'O'
    },
    {
      name: 'VIN',
      value: 'V'
    }
  ];
  StatusList: any = [
    {
      name: 'Active',
      value: '1'
    },
    {
      name: 'Expired',
      value: '2'
    }
  ];

  constructor(
    private translationService: TranslationService,
    private dialogService: ConfirmDialogService,
    private subscriptionService: SubscriptionService,
    public dialog: MatDialog) { 
    this.defaultTranslation();
  }

  defaultTranslation(){
    this.translationData = {
      lblSearch: "Search",
      lblSubscriptionManagement: "Subscription Management",
      lblSubscriptionRelationshipDetails: "Subscription Relationship Details",
      lblNoRecordFound: "No Record Found",
    }
  }

  setDate(date : any){
    if (date === 0) {
      return 0;
    } else {
      var newdate = new Date(date);
      var day = newdate.getDate();
      var month = newdate.getMonth();
      var year = newdate.getFullYear();
      return (`${day}/${month + 1}/${year}`);
    }
  }

  ngOnInit() { 
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.orgID = parseInt(localStorage.getItem('accountOrganizationId'));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 3 //-- for user mgnt
    }
    this.translationService.getMenuTranslations(translationObj).subscribe( (data) => {
        this.processTranslation(data);
        this.loadSubscriptionData();
      });
      this.loadSubscriptionData();
  }

  loadSubscriptionData(){
    this.subscriptionService.getSubscriptions(this.orgID).subscribe((data : any) => {
      this.initData = data["subscriptionList"];
      this.updatedTableData(this.initData);
    });

    this.subscriptionService.getOrganizations().subscribe((data: any) => {
      if(data)
      {
        this.organizationList = data["organizationList"];
      }
    });
  }

  updatedTableData(tableData : any) {
    this.dataSource = new MatTableDataSource(this.initData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  onVehicleClick(rowData: any){
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${rowData.subscriptionId} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    this.subscriptionService.getVehicleBySubscriptionId(rowData.subscriptionId).subscribe((vehList: any) => {
      this.vehicleData = vehList["vehicles"]
      this.callToCommonTable(this.vehicleData, colsList, colsName, tableTitle);
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

  processTranslation(transData: any){
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter= filterValue;
  }

  masterToggleForSubscription() {
    this.isAllSelectedForSubscription()
      ? this.selectionForSubscription.clear()
      : this.dataSource.data.forEach((row: any) =>
        this.selectionForSubscription.select(row)
      );
  }

  isAllSelectedForSubscription() {
    const numSelected = this.selectionForSubscription.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForSubscription(row?: any): string {
    if (row)
      return `${this.isAllSelectedForSubscription() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectionForSubscription.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  filterStatus(selectedValue) {
    selectedValue = selectedValue.trim(); 
    selectedValue = selectedValue.toLowerCase(); 
    this.dataSource.filter= selectedValue != 'all' ? selectedValue : ''
  }

  applyFilterOnOrganization(filterValue: string){
      this.subscriptionService.getSubscriptions(filterValue).subscribe((data : any) => {
      this.initData = data["subscriptionList"];
      this.updatedTableData(this.initData);
      this.changedOrgId = filterValue;
    });
   }
  
   applyFilterOnStatus(data: any, status: any){
    this.subscriptionService.getSubscriptionByStatus(this.changedOrgId ? this.changedOrgId : this.orgID, status).subscribe((data : any) => {
    this.initData = data["subscriptionList"];
    this.updatedTableData(this.initData);
  });
 }

 applyFilterOnType(data: any, type: any){
  this.subscriptionService.getSubscriptionByType(this.changedOrgId ? this.changedOrgId : this.orgID, type).subscribe((data : any) => {
  this.initData = data["subscriptionList"];
  this.updatedTableData(this.initData);
});
}
}


