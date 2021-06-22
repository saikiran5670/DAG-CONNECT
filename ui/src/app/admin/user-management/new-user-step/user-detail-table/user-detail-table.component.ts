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
    this.updateDataSource();
  }
  
  updateDataSource() {
    this.dataSource = new MatTableDataSource(this.data.tableData);
    setTimeout(() => {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.sortData = (data: String[], sort: MatSort) => {
        const isAsc = sort.direction === 'asc';
        return data.sort((a: any, b: any) => {
          return this.compare(a[sort.active], b[sort.active], isAsc);
        });
       }
    });
  }

  compare(a: Number | String, b: Number | String, isAsc: boolean) {
    if(!(a instanceof Number)) a = a.toUpperCase();
    if(!(b instanceof Number)) b = b.toUpperCase();
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