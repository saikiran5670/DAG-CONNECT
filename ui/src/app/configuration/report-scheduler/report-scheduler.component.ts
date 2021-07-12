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
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-report-scheduler',
  templateUrl: './report-scheduler.component.html',
  styleUrls: ['./report-scheduler.component.less']
})

export class ReportSchedulerComponent implements OnInit {

  displayedColumns: string[] = ['reportName','scheduledReportVehicleRef','frequencyType','recipientList','driverList','lastScheduleRunDate','nextScheduleRunDate','status','action'];
  grpTitleVisible : boolean = false;
  errorMsgVisible: boolean = false;
  displayMessage: any;
  createEditStatus: boolean = false;
  viewStatus: boolean= false;
  showLoadingIndicator: any = false;
  actionType: any = '';
  selectedRowData: any= [];
  titleText: string;
  translationData: any= [];
  localStLanguage: any;
  dataSource: any; 
  initData: any = [];
  schedulerData: any= [];
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
  reportSchedulerParameterData: any= {};


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
        this.loadScheduledReports();  
      }); 

      this.reportSchedulerService.getReportSchedulerParameter(this.accountId, this.accountOrganizationId).subscribe(parameterData => {
        this.reportSchedulerParameterData = parameterData;
        this.ReportTypeList = this.reportSchedulerParameterData["reportType"];
        this.StatusList= [{id : "A", name : "Active"}, {id : "I", name : "Suspended"}]
      })
      
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
    this.createEditStatus = true;
  }

  onClose(){
    this.grpTitleVisible = false;
  }
 
  onBackToPage(objData){
    this.createEditStatus = objData.actionFlag;
    this.viewStatus = objData.actionFlag;
    if(objData.successMsg && objData.successMsg != ''){
      this.successMsgBlink(objData.successMsg);
    }
    this.loadScheduledReports();
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

   loadScheduledReports(){    
     this.showLoadingIndicator = true;
     this.reportSchedulerService.getReportSchedulerData(this.accountId, this.accountOrganizationId).subscribe((data) => {
       this.reportTypeSelection= 0;
       this.statusSelection= 0;
       this.schedulerData =this.makeLists(data["reportSchedulerRequest"]);  
       this.updateDatasource(this.schedulerData);  

       this.hideloader();     
    }, (error) => {
       this.hideloader();     
    })   
   
 }

 makeLists(initdata: any){
  let accountId =  localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
  initdata.forEach((element, index) => {
    let recipientTxt: any = '';
    let driverTxt: any = '';
    let vehicleGroupTxt: any = '';

    element.scheduledReportRecipient.forEach(resp => {
      recipientTxt += resp.email + ', ';
    });
    if(element.scheduledReportDriverRef.length == 1){
      driverTxt += element.scheduledReportDriverRef[0].driverName;
    }
    else{
      element.scheduledReportDriverRef.forEach(resp => {
        driverTxt += resp.driverName + ', ';
      });
    }

    if(element.scheduledReportVehicleRef.length > 1){
      let vehicleGroups = element.scheduledReportVehicleRef.filter(item => item.vehicleGroupType == 'G');
      if(vehicleGroups.length > 0){
        vehicleGroups = this.getUnique(vehicleGroups, 'vehicleGroupId');
      }
      vehicleGroups.forEach(resp => {
        vehicleGroupTxt += resp.vehicleGroupName + ', ';
      });

      let vehicles = element.scheduledReportVehicleRef.filter(item => item.vehicleGroupType == 'S');
      vehicles.forEach(resp => {
        vehicleGroupTxt += resp.vin + ', ';
      });
    }
  
    initdata[index].recipientList = recipientTxt.slice(0, -2); 
    initdata[index].driverList = driverTxt.slice(0, -2);
    initdata[index].vehicleGroupAndVehicleList = vehicleGroupTxt == "" ? vehicleGroupTxt : vehicleGroupTxt.slice(0, -2);
    initdata[index].lastScheduleRunDate = element.lastScheduleRunDate == 0 ? '-' : Util.convertUtcToDateFormat(element.lastScheduleRunDate, "MM/DD/YYYY");
    initdata[index].nextScheduleRunDate = Util.convertUtcToDateFormat(element.nextScheduleRunDate, "MM/DD/YYYY");
    initdata[index].isDriver = this.ReportTypeList.filter(item => item.id == initdata[index].reportId)[0].isDriver == 'Y' ? true : false;
  });
  
  return initdata;
}

getUnique(arr, comp) {

  // store the comparison  values in array
  const unique =  arr.map(e => e[comp])

    // store the indexes of the unique objects
    .map((e, i, final) => final.indexOf(e) === i && i)

    // eliminate the false indexes & return unique objects
  .filter((e) => arr[e]).map(e => arr[e]);

  return unique;
}

  updateDatasource(data){
    this.initData = data;
    if(this.initData.length > 0){
      this.initData = this.getNewTagData(data); 
    } 
    this.dataSource = new MatTableDataSource(this.initData);
    // this.dataSource.filterPredicate = function(data: any, filter: string): boolean {
    //   return (
    //     data.reportName.toString().toLowerCase().includes(filter) ||
    //     data.recipientList.toString().toLowerCase().includes(filter) ||
    //     data.driverList.toString().toLowerCase().includes(filter) ||
    //     data.status.toString().toLowerCase().includes(filter) 
    //   );
    // };
   
    this.dataSource.sortingDataAccessor = (data: any, sortHeaderId: string): string => {
      if (typeof data[sortHeaderId] === 'string') {
        return data[sortHeaderId].toLocaleLowerCase();
      }
    
      return data[sortHeaderId];
    };

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
    let name = item.reportName;
    this.dialogService.DeleteModelOpen(options, name);
    this.dialogService.confirmedDel().subscribe((res) => {
    if (res) {
      this.reportSchedulerService.deleteScheduledReport(item.id).subscribe((res) => {
          this.successMsgBlink(this.getDeletMsg(name));
          this.loadScheduledReports();
        }, error => {
      
        });
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
    this.rowsData= [];
    this.viewStatus= true;
    this.actionType = action;
    this.rowsData.push(row);
  }

  onEditReportScheduler(row: any, action : string) {
    this.rowsData= [];
    this.createEditStatus= true;
    this.actionType = 'edit';
    this.titleText = this.translationData.lblEditReportScheduler || "Edit Report Scheduler";
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
      confirmText: (rowData.status == 'A') ? this.translationData.lblDeactivate || " Deactivate" : this.translationData.lblActivate || " Activate",
      status: rowData.status == 'A' ? 'Deactivate' : 'Activate' ,
      name: rowData.reportName
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){ 
        let obj = {
          "reportId": rowData.id,
          "status": rowData.status
        }
        this.reportSchedulerService.enableDisableScheduledReport(obj).subscribe((data) => {
          let successMsg = "Status updated successfully."
          this.successMsgBlink(successMsg);
          this.loadScheduledReports();
        }, error => {
          this.loadScheduledReports();
        });
      }else {
        this.loadScheduledReports();
      }
    });
  }

  onVehicleGroupClick(data: any) {   
    const colsList = ['name','vin','licensePlateNumber'];
    const colsName =[this.translationData.lblVehicleName || 'Vehicle Name', this.translationData.lblVIN || 'VIN', this.translationData.lblRegistrationNumber || 'Registration Number'];
    const tableTitle =`${data.scheduledReportVehicleRef[0].vehicleGroupName} - ${this.translationData.lblVehicles || 'Vehicles'}`;
    let objData = {
      groupId: data.scheduledReportVehicleRef[0].vehicleGroupId,
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
      this.updateDatasource(this.schedulerData); //-- load all data
    }
    else if(this.reportTypeSelection == 0 && this.statusSelection != 0){
      let filterData = this.schedulerData.filter(item => item.status == this.statusSelection);
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
      let reportSchedulerData = this.schedulerData.filter(item => item.reportId === selectedReportType);
      if(selectedStatus != 0){
        reportSchedulerData = reportSchedulerData.filter(item => item.status === selectedStatus);
      }
      this.updateDatasource(reportSchedulerData);
    }
  }

  onStatusSelectionChange(_event: any){
    this.statusSelection = _event.value == '0' ? parseInt(_event.value) : _event.value;
    if(this.reportTypeSelection == 0 && this.statusSelection == 0){
      this.updateDatasource(this.schedulerData); //-- load all data
    }
    else if(this.statusSelection == 0 && this.reportTypeSelection != 0){
      let filterData = this.schedulerData.filter(item => item.reportId === this.reportTypeSelection);
      if(filterData){
        this.updateDatasource(filterData);
      }
      else{
        this.updateDatasource([]);
      }
    }
    else if(this.statusSelection != 0 && this.reportTypeSelection == 0){
      let filterData = this.schedulerData.filter(item => item.status == this.statusSelection);
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
      let reportSchedulerData = this.schedulerData.filter(item => item.reportId === selectedReportType);
      if(selectedStatus != 0){
        reportSchedulerData = reportSchedulerData.filter(item => item.status === selectedStatus);
      }
      this.updateDatasource(reportSchedulerData);
    }
  }

}
