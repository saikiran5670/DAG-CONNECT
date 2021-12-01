import {
  Component,
  OnInit,
  ViewChild,
  Inject,
  HostListener,
} from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Util } from 'src/app/shared/util';

@Component({
  selector: 'app-user-detail-table',
  templateUrl: './user-detail-table.component.html',
  styleUrls: ['./user-detail-table.component.less']
})
export class UserDetailTableComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  closePopup: boolean = true;
  dataSource: any;
  colsList: any;
  filterValue: any;
  colValues: any;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      tableData: any,
      colsList: any,
      colsName: any,
      tableTitle: any
    },
    private mdDialogRef: MatDialogRef<UserDetailTableComponent>
  ) {
    this.updateDataSource(this.data.tableData);

  }

  updateDataSource(tabel: any) {
    this.dataSource = new MatTableDataSource(this.data.tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        let columnName = this.sort.active
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc, columnName);
        });

       }

    });
    Util.applySearchFilter(this.dataSource, this.data.colsList ,this.filterValue );
  }

  compare(a: any, b: any, isAsc: boolean, columnName) {
    if(columnName == "roles" && (Array.isArray(a) || Array.isArray(b))) {
      a= Object.keys(a).length > 0 ? a[0].name : "";
      b= Object.keys(b).length > 0 ? b[0].name : "";
      a = a.toUpperCase();
      b = b.toUpperCase();
      // a.roles.forEach(rolesValue => {
      //   a = rolesValue.name
      // });
    }
    if(!(a instanceof Number)) a = a.toString().toUpperCase();
    if(!(b instanceof Number)) b = b.toString().toUpperCase();
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  onClose(val: boolean) {
    this.closePopup = val;
    this.mdDialogRef.close(val);
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.onClose(false);
  }

  ngOnInit(){ }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

}
