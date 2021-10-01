import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { TranslationService } from 'src/app/services/translation.service';

@Component({
  selector: 'app-language-selection',
  templateUrl: './language-selection.component.html',
  styleUrls: ['./language-selection.component.less']
})
export class LanguageSelectionComponent implements OnInit {
  closePopup: boolean = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: any;
  selectionData = new SelectionModel(true, []);
  languages: any = [];
  translationData: any = {};

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      colsList: any,
      colsName: any;
      translationData: any,
      tableHeader: any,
    },
    private mdDialogRef: MatDialogRef<LanguageSelectionComponent>,
    private translationService: TranslationService
  ) { }

  ngOnInit(): void {
    this.translationService.getLanguageCodes().subscribe(res => {
      if(res){
        this.languages = res;
        setTimeout(() => {
          this.dataSource = new MatTableDataSource(this.languages);
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        });
      }
    })
  }

  onClickDownloadTemplate(){
    if(this.selectionData){
      let selectedLanguages = [];
      this.selectionData.selected.forEach(item => {
        selectedLanguages.push(item.code);
      })
      this.onClose({ languagesSelected: selectedLanguages });
    }
    else
    this.onClose(false);

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

  onClose(val: any) {
    this.closePopup = false;
    this.mdDialogRef.close(val);
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.onClose(false);
  }

}
