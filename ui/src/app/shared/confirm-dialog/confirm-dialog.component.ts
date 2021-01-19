import { Component, HostListener, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
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
export class ConfirmDialogComponent {
  result : any;
  constructor(@Inject(MAT_DIALOG_DATA) public data: {
                  button1Text: string,
                  button2Text: string,
                  message: string,
                  title: string,
                  inputText:string
              }, private mdDialogRef: MatDialogRef<ConfirmDialogComponent>) { }

  public cancel() {
    this.close(false);
  }

  public close(value) {
    this.mdDialogRef.close(value);
  }

  public onClickButton1(){
    this.result = {
      type : "create",
      inputValue : this.data.inputText
    };
    this.close(this.result);
  }

  public onClickButton2() {
    this.result = {
      type : "createContinue",
      inputValue : this.data.inputText
    };
    this.close(this.result);
  }
  
  @HostListener('keydown.esc')
  public onEsc() {
    this.close(false);
  }
  
}
