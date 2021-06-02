import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ReportSchedulerService } from 'src/app/services/report.scheduler.service';
import { TranslationService } from 'src/app/services/translation.service';
import { VehicleService } from 'src/app/services/vehicle.service';
import { ActiveInactiveDailogComponent } from 'src/app/shared/active-inactive-dailog/active-inactive-dailog.component';
import { CommonTableComponent } from 'src/app/shared/common-table/common-table.component';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';

@Component({
  selector: 'app-report-scheduler',
  templateUrl: './report-scheduler.component.html',
  styleUrls: ['./report-scheduler.component.less']
})

export class ReportSchedulerComponent implements OnInit {

  displayedColumns: string[] = ['reportType','vehicleGroupName','frequency','recipient','driver','lastRun','nextRun','state','action'];
  grpTitleVisible : boolean = false;
  errorMsgVisible: boolean = false;
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
  originalAlertData: any= [];
  rowsData: any;
  accountOrganizationId: any;
  accountId: any;
  titleVisible : boolean = false;
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  dialogVeh: MatDialogRef<CommonTableComponent>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  adminAccessType: any = JSON.parse(localStorage.getItem("accessType"));
  reportTypeSelection: any= 0;
  statusSelection: any= 0;
  ReportTypeList: any= [];
  StatusList: any= [];

