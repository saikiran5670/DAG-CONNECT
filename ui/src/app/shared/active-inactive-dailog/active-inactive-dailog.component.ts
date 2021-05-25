import { Component, HostListener, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-active-inactive-dailog',
  templateUrl: './active-inactive-dailog.component.html',
  styleUrls: ['./active-inactive-dailog.component.less']
})

export class ActiveInactiveDailogComponent implements OnInit {

  public alertMsg: any = '';
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
                  cancelText: string,
                  confirmText: string,
                  message: string,
                  title: string,
                  status: string,
                  name: string,
                  list?: string
              }, private mdDialogRef: MatDialogRef<ActiveInactiveDailogComponent>) {
                this.alertMsg = this.data.message.replace('#', this.data.status);
                this.alertMsg = this.alertMsg.replace('$', this.data.name);
  }

  ngOnInit() {
  }

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