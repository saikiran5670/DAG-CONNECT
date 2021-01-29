import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AccountService } from 'src/app/services/account.service';
import { EmployeeService } from 'src/app/services/employee.service';

@Component({
  selector: 'app-edit-common-table',
  templateUrl: './edit-common-table.component.html',
  styleUrls: ['./edit-common-table.component.less']
})
export class EditCommonTableComponent implements OnInit {
  closePopup: boolean = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: any;
  selectionData = new SelectionModel(true, []);

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      accountInfo: any,
      type: any,
      colsList: any,
      colsName: any,
      translationData: any,
      tableData: any,
      tableHeader: any,
      selectedData: any
    },
    private mdDialogRef: MatDialogRef<EditCommonTableComponent>,
    private userService: EmployeeService,
    private accountService: AccountService
  ) { }

  ngOnInit() {
    this.dataSource = new MatTableDataSource(this.data.tableData);    
    setTimeout(() => {
       this.dataSource.paginator = this.paginator;
       this.dataSource.sort = this.sort;
    });
    this.selectTableRows();
  }

  selectTableRows(){
    this.dataSource.data.forEach(row => {
      let search = this.data.selectedData.filter(item => item.id == (this.data.type=='role' ? row.roleId  : row.id));
      if(search.length > 0){
        this.selectionData.select(row);
      }
    });
  }

  onClose(val: any) {
    this.closePopup = false;
    this.mdDialogRef.close(val);
  }
  @HostListener('keydown.esc')
  public onEsc() {
    this.onClose(false);
  }

  onConfirm(){
    if(this.data.type == 'role'){
      let mapRoleIds :any = this.data.selectedData.map(resp => resp.id);
    //let mapRoleData: any = [];
    
    // if(mapRoleIds.length > 0){
    //   mapRoleData = mapRoleIds;
    // }
    // else{
    //   mapRoleData = [0];
    // }

    let roleDeleteObj = {
      accountId: this.data.accountInfo.id,
      organizationId: this.data.accountInfo.organizationId,
      roles: mapRoleIds
    }

    let selectedRoleIds = this.selectionData.selected.map(resp => resp.roleId);

    this.accountService.deleteAccountRoles(roleDeleteObj).subscribe(delResp => {
      let roleAddObj = {
        accountId: this.data.accountInfo.id,
        organizationId: this.data.accountInfo.organizationId,
        roles: selectedRoleIds
      }
      this.accountService.addAccountRoles(roleAddObj).subscribe(addResp => {
        let updatedRoles = [];
        selectedRoleIds.forEach(element => {
          updatedRoles.push({id: element});
        });

        this.onClose({data: updatedRoles, type: this.data.type});    
      })
    })
    }else{
      //TODO : update account group
    }
    
  }
  

  onReset(){
    //console.log("Table reset...")
    this.selectionData.clear();
    this.selectTableRows();
  }

  masterToggleForSelectionData() {
    this.isAllSelectedForSelectionData()
      ? this.selectionData.clear()
      : this.dataSource.data.forEach((row) =>
          this.selectionData.select(row)
        );
  }

  isAllSelectedForSelectionData() {
    const numSelected = this.selectionData.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  checkboxLabelForSelectionData(row?): string {
    if (row)
      return `${this.isAllSelectedForSelectionData() ? 'select' : 'deselect'} all`;
    else
      return `${
        this.selectionData.isSelected(row) ? 'deselect' : 'select'
      } row`;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }
  
}
