import {
  Component,
  OnInit,
  Input,
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
  customWidth: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      tableData: any,
      colsList: any,
      colsName: any,
      tableTitle: any,
      translationData: any,
      popupWidth?: any
    },
    private mdDialogRef: MatDialogRef<UserDetailTableComponent>
  ) {
    this.customWidth = data.popupWidth ? true : false;
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
       Util.applySearchFilter(this.dataSource, this.data.colsList ,this.filterValue );
    });
  }

  compare(a: any, b: any, isAsc: boolean, columnName: any) {

    if(columnName === "licensePlateNumber"){
      let reA = /[^a-zA-Z]/g;
      let reN = /[^0-9]/g;
      let aA = a.replace(reA, "").toUpperCase();
      let bA = b.replace(reA, "").toUpperCase();
      if (aA === bA) {
        var aN = parseInt(a.replace(reN, ""), 10);
        var bN = parseInt(b.replace(reN, ""), 10);
        return (aN === bN ? 0 : aN > bN ? 1 : -1) * (isAsc ? 1: -1) ;
      } else {
        return (aA > bA ? 1 : -1) * (isAsc ? 1: -1);
      }
    }
   

    if(columnName === "roleList" || columnName === "accountGroupList" ||  columnName === "roles") { //Condition added for roles columns
      a=  a.toString().toUpperCase() ;
      b= b.toString().toUpperCase();
   
    }

    if(columnName === "firstName"){
      if(!(a instanceof Number)) a = a.replace(/[^\w\s]/gi, 'z').toUpperCase();
      if(!(b instanceof Number)) b = b.replace(/[^\w\s]/gi, 'z').toUpperCase();

    }


      if(!(a instanceof Number)) a = a?.replace(/[^\w\s]/gi, 'z').toUpperCase();
      if(!(b instanceof Number)) b = b?.replace(/[^\w\s]/gi, 'z').toUpperCase();

     

      return ( a < b ? -1 : 1) * (isAsc ? 1: -1);
  }

//  sortAlphaNum(a, b,isAsc?,col?) {
 
//  }




  // compare(a: any, b: any, isAsc: boolean, columnName) {
  //   if(columnName == "roles" && (Array.isArray(a) || Array.isArray(b))) {
  //     a= Object.keys(a).length > 0 ? a[0].name : "";
  //     b= Object.keys(b).length > 0 ? b[0].name : "";
  //     a = a.toUpperCase();
  //     b = b.toUpperCase();
  //     // a.roles.forEach(rolesValue => {
  //     //   a = rolesValue.name
  //     // });
  //   }
  //   if(!(a instanceof Number)) a = a.toString().toUpperCase();
  //   if(!(b instanceof Number)) b = b.toString().toUpperCase();
  //   return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  // }

  onClose(val: boolean) {
    this.closePopup = val;
    this.mdDialogRef.close(val);
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.onClose(false);
  }

  ngOnInit(){ 
    
  }

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // Datasource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

}
