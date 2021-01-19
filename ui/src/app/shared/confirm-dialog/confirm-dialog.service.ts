import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/internal/operators';
import { ConfirmDialogComponent } from './confirm-dialog.component';
import { DeleteDialogComponent } from './delete-dialog.component';

@Injectable()
export class ConfirmDialogService {
  constructor(private dialog: MatDialog) {}
  dialogRef: MatDialogRef<ConfirmDialogComponent>;
  dialogRefDel: MatDialogRef<DeleteDialogComponent>;

  public open(options) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      title: options.title,
      message: options.message,
      button1Text: options.button1Text,
      button2Text: options.button2Text,
      inputText: options.inputText,
    }
    this.dialogRef = this.dialog.open(ConfirmDialogComponent, dialogConfig);
  }

  public DeleteModelOpen(options: any, name?: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = {
      title: options.title,
      message: options.message,
      cancelText: options.cancelText,
      confirmText: options.confirmText,
      name: name,
      extraText: options.extraText
    }
    this.dialogRefDel = this.dialog.open(DeleteDialogComponent, dialogConfig);
  }

  public confirmed(): Observable<any> {
    return this.dialogRef.afterClosed().pipe(take(1), map(res => {
        return res;
      }
    ));

  }

  public confirmedDel(): Observable<any> {
    return this.dialogRefDel.afterClosed().pipe(take(1), map(res => {
        return res;
      }
    ));

  }
}