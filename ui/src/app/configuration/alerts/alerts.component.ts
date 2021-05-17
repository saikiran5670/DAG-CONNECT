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
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.less']
})

export class AlertsComponent implements OnInit {
  displayedColumns: string[] = ['highUrgencyLevel','name','category','type','thresholdValue','vehicleGroupName','state','action'];
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
  filterValues = {};  
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
    private alertService: AlertService,
    private dialogService: ConfirmDialogService ) { }
  
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
      });  
    }
    
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
  }

  loadFiltersData(){    
    this.showLoadingIndicator = true;   
    this.alertService.getAlertFilterData(this.accountId, this.accountOrganizationId).subscribe((data) => {
      let filterData = data["enumTranslation"];
      filterData.forEach(element => {
        element["value"]= this.translationData[element["key"]];
      });
      this.alertCategoryList= filterData.filter(item => item.type == 'C');
      this.alertTypeList= filterData.filter(item => item.type == 'T');
      this.alertCriticalityList= filterData.filter(item => item.type == 'U');
      this.vehicleList= data["vehicleGroup"].filter(item=>item.vehicleName!='');
      this.alertStatusList=[{
       id: 1,
       value:"A",
       key:'Active'
      },{
        id: 2,
        value:"I",
        key:'Suspended'
       }
    ]    
    this.loadAlertsData();        
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
    let obj: any = {
      accountId: 0,
      organizationId: this.accountOrganizationId,
      accountGroupId: 0,
      vehicleGroupGroupId: 0,
      roleId: 0,
      name: ""
    }    
    this.alertService.getAlertData(this.accountId,this.accountOrganizationId).subscribe((data) => {
     //this.initData = data["alertRequest"]; 
      data.forEach(item => {           
      let catVal = this.alertCategoryList.filter(cat => cat.enum == item.category);
      catVal.forEach(obj => { 
        item["category"]=obj.value;
      });     
      let typeVal = this.alertTypeList.filter(type => type.enum == item.type);
      typeVal.forEach(obj => { 
        item["type"]=obj.value;
      });  
     
      let critical  = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'C');
      let warning   = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'W');
      let advisory   = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'A');
     
      if(critical.length > 0){
      critical.forEach(obj => { 
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});         
      }); 
      }
      else if(warning.length > 0){
      warning.forEach(obj => { 
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});
      });     
      }       
      else {
      advisory.forEach(obj => { 
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});  
      });   
      }   

     });     
      this.initData =data; 
      this.updateDatasource(this.initData);  
    }, (error) => {
    })   
   this.hideloader();     
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
      title: this.translationData.lblDeleteAlert || "Delete Alert",
      message: this.translationData.lblAreousureyouwanttodeleteAlert || "Are you sure you want to delete '$' alert?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    let name = item.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.alertService.deleteAlert(item.id).subscribe((res) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadAlertsData();
        });
    }
   });
  }
    
  getDeletMsg(alertName: any){
    if(this.translationData.lblAlertDelete)
      return this.translationData.lblAlertDelete.replace('$', alertName);
    else
      return ("Alert '$' was successfully deleted").replace('$', alertName);
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

  onChangeAlertStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert || "Alert",
      message: this.translationData.lblYouwanttoDetails || "You want to # '$' Details?",   
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.state == 'A') ? this.translationData.lblDeactivate || " Suspend" : this.translationData.lblActivate || " Activate",
      status: rowData.state == 'A' ? 'Suspend' : 'Activate' ,
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){ 
       if(rowData.state == 'A'){
          this.alertService.suspendAlert(rowData.id).subscribe((data) => {
            this.loadAlertsData();
            // let successMsg = "Updated Successfully!";
            // this.successMsgBlink(successMsg);
          }, error => {
            this.loadAlertsData();
          });
       }
       else{
        this.alertService.activateAlert(rowData.id).subscribe((data) => {
          this.loadAlertsData();
          // let successMsg = "Updated Successfully!";
          // this.successMsgBlink(successMsg);
        }, error => {
          this.loadAlertsData();
        });

       }
        
      }else {
        this.loadAlertsData();
      }
    });
  }

  onVehicleGroupClick(data: any) {   
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${data.vehicleGroupName} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    let objData = {
      groupId: data.vehicleGroupId,
      groupType: 'G',
      functionEnum: 'A',
      organizationId: data.organizationId    
      // groupType: data.groupType,
      // functionEnum: data.functionEnum
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
}
