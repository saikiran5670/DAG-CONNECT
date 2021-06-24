import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../../services/translation.service';
import { VehicleService } from '../../../services/vehicle.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { ActiveInactiveDailogComponent } from '../../../shared/active-inactive-dailog/active-inactive-dailog.component';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { MatTableExporterDirective } from 'mat-table-exporter';

@Component({
  selector: 'app-vehicle-connect-settings',
  templateUrl: './vehicle-connect-settings.component.html',
  styleUrls: ['./vehicle-connect-settings.component.less']
})
export class VehicleConnectSettingsComponent implements OnInit {
  actionType: any = '';
  selectedRowData: any = [];
  displayedColumns: string[] = ['name', 'vin', 'licensePlateNumber', 'modelId', 'relationShip', 'status', 'connected', 'terminated'];
  dataSource: any = new MatTableDataSource([]);
  vehicleUpdatedMsg: any = '';
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  dialogRef: MatDialogRef<ActiveInactiveDailogComponent>;
  initData: any = [];
  translationData: any;
  accountOrganizationId: any = 0;
  titleVisible: boolean = false;
  showLoadingIndicator: any = false;
  localStLanguage: any;
  actionBtn:any; 
  updateViewStatus: boolean = false;
  vehicleOptInOut:any=[];
  accountId: number;
  connectedAll:any;
  totalVehicles: any = 0;
  connectedchecked: boolean = false;
 
  constructor(private vehicleService: VehicleService, private dialogService: ConfirmDialogService, private translationService: TranslationService, private dialog: MatDialog,) {
    this.defaultTranslation();      
  }

  defaultTranslation() {
    this.translationData = {
      lblAllVehicleDetails: "All Vehicle Details",
      lblNoRecordFound: "No Record Found",
      lblVehicle: "Vehicle",
      lblVIN: "VIN"      
    };
  }
 
