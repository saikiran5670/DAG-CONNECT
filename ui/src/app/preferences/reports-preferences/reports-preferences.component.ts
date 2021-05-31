import { SelectionModel } from '@angular/cdk/collections';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-reports-preferences',
  templateUrl: './reports-preferences.component.html',
  styleUrls: ['./reports-preferences.component.less']
})

export class ReportsPreferencesComponent implements OnInit {
  localStLanguage: any;
  @Input() translationData: any = {};
  updateMsgVisible: boolean = false;
  initData: any = [];
  displayedColumns = ['All','name'];
  showLoadingIndicator: any = false;
  dataSource: any;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  accountId: number;
  accountOrganizationId: number;
  roleID: number;
  selectionForColumns = new SelectionModel(true, []);
  showReport: boolean = false;
  editFlag: boolean = false;

  constructor(  private reportService: ReportService, ) { }

  ngOnInit() {
    this.localStLanguage = JSON.parse(localStorage.getItem("language"));
    this.accountId = parseInt(localStorage.getItem("accountId"));
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.roleID = parseInt(localStorage.getItem('accountRoleId'));
    this.loadReportData();
  }

  loadReportData(){
    this.showLoadingIndicator = true;
    this.reportService.getUserPreferenceReport(1,this.accountId,this.accountOrganizationId).subscribe((data : any) => {
      this.initData = data["userPreferences"];
      this.hideloader();
      this.updatedTableData(this.initData);
    }, (error) => {
      this.initData = [];
      this.hideloader();
      this.updatedTableData(this.initData);
    });
  }

  updatedTableData(tableData : any) {
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
    });
  }

  getName(name: any) {
    let updatedName = name.slice(15);
    return updatedName;
  }

  
  hideloader() {
    // Setting display of spinner
    this.showLoadingIndicator = false;
  }

  onClose() {
    this.updateMsgVisible = false;
  }

  editTripReportPreferences(){
    this.editFlag =  true;
  }

  isAllSelectedForColumns(){
    const numSelected = this.selectionForColumns.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForColumns(){
    this.isAllSelectedForColumns() ? 
    this.selectionForColumns.clear() : this.dataSource.data.forEach(row => {this.selectionForColumns.select(row)});

  }

  checkboxLabelForColumns(row?: any): string{
    if(row)
      return `${this.isAllSelectedForColumns() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForColumns.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  pageSizeUpdated(_event){
    setTimeout(() => {
      document.getElementsByTagName('mat-sidenav-content')[0].scrollTo(0, 0)
    }, 100);
  }

  onCancel(){
    this.editFlag = false;
  }

  onReset(){

  }

  onConfirm(){
    this.editFlag = false;
  }

}
