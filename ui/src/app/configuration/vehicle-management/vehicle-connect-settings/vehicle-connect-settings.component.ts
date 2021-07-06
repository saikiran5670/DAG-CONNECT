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
  displayedColumns: string[] = ['name', 'vin', 'licensePlateNumber', 'modelId', 'status', 'connected', 'terminated'];
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
  legendsDisabled: boolean = false;
  loadVehData: any = [];
  connectedOn:any=[];
  connectedOff:any=[];
 
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
      this.loadVehData = vehData;
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
 
  filterChangeStatus(event){
    let filterValue='';
    filterValue = event.value;   
    if(filterValue == ""){
      this.dataSource.filter = '';
    }
    else{
    this.dataSource.filterPredicate = function(data, filter: string): boolean {
      return data.status === filter;
    };  
    this.dataSource.filter = filterValue;
    }    
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
    let obj = {};    
    this.loadVehData.forEach(element => {
      if(element.id == item)
      {
        let optInStatus='';
        if(element.optIn == 'U' ||element.optIn == 'H'){
          optInStatus = 'I';
          this.connectedOn.push(element.id)
        }
        else{
          optInStatus = 'U';
          this.connectedOff.push(element.id)
        }
        obj = {
          opt_In: optInStatus,
          modifiedBy: this.accountId,
          vehicleId:Number(item)
        }
      }    
    }); 
    return obj;
  }
 
  onChangeConnectedAllStatus(rowData: any){    
    if( this.vehicleOptInOut.length > 0){    
    let connectedData: any = []; 
    this.connectedOn=[];
    this.connectedOff=[];
    this.vehicleOptInOut.forEach(element => {
      connectedData.push(this.getVehicleData(element));
    });
    let connectedOffData = this.connectedOff.length != 0  ? this.connectedOff.length  + ' will be change the status from the connected On to Off!   ' : '';
    let connectedOnData = this.connectedOn.length != 0  ? this.connectedOn.length  + ' will be change the status from the connected Off to On!     ' : '';
    const options = {
      title: this.translationData.lblConnected || "Confirmation",
      message: this.translationData.lblYouwanttoDetails || "Are you sure want to change all vehicle status? \n Out of "+ this.totalVehicles +" vehicles    "+ connectedOnData  + connectedOffData,   
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: this.translationData.lblConnected || "Confirm",
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
      confirmText: (rowData.status == "C" && rowData.optIn == "I"|| rowData.status == "C" && rowData.optIn == "H" || rowData.status == "N" && rowData.optIn == "H") ? this.translationData.lblDeactivate || "Connected Off" : this.translationData.lblActivate || " Connected On",
      status: (rowData.status == "C" && rowData.optIn == "I" || rowData.status == 'C'  && rowData.optIn == "H"|| rowData.status == "N"  && rowData.optIn == "H")? 'On to Off' : 'Off to On' ,
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
