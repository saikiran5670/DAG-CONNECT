import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-link-org-popup',
  templateUrl: './link-org-popup.component.html',
  styleUrls: ['./link-org-popup.component.less']
})

export class LinkOrgPopupComponent implements OnInit {
  public emailMsg: any = '';
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
                  cancelText: any,
                  confirmText: any,
                  existMessage: any,
                  alertMessage: any,
                  title: any,
                  email: any
              }, private mdDialogRef: MatDialogRef<LinkOrgPopupComponent>) {
                this.emailMsg = this.data.existMessage.replace('$', this.data.email);
  }

  ngOnInit() { }

  public cancel() {
    this.close(false);
  }

  public close(value: any) {
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
