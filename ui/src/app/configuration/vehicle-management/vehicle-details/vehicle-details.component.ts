import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from '../../../services/translation.service';
import { VehicleService } from '../../../services/vehicle.service';
import { ConfirmDialogService } from '../../../shared/confirm-dialog/confirm-dialog.service';
import { MatTableExporterDirective } from 'mat-table-exporter';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import { Workbook } from 'exceljs';
import * as fs from 'file-saver';


@Component({
  selector: 'app-vehicle-details',
  templateUrl: './vehicle-details.component.html',
  styleUrls: ['./vehicle-details.component.less']
})

export class VehicleDetailsComponent implements OnInit {
  columnCodes = ['name', 'vin', 'licensePlateNumber', 'modelId', 'relationShip', 'viewstatus', 'action'];
  columnLabels = ['Vehicle','VIN', 'RegistrationNumber', 'Model', 'Relationship', 'Status', 'Action'];
  actionType: any = '';
  selectedRowData: any = [];
  // displayedColumns: string[] = ['name', 'vin', 'licensePlateNumber', 'modelId', 'relationShip', 'status', 'action'];
  dataSource: any = new MatTableDataSource([]);
  vehicleUpdatedMsg: any = '';
  @Output() updateRelationshipVehiclesData = new EventEmitter();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatTableExporterDirective) matTableExporter: MatTableExporterDirective
  initData: any = [];
  @Input() translationData;
  @Input() relationshipVehiclesData;
  // accountOrganizationId: any = 0;
  titleVisible: boolean = false;
  showLoadingIndicator: any = false;
  // localStLanguage: any;
  actionBtn:any; 
  updateViewStatus: boolean = false;

  constructor(private vehicleService: VehicleService, private dialogService: ConfirmDialogService, private translationService: TranslationService, ) {
    this.defaultTranslation();
  }

  defaultTranslation() {
    let defaultValues = {
      lblAllVehicleDetails: "All Vehicle Details",
      lblNoRecordFound: "No Record Found",
      lblVehicle: "Vehicle",
      lblVIN: "VIN"      
    };
    this.translationData = Object.assign( {}, this.translationData, defaultValues );
  }

  onClose() {
    this.titleVisible = false;
  }
  
  ngOnInit() {
    // this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    // this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    // let translationObj = {
    //   id: 0,
    //   code: this.localStLanguage.code,
    //   type: 'Menu',
    //   name: '',
    //   value: '',
    //   filter: '',
    //   menuId: 21 //-- for vehicle mgnt
    // };
    // this.translationService.getMenuTranslations(translationObj).subscribe((data: any) => {
    //   this.processTranslation(data);
    //   // this.loadVehicleData();
    // });
    this.initData = this.updateStatusName(this.relationshipVehiclesData);
    // this.updateDataSource(this.relationshipVehiclesData)
  }
  
  // processTranslation(transData: any) {
  //   this.translationData = transData.reduce((acc: any, cur: any) => ({ ...acc, [cur.name]: cur.value }),{});
  // }

  getRelationshipVehiclesData() {
    this.updateRelationshipVehiclesData.emit()
  }

  updateStatusName(relationshipVehiclesData) {
    relationshipVehiclesData.forEach(item => {
      if(item.status == 'T'){
        item.viewstatus = 'Terminate'
      } else if(item.status == 'N'){
        item.viewstatus = 'Opt-In + OTA'
      } else if(item.status == 'A'){
        item.viewstatus = 'OTA'
      } else if(item.status == 'C'){
        item.viewstatus = 'Opt-In'
      }  else if(item.status == 'O'){
        item.viewstatus = 'Opt-Out'
      }  
    }); 
    return relationshipVehiclesData;
  }

  // updateDataSource(tableData: any) {
  //   this.initData = tableData;
  //   setTimeout(() => {
  //     this.dataSource = new MatTableDataSource(this.initData);
  //     this.dataSource.paginator = this.paginator;
  //     this.dataSource.sort = this.sort;
  //     this.dataSource.sortData = (data: String[], sort: MatSort) =>{
  //       const isAsc = sort.direction === 'asc';
  //       let columnName = this.sort.active;
  //       return data.sort((a: any, b: any) => {
  //         return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
  //       });
  //     }

  //   });
  // }

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
    this.showLoadingIndicator = true;
    if(rowData['associatedGroups'] && rowData['associatedGroups'] != '') {
      this.selectedRowData = rowData;
      this.actionType = type;
      this.updateViewStatus = true;
      this.hideloader();
    } else {
      this.vehicleService.getVehicleAssociatedGroups(rowData.id).subscribe((res) => {
        console.log("res", res)
        rowData['associatedGroups'] = res;
        this.selectedRowData = rowData;
        this.actionType = type;
        this.updateViewStatus = true;
        this.hideloader();
      })
    }
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
    // this.updateDataSource(this.initData);
  }

  showSuccessMessage(msg: any){
    this.vehicleUpdatedMsg = msg;
    this.titleVisible = true;
    setTimeout(() => {
      this.titleVisible = false;
    }, 5000);
  }

//   exportAsCSV(){
//     console.log("Yes, It is working Properly");
//     this.matTableExporter.exportTable('csv', {fileName:'VehicleMgmt_Data', sheet: 'sheet_name'});
       
// }

exportAsCSV(){  
  const title = 'Vehicle Details';
  
  const header = ['Vehicle','VIN', 'Registration Number', 'Model', 'Relationship','Status'];
  
  //Create workbook and worksheet
  let workbook = new Workbook();
  let worksheet = workbook.addWorksheet('Vehicle Management');
  //Add Row and formatting
  let titleRow = worksheet.addRow([title]);
  worksheet.addRow([]);
  titleRow.font = { name: 'sans-serif', family: 4, size: 14, underline: 'double', bold: true }
 
  worksheet.addRow([]);  
  let headerRow = worksheet.addRow(header);
  headerRow.eachCell((cell, number) => {
    cell.fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFFFFF00' },
      bgColor: { argb: 'FF0000FF' }
    }
    cell.border = { top: { style: 'thin' }, left: { style: 'thin' }, bottom: { style: 'thin' }, right: { style: 'thin' } }
  })
  this.initData.forEach(item => {
    let status1 = '';
    if(item.status == 'T'){
     status1 = 'Terminate'
    } else if(item.status == 'N'){
      status1 = 'Opt-In + OTA'
    } else if(item.status == 'A'){
      status1 = 'OTA'
    } else if(item.status == 'C'){
      status1 = 'Opt-In'
    }  else if(item.status == 'O'){
      status1 = 'Opt-Out'
    }  
    worksheet.addRow([item.name,item.vin, item.licensePlateNumber, item.modelId, item.relationShip, status1]);   
  }); 
  worksheet.mergeCells('A1:D2'); 
  for (var i = 0; i < header.length; i++) {    
    worksheet.columns[i].width = 20;      
  }
  worksheet.addRow([]); 
  workbook.xlsx.writeBuffer().then((data) => {
    let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    fs.saveAs(blob, 'VehicleMgmt_Data.xlsx');
 }) 
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
