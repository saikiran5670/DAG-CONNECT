import { Component, Inject, OnInit,} from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-terms-and-condition-popup',
  templateUrl: './terms-and-condition-popup.component.html',
  styleUrls: ['./terms-and-condition-popup.component.less']
})
export class TermsAndConditionPopupComponent implements OnInit {
  dataSource: any;

  constructor( @Inject(MAT_DIALOG_DATA)
  public data: {
    tableData: any,
    colsList: any,
    colsName: any,
  }, private mdDialogRef: MatDialogRef<TermsAndConditionPopupComponent>) {
    this.updateDataSource(this.data.tableData);
   }

  ngOnInit(): void {
  }

  updateDataSource(tableData: any) {
    this.dataSource = new MatTableDataSource(this.data.tableData);
  }

  onCancel(){
    this.mdDialogRef.close();
  }

  acceptLoginClicked(){
    let acceptCookies = true;
    this.mdDialogRef.close({data:acceptCookies});
  }
}
