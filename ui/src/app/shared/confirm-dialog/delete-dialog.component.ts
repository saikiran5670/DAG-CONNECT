import { Component, HostListener, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-delete-dialog',
  templateUrl: './delete-dialog.component.html',
  styles: [
    `
      .header,
      .dialog-message {
        text-transform: lowercase;
      }
      .header::first-letter,
      .dialog-message::first-letter {
        text-transform: uppercase;
      }
      .btn-cancel {
        background-color: red;
        color: #fff;
      }
    `,
  ],
})
export class DeleteDialogComponent {
  public deleteMsg: any = '';
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
                  cancelText: string,
                  confirmText: string,
                  message: string,
                  title: string,
                  name: string
              }, private mdDialogRef: MatDialogRef<DeleteDialogComponent>) {
                this.deleteMsg = this.data.message.replace('$', this.data.name);
  }

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
