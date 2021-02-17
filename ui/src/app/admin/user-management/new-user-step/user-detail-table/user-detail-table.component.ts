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
    });
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