  constructor(
    private translationService: TranslationService,
    private dialog: MatDialog,
    private vehicleService: VehicleService,
    private reportSchedulerService: ReportSchedulerService,
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
        menuId: 19 //-- for report scheduler
      }
      this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
        this.processTranslation(data);      
      }); 
      
      this.ReportTypeList= [{id : 1, name : "Fuel Report"}, {id : 2, name : "Distance Report"}, {id : 3, name : "Milage Report"}]
      this.StatusList= [{id : "A", name : "Active"}, {id : "I", name : "Suspended"}]
    }
    
  
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc, cur) => ({ ...acc, [cur.name]: cur.value }), {});
    //console.log("process translationData:: ", this.translationData)
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
  
  onClickNewReportScheduler(){
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

   loadAlertsData(){    
    let obj: any = {
      
    } 
    this. initData = [
      {
        reportType : "Fuel Report",
        vehicleGroupName : "Vehicle Group 1",
        frequency : "Monthly",
        recipient : "abc@xyz.com",
        driver : "Driver name 1",
        lastRun : "19/10/2020",
        nextRun : "19/11/2020",
        state : "A"
      },
      {
        reportType : "Distance Report",
        vehicleGroupName : "Vehicle Group 1",
        frequency : "Weekly",
        recipient : "abc@xyz.com",
        driver : "Driver name 2",
        lastRun : "19/10/2020",
        nextRun : "19/11/2020",
        state : "I"
      },
      {
        reportType : "Milage Report",
        vehicleGroupName : "Vehicle Group 2",
        frequency : "Daily",
        recipient : "pqr@xyz.com",
        driver : "Driver name 1",
        lastRun : "19/10/2020",
        nextRun : "19/11/2020",
        state : "A"
      },
      {
        reportType : "Fuel Report",
        vehicleGroupName : "Vehicle Group 2",
        frequency : "Monthly",
        recipient : "mno@xyz.com",
        driver : "Driver name 2",
        lastRun : "19/10/2020",
        nextRun : "19/11/2020",
        state : "A"
      },
      {
        reportType : "Distance Report",
        vehicleGroupName : "Vehicle Group 1",
        frequency : "Quarterly",
        recipient : "abc@abc.com",
        driver : "Driver name 1",
        lastRun : "19/10/2020",
        nextRun : "19/11/2020",
        state : "I"
      }
    ]
       
    // this.reportSchedulerService.getReportSchedulerData().subscribe((data) => {
    //   this.initData =data;  
       this.updateDatasource(this.initData);  
    // }, (error) => {
    // })   
   this.hideloader();     
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

  onDeleteReportScheduler(item: any) {
    const options = {
      title: this.translationData.lblDeleteReportScheduler || "Delete Report Scheduler",
      message: this.translationData.lblAreousureyouwanttodeleteReportScheduler || "Are you sure you want to delete '$' report scheduler?",
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblDelete || "Delete"
    };
    let name = item.name;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      // this.reportSchedulerService.deleteReportScheduler(item.id).subscribe((res) => {
      //     this.successMsgBlink(this.getDeletMsg(name));
      //     this.loadAlertsData();
      //   }, error => {
      
      //   });
    }
   });
  }
    
  getDeletMsg(reportSchedulerName: any){
      if(this.translationData.lblReportSchedulerDelete)
        return this.translationData.lblReportSchedulerDelete.replace('$', reportSchedulerName);
      else
        return ("Report scheduler '$' was successfully deleted").replace('$', reportSchedulerName);
  }

  onViewReportScheduler(row: any, action: any) {
    this.createViewEditStatus= true;
    this.actionType = action;
    this.rowsData.push(row);
  }

  onEditReportScheduler(row: any, action : string) {
    this.createViewEditStatus= true;
    this.actionType = 'edit';
    this.titleText = this.translationData.lblEditAlertDetails || "Edit Alert Details";
    this.rowsData.push(row);
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

  onChangeReportSchedulerStatus(rowData: any){
    const options = {
      title: this.translationData.lblReportScheduler || "Report Scheduler",
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
      //  if(rowData.state == 'A'){
      //     this.reportSchedulerService.suspendAlert(rowData.id).subscribe((data) => {
      //       this.loadAlertsData();
            
      //     }, error => {
      //       this.loadAlertsData();
      //     });
      //  }
      //  else{
      //   this.alertService.activateAlert(rowData.id).subscribe((data) => {
      //     this.loadAlertsData();
          
      //   }, error => {
      //     this.loadAlertsData();
      //   });

      //  }
        
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
    this.dialogVeh = this.dialog.open(CommonTableComponent, dialogConfig);
  }  
  
  onReportTypeChange(_event: any){
    this.reportTypeSelection = parseInt(_event.value);
    if(this.reportTypeSelection == 0 && this.statusSelection == 0){
      this.updateDatasource(this.initData); //-- load all data
    }
    else if(this.reportTypeSelection == 0 && this.statusSelection != 0){
      let filterData = this.initData.filter(item => item.state == this.statusSelection);
      if(filterData){
        this.updateDatasource(filterData);
      }
      else{
        this.updateDatasource([]);
      }
    }
    else{
      let selectedReportType = this.reportTypeSelection;
      let selectedStatus = this.statusSelection;
      let reportSchedulerData = this.initData.filter(item => item.reportTypeId === selectedReportType);
      if(selectedStatus != 0){
        reportSchedulerData = reportSchedulerData.filter(item => item.state === selectedStatus);
      }
      this.updateDatasource(reportSchedulerData);
    }
  }

  onStatusSelectionChange(_event: any){
    this.statusSelection = _event.value == '0' ? parseInt(_event.value) : _event.value;
    if(this.reportTypeSelection == 0 && this.statusSelection == 0){
      this.updateDatasource(this.initData); //-- load all data
    }
    else if(this.statusSelection == 0 && this.reportTypeSelection != 0){
      let filterData = this.initData.filter(item => item.reportTypeId === this.reportTypeSelection);
      if(filterData){
        this.updateDatasource(filterData);
      }
      else{
        this.updateDatasource([]);
      }
    }
    else if(this.statusSelection != 0 && this.reportTypeSelection == 0){
      let filterData = this.initData.filter(item => item.state == this.statusSelection);
      if(filterData){
        this.updateDatasource(filterData);
      }
      else{
        this.updateDatasource([]);
      }
    }
    else{
      let selectedReportType = this.reportTypeSelection;
      let selectedStatus = this.statusSelection;
      let reportSchedulerData = this.initData.filter(item => item.reportTypeId === selectedReportType);
      if(selectedStatus != 0){
        reportSchedulerData = reportSchedulerData.filter(item => item.state === selectedStatus);
      }
      this.updateDatasource(reportSchedulerData);
    }
  }



}