  onClose() {
    this.titleVisible = false;
  }
  ngOnInit(): void {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.accountId = localStorage.getItem('accountId') ? parseInt(localStorage.getItem('accountId')) : 0;
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    let translationObj = {
      id: 0,
      code: this.localStLanguage.code,
      type: 'Menu',
      name: '',
      value: '',
      filter: '',
      menuId: 21 //-- for vehicle mgnt
    };
    this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
      this.processTranslation(data);  
      this.updateDataSource(data);
      this.loadVehicleData();   
    }); 
  }
  processTranslation(transData: any) {
    this.translationData = transData.reduce((acc: any, cur: any) => ({ ...acc, [cur.name]: cur.value }),{});
  }

  loadVehicleData(){
    this.showLoadingIndicator = true;
    this.vehicleService.getVehiclesData(this.accountOrganizationId).subscribe((vehData: any) => {
      this.hideloader();
      this.updateDataSource(vehData);
      this.vehicleOptInOut = [];
      this.connectedAll = false;  
      this.totalVehicles= vehData.length;
    }, (error) => {
        //console.error(error);
        this.hideloader();
        this.updateDataSource([]);
      }
    );
  }
 
  updateDataSource(tableData: any) {
    this.initData = tableData;
    setTimeout(() => {
      this.dataSource = new MatTableDataSource(this.initData);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  
  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  editViewVehicle(rowData: any, type: any){
    this.selectedRowData = rowData;
    this.actionType = type;
    this.updateViewStatus = true;
  }

  onVehicleUpdateView(item: any){
    //this.updateViewStatus = !this.updateViewStatus;
    this.updateViewStatus = item.stepFlag;
    if(item.successMsg && item.successMsg != ''){
      this.showSuccessMessage(item.successMsg);
    }
    if(item.tableData){
      this.initData = item.tableData;  
    }
    this.updateDataSource(this.initData);
  }

  showSuccessMessage(msg: any){
    this.vehicleUpdatedMsg = msg;
    this.titleVisible = true;
    setTimeout(() => {
      this.titleVisible = false;
    }, 5000);
  }
  
  filterChangeStatus(data){
  }

  onCheckboxChange(e) {  
    if (e.target.checked) {
      this.vehicleOptInOut.push(e.target.value);
    } else {   
       const index = this.vehicleOptInOut.indexOf(e.target.value)
       this.vehicleOptInOut.splice(index,1);
    }   
    console.log(this.vehicleOptInOut);
  }

  getVehicleData(item: any){
    let obj = {
      opt_In: 'I',
      modifiedBy: this.accountId,
      vehicleId:Number(item)
    }    
    return obj;
  }
  fieldsChange(values){
    if (values.checked) {
      this.connectedAll = true;     
    }else{
      this.connectedAll = false;       
    }   
  }
  onChangeConnectedAllStatus(rowData: any){  
    if( this.vehicleOptInOut.length > 0){    
    const options = {
      title: this.translationData.lblConnected || "Confirmation",
      message: this.translationData.lblYouwanttoDetails || "Are you sure want to change all vehicle status Connected  # ?\n Out of "+ this.totalVehicles +" vehicles "+ this.vehicleOptInOut.length  + " will be change the status from the connected Off to On!",   
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.optIn == 'I') ? this.translationData.lblDeactivate || "Connected Off" : this.translationData.lblActivate || " Connected On",
      status: rowData.optIn == 'I' ? 'On to Off' : 'Off to On' ,
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = false;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){           
       let connectedData: any = []; 
        this.vehicleOptInOut.forEach(element => {
          connectedData.push(this.getVehicleData(element));
        });  
        this.vehicleService.updatevehicleconnection(connectedData).subscribe((data) => {
            this.loadVehicleData();           
          }, error => {
            this.loadVehicleData();
          });         
      }else {       
         this.loadVehicleData();        
      }         
    });  
  }
  else{   
      let selectCheckBoxMsg= this.translationData.lblselectCheckBoxMsg || 'Select list checkbox for changing the status' 
      alert(selectCheckBoxMsg);     
      return false;     
  }
}

  
  onChangeConnectedStatus(rowData: any){
    const options = {
      title: this.translationData.lblConnected || "Confirmation",
      message: this.translationData.lblYouwanttoDetails || "Are you sure want to change status Connected  # '$' Vehicle?",   
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.optIn == 'I') ? this.translationData.lblDeactivate || "Connected Off" : this.translationData.lblActivate || " Connected On",
      status: rowData.status == 'C' ? 'On to Off' : 'Off to On' ,
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){ 
        let statusOptIn;
        if(rowData.optIn == 'I'){
          statusOptIn='U';
        }
        else{
          statusOptIn='I';
        }
        let statusObj = {
          isOptIn: statusOptIn,
          modifiedBy: this.accountId,
          vehicleId: rowData.id    
        };               
        this.vehicleService.setoptinstatus(statusObj).subscribe((data) => {
            this.loadVehicleData();          
          }, error => {
            this.loadVehicleData();
          });         
      }else {
        this.loadVehicleData();
      }
    });
  }

  onChangeTerminatedStatus(rowData: any){
    if(rowData.status != 'T'){    
    const options = {
      title: this.translationData.lblConnected || "Confirmation",
      message: this.translationData.lblYouwanttoDetails || "Are you sure want to Terminated '$' Vehicle?",   
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.status == 'T') ? this.translationData.lblDeactivate || "Terminated Off" : this.translationData.lblActivate || " Terminated On",
      name: rowData.name
    };
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){         
        let statusObj = {
          isTerminate: true,
          modifiedBy: this.accountId,
          vehicleId: rowData.id    
        };            
        this.vehicleService.terminateVehiclestatus(statusObj).subscribe((data) => {
            this.loadVehicleData();          
          }, error => {
            this.loadVehicleData();
          });         
      }else {
        this.loadVehicleData();
      }
    });  
  } 
}
}
