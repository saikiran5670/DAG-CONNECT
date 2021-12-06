import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { AccountService } from 'src/app/services/account.service';
import { Util } from 'src/app/shared/util';

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
  servicesIcon: any = ['service-icon-daf-connect', 'service-icon-eco-score', 'service-icon-open-platform', 'service-icon-open-platform-inactive', 'service-icon-daf-connect-inactive', 'service-icon-eco-score-inactive', 'service-icon-open-platform-1', 'service-icon-open-platform-inactive-1'];
  filterValue: string;
  columns: any;

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
    private accountService: AccountService
  ) { }

  ngOnInit() {
    this.dataSource = new MatTableDataSource(this.data.tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
            let columnName = sort.active;
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });

    }
    Util.applySearchFilter(this.dataSource, this.columns ,this.filterValue );

    this.selectTableRows();
  });
}
compare(a: any, b: any, isAsc: boolean, columnName:any) {
  if(!(a instanceof Number)) a = a.toString().toUpperCase();
  if(!(b instanceof Number)) b = b.toString().toUpperCase();
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}


  selectTableRows() {
    this.dataSource.data.forEach(row => {
      let search = this.data.selectedData.filter(item => (this.data.type == 'role' ? item.id : item.groupId) == (this.data.type == 'role' ? row.roleId : row.groupId));
      if (search.length > 0) {
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

  onConfirm() {
    if (this.data.type == 'role') { //-- role
      let mapRoleIds: any = this.data.selectedData.map(resp => resp.id);

      let roleDeleteObj = {
        accountId: this.data.accountInfo.id,
        organizationId: this.data.accountInfo.organizationId
        //roles: mapRoleIds
      }

      let selectedRoleIds = this.selectionData.selected.map(resp => resp.roleId);
      let roleAddObj = {
        accountId: this.data.accountInfo.id,
        organizationId: this.data.accountInfo.organizationId,
        roles: selectedRoleIds
      }
      let updatedRoles = [];

      if (mapRoleIds.length > 0 && selectedRoleIds.length > 0) {
        this.accountService.deleteAccountRoles(roleDeleteObj).subscribe(delResp => {
          this.accountService.addAccountRoles(roleAddObj).subscribe(addResp => {
            updatedRoles = [];
            selectedRoleIds.forEach(element => {
              updatedRoles.push({ id: element });
            });
            this.onClose({ data: updatedRoles, type: this.data.type });
          })
        })
      } else if (mapRoleIds.length == 0 && selectedRoleIds.length > 0) {
        this.accountService.addAccountRoles(roleAddObj).subscribe(addResp => {
          updatedRoles = [];
          selectedRoleIds.forEach(element => {
            updatedRoles.push({ id: element });
          });
          this.onClose({ data: updatedRoles, type: this.data.type });
        })
      } else if (mapRoleIds.length > 0 && selectedRoleIds.length == 0) {
        this.accountService.deleteAccountRoles(roleDeleteObj).subscribe(delResp => {
          updatedRoles = [];
          this.onClose({ data: updatedRoles, type: this.data.type });
        })
      } else {
        updatedRoles = [];
        this.onClose({ data: updatedRoles, type: this.data.type });
      }
    }
    else { // account group
      let accountId = this.data.accountInfo.id;
      let mapGrpData: any = [];
      let selectedGrpIds: any = this.selectionData.selected.map(resp => resp.groupId); //--id
      if (selectedGrpIds.length > 0) {
        selectedGrpIds.forEach(element => {
          mapGrpData.push({
            accountGroupId: element,
            accountId: accountId
          });
        });
      }
      else {
        mapGrpData = [{
          accountGroupId: 0,
          accountId: accountId
        }];
      }

      let grpObj = {
        accounts: mapGrpData
      }

      if (selectedGrpIds.length > 0 && this.data.selectedData.length > 0) {
        this.accountService.deleteAccountGroupsForAccount(accountId).subscribe(resp => {
          this.accountService.addAccountGroups(grpObj).subscribe((data) => {
            this.onClose({ data: this.selectionData.selected, type: this.data.type });
          }, (error) => { });
        })
      } else if (selectedGrpIds.length > 0 && this.data.selectedData.length == 0) {
        this.accountService.addAccountGroups(grpObj).subscribe((data) => {
          this.onClose({ data: this.selectionData.selected, type: this.data.type });
        }, (error) => { });
      } else if (selectedGrpIds.length == 0 && this.data.selectedData.length > 0) {
        this.accountService.deleteAccountGroupsForAccount(accountId).subscribe(resp => {
          this.onClose({ data: this.selectionData.selected, type: this.data.type });
        }, (error) => { });
      } else {
        this.onClose({ data: this.selectionData.selected, type: this.data.type });
      }
    }
  }

  onReset() {
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
      return `${this.selectionData.isSelected(row) ? 'deselect' : 'select'
        } row`;
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

}
