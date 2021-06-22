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

  constructor(private vehicleService: VehicleService, private dialogService: ConfirmDialogService, private translationService: TranslationService, private dialog: MatDialog, ) {
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
  onChangeConnectedAllStatus(){

  }

  filterChangeStatus(data){

  }
  onChangeConnectedStatus(rowData: any){
    const options = {
      title: this.translationData.lblAlert || "Alert",
      message: this.translationData.lblYouwanttoDetails || "You want to # '$' Details?",   
      cancelText: this.translationData.lblCancel || "Cancel",
      confirmText: (rowData.state == 'A') ? this.translationData.lblDeactivate || " Suspend" : this.translationData.lblActivate || " Activate",
      status: rowData.state == 'A' ? 'Suspend' : 'Activate' ,
      name: rowData.name
    };
    const dialogConfig = new MatTableDataSource(this.initData);
    // dialogConfig.disableClose = true;
    // dialogConfig.autoFocus = true;
    // dialogConfig.data = options;
    this.dialogRef = this.dialog.open(ActiveInactiveDailogComponent, dialogConfig);
    this.dialogRef.afterClosed().subscribe((res: any) => {
      if(res == true){ 
       if(rowData.state == 'A'){
          this.vehicleService.setoptinstatus(rowData.id).subscribe((data) => {
            this.loadVehicleData();
            // let successMsg = "Updated Successfully!";
            // this.successMsgBlink(successMsg);
          }, error => {
            this.loadVehicleData();
          });
       }
       else{
        this.vehicleService.setoptinstatus(rowData.id).subscribe((data) => {
          this.loadVehicleData();
          // let successMsg = "Updated Successfully!";
          // this.successMsgBlink(successMsg);
        }, error => {
          this.loadVehicleData();
        });

       }
        
      }else {
        this.loadVehicleData();
      }
    });
  }
}
