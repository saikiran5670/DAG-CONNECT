import { Component, Input, OnInit,ViewChild } from '@angular/core';
import { TranslationService } from 'src/app/services/translation.service';
import { AccountService } from '../../services/account.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ActiveInactiveDailogComponent } from '../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { PackageService } from 'src/app/services/package.service';

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
  accountOrganizationId: any;
  EmployeeDataService : any= [];  
  packageCreatedMsg : any = '';
  titleVisible : boolean = false;

  stringifiedData: any;  
  parsedJson: any;  

  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));

  constructor(
    private translationService: TranslationService,
    private alertService: AccountService,
    private packageService: PackageService, 
    private dialog: MatDialog,) { }
 
  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    let translationObj = {
      id: 0,
      code: this.localStLanguage ? this.localStLanguage.code : "EN-GB",
      type: "Menu",
      name: "",
      value: "",
      filter: "",
      menuId: 18 //-- for landmark
    }
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);
      this.loadAlertsData();
    });
    
  }
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
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
    if(objData.gridData){
      this.initData = objData.gridData;
    }
    this.updateDatasource(this.initData);
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

  deleteUser(item: any) {
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
  editViewUser(element: any, type: any) {
   
   }

   successMsgBlink(msg: any){
    this.titleVisible = true;
    this.packageCreatedMsg = msg;
    setTimeout(() => {  
      this.titleVisible = false;
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
  }
   myData =[ 
    {
      id: 1,
      name:"Test Alert 01",
      category:"Fuel & Driver Performance",
      alertType:"Exiting Zone",
      threshold:"-",
      vehicleGroup:"Test Group 1",
      status:"Active",
      alertIcon:"https://icon2.cleanpng.com/20180701/ffq/kisspng-computer-icons-royalty-free-clip-art-red-alert-5b38fb8f31e246.8917341215304610712043.jpg",
      createdAt: new Date().getTime()
    },
    {
      id: 2,
      name:"Test Alert 02",
      category:"Logistics Alert",
      alertType:"Fuel Loss During Trip",
      threshold:"25%",
      vehicleGroup:"Test Group 2",
      status:"Suspended",
      alertIcon:"https://w7.pngwing.com/pngs/46/279/png-transparent-caution-logo-warning-sign-symbol-yellow-triangle-s-sign-signage-color-triangle.png",
      createdAt: new Date().getTime()
    },
    {
      id: 3,
      name:"Test Alert 03",
      category:"Repair And Maintenance",
      alertType:"Excessive Average Speed",
      threshold:"63.1347mph",
      vehicleGroup:"Test Group 3",
      status:"Active",
      alertIcon:"https://www.vhv.rs/dpng/d/467-4679073_free-png-warning-vectors-and-icons-transparent-background.png",
      createdAt: new Date().getTime()
    },
    {
      id: 4,
      name:"Test Alert 04",
      category:"Fuel & Driver Performance",
      alertType:"Exiting Zone",
      threshold:"-",
      vehicleGroup:"Test Group 4",
      status:"Active",
      alertIcon:"https://icon2.cleanpng.com/20180701/ffq/kisspng-computer-icons-royalty-free-clip-art-red-alert-5b38fb8f31e246.8917341215304610712043.jpg",
      createdAt: new Date().getTime()
    },
    {
      id: 5,
      name:"Test Alert 05",
      category:"Logistics Alert",
      alertType:"Fuel Loss During Trip",
      threshold:"25%",
      vehicleGroup:"Test Group 5",
      status:"Suspended",
      alertIcon:"https://w7.pngwing.com/pngs/46/279/png-transparent-caution-logo-warning-sign-symbol-yellow-triangle-s-sign-signage-color-triangle.png",
      createdAt: new Date().getTime()
    },
    {
      id: 6,
      name:"Test Alert 06",
      category:"Repair And Maintenance",
      alertType:"Excessive Average Speed",
      threshold:"63.1347mph",
      vehicleGroup:"Test Group 6",
      status:"Active",
      alertIcon:"https://www.vhv.rs/dpng/d/467-4679073_free-png-warning-vectors-and-icons-transparent-background.png",
      createdAt: new Date().getTime()
    },
    {
      id: 7,
      name:"Test Alert 07",
      category:"Fuel & Driver Performance",
      alertType:"Exiting Zone",
      threshold:"-",
      vehicleGroup:"Test Group 7",
      status:"Active",
      alertIcon:"https://icon2.cleanpng.com/20180701/ffq/kisspng-computer-icons-royalty-free-clip-art-red-alert-5b38fb8f31e246.8917341215304610712043.jpg",
      createdAt: new Date().getTime()
    },
    {
      id: 8,
      name:"Test Alert 08",
      category:"Logistics Alert",
      alertType:"Fuel Loss During Trip",
      threshold:"25%",
      vehicleGroup:"Test Group 8",
      status:"Suspended",
      alertIcon:"https://w7.pngwing.com/pngs/46/279/png-transparent-caution-logo-warning-sign-symbol-yellow-triangle-s-sign-signage-color-triangle.png",
      createdAt: new Date().getTime()
    },
    {
      id: 9,
      name:"Test Alert 09",
      category:"Repair And Maintenance",
      alertType:"Excessive Average Speed",
      threshold:"63.1347mph",
      vehicleGroup:"Test Group 9",
      status:"Active",
      alertIcon:"https://www.vhv.rs/dpng/d/467-4679073_free-png-warning-vectors-and-icons-transparent-background.png",
      createdAt: new Date().getTime()
    }
];  

}
