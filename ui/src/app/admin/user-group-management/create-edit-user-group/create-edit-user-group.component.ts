import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { forkJoin } from 'rxjs';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { EmployeeService } from 'src/app/services/employee.service';
import { SelectionModel } from '@angular/cdk/collections';
import { TranslationService } from '../../../../app/services/translation.service';
import { VehicleGroup } from 'src/app/models/vehicle.model';
import { ConfirmDialogService } from 'src/app/shared/confirm-dialog/confirm-dialog.service';
import { Product, UserGroup } from 'src/app/models/users.model';

export interface vehGrpCreation {
  groupName: null;
  groupDesc: null;
}
@Component({
  selector: 'app-create-edit-user-group',
  templateUrl: './create-edit-user-group.component.html',
  styleUrls: ['./create-edit-user-group.component.css']
})
export class CreateEditUserGroupComponent implements OnInit {
  usrgrp: UserGroup = {
    organizationId: null,
    name: null,
    isActive: null,
    id: null,
    usergroupId: null,
    vehicles: null,
    users: null,
    userGroupDescriptions: null,
  };

  @Output() backToPage = new EventEmitter<any>();
  UsrGrpColumns: string[] = [
    'All',
    'User Name',
    'Email ID',
    'User Role',
    'User Group',
  ];
  displayedColumns: string[] = [
    'select',
    'firstName',
    'emailId',
    'role',
    'userGroup',
  ];
 
  vehGrp: VehicleGroup;
  vehSelectionFlag: boolean = false;
  mainTableFlag: boolean = true;
  vehGC: vehGrpCreation = { groupName: null, groupDesc: null };
  selectionForVehGrp = new SelectionModel(true, []);
  dataSource: any = new MatTableDataSource([]);
  dataSourceUsers: any = new MatTableDataSource([]);
  selectedType: any = '';
  columnNames: string[];
  products: any[] = [];
  newUserGroupName: any;
  enteredUserGroupDescription: any; 
  editUserContent: boolean = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  inputText: any;
  @Input() translationData : any;
  @Input() createStatus:boolean;
  @Input() editFlag:boolean;
  @Input() viewDisplayFlag:boolean;
  @Input() selectedRowData:any;

  userCreatedMsg: any = '';
  // grpTitleVisible: boolean = false;
  userName: string = '';
  viewFlag: boolean = false;
  initData: any;
  rowsData: any;
  titleText: string;
  
  UserGroupForm: FormGroup;
  constructor(private _formBuilder: FormBuilder,
    private userService: EmployeeService,
    private translationService: TranslationService,
    private _snackBar: MatSnackBar,
    private dialogService: ConfirmDialogService) { }


  ngOnInit(): void {

    this.UserGroupForm = this._formBuilder.group({
      userGroupName: ['', [Validators.required]],
      userGroupDescription: [],
    });
   
    this.loadUsersData();

  }

 
  loadUsersData(){
    this.userService.getUsers().subscribe((usrlist)=>{
      // this.filterFlag = true;
      this.initData = usrlist;
      this.dataSourceUsers = new MatTableDataSource(usrlist);
      this.dataSourceUsers.paginator = this.paginator;
      this.dataSourceUsers.sort = this.sort;
    });
  }
  OpenDialog(options, flag, item) {
    // this.alertService.success("sucess!");

    if (flag == '') {
      //Model for create
      this.dialogService.open(options);
      this.dialogService.confirmed().subscribe((res) => {
        if (res) {
          this.loadUsersData();
        }
      });
    } else {
      //Model for delete
      let name = `${item.salutation} ${item.firstName} ${item.lastName}`;
      this.dialogService.DeleteModelOpen(options, name);
      this.dialogService.confirmedDel().subscribe((res) => {
        if (res) {
          //console.log('userID :',item.userID);
          this.userService.deleteUser(item.userID).subscribe(d=>{
            console.log(d);
          });
          this.openSnackBar('Item delete', 'dismiss');
          this.loadUsersData();
        }
      });
    }
  }

  onCancel() {
    this.createStatus = false;
    this.backToPage.emit({ editFlag: false, editText: 'cancel' });
  }
  onReset(){
    // this.newUserGroupName = '';
    // this.enteredUserGroupDescription = '';
  }
  onInputChange(event){
    
    this.newUserGroupName = event.target.value;
  }
  onInputGD(event){
    this.enteredUserGroupDescription = event.target.value;
  }

