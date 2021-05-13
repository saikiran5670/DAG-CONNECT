import { Component, Input, OnInit,ViewChild } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { AccountService } from '../../services/account.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { UserDetailTableComponent } from '../../admin/user-management/new-user-step/user-detail-table/user-detail-table.component';
import { MatSort } from '@angular/material/sort';
import { VehicleService } from '../../services/vehicle.service';
import { PackageService } from 'src/app/services/package.service';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.less']
})

export class AlertsComponent implements OnInit {
  displayedColumns: string[] = ['alertIcon','name','category','alertType','threshold','vehicleGroup','status','action'];
  grpTitleVisible : boolean = false;
  displayMessage: any;
  createViewEditStatus: boolean = false;
  showLoadingIndicator: any = false;
  actionType: any = '';
  selectedRowData: any= [];
  titleText: string;
  translationData: any= [];
  localStLanguage: any;
  dataSource: any; 
  initData: any = [];
  rowsData: any;
  createStatus: boolean;
  editFlag: boolean = false;
  duplicateFlag: boolean = false;
  accountOrganizationId: any;
  accountId: any;
  EmployeeDataService : any= [];  
  packageCreatedMsg : any = '';
  titleVisible : boolean = false;
  alertCategoryList: any= [];
  alertTypeList: any= [];
  vehicleGroupList: any= [];
  vehicleList: any= [];
  alertCriticalityList: any= [];
  alertStatusList: any= [];

