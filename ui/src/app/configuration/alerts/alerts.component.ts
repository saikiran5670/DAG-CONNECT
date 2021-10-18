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
import { ReportMapService } from '../../report/report-map.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.less']
})

export class AlertsComponent implements OnInit {
  displayedColumns: string[] = ['highUrgencyLevel','name','category','type','thresholdValue','vehicleGroupName','state','action'];
  grpTitleVisible : boolean = false;
  errorMsgVisible: boolean = false;
  displayMessage: any;
  createViewEditStatus: boolean = false;
  showLoadingIndicator: any = false;
  actionType: any = '';
  selectedRowData: any= [];
  titleText: string;
  translationData: any= {};
  localStLanguage: any;
  dataSource: any; 
  initData: any = [];
  originalAlertData: any= [];
  rowsData: any;
  //createStatus: boolean;
  editFlag: boolean = false;
  duplicateFlag: boolean = false;
  accountOrganizationId: any;
  accountId: any;
  accountRoleId: any;
  EmployeeDataService : any= [];  
  packageCreatedMsg : any = '';
  titleVisible : boolean = false;
  alertCategoryList: any= [];
  alertTypeList: any= [];
  vehicleGroupList: any= [];
  vehicleList: any= [];
  alertCriticalityList: any= [];
  alertStatusList: any= [];
  alertTypeListBasedOnPrivilege: any= [];
  vehicleGroupListBasedOnPrivilege: any= [];
  vehicleListBasedOnPrivilege: any= [];
  stringifiedData: any;  
  parsedJson: any;  
  filterValues = {};  
  alertCategoryTypeMasterData: any= [];
  alertCategoryTypeFilterData: any= [];
  associatedVehicleData: any= [];
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
    private dialogService: ConfirmDialogService,
    private reportMapService: ReportMapService ) { }
  
    ngOnInit() {
      this.localStLanguage = JSON.parse(localStorage.getItem("language"));
      //this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
      if(localStorage.getItem('contextOrgId')){
        this.accountOrganizationId = localStorage.getItem('contextOrgId') ? parseInt(localStorage.getItem('contextOrgId')) : 0;
      }
      else{
        this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
      } 
      
      this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
      this.accountRoleId = localStorage.getItem('accountRoleId') ? parseInt(localStorage.getItem('accountRoleId')) : 0;
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
      // console.log("alertTypeList=" +this.alertTypeList);
      // console.log("filterData=" +filterData);
      this.alertCriticalityList= filterData.filter(item => item.type == 'U');
      this.vehicleList= data["vehicleGroup"].filter(item => item.vehicleName != '');
      this.vehicleList = this.removeDuplicates(this.vehicleList, "vehicleName");

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
    this.loadDataBasedOnPrivileges();
        
    }, (error) => {

    })
  }

  loadDataBasedOnPrivileges(){
    this.alertService.getAlertFilterDataBasedOnPrivileges(this.accountId, this.accountRoleId).subscribe((data) => {
      this.alertCategoryTypeMasterData = data["enumTranslation"];
      this.alertCategoryTypeFilterData = data["alertCategoryFilterRequest"];
      this.associatedVehicleData = data["associatedVehicleRequest"];

      let alertTypeMap = new Map();
      this.alertCategoryTypeFilterData.forEach(element => {
        alertTypeMap.set(element.featureKey, element.featureKey);
      });

      if(alertTypeMap != undefined){
        this.alertCategoryTypeMasterData.forEach(element => {
          if(alertTypeMap.get(element.key)){
            element["value"]= this.translationData[element["key"]];
            this.alertTypeListBasedOnPrivilege.push(element);
          }
        });
      }

      this.associatedVehicleData.forEach(element => {
        if(element.vehicleGroupDetails != ""){
          let vehicleGroupDetails= element.vehicleGroupDetails.split(",");
          vehicleGroupDetails.forEach(item => {
            let itemSplit = item.split("~");
            // if(itemSplit[2] != 'S') {
            let vehicleGroupObj= {
              "vehicleGroupId" : itemSplit[0],
              "vehicleGroupName" : itemSplit[1]
            }
            this.vehicleGroupListBasedOnPrivilege.push(vehicleGroupObj);
          //  } 
          });
        }

        this.vehicleListBasedOnPrivilege.push({"vehicleId" : element.vehicleId});
      });
      
      this.vehicleGroupListBasedOnPrivilege = this.removeDuplicates(this.vehicleGroupListBasedOnPrivilege, "vehicleGroupId");

      this.loadAlertsData();   
    })
  }


  removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject  = {}; 
    for(var i in originalArray) {
       lookupObject[originalArray[i][prop]] = originalArray[i];
    } 
    for(i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
     return newArray;
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
    this.alertService.getAlertData(this.accountId, this.accountOrganizationId).subscribe((data) => {
      let initDataBasedOnPrivilege = data;
      // this.initData =data; 
      initDataBasedOnPrivilege.forEach(item => {
        let hasPrivilege = (this.alertTypeListBasedOnPrivilege.filter(element => element.enum == item.type).length > 0 && (this.vehicleGroupListBasedOnPrivilege.filter(element => element.vehicleGroupId == item.vehicleGroupId).length > 0 || this.vehicleListBasedOnPrivilege.filter(element => element.vehicleId == item.vehicleGroupId).length > 0));    
        if(hasPrivilege){
          this.initData.push(item);
        }
      })

      this.originalAlertData= JSON.parse(JSON.stringify(data)); //Clone array of objects
      this.initData.forEach(item => {      
       
      let catVal = this.alertCategoryList.filter(cat => cat.enum == item.category);
      catVal.forEach(obj => { 
        item["category"]=obj.value;
      });     
      let typeVal = this.alertTypeList.filter(type => type.enum == item.type);
      typeVal.forEach(obj => { 
        item["type"]=obj.value;
      });  
      
      let alertUrgency=({
      alertFilterRefs: [],
      alertId: 42,
      createdAt: 1621588594280,
      dayType: [],
      id: 38,
      modifiedAt: 0,
      periodType: "A",
      state: "A",
      thresholdValue: 25,
      unitType: "H",
      urgencyLevelType: "W",
      urgencylevelEndDate: 0,
      urgencylevelStartDate: 0
    })
      // let alertUrgency =({      
      // thresholdValue: 300,
      // urgencyLevelType: "A"      
      // })

     // item.alertUrgencyLevelRefs.push(alertUrgency);
      //item.alertUrgencyLevelRefs.push(alertUrgency);
      
      let critical  = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'C');
      let warning   = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'W');
      let advisory   = item.alertUrgencyLevelRefs.filter(lvl=> lvl.urgencyLevelType == 'A');
     
      if(critical.length > 0){
      critical.forEach(obj => { 
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});    
      if(obj.unitType !='N') {
      item.highThresholdValue = this.getConvertedThresholdValues(item.highThresholdValue, obj.unitType);
      }
      }); 
      }
      else if(warning.length > 0){
      warning.forEach(obj => { 
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});  
      if(obj.unitType !='N') {
      item.highThresholdValue = this.getConvertedThresholdValues(item.highThresholdValue, obj.unitType);
      }
    });     
      }       
      else {
      advisory.forEach(obj => { 
      item =  Object.defineProperty(item, "highUrgencyLevel", {value : obj.urgencyLevelType,
      writable : true,enumerable : true, configurable : true});
      item =  Object.defineProperty(item, "highThresholdValue", {value : obj.thresholdValue,
      writable : true,enumerable : true, configurable : true});  
      if(obj.unitType !='N') {
      item.highThresholdValue = this.getConvertedThresholdValues(item.highThresholdValue, obj.unitType);  
      }
    });   
      }  
      if(item.vehicleGroupName!=''){     
        this.vehicleGroupList.push({value:item.vehicleGroupName, key:item.vehicleGroupName});
        this.vehicleGroupList = this.vehicleGroupList.filter((test, index, array) =>
          index === array.findIndex((findTest) =>
          findTest.value === test.value
          )   
       ); 
      } else {
        item.vehicleGroupName = item.vehicleName;
      }              
     }); 
      this.updateDatasource(this.initData);  
    }, (error) => {
    })     
   this.hideloader();     
 }
 
 getConvertedThresholdValues(originalThreshold,unitType){
   let threshold;
   if(unitType == 'H' || unitType == 'T' ||unitType == 'S'){
    threshold =this.reportMapService.getConvertedTime(originalThreshold,unitType);
   }
   else if(unitType == 'K' || unitType == 'L'){
    threshold =this.reportMapService.getConvertedDistance(originalThreshold,unitType);
    }
  else if(unitType == 'A' || unitType == 'B'){
    threshold =this.reportMapService.getConvertedSpeed(originalThreshold,unitType);
   }
   return threshold;
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
      // this.dataSource.sortData = (data: String[], sort: MatSort) => {
      //   const isAsc = sort.direction === 'asc';
      //   return data.sort((a: any, b: any) => {
      //     var a1;
      //     var b1;
      //     if(sort.active && sort.active === 'thresholdValue'){
      //       a1 = a.highThresholdValue;
      //       b1 = b.highThresholdValue;
      //     } else {
      //       a1 = a[sort.active];
      //       b1 = b[sort.active];
      //     }
      //     return this.compare(a1, b1, isAsc);
      //   });
      //  }
      this.dataSource.sortData = (data : String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }
    });
  }
  compare(a: Number | String, b: Number | String, isAsc: boolean, columnName: any) {
    if(columnName == "name" || columnName =="category"){
      if(!(a instanceof Number)) a = a.toString().toUpperCase();
      if(!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1); 
  }
  

  // compare(a: any, b: any, isAsc: boolean) {
  //   if(!(a instanceof Number) && isNaN(a)) a = a.toUpperCase();
  //   if(!(b instanceof Number) && isNaN(b)) b = b.toUpperCase();
  //   return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  // }

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

  onDeleteAlert(item: any) {
    const options = {
      title: this.translationData.lblDeleteAlert,
      message: this.translationData.lblAreousureyouwanttodeleteAlert,
      cancelText: this.translationData.lblCancel,
      confirmText: this.translationData.lblDelete
    };
    let name = item.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.alertService.deleteAlert(item.id).subscribe((res) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadAlertsData();
        }, error => {
          if(error.status == 409){
            this.errorMsgBlink(this.getDeletMsg(name, true));
          }
        });
    }
   });
  }
    
  getDeletMsg(alertName: any, isError? :boolean){
    if(!isError){
      if(this.translationData.lblAlertDelete)
        return this.translationData.lblAlertDelete.replace('$', alertName);
      else
        return ("Alert '$' was successfully deleted").replace('$', alertName);
    }
    else{
      if(this.translationData.lblAlertDeleteError)
        return this.translationData.lblAlertDeleteError.replace('$', alertName);
      else
        return ("Alert '$' cannot be deleted as notification is associated with this").replace('$', alertName);
    }
  }

  onViewAlert(row: any, action: any) {
    this.createViewEditStatus= true;
    this.actionType = action;
    this.rowsData = this.originalAlertData.filter(element => element.id == row.id)[0];
  }

  onEditDuplicateAlert(row: any, action : string) {
    this.createViewEditStatus= true;
    this.actionType = 'edit';
    if(action == 'duplicate'){
      this.actionType = 'duplicate';
    }
    this.titleText = this.actionType == 'duplicate' ? this.translationData.lblCreateNewAlert  : this.translationData.lblEditAlertDetails;
    this.rowsData = this.originalAlertData.filter(element => element.id == row.id)[0];
    //this.rowsData.push(row);
    //this.editFlag = true;
    //this.createStatus = false;    
  }

   successMsgBlink(msg: any){
    this.grpTitleVisible = true;
    this.displayMessage = msg;
    setTimeout(() => {  
      this.grpTitleVisible = false;
    }, 5000);
  }

  errorMsgBlink(errorMsg: any){
    this.errorMsgVisible = true;
    this.displayMessage = errorMsg;
    setTimeout(() => {  
      this.errorMsgVisible = false;
    }, 5000);
  }

  onChangeAlertStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert,
      message: this.translationData.lblYouwanttoDetails,   
      cancelText: this.translationData.lblCancel,
      confirmText: (rowData.state == 'A') ? this.translationData.lblDeactivate : this.translationData.lblActivate,
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
    const colsName =[this.translationData.lblVehicleName, this.translationData.lblVIN, this.translationData.lblRegistrationNumber];
    const tableTitle =`${data.vehicleGroupName} - ${this.translationData.lblVehicles}`;
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