  onCreate(res){
    let create = document.getElementById("createUpdateButton");

    // this.UserGroupForm.controls.userGroupName.value
    // let create = this.UserGroupForm.get('createUpdateButton')
  // mockData added for API
  let randomMockId = Math.random();
    let id = randomMockId;
  this.usrgrp = {
    organizationId: 1,
    name: this.UserGroupForm.controls.userGroupName.value,
    isActive: true,
    id: id,
    usergroupId: id,
    vehicles: "05",
    users: "04",
    userGroupDescriptions: this.UserGroupForm.controls.userGroupDescription.value,
  }
    this.userCreatedMsg = this.getUserCreatedMessage();
    this.createStatus = false;
    this.editUserContent = false;
    this.editFlag= false;
    this.viewDisplayFlag = false;

    if(create.innerText == "Confirm"){
      this.usrgrp = {
        organizationId: this.selectedRowData.organizationId,
        name: this.newUserGroupName,
        isActive: true,
        id: this.selectedRowData.id,
        usergroupId: this.selectedRowData.id,
        vehicles: "05",
        users: "04",
        userGroupDescriptions: this.enteredUserGroupDescription,
      }
      this.userService.updateUserGroup(this.usrgrp).subscribe((result) => {
        console.log(result);
        this.userService.getUserGroup(1, true).subscribe((grp) => {
          this.products = grp;
          this.initData = grp;
          this.dataSource = new MatTableDataSource(grp);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.backToPage.emit({ FalseFlag: false, editText: 'create', gridData: grp, successMsg: this.userCreatedMsg });
        });
        });
        }else if(create.innerText == "Create" ){
        this.userService.createUserGroup(this.usrgrp).subscribe((d) => {
          this.userService.getUserGroup(1, true).subscribe((grp) => {
            // this.products = grp;
            // this.initData = grp;
            // this.dataSource = new MatTableDataSource(grp);
            // this.dataSource.paginator = this.paginator;
            // this.dataSource.sort = this.sort;
            this.backToPage.emit({ FalseFlag: false, editText: 'create', gridData: grp,successMsg: this.userCreatedMsg  });
          });
            });
          }
          
  }

  getUserCreatedMessage(){
    this.userName = `${this.UserGroupForm.controls.userGroupName.value}`;  
    if(this.createStatus){
      if(this.translationData.lblUserAccountCreatedSuccessfully)
        return this.translationData.lblUserAccountCreatedSuccessfully.replace('$', this.userName);
      else
        return ("User Account '$' Created Successfully").replace('$', this.userName);
    }else{
      if(this.translationData.lblUserAccountUpdatedSuccessfully)
        return this.translationData.lblUserAccountUpdatedSuccessfully.replace('$', this.userName);
      else
        return ("User Account '$' Updated Successfully").replace('$', this.userName);
    }
  }

loadUserGroupData(orgid) {
  this.userService.getUserGroup(orgid, true).subscribe((grp) => {
    this.products = grp;
    this.initData = grp;
    this.dataSource = new MatTableDataSource(grp);
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  });
}
  
  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSourceUsers.filter = filterValue;
  }

  openSnackBar(message: string, action: string) {
    let snackBarRef = this._snackBar.open(message, action, { duration: 2000 });
    snackBarRef.afterDismissed().subscribe(() => {
      console.log('The snackbar is dismissed');
    });
    snackBarRef.onAction().subscribe(() => {
      console.log('The snackbar action was triggered!');
    });
  }

  editFunc() {
    // this.userService
    // .updateUserGroup(this.usrgrp)
    // .subscribe((result) => {
    //   console.log(result);
    // });
    // this.loadUserGroupData(1);
    // this.loadUsersData();
    // this.newUserGroupName = this.UserGroupForm.controls.userGroupName.value;
    // this.enteredUserGroupDescription = this.UserGroupForm.controls.userGroupDescription.value;


    this.createStatus = false;
    this.editFlag = false;
    this.editUserContent = true;
    this.UserGroupForm.patchValue({
                  userGroupName: this.selectedRowData.name,
                  userGroupDescription: this.selectedRowData.userGroupDescriptions,
                }) 
  }
 

  masterToggleForVehGrp() {
    this.isAllSelectedForVehGrp()
      ? this.selectionForVehGrp.clear()
      : this.dataSourceUsers.data.forEach((row) =>
          this.selectionForVehGrp.select(row)
        );
  }

  isAllSelectedForVehGrp() {
    const numSelected = this.selectionForVehGrp.selected.length;
    const numRows = this.dataSourceUsers.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForVehGrp(row?): string {
    if (row)
      return `${this.isAllSelectedForVehGrp() ? 'select' : 'deselect'} all`;
    else
      return `${
        this.selectionForVehGrp.isSelected(row) ? 'deselect' : 'select'
      } row`;
  }
  

}

