import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../../services/translation.service';
import { VehicleService } from '../../../services/vehicle.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';


@Component({
  selector: 'app-vehicle-details',
  templateUrl: './vehicle-details.component.html',
  styleUrls: ['./vehicle-details.component.less']
})

export class VehicleDetailsComponent implements OnInit {
  actionType: any = '';
  selectedRowData: any = [];
  displayedColumns: string[] = ['name', 'vin', 'licensePlateNumber', 'modelId', 'relationShip', 'status', 'action'];
  dataSource: any = new MatTableDataSource([]);
  vehicleUpdatedMsg: any = '';
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  initData: any = [];
  translationData: any;
  accountOrganizationId: any = 0;
  titleVisible: boolean = false;
  showLoadingIndicator: any = false;
  localStLanguage: any;
  actionBtn:any; 
  updateViewStatus: boolean = false;

  constructor(private vehicleService: VehicleService, private dialogService: ConfirmDialogService, private translationService: TranslationService, ) {
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
  
  ngOnInit() {
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
      this.dataSource.sortData = (data: String[], sort: MatSort) =>{
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active;
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
      }

    });
  }

  compare(a: Number  | String, b: Number  | String, isAsc: boolean, columnName: any){
    if(columnName == "name"  || columnName == "vin"){
      if(!(a instanceof Number)) a = a.toString().toUpperCase();
      if(!(b instanceof Number)) b = b.toString().toUpperCase();
    }
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
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

  exportAsCSV(){
    this.matTableExporter.exportTable('csv', {fileName:'VehicleMgmt_Data', sheet: 'sheet_name'});
}

exportAsPdf() {
  let DATA = document.getElementById('vehicleMgmtData');
    
  html2canvas( DATA , { onclone: (document) => {
    this.actionBtn = document.getElementsByClassName('action');
    for (let obj of this.actionBtn) {
      obj.style.visibility = 'hidden';  }       
  }})
  .then(canvas => {  
      
      let fileWidth = 208;
      let fileHeight = canvas.height * fileWidth / canvas.width;
      
      const FILEURI = canvas.toDataURL('image/png')
      let PDF = new jsPDF('p', 'mm', 'a4');
      let position = 0;
      PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight)
      
      PDF.save('VehicleMgmt_Data.pdf');
      PDF.output('dataurlnewwindow');
  });     
}

}
