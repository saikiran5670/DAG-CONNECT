import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-delete-category-popup',
  templateUrl: './delete-category-popup.component.html',
  styleUrls: ['./delete-category-popup.component.less']
})

export class DeleteCategoryPopupComponent implements OnInit {
  deleteMsg: any = '';
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
        cancelText: string,
        confirmText: string,
        delMessage: string,
        noDelMessage: string,
        title: string,
        deleteCatList: string,
        noDeleteCatList: string
    }, private mdDialogRef: MatDialogRef<DeleteCategoryPopupComponent>) { 
      this.deleteMsg = data.delMessage.replace('$', data.deleteCatList)
    }

  ngOnInit() {}

  public cancel() {
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  public confirm() {
    this.close(true);
  }

  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }

}
