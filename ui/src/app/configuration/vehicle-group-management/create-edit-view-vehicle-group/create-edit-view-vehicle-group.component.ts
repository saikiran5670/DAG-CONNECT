import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

@Component({
  selector: 'app-create-edit-view-vehicle-group',
  templateUrl: './create-edit-view-vehicle-group.component.html',
  styleUrls: ['./create-edit-view-vehicle-group.component.less']
})

export class CreateEditViewVehicleGroupComponent implements OnInit {
  accountOrganizationId: any = 0;
  @Output() backToPage = new EventEmitter<any>();
  displayedColumns: string[] = ['select', 'name', 'vin', 'licensePlateNumber', 'modelId'];
  selectedVehicles = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() translationData: any;
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Input() vehicleListData: any;
  vehGroupTypeList: any = [];
  methodTypeList: any = [];
  vehicleGroupForm: FormGroup;
  breadcumMsg: any = '';
  duplicateVehicleGroupMsg: boolean = false;
  showVehicleList: boolean = true;

  constructor(private _formBuilder: FormBuilder) { }

  ngOnInit() { 
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.vehicleGroupForm = this._formBuilder.group({
      vehicleGroupName: ['', [Validators.required]],
      vehicleGroupType: ['', [Validators.required]],
      methodType: [],
      vehicleGroupDescription: []
    });
    this.vehGroupTypeList = [
      {
        name: this.translationData.lblGroup || 'Group',
        value: 'G'
      },
      {
        name: this.translationData.lblDynamic || 'Dynamic',
        value: 'D'
      }
    ];
    this.methodTypeList = [
      {
        name: this.translationData.lblAll || 'All',
        value: 'A'
      },
      {
        name: this.translationData.lblOwnedVehicles || 'Owned Vehicles',
        value: 'O'
      },
      {
        name: this.translationData.lblVisibleVehicles || 'Visible Vehicles',
        value: 'V'
      }
    ];
    this.vehicleGroupForm.get('vehicleGroupType').setValue('G'); //-- default selection Group
    if(this.actionType == 'edit' ){
      this.setDefaultValue();
    }
    if(this.actionType == 'view' || this.actionType == 'edit'){
      this.showHideVehicleList();
    }
    this.loadGridData(this.vehicleListData);
    this.breadcumMsg = this.getBreadcum();
  }

  getBreadcum() {
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} / ${this.translationData.lblConfiguration ? this.translationData.lblConfiguration : 'Configuration'} / ${this.translationData.lblVehicleGroupManagement ? this.translationData.lblVehicleGroupManagement : "Vehicle Group Management"} / ${this.translationData.lblVehicleGroupDetails ? this.translationData.lblVehicleGroupDetails : 'Vehicle Group Details'}`;
  }

  setDefaultValue(){
    this.vehicleGroupForm.get('vehicleGroupName').setValue(this.selectedRowData.groupName);
    this.vehicleGroupForm.get('vehicleGroupType').setValue(this.selectedRowData.groupType);
    this.vehicleGroupForm.get('methodType').setValue(this.selectedRowData.groupType);
    this.vehicleGroupForm.get('vehicleGroupDescription').setValue(this.selectedRowData.description);
  }

  showHideVehicleList(){
    if(this.selectedRowData.groupType == 'D'){ //-- Dynamic Group
      this.showVehicleList = false;
    }else{ //-- Normal Group
      this.showVehicleList = true;
    }
  }

  onCancel(){
    let emitObj = {
      stepFlag: false,
      successMsg: ""
    }  
    this.backToPage.emit(emitObj);
  }

  selectTableRows(){
    this.dataSource.data.forEach((row: any) => {
      let search = this.selectedRowData.selectedVehicleList.filter((item: any) => item.id == row.id);
      if (search.length > 0) {
        this.selectedVehicles.select(row);
      }
    });
  }

  loadGridData(tableData: any){
    let selectedVehicleList: any = [];
    if(this.actionType == 'view'){
      tableData.forEach((row: any) => {
        let search = this.selectedRowData.selectedVehicleList.filter((item: any) => item.id == row.id);
        if (search.length > 0) {
          selectedVehicleList.push(row);
        }
      });
      tableData = selectedVehicleList;
      this.displayedColumns = ['name', 'vin', 'licensePlateNumber', 'modelId'];
    }
    this.updateDataSource(tableData);
    if(this.actionType == 'edit' ){
      this.selectTableRows();
    }
  }

  updateDataSource(tableData: any){
    this.dataSource = new MatTableDataSource(tableData);
    setTimeout(()=>{
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  onReset(){ //-- Reset
    this.selectedVehicles.clear();
    this.selectTableRows();
    this.setDefaultValue();
    this.showHideVehicleList();
  }

  vehGroupTypeChange(event: any){
    if(event.value == 'D'){ //-- Dynamic Group
      this.showVehicleList = false;
      this.vehicleGroupForm.get('methodType').setValue('A');
    }
    else{ //-- Normal Group
      this.showVehicleList = true;
      this.selectedVehicles.clear();
    }
  }

  methodTypeChange(event: any){

  }

  onCreateUpdateVehicleGroup(){
    this.onCancel();
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  masterToggleForVehicle() {
    this.isAllSelectedForVehicle()
      ? this.selectedVehicles.clear()
      : this.dataSource.data.forEach((row) =>
        this.selectedVehicles.select(row)
      );
  }

  isAllSelectedForVehicle() {
    const numSelected = this.selectedVehicles.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForVehicle(row?: any): string {
    if (row)
      return `${this.isAllSelectedForVehicle() ? 'select' : 'deselect'} all`;
    else
      return `${this.selectedVehicles.isSelected(row) ? 'deselect' : 'select'} row`;
  }

}