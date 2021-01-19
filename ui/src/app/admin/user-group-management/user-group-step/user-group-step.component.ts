import { Component, Input, OnInit, Output, EventEmitter, ViewChild, QueryList, ViewChildren } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import {ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-user-group-step',
  templateUrl: './user-group-step.component.html',
  styleUrls: ['./user-group-step.component.less']
})
export class UserGroupStepComponent implements OnInit {
  @Input() grpName: string;
  @Input() roleData: any;
  @Input() vehGrpData: any;
  @Input() translationData: any;

  @Output() grpCreate = new EventEmitter<boolean>();

  selectedType : any = 'both';
  grpTitleVisible: boolean = true;
  grpCreatedMsg: any = '';
  isLinear = false;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  roleDisplayedColumns: string[] = ['select', 'name', 'services'];
  vehGrpdisplayedColumns: string[] = ['select', 'name', 'vehicles', 'registrationNumber'];
  // displayedColumnsRoleConfirm: string[] = ['roleMasterId', 'name', 'isactive'];
  // displayedColumnsVehGrpConfirm: string[] = ['vehicleGroupID', 'name', 'isActive'];
  products: any[] = [{
    id: 1,
    productName: 'Netgear Cable Modem',
    productCode: 'CM700'
  },
  {
    id: 2,
    productName: 'Modem-900',
    productCode: 'CM900'
  },
  {
    id: 3,
    productName: 'Netgear Cable Modem',
    productCode: 'CM700'
  },
  {
    id: 4,
    productName: 'Modem-900',
    productCode: 'CM900'
  },
  {
    id: 5,
    productName: 'Netgear Cable Modem',
    productCode: 'CM700'
  }];
  roleDataSource: any = [];
  vehGrpDataSource: any = [];
  // selectedRoleDataSource: any = [];
  // selectedVehGrpDataSource: any = [];
  //roleDataSource = new MatTableDataSource(this.products);
  @ViewChildren(MatPaginator) paginator = new QueryList<MatPaginator>();
  @ViewChildren(MatSort) sort = new QueryList<MatSort>();

  selectionForRole = new SelectionModel(true, []);
  selectionForVehGrp = new SelectionModel(true, []);

  constructor(private _formBuilder: FormBuilder, private cdref: ChangeDetectorRef) { }

  ngAfterContentChecked() {
    // this.selectedRoleDataSource.paginator = this.paginator.toArray()[2];
    // this.selectedRoleDataSource.sort = this.sort.toArray()[2];
    // this.selectedVehGrpDataSource.paginator = this.paginator.toArray()[3];
    // this.selectedVehGrpDataSource.sort = this.sort.toArray()[3];
     //this.cdref.detectChanges(); 
  }

  ngAfterViewInit() {
    this.roleDataSource.paginator = this.paginator.toArray()[0];
    this.roleDataSource.sort = this.sort.toArray()[0];
    this.vehGrpDataSource.paginator = this.paginator.toArray()[1];
    this.vehGrpDataSource.sort = this.sort.toArray()[1];
  }

  ngOnInit() {
    //console.log("translationData:: ", this.translationData)
    this.firstFormGroup = this._formBuilder.group({
      firstCtrl: ['', Validators.required]
    });
    this.secondFormGroup = this._formBuilder.group({
      secondCtrl: ['', Validators.required]
    });
    this.roleDataSource = new MatTableDataSource(this.roleData);
    this.vehGrpDataSource = new MatTableDataSource(this.vehGrpData);
    if(this.translationData.lblUserGroupCreatedSuccessfully)
      this.grpCreatedMsg = this.translationData.lblUserGroupCreatedSuccessfully.replace('$', this.grpName);
    else
      this.grpCreatedMsg = ("User Group '$' Created Successfully").replace('$', this.grpName);
  }

  onCancel(){
    this.grpCreate.emit(false);
  }

  onConfirm() {
    this.grpCreate.emit(false);
  }

  onClose(){
    this.grpTitleVisible = false;
  }

  isAllSelectedForRole(){
    const numSelected = this.selectionForRole.selected.length;
    const numRows = this.roleDataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForRole(){
    this.isAllSelectedForRole() ? this.selectionForRole.clear() : this.roleDataSource.data.forEach(row => this.selectionForRole.select(row));
  }

  checkboxLabel(row?): string{
    if(row)
      return `${this.isAllSelectedForRole() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForRole.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  isAllSelectedForVehGrp(){
    const numSelected = this.selectionForVehGrp.selected.length;
    const numRows = this.vehGrpDataSource.data.length;
    return numSelected === numRows;
  }

  masterToggleForVehGrp(){
    this.isAllSelectedForVehGrp() ? this.selectionForVehGrp.clear() : this.vehGrpDataSource.data.forEach(row => this.selectionForVehGrp.select(row));
  }

  checkboxLabelForVehGrp(row?): string{
    if(row)
      return `${this.isAllSelectedForVehGrp() ? 'select' : 'deselect'} all`;
    else  
      return `${this.selectionForVehGrp.isSelected(row) ? 'deselect' : 'select'} row`;
  }

  onChange(event){
    this.selectedType = event.value;
    if(event.value === 'group'){

    }
    else if(event.value === 'vehicle'){

    }
    else if(event.value === 'both'){

    }
  }
  // selectedRoleDataSourceAll(){
  //   this.selectedRoleDataSource = new MatTableDataSource(this.selectionForRole.selected);
  //   return this.selectedRoleDataSource;
  // }

  // selectedVehGrpDataSourceAll(){
  //   this.selectedVehGrpDataSource = new MatTableDataSource(this.selectionForVehGrp.selected);
  //   return this.selectedVehGrpDataSource;
  // }
}
