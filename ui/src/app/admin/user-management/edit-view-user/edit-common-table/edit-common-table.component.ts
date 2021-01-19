import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
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
      colsList: any,
      colsName: any,
      translationData: any,
      tableData: any,
      tableHeader: any,
      selectedData: any
    },
    private mdDialogRef: MatDialogRef<EditCommonTableComponent>,
    private userService: EmployeeService
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
      let search = this.data.selectedData.filter(item => item.id === row.id );
      if(search.length > 0){
        this.selectionData.select(row);
      }
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

  onConfirm(){
    this.onClose(false);
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