  stringifiedData: any;  
  parsedJson: any;  

  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  dialogVeh: MatDialogRef<UserDetailTableComponent>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));

  constructor(
    private translationService: TranslationService,
    private accountService: AccountService,
    private packageService: PackageService, 
    private dialog: MatDialog,
    private vehicleService: VehicleService,
    private alertService: AlertService ) { }
 
  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 17 //-- for alerts
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.loadFiltersData();
      this.loadAlertsData();
    });  
  
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadFiltersData(){
    this.alertService.getAlertFilterData(this.accountId, this.accountOrganizationId).subscribe((data) => {
      let filterData = data["enumTranslation"];
      filterData.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });
      this.alertCategoryList= filterData.filter(item => item.type == 'C');
      this.alertTypeList= filterData.filter(item => item.type == 'T');
      this.alertCriticalityList= filterData.filter(item => item.type == 'U');
      this.vehicleList= data["vehicleGroup"];
      this.alertStatusList=[{
       id: 1,
       status:"Active",
       value:'Active'
      },{
        id: 2,
        status:"Suspended",
        value:'Suspended'
       }
    ]
    }, (error) => {

    })
  }

  onClickNewAlert(){
    //this.titleText = this.translationData.lblAddNewGroup || "Add New Alert";
    this.actionType = 'create';
    this.createViewEditStatus = true;
  }
  onClose(){
    this.grpTitleVisible = false;
  }

  onBackToPage(objData){
    this.createViewEditStatus = objData.actionFlag;
    if(objData.successMsg && objData.successMsg != ''){
      this.successMsgBlink(objData.successMsg);
    }
    this.loadAlertsData();
  }
  
  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  hideloader() {
    // Setting display of spinner
      this.showLoadingIndicator=false;
  }

  makeRoleAccountGrpList(initdata: any){
    let accountId =  localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    initdata = initdata.filter(item => item.id != accountId);
    initdata.forEach((element, index) => {
      let roleTxt: any = '';
      let accGrpTxt: any = '';
      element.roles.forEach(resp => {
        roleTxt += resp.name + ', ';
      });
      element.accountGroups.forEach(resp => {
        accGrpTxt += resp.name + ', ';
      });

      if(roleTxt != ''){
        roleTxt = roleTxt.slice(0, -2);
      }
      if(accGrpTxt != ''){
        accGrpTxt = accGrpTxt.slice(0, -2);
      }

      initdata[index].roleList = roleTxt; 
      initdata[index].accountGroupList = accGrpTxt;
    });
    
    return initdata;
  }

  loadAlertsData(){
    this.showLoadingIndicator = true;
    // this.alertService.getAlertData(this.accountId, this.accountOrganizationId).subscribe((data) => {
    //   data.forEach(element => {
    //     let filterData = data;       
    //   });      
    // }, (error) => {
    // })
    
    let obj: any = {
      accountId: 0,
      organizationId: this.accountOrganizationId,
      accountGroupId: 0,
      vehicleGroupGroupId: 0,
      roleId: 0,
      name: ""
    }
     this.stringifiedData = JSON.stringify(this.myData);  
     this.parsedJson = JSON.parse(this.stringifiedData); 
    
   this.hideloader();
   this.initData = this.parsedJson;  
   this.updateDatasource(this.initData);
 }
 getFilteredValues(dataSource){
  this.dataSource = dataSource;
  this.dataSource.paginator = this.paginator;
  this.dataSource.sort = this.sort;
}
  updateDatasource(data){
    if(data && data.length > 0){
      this.initData = this.getNewTagData(data); 
    } 
    this.dataSource = new MatTableDataSource(this.initData);
    // this.dataSource.filterPredicate = function(data: any, filter: string): boolean {
    //   return (
    //     data.name.toString().toLowerCase().includes(filter) ||
    //     data.poiCount.toString().toLowerCase().includes(filter) ||
    //     data.geofenceCount.toString().toLowerCase().includes(filter)
    //   );
    // };
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  getNewTagData(data: any){
    let currentDate = new Date().getTime();
    data.forEach(row => {
      if(row.createdAt){
        let createdDate = parseInt(row.createdAt); 
        let nextDate = createdDate + 86400000;
        if(currentDate >= createdDate && currentDate < nextDate){
          row.newTag = true;
        }
        else{
          row.newTag = false;
        }
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

  deleteAlertData(item: any) {
    const options = {
      title: this.translationData.lblDeleteAccount || "Delete Account",
      message: this.translationData.lblAreyousureyouwanttodeleteuseraccount || "Are you sure you want to delete '$' account?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    this.OpenDialog(options, 'delete', item);
  }
    
  OpenDialog(options: any, flag: any, item: any) {
   
  }
  editViewAlertData(element: any, type: any) {
   
  }

  editAlertData(row: any, action : string) {
    this.duplicateFlag = false;
    if(action == 'duplicate'){
      this.duplicateFlag = true;
    }
    this.titleText = this.duplicateFlag ? this.translationData.lblCreateNewUserRole || "Create New Alert" : this.translationData.lblEditUserRoleDetails || "Edit Alert Details";
    this.rowsData = [];
    this.rowsData.push(row);
    this.editFlag = true;
    this.createStatus = false;    
  }

   successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

   changePackageStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert || "Alert",
      message: this.translationData.lblYouwanttoDetails || "You want to # '$' Details?",   
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.status == 'Active') ? this.translationData.lblDeactivate || " Suspended" : this.translationData.lblActivate || " Activate",
      status: rowData.status == 'Active' ? 'Inactive' : 'Active' ,
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){ 
        // TODO: change status with latest grid data
        let updatePackageParams = {
          "packageId": rowData.id,
          "status":rowData.status === "Active" ? "I" : "A"
        }
        this.packageService.updateChangedStatus(updatePackageParams).subscribe((data) => {
          this.loadAlertsData();
          let successMsg = "Updated Successfully!";
          this.successMsgBlink(successMsg);
        })
      }else {
        this.loadAlertsData();
      }
    });
  }

  onVehicleGroupClick(data: any) {   
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'Group Name', this.translationData.lblRegistrationNumber || 'Status'];
    const tableTitle =`${data.vehicleGroup} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    let objData = {
      // groupId: data.groupId,
      // groupType: data.groupType,
      // functionEnum: data.functionEnum,
      // organizationId: data.organizationId
      groupId: 97,
      groupType: 'G',
      functionEnum: 'A',
      organizationId: 36
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
    this.dialogVeh = this.dialog.open(UserDetailTableComponent, dialogConfig);
  }
  myData =[ 
    {
      id: 1,
      name:"Test Alert 01",
      category:"Fuel and Driver Performance",
      alertType:"Exiting Zone",
      threshold:"-",
      vehicleGroup:"Test Group 1",
      status:"Active",
      alertIcon:"#DC143C",
      createdAt: new Date().getTime()
    },
    {
      id: 2,
      name:"Test Alert 02",
      category:"Logistics Alerts",
      alertType:"Fuel Loss During Trip",
      threshold:"25%",
      vehicleGroup:"Test Group 2",
      status:"Suspended",
      alertIcon:"#FFD700",
      createdAt: new Date().getTime()
    },
    {
      id: 3,
      name:"Test Alert 03",
      category:"Repair and Maintenance",
      alertType:"Excessive Average Speed",
      threshold:"63.1347mph",
      vehicleGroup:"Test Group 3",
      status:"Active",
      alertIcon:"#FF8C00",
      createdAt: new Date().getTime()
    },
    {
      id: 4,
      name:"Test Alert 04",
      category:"Fuel and Driver Performance",
      alertType:"Exiting Zone",
      threshold:"-",
      vehicleGroup:"Test Group 4",
      status:"Active",
      alertIcon:"#DC143C",
      createdAt: new Date().getTime()
    },
    {
      id: 5,
      name:"Test Alert 05",
      category:"Logistics Alerts",
      alertType:"Fuel Loss During Trip",
      threshold:"25%",
      vehicleGroup:"Test Group 5",
      status:"Suspended",
      alertIcon:"#FFD700",
      createdAt: new Date().getTime()
    },
    {
      id: 6,
      name:"Test Alert 06",
      category:"Repair and Maintenance",
      alertType:"Excessive Average Speed",
      threshold:"63.1347mph",
      vehicleGroup:"Test Group 6",
      status:"Active",
      alertIcon:"#FF8C00",
      createdAt: new Date().getTime()
    },
    {
      id: 7,
      name:"Test Alert 07",
      category:"Fuel and Driver Performance",
      alertType:"Exiting Zone",
      threshold:"-",
      vehicleGroup:"Test Group 7",
      status:"Active",
      alertIcon:"#DC143C",
      createdAt: new Date().getTime()
    },
    {
      id: 8,
      name:"Test Alert 08",
      category:"Logistics Alerts",
      alertType:"Fuel Loss During Trip",
      threshold:"25%",
      vehicleGroup:"Test Group 8",
      status:"Suspended",
      alertIcon:"#FFD700",
      createdAt: new Date().getTime()
    },
    {
      id: 9,
      name:"Test Alert 09",
      category:"Repair and Maintenance",
      alertType:"Excessive Average Speed",
      threshold:"63.1347mph",
      vehicleGroup:"veh002 grp",
      status:"Active",
      alertIcon:"#FF8C00",
      createdAt: new Date().getTime()
    }
];  

}
