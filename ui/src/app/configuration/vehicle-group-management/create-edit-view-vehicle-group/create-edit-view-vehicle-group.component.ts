import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { VehicleService } from '../../../services/vehicle.service';
import { CustomValidators } from '../../../shared/custom.validators';
import { Util } from 'src/app/shared/util';

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
  @Input() translationData: any = {};
  @Input() selectedRowData: any;
  @Input() actionType: any;
  @Input() vehicleListData: any;
  vehGroupTypeList: any = [];
  methodTypeList: any = [];
  vehicleGroupForm: FormGroup;
  breadcumMsg: any = '';
  duplicateVehicleGroupMsg: boolean = false;
  showVehicleList: boolean = true;
  duplicateVehicleCheck: boolean = false;
  existingGroupList:any =[];
  filterValue: string;
  showLoadingIndicator: boolean = false;

  constructor(private _formBuilder: FormBuilder, private vehicleService: VehicleService) { }

  ngOnInit() {
    this.accountOrganizationId = localStorage.getItem('accountOrganizationId') ? parseInt(localStorage.getItem('accountOrganizationId')) : 0;
    this.vehicleGroupForm = this._formBuilder.group({
      vehicleGroupName: ['', [Validators.required, CustomValidators.noWhitespaceValidatorforDesc]],
      vehicleGroupType: ['', [Validators.required]],
      methodType: [],
      vehicleGroupDescription: ['', [CustomValidators.noWhitespaceValidatorforDesc]]
    },
    {
      validator: [
        CustomValidators.specialCharValidationForName('vehicleGroupName')
      ]
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
      this.breadcumMsg = this.getBreadcum();
    } else {
      this.breadcumMsg = this.getBreadcum();
    }
    this.loadGridData(this.vehicleListData);
    this.getVehicleGroupDataForCheckDuplicate();
  }

  getBreadcum() {
    let lastBreadCumValue : any;
    if(this.actionType == 'create') {
      lastBreadCumValue = this.translationData.lblNewVehicleGroupDetails ? this.translationData.lblNewVehicleGroupDetails : 'New Vehicle Group Details';
    } else if(this.actionType == 'edit') {
      lastBreadCumValue = this.translationData.lblEditVehicleGroupDetails ? this.translationData.lblEditVehicleGroupDetails : 'Edit Vehicle Group Details';
    } else {
      lastBreadCumValue = this.translationData.lblViewVehicleGroupDetails ? this.translationData.lblViewVehicleGroupDetails : 'View Vehicle Group Details'
    }
    return `${this.translationData.lblHome ? this.translationData.lblHome : 'Home'} /
    ${this.translationData.lblAdmin ? this.translationData.lblAdmin : 'Admin'} /
    ${this.translationData.lblVehicleGroupManagement ? this.translationData.lblVehicleGroupManagement : "Vehicle Group Management"} / `+lastBreadCumValue;
    // ${(this.actionType == 'edit') ? (this.translationData.lblEditVehicleGroupDetails ? this.translationData.lblEditVehicleGroupDetails : 'Edit Vehicle Group Details') : (this.translationData.lblViewVehicleGroupDetails ? this.translationData.lblViewVehicleGroupDetails : 'View Vehicle Group Details') }`;
  }

  setDefaultValue(){
    this.vehicleGroupForm.get('vehicleGroupName').setValue(this.selectedRowData.groupName);
    this.vehicleGroupForm.get('vehicleGroupType').setValue(this.selectedRowData.groupType);
    this.vehicleGroupForm.get('methodType').setValue(this.selectedRowData.functionEnum);
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
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
            let columnName = sort.active;
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });
       }
      Util.applySearchFilter(this.dataSource, this.displayedColumns ,this.filterValue );
    });
  }

  compare(a: any, b: any, isAsc: boolean, columnName: any) {
    if(columnName =="name"){
      if(!(a instanceof Number)) a = a.replace(/[^\w\s]/gi, 'z').toUpperCase();
      if(!(b instanceof Number)) b = b.replace(/[^\w\s]/gi, 'z').toUpperCase();
    }
      return ( a < b ? -1 : 1) * (isAsc ? 1: -1);
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
    this.duplicateVehicleGroupMsg = false;
    let vehicleList = [];
    this.selectedVehicles.selected.forEach(element => {
      vehicleList.push({ "vehicleGroupId": (this.actionType == 'create' ? 0 : this.selectedRowData.groupId), "vehicleId": element.id })
    });
    if(this.actionType == 'create'){ // create
      let createVehGrpObj = {
        id: 0,
        name: this.vehicleGroupForm.controls.vehicleGroupName.value,
        description: this.vehicleGroupForm.controls.vehicleGroupDescription.value,
        organizationId: this.accountOrganizationId,
        groupType: this.vehicleGroupForm.controls.vehicleGroupType.value,
        functionEnum: (this.vehicleGroupForm.controls.vehicleGroupType.value == "G") ? "N" : this.vehicleGroupForm.controls.methodType.value, //-- N-> Group &  O/A/V -> Dynamic
        vehicles: vehicleList
      }
      this.showLoadingIndicator=true;
      this.vehicleService.createVehicleGroup(createVehGrpObj).subscribe((createVehData: any) => {
        this.getVehicleGroupData();
        this.showLoadingIndicator=false;
      }, (err) => {
        this.showLoadingIndicator=false;
        ////console.log(err);
        if (err.status == 409) {
          this.duplicateVehicleGroupMsg = true;
        }
      });
    }
    else{ // update
      this.checkDuplicateGroupName();
      if(this.actionType == 'edit' && this.duplicateVehicleCheck){
        this.duplicateVehicleCheck=false;
        this.duplicateVehicleGroupMsg = true;
      } else{
      let updateVehGrpObj = {
        id: this.selectedRowData.groupId,
        name: this.vehicleGroupForm.controls.vehicleGroupName.value,
        description: this.vehicleGroupForm.controls.vehicleGroupDescription.value,
        organizationId: this.selectedRowData.organizationId,
        groupType: this.vehicleGroupForm.controls.vehicleGroupType.value,
        functionEnum: (this.vehicleGroupForm.controls.vehicleGroupType.value == "G") ? "N" : this.vehicleGroupForm.controls.methodType.value, //-- N-> Group &  O/A/V -> Dynamic
        vehicles: vehicleList
      }
      this.showLoadingIndicator=true;
      this.vehicleService.updateVehicleGroup(updateVehGrpObj).subscribe((updateVehData: any) => {
        this.getVehicleGroupData();
        this.showLoadingIndicator=false;
      }, (err) => {
        this.showLoadingIndicator=false;
        ////console.log(err);
        if (err.status == 409) {
          this.duplicateVehicleGroupMsg = true;
        }
      });
    }
   }
  }

  checkDuplicateGroupName(){
    if(this.actionType == 'edit'){
      let vehGrpName = `${this.vehicleGroupForm.controls.vehicleGroupName.value}`;
      this.existingGroupList.forEach(element => {
        let vehicleName = element.groupName;
        if(vehGrpName !== this.selectedRowData.groupName && vehGrpName==vehicleName){
            this.duplicateVehicleCheck = true;
         }
      });
      }
   }
    getVehicleGroupDataForCheckDuplicate(){
      this.vehicleService.getVehicleGroupList(this.accountOrganizationId).subscribe((oldVehGrpData: any) => {
        this.existingGroupList = oldVehGrpData;
      }, (error) => {
        if(error.status == 404){
          let oldVehGrpData = [];
        }
      });
    }

  getVehicleGroupData(){
    this.vehicleService.getVehicleGroupList(this.accountOrganizationId).subscribe((vehGrpData: any) => {
      this.goToLandingPage(vehGrpData);
    }, (error) => {
      if(error.status == 404){
        let vehGrpData = [];
        this.goToLandingPage(vehGrpData);
      }
    });
  }

  goToLandingPage(tableData: any){
    let createUpdateMsg = this.getVehicleCreateUpdateMessage();
    let emitObj = { stepFlag: false, gridData: tableData, successMsg: createUpdateMsg };
    this.backToPage.emit(emitObj);
  }

  getVehicleCreateUpdateMessage(){
    let vehGrpName = `${this.vehicleGroupForm.controls.vehicleGroupName.value}`;
    if(this.actionType == 'create') {
      if(this.translationData.lblNewVehicleGroupCreatedSuccessfully)
        return this.translationData.lblNewVehicleGroupCreatedSuccessfully.replace('$', vehGrpName);
      else
        return ("New Vehicle Group '$' Created Successfully").replace('$', vehGrpName);
    }else if(this.actionType == 'edit') {
      if (this.translationData.lblVehicleGroupUpdatedSuccessfully)
        return this.translationData.lblVehicleGroupUpdatedSuccessfully.replace('$', vehGrpName);
      else
        return ("Vehicle Group '$' Updated Successfully").replace('$', vehGrpName);
    }
    else{
      return '';
    }
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
